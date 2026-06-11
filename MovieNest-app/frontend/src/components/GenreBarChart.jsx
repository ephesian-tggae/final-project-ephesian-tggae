export default function GenreBarChart({ genreCounts }) {
  const topGenres = (genreCounts ?? []).slice(0, 8);
  const maxCount = topGenres[0]?.count ?? 0;

  return (
    <section className="genre-chart-section" aria-labelledby="genre-chart-heading">
      <h3 id="genre-chart-heading">Your genres</h3>
      <p className="genre-chart-note">
        Genre counts from your watchlist, watched history, and reviews (TMDB-enriched
        data).
      </p>

      {topGenres.length === 0 ? (
        <p className="genre-chart-empty">
          Add movies to your watchlist to see genre trends.
        </p>
      ) : (
        <ul className="genre-chart">
          {topGenres.map((genre) => (
            <li key={genre.name} className="genre-chart-row">
              <span className="genre-chart-label">{genre.name}</span>
              <div className="genre-chart-bar-track" aria-hidden="true">
                <div
                  className="genre-chart-bar"
                  style={{ width: `${(genre.count / maxCount) * 100}%` }}
                />
              </div>
              <span className="genre-chart-count">
                <span className="sr-only">{genre.name}: </span>
                {genre.count}
              </span>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
