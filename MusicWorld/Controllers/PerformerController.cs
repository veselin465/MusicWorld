using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicWorld.Data;
using MusicWorld.Data.Entity;
using MusicWorld.Models.Albums;
using MusicWorld.Models.Performers;
using MusicWorld.Models.Songs;

namespace MusicWorld.Controllers
{
    public class PerformerController : Controller
    {

        private readonly MusicWorldDbContext _dbContext;

        public PerformerController(MusicWorldDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public async Task<IActionResult> Index(string searchName = "")
        {

            var allPerformers = await _dbContext.Performers.ToListAsync();
            allPerformers = allPerformers.OrderBy(x => x.Name).ToList();

            if (searchName != null && searchName != "")
            {
                allPerformers = allPerformers.Where(x => x.Name.Contains(searchName)).ToList();
            }

            var model = new PerformersAllViewModel()
            {
                Performers = new List<PerformersViewModel>()
            };

            foreach (var performer in allPerformers)
            {

                int albumsNumber = _dbContext.Albums.Where(x => x.PerformerId == performer.Id).Count();
                int songsNumber = _dbContext.Songs.Where(x => x.PerformerId == performer.Id).Count();

                var singleViewModel = new PerformersViewModel()
                {
                    Id = performer.Id,
                    Name = performer.Name,
                    Description = performer.Description,
                    AlbumsNumber = albumsNumber,
                    SongsNumber = songsNumber
                };

                model.Performers.Add(singleViewModel);

            }

            return View(model);

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(PerformersAllViewModel model)
        {
            return await this.Index(model.SearchWord);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new PerformersCreateViewModel();
            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var performer = await _dbContext.Performers.FindAsync(id);

            if (performer == null)
            {
                return NotFound();
            }

            var model = new PerformersDetailViewModel
            {
                PerformerInformation = new PerformersViewModel()
                {
                    Id = performer.Id,
                    Name = performer.Name,
                    Description = performer.Description,
                    AlbumsNumber = 0,
                    SongsNumber = 0
                },
                Albums = new List<AlbumsDetailViewModel>()
            };

            foreach (var album in _dbContext.Albums.Where(x => x.PerformerId == id))
            {

                var songs = _dbContext.Songs.Where(x => x.AlbumId == album.Id);

                var songsViewModel = new List<SongsViewModel>();

                foreach (var song in songs)
                {
                    var singleSongVM = new SongsViewModel()
                    {
                        Id = song.Id,
                        Name = song.Name,
                        Duration = FormatDuration(song.DurationSeconds)
                    };

                    songsViewModel.Add(singleSongVM);

                }

                model.Albums.Add(new AlbumsDetailViewModel()
                {
                    AlbumInformation = new AlbumsViewModel()
                    {
                        Id = album.Id,
                        Name = album.Name,
                        DateOfRelease = album.DateOfRelease
                    },
                    Songs = songsViewModel
                });

            }

            return View(model);

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PerformersCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var performer = new Performer()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Description = model.Description
                };

                await _dbContext.Performers.AddAsync(performer);
                await _dbContext.SaveChangesAsync();

                return Redirect("/Performer");

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

            var performer = await _dbContext.Performers.FindAsync(id);

            if (performer == null)
            {
                return NotFound();
            }

            var model = new PerformersEditViewModel()
            {
                Id = performer.Id,
                Name = performer.Name,
                Description = performer.Description,
            };

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(PerformersEditViewModel model)
        {

            if (ModelState.IsValid)
            {

                var performer = _dbContext.Performers.Find(model.Id);

                performer.Name = model.Name;
                performer.Description = model.Description;

                try
                {
                    _dbContext.Performers.Update(performer);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Performers.Any(e => e.Id == performer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return Redirect("/Performer");

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

            var performer = await _dbContext.Performers.FindAsync(id);

            if (performer == null)
            {
                return NotFound();
            }

            var model = new PerformersViewModel()
            {
                Id = performer.Id,
                Name = performer.Name,
                Description = performer.Description
            };

            return View(model);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {

            var performer = await _dbContext.Performers.FindAsync(id);

            try
            {
                _dbContext.Performers.Remove(performer);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Performers.Any(e => e.Id == performer.Id))
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

    }
}