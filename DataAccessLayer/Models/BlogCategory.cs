using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BlogCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string TagColor { get; set; } = null!;

    public string? TagIcon { get; set; }

    public bool IsHidden { get; set; }

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
