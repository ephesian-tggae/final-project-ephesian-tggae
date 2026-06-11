import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchHistory, fetchReviews, fetchWatchlist } from '../api';
import { useAuth } from '../AuthContext';
import { aggregateGenreCounts, computeAverageRating } from '../utils/dashboardStats';
import DashboardStatCards from './DashboardStatCards';
import GenreBarChart from './GenreBarChart';
import StatusMessage from './StatusMessage';

export default function UserDashboard() {
  const { user } = useAuth();
  const [watchlist, setWatchlist] = useState([]);
  const [history, setHistory] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!user) {
      setWatchlist([]);
      setHistory([]);
      setReviews([]);
      setError(null);
      return;
    }

    setLoading(true);
    setError(null);

    Promise.all([fetchWatchlist(), fetchHistory(), fetchReviews()])
      .then(([watchlistData, historyData, reviewsData]) => {
        if (
          watchlistData === null
          || historyData === null
          || reviewsData === null
        ) {
          setError('Not signed in');
          setWatchlist([]);
          setHistory([]);
          setReviews([]);
          return;
        }

        setWatchlist(watchlistData);
        setHistory(historyData);
        setReviews(reviewsData);
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [user]);

  const genreCounts = useMemo(
    () => aggregateGenreCounts([watchlist, history, reviews]),
    [watchlist, history, reviews],
  );

  const averageRating = useMemo(
    () => computeAverageRating(reviews),
    [reviews],
  );

  if (!user) {
    return null;
  }

  const isEmpty =
    !loading
    && !error
    && watchlist.length === 0
    && history.length === 0
    && reviews.length === 0;

  return (
    <section
      className="user-dashboard landing-stats"
      aria-labelledby="user-dashboard-heading"
    >
      <h2 id="user-dashboard-heading">Your MovieNest</h2>
      <p className="subtitle">
        Your watchlist, watched history, and reviews — saved to your account.
      </p>

      <StatusMessage type="status" message={loading ? 'Loading your dashboard…' : null} />
      <StatusMessage type="error" message={!loading ? error : null} />

      {!loading && !error && (
        <>
          <DashboardStatCards
            watchlistCount={watchlist.length}
            historyCount={history.length}
            reviewCount={reviews.length}
            averageRating={averageRating}
          />

          {isEmpty && (
            <p className="dashboard-empty">
              Get started on{' '}
              <Link to="/discover">Discover</Link> or add a movie to your{' '}
              <Link to="/watchlist">Watchlist</Link>.
            </p>
          )}

          <GenreBarChart genreCounts={genreCounts} />
        </>
      )}
    </section>
  );
}
