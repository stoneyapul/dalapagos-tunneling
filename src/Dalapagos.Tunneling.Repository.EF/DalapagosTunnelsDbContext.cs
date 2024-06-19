using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Dalapagos.Tunneling.Repository.EF;

public partial class DalapagosTunnelsDbContext : DbContext
{
    public DalapagosTunnelsDbContext()
    {
    }

    public DalapagosTunnelsDbContext(DbContextOptions<DalapagosTunnelsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceGroup> DeviceGroups { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("Device");

            entity.HasIndex(e => e.DeviceUuid, "Index_Device_Uuid").IsUnique();

            entity.Property(e => e.DeviceName).HasMaxLength(64);
            entity.Property(e => e.DeviceUuid).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.DeviceGroup).WithMany(p => p.Devices)
                .HasForeignKey(d => d.DeviceGroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Device_DeviceGroupId");
        });

        modelBuilder.Entity<DeviceGroup>(entity =>
        {
            entity.ToTable("DeviceGroup");

            entity.HasIndex(e => e.DeviceGroupUuid, "Index_DeviceGroup_Uuid").IsUnique();

            entity.Property(e => e.DeviceGroupName).HasMaxLength(64);
            entity.Property(e => e.DeviceGroupUuid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ServerLocation).HasMaxLength(50);

            entity.HasOne(d => d.Organization).WithMany(p => p.DeviceGroups)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_DeviceGroup_OrgId");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("Organization");

            entity.HasIndex(e => e.OrganizationUuid, "Index_Organization_Uuid").IsUnique();

            entity.Property(e => e.OrganizationName).HasMaxLength(64);
            entity.Property(e => e.OrganizationUuid).HasDefaultValueSql("(newid())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
