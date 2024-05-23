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
    public interface IIssue
    {
        decimal ReturnIssueQty(string issueReturnId, string itemNo, SysDBInfoVMTemp connVM = null);
        int GetUnProcessedCount(SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);
        string[] ImportBigData(DataTable issueData, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);
        string[] ImportData(DataTable dtIssueM, DataTable dtIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "");
        string[] ImportExcelFile(IssueMasterVM paramVM, SysDBInfoVMTemp connVM = null);
        string[] ImportReceiveBigData(DataTable receiveData, SysDBInfoVMTemp connVM = null);
        string[] IssueInsert(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] IssuePost(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null, string UserId = "");
        string[] IssueUpdate(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);
        string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null);
        string[] SaveTempIssue(DataTable dtTable, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string branchCode="");
        DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        List<IssueMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null);
        List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,bool multipleIssue = false);
        DataTable SelectAllHeaderTemp(int Id = 0, string[] conditionFields = null,
             string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
             SaleMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);
        DataTable SearchIssueDetailTemp(string SalesInvoiceNo, string userId, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);


    }
}
