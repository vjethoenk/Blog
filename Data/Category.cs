using System;
using System.Collections.Generic;

namespace Blog.Data;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
