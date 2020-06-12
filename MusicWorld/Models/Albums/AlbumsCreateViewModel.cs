using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Albums
{
    public class AlbumsCreateViewModel
    {

        [Required]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string Name { get; set; }

        [Required]
        public string PerformerId { get; set; }

        [Display(Name = "Date of release")]
        [DisplayFormat(ApplyFormatInEditMode = true,NullDisplayText = "Current Date", DataFormatString = "{0:MM/dd/yyyy  HH:mm}")]
        public DateTime? DateOfRelease { get; set; }

    }
}
