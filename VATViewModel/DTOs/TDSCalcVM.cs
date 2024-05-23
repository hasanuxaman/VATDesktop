using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class TDSCalcVM
    {

        public string VendorId { get; set; }
        public string InvoiceNo { get; set; }
        public string ReceiveDate { get; set; }
        public decimal TotalSubTotal { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal TotalVDSAmount { get; set; }
        public decimal TDSAmount { get; set; }
        public decimal NetBill { get; set; }
        public decimal PreviousSubTotal { get; set; }

        public string Status { get; set; }
        public string Message { get; set; }

    }
}
