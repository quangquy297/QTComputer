using System;
using System.Collections.Generic;
using QTComputer.Models;

namespace QTComputer.ModelViews
{
    public class HomeVM
    {
        public List<News> TinTuc { get; set; }
        public List<ProductHomeVM> Products { get; set; }
    }
}