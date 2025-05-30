namespace Dalapagos.Tunneling.Api.Tests;

using Api.Mappers;
using Core.Model;
using Shouldly;

public class OperationResultMapperTests
{
    [Fact]
    public void MapDeviceResponsePass()
    {
        var device = new Device
        (
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Device",
            Os.Windows,
            RestProtocol.Http,
            80);

        var operationResult = new OperationResult<Device>(device, true, 200, []);
        var mapper = new DeviceMapper();

        var result = mapper.MapOperationResult(operationResult);

        result.ShouldNotBeNull();
        result.IsSuccessful.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(device.Name);
        result.Data.DeviceId.ShouldBe(device.Id!.Value);
        result.Data.Os.ShouldBe(device.Os.ToString());
        result.Data.RestProtocol.ShouldBe(device.RestProtocol.ToString());
        result.Data.RestPort.ShouldBe(device.RestPort);
    }

    [Fact]
    public void MapDeviceResponseWithErrorPass()
    {
        var operationResult = new OperationResult(false, 500, ["Error Message"]);
        var mapper = new DeviceMapper();

        var result = mapper.MapOperationResult(operationResult);

        result.ShouldNotBeNull();
        result.IsSuccessful.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Length.ShouldBe(1);
        result.Errors[0].ShouldBe("Error Message");
    }
}