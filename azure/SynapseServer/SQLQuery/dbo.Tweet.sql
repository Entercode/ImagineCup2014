CREATE TABLE [dbo].[Tweet]
(
	[BindId] INT NOT NULL PRIMARY KEY, 
    [ClientTweetTime] DATETIME2(2) NOT NULL, 
    [ServerRecieveTime] DATETIME2(2) NOT NULL, 
    [Tweet] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_Tweet_ToAccountDevice] FOREIGN KEY ([BindId]) REFERENCES [AccountDevice]([BindId])
)
