using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TrackList> _loadedTracks;
        private List<TrackList> _resultOfFiltering;
        private bool _isDBLoaded = false;
        public MainWindow()
        {
            InitializeComponent();
            ApplyPermissions();
            LoadUserAlbum();
        }
        private void ApplyPermissions()
        {
            if (App.CurrentUser == null || !App.CurrentUser.IsAdmin)
            {
                if(UserAddAlbomButton != null)
                    UserAddAlbomButton.Visibility = Visibility.Visible;
                if (UserMain1WindowController.AddAlbumButton != null)
                    UserMain1WindowController.AddAlbumButton.Visibility = Visibility.Collapsed;
                if(UserMain1WindowController.AddTrackButton != null)
                    UserMain1WindowController.AddTrackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (UserAddAlbomButton != null)
                    UserAddAlbomButton.Visibility = Visibility.Collapsed;
                if (UserMain1WindowController.AddAlbumButton != null)
                    UserMain1WindowController.AddAlbumButton.Visibility = Visibility.Visible;
                if (UserMain1WindowController.AddTrackButton != null)
                    UserMain1WindowController.AddTrackButton.Visibility = Visibility.Visible;
            }
        }
        public void LoadUserAlbum()
        {
            if (!_isDBLoaded)
            {
                try
                {
                    DBDataLoader loader = new DBDataLoader();
                    _loadedTracks = loader.LoadAlbum(DBDataLoader.LoadDataType.UserData, App.CurrentUser.UserId);
                    ListOfUserAlbums.ItemsSource = _loadedTracks;
                    _isDBLoaded = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else return;
        }

        //кнопка для повернення на домашню сторінку з усіма колекціями
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindowGrid.Visibility = Visibility.Collapsed;
            UserMain1WindowController.Visibility = Visibility.Visible;
            UserMain1WindowController.LoadUserMainWindow1();
            _isDBLoaded = false;
        }
       
        //кнопка для редагування колекції альбомів 
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                TrackList trackList = (TrackList)btn.Tag;
                if (trackList.IsOfficial && !App.CurrentUser.IsAdmin)
                {
                    MessageBox.Show("У вас немає прав для редагування офіційних альбомів системи");
                    return;
                }
                EditAlbum editAlbum = new EditAlbum(trackList);
                editAlbum.Owner = Window.GetWindow(this);
                if (editAlbum.ShowDialog() == true)
                {
                    _isDBLoaded = false;
                    LoadUserAlbum();
                }
            }
        }

        private void AddUserAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewAlbomWindow addAlbumWin = new AddNewAlbomWindow();
            addAlbumWin.Owner = Window.GetWindow(this);
            if (addAlbumWin.ShowDialog() == true)
            {
                _isDBLoaded = false;
                LoadUserAlbum();
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
                    if (dataEditor.DeleteUserAlbum(albumId))
                    {
                        _isDBLoaded = false;
                        LoadUserAlbum();
                    }
                }
            }
        }

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
                _resultOfFiltering = searchFilter.FilterAlbums(AlbumTitleInput.Text, releaseYear, false);
                if (_resultOfFiltering.Count == 0)
                    MessageBox.Show("Не вдалося знайти альбоми за введеним фільтром, будь ласка, введіть інші значення");
                else
                    ListOfUserAlbums.ItemsSource = _resultOfFiltering;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ListOfUserAlbums.ItemsSource = _loadedTracks;
            }
        }
        //експорт до ворду
        private void ExportToMSButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IReportGenerator reportGenerator = new ReportGenerator();
                reportGenerator.AlbumsGenerateReport(_resultOfFiltering);
            }
            catch(Exception ex)
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
    }
}