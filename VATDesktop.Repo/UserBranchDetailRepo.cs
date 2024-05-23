using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.UserBranchDetailWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class UserBranchDetailRepo : IUserBranchDetail
    {
       UserBranchDetailWCFClient wcf = new UserBranchDetailWCFClient();

        public string[] Insert(List<UserBranchDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string Detailswcf = JsonConvert.SerializeObject(Details);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Insert(Detailswcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
        public string[] InsertfromExcel(DataTable datatable, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string CreatedBy = null, string Createdon = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string datatablewcf = JsonConvert.SerializeObject(datatable);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertfromExcel(datatablewcf,CreatedBy,Createdon, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
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


                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<UserBranchDetailVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllLst(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<UserBranchDetailVM> results = JsonConvert.DeserializeObject<List<UserBranchDetailVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
