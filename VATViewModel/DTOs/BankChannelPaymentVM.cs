using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class BankChannelPaymentVM
    {

        public string Id { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal VATAmount { get; set; }
        public string Remarks { get; set; }
        public string PaymentType { get; set; }
        public string Post { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        //MIS report
        public string PurchaseFromDate { get; set; }
        public string PurchaseToDate { get; set; }
        public string IsBankingChannelPay { get; set; }
        public string bankPayRange { get; set; }


    }
}
