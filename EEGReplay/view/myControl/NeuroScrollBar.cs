using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace EEGReplay
{
    /// <summary>
    /// 自定义滚动条
    /// </summary>
    public partial class NeuroScrollBar : UserControl // UserControl, ToolStripControlHost
    {
        private int _maxValue = 1;

        public int MaxValue
        {
            set
            {
                if (value < 1)
                    throw new Exception("NeuroScrollBar MaxValue must >= 1, your value: " + value);
                if (value < this._minValue)
                    throw new Exception("NeuroScrollBar MaxValue must <= MinValue, your value: " + value + ", MinValue: " + this._minValue);

                int oldValue = this._maxValue;
                this._maxValue = value;

                if (this._maxValue != oldValue)
                    this.resetPagePicBox();
            }
            get
            {
                return this._maxValue;
            }
        }

        private int _minValue = 1;

        public int MinValue
        {
            set
            {
                if (value < 1)
                    throw new Exception("NeuroScrollBar MinValue must >= 1, your value: " + value);
                if (value > this._maxValue)
                    throw new Exception("NeuroScrollBar MinValue must <= MaxValue, your value: " + value + ", MaxValue: " + this._maxValue);

                int oldValue = this._minValue;
                this._minValue = value;

                if (this._minValue != oldValue)
                    this.resetPagePicBox();
            }
            get
            {
                return this._minValue;
            }
        }

        private float _value = 1;

        public float Value
        {
            set
            {
                if (value > this._maxValue || value < this._minValue)
                    throw new Exception("Out of range, MaxValue: " + this._maxValue + ", MinValue: " + this._minValue + ", your value: " + value);

                float oldValue = this._value;
                this._value = value;

                if (this._value != oldValue)
                    this.resetPagePicBox();
            }
            get
            {
                return this._value;
            }
        }

        private PictureBox _pagePicBox = new PictureBox();

        public NeuroScrollBar()
        {
            InitializeComponent();

            this._pagePicBox.BackColor = Color.Red;
            this.init();
        }

        public NeuroScrollBar(Color scrollBarColor, Color scrollColor)
        {
            InitializeComponent();

            this.BackColor = scrollBarColor;
            this._pagePicBox.BackColor = scrollColor;

            this.init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            this.Controls.Add(this._pagePicBox);
            this._pagePicBox.Location = new Point(0, 0);
            this._pagePicBox.Size = this.Size;

            this._pagePicBox.MouseClick -= new MouseEventHandler(this.pagePicBox_MouseClick);
            this._pagePicBox.MouseClick += new MouseEventHandler(this.pagePicBox_MouseClick);

            this.MouseClick -= new MouseEventHandler(this.NeuroScrollBar_MouseClick);
            this.MouseClick += new MouseEventHandler(this.NeuroScrollBar_MouseClick);
        }

        /// <summary>
        /// 鼠标点击pictureBox，事件传递给滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pagePicBox_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, e.X + this._pagePicBox.Location.X, e.Y + this._pagePicBox.Location.Y, e.Delta);
            this.OnMouseClick(e2);
        }

        /// <summary>
        /// 鼠标点击，移动pictureBox的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeuroScrollBar_MouseClick(object sender, MouseEventArgs e)
        {
            // MessageBox.Show("NeuroScrollBar_MouseClick -- 1");

            float width = (float)this.Size.Width / (float)this._maxValue;
            int clickPos = e.X;

            float newValue = (float)clickPos / width + 1;
            if (newValue < 1) newValue = 1;
            if (newValue > this._maxValue) newValue = this._maxValue;
            this.Value = newValue;

            // Debug.WriteLine("NeuroScrollBar_MouseClick  value = " + this._value);
        }

        /// <summary>
        /// 根据当前值，自动设置picBox的位置
        /// </summary>
        private void resetPagePicBox()
        {
            float width = (float)this.Size.Width / (float)this._maxValue;
            float height = this.Size.Height;

            Size newSize;
            if (width < 10)
            {
                newSize = new Size(10, (int)height);
            } else
            {
                newSize = new Size((int)width, (int)height);
            }
            this._pagePicBox.Size = newSize;

            int y = 0;
            int x = (int)(width * (this._value - 1));
            Point loc = new Point(x, y);
            this._pagePicBox.Location = loc;

            // Debug.WriteLine("resetPagePicBox() size:" + newSize + ", Location:" + loc);
        }
    }
}
