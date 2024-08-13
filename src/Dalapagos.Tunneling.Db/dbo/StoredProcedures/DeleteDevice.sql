
CREATE PROCEDURE [dbo].[DeleteDevice] 
(
    @DeviceUuid UNIQUEIDENTIFIER
)
AS
BEGIN
    DELETE FROM dbo.Device
    WHERE DeviceUuid = @DeviceUuid;
END;

GO

