namespace Dalapagos.Tunneling.Repository.EF;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

public class EfTunnelingRepository(DalapagosTunnelsDbContext dbContext) : Core.Infrastructure.ITunnelingRepository
{
    public async Task<Core.Model.Device> UpsertDeviceAsync(Guid? deviceId, Guid deviceGroupId, string deviceName, CancellationToken cancellationToken)
    {
        if (!deviceId.HasValue)
        {
            deviceId = Guid.NewGuid();
        }

        var deviceIdParam = new SqlParameter("@DeviceUuid", deviceId);
        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", deviceGroupId);
        var deviceNameParam = new SqlParameter("@DeviceName", deviceName);

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertDevice @DeviceUuid, @DeviceGroupUuid, @DeviceName", 
            deviceIdParam, 
            deviceGroupIdParam, 
            deviceNameParam, 
            cancellationToken);

        return new Core.Model.Device(deviceId.Value, deviceGroupId, deviceName);
    }

    public async Task<Core.Model.DeviceGroup> UpsertDeviceGroupAsync(
        Guid? deviceGroupId, 
        Guid organizationId, 
        string deviceGroupName, 
        string serverName, 
        string serverLocation, 
        Core.Model.ServerStatus serverStatus, 
        Guid? adminGroupId, 
        Guid? userGroupId, 
        CancellationToken cancellationToken)
    {
        if (!deviceGroupId.HasValue)
        {
            deviceGroupId = Guid.NewGuid();
        }

        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", deviceGroupId);
        var organizationIdParam = new SqlParameter("@OrganizationUuid", organizationId);
        var deviceGroupNameParam = new SqlParameter("@DeviceGroupName", deviceGroupName);
        var adminGroupIdParam = new SqlParameter("@EntraAdminGroupId", adminGroupId);
        var userGroupIdParam = new SqlParameter("@EntraUserGroupId", userGroupId);
        var serverNameParam = new SqlParameter("@ServerName", serverName);
        var serverLocationParam = new SqlParameter("@ServerLocation", serverLocation);
        var serverStatusParam = new SqlParameter("@ServerStatus", (int)serverStatus);

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertDeviceGroup @DeviceGroupUuid, @OrganizationUuid, @DeviceGroupName, @EntraAdminGroupId, @EntraUserGroupId, @ServerName, @ServerLocation, @ServerStatus", 
            deviceGroupIdParam, 
            organizationIdParam,
            deviceGroupNameParam, 
            adminGroupIdParam,
            userGroupIdParam,
            serverNameParam,
            serverLocationParam,
            serverStatusParam,
            cancellationToken);

        return new Core.Model.DeviceGroup(deviceGroupId, organizationId, deviceGroupName, serverName, serverLocation, serverStatus, adminGroupId, userGroupId);
    }

    public async Task<Core.Model.Organization> UpsertOrganizationAsync(Guid? organizationId, string organizationName, CancellationToken cancellationToken)
    {
        if (!organizationId.HasValue)
        {
            organizationId = Guid.NewGuid();
        }

        var organizationIdParam = new SqlParameter("@OrganizationUuid", organizationId);
        var organizationNameParam = new SqlParameter("@OrganizationName", organizationName);

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE UpsertOrganization @OrganizationUuid, @OrganizationName", 
            organizationIdParam, 
            organizationNameParam, 
            cancellationToken);

        return new Core.Model.Organization(organizationId, organizationName);
    }

    public async Task DeleteDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var deviceIdParam = new SqlParameter("@DeviceUuid", deviceId);

        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE DeleteDevice @DeviceUuid", 
            deviceIdParam,
            cancellationToken);
    }

    public async Task DeleteDeviceGroupAsync(Guid deviceGroupId, CancellationToken cancellationToken)
    {
        var deviceGroupIdParam = new SqlParameter("@DeviceGroupUuid", deviceGroupId);

         await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE DeleteDeviceGroup @DeviceGroupUuid", 
            deviceGroupIdParam, 
            cancellationToken);
    }

    public async Task DeleteOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organizationIdParam = new SqlParameter("@OrganizationUuid", organizationId);
 
        await dbContext.Database.ExecuteSqlRawAsync(
            "EXECUTE DeleteOrganization @OrganizationUuid", 
            organizationIdParam, 
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

    public async Task<Core.Model.Device> RetrieveDeviceAsync(Guid deviceGroupId, Guid deviceId, CancellationToken cancellationToken)
    {
        var deviceEntity = await dbContext.Devices
            .AsNoTracking()
            .Where(d => d.DeviceUuid == deviceId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new DataNotFoundException($"Device with id {deviceId} not found");

        return MapToDevice(deviceGroupId, deviceEntity);
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

        return new Core.Model.DeviceGroup(
                deviceGroupEntity.DeviceGroupUuid, 
                organizationId, 
                deviceGroupEntity.DeviceGroupName, 
                deviceGroupEntity.ServerName, 
                deviceGroupEntity.ServerLocation, 
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
                deviceEntity.DeviceName);
    }
}
