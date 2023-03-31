﻿using System;
using System.Collections.Generic;

namespace QTComputer.Models;

public partial class TransactStatus
{
    public int TransactStatusId { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
