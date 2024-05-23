using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class AdjustmentMISViewModel
    {
        public string AdjType { get; set; }
        public string Name { get; set; }
        public bool Post { get; set; }
        public string AdjDateFrom { get; set; }
        public string AdjDateTo { get; set; }
        public int FontSize { get; set; }
        public int BranchId { get; set; }
    }
}
