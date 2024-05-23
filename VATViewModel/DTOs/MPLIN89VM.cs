using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using VATViewModel.Integration.JsonModels;

namespace VATViewModel.DTOs
{

    public class MPLIN89VM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }
        public string TransactionDateTime { get; set; }
        public string TransactionType { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string SignatoryDesig { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal DIP { get; set; }
        public decimal Temperature { get; set; }
        public decimal SP_Gravity { get; set; }
        public decimal IssueNaturalQuantity { get; set; }
        public decimal Issueat30Quantity { get; set; }
        public decimal ReceiveNaturalQuantity { get; set; }
        public decimal Receiveat30Quantity { get; set; }
        public decimal GainNaturalQuantity { get; set; }
        public decimal Gainat30Quantity { get; set; }
        public bool IsLoss { get; set; }


        public decimal Quantity { get; set; }
        public decimal RequestedVolumn { get; set; }

        public decimal QtyAt30Temperature { get; set; }
        public int TransferIssueMasterRefId { get; set; }
        public int TransferIssueDetailsRefId { get; set; }
        public string TransferReceiveNo { get; set; }
        public string TransferIssueNo { get; set; }

        public string ReceiveDateTime { get; set; }
        public string ItemNo { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string WagonNo { get; set; }
        public string TransferFrom { get; set; }


        public List<string> IDs { get; set; }
        public string Operation { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }

        public List<MPLIN89DetailsVM> MPLIN89DetailsVMs { get; set; }
        public List<MPLIN89IssueDetailsVM> MPLIN89IssueDetailsVMs { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }
    }

    public class MPLIN89DetailsVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int MPLIN89HeaderId { get; set; }
        public string TransactionDateTime { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public int LineNumber { get; set; }
        public int TransferMPLReceiveDetailId { get; set; }
        public int TransferMPLReceiveId { get; set; }
        public string ItemNo { get; set; }
        public decimal ReceiveNaturalQuantity { get; set; }

        //public int TransferIssueDetailsRefId { get; set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string TransferReceiveNo { get; set; }
        public string TransferFrom { get; set; }
        //public decimal IssueNaturalQuantity { get; set; }
        //public decimal Issueat30Quantity { get; set; }

        
        public decimal RequestedVolumn { get; set; }
        public decimal Quantity { get; set; }
        public decimal UOMQty { get; set; }
        public decimal Temperature { get; set; }
        public decimal SP_Gravity { get; set; }
        public decimal QtyAt30Temperature { get; set; }
        public decimal DIP { get; set; }
        public string TransferIssueNo { get; set; }

        //public int TransferIssueMasterRefId { get; set; }
        public string ReceiveDateTime { get; set; }
        public string WagonNo { get; set; }
        //public int TransferMPLIssueId { get; set; }
        
    }

    public class MPLIN89IssueDetailsVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int MPLIN89HeaderId { get; set; }

        public string TransactionDateTime { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string WagonNo { get; set; }
        public string ItemNo { get; set; }

        public int LineNumber { get; set; }
        public int TransferIssueMasterRefId { get; set; }
        public int TransferIssueDetailsRefId { get; set; }
        public decimal ReceiveNaturalQuantity { get; set; }
        //public int TransferMPLReceiveDetailId { get; set; }
        //public int TransferMPLReceiveId { get; set; }

        public decimal Temperature { get; set; }
        public decimal SP_Gravity { get; set; }
        public decimal DIP { get; set; }

        public decimal QtyAt30Temperature { get; set; }
        public decimal IssueNaturalQuantity { get; set; }
        public decimal Issueat30Quantity { get; set; }
        public decimal Quantity { get; set; }
        //public decimal RequestedVolumn { get; set; }
        public decimal UOMQty { get; set; }

        public string ReceiveDateTime { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string TransferIssueNo { get; set; }
        //public string TransferReceiveNo { get; set; }
        public string TransferFrom { get; set; }


    }

}
