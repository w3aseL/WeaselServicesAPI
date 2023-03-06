using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Image
{
    public Guid Id { get; set; }

    public string Key { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public virtual ICollection<Education> Educations { get; } = new List<Education>();

    public virtual ICollection<Position> Positions { get; } = new List<Position>();

    public virtual ICollection<ProjectImage> ProjectImages { get; } = new List<ProjectImage>();

    public virtual ICollection<Tool> Tools { get; } = new List<Tool>();
}
