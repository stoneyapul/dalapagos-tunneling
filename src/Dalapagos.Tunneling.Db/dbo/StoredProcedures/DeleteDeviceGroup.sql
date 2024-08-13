
CREATE PROCEDURE [dbo].[DeleteDeviceGroup] 
(
    @DeviceGroupUuid UNIQUEIDENTIFIER
)
AS
BEGIN
    DELETE FROM dbo.DeviceGroup
    WHERE DeviceGroupUuid = @DeviceGroupUuid;
END;

GO

