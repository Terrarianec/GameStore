CREATE FUNCTION IsGamePurchased
(
	@GameId INT,
	@UserId INT
)
RETURNS BIT
AS
BEGIN
	IF(EXISTS(SELECT * FROM PurchasedGames WHERE GameId = @GameId AND UserId = @UserId) 
	OR EXISTS(SELECT * FROM TeamMembers m INNER JOIN Games g ON m.TeamId = g.TeamId WHERE m.UserId = @UserId AND g.Id = @GameId))
		RETURN 1;

	RETURN 0;
END
GO

