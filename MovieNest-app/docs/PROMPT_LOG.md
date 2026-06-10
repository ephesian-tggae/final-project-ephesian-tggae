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
