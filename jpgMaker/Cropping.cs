using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace jpgMaker
{
    public partial class Cropping : Form
    {
        public Cropping()
        {
            InitializeComponent();
        }

        //handly
        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ExecuteBatFile(AppDomain.CurrentDomain.BaseDirectory + "positive", "objectmarker.exe");
        }

        //automatic
        private void button2_Click(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory+"positive/rawdata");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files
            string str = "";

            Bitmap bmp;

            using (StreamWriter writer = new StreamWriter("positive/info.txt"))
            {
                foreach (FileInfo file in Files)
                {
                    bmp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory+"positive/rawdata/"+file.Name);
                    str = "rawdata/"+file.Name+" 1 1 1 "+(bmp.Width-2).ToString()+" "+(bmp.Height-2).ToString();
                    writer.WriteLine(str);
                }
            }
            this.Hide();
        }
    }
}
