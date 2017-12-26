using EDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EEGReplay
{
    /// <summary>
    /// 功率谱绘制窗口 add by lzy 20161215
    /// </summary>
    public partial class FormGonglvpu : Form
    {
        private FormMain myEEGForm;

        private ReplayController controller;

        private List<double[,]> _gonglvpuData; // 全部通道的功率谱数据

        private int _numberOfChannels; // 全部通道数量

        private EDFFile _edffile; // edf文件

        private List<Chart> _chartList; // 绘图控件列表

        private int _numberOfChannelDisplay; // 展示/绘制的通道数量

        #region 初始化

        public FormGonglvpu(FormMain form)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.myEEGForm = form;
            this._chartList = new List<Chart>();

            this.printGLPDocument.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printGLP_BeginPrint);
            this.printGLPDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printGLP_PrintPage);
            this.printGLPDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(this.printGLP_EndPrint);

            this.printGLPPreviewDialog.Document = this.printGLPDocument;
        }

        public void ReigisterNeuroControl(ReplayController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// 初始化全部的chart
        /// </summary>
        public void initChart()
        {
            Point point = new Point(50, 50); // 起始点
            Size size = new Size(230, 150); // 每个chart的Size

            for (int i = 0; i < _numberOfChannels; i++)
            {
                Chart newchart = new Chart();
                newchart.Location = point;
                newchart.Size = size;
                newchart.Name = "gonglvpuChart_" + i;
                // newchart.Titles.Add("hahaha"); // 设置每个chart的标题

                initChart(newchart, i);
                this.Controls.Add(newchart);
                this._chartList.Add(newchart);

                // 修正起始点
                point.X += 250;
                if (point.X > Screen.PrimaryScreen.Bounds.Width - size.Width)
                {
                    point.X = 50;
                    point.Y += 180;
                }
            }
        }

        private void initChart(Chart chart, int index)
        {
            //为了删除第一路心电数据，index=0时，将index纠正为numofChannels
            int indexCorrection = 0;
            //20170412,wdp,心电不进行功率谱变换
            //if(index==_numberOfChannels-1)
            //{
            //    indexCorrection = 0;
            //}
            //else
            //{
            indexCorrection = index + 1;
            //}
            chart.ChartAreas.Clear();
            ChartArea chartAreaGLP = new ChartArea("GLP");
            chartAreaGLP.Visible = true;

            chartAreaGLP.AxisX.Title = "通道:" + _edffile.Header.Signals[indexCorrection].Label.Trim() + " 频率:Hz"; // X轴不显示则设为空
            chartAreaGLP.AxisX.Enabled = AxisEnabled.True;
            chartAreaGLP.AxisX.MajorGrid.Enabled = true; // 网格线
            chartAreaGLP.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartAreaGLP.AxisX.Maximum = 100;// windowSize; //_speed; 41.7090099009901  41.409
            chartAreaGLP.AxisX.Minimum = 0;
            chartAreaGLP.AxisX.Interval = 20;

            chartAreaGLP.AxisY.Title = "功率:dB";
            chartAreaGLP.AxisY.Enabled = AxisEnabled.True;
            chartAreaGLP.AxisY.MajorGrid.Enabled = true; // 网格线
            chartAreaGLP.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            //chartAreaGLP.AxisY.Maximum = 100;
            chartAreaGLP.AxisY.Minimum = 0;

            chart.ChartAreas.Add(chartAreaGLP);

            Series series = new Series();
            series.ChartArea = "GLP";
            series.ChartType = SeriesChartType.FastLine; // do not display data point markers,data point labels, or line shadows
            series.Enabled = true;
            series.Color = Color.Red;

            chart.Series.Add(series);
        }

        #endregion 初始化

        #region 访问器

        /// <summary>
        /// 全部通道的功率谱数据
        /// </summary>
        public List<double[,]> GonglvpuData
        {
            set { this._gonglvpuData = value; }
        }

        /// <summary>
        /// 全部通道数量，并设置需要展示/绘制的通道数量
        /// </summary>
        public int NumberOfChannels
        {
            set
            {
                this._numberOfChannels = value;
                // 设置需要展示/绘制的通道数量
                _numberOfChannelDisplay = 0;
                for (int i = 0; i < _numberOfChannels; i++)
                {
                    if (this.GetSignalDisplay(i)) _numberOfChannelDisplay++;
                }
            }
        }

        /// <summary>
        /// edf文件
        /// </summary>
        public EDFFile EDFFile
        {
            set { this._edffile = value; }
        }

        #endregion 访问器

        #region 私有方法

        /// <summary>
        /// 获取当前信道是否展示
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool GetSignalDisplay(int i)
        {
            return true;
        }

        #endregion 私有方法

        /// <summary>
        /// 绘制功率谱
        /// </summary>
        public void drawGonglvpu()
        {
            // 统一Y轴最大值
            double maxOfAxisY = 0;
            for (int i = 0; i < _gonglvpuData.Count; i++)
            {
                double[,] data = _gonglvpuData[i];
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    double y = data[1, j];
                    double x = data[0, j];
                    if (x > 30) break;
                    if (maxOfAxisY < y) maxOfAxisY = y;
                }
            }
            //maxOfAxisY = Math.Pow(10, maxOfAxisY / 10);
            maxOfAxisY = Convert.ToDouble(maxOfAxisY.ToString("0.0"));
            System.Diagnostics.Debug.WriteLine("功率谱Y轴最大值：" + maxOfAxisY);
            for (int i = 0; i < _numberOfChannels; i++)
            {
                Chart chart = _chartList[i];
                chart.ChartAreas[0].AxisY.Maximum = maxOfAxisY;
            }
            // 遍历，绘制功率谱
            for (int i = 0; i < _numberOfChannels; i++)
            {
                // 被设置不显示的通道不绘制数据
                if (this.GetSignalDisplay(i) == false)
                    continue;

                Chart chart = _chartList[i];
                int m = 0;
                if (i == _numberOfChannels - 1)
                    m = 0;
                else
                    m = m + 1;
                double[,] data = _gonglvpuData[i];
                int pointCount = data.GetLength(1); // 某一维度的长度，即：有多少坐标点

                int tmp = pointCount / 10000 + 1;

                double maxY = 0; // 记录最大功率
                double maxX = 0; // 记录最大功率对应的频率

                for (int j = 0; j < pointCount; j++)
                {
                    double x = data[0, j];
                    double y = data[1, j];

                    // 计算转换 modify by lzy 20170204
                    //y = Math.Pow(10, y*(data[0,2]-data[0,1]) / 10);
                    //  y = Math.Pow(10, y / 10);
                    // 0点对应Y值固定为0
                    if (0 == x) y = 0;
                    // 只画0-30Hz的范围
                    if (x > 30) break;

                    chart.Series[0].Points.AddXY(x, y);

                    if (y > maxY) { maxY = y; maxX = x; }
                }

                // 设置标题内容：最大功率：0.00dB / 0.00Hz
                chart.Titles.Add("最大值:" + Convert.ToDouble(maxY).ToString("0.00") + "dbμv/Hz | " + Convert.ToDouble(maxX).ToString("0.00") + "Hz");
            }

            // 释放内存
            _gonglvpuData.Clear();
        }

        private void FormGonglvpu_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < _chartList.Count; i++)
            {
                Chart chart = _chartList[i];
                //chart.Series[0].Points.Clear();
                chart.Series.Clear();
                chart.ChartAreas.Clear();
                chart = null;
            }
        }

        /// <summary>
        /// 打印功率谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printGLPButton_Click(object sender, EventArgs e)
        {
            this.printGLPDocument.DocumentName = @"功率谱打印";

            PaperSize pageSize = null;
            foreach (PaperSize tmp in this.printGLPDocument.PrinterSettings.PaperSizes)
            {
                if (tmp.PaperName.Equals("A4"))
                {
                    pageSize = tmp;
                    break;
                }
            }
            // A4 pageSize.Width = 827, pageSize.Height = 1169
            Debug.WriteLine("A4 pageSize.Width = " + pageSize.Width + ", pageSize.Height = " + pageSize.Height);
            if (pageSize != null) this.printGLPDocument.DefaultPageSettings.PaperSize = pageSize;

            // 设置纸张横向，true 纸张横向 & false 纸张纵向(默认)
            this.printGLPDocument.DefaultPageSettings.Landscape = true;

            // 设置纸张的页边距为0
            this.printGLPDocument.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

            this.printGLPPreviewDialog.ShowDialog();
        }

        #region 打印事件

        /// <summary>
        /// 打印开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printGLP_BeginPrint(object sender, PrintEventArgs e)
        {
            Debug.WriteLine("printGLP_BeginPrint()");
        }

        /// <summary>
        /// 打印中
        /// A4 pageSize.Width = 827, pageSize.Height = 1169
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printGLP_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (this._chartList.Count < 1) return;
            Debug.WriteLine("printGLP_PrintPage()");

            float x = 10, y = 10, width = 0, height = 0;
            float offset = 10;

            PaperSize paperSize = this.printGLPDocument.DefaultPageSettings.PaperSize;
            Size chartSize = this._chartList[0].Size;
            width = (paperSize.Height - ((5 + 1) * offset)) / 5;
            height = width / chartSize.Width * chartSize.Height;

            for (int i = 0; i < this._chartList.Count; i++)
            {
                Chart chart = this._chartList[i];
                Rectangle rectangle = new Rectangle((int)x, (int)y, (int)width, (int)height);
                chart.Printing.PrintPaint(e.Graphics, rectangle);

                if (i != 0 && i % 5 == 0)
                {
                    x = offset;
                    y += (offset + height);
                }
                else
                {
                    x += (offset + width);
                }
            }
        }

        /// <summary>
        /// 打印结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printGLP_EndPrint(object sender, PrintEventArgs e)
        {
            Debug.WriteLine("printGLP_EndPrint()");
        }

        #endregion 打印事件
    }
}