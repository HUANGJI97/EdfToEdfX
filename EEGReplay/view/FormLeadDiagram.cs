using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EEGReplay
{
    public partial class FormLeadDiagram : Form
    {
        /// <summary>
        /// 暴露按钮控件方便外部调用
        /// </summary>
        public Dictionary<String, Button> dicButtons;

        /// <summary>
        /// 图示面板
        /// </summary>
        private Panel pnlLeadSource;

        public FormLeadDiagram()
        {
            InitializeComponent();

            // 定位图示面板
            pnlLeadSource = this.eegSpcMain.Panel2;

            // 整理按钮列表
            dicButtons = new Dictionary<string, Button>();
            foreach (Control item in pnlLeadSource.Controls)
            {
                if (item.Name.StartsWith("btn_"))
                {
                    dicButtons.Add(item.Name.Replace("btn_", ""), item as Button);
                }
            }
        }

        /// <summary>
        /// 载入导联源
        /// </summary>
        public void LoadLeadSource(Dictionary<int, String> dicLeadSource)
        {
            Label lblLeadSource;
            foreach (var item in dicLeadSource)
            {
                if (dicButtons[item.Value] != null)
                {
                    lblLeadSource = new Label();
                    lblLeadSource.Size = new Size(42, 20);
                    lblLeadSource.Font = new Font("Microsoft YaHei", 10);
                    lblLeadSource.TextAlign = ContentAlignment.MiddleCenter;
                    lblLeadSource.Text = item.Key.ToString();
                    lblLeadSource.Location = new Point(dicButtons[item.Value].Location.X, dicButtons[item.Value].Location.Y - 25);

                    pnlLeadSource.Controls.Add(lblLeadSource);
                }
            }
        }
    }
}