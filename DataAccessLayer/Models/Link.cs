using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Link
{
    public int Id { get; set; }

    public string LinkName { get; set; } = null!;

    public string LinkUrl { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? LogoAlt { get; set; }
}
