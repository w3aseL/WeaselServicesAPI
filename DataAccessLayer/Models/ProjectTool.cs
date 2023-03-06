using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ProjectTool
{
    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public int ToolId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Tool Tool { get; set; } = null!;
}
