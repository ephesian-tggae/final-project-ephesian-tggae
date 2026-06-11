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
                <div className="search-poster search-poster--empty" aria-hidden="true">
                  No poster
                </div>
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
                        aria-label={
                          isAdded
                            ? `${movie.title} is already on your watchlist`
                            : isAdding
                              ? `Adding ${movie.title} to watchlist`
                              : `Add ${movie.title} to watchlist`
                        }
                      >
                        {isAdding
                          ? 'Adding…'
                          : isAdded
                            ? 'On watchlist'
                            : 'Add to watchlist'}
                      </button>
                    ) : (
                      <button
                        type="button"
                        onClick={onSignInToSave}
                        aria-label={`Sign in to save ${movie.title} to your watchlist`}
                      >
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
