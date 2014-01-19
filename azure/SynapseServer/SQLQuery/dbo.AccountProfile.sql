CREATE TABLE [dbo].[AccountProfile] (
    [UserId]  VARCHAR (50)   NOT NULL,
    [Profile] NVARCHAR (MAX) NULL,
    [ImageBinary]   IMAGE          NULL,
    [ImageType] VARCHAR(50) NULL, 
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_AccountProfile_ToAccount] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Account] ([UserId])
);

