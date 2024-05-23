using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class TransferReceiveVM
    {
        public string TransferReceiveNo { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionType { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string ReferenceNo { get; set; }
        public int TransferFrom { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Id { get; set; }

        public string ProductType { get; set; }
        public List<TransferReceiveDetailVM> Details { get; set; }

        public string ReceiveDateFrom { get; set; }
        public string BranchName { get; set; }
        public string ReceiveDateTo { get; set; }
        public string TransferFromNo { get; set; }
        public string TransferNo { get; set; }

        public string Operation { get; set; }

        public decimal TotalVATAmount { get; set; }
        public decimal TotalSDAmount { get; set; }
        public int BranchId { get; set; }


        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }

        public string SearchField { get; set; }
        public string SearchValue { get; set; }

        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }

    }

    public class TransferReceiveDetailVM
    {
        public string TransferReceiveNo { get; set; }
        public string ReceiveLineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public int TransferFrom { get; set; }
        public decimal SubTotal { get; set; }
        public string Comments { get; set; }
        public string TransactionType { get; set; }
        public string TransactionDateTime { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public string TransferFromNo { get; set; }
        public string ProductCode { get; set; }
        public string ItemName { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SDRate { get; set; }
        public decimal SDAmount { get; set; }
        public string Weight { get; set; }
        public int BranchId { get; set; }


        

    }
}
