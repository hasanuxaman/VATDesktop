
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
    public class DecathlonIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        BranchProfileDAL _branchDAL = new BranchProfileDAL();

        #region API Connection

        //////private string saleUrl = "https://zvatapi.bergerbd.com:8081/api/GetVatInfo";
        //////private string saleUrl_InvoiceNumebr = "https://zvatapi.bergerbd.com/api/GetVatInfoByInvoiceNumebr";

        //private string saleUrl = "http://103.123.11.189/Decathlon/api/GetSalesByInvoice/xls/10001";
        private string saleUrl = "http://36.255.70.93/Decathlon/api/GetSalesByInvoice/xls/10001";
        //private string productUrl = "http://103.123.11.189/Decathlon/api/app/Product/10001/";
        private string productUrl = "http://36.255.70.93/Decathlon/api/app/Product/10001/";
        ////private string productUrl11 = "http://103.123.11.189/Decathlon/api/app/Product/10001/20180201103524";
        private string purchaseUrl = "http://36.255.70.93/Decathlon/api/GetPurchaseReceiveDetailsByChallanNo/10001";
        private string transferUrl = "http://36.255.70.93/Decathlon/api/GetStoreDeilveryDetailsByChallanNo/10001";
        private string purchaseUrl1 = "http://36.255.70.93/Decathlon/api/GetPurchaseReceiveDetailsByChallanNo/10001/100010001/R00012102110001/01-feb-2021/20-feb-2021";
        ////private string saleUrl = "http://103.123.11.189/Decathlon/api/GetSalesByInvoice/xls/10001/15-jun-2021/15-jun2021";
        //////private string saleUrl = "http://103.123.11.189/Decathlon/api/GetSalesByInvoice/xls/10001/15-jun-2021/15-jun2021/100010002/1121061500039";
        private string saleUrl_InvoiceNumebr = "http://36.255.70.93/Decathlon/api/GetSalesByInvoice/xls/10001/15-jun-2021/15-jun2021/100010002/";

        #endregion

        private AuthRoot Auth = new AuthRoot();
        private DecathlonParam DecathlonParam = new DecathlonParam();

        public DecathlonIntegrationDAL()
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
                            int count = GetData.Rows.Count;

                            sqlResults = SaveSaleData(GetData, paramVM, connVM);

                            if (sqlResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(sqlResults[1]);
                            }

                        }


                    }

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

                dt = GetSaleDataAPI(param);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }

                //dt.Columns.Remove("TransactionType");
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
ReasonOfReturn varchar(100),
TransactionType varchar(50)
)";

                #endregion

//                #region UpdateTypeVATRate

//                string UpdateTypeVATRate = @"
//----------- VAT Rate Update --------------
//update #saletemp set #saletemp.VAT_Rate=Products.VATRate , #saletemp.UOM= Products.UOM, #saletemp.SD_Rate= Products.SD
//from Products where #saletemp.Item_Code=Products.ProductCode
//
//----------- Type Update --------------
//
//update #saletemp set #saletemp.[Type]='VAT'
// where #saletemp.VAT_Rate='15'
//
// update #saletemp set #saletemp.[Type]='NonVAT'
// where #saletemp.VAT_Rate='0'
//
// update #saletemp set #saletemp.[Type]='OtherRate'
// where #saletemp.VAT_Rate!='0' and #saletemp.VAT_Rate!='15' 
//
// --update #saletemp set #saletemp.NBR_Price = ((((#saletemp.NBR_Price * #saletemp.Quantity)- #saletemp.Discount_Amount)/#saletemp.Quantity)
// --*100/(100+#saletemp.VAT_Rate))
//
// --update #saletemp set #saletemp.Discount_Amount=0
//
//";


//                #endregion

                #region VAT Calculation query

                string GetDataForVATCalculation = @"

select 
 ID
,Branch_Code
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
,SD_Rate

from #saletemp
where 1=1 
and Quantity>0

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
,TransactionType

from #saletemp

drop table #saletemp

";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                //cmd.CommandText = UpdateTypeVATRate;
                //cmd.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                #region VAT Calculation

                if (code.ToLower() == "DECATHLON".ToLower())
                {
                    cmd.CommandText = GetDataForVATCalculation;
                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {

                            string ID = row["ID"].ToString();
                            string Branch_Code = row["Branch_Code"].ToString();
                            string Item_Code = row["Item_Code"].ToString();

                            decimal SubTotal = Convert.ToDecimal(row["SubTotal"].ToString());
                            decimal VAT_Rate = Convert.ToDecimal(row["VAT_Rate"].ToString());
                            decimal SD_Rate = Convert.ToDecimal(row["SD_Rate"].ToString());
                            decimal Quantity = Convert.ToDecimal(row["Quantity"].ToString());

                            VATCalculationVM vm = new VATCalculationVM();
                            vm.InpTotalAmount = SubTotal;
                            vm.InpVAT_Rate = VAT_Rate;
                            vm.InpSD_Rate = SD_Rate;

                            VATCalculationVM OutVM = new CommonDAL().IncludedVATCalculation(vm);

                            decimal NBRPrice = OutVM.OutSubTotal / Quantity;


                            string SubTotalUpdate = @"

update #saletemp set NBR_Price=@NBR_Price,SubTotal=@SubTotal,VAT_Amount=@VAT_Amount,SDAmount=@SDAmount 
where ID=@ID and Branch_Code=@Branch_Code and Item_Code=@Item_Code

";
                            cmd = new SqlCommand(SubTotalUpdate, currConn, transaction);
                            cmd.Parameters.AddWithValue("@NBR_Price", NBRPrice);
                            cmd.Parameters.AddWithValue("@SubTotal", OutVM.OutSubTotal);
                            cmd.Parameters.AddWithValue("@VAT_Amount", OutVM.OutVAT_Amount);
                            cmd.Parameters.AddWithValue("@SDAmount", OutVM.OutSDAmount);
                            cmd.Parameters.AddWithValue("@ID", ID);
                            cmd.Parameters.AddWithValue("@Branch_Code", Branch_Code);
                            cmd.Parameters.AddWithValue("@Item_Code", Item_Code);

                            cmd.ExecuteNonQuery();

                        }

                    }

                }

                #endregion

                cmd.CommandText = getAllData;
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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
                string IntBranchCode = GetBranchcode(param.BranchId, connVM);
                List<DataRoot_Decathlon> apiData;

                apiData = GetSaleApiData(Auth, param);

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


                #endregion

                if (apiData != null)
                {

                    if (param.TransactionType == "Credit")
                    {

                        #region Credit Note

                        //    foreach (var items in apiData.items)
                        //    {
                        //        string ID = items.order.invoice_no.ToString();
                        //        string Branch_Code = items.location.id.ToString();
                        //        string Invoice_Date_Time = items.order.invoice_date.ToString("yyyy-MM-dd HH:mm:ss");
                        //        string DeliveryDateTime = items.approved_at.ToString("yyyy-MM-dd HH:mm:ss");
                        //        string Customer_Code = items.order.customer.customer_code;
                        //        string Customer_Name = items.order.customer.company_name;
                        //        string ReasonOfReturn = items.description;
                        //        string Delivery_Address = items.location.delivery_address;

                        //        foreach (var Lineitems in items.sale_return_lines)
                        //        {

                        //            OrderLine orderLine = items.order.order_lines
                        //                .FirstOrDefault(x => x.product.product_code == Lineitems.product.product_code);

                        //            DataRow dr = dt.NewRow();
                        //            dr["ID"] = ID;
                        //            dr["Branch_Code"] = Branch_Code;
                        //            dr["Customer_Code"] = Customer_Code;
                        //            dr["Customer_Name"] = Customer_Name;
                        //            dr["Invoice_Date_Time"] = Invoice_Date_Time;
                        //            dr["Reference_No"] = ID;
                        //            dr["Item_Code"] = Lineitems.product.product_code;
                        //            dr["Item_Name"] = Lineitems.product.name;
                        //            dr["UOM"] = Lineitems.product.unit.name;
                        //            dr["Quantity"] = Convert.ToDecimal(Lineitems.quantity);
                        //            ////dr["VAT_Rate"] = orderLine.vat;

                        //            decimal VATRate = 0;

                        //            if (orderLine.vat != 0)
                        //            {
                        //                VATRate = 15;
                        //            }
                        //            dr["VAT_Rate"] = VATRate;

                        //            if (VATRate == 15)
                        //            {
                        //                dr["Type"] = "VAT";
                        //            }
                        //            else if (VATRate == 0)
                        //            {
                        //                dr["Type"] = "NoNVAT";
                        //            }
                        //            else
                        //            {
                        //                dr["Type"] = "OtherRate";
                        //            }

                        //            dr["SubTotal"] = "0";
                        //            ////dr["SubTotal"] = orderLine.total_price;
                        //            dr["Discount_Amount"] = Lineitems.discount;
                        //            dr["Comments"] = "-";
                        //            dr["NBR_Price"] = Lineitems.rate;
                        //            dr["Delivery_Address"] = Delivery_Address;
                        //            dr["Promotional_Quantity"] = "0";
                        //            dr["Sale_Type"] = "New";
                        //            dr["Vehicle_No"] = "NA";
                        //            dr["VehicleType"] = "NA";

                        //            dr["SD_Rate"] = "0";
                        //            dr["SDAmount"] = "0";
                        //            dr["Is_Print"] = "N";
                        //            dr["Tender_Id"] = "0";
                        //            dr["Post"] = "N";
                        //            dr["LC_Number"] = "NA";
                        //            dr["Currency_Code"] = "BDT";
                        //            dr["CommentsD"] = "NA";

                        //            dr["Non_Stock"] = "N";
                        //            dr["Trading_MarkUp"] = "0";

                        //            #region Comments

                        //            ////if (orderLine.vat == 15)
                        //            ////{
                        //            ////    dr["Type"] = "VAT";
                        //            ////}
                        //            ////else if (orderLine.vat == 0)
                        //            ////{
                        //            ////    dr["Type"] = "NoNVAT";
                        //            ////}
                        //            ////else
                        //            ////{
                        //            ////    dr["Type"] = "OtherRate";
                        //            ////}

                        //            #endregion

                        //            dr["VAT_Name"] = "VAT 4.3";

                        //            dr["Previous_Invoice_No"] = ID;
                        //            dr["PreviousInvoiceDateTime"] = Invoice_Date_Time;

                        //            dr["PreviousNBRPrice"] = orderLine.rate;
                        //            dr["PreviousQuantity"] = orderLine.quantity;

                        //            dr["PreviousSubTotal"] = orderLine.total_price;
                        //            dr["PreviousVATAmount"] = "0";
                        //            dr["PreviousVATRate"] = orderLine.vat;
                        //            dr["PreviousSD"] = "0";
                        //            dr["PreviousSDAmount"] = "0";
                        //            dr["ReasonOfReturn"] = ReasonOfReturn;

                        //            dt.Rows.Add(dr);

                        //        }

                        //    }
                        #endregion

                    }
                    else
                    {
                        #region Sales

                        foreach (var items in apiData)
                        {
                            string ID = items.INVOICE_NO;
                            //string Branch_Code = items.STORE_CODE;
                            //string Branch_Code = "100010002";
                            //string Branch_Code = "100010" + param.dtConnectionInfo.Rows[0]["BranchCode"].ToString();
                            string Branch_Code = IntBranchCode;

                            string Invoice_Date_Time = items.INVOICE_DT.ToString("yyyy-MM-dd HH:mm:ss");
                            string DeliveryDateTime = items.INVOICE_DT.ToString("yyyy-MM-dd HH:mm:ss");
                            string Customer_Code = items.CUSTOMER_CODE;
                            string Customer_Name = items.CUSTOMER_NAME;
                            if (string.IsNullOrEmpty(Customer_Name))
                            {
                                Customer_Name = "";
                            }
                            ////string Comments = items.DeliveryDoc + "~" + items.BillingDoc;
                            string Comments = "-";
                            //string Delivery_Address = items.DeliveryAddress;
                            string Delivery_Address = "-";

                            string transactionType = "-";

                            string firstTwoChars = ID.Substring(0, 2);

                            if (firstTwoChars.ToLower()=="cr")
                            {
                                transactionType = "Credit";
                            }
                            else
                            {
                                transactionType = "Other";
                            }

                            //if(items.IsCreditNote==true || items.IsCashReturnDone==true)
                            //{
                            //   transactionType = "Credit";
                            //}
                            //else
                            //{
                            //   transactionType = "Other";
                            //}

                            ////foreach (var Lineitems in items.order_lines)
                            ////{
                            DataRow dr = dt.NewRow();
                            dr["ID"] = ID;
                            dr["Branch_Code"] = Branch_Code;
                            dr["Customer_Code"] = Customer_Code;
                            dr["Customer_Name"] = Customer_Name;
                            dr["Invoice_Date_Time"] = Invoice_Date_Time;
                            dr["Reference_No"] = ID;
                            dr["Item_Code"] = items.BARCODE;
                            dr["Item_Name"] = items.DISPLAY_NAME;
                            //dr["UOM"] = items.UoM;
                            //dr["UOM"] = "Pcs";
                            dr["UOM"] = "-";

                            if (transactionType == "Credit")
                            {
                                dr["Quantity"] = Convert.ToDecimal(items.RQTY);
                                dr["SubTotal"] = items.RTN_AMT;
                                dr["Sale_Type"] = "Credit";

                            }
                            else
                            {
                                dr["Quantity"] = Convert.ToDecimal(items.SQTY);
                                dr["SubTotal"] = items.NET_AMT;
                                dr["Sale_Type"] = "New";
                            }

                            dr["VAT_Rate"] = items.VAT_PRCNT;
                         
                            //dr["VAT_Amount"] = Convert.ToDecimal(items.VAT_AMT);
                            dr["VAT_Amount"] = "0";
                            dr["SDAmount"] = Convert.ToDecimal(items.SD_AMT);

                            //////////////dr["VAT_Rate"] = Lineitems.vat;
                            ////dr["Currency_Code"] = items.waerk;
                            dr["Currency_Code"] = "BDT";
                            //dr["SubTotal"] = "0";                         
                            dr["Discount_Amount"] = items.DISC_AMT;
                            dr["Comments"] = Comments;

                            //decimal  NBR_Price = items.MRP / 115 * 100;
                            //dr["NBR_Price"] = NBR_Price;
                            dr["NBR_Price"] = items.MRP;

                            dr["Delivery_Address"] = Delivery_Address;

                            //dr["TransactionType"] = param.TransactionType;
                            dr["TransactionType"] = transactionType;
                            dr["CompanyCode"] = code;

                            //decimal VATRate = 15;
                            decimal VATRate = 0;

                            //if (items.vat != 0)
                            //{
                            //    VATRate = 15;
                            //}
                            //dr["VAT_Rate"] = VATRate;
                            VATRate = Convert.ToDecimal(dr["VAT_Rate"]);

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

                            //////////dr["SubTotal"] = "0";

                            dr["Promotional_Quantity"] = "0";                          
                            dr["Vehicle_No"] = "NA";
                            dr["VehicleType"] = "NA";
                            //////////////////dr["TransactionType"] = param.TransactionType;

                            dr["SD_Rate"] = "0";
                            dr["Is_Print"] = "N";
                            dr["Tender_Id"] = "0";
                            dr["Post"] = "N";
                            dr["LC_Number"] = "NA";
                            dr["CommentsD"] = "NA";

                            dr["Non_Stock"] = "N";
                            dr["Trading_MarkUp"] = "0";

                            #region Comments

                            //////if (Lineitems.vat == 15)
                            //////{
                            //////    dr["Type"] = "VAT";
                            //////}
                            //////else if (Lineitems.vat == 0)
                            //////{
                            //////    dr["Type"] = "NoNVAT";
                            //////}
                            //////else
                            //////{
                            //////    dr["Type"] = "OtherRate";
                            //////}
                            #endregion

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

                            ////}

                        }

                        #endregion

                    }

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


        private List<DataRoot_Decathlon> GetSaleApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null)
        {

            string url = "";

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");
            string branchCode = param.dtConnectionInfo.Rows[0]["BranchCode"].ToString();
            string IntBranchCode = GetBranchcode(param.BranchId, connVM);

            if (!string.IsNullOrWhiteSpace(param.FromDate))
            {
                url = saleUrl + "/" + param.FromDate;
            }
           
            if (!string.IsNullOrWhiteSpace(param.ToDate))
            {
                url = url + "/" + param.ToDate;
            }

            if (!string.IsNullOrWhiteSpace(IntBranchCode))
            {
                //url = url + "/" + "100010" + branchCode;
                url = url + "/" + IntBranchCode;
            }

            if (!string.IsNullOrWhiteSpace(param.RefNo))
            {
                url = url + "/" + param.RefNo;

            }
            else
            {
                url = url +"/" + "{invoiceNo}";
                //url = url + "/1121061500039";

            }

           
            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "symphony:LLEKCOp3omA+Nfu0a/ywyQ==";

            //////key = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") +
            //////    DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") +
            //////    ApiKey;
            ////////key = ApiKey;

            //////byte[] bytes = Encoding.UTF8.GetBytes(key);

            //////key = Convert.ToBase64String(bytes);

            DecathlonParam = new DecathlonParam();

            ////DecathlonParam.FromDate = param.FromDate;
            ////DecathlonParam.ToDate = param.ToDate;
            ////DecathlonParam.InvoiceNumber = param.RefNo;
            ////DecathlonParam.DepotId = param.BranchCode;
            //////////DecathlonParam.CompanyId = "1000";
            //////bergerParam.ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";
            ////DecathlonParam.ApiKey = key;



            string apiKeyValue = "";
            ////if (!string.IsNullOrWhiteSpace(DecathlonParam.InvoiceNumber))
            ////{
            ////    apiKeyValue = "InvoiceNumber=" + DecathlonParam.InvoiceNumber + "&ApiKey=" + DecathlonParam.ApiKey;
            ////}
            ////else
            ////{
            ////    ////apiKeyValue = "DepotId=" + bergerParam.DepotId + "&CompanyId=" + bergerParam.CompanyId
            ////    ////+ "&FromDate=" + bergerParam.FromDate + "T00:00:00" + "&ToDate=" + bergerParam.ToDate + "T00:00:00" 
            ////    ////+ "&ApiKey=" + bergerParam.ApiKey;

            ////    apiKeyValue = "DepotId=" + DecathlonParam.DepotId + "&CompanyId=" + DecathlonParam.CompanyId
            ////       + "&FromDate=" + DecathlonParam.FromDate + "&ToDate=" + DecathlonParam.ToDate + "&ApiKey=" + DecathlonParam.ApiKey;
            
            
            ////}


            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, DecathlonParam, apiKeyValue);

            List<DataRoot_Decathlon> salesApiData = JsonConvert.DeserializeObject <List<DataRoot_Decathlon>>(result);
            //////Root ApiData = JsonConvert.DeserializeObject<Root>(salesApiData.data);

            return salesApiData;
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

        public ResultVM SaveSale_Decathlon_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
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

        #region Purchase data
        public DataTable GetPurchaseData(IntegrationParam param, SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string IsProcess = "N")
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

                dt = GetPurchseDataAPI(param, connVM);

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
create table #purchasetemp(              
sl int identity(1,1), 
ID varchar(6000),
BranchCode varchar(100),
Vendor_Code varchar(100),
Vendor_Name varchar(100),
Receive_Date varchar(100),
Invoice_Date varchar(100),
Referance_No varchar(100),
LC_No varchar(100),
BE_Number varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
UOM varchar(100),
VAT_Amount decimal(25,7),
Total_Price decimal(25,7),
Type varchar(100),
SD_Amount decimal(25,7),
Assessable_Value decimal(25, 2),
CD_Amount decimal(25,7),
RD_Amount decimal(25,7),
AT_Amount decimal(25,7),
AITAmount decimal(25,7),
Others_Amount decimal(25,7),
Remarks varchar(100),
With_VDS varchar(100),
Comments varchar(200),
Post varchar(100),
Rebate_Rate decimal(25,7),
VATRate decimal(25,7),
IsProcess varchar(100)
)";

                #endregion

                #region UpdateIsProcessFlg

                string UpdateIsProcessFlg = @"

update #purchasetemp set IsProcess='N'

update #purchasetemp set IsProcess='Y' from PurchaseInvoiceHeaders where #purchasetemp.ID =PurchaseInvoiceHeaders.ImportIDExcel

";

                #endregion

                #region getAllData

                string getAllData = @"

select 
 ID
,BranchCode
,Vendor_Code
,Vendor_Name
,Receive_Date
,Invoice_Date
,Referance_No
,LC_No
,BE_Number
,Item_Code
,Item_Name
,UOM
,Quantity
,VAT_Amount
,Total_Price
,Type
,SD_Amount
,Assessable_Value
,CD_Amount
,RD_Amount
,AT_Amount
,AITAmount
,Others_Amount
,Remarks
,With_VDS
,Comments
,Post
,Rebate_Rate
,VATRate

from #purchasetemp
where 1=1 

";
                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    getAllData += @" and IsProcess=@IsProcess";
                }

                getAllData += @" drop table #purchasetemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#purchasetemp", dt, currConn, transaction);
                //result = commonDal.BulkInsert("SalesTempData", dt, currConn, transaction);

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();


                cmd.CommandText = getAllData;
                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    cmd.Parameters.AddWithValue("@IsProcess", IsProcess);
                }
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
        }

        public DataTable GetPurchseDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string IntBranchCode = GetBranchcode(param.BranchId, connVM);

                //DataRoot apiData;

                string limit = "1";
                List<DataRoot_DecathlonPur> apiData;

                //apiData = GetPurchaseApiData(Auth, param, limit, connVM, IntBranchCode);
                apiData = GetPurchaseApiData(Auth, param,limit,connVM,IntBranchCode);

                //string total = "1";
                //if (apiData.total != 0)
                //{
                //    total = apiData.total.ToString();
                //}

                ////apiData = GetPurchaseApiData(Auth, param, total, connVM, IntBranchCode);
                //apiData = GetPurchaseApiData(Auth, param, total, connVM);


                // convert api data to VAT system format 

                #region Create DataTable

                //CommonDAL commonDal = new CommonDAL();

                //string code = commonDal.settingValue("CompanyCode", "Code");


                string[] result = new[] { "Fail" };

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("ID");
                dt.Columns.Add("BranchCode");
                dt.Columns.Add("Vendor_Code");
                dt.Columns.Add("Vendor_Name");
                dt.Columns.Add("Receive_Date");
                dt.Columns.Add("Invoice_Date");
                dt.Columns.Add("Referance_No");
                dt.Columns.Add("LC_No");
                dt.Columns.Add("BE_Number");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("UOM");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Total_Price");
                dt.Columns.Add("Type");
                dt.Columns.Add("SD_Amount");
                dt.Columns.Add("Assessable_Value");
                dt.Columns.Add("CD_Amount");
                dt.Columns.Add("RD_Amount");
                dt.Columns.Add("AT_Amount");
                dt.Columns.Add("AITAmount");
                dt.Columns.Add("Others_Amount");
                dt.Columns.Add("Remarks");
                dt.Columns.Add("With_VDS");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Post");
                dt.Columns.Add("Rebate_Rate");
                dt.Columns.Add("VATRate");


                #endregion

                if (apiData != null)
                {
                    if (param.TransactionType == "Import")
                    {
                        #region Import Purchase

                        //foreach (var items in apiData)
                        //{
                        //    string ID = items.id;
                        //    string Referance_No = items.id;
                        //    //////string Branch_Code = items.location.id.ToString();
                        //    string Invoice_Date = items.invoice_at.ToString("yyyy-MM-dd HH:mm:ss");
                        //    string Receive_Date = items.qc_approved_at.ToString("yyyy-MM-dd HH:mm:ss");

                        //    string Vendor_Code = items.purchase.customer.customer_code;
                        //    string Vendor_Name = items.purchase.customer.company_name;
                        //    string Comments = items.remark;

                        //    string CommercialId = items.commercial_invoice.id.ToString();

                        //    foreach (var Lineitems in items.purchase_receive_lines)
                        //    {

                        //        DataRow dr = dt.NewRow();

                        //        dr["ID"] = ID;
                        //        dr["BranchCode"] = Lineitems.purchase_line.requisition_line.requisition.location.id;
                        //        dr["Vendor_Code"] = Vendor_Code;
                        //        dr["Vendor_Name"] = Vendor_Name;

                        //        dr["BE_Number"] = "-";
                        //        dr["Receive_Date"] = Receive_Date;
                        //        dr["Invoice_Date"] = Invoice_Date;
                        //        dr["Referance_No"] = Referance_No;
                        //        dr["LC_No"] = "-";
                        //        dr["Item_Code"] = Lineitems.purchase_line.requisition_line.product.product_code;
                        //        dr["Item_Name"] = Lineitems.purchase_line.requisition_line.product.name;
                        //        dr["UOM"] = Lineitems.purchase_line.requisition_line.product.unit.name;

                        //        decimal Quantity = Lineitems.receive_quantity;
                        //        decimal chalan_quantity = Lineitems.chalan_quantity;

                        //        decimal Discount = 0;
                        //        if (Lineitems.invoice_discount != null)
                        //        {
                        //            Discount = Lineitems.invoice_discount.Value;
                        //        }
                        //        decimal Rate = Lineitems.purchase_line.rate;
                        //        decimal VatRate = 0;

                        //        if (Lineitems.purchase_line.vat_percent != null)
                        //        {
                        //            VatRate = Lineitems.purchase_line.vat_percent.Value;
                        //        }

                        //        decimal Total_Price = 0;
                        //        decimal VAT_Amount = 0;

                        //        if (chalan_quantity < Quantity)
                        //        {
                        //            Total_Price = (chalan_quantity * Rate) - Discount;
                        //        }
                        //        else
                        //        {
                        //            Total_Price = (Quantity * Rate) - Discount;
                        //        }

                        //        if (chalan_quantity < Quantity)
                        //        {
                        //            VAT_Amount = (chalan_quantity * Rate) * VatRate;
                        //        }
                        //        else
                        //        {
                        //            VAT_Amount = (Quantity * Rate) * VatRate;
                        //        }


                        //        string Type;

                        //        if (VatRate == 15)
                        //        {
                        //            Type = "VAT";
                        //        }
                        //        else if (VatRate == 0)
                        //        {
                        //            Type = "Exempted";
                        //        }
                        //        else
                        //        {
                        //            Type = "OtherRate";
                        //        }

                        //        dr["Quantity"] = Quantity;
                        //        dr["VAT_Amount"] = VAT_Amount;
                        //        dr["Total_Price"] = Total_Price;

                        //        dr["Type"] = Type;
                        //        dr["SD_Amount"] = 0;

                        //        dr["Assessable_Value"] = 0;
                        //        dr["CD_Amount"] = 0;
                        //        dr["RD_Amount"] = 0;
                        //        dr["AT_Amount"] = 0;
                        //        dr["AITAmount"] = 0;

                        //        dr["Others_Amount"] = 0;
                        //        dr["Remarks"] = Comments;
                        //        dr["With_VDS"] = "N";
                        //        dr["Comments"] = Comments;
                        //        dr["Post"] = "N";
                        //        dr["Rebate_Rate"] = 0;
                        //        //dr["VAT_Rate"] = VatRate;


                        //        dt.Rows.Add(dr);

                        //    }

                        //}

                        #endregion
                    }
                    else
                    {
                        #region Purchase

                        foreach (var items in apiData)
                        {
                            string ID = items.MEMO_NO;
                            ////////string Branch_Code = "100010" + param.dtConnectionInfo.Rows[0]["BranchCode"].ToString();
                            string Branch_Code = items.STORE_CODE;
                            string Referance_No = items.MEMO_NO;
                            //string Branch_Code = items.location.id.ToString();
                            string Invoice_Date = items.PURCHASE_DATE.ToString("yyyy-MM-dd HH:mm:ss");
                            string Receive_Date = items.PURCHASE_DATE.ToString("yyyy-MM-dd HH:mm:ss");

                            //string Vendor_Code = items.purchase.customer.customer_code;
                            //string Vendor_Name = items.purchase.customer.company_name;

                            //string Vendor_Code = items.VENDOR_CODE;
                            string Vendor_Code = "VEN-009";
                            //string Vendor_Name = items.VENDOR_NAME;
                            string Vendor_Name = "CEVA Freight (Bangladesh) Co. Ltd";
                            string Comments = "-";
                            decimal Quantity = items.PUR_QTY;
                            decimal VatRate = items.VAT_PRCNT;
                            decimal VAT_Amount = items.VAT;
                            decimal Total_Price = items.AMOUNT;


                            //foreach (var Lineitems in items)
                            //{

                                DataRow dr = dt.NewRow();
                                dr["ID"] = ID;
                                //dr["BranchCode"] = Lineitems.purchase_line.requisition_line.requisition.location.id;
                                dr["BranchCode"] = Branch_Code;
                                dr["Vendor_Code"] = Vendor_Code;
                                dr["Vendor_Name"] = Vendor_Name;

                                dr["BE_Number"] = items.MEMO_NO;
                                dr["Receive_Date"] = Receive_Date;
                                dr["Invoice_Date"] = Invoice_Date;
                                dr["Referance_No"] = Referance_No;
                                dr["LC_No"] = "-";
                                dr["Item_Code"] = items.BARCODE;
                                dr["Item_Name"] = items.DISPLAY_NAME;
                                dr["UOM"] = items.PUR_UOM_NAME;


                                //decimal chalan_quantity = Lineitems.chalan_quantity;

                                //decimal Discount = 0;
                                //if (Lineitems.invoice_discount != null)
                                //{
                                //    Discount = Lineitems.invoice_discount.Value;
                                //}
                                //////decimal Discount = Lineitems.invoice_discount;
                                //decimal Rate = Lineitems.purchase_line.rate;
                                //decimal VatRate = 0;

                                //if (Lineitems.purchase_line.vat_percent != null)
                                //{
                                //    VatRate = Lineitems.purchase_line.vat_percent.Value;
                                //}

                                //decimal Total_Price = 0;
                                //decimal VAT_Amount = 0;

                                //if (chalan_quantity < Quantity)
                                //{
                                //    Total_Price = (chalan_quantity * Rate) - Discount;
                                //}
                                //else
                                //{
                                //    Total_Price = (Quantity * Rate) - Discount;
                                //}

                                //if (chalan_quantity < Quantity)
                                //{
                                //    VAT_Amount = (chalan_quantity * Rate) * VatRate / 100;
                                //}
                                //else
                                //{
                                //    VAT_Amount = (Quantity * Rate) * VatRate / 100;
                                //}


                                string Type;

                                if (VatRate == 15)
                                {
                                    Type = "VAT";
                                }
                                else if (VatRate == 0)
                                {
                                    Type = "Exempted";
                                }
                                else
                                {
                                    Type = "OtherRate";
                                }

                                dr["Quantity"] = Quantity;
                                dr["VAT_Amount"] = VAT_Amount;
                                dr["Total_Price"] = Total_Price;

                                dr["Type"] = Type;
                                dr["SD_Amount"] = 0;

                                dr["Assessable_Value"] = 0;
                                dr["CD_Amount"] = 0;
                                dr["RD_Amount"] = 0;
                                dr["AT_Amount"] = 0;
                                dr["AITAmount"] = 0;

                                dr["Others_Amount"] = 0;
                                dr["Remarks"] = Comments;
                                dr["With_VDS"] = "N";
                                dr["Comments"] = Comments;
                                dr["Post"] = "N";
                                dr["Rebate_Rate"] = 0;
                                dr["VATRate"] = VatRate;


                                dt.Rows.Add(dr);

                            //}

                        }

                        #endregion
                    }



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

            }
        }

        private List<DataRoot_DecathlonPur> GetPurchaseApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url = "";

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");
            string branchCode = param.dtConnectionInfo.Rows[0]["BranchCode"].ToString();

            if (!string.IsNullOrWhiteSpace(IntBranchCode))
            {
                //url = purchaseUrl + "/" + "100010" + branchCode;
                url = purchaseUrl + "/" + IntBranchCode;
            }

            if (!string.IsNullOrWhiteSpace(param.RefNo))
            {
                url = url + "/" + param.RefNo;

            }

            else
            {
                url = url + "/" + "{memo_no}";
                //url = url + "/1121061500039";

            }
            

            if (!string.IsNullOrWhiteSpace(param.FromDate))
            {
                url = url + "/" + param.FromDate;
            }

            if (!string.IsNullOrWhiteSpace(param.ToDate))
            {
                url = url + "/" + param.ToDate;
            }


            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "symphony:LLEKCOp3omA+Nfu0a/ywyQ==";



            DecathlonParam = new DecathlonParam();

            string apiKeyValue = "";
          
            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, DecathlonParam, apiKeyValue);

            List<DataRoot_DecathlonPur> purchaseApiData = JsonConvert.DeserializeObject<List<DataRoot_DecathlonPur>>(result);

            return purchaseApiData;
        }

        public ResultVM SavePurchase_Decathlon_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            PurchaseDAL purchaseDal = new PurchaseDAL();
            try
            {

                //DataTable PurchaseData = GetPurchaseData(paramVM, connVM);
                DataTable PurchaseData = GetPurchseDataAPI(paramVM, connVM);

                if (PurchaseData.Columns.Contains("VATRate"))
                {
                    PurchaseData.Columns.Remove("VATRate");
                }

                string[] result = purchaseDal.SaveTempPurchase(PurchaseData, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, Convert.ToInt32(paramVM.BranchId), () => { }, null, null, connVM);


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

        #region Products

        public DataTable GetProductDataAPI(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                ////List<DataRoot_DecathlonProduct> apiData;
                DataRoot_DecathlonProduct apiData;

                apiData = GetProductApiData(Auth);

                // convert api data to VAT system format 

                #region Create DataTable

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("SL");
                dt.Columns.Add("ProductCode");
                ////dt.Columns.Add("InvoiceNo");
                dt.Columns.Add("ProductName");
                dt.Columns.Add("ProductGroup");
                dt.Columns.Add("UOM");
                dt.Columns.Add("HSCode");
                dt.Columns.Add("UnitPrice");
                dt.Columns.Add("SDRate");
                dt.Columns.Add("VATRate");
                dt.Columns.Add("Description");
                dt.Columns.Add("IsProcessed");
                dt.Columns.Add("IsUpdated");
                dt.Columns.Add("CompanyCode");


                #endregion

                if (apiData != null)
                {


                    #region Products

                    foreach (var items in apiData.Data)
                    {
                        string SL = items.SKU.ToString();
                        string ProductCode = items.BARCODE;
                        string ProductName = items.DISPLAY_NAME;
                        //string ProductGroup = items.COMPANY_NAME;
                        string ProductGroup = "Trading";
                        string UOM = items.SAL_UOM_NAME;
                        string HSCode = "NA";
                        string UnitPrice = items.LAST_PUR_PRICE.ToString();                       
                        string SDRate = "0";
                        string VATRate = items.SAL_VAT_PERCENT.ToString();
                        string Description = items.REGIONAL_NAME;
                        //string IsProcessed = items.DISPLAY_NAME;
                        //string IsUpdated = items.DISPLAY_NAME;
                        //string CompanyCode = items.DISPLAY_NAME;



                        ////foreach (var Lineitems in items.order_lines)
                        ////{
                        DataRow dr = dt.NewRow();
                        dr["ProductCode"] = ProductCode;
                        dr["ProductName"] = ProductName;
                        dr["ProductGroup"] = ProductGroup;

                        dr["UOM"] = UOM;
                        dr["HSCode"] = HSCode;
                        dr["UnitPrice"] = UnitPrice;
                        dr["SDRate"] = SDRate;
                        dr["VATRate"] = VATRate;
                        dr["Description"] = Description;
                        //dr["IsProcessed"] = IsProcessed;
                        //dr["IsUpdated"] = IsUpdated;
                        //dr["CompanyCode"] = CompanyCode;


                        dt.Rows.Add(dr);

                        ////}

                    }

                    #endregion

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

            }
        }

        private DataRoot_DecathlonProduct GetProductApiData(AuthRoot auth, string limit = "1", SysDBInfoVMTemp connVM = null)
        {

            string url = "";

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");
            //string branchCode = param.dtConnectionInfo.Rows[0]["BranchCode"].ToString();


            url = productUrl;
            //url = productUrl11;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "symphony:LLEKCOp3omA+Nfu0a/ywyQ==";

            DecathlonParam = new DecathlonParam();

            string apiKeyValue = "";

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, DecathlonParam, apiKeyValue);
            DataRoot_DecathlonProduct prductsApiData = JsonConvert.DeserializeObject<DataRoot_DecathlonProduct>(result);

            return prductsApiData;
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
                //adapter = new SqlDataAdapter(cmd);


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

        public DataTable GetTransferDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string IntBranchCode = GetBranchcode(param.BranchId, connVM);

                //DataRoot apiData;
                List<DataRoot_DecathlonTransfer> apiData;

                apiData = GetTransferApiData(Auth, param, "1", connVM, IntBranchCode);

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

                if (apiData!= null)
                {

                    #region Transfer

                    foreach (var items in apiData)
                    {
                        string ID = items.CHALLAN_NO;
                        string BranchCode = items.STORE_CODE;
                        string TransactionDateTime = Convert.ToDateTime(items.DELIVERY_DATE).ToString("yyyy-MM-dd HH:mm:ss");
                        string TransferToBranchCode = items.DELIVERY_TO;

                        string product_code = items.BARCODE;

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["BranchCode"] = BranchCode;
                        dr["TransferToBranchCode"] = TransferToBranchCode;
                        dr["TransactionDateTime"] = TransactionDateTime;
                        dr["ReferenceNo"] = ID;
                        dr["ProductCode"] = product_code;
                        dr["ProductName"] = items.DISPLAY_NAME;
                        dr["UOM"] = items.SAL_UOM_NAME;
                        dr["Quantity"] = Convert.ToDecimal(items.DEL_QTY);
                        //dr["CostPrice"] = Convert.ToDecimal(items.rate);
                        dr["CostPrice"] = Convert.ToDecimal(items.SAL_PRICE);

                        //string type = items.product_info.type;

                        //if (type == "Packaging")
                        //{
                        //    type = "Raw";
                        //}
                        //dr["TransactionType"] = type;
                        dr["TransactionType"] = "Trading";
                        dr["Comments"] = "-";
                        dr["Post"] = "N";
                        dr["BranchFromRef"] = BranchCode;
                        dr["BranchToRef"] = TransferToBranchCode;
                        dr["VehicleNo"] = "NA";
                        dr["VehicleType"] = "NA";
                        dr["CommentsD"] = "";
                        dr["VAT_Rate"] = "0";

                        //////if (!string.IsNullOrWhiteSpace(product_code))
                        //////{
                        //////    ProductVM vm = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { product_code }, null, null, null, connVM).FirstOrDefault();

                        //////    if (vm != null)
                        //////    {
                        //////        dr["VAT_Rate"] = vm.VATRate;
                        //////    }

                        //////}

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

        private List<DataRoot_DecathlonTransfer> GetTransferApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url = "";

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");
            string branchCode = param.dtConnectionInfo.Rows[0]["BranchCode"].ToString();


            if (!string.IsNullOrWhiteSpace(param.RefNo))
            {
                url = transferUrl + "/" + param.RefNo;

            }

            else
            {
                url = transferUrl + "/" + "{chlnNo}";

            }

            if (!string.IsNullOrWhiteSpace(IntBranchCode))
            {
                //url = purchaseUrl + "/" + "100010" + branchCode;
                url = url + "/" + IntBranchCode;
            }

            if (!string.IsNullOrWhiteSpace(param.FromDate))
            {
                url = url + "/" + param.FromDate;
            }

            if (!string.IsNullOrWhiteSpace(param.ToDate))
            {
                url = url + "/" + param.ToDate;
            }


            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string key = string.Empty;
            string ApiKey = "symphony:LLEKCOp3omA+Nfu0a/ywyQ==";



            DecathlonParam = new DecathlonParam();

            string apiKeyValue = "";

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM, DecathlonParam, apiKeyValue);

            List<DataRoot_DecathlonTransfer> transferApiData = JsonConvert.DeserializeObject<List<DataRoot_DecathlonTransfer>>(result);

            return transferApiData;
        }

        public ResultVM SaveTransferdata_Decathlon_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SaleDAL salesDal = new SaleDAL();
            try
            {

                DataTable transferData = GetTransferData(paramVM, connVM);

                TableValidation_Transfer(transferData, connVM);

                string[] result = SaveTransfer_Decathlon_Pre(transferData, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, Convert.ToInt32(paramVM.BranchId), () => { }, null, null, false, connVM, "", paramVM.CurrentUserId);

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

        public string[] SaveTransfer_Decathlon_Pre(DataTable dtTransfer, string BranchCode, string transactionType, string CurrentUser,
           int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
           bool integration = false, SysDBInfoVMTemp connVM = null, string entryTime = "", string UserId = "")
        {
            ResultVM rVM = new ResultVM();
            ////IssueDAL issueDal = new IssueDAL();
            TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

            string[] sqlResults = new string[6];

            try
            {

                if (dtTransfer == null || dtTransfer.Rows.Count <= 0)
                {
                    throw new Exception("There is no data to save");
                }

                TableValidation_Transfer(dtTransfer, connVM);

                ////DataTable PIssueData = GetPIssueData(paramVM, connVM);
                ////paramVM.Data = PIssueData;

                //////string[] result = issueDal.SaveIssue_Split(paramVM, connVM);

                sqlResults = _TransferIssueDAL.SaveTempTransfer(dtTransfer, null, null, CurrentUser, 0,
                    () => { }, null, null, true, connVM, DateTime.Now.ToString("HH:mm:ss"), UserId);


                rVM.Status = sqlResults[0];
                rVM.Message = sqlResults[1];

            }
            catch (Exception ex)
            {
                sqlResults[0] = "Fail";
                sqlResults[1] = ex.Message;
                throw ex;
            }
            finally
            {


            }
            return sqlResults;
        }


        private void TableValidation_Transfer(DataTable dtTransfer, SysDBInfoVMTemp connVM = null)
        {
            //if (!dtTransfer.Columns.Contains("SL"))
            //{
            //    DataColumn varDataColumn = new DataColumn("SL") { DefaultValue = "" };
            //    dtTransfer.Columns.Add(varDataColumn);
            //}
            //if (!dtTransfer.Columns.Contains("BranchId"))
            //{
            //    DataColumn varDataColumn = new DataColumn("BranchId") { DefaultValue = "" };
            //    dtTransfer.Columns.Add(varDataColumn);
            //}
            //if (!dtTransfer.Columns.Contains("TransferToBranchId"))
            //{
            //    DataColumn varDataColumn = new DataColumn("TransferToBranchId") { DefaultValue = "" };
            //    dtTransfer.Columns.Add(varDataColumn);
            //}
            if (!dtTransfer.Columns.Contains("VehicleNo"))
            {
                DataColumn varDataColumn = new DataColumn("VehicleNo") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }

            ////if (!dtTransfer.Columns.Contains("BomId"))
            ////{
            ////    DataColumn varDataColumn = new DataColumn("BomId") { DefaultValue = "" };
            ////    dtTransfer.Columns.Add(varDataColumn);
            ////}

            //if (!dtTransfer.Columns.Contains("ItemNo"))
            //{
            //    DataColumn varDataColumn = new DataColumn("ItemNo") { DefaultValue = "" };
            //    dtTransfer.Columns.Add(varDataColumn);
            //}

            if (!dtTransfer.Columns.Contains("Weight"))
            {
                DataColumn varDataColumn = new DataColumn("Weight") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }

            ////var SL = new DataColumn("SL") { DefaultValue = "" };
            ////var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
            ////var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };


        }

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

        private string GetData(string url, AuthRoot auth, SysDBInfoVMTemp connVM = null, DecathlonParam _DecathlonParam = null, string apiKeyValue = "")
        {
            try
            {

                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                request.Headers.Add("Authorization", "symphony:LLEKCOp3omA+Nfu0a/ywyQ==");
                ////request.Headers.Add("company-id", auth.company.FirstOrDefault().id.ToString());
                ////request.Headers.Add("company-id", CompanyId);

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
            //salesData.Columns.Add(TransactionType);
        }

        private string GetBranchcode(string BranchId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();
                string code = commonDal.settingValue("CompanyCode", "Code", connVM);

                BranchProfileDAL Branchdal = new BranchProfileDAL();
                BranchProfileVM vm = new BranchProfileVM();
                //vm.BranchID = Convert.ToInt32(param.BranchId);
                vm.BranchID = Convert.ToInt32(BranchId);

                DataTable dtBranch = Branchdal.SearchBranchMapDetails(vm, null, null, connVM);

                string IntBranchCode = dtBranch.Rows[0]["IntegrationCode"].ToString();



                return IntBranchCode;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}
