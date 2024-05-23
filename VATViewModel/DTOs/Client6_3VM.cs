using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class Client6_3VM
    {

        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public string VendorID { get; set; }
        public string Address { get; set; }
        public string InvoiceDateTime { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public int BranchId { get; set; }
        public string SignatoryName { get; set; }
        public string SignatoryDesig { get; set; }
        public string TransactionType { get; set; }
        public string PeriodId { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
       


        public List<Client6_3DetailVM> Details { get; set; }


        public string Operation { get; set; }

        public string VendorName { get; set; }
    }



    public class Client6_3DetailVM
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceLineNo { get; set; }
        public string ReceiveNo { get; set; }
        public string InvoiceDateTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal SDRate { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal LineTotalAmount { get; set; }
        public int BranchId { get; set; }
        public string PeriodId { get; set; }
        public string TransactionType { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }


        public string ItemNo { get; set; }
        public string ProductName { get; set; }

        
        public string UOM { get; set; }
    }
}
