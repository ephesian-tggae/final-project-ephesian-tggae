# Who is this app for? What problem does it solve?

MovieNest is for people who watch movies and want one place to discover movies, save movies they want to watch, track movies they already watched, and write reviews. It solves the problem of having movie discovery, watchlists, history, and reviews spread across different places. MovieNest gives user's to have all that movie activity in one account.

# Which domain and third-party API did you choose, and why?

The domain I chose was movies and movie tracking and third-party API was TMDB API. Reason for TMDB was because it gives real movie information like titles, release years, posters, genres, and search results.

# What user-owned data does your app store?

The app stores user-owned data like each user’s watchlist, watched movies, reviews, ratings, and recommendations. This data belongs to the signed in user and is kept separate from other users.

# What are your main entities and their relationships?

The main entities in my app are User, Movie, UserMovie, Review, Genre, MovieGenre, and Recommendation. A user can have many saved movies, watched movies, reviews, and recommendations. A movie can have many genres, and users connect to movies through things like watchlists, reviews, and recommendations.

# Which routes and endpoints are protected, and how?

Some routes and endpoints are protected because they deal with private user data. Pages like watchlist, history, reviews, profile, settings, and search require the user to be signed in. Backend endpoints like /api/me, /api/watchlist, /api/history, /api/reviews, and /api/recommendations are also protected. If someone is not signed in, the backend returns 401.

# How does your backend prevent cross-user data access?

The backend prevents users from accessing each other’s data by using the signed in user from the session. It doesn't trust a user id from the frontend. When the app gets watchlist items, reviews, history, or recommendations, it only gets records that belong to the current user. If one user tries to edit or delete another user’s data, the backend returns 403.

# Which advanced integration did you implement and why?

For the advanced integration, I added a recommendation engine. I chose this because it fits the movie app well. The recommendation system uses the user’s watchlist, watched movies, reviews, genres, and simulated community data to suggest movies.
