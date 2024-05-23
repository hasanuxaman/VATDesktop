using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class BillInvoiceMasterVM
    {
 
        public string BillNo { get; set; }
        public int BranchId { get; set; }
        public string BillDate { get; set; }
        public string CustomerID { get; set; }
        public string PONo { get; set; }
        public string ChallanNo { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string PeriodID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public List<BillInvoiceDetailVM> Details { get; set; }
        public string Operation { get; set; }
        public string Id { get; set; }

        public string BillDateTimeFrom { get; set; }
        public string BillDateTimeTo { get; set; }


        //master properties

    }

    public class BillInvoiceDetailVM
    {
        public int BillId { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public int BranchId { get; set; }
        public decimal Quantity { get; set; }
        public decimal VATRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VATAmount { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string PeriodID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Id { get; set; }
        public string ItemNo { get; set; }
        public decimal NBRPrice { get; set; }



  
    }
}
