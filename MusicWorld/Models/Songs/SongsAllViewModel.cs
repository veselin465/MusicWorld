using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Songs
{
    public class SongsAllViewModel
    {
        public ICollection<SongsViewModel> Songs { get; set; }

        public string SearchWord { get; set; }

    }
}
