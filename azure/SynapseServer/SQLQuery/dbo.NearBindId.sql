CREATE FUNCTION [dbo].[NearBindId]
(
	@UserBindId int,
	@Parameter float
)
RETURNS @result TABLE
(
	NearBindId int,
	PassedTime datetime2(2)
)
AS
BEGIN
	DECLARE @near TABLE(UserBindId INT, PassedBindId INT,PassedTime datetime2(2), Rate float);
	DECLARE @first TABLE(BindId INT, PassedTime datetime2(2));
	DECLARE @second TABLE(BindId INT, PassedTime datetime2(2));

	INSERT @near
	SELECT * FROM NearnessRated(@Parameter);

	INSERT @first
	SELECT F.PassedBindId, F.PassedTime
	FROM @near F
	WHERE F.UserBindId = @UserBindId AND F.Rate < 100.0;

	INSERT @second
	SELECT S.PassedBindId, (CASE WHEN F.Rate < S.Rate THEN S.PassedTime ELSE F.PassedTime END) AS PassedTime
	FROM @near F, @near S
	WHERE F.UserBindId = @UserBindId AND F.PassedBindId = S.UserBindId AND S.PassedBindId <> @UserBindId AND F.Rate + S.Rate < 100.0;

	INSERT @result
	SELECT * FROM @first
	UNION
	SELECT * FROM @second;


	RETURN
END