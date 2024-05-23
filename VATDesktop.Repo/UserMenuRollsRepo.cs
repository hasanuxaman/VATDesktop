using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.UserMenuRollsWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class UserMenuRollsRepo : IUserMenuRolls
    {
       UserMenuRollsWCFClient wcf = new UserMenuRollsWCFClient();
       public string UserMenuRollsInsert(string FormID, string UserID, string FormName, string Access, string PostAccess, string AddAccess, string EditAccess, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string name = wcf.UserMenuRollsInsert(FormID,UserID,FormName,Access,PostAccess,AddAccess,EditAccess, connVMwcf);


               return name;
           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] UserMenuRollsInsertByUser(string UserID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string connVMWCF = JsonConvert.SerializeObject(connVM);

               string result = wcf.UserMenuRollsInsertByUser(UserID, connVMWCF);

               return JsonConvert.DeserializeObject<string[]>(result);
           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable UserMenuRollsSelectAll(string UserID, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string table = wcf.UserMenuRollsSelectAll(UserID, conditionFieldswcf, conditionValueswcf, Dt, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(table);


               return results;
           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] UserMenuRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string dataWCF = JsonConvert.SerializeObject(dt);
               string connVMWCF = JsonConvert.SerializeObject(connVM);

               string result = wcf.UserMenuRollsUpdate(dataWCF, connVMWCF);

               return JsonConvert.DeserializeObject<string[]>(result);
           }
           catch (Exception e)
           {
               throw e;
           }
       }
    }
}
