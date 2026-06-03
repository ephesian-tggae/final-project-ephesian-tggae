import { Link } from 'react-router-dom';
import { useAuth } from '../AuthContext';

export default function Watchlist() {
  const { user } = useAuth();

  return (
    <main className="page">
      <h1>Watchlist</h1>
      <p>Protected page — only signed-in users can see this.</p>
      <p>Hello, {user.name}.</p>
      <Link to="/">← Back to home</Link>
    </main>
  );
}
