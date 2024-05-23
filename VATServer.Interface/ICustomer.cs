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
    public interface ICustomer
    {

        List<CustomerVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);


        List<CustomerVM> DropDown(SysDBInfoVMTemp connVM = null);

        string[] Delete(CustomerVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] DeleteCustomerAddress(string CustomerID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] InsertToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] InsertToCustomerNew(CustomerVM vm, bool CustomerAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateToCustomerNew(CustomerVM vm, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelAddress(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        DataTable SearchCountry(string customer, SysDBInfoVMTemp connVM = null);
        DataTable SearchCustomerAddress(string CustomerID, string Id, string address, SysDBInfoVMTemp connVM = null);
        DataTable SearchCustomerByCode(string customerCode, SysDBInfoVMTemp connVM = null);
        DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);
        
    }
}
