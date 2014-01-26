CREATE FUNCTION [dbo].[NearnessRated]
(
	@Parameter float
)
RETURNS @result TABLE
(
	UserBindId int,
	PassedBindId int,
	PassedTime datetime2(2),
	Rate float
)
AS
BEGIN
	DECLARE @buf TABLE(UserBindId int, PassedBindId int, PassedTime datetime2(2), Rate float);

	INSERT @buf
	SELECT S.UserBindId, S.PassedBindId, S.PassedTime, dbo.StreetPassRating(S.PassedTime, @Parameter) AS Rate
	FROM StreetPass S;
	
	INSERT @result
	SELECT * FROM @buf WHERE 0.00000 <= Rate AND Rate <= 100.00000;
	RETURN
END