using System;
using System.Collections.Generic;

namespace QTComputer.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? CatName { get; set; }

    public string? Description { get; set; }

    public bool Published { get; set; }

    public string? Thumb { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
