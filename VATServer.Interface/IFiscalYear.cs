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
    public interface IFiscalYear
    {

        string[] FiscalYearInsert(List<FiscalYearVM> Details, SysDBInfoVMTemp connVM = null);
        string[] FiscalYearUpDate(List<FiscalYearVM> Details, SysDBInfoVMTemp connVM = null);
        string[] FiscalYearUpdate(List<FiscalYearVM> Details, string modifiedBy, SysDBInfoVMTemp connVM = null);
        int LockChek(SysDBInfoVMTemp connVM = null);
        string MaxDate(SysDBInfoVMTemp connVM = null);
        DataTable SearchYear(SysDBInfoVMTemp connVM = null);
        DataTable LoadYear(string CurrentYear, SysDBInfoVMTemp connVM = null);
        List<FiscalYearVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        FiscalYearPeriodVM StartEndPeriod(string year, SysDBInfoVMTemp connVM = null);
        List<FiscalYearVM> DropDownAll(SysDBInfoVMTemp connVM = null);
    

    }
}
