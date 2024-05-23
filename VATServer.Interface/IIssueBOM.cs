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
    public interface IIssueBOM
    {


        string[] ImportData(DataTable dtIssueM, DataTable dtIssueD, SysDBInfoVMTemp connVM = null);
        string[] ImportExcelFile(IssueMasterVM paramVM, SysDBInfoVMTemp connVM = null);
        string[] IssueInsert(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] IssuePost(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "");
        string[] IssueUpdate(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null);
        decimal ReturnIssueQty(string issueReturnId, string itemNo, SysDBInfoVMTemp connVM = null);
        DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName, SysDBInfoVMTemp connVM = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);
        List<IssueMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null);
        List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);





    }
}
