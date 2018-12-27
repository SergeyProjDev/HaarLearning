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


        

        MJPEGStream stream;

        void Form1_Load(object sender, EventArgs e)
        {
            //Cleaning dir`s
            try
            {
                string[] filePaths;
                filePaths = System.IO.Directory.GetFiles(@"negative\", "*.jpg");
                foreach (string filePath in filePaths) System.IO.File.Delete(filePath);

                filePaths = System.IO.Directory.GetFiles(@"positive\rawdata\", "*.bmp");
                foreach (string filePath in filePaths) System.IO.File.Delete(filePath);

                System.IO.DirectoryInfo di = new DirectoryInfo(@"cascades\");
                foreach (DirectoryInfo dir in di.GetDirectories()) dir.Delete(true);

                filePaths = System.IO.Directory.GetFiles(@"vector\", "*.vec");
                foreach (string filePath in filePaths) System.IO.File.Delete(filePath);
            }
            catch (Exception Ex) {
                MessageBox.Show(Ex.ToString());
            }

            //Getting IP DroidCam Link
            IPStream IPGet = new IPStream();
            IPGet.ShowDialog(); 

            //Starting stream
            stream = new MJPEGStream(IPGet.StreamLink);
            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }





        Random rnd = new Random();
        public static int counter = 1;
        public static int pos = 1;
        int five = 1;

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

            //on Shift Hold
            
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) 
            {
                if (five == 5)
                {
                    SavePhoto(CroppedBmp, "positive/rawdata/", ".bmp");
                    five = 1;
                    pos++;
                }
                    

                using (Pen pen = new Pen(Color.FromArgb(0, 255, 0), 3))
                {
                    int ax = rnd.Next(633-100);
                    int ay = rnd.Next(461-100);
                    while ( (ax > 150) && (ax < 400) )
                        ax = rnd.Next(633 - 100);
                    while ( (ay > 50) && (ay < 300) )
                        ay = rnd.Next(461 - 100);
                    CroppedBmp = bmp.Clone(new System.Drawing.Rectangle(ax, ay, 100, 100), bmp.PixelFormat);
                    g.DrawRectangle(pen, ax, ay, 100, 100);
                }
                SavePhoto(CroppedBmp, "negative/", ".jpg");
                five++;

                counter++;
            }

            g.Dispose();

            //paint
            pictureBox1.Image = bmp; 
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





        void SavePhoto(Bitmap Bmp, string path, string format)
            => Bmp.Save(path + counter + format);





        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
                Form2 form2 = new Form2();
                form2.Show();
            }
            
                
        }
    }
}
