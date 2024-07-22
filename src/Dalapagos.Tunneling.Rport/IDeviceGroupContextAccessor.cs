namespace Dalapagos.Tunneling.Rport;

public interface IDeviceGroupContextAccessor
{
    DeviceGroupContext Current { get; set; }
}
