using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Project
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Url { get; set; }

    public string? RepoUrl { get; set; }

    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();

    public virtual ICollection<ProjectTool> ProjectTools { get; set; } = new List<ProjectTool>();
}
