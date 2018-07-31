using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib.Flac;
using TagLib.Id3v1;
using TagLib.Id3v2;
using TagLib.Ogg;
using System.IO;
namespace Soulseek_Sorter
{
    public partial class Form1 : Form
    {
        
        
        DataStorage ds = new DataStorage();
        public Form1()
        {
            InitializeComponent();
            label1.Text = ds.loadCompletedFolderAsString();
            label2.Text = ds.loadDestinationFolderAsString();
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label1.Text = fbd.SelectedPath;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label2.Text = fbd.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ds.saveCompletedFolder(label1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ds.saveDestinationFolder(label2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Sorter sorter = new Sorter();
            sorter.sortDownloads(label1.Text, label2.Text);
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            this.button1.ForeColor = Color.Firebrick;
        }

    }
}