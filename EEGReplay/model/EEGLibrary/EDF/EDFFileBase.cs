using System;

namespace EEGLibrary.EDF
{
    /// <summary>
    /// EDF文件基础类，用于定义通用的属性和方法
    /// </summary>
    public abstract class EDFFileBase : IDisposable
    {
        #region 保护字段

        /// <summary>
        /// 小数点分隔符
        /// </summary>
        protected char DecimalSeparator { get; set; }

        #endregion 保护字段

        public EDFFileBase()
        {
            this.DecimalSeparator = EDFConstants.DefaultDecimalSeparator;
        }

        #region 析构函数、释放函数

        /// <summary>
        /// 析构函数，GC时自动释放非托管资源
        /// </summary>
        ~EDFFileBase()
        {
            // 仅释放非托管资源
            Dispose(false);
        }

        /// <summary>
        /// 用于手动调用的释放函数
        /// </summary>
        public void Dispose()
        {
            // 释放托管和非托管资源
            Dispose(true);

            // 从GC的链表里移除此对象
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放函数，无法直接调用
        /// </summary>
        /// <param name="disposing">用于区分是手动调用还是GC自动调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 释放托管资源
                // TODO
            }

            // 释放非托管资源
            // TODO
        }

        #endregion 构造函数、析构函数
    }
}