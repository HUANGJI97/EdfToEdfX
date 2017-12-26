using System;

namespace EEGReplay.model
{
    /// <summary>
    /// 标记类
    /// </summary>
    public class Mark
    {
        #region 属性

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTime { get; set; }

        #endregion 属性

        #region 构造函数

        public Mark(string name, DateTime dateTime)
        {
            Name = name;
            DateTime = dateTime;
        }

        #endregion 构造函数
    }
}