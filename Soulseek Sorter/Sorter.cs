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
        public void sortDownloads(string inputPath, string outputPath)
        {
            string[] bannedChars = { "*", "\"", "/", "\\", "[", "]", ":", ";", "|", "=", "?", "!", "%" };
            if (Directory.Exists(inputPath))
            {
                string[] downloadedFolders = Directory.GetDirectories(inputPath);
                foreach (string folder in downloadedFolders)
                {
                    int counter = 0;
                    Debug.WriteLine(folder);
                    string[] downloadedFiles = Directory.GetFiles(folder);
                    foreach (string file in downloadedFiles)
                    {
                        Debug.WriteLine(file);
                        if (file.Contains(".mp3"))
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

                            Debug.WriteLine("Track: " + title);
                            Debug.WriteLine("Album: " + album);
                            Debug.WriteLine("Artist: " + artist);
                            string targetPath = outputPath + "\\" + artist + "\\" + album;
                            if (!Directory.Exists(targetPath))
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            if (!System.IO.File.Exists(Path.Combine(targetPath, title + ".mp3")))
                            {
                                System.IO.File.Move(file, Path.Combine(targetPath, title + ".mp3"));
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

                            Debug.WriteLine("Track: " + title);
                            Debug.WriteLine("Album: " + album);
                            Debug.WriteLine("Artist: " + artist);
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
