using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.IssueWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class IssueRepo:IIssue
    {

        IssueWCFClient wcf = new IssueWCFClient();


        public decimal ReturnIssueQty(string issueReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReturnIssueQty(issueReturnId, itemNo, connVMwcf);

                decimal results = Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAllHeaderTemp(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, bool Dt = false,
          SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }
        public DataTable SearchIssueDetailTemp(string IssueNo, string userId, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            throw new NotImplementedException();
        }
        public int GetUnProcessedCount(SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
        public string[] ImportBigData(DataTable issueData, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                string issueDatawcf = JsonConvert.SerializeObject(issueData);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportBigData(issueDatawcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportData(DataTable dtIssueM, DataTable dtIssueD, int branchId = 0, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string dtIssueMwcf = JsonConvert.SerializeObject(dtIssueM);
                string dtIssueDwcf = JsonConvert.SerializeObject(dtIssueD);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportData(dtIssueMwcf, dtIssueDwcf, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportExcelFile(IssueMasterVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string paramVMwcf = JsonConvert.SerializeObject(paramVM);

                string result = wcf.ImportExcelFile(paramVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportReceiveBigData(DataTable receiveData, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string receiveDatawcf = JsonConvert.SerializeObject(receiveData);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportReceiveBigData(receiveDatawcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] IssueInsert(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.IssueInsert(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] IssuePost(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.IssuePost(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] IssueUpdate(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.IssueUpdate(Masterwcf, Detailswcf, connVMwcf);

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
        public string[] SaveTempIssue(DataTable dtTable, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,string BranchCode = "")
        {
            try
            {
                string dtTablewcf = JsonConvert.SerializeObject(dtTable);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SaveTempIssue(dtTablewcf, transactionType, CurrentUser, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                //string IssueNowcf = JsonConvert.SerializeObject(IssueNo);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchIssueDetailDTNew(IssueNo, databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
        public List<IssueMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);

                List<IssueMasterVM> results = JsonConvert.DeserializeObject<List<IssueMasterVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool multipleIssue = false)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectIssueDetail(issueNo, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<IssueDetailVM> results = JsonConvert.DeserializeObject<List<IssueDetailVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}
