import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchPopularMovies, searchPublicMovies, startLogin } from '../api';
import MovieResultList from '../components/MovieResultList';
import TmdbAttribution from '../components/TmdbAttribution';

export default function Discover() {
  const [movies, setMovies] = useState([]);
  const [query, setQuery] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [mode, setMode] = useState('popular');

  async function loadPopular() {
    setLoading(true);
    setError(null);
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
      {loading && <p>Loading movies…</p>}

      {!loading && mode === 'search' && movies.length === 0 && !error && (
        <p>No movies found for &ldquo;{query.trim()}&rdquo;.</p>
      )}

      <MovieResultList movies={movies} />

      <p className="discover-cta">
        Want your own watchlist?{' '}
        <button type="button" className="link-button" onClick={startLogin}>
          Log in with Google
        </button>
      </p>

      <Link to="/">← Back to home</Link>
    </main>
  );
}
