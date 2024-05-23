using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.SalesInvoiceExpWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class SalesInvoiceExpRepo : ISalesInvoiceExp
    {
        SalesInvoiceExpWCFClient wcf = new SalesInvoiceExpWCFClient();

        public string[] Delete(SalesInvoiceExpVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
              try
            {
                string vmwcf =JsonConvert.SerializeObject(vm);
                string idswcf =JsonConvert.SerializeObject(ids);
                string connVMwcf =JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.Delete(vmwcf, idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(resultInsert);

                return results;
                

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToSalesInvoiceExp(SalesInvoiceExpVM vm, SysDBInfoVMTemp connVM = null)
        {
           try
            {
                string vmwcf =JsonConvert.SerializeObject(vm);
                string connVMwcf =JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.InsertToSalesInvoiceExp(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(resultInsert);

                return results;
                

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(int ID = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
           try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf =JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.SelectAll(ID,conditionFieldswcf,conditionValueswcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(resultInsert);

                return results;
                

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<SalesInvoiceExpVM> SelectAllList(int Id, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf,connVMwcf);

                List<SalesInvoiceExpVM> results = JsonConvert.DeserializeObject<List<SalesInvoiceExpVM>>(resultInsert);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateSalesInvoiceExps(SalesInvoiceExpVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.UpdateSalesInvoiceExps(vmwcf,connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(resultInsert);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
