using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Music_collection
{
    internal class SearchFilter
    {
        public List<TrackList> FilterAlbums(string albumName, int? releaseYear, bool isOfficialOnly)
        {
            int id, year;
            string author;
            bool isOfficial;
            if (string.IsNullOrWhiteSpace(albumName) && releaseYear == null)
            {
                throw new Exception("Критерії фільтру пусті, пошук неможливий. Будь ласка, для пошуку введіть значення у відповідні строки");
            }
            List<TrackList> filteredAlbums = new List<TrackList>();
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

            string query;
            if (isOfficialOnly)
            {
                query = "SELECT id_album, album_title, artist, release_year, is_official FROM albums WHERE is_official = 1";
            }
            else
            {
                query = @"SELECT a.id_album, a.album_title, a.artist, a.release_year, a.is_official 
                  FROM albums a JOIN user_albums ua ON a.id_album = ua.album_id WHERE ua.user_id = @user_id";

                sqlParameters.Add(new MySqlParameter("@user_id", App.CurrentUser.UserId));
            }

            //якщо ввели назву альбому
            if (!string.IsNullOrWhiteSpace(albumName))
            {
                query += " AND album_title LIKE @title";
                sqlParameters.Add(new MySqlParameter("@title", "%" + albumName.Trim() + "%"));
            }

            //якщо ввели рік
            if (releaseYear.HasValue)
            {
                query += " AND release_year = @release_year";
                sqlParameters.Add(new MySqlParameter("@release_year", releaseYear.Value ));
            }
            //запит в БД
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        foreach (var param in sqlParameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = Convert.ToInt32(reader["id_album"]);
                                albumName = reader["album_title"].ToString();
                                author = reader["artist"].ToString();
                                year = Convert.ToInt32(reader["release_year"]);
                                isOfficial = Convert.ToBoolean(reader["is_official"]);
                                filteredAlbums.Add(new TrackList(id, albumName, author, year, isOfficial));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return filteredAlbums;
        }
        public List<MusicTrack> FilterTracks(string trackName, int? releaseYear)
        {
            int id, year;
            string author, albumTitle;
            if (string.IsNullOrWhiteSpace(trackName) && releaseYear == null)
            {
                throw new Exception("Критерії фільтру пусті, пошук неможливий. Будь ласка, для пошуку введіть значення у відповідні строки");
            }
            List<MusicTrack> filteredTracks = new List<MusicTrack>();
            string query = "SELECT Id, Author, Title, Album, ReleaseYear FROM musictracks WHERE 1=1";

            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

            //якщо ввели назву альбому
            if (!string.IsNullOrWhiteSpace(trackName))
            {
                query += " AND Title LIKE @title";
                sqlParameters.Add(new MySqlParameter("@title", "%" + trackName.Trim() + "%"));
            }

            //якщо ввели рік
            if (releaseYear.HasValue)
            {
                query += " AND ReleaseYear = @release_year";
                sqlParameters.Add(new MySqlParameter("@release_year", releaseYear.Value));
            }
            //запит в БД
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        foreach (var param in sqlParameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = Convert.ToInt32(reader["Id"]);
                                trackName = reader["Title"].ToString();
                                author = reader["Author"].ToString();
                                year = Convert.ToInt32(reader["ReleaseYear"]);
                                albumTitle = reader["Album"].ToString();
                                filteredTracks.Add(new MusicTrack(id,author,trackName,albumTitle,year));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return filteredTracks;
        }
    }
}
