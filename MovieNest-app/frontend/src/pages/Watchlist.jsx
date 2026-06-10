import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { addToWatchlist, fetchWatchlist, markAsWatched, removeFromWatchlist } from '../api';
import MovieEntryForm from '../components/MovieEntryForm';
import ShelfMovieList from '../components/ShelfMovieList';
import TmdbAttribution from '../components/TmdbAttribution';
import UserDataNote from '../components/UserDataNote';
import { validateWatchlistEntry } from '../utils/validateMovieEntry';

export default function Watchlist() {
  const [items, setItems] = useState([]);
  const [title, setTitle] = useState('');
  const [releaseYear, setReleaseYear] = useState('');
  const [fieldErrors, setFieldErrors] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);
  const [removingId, setRemovingId] = useState(null);
  const [markingId, setMarkingId] = useState(null);

  async function loadWatchlist() {
    const data = await fetchWatchlist();
    if (data === null) {
      setError('Not signed in');
      setItems([]);
    } else {
      setItems(data);
      setError(null);
    }
    return data;
  }

  useEffect(() => {
    loadWatchlist()
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  function handleChange(field, value) {
    if (field === 'title') {
      setTitle(value);
    } else if (field === 'releaseYear') {
      setReleaseYear(value);
    }

    if (fieldErrors[field]) {
      setFieldErrors((prev) => {
        const next = { ...prev };
        delete next[field];
        return next;
      });
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const values = { title, releaseYear };
    const errors = validateWatchlistEntry(values);
    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors);
      return;
    }

    setFieldErrors({});
    setSaving(true);
    setError(null);

    try {
      const year = releaseYear ? parseInt(releaseYear, 10) : null;
      await addToWatchlist(title.trim(), year);
      setTitle('');
      setReleaseYear('');
      setLoading(true);
      await loadWatchlist();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
      setLoading(false);
    }
  }

  async function handleReload() {
    setLoading(true);
    setError(null);
    try {
      await loadWatchlist();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleRemove(id) {
    setRemovingId(id);
    setError(null);

    try {
      await removeFromWatchlist(id);
      await loadWatchlist();
    } catch (err) {
      setError(err.message);
    } finally {
      setRemovingId(null);
    }
  }

  async function handleMarkWatched(id) {
    setMarkingId(id);
    setError(null);

    try {
      await markAsWatched(id);
      await loadWatchlist();
    } catch (err) {
      setError(err.message);
    } finally {
      setMarkingId(null);
    }
  }

  return (
    <main className="page">
      <h1>Watchlist</h1>
      <UserDataNote context="watchlist" />
      <TmdbAttribution />
      <p className="data-attribution">
        Tip: add movies faster from Discover or Search using Add to watchlist on TMDB
        results.
      </p>

      <MovieEntryForm
        mode="watchlist"
        values={{ title, releaseYear }}
        errors={fieldErrors}
        onChange={handleChange}
        onSubmit={handleSubmit}
        submitting={saving}
        submitLabel="Add to watchlist"
        submittingLabel="Saving…"
      />

      <button type="button" onClick={handleReload} disabled={loading}>
        Reload from database
      </button>

      {loading && <p>Loading your watchlist from the backend…</p>}
      {error && <p className="error">{error}</p>}

      {!loading && items.length > 0 && (
        <ShelfMovieList
          items={items}
          dateLabel="added"
          renderActions={(item) => (
            <>
              <button
                type="button"
                onClick={() => handleMarkWatched(item.id)}
                disabled={markingId === item.id || removingId === item.id}
              >
                {markingId === item.id ? 'Updating…' : 'Mark as watched'}
              </button>
              <button
                type="button"
                onClick={() => handleRemove(item.id)}
                disabled={removingId === item.id || markingId === item.id}
              >
                {removingId === item.id ? 'Removing…' : 'Remove'}
              </button>
            </>
          )}
        />
      )}

      {!loading && !error && items.length === 0 && (
        <p>No movies yet. Add one above.</p>
      )}

      <Link to="/">← Back to home</Link>
    </main>
  );
}
