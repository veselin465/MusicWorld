using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Data.Entity
{
    public class CatalogSong
    {
        [ForeignKey("Catalog")]
        public string CatalogId { get; set; }

        public Catalog Catalog { get; set; }

        [ForeignKey("Song")]
        public string SongId { get; set; }

        public Song Song { get; set; }

    }
}
