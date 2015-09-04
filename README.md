# guiWords
This application is a port of the popular Latin dictionary application Whitacker's Words. Rather than using a series of flat files and a command line, I transferred the data to a SQL database and designed a graphical interface.

The application works by taking the entered search term(s) and passing it to a stored procedure. The stored procedure parses the comma delimited string into a temp table and then queries the database for the forms. The results are dynamically generated and displayed in expanders. There is a history of all searches made in a session for quick reference as you work.

The most recent major feature is a window which shows all of the forms of the word. This window works by taking the id passed through the button in the main form and passing it through a stored procedure. That procedure returns all of the forms of that word in the database. Based on the part of speech, the window generates a set of expanders with grids and fields. Displaying the verb was the hardest part and it was the first thing I worked on (after creating an alternative way which did not work). I replaced the old method with a single extension method which uses LINQ to pull only the forms desired. This filter made it possible to break up the over 200 forms for verbs and display them in their own expanders and grids based on tense, mood, and voice.

There is a serialization dll which seems to not be included in the repository. This dll is required to log in to the remote database. Until I am able to deploy the final solution, I have been using a local database instead. The remote database does not have good data and I have not been able to complete the deployment to that remote database for various reasons.
