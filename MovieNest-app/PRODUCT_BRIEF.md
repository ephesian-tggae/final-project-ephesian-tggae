My app's name is MovieNest and the cor purpose is having a community movie platform where users can track a watchlist and watched history, write rating/reviews, and get personalized movie recommendations based on community activity.

The app is for movie fans who want once place to organize what they've watched, plan what to watch next, and discover movies through recommendations and community reviews.

The domain I chose was movies/films discovery and tracking.

The third-party API I'm using is TMDB for movie titles, posters, genres, and search.

Advanced integration (rubric Option C): a personalized recommendation engine that suggests movies from the user's watch history, reviews, genres, and community activity (including seeded users). Each recommendation has a score and a human-readable reason.

We are not using SignalR/WebSockets (Option A) or MCP (Option B). A live SignalR community feed was mentioned in the original project proposal but is deferred/out of scope unless added later.

User (real OAuth users + seeded simulated users)
Movie (TMDB id + metadata)
Genre (Action, Comedy, etc.)
UserMovie (join table): connects User ↔ Movie with fields like status (watchlist/watching/watched), dates, optional progress
Review: belongs to a User and a Movie (rating + text + timestamps)
Recommendation: belongs to a User and a Movie (score + reason)
Relationships (simple):

User has many UserMovie entries
Movie has many UserMovie entries
User has many Reviews; Movie has many Reviews
Movie has many Genres (many-to-many)
User has many Recommendations

1. As a user, I want to sign in with my account so that my watchlist and reviews are saved securely.

2. As a user, I want to search movies and view details so that I can find movies to add to my lists.

3. As a user, I want to add movies to my watchlist and mark movies as watched so that I can track what I plan to watch and what I've completed

4. As a user, I want to rate and review movies so that I can remember my opinoins and contribute to the community

5. As a user, I want personalized recommendations so that I can discover movies based on my watch history and the community reviews.

I will be using Google OAuth for my OAuth provider.

I plan to host the frontend on Vercel and Azure for the backend.
