namespace EEGReplay
{
    partial class FormDixingtu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDixingtu));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFile = new System.Windows.Forms.Button();
            this.printDXTButton = new System.Windows.Forms.Button();
            this.printDXTPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.printDXTDocument = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(12, 59);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(472, 347);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // openFile
            // 
            this.openFile.Location = new System.Drawing.Point(12, 30);
            this.openFile.Name = "openFile";
            this.openFile.Size = new System.Drawing.Size(111, 23);
            this.openFile.TabIndex = 2;
            this.openFile.Text = "打开地形图文件";
            this.openFile.UseVisualStyleBackColor = true;
            this.openFile.Click += new System.EventHandler(this.openFile_Click);
            // 
            // printDXTButton
            // 
            this.printDXTButton.Location = new System.Drawing.Point(129, 30);
            this.printDXTButton.Name = "printDXTButton";
            this.printDXTButton.Size = new System.Drawing.Size(75, 23);
            this.printDXTButton.TabIndex = 3;
            this.printDXTButton.Text = "打印地形图";
            this.printDXTButton.UseVisualStyleBackColor = true;
            this.printDXTButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.printDXTButton_MouseClick);
            this.printDXTButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.printDXTButton_MouseDown);
            // 
            // printDXTPreviewDialog
            // 
            this.printDXTPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printDXTPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printDXTPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printDXTPreviewDialog.Enabled = true;
            this.printDXTPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printDXTPreviewDialog.Icon")));
            this.printDXTPreviewDialog.Name = "printDXTPreviewDialog";
            this.printDXTPreviewDialog.Visible = false;
            // 
            // FormDixingtu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(496, 418);
            this.Controls.Add(this.printDXTButton);
            this.Controls.Add(this.openFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormDixingtu";
            this.Text = "地形图展示";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.FormDixingtu_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button openFile;
        private System.Windows.Forms.Button printDXTButton;
        private System.Windows.Forms.PrintPreviewDialog printDXTPreviewDialog;
        private System.Drawing.Printing.PrintDocument printDXTDocument;
    }
}