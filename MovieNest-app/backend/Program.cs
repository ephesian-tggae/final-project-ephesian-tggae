using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MovieNestDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
