using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicWorld.Data;
using MusicWorld.Data.Entity;
using MusicWorld.Models.Users;

namespace MusicWorld.Controllers
{
    public class UserController : Controller
    {

        private readonly MusicWorldDbContext _dbContext;

        public UserController(MusicWorldDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {

            var allUsers = _dbContext.Users.ToList();
            allUsers = allUsers.OrderBy(x => x.UserName).ToList();

            var model = new UsersAllViewModel()
            {
                Users = new List<UsersViewModel>()
            };

            foreach (var user in allUsers)
            {

                string roleId = _dbContext.UserRoles.Where(r => r.UserId == user.Id).FirstOrDefault().RoleId;
                string roleName = (await _dbContext.Roles.FindAsync(roleId)).Name;

                var singleViewModel = new UsersViewModel()
                {
                    Id = user.Id,
                    FirstName = ShortenTooLongStrings(user.FirstName),
                    LastName = ShortenTooLongStrings(user.LastName),
                    Username = ShortenTooLongStrings(user.UserName),
                    Role = roleName
                };

                model.Users.Add(singleViewModel);

            }

            return View(model);

        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UsersDetailViewModel();

            string roleId = _dbContext.UserRoles.Where(r => r.UserId == user.Id).FirstOrDefault().RoleId;
            string roleName = (await _dbContext.Roles.FindAsync(roleId)).Name;

            model.UserInformation = new UsersViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roleName
            };

            return View(model);

        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UsersEditViewModel()
            {
                Id = id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ErrorMessages = new List<string>()
            };

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UsersEditViewModel model)
        {

            if (ModelState.IsValid)
            {

                model = CheckEditViewModel(model);

                if (model.ErrorMessages != null && model.ErrorMessages.Count != 0)
                {
                    return View(model);
                }

                MyUser user = _dbContext.Users.Find(model.Id);


                user.UserName = model.Username;
                user.NormalizedUserName = model.Username.ToUpper();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;


                try
                {
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Users.Any(e => e.Id == user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return Redirect("/User");

            }

            return View(model);

        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            string roleId = _dbContext.UserRoles.Where(r => r.UserId == user.Id).FirstOrDefault().RoleId;
            string roleName = (await _dbContext.Roles.FindAsync(roleId)).Name;

            var model = new UsersViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Role = roleName
            };

            return View(model);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {

            MyUser user = await _dbContext.Users.FindAsync(id);

            string roleId = _dbContext.UserRoles.Where(r => r.UserId == user.Id).FirstOrDefault().RoleId;
            string roleName = (await _dbContext.Roles.FindAsync(roleId)).Name;

            if (roleName == "Admin")
            {
                return View(id);
            }

            try
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Users.Any(e => e.Id == user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            return RedirectToAction(nameof(Index));


        }


        private UsersEditViewModel CheckEditViewModel(UsersEditViewModel model)
        {

            model.ErrorMessages = new List<string>();

            MyUser DbUserWithMatchingUsername = _dbContext.Users.Where(x => x.NormalizedUserName == model.Username.ToUpper()).FirstOrDefault();

            if (DbUserWithMatchingUsername != null && DbUserWithMatchingUsername.Id != model.Id)
            {
                model.ErrorMessages.Add("This username it taken!");
            }

            return model;

        }

        private string ShortenTooLongStrings(string str, int maxAmountOfCharacter = 12)
        {

            if (str.Length <= maxAmountOfCharacter) { return str; }

            int takeAmount = maxAmountOfCharacter / 2;

            if (maxAmountOfCharacter % 2 == 1) takeAmount++;

            string res =
                (str.Substring(0, takeAmount))
                + "..."
                + (str.Substring(str.Length - (maxAmountOfCharacter - takeAmount), maxAmountOfCharacter - takeAmount));

            return res;

        }


    }
}