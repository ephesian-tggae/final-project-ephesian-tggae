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

# Prompt Log

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

# Prompt Log

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

# Prompt Log

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**

# Prompt Log

## Prompt :

**Tool:** Cursor
**Goal:**
**Prompt:** "..."
**Result:**
**Accepted:**
**Changes I made:**
**Rejected:**
**Tested:**
