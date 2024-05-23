using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
    public class ReceiveMasterVM
    {
        public string Id { get; set; }
        public string IssueFromBOM { get; set; }

        public bool TrackingTrace { get; set; }


        public string ReceiveNo { get; set; }
        public string ReceiveDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string transactionType { get; set; }
        public string Post { get; set; }
        public string FromBOM ="Y";
        public string ReturnId { get; set; }
        public string ImportId { get; set; }
        public string ReferenceNo { get; set; }
        public string WithToll { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }

        public List<ReceiveDetailVM> Details { get; set; }
        public string Operation { get; set; }
        public string IssueDateTimeFrom { get; set; }
        public string IssueDateTimeTo { get; set; }

        public string ShiftId { get; set; }

        public string SearchField { get; set; }
        public string SearchValue { get; set; }

        public string ProductType { get; set; }

        public int ProductCategoryId { get; set; }

        public string VatName { get; set; }
        public HttpPostedFileBase File { get; set; }

        public int BranchId { get; set; }
        //public string ReceiveReturnId { get; set; }

        public string ReceiveNumber { get; set; }

        public string FiscalYear { get; set; }

        public string AppVersion { get; set; }

        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }

        public string MultipleTransactionType { get; set; }

        public bool ReceiveOnly { get; set; }

        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }


        public string CurrentUserID { get; set; }
        public string FormNumeric { get; set; }
        public List<TrackingVM> Trackings { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }

    }

    public class ReceiveDetailVM
    {
        public string BOMReferenceNo { get; set; }

        public string ReceiveNo { get; set; }
        public string ReceiveLineNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string CommentsD { get; set; }
        public string VatName { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        ////////public int BOMId { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public string ReturnTransactionType { get; set; }
        public string Post { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedOn { get; set; }

        public string ReceiveDateTime { get; set; }

        public string transactionType { get; set; }

        public string ReturnId { get; set; }

        public string Id { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal DiscountedNBRPrice { get; set; }

        public decimal DollerValue { get; set; }

        public decimal CurrencyValue { get; set; }

        public int BranchId { get; set; }
        public int BOMId { get; set; }

        public string Weight { get; set; }

    }
    
    //public class ReceiveMasterVM
    //{
    //  public string ReceiveNo { get; set; }
    //    public string ReceiveDateTime { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public decimal TotalVATAmount { get; set; }
    //    public string SerialNo { get; set; }
    //    public string Comments { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public string transactionType { get; set; }
    //    public string Post { get; set; }
    //    public string FromBOM { get; set; }
    //    public string ReturnId { get; set; }
    //    public string ImportId { get; set; }
    //    public string ReferenceNo { get; set; }
    //    public string WithToll { get; set; }
    //    public string ShiftId { get; set; }
        


    //    public string CustomerID { get; set; }
    //}

    //public class ReceiveDetailVM
    //{
       
    //    public string ReceiveLineNo { get; set; }
    //    public string ItemNo { get; set; }
    //    public string ItemName { get; set; }
    //    public string ItemCode { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal CostPrice { get; set; }
    //    public decimal NBRPrice { get; set; }
    //    public string UOM { get; set; }
    //    public decimal VATRate { get; set; }
    //    public decimal VATAmount { get; set; }
    //    public decimal SubTotal { get; set; }
    //    public string CommentsD { get; set; }
    //    public string VatName { get; set; }
    //    public decimal SD { get; set; }
    //    public decimal SDAmount { get; set; }
    //    public string BOMId { get; set; }
    //    public decimal UOMQty { get; set; }
    //    public string UOMn { get; set; }
    //    public decimal UOMc { get; set; }
    //    public decimal UOMPrice { get; set; }
    //    public string ReturnTransactionType { get; set; }
        
        

    //}
}
