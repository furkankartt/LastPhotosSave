using MediaDevices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LastPhotosSave
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonGozat_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    textBoxPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var devices = MediaDevice.GetDevices();
            using (var device = devices.First(d => d.FriendlyName == "Apple iPhone"))
            {
                device.Connect();
                var photoDir = device.GetDirectoryInfo(@"\Dahili olarak paylaşılan depolama alanı\DCIM\Camera");
                var videoDir = device.GetDirectoryInfo(@"\Dahili olarak paylaşılan depolama alanı\DCIM\Camera");

                var files = photoDir.EnumerateFiles("*.jpg", SearchOption.AllDirectories);
                var fileVideos = photoDir.EnumerateFiles("*.mp4", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    MemoryStream memoryStream = new System.IO.MemoryStream();
                    device.DownloadFile(file.FullName, memoryStream);
                    memoryStream.Position = 0;
                    WriteSreamToDisk($@"{textBoxPath.Text}/{file.Name}", memoryStream);
                    device.DeleteFile(file.FullName);
                }
                foreach (var fileVideo in fileVideos)
                {
                    MemoryStream memoryStream = new System.IO.MemoryStream();
                    device.DownloadFile(fileVideo.FullName, memoryStream);
                    memoryStream.Position = 0;
                    WriteSreamToDiskVideo($@"{textBoxPath.Text}/{fileVideo.Name}", memoryStream);
                    device.DeleteFile(fileVideo.FullName);
                }
                device.Disconnect();
            }
            textBoxPath.Text = "";
            MessageBox.Show("Fotoğraflar başarılı bir şekilde aktarıldı.");
        }

        static void WriteSreamToDisk(string filePath, MemoryStream memoryStream)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                file.Write(bytes, 0, bytes.Length);
                memoryStream.Close();
            }
        }
        static void WriteSreamToDiskVideo(string filePath, MemoryStream memoryStream)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                file.Write(bytes, 0, bytes.Length);
                memoryStream.Close();
            }
        }
    }
}
