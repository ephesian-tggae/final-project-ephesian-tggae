namespace MovieNest.Api.Models;

public class Recommendation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int MovieId { get; set; }

    public Movie Movie { get; set; } = null!;

    public int Score { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
