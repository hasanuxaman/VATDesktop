using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo.AdjustmentWCF;

namespace VATDesktop.Repo
{
    public class AdjustmentRepo : IAdjustment
    {
       //AdjustmentDAL _dal = new AdjustmentDAL();

       AdjustmentWCFClient wcf = new AdjustmentWCFClient();
       

        public string[] DeleteAdjustmentName(string AdjId, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.DeleteAdjustmentName(AdjId, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);

               return results;
           }
           catch (Exception e)
           {
               throw e;
           }
       }
        public List<AdjustmentVM> DropDown( SysDBInfoVMTemp connVM=null)
       {

           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.DropDown(connVMwcf);

               List<AdjustmentVM> results = JsonConvert.DeserializeObject<List<AdjustmentVM>>(result);

               return results;
           }
           catch (Exception e)
           {
               throw e;
           }

       }
        public string[] InsertAdjustmentName(AdjustmentVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string VMwcf = JsonConvert.SerializeObject(vm);


                string result = wcf.InsertAdjustmentName(VMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateAdjustmentName(AdjustmentVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string VMwcf = JsonConvert.SerializeObject(vm);


                string result = wcf.UpdateAdjustmentName(VMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<AdjustmentVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);
                List<AdjustmentVM> results = JsonConvert.DeserializeObject<List<AdjustmentVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                

                string result = wcf.SelectAll( Id,conditionFieldswcf,conditionValueswcf , Dt, connVMwcf);

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
