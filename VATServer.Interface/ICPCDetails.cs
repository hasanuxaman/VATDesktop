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
    public interface ICPCDetails
    {
        string[] Delete(CPCDetailsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] InsertfromExcel(CPCDetailsVM vm, SysDBInfoVMTemp connVM = null);
        string[] InsertToCPCDetails(CPCDetailsVM vm,bool AutoSave = false, SysDBInfoVMTemp connVM = null);
        string[] UpdateCPCDetails(CPCDetailsVM vm, SysDBInfoVMTemp connVM = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);
        List<CPCDetailsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
    

    }
}
