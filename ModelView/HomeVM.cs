using System;
using System.Collections.Generic;
using QTComputer.Models;

namespace QTComputer.ModelViews
{
    public class HomeVM
    {
        public List<TinDang> TinTucs { get; set; }
        public List<ProductHomeVM> Products { get; set; }
        public QuangCao quangcao { get; set; }
    }
}