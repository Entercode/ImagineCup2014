CREATE TABLE [dbo].[StreetPass]
(
	[UserBindId] INT NOT NULL, 
    [PassedDeviceId] BIGINT NOT NULL, 
    [PassedTime] DATETIME2(2) NOT NULL, 
    CONSTRAINT [FK_StreetPass_ToAccountDevice] FOREIGN KEY ([UserBindId]) REFERENCES [AccountDevice]([BindId]), 
    CONSTRAINT [PK_StreetPass] PRIMARY KEY ([UserBindId],[PassedTime]) 
)
