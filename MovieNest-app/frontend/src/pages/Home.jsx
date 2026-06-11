import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchPublicStats, startLogin } from '../api';
import { useAuth } from '../AuthContext';
import UserDashboard from '../components/UserDashboard';
import StatusMessage from '../components/StatusMessage';

export default function Home() {
  const { user, error, logout } = useAuth();
  const [stats, setStats] = useState(null);
  const [statsLoading, setStatsLoading] = useState(true);
  const [statsError, setStatsError] = useState(null);

  useEffect(() => {
    fetchPublicStats()
      .then(setStats)
      .catch((err) => setStatsError(err.message))
      .finally(() => setStatsLoading(false));
  }, []);

  return (
    <main className="page landing-page">
      <header className="landing-hero">
        <h1>MovieNest</h1>
        <p className="landing-tagline">
          A community movie platform for fans who want one place to discover films,
          build watchlists, track what they have watched, and share reviews.
        </p>
        <p className="data-attribution landing-tmdb-note">
          Discover and search powered by TMDB.
        </p>
      </header>

      <StatusMessage type="error" message={error ? `Error: ${error}` : null} />

      <section className="landing-features" aria-labelledby="features-heading">
        <h2 id="features-heading">What you can do</h2>
        <ul>
          <li>Discover movies and see what is popular right now</li>
          <li>Build a personal watchlist and mark films as watched</li>
          <li>Sign in with Google so your lists stay private and secure</li>
        </ul>
      </section>

      <section className="landing-stats" aria-labelledby="community-stats-heading">
        <h2 id="community-stats-heading">Community at a glance</h2>
        <StatusMessage
          type="status"
          message={statsLoading ? 'Loading community stats…' : null}
        />
        {stats && (
          <>
            <div className="stats-grid">
              <div className="stat-card">
                <strong>{stats.totalMovies.toLocaleString()}</strong>
                <span>movies tracked</span>
              </div>
              <div className="stat-card">
                <strong>{stats.totalMembers.toLocaleString()}</strong>
                <span>members</span>
              </div>
              <div className="stat-card">
                <strong>{stats.totalActivity.toLocaleString()}</strong>
                <span>watchlist &amp; watched entries</span>
              </div>
            </div>
            <p className="stats-note">Totals only — no personal data shown here.</p>
          </>
        )}
        <StatusMessage
          type="error"
          message={statsError ? `Could not load stats: ${statsError}` : null}
        />
      </section>

      <UserDashboard />

      {user ? (
        <section className="auth-box">
          <p className="signed-in">
            Signed in as <strong>{user.name}</strong>
          </p>
          <p className="email">{user.email}</p>
          <button type="button" onClick={logout}>
            Log out
          </button>
          <nav className="nav" aria-label="Main navigation">
            <Link to="/discover">Discover</Link>
            <Link to="/watchlist">Watchlist</Link>
            <Link to="/history">History</Link>
            <Link to="/reviews">Reviews</Link>
            <Link to="/search">Search</Link>
            <Link to="/profile">Profile</Link>
            <Link to="/settings">Settings</Link>
          </nav>
        </section>
      ) : (
        <section className="landing-cta auth-box">
          <div className="landing-cta-buttons">
            <Link to="/discover" className="landing-btn landing-btn--primary">
              Browse movies
            </Link>
            <button
              type="button"
              className="landing-btn landing-btn--secondary"
              onClick={startLogin}
            >
              Log in with Google
            </button>
          </div>
          <p className="landing-cta-note">
            Browse without an account, or sign in to save your watchlist.
          </p>
        </section>
      )}
    </main>
  );
}
