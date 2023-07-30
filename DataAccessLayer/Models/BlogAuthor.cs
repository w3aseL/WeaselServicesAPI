using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BlogAuthor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    public virtual User User { get; set; } = null!;
}
