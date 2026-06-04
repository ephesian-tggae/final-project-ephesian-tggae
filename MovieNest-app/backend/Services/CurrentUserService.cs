using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class CurrentUserService
{
    private readonly MovieNestDbContext _db;

    public CurrentUserService(MovieNestDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserAsync(ClaimsPrincipal principal)
    {
        var subjectId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(subjectId))
        {
            return null;
        }

        return await _db.Users.FirstOrDefaultAsync(u => u.OAuthSubjectId == subjectId);
    }
}
