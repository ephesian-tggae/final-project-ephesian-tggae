import { useState } from 'react';
import { Link } from 'react-router-dom';
import { searchMovies } from '../api';
import MovieResultList from '../components/MovieResultList';

export default function Search() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [searched, setSearched] = useState(false);

  async function handleSubmit(event) {
    event.preventDefault();
    const trimmed = query.trim();
    if (!trimmed) {
      return;
    }

    setLoading(true);
    setError(null);
    setSearched(true);

    try {
      const data = await searchMovies(trimmed);
      if (data === null) {
        setError('Not signed in');
        setResults([]);
      } else {
        setResults(data);
      }
    } catch (err) {
      setError(err.message);
      setResults([]);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="page search-page">
      <h1>Search</h1>
      <p className="subtitle">
        Search movies from TMDB. Movie data and posters from TMDB.
      </p>

      <form className="search-form" onSubmit={handleSubmit}>
        <label>
          Movie title
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="e.g. Inception"
          />
        </label>
        <button type="submit" disabled={loading}>
          {loading ? 'Searching…' : 'Search'}
        </button>
      </form>

      {error && <p className="error">{error}</p>}

      {!loading && searched && results.length === 0 && !error && (
        <p>No movies found for &ldquo;{query.trim()}&rdquo;.</p>
      )}

      <MovieResultList movies={results} />

      <Link to="/">← Back to home</Link>
    </main>
  );
}
