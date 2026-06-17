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
    /// Interaction logic for UserMainWindow1.xaml
    /// </summary>
    public partial class UserMainWindow1 : UserControl
    {
        private bool _isDBLoaded = false;
        private List<TrackList> _filteredAlbums;
        private List<MusicTrack> _filteredTracks;
        private List<TrackList> _loadedAlbums;
        private List<MusicTrack> _loadedTracks;
        public UserMainWindow1()
        {
            InitializeComponent();
        }
        //метод завантаження домашнього вікна з усіма альбомами та колекціями
        public void LoadUserMainWindow1()
        {
            //перевірка щоб дані з БД завантажувалися тільки 1 раз
            if (_isDBLoaded)
                return;
            //завантаження даних про усі доступні треки та альбоми з БД
            try
            {
                DBDataLoader loader = new DBDataLoader();
                _loadedAlbums = loader.LoadAlbum(DBDataLoader.LoadDataType.AllData);
                _loadedTracks = loader.LoadTracks(DBDataLoader.LoadDataType.RawTracks);
                ListOfAllAlbums.ItemsSource = _loadedAlbums;
                ListOfAllMusickTracks.ItemsSource = _loadedTracks;
                _isDBLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //метод для кнопки повернення до особистого профілю
        private void MainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWin = Window.GetWindow(this) as MainWindow;
            if (mainWin != null)
            {
                this.Visibility = Visibility.Collapsed;
                mainWin.MainWindowGrid.Visibility = Visibility.Visible;
                mainWin.LoadUserAlbum();
            }
        }
        //метод для кнопки додавання альбому у особистий список альбомів
        private void UserAddAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int albumId = (int)btn.Tag;
                DataEditor dataEditor = new DataEditor();
                dataEditor.AddAlbumToUserCollection(albumId);
            }
        }
        //метод для додавання треку до особистого альбому
        private void UserAddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int trackId = (int)btn.Tag;
                //тут будет код для додавання треку до якогось особистого альбому
            }
        }
        //метод для додавання альбому
        private void AddAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewAlbomWindow addAlbumWin = new AddNewAlbomWindow();
            addAlbumWin.Owner = Window.GetWindow(this);

            if (addAlbumWin.ShowDialog() == true)
            {
                _isDBLoaded = false;
                LoadUserMainWindow1();
            }
        }
        private void AlbumEditorButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                TrackList trackList = (TrackList)btn.Tag;
                EditAlbum editAlbum = new EditAlbum(trackList);
                editAlbum.Owner = Window.GetWindow(this);
                if (editAlbum.ShowDialog() == true)
                {
                    _isDBLoaded = false;
                    LoadUserMainWindow1();
                }
            }
        }
        private void DeleteAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int albumId = (int)btn.Tag;
                MessageBoxResult result = MessageBox.Show("Ви дійсно хочете видалити цей альбом?",
                    "Підтвердження видалення", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DataEditor dataEditor = new DataEditor();
                    if (dataEditor.DeleteAlbum(albumId))
                    {
                        _isDBLoaded = false;
                        LoadUserMainWindow1();
                    }
                }
            }
        }
        private void DeleteTrackButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int trackId = (int)btn.Tag;
                MessageBoxResult result = MessageBox.Show("Ви дійсно хочете видалити цю музичну композицію?",
                    "Підтвердження видалення", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DataEditor dataEditor = new DataEditor();
                    if (dataEditor.DeleteTrack(trackId))
                    {                        
                        _isDBLoaded = false;
                        LoadUserMainWindow1();
                    }
                }
            }
        }
        private void TrackEditorButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                MusicTrack musicTrack = (MusicTrack)btn.Tag;
                EditTrack editTrack = new EditTrack(musicTrack);
                editTrack.Owner = Window.GetWindow(this);
                if (editTrack.ShowDialog() == true)
                {
                    _isDBLoaded = false;
                    LoadUserMainWindow1();
                }
            }
        }
        //метод для додавання треку
        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTrackWindow1 addTrackWindow = new AddNewTrackWindow1();
            addTrackWindow.Owner = Window.GetWindow(this);
            if(addTrackWindow.ShowDialog() == true)
            {
                _isDBLoaded = false;
                LoadUserMainWindow1();
            }
        }
        //пошук альбому за фільтром 
        private void AlbumFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? releaseYear;

                if (string.IsNullOrWhiteSpace(AlbumYearInput.Text))
                    releaseYear = null;
                else if (int.TryParse(AlbumYearInput.Text, out int _releaseYear))
                    releaseYear = _releaseYear;
                else
                    throw new Exception("У поле року виходу введено не числове значення, будь ласка введіть рік правильно");

                SearchFilter searchFilter = new SearchFilter();
                _filteredAlbums = searchFilter.FilterAlbums(AlbumTitleInput.Text, releaseYear, true);
                if (_filteredAlbums.Count == 0)
                    throw new Exception("Не вдалося знайти альбоми за введеним фільтром, будь ласка, введіть інші значення");
                else
                    ListOfAllAlbums.ItemsSource = _filteredAlbums;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ListOfAllAlbums.ItemsSource = _loadedAlbums;
            }
        }
        //пошук трека за фільтром
        private void TrackFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? releaseYear;

                if (string.IsNullOrWhiteSpace(TrackYearInput.Text))
                    releaseYear = null;
                else if (int.TryParse(TrackYearInput.Text, out int _releaseYear))
                    releaseYear = _releaseYear;
                else
                    throw new Exception("У поле року виходу введено не числове значення, будь ласка введіть рік правильно");

                SearchFilter searchFilter = new SearchFilter();
                _filteredTracks = searchFilter.FilterTracks(TrackTitleInput.Text, releaseYear);
                if (_filteredTracks.Count > 0)
                    ListOfAllMusickTracks.ItemsSource = _filteredTracks;
                else
                    throw new Exception("Не вдалося знайти музичні композиції за введеним фільтром, будь ласка, введіть інші значення");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ListOfAllMusickTracks.ItemsSource= _loadedTracks;
            }
        }

        private void ExportAlbumsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IReportGenerator reportGenerator = new ReportGenerator();
                reportGenerator.AlbumsGenerateReport(_filteredAlbums);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void ExportTracksButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IReportGenerator reportGenerator = new ReportGenerator();
                reportGenerator.TracksGenerateReport(_filteredTracks);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AlbumTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            AlbumTitlePlaceholder.Visibility = string.IsNullOrWhiteSpace(AlbumTitleInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void AlbumYearInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            AlbumYearPlaceholder.Visibility = string.IsNullOrWhiteSpace(AlbumYearInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void TrackTitleInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TrackTitlePlaceholder.Visibility = string.IsNullOrWhiteSpace(TrackTitleInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void TrackYearInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TrackYearPlaceholder.Visibility = string.IsNullOrWhiteSpace(TrackYearInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
