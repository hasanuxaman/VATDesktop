using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class ReportParamVM
    {
        public ReportParamVM()
        {
            PreviewOnly = true;

        }

        public string InvoiceNo { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string DepositId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PrintCopy { get; set; }
        public int AlreadyPrintCopy { get; set; }
        public bool PreviewOnly { get; set; }
        public string TransactionType { get; set; }
        public int BranchId{get; set;}
        public int FontSize { get; set; }
        public string TollNo { get; set; }
        public string Id { get; set; }


       


    }

    public class StockMovementVM
    { 
        public string ItemNo { get; set; }


        public string StartDate { get; set; }

        public int BranchId { get; set; }

        public string ToDate { get; set; }

        public bool CategoryLike { get; set; }

        public string FormNumeric { get; set; }

        public string ProductGroupName { get; set; }

        public string CategoryId { get; set; }

        public string Post1 { get; set; }

        public string Post2 { get; set; }

        public string ProductType { get; set; }

        public string CurrentUserID { get; set; }

        public string chkTrading { get; set; }
        public bool Branchwise { get; set; }

        public string CurrentUserName { get; set; }
        public string ZoneID { get; set; }
        public bool WithOutZero { get; set; }

        public bool ExcelRptDetail { get; set; }
        public string OrderBy { get; set; }

    }
    public class VAT6_2ParamVM
    {
        public string ItemNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Post1 { get; set; }
        public string Post2 { get; set; }
        public int BranchId { get; set; }
        public bool Opening { get; set; }
        public bool Opening6_2 { get; set; }
        public string ProdutType { get; set; }
        public string ProdutCategoryId { get; set; }
        public bool VAT6_2_1 { get; set; }
        public bool StockMovement { get; set; }
        public string Flag { get; set; }
        public string UOMTo { get; set; }
        public bool rbtnService { get; set; }
        public bool rbtnWIP { get; set; }
        public bool IsBureau { get; set; }
        public bool AutoAdjustment { get; set; }
        public bool PreviewOnly { get; set; }
        public string InEnglish { get; set; }
        public decimal UOMConversion { get; set; }
        public string UOM { get; set; }
        public bool IsMonthly { get; set; }
        public string ProdutGroupName { get; set; }
        public bool ProdutCategoryLike { get; set; }
        public bool IsTopSheet { get; set; }
        public string UserId { get; set; }
        public string ReportType { get; set; }
        public bool Permanent { get; set; }
        public bool PermanentProcess { get; set; }
        public bool SkipOpening { get; set; }
        public bool MainProcess { get; set; }
        public string DecimalPlace { get; set; }
        public string QuentitylPlace { get; set; }
        public string ValuePlace { get; set; }
        public bool BranchWise { get; set; }
        public string FromPeriodName { get; set; }
        public string ToPeriodName { get; set; }
        public string ProductName { get; set; }
        public bool IsChecked { get; set; }
        public string PeriodMonth { get; set; }
        public bool FilterProcessItems { get; set; }
        public string FontSize { get; set; }
        public string ExportInBDT { get; set; }
        public string IsExport { get; set; }
        public string PDesc { get; set; }
        public bool IsBranch { get; set; }

        public string PeriodId { get; set; }
        public bool FromSP { get; set; }
        public string SPSQLText { get; set; }
    }
    public class VAT6_2SPParamVM
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Post1 { get; set; }
        public string Post2 { get; set; }
        public string IsExport { get; set; }
        public int BranchId { get; set; }
        public string UserID { get; set; }
        public string PeriodId { get; set; }
    }


    public class VAT6_1ParamVM
    {
        public string ItemNo { get; set; }
        public string UserName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Post1 { get; set; }
        public string Post2 { get; set; }
        public string ReportName { get; set; }
        public int BranchId { get; set; }
        public bool Opening { get; set; }
        public bool OpeningFromProduct { get; set; }
        public string ProdutType { get; set; }
        public string ProdutCategoryId { get; set; }
        public bool VAT6_2_1 { get; set; }
        public bool StockMovement { get; set; }
        public bool IsMonthly { get; set; }
        public bool PreviewOnly { get; set; }
        public string InEnglish { get; set; }
        public string DecimalPlace { get; set; }
        public string DecimalPlaceValue { get; set; }

        //public decimal UomConversion { get; set; }
        public string UOM { get; set; }
        public string UOMTo { get; set; }
        public string Flag { get; set; }
        public string ProdutGroupName { get; set; }
        public bool ProdutCategoryLike { get; set; }
        public decimal UOMConversion { get; set; }

        public bool IsTopSheet { get; set; }

        public bool IsGroupTopSheet { get; set; }

        public string UserId { get; set; }
        public string ReportType { get; set; }
        public bool Is6_1Permanent { get; set; }

        public string ZoneID { get; set; }

        public bool PermanentProcess { get; set; }

        public int PeriodId { get; set; }

        public bool SkipOpening { get; set; }

        public bool BranchWise { get; set; }
        public string FromPeriodName { get; set; }
        public string ToPeriodName { get; set; }
        public string ProductName { get; set; }
        public bool IsChecked { get; set; }
        public string PeriodMonth { get; set; }
        public int FontSize { get; set; }
        public string ProcesType { get; set; }



        public bool FilterProcessItems { get; set; }
        public bool StockProcess { get; set; }
        public bool IsBranch { get; set; }
        public bool FromSP { get; set; }
        public string  SPSQLText { get; set; }
    }


}
