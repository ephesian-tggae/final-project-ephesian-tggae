import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import StatusMessage from './components/StatusMessage';

export default function ProtectedRoute({ children }) {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <StatusMessage type="status" message="Checking if you are signed in…" />
    );
  }

  if (!user) {
    return <Navigate to="/" replace />;
  }

  return children;
}
