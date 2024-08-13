CREATE TABLE [dbo].[Device] (
    [DeviceId]       INT              IDENTITY (1, 1) NOT NULL,
    [DeviceUuid]     UNIQUEIDENTIFIER CONSTRAINT [DEFAULT_Device_DeviceUuid] DEFAULT (newid()) NULL,
    [DeviceGroupId]  INT              NULL,
    [DeviceName]     NVARCHAR (64)    NOT NULL,
    [Os]             INT              CONSTRAINT [DEFAULT_Device_Os] DEFAULT ((0)) NOT NULL,
    [OrganizationId] INT              NOT NULL,
    CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([DeviceId] ASC),
    CONSTRAINT [FK_Device_DeviceGroupId] FOREIGN KEY ([DeviceGroupId]) REFERENCES [dbo].[DeviceGroup] ([DeviceGroupId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Device_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationId])
);


GO

CREATE UNIQUE NONCLUSTERED INDEX [Index_Device_Uuid]
    ON [dbo].[Device]([DeviceUuid] ASC);


GO

