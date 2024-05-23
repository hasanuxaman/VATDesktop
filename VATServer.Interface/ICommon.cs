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
   public interface ICommon
    {

       int DataAlreadyUsed(string CompareTable, string CompareField, string CompareWith, SqlConnection currConn,
            SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

       string settings(string SettingGroup, string SettingName, SqlConnection VcurrConn = null
           , SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string settingValue(string settingGroup, string settingName, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null
           , SqlTransaction Vtransaction = null);

       string settingsDesktop(string SettingGroup, string SettingName, DataTable dt = null, SysDBInfoVMTemp connVM = null);

       DataTable MultipleSearchSQL(string sqlText, string SearchValue, string[] ConditionFields = null, string count = "", string SQLTextRecordCount = "", SysDBInfoVMTemp connVM = null);

       DataTable MultipleSearch(string tableName, string SearchValue, string[] ConditionFields = null, string FixedConditions = "", SysDBInfoVMTemp connVM = null);

       string[] BulkInsert(string tableName, DataTable data, SqlConnection VcurrConn = null,
           SqlTransaction Vtransaction = null, int batchSize = 0,
           SqlRowsCopiedEventHandler rowsCopiedCallBack = null, SysDBInfoVMTemp connVM = null);

       int NextId(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null);

       DataTable GenericSelection(string tableName, string databaseName, string[] cFields = null, string[] cVals = null, SysDBInfoVMTemp connVM = null);

       DataTable GenericSelectionNotSync(string tableName, string databaseName, SysDBInfoVMTemp connVM = null);

       string[] UpdateIsVATComplete(string TableName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string[] BomSynchronize(DataTable dtBOMs, DataTable dtBOMRaws, DataTable dtBOMOverhead, SysDBInfoVMTemp connVM = null);

       string[] TransactionSync(string TableName, DataSet ds, string loadedDatabase, SysDBInfoVMTemp connVM = null);

       DataTable SearchTransanctionHistoryNew(string TransactionNo, string TransactionType, string TransactionDateFrom, string TransactionDateTo, string ProductName, string databaseName, SysDBInfoVMTemp connVM = null);

       bool TestConnection(string userName, string Password, string Datasource, bool IsWindowsAuthentication = false, SysDBInfoVMTemp connVM = null);

       DataSet CompanyList(string ActiveStatus, SysDBInfoVMTemp connVM = null, string CompanyList = "");

       DataTable SuperAdministrator(SysDBInfoVMTemp connVM = null);

       DataSet SuperDBInformation(SysDBInfoVMTemp connVM = null);

       string[] NewDBCreate(CompanyProfileVM companyProfiles, string databaseName, List<FiscalYearVM> fiscalDetails, SysDBInfoVMTemp connVM = null);

       string[] SuperAdministratorUpdate(string miki, string mouse, SysDBInfoVMTemp connVM = null);

       string[] DatabaseInformationUpdate(string Tom, string jary, string mini, SysDBInfoVMTemp connVM = null);

       bool UpdateSystemData(string userName, string password, string source, SysDBInfoVMTemp connVM = null);

       void AddBranchInfo(SysDBInfoVMTemp connVM = null);

       int DataAlreadyUsedWithoutThis(string CompareTable, string CompareField, string CompareWith, string IdField, string IdValue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

       string GetHardwareID(SysDBInfoVMTemp connVM = null);

       string GetServerHardwareId(SysDBInfoVMTemp connVM = null);

       DataTable ExecuteQuerySelect(string sqlText = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

       string[] ExecuteQuery(string sqlText = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string ServerDateTime(SysDBInfoVMTemp connVM = null);

       void SettingChange(SysDBInfoVMTemp connVM = null);

       string[] SaveTempTest(List<SaleTempVM> vms, SysDBInfoVMTemp connVM = null);

       //static string CompanyListUpdate1(SqlCommand objCommCustomerGroup, string CompanyID, string CompanyName, string DatabaseName, string ActiveStatus);

    }
}
