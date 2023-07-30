using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Resume
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Key { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
