using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Catalogs
{
    public class CatalogsEditViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(64, ErrorMessage = "Name length should be no more than 64 characters!")]
        public string Name { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Name length should be no more than 255 characters!")]
        public string Descritpion { get; set; }

    }
}
