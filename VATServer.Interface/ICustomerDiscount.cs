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
   public interface ICustomerDiscount
    {
        string[] Delete(CustomerDiscountVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] InsertToCustomerDiscountNew(CustomerDiscountVM vm, bool BranchProfileAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

        List<CustomerDiscountVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] UpdateToCustomerDiscountNew(CustomerDiscountVM vm, SysDBInfoVMTemp connVM = null);

        DataTable SearchCustomerDiscount(string CustomerID, string Id, string address, SysDBInfoVMTemp connVM = null);

        string[] DeleteCustomerDiscount(string CustomerID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);


    }
}
