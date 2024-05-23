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
using VATDesktop.Repo.PrefixWCF;

namespace VATDesktop.Repo
{
    public class PrefixRepo : IPrefix
    {

        PrefixWCFClient wcf = new PrefixWCFClient();


        public List<CodeVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownAll( connVMwcf);

                List<CodeVM> results = JsonConvert.DeserializeObject<List<CodeVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] PrefixDataUpdate(CodeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.PrefixDataUpdate(vmwcf, connVMwcf);

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

                string result = wcf.SelectAll(Id, conditionFieldswcf , conditionValueswcf ,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<CodeVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);

                string result = wcf.SelectAllList(Id,conditionFieldswcf, conditionValueswcf);

                List<CodeVM> results = JsonConvert.DeserializeObject<List<CodeVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
