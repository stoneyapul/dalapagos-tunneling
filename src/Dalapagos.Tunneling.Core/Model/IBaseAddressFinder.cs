namespace Dalapagos.Tunneling.Core.Model;

public interface IBaseAddressFinder
{
    Task<string> GetBaseAddressAsync(Guid deviceGroupId);
}
