using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATDesktop.Repo.SaleExportWCF;
using Newtonsoft.Json;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class SaleExportRepo : ISaleExport
    {
        SaleExportWCFClient wcf = new SaleExportWCFClient();

        public string[] SaleExportInsert(SaleExportVM Master, List<SaleExportInvoiceVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Table = wcf.SaleExportInsert(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] SaleExportPost(SaleExportVM Master, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Table = wcf.SaleExportPost(Masterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] SaleExportUpdate(SaleExportVM Master, List<SaleExportInvoiceVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Table = wcf.SaleExportUpdate(Masterwcf,Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(Table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchSaleExportDetailDTNew(string SaleExportNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Table = wcf.SearchSaleExportDetailDTNew(SaleExportNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(Table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchSaleExportDTNew(string SaleExportNo, string SaleExportDateFrom, string SaleExportDateTo, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string Table = wcf.SearchSaleExportDTNew(SaleExportNo, SaleExportDateFrom, SaleExportDateTo, Post,  BranchId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(Table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    


    }
}
