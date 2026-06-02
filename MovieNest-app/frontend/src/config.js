// Vite exposes env vars that start with VITE_
export const apiBaseUrl =
  import.meta.env.VITE_API_URL ?? 'http://localhost:5102';
