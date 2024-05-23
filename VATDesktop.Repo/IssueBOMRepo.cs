using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.IssueBOMWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class IssueBOMRepo : IIssueBOM
    {

        IssueBOMWCFClient wcf = new IssueBOMWCFClient();

        public string[] ImportData(DataTable dtIssueM, DataTable dtIssueD, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtIssueMwcf = JsonConvert.SerializeObject(dtIssueM);
                string dtIssueDwcf = JsonConvert.SerializeObject(dtIssueD);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportData(dtIssueMwcf, dtIssueDwcf, connVMwcf);

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

        public string[] IssuePost(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "")
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

        public string[] IssueUpdate(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null)
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

        public DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
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

        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
