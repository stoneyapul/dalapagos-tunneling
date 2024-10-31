SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Device](
	[DeviceId] [int] IDENTITY(1,1) NOT NULL,
	[DeviceUuid] [uniqueidentifier] NULL,
	[DeviceGroupId] [int] NULL,
	[DeviceName] [nvarchar](64) NOT NULL,
	[Os] [int] NOT NULL,
	[OrganizationId] [int] NOT NULL,
	[RestProtocol] [nvarchar](5) NULL,
	[RestPort] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Device] ADD  CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[DeviceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_Device_Uuid] ON [dbo].[Device]
(
	[DeviceUuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Device] ADD  CONSTRAINT [DEFAULT_Device_DeviceUuid]  DEFAULT (newid()) FOR [DeviceUuid]
GO
ALTER TABLE [dbo].[Device] ADD  CONSTRAINT [DEFAULT_Device_Os]  DEFAULT ((0)) FOR [Os]
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_DeviceGroupId] FOREIGN KEY([DeviceGroupId])
REFERENCES [dbo].[DeviceGroup] ([DeviceGroupId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_DeviceGroupId]
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([OrganizationId])
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_OrganizationId]
GO
