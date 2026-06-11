# Prompt Log

## Prompt 1: EF Core DbContext

**Tool:** Cursor

**Goal:** Set up the backend database using Entity Framerwork Core with the existing models

**Prompt:** "I want you to set up the database for the backend using EF Core. Make sure you use
User and Movie classes that are already in the project to both become tables in the database.
Then use SQLite for now so the database can run locally on my computer.

Create and run the first migration so the database file and tables are actually made. Don't any
anything more. I want you to run dotnet build then tell me what commands you used."

**Result:** created the database setup for the backend. It added a DbContext, connected the project to SQLite, and made the User and Movie classes into database tables. It also created and ran the first migration
**Accepted:** I kept the DbContext, the SQLite setup, and the tables because they matched what I needed for this setup.
**Changes I made:** none
**Rejected:** none
**Tested:** I checked the result by running dotnet build in the backend.

## Prompt 2: Set up Google login

**Tool:** Cursor

**Goal:** I wanted users to sign in with Google, come back to the app, and be saved in the database.

**Prompt:** "I want you to set up Google login for my MovieNest backend for me.

I already have the backend database working with EF Core and SQLite, and I already saved my Google
login client ID and secret in user-secrets. I want users to be able to go to a login endpoint, sign
in with Google, and come back to the app without errors. After they log in, the app should save them
in the database if they are new, or update their information if they already exist. Also add a logout endpoint.

Keep the code simple and easy to understand. Do not add extra features beyond Google login and
logout. Do not put any secrets in the code or appsettings files. Only change what is needed for this
step. Once done run dotnet build. Tell me the commands you used. The URL I should open to test Google
login and how I can check that the user was saved. Then give a simple explanation of what the main
code changes do."

**Result:** Added a login endpoint, a logout endpoint, and the code needed to send users to Google and bring them back after signing in.

**Accepted:** I kept the Google login setup, the login and logout endpoints, and the database logic for saving or updating users

**Changes I made:** None
**Rejected:** None
**Tested:**Ttested the login URL in the browser, signed in with Google, and checked that the user was saved in the SQLite database.

## Prompt 3: Add a protected endpoint

**Tool:** Cursor

**Goal:** Add one backend endpoint that only works when the user is logged in. If they are not then it should return 401 Unauthorized.
**Prompt:** "I want you to add one protected test endpoint to my MovieNest backend. Give a 401 unauthorized when someone who
isn't logged in tries using the endpoint.

Keep the code simple, show me what commands you used, and the urls I should test."

**Result:** Added a protected endpoint but when testing, it returned 302 instead of 401.

**Accepted:** I kept the protected endpoint because it still required the user to be logged in

**Changes I made:** I used a follow up prompt to fix the response code: "Can you have the one
protected endpoint return 401 for unauthenticated requests". After that, Cursor changed the cookie
login behavior so unauthenticated requests returned 401.

**Rejected:**I rejected the original 302 behavior

**Tested:** I tested the endpoint while logged out

## Prompt 4: Connect react frontend to backend

**Tool:** Cursor
**Goal:** See if the react app can talk to the backend over HTTP.

**Prompt:** "I want you to connect my react frontend to my MovieNest backend over HTTP.
For now just add a simple frontend test page that calls one public backend endpoint and shows
the data on the screen. The goal is to see that the react app can talk to the backend. Make sure the code is
simple and explain for the important parts. Show me how to test it too"

**Result:** Gave a simple frontend page that calls a public backend endpoint and shows the
returned data on the screen

**Accepted:** I kept the frontend to backend connection
**Changes I made:** none
**Rejected:**none
**Tested:** ran the backend and frontend at the same time then opened the app in my browser to see if the page
showed data coming from the backend public endpoint.

## Prompt 5: Frontend sign in

**Tool:** Cursor
**Goal:** For the user to login in react, sign in with google, return to the app, and have react
show that the user is signed in.

**Prompt:** "I want you to make the sign in flow work from the frontend to the backend now. What I expect is for the user to click a login button in react, get sent to google login, comes back thorugh the backend callback, and then react can tell the user is signed in. Use the current Google login setup I have. Make sure to keep the code simple and easy to understand.

When done show me what commands you used, how the login flow works What urls I should test,
how I can tell in react that the user is signed in."

**Result:** Added a login button, made React send the user to the backend login endpoint, and added a way for it to check
if the user is signed in.

**Accepted:** I kept the whole frontend login flow
**Changes I made:** none
**Rejected:** none
**Tested:** I tested signing into with Google, returned to the app,

## Prompt 6: 3 protected routes

**Tool:** Cursor
**Goal:** Add 3 protected routes can signed in users can open and those that are not can't.

**Prompt:** "I want you to add at least three protected routes in my React app. These pages should only be available
when the user is signed in. If the user is not signed in and tries to open one of these pages, send them back to a
public page like the home page.

Keep the code simple and easy to understand. When done show me what commands you used.
Give me the three protected routes URLs. Tell me how I can test them while logged in and out."

**Result:** The protected routes onlu load when the user is signed in. If the user is signed out and tries to visit one of the 3 pages,
the app redirects them back to th epublic page.

**Accepted:** I kpet the protected routes setup because they proved the routes worked
**Changes I made:** none
**Rejected:** none
**Tested:** I tested the routes by entering the urls when logged off which redirected me the th epublic page. Tested when signed in
to see the they would open normally.

## Prompt 7: Call protected endpoint from React

**Tool:** Cursor
**Goal:** Have React send the signed in user authentication cookie to the backend and show the protected data on the page.

**Prompt:** "I want you to connect one protected endpoint to my React app. I currently have login working and a protected
endpoint that only works when the user is signed in so I want React to call that protected endpoint after the user logs
in and show the returned data on the page.

Make sure the request includes the signed-in user's authentication cookie so the backend knows who the user is. Keep
the code simple and easy to understand. Don't change or add anything else. Tell me what commands you used, how I can
test it logged in and what should ahppened if I test it while logged out."

**Result:** React can now call the protected endpoint after login and shows the returned user data on the page.

**Accepted:** I kept the protected endpoint call and the part that sends the authentication cookie with the request so the backend
can recognize the signed in user.
**Changes I made:** none
**Rejected:** none
**Tested:** I tested while logged in to see if React showed the protected data from the backend. I also tested when signed out
to see that the protected request did not return the signed in user data.

## Prompt 8: Create user owned data

**Tool:** Cursor
**Goal:** To have a feature that can have signed in users create their own data from the app and have it saved in the database.

**Prompt:** "The data should belong to the signed in user only. React should have a simple form or button to create it,
the backend should save it with the current logged in user, and the page should show the saved result after it is created.

Keep the code simple and easy to understand. Show me what commands you sued. Explain how the backend knows which user
owns the data. Show me how I can test it and how I can check that it was saved in the database"

**Result:** Created a form and button that lets users create data in their watchlist. The backend now saves the data in the database
and connects it to the currently signed in user to which is saved and displayed on the page..

**Accepted:** I kept the watchlist feature because it can be created from React, saved in the database, and be linked to the signed
in user.

**Changes I made:** none
**Rejected:** none
**Tested:** Signed in then created the data by choosing a move to add to my watchlist. Watch it display on the page after
I saved it. I also checked the database to see if the data was saved and connected to the signed in user.

## Prompt 9: Movie search

**Tool:** Cursor
**Goal:** So signed in users can search up movies

**Prompt:** "Now I want to update my /search page to actually search movies. Add a protected backend endpoint to search that calls
TMDB using its api key. Make sure to not expose the key in the frontend. Update the React search page so the user can type a movie
name, click search, and see movie results on the page. Keep the code simple and easy to understand."

**Result:** Added backend code that calls TMDB and returns movie results to the app. The page now has a search box
that can show movie results like posters, titles, and the year.

**Accepted:** Kept the protected seqarch feature and the backend tmdb api key setup.
**Changes I made:** none
**Rejected:** none
**Tested:** Signed in and search a movie name to see if the movie appeared. Tested when signed out to see if it was protected.

## Prompt 10: Watchlist update

**Tool:** Cursor
**Goal:** Have users marka watchlist movie as watched

**Prompt:** "I want to add a way for signed in users to mark a watchlist movie as watched. So on the watchlist page,
I should see each movie having a mark as wached button. When I click it, that movie should update and no longer
show on my watchlist because it’s watched now. Only the sined in users should be able to update their own movies
meaning that one user should not be able to change another user's watchlist.

Keep the code simple and only change for what is asked here."

**Result:** The watchlist page now has a button for each movie, and clicking it updates
that movie so it no longer appears in the watchlist.

**Accepted:** I accepted the mark as watched button feature because it function to update
the user's watchlist.

**Changes I made:** none
**Rejected:** none
**Tested:** Tested by logging in, entering a movie, then marked it as watched to see it disappear.

## Prompt 11: Seed data

**Tool:** Cursor
**Goal:** Add test data to the MovieNest database consisting of fake users, movies, and watchlist/watched records.

**Prompt:** "I want you to add a simple way to seed my MovieNest database with test data. After the seeding I need the database
to have about 500 movies, 50 fake users, and 1,000 watchlist or watched movie records. I want the fake users to be clearly seperated
from the real Google users so that it doesn't break the real login system. I also don't want running the seed to not
delete or mess up my real Google user data. Make sure to keep it simple. let me know how to run the seed, how to reset it, and
how to check that the seed worked."

**Result:** The seed the AI created had fake movies, fake users, and fake watchlist/watched records. It also made sure to
keep the fake users seperated from the real Google users so that the login system is not affected.

**Accepted:** I kept the seed setup because it gave the database realistic test data
**Changes I made:** none
**Rejected:** none
**Tested:** Tested by running the seed command and checking that the database had 500, 50, 1000 that I asked for.

## Prompt 12: Public experience

**Tool:** Cursor
**Goal:** Have the public pages have a improved public experience for MovieNest so people who are not signed in
can can still understand and use the app.

**Prompt:** "I want to improve the publix experience for MovieNest so look up Product_Brief and README to understand
what MovieNest is supposed to be. I expect the public pages to explain that MovieNest is for discovering movies,
building watchlists, and much more.

When updating make the home page feel more like a real landing page, not some basic login page. It should explain
the app, have a clear Google login button, and link to a publcic movie browsing page. I believe adding a public
browse or disocvoer page where sign out users can see real movie data like movie posters, titles, years and short
description is a good idea. I do want to remind that this page should not require any login since the movie data
can come from TMDB but as long as the API key stay safe in the backend and not be exposed in React.

Keep the code simple and only change what is needed for the public experience."

**Result:** It updated the landing page by adding public stats and highlights so visitors can see that the app has movie activity.

**Accepted:** I kept the new landing page because it displayed the browsing features and the purpose of the app well
**Changes I made:** none
**Rejected:** I rejected the first plain layout because it didn't look polished enough to be a good public
experience.
**Tested:** Tested by being logged out.

## Prompt 13: Styling

**Tool:** Cursor
**Goal:** Improve on the look of the page

**Prompt:** "The public landing page works but it still looks plain. I want it to look better and feel more like a real
movie app. I want the text easier to read, fix the spacing, make the buttons look cleaner, and make the page feel
more polished. Make the page have a more darker movie style look with light text and clear buttons"

**Result:** The layout, spacing, text readability, style, and buttons were improved.
**Accepted:** Kept the improved design because it came out looking more professional.
**Changes I made:** none
**Rejected:** none
**Tested:** Just viewing the page to see if it's to my liking

## Prompt 14: 403 Error

**Tool:** Cursor
**Goal:** Have code that prevents one signed in user to not be able to update or delete another user's watchlist item

**Prompt:** "I want to fix the watchlist security so that signed in users can't change or delete another user's watchlist
items. The functions of delete and mark as watched actions should only work if the item in the watchlist belongs to
them so if the item doesn't exist, it should return 404. If the item exists but belongs to a different user, return 403 forbidden.

Keep thenormal 401 response for anyone who isn't signed in. Keep the code simple. Tell me how I can test that one
user can't change another user's watchlist items"

**Result:** Updated the backend watchlist security for deleting and marking items as watched. The endpoints now
check whether the watchlist item exists and if it belongs to the signed in user.

**Accepted:** I kpet the security checks because they protect the user owned data and help prove user isolation
**Changes I made:** none
**Rejected:** none
**Tested:** Tested if a signed out user get 401. Tested if a signed in user gets 404 when the item doesn't exist.
Tested when a signed in user gets 403 when trying to update and delete another user's watchlist item through the console
when having the other user's watchlist item id.

## Prompt 15: Review feature

**Tool:** Cursor
**Goal:** For signed in users to have the abilitty to create, view, update, and delete their own movie reviews.

**Prompt:** "I want to add a Review feature to the app. I want signed in users to be able to write reviews for movies,
see their own reviews, edit a review, and delete a review. Each review should belong to the signed in user only so
one user should not be able to edit or delete another user's review. I expect the review to have the movie title, a rating
from 1 to 5, optional review text, and created and updated dates.

Add any backend database changes needed for reviews. Also add protected backend endpoints for creating, reading, updating,
and deleting reviews. Add a simple protected react page for reviews. Keep the code simple and match the style like
the other feature pages."

**Result:** A page where you can write reviews for movies. Its also a protected review endpoint and protected react
page so signed in users can manage their own reviews.

**Accepted:** I kept the review model, review database, protected review endpoints, and react reviews page
**Changes I made:** none
**Rejected:** none
**Tested:** While signed in I created a review, viewed it on the review page, edit it the rating and review text, then finally
deleted it.

## Prompt 16: Genre

**Tool:** Cursor
**Goal:** So movies can show categories like Action, Comedey, Drama, or Horro.

**Prompt:** "I want to add Genre to the app. I expect movies to be able to have genres like Action or Horror where it comes
from TMDB when the data is brought into the app. Add the backend database changes needed so movies can be connected
to genres. A movie can have more than one genre, and the same genre can belong to many movies

When movies are searched, added to the watchlist, or used in reviews, try to save and show their genres too. Update the
UI so movie cards can show genre tags when genre data is available. This can show up in places like search results, discover
movies, watchlist items, or reviews if those pages already show movies cards. Also add a simple public backend endpoint
that can return the list of genres saved in the database

Make sure the code is simple and only chnage what is needed for genres"

**Result:** Added backend database changes so movies can be connected to the genres. The UI is updated so now there
are genre tags that appear on movie cards when seen.

**Accepted:** I kpet the genre database changes, the movie to genre connection, and the UI genre tags because they
function the way I wanted them too

**Changes I made:** none
**Rejected:**none
**Tested:** Tested by going through pages like watchlist to see the genre tags of movies on my list.

## Prompt 17: Movies to watchlist

**Tool:** Cursor
**Goal:** For users to be able to save movies to their watchlist directly from the Discover and Search pages.

**Prompt:** "I want users to now be able to add movies to their watchlist from the Discover and Search pages. So now
I want each movie card to have an "Add to watchlist" button when searched through the Discover and Search pages.
When signed in users clicks it, the movie should be saved to their watchlist with the movie title, year, poster, TMDB
id, and any other movie details already available. If the user is signed out on the public Discover page, they should
see a clear option to sign in before saving a movie.

After a movie is added, the button should show that it was added or already on the watchlist. If the movie is already
saved, show a simple message instead of crashing or showing a confusing error.

Keep the code simple and only change what's needed for this feature. Also update the README accordingly"

**Result:** Connected the TMDB movie cards on Discover and Search to the watchlist feature. Signed in users can now
add a movie from the movie result card, and the movie appears on the Watchlist page with its saved movie details.

**Accepted:** Kept the add to watchlist buttons because they made the movie results more useful
**Changes I made:** none
**Rejected:** none
**Tested:** Signed in, searched for a movie, clicked the "Add to watchlist" button, and seeing that the movie appeared
in the Watchlist page. Did the same with the Discover page and also checked when signed out to see the option to sign in.

## Prompt 18: Resuable react forms

**Tool:** Cursor
**Goal:** To add reusable form components and client side validation to improve the React frontend quality

**Prompt:** "Please create a simple reusable form componets and use them in places where I already have forms. The forms
should show clear field level errors before sending anything to the backend. Just one example would be if the movie title
is empty, then show show an error under the title field. Do a few like that. Keep the design the same with the current
black MovieNest style. Make the forms accessible with labels, useful error messages, and meaningful poster alt text.

Make sure the code is simple, what reusable components were added, which pages now use the reusable forms and how I can test
the validation errors"

**Result:** Improved the frontend form structure by adding reusable form components and using them across existing
pages. Also added clint side validation so users can see error before submitting forms.

**Accepted:** Kept the reusable form components and validation changes because it made the app easier to use
**Changes I made:** none
**Rejected:** none
**Tested:** Tested by submitting empty or invalid values across the pages to see the error messages appear.

## Prompt 19: Dashboard to home page

**Tool:** Cursor
**Goal:** To have a dashboard at the home page for signed in user to show useful user data like reviews, genres, and more

**Prompt:** "I want a dashboard section for signed in users atthe homepage of MovieNest. I expect signed in user's home
page to show a "Your MovieNest" section that uses the user's real data from the app. The dashboard should show simple
stats like how many movies watched, a simple genre chart, and a recent watchlist table that the user can filter and sort

Make the dashbaord look like the rest of the dark MovieNest design. Also make sure it has loading, empty, and error
messages. Keep the code simple and the table should be easy to use and work on smaller screens"

**Result:** Having a dashboard section showing the user stats, genre chart and a recent watchlist table using existing
user data from the app.

**Accepted:** I kept almost everything about the dashboard because it helped make the home page more useful
**Changes I made:** none
**Rejected:** I got rid of the watchlist table because I realized that the more you add to your watchlist the long the table
will be and I decided not to fix that problem because we have a watchlist page so why do i need a table for it in
the home page.
**Tested:** I tested by checking the dashboard in the home page signed in. Checked on by clicking the button leading to the
other pages.

## Prompt 20: Validation and error handling

**Tool:** Cursor
**Goal:** Make request validation cleaner, make the API error responses consisten, and to avoid having random exception
messages returned in different formats.

**Prompt:** "Can I write this to my agent: Can you give me: Request validation using data annotations,
FluentValidation, or equivalent — not manual if-checks, Consistent error responses — a unified error response
shape, not random exception messages, and Centralized error handling middleware or exception filter.

Keep the code simple and tell me hot to test"

**Result:** Added data annotation validation and centralized error handling for the MovieNest API. Endpoints now use a unified
error shape, exception middleware, and a validation endpoint filter.
**Accepted:** kept the validation and error handling changes because they made the backend more consistent
**Changes I made:** none
**Rejected:** none
**Tested:**Tested by sending invalid request data and checking that the API returned a clear validation error. Also tested
not fround and unathorized cases to confirm the API returned a consistent error response shape.

## Prompt 21: Recommendation database model

**Tool:** Cursor
**Goal:** To start the recommendation engine on the backend

**Prompt:** "I want to start the recommendation engine for MovieNest but only on the backend for now. Have a recommendation entity
that connects user to a movie recommendation. Each one should store the user it belongs to, the movie being recommended,
a score for how strong the recommendation is, a short reason why the movie is recommended, and the date it was created.

Make sure a user can't have the same movie recommened more than once. Add this entity to the database context and set
up the relationships with User and Movie. Also create the EF Core migration for this change and make sure the backend
still builds. When done tell me what the recommendation entity stores, what migration was added, how I can run the migration,
and how I can test that the backend still builds"

**Result:** Added a Recommendation entity and connected it to the existing User and Movie models, it also updated the database
context and created an EF Core migration for the new recommendations table.

**Accepted:** I kept the recommendation model because it creates the database foundation needed for the recommendation
engine without making the feature too large at ounce.

**Changes I made:** none
**Rejected:** none
**Tested:** Tested by running the EF Core migration command and checking that the backend still built right.

## Prompt 22: Recommendation service

**Tool:** Cursor
**Goal:** To add the service logic for generating personalized movie recommendations

**Prompt:** "Now work on adding the RecommendationService that can generate personalized movie recommendations for a
signed in user. I feel like the recommendations should use data that the app already has like user's watchlist, the user's
watched movies, movie genres, user's reviews and rating, and community data from the seeded users and reviews/ratings

Return the top 10 recommended movies with each including a score and a short reason as to why the movie was
recommended. Keep the code simple and follow the patterns already used in the backend

When done tell me how the recommendation score is calculated, what data the service uses, how movies are excluded if
the user already has them, and how I can test that the backend still builds"

**Result:** Added a backend ReccommendationService that can generate personalized movie recommendations using existing
MovieNest data

**Accepted:** Kept it because it added the main backend logic needed for the recommendation engine
**Changes I made:** none
**Rejected:** none
**Tested:** Tested by making sure the backend built successfully. Also reviewed the service logic to see that it returns
top recommendations, gives each ine a score and reason, and more

## Prompt 23: Recommendations UI

**Tool:** Cursor
**Goal:** To connect the recommendation feature to the frontend
**Prompt:** "I want to add the personalized recommendations UI to the frontend of the app. The recommendations section should
show the movie poster, title, release year, genres, recommendation score, and reason why the movie was recommended. Use the
MovieNest dark theme and reuse the existing card/list styles where possible. Make sure the page handles loading state, error state,
empty state when there are no recommendations, and 401 case if ther user is not signed in.

Make sure the code is simple and when finished tell me what API function was added, what component was added, and how
the empty, laoding, and error states work"

**Result:** Added the API funciton to get recommendations, create reusable recommendation list component, and display
a "Recommended for you" section on the signed in dashboard.

**Accepted:** Kept most of the recommendations section because it connects the backend feature to the user interface
and makesthe signed in dashboard more useful.
**Changes I made:** none

**Rejected:** I got rid of the recommended score because I felt like it was not a useful tool and can be hard to be
accurate in scoring. Also got rid of the reason why it was recommeneded because I felt like regardless what it said
you would read the summary of the movie to feel if you'd like it or not so I felt it was useless.

**Tested:** Tested by signing in, going to the homepage, and checking that the "Recommended for you" section appeared. I
also checked that recommendations showed the poster, title, year, genres, and that loading, error, empty, and signed out
cases were handles correctly.

## Prompt 24: Scale seed data

**Tool:** Cursor
**Goal:** To scale up the seed data

**Prompt:** "I want to scale up the MovieNest seed data. First only focus on updating the seed data creation. I want you
to update the existing DatabaseSeeder so it can create at least 5,000 movies, 500 simulated users, and 10,000 user owned
interation records. These simulated users should stay separate from real Google users. Make sure the seed data feel more
realistic. I want to make sure the larger seed does not run too slowly. When done I want to see how te seed data
was made more realistic and how to run the seed command"

**Result:** The seeder now creates more movies, simulated users, and user owned interaction records for testing the
app at a larger scale.

**Accepted:** I kept the larger seed counts because it made the app feel more realistic
**Changes I made:** Stopped using made up poster paths with seed movies and instead used TMDB links because after
scaling the seed data the recommended movies poster stopped working.
**Rejected:** none
**Tested:**Tested by running thee seed command and checking that the app created the larger set of movies, simulated
users, and interaction records. Also checked that dotnet build passed.

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**
