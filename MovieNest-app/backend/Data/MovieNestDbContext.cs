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

    public DbSet<UserMovie> UserMovies => Set<UserMovie>();

    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.OAuthSubjectId)
            .IsUnique();

        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.TmdbId)
            .IsUnique();

        modelBuilder.Entity<UserMovie>()
            .HasIndex(um => new { um.UserId, um.MovieId })
            .IsUnique();

        modelBuilder.Entity<UserMovie>()
            .HasOne(um => um.User)
            .WithMany()
            .HasForeignKey(um => um.UserId);

        modelBuilder.Entity<UserMovie>()
            .HasOne(um => um.Movie)
            .WithMany()
            .HasForeignKey(um => um.MovieId);

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.MovieId })
            .IsUnique();

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany()
            .HasForeignKey(r => r.MovieId);
    }
}
