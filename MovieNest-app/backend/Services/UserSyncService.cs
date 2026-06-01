using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class UserSyncService
{
    private readonly MovieNestDbContext _db;

    public UserSyncService(MovieNestDbContext db)
    {
        _db = db;
    }

    public async Task<User> SyncFromGooglePrincipalAsync(ClaimsPrincipal principal)
    {
        var subjectId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(subjectId))
        {
            throw new InvalidOperationException("Google did not provide a user id.");
        }

        var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var displayName = principal.FindFirstValue(ClaimTypes.Name) ?? email;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.OAuthSubjectId == subjectId);
        if (user is null)
        {
            user = new User
            {
                OAuthSubjectId = subjectId,
                Email = email,
                DisplayName = displayName,
                JoinedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
        }
        else
        {
            user.Email = email;
            user.DisplayName = displayName;
        }

        await _db.SaveChangesAsync();
        return user;
    }
}
