using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class PurchaseMISViewModel
    {
        public string PurchaseNo { get; set; }
        public string VendorGroup { get; set; }
        public string VendorName { get; set; }
        public string VendorId { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public bool Post { get; set; }
        public bool Duty { get; set; }
        public bool PreviewOnly { get; set; }
        public string ReportType { get; set; }
        public string LC { get; set; }
        public string IsRebate { get; set; }


        public string VATType { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }

        public int FontSize { get; set; }
        public int BranchId { get; set; }
        public string reportName { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }
}
