using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class PurchaseTDSsVM
    {
        public int Id { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public string TDSCode { get; set; }
        public decimal PurchaseBillAmount { get; set; }
        public decimal TDSAmount { get; set; }
        public string Post { get; set; }
        public string PaymentDate { get; set; }
        public string DepositId { get; set; }

    }
}
