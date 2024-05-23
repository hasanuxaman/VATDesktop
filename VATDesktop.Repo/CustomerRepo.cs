using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.CustomerWCF;
using System.Data;

namespace VATDesktop.Repo
{
    public class CustomerRepo : ICustomer
    {

        CustomerWCFClient wcf = new CustomerWCFClient();


        public List<CustomerVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);
                List<CustomerVM> results = JsonConvert.DeserializeObject<List<CustomerVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }

        }
        

        public string[] Delete(CustomerVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.Delete(vmwcf, idswcf,connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DeleteCustomerAddress(string CustomerID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeleteCustomerAddress(CustomerID, Id, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<CustomerVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(connVMwcf);
                List<CustomerVM> results = JsonConvert.DeserializeObject<List<CustomerVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelAddress(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExcelAddress(idswcf, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetExcelData(idswcf, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToCustomerAddress(vmwcf, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToCustomerNew(CustomerVM vm, bool CustomerAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToCustomerNew(vmwcf,CustomerAutoSave, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchCountry(string customer, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchCountry(customer, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchCustomerAddress(string CustomerID, string Id, string address, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchCustomerAddress(CustomerID, Id, address,connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchCustomerByCode(string customerCode, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchCustomerByCode(customerCode, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf,Dt, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateToCustomerAddress(vmwcf, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToCustomerNew(CustomerVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateToCustomerNew(vmwcf, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
    }
}
