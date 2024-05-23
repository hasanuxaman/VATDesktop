using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.ProductCategoryWCF;

namespace VATDesktop.Repo
{
    public class ProductCategoryRepo : IProductCategory
    {
        ProductCategoryWCFClient wcf = new ProductCategoryWCFClient();


        //public void AddValueToAdapter(SqlDataAdapter adapter, string[] conditionValues, string[] conditionFields);
        
        public List<ProductCategoryVM> DropDown(string IsRaw, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(IsRaw, connVMwcf);

                List<ProductCategoryVM> results = JsonConvert.DeserializeObject<List<ProductCategoryVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductCategoryVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownAll(connVMwcf);

                List<ProductCategoryVM> results = JsonConvert.DeserializeObject<List<ProductCategoryVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<IdName> DropDownProductType(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownProductType(connVMwcf);

                List<IdName> results = JsonConvert.DeserializeObject<List<IdName>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ProductCategoryVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf,connVMwcf);

                List<ProductCategoryVM> results = JsonConvert.DeserializeObject<List<ProductCategoryVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] Delete(ProductCategoryVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.Delete(vmwcf, idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateProductCategory(ProductCategoryVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateProductCategory(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] InsertToProductCategory(ProductCategoryVM vm, SysDBInfoVMTemp ConnVm = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string ConnVmwcf = JsonConvert.SerializeObject(ConnVm);

                string result = wcf.InsertToProductCategory(vmwcf, ConnVmwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, string DbName = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf , conditionValueswcf , Dt , DbName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    

    }
}
