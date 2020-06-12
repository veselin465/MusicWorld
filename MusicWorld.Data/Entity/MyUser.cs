using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Data.Entity
{
    public class MyUser : IdentityUser<string>
    {

        [Required]
        [Display(Name = "First name")]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string LastName { get; set; }

        public virtual ICollection<Catalog> Catalogs { get; set; }

    }
}
