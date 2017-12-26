namespace EEGReplay
{
    partial class FormFilterConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_TimeConstant = new System.Windows.Forms.ComboBox();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxOrder = new System.Windows.Forms.CheckBox();
            this.checkBoxIs50Hz = new System.Windows.Forms.CheckBox();
            this.checkBoxIsFiler = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "时间常数(0表示无)：";
            // 
            // comboBox_TimeConstant
            // 
            this.comboBox_TimeConstant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TimeConstant.FormattingEnabled = true;
            this.comboBox_TimeConstant.Location = new System.Drawing.Point(137, 6);
            this.comboBox_TimeConstant.Name = "comboBox_TimeConstant";
            this.comboBox_TimeConstant.Size = new System.Drawing.Size(70, 20);
            this.comboBox_TimeConstant.TabIndex = 1;
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Location = new System.Drawing.Point(14, 226);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(75, 23);
            this.buttonConfirm.TabIndex = 2;
            this.buttonConfirm.Text = "确定";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(213, 226);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxOrder
            // 
            this.checkBoxOrder.AutoSize = true;
            this.checkBoxOrder.Location = new System.Drawing.Point(213, 9);
            this.checkBoxOrder.Name = "checkBoxOrder";
            this.checkBoxOrder.Size = new System.Drawing.Size(48, 16);
            this.checkBoxOrder.TabIndex = 4;
            this.checkBoxOrder.Text = "二阶";
            this.checkBoxOrder.UseVisualStyleBackColor = true;
            // 
            // checkBoxIs50Hz
            // 
            this.checkBoxIs50Hz.AutoSize = true;
            this.checkBoxIs50Hz.Location = new System.Drawing.Point(14, 42);
            this.checkBoxIs50Hz.Name = "checkBoxIs50Hz";
            this.checkBoxIs50Hz.Size = new System.Drawing.Size(72, 16);
            this.checkBoxIs50Hz.TabIndex = 5;
            this.checkBoxIs50Hz.Text = "50Hz陷波";
            this.checkBoxIs50Hz.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsFiler
            // 
            this.checkBoxIsFiler.AutoSize = true;
            this.checkBoxIsFiler.Location = new System.Drawing.Point(14, 64);
            this.checkBoxIsFiler.Name = "checkBoxIsFiler";
            this.checkBoxIsFiler.Size = new System.Drawing.Size(48, 16);
            this.checkBoxIsFiler.TabIndex = 6;
            this.checkBoxIsFiler.Text = "滤波";
            this.checkBoxIsFiler.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "50Hz带阻频宽：";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "3",
            "5",
            "10",
            "15"});
            this.comboBox1.Location = new System.Drawing.Point(107, 93);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(70, 20);
            this.comboBox1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(183, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Hz";
            // 
            // FormFilterConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 261);
            this.Controls.Add(this.checkBoxIsFiler);
            this.Controls.Add(this.checkBoxIs50Hz);
            this.Controls.Add(this.checkBoxOrder);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConfirm);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.comboBox_TimeConstant);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFilterConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "滤波设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_TimeConstant;
        private System.Windows.Forms.Button buttonConfirm;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxOrder;
        private System.Windows.Forms.CheckBox checkBoxIs50Hz;
        private System.Windows.Forms.CheckBox checkBoxIsFiler;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
    }
}