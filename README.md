My solution for the HabitLogger project.

Requirements:
1 This is an application where you’ll log occurrences of a habit.

2 This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)

3 Users need to be able to input the date of the occurrence of the habit

4 The application should store and retrieve data from a real database

5 When the application starts, it should create a sqlite database, if one isn’t present.

6 It should also create a table in the database, where the habit will be logged.

7 The users should be able to insert, delete, update and view their logged habit.

8 You should handle all possible errors so that the application never crashes.

9 You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.

10 Follow the DRY Principle, and avoid code repetition.


Challenges:

Originally I made the solution using a SqlServer database and the SqlClient library in dotnet. I found the concept of ADO.net and how it related to SqlServer and Sqlite confusing. 

Changing the code to use Sqlite instead of my original database was a little challenging because as I found the documentation to be confusing.

I apparantly used ADO. net here, but I'm not sure exactly how it relates to Sqlite or SqlClient. I need to improve my understanding of this.

Features:
Console GUI:

<img width="318" height="144" alt="image" src="https://github.com/user-attachments/assets/c4d3b8c4-0c61-4240-be2f-455cad985864" />

Standard CRUD for interacting with an Sqlite database.







