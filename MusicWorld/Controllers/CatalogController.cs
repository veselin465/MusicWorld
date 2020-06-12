using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicWorld.Data;
using MusicWorld.Data.Entity;
using MusicWorld.Models.Catalogs;
using MusicWorld.Models.Songs;

namespace MusicWorld.Controllers
{
    public class CatalogController : Controller
    {

        private readonly MusicWorldDbContext _dbContext;
        private readonly UserManager<MyUser> _userManager;

        public CatalogController(MusicWorldDbContext dbContext, UserManager<MyUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Index(string searchName = "")
        {

            var allCatalogs = _dbContext.Catalogs.ToList();
            var model = new CatalogsAllViewModel()
            {
                Catalogs = new List<CatalogsViewModel>()
            };

            if (!CheckWhetherCurrentUserIsAdmin())
            {
                allCatalogs = allCatalogs.Where(x => x.MyUserId == GetCurrentUserId()).ToList();
            }

            if (searchName != null && searchName != "")
            {
                allCatalogs = allCatalogs.Where(x => x.Name.Contains(searchName)).ToList();
            }

            

            foreach (var catalog in allCatalogs)
            {

                int songsCount = _dbContext.CatalogSong.Where(x => x.CatalogId == catalog.Id).Count();

                var singleViewModel = new CatalogsViewModel()
                {
                    Id = catalog.Id,
                    Name = catalog.Name,
                    Descritpion = catalog.Description,
                    MyUserId = catalog.MyUserId,
                    MyUserName = (await _dbContext.Users.FindAsync(catalog.MyUserId)).UserName,
                    SongsCount = songsCount
                };

                model.Catalogs.Add(singleViewModel);

            }

            return View(model);

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(SongsAllViewModel model)
        {
            return await this.Index(model.SearchWord);
        }


        [Authorize]
        public async Task<IActionResult> Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var catalog = await _dbContext.Catalogs.FindAsync(id);

            if (catalog == null)
            {
                return NotFound();
            }

            var model = new CatalogsDetailViewModel();

            model.CatalogInformation = new CatalogsViewModel()
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Descritpion = catalog.Description,
                MyUserId = catalog.MyUserId,
                MyUserName = (await _dbContext.Users.FindAsync(catalog.MyUserId)).UserName
            };

            var allSongs = _dbContext.Songs;
            var allAviableSongs = allSongs;
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var currentCatalogAllSongs = _dbContext.CatalogSong.Where(x => x.CatalogId == id).ToList();

            model.CurrentCatalogSongs = new List<SongsViewModel>();
            model.NotAddedSongs = new List<SongsViewModel>();

            foreach (var song in allSongs)
            {
                if (currentCatalogAllSongs.Any(x => x.SongId == song.Id))
                {
                    var singleViewModel = new SongsViewModel()
                    {
                        Id = song.Id,
                        Name = song.Name,
                        Duration = FormatDuration(song.DurationSeconds),
                        AlbumId = song.AlbumId,
                        PerformerId = song.PerformerId,
                        AlbumName = (await _dbContext.Albums.FindAsync(song.AlbumId)).Name,
                        PerformerName = (await _dbContext.Performers.FindAsync(song.PerformerId)).Name
                    };

                    model.CurrentCatalogSongs.Add(singleViewModel);
                }
                else
                {
                    var singleViewModel = new SongsViewModel()
                    {
                        Id = song.Id,
                        Name = song.Name,
                        Duration = FormatDuration(song.DurationSeconds),
                        AlbumId = song.AlbumId,
                        PerformerId = song.PerformerId,
                        AlbumName = (await _dbContext.Albums.FindAsync(song.AlbumId)).Name,
                        PerformerName = (await _dbContext.Performers.FindAsync(song.PerformerId)).Name
                    };

                    model.NotAddedSongs.Add(singleViewModel);
                }



            }

            return View(model);

        }


        [Authorize]
        public IActionResult Create()
        {
            var model = new CatalogsCreateViewModel();
            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CatalogsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var catalog = new Catalog()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Description = model.Descritpion,
                    MyUserId = _dbContext.Users.Where(x => x.NormalizedUserName == model.MyUser_UserName.ToUpper()).FirstOrDefault().Id
                };

                await _dbContext.Catalogs.AddAsync(catalog);
                await _dbContext.SaveChangesAsync();

                return Redirect("/Catalog");

            }
            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var catalog = await _dbContext.Catalogs.FindAsync(id);

            if (catalog == null)
            {
                return NotFound();
            }

            var model = new CatalogsEditViewModel()
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Descritpion = catalog.Description,
            };

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(CatalogsEditViewModel model)
        {

            if (ModelState.IsValid)
            {

                var catalog = _dbContext.Catalogs.Find(model.Id);

                catalog.Name = model.Name;
                catalog.Description = model.Descritpion;

                try
                {
                    _dbContext.Catalogs.Update(catalog);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Catalogs.Any(e => e.Id == catalog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return Redirect("/Catalog");

            }

            return View(model);

        }


        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirm(string id)
        {

            var catalog = await _dbContext.Catalogs.FindAsync(id);

            try
            {
                _dbContext.Catalogs.Remove(catalog);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Catalogs.Any(e => e.Id == catalog.Id))
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


        [Authorize]
        public async Task<IActionResult> AddSongToCatalog(string catalogId, string songId)
        {
            bool isAuthorized = CheckWhetherCatalogBelongsToCurrentUser(catalogId) || CheckWhetherCurrentUserIsAdmin();
            if (!isAuthorized)
            {
                // Alarm
                // A non-admin user made an attempt to change someone else's catalog information
                return NotFound();
            }

            var catalogSong = new CatalogSong()
            {
                CatalogId = catalogId,
                SongId = songId
            };

            if (_dbContext.CatalogSong.Any(x => x == catalogSong))
            {
                // Alarm
                // An authorized user made an attempt to add to their catalog dublicate song entity
                return Redirect($"Catalog/Details/{catalogId}");
            }

            try
            {
                await _dbContext.CatalogSong.AddAsync(catalogSong);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Catalogs.Any(e => e.Id == catalogId) ||
                    !_dbContext.Songs.Any(e => e.Id == songId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            return Redirect($"/Catalog/Details/{catalogId}");

        }


        [Authorize]
        public async Task<IActionResult> RemoveSongFromCatalog(string catalogId, string songId)
        {
            bool isAuthorized = CheckWhetherCatalogBelongsToCurrentUser(catalogId) || CheckWhetherCurrentUserIsAdmin();
            if (!isAuthorized)
            {
                // Alarm
                // A non-admin user made an attempt to change someone else's catalog information
                return NotFound();
            }

            var catalogSong = new CatalogSong()
            {
                CatalogId = catalogId,
                SongId = songId
            };

            if (!_dbContext.CatalogSong.Any(x => x == catalogSong))
            {
                // Alarm
                // An authorized user made an attempt to delete from their catalog not-existing entity
                return Redirect($"Catalog/Details/{catalogId}");
            }

            try
            {
                _dbContext.CatalogSong.Remove(catalogSong);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Catalogs.Any(e => e.Id == catalogId) ||
                    !_dbContext.Songs.Any(e => e.Id == songId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            return Redirect($"/Catalog/Details/{catalogId}");

        }


        private string FormatDuration(int durationSeconds)
        {
            int min = durationSeconds / 60;
            int sec = durationSeconds % 60;

            string res =
                ((min < 10) ? "0" : "") + min + ":" +
                ((sec < 10) ? "0" : "") + sec;

            return res;
        }

        private bool CheckWhetherCurrentUserIsAdmin()
        {
            bool isAdmin = false;

            string currentUserId = GetCurrentUserId();

            var currentUserRoles = _dbContext.UserRoles.Where(x => x.UserId == currentUserId).ToList();
            foreach (var role in currentUserRoles)
            {
                string roleId = role.RoleId;
                var roleName = _dbContext.Roles.Find(roleId).Name;
                if (roleName == "Admin")
                {
                    isAdmin = true;
                    break;
                }
            }

            return isAdmin;

        }

        private string GetCurrentUserId()
        {
            var currentUser = _userManager.GetUserAsync(HttpContext.User);
            string currentUserId = currentUser.Result.Id;
            return currentUserId;
        }

        private bool CheckWhetherCatalogBelongsToCurrentUser(string catalogId)
        {
            var catalog = _dbContext.Catalogs.Find(catalogId);
            var currentUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            var catalogUserId = catalog.MyUserId;

            return currentUserId == catalogUserId;

        }

    }
}