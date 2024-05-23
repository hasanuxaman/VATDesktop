using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo.ProductWCF;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class ProductRepo : IProduct
    {

        //ProductDAL _dal = new ProductDAL();

        ProductWCFClient wcf = new ProductWCFClient();


        public DataTable SearchBanderolProducts(SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string dt = wcf.SearchBanderolProducts(connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(dt);
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataTable SearchProductMiniDS(string ItemNo, string CategoryID, string IsRaw, string CategoryName,
            string ActiveStatus, string Trading, string NonStock, string ProductCode, SysDBInfoVMTemp connVM = null, List<string> IsRawList = null)
        {
           
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);



                OrdinaryVATDesktop _ord = new OrdinaryVATDesktop();
                string result = wcf.SearchProductMiniDS(ItemNo, CategoryID, IsRaw, CategoryName, ActiveStatus, Trading, NonStock, ProductCode, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public DataTable SearchOverheadForBOMNew(string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchOverheadForBOMNew(ActiveStatus, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


        public string GetTransactionType(string itemNo, string effectDate, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string retResult = wcf.GetTransactionType(itemNo, effectDate, connVMwcf);

                return retResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public DataTable GetLIFOPurchaseInformation(string itemNo, string receiveDate, string PurchaseInvoiceNo = "", SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.GetLIFOPurchaseInformation(itemNo, receiveDate, PurchaseInvoiceNo, connVMwcf);
                
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetLastNBRPriceFromBOM_VatName(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Result = wcf.GetLastNBRPriceFromBOM_VatName(itemNo, VatName, effectDate, CustomerID, connVMwcf);

               decimal retResult = Convert.ToDecimal(Result);
               return retResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public List<ProductVM> SelectAll(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null, DataTable dtPCodes = null)
        {

            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(ItemNo, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);
                List<ProductVM> results = JsonConvert.DeserializeObject<List<ProductVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public DataTable GetBOMReferenceNo(string itemNo, string VatName, string effectDate, SqlConnection currConn,
            SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetBOMReferenceNo(itemNo, VatName, effectDate, CustomerID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable AvgPriceNew(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction
            , bool isPost, bool Vat16 = true, bool Vat17 = true, bool transfer = true, SysDBInfoVMTemp connVM = null
            , string UserId = "", bool stockProcess = false, string ReportType="")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.AvgPriceNew(itemNo,  tranDate, isPost,  Vat16 , Vat17 , transfer, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable ProductDTByItemNo(string ItemNo, string ProductName = "", SqlConnection VcurrConn = null
            , SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ProductDTByItemNo(ItemNo, ProductName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectBOMRaw(string itemNo, string effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectBOMRaw(itemNo, effectDate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal GetLastVatableFromBOM(string itemNo, SqlConnection currConn, SqlTransaction transaction, 
            SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Result = wcf.GetLastVatableFromBOM(itemNo, connVMwcf);

                decimal retResult = Convert.ToDecimal(Result);
                return retResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public decimal GetLastTollChargeFBOMOH(string HeadName, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Result = wcf.GetLastTollChargeFBOMOH( HeadName,  VatName,  effectDate, connVMwcf);

                decimal retResult = Convert.ToDecimal(Result);
                return retResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal AvgPriceTender(string ItemNo, string StartDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.AvgPriceTender(ItemNo, StartDate, connVMwcf);

                decimal results = Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public decimal GetLastNBRPriceFromBOM(string itemNo, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetLastNBRPriceFromBOM(itemNo, effectDate, CustomerID,connVMwcf);

                decimal results = Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public decimal GetLastUseQuantityFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetLastUseQuantityFromBOM(FinishItemNo, RawItemNo, VatName,effectDate, connVMwcf);

                decimal results = Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public DataTable AvgPriceVAT16(string ItemNo, string StartDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.AvgPriceVAT16(ItemNo, StartDate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable AvgPriceVAT17(string ItemNo, string StartDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.AvgPriceVAT17(ItemNo, StartDate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetBOM(itemNo, VatName,effectDate,CustomerID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExcelData(idswcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelProductDetails(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExcelProductDetails(idswcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelProductStock(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            DataTable results = new DataTable();
            try
            {
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                //string result =   wcf.GetExcelProductStock(idswcf, connVMwcf);

                //DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
     
        public DataTable SearchByItemNo(string ItemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchByItemNo(ItemNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchChassis(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchChassis(connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchEngine(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchEngine(connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductbyMultipleSaleInvoice(string SaleInvoiceNos, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductbyMultipleSaleInvoice(SaleInvoiceNos,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductDT(string ItemNo, string ProductName, string CategoryID, string CategoryName, string UOM, string IsRaw, string SerialNo, string HSCodeNo, string ActiveStatus, string Trading, string NonStock, string ProductCode, string databaseName, string customerId = "0", string isvds = "", int BranchId = 0, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductDT(ItemNo, ProductName,  CategoryID,  CategoryName,  UOM,  IsRaw,  SerialNo,  HSCodeNo,  ActiveStatus,  Trading,  NonStock,  ProductCode,  databaseName,  customerId , isvds , BranchId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectProductDTAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, int BranchId = 0, string databaseName = "", string isvds = "", List<UserBranchDetailVM> userBranchs = null, SysDBInfoVMTemp connVM = null, string IsOverhead=null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string userBranchswcf = JsonConvert.SerializeObject(userBranchs);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectProductDTAll(conditionFieldswcf, conditionValueswcf, Dt, BranchId, databaseName, isvds, userBranchswcf,connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public DataTable SearchProductFinder(string ProductName, string ProductCode, string IsRaw, string CustomerId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductFinder(ProductName, ProductCode, IsRaw, CustomerId,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductMiniDS_WithProductvm(ProductVM Productvm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Productvmwcf = JsonConvert.SerializeObject(Productvm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductMiniDS_WithProductvm(Productvmwcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductMiniDSDispose(string purchaseNumber, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductMiniDSDispose(purchaseNumber, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductNames(string ItemNo, string Id, string Names, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductNames(ItemNo,Id,Names, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductStock(string ItemNo, string id, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductStock(ItemNo, id, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataSet GetReconsciliationData(string fromDate, string toDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetReconsciliationData(fromDate, toDate, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] Delete(ProductVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.Delete(vmwcf, idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DeleteProductNames(string itemNo, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeleteProductNames(itemNo, Id, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DeleteToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string psVMwcf = JsonConvert.SerializeObject(psVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeleteToProductStock(psVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InserToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string psVMwcf = JsonConvert.SerializeObject(psVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InserToProductStock(psVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType, bool AutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool IsNestle = false)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToProduct(vmwcf,Trackingswcf,ItemType,AutoSave, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToProductNames(ProductNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToProductNames(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateProduct(vmwcf,Trackingswcf,ItemType, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToProductNames(ProductNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateToProductNames(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string psVMwcf = JsonConvert.SerializeObject(psVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateToProductStock(psVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ProductVM> DropDown(int CategoryID = 0, string IsRaw = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(CategoryID, IsRaw,connVMwcf);

                List<ProductVM> results = JsonConvert.DeserializeObject<List<ProductVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownAll( connVMwcf);

                List<ProductVM> results = JsonConvert.DeserializeObject<List<ProductVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductVM> DropDownByCategory(string catId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownByCategory(catId,connVMwcf);

                List<ProductVM> results = JsonConvert.DeserializeObject<List<ProductVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductNameVM> SelectProductName(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectProductName(ItemNo,conditionFieldswcf, conditionValueswcf,likeVMwcf, connVMwcf);

                List<ProductNameVM> results = JsonConvert.DeserializeObject<List<ProductNameVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductVM> GetProductByType(string type, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductByType(type,connVMwcf);

                List<ProductVM> results = JsonConvert.DeserializeObject<List<ProductVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductVM> SelectAllOverhead(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllOverhead(ItemNo, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<ProductVM> results = JsonConvert.DeserializeObject<List<ProductVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ProductVM GetProductWithCostPrice(string productCode, string purchaseNo, string effectDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string result = wcf.GetProductWithCostPrice(productCode, purchaseNo, effectDate);

                ProductVM results = JsonConvert.DeserializeObject<ProductVM>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetProductCodeUOMn(string ProductCode, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductCodeUOMn(ProductCode, ProductGroup, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetProductIdByName(string ProductName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductIdByName(ProductName, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetBomIdFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetBomIdFromBOM(FinishItemNo,RawItemNo,VatName,effectDate, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetExistingProductName(string ProductName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExistingProductName(ProductName, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetProductNoByGroup(string ProductName, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductNoByGroup(ProductName, ProductGroup, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetProductNoByGroup_Code(string ProductCode, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductNoByGroup_Code(ProductCode, ProductGroup, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetProductUOMc(string uomFrom, string uomTo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductUOMc(uomFrom, uomTo, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetProductUOMn(string ProductName, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductUOMn(ProductName, ProductGroup, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string TrackingStockCheck(string ItemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.TrackingStockCheck(ItemNo, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
    }
}
