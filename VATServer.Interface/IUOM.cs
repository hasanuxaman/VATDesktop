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
   public interface IUOM
    {

       DataTable SearchUOMCodeOnly(string ActiveStatus, SysDBInfoVMTemp connVM = null);
       
       DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

       List<UOMConversionVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string[] Delete(UOMConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       string[] InsertToUOMNew(UOMConversionVM vm, SysDBInfoVMTemp connVM = null);

       string[] UpdateUOM(UOMConversionVM vm, SysDBInfoVMTemp connVM = null);
   
   }
}
