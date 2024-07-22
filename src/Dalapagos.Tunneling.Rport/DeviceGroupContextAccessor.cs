namespace Dalapagos.Tunneling.Rport;

public class DeviceGroupContextAccessor : IDeviceGroupContextAccessor
{
    public static readonly AsyncLocal<DeviceGroupContext> Context = new();

    public DeviceGroupContext Current
    {
        get => Context.Value ?? throw new Exception("Call IDeviceGroupContextAccessor.Current before calling .ExecuteAsync()");
        set => Context.Value = value;
   }
}