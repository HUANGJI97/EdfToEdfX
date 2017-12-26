using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EEGReplay
{
    public partial class FormLeadConfig : Form
    {
        private ReplayController controller;

        private FormLeadDiagram formLeadDiagram;

        private int originWidth;

        public FormLeadConfig(ReplayController controller)
        {
            InitializeComponent();

            this.controller = controller;

            #region 初始化DatagridView

            dgrdConfig.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "编号",
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 60
                }, new DataGridViewTextBoxColumn()
                {
                    HeaderText = "FPI",
                    SortMode = DataGridViewColumnSortMode.NotSortable
                }, new DataGridViewTextBoxColumn()
                {
                    HeaderText = "FPj",
                    SortMode = DataGridViewColumnSortMode.NotSortable
                }
            );

            #endregion 初始化DatagridView

            // 载入配置列表
            LoadLeadConfig();

            // 初始化编辑界面
            InitLeadEditView();

            // 载入导联图示
            LoadLeadDiagram();
        }

        /// <summary>
        /// 载入配置列表
        /// </summary>
        private void LoadLeadConfig()
        {
            this.cboList.Items.Clear();
            this.cboList.Items.AddRange(this.controller.leadConfigDic.Keys.ToArray());
            this.cboList.SelectedIndex = 0;
        }

        /// <summary>
        /// 刷新操作按钮状态
        /// </summary>
        private void RefreshActionButtons()
        {
            string selectedConfig = (String)this.cboList.SelectedItem;

            // 增加
            btn_Add.Enabled = true;

            // 修改
            btn_Mod.Enabled = !(String.IsNullOrEmpty(selectedConfig) || selectedConfig.Equals(ReplayController.DEFAULT_CONFIG_NAME));

            // 删除
            btn_Del.Enabled = !(String.IsNullOrEmpty(selectedConfig) || selectedConfig.Equals(ReplayController.DEFAULT_CONFIG_NAME));
        }

        /// <summary>
        /// 初始化编辑界面
        /// </summary>
        private void InitLeadEditView()
        {
            // 缓存原始宽度
            originWidth = this.Width;

            // 隐藏编辑界面
            HideLeadEditView();
        }

        /// <summary>
        /// 显示编辑界面
        /// </summary>
        private void ShowLeadEditView()
        {
            // 重复性判断
            if (this.eegSpcMain.Panel2Collapsed.Equals(false)) return;

            // 展开界面
            this.eegSpcMain.Panel2Collapsed = false;

            // 调整宽度
            this.Size = new Size(originWidth, this.Size.Height);

            // 防止重复操作
            cboList.Enabled = false;
            btn_Add.Enabled = false;
            btn_Mod.Enabled = false;
            btn_Del.Enabled = false;
        }

        /// <summary>
        /// 隐藏编辑界面
        /// </summary>
        private void HideLeadEditView()
        {
            // 重复性判断
            if (this.eegSpcMain.Panel2Collapsed) return;

            // 计算边框宽度
            int borderWidth = this.Width - this.eegSpcMain.Width;

            // 折叠界面
            this.eegSpcMain.Panel2Collapsed = true;

            // 调整宽度
            this.Size = new Size(this.eegSpcMain.SplitterDistance + borderWidth, this.Size.Height);

            // 启用控件
            cboList.Enabled = true;

            // 刷新操作按钮状态
            RefreshActionButtons();
        }

        /// <summary>
        /// 载入导联图示
        /// </summary>
        private void LoadLeadDiagram()
        {
            formLeadDiagram = new FormLeadDiagram();
            formLeadDiagram.FormBorderStyle = FormBorderStyle.None;
            formLeadDiagram.WindowState = FormWindowState.Maximized;
            formLeadDiagram.TopLevel = false;
            formLeadDiagram.Show();

            foreach (var item in formLeadDiagram.dicButtons)
            {
                item.Value.Click += delegate (object sender, EventArgs e)
                {
                    // 写入单元格内容
                    SetDataGridViewText(item.Key);
                };
            }

            this.spcEdit.Panel2.Controls.Add(formLeadDiagram);
        }

        /// <summary>
        /// 写入单元格内容
        /// </summary>
        /// <param name="key"></param>
        private void SetDataGridViewText(string key)
        {
            // 防止更改编号
            if (dgrdConfig.CurrentCell.ColumnIndex > 0)
            {
                // 清空错误提示
                dgrdConfig.CurrentRow.DefaultCellStyle.BackColor = Color.White;

                // 修改单元格内容
                dgrdConfig.CurrentCell.Value = key;

                // 非清空操作时定位下一个单元格
                if (string.IsNullOrEmpty(key) == false)
                {
                    DataGridViewRow nextMoveRow = dgrdConfig.Rows[dgrdConfig.CurrentCell.RowIndex];
                    DataGridViewCell nextMoveCell = null;

                    if (nextMoveRow.Cells.Count > dgrdConfig.CurrentCell.ColumnIndex + 1)
                    {
                        nextMoveCell = nextMoveRow.Cells[dgrdConfig.CurrentCell.ColumnIndex + 1];
                    }
                    else if (dgrdConfig.Rows.Count > dgrdConfig.CurrentCell.RowIndex + 1)
                    {
                        nextMoveRow = dgrdConfig.Rows[dgrdConfig.CurrentCell.RowIndex + 1];
                        nextMoveCell = nextMoveRow.Cells[1];
                    }

                    // 移动到下一个单元格
                    if (nextMoveCell != null)
                    {
                        nextMoveCell.Selected = true;
                        dgrdConfig.CurrentCell = nextMoveCell;
                    }
                }
            }
        }

        /// <summary>
        /// 配置选择项更改事件
        /// </summary>
        private void cboList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 载入配置数据
            LoadLeadData((sender as ComboBox).Text);
        }

        /// <summary>
        /// 载入配置数据
        /// </summary>
        private void LoadLeadData(string name)
        {
            // 清空旧数据
            dgrdConfig.Rows.Clear();

            // 读取配置
            int _index = 1;
            foreach (var item in this.controller.leadConfigDic[name])
            {
                dgrdConfig.Rows.Add(
                    (new string[] { _index++.ToString() }).Concat(item.Split('-')).ToArray()
                );
            }

            // 刷新操作按钮状态
            RefreshActionButtons();
        }

        /// <summary>
        /// 数据选择项更改事件
        /// </summary>
        private void dgrdConfig_SelectionChanged(object sender, EventArgs e)
        {
            if (dgrdConfig.CurrentCell != null &&
                dgrdConfig.CurrentCell.ColumnIndex == 0)
            {
                dgrdConfig.ClearSelection();
            }
        }

        /// <summary>
        /// 单元格内容更改事件
        /// </summary>
        private void dgrdConfig_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgrdConfig.Rows.Count > 0)
            {
                var lastRow = dgrdConfig.Rows[dgrdConfig.Rows.Count - 1];
                var lastCell = lastRow.Cells[lastRow.Cells.Count - 1];

                if (string.IsNullOrEmpty(lastCell.Value.ToString()) == false)
                {
                    // 插入空列
                    dgrdConfig.Rows.Add(dgrdConfig.Rows.Count + 1, string.Empty, string.Empty);
                }
            }
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        private void btn_Add_Click(object sender, EventArgs e)
        {
            // 清空名称
            tbName.Enabled = true;
            tbName.Text = string.Empty;

            // 清空旧数据
            dgrdConfig.Rows.Clear();

            // 插入空列
            dgrdConfig.Rows.Add(dgrdConfig.Rows.Count + 1, string.Empty, string.Empty);

            // 显示编辑界面
            ShowLeadEditView();
        }

        /// <summary>
        /// 编辑事件
        /// </summary>
        private void btn_Mod_Click(object sender, EventArgs e)
        {
            // 同步名称
            tbName.Enabled = false;
            tbName.Text = cboList.Text;

            // 插入空列
            dgrdConfig.Rows.Add(dgrdConfig.Rows.Count + 1, string.Empty, string.Empty);

            // 显示编辑界面
            ShowLeadEditView();
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        private void btn_Del_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除？", "此删除不可恢复", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 删除配置信息
                this.controller.leadConfigDic.Remove(cboList.Text);

                // 如果是当前使用的配置，重置到默认
                if (this.controller.currentLeadConfigName.Equals(cboList.Text))
                {
                    this.controller.currentLeadConfigName = ReplayController.DEFAULT_CONFIG_NAME;
                }

                // 重新载入配置列表
                LoadLeadConfig();
            }
        }

        /// <summary>
        /// 清除事件
        /// </summary>
        private void btn_Empty_Click(object sender, EventArgs e)
        {
            // 写入单元格内容
            SetDataGridViewText(string.Empty);
        }

        /// <summary>
        /// 清空事件
        /// </summary>
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            // 清空旧数据
            dgrdConfig.Rows.Clear();

            // 插入空列
            dgrdConfig.Rows.Add(dgrdConfig.Rows.Count + 1, string.Empty, string.Empty);
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            // 声明配置列表
            ArrayList arrLead = new ArrayList();

            // 名称判断
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("配置名称不能为空", "提示");
                return;
            }
            else if (tbName.Enabled && this.controller.leadConfigDic.ContainsKey(tbName.Text))
            {
                MessageBox.Show("已经存在相同的配置名称", "提示");
                return;
            }

            // 数据完整性判断
            DataGridViewRow tempRow;
            string tempCell1, tempCell2;
            string strError = string.Empty;
            for (int i = 0; i < dgrdConfig.Rows.Count; i++)
            {
                tempRow = dgrdConfig.Rows[i];
                tempCell1 = tempRow.Cells[1].Value.ToString();
                tempCell2 = tempRow.Cells[2].Value.ToString();

                // 导联配置为空时跳过
                if (string.IsNullOrEmpty(tempCell1) && string.IsNullOrEmpty(tempCell2))
                {
                    continue;
                }
                // 导联配置不完整
                else if (string.IsNullOrEmpty(tempCell1) || string.IsNullOrEmpty(tempCell2))
                {
                    strError = "请正确填写导联配置";
                }
                // 导联电极重复
                else if (tempCell1.Equals(tempCell2))
                {
                    strError = "同一导联中电极不能重复";
                }
                // 导联重复
                else if (arrLead.Contains(tempCell1 + "-" + tempCell2))
                {
                    strError = "不能出现重复导联";
                }

                // 提示错误信息
                if (string.IsNullOrEmpty(strError) == false)
                {
                    // 提示错误位置
                    tempRow.DefaultCellStyle.BackColor = Color.FromArgb(242, 222, 222);

                    // 提示错误信息
                    MessageBox.Show(strError, "错误");

                    return;
                }

                // 加入配置列表
                arrLead.Add(tempCell1 + "-" + tempCell2);
            }

            // 非空验证
            if (arrLead.Count < 1)
            {
                MessageBox.Show("请输入至少一条导联数据", "错误");

                return;
            }

            // 写入配置列表
            if (this.controller.leadConfigDic.ContainsKey(tbName.Text))
            {
                this.controller.leadConfigDic[tbName.Text] = arrLead.Cast<string>().ToList();
            }
            else
            {
                this.controller.leadConfigDic.Add(tbName.Text, arrLead.Cast<string>().ToList());
            }

            // 重载配置列表
            LoadLeadConfig();

            // 定位当前配置
            for (int i = 0; i < cboList.Items.Count; i++)
            {
                if (cboList.Items[i].ToString().Equals(tbName.Text))
                {
                    cboList.SelectedIndex = i;
                    break;
                }
            }

            // 隐藏编辑界面
            HideLeadEditView();
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // 重载配置数据
            LoadLeadData(cboList.SelectedItem.ToString());

            // 隐藏编辑界面
            HideLeadEditView();
        }
    }
}