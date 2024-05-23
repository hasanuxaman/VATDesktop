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
    public interface IMPLIN89
    {

        string[] MPLIN89Insert(MPLIN89VM vm, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] MPLIN89Update(MPLIN89VM vm, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLIN89VM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLIN89DetailsVM> SearchMPLIN89DetailsList(string mplIN89HeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<MPLIN89IssueDetailsVM> SearchMPLIN89IssueDetailsList(string mplIN89HeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] MPLIN89Post(MPLIN89VM vm, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        List<MPLIN89VM> DropDown(string branchId, SysDBInfoVMTemp connVM = null);

        string[] Delete(MPLIN89VM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);


        List<MPLIN89VM> TransReceiveIndex(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLIN89IssueDetailsVM> SearchTransferMPLIssuesList(string ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<MPLIN89DetailsVM> SearchTransferMPLReceivesList(string ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
