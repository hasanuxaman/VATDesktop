using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATViewModel.DTOs;
using VATDesktop.Repo.CommonWCF;
using Newtonsoft.Json;
using VATServer.Interface;
using System.Data;


namespace VATDesktop.Repo
{
    public class CommonRepo : ICommon
    {
        //CommonDAL _dal = new CommonDAL();

        CommonWCFClient wcf = new CommonWCFClient();

        public string settingsDesktop(string SettingGroup, string SettingName, DataTable dt = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string value = wcf.SettingsDesktop(SettingGroup, SettingName);


                return value;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int DataAlreadyUsed(string CompareTable, string CompareField, string CompareWith, SqlConnection currConn,
            SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.DataAlreadyUsed(CompareTable, CompareField, CompareWith, connVMwcf);

                int Results = Convert.ToInt32(retResults);

                return Results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public string settingValue(string settingGroup, string settingName, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null
           , SqlTransaction Vtransaction = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.SettingValue(settingGroup, settingName, connVMwcf);

                return retResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string settings(string SettingGroup, string SettingName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.Settings(SettingGroup, SettingName, connVMwcf);
                return retResults;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MultipleSearchSQL(string sqlText, string SearchValue, string[] ConditionFields = null, string count = "", string SQLTextRecordCount = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(ConditionFields);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.MultipleSearchSQL(sqlText, SearchValue, conditionFieldswcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MultipleSearch(string tableName, string SearchValue, string[] ConditionFields = null, string FixedConditions = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(ConditionFields);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.MultipleSearch(tableName, SearchValue, conditionFieldswcf,FixedConditions, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] BulkInsert(string tableName, DataTable data, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, int batchSize = 0,
            SqlRowsCopiedEventHandler rowsCopiedCallBack = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string datawcf = JsonConvert.SerializeObject(data);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.BulkInsert(tableName, datawcf, batchSize, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int NextId(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.NextId(tableName, connVMwcf);

                int results = Convert.ToInt32(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GenericSelection(string tableName, string databaseName, string[] cFields = null, string[] cVals = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string cFieldswcf = JsonConvert.SerializeObject(cFields);
                string cValswcf = JsonConvert.SerializeObject(cVals);

                string retResults = wcf.GenericSelection(tableName, databaseName, cFieldswcf, cValswcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GenericSelectionNotSync(string tableName, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string retResults = wcf.GenericSelectionNotSync(tableName, databaseName);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable SearchTransanctionHistoryNew(string TransactionNo, string TransactionType, string TransactionDateFrom, string TransactionDateTo, string ProductName, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.SearchTransanctionHistoryNew(TransactionNo, TransactionType, TransactionDateFrom, TransactionDateTo, ProductName, databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExecuteQuerySelect(string sqlText = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.ExecuteQuerySelect(sqlText, Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SuperAdministrator(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string retResults = wcf.SuperAdministrator();

                DataTable results = JsonConvert.DeserializeObject<DataTable>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet CompanyList(string ActiveStatus, SysDBInfoVMTemp connVM = null, string CompanyList = "")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.CompanyList(ActiveStatus,connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet SuperDBInformation(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string retResults = wcf.SuperDBInformation();

                DataSet results = JsonConvert.DeserializeObject<DataSet>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateIsVATComplete(string TableName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.UpdateIsVATComplete(TableName, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] BomSynchronize(DataTable dtBOMs, DataTable dtBOMRaws, DataTable dtBOMOverhead, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtBOMswcf = JsonConvert.SerializeObject(dtBOMs);
                string dtBOMRawswcf = JsonConvert.SerializeObject(dtBOMRaws);
                string dtBOMOverheadwcf = JsonConvert.SerializeObject(dtBOMOverhead);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.BomSynchronize(dtBOMswcf,  dtBOMRawswcf,  dtBOMOverheadwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] TransactionSync(string TableName, DataSet ds, string loadedDatabase, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dswcf = JsonConvert.SerializeObject(ds);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.TransactionSync(TableName, dswcf, loadedDatabase, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] NewDBCreate(CompanyProfileVM companyProfiles, string databaseName, List<FiscalYearVM> fiscalDetails, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string companyProfileswcf = JsonConvert.SerializeObject(companyProfiles);
                string fiscalDetailswcf = JsonConvert.SerializeObject(fiscalDetails);

                string retResults = wcf.NewDBCreate(companyProfileswcf, databaseName, fiscalDetailswcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SuperAdministratorUpdate(string miki, string mouse, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string retResults = wcf.SuperAdministratorUpdate(miki, mouse);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DatabaseInformationUpdate(string Tom, string jary, string mini, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.DatabaseInformationUpdate(Tom, jary, mini, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ExecuteQuery(string sqlText = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.ExecuteQuery(sqlText, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SaveTempTest(List<SaleTempVM> vms, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmswcf = JsonConvert.SerializeObject(vms);

                string retResults = wcf.SaveTempTest(vmswcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetHardwareID(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string retResults = wcf.GetHardwareID();


                return retResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetServerHardwareId(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string retResults = wcf.GetServerHardwareId(connVMwcf);


                return retResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ServerDateTime(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string retResults = wcf.ServerDateTime(connVMwcf);


                return retResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool TestConnection(string userName, string Password, string Datasource, bool IsWindowsAuthentication = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string retResults = wcf.TestConnection( userName,  Password,  Datasource);

                bool Results = Convert.ToBoolean(retResults);

                return Results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateSystemData(string userName, string password, string source,SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string retResults = wcf.UpdateSystemData(userName, password, source);

                bool Results = Convert.ToBoolean(retResults);

                return Results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddBranchInfo(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                wcf.AddBranchInfo();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SettingChange(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                wcf.SettingChange();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DataAlreadyUsedWithoutThis(string CompareTable, string CompareField, string CompareWith, string IdField, string IdValue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string retResults = wcf.DataAlreadyUsedWithoutThis(CompareTable, CompareField, CompareWith, IdField, IdValue, connVMwcf);

                int results = Convert.ToInt32(retResults);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static string CompanyListUpdate1(SqlCommand objCommCustomerGroup, string CompanyID, string CompanyName, string DatabaseName, string ActiveStatus);


    }
}
