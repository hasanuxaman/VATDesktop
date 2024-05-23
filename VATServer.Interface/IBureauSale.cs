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
    public interface IBureauSale
    {

        string[] CurrencyInfo(string salesInvoiceNo, SysDBInfoVMTemp connVM = null);

        string[] ImportInspectionData(DataTable dtSaleM, string noOfSale, SysDBInfoVMTemp connVM = null);

        string[] InsertCreditInfo(DataTable dtCredit, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string[] SalesInsert(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] SalesInsertImport(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] SalesPost(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SysDBInfoVMTemp connVM = null);

        string[] SalesUpdate(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SysDBInfoVMTemp connVM = null);

        string[] UpdatePrintNew(string InvoiceNo, int PrintCopy, SysDBInfoVMTemp connVM = null);

        string GetCategoryName(string itemNo, SysDBInfoVMTemp connVM = null);

        DataTable GetProductInfo(SysDBInfoVMTemp connVM = null);

        DataTable SearchSaleDetailDTNew(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null);

        DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName, SysDBInfoVMTemp connVM = null);

        DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string CustomerName, string CustomerGroupName, string VehicleType, string VehicleNo, string SerialNo, string InvoiceDateFrom, string InvoiceDateTo, string SaleType, string Trading, string IsPrint, string transactionType, string Post, SysDBInfoVMTemp connVM = null);

        int LoadIssueItems(SysDBInfoVMTemp connVM = null, string UserId = "");

        string SearchType(string buId, SysDBInfoVMTemp connVM = null);

        string UpdateCreditNo(string challanDate, SysDBInfoVMTemp connVM = null);

        decimal ReturnSaleQty(string saleReturnId, string itemNo, SysDBInfoVMTemp connVM = null);

    }
}
