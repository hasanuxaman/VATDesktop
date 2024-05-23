using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATDesktop.Repo.DepositTDSWCF;
using Newtonsoft.Json;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class DepositTDSRepo : IDepositTDS
    {
        DepositTDSWCFClient wcf = new DepositTDSWCFClient();


        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> tds, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string tdswcf = JsonConvert.SerializeObject(tds);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsert(Masterwcf, tdswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositInsert2(DepositMasterVM Master, List<VDSMasterVM> tds, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string tdswcf = JsonConvert.SerializeObject(tds);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsert2(Masterwcf, tdswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositInsertX(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string tdswcf = JsonConvert.SerializeObject(tds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsertX(Masterwcf, tdswcf,adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string tdswcf = JsonConvert.SerializeObject(tds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositPost(Masterwcf, tdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositPostX(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string tdswcf = JsonConvert.SerializeObject(tds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositPostX(Masterwcf, tdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> tds, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string tdswcf = JsonConvert.SerializeObject(tds);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositUpdate(Masterwcf, tdswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositUpdateX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositUpdateX(Masterwcf, vdswcf, adjHistorywcf,connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] ImportData(DataTable dtDeposit, DataTable dtVDS, int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtDepositwcf = JsonConvert.SerializeObject(dtDeposit);
                string dtVDSwcf = JsonConvert.SerializeObject(dtVDS);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ImportData(dtDepositwcf, dtVDSwcf,branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.UpdateVdsComplete(flag, VdsId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal ReverseAmount(string reverseDepositId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ReverseAmount(reverseDepositId, connVMwcf);

                decimal results = Convert.ToDecimal(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchVDSDT(string DepositNumber, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchVDSDT(DepositNumber, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

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

                string table = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, Dt,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DepositMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<DepositMasterVM> results = JsonConvert.DeserializeObject<List<DepositMasterVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    
    }
}
