import { useEffect, useState } from 'react';
import { addToWatchlist, fetchWatchlist } from '../api';

export function useWatchlistFromResults(isSignedIn) {
  const [addedTmdbIds, setAddedTmdbIds] = useState([]);
  const [addingTmdbId, setAddingTmdbId] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!isSignedIn) {
      return;
    }

    let cancelled = false;

    fetchWatchlist()
      .then((data) => {
        if (cancelled || data === null) {
          return;
        }

        const ids = data
          .map((item) => item.tmdbId)
          .filter((id) => id != null && id > 0);
        setAddedTmdbIds(ids);
      })
      .catch(() => {});

    return () => {
      cancelled = true;
    };
  }, [isSignedIn]);

  async function handleAddToWatchlist(movie) {
    setAddingTmdbId(movie.tmdbId);
    setError(null);
    setSuccessMessage(null);

    try {
      await addToWatchlist(movie.title, movie.releaseYear, {
        tmdbId: movie.tmdbId,
        posterPath: movie.posterUrl,
      });
      setAddedTmdbIds((prev) =>
        prev.includes(movie.tmdbId) ? prev : [...prev, movie.tmdbId],
      );
      setSuccessMessage(`Added "${movie.title}" to your watchlist.`);
    } catch (err) {
      if (err.message?.toLowerCase().includes('already')) {
        setAddedTmdbIds((prev) =>
          prev.includes(movie.tmdbId) ? prev : [...prev, movie.tmdbId],
        );
        setError('This movie is already on your watchlist.');
      } else {
        setError(err.message);
      }
    } finally {
      setAddingTmdbId(null);
    }
  }

  return {
    addedTmdbIds: isSignedIn ? addedTmdbIds : [],
    addingTmdbId,
    successMessage,
    watchlistError: error,
    handleAddToWatchlist,
    clearWatchlistMessages: () => {
      setError(null);
      setSuccessMessage(null);
    },
  };
}
