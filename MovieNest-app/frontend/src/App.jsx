import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './AuthContext';
import ProtectedRoute from './ProtectedRoute';
import Discover from './pages/Discover';
import History from './pages/History';
import Home from './pages/Home';
import Profile from './pages/Profile';
import Reviews from './pages/Reviews';
import Search from './pages/Search';
import Settings from './pages/Settings';
import Watchlist from './pages/Watchlist';
import AppFooter from './components/AppFooter';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/discover" element={<Discover />} />
          <Route
            path="/watchlist"
            element={
              <ProtectedRoute>
                <Watchlist />
              </ProtectedRoute>
            }
          />
          <Route
            path="/history"
            element={
              <ProtectedRoute>
                <History />
              </ProtectedRoute>
            }
          />
          <Route
            path="/reviews"
            element={
              <ProtectedRoute>
                <Reviews />
              </ProtectedRoute>
            }
          />
          <Route
            path="/profile"
            element={
              <ProtectedRoute>
                <Profile />
              </ProtectedRoute>
            }
          />
          <Route
            path="/settings"
            element={
              <ProtectedRoute>
                <Settings />
              </ProtectedRoute>
            }
          />
          <Route
            path="/search"
            element={
              <ProtectedRoute>
                <Search />
              </ProtectedRoute>
            }
          />
        </Routes>
        <AppFooter />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
