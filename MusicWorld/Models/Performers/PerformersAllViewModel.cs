using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Performers
{
    public class PerformersAllViewModel
    {
        public ICollection<PerformersViewModel> Performers { get; set; }

        public string SearchWord { get; set; }

    }
}
