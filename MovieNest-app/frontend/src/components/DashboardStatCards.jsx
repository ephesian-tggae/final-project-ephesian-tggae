import { Link } from 'react-router-dom';

export default function DashboardStatCards({
  watchlistCount,
  historyCount,
  reviewCount,
  averageRating,
}) {
  const ratingDisplay =
    averageRating === null ? '—' : `${averageRating.toLocaleString()} / 5`;

  const cards = [
    { value: watchlistCount, label: 'On watchlist', to: '/watchlist' },
    { value: historyCount, label: 'Watched', to: '/history' },
    { value: reviewCount, label: 'Reviews written', to: '/reviews' },
    { value: ratingDisplay, label: 'Average rating', to: '/reviews' },
  ];

  return (
    <div className="stats-grid dashboard-stats-grid">
      {cards.map((card) => (
        <Link key={card.label} to={card.to} className="stat-card stat-card--link">
          <strong>{typeof card.value === 'number' ? card.value.toLocaleString() : card.value}</strong>
          <span>{card.label}</span>
          <span className="stat-card-link">View →</span>
        </Link>
      ))}
    </div>
  );
}
