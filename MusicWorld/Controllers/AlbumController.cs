using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicWorld.Data;
using MusicWorld.Data.Entity;
using MusicWorld.Models.Albums;
using MusicWorld.Models.Songs;

namespace MusicWorld.Controllers
{
    public class AlbumController : Controller
    {

        private readonly MusicWorldDbContext _dbContext;

        public AlbumController(MusicWorldDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public async Task<IActionResult> Index(string searchName)
        {

            var allAlbums = _dbContext.Albums.ToList();
            allAlbums = allAlbums.OrderBy(x => x.Name).ToList();

            if (searchName != null && searchName != "")
            {
                allAlbums = allAlbums.Where(x => x.Name.Contains(searchName)).ToList();
            }

            var model = new AlbumsAllViewModel()
            {
                Albums = new List<AlbumsViewModel>()
            };



            foreach (var album in allAlbums)
            {

                int songNumber = _dbContext.Songs.Where(x => x.AlbumId == album.Id).Count();
                string performerName = (await _dbContext.Performers.FindAsync(album.PerformerId)).Name;

                var singleViewModel = new AlbumsViewModel()
                {
                    Id = album.Id,
                    Name = album.Name,
                    PerformerId = album.PerformerId,
                    PerformerName = performerName,
                    SongsNumber = songNumber
                };

                model.Albums.Add(singleViewModel);

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

            var album = await _dbContext.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            var songsVM = new List<SongsViewModel>();

            foreach (var song in _dbContext.Songs.Where(x => x.AlbumId == id))
            {
                var singleSongVM = new SongsViewModel()
                {
                    Id = song.Id,
                    Name = song.Name,
                    Duration = FormatDuration(song.DurationSeconds)
                };
                songsVM.Add(singleSongVM);
            }

            var model = new AlbumsDetailViewModel
            {
                AlbumInformation = new AlbumsViewModel()
                {
                    Id = album.Id,
                    Name = album.Name,
                    PerformerId = album.PerformerId,
                    PerformerName = (await _dbContext.Performers.FindAsync(album.PerformerId)).Name,
                    DateOfRelease = album.DateOfRelease,
                    SongsNumber = 0
                },
                Songs = songsVM
            };

            return View(model);

        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new AlbumsCreateViewModel();
            model.DateOfRelease = DateTime.Now;
            CreateSelectListForPerformers();
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AlbumsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var album = new Album()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    PerformerId = model.PerformerId,
                    DateOfRelease = model.DateOfRelease ?? DateTime.Now
                };

                await _dbContext.Albums.AddAsync(album);
                await _dbContext.SaveChangesAsync();

                return Redirect("/Album");

            }
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var album = await _dbContext.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            var model = new AlbumsEditViewModel()
            {
                Id = album.Id,
                Name = album.Name,
                PerformerId = album.PerformerId,
                DateOfRelease = album.DateOfRelease
            };

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(AlbumsEditViewModel model)
        {

            if (ModelState.IsValid)
            {

                var album = _dbContext.Albums.Find(model.Id);

                album.Name = model.Name;
                album.PerformerId = model.PerformerId;

                if (model.DateOfRelease != null)
                {
                    album.DateOfRelease = model.DateOfRelease.Value;
                }

                try
                {
                    _dbContext.Albums.Update(album);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Albums.Any(e => e.Id == album.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return Redirect("/Album");

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

            var album = await _dbContext.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            var model = new AlbumsViewModel()
            {
                Id = album.Id,
                Name = album.Name,
                PerformerId = album.PerformerId,
                PerformerName = (await _dbContext.Performers.FindAsync(album.PerformerId)).Name,
                DateOfRelease = album.DateOfRelease
            };

            return View(model);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {

            var album = await _dbContext.Albums.FindAsync(id);

            try
            {
                _dbContext.Albums.Remove(album);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Albums.Any(e => e.Id == album.Id))
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


        private string FormatDuration(int durationSeconds)
        {
            int min = durationSeconds / 60;
            int sec = durationSeconds % 60;

            string res =
                ((min < 10) ? "0" : "") + min + ":" +
                ((sec < 10) ? "0" : "") + sec;

            return res;
        }


        private void CreateSelectListForPerformers(string selectedAlbumId = "")
        {



            if (selectedAlbumId == null || selectedAlbumId == "")
            {
                ViewData["PerformerId"] = new SelectList(_dbContext.Performers, "Id", "Name");
            }
            else
            {
                ViewData["PerformerId"] = new SelectList(_dbContext.Performers, "Id", "Name", selectedAlbumId);
            }



        }

    }
}