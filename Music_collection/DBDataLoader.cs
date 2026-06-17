using System;
using System.IO;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Music_collection
{
    class DBDataLoader
    {
        //енум для типу завантаження
        public enum LoadDataType { UserData, AllData, RawTracks }
        public static string SqlFilePath {  get; private set; }
        private string selectQuery;
        //завантаження доступу до БД
        public static void ParseSqlFile()
        {
            string filePath = "C:\\Users\\Користувач\\Desktop\\root.txt";
            string password;
            using (StreamReader sr = new StreamReader(filePath))
            {
                password = sr.ReadToEnd();
                SqlFilePath = $"Server = localhost; Database = music_collection; Uid = root; Pwd = {password};";
            }
        }
        //завантаження альбомів музичних треків
        public List<TrackList> LoadAlbum(LoadDataType type, int currentUserId = 0)
        {
            //тимчасові змінні для збереження даних і ініціалізації треків у конструкторі
            int id, releaseYear;
            string author, albumTtitle;
            bool isOfficial;
            //для збереження треків
            List<TrackList> trackList = new List<TrackList>();
            //якщо передано щавантаження дати користувача то один запит, якщо ні то інший
            if (type == LoadDataType.UserData)
            {
                selectQuery = "SELECT a.* FROM albums a JOIN user_albums ua ON a.id_album = ua.album_id WHERE ua.user_id = @uid";
            }
            else
            {
                selectQuery = "SELECT * FROM albums WHERE is_official = 1";
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                    {
                        if (type == LoadDataType.UserData)//якщо тип дані користувача, то додаємо переданий параметр юзер айді в запис скл
                        {
                            cmd.Parameters.AddWithValue("@uid", currentUserId);
                        }
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = Convert.ToInt32(reader["id_album"]);
                                releaseYear = Convert.ToInt32(reader["release_year"]);
                                author = reader["artist"].ToString();
                                albumTtitle = reader["album_title"].ToString();
                                isOfficial = Convert.ToBoolean(reader["is_official"]);
                                trackList.Add(new TrackList(id, albumTtitle, author, releaseYear, isOfficial));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Connection error: {ex.Message}");//для кожного мтеоду в файлі змінено з мессаджбоксу на кидання
                                                                       //помилки
            }
            return trackList;
        }
        //завантаження музичних треків
        public List<MusicTrack> LoadTracks(LoadDataType type, int currentAlbumId = 0)
        {
            //додаткові дані для створення музичних треків у конструкторі
            int id, releaseYear;
            string author, trackTtitle, album;
            //список муз. треків
            List<MusicTrack> trackList = new List<MusicTrack>();


            if (type == LoadDataType.UserData)
            {
                selectQuery = @"SELECT mt.* FROM musictracks mt JOIN album_tracks at ON mt.Id = at.track_id WHERE at.album_id = @aid";
            }
            else if (type == LoadDataType.AllData)
            {
                selectQuery = @"SELECT mt.* FROM musictracks mt JOIN album_tracks at ON mt.Id = at.track_id";
            }
            else if(type == LoadDataType.RawTracks)
            {
                selectQuery = @"SELECT * FROM musictracks";
            }
            try
            {
                using (MySqlConnection conn = new MySqlConnection(SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                    {
                        if (type == LoadDataType.UserData)//якщо тип дані користувача, то додаємо переданий параметр юзер айді в запис скл
                        {
                            cmd.Parameters.AddWithValue("@aid", currentAlbumId);
                        }
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = Convert.ToInt32(reader["Id"]);
                                releaseYear = Convert.ToInt32(reader["ReleaseYear"]);
                                author = reader["Author"].ToString();
                                trackTtitle = reader["Title"].ToString();
                                album = reader["Album"].ToString();
                                trackList.Add(new MusicTrack(id, author, trackTtitle, album, releaseYear));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Connection error: {ex.Message}");
            }
            return trackList;
        }
        //додано метод для повернення айді треків за одним конкретним альбомом
        public List<int> GetTrackIdsByAlbum(int albumId)
        {
            List<int> ids = new List<int>();
            string query = "SELECT track_id FROM album_tracks WHERE album_id = @aid;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@aid", albumId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ids.Add(Convert.ToInt32(reader["track_id"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); 
            }
            return ids;
        }

    }
}