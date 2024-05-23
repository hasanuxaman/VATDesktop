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
    public interface IVendorGroup
    {
         string[] Delete(VendorGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
       
         List<VendorGroupVM> DropDown(SysDBInfoVMTemp connVM = null);

         List<VendorGroupVM> DropDownAll(SysDBInfoVMTemp connVM = null);

         string[] InsertToVendorGroup(VendorGroupVM vm, SysDBInfoVMTemp connVM = null);

         DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

         List<VendorGroupVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] UpdateToVendorGroup(VendorGroupVM vm, SysDBInfoVMTemp connVM = null);


    }
}
