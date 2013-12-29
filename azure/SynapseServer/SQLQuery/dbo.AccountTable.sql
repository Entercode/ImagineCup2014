CREATE TABLE [dbo].[AccountTable]
(
	[UserId] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [Nickname] NVARCHAR(50) NOT NULL, 
    [MailAddress] NVARCHAR(50) NOT NULL, 
    [PasswordHash] BIGINT NOT NULL, 
    [AuthHash] BIGINT NULL
)
