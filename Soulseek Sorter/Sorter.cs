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


public struct TrackInfo
{
    public string title;
    public string fileType;
}
public struct AlbumInfo
{
    public List<TrackInfo> trackInfos;
    public List<string> tracks;
    public string album;
    public string artist;
    public string imageURL;
};

namespace Soulseek_Sorter
{
    public class Sorter
    {

        public string artist;
        public string parentPath;
        public int counter1 = 0;
        public int progressBarIncrement;
        public void sortDownloads(string inputPath, string outputPath, Form1 form)
        {
            recurFileSort(inputPath, outputPath, ref form);
            form.printOutput("\nDONE!");
            form.richTextBox1.ScrollToCaret();
        }

        public string getArtistFromTextBox(System.Windows.Forms.TextBox textBox)
        {
            string text = textBox.Text;
            return text;
        }

        
        private void recurFileSort(string inPath, string outPath, ref Form1 form)
        {

            if (counter1 == 0)
            {
                parentPath = inPath;
                if (Directory.GetDirectories(parentPath).Length != 0) 
                    progressBarIncrement = 100 / Directory.GetDirectories(parentPath).Length;
            }
            counter1 = 1;
            try
            {
                string[] files = Directory.GetFiles(inPath);
                AlbumInfo albumInfo = new AlbumInfo();
                albumInfo.tracks = new List<string>();
                albumInfo.trackInfos = new List<TrackInfo>();

                int counter = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    Debug.WriteLine(files[i]);
                    if ((file.Contains(".mp3") || file.Contains(".wav") || file.Contains(".flac")) && !file.Contains(".reapeaks"))
                    {
                        TrackInfo trackInfo = new TrackInfo();
                        TagLib.File audioFile = null;
                        if(file.Contains(".mp3"))
                        {
                            audioFile = TagLib.File.Create(file);
                            trackInfo.fileType = ".mp3";
                        }
                        else if(file.Contains(".flac"))
                        {
                            audioFile = TagLib.Flac.File.Create(file);
                            trackInfo.fileType = ".flac";
                        }

                        string[] bannedChars = { "*", "\"", "/", "\\", "[", "]", ":", ";", "|", "=", "?", "!", "%", "-", "\"" };

                        trackInfo.title = audioFile.Tag.Title.ToString();
                        if (counter == 0)
                        {
                            
                            var initInfo = getInitialInfo(audioFile);
                            initInfo.Wait();
                            albumInfo.album = initInfo.Result.Item1;
                            albumInfo.artist = initInfo.Result.Item2;
                            albumInfo.imageURL = initInfo.Result.Item4;
                            counter = 1;
                            Debug.WriteLine(albumInfo.imageURL);

                            if (albumInfo.imageURL != null)
                            {
                                form.pictureBox1.Load(albumInfo.imageURL);
                            }
                            if(albumInfo.artist != null)
                            {
                                form.artistLabel.Text = albumInfo.artist;
                            }
                            if(albumInfo.album != null)
                            {
                                form.albumName.Text = albumInfo.album;
                            }
                        }

                        foreach (string charac in bannedChars)
                        {
                            if (trackInfo.title != null)
                            {
                                trackInfo.title = trackInfo.title.Replace(charac, "");
                            }
                            if (albumInfo.album != null)
                            {
                                albumInfo.album = albumInfo.album.Replace(charac, "");
                            }
                            if (albumInfo.artist != null)
                            {
                                albumInfo.artist = albumInfo.artist.Replace(charac, "");
                            }

                        }
                        albumInfo.tracks.Add(file);
                        albumInfo.trackInfos.Add(trackInfo);
                    }
                    else
                    {
                        if(file.Length < 260)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                }


                for (int i = 0; i < albumInfo.tracks.Count; i++)
                {
                    Debug.WriteLine(albumInfo.trackInfos[i].title);
                    sortFile(albumInfo.tracks[i], albumInfo, inPath, outPath, albumInfo.trackInfos[i].title, albumInfo.trackInfos[i].fileType);
                    
                    form.richTextBox1.AppendText("Moved: " + albumInfo.artist + "-" + albumInfo.album + "-" + albumInfo.trackInfos[i].title + "\n");
                    form.richTextBox1.ScrollToCaret();

                    System.IO.File.Delete(files[i]);
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            string[] subDirs = Directory.GetDirectories(inPath);
            foreach (string direc in subDirs)
            {
                Debug.WriteLine(direc);
                recurFileSort(direc, outPath, ref form);
            }
            if (inPath != parentPath)
            {
                System.IO.Directory.Delete(inPath);
                

            }
            form.progressBar.Value = 100;
            form.label4.Text = form.progressBar.Value.ToString() + "%";
            

        }

        private void sortFile(string file, AlbumInfo albumInfo, string inPath, string outPath, string trackTitle, string fileType)
        {
            
            string targetPath = outPath + "\\" + albumInfo.artist + "\\" + albumInfo.album; //Creates the new folder directory path, (MusicFolder\artist name\ album name)
            Debug.WriteLine(targetPath);
            if (!Directory.Exists(targetPath)) //If the directory doesn't alread exist create it
            {
                Directory.CreateDirectory(targetPath);
            }

            if (!System.IO.File.Exists(Path.Combine(targetPath, trackTitle + fileType)) && fileType != "") //If the current track file does not already exist in destination folder
            {
                Debug.WriteLine(Path.Combine(targetPath, trackTitle + fileType));
                System.IO.File.Move(file, Path.Combine(targetPath, trackTitle + fileType)); //Move the file from the input directory to the output directory
            }
        }
        private async Task<Tuple<string , string, string, string>> getInitialInfo(TagLib.File audioFile)
        {
            
            var client = new LastfmClient("26a4830066690612b113890b795bb307", "3229dd61c790557dcba24809e350d896");
             //Banned windows file name characters
            string artist2;
            try
            {
                artist2 = audioFile.Tag.FirstPerformer.ToString();
            }catch(Exception e)
            {
                artist2 = null;
            }
            string album = audioFile.Tag.Album.ToString();
            string title = audioFile.Tag.Title.ToString();
            string imgURL = null;
            if (artist2 == null) //If the artist tag is blank or does not exist
            {
                if (album != null)
                {

                    var albumNameSearchResults = Task.Run(() => {client.Album.SearchAsync(album); });//Searches the last fm api for info on the current album. Async function so the program awaits response
                    var searchResults = albumNameSearchResults.ContinueWith(t => client.Album.SearchAsync(album));
                    searchResults.Wait();
                    NoArtistPopUp pop = new NoArtistPopUp(album, this);//Open the new dialogue box to allow the user to enter the artist manually
                     //For each item received from the call to the Last.FM API (Provides a list of artists associated with the album)
                    foreach (var item in searchResults.Result.Result.Content)
                    {
                        pop.setTextBoxSuggestions(item.ArtistName); //Add the artists returned to the suggestions in the NoArtistPopUp Forms textbox

                    }
                    pop.ShowDialog(); //Show the new dialogue box
                    artist2 = artist;
                }
            }



            try
            {

                var albumInfoSearchResults = Task.Run(() => { client.Album.GetInfoAsync(artist2, album); });
                var searchResults = albumInfoSearchResults.ContinueWith(t => client.Album.GetInfoAsync(artist2, album));
                searchResults.Wait();
                imgURL = searchResults.Result.Result.Content.Images.ExtraLarge.AbsoluteUri;
            }
            catch (Exception e)
            {
                imgURL = null;
                Debug.WriteLine(e.Message);

            }

            var results = new Tuple<string, string, string, string>(album, artist2, title, imgURL);
            return results;
        }

        private void updateUI(ref Form1 form, string album, string picture, string artist)
        {
            form.artistLabel.Text = artist;
            form.albumName.Text = album;
            form.pictureBox1.Load(picture);
        }
    }
}
