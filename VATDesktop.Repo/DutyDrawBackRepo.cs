using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.DutyDrawBackWCF;
using Newtonsoft.Json;


namespace VATDesktop.Repo
{
    public class DutyDrawBackRepo : IDutyDrawBack
    {

        DutyDrawBackWCFClient wcf = new DutyDrawBackWCFClient();


        public string[] DutyDrawBacknsert(DDBHeaderVM Master, List<DDBDetailsVM> Details, List<DDBSaleInvoicesVM> ddbSaleInvoices, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string ddbSaleInvoiceswcf = JsonConvert.SerializeObject(ddbSaleInvoices);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DutyDrawBacknsert(Masterwcf, Detailswcf, ddbSaleInvoiceswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DutyDrawBackPost(DDBHeaderVM Master, List<DDBDetailsVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DutyDrawBackPost(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DutyDrawBackUpdate(DDBHeaderVM Master, List<DDBDetailsVM> Details, List<DDBSaleInvoicesVM> ddbSaleInvoices, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string ddbSaleInvoiceswcf = JsonConvert.SerializeObject(ddbSaleInvoices);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DutyDrawBackUpdate(Masterwcf, Detailswcf, ddbSaleInvoiceswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable Purchase_DDBQty(string PurchaseInvoiceNo, string PurItemNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.Purchase_DDBQty(PurchaseInvoiceNo, PurItemNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchddBackDetails(string DDBackNo, string oldSaleID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchddBackDetails(DDBackNo, oldSaleID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchDDBackHeader(string DDBackNo, string DDBackFromDate, string DDBackToDate, string DDBackSaleFromDate, string DDBackSaleToDate, string SalesInvoicNo, string CustomerName, string FinishGood, string Post, string BranchId = "0", string TransactionType = "DDB", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchDDBackHeader(DDBackNo, DDBackFromDate, DDBackToDate, DDBackSaleFromDate,DDBackSaleToDate, SalesInvoicNo, CustomerName, FinishGood, Post, BranchId , TransactionType,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchddbSaleInvoices(string DDBackNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchddbSaleInvoices(DDBackNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable VAT7_1(string ddbackno, string salesInvoice, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.VAT7_1(ddbackno, salesInvoice, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
