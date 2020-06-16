CREATE Database LeaderboardDB
GO

USE LeaderboardDB
GO

Create Table Leaderboards
(
	LeaderboardID int primary key identity,
	LeaderboardName nvarchar(50) NOT NULL,
)

Create Table Rankings
(
	RankingID int primary key identity,
	Username nvarchar(20) NOT NULL,
	LeaderboardID int FOREIGN KEY REFERENCES Leaderboards(LeaderboardID) NOT NULL,
	Rating int NOT NULL
)

CREATE INDEX idx_username
ON Rankings (Username);

CREATE INDEX idx_rating
ON Rankings (Rating);

CREATE INDEX idx_name
ON Leaderboards (LeaderboardName);

INSERT into Leaderboards values ('Leaderboard');
INSERT into Leaderboards values ('Leaderboard 2');

/*Inserts 25 users in leaderboard with id 1 */
DECLARE @cnt INT = 0;
WHILE @cnt < 25
BEGIN
   INSERT into Rankings values ('User' + CAST(@cnt as nvarchar(20)), 1, 100 * @cnt)
   SET @cnt = @cnt + 1;
END;

/*Inserts 50 users in leaderboard with id 2 */
DECLARE @cnt2 INT = 0;
WHILE @cnt2 < 50
BEGIN
   INSERT into Rankings values ('User' + CAST(@cnt2 as nvarchar(20)), 2, 10 * @cnt2)
   SET @cnt2 = @cnt2 + 1;
END;

/*Inserts 25 more users in leaderboard with id 1 */
DECLARE @cnt3 INT = 25;
WHILE @cnt3 < 50
BEGIN
   INSERT into Rankings values ('User' + CAST(@cnt3 as nvarchar(20)), 1, 10 * @cnt3)
   SET @cnt3 = @cnt3 + 1;
END;

