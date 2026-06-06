namespace MovieNest.Api.Models;

public class UserMovie
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int MovieId { get; set; }

    public Movie Movie { get; set; } = null!;

    public string Status { get; set; } = "watchlist";

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
