-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
CREATE PROCEDURE CreateTag
	@Name VARCHAR(64)
AS
BEGIN
	IF (@Name IS NULL OR LEN(@Name) < 4)
		RETURN 0;

	INSERT INTO Tags([Name]) VALUES(@Name);
	RETURN (SELECT SCOPE_IDENTITY())
END
GO