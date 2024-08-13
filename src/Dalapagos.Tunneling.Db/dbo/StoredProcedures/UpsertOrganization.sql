
CREATE PROCEDURE [dbo].[UpsertOrganization] 
(
    @OrganizationUuid UNIQUEIDENTIFIER, 
    @OrganizationName NVARCHAR(64)
)
AS
BEGIN
    DECLARE @OrganizationId INT;
    SELECT @OrganizationId = OrganizationId FROM Organization WHERE OrganizationUuid = @OrganizationUuid;
    
    IF @OrganizationId IS NULL
    BEGIN
        INSERT INTO dbo.Organization
        (OrganizationUuid, OrganizationName)
        VALUES
        (@OrganizationUuid, @OrganizationName);
    END
    ELSE
    BEGIN
        UPDATE dbo.Organization
        SET OrganizationName = @OrganizationName 
        WHERE OrganizationId = @OrganizationId;
    END

END;

GO

