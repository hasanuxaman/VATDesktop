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
using VATDesktop.Repo.DemandWCF;
using Newtonsoft.Json;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class DemandRepo : IDemand
    {
        //DemandDAL _dal = new DemandDAL();

        DemandWCFClient wcf = new DemandWCFClient();

        public decimal ReturnQty(string demandId, string bandeProId, string post, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ReturnQty(demandId, bandeProId, post, connVMwcf);

                decimal results =Convert.ToDecimal(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal BanderolStock(string itemNo, string BandProductID, string tranDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.BanderolStock(itemNo, BandProductID, tranDate, connVMwcf);

                decimal results = Convert.ToDecimal(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DemandInsert(DemandMasterVM Master, List<DemandDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DemandInsert(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DemandPost(DemandMasterVM Master, List<DemandDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DemandPost(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DemandUpdate(DemandMasterVM Master, List<DemandDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DemandUpdate(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchDemandDetailDTNew(string DemandNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchDemandDetailDTNew(DemandNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchDemandHeaderDTNew(string DemandNo, string DemandDateFrom, string DemandDateTo, string TransactionType, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchDemandHeaderDTNew(DemandNo,DemandDateFrom,DemandDateTo,TransactionType,Post,BranchId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable DemandQty(string demandId, string bandeProId, string post, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DemandQty(demandId,bandeProId,post, connVMwcf);

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
