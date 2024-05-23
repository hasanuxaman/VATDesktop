using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.TenderWCF;
using System.Data.SqlClient;

namespace VATDesktop.Repo
{
    public class TenderRepo : ITender
    {
        TenderWCFClient wcf = new TenderWCFClient();

        public DataTable SearchTenderDetailSaleNew(string TenderId, string transactiondate, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchTenderDetailSaleNew(TenderId, transactiondate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SearchTenderDetailSale(string TenderId, string transactiondate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchTenderDetailSale(TenderId, transactiondate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string[] ImportData(DataTable dtTenderM, DataTable dtTenderD, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtTenderMwcf = JsonConvert.SerializeObject(dtTenderM);
                string dtTenderDwcf = JsonConvert.SerializeObject(dtTenderD);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ImportData(dtTenderMwcf, dtTenderDwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchTenderDetail(string TenderId, string transactionDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchTenderDetail(TenderId, transactionDate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchTenderHeaderByCustomerGrp(string TenderId, string RefNo, string CustomerGrpID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchTenderHeaderByCustomerGrp(TenderId, RefNo,CustomerGrpID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

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
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<TenderDetailVM> SelectAllDetails(string tenderId = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAllDetails(tenderId, conditionFieldswcf, conditionValueswcf,connVMwcf);

                List<TenderDetailVM> results = JsonConvert.DeserializeObject<List<TenderDetailVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<TenderMasterVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<TenderMasterVM> results = JsonConvert.DeserializeObject<List<TenderMasterVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] TenderInsert(TenderMasterVM Master, List<TenderDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.TenderInsert(Masterwcf, Detailswcf,connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] TenderUpdate(TenderMasterVM Master, List<TenderDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.TenderUpdate(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
