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
