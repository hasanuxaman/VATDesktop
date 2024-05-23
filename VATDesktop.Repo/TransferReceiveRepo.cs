using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.TransferReceiveWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class TransferReceiveRepo : ITransferReceive
    {
       TransferReceiveWCFClient wcf = new TransferReceiveWCFClient();

        public DataSet FormLoad(UOMVM UOMvm, ProductVM Productvm, string Name, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string UOMvmwcf = JsonConvert.SerializeObject(UOMvm);
               string Productvmwcf = JsonConvert.SerializeObject(Productvm);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.FormLoad(UOMvmwcf, Productvmwcf, Name, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
        public string[] Insert(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null, bool CodeGenaration = true)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Insert(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Idswcf = JsonConvert.SerializeObject(Ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.MultiplePost(Idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] Post(TransferReceiveVM Master, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Post(Masterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchTransferDetail(string TransferReceiveNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchTransferDetail(TransferReceiveNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchTransferReceive(TransferReceiveVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchTransferReceive(vmwcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
        public List<TransferReceiveVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<TransferReceiveVM> results = JsonConvert.DeserializeObject<List<TransferReceiveVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<TransferReceiveDetailVM> SelectDetail(string TransferReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectDetail(TransferReceiveNo, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<TransferReceiveDetailVM> results = JsonConvert.DeserializeObject<List<TransferReceiveDetailVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] Update(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Update(Masterwcf,Detailswcf, connVMwcf);

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
