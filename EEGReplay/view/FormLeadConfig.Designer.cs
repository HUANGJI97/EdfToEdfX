namespace EEGReplay
{
    partial class FormLeadConfig
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLeadConfig));
            this.eegSpcMain = new System.Windows.Forms.SplitContainer();
            this.spcConfig = new System.Windows.Forms.SplitContainer();
            this.btn_Add = new System.Windows.Forms.Button();
            this.cboList = new System.Windows.Forms.ComboBox();
            this.btn_Mod = new System.Windows.Forms.Button();
            this.lblConfig = new System.Windows.Forms.Label();
            this.btn_Del = new System.Windows.Forms.Button();
            this.dgrdConfig = new System.Windows.Forms.DataGridView();
            this.spcEdit = new System.Windows.Forms.SplitContainer();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Empty = new System.Windows.Forms.Button();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.eegSpcMain)).BeginInit();
            this.eegSpcMain.Panel1.SuspendLayout();
            this.eegSpcMain.Panel2.SuspendLayout();
            this.eegSpcMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcConfig)).BeginInit();
            this.spcConfig.Panel1.SuspendLayout();
            this.spcConfig.Panel2.SuspendLayout();
            this.spcConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spcEdit)).BeginInit();
            this.spcEdit.Panel1.SuspendLayout();
            this.spcEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // eegSpcMain
            // 
            this.eegSpcMain.BackColor = System.Drawing.Color.LightSteelBlue;
            this.eegSpcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eegSpcMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.eegSpcMain.IsSplitterFixed = true;
            this.eegSpcMain.Location = new System.Drawing.Point(0, 0);
            this.eegSpcMain.Name = "eegSpcMain";
            // 
            // eegSpcMain.Panel1
            // 
            this.eegSpcMain.Panel1.Controls.Add(this.spcConfig);
            // 
            // eegSpcMain.Panel2
            // 
            this.eegSpcMain.Panel2.Controls.Add(this.spcEdit);
            this.eegSpcMain.Size = new System.Drawing.Size(764, 861);
            this.eegSpcMain.SplitterDistance = 220;
            this.eegSpcMain.SplitterWidth = 5;
            this.eegSpcMain.TabIndex = 0;
            // 
            // spcConfig
            // 
            this.spcConfig.BackColor = System.Drawing.Color.LightSteelBlue;
            this.spcConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcConfig.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcConfig.IsSplitterFixed = true;
            this.spcConfig.Location = new System.Drawing.Point(0, 0);
            this.spcConfig.Name = "spcConfig";
            this.spcConfig.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcConfig.Panel1
            // 
            this.spcConfig.Panel1.Controls.Add(this.btn_Add);
            this.spcConfig.Panel1.Controls.Add(this.cboList);
            this.spcConfig.Panel1.Controls.Add(this.btn_Mod);
            this.spcConfig.Panel1.Controls.Add(this.lblConfig);
            this.spcConfig.Panel1.Controls.Add(this.btn_Del);
            // 
            // spcConfig.Panel2
            // 
            this.spcConfig.Panel2.Controls.Add(this.dgrdConfig);
            this.spcConfig.Size = new System.Drawing.Size(220, 861);
            this.spcConfig.SplitterDistance = 80;
            this.spcConfig.SplitterWidth = 1;
            this.spcConfig.TabIndex = 2;
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Add.Location = new System.Drawing.Point(12, 45);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(60, 24);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "添加";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // cboList
            // 
            this.cboList.DropDownHeight = 120;
            this.cboList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cboList.FormattingEnabled = true;
            this.cboList.IntegralHeight = false;
            this.cboList.ItemHeight = 17;
            this.cboList.Location = new System.Drawing.Point(75, 12);
            this.cboList.Name = "cboList";
            this.cboList.Size = new System.Drawing.Size(130, 25);
            this.cboList.TabIndex = 2;
            this.cboList.SelectedIndexChanged += new System.EventHandler(this.cboList_SelectedIndexChanged);
            // 
            // btn_Mod
            // 
            this.btn_Mod.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Mod.Location = new System.Drawing.Point(79, 45);
            this.btn_Mod.Name = "btn_Mod";
            this.btn_Mod.Size = new System.Drawing.Size(60, 24);
            this.btn_Mod.TabIndex = 1;
            this.btn_Mod.Text = "编辑";
            this.btn_Mod.UseVisualStyleBackColor = true;
            this.btn_Mod.Click += new System.EventHandler(this.btn_Mod_Click);
            // 
            // lblConfig
            // 
            this.lblConfig.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblConfig.Location = new System.Drawing.Point(12, 12);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(68, 24);
            this.lblConfig.TabIndex = 1;
            this.lblConfig.Text = "配置列表：";
            this.lblConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_Del
            // 
            this.btn_Del.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Del.Location = new System.Drawing.Point(146, 45);
            this.btn_Del.Name = "btn_Del";
            this.btn_Del.Size = new System.Drawing.Size(60, 24);
            this.btn_Del.TabIndex = 2;
            this.btn_Del.Text = "删除";
            this.btn_Del.UseVisualStyleBackColor = true;
            this.btn_Del.Click += new System.EventHandler(this.btn_Del_Click);
            // 
            // dgrdConfig
            // 
            this.dgrdConfig.AllowUserToAddRows = false;
            this.dgrdConfig.AllowUserToDeleteRows = false;
            this.dgrdConfig.AllowUserToResizeColumns = false;
            this.dgrdConfig.AllowUserToResizeRows = false;
            this.dgrdConfig.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgrdConfig.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdConfig.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdConfig.ColumnHeadersHeight = 30;
            this.dgrdConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrdConfig.Location = new System.Drawing.Point(0, 0);
            this.dgrdConfig.MultiSelect = false;
            this.dgrdConfig.Name = "dgrdConfig";
            this.dgrdConfig.ReadOnly = true;
            this.dgrdConfig.RowHeadersVisible = false;
            this.dgrdConfig.RowHeadersWidth = 40;
            this.dgrdConfig.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dgrdConfig.RowTemplate.Height = 30;
            this.dgrdConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdConfig.Size = new System.Drawing.Size(220, 780);
            this.dgrdConfig.TabIndex = 0;
            this.dgrdConfig.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdConfig_CellValueChanged);
            this.dgrdConfig.SelectionChanged += new System.EventHandler(this.dgrdConfig_SelectionChanged);
            // 
            // spcEdit
            // 
            this.spcEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcEdit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcEdit.IsSplitterFixed = true;
            this.spcEdit.Location = new System.Drawing.Point(0, 0);
            this.spcEdit.Margin = new System.Windows.Forms.Padding(0);
            this.spcEdit.Name = "spcEdit";
            this.spcEdit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcEdit.Panel1
            // 
            this.spcEdit.Panel1.Controls.Add(this.btn_Cancel);
            this.spcEdit.Panel1.Controls.Add(this.btn_Empty);
            this.spcEdit.Panel1.Controls.Add(this.btn_Clear);
            this.spcEdit.Panel1.Controls.Add(this.btn_Save);
            this.spcEdit.Panel1.Controls.Add(this.tbName);
            this.spcEdit.Panel1.Controls.Add(this.lblName);
            this.spcEdit.Size = new System.Drawing.Size(539, 861);
            this.spcEdit.SplitterDistance = 80;
            this.spcEdit.SplitterWidth = 1;
            this.spcEdit.TabIndex = 0;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Cancel.Location = new System.Drawing.Point(213, 45);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(60, 24);
            this.btn_Cancel.TabIndex = 8;
            this.btn_Cancel.Text = "退出";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Empty
            // 
            this.btn_Empty.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Empty.Location = new System.Drawing.Point(12, 45);
            this.btn_Empty.Name = "btn_Empty";
            this.btn_Empty.Size = new System.Drawing.Size(60, 24);
            this.btn_Empty.TabIndex = 5;
            this.btn_Empty.Text = "清除";
            this.btn_Empty.UseVisualStyleBackColor = true;
            this.btn_Empty.Click += new System.EventHandler(this.btn_Empty_Click);
            // 
            // btn_Clear
            // 
            this.btn_Clear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Clear.Location = new System.Drawing.Point(79, 45);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(60, 24);
            this.btn_Clear.TabIndex = 6;
            this.btn_Clear.Text = "清空";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Save.Location = new System.Drawing.Point(146, 45);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(60, 24);
            this.btn_Save.TabIndex = 7;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.Location = new System.Drawing.Point(75, 13);
            this.tbName.MaxLength = 10;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(130, 23);
            this.tbName.TabIndex = 4;
            this.tbName.WordWrap = false;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblName.Location = new System.Drawing.Point(12, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(68, 24);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "配置名称：";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormLeadConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 861);
            this.Controls.Add(this.eegSpcMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormLeadConfig";
            this.Text = "导联配置";
            this.eegSpcMain.Panel1.ResumeLayout(false);
            this.eegSpcMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.eegSpcMain)).EndInit();
            this.eegSpcMain.ResumeLayout(false);
            this.spcConfig.Panel1.ResumeLayout(false);
            this.spcConfig.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcConfig)).EndInit();
            this.spcConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdConfig)).EndInit();
            this.spcEdit.Panel1.ResumeLayout(false);
            this.spcEdit.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcEdit)).EndInit();
            this.spcEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer eegSpcMain;
        private System.Windows.Forms.SplitContainer spcConfig;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.ComboBox cboList;
        private System.Windows.Forms.Button btn_Mod;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.Button btn_Del;
        private System.Windows.Forms.DataGridView dgrdConfig;
        private System.Windows.Forms.SplitContainer spcEdit;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btn_Empty;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Cancel;
    }
}