CREATE FUNCTION [dbo].[NearnessRated]
(
	@Parameter FLOAT
)
RETURNS @result TABLE
(
	UserBindId INT,
	PassedBindId INT,
	FirstPassedTime DATETIME2(2),
	Rate FLOAT
)
AS
BEGIN
	DECLARE @buf TABLE(Rate FLOAT, UserBindId INT, PassedBindId INT, PassedTime datetime2(2));

	INSERT @buf
	SELECT dbo.StreetPassRating(S.PassedTime, @Parameter) AS Rate, S.UserBindId, S.PassedBindId, S.PassedTime
	FROM StreetPass S;
	
	INSERT @result
	SELECT B.UserBindId, B.PassedBindId, (SELECT MIN(PassedTime) AS min_passed_time FROM @buf WHERE 0.0000 <= Rate AND Rate < 100.0000 AND PassedBindId = B.PassedBindId GROUP BY PassedBindId) AS FirstPassedTime, B.Rate
	FROM @buf AS B
	INNER JOIN(
		SELECT MIN(Rate) AS min_rate, PassedBindId
		FROM @buf
		WHERE 0.00000 <= Rate AND Rate < 100.0000
		GROUP BY PassedBindId) AS R
	ON R.min_rate = B.Rate AND R.PassedBindId = B.PassedBindId;

	RETURN;
END