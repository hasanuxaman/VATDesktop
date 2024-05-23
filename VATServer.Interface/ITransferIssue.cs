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
   public interface ITransferIssue
    {
       DataSet FormLoad(UOMVM UOMvm, ProductVM Productvm, string Name, SysDBInfoVMTemp connVM = null);

       DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       DataTable GetExcelTransferData(List<string> invoiceList, string transactionType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string[] ImportData(DataTable dtTrIssueM, DataTable dtTrIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, bool integration = false, SysDBInfoVMTemp connVM = null, string UserId = "");

       string[] ImportTransferData(DataTable dtTrIssueM, DataTable dtTrIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string[] Insert(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null, List<TrackingVM> Trackings = null, bool CodeGenaration = true);

       string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = null);

       string[] Post(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string[] PostTransfer(TransferIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       //string[] SaveTempTransfer(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool integration = false, SysDBInfoVMTemp connVM = null);

       DataTable SearchTransfer(TransferVM vm, SysDBInfoVMTemp connVM = null);

       DataTable SearchTransferDetail(string TransferIssueNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       DataTable SearchTransferIssue(TransferIssueVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

       List<TransferIssueVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       List<TransferIssueDetailVM> SelectDetail(string TransferIssueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       DataSet TransferMovement(string ItemNo, string FDate, string TDate, int BranchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Summery = false, SysDBInfoVMTemp connVM = null);

       DataSet TransferStock(string ItemNo, string FDate, int BranchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null) ;

       string[] Update(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null);

       string[] UpdateTransferIssue(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
