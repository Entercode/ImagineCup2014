CREATE VIEW [dbo].[TimelineView]
	AS
SELECT D.UserId, A.Nickname, T.BindId, T.ClientTweetTime, T.Tweet
FROM Tweet T,Account A, AccountDevice D
WHERE T.BindId = D.BindId AND D.UserId = A.UserId;