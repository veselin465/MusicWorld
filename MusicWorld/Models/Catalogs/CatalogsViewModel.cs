using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Catalogs
{
    public class CatalogsViewModel
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Descritpion { get; set; }

        public string MyUserId { get; set; }

        [Display(Name = "Username")]
        public string MyUserName { get; set; }

        public int SongsCount { get; set; }

    }
}
