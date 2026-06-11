import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchMe } from '../api';
import StatusMessage from '../components/StatusMessage';

export default function Profile() {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchMe()
      .then((data) => {
        if (!data) {
          setError('Backend returned 401 — not signed in');
        } else {
          setProfile(data);
        }
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <main className="page">
      <h1>Profile</h1>
      <p className="subtitle">Data from protected endpoint <code>GET /api/me</code>:</p>

      {loading && (
        <StatusMessage type="status" message="Loading profile from backend…" />
      )}

      {error && <StatusMessage type="error" message={error} />}

      {profile && (
        <pre className="health-data">{JSON.stringify(profile, null, 2)}</pre>
      )}

      <Link to="/">← Back to home</Link>
    </main>
  );
}
