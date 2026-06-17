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
    /// Interaction logic for AddNewTrackWindow1.xaml
    /// </summary>
    public partial class AddNewTrackWindow1 : Window
    {
        public AddNewTrackWindow1()
        {
            InitializeComponent();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //перевірка чи правильно введений рік
            if (!int.TryParse(TrackYearInput.Text, out int releaseYear))
            {
                MessageBox.Show("Рік виходу повинен бути числовим значенням, будь ласка введіть правильний формат року");
                return;
            }
            if(string.IsNullOrEmpty(TrackAlbumInput.Text) || string.IsNullOrEmpty(TrackAuthorInput.Text) || 
                string.IsNullOrEmpty(TrackTitleInput.Text))
            {
                MessageBox.Show("У вікні є незаповнені поля, будь ласка заповніть їх");
                return;
            }
            //створюємо музичний трек
            MusicTrack newTrack = new MusicTrack(0,TrackAuthorInput.Text,TrackTitleInput.Text,TrackAlbumInput.Text,releaseYear);
            //тестовий юзер
            DataEditor editor = new DataEditor();

            if (editor.AddTrack(newTrack))
            {
                this.DialogResult = true;
                this.Close();
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TrackTitleInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TrackTitlePlaceholder.Visibility = string.IsNullOrWhiteSpace(TrackTitleInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TrackAuthorInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TrackAuthorPlaceholder.Visibility = string.IsNullOrWhiteSpace(TrackAuthorInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TrackAlbumInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TrackAlbumPlaceholder.Visibility = string.IsNullOrWhiteSpace(TrackAlbumInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TrackYearInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TrackYearPlaceholder.Visibility = string.IsNullOrWhiteSpace(TrackYearInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
