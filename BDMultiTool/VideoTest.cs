using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BDMultiTool
{
    public partial class VideoTest : Form
    {
        private VideoWriter _vw;

        public VideoTest()
        {
            InitializeComponent();

            Application.Idle += ApplicationOnIdle;

            start();
        }

        public void start()
        {
            try
            {
                var capture = new Capture(0);

                _vw = new VideoWriter(@"c:\" + GetFileName(),15,new Size(1366,768), true);
            }
            catch (Exception ex)
            {
                
            }
        }

        private string GetFileName()
        {
            return "ScreenCapture_" + System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".avi";
        }

        private void ApplicationOnIdle(object sender, EventArgs eventArgs)
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Image = bitmap;

            Image<Bgr, Byte> frame = new Image<Bgr, byte>(bitmap);
            _vw.Write(frame.Mat);
        }
    }
}
