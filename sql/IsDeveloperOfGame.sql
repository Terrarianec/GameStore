CREATE FUNCTION IsDeveloperOfGame
(
	@GameId INT,
	@UserId INT
)
RETURNS bit
AS
BEGIN
	DECLARE MembersCursor SCROLL CURSOR FOR (SELECT UserId AS Id FROM TeamMembers m INNER JOIN Games g ON m.TeamId = g.TeamId WHERE g.Id = @GameId);
	DECLARE @CurrentMemberId INT = 0;
	DECLARE @IsDev BIT = 0;

	OPEN MembersCursor;

	WHILE @@FETCH_STATUS = 0 AND @IsDev = 0
	BEGIN
		FETCH NEXT FROM MembersCursor INTO @CurrentMemberId;

		IF(@CurrentMemberId = @UserId)
			SET @IsDev = 1;
	END

	CLOSE MembersCursor;
	DEALLOCATE MembersCursor;

	RETURN @IsDev;
END
GO

