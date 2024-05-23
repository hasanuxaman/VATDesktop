using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class CoEfficientMISViewModel
    {
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string GroupName { get; set; }
        public bool Post { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int FontSize { get; set; }
    }
}
