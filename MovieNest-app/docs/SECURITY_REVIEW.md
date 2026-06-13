# OAuth/OIDC flow: is the callback handled securely? Is the state parameter validated?

MovieNest uses Google OAuth for login. The user clicks “Log in with Google,” gets sent to Google, and then Google sends them back to the backend callback route. The backend handles the callback and creates the session. The callback should be handled securely because that is where the app confirms the login. Plus the state parameter is important too because it helps protect against fake login callback attacks. Also making sure the OAuth flow validates state and does not just trust any callback request.

# Token/session storage: where are tokens stored in the browser, and what are the tradeoffs?

MovieNest uses session cookies. security options like HttpOnly, Secure, and SameSite.

# Protected routes: are frontend guards sufficient alone, or does the backend enforce independently?

Frontend guards are not enough because a user could still call the backend API directly. That is why the backend also protects its endpoints. If a user is not signed in, protected backend endpoints return 401 unauthorized.

# Authorization: how is user isolation enforced? What would happen if a user replaced an ID in a URL?

The backend uses the signed in user from the session. It doesn't trust a user is from the frontend. If a user changes an id in the URL to try to access another user’s watchlist item or review, the backend checks ownership and returns 403 Forbidden.

# Input validation: what inputs are validated, and at which layer?

MovieNest validates inputs like movie titles, release years, review ratings, and review text. The frontend gives quick feedback, but the backend validation is more important because users can call the API directly.

# Error handling: do error messages expose implementation details?

The app should return consistent error messages and should not expose stack traces, database details, or internal exception messages.

# CORS: what origins are allowed, and why?

CORS controls which frontend websites are allowed to call the backend API. In production, the backend should only allow the deployed Vercel frontend origin and not allow every website.

This matters because the frontend and backend are hosted separately. The backend should allow the real MovieNest frontend but avoid being open to random origins.

# HTTPS: is it enforced in production?

Production uses HTTPS through Vercel and Render. HTTPS protects login sessions and private user data while requests travel between the browser and server.

# Secrets management: where are secrets stored, and how are they injected?

Google OAuth secrets, TMDB API keys, and production settings should be stored in environment variables, not committed to GitHub.

# Dependency vulnerabilities: have you run npm audit and dotnet list package --vulnerable?

Yes

# Known limitations: what security properties does your app NOT provide?

It does not have advanced rate limiting, full security logging, admin monitoring, or a full security audit. SQLite is also not ideal for a large production app.
