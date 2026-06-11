# MovieNest

Community movie platform: track watchlists and watch history, write reviews, and discover films through personalized recommendations.

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

Do not commit secrets, `.env` with real keys, or `movienest.db`.

## Database (SQLite, local)

- File: `backend/movienest.db` (gitignored)
- Apply migrations: `dotnet ef database update` (from `backend/`)

## Seed test data

The seed script adds **simulated** data only. It does **not** delete real Google OAuth users or their watchlist rows.

| Data | Count |
|------|------:|
| Movies (domain records) | 500 |
| Fake users | 50 |
| Watchlist / watched rows | 1,000 |

Fake users use ids like `seed:user:001` and emails like `seed-user-001@movienest.local`.

### Run the seed

From `backend/`:

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

Optional SQLite checks (from `backend/`):

```bash
sqlite3 movienest.db "SELECT COUNT(*) FROM Movies WHERE TmdbId >= 1000001;"
sqlite3 movienest.db "SELECT COUNT(*) FROM Users WHERE OAuthSubjectId LIKE 'seed:%';"
sqlite3 movienest.db "SELECT COUNT(*) FROM UserMovies um JOIN Users u ON u.Id = um.UserId WHERE u.OAuthSubjectId LIKE 'seed:%';"
```

Reseed typically takes a few seconds on a laptop.

## Stack

- **Backend:** ASP.NET Core, EF Core, SQLite (local) / PostgreSQL (production planned)
- **Frontend:** Vite + React, React Router
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
- **Community signals** — watch activity from seeded fake users and high ratings from other members’ reviews.

The UI shows each suggestion’s **title, release year, poster, and genres**. Results refresh when you navigate back to Home after changing your shelf or reviews.

### Data used

| Source | Role |
|--------|------|
| **Watchlist** (`UserMovies`, status `watchlist`) | Genre weights; excluded from results |
| **Watched history** (`UserMovies`, status `watched`) | Stronger genre weights; excluded from results |
| **Reviews / ratings** | Extra genre weight (higher for 4–5 stars); reviewed movies excluded |
| **Movie genres** (`MovieGenre` / TMDB) | Drives personal affinity and candidate discovery |
| **Seeded community data** | Fake users’ shelf rows and community review stats for cold-start and “popular with similar taste” behavior |

Run [`dotnet run -- seed`](#seed-test-data) so the community layer has enough movies and interactions to suggest from.

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
4. Optional: from `backend/`, run `dotnet run -- seed` for community data if suggestions feel sparse.
5. Go to the **signed-in home dashboard** (Home / `/`).
6. Scroll to **Recommended for you** — you should see personalized picks. Navigate to Watchlist or History, make a change, then return to Home to see updated suggestions.

### Known limitations (recommendations)

- Movies **without genre data** (for example, some manually typed titles) contribute less to personalization; TMDB-enriched movies work best.
- **Seed movies** may have fewer genres than real TMDB imports.
- Suggestions are **not persisted** per request; each API call recomputes results.
- There is **no movie detail page** from a recommendation card yet — titles are display-only.
- **Automated tests** for the recommendation engine are not yet in the test suite.

## Deployment (planned)

- **Frontend:** Vercel
- **Backend + database:** Azure App Service + Azure PostgreSQL

Deployed URL and CI badge will be added when Milestone 4 deployment is complete.

## Known limitations

- No movie detail page yet.
- Manual watchlist form still accepts typed titles (TMDB enrichment runs on the backend).
- Seed data is in the database for scale; the UI only shows the signed-in user’s own watchlist.
- Recommendation-specific limits are listed under [Known limitations (recommendations)](#known-limitations-recommendations).
