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
    public interface ICurrencies
    {

        string[] Delete(CurrencyVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        List<CurrencyVM> DropDown(SysDBInfoVMTemp connVM = null);
        string[] InsertToCurrency(CurrencyVM vm, SysDBInfoVMTemp connVM = null);
        DataTable SearchCurrency(string customer, SysDBInfoVMTemp connVM = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);
        List<CurrencyVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateCurrency(CurrencyVM vm, SysDBInfoVMTemp connVM = null);
    


    }
}
