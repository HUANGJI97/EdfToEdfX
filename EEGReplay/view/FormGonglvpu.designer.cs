namespace EEGReplay
{
    partial class FormGonglvpu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGonglvpu));
            this.printGLPButton = new System.Windows.Forms.Button();
            this.printGLPPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.printGLPDocument = new System.Drawing.Printing.PrintDocument();
            this.SuspendLayout();
            // 
            // printGLPButton
            // 
            this.printGLPButton.Location = new System.Drawing.Point(12, 12);
            this.printGLPButton.Name = "printGLPButton";
            this.printGLPButton.Size = new System.Drawing.Size(75, 23);
            this.printGLPButton.TabIndex = 0;
            this.printGLPButton.Text = "打印功率谱";
            this.printGLPButton.UseVisualStyleBackColor = true;
            this.printGLPButton.Click += new System.EventHandler(this.printGLPButton_Click);
            // 
            // printGLPPreviewDialog
            // 
            this.printGLPPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printGLPPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printGLPPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printGLPPreviewDialog.Enabled = true;
            this.printGLPPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printGLPPreviewDialog.Icon")));
            this.printGLPPreviewDialog.Name = "printGLPPreviewDialog";
            this.printGLPPreviewDialog.Visible = false;
            // 
            // FormGonglvpu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.printGLPButton);
            this.Name = "FormGonglvpu";
            this.Text = "功率谱测量";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGonglvpu_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button printGLPButton;
        private System.Windows.Forms.PrintPreviewDialog printGLPPreviewDialog;
        private System.Drawing.Printing.PrintDocument printGLPDocument;
    }
}