CREATE TABLE [dbo].[DeviceGroup] (
    [DeviceGroupId]   INT              IDENTITY (1, 1) NOT NULL,
    [DeviceGroupUuid] UNIQUEIDENTIFIER CONSTRAINT [DEFAULT_DeviceGroup_DeviceGroupUuid] DEFAULT (newid()) NOT NULL,
    [OrganizationId]  INT              NOT NULL,
    [DeviceGroupName] NVARCHAR (64)    NOT NULL,
    [ServerLocation]  NVARCHAR (50)    NOT NULL,
    [ServerStatus]    INT              CONSTRAINT [DEFAULT_DeviceGroup_ServerStatus] DEFAULT ((0)) NOT NULL,
    [ServerBaseUrl]   NVARCHAR (255)   NULL,
    CONSTRAINT [PK_DeviceGroup] PRIMARY KEY CLUSTERED ([DeviceGroupId] ASC),
    CONSTRAINT [FK_DeviceGroup_OrgId] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationId]) ON DELETE CASCADE
);


GO

CREATE UNIQUE NONCLUSTERED INDEX [Index_DeviceGroup_Uuid]
    ON [dbo].[DeviceGroup]([DeviceGroupUuid] ASC);


GO

