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
    public async Task<Core.Model.Device> UpsertDeviceAsync(Guid? deviceId, Guid? deviceGroupId, string deviceName, Core.Model.Os os, CancellationToken cancellationToken)
    {
        if (!deviceId.HasValue)
        {
            deviceId = Guid.NewGuid();
        }

        var deviceIdParam = new SqlParameter("@DeviceUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceId };
        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceGroupId ?? (object)DBNull.Value, IsNullable = true };
        var deviceNameParam = new SqlParameter("@DeviceName", System.Data.SqlDbType.NVarChar, 64) { Value = deviceName };
        var osParam = new SqlParameter("@Os", System.Data.SqlDbType.Int) { Value = (int)os };
        var parms = new List<object> { deviceIdParam, deviceGroupIdParam, deviceNameParam, osParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertDevice @DeviceUuid, @DeviceGroupUuid, @DeviceName, @Os", 
            parms, 
            cancellationToken);

        return new Core.Model.Device(deviceId.Value, deviceGroupId, deviceName, os);
    }

    public async Task<Core.Model.DeviceGroup> UpsertDeviceGroupAsync(
        Guid? deviceGroupId, 
        Guid organizationId, 
        string deviceGroupName, 
        Core.Model.ServerLocation serverLocation, 
        Core.Model.ServerStatus serverStatus, 
        Guid? adminGroupId, 
        Guid? userGroupId, 
        CancellationToken cancellationToken)
    {
        if (!deviceGroupId.HasValue)
        {
            deviceGroupId = Guid.NewGuid();
        }

        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = deviceGroupId };
        var organizationIdParam = new SqlParameter("@OrganizationUuid", System.Data.SqlDbType.UniqueIdentifier) { Value = organizationId };
        var deviceGroupNameParam = new SqlParameter("@DeviceGroupName", System.Data.SqlDbType.NVarChar, 64) { Value = deviceGroupName };
        var adminGroupIdParam = new SqlParameter("@EntraAdminGroupId", System.Data.SqlDbType.UniqueIdentifier) { Value = adminGroupId ?? (object)DBNull.Value, IsNullable = true };
        var userGroupIdParam = new SqlParameter("@EntraUserGroupId", System.Data.SqlDbType.UniqueIdentifier) { Value = userGroupId ?? (object)DBNull.Value, IsNullable = true };
        var serverLocationParam = new SqlParameter("@ServerLocation", System.Data.SqlDbType.NVarChar, 50) { Value = serverLocation.ToAzureLocation() };
        var serverStatusParam = new SqlParameter("@ServerStatus", System.Data.SqlDbType.Int) { Value = (int)serverStatus };
        var parms = new List<object> { deviceGroupIdParam, organizationIdParam, deviceGroupNameParam, adminGroupIdParam, userGroupIdParam, serverLocationParam, serverStatusParam };

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertDeviceGroup @DeviceGroupUuid, @OrganizationUuid, @DeviceGroupName, @EntraAdminGroupId, @EntraUserGroupId, @ServerLocation, @ServerStatus", 
            parms,
            cancellationToken);

        return new Core.Model.DeviceGroup(deviceGroupId, organizationId, deviceGroupName, serverLocation, serverStatus, adminGroupId, userGroupId);
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
                deviceGroupEntity.EntraAdminGroupId, 
                deviceGroupEntity.EntraUserGroupId,
                devices);
    }

   private static Core.Model.Device MapToDevice(Guid deviceGroupId, Device deviceEntity)
    {
        return new Core.Model.Device(
                deviceEntity.DeviceUuid, 
                deviceGroupId, 
                deviceEntity.DeviceName,
                (Core.Model.Os)deviceEntity.Os);
    }
}
