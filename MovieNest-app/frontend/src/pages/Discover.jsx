import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchPopularMovies, searchPublicMovies, startLogin } from '../api';
import { useAuth } from '../AuthContext';
import MovieResultList from '../components/MovieResultList';
import TmdbAttribution from '../components/TmdbAttribution';
import { useWatchlistFromResults } from '../hooks/useWatchlistFromResults';

export default function Discover() {
  const { user } = useAuth();
  const [movies, setMovies] = useState([]);
  const [query, setQuery] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [mode, setMode] = useState('popular');

  const isSignedIn = Boolean(user);
  const {
    addedTmdbIds,
    addingTmdbId,
    successMessage,
    watchlistError,
    handleAddToWatchlist,
    clearWatchlistMessages,
  } = useWatchlistFromResults(isSignedIn);

  async function loadPopular() {
    setLoading(true);
    setError(null);
    clearWatchlistMessages();
    setMode('popular');

    try {
      const data = await fetchPopularMovies();
      setMovies(data);
    } catch (err) {
      setError(err.message);
      setMovies([]);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadPopular();
  }, []);

  async function handleSubmit(event) {
    event.preventDefault();
    const trimmed = query.trim();
    if (!trimmed) {
      loadPopular();
      return;
    }

    setLoading(true);
    setError(null);
    clearWatchlistMessages();
    setMode('search');

    try {
      const data = await searchPublicMovies(trimmed);
      setMovies(data);
    } catch (err) {
      setError(err.message);
      setMovies([]);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="page discover-page">
      <h1>Discover movies</h1>
      <p className="subtitle">
        Browse and search popular films — results are live from TMDB. No account
        required to look around; sign in to save movies to your personal watchlist.
      </p>
      <TmdbAttribution />

      <form className="search-form" onSubmit={handleSubmit}>
        <label>
          Search movies
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="e.g. Inception"
          />
        </label>
        <div className="discover-actions">
          <button type="submit" disabled={loading}>
            {loading ? 'Loading…' : 'Search'}
          </button>
          <button type="button" onClick={loadPopular} disabled={loading}>
            Show popular
          </button>
        </div>
      </form>

      {error && <p className="error">{error}</p>}
      {watchlistError && <p className="error">{watchlistError}</p>}
      {successMessage && <p className="success">{successMessage}</p>}
      {loading && <p>Loading movies…</p>}

      {!loading && mode === 'search' && movies.length === 0 && !error && (
        <p>No movies found for &ldquo;{query.trim()}&rdquo;.</p>
      )}

      {!loading && movies.length > 0 && !isSignedIn && (
        <p className="data-attribution">Sign in to add movies to your watchlist.</p>
      )}

      <MovieResultList
        movies={movies}
        isSignedIn={isSignedIn}
        onAddToWatchlist={handleAddToWatchlist}
        onSignInToSave={startLogin}
        addingTmdbId={addingTmdbId}
        addedTmdbIds={addedTmdbIds}
      />

      {!isSignedIn && (
        <p className="discover-cta">
          Want your own watchlist?{' '}
          <button type="button" className="link-button" onClick={startLogin}>
            Log in with Google
          </button>
        </p>
      )}

      <Link to="/">← Back to home</Link>
    </main>
  );
}
