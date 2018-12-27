using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace jpgMaker
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        void Form2_Shown(object sender, EventArgs e)
        {
            int nNeg = Form1.counter;
            int nPos = Form1.pos;

            Step1();
            Step2();
            Step3();
            Step4();
            Step5();

            ActiveSaveButton();



            void Step1()
            {
                ExecuteBatFile(AppDomain.CurrentDomain.BaseDirectory + "negative", "create_list.bat");
                Done(checkBox1);
            }

            void Step2()
            {
                //creating coordinates list
                Cropping croppForm = new Cropping();
                croppForm.ShowDialog();

                Done(checkBox2);
            }

            void Step3()
            {
                ExecuteBatFile(AppDomain.CurrentDomain.BaseDirectory, "01 samples_creation.bat");
                Done(checkBox3);
            }

            void Step4()
            {
                string haarTrainingBat = "haartraining.exe -data cascades -vec vector/vector.vec -bg negative/bg.txt -npos "+nPos+" -nneg "+nNeg+" -nstages 15 -mem 1024 -mode ALL -w 24 -h 24";
                using (StreamWriter writer = new StreamWriter("02 haarTraining.bat"))
                {
                    writer.WriteLine(haarTrainingBat);
                    writer.WriteLine("rem -nonsym");          
                }
                ExecuteBatFile(AppDomain.CurrentDomain.BaseDirectory, "02 haarTraining.bat");
                try {
                    File.Delete("02 haarTraining.bat");
                }
                catch (Exception ex){
                    MessageBox.Show(ex.ToString());
                }
                Done(checkBox4);
            }

            void Step5()
            {
                ExecuteBatFile(AppDomain.CurrentDomain.BaseDirectory, "03 convert.bat");
                Done(checkBox5);
            }

            void ActiveSaveButton()
            {
                button1.Enabled = true;
                button1.BackColor = Color.Green;
            }
        }

        


        void Done(CheckBox cb)
        {
            cb.Checked = true;
            cb.ForeColor = Color.Green;
        }

        public void ExecuteBatFile(string dir, string fileName)
        {
            Process proc = null;
            try
            {
                string targetDir = string.Format(dir);   //this is where mybatch.bat lies
                proc = new Process();
                proc.StartInfo.WorkingDirectory = targetDir;
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.Arguments = string.Format("10");  //this is argument
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
            }catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        void button1_Click(object sender, EventArgs e)
        {
            string address = getSaveAddress();
            File.Move("myhaar.xml", address);

            CloseApplication();  
        }

        string getSaveAddress()
        {
            while (true){
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Сохранение Хаара.xml";
                saveFileDialog.FileName = "myhaar.xml";
                saveFileDialog.Filter = "xml|*.xml|All files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    return saveFileDialog.FileName;
                MessageBox.Show("Error!");
            }
            
        }

        void CloseApplication()
        {
            Process[] proc = Process.GetProcessesByName("jpgMaker");
            proc[0].Kill();
        }
    }
}
