export default function GenreTags({ genres }) {
  if (!genres?.length) {
    return null;
  }

  return (
    <div className="genre-tags">
      {genres.map((genre) => (
        <span key={genre.tmdbGenreId ?? genre.name} className="genre-tag">
          {genre.name}
        </span>
      ))}
    </div>
  );
}
