using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Education
{
    public Guid Id { get; set; }

    public string SchoolName { get; set; } = null!;

    public string SchoolType { get; set; } = null!;

    public string SchoolUrl { get; set; } = null!;

    public string RewardType { get; set; } = null!;

    public string? Major { get; set; }

    public DateTime GraduationDate { get; set; }

    public decimal Gpa { get; set; }

    public Guid? ImageId { get; set; }

    public virtual Image? Image { get; set; }
}
