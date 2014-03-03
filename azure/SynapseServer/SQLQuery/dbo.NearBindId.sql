CREATE FUNCTION [dbo].[NearBindId]
(
	@UserBindId INT,
	@Parameter FLOAT
)
RETURNS @result TABLE
(
	NearBindId INT,
	FirstPassedTime DATETIME2(2)
)
AS
BEGIN
	DECLARE @near TABLE(UserBindId INT, PassedBindId INT, FirstPassedTime DATETIME2(2), Rate FLOAT);
	DECLARE @first TABLE(BindId INT, FirstPassedTime DATETIME2(2));
	DECLARE @second TABLE(BindId INT, FirstPassedTime DATETIME2(2));

	INSERT @near
	SELECT * FROM NearnessRated(@Parameter);

	INSERT @first
	SELECT F.PassedBindId, F.FirstPassedTime
	FROM @near F
	WHERE F.UserBindId = @UserBindId;

	INSERT @second
	SELECT S.PassedBindId, (CASE WHEN F.FirstPassedTime < S.FirstPassedTime THEN S.FirstPassedTime ELSE F.FirstPassedTime END) AS FirstPassedTime
	FROM @near F, @near S
	WHERE F.UserBindId = @UserBindId AND F.PassedBindId = S.UserBindId AND S.PassedBindId <> @UserBindId AND F.Rate + S.Rate < 100.0;

	INSERT @result
	SELECT * FROM @first
	UNION
	SELECT * FROM @second;


	RETURN
END