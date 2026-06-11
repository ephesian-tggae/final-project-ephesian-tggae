import GenreTags from './GenreTags';
import TmdbImageCredit from './TmdbImageCredit';

export default function RecommendationList({ recommendations }) {
  if (!recommendations?.length) {
    return null;
  }

  return (
    <>
      <TmdbImageCredit />
      <ul className="watchlist-items">
        {recommendations.map((item) => (
          <li key={item.movieId} className="watchlist-item">
            {item.posterUrl ? (
              <img
                src={item.posterUrl}
                alt={`${item.title} poster`}
                className="search-poster"
              />
            ) : (
              <div className="search-poster search-poster--empty" aria-hidden="true">
                No poster
              </div>
            )}
            <div className="watchlist-item-details">
              <strong>{item.title}</strong>
              {item.releaseYear && <span> ({item.releaseYear})</span>}
              <GenreTags genres={item.genres} />
            </div>
          </li>
        ))}
      </ul>
    </>
  );
}
