import { Link } from 'react-router-dom';

export default function Settings() {
  return (
    <main className="page">
      <h1>Settings</h1>
      <p>Protected page — only signed-in users can see this.</p>
      <Link to="/">← Back to home</Link>
    </main>
  );
}
