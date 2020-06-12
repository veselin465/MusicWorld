using MusicWorld.Models.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Catalogs
{
    public class CatalogsDetailViewModel
    {

        public CatalogsViewModel CatalogInformation { get; set; }

        public ICollection<SongsViewModel> NotAddedSongs { get; set; }

        public ICollection<SongsViewModel> CurrentCatalogSongs { get; set; }

    }
}
