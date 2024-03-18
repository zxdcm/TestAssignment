using Feed.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feed.Infrastructure.Persistence.ModelBuilding;

public class FeedContext : DbContext
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Post> Posts { get; set; }

    public FeedContext(DbContextOptions<FeedContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .ToContainer("posts");
        
        modelBuilder.Entity<Comment>()
            .ToContainer("comments")
            .HasPartitionKey(p => p.PostId);
    }
}