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
In Postman I had to disable SSL certificate verification (Hit the wrench icon and turn off SSL)

For example:
https://localhost:44331/api/Ranking?name=leaderboard&ascending=false&size=5&begin=2
This will get all rankings in the leaderboard named "leaderboard" sorted in descending order and be fixed to the size of 5. And end at rank 3.
[
    {
        "Rank": 7,
        "Username": "User18",
        "Rating": 1800,
        "LeaderboardName": "Leaderboard"
    },
    {
        "Rank": 6,
        "Username": "User19",
        "Rating": 1900,
        "LeaderboardName": "Leaderboard"
    },
    {
        "Rank": 5,
        "Username": "User20",
        "Rating": 2000,
        "LeaderboardName": "Leaderboard"
    },
    {
        "Rank": 4,
        "Username": "User21",
        "Rating": 2100,
        "LeaderboardName": "Leaderboard"
    },
    {
        "Rank": 3,
        "Username": "User22",
        "Rating": 2200,
        "LeaderboardName": "Leaderboard"
    }
]


https://localhost:44331/api/Ranking?username=user23&leaderboard=leaderboard
This should return you a json object with user23 ranking on "leaderboard"
{
    "Rank": 2,
    "Username": "User23",
    "Rating": 2300,
    "LeaderboardName": "Leaderboard"
}

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

https://localhost:44331/api/Ranking?username=PostUser1&leaderboard=Leaderboard 3
This will delete the ranking of "PostUser1" on "Leaderboard 3"
