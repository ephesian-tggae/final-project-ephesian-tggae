# MovieNest

Community movie platform: track watchlists and watch history, write reviews, and discover films through personalized recommendations.

## Repository layout

| Path | Description |
|------|-------------|
| [`backend/`](backend/) | ASP.NET Core (.NET 10) API |
| [`frontend/`](frontend/) | Vite + React app |
| [`docs/`](docs/) | Project documentation (prompt log, architecture, etc.) |
| [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md) | Product brief (Milestone 0) |

## Local development

### Backend

From `MovieNest-app/backend/`:

```bash
dotnet ef database update
dotnet run
```

API runs at `http://localhost:5102`. Swagger: `/swagger` (Development only).

Google OAuth secrets (one-time setup):

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
dotnet user-secrets set "Tmdb:ApiKey" "YOUR_TMDB_KEY"
```

### Frontend

From `MovieNest-app/frontend/`:

```bash
npm install
npm run dev
```

App runs at `http://localhost:5173`. Set `VITE_API_URL=http://localhost:5102` in `.env` if needed.

### Database (SQLite, local)

Connection string: `backend/appsettings.Development.json` → `movienest.db` (gitignored).

## Seed test data (Milestone 3)

The seed script adds **simulated** data only. It does **not** delete real Google OAuth users or their watchlist rows.

| Data | Count |
|------|------:|
| Movies (domain records) | 500 |
| Fake users | 50 |
| Watchlist / watched rows | 1,000 |

Fake users use ids like `seed:user:001` and emails like `seed-user-001@movienest.local`.

### Run the seed

From `MovieNest-app/backend/`:

```bash
dotnet run -- seed
```

First run inserts data. If seed data already exists, the command skips inserts and prints counts.

### Reset and reseed

Removes **only** seed movies, seed users, and their interactions. Real Google users stay.

```bash
dotnet run -- seed --reset
```

### Check that seed worked

After `dotnet run -- seed`, the command prints counts, for example:

```text
  Seed movies:        500 (target 500)
  Seed users:         50 (target 50)
  Seed interactions:  1000 (target 1000)
  Real OAuth users:   1 (unchanged by seed)
```

Optional: inspect SQLite with DB Browser or:

```bash
sqlite3 movienest.db "SELECT COUNT(*) FROM Movies WHERE TmdbId >= 1000001;"
sqlite3 movienest.db "SELECT COUNT(*) FROM Users WHERE OAuthSubjectId LIKE 'seed:%';"
sqlite3 movienest.db "SELECT COUNT(*) FROM UserMovies um JOIN Users u ON u.Id = um.UserId WHERE u.OAuthSubjectId LIKE 'seed:%';"
```

Reseed typically takes a few seconds on a laptop.

## Stack

- **Backend:** ASP.NET Core, EF Core, SQLite (local) / PostgreSQL (production planned)
- **Frontend:** Vite + React
- **Auth:** Google OAuth (cookie sessions)
- **External API:** TMDB

## Deployment (planned)

- **Frontend:** Vercel
- **Backend + database:** Azure App Service + Azure PostgreSQL
