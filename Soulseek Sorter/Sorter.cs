using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using System.IO;
using Id3;
using ATL;
using System.Diagnostics;
using System.Windows.Forms;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Helpers;
namespace Soulseek_Sorter
{
    public class Sorter
    {

       
        public string artist;
        public async void sortDownloads(string inputPath, string outputPath, Form1 form)
        {
            var client = new LastfmClient("26a4830066690612b113890b795bb307", "3229dd61c790557dcba24809e350d896"); //Generates last.fm client to handle calls to API (Need to move keys to JSON/secure)
            string[] bannedChars = { "*", "\"", "/", "\\", "[", "]", ":", ";", "|", "=", "?", "!", "%", "-" }; //Banned windows file name characters
            string fileType;

            if (Directory.Exists(inputPath))
            {
                string[] downloadedFolders = Directory.GetDirectories(inputPath); //Array containing all folders in the input directory
                float progressIncrement = 0;
                form.progressBar.Value = 0;
                
                if(downloadedFolders.Length != 0)
                {
                    progressIncrement = (float)100 / (float)downloadedFolders.Length;
                }
                else if(downloadedFolders.Length == 0)
                {
                    progressIncrement = 100;
                }
                foreach (string folder in downloadedFolders) //For each folder in the input directory (Should contain the audio files)
                {
                    
                    int counter = 0;                  
                    form.richTextBox1.AppendText("\n" + folder + "\n");
                    foreach (string file in Directory.GetFiles(folder))
                    {

                        /*The Following if and elseif statements all perform the same action, the only variance is how they handle the file type*/
                        /*For brevity only the mp3 case is commented*/
                        if (file.Contains(".mp3"))
                            fileType = ".mp3";
                        else if (file.Contains(".flac"))
                            fileType = ".flac";
                        else if (file.Contains(".m4a"))
                            fileType = ".m4a";
                        else if (file.Contains(".wav"))
                            fileType = ".wav";
                        else
                            fileType = "";


                        if (fileType != "" && file.Length < 260) //If the file type is one of the valid types
                        {
                            TagLib.File audiofile; //Stores an audio file as a TagLib.File
                            if(fileType.Equals(".flac"))
                            {
                                audiofile = TagLib.Flac.File.Create(file);
                            }
                            else
                            {
                                audiofile = TagLib.File.Create(file);
                            }
                            string album = audiofile.Tag.Album; //Gets the album name (Assumes the tag exists already)
                            if (counter == 0) //If this is the first file read from this folder
                            {
                                artist = audiofile.Tag.FirstAlbumArtist; // Set the artist variable as the artist stored in the First Album Artist metadata tag
                            }
                            if (artist == null) //If the artist tag is blank or does not exist
                            {
                                if (album != null)
                                {
                                    var albumNameSearchResults = await client.Album.SearchAsync(album); //Searches the last fm api for info on the current album. Async function so the program awaits response
                                    
                                    
                                    NoArtistPopUp pop = new NoArtistPopUp(album, this);//Open the new dialogue box to allow the user to enter the artist manually
                                         
                                    //For each item received from the call to the Last.FM API (Provides a list of artists associated with the album)
                                    foreach (var item in albumNameSearchResults.Content)
                                    {
                                        pop.setTextBoxSuggestions(item.ArtistName); //Add the artists returned to the suggestions in the NoArtistPopUp Forms textbox
                                        
                                    }
                                    pop.ShowDialog(); //Show the new dialogue box
                                    counter = 1; //Sets the counter to 1 so that the artist entered in the dialogue box will be automatically applied to all files in the current folder
                                }
                            }
                            string fileText = audiofile.ToString(); //Stores the full track filename (song.mp3)
                            string title = audiofile.Tag.Title;  //Stores the title of a track (song)

                            //Goes through tag used to create the new sorted folder and files and ensures they have no banned characters that will throw errors when naming the new folders
                            foreach (string charac in bannedChars)
                            {
                                if (title != null)
                                {
                                    title = title.Replace(charac, "");
                                }
                                if (album != null)
                                {
                                    album = album.Replace(charac, "");
                                }
                                if (artist != null)
                                {
                                    artist = artist.Replace(charac, "");
                                }

                            }

                            //Small section to load the image result from last fm to the picture box
                            
                            try
                            {

                                var albumInfoSearchResults = await client.Album.GetInfoAsync(artist, album);
                                string imgURL = albumInfoSearchResults.Content.Images.ExtraLarge.AbsoluteUri;
                                if (!imgURL.Equals(null))
                                {
                                    imgURL = albumInfoSearchResults.Content.Images.ExtraLarge.AbsoluteUri;
                                    form.pictureBox1.Load(imgURL);
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.WriteLine(e.Message);
                                
                            }
                            form.artistLabel.Text = artist;
                            form.albumName.Text = album;
                            string targetPath = outputPath + "\\" + artist + "\\" + album; //Creates the new folder directory path, (MusicFolder\artist name\ album name)
                            if (!Directory.Exists(targetPath)) //If the directory doesn't alread exist create it
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            if (!System.IO.File.Exists(Path.Combine(targetPath, title + fileType)) && fileType != "") //If the current track file does not already exist in destination folder
                            {
                                System.IO.File.Move(file, Path.Combine(targetPath, title + fileType)); //Move the file from the input directory to the output directory
                                form.richTextBox1.AppendText("\n\tMoved: " + artist + " - " + album + " - " + title);
                                form.richTextBox1.ScrollToCaret();
                            }
                        }
                    }
                    form.richTextBox1.AppendText("\n");
                    form.progressBar.Value += (int)progressIncrement;
                    form.label4.Text = form.progressBar.Value.ToString() + "%";
                }
                
                //After all files are transferred, completely delete all remaining empty folders for cleanliness
                foreach(string folder in downloadedFolders)
                {
                    string[] downloadedFiles = Directory.GetFiles(folder);
                    foreach(string file in downloadedFiles)
                    {
                        if(file.Length < 260)
                            System.IO.File.Delete(file);
                        
                    }
                    System.IO.Directory.Delete(folder);
                }
                form.richTextBox1.AppendText("\nDONE!");
                form.richTextBox1.ScrollToCaret();
                form.progressBar.Value = 100;
                form.label4.Text = form.progressBar.Value.ToString() + "%";
            }
            
        }

        public string getArtistFromTextBox(System.Windows.Forms.TextBox textBox)
        {
            string text = textBox.Text;
            return text;
        }

        public string artistValue
        {
            get { return artist; }
            set { artist = value; }
        }

    }
}
