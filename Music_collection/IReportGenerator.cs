using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_collection
{
    internal interface IReportGenerator
    {
        void AlbumsGenerateReport(List<TrackList> albumsToExport);
        void TracksGenerateReport(List<MusicTrack> tracksToExport);
    }
}
