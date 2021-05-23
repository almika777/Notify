using Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Context
{
    public class NotifyDbContext : DbContext
    {
        public NotifyDbContext()
        {
        }

        public NotifyDbContext(DbContextOptions<NotifyDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Notify> Notifies { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = ../../NotifiesDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notify>()
                .ToTable("Notifies")
                .HasIndex(x => x.Date);

            modelBuilder.Entity<ChatUser>()
                .ToTable("ChatUsers");
        }
    }
}
