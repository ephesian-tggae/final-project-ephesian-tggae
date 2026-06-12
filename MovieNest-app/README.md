# MovieNest

Community movie platform: track watchlists and watch history, write reviews, and discover films through personalized recommendations.

## Live demo

| | URL |
|---|-----|
| **App** | https://final-project-ephesian-tggae.vercel.app |
| **API health** | https://movie-nest-app.onrender.com/api/health |

- **OAuth:** Google
- **Third-party API:** TMDB
- **Advanced integration:** Option C — personalized recommendation engine

## Repository layout

| Path | Description |
|------|-------------|
| [`backend/`](backend/) | ASP.NET Core (.NET 10) API |
| [`frontend/`](frontend/) | Vite + React app |
| [`docs/`](docs/) | Project documentation (prompt log, architecture, etc.) |
| [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md) | Product brief (Milestone 0) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 18+ and npm
- EF Core CLI (one-time): `dotnet tool install --global dotnet-ef`
- Google Cloud OAuth client (Web application) for local login
- Free [TMDB API key](https://www.themoviedb.org/settings/api) (v3)

## Local setup (first time)

### 1. Clone and open the project

```bash
git clone <your-repo-url>
cd MovieNest-app
```

### 2. Backend — database and secrets

From `backend/`:

```bash
cd backend
dotnet restore
dotnet ef database update
```

Set secrets with `dotnet user-secrets` (never commit these values):

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_GOOGLE_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_GOOGLE_CLIENT_SECRET"
dotnet user-secrets set "Tmdb:ApiKey" "YOUR_TMDB_V3_API_KEY"
```

**Google OAuth (local):** In Google Cloud Console, add authorized redirect URI:

```text
http://localhost:5102/signin-google
```

`appsettings.Development.json` already points the API at `http://localhost:5173` for CORS and post-login redirect.

### 3. Frontend — dependencies and API URL

From `frontend/`:

```bash
cd ../frontend
npm install
cp .env.example .env
```

`.env` should contain:

```text
VITE_API_URL=http://localhost:5102
```

### 4. Optional — seed test data

From `backend/` (API does not need to be running):

```bash
cd ../backend
dotnet run -- seed
```

See [Seed test data](#seed-test-data) below.

## Run locally

Use **two terminals**.

**Terminal 1 — API** (`backend/`):

```bash
dotnet run
```

API: `http://localhost:5102` · Swagger: `http://localhost:5102/swagger`

**Terminal 2 — React** (`frontend/`):

```bash
npm run dev
```

App: `http://localhost:5173`

### Quick test

1. Open `http://localhost:5173`
2. Click **Log in with Google** → complete OAuth → return to the app
3. Visit **Watchlist**, **Search**, **Profile**, **Settings** (protected routes)
4. On **Search** or **Discover** (signed in), search a movie (e.g. `Inception`) — click **Add to watchlist** on a TMDB result
5. On **Watchlist**, confirm the movie appears with poster and genres; **Mark as watched**, or **Remove**

Health check (no login): `http://localhost:5102/api/health`

## Environment variables

| Variable | Where | Description |
|----------|--------|-------------|
| `Authentication:Google:ClientId` | Backend user-secrets | Google OAuth client ID |
| `Authentication:Google:ClientSecret` | Backend user-secrets | Google OAuth client secret |
| `Tmdb:ApiKey` | Backend user-secrets | TMDB v3 API key (server-side only) |
| `VITE_API_URL` | `frontend/.env` | Backend base URL for React (`http://localhost:5102` locally) |
| `ConnectionStrings:DefaultConnection` | `backend/appsettings.Development.json` | SQLite file path (`movienest.db`) |
| `FrontendUrl` | `backend/appsettings.Development.json` | React URL after OAuth (`http://localhost:5173`) |
| `Cors:AllowedOrigins` | `backend/appsettings.Development.json` | Allowed frontend origin for cookie auth |
| `SEED_ON_STARTUP` | Render (optional) | Set to `true` on first deploy to seed community data; set `false` after |

**Production (Render + Vercel)** — set in each host’s dashboard, not in git:

| Variable | Host | Value |
|----------|------|--------|
| `VITE_API_URL` | Vercel | `https://movie-nest-app.onrender.com` |
| `Authentication__Google__ClientId` | Render | Google OAuth client ID |
| `Authentication__Google__ClientSecret` | Render | Google OAuth client secret |
| `Tmdb__ApiKey` | Render | TMDB v3 API key |
| `ConnectionStrings__DefaultConnection` | Render | `Data Source=/var/data/movienest.db` |
| `FrontendUrl` | Render | `https://final-project-ephesian-tggae.vercel.app` |
| `Cors__AllowedOrigins__0` | Render | Same as `FrontendUrl` |
| `SEED_ON_STARTUP` | Render | `true` for first deploy, then `false` |

Do not commit secrets, `.env` with real keys, or `movienest.db`.

## Database (SQLite, local)

- File: `backend/movienest.db` (gitignored)
- Apply migrations: `dotnet ef database update` (from `backend/`)

## Seed test data

The seed script populates the database with **simulated community data** so the app feels alive at scale (recommendations, community stats, and future features). It does **not** create real OAuth accounts and does **not** delete real Google users or their data when you reseed.

### What the seeder creates

| Data | Count | Notes |
|------|------:|-------|
| Movies (domain records) | **5,000** | Synthetic titles; `TmdbId` from `1_000_001` upward; genres via `MovieGenre` |
| Simulated users | **500** | Realistic names; `OAuthSubjectId` like `seed:user:0001` |
| User-owned interactions (total) | **10,000** | See breakdown below |

**Interaction breakdown (10,000 total):**

| Type | Count | Description |
|------|------:|-------------|
| `UserMovies` | **8,000** | Mix of `watchlist` and `watched` shelf rows on seed movies |
| `Reviews` | **2,000** | Ratings (and optional short review text) on seed movies |

The seeder also ensures standard **genre** rows exist and assigns **1–3 genres** per seed movie. Seed movie **posters** use real TMDB `poster_path` values from a bundled JSON file (`backend/Data/seed-poster-paths.json`) — **no TMDB API calls** during `dotnet run -- seed`.

### Simulated vs real external data

| Source | Simulated (seed) | Real (live app) |
|--------|------------------|-----------------|
| **Users** | Fake members (`seed:` OAuth ids, `@movienest.local` emails) | Google OAuth sign-in |
| **Movies** | 5,000 generated catalog entries (`TmdbId >= 1000001`) | TMDB search/discover when you add to your shelf |
| **Shelf / reviews** | Seed users’ watchlist, watched, and review rows | Your watchlist, history, and reviews after sign-in |
| **Posters (seed movies)** | Bundled real TMDB paths (static JSON) | TMDB when adding movies via Discover/Search |
| **Posters (display)** | Served from `image.tmdb.org` | Same CDN |

Real Google users and their shelf/review data are stored in the same database but are **kept separate** from seed users (see below).

### How seed users stay separate from real users

- **Seed users** have `OAuthSubjectId` values starting with `seed:` (for example `seed:user:0042`).
- **Real users** have Google OAuth subject ids (they do **not** start with `seed:`).
- Seed movies are identified by **`TmdbId >= 1000001`**. Movies you add from TMDB use real TMDB ids (typically much lower).
- The recommendation engine treats seed activity as **community** data; your signed-in dashboard only shows **your** shelf and reviews in the UI.

### Run the seed

From `backend/` (stop the API first if it is running — SQLite allows one writer):

```bash
cd backend
dotnet run -- seed
```

- First run (empty seed tables): inserts the full dataset and prints counts.
- If target counts are already present: **skips insert** (idempotent) and prints counts.
- If **partial or outdated** seed data exists (for example after changing scale): prints a message — run with `--reset` below.

### Reset and reseed

Removes **seed-only** data (seed users, seed movies in the `TmdbId >= 1000001` range, their genres, shelf rows, and reviews), then inserts a fresh dataset. **Real Google OAuth users are not deleted.**

```bash
dotnet run -- seed --reset
```

### Check that seed worked

After seeding, the command prints counts, for example:

```text
  Seed movies:              5000 (target 5000)
  Seed users:               500 (target 500)
  Seed UserMovies:          8000 (target 8000)
  Seed reviews:             2000 (target 2000)
  Seed interactions total:  10000 (target 10000)
  Real OAuth users:         3 (unchanged by seed)
```

Optional SQLite checks (from `backend/`):

```bash
# Seed movies (TmdbId >= 1000001)
sqlite3 movienest.db "SELECT COUNT(*) FROM Movies WHERE TmdbId >= 1000001;"

# Simulated users
sqlite3 movienest.db "SELECT COUNT(*) FROM Users WHERE OAuthSubjectId LIKE 'seed:%';"

# Seed shelf rows (watchlist + watched)
sqlite3 movienest.db "SELECT COUNT(*) FROM UserMovies um JOIN Users u ON u.Id = um.UserId WHERE u.OAuthSubjectId LIKE 'seed:%';"

# Seed reviews
sqlite3 movienest.db "SELECT COUNT(*) FROM Reviews r JOIN Users u ON u.Id = r.UserId WHERE u.OAuthSubjectId LIKE 'seed:%';"

# Real Google users (should not change when you seed)
sqlite3 movienest.db "SELECT COUNT(*) FROM Users WHERE OAuthSubjectId NOT LIKE 'seed:%';"
```

**Duration:** A full `dotnet run -- seed --reset` on a laptop takes about **8 seconds** end-to-end (including app startup); the insert step alone is about **3 seconds** after the reset clears old seed rows.

## Stack

- **Backend:** ASP.NET Core, EF Core, SQLite (local and production on Render)
- **Frontend:** Vite + React, React Router (hosted on Vercel)
- **Hosting:** Render (Docker API) + Vercel (SPA)
- **Auth:** Google OAuth (cookie sessions)
- **External API:** TMDB (movie search); genres are fetched from TMDB and stored on `Movie` via `MovieGenre`

## Data sources

- **Movie metadata, genres, and posters:** TMDB (live fetch on Discover/Search; cached in the database when you add to watchlist or write a review)
- **Watchlist, history, ratings, and reviews:** user-owned data stored in the MovieNest database
- **Poster image URLs:** served from `image.tmdb.org`

## Recommendations (Option C)

MovieNest implements **Option C — instructor-approved alternative**: a **personalized recommendation engine** (not SignalR and not MCP). The original proposal also mentioned a **SignalR live activity feed** (Option A); that remains **deferred/out of scope** unless added later.

### What it does

Signed-in users see up to **10 movie suggestions** on the home dashboard under **Recommended for you**. Suggestions are **computed on demand** when the app calls the API — they are not written to the database on each request. Movies already on your watchlist, watched history, or review list are excluded.

The engine ranks candidates using:

- **Your genre tastes** — built from genres on movies in your watchlist (lighter weight), watched history (stronger weight), and reviews (strongest when rated 4+ stars).
- **Similar members** — other users whose genre activity overlaps with yours; their highly rated or shelved movies can become candidates.
- **Community signals** — watch activity from **500 seeded simulated users** (8,000 shelf rows + 2,000 reviews) and high ratings from other members’ reviews.

The UI shows each suggestion’s **title, release year, poster, and genres**. Results refresh when you navigate back to Home after changing your shelf or reviews.

### Data used

| Source | Role |
|--------|------|
| **Watchlist** (`UserMovies`, status `watchlist`) | Genre weights; excluded from results |
| **Watched history** (`UserMovies`, status `watched`) | Stronger genre weights; excluded from results |
| **Reviews / ratings** | Extra genre weight (higher for 4–5 stars); reviewed movies excluded |
| **Movie genres** (`MovieGenre` / TMDB) | Drives personal affinity and candidate discovery |
| **Seeded community data** | **5,000** seed movies, **500** fake users, **10,000** interactions (8k shelf + 2k reviews) for cold-start and community ranking |

Run [`dotnet run -- seed --reset`](#reset-and-reseed) from `backend/` so the community layer has the full scaled dataset (5,000 movies, 500 users, 10,000 interactions).

### API

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET` | `/api/recommendations` | Required (cookie) | Returns a JSON array of up to 10 recommendations for the signed-in user |

**Example response shape** (each item):

```json
{
  "movieId": 42,
  "title": "Example Film",
  "releaseYear": 2010,
  "posterUrl": "https://image.tmdb.org/t/p/w500/...",
  "genres": [{ "id": 1, "name": "Drama" }]
}
```

Unauthorized requests return **401**. Try it in Swagger (`http://localhost:5102/swagger`) or with curl after copying your session cookie from the browser:

```bash
curl -s -H 'Cookie: .AspNetCore.Cookies=YOUR_COOKIE' \
  http://localhost:5102/api/recommendations
```

### Demo locally

With the [API and frontend running](#run-locally):

1. **Sign in** with Google (`Log in with Google` on the home page).
2. **Add movies** to your **Watchlist** or mark them **watched** (Discover, Search, or Watchlist). TMDB-backed titles include genres, which improves personalization.
3. **Add reviews/ratings** on the **Reviews** page (4–5 star ratings weigh genres more heavily).
4. From `backend/`, run `dotnet run -- seed --reset` if you have not seeded yet (or want fresh community data).
5. Go to the **signed-in home dashboard** (Home / `/`).
6. Scroll to **Recommended for you** — you should see personalized picks. Navigate to Watchlist or History, make a change, then return to Home to see updated suggestions.

### Known limitations (recommendations)

- Movies **without genre data** (for example, some manually typed titles) contribute less to personalization; TMDB-enriched movies work best.
- **Seed movies** use bundled TMDB poster paths and synthetic titles; they are not the same as movies you add live from Discover/Search.
- Suggestions are **not persisted** per request; each API call recomputes results.
- There is **no movie detail page** from a recommendation card yet — titles are display-only.
- **Automated tests** for the recommendation engine are not yet in the test suite.

## Deployment (Render + Vercel)

Production uses **Vercel** for the React frontend and **Render** for the .NET API. The API runs in a **Docker** container (see `backend/Dockerfile`) because Render’s native Node runtime does not include the .NET SDK.

### Frontend (Vercel)

1. Import the GitHub repo at [vercel.com](https://vercel.com).
2. **Root directory:** `MovieNest-app/frontend`
3. **Environment variable:** `VITE_API_URL=https://movie-nest-app.onrender.com`
4. Deploy, then copy the `.vercel.app` URL for Render CORS settings.

Redeploy Vercel after changing `VITE_API_URL` (Vite bakes env vars at build time).

### Backend (Render)

1. Create a **Web Service** with **Language: Docker** (not Node).
2. **Root directory:** `MovieNest-app/backend`
3. **Dockerfile path:** `./Dockerfile`
4. Leave **Build Command** and **Start Command** empty (Docker handles both).
5. Add [production environment variables](#environment-variables) in the Render dashboard.
6. On first deploy, set `SEED_ON_STARTUP=true` so migrations run and community seed data loads. Set to `false` after a successful seed to speed up cold starts.

The API auto-runs EF Core migrations on startup. Optional seed runs when `SEED_ON_STARTUP=true`.

### Google OAuth (production)

In Google Cloud Console → Credentials → your Web client:

| Setting | URI |
|---------|-----|
| **Authorized redirect URI** | `https://movie-nest-app.onrender.com/signin-google` |
| **Authorized JavaScript origin** | `https://final-project-ephesian-tggae.vercel.app` |

Keep the local redirect URI (`http://localhost:5102/signin-google`) for development.

### Database note

Production uses **SQLite** at `/var/data/movienest.db` on Render (simpler for a class demo on the free tier). Course materials often recommend PostgreSQL for durable multi-user hosting; Azure was the original plan but DePaul subscription limits led to Render + SQLite instead.

## Known limitations

- No movie detail page yet.
- Manual watchlist form still accepts typed titles (TMDB enrichment runs on the backend).
- Seed data is in the database for scale; the UI only shows the signed-in user’s own watchlist.
- Recommendation-specific limits are listed under [Known limitations (recommendations)](#known-limitations-recommendations).
