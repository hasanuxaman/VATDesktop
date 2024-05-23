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
    public interface IImport
    {
        DataTable GetSaleSQRDbDataSenora(string[] invoiceNo, DataTable db, string joinIds);
        DataTable GetSaleSQRDbData(string[] invoiceNo, DataTable db, string joinIds);
        DataTable GetSaleSQRDbDataSoap(string[] invoiceNo, DataTable db, string joinIds);
        DataTable GetSaleAduriDbData(string[] invoiceNo, DataTable db, string joinIds);


        DataSet GetDataSetFromExcel(ImportVM paramVM);
        DataTable GetDataTableFromExcel(ImportVM paramVM);
        DataSet GetDataTableFromExcelds(ImportVM paramVM);
        DataTable GetFGSQRDbData(string[] invoiceNo, DataTable db, string joinIds);
        DataTable GetFGSQRDbDataSenora(string[] invoiceNo, DataTable db, string joinIds);
        DataTable GetFGSQRDbDataSoap(string[] invoiceNo, DataTable db, string joinIds);
        DataTable GetSaleCDNData(string invoiceNo, string dataBase, SysDBInfoVMTemp connVM = null);
        DataTable GetSaleKohinoorDbData(string invoiceNo, DataTable conInfo, SysDBInfoVMTemp connVM = null);
        DataTable GetSaleKohinoorDbDataX(string invoiceNo, SysDBInfoVMTemp connVM = null);
        DataTable GetSaleOtherDbData(string invoiceNo, SysDBInfoVMTemp connVM = null);
        DataTable GetSaleOtherDbDatax(string invoiceNo, SysDBInfoVMTemp connVM = null);
        
        ////DataTable GetSale_DBData_SMC_Consumer(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo);
        
        string[] ImportBank(List<BankInformationVM> banks, SysDBInfoVMTemp connVM = null);
        string[] ImportCosting(List<CostingVM> costings, SysDBInfoVMTemp connVM = null);
        string[] ImportCustomer(List<CustomerVM> customers, List<CustomerVM> customeradd = null, SysDBInfoVMTemp connVM = null);
        string[] ImportExcelFile(ImportVM paramVM, DataTable dt = null,SysDBInfoVMTemp connVM = null);
        string[] ImportProduct(List<ProductVM> products, List<TrackingVM> trackings, List<ProductVM> productDetails = null,DataTable productStocks = null, SysDBInfoVMTemp connVM = null);
        string[] ImportProductOld(List<ProductVM> products, SysDBInfoVMTemp connVM = null);
        string[] ImportProductOpening(List<ProductVM> products, SysDBInfoVMTemp connVM = null);
        string[] ImportUOM(List<UOMVM> uoms, SysDBInfoVMTemp connVM = null);
        string[] ImportUOM_Names(List<UOMNameVM> uoms, SysDBInfoVMTemp connVM = null);
        string[] ImportVehicle(List<VehicleVM> vehicles, SysDBInfoVMTemp connVM = null);
        string[] ImportVendor(List<VendorVM> vendors, SysDBInfoVMTemp connVM = null);
        string[] InsertToCustomerAddress(CustomerVM vm, SqlConnection vConn, SqlTransaction vTransaction, SysDBInfoVMTemp connVM = null);
        string[] InsertToProductDetails(ProductVM vm, SqlConnection vConn, SqlTransaction vTransaction, SysDBInfoVMTemp connVM = null);
    



    }
}
