using System.IO;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Context
{
    public class NotifyDbContext : DbContext
    {
        public DbSet<Notify> Notifies { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=../../NotifiesDB.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notify>().ToTable("Notifies");
            modelBuilder.Entity<ChatUser>().ToTable("ChatUsers");
        }
    }
}
