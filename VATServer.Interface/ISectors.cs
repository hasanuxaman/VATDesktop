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
    public interface ISectors
    {

        string[] InsertSectors(SectorsVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);

        string[] UpdateSectors(SectorsVM vm, SysDBInfoVMTemp connVM = null);

        string[] Delete(SectorsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        
        List<SectorsVM> DropDownAll(SysDBInfoVMTemp connVM = null);

        List<SectorsVM> DropDown(SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<SectorsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
