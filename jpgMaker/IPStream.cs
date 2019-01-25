using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace jpgMaker
{
    public partial class IPStream : Form
    {
        public IPStream()
        {
            InitializeComponent();
        }



        string serializedFileName;
        Serialization serialization;
        public string StreamLink;



        //continue
        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.ToString();
            string port = textBox2.Text.ToString();
            StreamLink = "http://" + ip + ":" + port +"/mjpegfeed?596x385";

            if (!StreamCheck(StreamLink))
            {
                MessageBox.Show("Error! \n      Check:\n - you started DroidCam"+
                                    "\n - local devices are connected\n - ip\n - port");
                return;
            }
                


            //serialize
            Data data = new Data();
            data.setIp(ip);
            data.setPort(port);
            serialization.setSerializatedInfo(serializedFileName, data);

            this.Hide();
        }


        //check if is it true IP
        private bool StreamCheck(string url)
        {
            Uri urlCheck = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);
            request.Timeout = 1500;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        //app page
        private void button2_Click(object sender, EventArgs e)
        {
            string address = "https://play.google.com/store/apps/details?id=com.dev47apps.droidcam&hl=en";
            System.Diagnostics.Process.Start(address);
        }



        //getting serialize on start
        private void IPStream_Load(object sender, EventArgs e)
        {
            serializedFileName = "Information.dat";
            serialization = new Serialization();

            Data newData = new Data();
            serialization.getSerializatedInfo(serializedFileName, out newData); //get serialization
            try
            {
                textBox1.Text = newData.getIp();
                textBox2.Text = newData.getPort();
            }
            catch (Exception){}
        }


        [Serializable]
        class Data
        {
            private string ip;
            private string port;

            public void setIp(string ip) => this.ip = ip;
            public void setPort(string port) => this.port = port;

            public string getIp() => ip;
            public string getPort() => port;
        }


        class Serialization
        {
            public object getSerializatedInfo(string serializedFileName, out Data exemplar1)
            {
                try
                {
                    using (var fStream = File.OpenRead(serializedFileName))
                    {
                        BinaryFormatter formatter1 = new BinaryFormatter();

                        exemplar1 = (Data)formatter1.Deserialize(fStream);
                        return exemplar1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    exemplar1 = null;
                    return exemplar1;
                }
            }
            public void setSerializatedInfo(string serializedFileName, object serObj)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (var fStream = new FileStream(serializedFileName, FileMode.Create, 
                                                            FileAccess.Write, FileShare.None))
                    {
                        formatter.Serialize(fStream, serObj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
