﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace jpgMaker
{
    public partial class Cropping : Form
    {
        public Cropping()
        {
            InitializeComponent();
        }

        void Cropping_Load(object sender, EventArgs e)
        {

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
            
            using (StreamWriter writer = new StreamWriter("positive/info.txt"))
            {
                foreach (FileInfo file in Files)
                {
                    str = "rawdata/"+file.Name+" 1 1 1 148 148";
                    writer.WriteLine(str);
                }
            }
            this.Hide();
        }
    }
}