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
    public interface IPurchaseMPL
    {

        DataTable SearchPurchaseDutyDTNew(string PurchaseInvoiceNo, SysDBInfoVMTemp connVM = null, string ItemNo = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);
        
        DataTable SearchPurchaseInvoiceTracking(string purchaseInvoiceNo, string itemNo, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseInvoiceMPLHeadersVM likeVM = null
            , bool Dt = false, SysDBInfoVMTemp connVM = null, bool VDSSearch = false, bool IsDisposeRawSearch = false, string ItemNo = null, bool IsBankingChannelPay = false, bool IsClints6_3Search = false);

        DataTable SelectPurchaseDetail(string PurchaseInvoiceNo, string[] conditionFields = null
            , string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , bool Dt = false, SysDBInfoVMTemp connVM = null);

        string[] PurchaseInsert(PurchaseInvoiceMPLHeadersVM Master, List<PurchaseInvoiceMPLDetailVM> Details, List<PurchaseDutiesVM> Duties
            , List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, int branchId = 0
            , SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] PurchaseUpdate(PurchaseInvoiceMPLHeadersVM Master, List<PurchaseInvoiceMPLDetailVM> Details, List<PurchaseDutiesVM> Duties
            , List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] PurchasePost(PurchaseInvoiceMPLHeadersVM Master, List<PurchaseInvoiceMPLDetailVM> Details, List<PurchaseDutiesVM> Duties
            , List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null
            , SysDBInfoVMTemp connVM = null);

        decimal ReturnQty(string purchaseReturnId, string itemNo, SysDBInfoVMTemp connVM = null);

        string[] UpdateTDSAmount(string PurchaseInvoiceNo, decimal TDSAmount, SysDBInfoVMTemp connVM = null);



        string[] ImportData(DataTable dtPurchaseM, DataTable dtPurchaseD, DataTable dtPurchaseI, DataTable dtPurchaseT, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "");

        List<PurchaseInvoiceMPLHeadersVM> DropDown(SysDBInfoVMTemp connVM = null);
        string FindItemIdFromPurchase(string PurchaseInvoiceNo, string ItemNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        decimal FormatingNumeric(decimal value, int DecPlace, SysDBInfoVMTemp connVM = null);

        DataTable GetExcelData(List<string> invoiceList,bool withDuty, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        int GetUnProcessedCount(SysDBInfoVMTemp connVM = null);
        string[] ImportBigData(DataTable PurchaseData, SysDBInfoVMTemp connVM = null);
        string[] ImportExcelFile(PurchaseInvoiceMPLHeadersVM paramVM, SysDBInfoVMTemp connVM = null);
        string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null);
        DataTable PurchaseSearch(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseInvoiceMPLHeadersVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);
        string[] PurchaseUpdateDuty(DataTable DtDuty, SysDBInfoVMTemp connVM = null);
        string RateChangePercent(string ItemNo, decimal unitPrice, SysDBInfoVMTemp connVM = null);
        List<PurchaseInvoiceMPLDetailVM> RateCheck(List<PurchaseInvoiceMPLDetailVM> VMs, SysDBInfoVMTemp connVM = null);
        string[] SaveTempPurchase(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "", bool IsExcel = false);
        DataTable SearchProductbyPurchaseInvoice(string purchaseInvoiceNo, SysDBInfoVMTemp connVM = null);
        DataTable SearchPurchaseDutyDTDownload(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, SysDBInfoVMTemp connVM = null);
        DataTable SearchPurchaseHeaderDTNew2(string PurchaseInvoiceNo, string WithVDS, string VendorName, string VendorGroupID, string VendorGroupName, string InvoiceDateFrom, string InvoiceDateTo, string SerialNo, string T1Type, string T2Type, string BENumber, string Post, SysDBInfoVMTemp connVM = null);

        List<PurchaseInvoiceMPLHeadersVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseInvoiceMPLHeadersVM likeVM = null, SysDBInfoVMTemp connVM = null, string ItemNo = null, bool IsDisposeRawSearch = false);
        List<PurchaseInvoiceMPLDetailVM> SelectPurchaseDetailList(string PurchaseInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
    
    }
}
