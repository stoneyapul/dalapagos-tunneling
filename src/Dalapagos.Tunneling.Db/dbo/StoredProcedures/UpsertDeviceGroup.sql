
CREATE PROCEDURE [dbo].[UpsertDeviceGroup] 
(
    @DeviceGroupUuid UNIQUEIDENTIFIER, 
    @OrganizationUuid UNIQUEIDENTIFIER,
    @DeviceGroupName NVARCHAR(64),
    @ServerLocation NVARCHAR(50),
    @ServerStatus INT,
    @ServerBaseUrl NVARCHAR(255)
)
AS
BEGIN
    DECLARE @OrganizationId INT;
    SELECT @OrganizationId = OrganizationId FROM Organization WHERE OrganizationUuid = @OrganizationUuid;
    
    IF @OrganizationId IS NULL
        RAISERROR ('Organization not found.', 16,1);

    DECLARE @DeviceGroupId INT;
    SELECT @DeviceGroupId = DeviceGroupId FROM DeviceGroup WHERE DeviceGroupUuid = @DeviceGroupUuid;

    IF @DeviceGroupId IS NULL
    BEGIN
        INSERT INTO dbo.DeviceGroup
        (DeviceGroupUuid, OrganizationId, DeviceGroupName, ServerLocation, ServerStatus, ServerBaseUrl)
        VALUES
        (@DeviceGroupUuid, @OrganizationId, @DeviceGroupName, @ServerLocation, @ServerStatus, @ServerBaseUrl);
    END
    ELSE
    BEGIN
        UPDATE dbo.DeviceGroup
        SET OrganizationId = @OrganizationId, 
            DeviceGroupName = @DeviceGroupName,
            ServerLocation = @ServerLocation,
            ServerStatus = @ServerStatus,
            ServerBaseUrl = @ServerBaseUrl
        WHERE DeviceGroupId = @DeviceGroupId;
    END

END;

GO

