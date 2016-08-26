using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaperDownloaderHub
{
    public partial class PaperDownloader : Form
    {
        //Get Download Folder Path
        static string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static string pathDownload = Path.Combine(pathUser, "Downloads");

        public PaperDownloader()
        {
            InitializeComponent();
            //Writes download path to label
            lblSave.Text = pathDownload;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            //Reads lines from TextBox
            String[] lines = richTextBox1.Lines;
            progressBar1.Maximum = lines.Length;
            foreach (var line in lines)
            {
                if (!line.Equals(""))
                {
                    progressBar1.Value += 1;

                    downloadDoi(line);
                }
            }
        }

        public void downloadDoi(string doi)
        {
            try
            {
                //get download link from html website
                string url = getHTML("http://sci-hub.cc/" + doi);
                int s1 = url.IndexOf("<iframe src =");
                string link = url.Remove(0, s1 + 15);
                s1 = link.IndexOf("</iframe>");
                link = link.Remove(s1 - 13, link.Length - s1 + 13);

                //set filename
                string filename = doi.Replace("/", "");
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                WebClient Webclient1 = new WebClient();

                //corrects wrong urls without http
                string path = pathDownload + "\\" + filename + ".pdf";
                string begin = link.Remove(2, link.Length - 2);
                if (begin.Equals("//"))
                {
                    link = link.Replace("//", "http://");
                }

                //download and save paper
                Webclient1.DownloadFile(link, path);
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with DOI: " + doi);
            }
        }

        public string getHTML(string url)
        {
            //download html code
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string html = sr.ReadToEnd();
            sr.Close();
            response.Close();

            return html;
        }
    }
}
