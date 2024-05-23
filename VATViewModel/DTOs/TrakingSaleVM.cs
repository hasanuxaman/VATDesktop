using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{  
    public class TrakingSaleVM
    {

        public string ItemNo { get; set; }
        public string ProductName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string VendorGroup { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public string ExpireDateFrom { get; set; }
        public string ExpireDateTo { get; set; }
 
    }
}
