export default function GenreBarChart({ genreCounts }) {
  const topGenres = (genreCounts ?? []).slice(0, 8);
  const maxCount = topGenres[0]?.count ?? 0;

  return (
    <section className="genre-chart-section">
      <h3>Your genres</h3>
      <p className="genre-chart-note">
        Genre counts from your watchlist, watched history, and reviews (TMDB-enriched
        data).
      </p>

      {topGenres.length === 0 ? (
        <p className="genre-chart-empty">
          Add movies to your watchlist to see genre trends.
        </p>
      ) : (
        <div className="genre-chart" role="img" aria-label="Genre breakdown bar chart">
          {topGenres.map((genre) => (
            <div key={genre.name} className="genre-chart-row">
              <span className="genre-chart-label">{genre.name}</span>
              <div className="genre-chart-bar-track">
                <div
                  className="genre-chart-bar"
                  style={{ width: `${(genre.count / maxCount) * 100}%` }}
                />
              </div>
              <span className="genre-chart-count">{genre.count}</span>
            </div>
          ))}
        </div>
      )}
    </section>
  );
}
