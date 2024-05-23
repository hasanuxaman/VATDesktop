using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.CommonImportWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class CommonImportRepo : ICommonImport
    {
        CommonImportWCFClient wcf = new CommonImportWCFClient();


        public string FindVendorId(string vendorName, string vendorCode,  SqlConnection currConn = null, SqlTransaction transaction = null
            , bool AutoSave = false, SysDBInfoVMTemp connVM = null,int branchId = 1)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindVendorId(vendorName, vendorCode, AutoSave, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindItemId(string productName, string productCode, SqlConnection currConn, SqlTransaction transaction
            , bool AutoSave = false, string uom = "-", int branchId = 1, decimal vatRate = 0, decimal NbrPrice = 0
            , SysDBInfoVMTemp connVM = null, string Product_Group = "-", bool ProductNameCheck = false)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindItemId(productName, productCode, AutoSave , uom , branchId, vatRate , NbrPrice, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindUOMn(string ItemNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindUOMn(ItemNo, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindUOMc(string uomFrom, string uomTo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null, string itemNo = "", string productName = "", string productCode = "")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindUOMc(uomFrom, uomTo, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string CheckPrePurchaseNo(string PurchaseID, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CheckPrePurchaseNo(PurchaseID, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindCustomerId(string customerName, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindCustomerId( customerName, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetProductInfo(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.GetProductInfo( connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindCurrencyRateFromBDTForBureau(string currencyId, string convertionDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindCurrencyRateFromBDTForBureau( currencyId, convertionDate, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindSaleInvoiceNo(string customerId, string invoiceNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindSaleInvoiceNo( customerId, invoiceNo, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string CheckBankID(string BankName, string BranchName, string AccountNumber, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CheckBankID( BankName, BranchName, AccountNumber, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindBranchId(string branchCode, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindBranchId(branchCode, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindLastNBRPrice(string ItemNo, string VATName, string RequestDate, SqlConnection currConn
            , SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindLastNBRPrice( ItemNo,  VATName,  RequestDate, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindAvgPrice(string ItemNo, string RequestDate, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string result = wcf.FindAvgPrice( ItemNo,  RequestDate);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindCurrencyId(string currencyCode, SqlConnection currConn, SqlTransaction transaction
            , SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindCurrencyId(currencyCode, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string FindCurrencyRateFromBDT(string currencyId, SqlConnection currConn, SqlTransaction transaction
            , SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf. FindCurrencyRateFromBDT( currencyId, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindCurrencyRateBDTtoUSD(string currencyId, string convertionDate, SqlConnection currConn
            , SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindCurrencyRateBDTtoUSD( currencyId,  convertionDate, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string CheckReceiveReturnID(string ReturnID, SqlConnection currConn, SqlTransaction transaction
            , SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CheckReceiveReturnID(ReturnID, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string CheckIssueReturnID(string ReturnID, SqlConnection currConn, SqlTransaction transaction
            , SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CheckIssueReturnID(ReturnID, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable FindAvgPriceImport(string itemNo, string tranDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindAvgPriceImport( itemNo, tranDate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal FindLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FindLastNBRPriceFromBOM( itemNo, VatName, effectDate, connVMwcf);

                decimal results = Convert.ToDecimal(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindCustGroupID(string custGrpName, string custGrpID, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.FindCustGroupID( custGrpName, custGrpID, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string CheckSaleImportIdExist(string ImportId, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CheckSaleImportIdExist(ImportId, connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
