using System;
using System.Drawing;
using System.Windows.Forms;

namespace EEGReplay
{
    public partial class FormCalibrate : Form
    {
        private FormMain formMain;

        private ReplayController controller;

        public FormCalibrate(FormMain formMain, ReplayController controller)
        {
            InitializeComponent();

            this.formMain = formMain;
            this.controller = controller;
        }

        /// <summary>
        /// 面板重绘线条长度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.WhiteSmoke);

            Pen pen = new Pen(Color.Black, 5);
            int len = int.Parse(this.numericUpDown1.Value.ToString());
            Point p1 = new Point(10, 20);
            Point p2 = new Point(10 + len, 20);

            e.Graphics.DrawLine(pen, p1, p2);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            this.controller.pixelsPerCM = int.Parse(this.numericUpDown1.Value.ToString()) / 5;
            this.Close();
        }

        /// <summary>
        /// 调整数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.linePanel.Invalidate();
        }
    }
}