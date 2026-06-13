# Three ways Copilot accelerated or improved your work.

1. Cursor helped me set up the early parts of MovieNest, like the database, Google login, protected endpoints, and connecting React to the API. This helped a lot because setting up a full-stack project can be confusing at first. Even when it came to testing, even though I had to make sure to do it on my own, Cursor would set the expectation whcich gave me a good starting point.

2. After I had one feature working, like the watchlist, Cursor helped me build similar features like reviews, genres, search, and adding movies from TMDB. A lot of these features used the same patterns, like DTOs, mappers, protected routes, and form validation. This basically saved time because I didn't have to write every repeated part from scratch.

3. Cursor also helped me set up backend tests, frontend tests, Playwright E2E tests, and GitHub Actions. This would have taken me a long time to figure out by myself.

# Two places where AI output was wrong, insecure, incomplete, or misleading — and how you caught and fixed it.

1. One problem was that a protected backend endpoint returned 302 instead of 401 when I was logged out. This happened because ASP.NET cookie auth tries to redirect users by default. That wouldn't be a good thing for a React app because the frontend needed a clear 401 response.

I caught this by testing the endpoint while signed out. I fixed it by asking Cursor to make API routes return 401 instead of redirecting. After that, I kept testing protected features while logged in and logged out.

2. Github Actions failed for 2 reasons AI didn't anticipate. Had cursor look into the error logs to then made the fixes based off those.

# One architectural decision you made yourself rather than delegating to AI, and why.

One big decision I made myself was using Vercel for the frontend, Render for the backend, and SQLite for the database. Cursor sometimes suggested Azure and PostgreSQL because that is a more common production setup. I decided not to use that because Azure was harder to rely on for my class project, and PostgreSQL would have added more setup. Render with Docker and SQLite was simpler and cheaper for this project.

# One debugging session where you had to understand the code rather than just re-prompting.

One debugging session that stood out was fixing cross-user watchlist security.I wanted to make sure one user could not edit or delete another user’s watchlist item. At first, I was not sure if the issue was in the frontend or backend. I had to read the backend code and understand the flow. By knowing this helped me understand that frontend protected routes are not enough for security. The backend has to enforce ownership too. After the fix, I tested three cases which was being signed out gives 401, a missing item gives 404, and another's users item gives 403.

# How your prompting strategy evolved across the project. What did you learn to do differently?

Learned not to accept AI output just because it builds. Its better to test in the browser, Postman, and Github Actions. Also having my prompt be super specific to the point where I have to express what not to change, how I wanted to test the feature, and the what status codes I expect.
