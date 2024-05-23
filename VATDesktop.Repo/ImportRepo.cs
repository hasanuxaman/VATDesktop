using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VATDesktop.Repo.ImportWCF;
using VATServer.Interface;
using VATViewModel.DTOs;
using System.Data.SqlClient;

namespace VATDesktop.Repo
{
    public class ImportRepo : IImport
    {

        ImportWCFClient wcf = new ImportWCFClient();

        public DataTable GetSaleSQRDbDataSenora(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNoWCF = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetSaleSQRDbDataSenora(invoiceNoWCF, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable GetSaleSQRDbData(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNoWCF = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetSaleSQRDbData(invoiceNoWCF, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable GetSaleSQRDbDataSoap(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNoWCF = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetSaleSQRDbDataSoap(invoiceNoWCF, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable GetSaleAduriDbData(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNoWCF = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetSaleAduriDbData(invoiceNoWCF, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable GetDataTableFromExcel(ImportVM paramVM)
        {
            try
            {
                string paramVMwcf = JsonConvert.SerializeObject(paramVM);

                string result = wcf.GetDataTableFromExcel(paramVMwcf);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public DataTable GetFGSQRDbData(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNowcf = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetFGSQRDbData(invoiceNowcf, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetFGSQRDbDataSenora(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNowcf = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetFGSQRDbDataSenora(invoiceNowcf, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetFGSQRDbDataSoap(string[] invoiceNo, DataTable db, string joinIds)
        {
            try
            {
                string invoiceNowcf = JsonConvert.SerializeObject(invoiceNo);
                string dbwcf = JsonConvert.SerializeObject(db);

                string result = wcf.GetFGSQRDbDataSoap(invoiceNowcf, dbwcf, joinIds);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetSaleCDNData(string invoiceNo, string dataBase, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string result = wcf.GetSaleCDNData(invoiceNo, dataBase);

                return JsonConvert.DeserializeObject<DataTable>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetSaleKohinoorDbData(string invoiceNo, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conInfowcf = JsonConvert.SerializeObject(conInfo);

                string result = wcf.GetSaleKohinoorDbData(invoiceNo, conInfowcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetSaleKohinoorDbDataX(string invoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string result = wcf.GetSaleKohinoorDbDataX(invoiceNo);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetSaleOtherDbData(string invoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string result = wcf.GetSaleOtherDbData(invoiceNo);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetSaleOtherDbDatax(string invoiceNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string result = wcf.GetSaleOtherDbDatax(invoiceNo);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetSale_DBData_SMC_Consumer(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo)
        {
            try
            {
                string conInfowcf = JsonConvert.SerializeObject(conInfo);

                string result = wcf.GetSMCDbData(invoiceNo, SalesFromDate, SalesToDate, conInfowcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataSet GetDataSetFromExcel(ImportVM paramVM)
        {
            try
            {
                string paramVMwcf = JsonConvert.SerializeObject(paramVM);

                string result = wcf.GetDataSetFromExcel(paramVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataSet GetDataTableFromExcelds(ImportVM paramVM)
        {
            try
            {
                string paramVMwcf = JsonConvert.SerializeObject(paramVM);

                string result = wcf.GetDataTableFromExcelds(paramVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportBank(List<BankInformationVM> banks, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bankswcf = JsonConvert.SerializeObject(banks);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportBank(bankswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportCosting(List<CostingVM> costings, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string costingswcf = JsonConvert.SerializeObject(costings);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportCosting(costingswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportCustomer(List<CustomerVM> customers, List<CustomerVM> customeradd = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string customerswcf = JsonConvert.SerializeObject(customers);
                string customeraddwcf = JsonConvert.SerializeObject(customeradd);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportCustomer(customerswcf, customeraddwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportExcelFile(ImportVM paramVM, DataTable dt = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string paramVMwcf = JsonConvert.SerializeObject(paramVM);

                string result = wcf.ImportExcelFile(paramVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportProduct(List<ProductVM> products, List<TrackingVM> trackings, List<ProductVM> productDetails = null, DataTable productStocks = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string productswcf = JsonConvert.SerializeObject(products);
                string trackingswcf = JsonConvert.SerializeObject(trackings);
                string productDetailswcf = JsonConvert.SerializeObject(productDetails);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportProduct(productswcf, trackingswcf, productDetailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportProductOld(List<ProductVM> products, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string productswcf = JsonConvert.SerializeObject(products);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportProductOld(productswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportProductOpening(List<ProductVM> products, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string productswcf = JsonConvert.SerializeObject(products);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportProductOpening(productswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportUOM(List<UOMVM> uoms, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string uomswcf = JsonConvert.SerializeObject(uoms);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportUOM(uomswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportUOM_Names(List<UOMNameVM> uoms, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string uomswcf = JsonConvert.SerializeObject(uoms);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportUOM_Names(uomswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportVehicle(List<VehicleVM> vehicles, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vehicleswcf = JsonConvert.SerializeObject(vehicles);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportVehicle(vehicleswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] ImportVendor(List<VendorVM> vendors, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vendorswcf = JsonConvert.SerializeObject(vendors);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ImportVendor(vendorswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToCustomerAddress(CustomerVM vm, SqlConnection vConn, SqlTransaction vTransaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToCustomerAddress(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToProductDetails(ProductVM vm, SqlConnection vConn, SqlTransaction vTransaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToProductDetails(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[]Product(ProductVM vm, SqlConnection vConn, SqlTransaction vTransaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToProductDetails(vmwcf, connVMwcf);

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
