using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Songs
{
    public class SongsCreateViewModel
    {

        public SongsCreateViewModel()
        {
            ErrorMessages = new List<string>();
        }

        [Required]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string Name { get; set; }

        [Required]
        [Range(0,int.MaxValue, ErrorMessage = "Minutes should be a non-negative integer")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Seconds should be an integer between 0 and 59")]
        public int DurationSeconds { get; set; }

        [Required]
        public string AlbumId { get; set; }

        public string AlbumPerformerConcatenatedNames { get; set; }

        public ICollection<string> ErrorMessages { get; set; }

    }
}