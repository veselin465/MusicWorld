using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Data.Entity
{
    public class Album
    {

        public string Id { get; set; }

        [Required]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of release")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfRelease { get; set; }

        public string PerformerId { get; set; }

        public Performer Performer { get; set; }

        public virtual ICollection<Song> Songs { get; set; }

    }
}
