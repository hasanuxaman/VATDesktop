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
    public interface ICurrencyConversion
    {
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);
        


        string[] Delete(CurrencyConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] InsertToCurrencyConversion(CurrencyConversionVM vm, SysDBInfoVMTemp connVM = null);
        DataTable SearchCurrencyConversionNew(string CurrencyCodeF, string CurrencyNameF, string CurrencyCodeT, string CurrencyNameT, string ActiveStatus, string convDate, SysDBInfoVMTemp connVM = null);
        List<CurrencyConversionVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateCurrencyConversion(CurrencyConversionVM vm, SysDBInfoVMTemp connVM = null);
    

    }
}
