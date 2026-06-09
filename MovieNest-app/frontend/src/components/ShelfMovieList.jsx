export default function ShelfMovieList({ items, renderActions, dateLabel = 'added' }) {
  if (items.length === 0) {
    return null;
  }

  return (
    <ul className="watchlist-items">
      {items.map((item) => (
        <li key={item.id} className="watchlist-item">
          {item.posterUrl ? (
            <img
              src={item.posterUrl}
              alt=""
              className="search-poster"
            />
          ) : (
            <div className="search-poster search-poster--empty">No poster</div>
          )}
          <div className="watchlist-item-details">
            <strong>{item.title}</strong>
            {item.releaseYear && <span> ({item.releaseYear})</span>}
            <span className="meta">
              — {dateLabel} {new Date(item.addedAt).toLocaleString()}
            </span>
            {renderActions?.(item)}
          </div>
        </li>
      ))}
    </ul>
  );
}
