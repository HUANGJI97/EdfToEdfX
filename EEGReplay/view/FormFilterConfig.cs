using Filter;
using System;
using System.Windows.Forms;

namespace EEGReplay
{
    public partial class FormFilterConfig : Form
    {
        private FormMain formMain;

        private ReplayController controller;

        public FormFilterConfig(FormMain formMain, ReplayController controller)
        {
            InitializeComponent();

            this.formMain = formMain;
            this.controller = controller;

            // 时间常数范围
            this.comboBox_TimeConstant.Items.Clear();
            foreach (float value in this.controller.timeConstantList)
            {
                this.comboBox_TimeConstant.Items.Add(value.ToString());
            }
            this.comboBox_TimeConstant.Text = this.controller.timeConstant.ToString();

            this.checkBoxOrder.Checked = this.controller.isSecondOrderRC;

            this.checkBoxIs50Hz.Checked = this.controller.is50Hz;

            this.checkBoxIsFiler.Checked = this.controller.isFilter;

            // 20171128
            this.comboBox1.Text = FirFilter.trapWaveEdge.ToString();
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
            // 20171128
            FirFilter.trapWaveEdge = int.Parse(this.comboBox1.Text);

            this.controller.isSecondOrderRC = this.checkBoxOrder.Checked;
            this.controller.is50Hz = this.checkBoxIs50Hz.Checked;
            this.controller.isFilter = this.checkBoxIsFiler.Checked;
            this.controller.timeConstant = float.Parse(this.comboBox_TimeConstant.Text);

            this.Close();
        }
    }
}