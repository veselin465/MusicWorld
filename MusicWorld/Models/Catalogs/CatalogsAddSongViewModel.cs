using MusicWorld.Models.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Catalogs
{
    public class CatalogsAddSongViewModel
    {


        public string CatalogId { get; set; }

        public string SongId { get; set; }

        public ICollection<SongsViewModel> Songs { get; set; }

    }
}
