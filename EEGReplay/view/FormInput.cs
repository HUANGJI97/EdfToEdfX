using System;
using System.Windows.Forms;

namespace EEGReplay.view
{
    public partial class FormInput : Form
    {
        private FormInput()
        {
            InitializeComponent();
        }

        public FormInput(string title, string text, Action<string> callback)
            : this()
        {
            txtTitle.Text = title;

            txtInput.Text = text;

            txtInput.PreviewKeyDown += (sender, e) =>
            {
                // 退出
                if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }

                // 确认
                if (e.KeyCode == Keys.Enter)
                {
                    callback(txtInput.Text);
                    Close();
                }
            };
        }
    }
}