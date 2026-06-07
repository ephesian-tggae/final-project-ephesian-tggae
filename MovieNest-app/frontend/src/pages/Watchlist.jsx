import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { addToWatchlist, fetchWatchlist, markAsWatched, removeFromWatchlist } from '../api';

export default function Watchlist() {
  const [items, setItems] = useState([]);
  const [title, setTitle] = useState('');
  const [releaseYear, setReleaseYear] = useState('');
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

  // Runs on every page load / browser refresh — fetches saved rows from SQLite via the API
  useEffect(() => {
    loadWatchlist()
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleSubmit(event) {
    event.preventDefault();
    if (!title.trim()) {
      return;
    }

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
      <p>
        Saved in the database. Refresh the page or restart the backend — your list
        loads again from <code>GET /api/watchlist</code>.
      </p>

      <form className="watchlist-form" onSubmit={handleSubmit}>
        <label>
          Movie title
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="e.g. Inception"
            required
          />
        </label>
        <label>
          Year (optional)
          <input
            type="number"
            value={releaseYear}
            onChange={(e) => setReleaseYear(e.target.value)}
            placeholder="2010"
          />
        </label>
        <button type="submit" disabled={saving}>
          {saving ? 'Saving…' : 'Add to watchlist'}
        </button>
      </form>

      <button type="button" onClick={handleReload} disabled={loading}>
        Reload from database
      </button>

      {loading && <p>Loading your watchlist from the backend…</p>}
      {error && <p className="error">{error}</p>}

      {!loading && items.length > 0 && (
        <ul className="watchlist-items">
          {items.map((item) => (
            <li key={item.id}>
              <strong>{item.title}</strong>
              {item.releaseYear && ` (${item.releaseYear})`}
              <span className="meta"> — added {new Date(item.addedAt).toLocaleString()}</span>
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
            </li>
          ))}
        </ul>
      )}

      {!loading && !error && items.length === 0 && (
        <p>No movies yet. Add one above.</p>
      )}

      <Link to="/">← Back to home</Link>
    </main>
  );
}
