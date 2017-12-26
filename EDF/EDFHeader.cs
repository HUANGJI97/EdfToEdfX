using System;
using System.Globalization;
using System.IO;

namespace NeuroEDF
{
    /// <summary>
    /// EDF文件头
    /// </summary>
    public class EDFHeader
    {
        #region 属性

        #region 总长度

        /// <summary>
        /// 总长度
        /// </summary>
        public static readonly  int ByteCount = 256;

        #endregion 总长度

        #region 版本号

        /// <summary>
        /// 版本号长度
        /// </summary>
        private static readonly int BCO_Version = 8;

        /// <summary>
        /// 版本号
        /// </summary>
        private string Version { get; set; } = string.Empty;

        #endregion 版本号

        #region 病人ID

        /// <summary>
        /// 病人ID长度
        /// </summary>
        private static readonly int BCO_PatientID = 80;

        /// <summary>
        /// 病人ID
        /// </summary>
        public string PatientID { get; set; } = string.Empty;

        #endregion 病人ID

        #region 记录ID

        /// <summary>
        /// 记录ID长度
        /// </summary>
        private static readonly int BCO_RecordID = 80;

        /// <summary>
        /// 记录ID
        /// </summary>
        public string RecordID { get; set; } = string.Empty;

        #endregion 记录ID

        #region 开始日期

        /// <summary>
        /// 开始日期格式
        /// </summary>
        private static readonly string StartDatePartten = "dd.MM.yy";

        /// <summary>
        /// 开始日期长度
        /// </summary>
        private static readonly int BCO_StartDate = 8;

        /// <summary>
        /// 开始日期
        /// <para>dd.MM.yy</para>
        /// </summary>
        public string StartDate { get; set; } = string.Empty;

        #endregion 开始日期

        #region 开始时间

        /// <summary>
        /// 开始时间格式
        /// </summary>
        private static readonly string StartTimePartten = "HH.mm.ss";

        /// <summary>
        /// 开始时间长度
        /// </summary>
        private static readonly int BCO_StartTime = 8;

        /// <summary>
        /// 开始时间
        /// <para>HH.mm.ss</para>
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        #endregion 开始时间

        #region 头文件字节数

        /// <summary>
        /// 头文件字节数长度
        /// </summary>
        private static readonly int BCO_BCOHeader = 8;

        /// <summary>
        /// 头文件字节数
        /// </summary>
        public int ByteCountOfHeader { get; set; } = 0;

        #endregion 头文件字节数

        #region 保留字段

        /// <summary>
        /// 保留字段长度
        /// </summary>
        private static readonly int BCO_Reserved = 44;

        /// <summary>
        /// 保留字段
        /// </summary>
        public string Reserved { get; set; } = string.Empty;

        #endregion 保留字段

        #region 采样长度

        /// <summary>
        /// 采样长度长度
        /// </summary>
        private static readonly int BCO_DataDuration = 8;

        /// <summary>
        /// 采样长度
        /// </summary>
        public int DataDuration { get; set; } = 0;

        #endregion 采样长度

        #region 采样间隔

        /// <summary>
        /// 采样间隔长度
        /// </summary>
        private static readonly int BCO_DataInterval = 8;

        /// <summary>
        /// 采样间隔
        /// </summary>
        public double DataInterval { get; set; } = 0D;

        #endregion 采样间隔

        #region 信道数量

        /// <summary>
        /// 信道数量长度
        /// </summary>
        private static readonly int BCO_SignalCount = 4;

        /// <summary>
        /// 信道数量
        /// </summary>
        public int SignalCount { get; set; } = 0;

        #endregion 信道数量

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
        /// 解析头文件
        /// </summary>
        public static EDFHeader Parse(StreamReader sr)
        {
            return new EDFHeader()
            {
                // 版本号
                Version = ReadProperty<string>(sr, BCO_Version),

                // 病人ID
                PatientID = ReadProperty<string>(sr, BCO_PatientID),

                // 记录ID
                RecordID = ReadProperty<string>(sr, BCO_RecordID),

                // 开始日期
                StartDate = ReadProperty<string>(sr, BCO_StartDate),

                // 开始时间
                StartTime = ReadProperty<string>(sr, BCO_StartTime),

                // 头文件长度
                ByteCountOfHeader = ReadProperty<int>(sr, BCO_BCOHeader),

                // 保留字段
                Reserved = ReadProperty<string>(sr, BCO_Reserved),

                // 采样长度
                DataDuration = ReadProperty<int>(sr, BCO_DataDuration),

                // 采样间隔
                DataInterval = ReadProperty<double>(sr, BCO_DataInterval),

                // 信道数量
                SignalCount = ReadProperty<int>(sr, BCO_SignalCount),
            };
        }

        #endregion 公共方法
    }
}