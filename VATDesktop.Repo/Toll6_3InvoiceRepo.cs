using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.Toll6_3InvoiceWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class Toll6_3InvoiceRepo : IToll6_3Invoice
    {
       Toll6_3InvoiceWCFClient wcf = new Toll6_3InvoiceWCFClient();


        public string[] Insert(Toll6_3InvoiceVM MasterVM, List<Toll6_3InvoiceDetailVM> DetailVMs, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string MasterVMwcf = JsonConvert.SerializeObject(MasterVM);
               string DetailVMswcf = JsonConvert.SerializeObject(DetailVMs);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Insert(MasterVMwcf,DetailVMswcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public string[] InsertDetailList(List<Toll6_3InvoiceDetailVM> DetailVMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string DetailVMswcf = JsonConvert.SerializeObject(DetailVMs);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertDetailList( DetailVMswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] Post(Toll6_3InvoiceVM MasterVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string MasterVMwcf = JsonConvert.SerializeObject(MasterVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Post(MasterVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string TransactionType = "")
        {
            try
            {

                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAll(Id, conditionFieldswcf,conditionValueswcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Toll6_3InvoiceVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string TransactionType = "")
        {

            try
            {

                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<Toll6_3InvoiceVM> results = JsonConvert.DeserializeObject<List<Toll6_3InvoiceVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<Toll6_3InvoiceDetailVM> SelectDetail(string TollNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectDetail(TollNo, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<Toll6_3InvoiceDetailVM> results = JsonConvert.DeserializeObject<List<Toll6_3InvoiceDetailVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable TollSearch(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.TollSearch(Id, conditionFieldswcf, conditionValueswcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] Update(Toll6_3InvoiceVM MasterVM, List<Toll6_3InvoiceDetailVM> DetailVMs, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string MasterVMwcf = JsonConvert.SerializeObject(MasterVM);
                string DetailVMswcf = JsonConvert.SerializeObject(DetailVMs);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Update(MasterVMwcf, DetailVMswcf,connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateTollCompleted(string flag, string tollNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateTollCompleted(flag, tollNo, connVMwcf);

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
