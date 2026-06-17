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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Music_collection
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : UserControl
    {
        private IAuthManager _authManager;
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        public void LoadRegistrationWindow(IAuthManager authManager)
        {
            _authManager = authManager;
        }
        public void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isNewUser = _authManager.SignUp(LoginInput.Text, PasswordInput.Password);
                if (isNewUser)
                {
                    this.Visibility = Visibility.Collapsed;
                    LoginWindow1 loginWindow1 = Window.GetWindow(this) as LoginWindow1;
                    if (loginWindow1 != null)
                    {
                        MessageBox.Show("Ви успішно зареєструвались");
                        this.Visibility = Visibility.Collapsed;
                        loginWindow1.LoginGrid.Visibility = Visibility.Visible;
                        loginWindow1.LoginInput.Text = LoginInput.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow1 loginWindow1 = Window.GetWindow(this) as LoginWindow1;
            if (loginWindow1 != null)
            {
                this.Visibility = Visibility.Collapsed; 
                loginWindow1.LoginGrid.Visibility = Visibility.Visible;
            }
        }
        private void LoginInput_TextChanged(object sender, EventArgs e)
        {
            LoginInputPlaceholder.Visibility = string.IsNullOrWhiteSpace(LoginInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
