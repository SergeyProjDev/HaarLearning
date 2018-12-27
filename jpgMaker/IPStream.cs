using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace jpgMaker
{
    public partial class IPStream : Form
    {
        public IPStream()
        {
            InitializeComponent();
        }

        public string StreamLink;
        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.ToString();
            string port = textBox2.Text.ToString();
            StreamLink = "http://" + ip + ":" + port + "/mjpegfeed?596x385";

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string address = "https://play.google.com/store/apps/details?id=com.dev47apps.droidcam&hl=en";
            System.Diagnostics.Process.Start(address);
        }
    }
}
