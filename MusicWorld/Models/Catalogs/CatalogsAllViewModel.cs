using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Catalogs
{
    public class CatalogsAllViewModel
    {
        public ICollection<CatalogsViewModel> Catalogs { get; set; }

        public string SearchWord { get; set; }
    }
}
