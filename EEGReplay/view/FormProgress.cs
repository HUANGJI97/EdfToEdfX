using System.Windows.Forms;

namespace EEGReplay
{
    /// <summary>
    /// 进度条窗口
    /// </summary>
    public partial class FormProgress : Form
    {
        private FormMain myEEGForm;

        public FormProgress()
        {
            InitializeComponent();
        }

        public FormProgress(FormMain form)
        {
            InitializeComponent();

            this.myEEGForm = form;
        }

        public FormProgress(FormMain form, string title)
        {
            InitializeComponent();

            this.myEEGForm = form;
            this.label1.Text = title;
        }

        public void setProgressBarValue(int value)
        {
            value = value > this.progressBar1.Maximum ? this.progressBar1.Maximum : value;
            this.progressBar1.Value = value;

            if (value == this.progressBar1.Maximum)
            {
                if (this.label1.Text.IndexOf("功率谱") > 0)
                {
                    myEEGForm.ShowGonglvpuForm();
                }

                if (this.label1.Text.IndexOf("地形图") > 0)
                {
                    myEEGForm.ShowDixingtuForm();
                }

                this.Close();
                this.Dispose();
            }
        }
    }
}