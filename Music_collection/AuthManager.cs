using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Music_collection
{
    internal class AuthManager : IAuthManager
    {
        //метод для перевірки коректності заповненості полів з різними варіантами
        private bool isCorreсtInput(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(password))
                throw new Exception("У формі є незаповнені поля, будь ласка заповніть їх");
            else if (string.IsNullOrWhiteSpace(login))
                throw new Exception("Поле логіну пусте, будь ласка заповніть його");
            else if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Поле паролю пусте, будь ласка заповніть його");
            else
                return true;
        }
        //перевірка чи існує логін у системі
        private bool isLoginExist(string login)
        {
            int data;
            string checkExisting = @"SELECT COUNT(*) FROM users WHERE username = BINARY @login";
            using(MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
            {
                conn.Open();
                using(MySqlCommand cmd = new MySqlCommand(checkExisting,conn))
                {
                    cmd.Parameters.AddWithValue("@login",login.Trim());
                    data = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return data > 0;//перевірка  винесена за межі юзінг
        }
        //метод для входу у систему
        public User? SignIn(string login, string password)
        {
            try
            {
                if (isCorreсtInput(login, password))
                {
                    string checkQuery = "SELECT id_user, is_admin FROM users WHERE username = BINARY @login AND password = BINARY @password";
                    using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(checkQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@login", login.Trim());
                            cmd.Parameters.AddWithValue("@password", password.Trim());
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int.TryParse(reader["id_user"].ToString(), out int id);
                                    bool isAdmin = Convert.ToBoolean(reader["is_admin"]);
                                    return new User(id, login, isAdmin);
                                }
                                else return null;
                            }
                        }
                    }
                }
                else
                    throw new Exception("Помилка вводу даних");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //метод для реєстрації у системі
        public bool SignUp(string login, string password)
        {
            try
            {
                if (isCorreсtInput(login, password))
                {
                    int row = 0;
                    if (!isLoginExist(login))
                    {
                        string insertQuery = @"INSERT INTO users (username, password) VALUES (@login, @password);";
                        using (MySqlConnection conn = new MySqlConnection(DBDataLoader.SqlFilePath))
                        {
                            conn.Open();
                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@login", login);
                                cmd.Parameters.AddWithValue("@password", password);
                                row = cmd.ExecuteNonQuery();
                            }
                        }
                        if (row > 0)
                            return true;
                        else throw new Exception("Помилка реєстрації, будь ласка спробйуте пізніше");
                    }
                    else throw new Exception("Такий логін уже зареєстровано в системі, будь ласка, оберіть інший");
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
