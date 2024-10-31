namespace Dalapagos.Tunneling.Repository.EF;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public class EfTunnelingRepository(DalapagosTunnelsDbContext dbContext) : Core.Infrastructure.ITunnelingRepository
{
    public async Task<Core.Model.Device> UpsertDeviceAsync(
        Guid? deviceId, 
        Guid? deviceGroupId, 
        string deviceName, 
        Core.Model.Os os, 
        Guid organizationId,
        Core.Model.RestProtocol? restProtocol,
        ushort? restPort,
        CancellationToken cancellationToken)
    {
        if (!deviceId.HasValue)
        {
            deviceId = Guid.NewGuid();
        }

        var deviceIdParam = new SqlParameter("@DeviceUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceId };
        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceGroupId ?? (object)DBNull.Value, IsNullable = true };
        var deviceNameParam = new SqlParameter("@DeviceName", System.Data.SqlDbType.NVarChar, 64) { Value = deviceName };
        var osParam = new SqlParameter("@Os", System.Data.SqlDbType.Int) { Value = (int)os };
        var restProtocolParam = new SqlParameter("@RestProtocol", System.Data.SqlDbType.NVarChar, 5) { Value = restProtocol?.ToString() ?? (object)DBNull.Value, IsNullable = true };
        var restPortParam = new SqlParameter("@RestPort", System.Data.SqlDbType.Int) { Value = restPort ?? (object)DBNull.Value, IsNullable = true };
        var organizationIdParam = new SqlParameter("@OrganizationUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = organizationId };
        var parms = new List<object> { deviceIdParam, deviceGroupIdParam, deviceNameParam, osParam, organizationIdParam, restProtocolParam, restPortParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertDevice @DeviceUuid, @DeviceGroupUuid, @DeviceName, @Os, @OrganizationUuid, restProtocolParam, restPortParam", 
            parms, 
            cancellationToken);

        return new Core.Model.Device(
            deviceId.Value, 
            deviceGroupId, 
            deviceName, 
            os, 
            restProtocol, 
            restPort);
    }

    public async Task<Core.Model.DeviceGroup> UpsertDeviceGroupAsync(
        Guid? deviceGroupId, 
        Guid organizationId, 
        string deviceGroupName, 
        Core.Model.ServerLocation serverLocation, 
        Core.Model.ServerStatus serverStatus, 
        string? serverBaseUrl,
        CancellationToken cancellationToken)
    {
        if (!deviceGroupId.HasValue)
        {
            deviceGroupId = Guid.NewGuid();
        }

        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceGroupId };
        var organizationIdParam = new SqlParameter("@OrganizationUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = organizationId };
        var deviceGroupNameParam = new SqlParameter("@DeviceGroupName", System.Data.SqlDbType.NVarChar, 64) { Value = deviceGroupName };
        var serverLocationParam = new SqlParameter("@ServerLocation", System.Data.SqlDbType.NVarChar, 50) { Value = serverLocation.ToAzureLocation() };
        var serverStatusParam = new SqlParameter("@ServerStatus", System.Data.SqlDbType.Int) { Value = (int)serverStatus };
        var serverBaseUrlParam = new SqlParameter("@ServerBaseUrl", System.Data.SqlDbType.NVarChar, 255) { Value =  serverBaseUrl ?? (object)DBNull.Value, IsNullable = true };
        var parms = new List<object> { deviceGroupIdParam, organizationIdParam, deviceGroupNameParam, serverLocationParam, serverStatusParam, serverBaseUrlParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertDeviceGroup @DeviceGroupUuid, @OrganizationUuid, @DeviceGroupName, @ServerLocation, @ServerStatus, @ServerBaseUrl", 
            parms,
            cancellationToken);

        return new Core.Model.DeviceGroup(deviceGroupId, organizationId, deviceGroupName, serverLocation, serverStatus, serverBaseUrl);
    }

    public async Task UpdateDeviceGroupServerStatusAsync(Guid deviceGroupId, Core.Model.ServerStatus serverStatus, CancellationToken cancellationToken)
    {
        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceGroupId };
        var serverStatusParam = new SqlParameter("@ServerStatus", System.Data.SqlDbType.Int) { Value = (int)serverStatus };
        var parms = new List<object> { deviceGroupIdParam, serverStatusParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpdateDeviceGroupServerStatus @DeviceGroupUuid, @ServerStatus", 
            parms,
            cancellationToken);
    }

    public async Task<Core.Model.Organization> UpsertOrganizationAsync(Guid? organizationId, string organizationName, CancellationToken cancellationToken)
    {
        if (!organizationId.HasValue)
        {
            organizationId = Guid.NewGuid();
        }

        var organizationIdParam = new SqlParameter("@OrganizationUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = organizationId };
        var organizationNameParam = new SqlParameter("@OrganizationName", System.Data.SqlDbType.NVarChar, 64) { Value = organizationName };
        var parms = new List<object> { organizationIdParam, organizationNameParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertOrganization @OrganizationUuid, @OrganizationName", 
            parms, 
            cancellationToken);

        return new Core.Model.Organization(organizationId, organizationName);
    }

    public async Task DeleteDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var deviceIdParam = new SqlParameter("@DeviceUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceId };
        var parms = new List<object> { deviceIdParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE DeleteDevice @DeviceUuid", 
            parms,
            cancellationToken);
    }

    public async Task DeleteDeviceGroupAsync(Guid deviceGroupId, CancellationToken cancellationToken)
    {
        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceGroupId };
        var parms = new List<object> { deviceGroupIdParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE DeleteDeviceGroup @DeviceGroupUuid", 
            parms, 
            cancellationToken);
    }

    public async Task DeleteOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organizationIdParam = new SqlParameter("@OrganizationUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = organizationId };
        var parms = new List<object> { organizationIdParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE DeleteOrganization @OrganizationUuid", 
            parms, 
            cancellationToken);
    }

    public async Task<IList<Core.Model.Organization>> GetOrganizationsAsync(CancellationToken cancellationToken)
    {
        var organizationEntities = await dbContext.Organizations
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var organizations = new List<Core.Model.Organization>();
        foreach (var organizationEntity in organizationEntities)
        {
            organizations.Add(new Core.Model.Organization(organizationEntity.OrganizationUuid, organizationEntity.OrganizationName));
        }

        return organizations;
    }

    public async Task<IList<Core.Model.OrganizationUser>> GetOrganizationUsersByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var organizationUserEntities = await dbContext.OrganizationUsers
            .AsNoTracking()
            .Include(o => o.Organization)
            .Where(o => o.UserUuid == userId)
            .ToListAsync(cancellationToken);

        var organizationUsers = new List<Core.Model.OrganizationUser>();
        foreach (var organizationUserEntity in organizationUserEntities)
        {
            organizationUsers.Add(
                new Core.Model.OrganizationUser(
                    organizationUserEntity.Organization.OrganizationUuid, 
                    userId, 
                    organizationUserEntity.SecurityGroupUuid,
                    organizationUserEntity.Organization.OrganizationName));
        }

        return organizationUsers;
   }

    public async Task<Core.Model.Device> RetrieveDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
       var deviceEntity = await dbContext.Devices
            .AsNoTracking()
            .Include(d => d.DeviceGroup)
            .Where(d => d.DeviceUuid == deviceId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new DataNotFoundException($"Device with id {deviceId} not found");

        return MapToDevice(deviceEntity.DeviceGroup?.DeviceGroupUuid, deviceEntity);
    }

    public async Task<Core.Model.DeviceGroup> RetrieveDeviceGroupAsync(Guid organizationId, Guid deviceGroupId, CancellationToken cancellationToken)
    {
        var deviceGroupEntity = await dbContext.DeviceGroups
            .Include(d => d.Devices)
            .AsNoTracking()
            .Where(d => d.DeviceGroupUuid == deviceGroupId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new DataNotFoundException($"Device group with id {deviceGroupId} not found");

        return MapToDeviceGroup(organizationId, deviceGroupEntity);
    }

    public async Task<Core.Model.Organization> RetrieveOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organizationEntity = await dbContext.Organizations
            .Include(d => d.DeviceGroups)
            .AsNoTracking()
            .Where(o => o.OrganizationUuid == organizationId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new DataNotFoundException($"Organization with id {organizationId} not found");

        var deviceGroups = new List<Core.Model.DeviceGroup>();
        foreach (var deviceGroupEntity in organizationEntity.DeviceGroups)
        {
            deviceGroups.Add(MapToDeviceGroup(organizationId, deviceGroupEntity));
        }

        return new Core.Model.Organization(organizationEntity.OrganizationUuid, organizationEntity.OrganizationName, deviceGroups);
    }

    public async Task<Core.Model.DeviceGroup> RetrieveDeviceGroupByDeviceIdAsync(Guid organizationId, Guid deviceId, CancellationToken cancellationToken)
    {
        var deviceGroupEntity = (await dbContext.Devices
            .AsNoTracking()
            .Include(d => d.DeviceGroup)
            .Where(d => d.DeviceUuid == deviceId)
            .Select(d => d.DeviceGroup)
            .FirstOrDefaultAsync(cancellationToken) 
            ?? throw new DataNotFoundException($"Device with id {deviceId} not found.")) 
            ?? throw new DataNotFoundException($"Device with id {deviceId} does not belong to a device group.");

        return MapToDeviceGroup(organizationId, deviceGroupEntity);
    }

    private static Core.Model.DeviceGroup MapToDeviceGroup(Guid organizationId, DeviceGroup deviceGroupEntity)
    {
        var devices = new List<Core.Model.Device>();
        foreach (var deviceEntity in deviceGroupEntity.Devices)
        {
            devices.Add(MapToDevice(deviceGroupEntity.DeviceGroupUuid, deviceEntity));
        }

        var serverLocationEnum = Core.Model.ServerLocation.West;
        switch (deviceGroupEntity.ServerLocation)
        {
            case "centralus":
                serverLocationEnum = Core.Model.ServerLocation.Central;
                break;
            case "eastus2":
                serverLocationEnum = Core.Model.ServerLocation.East;
                break;
        }

        return new Core.Model.DeviceGroup(
                deviceGroupEntity.DeviceGroupUuid, 
                organizationId, 
                deviceGroupEntity.DeviceGroupName, 
                serverLocationEnum, 
                (Core.Model.ServerStatus)deviceGroupEntity.ServerStatus, 
                deviceGroupEntity.ServerBaseUrl,
                devices);
    }

   private static Core.Model.Device MapToDevice(Guid? deviceGroupId, Device deviceEntity)
    {
        return new Core.Model.Device(
            deviceEntity.DeviceUuid, 
            deviceGroupId, 
            deviceEntity.DeviceName,
            (Core.Model.Os)deviceEntity.Os,
            !string.IsNullOrWhiteSpace(deviceEntity.RestProtocol) ? Enum.Parse<Core.Model.RestProtocol>(deviceEntity.RestProtocol, true) : null,
            (ushort?)deviceEntity.RestPort);
    }
}
