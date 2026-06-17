using MySql.Data.MySqlClient;
using System.Windows;

namespace Music_collection
{
    class DataEditor
    {
        private MusicTrack activeTrack;

        //передаємо користувача, щоб дізнатися його права доступу
        public DataEditor()
        {

        }
        //перевірка прав доступу 
        public bool CheckPermissions()
        {
            return App.CurrentUser != null && App.CurrentUser.IsAdmin;
        }
        //метод для додавання треку
        public bool AddTrack(MusicTrack musickTrack)
        {
            if (CheckPermissions())
            {
                try
                {
                    string selectQuery = @"INSERT INTO musictracks (Title, Author, Album, ReleaseYear)
                                                VALUES (@title, @author, @album, @year);";
                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@title", musickTrack.Title);
                            cmd.Parameters.AddWithValue("@author", musickTrack.Author);
                            cmd.Parameters.AddWithValue("@album", musickTrack.Album);
                            cmd.Parameters.AddWithValue("@year", musickTrack.ReleaseYear);
                            int countRows = cmd.ExecuteNonQuery();//повертає кількість рядків, які було створено або оновлено
                            //якщо додавання треку успішне
                            if (countRows > 0)
                            {
                                MessageBox.Show($"Музичний трек {musickTrack.Title} був успішно доданий");
                                return true;
                            }
                            else
                                MessageBox.Show("Error, can`t add musicktrack");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
                return false;
        }
        //внесення змін до треку
        public bool EditTrack(int id, MusicTrack updatedTrack)
        {
            if (CheckPermissions())
            {
                try
                {
                    string selectQuery = @"UPDATE musictracks SET Title = @title, Author = @author, Album = @album, ReleaseYear = @year 
                                            WHERE Id = @id;";
                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@title", updatedTrack.Title);
                            cmd.Parameters.AddWithValue("@author", updatedTrack.Author);
                            cmd.Parameters.AddWithValue("@album", updatedTrack.Album);
                            cmd.Parameters.AddWithValue("@year", updatedTrack.ReleaseYear);
                            cmd.Parameters.AddWithValue("@id", id);
                            int countRows = cmd.ExecuteNonQuery();

                            //якщо оновлення треку успішне
                            if (countRows > 0)
                            {
                                MessageBox.Show($"Музиний трек '{updatedTrack.Title}' успішно оновлено");
                                return true;
                            }
                            else
                                MessageBox.Show("Помилка, неможливо оновити дані про музичний трек");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
                return false;
        }
        //метод для видалення треку
        public bool DeleteTrack(int id)
        {
            if (CheckPermissions())
            {
                try
                {
                    string selectQuery = @"DELETE FROM musictracks WHERE Id = @id;";
                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            int countRows = cmd.ExecuteNonQuery();
                            if (countRows > 0)
                            {
                                MessageBox.Show($"Музичний терк видалено успішно");
                                return true;
                            }
                            else
                                MessageBox.Show("Помилка, неможливо видалити трек");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
                return false;
        }
        //метод додавання альбому з треками
        public bool AddAlbumWithTracks(TrackList album, List<int> selectedTrackIds)
        {
            if (CheckPermissions())
            {
                try
                {
                    //оскільки при створенні альбома БД сама встановлює потрібний айді за допомогою авто інкремента
                    //потрібно повернути його, щоб далі створити зв'язки у зв'язуючій БД альбомів та треків
                    int newAlbumId = 0;
                    string insertAlbumQuery = @"INSERT INTO albums (album_title, artist, release_year, is_official) 
                                   VALUES (@title, @artist, @year, @isOfficial);
                                   SELECT LAST_INSERT_ID();";

                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(insertAlbumQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@title", album.AlbumTitle);
                            cmd.Parameters.AddWithValue("@artist", album.Artist);
                            cmd.Parameters.AddWithValue("@year", album.ReleaseYear);
                            cmd.Parameters.AddWithValue("@isOfficial", 1);
                            newAlbumId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        //якщо створення успішне то створюємо зв'язки з треками, які були додані
                        if (newAlbumId > 0 && selectedTrackIds != null && selectedTrackIds.Count > 0)
                        {
                            string insertRelationsQuery = @"INSERT INTO album_tracks (album_id, track_id) VALUES (@albumId, @trackId);";
                            using (MySqlCommand relCmd = new MySqlCommand(insertRelationsQuery, conn))
                            {
                                //ініціалізаація параметрів
                                relCmd.Parameters.Add("@albumId", MySqlDbType.Int32).Value = newAlbumId;
                                relCmd.Parameters.Add("@trackId", MySqlDbType.Int32);

                                foreach (int trackId in selectedTrackIds)
                                {
                                    relCmd.Parameters["@trackId"].Value = trackId;
                                    relCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Альбом {album.AlbumTitle} створено");
                            return true;
                        }
                    }
                    MessageBox.Show($"Альбом '{album.AlbumTitle}' та його треки успішно додані!");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка додавання альбому: {ex.Message}");
                    return false;
                }
            }
            else
                return false;
        }
        public bool AddUserAlbumWithTracks(TrackList album, List<int> selectedTrackIds)
        {
            try
            {
                int newAlbumId = 0;
                string insertAlbumQuery = @"INSERT INTO albums (album_title, artist, release_year, is_official) 
                                VALUES (@title, @artist, @year, @isOfficial);
                                SELECT LAST_INSERT_ID();";

                using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(insertAlbumQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", album.AlbumTitle);
                        cmd.Parameters.AddWithValue("@artist", album.Artist);
                        cmd.Parameters.AddWithValue("@year", album.ReleaseYear);
                        cmd.Parameters.AddWithValue("@isOfficial", 0); //0 бо альбом юзера

                        newAlbumId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (newAlbumId > 0)
                    {
                        //прив'язуємо альбом до користувача
                        string insertUserRelationQuery = @"INSERT INTO user_albums (user_id, album_id) VALUES (@uid, @aid);";
                        using (MySqlCommand userRelCmd = new MySqlCommand(insertUserRelationQuery, conn))
                        {
                            userRelCmd.Parameters.AddWithValue("@uid", App.CurrentUser.UserId);
                            userRelCmd.Parameters.AddWithValue("@aid", newAlbumId);
                            userRelCmd.ExecuteNonQuery();
                        }
                        //пов'язуємо альбом та треки,які було вибрано користувачем
                        if (selectedTrackIds != null && selectedTrackIds.Count > 0)
                        {
                            string insertTracksQuery = @"INSERT INTO album_tracks (album_id, track_id) VALUES (@albumId, @trackId);";
                            using (MySqlCommand relCmd = new MySqlCommand(insertTracksQuery, conn))
                            {
                                relCmd.Parameters.Add("@albumId", MySqlDbType.Int32).Value = newAlbumId;
                                relCmd.Parameters.Add("@trackId", MySqlDbType.Int32);

                                foreach (int trackId in selectedTrackIds)
                                {
                                    relCmd.Parameters["@trackId"].Value = trackId;
                                    relCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        MessageBox.Show($"Ваш особистий альбом '{album.AlbumTitle}' успішно створено!");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення особистого альбому: {ex.Message}");
            }
            return false;
        }
        //метод для внесення змін у альбом
        public bool EditAlbumWithTracks(int id, TrackList trackList, List<int> selectedTrackIds)
        {
            if (CheckPermissions())
            {
                try
                {
                    string selectQuery = @"UPDATE albums SET album_title = @title, artist = @author, release_year = @year, 
                                            is_official = @is_official WHERE id_album = @id;";
                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@title", trackList.AlbumTitle);
                            cmd.Parameters.AddWithValue("@author", trackList.Artist);
                            cmd.Parameters.AddWithValue("@year", trackList.ReleaseYear);
                            cmd.Parameters.AddWithValue("@is_official", trackList.IsOfficial);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        //повністю очищаємо старі зв'язки цього альбому та усіх треків
                        string deleteRelationsQuery = "DELETE FROM album_tracks WHERE album_id = @albumId;";
                        using (MySqlCommand delCmd = new MySqlCommand(deleteRelationsQuery, conn))
                        {
                            delCmd.Parameters.AddWithValue("@albumId", id);
                            delCmd.ExecuteNonQuery();
                        }

                        //створюємо нові зв'язки альбому та усіх доданих пісень наново через переданий список пісень 
                        if (selectedTrackIds != null && selectedTrackIds.Count > 0)
                        {
                            string insertRelationsQuery = "INSERT INTO album_tracks (album_id, track_id) VALUES (@albumId, @trackId);";
                            using (MySqlCommand relCmd = new MySqlCommand(insertRelationsQuery, conn))
                            {
                                relCmd.Parameters.Add("@albumId", MySqlDbType.Int32).Value = id;
                                relCmd.Parameters.Add("@trackId", MySqlDbType.Int32);

                                foreach (int trackId in selectedTrackIds)
                                {
                                    relCmd.Parameters["@trackId"].Value = trackId;
                                    relCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    MessageBox.Show($"Альбом '{trackList.AlbumTitle}' успішно оновлено!");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка оновлення альбому: {ex.Message}");
                    return false;
                }
            }
            else
                return false;
        }
        public bool EditUserAlbumWithTracks(int id, TrackList trackList, List<int> selectedTrackIds)
        {
            try
            {
                string updateAlbumQuery = @"UPDATE albums SET album_title = @title, artist = @author, release_year = @year 
                                    WHERE id_album = @id AND is_official = 0;";
                using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(updateAlbumQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", trackList.AlbumTitle);
                        cmd.Parameters.AddWithValue("@author", trackList.Artist);
                        cmd.Parameters.AddWithValue("@year", trackList.ReleaseYear);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    //повністю очищаємо старі зв'язки цього альбому та усіх треків
                    string deleteRelationsQuery = "DELETE FROM album_tracks WHERE album_id = @albumId;";
                    using (MySqlCommand delCmd = new MySqlCommand(deleteRelationsQuery, conn))
                    {
                        delCmd.Parameters.AddWithValue("@albumId", id);
                        delCmd.ExecuteNonQuery();
                    }

                    //створюємо нові зв'язки альбому та усіх доданих пісень наново через переданий список пісень 
                    if (selectedTrackIds != null && selectedTrackIds.Count > 0)
                    {
                        string insertRelationsQuery = "INSERT INTO album_tracks (album_id, track_id) VALUES (@albumId, @trackId);";
                        using (MySqlCommand relCmd = new MySqlCommand(insertRelationsQuery, conn))
                        {
                            relCmd.Parameters.Add("@albumId", MySqlDbType.Int32).Value = id;
                            relCmd.Parameters.Add("@trackId", MySqlDbType.Int32);

                            foreach (int trackId in selectedTrackIds)
                            {
                                relCmd.Parameters["@trackId"].Value = trackId;
                                relCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                MessageBox.Show($"Ваш альбом '{trackList.AlbumTitle}' успішно оновлено!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення вашого альбому: {ex.Message}");
                return false;
            }
        }
        //метод видалення альбому
        public bool DeleteAlbum(int id)
        {
            if (CheckPermissions())
            {
                try
                {
                    //видаляємо зв'язки альбому з піснями а потім сам альбом, щоб не видалити пісні заодно
                    string deleteQuery = @"DELETE FROM album_tracks WHERE album_id = @id;
                               DELETE FROM albums WHERE id_album = @id;";

                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            int countRows = cmd.ExecuteNonQuery();

                            if (countRows > 0)
                            {
                                MessageBox.Show("Альбом успішно видалено");
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Помилка, не вдалося видалити альбом");
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні: {ex.Message}");
                    return false;
                }
            }
            else
                return false;
        }
        //метод видалення альбому для юзера
        public bool DeleteUserAlbum(int albumId)
        {
            try
            {
                string query = @"DELETE FROM user_albums WHERE user_id = @uid AND album_id = @aid;";

                using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", App.CurrentUser.UserId);
                        cmd.Parameters.AddWithValue("@aid", albumId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Альбом успішно видалено з вашої особистої колекції");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Помилка, не вдалося видалити альбом у вашій колекції");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні альбому: {ex.Message}");
                return false;
            }
        }
        //метод додавання альбому 
        public bool AddAlbumToUserCollection(int albumId)
        {
            try
            {
                string query = @"INSERT INTO user_albums (user_id, album_id) VALUES (@uid, @aid);";

                using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", App.CurrentUser.UserId);
                        cmd.Parameters.AddWithValue("@aid", albumId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Альбом успішно додано до вашої колекції");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні: {ex.Message}");
            }
            return false;
        }
    }
}