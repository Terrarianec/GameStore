CREATE PROCEDURE TransferMoney
	@SenderId INT,
	@RecipientId INT,
	@Amount MONEY
AS
BEGIN
	IF(NOT EXISTS (SELECT * FROM Users WHERE Id = @SenderId) OR NOT EXISTS (SELECT * FROM Users WHERE Id = @RecipientId))
		RETURN 1;

	DECLARE @SenderBalance INT = (SELECT Balance FROM Users WHERE Id = @SenderId);

	IF(@SenderBalance < @Amount)
		RETURN 1;

	BEGIN TRANSACTION 
		UPDATE Users SET Balance = Balance + @Amount WHERE Id = @RecipientId;
		UPDATE Users SET Balance = Balance - @Amount WHERE Id = @SenderId;
	COMMIT

	RETURN 0;
END
GO
