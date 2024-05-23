using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace VATViewModel.DTOs
{
    public class TransferMPLReceiveVM
    {
        public int Id { get; set; }
        public string TransferReceiveNo { get; set; }
        public string TransferIssueNo { get; set; }
        public int BranchId { get; set; }
        public int TransferFrom { get; set; }
        public int TransferIssueMasterRefId { get; set; }
        public string TransferTRCode { get; set; }
        public string ReceiveTRCode { get; set; }
        public string TransferDateTime { get; set; }
        public string ReceiveDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionType { get; set; }
        public string Comments { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string Post { get; set; }
        public string SerialNo { get; set; }
        public string ReferenceNo { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalSDAmount { get; set; }
        public string BranchFromRef { get; set; }
        public string BranchToRef { get; set; }
        public string IsTransfer { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string SignatoryDesig { get; set; }
        public string LastModifiedOn { get; set; }
        public string RailwayReceiptNo { get; set; }
        public string RailwayReceiptDate { get; set; }
        public string RailwayInvoiceNo { get; set; }

        public decimal WeightChargeed { get; set; }
        public decimal FreightToPay { get; set; }
        public decimal FreightPrepaid { get; set; }

        public string DIP { get; set; }
        public string ArrivalDate { get; set; }
        public string BatchNo { get; set; }
        public string BatchDate { get; set; }
        public string TestReportNo { get; set; }
        public string TestReportDate { get; set; }
        public decimal TestReportTempPF { get; set; }
        public decimal TestReportSPGR { get; set; }
        public string DepartureDate { get; set; }

        public List<string> IDs { get; set; }

        public string Operation { get; set; }
        public string TransferFromBranch { get; set; }
        public string TransferToBranch { get; set; }
        public int TankId { get; set; }
        public string ItemNo { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }

        public List<TransferMPLReceiveDetailVM> TransferMPLReceiveDetailVMs { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }
        public string ReportType { get; set; }
        public string TransferType { get; set; }

    }


    public class TransferMPLReceiveDetailVM
    {
        public int Id { get; set; }
        public int TransferMPLReceiveId { get; set; }
        public int BranchId { get; set; }
        public string ReceiveLineNo { get; set; }
        public string TransferFrom { get; set; }
        public string TransferDateTime { get; set; }
        public string ReceiveDateTime { get; set; }
        public string Post { get; set; }
        public string ItemNo { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal RequestedVolumn { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal SubTotal { get; set; }
        public string LCF { get; set; }
        public string QU { get; set; }
        public string IsExcise { get; set; }
        public string IsCustoms { get; set; }
        public string Comments { get; set; }
        public string TransactionType { get; set; }
        public decimal UOMQty { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SDRate { get; set; }
        public decimal SDAmount { get; set; }
        public int PeriodID { get; set; }
        public int TankId { get; set; }
        public decimal Temperature { get; set; }
        public decimal SP_Gravity { get; set; }
        public decimal QtyAt30Temperature { get; set; }
        public decimal AlReadyReceivedQuantity { get; set; }
        public string IsReceiveCompleted { get; set; }
        public bool IsReceiveComplet { get; set; }



        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string TankCode { get; set; }
        public string ReportType { get; set; }
        public int TransferIssueMasterRefId { get; set; }
        public int TransferIssueDetailsRefId { get; set; }
        public string WagonNo { get; set; }
        public string UsedStatus { get; set; }



    }

}
