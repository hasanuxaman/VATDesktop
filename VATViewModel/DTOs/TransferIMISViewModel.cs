using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class TransferMISViewModel
    {
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string Type { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool Post { get; set; }
        public string ReportType { get; set; }
        public string OrderBy { get; set; }

        public int FromBranch { get; set; }
        public int ToBranch { get; set; }
    }
}
