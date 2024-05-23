using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
    //public class IssueMasterVM
    //{
    //    public string IssueNo { get; set; }
    //    public string IssueDateTime { get; set; }
    //    public decimal TotalVATAmount { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public string SerialNo { get; set; }
    //    public string Comments { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public string ReceiveNo { get; set; }
    //    public string transactionType { get; set; }
    //    public string Post { get; set; }
    //    public string ImportId { get; set; }
    //    public string ReturnId { get; set; }
    //    public string ShiftId { get; set; }

    //}

    //public class IssueDetailVM
    //{
        
    //    public string IssueNoD { get; set; }
    //    public string IssueLineNo { get; set; }
    //    public string ItemNo { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal NBRPrice { get; set; }
    //    public decimal CostPrice { get; set; }
    //    public string UOM { get; set; }
    //    public decimal VATRate { get; set; }
    //    public decimal VATAmount { get; set; }
    //    public decimal SubTotal { get; set; }
    //    public string CommentsD { get; set; }
    //    public string ReceiveNoD { get; set; }
    //    public string IssueDateTimeD { get; set; }
    //    public decimal SD { get; set; }
    //    public decimal SDAmount { get; set; }
    //    public decimal Wastage { get; set; }
    //    public string BOMDate { get; set; }
    //    public string FinishItemNo { get; set; }
    //    public decimal UOMQty { get; set; }
    //    public string UOMn { get; set; }
    //    public decimal UOMc { get; set; }
    //    public decimal UOMPrice { get; set; }
    //    public string BomId { get; set; }
    //}

    public class IssueMasterVM
    {
        public IssueMasterVM()
        {
            IDs = new List<string>(); 
        }
         

        public string Id { get; set; }
        public string IssueNo { get; set; }
        public string IssueDateTime { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string ReceiveNo { get; set; }
        public string transactionType { get; set; }
        public string Post { get; set; }
        public string ImportId { get; set; }
        public string ReturnId { get; set; }
        public string Operation { get; set; }
        public string IssueDateTimeFrom { get; set; }
        public string IssueDateTimeTo { get; set; }
        public List<IssueDetailVM> Details { get; set; }
        public string TargetId { get; set; }

        public string ProductType { get; set; }

        public int ProductCategoryId { get; set; }
        public HttpPostedFileBase File { get; set; }

        public string ShiftId { get; set; }
        public int BranchId { get; set; }

        public string SaleInvoiceNumber { get; set; }

        public string FiscalYear { get; set; }
        public int IssueNumber { get; set; }

        public string AppVersion { get; set; }

        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }



        public bool IssueOnly { get; set; }


        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }

    }

    public class IssueDetailVM
    {
        public string Id { get; set; }
        public string IssueNoD { get; set; }
        public string IssueNo { get; set; }
        public string IssueLineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal NBRPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string CommentsD { get; set; }
        public string ReceiveNoD { get; set; }
        public string IssueDateTimeD { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public decimal Wastage { get; set; }
        public decimal WQty { get; set; }
        public string BOMDate { get; set; }
        public string FinishItemNo { get; set; }
        public string FinishItemName { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public int BOMId { get; set; }
        public string Operation { get; set; }
        public string ItemName { get; set; }
        public string ProductCode { get; set; }
        public string Post { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedOn { get; set; }

        public string transactionType { get; set; }

        public string ReturnId { get; set; }

        public string ReceiveNo { get; set; }

        public string IssueDateTime { get; set; }

        //public string TransactionType { get; set; }

        public string IsProcess { get; set; }

        public string IssueReturnId { get; set; }

        public decimal DiscountedNBRPrice { get; set; }

        public decimal UOMWastage { get; set; }

        public decimal DiscountAmount { get; set; }
        public int BranchId { get; set; }

        public string VATName { get; set; }
        public int WIPBOMId { get; set; }

    }


    public class AVGPriceVm
    {
        public string AvgDateTime { get; set; }
        public string ItemNo { get; set; }

        public bool FullProcess { get; set; }
        public string ProductName { get; set; }
        public string flag { get; set; }
        public string SPSQLText { get; set; }
        public bool FromSP { get; set; }

    }
}
