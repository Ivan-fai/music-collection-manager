using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Music_collection
{
    public class TrackList
    {
        public int Id { get;private set; }
        public string AlbumTitle { get; private set; }
        public string Artist { get; private set; }
        public int ReleaseYear { get; private set; }
        public bool IsOfficial { get; private set; }
        public TrackList(int id,string albumTitle,string artist,int releaseYear, bool isOfficial)
        {
            Id = id;
            AlbumTitle = albumTitle;
            Artist = artist;
            ReleaseYear = releaseYear;
            IsOfficial = isOfficial;
        }
    }
}
