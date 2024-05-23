using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;

namespace VATServer.Library.Integration
{
    public class ShumiHotCakeIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        BranchProfileDAL _branchDAL = new BranchProfileDAL();

        #region Shumi HotCake Dhaka

        #region New API Connection

        ////private string AuthUrl = "http://202.74.246.84:82/api/v1/auth/login";
        ////private string saleUrl = "http://27.147.222.218:89/API/api/VMS/GetVMSTranDataSalesOnly"; // Old IP
        //private string saleUrl = "http://103.159.128.178:89/API/api/VMS/GetVMSTranDataSalesOnly"; // New IP 13 Mar 2023
        private string saleUrl = "http://103.159.128.178:8090/api/VMS/GetVMSTranDataSalesOnly"; // New IP 02 Dec 2023
        private string BasePssaleUrlCtg = "http://45.125.223.207:8091";
        //private string BasePssaleUrlCtg = "https://localhost:44312"; 

        #endregion

        private AuthRoot Auth = new AuthRoot();

        #region Sales Data

        public DataTable GetSaleData(IntegrationParam param, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null, string IsProcess = "N")
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

                dt = GetSaleDataAPI(param, connVM);

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
Item_Name varchar(200),
Quantity decimal(25,7),
UOM varchar(100),
NBR_Price decimal(25,7),
SubTotal decimal(25,7),
Vehicle_No varchar(100),
VehicleType varchar(100),
CustomerGroup varchar(100),
Delivery_Address varchar(500),
Comments varchar(2000),
Sale_Type varchar(100),
Discount_Amount decimal(25,7),
Promotional_Quantity decimal(25,7),
VAT_Rate decimal(25,7),
VAT_Amount decimal(25,7),
SD_Rate decimal(25,7),
SDAmount decimal(25,7),
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
ReasonOfReturn varchar(100),
IsProcess varchar(100),
foc decimal(25,7),
Option2 varchar(100),
Option3 varchar(20),


)";

                #endregion

                #region UpdateTypeVATRate

                string UpdateTypeVATRate = @"
----------- VAT Rate Update --------------
----update #saletemp set #saletemp.VAT_Rate=Products.VATRate , #saletemp.UOM= Products.UOM, #saletemp.SD_Rate= Products.SD
----from Products where #saletemp.Item_Code=Products.ProductCode

--------------- Type Update --------------

----update #saletemp set #saletemp.[Type]='VAT'
---- where #saletemp.VAT_Rate='15'

---- update #saletemp set #saletemp.[Type]='NonVAT'
---- where #saletemp.VAT_Rate='0'

---- update #saletemp set #saletemp.[Type]='OtherRate'
---- where #saletemp.VAT_Rate!='0' and #saletemp.VAT_Rate!='15' 

 ----update #saletemp set #saletemp.NBR_Price = ((((#saletemp.NBR_Price * #saletemp.Quantity)- #saletemp.Discount_Amount)/#saletemp.Quantity)
 ----*100/(100+#saletemp.VAT_Rate))

 ----update #saletemp set #saletemp.Discount_Amount=0

";

                string UpdateCustomer = @"
----------- Type Customer --------------

update #saletemp set #saletemp.Customer_Code='10001' , #saletemp.Customer_Name='Cash Counter'
";

                string UpdateIsProcessFlg = @"

update #saletemp set IsProcess='N'

update #saletemp set IsProcess='Y' from SalesInvoiceHeaders where #saletemp.ID =SalesInvoiceHeaders.ImportIDExcel

";

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
,Customer_Code
,Customer_Name
,Invoice_Date_Time
,Reference_No
,Item_Code
,Item_Name
,UOM
,Quantity
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
,foc Promotional_Quantity
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

,Option2 Option2
,Option3 Option3

from #saletemp
where 1=1 
and Quantity>0

";
                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    getAllData += @" and IsProcess=@IsProcess";
                }

                getAllData += @" drop table #saletemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                cmd.CommandText = GetZeroQty;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string IDs = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        IDs = IDs + row["ID"].ToString() + ",";
                    }

                    IDs = IDs.Trim(',');
                    throw new ArgumentNullException("", "Quantity is mandatory.  Zero Quantity is not allowed IDs(" + IDs + ")");

                }

                ////cmd.CommandText = UpdateTypeVATRate;
                ////cmd.ExecuteNonQuery();

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();

                cmd.CommandText = UpdateCustomer;
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllData;
                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    cmd.Parameters.AddWithValue("@IsProcess", IsProcess);
                }
                adapter = new SqlDataAdapter(cmd);

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

        public DataTable GetSaleDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");
                //string IntBranchCode = GetBranchcode(param.BranchId, connVM);

                List<JsonModel_ShumiHotCake> apiData;

                apiData = GetSaleApiData(Auth, param, connVM);

                // convert api data to VAT system format 

                #region Create DataTable

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("ID");
                ////dt.Columns.Add("InvoiceNo");
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
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                //////dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");

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
                dt.Columns.Add("Option2");
                dt.Columns.Add("Option3");

                #endregion

                if (apiData != null)
                {

                    #region Sales

                    string InvoiceTime = DateTime.Now.ToString("HH:mm:ss");

                    foreach (var items in apiData)
                    {
                        string ID = items.TicketNumber;
                        string Reference_No = ID;
                        string Branch_Code = items.OutletCode.ToString();
                        string Invoice_Date_Time = items.OrderCreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        string DeliveryDateTime = Invoice_Date_Time;
                        string Customer_Code = "NA";
                        string Customer_Name = "NA";

                        if (string.IsNullOrEmpty(Customer_Name))
                        {
                            Customer_Name = "";
                        }
                        string Comments = "";
                        string Delivery_Address = "-";

                        #region Lineitems

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["Branch_Code"] = Branch_Code;
                        dr["Customer_Code"] = Customer_Code;
                        dr["Customer_Name"] = Customer_Name;
                        dr["Invoice_Date_Time"] = Invoice_Date_Time;
                        dr["Reference_No"] = Reference_No;
                        dr["Item_Code"] = items.MenuItemCode;
                        dr["Item_Name"] = items.MenuItemName;
                        dr["UOM"] = items.UOM;

                        decimal Discount = items.Discount;
                        Discount = Discount * -1;
                        decimal perqtyDis = Discount / items.Quantity;

                        dr["Currency_Code"] = "BDT";
                        dr["Quantity"] = items.Quantity;
                        dr["SD_Rate"] = items.SDPerc;
                        dr["SDAmount"] = items.SD;
                        dr["NBR_Price"] = items.Price;
                        dr["SubTotal"] = items.NetSales;
                        dr["Discount_Amount"] = perqtyDis;
                        dr["Comments"] = Comments;
                        dr["Delivery_Address"] = Delivery_Address;
                        dr["TransactionType"] = param.TransactionType;
                        dr["CompanyCode"] = code;

                        decimal VATRate = 0;
                        decimal VATAmount = 0;

                        decimal VatFivePerc = items.VatFivePerc;
                        decimal VatFive = items.VatFive;

                        decimal VatFifteenPerc = items.VatFifteenPerc;
                        decimal VatFifteen = items.VatFifteen;

                        if (VatFivePerc == 0 && VatFifteenPerc == 0)
                        {
                            VATRate = 0;
                            VATAmount = 0;
                        }
                        else if (VatFivePerc > VatFifteenPerc)
                        {
                            VATRate = VatFivePerc;
                            VATAmount = VatFive;
                        }
                        else if (VatFivePerc < VatFifteenPerc)
                        {
                            VATRate = VatFifteenPerc;
                            VATAmount = VatFifteen;
                        }

                        dr["VAT_Rate"] = VATRate;

                        dr["VAT_Amount"] = VATAmount;

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
                        dr["Option2"] = items.OutletCode;
                        dr["Option3"] = items.OutletName;

                        dt.Rows.Add(dr);

                        #endregion

                    }
                    #endregion

                }

                #region Insert data in Middleware


                //#region ConnectionInfo

                //DataTable dtConnectionInfo = new DataTable();

                //dtConnectionInfo = _branchDAL.SelectAll(param.BranchId, null, null, null, null, true);

                //#endregion

                //#region Open Connection and Transaction

                //SqlConnection currConn = null;
                //SqlTransaction transaction = null;

                //currConn = _dbsqlConnection.GetDepoConnection(dtConnectionInfo);

                //if (currConn.State != ConnectionState.Open)
                //{
                //    currConn.Open();
                //}

                //transaction = currConn.BeginTransaction();

                //#endregion

                //commonDal.BulkInsert("SalesInvoices", dt, currConn, transaction);

                //transaction.Commit();

                //if (currConn != null)
                //{
                //    if (currConn.State == ConnectionState.Open)
                //    {
                //        currConn.Close();
                //    }

                //}

                #endregion


                return dt;

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

        private List<JsonModel_ShumiHotCake> GetSaleApiData(AuthRoot auth, IntegrationParam param, SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            url = saleUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (!string.IsNullOrEmpty(param.FromDate))
            {
                ////DateTime StartDate = Convert.ToDateTime(param.FromDate);
                ////string sDate = StartDate.ToString("dd") + "%20" + StartDate.ToString("MMM") + "%20" + StartDate.ToString("yyyy");
                query["StartDate"] = param.FromDate;
            }

            if (!string.IsNullOrEmpty(param.ToDate))
            {
                //////DateTime EndDate = Convert.ToDateTime(param.ToDate);
                //////string eDate = EndDate.ToString("dd") + "%20" + EndDate.ToString("MMM") + "%20" + EndDate.ToString("yyyy");
                query["EndDate"] = param.ToDate;
            }

            ////if (!string.IsNullOrEmpty(param.RefNo))
            ////{
            ////    query["invoiceNo"] = param.RefNo;
            ////}

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth);

            List<JsonModel_ShumiHotCake> salesApiData = JsonConvert.DeserializeObject<List<JsonModel_ShumiHotCake>>(result);

            return salesApiData;
        }

        public string[] SaveSaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            SaleDAL salesDal = new SaleDAL();
            string[] result = new string[6];

            result[0] = "Fail";
            result[1] = "Fail";

            try
            {
                DataTable GetData = GetSaleData(paramVM, null, null, connVM);

                TableValidation(GetData, paramVM);

                result = salesDal.SaveAndProcess(GetData, () => { }, Convert.ToInt32(paramVM.BranchId), "", connVM, paramVM, null, null, paramVM.CurrentUserId);

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
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


        public ResultVM SaveSale_ShumiHotCake_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SaleDAL salesDal = new SaleDAL();
            try
            {

                DataTable salesData = GetSaleData(paramVM, null, null, connVM);

                TableValidation(salesData, paramVM);

                string[] result = salesDal.SaveAndProcess(salesData, () => { }, Convert.ToInt32(paramVM.BranchId), "", connVM, paramVM, null, null, paramVM.CurrentUserId);

                rVM.Status = result[0];
                rVM.Message = result[1];

            }
            catch (Exception ex)
            {

                rVM.Message = ex.Message;
                throw ex;
            }
            finally
            {


            }
            return rVM;
        }


        #endregion

        private string GetData(string url, AuthRoot auth, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string CompanyId = "1";
                CommonDAL commonDal = new CommonDAL();

                //////string code = commonDal.settingValue("CompanyCode", "Code");

                //////if (code.ToLower() == "eahpl")
                //////{
                //////    CompanyId = "4";
                //////}
                //////else if (code.ToLower() == "eail")
                //////{
                //////    CompanyId = "5";
                //////}
                //////else if (code.ToLower() == "eeufl")
                //////{
                //////    CompanyId = "6";
                //////}
                //////else if (code.ToLower() == "exfl")
                //////{
                //////    CompanyId = "7";
                //////}

                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                ////request.Headers.Add("jw-token", "Bearer " + auth.token);
                ////////request.Headers.Add("company-id", auth.company.FirstOrDefault().id.ToString());
                ////request.Headers.Add("company-id", CompanyId);

                //byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                //request.ContentLength = byteArray.Length;

                //request.ContentType = "text/xml;charset=UTF-8";

                //NetworkCredential creds = GetCredentials();
                //request.Credentials = creds;

                //datastream.Write(byteArray, 0, byteArray.Length);
                //datastream.Close();

                WebResponse response = request.GetResponse();
                Stream datastream = response.GetResponseStream();

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
        
        #endregion

        #region Shumi HotCake Ctg

        private void TableValidationCtg(DataTable salesData, IntegrationParam paramVM)
        {



            List<string> oldColumnNames = new List<string> { "BranchCode", "CustomerCode", "DeliveryAddress", "InvoiceDateTime", "ReferenceNo", "PreviousInvoiceNo", "CurrencyCode", "VehicleNo", "IsPrint"
            ,"LCNumber","SaleType","ItemCode","NBRPrice","SDRate","VATRate","VATAmount","DiscountAmount","PromotionalQuantity","VATName","TenderId","CommercialName"};

            List<string> newColumnNames = new List<string> { "Branch_Code", "Customer_Code", "Delivery_Address", "Invoice_Date_Time", "Reference_No", "Previous_Invoice_No", "Currency_Code", "Vehicle_No", "Is_Print"
            ,"LC_Number","Sale_Type","Item_Code","NBR_Price","SD_Rate","VAT_Rate","VAT_Amount","Discount_Amount","Promotional_Quantity","VAT_Name","Tender_Id","ProductDescription"};

            salesData = OrdinaryVATDesktop.DtColumnNameChangeList(salesData, oldColumnNames, newColumnNames);


            var Customer_Name = new DataColumn("Customer_Name") { DefaultValue = "-" };
            var Item_Name = new DataColumn("Item_Name") { DefaultValue = "-" };
            var SL = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = paramVM.CurrentUser };

            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(Customer_Name);
            salesData.Columns.Add(Item_Name);
            salesData.Columns.Add(SL);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            List<string> ColumnNames = new List<string> { "BIN", "User" };
            OrdinaryVATDesktop.DtDeleteColumns(salesData, ColumnNames);
        }

        public DataTable GetSaleDataCtg(IntegrationParam param)
        {
            CommonDAL commonDal = new CommonDAL();

            try
            {
                DataTable dt = new DataTable();
                string code = commonDal.settingValue("CompanyCode", "Code");
                AuthModel vm = GetAuthentication();

                if (vm != null)
                {
                    string ActionUrl = @"/api/SaleInvoices/GetSaleHeaders";
                    CommonDto Common = new CommonDto();
                    Common.FromInvoiceDateTime = param.FromDate;
                    Common.ToInvoiceDateTime = param.ToDate;
                    var result = PostData(BasePssaleUrlCtg + ActionUrl, vm, JsonConvert.SerializeObject(Common));
                    RootModel<Invoice> InvoiceResult = JsonConvert.DeserializeObject<RootModel<Invoice>>(result);
                    if (InvoiceResult.StatusCode == "201")
                    {
                        if (InvoiceResult.Data.Count > 0)
                        {
                            dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(InvoiceResult.Data));
                        }
                    }

                }


                return dt;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public ResultVM SaveSale_ShumiHotCake_PreCtg(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SaleDAL salesDal = new SaleDAL();
            DataTable dt = new DataTable();

            try
            {

                AuthModel vm = GetAuthentication();

                if (vm != null)
                {
                    string ActionUrl = @"/api/SaleInvoices/GetSaleInvoices";
                    CommonDto Common = new CommonDto();
                    Common.FromInvoiceDateTime = paramVM.FromDate;
                    Common.ToInvoiceDateTime = paramVM.ToDate;

                    var Dataresult = PostData(BasePssaleUrlCtg + ActionUrl, vm, JsonConvert.SerializeObject(Common));
                    RootModel<SaleInvoice> InvoiceResult = JsonConvert.DeserializeObject<RootModel<SaleInvoice>>(Dataresult);
                    if (InvoiceResult.StatusCode == "201")
                    {
                        if (InvoiceResult.Data.Count > 0)
                        {
                            dt = SalesConvertToDataTable(InvoiceResult.Data);
                        }
                    }

                }
                TableValidationCtg(dt, paramVM);

                string[] result = salesDal.SaveAndProcess(dt, () => { }, Convert.ToInt32(paramVM.BranchId), "", connVM, paramVM, null, null, paramVM.CurrentUserId);

                rVM.Status = result[0];
                rVM.Message = result[1];

            }
            catch (Exception ex)
            {

                rVM.Message = ex.Message;
                throw ex;
            }
            finally
            {


            }
            return rVM;
        }


        public AuthModel GetAuthentication()
        {
            try
            {
                var keyValues = new Dictionary<string, string>();
                var key = "3eJHVm1fSEbf7xuIX9fS3Rf0qeVLH53wUXflYGgGGSVCxs0/WcKdtVJGDcSIjzi5JSWOt7vIpvgPyEz4bcI67Q==";
                var Grant_type = "password";


                keyValues.Add("ApiKey", key);
                keyValues.Add("Grant_type", Grant_type);
                string ActionUrl = @"/token";

                var result = PostFormUrlEncoded(BasePssaleUrlCtg + ActionUrl, new FormUrlEncodedContent(keyValues));

                return JsonConvert.DeserializeObject<AuthModel>(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        //public string PostData(string url, AuthModel auth, string payLoad)
        //{
        //    try
        //    {
        //        WebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = "POST";
        //        request.Headers.Add("Authorization", "Bearer " + auth.Access_token);
        //        byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
        //        request.ContentLength = byteArray.Length;
        //        request.ContentType = "application/json";


        //        Stream datastream = request.GetRequestStream();
        //        datastream.Write(byteArray, 0, byteArray.Length);
        //        datastream.Close();

        //        WebResponse response = request.GetResponse();
        //        datastream = response.GetResponseStream();

        //        StreamReader reader = new StreamReader(datastream);
        //        string responseMessage = reader.ReadToEnd();

        //        reader.Close();

        //        return responseMessage;

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public string PostData(string url, AuthModel auth, string payLoad)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + auth.Access_token);
                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";

                using (Stream datastream = request.GetRequestStream())
                {
                    datastream.Write(byteArray, 0, byteArray.Length);
                }

                using (WebResponse response = request.GetResponse())
                using (Stream datastream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(datastream))
                {
                    string responseMessage = reader.ReadToEnd();
                    return responseMessage;
                }
            }
            catch (Exception e)
            {

                return ""; // You can return null or some other value to indicate failure.
            }
        }

        public string PostFormUrlEncoded(string url, FormUrlEncodedContent formUrlEncodedContent)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(formUrlEncodedContent.ReadAsStringAsync().Result);
                request.ContentLength = byteArray.Length;

                request.ContentType = "application/x-www-form-urlencoded";

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

        public static DataTable SalesConvertToDataTable(List<SaleInvoice> saleInvoices)
        {
            DataTable dataTable = new DataTable("SaleInvoices");

            try
            {
                // Define the columns for the DataTable based on the Invoice class
                PropertyInfo[] invoiceProperties = typeof(Invoice).GetProperties();
                invoiceProperties = invoiceProperties.Take(invoiceProperties.Length - 1).ToArray();
                foreach (var property in invoiceProperties)
                {
                    dataTable.Columns.Add(property.Name, property.PropertyType);
                }

                // Define additional columns for the Item properties
                PropertyInfo[] itemProperties = typeof(SaleDetsils).GetProperties();
                foreach (var property in itemProperties)
                {
                    dataTable.Columns.Add(property.Name, property.PropertyType);
                }

                // Populate the DataTable with data from SaleInvoice objects and their Items
                if (saleInvoices != null)
                {
                    foreach (var saleInvoice in saleInvoices)
                    {
                        if (saleInvoice.Invoice.Items != null)
                        {
                            foreach (var item in saleInvoice.Invoice.Items)
                            {
                                DataRow row = dataTable.NewRow();

                                // Populate the row with data from Invoice properties
                                foreach (var property in invoiceProperties)
                                {
                                    row[property.Name] = property.GetValue(saleInvoice.Invoice);
                                }

                                // Populate the row with data from Item properties
                                foreach (var property in itemProperties)
                                {
                                    row[property.Name] = property.GetValue(item);
                                }

                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return dataTable;
        }

        public class RootModel<TModel> where TModel : class, new()
        {
            public string StatusCode { get; set; }

            public string Message { get; set; }
            public List<TModel> Data { get; set; }
        }

        public class AuthModel
        {
            public string Access_token { get; set; }

            public string Token_type { get; set; }

            public string Expires_in { get; set; }
        }


        public class SalesList
        {
            public SalesList()
            {
                Result = new ResultModel();
            }
            public ResultModel Result { get; set; }
            public List<SaleInvoice> Invoices { get; set; }
        }

        public class SaleInvoice
        {
            public Invoice Invoice { get; set; }
        }
        public class Invoice
        {

            public string BIN { get; set; }
            public string BranchCode { get; set; }
            public string User { get; set; }
            public string ID { get; set; }
            public string CustomerCode { get; set; }
            public string DeliveryAddress { get; set; }
            public string InvoiceDateTime { get; set; }
            public string ReferenceNo { get; set; }
            public string CurrencyCode { get; set; }
            public string VehicleNo { get; set; }
            public string VehicleType { get; set; }
            public string SaleType { get; set; }
            public string TransactionType { get; set; }
            public string IsPrint { get; set; }
            public string Post { get; set; }
            public string LCNumber { get; set; }
            public string TenderId { get; set; }
            public string PreviousInvoiceNo { get; set; }
            public string Comments { get; set; }
            public string LCBank { get; set; }
            public string LCDate { get; set; }
            public string PINo { get; set; }
            public string PIDate { get; set; }
            public string BOe { get; set; }
            public string EXPFormDate { get; set; }
            public string InvoiceDiscountAmount { get; set; }

            public List<SaleDetsils> Items { get; set; }
        }
        public class SaleDetsils
        {
            public string ItemCode { get; set; }
            public string UOM { get; set; }
            public decimal Quantity { get; set; }
            public decimal NBRPrice { get; set; }
            public decimal SubTotal { get; set; }
            public decimal SDRate { get; set; }
            public decimal SDAmount { get; set; }
            public decimal VATRate { get; set; }
            public decimal VATAmount { get; set; }
            public string Type { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal PromotionalQuantity { get; set; }
            public string VATName { get; set; }
            public string CommercialName { get; set; }
            public string Weight { get; set; }
            public string CPCName { get; set; }
            public string CommentsD { get; set; }
            public string PreviousInvoiceDateTime { get; set; }
            public decimal PreviousNBRPrice { get; set; }
            public decimal PreviousQuantity { get; set; }
            public decimal PreviousSubTotal { get; set; }
            public decimal PreviousSD { get; set; }
            public decimal PreviousSDAmount { get; set; }
            public decimal PreviousVATRate { get; set; }
            public decimal PreviousVATAmount { get; set; }
            public string PreviousUOM { get; set; }
            public string ReasonOfReturn { get; set; }
        }
        public class Sales
        {
            public Sales()
            {
                Result = new ResultModel();
            }
            public ResultModel Result { get; set; }
            public List<Invoice> Invoices { get; set; }
        }
        public class ResultModel
        {
            public ResultModel()
            {
                Status = ResultStatus.Undefined;
            }

            public ResultStatus Status { get; set; }
            public string Message { get; set; }
            public Exception Exception { get; set; }
        }
        public enum ResultStatus
        {
            Success,
            Fail,
            Warning,
            Undefined
        }
        public class CommonDto
        {
            public string FromInvoiceDateTime { get; set; }
            public string ToInvoiceDateTime { get; set; }
            public string branchId { get; set; }

        }
        #endregion
    }
}
