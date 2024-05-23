using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.ReceiveWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class ReceiveRepo : IReceive
    {
        ReceiveWCFClient wcf = new ReceiveWCFClient();

        public DataTable SearchByReferenceNo(string ReferenceNo, string ItemNo = "", SysDBInfoVMTemp connVM = null
            , string transactionType = "", string ShiftId = "0")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchByReferenceNo(ReferenceNo, ItemNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;


            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }

        public decimal FormatingNumeric(decimal value, int DecPlace, SysDBInfoVMTemp connVM = null)
        {
             try
            {

                string result = wcf.FormatingNumeric(value, DecPlace);

                decimal results = JsonConvert.DeserializeObject<decimal>(result);

                return results;


            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExcelData(invoiceListwcf,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] GetUSDCurrency(decimal subTotal, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string result = wcf.GetUSDCurrency(subTotal);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] ImportData(DataTable dtReceiveM, DataTable dtReceiveD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string dtReceiveMwcf = JsonConvert.SerializeObject(dtReceiveM);
                string dtReceiveDwcf = JsonConvert.SerializeObject(dtReceiveD);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportData(dtReceiveMwcf, dtReceiveDwcf, branchId,null, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] ImportData_Sanofi(DataTable dtReceiveM, DataTable dtReceiveD, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtReceiveMwcf = JsonConvert.SerializeObject(dtReceiveM);
                string dtReceiveDwcf = JsonConvert.SerializeObject(dtReceiveD);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportData_Sanofi(dtReceiveMwcf, dtReceiveDwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] ImportExcelFile(ReceiveMasterVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string paramVMwcf = JsonConvert.SerializeObject(paramVM);

                string result = wcf.ImportExcelFile(paramVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string Idswcf = JsonConvert.SerializeObject(Ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.MultiplePost(Idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] ReceiveInsert(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, int BranchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "")
        {

            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReceiveInsert(Masterwcf, Detailswcf, Trackingswcf, BranchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] ReceivePost(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReceivePost(Masterwcf, Detailswcf, Trackingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] ReceiveUpdate(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, SqlConnection currConn = null, SqlTransaction transaction = null, string UserId = "")
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string Trackingswcf = JsonConvert.SerializeObject(Trackings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReceiveUpdate(Masterwcf, Detailswcf, Trackingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public decimal ReturnReceiveQty(string receiveReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReturnReceiveQty(receiveReturnId, itemNo, connVMwcf);

                decimal results = JsonConvert.DeserializeObject<decimal>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] SaveTempReceive(DataTable dtTableResult, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,string BranchCode = "")
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(dtTableResult);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SaveTempReceive(Masterwcf, transactionType, CurrentUser,branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DataTable SearchReceiveDetailNew(string ReceiveNo, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchReceiveDetailNew(ReceiveNo, databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DataTable SearchReceiveHeaderDTNew(string ReceiveNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string result = wcf.SearchReceiveHeaderDTNew(ReceiveNo);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ReceiveMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf,likeVMwcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(resultInsert);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ReceiveMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ReceiveMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);

                string resultInsert = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf);

                List<ReceiveMasterVM> results = JsonConvert.DeserializeObject<List<ReceiveMasterVM>>(resultInsert);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ReceiveDetailVM> SelectReceiveDetail(string ReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.SelectReceiveDetail(ReceiveNo, conditionFieldswcf, conditionValueswcf,connVMwcf);

                List<ReceiveDetailVM> results = JsonConvert.DeserializeObject<List<ReceiveDetailVM>>(resultInsert);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
