using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Models;

namespace MovieNest.Api.Data;

public class MovieNestDbContext : DbContext
{
    public MovieNestDbContext(DbContextOptions<MovieNestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.OAuthSubjectId)
            .IsUnique();

        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.TmdbId)
            .IsUnique();
    }
}
