CREATE PROCEDURE BalanceReplenishment
	@UserId INT,
	@Amount MONEY
AS
BEGIN
	IF(NOT EXISTS (SELECT * FROM Users WHERE Id = @UserId))
		RETURN 1;

	UPDATE Users SET Balance = Balance + @Amount WHERE Id = @UserId

	RETURN 0;
END
GO
