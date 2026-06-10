import GenreTags from './GenreTags';
import TmdbImageCredit from './TmdbImageCredit';

export default function MovieResultList({
  movies,
  onAddToWatchlist,
  onSignInToSave,
  isSignedIn = false,
  addingTmdbId = null,
  addedTmdbIds = [],
}) {
  if (movies.length === 0) {
    return null;
  }

  const showWatchlistActions = Boolean(onAddToWatchlist || onSignInToSave);
  const addedSet = new Set(addedTmdbIds);

  return (
    <>
      <TmdbImageCredit />
      <ul className="search-results">
        {movies.map((movie) => {
          const isAdded = addedSet.has(movie.tmdbId);
          const isAdding = addingTmdbId === movie.tmdbId;

          return (
            <li key={movie.tmdbId} className="search-result">
              {movie.posterUrl ? (
                <img
                  src={movie.posterUrl}
                  alt={`${movie.title} poster`}
                  className="search-poster"
                />
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
                {showWatchlistActions && (
                  <div className="movie-result-actions">
                    {isSignedIn ? (
                      <button
                        type="button"
                        onClick={() => onAddToWatchlist?.(movie)}
                        disabled={isAdded || isAdding}
                      >
                        {isAdding
                          ? 'Adding…'
                          : isAdded
                            ? 'On watchlist'
                            : 'Add to watchlist'}
                      </button>
                    ) : (
                      <button type="button" onClick={onSignInToSave}>
                        Sign in to save
                      </button>
                    )}
                  </div>
                )}
              </div>
            </li>
          );
        })}
      </ul>
    </>
  );
}
