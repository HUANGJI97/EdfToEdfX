using System.Linq;

namespace EDF
{
    /// <summary>
    /// EDF类型
    /// </summary>
    internal class EDFType
    {
        /// <summary>
        /// 标准类型
        /// </summary>
        public static readonly  string EDF = ".edf";

        /// <summary>
        /// 妞诺专用格式
        /// </summary>
        public static readonly  string EDFX = ".edfx";

        /// <summary>
        /// 后缀名列表
        /// </summary>
        private static readonly string[] ExtensionList = new string[]
        {
            EDF, EDFX
        };

        /// <summary>
        /// 判断是否为EDF文件
        /// </summary>
        public static bool IsEDFFile(string extension)
        {
            // 文件后缀名为空
            if (string.IsNullOrEmpty(extension)) return false;

            // 返回文件后缀名是否在列表中
            return ExtensionList.Contains(extension.ToLowerInvariant());
        }
    }
}