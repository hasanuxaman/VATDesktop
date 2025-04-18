﻿using System;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class WastageMISViewModel
    {
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string ProductType { get; set; }
        public string ProductGroup { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool Post { get; set; }
        public bool WithOutZero { get; set; }
        public bool QuantityOnly { get; set; }
        public bool Summary { get; set; }
        public int FontSize { get; set; }

    }
}
