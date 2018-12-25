using AForge.Video;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace jpgMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }





        string dir;

        string Save()
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();

            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            folderBrowser.FileName = "Select";

            if (folderBrowser.ShowDialog() == DialogResult.OK)
                return Path.GetDirectoryName(folderBrowser.FileName);
            else
                return null;
        }



        

        MJPEGStream stream;

        void Form1_Load(object sender, EventArgs e)
        {
            dir = Save();
            if (!Directory.Exists(dir + "/negative"))
                Directory.CreateDirectory(dir + "/negative");
            if (!Directory.Exists(dir + "/positive/rawdata"))
                Directory.CreateDirectory(dir + "/positive/rawdata");

            MessageBox.Show("Welcome!\nHold Shift to record");

            stream = new MJPEGStream("http://192.168.1.196:4747/mjpegfeed?596x385"); //IP+Port
            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }





        Random rnd = new Random();
        int counter = 0;

        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap orig = createBitmap(sender, eventArgs);
            Bitmap bmp = MakeGrayscale(orig);

            Bitmap CroppedBmp;
            Graphics g = Graphics.FromImage(bmp);

            using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 5))
            {
                g.DrawRectangle(pen, 250, 150, 150, 150);
                CroppedBmp = orig.Clone(new System.Drawing.Rectangle(255, 155, 140, 140), bmp.PixelFormat);
            }

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) //on Shift Hold
            {
                //SavePhoto(CroppedBmp, dir + "/positive/rawdata/", ".bmp");

                using (Pen pen = new Pen(Color.FromArgb(0, 255, 0), 3))
                {
                    int ax = rnd.Next(633-150);
                    int ay = rnd.Next(461-150);
                    while ((ax > 255) && (ax < 405))
                        ax = rnd.Next(633 - 50);
                    while ((ay > 105) && (ay < 245))
                        ay = rnd.Next(461 - 50);
                    CroppedBmp = bmp.Clone(new System.Drawing.Rectangle(ax, ay, 150, 150), bmp.PixelFormat);
                    g.DrawRectangle(pen, ax, ay, 150, 150);
                }
                //SavePhoto(CroppedBmp, dir + "/negative/", ".jpg");
                counter++;
            }

            g.Dispose();  
            
            pictureBox1.Image = bmp; //paint
        }





        Bitmap createBitmap(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipX); //зеркально
            return bmp;
        }





        Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            Graphics g = Graphics.FromImage(newBitmap);
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
               });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }





        void SavePhoto(Bitmap Bmp, string path, string format) => Bmp.Save(path + counter + format);
    }
}
