using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.SDDepositWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class SDDepositRepo : ISDDeposit
    {
        SDDepositWCFClient wcf = new SDDepositWCFClient();

        public string[] DepositInsert(SDDepositVM Master, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsert(Masterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DepositPost(SDDepositVM Master, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositPost(Masterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DepositUpdate(SDDepositVM Master, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositUpdate(Masterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

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

                string table = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<SDDepositVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf,connVMwcf);

                List<SDDepositVM> results = JsonConvert.DeserializeObject<List<SDDepositVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
