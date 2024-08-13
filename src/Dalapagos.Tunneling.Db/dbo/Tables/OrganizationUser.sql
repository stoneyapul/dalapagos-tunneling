CREATE TABLE [dbo].[OrganizationUser] (
    [OrganizationUserId] INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationId]     INT              NOT NULL,
    [UserUuid]           UNIQUEIDENTIFIER NOT NULL,
    [SecurityGroupUuid]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_OrganizationUser] PRIMARY KEY CLUSTERED ([OrganizationUserId] ASC),
    CONSTRAINT [FK_Organization_OrganizationUser] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [Index_UserUuid]
    ON [dbo].[OrganizationUser]([UserUuid] ASC);


GO

