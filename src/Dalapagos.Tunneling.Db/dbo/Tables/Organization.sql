CREATE TABLE [dbo].[Organization] (
    [OrganizationId]   INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationName] NVARCHAR (64)    NOT NULL,
    [OrganizationUuid] UNIQUEIDENTIFIER CONSTRAINT [DEFAULT_Organization_OrganizationUuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED ([OrganizationId] ASC)
);


GO

CREATE UNIQUE NONCLUSTERED INDEX [Index_Organization_Uuid]
    ON [dbo].[Organization]([OrganizationUuid] ASC);


GO

