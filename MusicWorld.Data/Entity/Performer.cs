using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Data.Entity
{
    public class Performer
    {

        public string Id { get; set; }

        [Required]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string Name { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Description length should be no more than 255 characters!")]
        public string Description { get; set; }

        public virtual ICollection<Album> Albums { get; set; }

    }
}
