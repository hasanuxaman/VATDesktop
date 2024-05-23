
using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace VATServer.Library
{
    public class BergerIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        BranchProfileDAL _branchDAL = new BranchProfileDAL();

        #region API Connection

        //////private string saleUrl = "https://zvatapi.bergerbd.com:8081/api/GetVatInfo";
        //////private string saleUrl_InvoiceNumebr = "https://zvatapi.bergerbd.com/api/GetVatInfoByInvoiceNumebr";

        ////private string saleUrl = "http://dev-zvat.bergerbd.com:8081/api/GetVatInfo";
        ////private string saleUrl_InvoiceNumebr = "http://dev-zvat.bergerbd.com:8081/api/GetVatInfoByInvoiceNumber";
        ////////private string PReceiveUrl = "http://dev-zvat.bergerbd.com:8081/api/VatProductionFGInfo";
        ////private string PReceiveUrl = "http://dev-zvat.bergerbd.com:8081/api/VatProductionFGInfoByDateRange";

        private string saleUrl = "https://vatapi.bergerbd.com/api/GetVatInfo";
        private string saleUrl_InvoiceNumebr = "https://vatapi.bergerbd.com/api/GetVatInfoByInvoiceNumber";
        private string PReceiveUrl = "https://vatapi.bergerbd.com/api/VatProductionFGInfoByDateRange";
        private string TransferIssueUrl = "https://vatapi.bergerbd.com/api/VatFGTransferInfoByDateRange";

        #endregion

        private AuthRoot Auth = new AuthRoot();
        private BergerParam bergerParam = new BergerParam();

        public BergerIntegrationDAL()
        {
            ////Auth = GetAuthToken();
        }

        #region Sale Data

        public string[] ProcessSalesData(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            string[] sqlResults = new string[6];

            #region try

            try
            {

                if (!string.IsNullOrWhiteSpace(param.RefNo))
                {
                    DataTable GetData = GetSaleData(param, null, null, connVM);

                    if (GetData == null || GetData.Rows.Count == 0)
                    {
                        sqlResults[0] = "Fail";
                        sqlResults[1] = "There is no data to save";

                        return sqlResults;
                    }

                    sqlResults = SaveSaleData(GetData, param, connVM);

                }
                else
                {

                    sqlResults = ProcessAndSaveData(param, connVM);

                }


            }
            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                sqlResults[0] = "fail";
                sqlResults[1] = ex.ToString();

                FileLogger.Log("BergerIntegrationDAL", "ProcessSalesData", ex.ToString());

                throw ex;
            }

            #endregion

            return sqlResults;
        }

        public string[] ProcessAndSaveData(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            string[] sqlResults = new string[6];

            #region try

            try
            {

                var dal = new BranchProfileDAL();

                DataTable dt = dal.SelectAll(null, null, null, null, null, true, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    string _branchId = dr["BranchID"].ToString();
                    string branchCode = dr["BranchCode"].ToString();

                    param.BranchCode = branchCode;

                    IntegrationParam paramVM = new IntegrationParam();

                    paramVM.BranchCode = branchCode;
                    paramVM.BranchId = _branchId;
                    paramVM.CurrentUserId = param.CurrentUserId;


                    DateTime FromDate = Convert.ToDateTime(param.FromDate);
                    DateTime ToDate = Convert.ToDateTime(param.ToDate);
                    Double Days = (ToDate - FromDate).TotalDays + 1;

                    DateTime CurrentDate = FromDate;


                    for (int i = 0; i < Days; i++)
                    {
                        //////paramVM.FromDate = CurrentDate.AddDays(-1).ToString("yyyy-MM-dd");
                        //////paramVM.ToDate = CurrentDate.AddDays(1).ToString("yyyy-MM-dd");

                        paramVM.FromDate = CurrentDate.ToString("yyyy-MM-dd");
                        paramVM.ToDate = CurrentDate.ToString("yyyy-MM-dd");

                        CurrentDate = CurrentDate.AddDays(1);

                        DataTable GetData = GetSaleData(paramVM, null, null, connVM);

                        if (GetData != null && GetData.Rows.Count > 0)
                        {
                            try
                            {
                                int count = GetData.Rows.Count;

                                sqlResults = SaveSaleData(GetData, paramVM, connVM);

                                if (sqlResults[0] == "Fail")
                                {
                                    #region Error log

                                    try
                                    {
                                        ErrorLogVM evm = new ErrorLogVM();

                                        evm.ImportId = "0";
                                        evm.FileName = "From Date: " + paramVM.FromDate + " to Date : " + paramVM.ToDate;
                                        evm.ErrorMassage = sqlResults[1];
                                        evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        evm.SourceName = "ProcessAndSaveData";
                                        evm.ActionName = "BergerIntegrationDAL";
                                        evm.TransactionName = "Sales";

                                        CommonDAL _cDal = new CommonDAL();

                                        string[] ErrorResult = _cDal.InsertErrorLogs(evm);

                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    #endregion

                                    ////throw new ArgumentNullException(sqlResults[1]);
                                }

                            }
                            catch (Exception ex)
                            {
                                #region Error log

                                try
                                {
                                    ErrorLogVM evm = new ErrorLogVM();

                                    evm.ImportId = "0";
                                    evm.FileName = "From Date: " + paramVM.FromDate + " to Date : " + paramVM.ToDate;
                                    evm.ErrorMassage = ex.Message;
                                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    evm.SourceName = "ProcessAndSaveData";
                                    evm.ActionName = "BergerIntegrationDAL";
                                    evm.TransactionName = "Sales";

                                    CommonDAL _cDal = new CommonDAL();

                                    string[] ErrorResult = _cDal.InsertErrorLogs(evm);

                                }
                                catch (Exception e)
                                {

                                }

                                #endregion
                            }

                        }

                    }

                    sqlResults[0] = "Success";
                    sqlResults[1] = "Data Process Successfully";

                }

            }
            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                sqlResults[0] = "fail";
                sqlResults[1] = ex.ToString();

                FileLogger.Log("BergerIntegrationDAL", "ProcessAndSaveData", ex.ToString());

                throw ex;
            }

            #endregion

            return sqlResults;
        }

        public DataTable GetSaleData(IntegrationParam param, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                string[] result = new[] { "Fail" };

                DataTable dt = new DataTable();
                DataTable dtapiData = new DataTable();

                DataSet ds = new DataSet();

                ds = GetSaleDataAPI(param);

                dt = ds.Tables[0];
                dtapiData = ds.Tables[1];

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }

                dt.Columns.Remove("TransactionType");
                dt.Columns.Remove("CompanyCode");

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region tempTable

                string tempTable = @"
create table #saletemp(
sl int identity(1,1), 
ID varchar(6000),
Branch_Code varchar(100),
Customer_Code varchar(100),
Customer_Name varchar(100),
Invoice_Date_Time varchar(100),
Reference_No varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
UOM varchar(100),
NBR_Price decimal(25,7),
SubTotal decimal(25,7),
VAT_Rate decimal(25,7),
VAT_Amount decimal(25,7),
SD_Rate decimal(25,7),
SDAmount decimal(25,7),
Vehicle_No varchar(100),
VehicleType varchar(100),
CustomerGroup varchar(100),
Delivery_Address varchar(500),
Comments varchar(500),
Sale_Type varchar(100),
Discount_Amount decimal(25,7),
Promotional_Quantity decimal(25,7),
Is_Print varchar(100),
Tender_Id varchar(100),
Post varchar(100),
LC_Number varchar(100),
Currency_Code varchar(100),
CommentsD varchar(100),
Non_Stock varchar(100),
Trading_MarkUp varchar(100),
[Type] varchar(100),
VAT_Name varchar(100),
Previous_Invoice_No varchar(100),
PreviousInvoiceDateTime varchar(100),
PreviousNBRPrice decimal(25,7),
PreviousQuantity decimal(25,7),
PreviousUOM varchar(100),
PreviousSubTotal decimal(25,7),
PreviousVATAmount decimal(25,7),
PreviousVATRate decimal(25,7),
PreviousSD decimal(25,7),
PreviousSDAmount decimal(25,7),
ReasonOfReturn varchar(100)
)";

                #endregion

                #region UpdateTypeVATRate

                string UpdateTypeVATRate = @"
----------- VAT Rate Update --------------
--update #saletemp set #saletemp.VAT_Rate=Products.VATRate 
----, #saletemp.UOM= Products.UOM
--, #saletemp.SD_Rate= Products.SD
--from Products where #saletemp.Item_Code=Products.ProductCode

----------- Type Update --------------

--update #saletemp set #saletemp.[Type]='VAT'
--where #saletemp.VAT_Rate='15'

--update #saletemp set #saletemp.[Type]='NoNVAT'
--where #saletemp.VAT_Rate='0'

--update #saletemp set #saletemp.[Type]='OtherRate'
--where #saletemp.VAT_Rate!='0' and #saletemp.VAT_Rate!='15' 

 --update #saletemp set #saletemp.NBR_Price = ((((#saletemp.NBR_Price * #saletemp.Quantity)- #saletemp.Discount_Amount)/#saletemp.Quantity)
 --*100/(100+#saletemp.VAT_Rate))

 --update #saletemp set #saletemp.Discount_Amount=0

delete #saletemp where Quantity<=0

";

                #endregion

                #region getAllData

                string getAllData = @"

select 
 ID
,Branch_Code
,Customer_Code
,Customer_Name
,Invoice_Date_Time
,Reference_No
,Item_Code
,Item_Name
,Quantity
,UOM
,NBR_Price
,SubTotal
,VAT_Rate
,VAT_Amount
,SD_Rate
,SDAmount
,Vehicle_No
,VehicleType
,CustomerGroup
,Delivery_Address
,Comments
,Sale_Type
,Discount_Amount
,Promotional_Quantity


,Is_Print
,Tender_Id
,Post
,LC_Number
,Currency_Code
,CommentsD
,Non_Stock
,Trading_MarkUp
,[Type]
,VAT_Name
,Previous_Invoice_No
,PreviousInvoiceDateTime
,PreviousNBRPrice
,PreviousQuantity
,PreviousUOM
,PreviousSubTotal
,PreviousVATAmount
,PreviousVATRate
,PreviousSD
,PreviousSDAmount
,ReasonOfReturn

from #saletemp

drop table #saletemp

";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                cmd.CommandText = UpdateTypeVATRate;
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                try
                {
                    if (dtapiData != null || dtapiData.Rows.Count != 0)
                    {
                        ////dtapiData.Columns.Remove("__metadata");

                        var ProcessDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };

                        dtapiData.Columns.Add(ProcessDateTime);

                        commonDal.BulkInsert("SalesAudits", dtapiData, currConn, transaction);
                    }
                }
                catch (Exception)
                {

                }

                if (transaction != null && vTransaction == null)
                {
                    transaction.Commit();
                }

                return dt;

                ////return new DataTable();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
        }

        public DataSet GetSaleDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                DataSet dataSet = new DataSet();
                DataTable dt = new DataTable();

                DataTable dtapiData = new DataTable("APIData");
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                Root apiData;

                apiData = GetSaleApiData(Auth, param);

                // convert api data to VAT system format 

                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                dt.Columns.Add("Branch_Code");
                dt.Columns.Add("Customer_Code");
                dt.Columns.Add("Customer_Name");
                dt.Columns.Add("Invoice_Date_Time");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("SubTotal");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                //////dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");
                dt.Columns.Add("SD_Rate");
                dt.Columns.Add("SDAmount");
                dt.Columns.Add("Is_Print");
                dt.Columns.Add("Tender_Id");
                dt.Columns.Add("Post");
                dt.Columns.Add("LC_Number");
                dt.Columns.Add("Currency_Code");
                dt.Columns.Add("CommentsD");
                dt.Columns.Add("Non_Stock");
                dt.Columns.Add("Trading_MarkUp");
                dt.Columns.Add("Type");
                dt.Columns.Add("VAT_Name");
                dt.Columns.Add("Previous_Invoice_No");
                dt.Columns.Add("PreviousInvoiceDateTime");
                dt.Columns.Add("PreviousNBRPrice");
                dt.Columns.Add("PreviousQuantity");
                dt.Columns.Add("PreviousUOM");
                dt.Columns.Add("PreviousSubTotal");
                dt.Columns.Add("PreviousVATAmount");
                dt.Columns.Add("PreviousVATRate");
                dt.Columns.Add("PreviousSD");
                dt.Columns.Add("PreviousSDAmount");
                dt.Columns.Add("ReasonOfReturn");
                dt.Columns.Add("TransactionType");
                dt.Columns.Add("CompanyCode");
                //////dt.Columns.Add("vkorg");

                #endregion

                if (apiData != null)
                {
                    #region Sales

                    foreach (var items in apiData.d.results)
                    {
                        if (items.vkorg != "1200")
                        {

                            string ID = items.BillingDoc;
                            string Branch_Code = items.BusArea;
                            string Invoice_Date_Time = items.BillingDate.ToString("yyyy-MM-dd HH:mm:ss");
                            string DeliveryDateTime = items.BillingDate.ToString("yyyy-MM-dd HH:mm:ss");
                            string Customer_Code = items.CustomerNo;
                            string Customer_Name = items.CustomerName;
                            if (string.IsNullOrEmpty(Customer_Name))
                            {
                                Customer_Name = "";
                            }
                            string Comments = "-";
                            string Delivery_Address = items.DeliveryAddress;

                            DataRow dr = dt.NewRow();

                            dr["ID"] = ID;
                            dr["Branch_Code"] = Branch_Code;
                            dr["Customer_Code"] = Customer_Code;
                            dr["Customer_Name"] = Customer_Name;
                            dr["Invoice_Date_Time"] = Invoice_Date_Time;
                            dr["Reference_No"] = ID;
                            dr["Item_Code"] = items.MaterialNo;
                            dr["Item_Name"] = items.MaterialName;
                            dr["UOM"] = items.UoM;
                            dr["Quantity"] = Convert.ToDecimal(items.BillQty);
                            dr["VAT_Amount"] = Convert.ToDecimal(items.VATAmount);
                            dr["SDAmount"] = Convert.ToDecimal(items.SdAmount);

                            dr["Currency_Code"] = items.waerk;
                            dr["SubTotal"] = items.Revenue;
                            dr["Discount_Amount"] = items.DisAmount;
                            dr["Comments"] = Comments;
                            dr["NBR_Price"] = items.UnitPrice;
                            dr["Delivery_Address"] = Delivery_Address;

                            dr["TransactionType"] = param.TransactionType;
                            dr["CompanyCode"] = code;

                            decimal SD_Rate = 0;

                            decimal VATRate = 0;

                            if (!string.IsNullOrWhiteSpace(items.VatPct))
                            {
                                VATRate = Convert.ToDecimal(items.VatPct);
                                VATRate = VATRate * 10;
                            }
                            if (!string.IsNullOrWhiteSpace(items.SdPct))
                            {
                                SD_Rate = Convert.ToDecimal(items.SdPct);
                                SD_Rate = SD_Rate * 10;
                            }
                            
                            dr["VAT_Rate"] = VATRate;

                            if (VATRate == 15)
                            {
                                dr["Type"] = "VAT";
                            }
                            else if (VATRate == 0)
                            {
                                dr["Type"] = "NoNVAT";
                            }
                            else
                            {
                                dr["Type"] = "OtherRate";
                            }

                            dr["SD_Rate"] = SD_Rate;


                            dr["Promotional_Quantity"] = "0";
                            dr["Sale_Type"] = "New";
                            dr["Vehicle_No"] = "NA";
                            dr["VehicleType"] = "NA";
                            dr["Is_Print"] = "N";
                            dr["Tender_Id"] = "0";
                            dr["Post"] = "N";
                            dr["LC_Number"] = "NA";
                            dr["CommentsD"] = "NA";
                            dr["Non_Stock"] = "N";
                            dr["Trading_MarkUp"] = "0";
                            dr["VAT_Name"] = "VAT 4.3";
                            dr["Previous_Invoice_No"] = "0";
                            dr["PreviousNBRPrice"] = "0";
                            dr["PreviousQuantity"] = "0";
                            dr["PreviousSubTotal"] = "0";
                            dr["PreviousVATAmount"] = "0";
                            dr["PreviousVATRate"] = "0";
                            dr["PreviousSD"] = "0";
                            dr["PreviousSDAmount"] = "0";
                            dr["ReasonOfReturn"] = "NA";

                            dt.Rows.Add(dr);
                        }

                    }
                    #endregion

                }

                #region Insert data in Middleware

                if (apiData != null)
                {
                    if (apiData.d.results.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(apiData.d.results);
                        dtapiData = JsonConvert.DeserializeObject<DataTable>(json);
                    }
                }

                #endregion

                dataSet.Tables.Add(dt);
                dataSet.Tables.Add(dtapiData);

                return dataSet;

                ////return new DataTable();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
        }

        private Root GetSaleApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            if (!string.IsNullOrWhiteSpace(param.RefNo))
            {
                url = saleUrl_InvoiceNumebr;

            }
            else
            {
                url = saleUrl;

            }

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";

            key = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") +
                DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") +
                ApiKey;
            //key = ApiKey;

            byte[] bytes = Encoding.UTF8.GetBytes(key);


            key = Convert.ToBase64String(bytes);



            bergerParam = new BergerParam();

            bergerParam.FromDate = param.FromDate;
            bergerParam.ToDate = param.ToDate;
            bergerParam.InvoiceNumber = param.RefNo;
            bergerParam.DepotId = param.BranchCode;
            bergerParam.CompanyId = "1000";
            //bergerParam.ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";
            bergerParam.ApiKey = key;



            string apiKeyValue = "";
            if (!string.IsNullOrWhiteSpace(bergerParam.InvoiceNumber))
            {
                apiKeyValue = "InvoiceNumber=" + bergerParam.InvoiceNumber + "&ApiKey=" + bergerParam.ApiKey;
            }
            else
            {
                ////apiKeyValue = "DepotId=" + bergerParam.DepotId + "&CompanyId=" + bergerParam.CompanyId
                ////+ "&FromDate=" + bergerParam.FromDate + "T00:00:00" + "&ToDate=" + bergerParam.ToDate + "T00:00:00" 
                ////+ "&ApiKey=" + bergerParam.ApiKey;

                apiKeyValue = "DepotId=" + bergerParam.DepotId + "&CompanyId=" + bergerParam.CompanyId
                   + "&FromDate=" + bergerParam.FromDate + "&ToDate=" + bergerParam.ToDate + "&ApiKey=" + bergerParam.ApiKey;

            }

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, bergerParam, apiKeyValue);

            DataRoot_Berger salesApiData = JsonConvert.DeserializeObject<DataRoot_Berger>(result);
            Root ApiData = JsonConvert.DeserializeObject<Root>(salesApiData.data);

            return ApiData;
        }

        public string[] SaveSaleData(DataTable salesData, IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            SaleDAL salesDal = new SaleDAL();
            string[] result = new string[6];

            result[0] = "Fail";
            result[1] = "Fail";

            try
            {

                TableValidation(salesData, paramVM);

                result = salesDal.SaveAndProcess(salesData, () => { }, Convert.ToInt32(paramVM.BranchId), "", connVM, paramVM, null, null, paramVM.CurrentUserId);

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
        }

        #endregion

        #region Production Receive (FG)

        public DataTable GetPReceiveData(IntegrationParam param, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {

                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                string[] result = new[] { "Fail" };

                DataTable dt = new DataTable();

                dt = GetPReceiveDataAPI(param);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }


                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region tempTable

                string tempTable = @"
create table #temp(
sl int identity(1,1), 
ID varchar(6000),
BranchCode varchar(100),
Receive_DateTime varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
UOM varchar(100),
Reference_No varchar(100),
Comments varchar(100),
Post varchar(10),
Return_Id varchar(100),
With_Toll varchar(100),
NBR_Price decimal(25,7),
VAT_Name varchar(100),
CustomerCode varchar(100),
Customer_Name varchar(100)

)";

                #endregion

                #region UpdateTypeVATRate

                string UpdateNBRPrice = @"
----------- VAT Rate Update --------------
update #temp set #temp.NBR_Price=Products.NBRPrice
from Products where #temp.Item_Code=Products.ProductCode

update #temp set #temp.UOM=Products.UOM
from Products where #temp.Item_Code=Products.ProductCode 
and #temp.UOM =''

delete #temp where Quantity<=0

";

                #endregion

                #region getAllData

                string getAllData = @"

select 
ID,
BranchCode,
Receive_DateTime,
Item_Code,
Item_Name,
Quantity,
UOM,
Reference_No,
Comments,
Post,
Return_Id,
With_Toll,
NBR_Price,
VAT_Name,
CustomerCode,
Customer_Name

from #temp
--where ID !=''
--and ID='000002052422'

drop table #temp

";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#temp", dt, currConn, transaction);

                cmd.CommandText = UpdateNBRPrice;
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                return dt;

                ////return new DataTable();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
        }

        public DataTable GetPReceiveDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                List<Root> apiData;

                apiData = GetPReceiveApiData(Auth, param);

                // convert api data to VAT system format 

                #region Create DataTable

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("ID");
                dt.Columns.Add("BranchCode");
                dt.Columns.Add("Receive_DateTime");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Post");
                dt.Columns.Add("Return_Id");
                dt.Columns.Add("With_Toll");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("VAT_Name");
                dt.Columns.Add("CustomerCode");

                #endregion

                if (apiData != null)
                {

                    #region Receive

                    foreach (var items in apiData)
                    {
                        string ID = items.ID.ToString();
                        string Branch_Code = items.BRANCHCODE.ToString();
                        string Receive_DateTime = Convert.ToDateTime(items.RECEIVE_DATETIME).ToString("yyyy-MM-dd HH:mm:ss");

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["BranchCode"] = Branch_Code;
                        dr["Receive_DateTime"] = Receive_DateTime;
                        dr["Reference_No"] = ID;

                        dr["Item_Code"] = items.ITEM_CODE;
                        dr["Item_Name"] = items.ITEM_NAME;
                        dr["UOM"] = items.UOM;

                        decimal QUANTITY = 0;
                        if (!string.IsNullOrWhiteSpace(items.QUANTITY))
                        {
                            QUANTITY = Convert.ToDecimal(items.QUANTITY);
                        }
                        dr["Quantity"] = QUANTITY;

                        decimal NBR_PRICE = 0;
                        if (!string.IsNullOrWhiteSpace(items.NBR_PRICE))
                        {
                            NBR_PRICE = Convert.ToDecimal(items.NBR_PRICE);
                        }

                        dr["NBR_Price"] = NBR_PRICE;

                        dr["Comments"] = items.COMMENTS;
                        dr["Post"] = items.POST;

                        dr["Return_Id"] = items.RETURN_ID;
                        dr["With_Toll"] = items.WITH_TOLL;
                        dr["VAT_Name"] = "VAT 4.3";
                        if (!string.IsNullOrWhiteSpace(items.CUSTOMERCODE))
                        {
                            dr["CustomerCode"] = items.CUSTOMERCODE;

                        }
                        else
                        {
                            dr["CustomerCode"] = "N/A";
                        }

                        dt.Rows.Add(dr);

                    }

                    #endregion

                }

                return dt;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
        }

        private List<Root> GetPReceiveApiData(AuthRoot auth, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            url = PReceiveUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";

            key = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") +
               DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") +
               ApiKey;
            //key = ApiKey;

            byte[] bytes = Encoding.UTF8.GetBytes(key);


            key = Convert.ToBase64String(bytes);

            bergerParam = new BergerParam();

            bergerParam.FromDate = param.FromDate;
            bergerParam.ToDate = param.ToDate;
            bergerParam.InvoiceNumber = param.RefNo;
            bergerParam.DepotId = param.BranchCode;
            bergerParam.CompanyId = "1000";
            bergerParam.ApiKey = key;

            string apiKeyValue = "";

            apiKeyValue = "ApiKey=" + bergerParam.ApiKey + "&FromDate=" + bergerParam.FromDate + "&ToDate=" + bergerParam.ToDate + "&TransactionType=" + "PRD";

            if (!string.IsNullOrWhiteSpace(bergerParam.InvoiceNumber))
            {
                apiKeyValue = "ApiKey=" + bergerParam.ApiKey + "&FromDate=" + bergerParam.FromDate + "&ToDate=" + bergerParam.ToDate + "&OrderNo=" + bergerParam.InvoiceNumber + "&TransactionType=" + "PRD";
            }

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, bergerParam, apiKeyValue);

            DataRoot_Berger salesApiData = JsonConvert.DeserializeObject<DataRoot_Berger>(result);
            List<Root> ApiData = JsonConvert.DeserializeObject<List<Root>>(salesApiData.data);

            return ApiData;
        }

        public ResultVM SavePReceive(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            ReceiveDAL receiveDal = new ReceiveDAL();
            CommonDAL commonDal = new CommonDAL();
            string[] result = new[] { "Fail" };
            string[] sqlResults = new[] { "Fail" };

            try
            {

                DataTable Data = GetPReceiveData(paramVM, null, null, connVM);
                if (Data == null || Data.Rows.Count == 0)
                {
                    rVM.Status = "Fail";
                    rVM.Message = "There is not data to save";
                    return rVM;

                }

                BulkInsertPReceiveData_day(Data, null, null, connVM);

                DateTime FromDate = Convert.ToDateTime(paramVM.FromDate);
                DateTime ToDate = Convert.ToDateTime(paramVM.ToDate);
                Double Days = (ToDate - FromDate).TotalDays + 1;

                DateTime CurrentDate = FromDate;

                for (int i = 0; i < Days; i++)
                {
                    IntegrationParam _IntegrationParam = new IntegrationParam();
                    _IntegrationParam.BranchCode = paramVM.BranchCode;
                    _IntegrationParam.BranchId = paramVM.BranchId;
                    _IntegrationParam.TransactionType = paramVM.TransactionType;
                    _IntegrationParam.RefNo = paramVM.RefNo;
                    _IntegrationParam.dtConnectionInfo = paramVM.dtConnectionInfo;

                    _IntegrationParam.FromDate = CurrentDate.ToString("yyyy-MM-dd");
                    _IntegrationParam.ToDate = CurrentDate.ToString("yyyy-MM-dd");

                    CurrentDate = CurrentDate.AddDays(1);

                    DataTable GetData = GetPReceiveData_day(_IntegrationParam, null, null, connVM);

                    if (GetData != null && GetData.Rows.Count > 0)
                    {
                        _IntegrationParam.Data = GetData;
                        ////paramVM.Data.Columns.Remove("Customer_Name");

                        result = receiveDal.SaveReceive_Split(_IntegrationParam, connVM);

                        ////if (result[0] == "Fail")
                        ////{
                        ////    throw new ArgumentNullException(result[1]);
                        ////}

                    }

                }

                rVM.Status = result[0];
                rVM.Message = result[1];

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                return rVM;

            }
            finally
            {


            }
            return rVM;
        }

        public DataTable GetPReceiveData_day(IntegrationParam param, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {

                CommonDAL commonDal = new CommonDAL();

                string[] result = new[] { "Fail" };

                DataTable dt = new DataTable();

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                string fromDate = Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd 00:00:00.000");
                string ToDate = Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd 23:59:59.000");

                #region getAllData

                string getAllData = @"

select 
 ID
,BranchCode
,Receive_DateTime
,Item_Code
,Item_Name
,Quantity
,UOM
,Reference_No
,Comments
,Post
,Return_Id
,With_Toll
,NBR_Price
,VAT_Name
,CustomerCode

from tempReceiveData_Day
where 1=1

and Receive_DateTime>=@Receive_DateTimeFrom
and Receive_DateTime<=@Receive_DateTimeTo

";

                #endregion

                SqlCommand cmd = new SqlCommand(getAllData, currConn, transaction);
                cmd.Parameters.AddWithValue("@Receive_DateTimeFrom", fromDate);
                cmd.Parameters.AddWithValue("@Receive_DateTimeTo", ToDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();
                adapter.Fill(dt);
                return dt;

                ////return new DataTable();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
        }

        public string BulkInsertPReceiveData_day(DataTable Data, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string result = "";
            CommonDAL commonDal = new CommonDAL();

            string[] sqlResults = new[] { "Fail" };

            #endregion variable

            try
            {

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                string deleteData = @"delete tempReceiveData_Day  ";
                SqlCommand cmd = new SqlCommand(deleteData, currConn, transaction);
                cmd.ExecuteNonQuery();

                sqlResults = commonDal.BulkInsert("tempReceiveData_Day", Data, currConn, transaction, 10000, null, connVM);

                if (transaction != null && vTransaction == null)
                {
                    transaction.Commit();
                }

                result = "success";

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }

            return result;
        }


        #endregion

        #region Transfer

        public DataTable GetTransferData(IntegrationParam param, SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string IsProcess = "N")
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");


                string[] result = new[] { "Fail" };

                DataTable dt = new DataTable();

                dt = GetTransferDataAPI(param, connVM);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }

                //////dt.Columns.Remove("TransactionType");
                //////dt.Columns.Remove("CompanyCode");

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region tempTable

                string tempTable = @"
create table #temp(
ID varchar(6000),
BranchCode varchar(100),
TransactionDateTime varchar(100),
TransactionType varchar(100),
CostPrice varchar(100),
TransferToBranchCode varchar(100),
ReferenceNo varchar(100),
Comments varchar(100),
ProductCode varchar(100),
ProductName varchar(100),
Quantity decimal(25,7),
UOM varchar(500),
VAT_Rate decimal(25,7),
Post varchar(100),
CommentsD varchar(100),
BranchFromRef varchar(100),
BranchToRef varchar(100),
VehicleNo varchar(100),
VehicleType varchar(100),
IsProcess varchar(100)

)";

                #endregion

                #region Update VATRate

                string UpdateVATRate = @"
----------- VAT Rate Update --------------
update #temp set #temp.VAT_Rate=Products.VATRate , #temp.CostPrice=Products.NBRPrice
from Products where #temp.ProductCode=Products.ProductCode

update #temp set #temp.VehicleNo='N/A' where #temp.VehicleNo='' or #temp.VehicleNo is null
update #temp set #temp.VehicleType='N/A' where #temp.VehicleType='' or #temp.VehicleType is null
";
                string UpdateIsProcessFlg = @"

update #temp set IsProcess='N'

update #temp set IsProcess='Y' from TransferIssues where #temp.ID =TransferIssues.ImportIDExcel

";

                string DeleteRM = @"
delete #temp where TransactionType='Raw'

";
                string DeleteFG = @"
delete #temp where TransactionType!='Raw'

";


                #endregion

                #region getAllData

                string getAllData = @"

select 
ID 
,BranchCode 
,TransactionDateTime 
,TransactionType 
,ProductCode 
,ProductName 
,Quantity 
,CostPrice 
,UOM 
,VAT_Rate 
,TransferToBranchCode 
,ReferenceNo 
,Comments 
,Post 
,CommentsD 
,BranchFromRef 
,BranchToRef 
,VehicleNo 
,VehicleType 

from #temp
where 1=1 

";



                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    getAllData += @" and IsProcess=@IsProcess";
                }

                getAllData += @" drop table #temp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#temp", dt, currConn, transaction);
                //result = commonDal.BulkInsert("SalesTempData", dt, currConn, transaction);

                cmd.CommandText = UpdateVATRate;
                cmd.ExecuteNonQuery();

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();

                if (param.TransactionType == "62Out")
                {
                    cmd.CommandText = DeleteRM;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = DeleteFG;
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = getAllData;
                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    cmd.Parameters.AddWithValue("@IsProcess", IsProcess);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
        }

        public DataTable GetTransferDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                List<Root> apiData;

                apiData = GetTransferApiData(Auth, param, connVM);

                // convert api data to VAT system format 

                #region Create DataTable

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("ID");//
                dt.Columns.Add("BranchCode");
                dt.Columns.Add("TransactionDateTime");//
                dt.Columns.Add("TransactionType");
                dt.Columns.Add("CostPrice");//
                dt.Columns.Add("TransferToBranchCode");
                dt.Columns.Add("ReferenceNo");//
                dt.Columns.Add("Comments");//
                dt.Columns.Add("ProductCode");//
                dt.Columns.Add("ProductName");//
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");//
                dt.Columns.Add("VAT_Rate");//
                dt.Columns.Add("Post");//
                dt.Columns.Add("CommentsD");//
                dt.Columns.Add("BranchFromRef");
                dt.Columns.Add("BranchToRef");
                dt.Columns.Add("VehicleNo");
                dt.Columns.Add("VehicleType");

                #endregion

                ProductDAL dal = new ProductDAL();

                if (apiData != null)
                {

                    #region Transfer

                    string TransactionTime = DateTime.Now.ToString("HH:mm:ss");

                    foreach (var items in apiData)
                    {
                        string ID = items.ID.ToString();
                        string BranchCode = items.BRANCHCODE.ToString();
                        string TransactionDateTime = Convert.ToDateTime(items.RECEIVE_DATETIME).ToString("yyyy-MM-dd " + TransactionTime);
                        string TransferToBranchCode = items.TRANS_TO_BRANCH_CODE.ToString();

                        string product_code = items.ITEM_CODE;

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["BranchCode"] = BranchCode;
                        dr["TransferToBranchCode"] = TransferToBranchCode;
                        dr["TransactionDateTime"] = TransactionDateTime;
                        dr["ReferenceNo"] = ID;
                        dr["ProductCode"] = product_code;
                        dr["ProductName"] = items.ITEM_NAME;
                        dr["UOM"] = items.UOM;
                        dr["Quantity"] = Convert.ToDecimal(items.QUANTITY);
                        dr["CostPrice"] = 0;

                        string type = items.MATERIAL_TYPE_DESC;

                        if (type == "Packaging" || type.ToLower() == "Packaging Product".ToLower())
                        {
                            type = "Raw";
                        }
                        if (type.ToLower() == "Finished Product".ToLower())
                        {
                            type = "Finished";
                        }


                        dr["TransactionType"] = type;
                        dr["Comments"] = "-";
                        dr["Post"] = items.POST;
                        dr["BranchFromRef"] = BranchCode;
                        dr["BranchToRef"] = TransferToBranchCode;
                        dr["VehicleNo"] = items.VEHICLE_NO;
                        dr["VehicleType"] = items.VEHICLE_TYPE;
                        dr["CommentsD"] = items.COMMENTS;
                        dr["VAT_Rate"] = "0";

                        ////////if (!string.IsNullOrWhiteSpace(product_code))
                        ////////{
                        ////////    ProductVM vm = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { product_code }, null, null, null, connVM).FirstOrDefault();

                        ////////    if (vm != null)
                        ////////    {
                        ////////        dr["VAT_Rate"] = vm.VATRate;
                        ////////    }

                        ////////}

                        dt.Rows.Add(dr);

                    }

                    #endregion

                }

                return dt;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<Root> GetTransferApiData(AuthRoot auth, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            url = TransferIssueUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";

            key = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00")
                + DateTime.Now.Hour.ToString("00") + ApiKey;
            //key = ApiKey;

            byte[] bytes = Encoding.UTF8.GetBytes(key);

            key = Convert.ToBase64String(bytes);

            bergerParam = new BergerParam();

            bergerParam.FromDate = param.FromDate;
            bergerParam.ToDate = param.ToDate;
            bergerParam.InvoiceNumber = param.RefNo;
            bergerParam.DepotId = param.BranchCode;
            bergerParam.CompanyId = "1000";
            bergerParam.ApiKey = key;

            string apiKeyValue = "";

            apiKeyValue = "ApiKey=" + bergerParam.ApiKey + "&FromDate=" + bergerParam.FromDate + "&ToDate=" + bergerParam.ToDate;//+ "&DepotId=" + bergerParam.DepotId

            if (!string.IsNullOrWhiteSpace(bergerParam.InvoiceNumber))
            {
                apiKeyValue = "ApiKey=" + bergerParam.ApiKey + "&FromDate=" + bergerParam.FromDate + "&ToDate=" + bergerParam.ToDate + "&OrderNo=" + bergerParam.InvoiceNumber;//+ "&DepotId=" + bergerParam.DepotId
            }

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, bergerParam, apiKeyValue);

            DataRoot_Berger salesApiData = JsonConvert.DeserializeObject<DataRoot_Berger>(result);
            List<Root> ApiData = JsonConvert.DeserializeObject<List<Root>>(salesApiData.data);

            return ApiData;
        }

        public ResultVM SaveTransferData(DataTable dtTransfer, string BranchCode, string transactionType, string CurrentUser, int branchId
            , Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            bool integration = false, SysDBInfoVMTemp connVM = null, string entryTime = "", string UserId = "")
        {
            ResultVM rVM = new ResultVM();
            TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

            string[] sqlResults = new string[6];

            try
            {

                if (dtTransfer == null || dtTransfer.Rows.Count <= 0)
                {
                    throw new Exception("There is no data to save");
                }

                TableValidation_Transfer(dtTransfer, connVM);

                sqlResults = _TransferIssueDAL.SaveTempTransfer(dtTransfer, null, null, CurrentUser, 0, () => { }, null, null, true, connVM, DateTime.Now.ToString("HH:mm:ss"), UserId);

                rVM.Status = sqlResults[0];
                rVM.Message = sqlResults[1];

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
                ////throw ex;
            }
            finally
            {


            }
            return rVM;
        }

        private void TableValidation_Transfer(DataTable dtTransfer, SysDBInfoVMTemp connVM = null)
        {

            if (!dtTransfer.Columns.Contains("Weight"))
            {
                DataColumn varDataColumn = new DataColumn("Weight") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }

        }

        public ResultVM SaveTransferdata_Berger_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SaleDAL salesDal = new SaleDAL();
            try
            {

                DataTable transferData = GetTransferData(paramVM, connVM);

                rVM = SaveTransferData(transferData, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, Convert.ToInt32(paramVM.BranchId), () => { }, null, null, false, connVM, "", paramVM.CurrentUserId);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
                throw ex;
            }
            finally
            {

            }
            return rVM;
        }

        ////public string[] SaveTransfer_Berger_Pre(DataTable dtTransfer, string BranchCode, string transactionType, string CurrentUser,
        //// int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
        //// bool integration = false, SysDBInfoVMTemp connVM = null, string entryTime = "", string UserId = "")
        ////{
        ////    ResultVM rVM = new ResultVM();
        ////    ////IssueDAL issueDal = new IssueDAL();
        ////    TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

        ////    string[] sqlResults = new string[6];

        ////    try
        ////    {

        ////        if (dtTransfer == null || dtTransfer.Rows.Count <= 0)
        ////        {
        ////            throw new Exception("There is no data to save");
        ////        }

        ////        TableValidation_Transfer(dtTransfer, connVM);

        ////        ////DataTable PIssueData = GetPIssueData(paramVM, connVM);
        ////        ////paramVM.Data = PIssueData;

        ////        //////string[] result = issueDal.SaveIssue_Split(paramVM, connVM);

        ////        sqlResults = _TransferIssueDAL.SaveTempTransfer(dtTransfer, null, null, CurrentUser, 0,
        ////            () => { }, null, null, true, connVM, DateTime.Now.ToString("HH:mm:ss"), UserId);


        ////        rVM.Status = sqlResults[0];
        ////        rVM.Message = sqlResults[1];

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        sqlResults[0] = "Fail";
        ////        sqlResults[1] = ex.Message;
        ////        throw ex;
        ////    }
        ////    finally
        ////    {


        ////    }
        ////    return sqlResults;
        ////}


        #endregion

        private string PostData(string url, string payLoad, bool isCredential = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;

                request.ContentType = "application/json;charset=UTF-8";

                NetworkCredential creds = GetCredentials();
                request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);

                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string GetData(string url, AuthRoot auth, SysDBInfoVMTemp connVM = null, BergerParam _BergerParam = null, string apiKeyValue = "")
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                var json = JsonConvert.SerializeObject(_BergerParam);

                ////////request.Headers.Add("jw-token", "Bearer " + auth.token);
                ////////request.Headers.Add("company-id", auth.company.FirstOrDefault().id.ToString());

                ////string apiKey = "DepotId=" + _BergerParam.DepotId + "&CompanyId=" + _BergerParam.CompanyId
                ////    + "&FromDate=" + "2020-01-06T00:00:00" + "&ToDate=" + "2020-01-7T00:00:00" + "&ApiKey=" + _BergerParam.ApiKey;

                ////string fromDate = _BergerParam.FromDate + "T00:00:00";
                ////string ToDate = _BergerParam.ToDate + "T00:00:00";

                string apiKey = "";
                ////if (!string.IsNullOrWhiteSpace(_BergerParam.InvoiceNumber))
                ////{
                ////    apiKey = "InvoiceNumber=" + _BergerParam.InvoiceNumber + "&ApiKey=" + _BergerParam.ApiKey;
                ////}
                ////else
                ////{
                ////    apiKey = "DepotId=" + _BergerParam.DepotId + "&CompanyId=" + _BergerParam.CompanyId
                ////    + "&FromDate=" + _BergerParam.FromDate + "T00:00:00" + "&ToDate=" + _BergerParam.ToDate + "T00:00:00" + "&ApiKey=" + _BergerParam.ApiKey;

                ////}

                apiKey = apiKeyValue;

                byte[] byteArray = Encoding.UTF8.GetBytes(apiKey);
                request.ContentLength = byteArray.Length;

                request.ContentType = "application/x-www-form-urlencoded";

                ////////////NetworkCredential creds = GetCredentials();
                ////////////request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);
                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private NetworkCredential GetCredentials(SysDBInfoVMTemp connVM = null)
        {

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            if (code.ToLower() == "purofood")
            {
                ////return new NetworkCredential("rest", "123456");
                return new NetworkCredential("vatuser", "123456");

            }
            else
            {
                //////// read from config file
                ////return new NetworkCredential("rest", "123456");
                return new NetworkCredential("vatuser", "123456");
            }


        }

        private void TableValidation(DataTable salesData, IntegrationParam paramVM)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = paramVM.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = paramVM.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = paramVM.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);
        }

        public DataTable GetSaleDataXML(DataTable table, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null, string IsProcess = "N")
        {
            #region variable

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            DataTable dt = new DataTable();
            CommonDAL commonDal = new CommonDAL();

            #endregion variable

            try
            {

                dt = table.Copy();

                string[] result = new[] { "Fail" };

                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("There is no data for integration.");
                }

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region tempTable

                string tempTable = @"
create table #saletemp(
sl int identity(1,1), 
ID varchar(6000),
Branch_Code varchar(100),
CustomerGroup varchar(100),
Customer_Name varchar(100),
Customer_Code varchar(100),
CustomerBIN varchar(100),
CustomerAddress varchar(500),
Delivery_Address varchar(500),
Invoice_Date_time varchar(100),
Delivery_Date_Time varchar(100),
Reference_No varchar(100),
Vehicle_No varchar(100),
VehicleType varchar(100),
Comments varchar(2000),
Sale_Type varchar(100),
Previous_Invoice_No varchar(100),
Is_Print varchar(100),
Tender_Id varchar(100),
Post varchar(100),
LC_Number varchar(100),
Currency_Code varchar(100),
CommentsD varchar(100),
Item_Code varchar(100),
Item_Name varchar(200),
Quantity decimal(25,7),
NBR_Price decimal(25,7),
UOM varchar(100),
VAT_Rate decimal(25,7),
SD_Rate decimal(25,7),
Non_Stock varchar(100),
Trading_Markup varchar(100),
[Type] varchar(100),
Discount_Amount decimal(25,7),
Promotion_Quantity decimal(25,7),
VAT_Name varchar(100),
SDAmount decimal(25,7),
VAT_Amount decimal(25,7),
SubTotal decimal(25,7),
TransactionType varchar(100),
ExpDescription varchar(200),
ExpQuantity varchar(200),
ExpGrossWeight varchar(200),
ExpNetWeight varchar(200),
ExpNumberFrom varchar(200),
ExpNumberTo varchar(200),
ReturnId varchar(200),
PreviousInvoiceDateTime varchar(100),
PreviousNBRPrice decimal(25,7),
PreviousQuantity decimal(25,7),
PreviousUOM varchar(100),
PreviousSD decimal(25,7),
PreviousVATRate decimal(25,7),
ReasonOfReturn varchar(100),
UserName varchar(100)

)";

                #endregion

                #region GetZeroQty

                string GetZeroQty = @"

select 
 ID
,Branch_Code
,Customer_Code
,Customer_Name
,Invoice_Date_Time
,Item_Code
,Item_Name
,Quantity
,UOM
,NBR_Price
,SubTotal
,Discount_Amount
,Promotional_Quantity
,VAT_Rate
,VAT_Amount

from #saletemp
where Quantity<=0
";


                #endregion

                #region getAllData

                string getAllData = @"

select 
 ID 
,Branch_Code 
,CustomerGroup
,Customer_Name
,Customer_Code
,CustomerBIN 
,CustomerAddress 
,Delivery_Address
,Invoice_Date_time 
,Delivery_Date_Time
,Reference_No
,Vehicle_No
,VehicleType
,Comments
,Sale_Type
,Previous_Invoice_No
,Is_Print
,Tender_Id
,Post
,LC_Number
,Currency_Code
,CommentsD
,Item_Code
,Item_Name
,Quantity 
,UOM
,NBR_Price
,VAT_Rate 
,SD_Rate
,Non_Stock
,Trading_MarkUp
,[Type]
,Discount_Amount
,Promotion_Quantity
,VAT_Name
,SDAmount
,VAT_Amount
,SubTotal
,TransactionType
,ExpDescription 
,ExpQuantity
,ExpGrossWeight
,ExpNetWeight
,ExpNumberFrom
,ExpNumberTo
,ReturnId
,PreviousInvoiceDateTime
,PreviousNBRPrice
,PreviousQuantity
,PreviousUOM
,PreviousSD
,PreviousVATRate
,ReasonOfReturn
,UserName

from #saletemp
where 1=1 
and Quantity>0 and NBR_Price>0

";

                getAllData += @" drop table #saletemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                #region Get Zero value data

                //cmd.CommandText = GetZeroQty;
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //dt = new DataTable();

                //adapter.Fill(dt);

                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    string IDs = "";
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        IDs = IDs + row["ID"].ToString() + ",";
                //    }

                //    IDs = IDs.Trim(',');
                //    throw new ArgumentNullException("", "Quantity is mandatory.  Zero Quantity or NBR Price is not allowed IDs(" + IDs + ")");

                //}

                #endregion

                cmd.CommandText = getAllData;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                return dt;

                ////return new DataTable();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
        }



    }

}
