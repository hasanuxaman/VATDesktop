using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
   public interface IReport
    {

       DataSet TransferIssueNew(string TransferIssueNo, string IssueDateFrom, string IssueDateTo, string itemNo,
                               string categoryID, string productType, string TransactionType, string Post, string DBName = null, SysDBInfoVMTemp connVM = null);
       DataTable MIS19(string StartDate, string EndDate, SysDBInfoVMTemp connVM = null);

       DataSet VAT6_10Report(string TotalAmount, string StartDate
            , string EndDate, string post1, string post2, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet VDS12KhaNew(string VendorId, string DepositNumber, string DepositDateFrom, string DepositDateTo,
                                   string IssueDateFrom, string IssueDateTo, string BillDateFrom, string BillDateTo,
                                   string PurchaseNumber, bool chkPurchaseVDS, bool chkAll, SysDBInfoVMTemp connVM = null);
       DataSet VAT6_3(string SalesInvoiceNo, string Post1, string Post2, string ddmmyy = "n"
           , SysDBInfoVMTemp connVM = null, bool mulitplePreview = false, string getTopValue = "", string pdfFlag = "N", int FromRow = 0, int ToRow = 99999, string transactionType = "");

       DataSet MegnaVAT6_3(string SalesInvoiceId, string Post1, string Post2, string ddmmyy = "n"
        , SysDBInfoVMTemp connVM = null, bool mulitplePreview = false, string getTopValue = "", string pdfFlag = "N", int FromRow = 0, int ToRow = 99999, string transactionType = "");


       DataSet PurchaseReturn(string PurchaseInvoiceNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null, int FromRow = 0, int ToRow = 99999, string transactionType = "");

       DataSet VAT11ReportCommercialImporterNew(string SalesInvoiceNo, string Post1, string Post2, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       

       DataSet SaleTrackingReport(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null);

       DataSet VAT6_7(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet VAT6_8(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet CreditNoteNew(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet CreditNoteAmountNew(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet DebitNoteNew(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet VAT18New(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet RptDeliveryReport(string challanNo, SysDBInfoVMTemp connVM = null);

       DataSet VAT6_3Toll(string TollNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null);

       DataSet BatchTracking(string BatchNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet BatchTracking1(string BatchNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet BatchTracking2(string BatchNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet VAT16NewforTollRegister(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, SysDBInfoVMTemp connVM = null);

       DataSet VAT6_1Toll(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2,
           string ReportName, int BranchId = 0, SysDBInfoVMTemp connVM = null, bool IsOpening = true,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, VAT6_1ParamVM vm = null);

       #region Comments Sep-09-2020
       
       ////DataSet VAT6_1_WithConn(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       #endregion

       DataSet VAT6_1_Branching(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, string DBName = "", string BranchName = "", SysDBInfoVMTemp connVM = null);

       DataSet VAT6_1(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, string DBName = "", string BranchName = "", SysDBInfoVMTemp connVM = null);

       DataSet VAT6_2_1(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet VAT6_2Toll(string ItemNo, string StartDate, string EndDate, string post1, string post2, int BranchId = 0,
           SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, VAT6_2ParamVM vm = null, bool openingValue = true);

       DataSet SD_Data(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet VAT19NewNewformat(string PeriodName, string ExportInBDT, SysDBInfoVMTemp connVM = null);


       #region Comments Jul-12-2020
       
       //////DataSet VAT9_1(string PeriodName, int BranchId = 0, string Date = "", SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormAPart3(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormAPart4(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormBPart3(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormCPart3(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormCPart4(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormGPart8(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormDPart5(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormEPart6(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////DataTable VAT9_1_SubFormFPart6(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       //////string[] VAT9_1_Process(string PeriodName, int BranchId = 0, string ExportInBDT = "Y", SysDBInfoVMTemp connVM = null);

       //////DataSet VAT9_1_Report(string PeriodName, int BranchId = 0, string ExportInBDT = "Y", SysDBInfoVMTemp connVM = null);


       //////DataSet VAT9_1_V2Save(string PeriodName, int BranchId = 0, string Date = "", SysDBInfoVMTemp connVM = null);

       #endregion



       DataSet VAT18Breakdown(string PeriodName, string ExportInBDT, SysDBInfoVMTemp connVM = null);

       DataSet RepFormKaTradingNew(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       //DataSet VAT6_2_1(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

     

       DataSet VAT24(string ddbackno, string ddbFinishItemNo, string SalesInvoiceNo, SysDBInfoVMTemp connVM = null);

       DataSet VAT22(string ddbackno, SysDBInfoVMTemp connVM = null);

       DataSet VATDDB(string ddbackno, string salesInvoice, SysDBInfoVMTemp connVM = null);

       DataSet PurchaseMis(string PurchaseId, int BranchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null);

       DataSet SaleMis(string SaleId, string ShiftId = "0", int BranchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null, string OrderBy = "");

       DataSet IssueMis(string IssueId, int BranchId, SysDBInfoVMTemp connVM = null);

       DataSet ReceiveMis(string ReceiveId, string ShiftId = "0", int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet SaleReceiveMIS(string StartDate, string EndDate, string ShiftId = "0", string Post = null, SysDBInfoVMTemp connVM = null,string Toll="N");

       DataSet VAT1KaNew(string FinishItemNo, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null);

       DataSet VAT1KhaNew(string FinishItemNo, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null);

       DataSet VAT1GaNew(string FinishItemNo, string EffectDate, string VATName, string IsPercent, SysDBInfoVMTemp connVM = null);

       DataSet VAT1GhaNew(string finishitemno, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null);

       DataSet FormKaNew(string FinishItemNo, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null);

       DataSet BOMNew_withFNo(string FinishItemNo, string EffectDate, string VATName, string IsPercent, SysDBInfoVMTemp connVM = null);

       DataSet BOMNew(string BOMId, string VATName, string IsPercent, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet BOMDownload(SysDBInfoVMTemp connVM = null);

       DataSet BankNew(string BankID, SysDBInfoVMTemp connVM = null);

       DataSet CustomerGroupNew(string CustomerGroupID, SysDBInfoVMTemp connVM = null);

       DataSet CustomerNew(string CustomerID, string CustomerGroupID, SysDBInfoVMTemp connVM = null);

       DataSet DepositNew(string DepositNo,
                                 string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, SysDBInfoVMTemp connVM = null);

       DataSet VATDisposeDsNew(string DisposeNumber, SysDBInfoVMTemp connVM = null);

       DataSet MISVAT16New(string CategoryId, string StartDate, string EndDate, string UserName, string ItemNo, SysDBInfoVMTemp connVM = null);

       DataSet MISVAT17New(string CategoryId, string UserName, string StartDate, string EndDate, string ItemNo, SysDBInfoVMTemp connVM = null);

       DataSet MISVAT18New(string UserName, string StartDate, string EndDate, SysDBInfoVMTemp connVM = null);

       DataSet ProductCategoryNew(string cgID, SysDBInfoVMTemp connVM = null);

       DataSet ProductNew(string ItemNo, string CategoryID, string IsRaw, SysDBInfoVMTemp connVM = null, string ProductCode = "");

       DataSet PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                             string VendorId, string ItemNo, string CategoryID, string ProductType,
                             string TransactionType, string Post,
                             string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn,
                             decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string VatType = "", string IsRebate = null, SysDBInfoVMTemp connVM = null);

       DataSet IssueNew(string IssueNo, string IssueDateFrom, string IssueDateTo, string itemNo,
                                string categoryID, string productType, string TransactionType, string Post, string waste, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet ReceiveNew(string ReceiveNo, string ReceiveDateFrom, string ReceiveDateTo, string itemNo,
                                  string categoryID, string productType, string transactionType, string post, string ShiftId = "0", int BranchId = 0, SysDBInfoVMTemp connVM = null);


       DataSet SaleNew(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid,
                            string ItemNo, string CategoryID, string productType, string TransactionType, string Post,
                            string onlyDiscount, bool bPromotional, string CustomerGroupID, bool pCategoryLike = false, string PGroup = ""
            , string ShiftId = "0", int branchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null, string OrderBy = "", string DataSource = "", string Toll = "N", string Type = "",  string ReportType = "");

       DataSet SaleNewWithChassisEngine(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid,
                               string ItemNo, string CategoryID, string productType, string TransactionType, string Post,
                               string onlyDiscount, bool bPromotional, string CustomerGroupID, string chassis, string engine, string ShiftId = "0",
           int branchId = 0, SysDBInfoVMTemp connVM = null, string OrderBy = "");



       DataSet StockNew(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate,
                                string Post1, string Post2, bool WithoutZero = false, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, SysDBInfoVMTemp connVM = null);



       DataSet StockWithAdjNew(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate,
                               string Post1, string Post2, bool WithoutZero = false, int BranchId = 0, SysDBInfoVMTemp connVM = null);


       DataSet StockWastage(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate,
                             string Post1, string Post2, bool WithoutZero = false, int BranchId = 0, SysDBInfoVMTemp connVM = null);


       DataSet VehicleNew(string VehicleNo, SysDBInfoVMTemp connVM = null);

       DataSet Adjustment(string HeadId, string AdjType, string StartDate, string EndDate, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet VendorGroupNew(string VendorGroupID, SysDBInfoVMTemp connVM = null);

       DataSet InputOutputCoEfficient(string RawItemNo, string StartDate, string EndDate, string Post1, string Post2, SysDBInfoVMTemp connVM = null);

       DataSet VendorReportNew(string VendorID, string VendorGroupID, SysDBInfoVMTemp connVM = null);

       DataSet TrasurryDepositeNew(string DepositId, SysDBInfoVMTemp connVM = null);

       DataSet TDSDeposit(string DepositId, SysDBInfoVMTemp connVM = null);

       DataSet TDSDepositDetail(string DepositId, SysDBInfoVMTemp connVM = null);

       DataSet TDSDepositDetail_MISReport(string DepositNo,
                               string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                               string transactionType, SysDBInfoVMTemp connVM = null);

       DataSet TDSDeposit_MISReport(string DepositNo,
                                 string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, SysDBInfoVMTemp connVM = null);


       DataSet VDSDepositNew(string DepositId, SysDBInfoVMTemp connVM = null);

       DataSet SDTrasurryDepositeNew(string DepositId, SysDBInfoVMTemp connVM = null);

       DataSet ComapnyProfileString(string CompanyID, string UserId, SysDBInfoVMTemp connVM = null);

       DataSet ComapnyProfile(string CompanyID, SysDBInfoVMTemp connVM = null);

       DataSet CurrencyReportNew(SysDBInfoVMTemp connVM = null);

       DataSet CostingNew(string ID, string ItemNo, string UOM, string UOMn, decimal UOMc,
                                  decimal totalQty, decimal rCostPrice, SysDBInfoVMTemp connVM = null);

       DataSet ComapnyProfileSecurity(string CompanyID, SysDBInfoVMTemp connVM = null);

       DataTable MonthlyPurchases(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
          string VendorId, string ItemNo, string CategoryID, string ProductType, string TransactionType, string Post,
          string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn, decimal UOMc, decimal TotalQty, decimal rCostPrice, int BranchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null);


       DataSet VDSReport(string DepositNo,
                                 string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, string VendorId, SysDBInfoVMTemp connVM = null);

       DataSet DemandReport(string DemandNo, string DemandDateFrom, string DemandDateTo, string TransactionType, string Post, SysDBInfoVMTemp connVM = null);

       DataSet BanderolForm_4(string BanderolID, string post1, string StartDate, string EndDate, string post2, SysDBInfoVMTemp connVM = null);

       DataSet BanderolForm_5(string PeriodName, SysDBInfoVMTemp connVM = null);

       string GetReturnType(string itemNo, string transactionType, SysDBInfoVMTemp connVM = null);

       DataSet SelectMultipleInvoices(int noOfChallan, string transactionType, string challanDateFrom, string challanDateTo, SysDBInfoVMTemp connVM = null);

       DataSet RptBanderolProduct(string ProductCode, SysDBInfoVMTemp connVM = null);

       DataSet BureauVAT11Report(string SalesInvoiceNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null);

       DataSet BureauVAT6_1Report(string ItemNo, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet BureauVAT18Report(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet BureauVAT18_OldFormat(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataTable BureauMonthlySales(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                                     string Customerid, string ItemNo, string CategoryID, string productType,
                                     string TransactionType, string Post, string onlyDiscount, bool bPromotional,
                                     string CustomerGroupID, string ShiftId = "1", int branchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet BureauSaleMis(string SaleId, string ShiftId = "0", int branchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet BureauSaleNew(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid,
                              string ItemNo, string CategoryID, string productType, string TransactionType, string Post,
                              string onlyDiscount, bool bPromotional, string CustomerGroupID, string ShiftId = "1", int branchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet BureauCreditNote(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet BureauVAT19Report(string PeriodName, string ExportInBDT, SysDBInfoVMTemp connVM = null);

       DataSet RptVAT7Report(string vat7No, SysDBInfoVMTemp connVM = null);

       DataSet TollRegister(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet TollRegisterRaw(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet VAT16AttachToll(string ItemNo, string UserName, string StartDate, string EndDate, string post1,
                               string post2, SysDBInfoVMTemp connVM = null);

       DataSet PurchaseReturnNew(string PurchaseInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet Current_AC_VAT18(string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet SerialStockStatus(string ItemNo, string CategoryID, string ProductType, string StartDate, string ToDate, string post1, string post2, SysDBInfoVMTemp connVM = null);


       DataSet PurchaseWithLCInfo(string PurchaseInvoiceNo, string LCDateFrom, string LCDateTo,
                                 string VendorId, string ItemNo, string VendorGroupId, string LCNo, string Post
                               , SysDBInfoVMTemp connVM = null);



       DataSet VAT18_Sanofi(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null);

       DataSet TDSReport(SysDBInfoVMTemp connVM = null);

       DataSet LocalPurchaseReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet ImportDataReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet ReceiedVsSaleReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,string Toll="N");

       DataSet SalesStatementForServiceReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,string Toll="N");

       DataSet SalesStatementDeliveryReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,string Toll="N");

       DataSet StockReportFGReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,string FiscalYear=null, string UserId = "");

       DataSet StockReportRMReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "");


       DataSet TransferToDepotReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet VDSStatementReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet Chak_kaReport(string StartDate, string EndDate, int BranchId = 0, int TransferTo = 0, SysDBInfoVMTemp connVM = null);

       DataSet Chak_khaReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataSet TDS_Certificatet(string DepositId, SysDBInfoVMTemp connVM = null);

       DataSet TDSAmountReport(string[] conditionFields = null, string[] conditionValues = null, SysDBInfoVMTemp connVM = null);

       DataSet TransferIssueOutReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0, int TransferTo = 0, SysDBInfoVMTemp connVM = null, string ShiftId = "0");

       DataSet TransferReceiveInReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0, int BranchFromId = 0, SysDBInfoVMTemp connVM = null);

       DataSet Wastage(string ItemNo, string ProdutCategoryId, string ProductType, string post1, string post2, string StartDate, string EndDate, int branchId = 0, SysDBInfoVMTemp connVM = null);



    }
}
