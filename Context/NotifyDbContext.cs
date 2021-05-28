using Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Context
{
    public class NotifyDbContext : DbContext
    {
        public DbSet<Notify> Notifies { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        public NotifyDbContext(DbContextOptions<NotifyDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
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
