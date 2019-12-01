using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poll_core_web_API.Models;

namespace Poll_core_web_API.Models
{
    public class PollContext: DbContext
    {
        public PollContext(DbContextOptions<PollContext> options)
            : base (options)
        {

        }

        public DbSet<Antwoord> Antwoorden { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollGebruiker> PollGebruikers { get; set; }
        public DbSet<Stem> Stemmen { get; set; }
        public DbSet<Vriend> Vrienden { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Antwoord>().ToTable("Antwoord");
            modelBuilder.Entity<Gebruiker>().ToTable("Gebruiker");
            modelBuilder.Entity<Poll>().ToTable("Poll");
            modelBuilder.Entity<PollGebruiker>().ToTable("PollGebruiker");
            modelBuilder.Entity<Stem>().ToTable("Stem");
            modelBuilder.Entity<Vriend>().ToTable("Vriend");
        }
    }
}
