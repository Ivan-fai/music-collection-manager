using Org.BouncyCastle.Crypto.Signers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Music_collection
{
    /// <summary>
    /// Interaction logic for LoginWindow1.xaml
    /// </summary>
    public partial class LoginWindow1 : Window
    {
        private IAuthManager _authManager = new AuthManager();
        public LoginWindow1()
        {
            InitializeComponent();
            DBDataLoader.ParseSqlFile();//отримуємо шлях до файлу БД з початку програми
        }
        //обробка натиску кнопки входу
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //заносимо в змінну поточного користувача
                App.CurrentUser = _authManager.SignIn(LoginInput.Text, PasswordInput.Password);
                if (App.CurrentUser != null)
                {
                    MessageBox.Show($"Ласкаво просимо, користувач {App.CurrentUser.UserName}");
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else throw new Exception("Помилка входу, ім'я користувача або пароль введені неправильно.\nСпробуйте ввести правильні дані ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //обробка натиску кнопки регістрації
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegistrationWindowController.Visibility = Visibility.Visible;
            RegistrationWindowController.LoadRegistrationWindow(_authManager); 
        }

        private void LoginInput_TextChanged(object sender, EventArgs e)
        {
            LoginInputPlaceholder.Visibility =  string.IsNullOrWhiteSpace(LoginInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
