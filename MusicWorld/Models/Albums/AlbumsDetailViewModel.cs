using MusicWorld.Models.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Albums
{
    public class AlbumsDetailViewModel
    {

        public AlbumsViewModel AlbumInformation { get; set; }

        public ICollection<SongsViewModel> Songs { get; set; }

    }
}
