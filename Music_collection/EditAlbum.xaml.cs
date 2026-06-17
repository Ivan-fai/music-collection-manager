using Music_collection;
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
    /// Interaction logic for EditAlbum.xaml
    /// </summary>
    public partial class EditAlbum : Window
    {
        private TrackList _trackList;
        public EditAlbum(TrackList trackList)
        {
            InitializeComponent();

            _trackList = trackList;
            //показ початкової інформації про альбом
            AlbumTitleInput.Text = _trackList.AlbumTitle;
            AlbumArtistInput.Text = _trackList.Artist;
            AlbumYearInput.Text = _trackList.ReleaseYear.ToString();

            LoadAllTracksInAlbum();
        }
        //метод для завантаження всіх треків в вибраному альбомі
        private void LoadAllTracksInAlbum()
        {
            try
            {
                //(додати перевірку чи були завантажені дані попередньо)
                //завантажуємо треки з БД
                DBDataLoader dBDataLoader = new DBDataLoader();
                List<MusicTrack> allTracks = dBDataLoader.LoadTracks(DBDataLoader.LoadDataType.RawTracks);
                //завантаження треків, які знаходяться саме в вибраному альбомі 
                List<int> currentAlbumTrackIds = dBDataLoader.GetTrackIdsByAlbum(_trackList.Id);

                List<MusicTrack> selectionList = new List<MusicTrack>();

                foreach (var track in allTracks)
                {
                    selectionList.Add(new MusicTrack(track.Id, track.Author, track.Title, track.Album,
                        track.ReleaseYear, currentAlbumTrackIds.Contains(track.Id)));
                }
                TracksSelectionListBox.ItemsSource = selectionList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AlbumYearInput.Text, out int releaseYear))
            {
                MessageBox.Show("Рік повинен бути числом, будь ласка введіть інше значення");
                return;
            }
            if (string.IsNullOrEmpty(AlbumTitleInput.Text) || string.IsNullOrEmpty(AlbumArtistInput.Text))
            {
                MessageBox.Show("У вікні є незаповнені поля, будь ласка заповніть їх");
                return;
            }
            //список для треків які точно будуть у альбомі після збереження (з галочкою)
            List<int> selectedIds = new List<int>();

            foreach (MusicTrack item in TracksSelectionListBox.ItemsSource)
            {
                //якщо трек обраний, додаємо до списку збереження
                if (item.IsSelected)
                {
                    selectedIds.Add(item.Id);
                }
            }
            //створення нового оновленого альбому
            TrackList updatedAlbum = new TrackList(_trackList.Id, AlbumTitleInput.Text, AlbumArtistInput.Text,
                releaseYear, _trackList.IsOfficial);
            DataEditor editor = new DataEditor();
            bool isSuccess = false;

            if (App.CurrentUser.IsAdmin)
            {
                //зміна від ролі адміна
                isSuccess = editor.EditAlbumWithTracks(_trackList.Id, updatedAlbum, selectedIds);
            }
            else
            {
                //зміни у своєму альбомі від користувача
                isSuccess = editor.EditUserAlbumWithTracks(_trackList.Id, updatedAlbum, selectedIds);
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
    }
}
