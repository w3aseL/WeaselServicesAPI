using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Tool
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? CategoryId { get; set; }

    public Guid? ImageId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<ProjectTool> ProjectTools { get; } = new List<ProjectTool>();
}
