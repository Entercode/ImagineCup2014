CREATE TABLE [dbo].[StreetPass] (
    [UserBindId]         INT            NOT NULL,
    [PassedDeviceIdHash] VARBINARY (20) NOT NULL,
    [PassedTime]         DATETIME2 (2)  NOT NULL,
    CONSTRAINT [PK_StreetPass] PRIMARY KEY CLUSTERED ([UserBindId] ASC, [PassedTime] ASC),
    CONSTRAINT [FK_StreetPass_ToAccountDevice(PassedDeviceIdHash)] FOREIGN KEY ([PassedDeviceIdHash]) REFERENCES [dbo].[AccountDevice] ([DeviceIdHash]),
    CONSTRAINT [FK_StreetPass_ToAccountDevice(UserBindId)] FOREIGN KEY ([UserBindId]) REFERENCES [dbo].[AccountDevice] ([BindId])
);

