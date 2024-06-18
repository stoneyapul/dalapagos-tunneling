namespace Dalapagos.Tunneling.Core.Commands;

using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Mediator;
using Model;

public record AddDeviceGroupCommand() : IRequest<OperationResult<DeviceGroup>>;
