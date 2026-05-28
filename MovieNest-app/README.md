# MovieNest

Community movie platform: track watchlists and watch history, write reviews, and discover films through personalized recommendations.

## Repository layout

| Path | Description |
|------|-------------|
| [`backend/MovieNest.Api/`](backend/MovieNest.Api/) | ASP.NET Core (.NET 10) API |
| [`docs/`](docs/) | Project documentation (prompt log, architecture, etc.) |
| [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md) | Product brief (Milestone 0) |

## Local development (backend)

From `backend/MovieNest.Api/`:

```bash
dotnet build
dotnet run
```

API runs at the URL shown in the terminal (e.g. `http://localhost:5102`). Swagger: `/swagger` (when enabled).

### Database (SQLite, local)

```bash
dotnet ef database update
```

Connection string: `appsettings.Development.json` → `Data Source=movienest.db` (file is gitignored).

## Stack

- **Backend:** ASP.NET Core, EF Core, SQLite (local) / PostgreSQL (production planned)
- **Frontend:** React (planned) — `frontend/` folder
- **Auth:** Google OAuth (planned)
- **External API:** TMDB (planned)

## Deployment (planned)

- **Frontend:** Vercel
- **Backend + database:** Azure App Service + Azure PostgreSQL
