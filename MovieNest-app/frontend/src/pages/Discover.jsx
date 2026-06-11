import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchPopularMovies, searchPublicMovies, startLogin } from '../api';
import { useAuth } from '../AuthContext';
import MovieResultList from '../components/MovieResultList';
import SearchForm from '../components/SearchForm';
import StatusMessage from '../components/StatusMessage';
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

      <SearchForm
        query={query}
        onQueryChange={(e) => setQuery(e.target.value)}
        onSubmit={handleSubmit}
        loading={loading}
        loadingLabel="Loading…"
        allowEmptySubmit
        showPopularButton
        onShowPopular={loadPopular}
        popularDisabled={loading}
        inputLabel="Search movies"
      />

      <StatusMessage type="error" message={error} />
      <StatusMessage type="error" message={watchlistError} />
      <StatusMessage type="success" message={successMessage} />
      <StatusMessage type="status" message={loading ? 'Loading movies…' : null} />

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
