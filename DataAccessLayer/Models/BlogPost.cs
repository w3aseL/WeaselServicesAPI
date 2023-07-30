using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BlogPost
{
    public int BlogId { get; set; }

    public string? BlogTitle { get; set; }

    public string? BlogDescription { get; set; }

    public string? BlogContent { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? LastModified { get; set; }

    public int? CategoryId { get; set; }

    public int? AuthorId { get; set; }

    public virtual BlogAuthor? Author { get; set; }

    public virtual BlogCategory? Category { get; set; }
}
