using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BlogPost
{
    public int BlogPostId { get; set; }

    public string? BlogTitle { get; set; }

    public string? BlogDescription { get; set; }

    public string? BlogContent { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? LastModified { get; set; }

    public int? AuthorId { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? DatePublished { get; set; }

    public virtual BlogAuthor? Author { get; set; }

    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
}
