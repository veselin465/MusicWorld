using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicWorld.Data;
using MusicWorld.Data.Entity;
using MusicWorld.Models.Songs;

namespace MusicWorld.Controllers
{
    public class SongController : Controller
    {
        private readonly MusicWorldDbContext _dbContext;

        public SongController(MusicWorldDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Song
        [Authorize]
        public async Task<IActionResult> Index(string searchName = "")
        {

            var allSongs = _dbContext.Songs.ToList();
            allSongs = allSongs.OrderBy(x => x.Name).ToList();

            if (searchName != null && searchName != "")
            {
                allSongs = allSongs.Where(x => x.Name.Contains(searchName)).ToList();
            }

            var model = new SongsAllViewModel()
            {
                Songs = new List<SongsViewModel>()
            };

            foreach (var song in allSongs)
            {

                var singleViewModel = new SongsViewModel()
                {
                    Id = song.Id,
                    Name = song.Name,
                    Duration = FormatDuration(song.DurationSeconds),
                    AlbumId = song.AlbumId,
                    PerformerId = song.PerformerId,
                    AlbumName = (await _dbContext.Albums.FindAsync(song.AlbumId)).Name,
                    PerformerName = (await _dbContext.Performers.FindAsync(song.PerformerId)).Name,
                };

                model.Songs.Add(singleViewModel);

            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(SongsAllViewModel model)
        {
            return await this.Index(model.SearchWord);
        }

        // GET: Song/Details/5
        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = (await _dbContext.Songs.FindAsync(id));

            if (song == null)
            {
                return NotFound();
            }

            var model = new SongsDetailViewModel();

            model.SongInformation = new SongsViewModel()
            {
                Id = song.Id,
                Name = song.Name,
                Duration = FormatDuration(song.DurationSeconds),
                AlbumId = song.AlbumId,
                PerformerId = song.PerformerId,
                AlbumName = (await _dbContext.Albums.FindAsync(song.AlbumId)).Name,
                PerformerName = (await _dbContext.Performers.FindAsync(song.PerformerId)).Name
            };

            return View(model);
        }

        // GET: Song/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new SongsCreateViewModel();
            CreateSelectListForAlbums();
            return View(model);
        }

        // POST: Song/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(SongsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {

                int seconds = (model.DurationMinutes * 60 + model.DurationSeconds);

                if (seconds <= 0)
                {
                    model.ErrorMessages.Add("Duration should be at least 1 second");
                    CreateSelectListForAlbums(model.AlbumId);
                    return View(model);
                }

                var song = new Song()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    AlbumId = model.AlbumId,
                    DurationSeconds = (model.DurationMinutes * 60 + model.DurationSeconds),
                    PerformerId = _dbContext.Albums.Find(model.AlbumId).PerformerId
                };

                await _dbContext.Songs.AddAsync(song);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            CreateSelectListForAlbums(model.AlbumId);

            return View(model);
        }

        // GET: Song/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _dbContext.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var model = new SongsEditViewModel()
            {
                Id = song.Id,
                Name = song.Name,
                DurationMinutes = (song.DurationSeconds / 60),
                DurationSeconds = (song.DurationSeconds % 60)
            };

            CreateSelectListForAlbums(model.AlbumId);

            return View(model);
        }

        // POST: Song/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(SongsEditViewModel model)
        {
            if (ModelState.IsValid)
            {

                var song = _dbContext.Songs.Find(model.Id);

                song.Name = model.Name;
                song.AlbumId = model.AlbumId;
                song.PerformerId = _dbContext.Albums.Find(model.AlbumId).PerformerId;
                song.DurationSeconds = (model.DurationMinutes * 60 + model.DurationSeconds);

                try
                {
                    _dbContext.Songs.Update(song);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Songs.Any(e => e.Id == song.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return Redirect("/Song");

            }

            CreateSelectListForAlbums(model.AlbumId);

            return View(model);
        }

        // GET: Song/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var song = await _dbContext.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var model = new SongsViewModel()
            {
                Id = song.Id,
                Name = song.Name,
                Duration = FormatDuration(song.DurationSeconds),
                AlbumId = song.AlbumId,
                PerformerId = song.PerformerId,
                AlbumName = (await _dbContext.Albums.FindAsync(song.AlbumId)).Name,
                PerformerName = (await _dbContext.Performers.FindAsync(song.PerformerId)).Name
            };

            return View(model);
        }

        // POST: Song/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var song = await _dbContext.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            _dbContext.Songs.Remove(song);
            await _dbContext.SaveChangesAsync();

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

        private void CreateSelectListForAlbums(string selectedAlbumId = "")
        {

            var resSelectList = new List<SelectListItem>();

            foreach (var album in _dbContext.Albums)
            {
                var currentAlbum = _dbContext.Albums.Find(album.Id);
                var currentPerformer = _dbContext.Performers.Find(currentAlbum.PerformerId);
                resSelectList.Add(new SelectListItem()
                {
                    Value = currentAlbum.Id,
                    Text = currentAlbum.Name + " - "+currentPerformer.Name
                });
                if (selectedAlbumId == album.Id)
                {
                    resSelectList.Last().Selected = true;
                }
            }

            ViewData["AlbumId"] = resSelectList;

        }

    }



}
