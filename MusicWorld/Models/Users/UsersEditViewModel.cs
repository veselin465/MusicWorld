using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWorld.Models.Users
{
    public class UsersEditViewModel
    {

        public UsersEditViewModel()
        {
            ErrorMessages = new List<string>();
        }

        public string Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<string> ErrorMessages { get; set; }

    }
}
