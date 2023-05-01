using System;
using System.Collections.Generic;

namespace QTComputer.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ShortDesc { get; set; }

    public string? Description { get; set; }

    public int? CatId { get; set; }

    public int? Price { get; set; }

    public string? Thumb { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateModified { get; set; }

    public string? Title { get; set; }

    public int? UnitsInStock { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();
}
