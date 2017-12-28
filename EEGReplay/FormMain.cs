using DiXingTu;
using EEGReplay.model;
using EEGReplay.view;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EDF;
using EEGReplay.model.edf;

namespace EEGReplay
{
    public partial class FormMain : Form
    {
        private ReplayController controller;

        /// <summary>
        /// 导联标签列表
        /// </summary>
        private List<Label> leadLabelList = new List<Label>();

        /// <summary>
        /// 导联标签面板
        /// </summary>
        private Panel labelPanel;

        /// <summary>
        /// 波形图面板
        /// </summary>
        private Panel eegPanel;

        /// <summary>
        /// 模式列表
        /// </summary>
        private ToolStripMenuItem tsrLeadTypeList;

        /// <summary>
        /// 自定义滚动条
        /// </summary>
        private NeuroScrollBar scrollBar = new NeuroScrollBar();

        /// <summary>
        /// 标记文本框
        /// </summary>
        private ListBox markListBox;

        public FormMain()
        {
            InitializeComponent();

            this.init();

            // 添加自定义滚动条
            this.scrollBar.MouseClick -= new MouseEventHandler(this.scrollBar_MouseClick);
            this.scrollBar.MouseClick += new MouseEventHandler(this.scrollBar_MouseClick);
            MyToolStrip myTooStrip = new MyToolStrip(this.scrollBar);
            myTooStrip.AutoSize = false; // 避免尺寸自动压缩
            this.toolStrip2.Items.Add(myTooStrip);
        }

        private void init()
        {
            this.controller = new ReplayController(this);

            this.labelPanel = this.eegSpcMain.Panel1;
            this.eegPanel = this.eegSpcMain.Panel2;

            // 加载菜单项
            LoadMenuStrip();

            // 载入模式列表
            LoadLeadConfig();

            // 灵敏度
            this.toolStripComboBox_sensitivity.Items.Clear();
            foreach (int v in this.controller.sensitivityList)
            {
                this.toolStripComboBox_sensitivity.Items.Add(v.ToString());
            }
            this.toolStripComboBox_sensitivity.Text = this.controller.sensitivity.ToString();

            // 滤波频率-低频
            this.toolStripComboBox_LowFreq.Items.Clear();
            foreach (var v in this.controller.lowFreqList)
            {
                this.toolStripComboBox_LowFreq.Items.Add(v.ToString());
            }
            this.toolStripComboBox_LowFreq.Text = this.controller.lowFreq.ToString();

            // 滤波频率-高频
            this.toolStripComboBox_HighFreq.Items.Clear();
            foreach (var v in this.controller.highFreqList)
            {
                this.toolStripComboBox_HighFreq.Items.Add(v.ToString());
            }
            this.toolStripComboBox_HighFreq.Text = this.controller.highFreq.ToString();

            // 走纸速度
            this.toolStripComboBox_speed.Items.Clear();
            foreach (var v in this.controller.speedList)
            {
                this.toolStripComboBox_speed.Items.Add(v.ToString());
            }
            this.toolStripComboBox_speed.Text = this.controller.speed.ToString();

            // 倍速
            this.toolStripComboBox_rate.Items.Clear();
            foreach (int v in this.controller.rateList)
            {
                this.toolStripComboBox_rate.Items.Add(v.ToString());
            }
            this.toolStripComboBox_rate.Text = this.controller.rate.ToString();
        }

        #region 加载菜单项

        /// <summary>
        /// 加载菜单项
        /// </summary>
        private void LoadMenuStrip()
        {
            #region 导联

            ToolStripMenuItem tsrLead = new ToolStripMenuItem("导联");

            // 图示
            ToolStripMenuItem tsrLeadDiagram = new ToolStripMenuItem("图示");
            tsrLeadDiagram.Click += delegate (object sender, EventArgs e)
            {
                FormLeadDiagram formLeadDiagram = new FormLeadDiagram();
                formLeadDiagram.LoadLeadSource(this.controller.leadSource);
                formLeadDiagram.ShowDialog();
            };

            // 模式
            tsrLeadTypeList = new ToolStripMenuItem("模式");

            // 配置
            ToolStripMenuItem tsrLeadConfig = new ToolStripMenuItem("配置");
            tsrLeadConfig.Click += delegate (object sender, EventArgs e)
            {
                FormLeadConfig formLeadConfig = new FormLeadConfig(this.controller);
                formLeadConfig.FormClosing += delegate (object _sender, FormClosingEventArgs _e)
                {
                    LoadLeadConfig();
                };
                formLeadConfig.ShowDialog();
            };

            // 加入菜单列表
            tsrLead.DropDownItems.Add(tsrLeadDiagram);
            tsrLead.DropDownItems.Add(tsrLeadTypeList);
            tsrLead.DropDownItems.Add(tsrLeadConfig);

            #endregion 导联

            // 20170615 暂时屏蔽屏幕校准功能，使用新逻辑
            this.toolStripMenuItem_calibrate.Enabled = false;

            // TODO 其他菜单项也应该动态载入
            this.mnsMain.Items.Insert(this.mnsMain.Items.Count - 1, tsrLead);
        }

        #endregion 加载菜单项

        /// <summary>
        /// 载入导联模式列表
        /// </summary>
        private void LoadLeadConfig()
        {
            // 清空模式列表
            tsrLeadTypeList.DropDownItems.Clear();

            // 工具栏导联配置刷新
            this.toolStripComboBox_leadConfig.Items.Clear();

            // 读取模式列表
            ToolStripMenuItem tsrLeadType;
            foreach (String strLeadType in this.controller.leadConfigDic.Keys)
            {
                tsrLeadType = new ToolStripMenuItem(strLeadType);
                tsrLeadType.CheckOnClick = true;
                tsrLeadType.Click += delegate (object sender, EventArgs e)
                {
                    ToolStripMenuItem tsrSender = (ToolStripMenuItem)sender;

                    // 单选机制
                    if (tsrSender.Checked)
                    {
                        for (var i = 0; i < tsrLeadTypeList.DropDownItems.Count; i++)
                        {
                            ((ToolStripMenuItem)tsrLeadTypeList.DropDownItems[i]).Checked = false;
                        }

                        this.controller.currentLeadConfigName = tsrSender.Text;
                        // Text内容改变时，会触发SelectedIndexChanged事件，在该事件中刷新导联和重绘波形 add by lzy 20170704
                        this.toolStripComboBox_leadConfig.Text = this.controller.currentLeadConfigName;
                    }

                    tsrSender.Checked = true;
                };

                tsrLeadTypeList.DropDownItems.Add(tsrLeadType);
                this.toolStripComboBox_leadConfig.Items.Add(strLeadType);

                // 选中默认模式
                if (strLeadType.Equals(this.controller.currentLeadConfigName))
                {
                    tsrLeadType.Checked = true;
                    this.toolStripComboBox_leadConfig.Text = strLeadType;
                }
            }

            // 重载导联标签
            this.refreshLeadConfigLabel();
        }

        /// <summary>
        /// 刷新导联标签
        /// </summary>
        private void refreshLeadConfigLabel(List<String> leadConfig = null)
        {
            if (leadConfig == null)
            {
                leadConfig = this.controller.leadConfig;
            }

            int count = leadConfig.Count;
            if (count <= 0) return;

            // 最后一路固定是EKG心电
            // 20170626 针对BIO转换后的NoDisplay格式文件
            // TODO 临时给非CCF的配置加入EKG
            if (this.controller.currentLeadConfigName.IndexOf("CCF") < 0)
            {
                count += 1;
            }

            this.labelPanel.Controls.Clear();
            this.leadLabelList.Clear();

            Size size = new Size(70, 20);

            int totalHeight = this.labelPanel.Height;
            int offSet = totalHeight / (count + 1);
            int x = 0, y = offSet;

            // 添加全部导联
            foreach (String lead in leadConfig)
            {
                Label label = new Label();
                label.Text = lead;
                label.Name = lead;
                label.Location = new Point(x, y);
                label.Size = size;
                label.TextAlign = ContentAlignment.MiddleRight;

                // 20170626 针对BIO转换后的NoDisplay格式文件
                // TODO 临时的EKG、EMG转换逻辑
                switch (lead)
                {
                    case "X1-Y1":
                        label.Text = "EKG";
                        break;

                    case "X5-Y5":
                        label.Text = "EMG-LS";
                        break;

                    case "X6-Y6":
                        label.Text = "EMG-RS";
                        break;
                }

                this.leadLabelList.Add(label);
                this.labelPanel.Controls.Add(label);
                y += offSet;
            }

            // 添加最后一路EKG心电
            // 20170626 针对BIO转换后的NoDisplay格式文件
            // TODO 临时给非CCF的配置加入EKG
            if (this.controller.currentLeadConfigName.IndexOf("CCF") < 0)
            {
                Label ekgLabel = new Label();
                ekgLabel.Text = "EKG";
                ekgLabel.Name = "X1-Y1";
                ekgLabel.Location = new Point(x, y);
                ekgLabel.Size = size;
                ekgLabel.TextAlign = ContentAlignment.MiddleRight;
                this.leadLabelList.Add(ekgLabel);
                this.labelPanel.Controls.Add(ekgLabel);
            }

            // 重新绘制波形图
            if (this.controller.replayFile != null)
            {
                this.drawEEG();

                #region 绘制时间线

                if (replayTimerLineBeginP != null && replayTimerLineBeginP.X > 0)
                {
                    // 防止时间从头开始
                    isPaused = true;

                    // 跳转定位
                    SetTimeLine(replayTimerLineBeginP.X);
                }

                #endregion 绘制时间线
            }
        }

        public int getEEGPanelWidth()
        {
            return this.eegPanel.Width;
        }

        #region 菜单栏 - 事件

        private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
        {
            this.openFile();
        }

        /// <summary>
        /// 滤波设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Filter_Click(object sender, EventArgs e)
        {
            FormFilterConfig formFilterConfig = new FormFilterConfig(this, this.controller);
            formFilterConfig.ShowDialog();
        }

        /// <summary>
        /// 屏幕校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_calibrate_Click(object sender, EventArgs e)
        {
            FormCalibrate formCalibrate = new FormCalibrate(this, this.controller);
            formCalibrate.ShowDialog();
        }

        #endregion 菜单栏 - 事件

        #region 工具栏 - 事件

        /// <summary>
        /// 打开按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            this.openFile();
        }

        #endregion 工具栏 - 事件

        // TODO 应该把统一写到专用类里 20170619
        private FormVideo formVideo;

        /// <summary>
        /// 打开文件
        /// </summary>
        private void openFile()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "EDF|*.edf;*.edfx";
            DialogResult result = openDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    // 获取文件路径
                    var filePath = openDialog.FileName;

                    // 取文件名前两段作为识别连续文件的依据
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    {
                        var fileNameSplit = fileName.Split(' ', '_');
                        if (fileNameSplit.Length > 2)
                        {
                            fileName = $"{fileNameSplit[0]}?{fileNameSplit[1]}*.edf?";
                        }
                        else
                        {
                            fileName = $"{fileName}.edf?";
                        }
                    }

                    // 获取文件列表
                    ReplayList = Directory.GetFiles(Path.GetDirectoryName(filePath), fileName);
                    {
                        // 获取当前文件下标
                        for (int i = 0; i < ReplayList.Length; i++)
                        {
                            if (ReplayList[i].Equals(filePath))
                            {
                                ReplayCurrentIndex = i;
                                break;
                            }
                        }
                    }

                    // 生成控件
                    if (ReplayListTips == null)
                    {
                        ReplayListTips = new ToolStripLabel();
                        ReplayListSeparator = new ToolStripSeparator();

                        toolStrip2.Items.Insert(0, ReplayListTips);
                        toolStrip2.Items.Insert(1, ReplayListSeparator);
                    }

                    // 控件可见性
                    ReplayListTips.Visible = ReplayList.Length > 1;
                    ReplayListSeparator.Visible = ReplayListTips.Visible;

                    // 播放文件
                    OpenFile(ReplayCurrentIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }



        private void openFile2()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "EDF|*.edf;*.edfx;";
            DialogResult result = openDialog.ShowDialog();
            Debug.WriteLine(result);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
             
                try
                {
                    // 获取文件路径
                    var filePath = openDialog.FileName;
                   
                    // 取文件名前两段作为识别连续文件的依据
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    //MessageBox.Show(fileName);

                    {
                        //切割文件名
                        var fileNameSplit = fileName.Split(' ', '_');
                        //MessageBox.Show(fileNameSplit.ToString());
                        if (fileNameSplit.Length > 2) {
                            fileName = $"{fileNameSplit[0]}?{fileNameSplit[1]}*.edf?";
                        } else {
                            fileName = $"{fileName}.edf?";
                        }
                    }

                    // 获取文件列表
                    ReplayList = Directory.GetFiles(Path.GetDirectoryName(filePath), fileName);
                    {
                        // 获取当前文件下标
                        for (int i = 0; i < ReplayList.Length; i++)
                        {
                            if (ReplayList[i].Equals(filePath))
                            {
                                ReplayCurrentIndex = i;
                                break;
                            }
                        }
                       
                    }

                    // 生成控件
                    if (ReplayListTips == null)
                    {
                        ReplayListTips = new ToolStripLabel();
                        ReplayListSeparator = new ToolStripSeparator();

                        toolStrip2.Items.Insert(0, ReplayListTips);
                        toolStrip2.Items.Insert(1, ReplayListSeparator);
                    }

                    // 控件可见性
                    ReplayListTips.Visible = ReplayList.Length > 1;
                    ReplayListSeparator.Visible = ReplayListTips.Visible;


                    // 播放文件
                    //OpenFile(ReplayCurrentIndex);


                    MyEDF edfFile = new MyEDF();
                    edfFile.readFile(filePath);//读取EDF 文件 


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        #region 20171117 文件连续播放

        /// <summary>
        /// 回放列表
        /// </summary>
        private string[] ReplayList;

        /// <summary>
        /// 当前回放下标
        /// </summary>
        private int ReplayCurrentIndex;

        /// <summary>
        /// 回放进度提示控件
        /// </summary>
        private ToolStripLabel ReplayListTips;

        /// <summary>
        /// 回放进度提示控件分隔符
        /// </summary>
        private ToolStripSeparator ReplayListSeparator;

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenFile(int index)
        {
            // 数据完整性验证
            if (index < 0 || index >= ReplayList.Length) return;

            // 更新播放进度
            ReplayListTips.Text = $"播放进度：{index + 1}/{ReplayList.Length}";

            // 获取回放路径
            var filePath = ReplayList[index];

            // 初始化回放文件
            this.controller.initReplayFile(filePath);
            /* 20170626 NoDisplay现在不需要从EDF里读取导联
            // 加载EDF导联
            if (this.controller.replayFile.isEDFXFile() == false)
            {
                List<string> _leadConfig = new List<string>();
                for (int i = 0; i < this.controller.replayFile.Header.Signals.Count - 1; i++)
                {
                    _leadConfig.Add(this.controller.replayFile.Header.Signals[i].Label);
                }
                refreshLeadConfigLabel(_leadConfig);
            }
            */
            // 尝试打开同名的视频文件
            FileInfo videoFile = null;
            foreach (var item in new string[] { "mp4", "mov", "avi" })
            {
                videoFile = new FileInfo(Path.ChangeExtension(filePath, item));
                if (videoFile.Exists) break;
            }

            if (formVideo != null)
            {
                formVideo.Dispose();
            }
            formVideo = null;

            if (videoFile.Exists)
            {
                formVideo = new FormVideo();
                formVideo.VlcPlayer.SetMedia(videoFile);
                formVideo.VlcPlayer.Rate = this.controller.rate;

                this.controller.videoFilePath = videoFile.FullName;
            }
        }

        /// <summary>
        /// 上一个文件
        /// </summary>
        public void OpenPrev()
        {
            if (ReplayCurrentIndex > 0)
            {
                OpenFile(--ReplayCurrentIndex);

                controller.second = controller.duration - (int)controller.secondsPerPage + 1;
            }
        }

        /// <summary>
        /// 下一个文件
        /// </summary>
        public void OpenNext()
        {
            if (ReplayCurrentIndex + 1 < ReplayList.Length)
            {
                OpenFile(++ReplayCurrentIndex);
            }
        }

        #endregion 20171117 文件连续播放

        /// <summary>
        /// 更新工具栏信息
        /// </summary>
        public void updateToolBarInfo()
        {
            // 当前时间/总时间
            this.toolStripLabel_durationStatus.Text = "00:00:00/" + Tool.getTimeStringBySeconds(this.controller.duration);

            // 开始时间
            this.toolStripLabel_timeStatus.Text = this.controller.replayFile.Header.StartDateTime.ToString();
        }

        #region 绘制波形图 现有的多线程绘图导致不能缓存图形 反而影响重绘效率 初步计划先单线程完成功能 20170616

        // TODO 现在存在未加载完成的情况下重绘导致数组越界异常的情况

        /// <summary>
        /// 绘制波形图
        /// </summary>
        public void drawEEG()
        {
            // 回放终止
            timerReplay.Enabled = false;
            // TODO 临时用于判断是否暂停
            isPaused = false;
            // TODO 暂停播放
            if (formVideo != null)
            {
                if (formVideo.VlcPlayer.IsPlaying)
                {
                    formVideo.VlcPlayer.Pause();
                }
            }
            // TODO 更新当前时间

            #region 更新当前时间

            // 计算每秒钟的像素数（走纸速度*每厘米像素数）
            double _numOfPixelPerSecond = this.controller.pixelsPerSecond;// 暂时换个参数名不影响别的地方使用
            int duration = this.controller.second;
            this.toolStripLabel_durationStatus.Text = Tool.getTimeStringBySeconds(duration) + @"/" + Tool.getTimeStringBySeconds(this.controller.duration);

            #endregion 更新当前时间

            // TODO 翻页时校准视频时间 20170619
            if (formVideo != null && formVideo.VlcPlayer != null)
            {
                // 若视频播放完毕，则尝试重新播放，否则视频定位失效
                if (formVideo.VlcPlayer.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Stopped)
                {
                    formVideo.VlcPlayer.Play();
                    while (formVideo.VlcPlayer.State != Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                    {
                        Thread.Sleep(200);
                    }
                    formVideo.VlcPlayer.Pause();
                }
                if (formVideo.VlcPlayer.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Ended)
                {
                    formVideo.VlcPlayer.SetMedia(new FileInfo(this.controller.videoFilePath));
                    formVideo.VlcPlayer.Play();
                    while (formVideo.VlcPlayer.State != Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                    {
                        Thread.Sleep(200);
                    }
                    formVideo.VlcPlayer.Pause();
                }

                formVideo.VlcPlayer.Time = duration * 1000;
            }

            // 缓存波形图
            replayTimerCache = new Bitmap(this.eegPanel.Width, this.eegPanel.Height);

            #region 重置空白背景

            lock (_replayTimerCacheLocker)
            {
                using (Graphics c = Graphics.FromImage(replayTimerCache))
                {
                    using (Graphics g = this.eegPanel.CreateGraphics())
                    {
                        // 清空并用背景色填充
                        g.Clear(Color.WhiteSmoke);
                        // 缓存
                        c.Clear(Color.WhiteSmoke);

                        // 走纸速度大于1mm/s时绘制标尺
                        if (this.controller.speed > 1)
                        {
                            // 计算每秒钟的像素数（走纸速度*每厘米像素数）
                            var numOfPixelPerSecond = this.controller.pixelsPerSecond;

                            // 绘制标尺
                            //Pen rulerPerSecondPen = new Pen(Color.LightGray, 1);
                            Pen rulerPerSecondPen = new Pen(Color.FromArgb(102, 102, 102), 1);
                            PointF beginP, endP;
                            for (double i = 0; i < this.eegPanel.Width; i += numOfPixelPerSecond)
                            {
                                beginP = new PointF((float)i, 0);
                                endP = new PointF((float)i, this.eegPanel.Height);

                                // 绘图
                                g.DrawLine(rulerPerSecondPen, beginP, endP);
                                // 缓存
                                c.DrawLine(rulerPerSecondPen, beginP, endP);
                            }
                        }
                    }
                }
            }

            #endregion 重置空白背景

            #region 绘制波形图

            List<ManualResetEvent> mreList = new List<ManualResetEvent>();
            int width = this.eegPanel.Width; // 当前页宽像素数
            int dw = this.controller.generateDW(0);

            // TODO 20170619 逻辑理清前暂时用这个方法解决最后一页的问题
            if (this.controller.duration - this.controller.second < this.controller.secondsPerPage)
            {
                int _secondPerPage = (int)(this.eegPanel.Width / _numOfPixelPerSecond);
                width = (int)(width * (this.controller.duration - this.controller.second) / this.controller.secondsPerPage);
            }

            // 20170620
            var edfFile = this.controller.replayFile;

            if (edfFile.isEDFXFile() == false)
            {
                #region 旧逻辑，NoDisplay采用做差方式 20170626

                /*
                int _diffHeight = Math.Abs(this.leadLabelList[0].Location.Y + this.leadLabelList[0].Size.Height / 2) - (this.leadLabelList[1].Location.Y + this.leadLabelList[1].Size.Height / 2);

                #region 按实际文件的通道来画

                for (int i = 0; i < edfFile.Header.Signals.Count; i++)
                {
                    int count = this.controller.filterData[i].Length; // 当前页总点数

                    Label label = this.leadLabelList[i];
                    int baseLineHeight = label.Location.Y + label.Size.Height / 2;

                    // 开始画点
                    Graphics g = this.eegPanel.CreateGraphics();

                    PointF[] points = new PointF[count];
                    for (int k = 0; k < count; k++)
                    {
                        float x = 0f, y = 0f;
                        x = k * (float)width / (float)count;
                        //x = (float)(k * edfFile.Header.DurationOfDataRecordInSeconds / edfFile.Header.Signals[i].NumberOfSamplesPerDataRecord * this.controller.speed / 10);
                        //x = (float)(x * this.controller.pixelsPerCM);
                        if (edfFile.Header.Signals[i].Label.Equals("EKG"))
                        {
                            // EDF文件暂时写死灵敏度
                            var _EKGSensitivity = 1000;

                            //y = (float)((this.controller.filterData[i][k] / _EKGSensitivity * this.controller.pixelsPerCM) * dw + baseLineHeight);
                            y = (float)((this.controller.filterData[i][k] / _EKGSensitivity * _diffHeight) * dw + baseLineHeight);
                        }
                        else
                        {
                            //y = (float)(this.controller.generateDataOnChartViaData(this.controller.filterData[i][k]) * dw + baseLineHeight);
                            y = (float)((this.controller.filterData[i][k] / this.controller.sensitivity * _diffHeight) * dw + baseLineHeight);
                        }
                        points[k] = new PointF(x, y);
                    }

                    // 20170622 新的抽点算法，间隔像素内只存在一个点
                    if (points.Length > 0)
                    {
                        Console.WriteLine($"当前信道：{label.Text}。抽点前数据为：{points.Length}");

                        var _interval = 3;

                        List<PointF> tempPoints = new List<PointF>();
                        tempPoints.Add(points[0]);

                        PointF bPoint = points[0];
                        for (int k = 1; k < points.Length; k++)
                        {
                            PointF ePoint = points[k];

                            var diff = Math.Sqrt(Math.Pow((ePoint.X - bPoint.X), 2) + Math.Pow((ePoint.Y - bPoint.Y), 2));
                            if (diff > _interval)
                            {
                                bPoint = points[k];

                                tempPoints.Add(points[k]);
                            }
                        }

                        points = tempPoints.ToArray();

                        Console.WriteLine($"间隔单位为：{_interval}。抽点后数据为：{points.Length}");
                    }

                    Pen pen = new Pen(Color.Black, 1);
                    g.DrawLines(pen, points);

                    lock (_replayTimerCacheLocker)
                    {
                        using (Graphics c = Graphics.FromImage(replayTimerCache))
                        {
                            c.DrawLines(pen, points);
                        }
                    }

                    points = null;
                    g.Dispose();
                    g = null;
                }

                #endregion 按实际文件的通道来画

                */

                #endregion 旧逻辑，NoDisplay采用做差方式 20170626

                #region 20170626 新逻辑，针对BIO转换后的NoDisplay格式文件

                #region 转换信道方便调用

                Dictionary<string, int> dicSignals = new Dictionary<string, int>();
                for (int i = 0; i < edfFile.Header.Signals.Count; i++)
                {
                    dicSignals.Add(edfFile.Header.Signals[i].Label.ToUpper(), i);
                }

                /*
                Dictionary<string, int> dicSignals = new Dictionary<string, int>();
                EDFSignal signal;
                string signalName;
                for (int i = 0; i < edfFile.Header.Signals.Count; i++)
                {
                    signal = edfFile.Header.Signals[i];

                    // 没有Ref的信道直接使用做差后的数据
                    if (signal.Label.EndsWith("Ref", StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        dicSignals.Add(signal.Label, i);
                    }
                    // 其余信道忽略Ref
                    else
                    {
                        signalName = signal.Label.Split('-')[0].ToUpper();

                        #region PG1 = T1, PG2 = T2

                        signalName = signalName.Replace("PG1", "T1");
                        signalName = signalName.Replace("PG2", "T2");

                        #endregion PG1 = T1, PG2 = T2

                        dicSignals.Add(signalName, i);
                    }
                }
                */

                #endregion 转换信道方便调用

                #region 开始画点

                // 计算两个信道间的高度差作为阈值
                int _diffHeight = Math.Abs((this.leadLabelList[0].Location.Y + this.leadLabelList[0].Size.Height / 2) - (this.leadLabelList[1].Location.Y + this.leadLabelList[1].Size.Height / 2));

                // 依次画出每个信道的值
                Label leadLabel;
                for (int i = 0; i < this.leadLabelList.Count; i++)
                {
                    leadLabel = this.leadLabelList[i];

                    // Debug.WriteLine("drawChart(): leadLabel = " + leadLabel);

                    #region 当前信道的名称、Y轴基准点

                    string leadName = leadLabel.Name;
                    double basePointY = leadLabel.Location.Y + leadLabel.Size.Height / 2;

                    #endregion 当前信道的名称、Y轴基准点

                    #region 获取当前信道的数据

                    double[] signalData = null;

                    // 如果已存在做过差的信道则直接使用
                    if (dicSignals.ContainsKey(leadName))
                    {
                        signalData = this.controller.filterData[dicSignals[leadName]];
                    }
                    // 尝试解析信道并做差
                    else
                    {
                        // 解析信道
                        var splitSignals = leadName.Split('-');
                        if (splitSignals.Length.Equals(2))
                        {
                            splitSignals[0] = splitSignals[0].ToUpper();
                            splitSignals[1] = splitSignals[1].ToUpper();

                            // FPZ = REF
                            if (splitSignals[0].EndsWith("FPZ"))
                            {
                                signalData = this.controller.filterData[dicSignals[splitSignals[1]]];
                                for (int p = 0; p < signalData.Length; p++)
                                {
                                    signalData[p] = -signalData[p];
                                }
                            }
                            else if (splitSignals[1].EndsWith("FPZ"))
                            {
                                if (dicSignals.ContainsKey(splitSignals[0]))
                                    signalData = this.controller.filterData[dicSignals[splitSignals[0]]];
                            }
                            else if (dicSignals.ContainsKey(splitSignals[0]) && dicSignals.ContainsKey(splitSignals[1]))
                            {
                                var signalPositive = dicSignals[splitSignals[0]];
                                var signalNegative = dicSignals[splitSignals[1]];

                                signalData = new double[this.controller.filterData[signalPositive].Length];

                                // 做差
                                for (int p = 0; p < signalData.Length; p++)
                                {
                                    signalData[p] = this.controller.filterData[signalPositive][p] - this.controller.filterData[signalNegative][p];
                                }
                            }
                        }
                    }

                    // 解析失败则跳过
                    if (signalData == null) continue;

                    #endregion 获取当前信道的数据

                    #region 计算每个点在画布上的位置

                    PointF[] signalPoints = new PointF[signalData.Length];

                    for (int p = 0; p < signalData.Length; p++)
                    {
                        float x = 0f, y = 0f;

                        // 点平均分布在X轴上
                        x = p * (float)width / (float)signalData.Length;

                        // 根据灵敏度计算Y轴坐标
                        if (leadLabel.Name.IndexOfAny(new char[] { 'X', 'Y' }) < 0)
                        {
                            y = (float)((signalData[p] / this.controller.sensitivity * _diffHeight / 2) * dw + basePointY);
                        }
                        else
                        {
                            // EKG暂时写死灵敏度 2000
                            var _EKGSensitivity = 2000;
                            y = (float)((signalData[p] / _EKGSensitivity * _diffHeight / 2) * dw + basePointY);
                        }

                        signalPoints[p] = new PointF(x, y);
                    }

                    #endregion 计算每个点在画布上的位置

                    #region 20170622 新的抽点算法，间隔像素内只存在一个点

                    if (signalPoints.Length > 0)
                    {
                        var _interval = 0;

                        if (_interval > 0)
                        {
                            // Console.WriteLine($"当前信道：{leadLabel.Text}。抽点前数据为：{signalPoints.Length}");
                            Console.WriteLine(String.Format("当前信道：{0}。抽点前数据为：{1}", leadLabel.Text, signalPoints.Length));

                            List<PointF> tempPoints = new List<PointF>();
                            tempPoints.Add(signalPoints[0]);

                            PointF bPoint = signalPoints[0];
                            for (int k = 1; k < signalPoints.Length; k++)
                            {
                                PointF ePoint = signalPoints[k];

                                var diff = Math.Sqrt(Math.Pow((ePoint.X - bPoint.X), 2) + Math.Pow((ePoint.Y - bPoint.Y), 2));
                                if (diff > _interval)
                                {
                                    bPoint = signalPoints[k];

                                    tempPoints.Add(signalPoints[k]);
                                }
                            }

                            signalPoints = tempPoints.ToArray();

                            // Console.WriteLine($"间隔单位为：{_interval}。抽点后数据为：{signalPoints.Length}");
                            Console.WriteLine(String.Format("间隔单位为：{0}。抽点后数据为：{1}", _interval, signalPoints.Length));
                        }
                    }

                    #endregion 20170622 新的抽点算法，间隔像素内只存在一个点

                    #region 开始画点

                    Pen pen = new Pen(Color.Black, 1);
                    using (Graphics g = this.eegPanel.CreateGraphics())
                    {
                        g.SmoothingMode = SmoothingMode.HighSpeed;
                        g.DrawLines(pen, signalPoints);

                        lock (_replayTimerCacheLocker)
                        {
                            using (Graphics c = Graphics.FromImage(replayTimerCache))
                            {
                                c.SmoothingMode = SmoothingMode.HighSpeed;
                                c.DrawLines(pen, signalPoints);
                            }
                        }
                    }

                    #endregion 开始画点
                }

                #endregion 开始画点

                #endregion 20170626 新逻辑，针对BIO转换后的NoDisplay格式文件
            }
            else
            {
                int count = this.controller.filterData[0].Length; // 当前页总点数

                // 最后一个导联是EKG心电，所以单独处理
                for (int i = 0; i < this.leadLabelList.Count - 1; i++)
                {
                    Label label = this.leadLabelList[i];
                    int baseLineHeight = label.Location.Y + label.Size.Height / 2; // 每条波形的基准线
                    ManualResetEvent m = new ManualResetEvent(false);
                    mreList.Add(m);

                    ThreadPool.QueueUserWorkItem(delegate (Object obj)
                    {
                        Object[] param = (Object[])obj;

                        int zeroLine = (int)param[0]; // Y轴基准高度
                        String leadConfig = (String)param[1];
                        ManualResetEvent mre = (ManualResetEvent)param[2];
                        int sIndex = (int)param[3];

                        String[] FPi_FPj = leadConfig.Split(new char[] { '-' });
                        // 查找做差电极对应的通道号
                        int channelNum_Positive = 1;
                        int channelNum_Negative = 1;
                        foreach (int key in this.controller.leadSource.Keys)
                        {
                            if (this.controller.leadSource[key].Equals(FPi_FPj[0]))
                                channelNum_Positive = key;
                            if (this.controller.leadSource[key].Equals(FPi_FPj[1]))
                                channelNum_Negative = key;
                        }

                        // 两个电极电位值，二者的差值
                        double sampleValue_Positive = 0; double sampleValue_Negative = 0; double sampleDifferValue = 0;

                        Graphics g = this.eegPanel.CreateGraphics();
                        PointF[]
                        points = new PointF[count];
                        for (int k = 0; k < count; k++)
                        {
                            float x = 0f, y = 0f;
                            x = k * ((float)width / (float)count);

                            if (!FPi_FPj[0].Equals("FPz")) sampleValue_Positive = this.controller.filterData[channelNum_Positive][k];
                            if (!FPi_FPj[1].Equals("FPz")) sampleValue_Negative = this.controller.filterData[channelNum_Negative][k];
                            if (FPi_FPj[1].Equals("AVG")) sampleValue_Negative = this.controller.filterData[this.controller.filterData.Length - 1][k];
                            sampleDifferValue = sampleValue_Positive - sampleValue_Negative;

                            y = (float)(-this.controller.generateDataOnChartViaData(sampleDifferValue)) * dw + zeroLine;

                            points[k] = new PointF(x, y);
                        }

                        Pen pen = new Pen(Color.Black, 1);
                        g.DrawLines(pen, points);

                        lock (_replayTimerCacheLocker)
                        {
                            using (Graphics c = Graphics.FromImage(replayTimerCache))
                            {
                                c.DrawLines(pen, points);
                            }
                        }

                        points = null;
                        g.Dispose();
                        g = null;

                        mre.Set();
                    }, new Object[] { baseLineHeight, label.Text, m, i });
                }

                // 绘制EKG心电
                Label ekgLabel = this.leadLabelList[this.leadLabelList.Count - 1];
                int ekgBaseLine = ekgLabel.Location.Y + ekgLabel.Size.Height / 2;
                PointF[] ekgPoints = new PointF[count];
                for (int k = 0; k < count; k++)
                {
                    float x = 0f, y = 0f;
                    x = k * ((float)width / (float)count);
                    y = (float)(-this.controller.generateDataOnChartViaData(this.controller.filterData[0][k])) * dw + ekgBaseLine;
                    ekgPoints[k] = new PointF(x, y);
                }
                Graphics g2 = this.eegPanel.CreateGraphics();
                g2.DrawLines(new Pen(Color.Black, 1), ekgPoints);

                lock (_replayTimerCacheLocker)
                {
                    using (Graphics c = Graphics.FromImage(replayTimerCache))
                    {
                        c.DrawLines(new Pen(Color.Black, 1), ekgPoints);
                    }
                }

                ekgPoints = null;
                g2.Dispose();
                g2 = null;

                foreach (ManualResetEvent tmp in mreList)
                {
                    tmp.WaitOne();
                }
            }

            #endregion 绘制波形图

            #region 绘制标记信息

            if (this.controller.MarkList.Count > 0)
            {
                using (Graphics c = Graphics.FromImage(replayTimerCache), g = this.eegPanel.CreateGraphics())
                {
                    int secondsPerPage = (int)this.controller.secondsPerPage;

                    Dictionary<float, int> offsetDic = new Dictionary<float, int>(); // 暂存每个标记的偏移位置，如果有一样的，则标记文字需要下移

                    foreach (var mark in this.controller.MarkList)
                    {
                        // 标记时间
                        DateTime markTime = mark.DateTime;
                        var markSecond = markTime.Hour * 3600 + markTime.Minute * 60 + markTime.Second + (double)markTime.Millisecond / 1000;

                        int pageStartSecond = this.controller.second - 1;
                        int pageEndSecond = pageStartSecond + secondsPerPage;

                        if (markSecond >= pageStartSecond && markSecond <= pageEndSecond)
                        {
                            var p = new Pen(Color.Orange, 3);
                            var b = new SolidBrush(Color.Black);

                            // 高亮选中项
                            {
                                if (markListBox != null)
                                {
                                    var selectedMark = markListBox.SelectedIndex;
                                    if (selectedMark >= 0)
                                    {
                                        if (mark.Equals(controller.MarkList[selectedMark]))
                                        {
                                            b = new SolidBrush(Color.Red);
                                        }
                                    }
                                }
                            }

                            PointF beginP, endP, stringP;
                            float offsetSeconds = (float)markSecond - pageStartSecond;
                            float offset = offsetSeconds * (float)controller.pixelsPerSecond;

                            // 绘制直线
                            if (offsetDic.ContainsKey(offset) == false)
                            {
                                beginP = new PointF(offset, 0);
                                endP = new PointF(offset, this.eegPanel.Height);

                                g.DrawLine(p, beginP, endP);
                                c.DrawLine(p, beginP, endP);
                            }

                            int times = 0;
                            if (offsetDic.ContainsKey(offset))
                            {
                                times = offsetDic[offset];
                            }
                            times++;

                            offsetDic[offset] = times;

                            // 绘制标签
                            stringP = new PointF(offset, times * 30F);

                            g.DrawString(mark.Name, new Font("黑体", 12, FontStyle.Bold), b, stringP);

                            c.DrawString(mark.Name, new Font("黑体", 12, FontStyle.Bold), b, stringP);
                        }
                    }

                    offsetDic.Clear();
                    offsetDic = null;
                }
            }

            #endregion 绘制标记信息
        }

        /*
        /// <summary>
        /// 绘制波形图
        /// </summary>
        public void drawEEG()
        {
            #region 绘制波形图

            this.eegPanelClear();

            List<ManualResetEvent> mreList = new List<ManualResetEvent>();
            int count = this.controller.filterData[0].Length; // 当前页总点数
            int width = this.eegPanel.Width; // 当前页宽像素数
            int dw = this.controller.generateDW(0);

            // 最后一个导联是EKG心电，所以单独处理
            for (int i = 0; i < this.leadLabelList.Count - 1; i++)
            {
                Label label = this.leadLabelList[i];
                int baseLineHeight = label.Location.Y + label.Size.Height / 2; // 每条波形的基准线
                ManualResetEvent m = new ManualResetEvent(false);
                mreList.Add(m);

                ThreadPool.QueueUserWorkItem(delegate (Object obj)
                {
                    Object[] param = (Object[])obj;

                    int zeroLine = (int)param[0]; // Y轴基准高度
                    String leadConfig = (String)param[1];
                    ManualResetEvent mre = (ManualResetEvent)param[2];
                    int sIndex = (int)param[3];

                    String[] FPi_FPj = leadConfig.Split(new char[] { '-' });
                    // 查找做差电极对应的通道号
                    int channelNum_Positive = 1;
                    int channelNum_Negative = 1;
                    foreach (int key in this.controller.leadSource.Keys)
                    {
                        if (this.controller.leadSource[key].Equals(FPi_FPj[0]))
                            channelNum_Positive = key;
                        if (this.controller.leadSource[key].Equals(FPi_FPj[1]))
                            channelNum_Negative = key;
                    }
                    // 两个电极电位值，二者的差值
                    double sampleValue_Positive = 0; double sampleValue_Negative = 0; double sampleDifferValue = 0;

                    Graphics g = this.eegPanel.CreateGraphics();
                    PointF[] points = new PointF[count];
                    for (int k = 0; k < count; k++)
                    {
                        float x = 0f, y = 0f;
                        x = k * ((float)width / (float)count);

                        if (!FPi_FPj[0].Equals("FPz")) sampleValue_Positive = this.controller.filterData[channelNum_Positive][k];
                        if (!FPi_FPj[1].Equals("FPz")) sampleValue_Negative = this.controller.filterData[channelNum_Negative][k];
                        sampleDifferValue = sampleValue_Positive - sampleValue_Negative;

                        y = (float)(this.controller.generateDataOnChartViaData(sampleDifferValue)) * dw + zeroLine;
                        points[k] = new PointF(x, y);
                    }

                    Pen pen = new Pen(Color.Black, 1);
                    g.DrawLines(pen, points);

                    points = null;
                    g.Dispose();
                    g = null;

                    mre.Set();
                }, new Object[] { baseLineHeight, label.Text, m, i });
            }

            // 绘制EKG心电
            Label ekgLabel = this.leadLabelList[this.leadLabelList.Count - 1];
            int ekgBaseLine = ekgLabel.Location.Y + ekgLabel.Size.Height / 2;
            PointF[] ekgPoints = new PointF[count];
            for (int k = 0; k < count; k++)
            {
                float x = 0f, y = 0f;
                x = k * ((float)width / (float)count);
                y = (float)(this.controller.generateDataOnChartViaData(this.controller.filterData[0][k])) * dw + ekgBaseLine;
                ekgPoints[k] = new PointF(x, y);
            }
            Graphics g2 = this.eegPanel.CreateGraphics();
            g2.DrawLines(new Pen(Color.Black, 1), ekgPoints);
            ekgPoints = null;
            g2.Dispose();
            g2 = null;

            foreach (ManualResetEvent tmp in mreList)
            {
                tmp.WaitOne();
            }

            #endregion 绘制波形图
        }

        */

        #endregion 绘制波形图 现有的多线程绘图导致不能缓存图形 反而影响重绘效率 初步计划先单线程完成功能 20170616

        /// <summary>
        /// 重置波形图面板
        /// </summary>
        private void eegPanelClear()
        {
            using (Graphics g = this.eegPanel.CreateGraphics())
            {
                // 清空并用背景色填充
                g.Clear(Color.WhiteSmoke);

                // 走纸速度大于1mm/s时绘制标尺
                if (this.controller.speed > 1)
                {
                    // 计算每秒钟的像素数（走纸速度*每厘米像素数）
                    double numOfPixelPerSecond = this.controller.speed * this.controller.pixelsPerCM / 10;

                    // 绘制标尺
                    Pen rulerPerSecondPen = new Pen(Color.LightGray, 1);
                    PointF beginP, endP;
                    for (double i = 0; i < this.eegPanel.Width; i += numOfPixelPerSecond)
                    {
                        beginP = new PointF((float)i, 0);
                        endP = new PointF((float)i, this.eegPanel.Height);

                        g.DrawLine(rulerPerSecondPen, beginP, endP);
                    }
                }
            }
        }

        #region 工具栏 - 事件

        /// <summary>
        /// 灵敏度改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_sensitivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.controller.sensitivity = int.Parse(this.toolStripComboBox_sensitivity.Text);
        }

        /// <summary>
        /// 低频滤波参数改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_LowFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.controller.lowFreq = double.Parse(this.toolStripComboBox_LowFreq.Text);
        }

        /// <summary>
        /// 高频滤波参数改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_HighFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.controller.highFreq = double.Parse(this.toolStripComboBox_HighFreq.Text);
        }

        /// <summary>
        /// 走纸速度改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_speed_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.controller.speed = double.Parse(this.toolStripComboBox_speed.Text);
        }

        /// <summary>
        /// 回放倍速改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_rate_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.controller.rate = int.Parse(this.toolStripComboBox_rate.Text);

            // TODO 20170619
            if (formVideo != null && formVideo.VlcPlayer != null)
            {
                formVideo.VlcPlayer.Rate = float.Parse(this.toolStripComboBox_rate.Text);
            }
        }

        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_zoomUp_Click(object sender, EventArgs e)
        {
            int index = this.toolStripComboBox_sensitivity.SelectedIndex;
            if (index == 0) return;
            this.toolStripComboBox_sensitivity.SelectedIndex = index - 1;
        }

        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_zoomDown_Click(object sender, EventArgs e)
        {
            int index = this.toolStripComboBox_sensitivity.SelectedIndex;
            if (index == this.toolStripComboBox_sensitivity.Items.Count - 1) return;
            this.toolStripComboBox_sensitivity.SelectedIndex = index + 1;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_prePage_Click(object sender, EventArgs e)
        {
            // if (this.controller.second < this.controller.secondsPerPage) return;
            this.controller.second = this.controller.second - (int)this.controller.secondsPerPage;
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_nextPage_Click(object sender, EventArgs e)
        {
            // if (this.controller.second + this.controller.secondsPerPage > this.controller.duration) return;
            this.controller.second = this.controller.second + (int)this.controller.secondsPerPage;
        }

        /// <summary>
        /// 上一秒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_perSecond_Click(object sender, EventArgs e)
        {
            this.controller.second = this.controller.second - 1;
        }

        /// <summary>
        /// 下一秒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_nextSecond_Click(object sender, EventArgs e)
        {
            this.controller.second = this.controller.second + 1;
        }

        #endregion 工具栏 - 事件

        /// <summary>
        /// 窗口加载完成后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Shown(object sender, EventArgs e)
        {
            // 重载导联
            this.refreshLeadConfigLabel();

            // 计算标尺（每页30mm/s*10s)
            this.controller.pixelsPerCM = (float)this.eegPanel.Width / 30;
        }

        /// <summary>
        /// 窗口关闭时，保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.controller.Dispose();
        }

        /// <summary>
        /// 重载后重绘波形图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eegPanel_Paint(object sender, PaintEventArgs e)
        {
            // 重新绘制波形图
            if (this.controller.replayFile != null)
            {
                // TODO 公用的重绘缓存方法
                if (replayTimerCache != null)
                {
                    using (Bitmap cBitMap = (Bitmap)replayTimerCache.Clone())
                    {
                        using (replayTimerGraphics = Graphics.FromImage(cBitMap))
                        {
                            using (Graphics g = this.eegPanel.CreateGraphics())
                            {
                                g.DrawImage(cBitMap, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    this.drawEEG();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            //拦截双击标题栏、移动窗体的系统消息
            if (m.Msg != 0xA3 && m.Msg != 0x0003 && m.WParam != (IntPtr)0xF012)
            {
                base.WndProc(ref m);
            }
        }

        #region 回放 20170615

        // TODO 应该写一个专用类
        private Graphics replayTimerGraphics;

        private PointF replayTimerLineBeginP, replayTimerLineEndP;

        // 这个实际作用于缓存当前页波形图，逻辑可以提升为重绘页面时也能使用
        private Bitmap replayTimerCache;

        // 20170619 尝试用锁防止多线程调用出错
        private object _replayTimerCacheLocker = new object();

        private Pen replayTimerPen = new Pen(Color.Red, 1);

        // TODO 临时用于判断是否暂停
        private bool isPaused = false;

        /// <summary>
        /// 回放
        /// </summary>
        private void toolStripButton_play_Click(object sender, EventArgs e)
        {
            // TODO 应该根据文件加载/正在播放情况禁用回放按钮
            if (this.controller.replayFile == null) return;

            // TODO 应该根据正在播放情况判断是否新增时间线
            if (timerReplay.Enabled) return;

            // 开始回放
            StartReplay();
            if (this.formVideo != null)
            {
                this.formVideo.Show();

                // 开始回放时，重新定位视频，和回放红线保持一致 add by lzy 20170703
                formVideo.VlcPlayer.Time = (this.controller.second + (long)(replayTimerLineBeginP.X / this.controller.pixelsPerSecond)) * 1000;
                Debug.WriteLine("formVideo.VlcPlayer.Time = " + formVideo.VlcPlayer.Time);
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_pause_Click(object sender, EventArgs e)
        {
            // 尝试暂停视频文件
            if (formVideo != null)
            {
                if (formVideo.VlcPlayer.IsPlaying)
                {
                    formVideo.VlcPlayer.Pause();
                }
            }

            timerReplay.Enabled = false;
            isPaused = true;
        }

        /// <summary>
        /// 回放定时器（每100毫秒执行一次）
        /// </summary>
        private void timerReplay_Tick(object sender, EventArgs e)
        {
            // 计算每秒钟的像素数（走纸速度*每厘米像素数）
            double numOfPixelPerSecond = this.controller.pixelsPerSecond;

            // 计算新的X轴（*倍率）
            double newPointX = replayTimerLineBeginP.X + numOfPixelPerSecond / (1000 / this.timerReplay.Interval) * this.controller.rate;

            // 超出范围则跳转至下一页或者结束
            if (newPointX > this.eegPanel.Width)
            {
                // 继续播放下一页
                if (this.controller.second + this.controller.secondsPerPage < this.controller.duration)
                {
                    this.controller.second = this.controller.second + (int)this.controller.secondsPerPage;

                    StartReplay();
                }
                else
                {
                    // TODO 公用的重绘缓存方法
                    if (replayTimerCache != null)
                    {
                        using (Bitmap cBitMap = (Bitmap)replayTimerCache.Clone())
                        {
                            using (replayTimerGraphics = Graphics.FromImage(cBitMap))
                            {
                                using (Graphics g = this.eegPanel.CreateGraphics())
                                {
                                    g.DrawImage(cBitMap, 0, 0);
                                }
                            }
                        }
                    }

                    // TODO
                    this.toolStripLabel_durationStatus.Text = Tool.getTimeStringBySeconds(this.controller.duration) + @"/" + Tool.getTimeStringBySeconds(this.controller.duration);

                    // 停止计时器
                    timerReplay.Enabled = false;
                }
                return;
            }
            else
            {
                replayTimerLineBeginP.X = (float)newPointX;
            }

            // TODO
            // 绘制回放时间线
            using (Bitmap cBitMap = (Bitmap)replayTimerCache.Clone())
            {
                using (replayTimerGraphics = Graphics.FromImage(cBitMap))
                {
                    replayTimerGraphics.DrawLine(replayTimerPen,
                        replayTimerLineBeginP,
                        new PointF(replayTimerLineBeginP.X, this.eegPanel.Height)
                    );

                    using (Graphics g = this.eegPanel.CreateGraphics())
                    {
                        g.DrawImage(cBitMap, 0, 0);

                        // cBitMap.Save(@"C:\edfx\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") +".png");
                    }
                }
            }

            // 更新当前时间
            int duration = this.controller.second + (int)(newPointX / this.controller.pixelsPerSecond);
            this.toolStripLabel_durationStatus.Text = Tool.getTimeStringBySeconds(duration) + @"/" + Tool.getTimeStringBySeconds(this.controller.duration);

            // TODO 虽然不知道为什么现在的绘制波形图方法能划出超过总时长的波形图，但是到了总时长还是应该停下来
            if (duration.Equals(this.controller.duration) || duration > this.controller.duration)
            {
                timerReplay.Enabled = false;
            }
        }

        /// <summary>
        /// 开始回放
        /// </summary>
        private void StartReplay()
        {
            // 判断是否为继续播放
            if (isPaused == false)
            {
                // 初始化时间线
                replayTimerLineBeginP = new PointF(0f, 0f);

                // TODO
                // 绘制回放时间线
                using (Bitmap cBitMap = (Bitmap)replayTimerCache.Clone())
                {
                    using (replayTimerGraphics = Graphics.FromImage(cBitMap))
                    {
                        replayTimerGraphics.DrawLine(replayTimerPen,
                            replayTimerLineBeginP,
                            new PointF(replayTimerLineBeginP.X, this.eegPanel.Height)
                        );

                        using (Graphics g = this.eegPanel.CreateGraphics())
                        {
                            g.DrawImage(cBitMap, 0, 0);
                        }
                    }
                }
            }

            // 开始回放
            timerReplay.Enabled = true;

            // 解除暂停
            isPaused = false;

            // 尝试播放视频文件
            if (formVideo != null)
            {
                // formVideo.Show(); // 此处先不要show，否则会导致在回放时关不掉视频窗口
                formVideo.VlcPlayer.Play();
            }
        }

        #endregion 回放 20170615

        #region 鼠标定位时间轴

        private bool isMoving = false;

        /// <summary>
        /// 鼠标定位时间轴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eegSpcMain_Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.controller.replayFile != null)
            {
                // 尝试暂停
                toolStripButton_pause_Click(sender, e);

                // 20171205 测量
                if (InMeasurementMode)
                {
                    // 目标点重叠的情况下更新（防止双击）
                    if ((MPQ[1] != null) && MPQ[1].X.Equals(e.X) && MPQ[1].Y.Equals(e.Y))
                    {
                        return;
                    }

                    // 按下ctrl的情况下只修改最后一点
                    if ((ModifierKeys & Keys.Control) != Keys.Control)
                    {
                        MPQ[0] = MPQ[1];
                    }

                    MPQ[1] = new Point(e.X, e.Y);

                    DrawMeasurementArea();

                    return;
                }

                // 跳转定位
                SetTimeLine(e.X);

                // 解锁移动
                isMoving = true;
            }
        }

        private void eegSpcMain_Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                // 跳转定位
                SetTimeLine(e.X);
            }
        }

        private void eegSpcMain_Panel2_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }

        #endregion 鼠标定位时间轴

        /// <summary>
        /// 跳转定位
        /// </summary>
        private void SetTimeLine(float newPointX)
        {
            // 按下ctrl且移动的情况下
            if (((ModifierKeys & Keys.Control) == Keys.Control) && isMoving)
            {
                replayTimerLineEndP.X = newPointX == 0 ? -1 : newPointX;
            }
            else
            {
                replayTimerLineBeginP.X = (float)newPointX;
                replayTimerLineEndP = PointF.Empty;
            }

            // 计算每秒钟的像素数（走纸速度*每厘米像素数）
            double numOfPixelPerSecond = this.controller.pixelsPerSecond;

            // 更新当前时间
            int duration = this.controller.second + (int)(newPointX / this.controller.pixelsPerSecond);
            this.toolStripLabel_durationStatus.Text = Tool.getTimeStringBySeconds(duration) + @"/" + Tool.getTimeStringBySeconds(this.controller.duration);

            if (duration <= this.controller.duration)
            {
                // TODO
                // 绘制回放时间线
                using (Bitmap cBitMap = (Bitmap)replayTimerCache.Clone())
                {
                    using (replayTimerGraphics = Graphics.FromImage(cBitMap))
                    {
                        if (replayTimerLineEndP.IsEmpty)
                        {
                            replayTimerGraphics.DrawLine(replayTimerPen,
                                replayTimerLineBeginP,
                                new PointF(replayTimerLineBeginP.X, this.eegPanel.Height)
                            );
                        }
                        else
                        {
                            replayTimerGraphics.FillRectangle(
                                new SolidBrush(Color.FromArgb(128, Color.LightGreen)),
                                new Rectangle(
                                    (int)(Math.Min(replayTimerLineBeginP.X, replayTimerLineEndP.X)), 0,
                                    (int)(Math.Abs(replayTimerLineBeginP.X - replayTimerLineEndP.X)),
                                    this.eegPanel.Height)
                            );
                        }

                        using (Graphics g = this.eegPanel.CreateGraphics())
                        {
                            g.DrawImage(cBitMap, 0, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 工具栏导联切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_leadConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 根据选择的模式重载导联
            this.controller.currentLeadConfigName = this.toolStripComboBox_leadConfig.Text;
            this.refreshLeadConfigLabel();

            // 调整菜单栏的导联标记选中项
            for (int i = 0; i < tsrLeadTypeList.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)tsrLeadTypeList.DropDownItems[i];
                item.Checked = this.controller.currentLeadConfigName.Equals(item.Text);
            }
        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys modifiers = e.Modifiers;

            // 首次按下Ctrl，不处理
            if (keyCode == Keys.ControlKey && modifiers == Keys.Control) return;

            switch (keyCode)
            {
                case Keys.Up:
                    if (toolStripComboBox_sensitivity.SelectedIndex > 0)
                    {
                        toolStripComboBox_sensitivity.SelectedIndex = toolStripComboBox_sensitivity.SelectedIndex - 1;
                    }
                    e.Handled = true;
                    break;

                case Keys.Down:
                    if (toolStripComboBox_sensitivity.SelectedIndex < toolStripComboBox_sensitivity.Items.Count - 1)
                    {
                        toolStripComboBox_sensitivity.SelectedIndex = toolStripComboBox_sensitivity.SelectedIndex + 1;
                    }
                    e.Handled = true;
                    break;

                case Keys.Left:
                    if (modifiers == Keys.Control)
                    {
                        this.controller.second = this.controller.second - 1;
                    }
                    else
                    {
                        this.toolStripButton_prePage_Click(this.toolStripButton_prePage, null);
                    }
                    e.Handled = true;
                    break;

                case Keys.Right:
                    if (modifiers == Keys.Control)
                    {
                        this.controller.second = this.controller.second + 1;
                    }
                    else
                    {
                        this.toolStripButton_nextPage_Click(this.toolStripButton_nextPage, null);
                    }
                    e.Handled = true;
                    break;

                case Keys.Space:
                    if (this.timerReplay.Enabled == true)
                    {
                        toolStripButton_pause_Click(this.toolStripButton_pause, null);
                    }
                    else
                    {
                        toolStripButton_play_Click(this.toolStripButton_play, null);
                    }
                    e.Handled = true;
                    break;
            }
        }

        #region 滚动条设置

        /// <summary>
        /// 重置滚动条，匹配正确的页码
        /// </summary>
        public void resetScrollBar()
        {
            float newValue = (float)(this.controller.duration / this.controller.secondsPerPage);
            this.scrollBar.MaxValue = newValue < 1 ? 1 : (int)newValue;

            newValue = (float)(this.controller.second / this.controller.secondsPerPage);
            if (newValue < 1) newValue = 1;
            if (newValue > this.scrollBar.MaxValue) newValue = this.scrollBar.MaxValue;
            this.scrollBar.Value = newValue;

            // Debug.WriteLine("scrollBar.MaxValue = " + scrollBar.MaxValue + ", scrollBar.Value = " + scrollBar.Value + ", scrollBar.Size = " + scrollBar.Size);
        }

        /// <summary>
        /// 滚动条事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollBar_MouseClick(Object sender, MouseEventArgs e)
        {
            // MessageBox.Show("scrollBar_MouseClick -- 2, " + this.scrollBar.Value);
            if (!sender.Equals(this.scrollBar)) return;

            this.controller.second = (int)Math.Ceiling((this.scrollBar.Value) * this.controller.secondsPerPage);
        }

        #endregion 滚动条设置

        #region 标记信息功能

        /// <summary>
        /// 切换标记栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMark_Click(object sender, EventArgs e)
        {
            if (controller.replayFile == null)
            {
                MessageBox.Show("请先打开脑电文件！");

                tsbMark.Checked = false;
                return;
            }

            // 切换按钮状态
            tsbMark.Checked = !tsbMark.Checked;

            // 是否展示标记信息窗口
            var isShowMarkWindow = tsbMark.Checked;

            // 标记信息窗口变化之前，eegPanel的宽度
            int tmpWidth = this.eegPanel.Width;

            // 刷新标记信息窗口
            if (isShowMarkWindow == true)
            {
                // 初始化标记控件
                initMarkListControl();
            }

            // 切换标记栏
            tsMarkInsert.Visible = isShowMarkWindow;

            // 切换侧边栏
            spcMain.Panel1Collapsed = !isShowMarkWindow;

            // 标记窗口变化后，若是打开，则需要压缩波形并重绘，若是关闭标记窗口，则恢复原状
            controller.pixelsPerCM = controller.pixelsPerCM * ((double)eegPanel.Width / tmpWidth);

            // 重载导联标签
            refreshLeadConfigLabel();
        }

        /// <summary>
        /// 初始化标记控件
        /// </summary>
        private void initMarkListControl()
        {
            // 生成控件
            if (markListBox == null)
            {
                // 标记栏
                {
                    // 标记列表
                    var markType = new string[]
                    {
                        "睁眼", "闭眼", "睡醒", "瞌睡", "熟睡",
                        "移动", "说话", "咀嚼", "吞咽", "打鼾",
                        "运睛", "眼神", "眼呆", "无神", "镇静",
                        "事件", "无聊", "笑声", "抽搐", "眼飘",
                        "发绀", "腿挛", "肢麻", "体搐"
                    };

                    // 添加列表
                    foreach (var item in markType)
                    {
                        // 按钮
                        var tsb = new ToolStripButton(item);

                        // 按钮事件
                        tsb.Click += (sender, e) =>
                        {
                            addMark(item);
                        };

                        // 添加按钮
                        tsMarkInsert.Items.Add(tsb);

                        // 添加分割线
                        tsMarkInsert.Items.Add(new ToolStripSeparator());
                    }

                    // 添加自定义按钮
                    {
                        tsMarkInsert.Items.Add(new ToolStripButton("自定义", null, (sender, e) =>
                        {
                            var title = "输入标记名称";

                            var text = string.Empty;

                            var callback = new Action<string>((input) =>
                            {
                                addMark(input);
                            });

                            using (var form = new FormInput(title, text, callback))
                            {
                                form.ShowDialog();
                            }
                        }));
                    }
                }

                // 侧边栏
                {
                    markListBox = new ListBox();
                    markListBox.Parent = spcMain.Panel1;

                    // 标记列表尺寸设置（在显示后的值才对，否则尺寸不正确）
                    markListBox.Dock = DockStyle.Fill;

                    // 字号调整
                    markListBox.Font = new Font(FontFamily.GenericMonospace, 8);

                    // 标记窗口自定义事件
                    markListBox.SelectedIndexChanged -= markListBox_SelectedIndexChanged;
                    markListBox.SelectedIndexChanged += markListBox_SelectedIndexChanged;

                    // 快捷键
                    markListBox.KeyDown += (sender, e) =>
                    {
                        // 未选中时无效
                        if (markListBox.SelectedIndex < 0) return;

                        // 删除
                        if (e.KeyCode == Keys.Delete)
                        {
                            removeSelectedMark();
                        }
                    };

                    // 标记右键菜单
                    {
                        // 生成右键菜单
                        var cms = new ContextMenuStrip();

                        // 编辑
                        {
                            cms.Items.Add(new ToolStripButton("编辑", null, (sender, e) =>
                            {
                                var selectedIndex = markListBox.SelectedIndex;

                                var title = "输入要修改的名称";

                                var text = controller.MarkList[selectedIndex].Name;

                                var callback = new Action<string>((input) =>
                                {
                                    updateSelectedMark(input);
                                });

                                using (var form = new FormInput(title, text, callback))
                                {
                                    form.ShowDialog();
                                }
                            }));
                        }

                        // 删除
                        {
                            cms.Items.Add(new ToolStripButton("删除", null, (sender, e) =>
                            {
                                removeSelectedMark();
                            }));
                        }

                        // 右键点击事件
                        markListBox.MouseDown += (sender, e) =>
                        {
                            if (e.Button == MouseButtons.Right)
                            {
                                // 未选中时不弹出菜单
                                if (markListBox.SelectedIndex < 0) return;

                                // 弹出菜单项
                                cms.Show(MousePosition.X, MousePosition.Y);
                            }
                        };
                    }
                }
            }

            // 若没有标记信息，则弹出提示框
            /*
            if (this.controller.MarkInfoDic.Count < 1)
            {
                MessageBox.Show("该脑电文件不包含标记信息！");
                this.toolStripMenuItem_markInfo.Checked = false;
                return;
            }
            */

            // 刷新标记栏信息
            refreshMarkListBox();
        }

        /// <summary>
        /// 刷新标记栏信息
        /// </summary>
        public void refreshMarkListBox()
        {
            // 尚未初始化
            if (markListBox == null) return;

            // 清空列表
            markListBox.Items.Clear();

            // 加载列表
            markListBox.Items.AddRange(
                controller.MarkList.Select(mark =>
                {
                    var markTime = mark.DateTime;

                    var length = 30 - (Encoding.Default.GetBytes(mark.Name).Length - mark.Name.Length);

                    return mark.Name.PadRight(length) + markTime.ToString("HH:mm:ss.fff");
                }).ToArray()
            );
        }

        /// <summary>
        /// 选中标记文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void markListBox_SelectedIndexChanged(Object sender, EventArgs e)
        {
            var selectedIndex = markListBox.SelectedIndex;
            if (selectedIndex < 0) return; // 选中空白区域时，SelectedIndex = -1
            if (selectedIndex.Equals(markListBox.Tag)) return; // 防止重复选中

            // 记录当前选中下标
            markListBox.Tag = selectedIndex;

            // 获取选中的标记时间
            var markTime = controller.MarkList[selectedIndex].DateTime;

            // 计算标记对应秒数
            var markSecond = markTime.Hour * 3600 + markTime.Minute * 60 + markTime.Second;
            if (markSecond <= 0) markSecond = 1;

            // 跳转到标记
            controller.second = markSecond;
        }

        /// <summary>
        /// 编辑选中的标记
        /// </summary>
        private void updateSelectedMark(string text)
        {
            // 获取当前选中的标记
            var selectedIndex = markListBox.SelectedIndex;

            // 更新标记列表
            {
                // 获取标记队列
                var mark = controller.MarkList[selectedIndex];

                // 修改标记名称
                mark.Name = text;

                // 获取标记时间
                var markTime = mark.DateTime;

                // 计算标记名称长度
                var length = 30 - (Encoding.Default.GetBytes(mark.Name).Length - mark.Name.Length);

                // 更新标记列表
                markListBox.Items[selectedIndex] = mark.Name.PadRight(length) + markTime.ToString("HH:mm:ss.fff");
            }

            // 重绘波形图
            drawEEG();

            #region 绘制时间线

            if (replayTimerLineBeginP != null && replayTimerLineBeginP.X > 0)
            {
                // 防止时间从头开始
                isPaused = true;

                // 跳转定位
                SetTimeLine(replayTimerLineBeginP.X);
            }

            #endregion 绘制时间线

            // 更新标记文件
            updateMarkFile();
        }

        /// <summary>
        /// 删除选中的标记
        /// </summary>
        private void removeSelectedMark()
        {
            // 获取当前选中的标记
            var selectedIndex = markListBox.SelectedIndex;

            // 清除记录的下标
            markListBox.Tag = null;

            // 移除标记列表
            markListBox.Items.RemoveAt(selectedIndex);

            // 移除标记队列
            controller.MarkList.RemoveAt(selectedIndex);

            // 选中下一个标记
            var nextIndex = selectedIndex - 1;
            if (markListBox.Items.Count > 0)
            {
                markListBox.SelectedIndex = nextIndex < 0 ? 0 : nextIndex;
            }

            // 重绘波形图
            drawEEG();

            #region 绘制时间线

            if (replayTimerLineBeginP != null && replayTimerLineBeginP.X > 0)
            {
                // 防止时间从头开始
                isPaused = true;

                // 跳转定位
                SetTimeLine(replayTimerLineBeginP.X);
            }

            #endregion 绘制时间线

            // 更新标记文件
            updateMarkFile();
        }

        /// <summary>
        /// 添加标记
        /// </summary>
        private void addMark(string text)
        {
            // 获取标记时间
            double markSeconds = controller.second - 1;
            {
                if (replayTimerLineBeginP != null && replayTimerLineBeginP.X > 0)
                {
                    markSeconds = markSeconds + (replayTimerLineBeginP.X / controller.pixelsPerSecond);
                }
            }

            // 计算标记日期
            var markTime = controller.replayFile.Header.StartDateTime;

            // 追加秒数以校准日期
            markTime = markTime.AddSeconds(markSeconds);

            // 更新时间
            markTime = new DateTime(markTime.Year, markTime.Month, markTime.Day, 0, 0, 0);
            markTime = markTime.AddSeconds(markSeconds);

            // 生成标记
            var mark = new Mark(text, markTime);

            // 插入列表
            {
                var added = false;

                for (int i = 0; i < controller.MarkList.Count; i++)
                {
                    if (controller.MarkList[i].DateTime > markTime)
                    {
                        // 插入列表
                        {
                            // 计算标记名称长度
                            var length = 30 - (Encoding.Default.GetBytes(mark.Name).Length - mark.Name.Length);

                            // 插入标记列表
                            markListBox.Items.Insert(i, mark.Name.PadRight(length) + markTime.ToString("HH:mm:ss.fff"));
                        }

                        // 插入队列
                        controller.MarkList.Insert(i, mark);

                        // 标记插入完毕
                        added = true;
                        break;
                    }
                }

                // 清除记录的下标
                markListBox.Tag = null;

                // 加入末尾
                if (added == false)
                {
                    // 插入列表
                    {
                        // 计算标记名称长度
                        var length = 30 - (Encoding.Default.GetBytes(mark.Name).Length - mark.Name.Length);

                        // 插入标记列表
                        markListBox.Items.Add(mark.Name.PadRight(length) + markTime.ToString("HH:mm:ss.fff"));
                    }

                    // 插入队列
                    controller.MarkList.Add(mark);
                }
            }

            // 重绘波形图
            drawEEG();

            #region 绘制时间线

            if (replayTimerLineBeginP != null && replayTimerLineBeginP.X > 0)
            {
                // 防止时间从头开始
                isPaused = true;

                // 跳转定位
                SetTimeLine(replayTimerLineBeginP.X);
            }

            #endregion 绘制时间线

            // 更新标记文件
            updateMarkFile();
        }

        /// <summary>
        /// 更新标记文件
        /// </summary>
        private void updateMarkFile()
        {
            controller.SaveMark();
        }

        #endregion 标记信息功能

        #region 20171201 功率谱地形图

        /// <summary>
        /// 功率谱
        /// </summary>
        private void 功率谱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 声明起止时间
            int start = 0, end = 0;

            // 获取选中范围
            if (GetSelectedRange(ref start, ref end))
            {
                // 初始化进度条窗口
                if (myProgressForm == null || myProgressForm.IsDisposed)
                {
                    myProgressForm = new FormProgress(this, "正在处理功率谱数据，请稍后...");
                }

                // 初始化代理，在其他线程中异步调用
                SetProgressFormValue = new progressDelegate(myProgressForm.setProgressBarValue);

                // 在其他线程计算功率谱数据
                GLPThreadRunParams param = new GLPThreadRunParams(start, end);
                Thread dataThread = new Thread(new ParameterizedThreadStart(GenerateGonglvpuDataWithThread));
                dataThread.Start(param);

                // 显示进度条
                myProgressForm.ShowDialog();
            }
        }

        /// <summary>
        /// 地形图
        /// </summary>
        private void 地形图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 获取选中范围
            if (GetSelectedRange(ref beginIndexOfMatlab, ref endIndexOfMatlab))
            {
                // 初始化进度条窗口
                if (myProgressForm == null || myProgressForm.IsDisposed)
                {
                    myProgressForm = new FormProgress(this, "正在处理地形图数据，请稍后...");
                }

                // 初始化代理，在其他线程中异步调用
                SetProgressFormValue = new progressDelegate(myProgressForm.setProgressBarValue);

                // 在其他线程计算功率谱数据
                GLPThreadRunParams param = new GLPThreadRunParams(beginIndexOfMatlab, endIndexOfMatlab);
                Thread dataThread = new Thread(new ParameterizedThreadStart(DrawDixingtuWithThread));
                dataThread.Start(param);

                // 显示进度条
                myProgressForm.ShowDialog();
            }
        }

        /// <summary>
        /// 获取选中范围
        /// </summary>
        private bool GetSelectedRange(ref int start, ref int end)
        {
            // 未打开文件
            if (controller.replayFile == null)
            {
                MessageBox.Show("请先打开脑电文件！");
                return false;
            }

            // 未选择时间
            if (replayTimerLineEndP.IsEmpty)
            {
                MessageBox.Show("请按住CTRL键拖动选择需要分析的范围");
                return false;
            }

            // 获取当前页起始时间
            var second = controller.second - 1;

            // 获取每秒像素数
            var pixelPerSecond = (float)controller.pixelsPerSecond;

            // 获取每秒点数
            var simplePerSecond = controller.numOfSamplesPerSecond;

            // 获取选中范围
            var startP = Math.Min(replayTimerLineBeginP.X, replayTimerLineEndP.X);
            {
                startP = startP < 0 ? 0 : startP;
                startP = startP / pixelPerSecond + second;
            }
            var endP = Math.Max(replayTimerLineBeginP.X, replayTimerLineEndP.X);
            {
                endP = endP > eegPanel.Width ? eegPanel.Width : endP;
                endP = endP / pixelPerSecond + second;
            }

            // 至少分析两秒数据
            if (endP - startP < 2)
            {
                MessageBox.Show("请选择两秒以上数据进行分析！");
                return false;
            }

            // 传递结果
            start = (int)(startP * simplePerSecond);
            end = (int)(endP * simplePerSecond);

            // 验证通过
            return true;
        }

        private FormGonglvpu myGonglvpuForm; // 绘制功率谱窗口

        private FormDixingtu myDixingtuForm; // 展示地形图窗口
        private string _dixingtuImageFilePath = @"地形图"; // 地形图图片文件路径

        private List<double[,]> _gonglvpuData = new List<double[,]>(); // 计算后的功率谱数据

        private delegate void progressDelegate(int value); // 代理定义，可以在invoke中传入参数，处理进度条显示

        private progressDelegate SetProgressFormValue = null; // 代理方法，控制进度条窗口的显示

        private FormProgress myProgressForm; // 进度条窗口

        private int beginIndexOfMatlab = 0;
        private int endIndexOfMatlab = 0;

        #region 功率谱

        /// <summary>
        /// 在其他线程处理功率谱数据
        /// </summary>
        /// <param name="obj">一个对象含多个参数</param>
        private void GenerateGonglvpuDataWithThread(Object obj)
        {
            GLPThreadRunParams param = (GLPThreadRunParams)obj;
            GenerateGonglvpuData(param.beginIndex, param.endIndex);
        }

        /// <summary>
        /// 计算生成功率谱数据
        /// </summary>
        /// <param name="bindex">起始位置</param>
        /// <param name="eindex">结束位置</param>
        private void GenerateGonglvpuData(int bindex, int eindex)
        {
            _gonglvpuData.Clear();
            this.Invoke(this.SetProgressFormValue, new Object[] { 0 });

            // 实际通道数量，edf文件可能少于25，edfx文件都是25
            int numberOfChannelsInFact = controller.filterData.GetLength(0);
            if (numberOfChannelsInFact > 20)
            {
                numberOfChannelsInFact = 20;//限制功率谱变换的通道数量,20170412,wdp
            }
            double[][] filterDataForGonglvpu = updateFilterDataForGonglvpu();

            for (int i = 0; i < numberOfChannelsInFact; i++)
            {
                double[] source = new double[eindex - bindex];
                for (int j = bindex; j < eindex; j++)
                {
                    source[j - bindex] = filterDataForGonglvpu[i][j] * controller.generateDW(i);
                    //source[i] = edfFile.MyDataRecords[i][3];
                }

                MWNumericArray arg0 = (MWNumericArray)source;
                MWNumericArray morder = 15;
                MWNumericArray mNfft = source.Length;
                MWNumericArray mFs = 500;
                MWArray[] inPSD = new MWArray[] { arg0, morder, mNfft, mFs };
                MWArray[] outPSD = new MWArray[1];

                // plotFig.plotFigClass plotFig = new plotFig.plotFigClass();
                testPburgPrj.testPburgClass testPburg = new testPburgPrj.testPburgClass();

                // 计算功率谱
                testPburg.testPburg(1, ref outPSD, inPSD);
                // plotFig.DrawFigure(outPSD[0]);

                MWNumericArray mwarray = (MWNumericArray)outPSD[0].Clone();
                Array array = mwarray.ToVector(MWArrayComponent.Real);
                double[,] darray = new double[2, mwarray.NumberOfElements / 2];
                for (int p = 0; p < mwarray.NumberOfElements / 2; p++)
                {
                    darray[0, p] = (double)array.GetValue(p * 2); // 频率 hz
                    darray[1, p] = (double)array.GetValue(p * 2 + 1); // 强度 dbm
                }

                _gonglvpuData.Add(darray);
                int value = 100 * i / (numberOfChannelsInFact - 1);
                this.Invoke(this.SetProgressFormValue, new Object[] { value });
            }
        }

        /// <summary>
        /// 绘制功率谱时，单独计算功率谱的滤波数据 add by lzy 20170205
        /// 注：1.不要时间常数 2.滤波范围固定0.1-100Hz
        /// </summary>
        private double[][] updateFilterDataForGonglvpu()
        {
            double[][] result;

            if (controller.replayFile == null) return null;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 重新计算通道数，edf格式的通道数可能少于25，edfx的通道数为25
            // filterData = new double[numberOfChannels][];
            result = new double[this.controller.replayFile.Header.Signals.Count][];
            List<ManualResetEvent> mreList = new List<ManualResetEvent>();

            #region edfx格式的滤波

            if (controller.replayFile.MyDataRecords.Count > 0)
            {
                for (int m = 0; m < controller.replayFile.Header.Signals.Count; m++)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
                    mreList.Add(mre);

                    ThreadPool.QueueUserWorkItem(delegate (Object obj)
                    {
                        if (obj is int)
                        {
                            int index = (int)obj;
                            int i = 0;
                            if (index == controller.replayFile.Header.Signals.Count - 1)
                            {
                                i = 0;
                            }
                            else
                            {
                                i = index + 1;
                            }
                            //该信道总共有多少个SAMPLE点
                            var _samplesOfCurrentSignal = controller.replayFile.Header.NumberOfDataRecords * controller.replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;

                            result[index] = new double[_samplesOfCurrentSignal];

                            for (int k = 0; k < _samplesOfCurrentSignal; k++)
                            {
                                result[index][k] = Convert.ToDouble(controller.replayFile.MyDataRecords[k][i]);
                            }

                            // result[i] = firFilter.RCFilter(i, result[i], timeConstant, fs, isSecondOrderRC, lowPassFrequency, isSecondOrderRCLowPass);
                            // result[i] = firFilter.BandpassFilterReplay(i, result[i], is50Hz, isFilter, 0.1, 100, fs);
                            result[index] = controller.firFilter.BandpassFilterReplay(index, result[index], controller.is50Hz, controller.isFilter, controller.lowFreq, controller.highFreq, controller.numOfSamplesPerSecond);

                            mreList[index].Set();
                        }
                    }, m);
                }

                foreach (var temp in mreList)
                {
                    temp.WaitOne();
                }
            }

            #endregion edfx格式的滤波

            #region edf格式的滤波

            if (controller.replayFile.DataRecords.Count > 0)
            {
                // 标准edf文件中，每个信道的数据量可能不同，这里是为了得到各信道数据量的最大值
                int maxNumOfSamples = 0;
                for (int i = 0; i < controller.replayFile.Header.Signals.Count; i++)
                {
                    int tmp = controller.replayFile.Header.NumberOfDataRecords * controller.replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;
                    if (tmp > maxNumOfSamples)
                        maxNumOfSamples = tmp;
                }

                for (int i = 0; i < controller.replayFile.Header.Signals.Count; i++)
                {
                    // 该信道总共有多少个SAMPLE点
                    var _samplesOfCurrentSignal = controller.replayFile.Header.NumberOfDataRecords * controller.replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;

                    ManualResetEvent mre = new ManualResetEvent(false);
                    mreList.Add(mre);
                    ThreadPool.QueueUserWorkItem(delegate (Object obj)
                    {
                        if (obj is int)
                        {
                            int index = (int)obj;
                            result[index] = new double[maxNumOfSamples];

                            // 每个数据块中包含的点的个数
                            // 标准edf文件数据是分块存放的，每块包含一段时间内全部通道的数据
                            int samplesOfBlock = _samplesOfCurrentSignal / controller.replayFile.DataRecords.Count;
                            if (samplesOfBlock == 0) samplesOfBlock = 1;

                            for (int k = 0; k < _samplesOfCurrentSignal; k++)
                            {
                                double value = (double)controller.replayFile.DataRecords[k / samplesOfBlock][controller.replayFile.Header.Signals[index].IndexNumberWithLabel][k % samplesOfBlock];
                                result[index][k] = value;
                            }

                            Debug.WriteLine("滤波处理前：filterData[" + index + "].Length = " + result[index].Length);

                            // result[i] = firFilter.RCFilter(i, result[i], timeConstant, fs, isSecondOrderRC, lowPassFrequency, isSecondOrderRCLowPass);
                            // result[i] = firFilter.BandpassFilterReplay(i, result[i], is50Hz, isFilter, 0.1, 100, fs);
                            result[index] = controller.firFilter.BandpassFilterReplay(index, result[index], controller.is50Hz, controller.isFilter, controller.lowFreq, controller.highFreq, controller.numOfSamplesPerSecond);

                            mreList[index].Set();

                            Debug.WriteLine("滤波处理后：filterData[" + index + "].Length = " + result[index].Length);
                        }
                    }, i);
                }

                foreach (var temp in mreList)
                {
                    temp.WaitOne();
                }
            }

            #endregion edf格式的滤波

            sw.Stop();
            Debug.WriteLine("updateFilterDataForGonglvpu() 耗时： " + sw.ElapsedMilliseconds + " ms");

            return result;
        }

        /// <summary>
        /// 显示功率谱界面
        /// </summary>
        public void ShowGonglvpuForm()
        {
            // 处理功率谱数据线程完成后，打开功率谱窗口
            if (myGonglvpuForm == null || myGonglvpuForm.IsDisposed)
            {
                myGonglvpuForm = new FormGonglvpu(this);
                myGonglvpuForm.ReigisterNeuroControl(this.controller);

                myGonglvpuForm.GonglvpuData = _gonglvpuData;
                myGonglvpuForm.NumberOfChannels = this.controller.replayFile.Header.Signals.Count; // 按实际通道数绘制
                if (this.controller.replayFile.Header.Signals.Count > 20)
                {
                    myGonglvpuForm.NumberOfChannels = 20;//20170412,wdp
                }
                myGonglvpuForm.EDFFile = this.controller.replayFile;
                myGonglvpuForm.initChart();

                // 绘制功率谱数据
                myGonglvpuForm.drawGonglvpu();

                myGonglvpuForm.Show();
                myGonglvpuForm.BringToFront();
            }
        }

        #endregion 功率谱

        #region 地形图

        /// <summary>
        /// 绘制地形图，其他线程
        /// </summary>
        private void DrawDixingtuWithThread(Object obj)
        {
            GLPThreadRunParams param = (GLPThreadRunParams)obj;
            DrawDixingtu(param.beginIndex, param.endIndex);
        }

        /// <summary>
        /// 绘制地形图
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        private void DrawDixingtu(int beginIndex, int endIndex)
        {
            this.Invoke(SetProgressFormValue, new object[] { 0 });

            DXT dxt = new DXT();

            this.Invoke(SetProgressFormValue, new object[] { 20 });

            // 实际通道数：edfx按20通道计算, edf可能不足20，按实际通道数计算
            int numberOfChannelsInFact = !this.controller.replayFile.isEDFXFile() ? this.controller.replayFile.Header.Signals.Count : 20;
            // 画地形图时，不能包含EKG心电通道(即：第1个通道)
            numberOfChannelsInFact--;

            double[][] filterDataForDixingtu = updateFilterDataForDixingtu();
            double[,] source = new double[numberOfChannelsInFact, endIndex - beginIndex];
            for (int i = 0; i < numberOfChannelsInFact; i++)
            {
                for (int j = beginIndex; j < endIndex; j++)
                {
                    source[i, j - beginIndex] = filterDataForDixingtu[i + 1][j] * controller.generateDW(i + 1);
                }
            }

            this.Invoke(SetProgressFormValue, new object[] { 30 });

            MWNumericArray arg0 = (MWNumericArray)source;
            MWNumericArray morder = numberOfChannelsInFact;
            MWNumericArray mNfft = endIndex - beginIndex;
            MWNumericArray mFs = 500;

            this.Invoke(SetProgressFormValue, new object[] { 50 });

            MWArray[] outArray = dxt.calTopoplot(2, arg0, morder, mNfft, mFs);
            MWArray res = outArray[1];

            this.Invoke(SetProgressFormValue, new object[] { 80 });

            _dixingtuImageFilePath = Environment.CurrentDirectory + @"\地形图\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            string dir = Path.GetDirectoryName(_dixingtuImageFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string dixingtuConfigFileName = @"eloc" + numberOfChannelsInFact.ToString() + @".txt";
            Debug.WriteLine("绘制地形图，使用配置文件: " + dixingtuConfigFileName);

            if (!File.Exists(Environment.CurrentDirectory + @"\" + dixingtuConfigFileName))
            {
                MessageBox.Show(dixingtuConfigFileName + " 文件不存在，绘制地形图会出现错误！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dxt.DrawDiXingTu(res, dixingtuConfigFileName, _dixingtuImageFilePath);

            this.Invoke(SetProgressFormValue, new object[] { 100 });
        }

        /// <summary>
        /// 绘制地形图时，单独计算地形图的滤波数据 add by lzy 20170205
        /// 注：不要时间常数
        /// </summary>
        private double[][] updateFilterDataForDixingtu()
        {
            double[][] result;

            if (controller.replayFile == null) return null;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 重新计算通道数，edf格式的通道数可能少于25，edfx的通道数为25
            // filterData = new double[numberOfChannels][];
            result = new double[this.controller.replayFile.Header.Signals.Count][];
            List<ManualResetEvent> mreList = new List<ManualResetEvent>();

            #region edfx格式的滤波

            if (controller.replayFile.MyDataRecords.Count > 0)
            {
                for (int i = 0; i < controller.replayFile.Header.Signals.Count; i++)
                {
                    //该信道总共有多少个SAMPLE点
                    var _samplesOfCurrentSignal = controller.replayFile.Header.NumberOfDataRecords * controller.replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;

                    ManualResetEvent mre = new ManualResetEvent(false);
                    mreList.Add(mre);

                    ThreadPool.QueueUserWorkItem(delegate (Object obj)
                    {
                        if (obj is int)
                        {
                            int index = (int)obj;

                            result[index] = new double[_samplesOfCurrentSignal];
                            for (int k = 0; k < _samplesOfCurrentSignal; k++)
                            {
                                result[index][k] = Convert.ToDouble(controller.replayFile.MyDataRecords[k][index]);
                            }

                            // result[i] = firFilter.RCFilter(i, result[i], timeConstant, fs, isSecondOrderRC, lowPassFrequency, isSecondOrderRCLowPass);
                            result[index] = controller.firFilter.BandpassFilterReplay(index, result[index], controller.is50Hz, controller.isFilter, controller.lowFreq, controller.highFreq, controller.numOfSamplesPerSecond);

                            mreList[index].Set();
                        }
                    }, i);
                }

                foreach (var temp in mreList)
                {
                    temp.WaitOne();
                }
            }

            #endregion edfx格式的滤波

            #region edf格式的滤波

            if (controller.replayFile.DataRecords.Count > 0)
            {
                // 标准edf文件中，每个信道的数据量可能不同，这里是为了得到各信道数据量的最大值
                int maxNumOfSamples = 0;
                for (int i = 0; i < controller.replayFile.Header.Signals.Count; i++)
                {
                    int tmp = controller.replayFile.Header.NumberOfDataRecords * controller.replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;
                    if (tmp > maxNumOfSamples)
                        maxNumOfSamples = tmp;
                }

                for (int i = 0; i < controller.replayFile.Header.Signals.Count; i++)
                {
                    // 该信道总共有多少个SAMPLE点
                    var _samplesOfCurrentSignal = controller.replayFile.Header.NumberOfDataRecords * controller.replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;

                    ManualResetEvent mre = new ManualResetEvent(false);
                    mreList.Add(mre);

                    ThreadPool.QueueUserWorkItem(delegate (Object obj)
                    {
                        if (obj is int)
                        {
                            int index = (int)obj;

                            result[index] = new double[maxNumOfSamples];

                            // 每个数据块中包含的点的个数
                            // 标准edf文件数据是分块存放的，每块包含一段时间内全部通道的数据
                            int samplesOfBlock = _samplesOfCurrentSignal / controller.replayFile.DataRecords.Count;
                            if (samplesOfBlock == 0) samplesOfBlock = 1;

                            for (int k = 0; k < _samplesOfCurrentSignal; k++)
                            {
                                double value = (double)controller.replayFile.DataRecords[k / samplesOfBlock][controller.replayFile.Header.Signals[index].IndexNumberWithLabel][k % samplesOfBlock];
                                result[index][k] = value;
                            }

                            // result[i] = firFilter.RCFilter(i, result[i], timeConstant, fs, isSecondOrderRC, lowPassFrequency, isSecondOrderRCLowPass);
                            result[index] = controller.firFilter.BandpassFilterReplay(index, result[index], controller.is50Hz, controller.isFilter, controller.lowFreq, controller.highFreq, controller.numOfSamplesPerSecond);
                        }
                    }, i);
                }

                foreach (var temp in mreList)
                {
                    temp.WaitOne();
                }
            }

            #endregion edf格式的滤波

            sw.Stop();
            Debug.WriteLine("updateFilterDataForDixingtu() 耗时： " + sw.ElapsedMilliseconds + " ms");

            return result;
        }

        /// <summary>
        /// 展示地形图窗口
        /// </summary>
        public void ShowDixingtuForm()
        {
            if (myDixingtuForm == null || myDixingtuForm.IsDisposed)
            {
                myDixingtuForm = new FormDixingtu(this);
                myDixingtuForm.ReigisterNeuroControl(this.controller);

                myDixingtuForm.DixingtuImageFilePath = _dixingtuImageFilePath;
                myDixingtuForm.BeginIndexOfEDFFile = this.beginIndexOfMatlab;
                myDixingtuForm.EndIndexOfEDFFile = this.endIndexOfMatlab;

                myDixingtuForm.Show();
                myDixingtuForm.BringToFront();
            }
        }

        #endregion 地形图

        #endregion 20171201 功率谱地形图

        #region 20171204 打印波形

        private void 打印波形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 获取选中区间

            double start, end;
            {
                // 未打开文件
                if (controller.replayFile == null)
                {
                    MessageBox.Show("请先打开脑电文件！");
                    return;
                }

                // 非EDFX文件
                if (controller.replayFile.isEDFXFile() == false)
                {
                    MessageBox.Show("当前只支持打印EDFX文件！");
                    return;
                }

                // 获取当前页起始时间
                var second = controller.second - 1;

                // 获取走纸速度
                var secondPerPage = controller.secondsPerPage;

                // 获取每秒像素数
                var pixelPerSecond = (float)controller.pixelsPerSecond;

                // 获取每秒点数
                var simplePerSecond = controller.numOfSamplesPerSecond;

                // 获取选中范围
                {
                    // 未选择时间默认整页
                    if (replayTimerLineEndP.IsEmpty)
                    {
                        start = second;
                        end = second + secondPerPage;
                    }
                    // 获取选中区间
                    else
                    {
                        var startP = Math.Min(replayTimerLineBeginP.X, replayTimerLineEndP.X);
                        {
                            startP = startP < 0 ? 0 : startP;
                            startP = startP / pixelPerSecond + second;
                        }
                        var endP = Math.Max(replayTimerLineBeginP.X, replayTimerLineEndP.X);
                        {
                            endP = endP > eegPanel.Width ? eegPanel.Width : endP;
                            endP = endP / pixelPerSecond + second;
                        }

                        start = startP;
                        end = endP;
                    }
                }
            }

            #endregion 获取选中区间

            // TODO 用旧版的打印方式
            {
                // printEEG
                {
                    if (this.printEEG == null)
                    {
                        printEEG = new PrintDocument();
                        printEEG.BeginPrint += PrintEEG_BeginPrint;
                        printEEG.EndPrint += PrintEEG_EndPrint;
                        printEEG.PrintPage += PrintEEG_PrintPage;

                        printPreviewDialog = new PrintPreviewDialog();
                    }
                }

                _paginationIndex = 0; // 分页位置

                // 设置打印控件属性
                this.printEEG.DocumentName = "EEG波形打印";
                // this.printEEG.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custum", 1920, 1080);
                // 选取纸张 A4
                PaperSize ps = null;
                foreach (PaperSize tmp in this.printEEG.PrinterSettings.PaperSizes)
                {
                    if (tmp.PaperName.Equals("A4"))
                    {
                        ps = tmp;
                        break;
                    }
                }
                if (ps != null) this.printEEG.DefaultPageSettings.PaperSize = ps;

                // 设置纸张横向，true 纸张横向 & false 纸张纵向(默认)
                this.printEEG.DefaultPageSettings.Landscape = true;

                // 设置纸张的页边距为0
                this.printEEG.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                // 打印预览
                this.printPreviewDialog.Document = this.printEEG;

                this.printPreviewDialog.ShowDialog();
            }
        }

        private void PrintEEG_BeginPrint(object sender, PrintEventArgs e)
        {
            _paginationIndex = 0;
        }

        private void PrintEEG_EndPrint(object sender, PrintEventArgs e)
        {
            // true 表示预览时的打印至显示器，false 表示实际打印
            if (this.printEEG.PrintController.IsPreview == false)
            {
                // 实际打印时，关闭预览窗口
                this.printPreviewDialog.Close();
            }
        }

        private void PrintEEG_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            /*
            // 每秒钟的点数
            int samplesPerSecond = (int)(this.controller.replayFile.Header.Signals[0].NumberOfSamplesPerDataRecord / this.controller.replayFile.Header.DurationOfDataRecordInSeconds);
            // 每页的时间 秒数
            double secondsPerPage = this.controller.secondsPerPage;
            // 每页的点数
            int samplesPerPage = (int)(samplesPerSecond * secondsPerPage);
            */

            int samplesPerPage = this.controller.filterData[0].Length; // 当前页总点数
            double secondsPerPage = this.controller.secondsPerPage;
            int samplesPerSecond = (int)(samplesPerPage / secondsPerPage);

            Boolean isPrint = true; // 当前信道是否打印
            Boolean isPrintTest = true; // TEST信道是否打印

            PointF beginP = new PointF(60, 20);
            PointF endP = new PointF(60, 20);
            float lineYvalue = beginP.Y; // 保留每次起始点坐标的Y值，作为每一路波形的参考原点Y坐标
            PointF testBeginP = new PointF(), testEndP = new PointF();

            // 每个点的间距有多少像素
            float interval = 1;
            double pixelPerCM = (this.printEEG.DefaultPageSettings.PaperSize.Height - 100) / 29.7; // 每CM的像素数(A4纸的实际长度是29.7cm)
            double pixelPerSecond = pixelPerCM * this.controller.speed / 10;

            interval = (float)(pixelPerSecond / samplesPerSecond);

            // 打印时，每CM打印的像素数（单位：samplesPerSecond * interval是每秒打印的的像素数，this.getSpeed()是每秒多少CM）
            int pixelPerCMInPrint = (int)(samplesPerSecond * interval / this.controller.speed * 10);

            // 需要打印的通道数，保证打印的每个通道间隔均匀分布
            int numOfPrintChannels = leadLabelList.Count - 1;
            /* 20171206
            int numOfPrintChannels = 0;
            //for (int i = 0; i < this.myEEGForm.GetNumberOfChannels(); i++)
            for (int i = 0; i < this.myEEGForm.GetLeadConfigCount(); i++)
            {
                if (this.GetSignalDisplay(i)) numOfPrintChannels++;
            }
            if (controller.GetDemarcateState()) numOfPrintChannels++;
            */

            // 要打印的通道之间的间隔，即：Y轴的像素数
            int spaceOfPrintChannel = 700 / numOfPrintChannels;

            #region 按导联配置，打印导联差值 modify by lzy 20170122

            // 遍历导联名称
            for (int i = 0; i < leadLabelList.Count; i++)
            {
                // 获取做差的两个电极名称
                string[] FPi_FPj = leadLabelList[i].Text.ToString().Split(new char[] { '-' });
                if (FPi_FPj.Length < 2) continue;

                // 查找做差电极对应的通道号
                int channelNum_Positive = 1;
                int channelNum_Negative = 1;
                // 由通道号对应通道名称的哈希表中读取需要显示的通道号
                foreach (var item in controller.leadSource)
                {
                    if (item.Value.ToString() == FPi_FPj[0])
                        channelNum_Positive = Convert.ToInt32(item.Key);
                    if (item.Value.ToString() == FPi_FPj[1])
                        channelNum_Negative = Convert.ToInt32(item.Key);
                }
                // 两个电极电位值，二者的差值
                double sampleValue_Positive = 0;
                double sampleValue_Negative = 0;
                double sampleDifferValue = 0;

                // 单位 uv
                double dw = controller.generateDW(i);

                // 通道是否显示/打印
                // if (this.GetSignalDisplay(i) == false) continue;
                /*
                isPrint = this.GetSignalDisplay(i);
                isPrintTest = this.GetSignalDisplay(myEEGForm.leadConfigArrayList.Count);
                */

                // 画导联名称
                if (isPrint)
                {
                    e.Graphics.DrawString(leadLabelList[i].Text.ToString(),
                                            new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                            System.Drawing.Brushes.Black,
                                            beginP.X - 50, beginP.Y);
                }
                // 画完最后一个导联后，再打印TEST通道名称
                /*
                if (isPrintTest && controller.GetDemarcateState() && i == myEEGForm.leadConfigArrayList.Count - 1)
                {
                    e.Graphics.DrawString("TEST",
                                        new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                        System.Drawing.Brushes.Black,
                                        beginP.X - 50,
                                        isPrint ? beginP.Y + spaceOfPrintChannel : beginP.Y);
                }
                */

                // 遍历全部数据点
                for (int j = _paginationIndex; j < samplesPerPage; j++)
                {
                    // 信道不显示，则不打印该导联
                    // if (!isPrint) break;

                    // 根据当前页码，换算出实际的数据位置
                    // int index = this.myEEGForm.GetPage() == 0 ? j : j + samplesPerPage * this.myEEGForm.GetPage();
                    // 当前滤波只滤一屏，所以不需要换算
                    int index = j;

                    if (this.controller.replayFile.MyDataRecords.Count > 0)
                    {
                        if (index >= this.controller.replayFile.MyDataRecords.Count) break;
                    }
                    else if (this.controller.replayFile.DataRecords.Count > 0)
                    {
                        if (index >= this.controller.replayFile.DataRecords.Count) break;
                    }

                    // =========== 每秒钟画一条竖线，打印秒数 begin =============
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

                    if (!FPi_FPj[0].Equals("FPz"))
                        sampleValue_Positive = controller.filterData[channelNum_Positive][index];
                    if (!FPi_FPj[1].Equals("FPz"))
                        sampleValue_Negative = controller.filterData[channelNum_Negative][index];
                    sampleDifferValue = sampleValue_Positive - sampleValue_Negative;

                    sampleDifferValue = sampleDifferValue * dw;

                    // =========== 把uv转为像素个数 begin ==================
                    // 单位：myEEGForm.GetSensivity()是uv/cm，pixelPerCMInPrint是每厘米多少像素
                    sampleDifferValue = sampleDifferValue / (this.controller.sensitivity / (float)pixelPerCMInPrint);
                    // =========== 把uv转为像素个数 end ==================

                    endP.X += interval;
                    endP.Y = lineYvalue - (float)sampleDifferValue;

                    if (j == 0) { beginP.X = endP.X; beginP.Y = endP.Y; } // 避免最左侧第一个点的无效连线

                    Pen pen2 = new Pen(Color.Black, (float)0.5);
                    if (isPrint)
                        e.Graphics.DrawLine(pen2, beginP, endP); // 连线

                    /*
                    // 遍历至最后一个导联时，准备打印TEST标定通道
                    if (i == myEEGForm.leadConfigArrayList.Count - 1 && controller.GetDemarcateState() && isPrintTest)
                    {
                        double value = myEEGForm.filterData[myEEGForm.NumberOfDemarcateChannel + 1][j] / myEEGForm.ActualCV * myEEGForm.DemarcateCV;
                        value = value / (this.myEEGForm.GetSensivity() / (float)pixelPerCMInPrint);

                        if (j == _paginationIndex)
                        {
                            testBeginP.X = beginP.X; testBeginP.Y = isPrint ? lineYvalue + spaceOfPrintChannel : lineYvalue;
                            testEndP.X = testBeginP.X; testEndP.Y = testBeginP.Y;
                        }
                        else
                        {
                            testEndP.X += interval;
                            testEndP.Y = (float)((isPrint ? lineYvalue + spaceOfPrintChannel : lineYvalue) - value);
                        }

                        e.Graphics.DrawLine(pen2, testBeginP, testEndP);
                        testBeginP.X = testEndP.X; testBeginP.Y = testEndP.Y;
                    }
                    */

                    beginP.X = endP.X;
                    beginP.Y = endP.Y;

                    // 分页处理，预览两页，但是打印出来只要一页
                    if (i == leadLabelList.Count - 1 && endP.X >= this.printEEG.DefaultPageSettings.PaperSize.Height - 20)
                    {
                        _paginationIndex = j;

                        this.printInfo(beginP, endP, e, pixelPerCMInPrint, interval, samplesPerSecond);

                        // 预览两页，但是打印出来只要一页
                        if (this.printEEG.PrintController.IsPreview)
                        {
                            e.HasMorePages = true;
                            return;
                        }
                    }
                    else if (endP.X >= this.printEEG.DefaultPageSettings.PaperSize.Height - 20)
                    {
                        break;
                    }
                }

                Debug.WriteLine("endP = " + endP + ", lineYvalue = " + lineYvalue);

                // 一个导联数据画完，继续画下一个导联，坐标点移到下一行
                beginP.X = 60; beginP.Y = isPrint ? lineYvalue + spaceOfPrintChannel : lineYvalue;
                endP.X = beginP.X; endP.Y = beginP.Y;
                lineYvalue = endP.Y;
            }

            this.printInfo(beginP, endP, e, pixelPerCMInPrint, interval, samplesPerSecond);
            e.HasMorePages = false;

            #endregion 按导联配置，打印导联差值 modify by lzy 20170122

            #region 原有逻辑，打印edf通道及数据   已注释 by lzy 20170122

            /*
            // 遍历全部通道的数据
            for (int i = 0; i < this.myEEGForm.GetNumberOfChannels(); i++)
            {
                // 通道是否显示/打印
                // if (this.GetSignalDisplay(i) == false) continue;
                isPrint = this.GetSignalDisplay(i);

                // 画每个通道的名称
                if (isPrint)
                    e.Graphics.DrawString(this.myEEGForm.Edffile.Header.Signals[i].Label,
                                          new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                          System.Drawing.Brushes.Black,
                                          beginP.X - 50, beginP.Y);

                // 画出当前页的全部数据点
                for (int j = _paginationIndex; j < samplesPerPage; j++)
                {
                    // 信道不显示，则不打印该通道
                    if (!isPrint) break;

                    // 根据当前页码，换算出实际的数据位置
                    int index = this.myEEGForm.GetPage() == 0 ? j : j + samplesPerPage * this.myEEGForm.GetPage();

                    if (this.myEEGForm.Edffile.MyDataRecords.Count > 0)
                    {
                        if (index >= this.myEEGForm.Edffile.MyDataRecords.Count) break;
                    }
                    else if (this.myEEGForm.Edffile.DataRecords.Count > 0)
                    {
                        if (index >= this.myEEGForm.Edffile.DataRecords.Count) break;
                    }

                    // =========== 每秒钟画一条竖线，打印秒数 begin =============
                    // int currentSec = index % samplesPerSecond < interval ? index / samplesPerSecond : 0;
                    int currentSec = index % samplesPerSecond == 0 ? index / samplesPerSecond : 0;
                    if (currentSec >= 1)
                    {
                        Pen apen = new Pen(Color.Black, (float)0.3);
                        float[] floatDash = {2, 4};
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
                    double data = this.myEEGForm.filterData[i][index]; // 取滤波处理后的数据进行展示
                    if (i == myEEGForm.NumberOfDemarcateChannel + 1)
                    {
                        data = data / Math.Abs(myEEGForm.ActualCV) * myEEGForm.DemarcateCV;
                    }
                    else
                    {
                        data = data * this.myEEGForm.GetGenerateDW(i); // 单位 uv
                    }

                    // =========== 把uv转为像素个数 begin ==================
                    // 单位：myEEGForm.GetSensivity()是uv/cm，pixelPerCMInPrint是每厘米多少像素
                    data = data / (this.myEEGForm.GetSensivity() / (float)pixelPerCMInPrint);
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

                    // 分页处理，下一页继续打印
                    if (i == this.myEEGForm.GetNumberOfChannels() - 1 && endP.X >= this.printEEG.DefaultPageSettings.PaperSize.Height - 20)
                    {
                        _paginationIndex = j;

                        this.printInfo(beginP, endP, e, pixelPerCMInPrint, interval, samplesPerSecond);

                        e.HasMorePages = true;
                        return;
                    }
                    else if (endP.X >= this.printEEG.DefaultPageSettings.PaperSize.Height - 20)
                    {
                        break;
                    }
                }

                Debug.WriteLine("endP = " + endP + ", lineYvalue = " + lineYvalue);

                beginP.X = 60; beginP.Y = isPrint ? lineYvalue + spaceOfPrintChannel : lineYvalue;
                endP.X = beginP.X; endP.Y = beginP.Y;
                lineYvalue = endP.Y;
            }

            this.printInfo(beginP, endP, e, pixelPerCMInPrint, interval, samplesPerSecond);
            e.HasMorePages = false;
            */

            #endregion 原有逻辑，打印edf通道及数据   已注释 by lzy 20170122
        }

        /// <summary>
        /// 打印其他信息
        /// </summary>
        /// <param name="beginP"></param>
        /// <param name="endP"></param>
        /// <param name="e"></param>
        /// <param name="pixelPerCMInPrint"></param>
        /// <param name="interval"></param>
        /// <param name="samplesPerSecond"></param>
        private void printInfo(PointF beginP, PointF endP, PrintPageEventArgs e, int pixelPerCMInPrint, float interval, int samplesPerSecond)
        {
            #region 波形图和文字区域的分割线

            beginP.X = 20; beginP.Y = 730;
            endP.X = this.printEEG.DefaultPageSettings.PaperSize.Height - 20; endP.Y = 730;
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
            /*
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
            */

            // 打印灵敏度
            point.X = 20;
            point.Y += 12;
            e.Graphics.DrawString("灵敏度：" + this.controller.sensitivity + "uv/cm",
                                    new Font(new FontFamily("宋体"), 8, FontStyle.Regular),
                                    System.Drawing.Brushes.Black,
                                    point.X, point.Y);

            // 打印走纸速度
            point.X += 150;
            e.Graphics.DrawString("走纸速度：" + this.controller.speed + "mm/s",
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

        /// <summary>
        /// 打印分页时的每页数据的起始位置
        /// </summary>
        private int _paginationIndex = 0;

        /// <summary>
        /// 打印控件
        /// </summary>
        private PrintDocument printEEG;

        /// <summary>
        /// 打印预览
        /// </summary>
        private PrintPreviewDialog printPreviewDialog;

        #endregion 20171204 打印波形

        #region 20171205 测量

        private void 测量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 非空验证
            if (controller.replayFile == null) return;

            // 切换测量模式
            测量ToolStripMenuItem.Checked = InMeasurementMode = !InMeasurementMode;

            // 重置测量点
            MPQ = new Point[2];

            // 重绘波形图
            drawEEG();
        }

        /// <summary>
        /// 测量模式
        /// </summary>
        private bool InMeasurementMode = false;

        /// <summary>
        /// 测量路径
        /// </summary>
        private Point[] MPQ;

        /// <summary>
        /// 测量提示
        /// </summary>
        private ToolTip MT;

        private void 打开文件测试ToolStripMenuItem_Click(object sender, EventArgs e){
            //MessageBox.Show("Plese Open File");
            //this.openFile2(); 
        }

        private void toolStripMenuItem_Help_Click(object sender, EventArgs e)
        {

        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFile2();
        }

        /// <summary>
        /// 绘制测量区域
        /// </summary>
        private void DrawMeasurementArea()
        {
            using (var cBitMap = (Bitmap)replayTimerCache.Clone())
            using (var bb = new SolidBrush(Color.Blue))
            using (var pe = new Pen(bb))
            {
                using (replayTimerGraphics = Graphics.FromImage(cBitMap))
                {
                    if (MPQ[0].Equals(MPQ[1]) == false)
                    {
                        if (MPQ[0] != null)
                        {
                            pe.DashStyle = DashStyle.Dash;

                            replayTimerGraphics.DrawLine(
                                pe,
                                MPQ[0].X, 0,
                                MPQ[0].X, cBitMap.Height);
                            replayTimerGraphics.DrawLine(
                                pe,
                                0, MPQ[0].Y,
                                cBitMap.Width, MPQ[0].Y);
                        }

                        if (MPQ[1] != null)
                        {
                            pe.DashStyle = DashStyle.Dot;

                            replayTimerGraphics.DrawLine(
                                pe,
                                MPQ[1].X, 0,
                                MPQ[1].X, cBitMap.Height);
                            replayTimerGraphics.DrawLine(
                                pe,
                                0, MPQ[1].Y,
                                cBitMap.Width, MPQ[1].Y);
                        }

                        // 提示
                        {
                            if (MT == null)
                            {
                                MT = new ToolTip();

                                eegPanel.MouseLeave += (sender, e) =>
                                {
                                    MT.RemoveAll();
                                };
                            }

                            if ((MPQ[0] != null) && (MPQ[1] != null))
                            {
                                // X差值：时间
                                var xd = Math.Round(controller.secondsPerPage * ((double)(Math.Abs(MPQ[0].X - MPQ[1].X)) / eegPanel.Width), 3) + "s";
                                // Y差值：uv
                                var yd = Math.Round(Math.Abs(MPQ[0].Y - MPQ[1].Y) / controller.pixelsPerCM * controller.sensitivity, 3) + "uv";

                                MT.SetToolTip(eegPanel, $"X差值：{xd}\nY差值：{yd}");
                            }
                        }
                    }

                    using (Graphics g = this.eegPanel.CreateGraphics())
                    {
                        g.DrawImage(cBitMap, 0, 0);
                    }
                }
            }
        }

        #endregion 20171205 测量
    }
}