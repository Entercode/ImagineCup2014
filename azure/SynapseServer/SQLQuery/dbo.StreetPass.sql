CREATE TABLE [dbo].[StreetPass] (
    [UserBindId]   INT           NOT NULL,
    [PassedBindId] INT           NOT NULL,
    [PassedTime]   DATETIME2 (2) NOT NULL,
    CONSTRAINT [PK_StreetPass] PRIMARY KEY CLUSTERED ([UserBindId] ASC, [PassedTime] ASC),
    CONSTRAINT [FK_StreetPass_ToAccountDevice(UserBindId)] FOREIGN KEY ([UserBindId]) REFERENCES [dbo].[AccountDevice] ([BindId]),
    CONSTRAINT [FK_StreetPass_ToAccountDevice(DeviceIdHash)] FOREIGN KEY ([PassedBindId]) REFERENCES [dbo].[AccountDevice] ([BindId])
);

