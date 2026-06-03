import { Link } from 'react-router-dom';
import { useAuth } from '../AuthContext';

export default function Profile() {
  const { user } = useAuth();

  return (
    <main className="page">
      <h1>Profile</h1>
      <p>Protected page — only signed-in users can see this.</p>
      <p>Email: {user.email}</p>
      <Link to="/">← Back to home</Link>
    </main>
  );
}
