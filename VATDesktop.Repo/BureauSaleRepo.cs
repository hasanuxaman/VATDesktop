using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATDesktop.Repo.BureauSaleWCF;
using VATServer.Interface;


namespace VATDesktop.Repo
{
    public class BureauSaleRepo : IBureauSale
    {
        BureauSaleWCFClient wcf = new BureauSaleWCFClient();

        public string[] CurrencyInfo(string salesInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CurrencyInfo(salesInvoiceNo, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetCategoryName(string itemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetCategoryName(itemNo, connVMwcf);

                return result;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetProductInfo(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.GetProductInfo(connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] ImportInspectionData(DataTable dtSaleM, string noOfSale, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtSaleMwcf = JsonConvert.SerializeObject(dtSaleM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.ImportInspectionData(dtSaleMwcf,noOfSale, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] InsertCreditInfo(DataTable dtCredit, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string dtCreditwcf = JsonConvert.SerializeObject(dtCredit);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertCreditInfo(dtCreditwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int LoadIssueItems(SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.LoadIssueItems(connVMwcf);

                int results = Convert.ToInt32(result);

                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public decimal ReturnSaleQty(string saleReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ReturnSaleQty(saleReturnId, itemNo, connVMwcf);

                decimal results = Convert.ToInt32(result);

                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesInsert(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SalesInsert(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesInsertImport(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string ExportDetailswcf = JsonConvert.SerializeObject(ExportDetails);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SalesInsertImport(Masterwcf, Detailswcf,ExportDetailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesPost(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SalesPost(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SalesUpdate(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SalesUpdate(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchSaleDetailDTNew(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchSaleDetailDTNew(SalesInvoiceNo, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchSaleExportDTNew(SalesInvoiceNo, databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string CustomerName, string CustomerGroupName, string VehicleType, string VehicleNo, string SerialNo, string InvoiceDateFrom, string InvoiceDateTo, string SaleType, string Trading, string IsPrint, string transactionType, string Post, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchSalesHeaderDTNew(SalesInvoiceNo, CustomerName, CustomerGroupName, VehicleType, VehicleNo, SerialNo, InvoiceDateFrom, InvoiceDateTo, SaleType, Trading, IsPrint, transactionType, Post, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string SearchType(string buId, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string result = wcf.SearchType(buId);

                return result;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string UpdateCreditNo(string challanDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateCreditNo(challanDate, connVMwcf);

                return result;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdatePrintNew(string InvoiceNo, int PrintCopy, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdatePrintNew(InvoiceNo, PrintCopy, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
