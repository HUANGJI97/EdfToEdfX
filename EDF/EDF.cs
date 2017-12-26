using EDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NeuroEDF
{
    /// <summary>
    /// EDF文件类
    /// </summary>
    public class EDF : IDisposable
    {
        #region 文件资源

        /// <summary>
        /// 文件流
        /// </summary>
        private FileStream FileStream;

        /// <summary>
        /// 文件读取流
        /// </summary>
        private StreamReader StreamReader;

        #endregion 文件资源

        #region 属性

        #region 公共属性

        /// <summary>
        /// MD5 Hash
        /// </summary>
        public string MD5 { get; private set; }

        /// <summary>
        /// EDF文件头
        /// </summary>
        public EDFHeader Header { get; private set; }

        /// <summary>
        /// EDF信道列表
        /// </summary>
        public List<EDFSignal> SignalList { get; private set; } = new List<EDFSignal>();

        /// <summary>
        /// 是否为EDFX格式
        /// </summary>
        public bool IsEDFX { get; private set; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// 每秒采样数量
        /// </summary>
        public int SamplesPerSecond { get; private set; }

        #endregion 公共属性

        #region 私有属性

        /// <summary>
        /// 解析后的点的缓存
        /// <para>1：秒数</para>
        /// <para>2：信道</para>
        /// <para>3：数据</para>
        /// </summary>
        private double[][][] DataCache;

        /// <summary>
        /// 每个点的长度
        /// </summary>
        private int BCO_Sample;

        /// <summary>
        /// 解析一秒数据的方法
        /// </summary>
        private Func<byte[], double[][]> ParseSecondData;

        #endregion 私有属性

        #endregion 属性

        #region 公共方法

        /// <summary>
        /// 读取文件
        /// </summary>
        public void ReadFile(string path)
        {
            // 文件不存在
            if (File.Exists(path) == false) throw new FileNotFoundException();

            // 文件格式错误
            if (EDFType.IsEDFFile(Path.GetExtension(path)) == false) throw new FileLoadException();

            // 读取文件
            FileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader = new StreamReader(FileStream, Encoding.Default, true);

            // 解析文件头
            ParseHeader();

            // 解析信道列表
            ParseSignalList();

            // 计算文件的MD5
            ClacMD5Hash();
        }

        /// <summary>
        /// 通过时间获取数据
        /// <para>1：秒数</para>
        /// <para>2：信道</para>
        /// <para>3：数据</para>
        /// </summary>
        public double[][][] GetListDataByTime(int index, int duration)
        {
            // 参数格式错误
            if (index < 0 || index >= Duration || duration < 1) return null;

            var sw = Stopwatch.StartNew();

            // 声明返回值
            var result = new List<double[][]>();

            // 计算结束下标
            var endIndex = index + duration;
            {
                endIndex = endIndex >= Duration ? Duration - 1 : endIndex;
            }

            // 临时存储流
            byte[] buffer;

            // 从缓存中获取数据或解析文件数据
            for (var i = index; i < endIndex; i++)
            {
                // 解析文件数据
                if (DataCache[i] == null)
                {
                    // 初始化当前秒的缓存
                    DataCache[i] = new double[SignalList.Count][];

                    // 跳转流至数据起始位置
                    StreamReader.BaseStream.Seek(
                        EDFHeader.ByteCount + EDFSignal.ByteCount * SignalList.Count + SamplesPerSecond * BCO_Sample * index,
                        SeekOrigin.Begin);

                    // 初始化临时流
                    buffer = new byte[SamplesPerSecond * BCO_Sample];

                    // 读取流
                    if (StreamReader.BaseStream.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        DataCache[i] = ParseSecondData(buffer);
                    }
                    else
                    {
                        throw new FileLoadException();
                    }
                }

                // 加入返回值
                result.Add(DataCache[i]);
            }

            Console.WriteLine("================");
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine("================");

            return result.ToArray();
        }

        #endregion 公共方法

        #region 私有方法

        /// <summary>
        /// 解析文件头
        /// </summary>
        private void ParseHeader()
        {
            // 解析文件头
            Header = EDFHeader.Parse(StreamReader);

            // 计算文件秒数
            Duration = (int)(Header.DataDuration * Header.DataInterval);

            // 初始化数据缓存
            DataCache = new double[Duration][][];
        }

        /// <summary>
        /// 解析信道列表
        /// </summary>
        private void ParseSignalList()
        {
            // 解析信道列表
            SignalList = EDFSignal.ParseList(StreamReader, Header.SignalCount);

            // 计算每秒采样数量
            SignalList.ForEach((s) =>
            {
                s.SamplesPerSecond = (int)(1 / Header.DataInterval * s.SamplesPerData);
            });

            // 计算每秒采样数量总和
            SamplesPerSecond = SignalList.Sum(s => s.SamplesPerSecond);

            #region TODO 判断文件类型

            if (SignalList[0].DigitalMaximum.Equals(8388607))
            {
                IsEDFX = true;

                BCO_Sample = 3;

                ParseSecondData = (buffer) =>
                {
                    // 声明返回值
                    var result = new double[SignalList.Count][];

                    for (var i = 0; i < result.Length; i++)
                    {
                        var a = result[i];
                    }

                    // 返回结果
                    return result;
                };
            }
            else
            {
                IsEDFX = false;

                BCO_Sample = 2;

                ParseSecondData = (buffer) =>
                {
                    // 声明返回值
                    var result = new double[SignalList.Count][];

                    // 返回结果
                    return result;
                };
            }

            #endregion TODO 判断文件类型
        }

        /// <summary>
        /// 计算MD5值
        /// </summary>
        private void ClacMD5Hash()
        {
            // 跳转流至数据起始位置
            StreamReader.BaseStream.Seek(
                EDFHeader.ByteCount + EDFSignal.ByteCount * SignalList.Count,
                SeekOrigin.Begin);

            // 初始化临时流
            var buffer = new byte[SamplesPerSecond * BCO_Sample];

            // 读取流
            if (StreamReader.BaseStream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var hash = new StringBuilder();
                var md5provider = new MD5CryptoServiceProvider();

                // 计算MD5值
                buffer = md5provider.ComputeHash(buffer);

                // 转换MD5形式
                for (int i = 0; i < buffer.Length; i++)
                {
                    hash.Append(buffer[i].ToString("X2"));
                }

                // 写入MD5值
                MD5 = hash.ToString();
            }
            else
            {
                throw new FileLoadException();
            }
        }

        #endregion 私有方法

        #region IDisposable Support

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    #region 释放托管资源

                    // 文件流
                    {
                        StreamReader.Dispose();
                        FileStream.Dispose();

                        StreamReader = null;
                        FileStream = null;
                    }

                    // 其他资源
                    {
                        Header = null;
                        SignalList = null;
                    }

                    #endregion 释放托管资源
                }

                // 标记成已释放
                disposedValue = true;
            }
        }

        ~EDF()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 手动释放
            Dispose(true);

            // 将对象从垃圾回收器链表中移除
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 关闭并释放资源
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion IDisposable Support
    }
}