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
    public interface IProductCategory
    {


        //void AddValueToAdapter(SqlDataAdapter adapter, string[] conditionValues, string[] conditionFields);
        string[] Delete(ProductCategoryVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        List<ProductCategoryVM> DropDown(string IsRaw, SysDBInfoVMTemp connVM = null);
        List<ProductCategoryVM> DropDownAll(SysDBInfoVMTemp connVM = null);
        List<IdName> DropDownProductType(SysDBInfoVMTemp connVM = null);
        List<ProductCategoryVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
        string[] UpdateProductCategory(ProductCategoryVM vm, SysDBInfoVMTemp connVM = null);
        string[] InsertToProductCategory(ProductCategoryVM vm, SysDBInfoVMTemp ConnVm = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, string DbName = "", SysDBInfoVMTemp connVM = null);
    



    }
}
