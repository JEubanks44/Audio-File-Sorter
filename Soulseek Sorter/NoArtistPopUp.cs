using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;
namespace Soulseek_Sorter
{
    public partial class NoArtistPopUp : Form
    {
        public bool artistEntered = false;
        public string artist;
        Sorter sort;
        public NoArtistPopUp(string album, Sorter sort)
        {
            InitializeComponent();
            label1.Text = "Please Enter and Artist for this Album: " + album;
            this.sort = sort;
        }


        public void button1_Click(object sender, EventArgs e)
        {
            
            artist = comboBox1.Text;
            sort.artist = artist;
            this.Close();
        }

        public string getArtist()
        {
            return artist;
        }

        public void setTextBoxSuggestions(string suggestions)
        {
            if (!comboBox1.Items.Contains(suggestions))
            {
                comboBox1.Items.Add(suggestions);
            }
        }



    }
}
