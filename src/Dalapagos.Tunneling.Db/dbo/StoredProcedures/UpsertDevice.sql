
CREATE PROCEDURE [dbo].[UpsertDevice] 
(
    @DeviceUuid UNIQUEIDENTIFIER, 
    @DeviceGroupUuid UNIQUEIDENTIFIER, 
    @DeviceName NVARCHAR(64),
    @Os INT,
    @OrganizationUuid UNIQUEIDENTIFIER
    )
AS
BEGIN
    DECLARE @OrganizationId INT;
    SELECT @OrganizationId = OrganizationId FROM Organization WHERE OrganizationUuid = @OrganizationUuid;

    IF @OrganizationId IS NULL
        THROW 51000, 'Organization not found.', 1;

    DECLARE @DeviceGroupId INT;
    SELECT @DeviceGroupId = DeviceGroupId FROM DeviceGroup WHERE DeviceGroupUuid = @DeviceGroupUuid;
    
    DECLARE @DeviceId INT;
    SELECT @DeviceId = DeviceId FROM Device WHERE DeviceUuid = @DeviceUuid;

    IF @DeviceId IS NULL
    BEGIN
        INSERT INTO dbo.Device
        (DeviceUuid, DeviceGroupId, DeviceName, Os, OrganizationId)
        VALUES
        (@DeviceUuid, @DeviceGroupId, @DeviceName, @Os, @OrganizationId);
    END
    ELSE
    BEGIN
        UPDATE dbo.Device
        SET DeviceGroupId = @DeviceGroupId, 
            DeviceName = @DeviceName,
            Os = @Os,
            OrganizationId = @OrganizationId
        WHERE DeviceId = @DeviceId;
    END

END;

GO

