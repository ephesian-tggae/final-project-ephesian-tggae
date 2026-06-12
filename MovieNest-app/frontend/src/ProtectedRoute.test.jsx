import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import ProtectedRoute from './ProtectedRoute';
import { useAuth } from './hooks/useAuth';

vi.mock('./hooks/useAuth');

function renderProtectedRoute(initialPath = '/watchlist') {
  return render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Routes>
        <Route path="/" element={<div>Home page</div>} />
        <Route
          path="/watchlist"
          element={
            <ProtectedRoute>
              <div>Protected content</div>
            </ProtectedRoute>
          }
        />
      </Routes>
    </MemoryRouter>,
  );
}

describe('ProtectedRoute', () => {
  beforeEach(() => {
    vi.mocked(useAuth).mockReset();
  });

  it('shows a loading message while auth is loading', () => {
    vi.mocked(useAuth).mockReturnValue({
      user: null,
      loading: true,
      error: null,
      logout: vi.fn(),
    });

    renderProtectedRoute();

    expect(
      screen.getByText('Checking if you are signed in…'),
    ).toBeInTheDocument();
  });

  it('redirects to the home page when there is no user', () => {
    vi.mocked(useAuth).mockReturnValue({
      user: null,
      loading: false,
      error: null,
      logout: vi.fn(),
    });

    renderProtectedRoute();

    expect(screen.getByText('Home page')).toBeInTheDocument();
    expect(screen.queryByText('Protected content')).not.toBeInTheDocument();
  });

  it('shows protected content when there is a signed-in user', () => {
    vi.mocked(useAuth).mockReturnValue({
      user: { email: 'user@test.local', name: 'Test User' },
      loading: false,
      error: null,
      logout: vi.fn(),
    });

    renderProtectedRoute();

    expect(screen.getByText('Protected content')).toBeInTheDocument();
  });
});
