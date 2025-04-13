using Comments.Server.Data.Entities;
using Comments.Server.Data.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Comments.Server.Data;

public class CommentsDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public CommentsDbContext(DbContextOptions<CommentsDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<User>(new UserConfiguration());
        modelBuilder.ApplyConfiguration<Comment>(new CommentConfiguration());
    }
}
