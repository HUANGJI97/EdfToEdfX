namespace EEGReplay
{
    /// <summary>
    /// 处理功率谱数据的线程所需要传入的参数
    /// </summary>
    internal class GLPThreadRunParams
    {
        public int beginIndex;

        public int endIndex;

        /// <summary>
        /// 构造方法
        /// </summary>
        public GLPThreadRunParams(int arg0, int arg1)
        {
            this.beginIndex = arg0;
            this.endIndex = arg1;
        }
    }
}