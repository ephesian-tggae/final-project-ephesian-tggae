import { apiBaseUrl } from './config';

// Send the login cookie on cross-origin requests (React 5173 → API 5102)
const withCredentials = { credentials: 'include' };

export async function fetchHealth() {
  const response = await fetch(`${apiBaseUrl}/api/health`);

  if (!response.ok) {
    throw new Error(`Backend returned ${response.status}`);
  }

  return response.json();
}

export async function fetchMe() {
  const response = await fetch(`${apiBaseUrl}/api/me`, withCredentials);

  if (response.status === 401) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Backend returned ${response.status}`);
  }

  return response.json();
}

export function startLogin() {
  window.location.href = `${apiBaseUrl}/api/auth/login`;
}

export async function logout() {
  const response = await fetch(`${apiBaseUrl}/api/auth/logout`, {
    method: 'POST',
    ...withCredentials,
  });

  if (!response.ok) {
    throw new Error(`Backend returned ${response.status}`);
  }
}

export async function fetchWatchlist() {
  const response = await fetch(`${apiBaseUrl}/api/watchlist`, withCredentials);

  if (response.status === 401) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Backend returned ${response.status}`);
  }

  return response.json();
}

export async function addToWatchlist(title, releaseYear) {
  const response = await fetch(`${apiBaseUrl}/api/watchlist`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, releaseYear: releaseYear || null }),
    ...withCredentials,
  });

  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new Error(body.message || `Backend returned ${response.status}`);
  }

  return response.json();
}

export async function searchMovies(query) {
  const params = new URLSearchParams({ q: query });
  const response = await fetch(`${apiBaseUrl}/api/movies/search?${params}`, withCredentials);

  if (response.status === 401) {
    return null;
  }

  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new Error(body.message || `Backend returned ${response.status}`);
  }

  return response.json();
}
