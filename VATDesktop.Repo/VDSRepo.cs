using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.VDSWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class VDSRepo : IVDS
    {

       VDSWCFClient wcf = new VDSWCFClient();

       public List<VDSMasterVM> SelectVDSDetail(string VDSId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string TransactionType = "")
       {
           try
           {
               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SelectVDSDetail(VDSId, conditionFieldswcf, conditionValueswcf, connVMwcf);

               List<VDSMasterVM> results = JsonConvert.DeserializeObject<List<VDSMasterVM>>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }


       public string[] VDSMailSendUpdate(VDSMasterVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
       {
           throw new NotImplementedException();
       }
    }
}
