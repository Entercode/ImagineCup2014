CREATE FUNCTION [dbo].[Timeline]
(
	@UserBindId int,
	@Param float
)
RETURNS @result TABLE
(
	UserId varchar(50),
	Nickname nvarchar(50),
	TweetTime datetime2(2),
	Tweet nvarchar(MAX)
)
AS
BEGIN
	DECLARE @allTweets TABLE(UserId varchar(50), Nickname nvarchar(50), BindId int, ClientTweetTime datetime2(2), Tweet nvarchar(MAX));
	
	INSERT @allTweets
	SELECT D.UserId, A.Nickname, T.BindId, T.ClientTweetTime, T.Tweet
	FROM Tweet T,Account A, AccountDevice D
	WHERE T.BindId = D.BindId AND D.UserId = A.UserId;
	
	INSERT @result
	SELECT T.UserId, T.Nickname, T.ClientTweetTime, T.Tweet
	FROM @allTweets T, NearBindId(@UserBindId, @Param) N
	WHERE T.BindId = N.NearBindId AND T.ClientTweetTime > N.PassedTime
	UNION
	SELECT T.UserId, T.Nickname, T.ClientTweetTime, T.Tweet
	FROM @allTweets T
	WHERE T.BindId = @UserBindId

	RETURN
END