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

public struct ReleaseDate
{
    public int day;
    public int month;
    public int year;
}
/*
 * Structure: AlbumInfo
 * Child Stuctures: TrackInfo
 * Parent Structures: Top-Level(None)
 */
public struct AlbumInfo
{
    public ReleaseDate releaseDate;
    public List<TrackInfo> trackInfos; //List of TrackInfo structures that hold info for each track in album
    public List<string> tracks;
    public string album;
    public string originalTitle;
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
        LastfmClient client = new LastfmClient("26a4830066690612b113890b795bb307", "3229dd61c790557dcba24809e350d896"); //LastFM API Access Client
        Database data = new Database(); //Class Object to interact with Database
        string[] bannedChars = { "*", "\"", "/", "\\", "[", "]", ":", ";", "|", "=", "?", "!", "%", "-", "\"", "." }; //Characters that aren't allowed by windows file system (Used to remove before creating new file)


        /* Function Name: sortDownloads
         * Function Purpose: Runs necessary function when called from the UI
         * Parameters:
             -> inputPath: String containing the parent directory to be searched for audio files
             -> outputPath: String containing the output directory where the new assorted folders and files will be placed
             -> form: Instance of the main GUI form
        */
        public void sortDownloads(string inputPath, string outputPath, Form1 form)
        {
            
            recurFileSort(inputPath, outputPath, form);
            form.printOutput("\nDONE!");
            form.richTextBox1.ScrollToCaret();   
        }


        /* Function Name: recurFileSort
         * Function Purpose: Recursively searches the input directory and gathers information about audio files when found. Main function of class
         * Parameters:
         *      -> inPath: Parent directory that will be searched recursively
         *      -> outPath: Directory that sorted files will be placed in
         *      -> form: Instance of the main GUI form
         */
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
                        TagLib.File audioFile = generateAudioFile(file);
                        trackInfo.fileType = getFileType(file);
                        try
                        {
                            trackInfo.title = audioFile.Tag.Title.ToString();
                        }
                        catch(Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                        if (counter == 0)
                        { 
                            albumInfo.artist = getArtistName(audioFile);
                            albumInfo.originalTitle = getAlbumName(audioFile);
                            albumInfo.imageURL = getAlbumArtwork(audioFile, albumInfo.artist, albumInfo.originalTitle);
                            counter = 1;
                            Debug.WriteLine(albumInfo.imageURL);
                            updateUI(form, albumInfo.album, albumInfo.imageURL, albumInfo.artist);
                            try
                            {
                                data.updateAlbumWithInfo(albumInfo.artist, albumInfo.originalTitle, albumInfo.imageURL);
                            }
                            catch(Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }
                        }
                        trackInfo.title = removeBannedChars(trackInfo.title);
                        albumInfo.album = removeBannedChars(albumInfo.originalTitle);
                        albumInfo.artist = removeBannedChars(albumInfo.artist);
                        albumInfo.tracks.Add(file);
                        albumInfo.trackInfos.Add(trackInfo);
                    }
                    else
                    {
                        if(file.Length < 260)
                        {
                            System.IO.File.Delete(file); //If the file breaks the 260 character limit simply delete it
                        }
                    }
                }


                for (int i = 0; i < albumInfo.tracks.Count; i++)
                {
                    Debug.WriteLine(albumInfo.trackInfos[i].title);
                    sortFile(albumInfo.tracks[i], albumInfo, outPath, albumInfo.trackInfos[i].title, albumInfo.trackInfos[i].fileType);
                    form.richTextBox1.AppendText("Moved: " + albumInfo.artist + "-" + albumInfo.album + "-" + albumInfo.trackInfos[i].title + "\n");
                    form.richTextBox1.ScrollToCaret();
                    System.IO.File.Delete(files[i]);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e + "\n");
                Debug.WriteLine(inPath + "\n");
                
                form.richTextBox1.AppendText("An Error Occured Moving the file located at:" + inPath);
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

        /* Function Name: sortFile
         * Function Purpose: Sorts and moves the files into the output directory after necessary information is gathered 
         * Parameters:
         *      -> file: String containing the path of the audio file
         *      -> albumInfo: Structure containing the information gathered for an individual album
         *      -> outPath: Parent directory of where the new sorted files and folders will be stored
         *      -> trackTitle: String containing the title of the given audio file
         *      -> fileType: String containing the file type in order to append it to the end of the final path
         */
        private void sortFile(string file, AlbumInfo albumInfo, string outPath, string trackTitle, string fileType)
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


        private void updateUI(Form1 form, string album, string picture, string artist)
        {
            if(artist != null)
                form.artistLabel.Text = artist;
            if(album != null)
                form.albumName.Text = album;
            if(picture != null)
                form.pictureBox1.Load(picture);
        }


        private string getAlbumName(TagLib.File audioFile)
        {
            string albumName;
            albumName = audioFile.Tag.Album;
            return albumName;
        }


        private string getArtistName(TagLib.File audioFile)
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


        private string getAlbumArtwork(TagLib.File audioFile, string artist2, string album)
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


        private string removeBannedChars(string infoString)
        {
            foreach (string character in bannedChars)
            {
                if(infoString != null)
                {
                    infoString = infoString.Replace(character, "");
                }
            }
            return infoString;
        }


        private TagLib.File generateAudioFile(string file)
        {
            TagLib.File audioFile = null;
            if (file.Contains(".mp3"))
            {
                audioFile = TagLib.File.Create(file);
            }
            else if (file.Contains(".flac"))
            {
                audioFile = TagLib.Flac.File.Create(file);
            }
            return audioFile;

        }

        private string getFileType(string file)
        {
            string fileType = null;
            if (file.Contains(".mp3"))
            {
                fileType = ".mp3";
            }
            else if (file.Contains(".flac"))
            {
                fileType = ".flac";
            }
            return fileType;
        }

        private void updateDatabase
        {

        }
    }
}
