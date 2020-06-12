using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Songs
{
    public class SongsViewModel
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Duration { get; set; }

        public string AlbumId { get; set; }

        public string AlbumName { get; set; }

        public string PerformerId { get; set; }

        public string PerformerName { get; set; }

    }
}
