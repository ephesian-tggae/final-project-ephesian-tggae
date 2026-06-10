import TmdbAttribution from './TmdbAttribution';

export default function AppFooter() {
  return (
    <footer className="app-footer">
      <TmdbAttribution variant="footer" />
      <p className="tmdb-legal">
        This product uses the{' '}
        <a href="https://www.themoviedb.org/" target="_blank" rel="noreferrer">
          TMDB
        </a>{' '}
        API but is not endorsed or certified by TMDB.
      </p>
    </footer>
  );
}
