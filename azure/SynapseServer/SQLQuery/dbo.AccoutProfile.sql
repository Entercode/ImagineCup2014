CREATE TABLE [dbo].[AccountProfile]
(
	[UserId] VARCHAR(50) NOT NULL PRIMARY KEY, 
    [Profile] NVARCHAR(MAX) NULL, 
    [ImagePath] NVARCHAR(50) NULL, 
    CONSTRAINT [FK_AccountProfile_ToAccount] FOREIGN KEY ([UserId]) REFERENCES [Account]([UserId])
)
