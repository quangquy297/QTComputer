﻿using System;
using System.Collections.Generic;

namespace QTComputer.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
