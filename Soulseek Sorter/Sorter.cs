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
namespace Soulseek_Sorter
{
    public class Sorter
    {
       
        public string artist;
        public void sortDownloads(string inputPath, string outputPath, Form1 form)
        {
            string[] bannedChars = { "*", "\"", "/", "\\", "[", "]", ":", ";", "|", "=", "?", "!", "%" }; //Banned windows file name characters
            if (Directory.Exists(inputPath))
            {
                string[] downloadedFolders = Directory.GetDirectories(inputPath); //Array containing all folders in the input directory
                foreach (string folder in downloadedFolders) //For each folder in the input directory (Should contain the audio files)
                {
                    int counter = 0;
                    string[] downloadedFiles = Directory.GetFiles(folder); //Array containing all files in the current folder
                    foreach (string file in downloadedFiles)
                    {
                        /*The Following if and elseif statements all perform the same action, the only variance is how they handle the file type*/
                        /*For brevity only the mp3 case is commented*/
                        if (file.Contains(".mp3"))
                        { 
                            var audiofile = TagLib.File.Create(file); //Stores an audio file as a TagLib.File
                            string album = audiofile.Tag.Album; //Gets the album name (Assumes the tag exists already)
                            if (counter == 0) //If this is the first file read from this folder
                            {
                                artist = audiofile.Tag.FirstAlbumArtist; // Set the artist variable as the artist stored in the First Album Artist metadata tag
                            }
                            if (artist == null) //If the artist tag is blank or does not exist
                            {
                                NoArtistPopUp pop = new NoArtistPopUp(album, this);//Open the new dialogue box to allow the user to enter the artist manually
                                pop.ShowDialog(); //Show the new dialogue box
                                counter = 1; //Sets the counter to 1 so that the artist entered in the dialogue box will be automatically applied to all files in the current folder
                            }
                            string fileText = audiofile.ToString(); //Stores the full track filename (song.mp3)
                            string title = audiofile.Tag.Title;  //Stores the title of a track (song)

                            //Goes through tag used to create the new sorted folder and files and ensures they have no banned characters that will throw errors when naming the new folders
                            foreach(string charac in bannedChars)
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
                            
                            string targetPath = outputPath + "\\" + artist + "\\" + album; //Creates the new folder directory path, (MusicFolder\artist name\ album name)
                            if (!Directory.Exists(targetPath)) //If the directory doesn't alread exist create it
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            if (!System.IO.File.Exists(Path.Combine(targetPath, title + ".mp3"))) //If the current track file does not already exist in destination folder
                            {
                                System.IO.File.Move(file, Path.Combine(targetPath, title + ".mp3")); //Move the file from the input directory to the output directory
                            }      
                        }
                        else if (file.Contains(".flac"))
                        {
                            var audiofile = TagLib.File.Create(file);
                            string album = audiofile.Tag.Album;
                            if (counter == 0)
                            {
                                artist = audiofile.Tag.FirstAlbumArtist;
                            }
                            if (artist == null)
                            {
                                NoArtistPopUp pop = new NoArtistPopUp(album, this);
                                pop.ShowDialog();
                                counter = 1;
                            }
                            string fileText = audiofile.ToString();
                            string title = audiofile.Tag.Title;
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
                            string targetPath = outputPath + "\\" + artist + "\\" + album;
                            if (!Directory.Exists(targetPath))
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            if (!System.IO.File.Exists(Path.Combine(targetPath, title + ".flac")))
                            {
                                System.IO.File.Move(file, Path.Combine(targetPath, title + ".flac"));
                            }
                        }
                        else if (file.Contains(".wav"))
                        {
                            var audiofile = TagLib.File.Create(file);
                            string album = audiofile.Tag.Album;
                            if (counter == 0)
                            {
                                artist = audiofile.Tag.FirstAlbumArtist;
                            }
                            if (artist == null)
                            {
                                NoArtistPopUp pop = new NoArtistPopUp(album, this);
                                pop.ShowDialog();
                                counter = 1;
                            }
                            string fileText = audiofile.ToString();
                            string title = audiofile.Tag.Title;

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

                            Debug.WriteLine("Track: " + title);
                            Debug.WriteLine("Album: " + album);
                            Debug.WriteLine("Artist: " + artist);
                            string targetPath = outputPath + "\\" + artist + "\\" + album;
                            if (!Directory.Exists(targetPath))
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            if (!System.IO.File.Exists(Path.Combine(targetPath, title + ".wav")))
                            {
                                System.IO.File.Move(file, Path.Combine(targetPath, title + ".wav"));
                            }
                        }
                        else if (file.Contains(".m4a"))
                        {
                            var audiofile = TagLib.File.Create(file);
                            string album = audiofile.Tag.Album;
                            if (counter == 0)
                            {
                                artist = audiofile.Tag.FirstAlbumArtist;
                            }
                            if (artist == null)
                            {
                                NoArtistPopUp pop = new NoArtistPopUp(album, this);
                                pop.ShowDialog();
                                counter = 1;
                            }
                            string fileText = audiofile.ToString();
                            string title = audiofile.Tag.Title;

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

                            Debug.WriteLine("Track: " + title);
                            Debug.WriteLine("Album: " + album);
                            Debug.WriteLine("Artist: " + artist);
                            string targetPath = outputPath + "\\" + artist + "\\" + album;
                            if (!Directory.Exists(targetPath))
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            if (!System.IO.File.Exists(Path.Combine(targetPath, title + ".m4a")))
                            {
                                System.IO.File.Move(file, Path.Combine(targetPath, title + ".m4a"));
                            }
                        }
                    }
                    System.IO.File.Delete(folder); // Deletes the input folder after moving all files to the new directory
                }
                
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
