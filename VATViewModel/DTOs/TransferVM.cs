using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class TransferVM
    {
        public string TransferNo { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionType { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string ReferenceNo { get; set; }
        public int TransferFrom { get; set; }
        //////public int TransferTo { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string BranchName { get; set; }
        public string TransferFromName { get; set; }
        public string VehicleNo { get; set; }


        public string DateTimeFrom { get; set; }

        public string DateTimeTo { get; set; }

        public string TransferFromNo { get; set; }

        public decimal TotalVATAmount { get; set; }
        public decimal TotalSDAmount { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public int BranchId { get; set; }
        public List<string> IDs { get; set; }
        public string ReceiveDate { get; set; }

    }

    public class TransferDetailVM
    {
        public string TransferNo { get; set; }
        public string TransferLineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal SubTotal { get; set; }
        public string Comments { get; set; }
        public string TransactionType { get; set; }
        public string TransactionDateTime { get; set; }
        public string Post { get; set; }
        public int TransferFrom { get; set; }
        //////public int TransferTo { get; set; }

        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }


        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SDRate { get; set; }
        public decimal SDAmount { get; set; }
        public string ProductCode { get; set; }
        public string ItemName { get; set; }
        public string TransferFromNo { get; set; }
        public int BranchId { get; set; }

    }



  

}
