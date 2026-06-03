import { useEffect, useState } from 'react';
import { fetchMe, logout, startLogin } from './api';
import './App.css';

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchMe()
      .then(setUser)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleLogout() {
    await logout();
    setUser(null);
  }

  return (
    <main className="page">
      <h1>MovieNest</h1>

      {loading && <p>Checking if you are signed in…</p>}

      {error && <p className="error">Error: {error}</p>}

      {!loading && !error && user && (
        <section className="auth-box">
          <p className="signed-in">Signed in as <strong>{user.name}</strong></p>
          <p className="email">{user.email}</p>
          <button type="button" onClick={handleLogout}>
            Log out
          </button>
        </section>
      )}

      {!loading && !error && !user && (
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

export default App;
