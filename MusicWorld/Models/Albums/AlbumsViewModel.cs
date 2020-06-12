using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Albums
{
    public class AlbumsViewModel
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string PerformerId { get; set; }

        public string PerformerName { get; set; }

        public DateTime DateOfRelease { get; set; }

        public int SongsNumber { get; set; }

    }
}
