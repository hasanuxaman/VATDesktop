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
   public interface IToll6_3Invoice
    {
       string[] Insert(Toll6_3InvoiceVM MasterVM, List<Toll6_3InvoiceDetailVM> DetailVMs, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] InsertDetailList(List<Toll6_3InvoiceDetailVM> DetailVMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] Post(Toll6_3InvoiceVM MasterVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string TransactionType = "");

        List<Toll6_3InvoiceVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string TransactionType = "");

        List<Toll6_3InvoiceDetailVM> SelectDetail(string TollNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable TollSearch(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

        string[] Update(Toll6_3InvoiceVM MasterVM, List<Toll6_3InvoiceDetailVM> DetailVMs, SysDBInfoVMTemp connVM = null);

        string[] UpdateTollCompleted(string flag, string tollNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
