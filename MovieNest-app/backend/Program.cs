using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MovieNestDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserSyncService>();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (string.IsNullOrEmpty(googleClientId) || string.IsNullOrEmpty(googleClientSecret))
{
    throw new InvalidOperationException(
        "Set Authentication:Google:ClientId and Authentication:Google:ClientSecret with dotnet user-secrets.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
})
.AddGoogle(options =>
{
    options.ClientId = googleClientId;
    options.ClientSecret = googleClientSecret;
    options.Events.OnCreatingTicket = async context =>
    {
        var sync = context.HttpContext.RequestServices.GetRequiredService<UserSyncService>();
        await sync.SyncFromGooglePrincipalAsync(context.Principal!);
    };
});

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.RoutePrefix = "swagger");
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "MovieNest API is running.");

app.MapGet("/api/health", () =>
    Results.Ok(new
    {
        status = "ok",
        app = "MovieNest",
        timeUtc = DateTime.UtcNow
    }));

app.MapGet("/api/auth/login", () =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" },
        [GoogleDefaults.AuthenticationScheme]));

app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok(new { message = "Logged out" });
});

app.MapGet("/api/me", (ClaimsPrincipal user) =>
    Results.Ok(new
    {
        email = user.FindFirstValue(ClaimTypes.Email),
        name = user.FindFirstValue(ClaimTypes.Name)
    }))
    .RequireAuthorization();

app.Run();
