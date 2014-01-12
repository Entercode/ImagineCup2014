CREATE TABLE [dbo].[AccountDevice] (
    [UserId]       VARCHAR(50)  NOT NULL,
    [DeviceIdHash] VARBINARY (20) NOT NULL,
    [BindId]       INT            IDENTITY (1, 1) NOT NULL,
    [SessionId]    VARBINARY (20) NULL,
    CONSTRAINT [PK_AccountDevice] PRIMARY KEY CLUSTERED ([BindId] ASC),
    CONSTRAINT [AK_AccountDevice_Column] UNIQUE NONCLUSTERED ([DeviceIdHash] ASC),
    CONSTRAINT [FK_AccountDevice_ToAccount] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Account] ([UserId])
);

