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





        /*PRIVATE VARIABLES*/
            MJPEGStream stream; // video stream
            int todo = 1;  // exec. mode: 
                           //   0 - record background
                           //   1 - record foreground (on start)
            int x, y, xSize, ySize; // size in foreground:
                                    //           x, y - coords of start
                                    //   xSize, ySize - pixels after (x, y)
            Random rnd = new Random(); // random instance
            bool newSize = false; // if true - MouseDown/Up/Move are active
            Point start, end;    //  instruments for drowing
            bool drawing = false;//    new foreground size


        /*PUBLIC VARIABLES*/
            public static int neg = 1, //neg img count !static
                              pos = 1; //pos img count !static


        


        void Form1_Load(object sender, EventArgs e)
        {
            //Cleaning dirs
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
            catch (Exception Ex) { MessageBox.Show(Ex.ToString()); }

            //Getting IP DroidCam from IPStremFORM
            IPStream IPGet = new IPStream();
            IPGet.ShowDialog();

            //init size of foreground
            x = 250; xSize = 150;
            y = 150; ySize = 150;

            //Starting stream
            stream = new MJPEGStream(IPGet.StreamLink);
            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }



        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap orig = createBitmap(sender, eventArgs);
            Bitmap bmp = MakeGrayscale(orig);
            Graphics g = Graphics.FromImage(bmp);


            Bitmap CroppedBmp;

            // if foreground selected - Drowing rectangle
            if (todo == 1)
            {
                using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                   g.DrawRectangle(pen, x, y, xSize, ySize);
            }

            /*ON SHIFT HOLD*/
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // foreground selected. Trimming and saving
                if (todo == 1) 
                {
                    CroppedBmp = orig.Clone(new System.Drawing.Rectangle
                        ( x + 2, y + 2, xSize - 2 * 2, xSize - 2 * 2), bmp.PixelFormat);
                    SavePhoto(CroppedBmp, "positive/rawdata/", pos, ".bmp");
                    pos++;
                }
                // backgroung selected. Trimming and saving
                if (todo == 0) 
                {
                    using (Pen pen = new Pen(Color.FromArgb(0, 255, 0), 3))
                    {
                        int ax = rnd.Next(pictureBox1.Width  - 150);
                        int ay = rnd.Next(pictureBox1.Height - 150);
                        CroppedBmp = bmp.Clone(new System.Drawing.Rectangle
                            (ax, ay, 150, 150), bmp.PixelFormat);
                        g.DrawRectangle(pen, ax, ay, 150, 150);
                    }
                    SavePhoto(CroppedBmp, "negative/", neg, ".jpg");
                    neg++;
                }
            }
            g.Dispose();

            //label counts visualization
            label2.Invoke((MethodInvoker)(() => label2.Text = Convert.ToString(pos - 1)));
            label4.Invoke((MethodInvoker)(() => label4.Text = Convert.ToString(neg - 1)));

            //painting img
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

        void SavePhoto(Bitmap Bmp, string path, int name, string format) => Bmp.Save(path + name + format);

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B) // flag on BackGround
            { 
                todo = 0; 
                label10.Invoke((MethodInvoker)(() => label10.Text = "   Background mode selected.\nHold Shift and DON`t show the object." ));
            }
            if (e.KeyCode == Keys.F) // flag on ForeGround
            { 
                todo = 1;
                label10.Invoke((MethodInvoker)(() => label10.Text = "   Foreground mode selected.\nPut object in rectangle and hold Shift." +xSize.ToString()+"x"+ ySize.ToString()));
            }

            if (e.KeyCode == Keys.N) // Change size
            {
                xSize = 0; ySize = 0;
                newSize = true;
                label10.Invoke((MethodInvoker)(() => label10.Text = "   Click with the mouse in the left upper point of the image and do not let go until the mouse is in the lower right corner."));
            }

            if (e.KeyCode == Keys.Escape) // to bat exec. step
            { 
                NextStep();
            }
        }

        void pictureBox1_MouseDown(object sender, MouseEventArgs e)//Pressed
        {
            if (newSize)
            {
                start = new Point(e.X, e.Y);
                x = e.X;
                y = e.Y;
                xSize = 1;
                ySize = 1;
                drawing = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string address = "https://www.cs.auckland.ac.nz/~m.rezaei/Tutorials/Creating_a_Cascade_of_Haar-Like_Classifiers_Step_by_Step.pdf";
            System.Diagnostics.Process.Start(address);
        }

        void pictureBox1_MouseMove(object sender, MouseEventArgs e)//Moving
        {
            if (newSize)
            {
                if (!drawing) return;

                pictureBox1.Refresh();
                var finish = new Point(e.X, e.Y);
                xSize = e.X - x;
                ySize = e.Y - y;
                var pen = new Pen(Color.Red, 2f);
                var g = Graphics.FromHwnd(pictureBox1.Handle);
                g.DrawRectangle(pen, start.X, start.Y, finish.X - start.X, finish.Y - start.Y);
            }
        }
        void pictureBox1_MouseUp(object sender, MouseEventArgs e)//Up
        {
            if (newSize)
            {
                end = new Point(e.X, e.Y);
                xSize = e.X - x;
                ySize = e.Y - y;
                drawing = false;

                label10.Invoke((MethodInvoker)(() => label10.Text = "   New size: "+xSize.ToString() +"x"+ySize.ToString() + ". Foreground mode selected.\nPut object in rectangle and hold Shift."));
                newSize = false;
                todo = 1;
            }
        }

        //fully close on exit button clicked
        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form2 f = new Form2();
            f.CloseApplication();
        }
        

        void NextStep()
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        void Form1_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Buttons:\n" +
                         "  Click:\n" +
                         "     B - switch on Background;\n" +
                         "     F - switch on Foreground;\n" +
                         "     N - next camera size.\n" +
                         "  Hold:\n" +
                         "     Shift - record.\n" +
                         "  ESC - continue";
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
