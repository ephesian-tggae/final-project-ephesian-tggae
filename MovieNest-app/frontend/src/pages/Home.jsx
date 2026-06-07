import { Link } from 'react-router-dom';
import { startLogin } from '../api';
import { useAuth } from '../AuthContext';

export default function Home() {
  const { user, error, logout } = useAuth();

  return (
    <main className="page">
      <h1>MovieNest</h1>
      <p className="subtitle">Home (public)</p>

      {error && <p className="error">Error: {error}</p>}

      {user ? (
        <section className="auth-box">
          <p className="signed-in">
            Signed in as <strong>{user.name}</strong>
          </p>
          <p className="email">{user.email}</p>
          <button type="button" onClick={logout}>
            Log out
          </button>
          <nav className="nav">
            <Link to="/watchlist">Watchlist</Link>
            <Link to="/search">Search</Link>
            <Link to="/profile">Profile</Link>
            <Link to="/settings">Settings</Link>
          </nav>
        </section>
      ) : (
        <section className="auth-box">
          <p>You are not signed in.</p>
          <button type="button" onClick={startLogin}>
            Log in with Google
          </button>
        </section>
      )}
    </main>
  );
}
