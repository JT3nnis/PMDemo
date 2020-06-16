[Installation]
Visual Studio 2019
Make sure to install from the tools and features section
- SQL Server
- Entity Framework 6

SSMS (SQL Server Management Studio 18)
SQL Server 2019 (Developer)

[Setup]
You will need to have your SQL Server running locally.
Clone the Repo
Open SSMS and run the Leaderboard.sql SQL Script
Edit the App.config connection string in the LeaderboardDataAccess Project (Replace the datasource with your SQL Datasource)
Edit the Web.config connection string in the PMDemo Project (Replace the datasource with your SQL Datasource)

Documentation of all available APIs can be found when you launch project
Refer to https://localhost:44331/Help

Use Postman or Fiddler to test API's
(In Postman I had to disable SSL certificate verification)

For example:
https://localhost:44331/api/Ranking?name=leaderboard&ascending=true&size=15&begin=2
This will get all rankings in the leaderboard named "leaderboard" sorted in ascending order and be fixed to the size of 15. And begin at rank 3.


https://localhost:44331/api/ranking
Body:
{
    "Username": "PostUser1",
    "LeaderboardName": "Leaderboard 3",
    "Rating": 4000
}
This will create a new ranking with usernmae "PostUser1" on "Leaderboard 3" with a Rating of 4000

https://localhost:44331/api/ranking
Body:
{
    "Username": "PostUser1",
    "LeaderboardName": "Leaderboard 3",
    "Rating": 5000
}
This will update user "PostUser1" on "Leaderboard 3" with rating 5000

