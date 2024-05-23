using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo.AdjustmentHistoryWCF;
using Newtonsoft.Json;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class AdjustmentHistoryRepo : IAdjustmentHistory
    {
       //AdjustmentHistoryDAL _dal = new AdjustmentHistoryDAL();

       AdjustmentHistoryWCFClient wcf = new AdjustmentHistoryWCFClient();


       public string[] InsertAdjustmentHistory(AdjustmentHistoryVM vm, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertAdjustmentHistory(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }

            catch (Exception e)
            {
                throw e;
            }
               
        }

       public string[] UpdateAdjustmentHistory(AdjustmentHistoryVM vm, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string vmwcf = JsonConvert.SerializeObject(vm);
               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string result = wcf.UpdateAdjustmentHistory(vmwcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);

               return results;
           }

           catch (Exception e)
           {
               throw e;
           }

       }

       public string[] PostAdjustmentHistory(string AdjHistoryID, string AdjId, string AdjDate, decimal AdjAmount, decimal AdjVATRate, decimal AdjVATAmount, decimal AdjSD,
        decimal AdjSDAmount, decimal AdjOtherAmount, string AdjType, string AdjDescription, string CreatedBy,
        string CreatedOn, string LastModifiedBy, string LastModifiedOn
        , decimal AdjInputAmount, decimal AdjInputPercent, string AdjReferance, string Post, string AdjHistoryNo, SysDBInfoVMTemp connVM = null)
       {
          
           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);

             string  result = wcf.PostAdjustmentHistory(AdjHistoryID, AdjId, AdjDate, AdjAmount,  AdjVATRate, AdjVATAmount,  AdjSD,
         AdjSDAmount,  AdjOtherAmount,  AdjType,  AdjDescription,  CreatedBy,
         CreatedOn,  LastModifiedBy,  LastModifiedOn ,  AdjInputAmount,  AdjInputPercent,  AdjReferance,  Post,  AdjHistoryNo, connVMwcf );

             string[] results = JsonConvert.DeserializeObject<string[]>(result);


             return results;
           
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public DataTable SearchAdjustmentHistory(string AdjHistoryNo, string AdjReferance, string AdjType, string Post, 
           string AdjFromDate, string AdjToDate, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {

           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string result = wcf.SearchAdjustmentHistory( AdjHistoryNo, AdjReferance, AdjType, Post, AdjFromDate, AdjToDate, 
                   BranchId, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

               return results;
           }
           catch (Exception e)
           {
               throw e;
           }

       }

       public DataTable SearchAdjustmentHistoryForDeposit(string AdjHistoryNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string result = wcf.SearchAdjustmentHistoryForDeposit(AdjHistoryNo, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

               return results;
           }
           catch (Exception e)
           {
               throw e;
           }

       }

       public List<AdjustmentHistoryVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll( Id , conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<AdjustmentHistoryVM> results = JsonConvert.DeserializeObject<List<AdjustmentHistoryVM>>(result);

                return results;
            }

            catch (Exception e)
            {
                throw e;
            }
       }

       public List<AdjustmentHistoryVM> SelectAllCashPayable(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string result = wcf.SelectAllCashPayable(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

               List<AdjustmentHistoryVM> results = JsonConvert.DeserializeObject<List<AdjustmentHistoryVM>>(result);

               return results;
           }

           catch (Exception e)
           {
               throw e;
           }
       }


    }
}
