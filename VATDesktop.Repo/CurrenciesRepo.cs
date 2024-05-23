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
using VATDesktop.Repo.CurrenciesWCF;

namespace VATDesktop.Repo
{
    public class CurrenciesRepo : ICurrencies
    {
        CurrenciesWCFClient wcf = new CurrenciesWCFClient();

        public string[] Delete(CurrencyVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
        public List<CurrencyVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(connVMwcf);
                List<CurrencyVM> results = JsonConvert.DeserializeObject<List<CurrencyVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToCurrency(CurrencyVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToCurrency(vmwcf, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchCurrency(string customer, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchCurrency(customer, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, Dt, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<CurrencyVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf,connVMwcf);
                List<CurrencyVM> results = JsonConvert.DeserializeObject<List<CurrencyVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public string[] UpdateCurrency(CurrencyVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateCurrency(vmwcf, connVMwcf);
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
