import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchPublicStats, startLogin } from '../api';
import { useAuth } from '../AuthContext';

export default function Home() {
  const { user, error, logout } = useAuth();
  const [stats, setStats] = useState(null);
  const [statsError, setStatsError] = useState(null);

  useEffect(() => {
    fetchPublicStats()
      .then(setStats)
      .catch((err) => setStatsError(err.message));
  }, []);

  return (
    <main className="page landing-page">
      <header className="landing-hero">
        <h1>MovieNest</h1>
        <p className="landing-tagline">
          A community movie platform for fans who want one place to discover films,
          build watchlists, track what they have watched, and share reviews.
        </p>
      </header>

      {error && <p className="error">Error: {error}</p>}

      <section className="landing-features">
        <h2>What you can do</h2>
        <ul>
          <li>Discover movies and see what is popular right now</li>
          <li>Build a personal watchlist and mark films as watched</li>
          <li>Sign in with Google so your lists stay private and secure</li>
        </ul>
      </section>

      {stats && (
        <section className="landing-stats">
          <h2>Community at a glance</h2>
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
        </section>
      )}

      {statsError && <p className="error">Could not load stats: {statsError}</p>}

      {user ? (
        <section className="auth-box">
          <p className="signed-in">
            Signed in as <strong>{user.name}</strong>
          </p>
          <p className="email">{user.email}</p>
          <button type="button" onClick={logout}>
            Log out
          </button>
          <nav className="nav">
            <Link to="/discover">Discover</Link>
            <Link to="/watchlist">Watchlist</Link>
            <Link to="/history">History</Link>
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
