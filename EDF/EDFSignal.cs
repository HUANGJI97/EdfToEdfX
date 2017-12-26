using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NeuroEDF
{
    public class EDFSignal
    {
        #region 属性

        #region 总长度

        /// <summary>
        /// 总长度
        /// </summary>
        public static readonly  int ByteCount = 256;

        #endregion 总长度

        #region 信道标签

        /// <summary>
        /// 信道标签长度
        /// </summary>
        private static readonly int BCO_Label = 16;

        /// <summary>
        /// 信道标签
        /// </summary>
        public string Label { get; set; } = string.Empty;

        #endregion 信道标签

        #region 传感器类型

        /// <summary>
        /// 传感器类型长度
        /// </summary>
        private static readonly int BCO_TransducerType = 80;

        /// <summary>
        /// 传感器类型
        /// </summary>
        public string TransducerType { get; set; } = string.Empty;

        #endregion 传感器类型

        #region PhysicalDimension

        /// <summary>
        /// PhysicalDimension长度
        /// </summary>
        private static readonly int BCO_PhysicalDimension = 8;

        /// <summary>
        /// PhysicalDimension
        /// </summary>
        public string PhysicalDimension { get; set; } = string.Empty;

        #endregion PhysicalDimension

        #region PhysicalMinimum

        /// <summary>
        /// PhysicalMinimum长度
        /// </summary>
        private static readonly int BCO_PhysicalMinimum = 8;

        /// <summary>
        /// PhysicalMinimum
        /// </summary>
        public float PhysicalMinimum { get; set; } = 0F;

        #endregion PhysicalMinimum

        #region PhysicalMaximum

        /// <summary>
        /// PhysicalMaximum长度
        /// </summary>
        private static readonly int BCO_PhysicalMaximum = 8;

        /// <summary>
        /// PhysicalMinimum
        /// </summary>
        public float PhysicalMaximum { get; set; } = 0F;

        #endregion PhysicalMaximum

        #region DigitalMinimum

        /// <summary>
        /// DigitalMinimum长度
        /// </summary>
        private static readonly int BCO_DigitalMinimum = 8;

        /// <summary>
        /// DigitalMinimum
        /// </summary>
        public float DigitalMinimum { get; set; } = 0F;

        #endregion DigitalMinimum

        #region DigitalMaximum

        /// <summary>
        /// DigitalMaximum长度
        /// </summary>
        private static readonly int BCO_DigitalMaximum = 8;

        /// <summary>
        /// DigitalMaximum
        /// </summary>
        public float DigitalMaximum { get; set; } = 0F;

        #endregion DigitalMaximum

        #region Prefiltering

        /// <summary>
        /// Prefiltering长度
        /// </summary>
        private static readonly int BCO_Prefiltering = 80;

        /// <summary>
        /// Prefiltering
        /// </summary>
        public string Prefiltering { get; set; } = string.Empty;

        #endregion Prefiltering

        #region 每段数据采样点数

        /// <summary>
        /// 每段数据采样点数长度
        /// </summary>
        private static readonly int BCO_SamplesPerData = 8;

        /// <summary>
        /// 每段数据采样点数
        /// </summary>
        public int SamplesPerData { get; set; } = 0;

        #endregion 每段数据采样点数

        #region 每秒数据采样点数

        /// <summary>
        /// 每秒数据采样点数
        /// </summary>
        public int SamplesPerSecond { get; set; } = 0;

        #endregion 每秒数据采样点数

        #endregion 属性

        #region 私有方法

        /// <summary>
        /// 读取属性
        /// </summary>
        private static T ReadProperty<T>(StreamReader sr, int propertyLength)
        {
            // 流内容
            var buffer = new byte[propertyLength];

            // 流的字符串形式
            var parsedString = string.Empty;

            // 读取流
            if (sr.BaseStream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                // 转换成字符串
                parsedString = sr.CurrentEncoding.GetString(buffer).Trim();

                // 释放内存
                buffer = null;

                // 转换成属性类型
                return (T)Convert.ChangeType(parsedString, typeof(T), CultureInfo.InvariantCulture);
            }

            // 文件长度异常
            throw new FileLoadException();
        }

        #endregion 私有方法

        #region 公共方法

        /// <summary>
        /// 解析信道列表
        /// </summary>
        public static List<EDFSignal> ParseList(StreamReader sr, int count)
        {
            // 声明返回值
            var result = new List<EDFSignal>();

            // 生成空信道
            for (int i = 0; i < count; i++)
            {
                result.Add(new EDFSignal());
            }

            // 信道名
            for (int i = 0; i < count; i++)
            {
                result[i].Label = ReadProperty<string>(sr, BCO_Label);
            }

            // 传感器类型
            for (int i = 0; i < count; i++)
            {
                result[i].TransducerType = ReadProperty<string>(sr, BCO_TransducerType);
            }

            // PhysicalDimension
            for (int i = 0; i < count; i++)
            {
                result[i].PhysicalDimension = ReadProperty<string>(sr, BCO_PhysicalDimension);
            }

            // PhysicalMinimum
            for (int i = 0; i < count; i++)
            {
                result[i].PhysicalMinimum = ReadProperty<float>(sr, BCO_PhysicalMinimum);
            }

            // PhysicalMaximum
            for (int i = 0; i < count; i++)
            {
                result[i].PhysicalMaximum = ReadProperty<float>(sr, BCO_PhysicalMaximum);
            }

            // DigitalMinimum
            for (int i = 0; i < count; i++)
            {
                result[i].DigitalMinimum = ReadProperty<float>(sr, BCO_DigitalMinimum);
            }

            // DigitalMaximum
            for (int i = 0; i < count; i++)
            {
                result[i].DigitalMaximum = ReadProperty<float>(sr, BCO_DigitalMaximum);
            }

            // Prefiltering
            for (int i = 0; i < count; i++)
            {
                result[i].Prefiltering = ReadProperty<string>(sr, BCO_Prefiltering);
            }

            // 每段数据采样点数
            for (int i = 0; i < count; i++)
            {
                result[i].SamplesPerData = ReadProperty<int>(sr, BCO_SamplesPerData);
            }

            // 返回结果
            return result;
        }

        #endregion 公共方法
    }
}