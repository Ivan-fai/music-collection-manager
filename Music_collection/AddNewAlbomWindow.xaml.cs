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
    /// Interaction logic for AddNewAlbomWindow.xaml
    /// </summary>
    public partial class AddNewAlbomWindow : Window
    {
        private List<int> _selectedTrackIds = new List<int>();
        public AddNewAlbomWindow()
        {
            InitializeComponent();
            LoadAllTracksToSelection();
        }
        private void LoadAllTracksToSelection()
        {
            try
            {
                DBDataLoader loader = new DBDataLoader();
                TracksSelectionListBox.ItemsSource = loader.LoadTracks(DBDataLoader.LoadDataType.RawTracks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження треків: {ex.Message}");
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Tag != null)
            {
                int trackId = (int)cb.Tag;
                if (!_selectedTrackIds.Contains(trackId))
                    _selectedTrackIds.Add(trackId); // Добавили ID в список
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Tag != null)
            {
                int trackId = (int)cb.Tag;
                _selectedTrackIds.Remove(trackId); // Убрали ID из списка, если галочку сняли
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AlbumYearInput.Text, out int releaseYear))
            {
                MessageBox.Show("Рік виходу повинен бути числом!");
                return;
            }

            if (string.IsNullOrEmpty(AlbumTitleInput.Text) || string.IsNullOrEmpty(AlbumArtistInput.Text))
            {
                MessageBox.Show("Заповніть назву та виконавця альбому!");
                return;
            }

            TrackList newAlbum = new TrackList(0, AlbumTitleInput.Text, AlbumArtistInput.Text, releaseYear, App.CurrentUser.IsAdmin);

            DataEditor editor = new DataEditor();
            bool isSuccess = false;

            if (App.CurrentUser.IsAdmin)
            {
                //адміністратор створив альбом
                isSuccess = editor.AddAlbumWithTracks(newAlbum, _selectedTrackIds);
            }
            else
            {
                //юзер створив альбом
                isSuccess = editor.AddUserAlbumWithTracks(newAlbum, _selectedTrackIds);
            }
            if (isSuccess)
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

        private void AlbumTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            AlbumTitlePlaceholder.Visibility = string.IsNullOrWhiteSpace(AlbumTitleInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AlbumArtistInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            AlbumArtistPlaceholder.Visibility = string.IsNullOrWhiteSpace(AlbumArtistInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AlbumYearInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            AlbumYearPlaceholder.Visibility = string.IsNullOrWhiteSpace(AlbumYearInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
