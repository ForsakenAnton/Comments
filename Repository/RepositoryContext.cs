
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.EntityConfiguration;

namespace Repository;

public class RepositoryContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public RepositoryContext(DbContextOptions<RepositoryContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<User>(new UserConfiguration());
        modelBuilder.ApplyConfiguration<Comment>(new CommentConfiguration());
    }
}
