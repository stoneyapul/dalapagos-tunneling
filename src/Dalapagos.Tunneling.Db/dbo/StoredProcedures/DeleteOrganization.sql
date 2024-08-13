
CREATE PROCEDURE [dbo].[DeleteOrganization] 
(
    @OrganizationUuid UNIQUEIDENTIFIER
)
AS
BEGIN
    DELETE FROM dbo.Organization
    WHERE OrganizationUuid = @OrganizationUuid;
END;

GO

