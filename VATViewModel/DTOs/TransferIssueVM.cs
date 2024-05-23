using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
    public class TransferIssueVM
    {
        public string Id { get; set; }
        public string FiscalYear { get; set; }
        public string TransferIssueNo { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionType { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string ReferenceNo { get; set; }
        public int TransferTo { get; set; }
        public string TransferBranchTo { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string IssueDateFrom { get; set; }
        public string IssueDateTo { get; set; }
        public string ImportExcelId { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalSDAmount { get; set; }
        public string IsTransfer { get; set; }
        public string SignatoryName{ get; set; }
        public string SignatoryDesig { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string BranchCode { get; set; }
        public List<TransferIssueDetailVM> Details { get; set; }
        public string Operation { get; set; }
        public string ProductType { get; set; }
        public string ProductCategoryId { get; set; }
        public string TripNo { get; set; }
        public string FormNumeric { get; set; }

        public int BranchId { get; set; }
        public int ShiftId { get; set; }
        public string UserId { get; set; }
        public string VehicleID { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public bool vehicleSaveInDB { get; set; }
        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }
        public string ImportIDExcel { get; set; }
        public string BranchFromRef { get; set; }
        public string BranchToRef { get; set; }
        public string VatName { get; set; }

        public string SearchField { get; set; }
        public string SearchValue { get; set; }

        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }

    }

    public class TransferIssueDetailVM
    {
        public int BOMId { get; set; }

        public string TransferIssueNo { get; set; }
        public string ItemName { get; set; }
        public string IssueLineNo { get; set; }
        public string ItemNo { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal SubTotal { get; set; }
        public string Comments { get; set; }
        public string TransactionType { get; set; }
        public string TransactionDateTime { get; set; }
        public int TransferTo { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SDRate { get; set; }
        public decimal SDAmount { get; set; }
        public string Weight { get; set; }
        public int BranchId { get; set; }


        public string BranchToRef { get; set; }

        public string BranchFromRef { get; set; }
        public string OtherRef { get; set; }



    }
}
