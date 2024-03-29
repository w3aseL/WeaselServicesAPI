﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ProjectImage
{
    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid ImageId { get; set; }

    public bool IsLogo { get; set; }

    public virtual Image Image { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
