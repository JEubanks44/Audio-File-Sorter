using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Diagnostics;
namespace Soulseek_Sorter
{
    class Database
    {
        static SqlConnection connection = new SqlConnection("Data Source = JOSEPHSPC\\JESERVER; Initial Catalog = MusicSorterDatabase; Integrated Security = True; Pooling=False");

        public Database()
        {
            if (connection == null)
            {
                Debug.WriteLine("CONNECTION TO DATABASE FAILED!");
            }
            else
            {
                Debug.WriteLine("CONNECTION TO DATABASE SUCCESSFUL!\n");
                Debug.WriteLine("CONNECTION STRING: " + connection.ConnectionString);
                Debug.WriteLine("DATABASE: " + connection.Database);
                Debug.WriteLine("Data Source: " + connection.DataSource);
            }
        }

        private int getArtistKeyID(string artist)
        {
            SqlCommand getArtistID = new SqlCommand("SELECT Id FROM Artists WHERE ([Name] = @Name)");
            getArtistID.Connection = connection;
            getArtistID.Parameters.AddWithValue("@Name", artist.ToUpper());
            connection.Open();
            int artistKeyID = (int)getArtistID.ExecuteScalar();
            connection.Close();
            return artistKeyID;
        }


        public bool checkArtistExists(string artist)
        {
            SqlCommand checkArtist = new SqlCommand("SELECT COUNT(*) FROM Artists WHERE ([Name] = @Name)");
            checkArtist.Connection = connection;
            checkArtist.Parameters.AddWithValue("@Name", artist.ToUpper());
            connection.Open();
            int check = (int)checkArtist.ExecuteScalar();
            connection.Close();
            if (check > 0)
            {
                Debug.WriteLine("ARTIST: " + artist.ToUpper() + " EXISTS IN DATABASE");
                return true;
            }
            return false;
        }


        public void addArtist(string artist)
        {
            if (checkArtistExists(artist))
                return;

            SqlCommand insertArtist = new SqlCommand("INSERT INTO Artists(Name) VALUES(@Name)");
            insertArtist.Connection = connection;
            insertArtist.Parameters.AddWithValue("@Name", artist.ToUpper());

            connection.Open();
            insertArtist.ExecuteNonQuery();
            connection.Close();
            Debug.WriteLine("ADDED ARTIST: " + artist.ToUpper());
        }


        public void deleteArtist(string artist)
        {
            if (!checkArtistExists(artist))
                return;

            SqlCommand deleteArtist = new SqlCommand("DELETE FROM Artists WHERE ([Name] = @Name)");
            SqlCommand deleteChildren = new SqlCommand("DELETE FROM Albums WHERE ArtistId = @ArtistId");
            deleteArtist.Connection = connection;
            deleteChildren.Connection = connection;

            deleteArtist.Parameters.AddWithValue("@Name", artist.ToUpper());
            deleteChildren.Parameters.AddWithValue("@ArtistId", getArtistKeyID(artist));
            connection.Open();
            deleteChildren.ExecuteNonQuery();
            deleteArtist.ExecuteNonQuery();
            connection.Close();
            Debug.WriteLine("DELETED ARTIST: " + artist.ToUpper());
        }
        

        public bool checkAlbumExists(string artist, string album)
        {
            if (!checkArtistExists(artist))
                return false;

            SqlCommand checkArtist = new SqlCommand("SELECT COUNT(*) FROM Albums WHERE (Title = @Title) AND (ArtistId = @ArtistId)");
            checkArtist.Connection = connection;
            checkArtist.Parameters.AddWithValue("@Title", album.ToUpper());
            checkArtist.Parameters.AddWithValue("@ArtistId", getArtistKeyID(artist));
            connection.Open();
            int check = (int)checkArtist.ExecuteScalar();
            connection.Close();
            if (check > 0)
            {
                Debug.WriteLine("ALBUM: " + album.ToUpper() + " EXISTS IN DATABASE");
                return true;
            }
            return false;
        }


        public void addAlbum(string artist, string album)
        {
            if (!checkArtistExists(artist))
                addArtist(artist);

            if (checkAlbumExists(artist, album))
                return;

            SqlCommand insertArtist = new SqlCommand("INSERT INTO Albums(Title, ArtistId) VALUES(@Title, @ArtistId)");
            insertArtist.Connection = connection;
            insertArtist.Parameters.AddWithValue("@Title", album.ToUpper());
            insertArtist.Parameters.AddWithValue("@ArtistId", getArtistKeyID(artist));
            connection.Open();
            insertArtist.ExecuteNonQuery();
            connection.Close();
            Debug.WriteLine("ADDED ALBUM: " + album.ToUpper());
        }

        public void updateAlbumWithInfo(string artist, string album, string artURL)
        {
            if (!checkAlbumExists(artist, album))
                addAlbum(artist, album);
            if(artURL == null)
            {
                artURL = "";
            }
            SqlCommand updateInfo = new SqlCommand("UPDATE Albums SET ArtURL = @URL WHERE (Title = @Title) AND (ArtistId = @ArtistId)");
            updateInfo.Parameters.AddWithValue("@URL", artURL);
            updateInfo.Parameters.AddWithValue("@Title", album.ToUpper());
            updateInfo.Parameters.AddWithValue("ArtistId", getArtistKeyID(artist));
            updateInfo.Connection = connection;
            connection.Open();
            updateInfo.ExecuteNonQuery();
            connection.Close();
            Debug.WriteLine("UPDATED ALBUM: " + album.ToUpper());
        }

        
    }
}
