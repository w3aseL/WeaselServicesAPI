using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Position
{
    public Guid Id { get; set; }

    public string JobTitle { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string CompanyUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Guid? ImageId { get; set; }

    public virtual Image? Image { get; set; }
}
