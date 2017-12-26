using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace EEGReplay
{
    /// <summary>
    /// 展示地形图窗口
    /// </summary>
    public partial class FormDixingtu : Form
    {
        private FormMain myEEGForm;

        private ReplayController controller;

        /// <summary>
        /// 高和宽的比例 高/宽
        /// </summary>
        private double _hwRatio;

        /// <summary>
        /// pictureBox的最大高度
        /// </summary>
        private int _picBoxMaxHeight = Screen.PrimaryScreen.WorkingArea.Height - 100;

        /// <summary>
        /// 地形图图片文件路径
        /// </summary>
        private string _dixingtuImageFilePath;

        /// <summary>
        /// 绘制地形图的EDF文件的开始位置
        /// </summary>
        private int _beginIndexOfEDFFile;

        /// <summary>
        /// 绘制地形图的EDF文件的结束位置
        /// </summary>
        private int _endIndexOfEDFFile;

        private enum PrintType
        {
            EEG,    // 打印波形图
            Image   // 打印图片
        };

        private PrintType _printType = PrintType.Image; // 打印类型

        #region get/set方法

        public string DixingtuImageFilePath
        {
            set
            {
                _dixingtuImageFilePath = value;

                // 显示地形图图片
                this.pictureBox1.Image = Image.FromFile(_dixingtuImageFilePath);
                this.pictureBox1.Size = this.pictureBox1.Image.Size;
                _hwRatio = (double)this.pictureBox1.Size.Height / (double)this.pictureBox1.Size.Width;
                this.label1.Text = "地形图图片路径：" + _dixingtuImageFilePath;
            }
        }

        public int BeginIndexOfEDFFile
        {
            set
            {
                this._beginIndexOfEDFFile = value;
            }
        }

        public int EndIndexOfEDFFile
        {
            set
            {
                this._endIndexOfEDFFile = value;
            }
        }

        #endregion get/set方法

        #region 构造方法

        public FormDixingtu()
        {
            InitializeComponent();
        }

        public FormDixingtu(FormMain form)
        {
            InitializeComponent();

            myEEGForm = form;
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.pirtureBox_MouseWheel);

            this.printDXTPreviewDialog.Document = this.printDXTDocument;
        }

        #endregion 构造方法

        public void ReigisterNeuroControl(ReplayController controller)
        {
            this.controller = controller;
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(_dixingtuImageFilePath))
            {
                Process.Start(_dixingtuImageFilePath);
            }
            else
            {
                MessageBox.Show("地形图文件不存在，请检查\n" + _dixingtuImageFilePath);
            }
        }

        private void pirtureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // 控制范围，避免无限制放大或缩小
            if (this.pictureBox1.Height >= _picBoxMaxHeight && e.Delta > 0)
                return;
            if (this.pictureBox1.Height <= 200 && e.Delta < 0)
                return;

            this.pictureBox1.Width += e.Delta / 5;
            this.pictureBox1.Height += (int)(e.Delta / 5 * _hwRatio);
            this.movePicBoxToCenter();

            //Debug.WriteLine(this.pictureBox1.Location + ", " + this.pictureBox1.Size);
        }

        /// <summary>
        /// 把picturebox移动到中间
        /// </summary>
        private void movePicBoxToCenter()
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            int x = (screenWidth - this.pictureBox1.Size.Width) / 2;
            int y = (screenHeight - this.pictureBox1.Size.Height) / 2;

            Point loc = new Point(x, y);
            this.pictureBox1.Location = loc;
        }

        private void FormDixingtu_Shown(object sender, EventArgs e)
        {
            if (this.pictureBox1.Size.Height > _picBoxMaxHeight)
            {
                Size newSize = new Size();

                newSize.Height = _picBoxMaxHeight;
                newSize.Width = (int)(newSize.Height / _hwRatio);

                this.pictureBox1.Size = newSize;
            }

            this.movePicBoxToCenter();
        }

        #region 点击事件

        /// <summary>
        /// 按钮不能响应右击事件，用此函数传递
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDXTButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.printDXTButton_MouseClick(sender, e);
            }
        }

        /// <summary>
        /// 打印地形图按钮 鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDXTButton_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("printDXTButton_MouseClick()");

            // 左键打印两页，右键打印一页
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    this.printDXTDocument.BeginPrint -= new PrintEventHandler(this.printDXT_BeginPrint);
                    this.printDXTDocument.PrintPage -= new PrintPageEventHandler(this.printDXT_PrintPage);
                    this.printDXTDocument.EndPrint -= new PrintEventHandler(this.printDXT_EndPrint);

                    this.printDXTDocument.BeginPrint -= new PrintEventHandler(this.printDXT_BeginPrint_2);
                    this.printDXTDocument.PrintPage -= new PrintPageEventHandler(this.printDXT_PrintPage_2);
                    this.printDXTDocument.EndPrint -= new PrintEventHandler(this.printDXT_EndPrint_2);

                    this.printDXTDocument.BeginPrint += new PrintEventHandler(this.printDXT_BeginPrint);
                    this.printDXTDocument.PrintPage += new PrintPageEventHandler(this.printDXT_PrintPage);
                    this.printDXTDocument.EndPrint += new PrintEventHandler(this.printDXT_EndPrint);
                    break;

                case System.Windows.Forms.MouseButtons.Right:
                    this.printDXTDocument.BeginPrint -= new PrintEventHandler(this.printDXT_BeginPrint);
                    this.printDXTDocument.PrintPage -= new PrintPageEventHandler(this.printDXT_PrintPage);
                    this.printDXTDocument.EndPrint -= new PrintEventHandler(this.printDXT_EndPrint);

                    this.printDXTDocument.BeginPrint -= new PrintEventHandler(this.printDXT_BeginPrint_2);
                    this.printDXTDocument.PrintPage -= new PrintPageEventHandler(this.printDXT_PrintPage_2);
                    this.printDXTDocument.EndPrint -= new PrintEventHandler(this.printDXT_EndPrint_2);

                    this.printDXTDocument.BeginPrint += new PrintEventHandler(this.printDXT_BeginPrint_2);
                    this.printDXTDocument.PrintPage += new PrintPageEventHandler(this.printDXT_PrintPage_2);
                    this.printDXTDocument.EndPrint += new PrintEventHandler(this.printDXT_EndPrint_2);
                    break;

                default:
                    return;
            }

            this.printDXTDocument.DocumentName = @"地形图打印";

            PaperSize pageSize = null;
            foreach (PaperSize tmp in this.printDXTDocument.PrinterSettings.PaperSizes)
            {
                if (tmp.PaperName.Equals("A4"))
                {
                    pageSize = tmp;
                    break;
                }
            }
            if (pageSize != null) this.printDXTDocument.DefaultPageSettings.PaperSize = pageSize;

            // 设置纸张横向，true 纸张横向 & false 纸张纵向(默认)
            this.printDXTDocument.DefaultPageSettings.Landscape = true;

            // 设置纸张的页边距为0
            this.printDXTDocument.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

            this.printDXTPreviewDialog.ShowDialog();
        }

        #endregion 点击事件

        #region 打印事件 打印两页，第一页波形图，第二页地形图

        /// <summary>
        /// 打印开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDXT_BeginPrint(object sender, PrintEventArgs e)
        {
            this._printType = PrintType.EEG;
        }

        /// <summary>
        /// 打印中
        /// 第一页打印选中的波形图，第二页打印地形图图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDXT_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.Clear(Color.White); // Color.White Color.Blue

            switch (this._printType)
            {
                case PrintType.EEG:
                    this.printEEGWithEEGForm(e);
                    this._printType = PrintType.Image;
                    e.HasMorePages = true;
                    break;

                case PrintType.Image:
                    this.printDXTImage(e);
                    e.HasMorePages = false;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 打印结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDXT_EndPrint(object sender, PrintEventArgs e)
        {
            if (!this.printDXTDocument.PrintController.IsPreview)
            {
                this.printDXTPreviewDialog.Close();
            }
        }

        #endregion 打印事件 打印两页，第一页波形图，第二页地形图

        #region 实际打印部分 打印两页，第一页波形图，第二页地形图

        /// <summary>
        /// 打印地形图图片
        /// </summary>
        /// <param name="e"></param>
        private void printDXTImage(PrintPageEventArgs e)
        {
            Image image = this.pictureBox1.Image;
            float x = 20;
            float y = 20;

            e.Graphics.DrawString("地形图打印，打印时间：" + DateTime.Now.ToString(),
                                    new Font(new FontFamily("宋体"), 15, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    x, y);
            y += 40;

            float imageHigh = 600;
            float imageWith = imageHigh / image.Size.Height * image.Size.Width;

            e.Graphics.DrawImage(image, x, y, imageWith, imageHigh);
        }

        /// <summary>
        /// 打印地形图的时间范围对应的波形图
        /// </summary>
        /// <param name="e"></param>
        private void printEEGWithEEGForm(PrintPageEventArgs e)
        {
            // 每秒钟的点数
            int samplesPerSecond = (int)(controller.replayFile.Header.Signals[0].NumberOfSamplesPerDataRecord / controller.replayFile.Header.DurationOfDataRecordInSeconds);
            // 每页的时间 秒数
            double secondsPerPage = this.myEEGForm.getEEGPanelWidth() / this.controller.speed;
            // 每页的点数
            int samplesPerPage = (int)(samplesPerSecond * secondsPerPage);

            Boolean isPrint = true; // 当前信道是否打印
            Boolean isPrintTest = true; // TEST信道是否打印

            PointF beginP = new PointF(60, 20);
            PointF endP = new PointF(60, 20);
            float lineYvalue = beginP.Y; // 保留每次起始点坐标的Y值，作为每一路波形的参考原点Y坐标
            PointF testBeginP = new PointF(), testEndP = new PointF();

            // 每个点的间距有多少像素
            float interval = 1;
            double pixelPerCM = this.printDXTDocument.DefaultPageSettings.PaperSize.Height / 29.7; // 每CM的像素数(A4纸的实际长度是29.7cm)
            double pixelPerSecond = pixelPerCM * this.controller.speed;
            interval = (float)(pixelPerSecond / samplesPerSecond);

            // 打印时，每CM打印的像素数（单位：samplesPerSecond * interval是每秒打印的的像素数，this.getSpeed()是每秒多少CM）
            int pixelPerCMInPrint = (int)(samplesPerSecond * interval / this.controller.speed);

            // 需要打印的通道数，保证打印的每个通道间隔均匀分布
            int numOfPrintChannels = this.controller.replayFile.Header.Signals.Count;

            // 要打印的通道之间的间隔，即：Y轴的像素数
            int spaceOfPrintChannel = 700 / numOfPrintChannels;

            // 打印原始数据，遍历全部通道的数据
            for (int i = 0; i < numOfPrintChannels; i++)
            {
                // 画每个通道的名称
                e.Graphics.DrawString(this.controller.replayFile.Header.Signals[i].Label,
                                      new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                      System.Drawing.Brushes.Black,
                                      beginP.X - 50, beginP.Y);

                // 画出当前页的全部数据点
                for (int j = _beginIndexOfEDFFile; j < _endIndexOfEDFFile; j++)
                {
                    // 根据实际数据位置，换算为滤波数据的位置
                    int index = controller.second == 1 ? j : j - controller.numOfSamplesPerSecond * (controller.second - 1);

                    if (this.controller.replayFile.MyDataRecords.Count > 0)
                    {
                        if (index >= this.controller.replayFile.MyDataRecords.Count) break;
                    }
                    else if (this.controller.replayFile.DataRecords.Count > 0)
                    {
                        if (index >= this.controller.replayFile.DataRecords.Count) break;
                    }

                    // =========== 每秒钟画一条竖线，打印秒数 begin =============
                    // int currentSec = index % samplesPerSecond < interval ? index / samplesPerSecond : 0;
                    int currentSec = index % samplesPerSecond == 0 ? index / samplesPerSecond : 0;
                    if (currentSec >= 1)
                    {
                        Pen apen = new Pen(Color.Black, (float)0.3);
                        float[] floatDash = { 2, 4 };
                        apen.DashPattern = floatDash;
                        e.Graphics.DrawLine(apen, beginP.X, 0, beginP.X, 720);
                        TimeSpan ts = new TimeSpan(0, 0, currentSec);
                        e.Graphics.DrawString(ts.ToString(),
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    beginP.X - 30, 720);
                    }
                    // =========== 每秒钟画一条竖线，打印秒数 end =============

                    //double data = this.myEEGForm.Edffile.MyDataRecords[index][i]; // 取原始数据进行展示
                    double data = this.controller.filterData[i][index]; // 取滤波处理后的数据进行展示
                    /*
                    if (i == myEEGForm.NumberOfDemarcateChannel + 1)
                    {
                        data = data / Math.Abs(myEEGForm.ActualCV) * myEEGForm.DemarcateCV;
                    }
                    else
                    {
                        data = data * this.myEEGForm.GetGenerateDW(i); // 单位 uv
                    }
                    */
                    data = data * this.controller.generateDW(i);

                    // =========== 把uv转为像素个数 begin ==================
                    // 单位：myEEGForm.GetSensivity()是uv/cm，pixelPerCMInPrint是每厘米多少像素
                    data = data / (this.controller.sensitivity / (float)pixelPerCMInPrint);
                    // =========== 把uv转为像素个数 end ==================

                    endP.X += interval;
                    // 下拉框显示通道变少时，data会变小，所以按比例换算回去
                    // endP.Y = lineYvalue - (float)data / 5 * ((float)this.myEEGForm.GetNumberOfChannels() / this.myEEGForm.GetShownSignalNum());
                    endP.Y = lineYvalue - (float)data;

                    if (j == 0) { beginP.X = endP.X; beginP.Y = endP.Y; } // 避免最左侧第一个点的无效连线
                    Pen pen2 = new Pen(Color.Black, (float)0.5);
                    e.Graphics.DrawLine(pen2, beginP, endP); // 连线
                    beginP.X = endP.X;
                    beginP.Y = endP.Y;

                    if (endP.X >= this.printDXTDocument.DefaultPageSettings.PaperSize.Height - 20)
                    {
                        break;
                    }
                }

                Debug.WriteLine("endP = " + endP + ", lineYvalue = " + lineYvalue);

                beginP.X = 60; beginP.Y = isPrint ? lineYvalue + spaceOfPrintChannel : lineYvalue;
                endP.X = beginP.X; endP.Y = beginP.Y;
                lineYvalue = endP.Y;
            }

            this.printEEGInfoWithEEGForm(beginP, endP, e, pixelPerCMInPrint, interval, samplesPerSecond);
            e.HasMorePages = false;
        }

        private void printEEGInfoWithEEGForm(PointF beginP, PointF endP, PrintPageEventArgs e, int pixelPerCMInPrint, float interval, int samplesPerSecond)
        {
            #region 波形图和文字区域的分割线

            beginP.X = 20; beginP.Y = 730;
            endP.X = this.printDXTDocument.DefaultPageSettings.PaperSize.Height - 20; endP.Y = 730;
            Pen tmpPen = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(tmpPen, beginP, endP);

            #endregion 波形图和文字区域的分割线

            #region 打印其他文字描述信息

            // 1.打印时间 2.姓名 3.脑电图号 4.医院名称 5、灵敏度 6.走纸速度 7.刻度
            // 默认为横排文字
            Point point = new Point();
            point.X = 20;
            point.Y = 740;
            //point.Y = 10 + this.printImage.Size.Height + 30;
            e.Graphics.DrawString("打印时间：" + DateTime.Now.ToString(),
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);
            point.X += 220;
            e.Graphics.DrawString("姓名：" + this.controller.replayFile.Header.PatientIdentification.PatientName,
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);
            point.X += 150;
            string sexStr = this.controller.replayFile.Header.PatientIdentification.PatientSex.Equals("M") ? "男" : "女";
            e.Graphics.DrawString("性别：" + sexStr,
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);
            point.X += 80;
            e.Graphics.DrawString("检测时间：" + this.controller.replayFile.Header.StartDateTime,
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);
            point.X += 220;
            e.Graphics.DrawString("脑电图号：123456789",
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);
            point.X += 150;
            e.Graphics.DrawString("XXX医院",
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);

            // 打印灵敏度
            point.X = 20;
            point.Y += 12;
            e.Graphics.DrawString("灵敏度：" + this.controller.sensitivity + " uv/cm",
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);

            // 打印走纸速度
            point.X += 150;
            e.Graphics.DrawString("走纸速度：" + this.controller.speed + " mm/s",
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);

            // 打印时间常数
            point.X += 150;
            e.Graphics.DrawString("时间常数：" + this.controller.timeConstant,
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);

            #endregion 打印其他文字描述信息

            #region 打印刻度波形

            // 打印刻度波形 时长1s，振幅按灵敏度，1s时间内画5个方波
            point.X = 500;
            point.Y += pixelPerCMInPrint;// 2 * pixelPerCMInPrint;
            float space = (float)samplesPerSecond * interval / 10;
            PointF p1 = new PointF(0, 0);
            PointF p2 = new PointF(space, 0);
            PointF p3 = new PointF(space, 0 - pixelPerCMInPrint);
            PointF p4 = new PointF(2 * space, 0 - pixelPerCMInPrint);
            PointF p5 = new PointF(2 * space, 0);
            PointF[] pointArray = { p1, p2, p3, p4, p5 };
            for (int i = 0; i < pointArray.Length; i++)
            {
                PointF tmp = pointArray[i];
                tmp.X += point.X;
                tmp.Y += point.Y;
                pointArray[i] = tmp;
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < pointArray.Length; j++)
                {
                    PointF tmp = pointArray[j];
                    if (i != 0) tmp.X += space * 2;
                    pointArray[j] = tmp;
                }
                // 5个点为一组，绘制方波
                e.Graphics.DrawLines(Pens.Black, pointArray);
            }
            point.X -= 50;
            point.Y -= pixelPerCMInPrint / 2;
            e.Graphics.DrawString(this.controller.sensitivity + "uv 1s",
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);

            #endregion 打印刻度波形
        }

        #endregion 实际打印部分 打印两页，第一页波形图，第二页地形图

        #region 打印事件 打印一页

        private void printDXT_BeginPrint_2(object sender, PrintEventArgs e)
        {
            this._printType = PrintType.EEG;
        }

        private void printDXT_PrintPage_2(object sender, PrintPageEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            this.printEEGAndDXT(e);
        }

        private void printDXT_EndPrint_2(object sender, PrintEventArgs e)
        {
            if (!this.printDXTDocument.PrintController.IsPreview)
            {
                this.printDXTPreviewDialog.Close();
            }
        }

        #endregion 打印事件 打印一页

        #region 实际打印部分 打印一页

        private void printEEGAndDXT(PrintPageEventArgs e)
        {
            float x = 20;
            float y = 20;

            // 打印标题
            e.Graphics.DrawString("地形图打印，打印时间：" + DateTime.Now.ToString(),
                                    new Font(new FontFamily("宋体"), 15, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    x, y);
            y += 40;

            #region 打印波形

            // 每秒钟的点数
            int samplesPerSecond = (int)(this.controller.replayFile.Header.Signals[0].NumberOfSamplesPerDataRecord / this.controller.replayFile.Header.DurationOfDataRecordInSeconds);
            // 每页的时间 秒数
            double secondsPerPage = this.myEEGForm.getEEGPanelWidth() / this.controller.speed;
            // 每页的点数
            int samplesPerPage = (int)(samplesPerSecond * secondsPerPage);

            Boolean isPrint = true; // 当前信道是否打印
            Boolean isPrintTest = true; // TEST信道是否打印

            PointF beginP = new PointF(60, 60);
            PointF endP = new PointF(60, 60);
            float lineYvalue = beginP.Y; // 保留每次起始点坐标的Y值，作为每一路波形的参考原点Y坐标
            PointF testBeginP = new PointF(), testEndP = new PointF();

            // 每个点的间距有多少像素
            float interval = 1;
            double pixelPerCM = this.printDXTDocument.DefaultPageSettings.PaperSize.Height / 29.7 / 2; // 每CM的像素数(A4纸的实际长度是29.7cm)
            double pixelPerSecond = pixelPerCM * this.controller.speed;
            interval = (float)(pixelPerSecond / samplesPerSecond);

            // 打印时，每CM打印的像素数（单位：samplesPerSecond * interval是每秒打印的的像素数，this.getSpeed()是每秒多少CM）
            int pixelPerCMInPrint = (int)(samplesPerSecond * interval / this.controller.speed);

            // 需要打印的通道数，保证打印的每个通道间隔均匀分布
            int numOfPrintChannels = this.controller.replayFile.Header.Signals.Count;

            // 要打印的通道之间的间隔，即：Y轴的像素数
            int spaceOfPrintChannel = 700 / numOfPrintChannels;

            // 打印原始数据，遍历全部通道的数据
            for (int i = 0; i < numOfPrintChannels; i++)
            {
                // 画每个通道的名称
                e.Graphics.DrawString(this.controller.replayFile.Header.Signals[i].Label,
                                      new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                      System.Drawing.Brushes.Black,
                                      beginP.X - 50, beginP.Y);

                // 画出当前页的全部数据点
                for (int j = _beginIndexOfEDFFile; j < _endIndexOfEDFFile; j++)
                {
                    // 信道不显示，则不打印该通道
                    if (!isPrint) break;

                    // 根据实际数据位置，换算为滤波数据的位置
                    int index = controller.second == 1 ? j : j - controller.numOfSamplesPerSecond * (controller.second - 1);

                    if (this.controller.replayFile.MyDataRecords.Count > 0)
                    {
                        if (index >= this.controller.replayFile.MyDataRecords.Count) break;
                    }
                    else if (this.controller.replayFile.DataRecords.Count > 0)
                    {
                        if (index >= this.controller.replayFile.DataRecords.Count) break;
                    }

                    // =========== 每秒钟画一条竖线，打印秒数 begin =============
                    // int currentSec = index % samplesPerSecond < interval ? index / samplesPerSecond : 0;
                    int currentSec = index % samplesPerSecond == 0 ? index / samplesPerSecond : 0;
                    if (currentSec >= 1)
                    {
                        Pen apen = new Pen(Color.Black, (float)0.3);
                        float[] floatDash = { 2, 4 };
                        apen.DashPattern = floatDash;
                        e.Graphics.DrawLine(apen, beginP.X, 0, beginP.X, 720);
                        TimeSpan ts = new TimeSpan(0, 0, currentSec);
                        e.Graphics.DrawString(ts.ToString(),
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    beginP.X - 30, 720);
                    }
                    // =========== 每秒钟画一条竖线，打印秒数 end =============

                    //double data = this.myEEGForm.Edffile.MyDataRecords[index][i]; // 取原始数据进行展示
                    double data = this.controller.filterData[i][index]; // 取滤波处理后的数据进行展示
                    /*
                    if (i == myEEGForm.NumberOfDemarcateChannel + 1)
                    {
                        data = data / Math.Abs(myEEGForm.ActualCV) * myEEGForm.DemarcateCV;
                    }
                    else
                    {
                        data = data * this.myEEGForm.GetGenerateDW(i); // 单位 uv
                    }
                    */
                    data = data * this.controller.generateDW(i);

                    // =========== 把uv转为像素个数 begin ==================
                    // 单位：myEEGForm.GetSensivity()是uv/cm，pixelPerCMInPrint是每厘米多少像素
                    data = data / (this.controller.sensitivity / (float)pixelPerCMInPrint);
                    // =========== 把uv转为像素个数 end ==================

                    endP.X += interval;
                    // 下拉框显示通道变少时，data会变小，所以按比例换算回去
                    // endP.Y = lineYvalue - (float)data / 5 * ((float)this.myEEGForm.GetNumberOfChannels() / this.myEEGForm.GetShownSignalNum());
                    endP.Y = lineYvalue - (float)data;

                    if (j == 0) { beginP.X = endP.X; beginP.Y = endP.Y; } // 避免最左侧第一个点的无效连线
                    Pen pen2 = new Pen(Color.Black, (float)0.5);
                    e.Graphics.DrawLine(pen2, beginP, endP); // 连线
                    beginP.X = endP.X;
                    beginP.Y = endP.Y;

                    if (endP.X >= this.printDXTDocument.DefaultPageSettings.PaperSize.Height - 20)
                    {
                        break;
                    }
                }

                Debug.WriteLine("endP = " + endP + ", lineYvalue = " + lineYvalue);

                beginP.X = 60; beginP.Y = isPrint ? lineYvalue + spaceOfPrintChannel : lineYvalue;
                endP.X = beginP.X; endP.Y = beginP.Y;
                lineYvalue = endP.Y;
            }

            #endregion 打印波形

            #region 打印地形图

            Image image = this.pictureBox1.Image;

            float imageHigh = 600;
            float imageWith = imageHigh / image.Size.Height * image.Size.Width;

            imageWith = this.printDXTDocument.DefaultPageSettings.PaperSize.Height / 2;
            imageHigh = imageWith * ((float)image.Size.Height / (float)image.Size.Width);

            e.Graphics.DrawImage(image, imageWith, y, imageWith, imageHigh);

            #endregion 打印地形图

            #region 打印其他描述信息

            this.printEEGInfoWithEEGForm(beginP, endP, e, pixelPerCMInPrint, interval, samplesPerSecond);

            #endregion 打印其他描述信息
        }

        #endregion 实际打印部分 打印一页
    }
}