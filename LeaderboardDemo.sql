CREATE Database LeaderboardDB
GO

USE LeaderboardDB
GO

Create Table Rankings
(
	ID int primary key identity,
	Username nvarchar(20) NOT NULL,
	Leaderboard nvarchar(50) NOT NULL,
	Rating int NOT NULL
)

/*Inserts 25 users in leaderboard named "Leaderboard" */
DECLARE @cnt INT = 0;
WHILE @cnt < 25
BEGIN
   INSERT into Rankings values ('User' + CAST(@cnt as nvarchar(20)), 'Leaderboard', 100 * @cnt)
   SET @cnt = @cnt + 1;
END;

/*Inserts 50 users in leaderboard named "Leaderboard 2" */
DECLARE @cnt2 INT = 0;
WHILE @cnt2 < 50
BEGIN
   INSERT into Rankings values ('User' + CAST(@cnt2 as nvarchar(20)), 'Leaderboard 2', 10 * @cnt2)
   SET @cnt2 = @cnt2 + 1;
END;

/*Inserts 25 more users in leaderboard named "Leaderboard" */
DECLARE @cnt3 INT = 25;
WHILE @cnt3 < 50
BEGIN
   INSERT into Rankings values ('User' + CAST(@cnt3 as nvarchar(20)), 'Leaderboard', 10 * @cnt3)
   SET @cnt3 = @cnt3 + 1;
END;

