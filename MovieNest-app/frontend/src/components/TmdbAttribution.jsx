export default function TmdbAttribution({ variant = 'inline' }) {
  const text =
    'Movie titles, overviews, genres, and posters from The Movie Database (TMDB).';

  const className =
    variant === 'footer' ? 'data-attribution tmdb-footer' : 'data-attribution';

  return <p className={className}>{text}</p>;
}
