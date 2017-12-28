namespace EEGReplay
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.mnsMain = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_config = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_calibrate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Filter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.测量ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.功率谱ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.地形图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打印波形ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbMark = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_sensitivity = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel11 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_LowFreq = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_HighFreq = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel9 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_speed = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_zoomUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_zoomDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_perSecond = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_nextSecond = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_prePage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_nextPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_play = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_pause = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_rate = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_leadConfig = new System.Windows.Forms.ToolStripComboBox();
            this.eegSpcMain = new System.Windows.Forms.SplitContainer();
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.timerReplay = new System.Windows.Forms.Timer(this.components);
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel_timeStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel_durationStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tsMarkInsert = new System.Windows.Forms.ToolStrip();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnsMain.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eegSpcMain)).BeginInit();
            this.eegSpcMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnsMain
            // 
            this.mnsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_File,
            this.toolStripMenuItem_config,
            this.tsmAdvanced,
            this.toolStripMenuItem_Help,
            this.openFileToolStripMenuItem});
            this.mnsMain.Location = new System.Drawing.Point(0, 0);
            this.mnsMain.Name = "mnsMain";
            this.mnsMain.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mnsMain.Size = new System.Drawing.Size(1350, 24);
            this.mnsMain.TabIndex = 0;
            this.mnsMain.Text = "menuStrip1";
            // 
            // toolStripMenuItem_File
            // 
            this.toolStripMenuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Open,
            this.toolStripMenuItem_Exit});
            this.toolStripMenuItem_File.Name = "toolStripMenuItem_File";
            this.toolStripMenuItem_File.Size = new System.Drawing.Size(44, 24);
            this.toolStripMenuItem_File.Text = "文件";
            // 
            // toolStripMenuItem_Open
            // 
            this.toolStripMenuItem_Open.Name = "toolStripMenuItem_Open";
            this.toolStripMenuItem_Open.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem_Open.Text = "打开";
            this.toolStripMenuItem_Open.Click += new System.EventHandler(this.toolStripMenuItem_Open_Click);
            // 
            // toolStripMenuItem_Exit
            // 
            this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
            this.toolStripMenuItem_Exit.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem_Exit.Text = "退出";
            // 
            // toolStripMenuItem_config
            // 
            this.toolStripMenuItem_config.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_calibrate,
            this.toolStripMenuItem_Filter});
            this.toolStripMenuItem_config.Name = "toolStripMenuItem_config";
            this.toolStripMenuItem_config.Size = new System.Drawing.Size(44, 24);
            this.toolStripMenuItem_config.Text = "配置";
            // 
            // toolStripMenuItem_calibrate
            // 
            this.toolStripMenuItem_calibrate.Name = "toolStripMenuItem_calibrate";
            this.toolStripMenuItem_calibrate.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem_calibrate.Text = "屏幕校准";
            this.toolStripMenuItem_calibrate.Click += new System.EventHandler(this.toolStripMenuItem_calibrate_Click);
            // 
            // toolStripMenuItem_Filter
            // 
            this.toolStripMenuItem_Filter.Name = "toolStripMenuItem_Filter";
            this.toolStripMenuItem_Filter.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem_Filter.Text = "滤波设置";
            this.toolStripMenuItem_Filter.Click += new System.EventHandler(this.toolStripMenuItem_Filter_Click);
            // 
            // tsmAdvanced
            // 
            this.tsmAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.测量ToolStripMenuItem,
            this.功率谱ToolStripMenuItem,
            this.地形图ToolStripMenuItem,
            this.打印波形ToolStripMenuItem});
            this.tsmAdvanced.Name = "tsmAdvanced";
            this.tsmAdvanced.Size = new System.Drawing.Size(44, 24);
            this.tsmAdvanced.Text = "功能";
            // 
            // 测量ToolStripMenuItem
            // 
            this.测量ToolStripMenuItem.Name = "测量ToolStripMenuItem";
            this.测量ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.测量ToolStripMenuItem.Text = "测量";
            this.测量ToolStripMenuItem.Click += new System.EventHandler(this.测量ToolStripMenuItem_Click);
            // 
            // 功率谱ToolStripMenuItem
            // 
            this.功率谱ToolStripMenuItem.Name = "功率谱ToolStripMenuItem";
            this.功率谱ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.功率谱ToolStripMenuItem.Text = "功率谱";
            this.功率谱ToolStripMenuItem.Click += new System.EventHandler(this.功率谱ToolStripMenuItem_Click);
            // 
            // 地形图ToolStripMenuItem
            // 
            this.地形图ToolStripMenuItem.Name = "地形图ToolStripMenuItem";
            this.地形图ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.地形图ToolStripMenuItem.Text = "地形图";
            this.地形图ToolStripMenuItem.Click += new System.EventHandler(this.地形图ToolStripMenuItem_Click);
            // 
            // 打印波形ToolStripMenuItem
            // 
            this.打印波形ToolStripMenuItem.Name = "打印波形ToolStripMenuItem";
            this.打印波形ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打印波形ToolStripMenuItem.Text = "打印波形";
            this.打印波形ToolStripMenuItem.Click += new System.EventHandler(this.打印波形ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem_Help
            // 
            this.toolStripMenuItem_Help.Name = "toolStripMenuItem_Help";
            this.toolStripMenuItem_Help.Size = new System.Drawing.Size(44, 24);
            this.toolStripMenuItem_Help.Text = "帮助";
            this.toolStripMenuItem_Help.Click += new System.EventHandler(this.toolStripMenuItem_Help_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripSeparator1,
            this.tsbMark,
            this.toolStripSeparator10,
            this.toolStripLabel1,
            this.toolStripComboBox_sensitivity,
            this.toolStripLabel11,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.toolStripComboBox_LowFreq,
            this.toolStripLabel8,
            this.toolStripComboBox_HighFreq,
            this.toolStripLabel9,
            this.toolStripSeparator3,
            this.toolStripLabel3,
            this.toolStripComboBox_speed,
            this.toolStripLabel10,
            this.toolStripSeparator4,
            this.toolStripButton_zoomUp,
            this.toolStripButton_zoomDown,
            this.toolStripSeparator7,
            this.toolStripButton_perSecond,
            this.toolStripButton_nextSecond,
            this.toolStripButton_prePage,
            this.toolStripButton_nextPage,
            this.toolStripButton_play,
            this.toolStripButton_pause,
            this.toolStripSeparator8,
            this.toolStripLabel7,
            this.toolStripComboBox_rate,
            this.toolStripSeparator5,
            this.toolStripLabel4,
            this.toolStripComboBox_leadConfig});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1350, 47);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(44, 44);
            this.toolStripButtonOpen.Text = "打开";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 47);
            // 
            // tsbMark
            // 
            this.tsbMark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMark.Image = ((System.Drawing.Image)(resources.GetObject("tsbMark.Image")));
            this.tsbMark.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbMark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMark.Name = "tsbMark";
            this.tsbMark.Size = new System.Drawing.Size(44, 44);
            this.tsbMark.Text = "标记";
            this.tsbMark.Click += new System.EventHandler(this.tsbMark_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 44);
            this.toolStripLabel1.Text = "灵敏度";
            // 
            // toolStripComboBox_sensitivity
            // 
            this.toolStripComboBox_sensitivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_sensitivity.Items.AddRange(new object[] {
            "75uv/mm",
            "75uv/mm",
            "75uv/mm",
            "75uv/mm",
            "75uv/mm"});
            this.toolStripComboBox_sensitivity.Name = "toolStripComboBox_sensitivity";
            this.toolStripComboBox_sensitivity.Size = new System.Drawing.Size(75, 47);
            this.toolStripComboBox_sensitivity.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_sensitivity_SelectedIndexChanged);
            // 
            // toolStripLabel11
            // 
            this.toolStripLabel11.Name = "toolStripLabel11";
            this.toolStripLabel11.Size = new System.Drawing.Size(43, 44);
            this.toolStripLabel11.Text = "uv/cm";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(56, 44);
            this.toolStripLabel2.Text = "滤波分析";
            // 
            // toolStripComboBox_LowFreq
            // 
            this.toolStripComboBox_LowFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_LowFreq.Name = "toolStripComboBox_LowFreq";
            this.toolStripComboBox_LowFreq.Size = new System.Drawing.Size(75, 47);
            this.toolStripComboBox_LowFreq.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_LowFreq_SelectedIndexChanged);
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(23, 44);
            this.toolStripLabel8.Text = "Hz";
            // 
            // toolStripComboBox_HighFreq
            // 
            this.toolStripComboBox_HighFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_HighFreq.Name = "toolStripComboBox_HighFreq";
            this.toolStripComboBox_HighFreq.Size = new System.Drawing.Size(75, 47);
            this.toolStripComboBox_HighFreq.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_HighFreq_SelectedIndexChanged);
            // 
            // toolStripLabel9
            // 
            this.toolStripLabel9.Name = "toolStripLabel9";
            this.toolStripLabel9.Size = new System.Drawing.Size(23, 44);
            this.toolStripLabel9.Text = "Hz";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(56, 44);
            this.toolStripLabel3.Text = "走纸速度";
            // 
            // toolStripComboBox_speed
            // 
            this.toolStripComboBox_speed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_speed.Items.AddRange(new object[] {
            "1cm/s",
            "3cm/s",
            "2cm/s"});
            this.toolStripComboBox_speed.Name = "toolStripComboBox_speed";
            this.toolStripComboBox_speed.Size = new System.Drawing.Size(75, 47);
            this.toolStripComboBox_speed.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_speed_SelectedIndexChanged);
            // 
            // toolStripLabel10
            // 
            this.toolStripLabel10.Name = "toolStripLabel10";
            this.toolStripLabel10.Size = new System.Drawing.Size(41, 44);
            this.toolStripLabel10.Text = "mm/s";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripButton_zoomUp
            // 
            this.toolStripButton_zoomUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_zoomUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_zoomUp.Image")));
            this.toolStripButton_zoomUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_zoomUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_zoomUp.Name = "toolStripButton_zoomUp";
            this.toolStripButton_zoomUp.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton_zoomUp.Text = "放大";
            this.toolStripButton_zoomUp.Click += new System.EventHandler(this.toolStripButton_zoomUp_Click);
            // 
            // toolStripButton_zoomDown
            // 
            this.toolStripButton_zoomDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_zoomDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_zoomDown.Image")));
            this.toolStripButton_zoomDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_zoomDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_zoomDown.Name = "toolStripButton_zoomDown";
            this.toolStripButton_zoomDown.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton_zoomDown.Text = "缩小";
            this.toolStripButton_zoomDown.Click += new System.EventHandler(this.toolStripButton_zoomDown_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripButton_perSecond
            // 
            this.toolStripButton_perSecond.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_perSecond.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_perSecond.Image")));
            this.toolStripButton_perSecond.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_perSecond.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_perSecond.Name = "toolStripButton_perSecond";
            this.toolStripButton_perSecond.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton_perSecond.Text = "上一秒";
            this.toolStripButton_perSecond.Click += new System.EventHandler(this.toolStripButton_perSecond_Click);
            // 
            // toolStripButton_nextSecond
            // 
            this.toolStripButton_nextSecond.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_nextSecond.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_nextSecond.Image")));
            this.toolStripButton_nextSecond.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_nextSecond.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_nextSecond.Name = "toolStripButton_nextSecond";
            this.toolStripButton_nextSecond.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton_nextSecond.Text = "下一秒";
            this.toolStripButton_nextSecond.Click += new System.EventHandler(this.toolStripButton_nextSecond_Click);
            // 
            // toolStripButton_prePage
            // 
            this.toolStripButton_prePage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_prePage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_prePage.Image")));
            this.toolStripButton_prePage.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_prePage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_prePage.Name = "toolStripButton_prePage";
            this.toolStripButton_prePage.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton_prePage.Text = "上一页";
            this.toolStripButton_prePage.Click += new System.EventHandler(this.toolStripButton_prePage_Click);
            // 
            // toolStripButton_nextPage
            // 
            this.toolStripButton_nextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_nextPage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_nextPage.Image")));
            this.toolStripButton_nextPage.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_nextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_nextPage.Name = "toolStripButton_nextPage";
            this.toolStripButton_nextPage.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton_nextPage.Text = "下一页";
            this.toolStripButton_nextPage.Click += new System.EventHandler(this.toolStripButton_nextPage_Click);
            // 
            // toolStripButton_play
            // 
            this.toolStripButton_play.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_play.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_play.Image")));
            this.toolStripButton_play.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_play.Name = "toolStripButton_play";
            this.toolStripButton_play.Size = new System.Drawing.Size(23, 44);
            this.toolStripButton_play.Text = "播放";
            this.toolStripButton_play.Click += new System.EventHandler(this.toolStripButton_play_Click);
            // 
            // toolStripButton_pause
            // 
            this.toolStripButton_pause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_pause.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_pause.Image")));
            this.toolStripButton_pause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_pause.Name = "toolStripButton_pause";
            this.toolStripButton_pause.Size = new System.Drawing.Size(23, 44);
            this.toolStripButton_pause.Text = "暂停";
            this.toolStripButton_pause.Click += new System.EventHandler(this.toolStripButton_pause_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(32, 44);
            this.toolStripLabel7.Text = "倍速";
            // 
            // toolStripComboBox_rate
            // 
            this.toolStripComboBox_rate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_rate.Name = "toolStripComboBox_rate";
            this.toolStripComboBox_rate.Size = new System.Drawing.Size(75, 47);
            this.toolStripComboBox_rate.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_rate_SelectedIndexChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 47);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(56, 44);
            this.toolStripLabel4.Text = "导联切换";
            // 
            // toolStripComboBox_leadConfig
            // 
            this.toolStripComboBox_leadConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_leadConfig.Name = "toolStripComboBox_leadConfig";
            this.toolStripComboBox_leadConfig.Size = new System.Drawing.Size(121, 47);
            this.toolStripComboBox_leadConfig.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_leadConfig_SelectedIndexChanged);
            // 
            // eegSpcMain
            // 
            this.eegSpcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eegSpcMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.eegSpcMain.IsSplitterFixed = true;
            this.eegSpcMain.Location = new System.Drawing.Point(0, 0);
            this.eegSpcMain.Margin = new System.Windows.Forms.Padding(0);
            this.eegSpcMain.Name = "eegSpcMain";
            // 
            // eegSpcMain.Panel2
            // 
            this.eegSpcMain.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.eegPanel_Paint);
            this.eegSpcMain.Panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.eegSpcMain_Panel2_MouseDown);
            this.eegSpcMain.Panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.eegSpcMain_Panel2_MouseMove);
            this.eegSpcMain.Panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.eegSpcMain_Panel2_MouseUp);
            this.eegSpcMain.Size = new System.Drawing.Size(1350, 565);
            this.eegSpcMain.SplitterDistance = 80;
            this.eegSpcMain.SplitterWidth = 1;
            this.eegSpcMain.TabIndex = 4;
            // 
            // spcMain
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcMain.IsSplitterFixed = true;
            this.spcMain.Location = new System.Drawing.Point(0, 0);
            this.spcMain.Margin = new System.Windows.Forms.Padding(0);
            this.spcMain.Name = "spcMain";
            this.spcMain.Panel1Collapsed = true;
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.eegSpcMain);
            this.spcMain.Size = new System.Drawing.Size(1350, 565);
            this.spcMain.SplitterDistance = 300;
            this.spcMain.SplitterWidth = 1;
            this.spcMain.TabIndex = 4;
            // 
            // timerReplay
            // 
            this.timerReplay.Tick += new System.EventHandler(this.timerReplay_Tick);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel5.Text = "开始时间：";
            // 
            // toolStripLabel_timeStatus
            // 
            this.toolStripLabel_timeStatus.Name = "toolStripLabel_timeStatus";
            this.toolStripLabel_timeStatus.Size = new System.Drawing.Size(62, 22);
            this.toolStripLabel_timeStatus.Text = "##:##:##";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(109, 22);
            this.toolStripLabel6.Text = "当前时长/总时长：";
            // 
            // toolStripLabel_durationStatus
            // 
            this.toolStripLabel_durationStatus.Name = "toolStripLabel_durationStatus";
            this.toolStripLabel_durationStatus.Size = new System.Drawing.Size(121, 22);
            this.toolStripLabel_durationStatus.Text = "##:##:##/##:##:##";
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel5,
            this.toolStripLabel_timeStatus,
            this.toolStripSeparator6,
            this.toolStripLabel6,
            this.toolStripLabel_durationStatus,
            this.toolStripSeparator9});
            this.toolStrip2.Location = new System.Drawing.Point(3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip2.Size = new System.Drawing.Size(383, 25);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.spcMain);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1350, 565);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 71);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1350, 590);
            this.toolStripContainer1.TabIndex = 5;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // tsMarkInsert
            // 
            this.tsMarkInsert.Location = new System.Drawing.Point(0, 71);
            this.tsMarkInsert.Name = "tsMarkInsert";
            this.tsMarkInsert.Size = new System.Drawing.Size(1350, 25);
            this.tsMarkInsert.TabIndex = 6;
            this.tsMarkInsert.Text = "添加标记";
            this.tsMarkInsert.Visible = false;
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.openFileToolStripMenuItem.Text = "OpenFile";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 661);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.tsMarkInsert);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.mnsMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnsMain;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "杭州妞诺科技-20170918-1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.mnsMain.ResumeLayout(false);
            this.mnsMain.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eegSpcMain)).EndInit();
            this.eegSpcMain.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnsMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_File;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Open;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Help;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Exit;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_sensitivity;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_LowFreq;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_HighFreq;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_speed;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton_zoomUp;
        private System.Windows.Forms.ToolStripButton toolStripButton_zoomDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton toolStripButton_prePage;
        private System.Windows.Forms.ToolStripButton toolStripButton_nextPage;
        private System.Windows.Forms.ToolStripButton toolStripButton_play;
        private System.Windows.Forms.ToolStripButton toolStripButton_pause;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_rate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_config;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_calibrate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Filter;
        private System.Windows.Forms.ToolStripLabel toolStripLabel11;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripLabel toolStripLabel9;
        private System.Windows.Forms.ToolStripLabel toolStripLabel10;
        private System.Windows.Forms.SplitContainer eegSpcMain;
        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.Timer timerReplay;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel_timeStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel_durationStatus;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton toolStripButton_perSecond;
        private System.Windows.Forms.ToolStripButton toolStripButton_nextSecond;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_leadConfig;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton tsbMark;
        private System.Windows.Forms.ToolStrip tsMarkInsert;
        private System.Windows.Forms.ToolStripMenuItem tsmAdvanced;
        private System.Windows.Forms.ToolStripMenuItem 功率谱ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 地形图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打印波形ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 测量ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
    }
}

