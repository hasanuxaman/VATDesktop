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
    public interface IMPLSalesInvSA4
    {

        string[] MPLSalesInvSA4Insert(MPLSaleInvoiceSA4VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] MPLSalesInvSA4Update(MPLSaleInvoiceSA4VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string ActiveStatus = "", string SelectTop = "100");

        List<MPLSaleInvoiceSA4VM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string ActiveStatus = "", string SelectTop = "100");

        List<MPLSaleInvoiceSA4VM> DropDown(int branchId, string ItemNo, SysDBInfoVMTemp connVM = null);

        string[] Delete(MPLSaleInvoiceSA4VM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] SaleInvoiceSA4Post(MPLSaleInvoiceSA4VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

    }
}
