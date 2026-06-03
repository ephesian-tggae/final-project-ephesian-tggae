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
