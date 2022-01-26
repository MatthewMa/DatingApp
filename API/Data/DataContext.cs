using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Config relationships
            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser).WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser).WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Message>()
                .HasOne(m => m.Sender).WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>()
                .HasOne(m => m.Receipient).WithMany(u => u.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
