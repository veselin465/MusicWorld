using MusicWorld.Models.Performers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Users
{
    public class UsersAllViewModel
    {
        public ICollection<UsersViewModel> Users { get; set; }

    }
}
