﻿using QTComputer.Models;
using System;
using System.Collections.Generic;

namespace QTComputer.ModelViews
{
    public class ProductHomeVM
    {
        public Category category { get; set; }
        public List<Product> lsProducts { get; set; }
    }
}