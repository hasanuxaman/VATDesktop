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
  public interface IUOMName
    {
         string[] Delete(UOMNameVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         List<UOMNameVM> DropDown(SysDBInfoVMTemp connVM = null);

         List<UOMNameVM> DropDownAll(SysDBInfoVMTemp connVM = null);

         string[] InsertToUOMName(UOMNameVM vm, SysDBInfoVMTemp connVM = null);

         DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

         List<UOMNameVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] UpdateUOMName(UOMNameVM vm, SysDBInfoVMTemp connVM = null);

    }
}
