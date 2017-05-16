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
using System.Xml.Serialization;

namespace File_Hidder_Sample
{
    public partial class Form1 : Form
    {
        long _totalBytesSelected = 0;
        string _mainFile = "";

        public BindingList<xFile> _selectedFiles = new BindingList<xFile>();

        // Constructor
        public Form1()
        {
            InitializeComponent();
            listBox1.DataSource = _selectedFiles;
            listBox1.DisplayMember = "Path";
        }

        // Add Item
        private void button2_Click(object sender, EventArgs e)
        {
            using (var open = new OpenFileDialog())
            {
                if (open.ShowDialog() == DialogResult.OK)
                {
                    var xFileTmp = new xFile { Path = open.FileName, Info = new FileInfo(open.FileName) };
                    if (xFileTmp.Info.Length <= 10000000)
                    {
                        xFileTmp.Bytes = File.ReadAllBytes(xFileTmp.Path);
                        _selectedFiles.Add(xFileTmp);
                        ChangeTotalBytesSelectedValue(+xFileTmp.Info.Length);
                    }
                }
            }
        }

        // Change Total Bytes Selected Value
        private void ChangeTotalBytesSelectedValue(long x)
        {
            _totalBytesSelected += x;
            groupBox2.Text = "Total Files : " + _selectedFiles.Count() + " , Bytes : " + _totalBytesSelected;
        }

        // Class for Files
        [Serializable]
        public class xFile
        {
            public string Path { get; set; }

            [XmlIgnore]
            public FileInfo Info { get; set; }
            public byte[] Bytes { get; set; }
        }

        // Select Main File
        private void button1_Click(object sender, EventArgs e)
        {
            using (var open = new OpenFileDialog())
                if (open.ShowDialog() == DialogResult.OK)
                {
                    _mainFile = open.FileName;
                    textBox1.Text = _mainFile;
                }
        }

        // Remove Item
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                xFile tmp = (xFile)listBox1.SelectedItem;
                long length = tmp.Info.Length;
                _selectedFiles.Remove(tmp);
                ChangeTotalBytesSelectedValue(-length);
            }
            catch
            {
                MessageBox.Show("No item is selected !");
            }
        }

        // Save
        private void button4_Click(object sender, EventArgs e)
        {
            using (var save = new SaveFileDialog())
            {
                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(_mainFile, save.FileName);
                    string tmp = Serializer.Serialize(_selectedFiles);
                    string split = "@HACKFORUMS.NET@Albania";

                    File.AppendAllText(save.FileName, split + tmp);
                }
            }
        }
    }
}
