namespace Dalapagos.Tunneling.Rport;

public class DeviceGroupContextAccessor : IDeviceGroupContextAccessor
{
    public static readonly AsyncLocal<DeviceGroupContext> Context = new();

    public DeviceGroupContext Current
    {
        get => Context.Value ?? throw new Exception("Call DeviceGroupContextAccessor.Current before sending https requests.");
        set => Context.Value = value;
   }
}