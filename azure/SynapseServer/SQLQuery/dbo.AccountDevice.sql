CREATE TABLE [dbo].[AccountDevice]
(
	[UserId] NVARCHAR(50) NOT NULL , 
    [DeviceId] BIGINT NOT NULL, 
    [BindId] INT NOT NULL IDENTITY, 
    CONSTRAINT [FK_AccountDevice_ToAccountTable] FOREIGN KEY ([UserId]) REFERENCES [AccountTable]([UserId]), 
    CONSTRAINT [PK_AccountDevice] PRIMARY KEY ([BindId])
)