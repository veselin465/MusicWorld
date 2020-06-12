using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicWorld.Data.Entity;

namespace MusicWorld.Data
{
    public class MusicWorldDbContext : IdentityDbContext<MyUser, IdentityRole, string>
    {

        public MusicWorldDbContext(DbContextOptions<MusicWorldDbContext> options)
            : base(options)
        { }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<Performer> Performers { get; set; }

        public DbSet<Song> Songs { get; set; }

        public DbSet<CatalogSong> CatalogSong { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Fluent API - only way to repsresent composite key in EF Core

            modelBuilder.Entity<CatalogSong>()
                .HasKey(o => new { o.CatalogId, o.SongId });

            modelBuilder.Entity<MyUser>()
                .HasMany(b => b.Catalogs)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Catalog>()
                .HasOne(b => b.User)
                .WithMany(a => a.Catalogs)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CatalogSong>()
               .HasOne(b => b.Catalog)
               .WithMany(a => a.CatalogSongs)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CatalogSong>()
               .HasOne(b => b.Song)
               .WithMany(a => a.CatalogSongs)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Album>()
               .HasOne(b => b.Performer)
               .WithMany(a => a.Albums)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Song>()
               .HasOne(b => b.Album)
               .WithMany(a => a.Songs)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

        }

    }
}
