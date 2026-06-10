import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchHistory } from '../api';
import ShelfMovieList from '../components/ShelfMovieList';
import TmdbAttribution from '../components/TmdbAttribution';
import UserDataNote from '../components/UserDataNote';

export default function History() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchHistory()
      .then((data) => {
        if (data === null) {
          setError('Not signed in');
          setItems([]);
        } else {
          setItems(data);
          setError(null);
        }
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <main className="page">
      <h1>Watched history</h1>
      <UserDataNote context="history" />
      <TmdbAttribution />

      {loading && <p>Loading your watched movies…</p>}
      {error && <p className="error">{error}</p>}

      {!loading && items.length > 0 && (
        <ShelfMovieList items={items} dateLabel="watched" />
      )}

      {!loading && !error && items.length === 0 && (
        <p>No watched movies yet — mark some from your watchlist.</p>
      )}

      <p>
        <Link to="/watchlist">Go to watchlist</Link>
      </p>
      <Link to="/">← Back to home</Link>
    </main>
  );
}
