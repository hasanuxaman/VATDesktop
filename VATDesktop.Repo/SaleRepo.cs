using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VATDesktop.Repo.SaleWCF;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class SaleRepo : ISale
    {
        //SaleDAL _dal = new SaleDAL();

        SaleWCFClient wcf = new SaleWCFClient();


        public string GetCategoryName(string itemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string name = wcf.GetCategoryName(itemNo, connVMwcf);


                return name;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesInsert(SaleMasterVM MasterVM, List<SaleDetailVm> DetailVMs, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, int branchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "", bool IsManualEntry = true, List<SaleDeliveryTrakingVM> DeliveryTrakings = null,bool CodeGenaration = true)
        {

            try
            {
                string MasterVMwcf =JsonConvert.SerializeObject(MasterVM);
                string DetailVMswcf =JsonConvert.SerializeObject(DetailVMs);
                string ExportDetailswcf =JsonConvert.SerializeObject(ExportDetails);
                string Trackingswcf =JsonConvert.SerializeObject(Trackings);
                string connVMwcf =JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.SalesInsert(MasterVMwcf, DetailVMswcf, ExportDetailswcf, Trackingswcf, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(resultInsert);

                return results;
                

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesUpdate(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, string UserId = "", bool IsManualEntry = true, List<SaleDeliveryTrakingVM> DeliveryTrakings = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string ExportDetailswcf = JsonConvert.SerializeObject(ExportDetails);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SalesUpdate(Masterwcf, Detailswcf, ExportDetailswcf, Trackingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesPost(SaleMasterVM Master, List<SaleDetailVm> Details, List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null, bool BulkPost = false, string UserId = "")
        {
           
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SalesPost(Masterwcf, Detailswcf, Trackingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] CurrencyInfo(string salesInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.CurrencyInfo(salesInvoiceNo, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string ImportIDExcel = "", SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchSalesHeaderDTNew(SalesInvoiceNo, ImportIDExcel, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchSaleDetailDTNew(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchSaleDetailDTNew(SalesInvoiceNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal ReturnSaleQty(string saleReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReturnSaleQty(saleReturnId, itemNo, connVMwcf);

                decimal results =Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchSaleExportDTNew(SalesInvoiceNo, databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchSaleDetailTemp(string SalesInvoiceNo, string userId, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public DataTable SelectAllHeaderTemp(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, bool Dt = false,
           SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] ImportData(DataTable dtSaleM, DataTable dtSaleD, DataTable dtSaleE, bool CommercialImporter = false, int branchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string dtSaleMwcf = JsonConvert.SerializeObject(dtSaleM);
                string dtSaleDwcf = JsonConvert.SerializeObject(dtSaleD);
                string dtSaleEwcf = JsonConvert.SerializeObject(dtSaleE);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportData(dtSaleMwcf, dtSaleDwcf, dtSaleEwcf, CommercialImporter , branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetDeliveryChallanNo(string saleInvoiceNo, string challanDate, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                wcf.SetDeliveryChallanNo(saleInvoiceNo, challanDate, connVMwcf);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetGatePass(string saleInvoiceNo, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                wcf.SetGatePass(saleInvoiceNo, connVMwcf);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
            
        public List<SaleMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , SaleMasterVM likeVM = null, SysDBInfoVMTemp connVM = null, string transactionType = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null)
        {
            
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result= wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);
                List<SaleMasterVM> results = JsonConvert.DeserializeObject<List<SaleMasterVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null
            , bool Dt = false, string TransectionType = null, string MultipleSearch = "N", SysDBInfoVMTemp connVM = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null, string IsVDSCompleted = null)
        {
          
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string table = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetSaleExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.GetSaleExcelData(invoiceListwcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = null)
        {
            try
            {
                string Idswcf = JsonConvert.SerializeObject(Ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.MultiplePost(Idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

              return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] PostSales(DataTable table, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string tablewcf = JsonConvert.SerializeObject(table);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PostSales(tablewcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

         public string[] UpdatePrintNew(string InvoiceNo, int PrintCopy, SysDBInfoVMTemp connVM = null)
        {
            try
            {
              
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdatePrintNew(InvoiceNo, PrintCopy, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

         public string[] SaveAndProcess(DataTable data, Action callBack = null, int branchId = 1, string app = "",
             SysDBInfoVMTemp connVM = null, IntegrationParam paramVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string UserId = "", bool IsExcelUpload = false)
         {
             try
             {
                 string dataWCF = JsonConvert.SerializeObject(data);
                 string connVMWCF = JsonConvert.SerializeObject(connVM);

                 string result = wcf.SaveAndProcess(dataWCF, branchId, app, connVMWCF);

                 return JsonConvert.DeserializeObject<string[]>(result);
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable CheckInvoiceNoExist(List<string> ids, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string idswcf = JsonConvert.SerializeObject(ids);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.CheckInvoiceNoExist(idswcf, connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable GetById(string id, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetById(id, connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable GetInvoiceNoFromTemp(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetInvoiceNoFromTemp(connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable GetMasterData(SqlConnection vConnection = null, SqlTransaction vTransaction = null, string app = "", SysDBInfoVMTemp connVM = null, string userId = null, bool orderBy = false, string token = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetMasterData(app, connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable GetOldDbList(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetOldDbList(connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable GetSaleJoin(string invoiceNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetSaleJoin(invoiceNo,connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable GetUnProcessedTempData(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetUnProcessedTempData(connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public DataTable SaleTempNoBranch(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.SaleTempNoBranch(connVMwcf);

                 DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public int GetUnProcessedCount(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetUnProcessedCount(connVMwcf);

                 int results = JsonConvert.DeserializeObject<int>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public int GetUnProcessedCount_tempTable(string tempTable, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetUnProcessedCount_tempTable(tempTable,connVMwcf);

                 int results = JsonConvert.DeserializeObject<int>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] GetFisrtInvoiceId(SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetFisrtInvoiceId(connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] GetTopUnProcessed(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.GetTopUnProcessed(connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] ImportBigData(DataTable salesData, bool deleteFlag = true, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null, bool IsDiscount = false, string userId = null, string token = null, IntegrationParam paramVM = null, bool IsExcelUpload = false)
         {
             try
             {
                 string salesDatawcf = JsonConvert.SerializeObject(salesData);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.ImportBigData(salesDatawcf, deleteFlag, branchId,connVMwcf,IsDiscount,userId);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] ImportSalesBigData(DataTable salesData, bool deleteFlag = true, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string salesDatawcf = JsonConvert.SerializeObject(salesData);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.ImportSalesBigData(salesDatawcf, deleteFlag, connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] ProccessTempData(SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.ProccessTempData(connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] SaveAndProcessOtherDb(DataTable data, Action callBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string datawcf = JsonConvert.SerializeObject(data);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.SaveAndProcessOtherDb(datawcf, branchId,connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] SaveAndProcessTempData(DataTable data, Action callBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string datawcf = JsonConvert.SerializeObject(data);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.SaveAndProcessTempData(datawcf, branchId, connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] SaveInvoiceIdSaleTemp(int firstId, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string table = wcf.SaveInvoiceIdSaleTemp(firstId, connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] SaveSalesToKohinoor(DataTable table, DataTable db, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string tablewcf = JsonConvert.SerializeObject(table);
                 string dbwcf = JsonConvert.SerializeObject(db);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string Table = wcf.SaveSalesToKohinoor(tablewcf,dbwcf, connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] SaveSalesToLink3(DataTable table, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string tablewcf = JsonConvert.SerializeObject(table);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string Table = wcf.SaveSalesToLink3(tablewcf,connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] SaveTempTest(DataTable table, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string tablewcf = JsonConvert.SerializeObject(table);
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string Table = wcf.SaveTempTest(tablewcf, connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public string[] UpdateIsProcessed(int flag, string Id, SysDBInfoVMTemp connVM = null)
         {
             try
             {
                 string connVMwcf = JsonConvert.SerializeObject(connVM);

                 string Table = wcf.UpdateIsProcessed(flag,Id, connVMwcf);

                 string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                 return results;
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         //public string[] EXPNoUpdate(SaleMasterVM Master, SysDBInfoVMTemp connVM = null, string UserId = "")
         //{
         //    try
         //    {
         //        string Masterwcf = JsonConvert.SerializeObject(Master);
         //        //string Detailswcf = JsonConvert.SerializeObject(Details);
         //        //string ExportDetailswcf = JsonConvert.SerializeObject(ExportDetails);
         //        //string Trackingswcf = JsonConvert.SerializeObject(Trackings);
         //        string connVMwcf = JsonConvert.SerializeObject(connVM);


         //        string result = wcf.EXPNoUpdate(Masterwcf, connVMwcf);

         //        string[] results = JsonConvert.DeserializeObject<string[]>(result);

         //        return results;
         //    }
         //    catch (Exception e)
         //    {
         //        throw e;
         //    }
         //}


    }
}
