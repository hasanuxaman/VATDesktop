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
    public interface IHSCode
    {
        string[] Delete(HSCodeVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] InsertfromExcel(HSCodeVM vm, SysDBInfoVMTemp connVM = null);
        string[] InsertToHSCode(HSCodeVM vm, SysDBInfoVMTemp connVM = null);
        string[] UpdateHSCode(HSCodeVM vm, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelData(List<string> HSCode, SqlConnection VCon = null, SqlTransaction VTransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);
        List<HSCodeVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
    
    }
}
