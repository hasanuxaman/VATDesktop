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
   public interface IVendor
    {
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

        string[] Delete(VendorVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        List<VendorVM> DropDown(SysDBInfoVMTemp connVM = null);
        List<VendorVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);


        DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);


        List<VendorVM> DropDownAll(SysDBInfoVMTemp connVM = null);
       string[] InsertToVendorNewSQL(VendorVM vm, bool autoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
       string[] UpdateVendorNewSQL(VendorVM vm, SysDBInfoVMTemp connVM = null);



    }
}
