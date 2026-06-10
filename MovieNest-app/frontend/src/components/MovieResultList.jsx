import GenreTags from './GenreTags';

export default function MovieResultList({ movies }) {
  if (movies.length === 0) {
    return null;
  }

  return (
    <ul className="search-results">
      {movies.map((movie) => (
        <li key={movie.tmdbId} className="search-result">
          {movie.posterUrl ? (
            <img src={movie.posterUrl} alt="" className="search-poster" />
          ) : (
            <div className="search-poster search-poster--empty">No poster</div>
          )}
          <div className="search-result-details">
            <strong>{movie.title}</strong>
            {movie.releaseYear && <span> ({movie.releaseYear})</span>}
            <GenreTags genres={movie.genres} />
            {movie.overview && (
              <p className="search-overview">{movie.overview}</p>
            )}
          </div>
        </li>
      ))}
    </ul>
  );
}
