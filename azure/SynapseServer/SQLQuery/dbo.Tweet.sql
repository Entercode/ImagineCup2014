CREATE TABLE [dbo].[Tweet] (
    [BindId]             INT            NOT NULL,
    [ClientTweetTime]    DATETIME2 (2)  NOT NULL,
    [ServerRecievedTime] DATETIME2 (2)  NOT NULL,
    [Tweet]              NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Tweet] PRIMARY KEY CLUSTERED ([BindId] ASC, [ServerRecievedTime] ASC),
    CONSTRAINT [FK_Tweet_ToAccountDevice] FOREIGN KEY ([BindId]) REFERENCES [dbo].[AccountDevice] ([BindId])
);

