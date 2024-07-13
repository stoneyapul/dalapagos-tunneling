﻿namespace Dalapagos.Tunneling.Api.Dto;

using System.ComponentModel.DataAnnotations;
using Core.Model;
using Validation;

/// <summary>
/// Represents a request to add a device.
/// </summary>
public class AddDeviceRequest
{
    /// <summary>
    /// A device id. If not provided, the device id will be generated.
    /// </summary>
    public Guid? DeviceId { get; set; }

    /// <summary>
    /// A device group id. If not provided, the device will not be associated with a device group.
    /// </summary>
    public Guid? DeviceGroupId { get; set; }

    /// <summary>
    /// A device name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64, MinimumLength = 1)]    
    public string Name { get; set; } = default!;

    /// <summary>
    /// The operating system that the device is running. Linux or Windows.
    /// </summary>
    [ValidEnum<Os>]  
    public string Os { get; set; } = default!;
}