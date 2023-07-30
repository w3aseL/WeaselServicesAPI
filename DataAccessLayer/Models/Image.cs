using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Image
{
    public Guid Id { get; set; }

    public string Key { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();

    public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
}
