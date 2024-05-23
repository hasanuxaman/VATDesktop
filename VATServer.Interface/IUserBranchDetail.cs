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
   public interface IUserBranchDetail
    {
         string[] Insert(List<UserBranchDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] InsertfromExcel(DataTable datatable, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string CreatedBy = null, string Createdon = null, SysDBInfoVMTemp connVM = null);

         DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

         List<UserBranchDetailVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
