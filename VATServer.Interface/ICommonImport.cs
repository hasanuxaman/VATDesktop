using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface ICommonImport
    {
        string FindVendorId(string vendorName, string vendorCode,  SqlConnection currConn, SqlTransaction transaction
            , bool AutoSave = false, SysDBInfoVMTemp connVM = null, int branchId = 1);

        string FindItemId(string productName, string productCode, SqlConnection currConn, SqlTransaction transaction
            , bool AutoSave = false, string uom = "-", int branchId = 1, decimal vatRate = 0, decimal NbrPrice = 0
            , SysDBInfoVMTemp connVM = null, string Product_Group = "-", bool ProductNameCheck = false);

        string FindUOMn(string ItemNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindUOMc(string uomFrom, string uomTo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null, string itemNo = "", string productName = "", string productCode = "");

        string CheckPrePurchaseNo(string PurchaseID, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindCustomerId(string customerName, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        DataTable GetProductInfo(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindCurrencyRateFromBDTForBureau(string currencyId, string convertionDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindSaleInvoiceNo(string customerId, string invoiceNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string CheckBankID(string BankName, string BranchName, string AccountNumber, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindBranchId(string branchCode, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindLastNBRPrice(string ItemNo, string VATName, string RequestDate, SqlConnection currConn
            , SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindAvgPrice(string ItemNo, string RequestDate, string UserId = "", SysDBInfoVMTemp connVM = null);

        string FindCurrencyId(string currencyCode, SqlConnection currConn, SqlTransaction transaction
            , SysDBInfoVMTemp connVM = null);

       string FindCurrencyRateFromBDT(string currencyId, SqlConnection currConn, SqlTransaction transaction
            , SysDBInfoVMTemp connVM = null);

       string FindCurrencyRateBDTtoUSD(string currencyId, string convertionDate, SqlConnection currConn
            , SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

      string CheckReceiveReturnID(string ReturnID, SqlConnection currConn, SqlTransaction transaction
        , SysDBInfoVMTemp connVM = null);

      string CheckIssueReturnID(string ReturnID, SqlConnection currConn, SqlTransaction transaction
          , SysDBInfoVMTemp connVM = null);

        DataTable FindAvgPriceImport(string itemNo, string tranDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        decimal FindLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string FindCustGroupID(string custGrpName, string custGrpID, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string CheckSaleImportIdExist(string ImportId, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);


    }
}
