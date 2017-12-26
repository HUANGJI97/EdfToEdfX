using System.IO;
using System.Windows.Forms;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace EEGReplay
{
    public partial class FormVideo : Form
    {
        public VlcControl VlcPlayer;

        public FormVideo()
        {
            InitializeComponent();

            InitializeVlcPlayer();
        }

        private void InitializeVlcPlayer()
        {
            VlcPlayer = new VlcControl();
            VlcPlayer.Dock = DockStyle.Fill;

            // TODO 最好可以配置
            VlcPlayer.BeginInit();

            DirectoryInfo vlcLibDirectory = null;
            foreach (var item in new string[] { "C", "D", "E" })
            {
                vlcLibDirectory = new DirectoryInfo(item + @":\Program Files (x86)\VideoLAN\VLC");
                if (vlcLibDirectory.Exists) break;
            }

            if (vlcLibDirectory != null && !vlcLibDirectory.Exists)
            {
                foreach (var item in new string[] { "C", "D", "E" })
                {
                    vlcLibDirectory = new DirectoryInfo(item + @":\Program Files\VideoLAN\VLC");
                    if (vlcLibDirectory.Exists) break;
                }
            }

            VlcPlayer.VlcLibDirectory = vlcLibDirectory;

            VlcPlayer.EndInit();

            if (vlcLibDirectory.Exists)
            {
                this.Controls.Add(VlcPlayer);
            }
            else
            {
                this.Visible = false;
            }
        }

        private void FormVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;

            e.Cancel = true;
        }
    }
}