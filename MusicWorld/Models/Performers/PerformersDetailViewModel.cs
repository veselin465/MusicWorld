using MusicWorld.Models.Albums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Performers
{
    public class PerformersDetailViewModel
    {

        public PerformersViewModel PerformerInformation { get; set; }

        public ICollection<AlbumsDetailViewModel> Albums { get; set; }

    }
}
