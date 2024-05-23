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
    public interface ICustomerGroup
    {

        string[] InsertToCustomerGroupNew(CustomerGroupVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);
        string[] Delete(CustomerGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateToCustomerGroupNew(CustomerGroupVM vm, SysDBInfoVMTemp connVM = null);

        List<CustomerGroupVM> DropDown(SysDBInfoVMTemp connVM = null);
        List<CustomerGroupVM> DropDownAll(SysDBInfoVMTemp connVM = null);
        List<CustomerGroupVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);




    }
}
