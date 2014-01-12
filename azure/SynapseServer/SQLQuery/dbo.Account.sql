CREATE TABLE [dbo].[Account] (
    [UserId]       VARCHAR (50)   NOT NULL,
    [Nickname]     NVARCHAR (50)   NOT NULL,
    [MailAddress]  VARCHAR (100)  NOT NULL,
    [PasswordHash] VARBINARY (20) NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [AK_Account_Column] UNIQUE NONCLUSTERED ([MailAddress] ASC)
);

