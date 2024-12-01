using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BlogPostCategory
{
    public int BlogPostId { get; set; }

    public int BlogCategoryId { get; set; }
}
