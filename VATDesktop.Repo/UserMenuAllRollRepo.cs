using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.UserMenuAllRollWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{

    public class UserMenuAllRollRepo : IUserMenuAllRoll
    {
     UserMenuAllRollWCFClient wcf = new UserMenuAllRollWCFClient();

        public DataTable UserMenuAllRollsSelectAll(string FormID = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            
           try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string table = wcf.UserMenuAllRollsSelectAll(FormID, conditionFieldswcf, conditionValueswcf, Dt,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void UserMenuAllRollsSettingChange(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                wcf.UserMenuAllRollsSettingChange(connVMwcf);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UserMenuAllRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dataWCF = JsonConvert.SerializeObject(dt);
                string connVMWCF = JsonConvert.SerializeObject(connVM);

                string result = wcf.UserMenuAllRollsUpdate(dataWCF,connVMWCF);

                return JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UserMenuAllFinalRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            string result = "";
            try
            {
                string dataWCF = JsonConvert.SerializeObject(dt);
                string connVMWCF = JsonConvert.SerializeObject(connVM);

                  //result = wcf.UserMenuAllFinalRollsUpdate(dataWCF, connVMWCF);

                return JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
   
    }
}
