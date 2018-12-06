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
using System.Threading;
using IF.Lastfm.Core.Objects;
public struct TrackInfo
{
    public string title;
    public string fileType;
}
/*
 * Structure: AlbumInfo
 * Child Stuctures: TrackInfo
 * Parent Structures: Top-Level(None)
 */
public struct AlbumInfo
{
    public List<TrackInfo> trackInfos; //List of TrackInfo structures that hold info for each track in album
    public List<string> tracks;
    public string album;
    public string artist;
    public string imageURL;
};

namespace Soulseek_Sorter
{
    public class Sorter
    {

        public string artist; //Variable that is used to hold the artist value returned from the No Artist Pop Up Window
        public string parentPath; //The parent folder, held to avoid it being deleted
        public int counter1 = 0;
        LastfmClient client = new LastfmClient("26a4830066690612b113890b795bb307", "3229dd61c790557dcba24809e350d896");
        Database data = new Database();
        [STAThread]
        public void sortDownloads(string inputPath, string outputPath, Form1 form)
        {
            /*
            data.addArtist("Deathspell Omega");
            data.addArtist("Absu");
            data.addArtist("Nyredolk");
            data.addArtist("Mgla");
            data.addAlbum("Nyredolk", "Demo 2018");
            data.addAlbum("Mgla", "With Hearts Towards None");
            data.addAlbum("Metallica", "...And Justice For All");
            data.addAlbum("Mgla", "Exercises in Futility");
            data.updateAlbumWithInfo("Mgla", "Exercises in Futility", "www.no-solace.com");
            data.updateAlbumWithInfo("Skinless", "Savagery", "www.noURL.com");
            data.addAlbum("Deathspell Omega", "Drought");     
            data.deleteArtist("Deathspell Omega");
            */
            recurFileSort(inputPath, outputPath, form);
            form.printOutput("\nDONE!");
            form.richTextBox1.ScrollToCaret();
            
        }
   
        private void recurFileSort(string inPath, string outPath, Form1 form)
        {

            if (counter1 == 0)
            {
                parentPath = inPath;
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
                    if ((file.Contains(".mp3") || file.Contains(".wav") || file.Contains(".flac")) && (!file.Contains(".reapeaks") || !file.Contains(".asd")))
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

                        string[] bannedChars = { "*", "\"", "/", "\\", "[", "]", ":", ";", "|", "=", "?", "!", "%", "-", "\"", "."};

                        try
                        {
                            trackInfo.title = audioFile.Tag.Title.ToString();
                        }
                        catch(Exception e)
                        {

                        }
                        if (counter == 0)
                        {
                            //var initInfo = getInitialInfo(audioFile); //Searches the last fm api for info on the current album. Async function so the program awaits response



                            albumInfo.album = getAlbumName(audioFile).Result;
                            albumInfo.artist = getArtistName(audioFile).Result;
                            albumInfo.imageURL = getAlbumArtwork(audioFile, albumInfo.artist, albumInfo.album).Result;
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
                            try
                            {
                                data.updateAlbumWithInfo(albumInfo.artist, albumInfo.album, albumInfo.imageURL);
                            }
                            catch(Exception e)
                            {
                                Debug.WriteLine(e.Message);
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
                Debug.WriteLine(e + "\n");
                Debug.WriteLine(inPath + "\n");
                form.richTextBox1.AppendText(e.Message);
                form.richTextBox1.AppendText(inPath);
            }
            string[] subDirs = Directory.GetDirectories(inPath);
            foreach (string direc in subDirs)
            {
                Debug.WriteLine(direc);
                recurFileSort(direc, outPath, form);
            }
            if (inPath != parentPath)
            {
                System.IO.Directory.Delete(inPath);
            }

            return;
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

        /*
         * Task: getInitialInfo
         * Purpose: Retrieves the main metadata from the local tags and from the last.fm API whenever a new album is found
         */

        private void updateUI(ref Form1 form, string album, string picture, string artist)
        {
            form.artistLabel.Text = artist;
            form.albumName.Text = album;
            form.pictureBox1.Load(picture);
        }

        private async Task<string> getAlbumName(TagLib.File audioFile)
        {
            string albumName;
            albumName = audioFile.Tag.Album;
            return albumName;
        }

        private async Task<string> getArtistName(TagLib.File audioFile)
        {
            string artistName;
            string albumName;
            albumName = audioFile.Tag.Album;
            try
            {
                artistName = audioFile.Tag.FirstPerformer;
            }
            catch(Exception e)
            {
                artistName = null;
            }
            if (albumName != null && artistName == null)
            {
                var artistNameSearch = Task.Run(() => { client.Album.SearchAsync(albumName); });
                var searchResults = artistNameSearch.ContinueWith(t => client.Album.SearchAsync(albumName));
                searchResults.Wait();
                NoArtistPopUp pop = new NoArtistPopUp(albumName, this);          
                foreach (var item in searchResults.Result.Result.Content)
                {
                    pop.setTextBoxSuggestions(item.ArtistName);
                }
                
                pop.ShowDialog();
                artistName = artist;
            }
            artist = "Unknown Artist";
            return artistName;
        }

        private async Task<string> getAlbumArtwork(TagLib.File audioFile, string artist2, string album)
        {
            string imgURL;
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
            return imgURL;
        }

        



        
    }
}
