using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.PurchaseWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class PurchaseRepo : IPurchase
    {

        PurchaseWCFClient wcf = new PurchaseWCFClient();


        public DataTable SearchPurchaseDutyDTNew(string PurchaseInvoiceNo, SysDBInfoVMTemp connVM = null, string ItemNo = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchPurchaseDutyDTNew(PurchaseInvoiceNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SearchPurchaseInvoiceTracking(string purchaseInvoiceNo, string itemNo, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchPurchaseInvoiceTracking(purchaseInvoiceNo, itemNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseMasterVM likeVM = null
            , bool Dt = false, SysDBInfoVMTemp connVM = null, bool VDSSearch = false, bool IsDisposeRawSearch = false, string ItemNo = null, bool IsBankingChannelPay = false, bool IsClints6_3Search = false)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf,VDSSearch);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectPurchaseDetail(string PurchaseInvoiceNo, string[] conditionFields = null
            , string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectPurchaseDetail(PurchaseInvoiceNo, conditionFieldswcf , conditionValueswcf , Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] PurchaseInsert(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties
            , List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, int branchId = 0
            , SysDBInfoVMTemp connVM = null, string UserId = "")
        {

            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Dutieswcf = JsonConvert.SerializeObject(Duties);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PurchaseInsert(Masterwcf, Detailswcf, Dutieswcf
            ,  Trackingswcf, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        public string[] PurchaseUpdate(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties
            , List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Dutieswcf = JsonConvert.SerializeObject(Duties);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PurchaseUpdate(Masterwcf, Detailswcf, Dutieswcf
            , Trackingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] PurchasePost(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties
            , List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null
            , SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Dutieswcf = JsonConvert.SerializeObject(Duties);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PurchasePost(Masterwcf, Detailswcf, Dutieswcf
            , Trackingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal ReturnQty(string purchaseReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
               
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReturnQty(purchaseReturnId, itemNo, connVMwcf);

                decimal results = Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateTDSAmount(string PurchaseInvoiceNo, decimal TDSAmount, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateTDSAmount(PurchaseInvoiceNo, TDSAmount , connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        public string[] ImportData(DataTable dtPurchaseM, DataTable dtPurchaseD, DataTable dtPurchaseI, DataTable dtPurchaseT, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {

                string dtPurchaseMwcf = JsonConvert.SerializeObject(dtPurchaseM);
                string dtPurchaseDwcf = JsonConvert.SerializeObject(dtPurchaseD);
                string dtPurchaseIwcf = JsonConvert.SerializeObject(dtPurchaseI);
                string dtPurchaseTwcf = JsonConvert.SerializeObject(dtPurchaseT);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportData(dtPurchaseMwcf, dtPurchaseDwcf, dtPurchaseIwcf, dtPurchaseTwcf,branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportBigData(DataTable PurchaseData, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string PurchaseDatawcf = JsonConvert.SerializeObject(PurchaseData);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportBigData(PurchaseDatawcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportExcelFile(PurchaseMasterVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string paramVMwcf = JsonConvert.SerializeObject(paramVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportExcelFile(paramVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null)
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
        public string[] PurchaseUpdateDuty(DataTable DtDuty, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string DtDutywcf = JsonConvert.SerializeObject(DtDuty);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PurchaseUpdateDuty(DtDutywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] SaveTempPurchase(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "", bool IsExcel = false)
        {
            try
            {

                string datawcf = JsonConvert.SerializeObject(data);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SaveTempPurchase(datawcf, BranchCode, transactionType, CurrentUser, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string RateChangePercent(string ItemNo, decimal unitPrice, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.RateChangePercent(ItemNo, unitPrice, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string FindItemIdFromPurchase(string PurchaseInvoiceNo, string ItemNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FindItemIdFromPurchase(PurchaseInvoiceNo, ItemNo, connVMwcf);

                return result;
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

                string result = wcf.GetUnProcessedCount(connVMwcf);

                int results = Convert.ToInt32(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public decimal FormatingNumeric(decimal value, int DecPlace, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FormatingNumeric(value, DecPlace,connVMwcf);

                int results = Convert.ToInt32(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelData(List<string> invoiceList, bool withDuty, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExcelData(invoiceListwcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable PurchaseSearch(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PurchaseSearch(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchProductbyPurchaseInvoice(string purchaseInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchProductbyPurchaseInvoice(purchaseInvoiceNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchPurchaseDutyDTDownload(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
            SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchPurchaseDutyDTDownload(PurchaseInvoiceNo,InvoiceDateFrom,InvoiceDateTo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchPurchaseHeaderDTNew2(string PurchaseInvoiceNo, string WithVDS, string VendorName, string VendorGroupID,
            string VendorGroupName, string InvoiceDateFrom, string InvoiceDateTo, string SerialNo, string T1Type, string T2Type,
            string BENumber, string Post, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchPurchaseHeaderDTNew2( PurchaseInvoiceNo,  WithVDS,  VendorName,  VendorGroupID,
             VendorGroupName,  InvoiceDateFrom,  InvoiceDateTo,  SerialNo,  T1Type,  T2Type,
             BENumber,  Post, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public List<PurchaseDetailVM> RateCheck(List<PurchaseDetailVM> VMs, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string VMswcf = JsonConvert.SerializeObject(VMs);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.RateCheck(VMswcf, connVMwcf);

                List<PurchaseDetailVM> results = JsonConvert.DeserializeObject<List<PurchaseDetailVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public List<PurchaseMasterVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(connVMwcf);

                List<PurchaseMasterVM> results = JsonConvert.DeserializeObject<List<PurchaseMasterVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<PurchaseMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseMasterVM likeVM = null,
            SysDBInfoVMTemp connVM = null, string ItemNo = null, bool IsDisposeRawSearch = false, bool VDSSearch = false)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);

                List<PurchaseMasterVM> results = JsonConvert.DeserializeObject<List<PurchaseMasterVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<PurchaseDetailVM> SelectPurchaseDetailList(string PurchaseInvoiceNo, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectPurchaseDetailList(PurchaseInvoiceNo, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<PurchaseDetailVM> results = JsonConvert.DeserializeObject<List<PurchaseDetailVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public DataTable SelectAllDuties(string PurchaseInvoiceNo, string[] conditionFields = null, string[] conditionValues = null,
        //    SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        //{
        //    try
        //    {

        //        string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
        //        string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
        //        string connVMwcf = JsonConvert.SerializeObject(connVM);

        //        string result = wcf.SelectAllDuties( PurchaseInvoiceNo, conditionFieldswcf , conditionValueswcf , Dt, connVMwcf);

        //        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

        //        return results;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        



    }
}
