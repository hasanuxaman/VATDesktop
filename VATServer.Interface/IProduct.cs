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
    public interface IProduct
    {

        DataTable SearchBanderolProducts(SysDBInfoVMTemp connVM = null);

        DataTable SearchProductMiniDS(string ItemNo, string CategoryID, string IsRaw, string CategoryName,
           string ActiveStatus, string Trading, string NonStock, string ProductCode, SysDBInfoVMTemp connVM = null, List<string> IsRawList = null);

        DataTable SearchOverheadForBOMNew(string ActiveStatus, SysDBInfoVMTemp connVM = null);

        string GetTransactionType(string itemNo, string effectDate, SysDBInfoVMTemp connVM = null);

        DataTable GetLIFOPurchaseInformation(string itemNo, string receiveDate, string PurchaseInvoiceNo = "", SysDBInfoVMTemp connVM = null);

        decimal GetLastNBRPriceFromBOM_VatName(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        List<ProductVM> SelectAll(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null, DataTable dtPCodes = null);

        DataTable GetBOMReferenceNo(string itemNo, string VatName, string effectDate, SqlConnection currConn,
            SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        DataTable AvgPriceNew(string itemNo, string tranDate, SqlConnection VcurrConn,
           SqlTransaction Vtransaction, bool isPost, bool Vat16 = true, bool Vat17 = true
            , bool transfer = true, SysDBInfoVMTemp connVM = null, string UserId = "", bool stockProceaa = false, string ReportType="");


        DataTable ProductDTByItemNo(string ItemNo, string ProductName = "", SqlConnection VcurrConn = null
           , SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectBOMRaw(string itemNo, string effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        decimal GetLastVatableFromBOM(string itemNo, SqlConnection currConn, SqlTransaction transaction,
       SysDBInfoVMTemp connVM = null);

        decimal GetLastTollChargeFBOMOH(string HeadName, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        decimal AvgPriceTender(string ItemNo, string StartDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable AvgPriceVAT16(string ItemNo, string StartDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        DataTable AvgPriceVAT17(string ItemNo, string StartDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        string[] Delete(ProductVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] DeleteProductNames(string itemNo, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] DeleteToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        List<ProductVM> DropDown(int CategoryID = 0, string IsRaw = "", SysDBInfoVMTemp connVM = null);
        List<ProductVM> DropDownAll(SysDBInfoVMTemp connVM = null);
        List<ProductVM> DropDownByCategory(string catId, SysDBInfoVMTemp connVM = null);
        DataTable GetBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);
        string GetBomIdFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelProductDetails(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelProductStock(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string GetExistingProductName(string ProductName, SysDBInfoVMTemp connVM = null);
        decimal GetLastNBRPriceFromBOM(string itemNo, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);
        decimal GetLastUseQuantityFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        List<ProductVM> GetProductByType(string type, SysDBInfoVMTemp connVM = null);
        string GetProductCodeUOMn(string ProductCode, string ProductGroup, SysDBInfoVMTemp connVM = null);
        string GetProductIdByName(string ProductName, SysDBInfoVMTemp connVM = null);

        string GetProductNoByGroup(string ProductName, string ProductGroup, SysDBInfoVMTemp connVM = null);
        string GetProductNoByGroup_Code(string ProductCode, string ProductGroup, SysDBInfoVMTemp connVM = null);
        string GetProductUOMc(string uomFrom, string uomTo, SysDBInfoVMTemp connVM = null);
        string GetProductUOMn(string ProductName, string ProductGroup, SysDBInfoVMTemp connVM = null);
        ProductVM GetProductWithCostPrice(string productCode, string purchaseNo, string effectDate, SysDBInfoVMTemp connVM = null);
        DataSet GetReconsciliationData(string fromDate, string toDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] InserToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "");
        string[] InsertToProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType, bool AutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool IsNestle = false);
        string[] InsertToProductNames(ProductNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable SearchByItemNo(string ItemNo, SysDBInfoVMTemp connVM = null);
        DataTable SearchChassis(SysDBInfoVMTemp connVM = null);
        DataTable SearchEngine(SysDBInfoVMTemp connVM = null);
        DataTable SearchProductbyMultipleSaleInvoice(string SaleInvoiceNos, SysDBInfoVMTemp connVM = null);

        DataTable SearchProductDT(string ItemNo, string ProductName, string CategoryID, string CategoryName, string UOM, string IsRaw, string SerialNo, string HSCodeNo, string ActiveStatus, string Trading, string NonStock, string ProductCode, string databaseName, string customerId = "0", string isvds = "", int BranchId = 0, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null);

        DataTable SelectProductDTAll(string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, int BranchId = 0
            , string databaseName = "", string isvds = "", List<UserBranchDetailVM> userBranchs = null
            , SysDBInfoVMTemp connVM = null, string IsOverhead = null);

        DataTable SearchProductFinder(string ProductName, string ProductCode, string IsRaw, string CustomerId, SysDBInfoVMTemp connVM = null);
        DataTable SearchProductMiniDS_WithProductvm(ProductVM Productvm, SysDBInfoVMTemp connVM = null);
        DataTable SearchProductMiniDSDispose(string purchaseNumber, SysDBInfoVMTemp connVM = null);
        DataTable SearchProductNames(string ItemNo, string Id, string Names, SysDBInfoVMTemp connVM = null);
        DataTable SearchProductStock(string ItemNo, string id, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null);
        List<ProductVM> SelectAllOverhead(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        List<ProductNameVM> SelectProductName(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null);
        string TrackingStockCheck(string ItemNo, SysDBInfoVMTemp connVM = null);
        string[] UpdateProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType, SysDBInfoVMTemp connVM = null);
        string[] UpdateToProductNames(ProductNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);



    }
}
