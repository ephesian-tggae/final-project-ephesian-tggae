# MovieNest

[![CI](https://github.com/ephesian-tggae/final-project-ephesian-tggae/actions/workflows/ci.yml/badge.svg)](https://github.com/ephesian-tggae/final-project-ephesian-tggae/actions/workflows/ci.yml)

**MovieNest** is a community movie platform where signed-in users track watchlists and watch history, write reviews, and get personalized recommendations powered by TMDB and seeded community activity.

## Deployed application

| | URL |
|---|-----|
| **App (Vercel)** | https://final-project-ephesian-tggae.vercel.app |
| **API (Render)** | https://movie-nest-app.onrender.com |
| **API health** | https://movie-nest-app.onrender.com/api/health |

- **OAuth provider:** Google
- **Third-party API:** TMDB
- **Advanced integration:** Option C — personalized recommendation engine

## Demo login

Open the [deployed app](https://final-project-ephesian-tggae.vercel.app) and click **Log in with Google**.

**Any Google account may sign in.** No demo username or password is required. The backend accepts every Google account that completes OAuth successfully; there is no in-app allowlist of test users.

> **Note for evaluators:** Sign-in works for any Google account because the Google OAuth consent screen is published (not limited to test users). If you ever see “Access blocked: app has not been verified,” the OAuth app was switched back to Testing mode in Google Cloud Console — contact the author or use your own Google account after being added as a test user.

## Local setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 18+ and npm
- EF Core CLI (one-time): `dotnet tool install --global dotnet-ef`
- Google Cloud OAuth client (Web application)
- Free [TMDB API key](https://www.themoviedb.org/settings/api) (v3)

### Environment variable setup

**Backend** — from `backend/`, set secrets with `dotnet user-secrets` (never commit values):

```bash
cd MovieNest-app/backend
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_GOOGLE_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_GOOGLE_CLIENT_SECRET"
dotnet user-secrets set "Tmdb:ApiKey" "YOUR_TMDB_V3_API_KEY"
```

In Google Cloud Console, add authorized redirect URI: `http://localhost:5102/signin-google`

**Frontend** — from `frontend/`:

```bash
cd MovieNest-app/frontend
npm install
cp .env.example .env
```

Set `VITE_API_URL=http://localhost:5102` in `.env`.

Local CORS, OAuth redirect, and SQLite path are already configured in `backend/appsettings.Development.json`.

### Backend run command

From `backend/`:

```bash
dotnet restore
dotnet ef database update    # first time / after new migrations
dotnet run
```

API: `http://localhost:5102` · Swagger: `http://localhost:5102/swagger`

### Frontend run command

From `frontend/` (second terminal):

```bash
npm run dev
```

App: `http://localhost:5173`

### Migration and seed commands

From `backend/`:

```bash
# Apply EF Core migrations
dotnet ef database update

# Seed community data (500 users, 5k movies, 10k interactions)
dotnet run -- seed

# Wipe seed-only data and reseed (keeps real Google users)
dotnet run -- seed --reset
```

Stop the API before seeding — SQLite allows one writer at a time. See [Seed test data](#seed-test-data) for details.

## Required environment variables

Descriptions only — set values in user-secrets (local), `.env` (frontend local), or host dashboards (production). **Do not commit secrets.**

| Variable | Where | Description |
|----------|--------|-------------|
| `Authentication:Google:ClientId` | Backend user-secrets / Render | Google OAuth 2.0 client ID for the Web application |
| `Authentication:Google:ClientSecret` | Backend user-secrets / Render | Google OAuth 2.0 client secret |
| `Tmdb:ApiKey` | Backend user-secrets / Render | TMDB v3 API key; used server-side only |
| `VITE_API_URL` | `frontend/.env` / Vercel | Base URL of the backend API for the React app |
| `ConnectionStrings:DefaultConnection` | `appsettings.Development.json` / Render | SQLite database file path |
| `FrontendUrl` | `appsettings.Development.json` / Render | Frontend URL used for post-OAuth redirect |
| `Cors:AllowedOrigins` | `appsettings.Development.json` / Render | Allowed frontend origin(s) for credentialed cross-origin requests |
| `SEED_ON_STARTUP` | Render (optional) | When `true`, runs community seed on API startup; set `false` after first successful deploy |
| `E2E_AUTH_ENABLED` | `appsettings.Development.json` | When `true` in Development, enables dev-only Playwright login endpoint |

Render uses double underscores for nested keys (e.g. `Authentication__Google__ClientId`, `Cors__AllowedOrigins__0`).

## Known limitations

- **No movie detail page** — search and discover show cards only; no dedicated film page.
- **Manual watchlist titles** — typed entries work but TMDB-enriched movies (from Discover/Search) get better posters and genres.
- **Recommendations** — computed on demand, not stored per request; no score/reason in UI; movies without genres personalize poorly. First load on Render can be slow on cold start.
- **SQLite in production** — single-file DB on Render disk; fine for a class demo, not ideal for high-traffic multi-instance hosting.
- **Render free tier** — API may sleep when idle; first request after idle can take ~30 seconds.
- **SignalR / MCP** — not implemented; advanced integration is the recommendation engine (Option C) only.
- **E2E tests** — Playwright runs locally only; CI runs backend xUnit and frontend Vitest.
- **Seed vs real data** — UI shows only the signed-in user’s watchlist/history/reviews; seed users power community stats and recommendations only.

---

## Repository layout

| Path | Description |
|------|-------------|
| [`backend/`](backend/) | ASP.NET Core (.NET 10) API |
| [`frontend/`](frontend/) | Vite + React app |
| [`docs/`](docs/) | Architecture, security review, prompt log, etc. |
| [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md) | Product brief (Milestone 0) |

## Quick local test

1. Open `http://localhost:5173`
2. Click **Log in with Google**
3. Visit **Watchlist**, **Search**, **Discover**, **Reviews**
4. Add a movie from Discover, mark watched, write a review
5. Return to **Home** → scroll to **Recommended for you**

Health check (no login): `http://localhost:5102/api/health`

Protected routes (`/watchlist`, `/history`, `/reviews`, `/profile`, `/settings`, `/search`) redirect to home when signed out. Direct URLs work in production thanks to `frontend/vercel.json` SPA rewrites.

## Tests

| Layer | Command | Location |
|-------|---------|----------|
| Backend | `dotnet test MovieNest.Api.Tests/MovieNest.Api.Tests.csproj` | `backend/` |
| Frontend unit | `npm test` | `frontend/` |
| E2E (local) | `npm run test:e2e` | `frontend/` (API + dev server running) |

CI (badge above) runs backend tests, ESLint, Vitest, and production build on push/PR to `main`. Vercel and Render deploy separately from GitHub.

## Seed test data

The seed script populates **simulated community data** for recommendations and public stats. It does **not** create real OAuth accounts and does **not** delete real Google users on `--reset`.

| Data | Count |
|------|------:|
| Seed movies | 5,000 |
| Simulated users | 500 |
| Shelf rows (`UserMovies`) | 8,000 |
| Reviews | 2,000 |

Seed users use `OAuthSubjectId` values starting with `seed:`; real Google users do not. Run `dotnet run -- seed --reset` from `backend/` for the full scaled dataset.

Optional verification:

```bash
sqlite3 movienest.db "SELECT COUNT(*) FROM Users WHERE OAuthSubjectId LIKE 'seed:%';"
sqlite3 movienest.db "SELECT COUNT(*) FROM Users WHERE OAuthSubjectId NOT LIKE 'seed:%';"
```

## Stack

- **Backend:** ASP.NET Core (.NET 10), EF Core, SQLite
- **Frontend:** Vite, React 19, React Router
- **Hosting:** Vercel (SPA) + Render (Docker API)
- **Auth:** Google OAuth, HTTP-only cookie sessions
- **External API:** TMDB (search, popular, genres, posters)

## Recommendations (Option C)

Signed-in users see up to **10 suggestions** on the home dashboard under **Recommended for you**. The engine uses your watchlist, watched history, reviews, genres, and seeded community activity. Call `GET /api/recommendations` (cookie required).

For best results locally: seed with `dotnet run -- seed --reset`, sign in, add TMDB movies to your watchlist, then open Home.

## Deployment

Production: **Vercel** (`MovieNest-app/frontend`) + **Render** (`MovieNest-app/backend`, Docker).

**Vercel:** set `VITE_API_URL` to your Render API URL; redeploy after changing it.

**Render:** Docker web service, env vars from [Required environment variables](#required-environment-variables), persistent disk at `/var/data/movienest.db`. Set `SEED_ON_STARTUP=true` on first deploy, then `false`.

**Google OAuth (production):**

| Setting | URI |
|---------|-----|
| Authorized redirect URI | `https://movie-nest-app.onrender.com/signin-google` |
| Authorized JavaScript origin | `https://final-project-ephesian-tggae.vercel.app` |

Push to `main` → GitHub Actions must pass → Vercel / Render auto-deploy.

## Documentation

| Doc | Purpose |
|-----|---------|
| [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) | System design, routes, endpoints, data model |
| [`docs/DESIGN_NOTE.md`](docs/DESIGN_NOTE.md) | Product and design decisions |
| [`docs/SECURITY_REVIEW.md`](docs/SECURITY_REVIEW.md) | Security review |
| [`docs/PROMPT_LOG.md`](docs/PROMPT_LOG.md) | AI prompt log |
| [`docs/AI_REFLECTION.md`](docs/AI_REFLECTION.md) | AI usage reflection |
| [`docs/ACCESSIBILITY_REPORT.md`](docs/ACCESSIBILITY_REPORT.md) | Accessibility audit |
| [`docs/TESTING_REVIEW.md`](docs/TESTING_REVIEW.md) | AI-generated test review |
