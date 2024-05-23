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
    public interface ISale
    {
        string[] ImportData(DataTable dtSaleM, DataTable dtSaleD, DataTable dtSaleE, bool CommercialImporter = false, int branchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "");
        
        string GetCategoryName(string itemNo, SysDBInfoVMTemp connVM = null);

        string[] SalesInsert(SaleMasterVM MasterVM, List<SaleDetailVm> DetailVMs, List<SaleExportVM> ExportDetails,
            List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, int branchId = 0,
            SysDBInfoVMTemp connVM = null, string UserId = "", bool IsManualEntry = true, List<SaleDeliveryTrakingVM> DeliveryTrakings=null,bool CodeGenaration = true);

        string[] SalesUpdate(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails,
            List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, string UserId = "", bool IsManualEntry = true, List<SaleDeliveryTrakingVM> DeliveryTrakings = null);

        string[] SalesPost(SaleMasterVM Master, List<SaleDetailVm> Details, List<TrackingVM> Trackings,
            SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null, bool BulkPost = false, string UserId = "");

        string[] CurrencyInfo(string salesInvoiceNo, SysDBInfoVMTemp connVM = null);

        DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string ImportIDExcel = "",
            SysDBInfoVMTemp connVM = null);

        DataTable SearchSaleDetailDTNew(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null);

        decimal ReturnSaleQty(string saleReturnId, string itemNo, SysDBInfoVMTemp connVM = null);

        DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName, SysDBInfoVMTemp connVM = null);

         DataTable SearchSaleDetailTemp(string SalesInvoiceNo, string userId, SysDBInfoVMTemp connVM = null);

        DataTable SelectAllHeaderTemp(int Id = 0, string[] conditionFields = null,
             string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
             SaleMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);
        void SetDeliveryChallanNo(string saleInvoiceNo, string challanDate, SysDBInfoVMTemp connVM = null);

        void SetGatePass(string saleInvoiceNo, SysDBInfoVMTemp connVM = null);

        List<SaleMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null,
            SysDBInfoVMTemp connVM = null, string transactionType = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null,
            bool Dt = false, string TransectionType = null, string MultipleSearch = "N", SysDBInfoVMTemp connVM = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null, string IsVDSCompleted = null);

        DataTable GetSaleExcelData(List<string> invoiceList, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null,string UserId=null);


        string[] PostSales(DataTable table, SysDBInfoVMTemp connVM = null);
        string[] UpdatePrintNew(string InvoiceNo, int PrintCopy, SysDBInfoVMTemp connVM = null);

        string[] SaveAndProcess(DataTable data, Action callBack = null, int branchId = 1, string app = "",
            SysDBInfoVMTemp connVM = null, IntegrationParam paramVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string UserId = "", bool IsExcelUpload = false);

        DataTable CheckInvoiceNoExist(List<string> ids, SysDBInfoVMTemp connVM = null);

        DataTable GetById(string id, SysDBInfoVMTemp connVM = null);

        DataTable GetInvoiceNoFromTemp(SysDBInfoVMTemp connVM = null);

        DataTable GetMasterData(SqlConnection vConnection = null, SqlTransaction vTransaction = null, string app = "", SysDBInfoVMTemp connVM = null, string userId = null, bool orderBy = false, string token = null);

        DataTable GetOldDbList(SysDBInfoVMTemp connVM = null);

        DataTable GetSaleJoin(string invoiceNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable GetUnProcessedTempData(SysDBInfoVMTemp connVM = null);

        DataTable SaleTempNoBranch(SysDBInfoVMTemp connVM = null);

        int GetUnProcessedCount(SysDBInfoVMTemp connVM = null);

        int GetUnProcessedCount_tempTable(string tempTable, SysDBInfoVMTemp connVM = null);

        string[] GetFisrtInvoiceId(SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null);

        string[] GetTopUnProcessed(SysDBInfoVMTemp connVM = null);

        string[] ImportBigData(DataTable salesData, bool deleteFlag = true, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null, bool IsDiscount = false, string userId = null, string token = null, IntegrationParam paramVM = null, bool IsExcelUpload = false);

        string[] ImportSalesBigData(DataTable salesData, bool deleteFlag = true, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, SysDBInfoVMTemp connVM = null);

        string[] ProccessTempData(SysDBInfoVMTemp connVM = null);

        string[] SaveAndProcessOtherDb(DataTable data, Action callBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null);

        string[] SaveAndProcessTempData(DataTable data, Action callBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null);

        string[] SaveInvoiceIdSaleTemp(int firstId, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null);

        string[] SaveSalesToKohinoor(DataTable table, DataTable db, SysDBInfoVMTemp connVM = null);

        string[] SaveSalesToLink3(DataTable table, SysDBInfoVMTemp connVM = null);

        string[] SaveTempTest(DataTable table, SysDBInfoVMTemp connVM = null);

        string[] UpdateIsProcessed(int flag, string Id, SysDBInfoVMTemp connVM = null);

    }
}
