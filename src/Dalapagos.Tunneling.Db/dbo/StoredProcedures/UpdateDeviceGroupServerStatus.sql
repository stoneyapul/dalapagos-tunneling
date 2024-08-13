
CREATE PROCEDURE [dbo].[UpdateDeviceGroupServerStatus] 
(
    @DeviceGroupUuid UNIQUEIDENTIFIER, 
    @ServerStatus INT
)
AS
BEGIN
    UPDATE dbo.DeviceGroup
    SET ServerStatus = @ServerStatus
    WHERE DeviceGroupUuid = @DeviceGroupUuid;
END;

GO

