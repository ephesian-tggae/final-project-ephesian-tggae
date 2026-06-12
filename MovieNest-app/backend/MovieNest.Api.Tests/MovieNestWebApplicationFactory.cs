using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MovieNest.Api.Data;

namespace MovieNest.Api.Tests;

public sealed class MovieNestWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // UseSetting is applied before Program.cs reads configuration at startup.
        builder.UseSetting("Authentication:Google:ClientId", "test-google-client-id");
        builder.UseSetting("Authentication:Google:ClientSecret", "test-google-client-secret");
        builder.UseSetting("Tmdb:ApiKey", "test-tmdb-api-key");
        builder.UseSetting("SEED_ON_STARTUP", "false");
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Data Source=:memory:");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<MovieNestDbContext>>();
            services.RemoveAll<MovieNestDbContext>();

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddSingleton(_connection);
            services.AddDbContext<MovieNestDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationSchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.AuthenticationSchemeName;
                options.DefaultScheme = TestAuthHandler.AuthenticationSchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.AuthenticationSchemeName,
                _ => { });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }

        base.Dispose(disposing);
    }
}
