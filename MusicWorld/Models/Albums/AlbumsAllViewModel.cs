using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Albums
{
    public class AlbumsAllViewModel
    {
        public ICollection<AlbumsViewModel> Albums { get; set; }

        public string SearchWord { get; set; }

    }
}
