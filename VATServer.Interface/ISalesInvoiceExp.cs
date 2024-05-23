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
   public interface ISalesInvoiceExp
    {
         string[] Delete(SalesInvoiceExpVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] InsertToSalesInvoiceExp(SalesInvoiceExpVM vm, SysDBInfoVMTemp connVM = null);

         DataTable SelectAll(int ID = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

         List<SalesInvoiceExpVM> SelectAllList(int Id, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] UpdateSalesInvoiceExps(SalesInvoiceExpVM vm, SysDBInfoVMTemp connVM = null);

    }
}
