using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace VATViewModel.DTOs
{
    public class IntegrationParam
    {
        public IntegrationParam()
        {
            DefaultBranchCode = "P001";
            DefaultBranchId = 1;
            CurrentUser = "admin";
            TransactionType = "other";
            SetLabel = s => { };
        }

        public string TransactionType { get; set; }
        public string DefaultBranchCode { get; set; }
        public string BranchCode { get; set; }
        public string ToBranchCode { get; set; }
        public int DefaultBranchId { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
        public string RefNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DataTable dtConnectionInfo { get; set; }
        public string Top { get; set; }
        public string Token { get; set; }
        public List<string> IDs { get; set; }
        public List<string> SKUCounts { get; set; }
        public List<string> DuplicateInvoiceIDs { get; set; }
        public string Processed { get; set; }
        public string PrintStatus { get; set; }
        public string PostStatus { get; set; }

        public bool IsEntryDate { get; set; }
        public bool IsMultiple { get; set; }
        public string InvoiceDateTime { get; set; }
        public string DataSourceType { get; set; }
        public Action callBack { get; set; }
        public Action<int> SetSteps { get; set; }
        public Action<string> SetLabel { get; set; }
        public bool WithIsProcessed { get; set; }
        public bool PreviewOnly { get; set; }

        public string MulitipleInvoice { get; set; }
        public string CustomerCode { get; set; }
        public string VehicleID { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }

        public SysDBInfoVMTemp SysDbInfoVmTemp { get; set; }

        public DataTable EngineChassis { get; set; }



        public DataTable Data { get; set; }

        public string InvoiceTime { get; set; }


        public string rmPmFlag { get; set; }

        public string rupshiFlag { get; set; }

        public string SearchField { get; set; }

        public DataTable VendorCode { get; set; }

        
        public bool IsJoinReference = true;

        public bool IsInstitution { get; set; }
        public string IsDuplicateInvoiceSave { get; set; }

        public string BranchId { get; set; }

        public string UserName { get; set; }


        public string GatePassNo { get; set; }
        public string GpDateFrom { get; set; }
        public string GpDateTo { get; set; }
        public string CompanyCode { get; set; }
        public bool IsTrading { get; set; }
        public string CurrentUserId { get; set; }
        public string FinishItemName { get; set; }
        public decimal ExpsenseValue { get; set; }
        public string EffectDate { get; set; }
        public HttpPostedFileBase File { get; set; }


        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsBureau { get; set; }
    }
}