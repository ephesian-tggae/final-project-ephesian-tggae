import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchHistory, fetchRecommendations, fetchReviews, fetchWatchlist } from '../api';
import { useAuth } from '../hooks/useAuth';
import { aggregateGenreCounts, computeAverageRating } from '../utils/dashboardStats';
import DashboardStatCards from './DashboardStatCards';
import GenreBarChart from './GenreBarChart';
import RecommendationList from './RecommendationList';
import StatusMessage from './StatusMessage';

export default function UserDashboard() {
  const { user } = useAuth();
  const [watchlist, setWatchlist] = useState([]);
  const [history, setHistory] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [recommendations, setRecommendations] = useState([]);
  const [recommendationsLoading, setRecommendationsLoading] = useState(true);
  const [recommendationsError, setRecommendationsError] = useState(null);
  const [recommendationsUnauthorized, setRecommendationsUnauthorized] = useState(false);

  useEffect(() => {
    if (!user) {
      return;
    }

    let cancelled = false;

    Promise.all([fetchWatchlist(), fetchHistory(), fetchReviews()])
      .then(([watchlistData, historyData, reviewsData]) => {
        if (cancelled) {
          return;
        }

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
      .catch((err) => {
        if (!cancelled) {
          setError(err.message);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, [user]);

  useEffect(() => {
    if (!user) {
      return;
    }

    let cancelled = false;

    fetchRecommendations()
      .then((data) => {
        if (cancelled) {
          return;
        }

        if (data === null) {
          setRecommendations([]);
          setRecommendationsUnauthorized(true);
          return;
        }

        setRecommendations(data);
      })
      .catch((err) => {
        if (!cancelled) {
          setRecommendations([]);
          setRecommendationsError(err.message);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setRecommendationsLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
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

          <section
            className="recommendations-section"
            aria-labelledby="recommendations-heading"
          >
            <h3 id="recommendations-heading">Recommended for you</h3>
            <p className="subtitle">
              Personalized picks from your activity and the MovieNest community.
            </p>

            <StatusMessage
              type="status"
              message={recommendationsLoading ? 'Loading recommendations…' : null}
            />
            <StatusMessage
              type="error"
              message={!recommendationsLoading ? recommendationsError : null}
            />

            {recommendationsUnauthorized && !recommendationsLoading && (
              <p className="dashboard-empty">
                Sign in again to see your recommendations.
              </p>
            )}

            {!recommendationsLoading
              && !recommendationsError
              && !recommendationsUnauthorized
              && recommendations.length === 0 && (
                <p className="dashboard-empty">
                  No recommendations yet. Add movies on{' '}
                  <Link to="/discover">Discover</Link> or your{' '}
                  <Link to="/watchlist">Watchlist</Link> to get started.
                </p>
            )}

            {!recommendationsLoading
              && !recommendationsError
              && !recommendationsUnauthorized && (
                <RecommendationList recommendations={recommendations} />
            )}
          </section>
        </>
      )}
    </section>
  );
}
