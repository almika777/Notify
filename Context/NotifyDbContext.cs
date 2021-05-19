using Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Context
{
    public class NotifyDbContext : DbContext
    {
        public NotifyDbContext(DbContextOptions<NotifyDbContext> options)
            : base(options) { }

        public DbSet<Notify> Notifies { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notify>()
                .HasOne(x => x.ChatUser);

            modelBuilder.Entity<Notify>()
                .ToTable("Notifies")
                .HasIndex(x => x.Date);

            modelBuilder.Entity<ChatUser>()
                .ToTable("ChatUsers")
                .HasMany<Notify>();
        }
    }
}
