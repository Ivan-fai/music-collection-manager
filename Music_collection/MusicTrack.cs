using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Music_collection
{
    public class MusicTrack
    {
        public int Id { get; private set; }
        public string Author { get; private set; }
        public string Title { get; private set; }
        public string Album { get; private set; }
        public int ReleaseYear { get; private set; }
        public bool IsSelected { get; set; }//додано для перевірки чи обраний трек у меню додавання треків
        //додано необов'язковий параметр перевірки чи обраний трек
        public MusicTrack(int _id,string _author,string _title, string _album, int _year,bool _isSelected = false)
        {
            Id = _id;
            Author = _author;
            Title = _title;
            Album = _album;
            ReleaseYear = _year;
            IsSelected = _isSelected;
        }
    }
}
