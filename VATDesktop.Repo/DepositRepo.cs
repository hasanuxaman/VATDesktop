using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.DepositWCF;
using Newtonsoft.Json;

namespace VATDesktop.Repo
{
    public class DepositRepo : IDeposit
    {
        DepositWCFClient wcf = new DepositWCFClient();

        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsert(Masterwcf, vdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositInsert2(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsert2(Masterwcf, vdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositInsertX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositInsertX(Masterwcf, vdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositPost(Masterwcf, vdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositPostX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositPostX(Masterwcf, vdswcf, adjHistorywcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string vdswcf = JsonConvert.SerializeObject(vds);
                string adjHistorywcf = JsonConvert.SerializeObject(adjHistory);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DepositUpdate(Masterwcf, vdswcf, adjHistorywcf, connVMwcf);

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
        public DataTable SearchVDSDT(string DepositNumber, SysDBInfoVMTemp connVM = null, bool chkPurchaseVDS = false)
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
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string transactionOpening = "", string CashPayableSearch = "")
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
        public List<DepositMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string transactionOpening = "")
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
        public string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null, string TransactionType ="")
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

        public string[] ImportDataSingle(DataTable dtDeposit, DataTable dtVDS, int branchId = 1, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtDepositwcf = JsonConvert.SerializeObject(dtDeposit);
                string dtVDSwcf = JsonConvert.SerializeObject(dtVDS);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ImportDataSingle(dtDepositwcf, dtVDSwcf, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SaveTempVDS(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string datawcf = JsonConvert.SerializeObject(data);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SaveTempVDS(datawcf, BranchCode, transactionType, CurrentUser, branchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.GetExcelData(invoiceListwcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



    

    }
}
