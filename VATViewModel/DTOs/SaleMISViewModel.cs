using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class SaleMISViewModel
    {
        public string InvoiceNo { get; set; }
        public string CustomerGroup { get; set; }
        public string CustomerName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string Type { get; set; }
        public string VatType { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool Post { get; set; }
        public bool Discount { get; set; }
        public bool PSale { get; set; }
        public bool PreviewOnly { get; set; }
        public string ReportType { get; set; }
        public int FontSize { get; set; }

        public string OrderBy { get; set; }
        public int BranchId { get; set; }

        public string reportName { get; set; }
        public string CustomerId { get; set; }


    }
}
