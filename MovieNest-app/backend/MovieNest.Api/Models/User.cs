namespace MovieNest.Api.Models;

public class User
{
    public int Id { get; set; }

    public string OAuthSubjectId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

