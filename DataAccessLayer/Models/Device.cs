using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Device
{
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceName { get; set; }

    public string? Manufacturer { get; set; }

    public string? DeviceIpaddress { get; set; }
}
