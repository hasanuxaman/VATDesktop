using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class MPLTradeChallanVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }

        public int SalesInvoiceRefId { get; set; }
        public int PreviousSalesInvoiceRefId { get; set; }

        public string AgainstSupplyOrderNo { get; set; }
        public string Consignee { get; set; }
        public string ContractOrATNo { get; set; }
        public string AgreementDate { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public string TransactionDateTime { get; set; }
        public string InvoiceDateTime { get; set; }
        public string DeliveryDate { get; set; }
        public List<MPLTradeChallanDetilsVM> MPLTradeChallanDetilsVMs { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
        public string FormNumeric { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        public string Post { get; set; }
        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }

    }
   public class MPLTradeChallanDetilsVM
   {
       public string ItemNo { get; set; }
       public string ProductName { get; set; }
       public string ProductCode { get; set; }
       public int BranchId { get; set; }

       public int Id { get; set; }

       public decimal Quantity { get; set; }
       public string VehicleNo { get; set; }
       public string VehicleType { get; set; }
       public string SalesInvoiceNo { get; set; }
       public string CustomerName { get; set; }
       public string CustomerCode { get; set; }
       public string InvoiceDateTime { get; set; }
       public string AgainstSupplyOrderNo { get; set; }
       public string ContractOrATNo { get; set; }

   }
}
