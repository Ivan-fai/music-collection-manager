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
    /// Interaction logic for EditTrack.xaml
    /// </summary>
    public partial class EditTrack : Window
    {
        private int currentId;
        public EditTrack(MusicTrack musicTrack)
        {
            InitializeComponent();
            currentId = musicTrack.Id;
            TrackTitleEdit.Text = musicTrack.Title;
            TrackAuthorEdit.Text = musicTrack.Author;
            TrackAlbumEdit.Text = musicTrack.Album;
            TrackYearEdit.Text = musicTrack.ReleaseYear.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //перевірка чи правильно введений рік
            if (!int.TryParse(TrackYearEdit.Text, out int releaseYear))
            {
                MessageBox.Show("Рік виходу повинен бути числовим значенням, будь ласка введіть правильний формат року");
                return;
            }
            if (string.IsNullOrEmpty(TrackAlbumEdit.Text) || string.IsNullOrEmpty(TrackAuthorEdit.Text) ||
                string.IsNullOrEmpty(TrackTitleEdit.Text))
            {
                MessageBox.Show("У вікні є незаповнені поля, будь ласка заповніть їх");
                return;
            }
            //створюємо музичний трек
            MusicTrack newTrack = new MusicTrack(0, TrackAuthorEdit.Text, TrackTitleEdit.Text, TrackAlbumEdit.Text, releaseYear);
            DataEditor editor = new DataEditor();

            if (editor.EditTrack(currentId, newTrack)) 
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
