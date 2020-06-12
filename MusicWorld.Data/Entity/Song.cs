using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Data.Entity
{
    public class Song
    {

        public string Id { get; set; }

        [Required]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Duration")]
        public int DurationSeconds { get; set; }

        public string AlbumId { get; set; }

        public Album Album { get; set; }

        public string PerformerId { get; set; }

        public Performer Performer { get; set; }

        public virtual ICollection<CatalogSong> CatalogSongs { get; set; }

    }
}
