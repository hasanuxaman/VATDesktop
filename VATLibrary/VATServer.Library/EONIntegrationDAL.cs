
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
using VATServer.Ordinary;

namespace VATServer.Library
{
    public class EONIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        BranchProfileDAL _branchDAL = new BranchProfileDAL();

        #region EON Foods

        #region Test API Connection

        ////private string AuthUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/auth/login";
        ////private string saleUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/order-list";
        ////private string CNUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/sale-return-list";
        ////private string PIssueUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/production-planning-list";
        ////private string PReceiveUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/production-planning-receive-list";
        ////private string PurchaseUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/purchase-receive-list";
        ////private string ImportPurchaseUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/lc-purchase-receive-list";
        ////private string lcImportPurchaseUrl = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/lc-clearing-forwarding-list";
        ////private string StockTransfer = "http://stage.jannaty.biznet_legacy.eoninfosys.com/app_dev.php/api/v1/list/stock-transfer-list";

        #endregion

        #region New API Connection

        private string AuthUrl = "http://202.74.246.84:82/api/v1/auth/login";
        private string saleUrl = "http://202.74.246.84:82/api/v1/list/order-list";
        private string CNUrl = "http://202.74.246.84:82/api/v1/list/sale-return-list";
        private string PIssueUrl = "http://202.74.246.84:82/api/v1/list/production-planning-list";
        private string PReceiveUrl = "http://202.74.246.84:82/api/v1/list/production-planning-receive-list";
        private string PurchaseUrl = "http://202.74.246.84:82/api/v1/list/purchase-receive-list";
        private string ImportPurchaseUrl = "http://202.74.246.84:82/api/v1/list/lc-purchase-receive-list";
        private string lcImportPurchaseUrl = "http://202.74.246.84:82/api/v1/list/lc-clearing-forwarding-list";
        private string StockTransfer = "http://202.74.246.84:82/api/v1/list/stock-transfer-list";
        private string customerUrl = "http://202.74.246.84:82/api/v1/customer/customer_list";

        #endregion

        #endregion

        #region Puro Foods

        #region API Connection

        #region Test Database

        ////private string PuroAuthUrl = "http://202.74.246.84:94/app_dev.php/api/v1/auth/login";
        ////private string PurosaleUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/order-list";
        ////private string PuroCNUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/sale-return-list";
        ////private string PuroPIssueUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/production-planning-list";
        ////private string PuroPReceiveUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/production-planning-receive-list";
        ////private string PuroPurchaseUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/purchase-receive-list";
        ////////////private string PuroImportPurchaseUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/lc-purchase-receive-list";
        ////////////private string PurolcImportPurchaseUrl = "http://202.74.246.84:94/app_dev.php/api/v1/list/lc-clearing-forwarding-list";
        ////private string PuroStockTransfer = "http://202.74.246.84:94/app_dev.php/api/v1/list/stock-transfer-list";

        #endregion

        #region Live Database

        private string PuroAuthUrl = "http://202.74.246.84:96/api/v1/auth/login";
        private string PurosaleUrl = "http://202.74.246.84:96/api/v1/list/order-list";
        private string PuroCNUrl = "http://202.74.246.84:96/api/v1/list/sale-return-list";
        private string PuroPIssueUrl = "http://202.74.246.84:96/api/v1/list/production-planning-list";
        private string PuroPReceiveUrl = "http://202.74.246.84:96/api/v1/list/production-planning-receive-list";
        private string PuroPurchaseUrl = "http://202.74.246.84:96/api/v1/list/purchase-receive-list";
        ////////private string PuroImportPurchaseUrl = "http://202.74.246.84:96/api/v1/list/lc-purchase-receive-list";
        ////////private string PurolcImportPurchaseUrl = "http://202.74.246.84:96/api/v1/list/lc-clearing-forwarding-list";
        private string PuroStockTransfer = "http://202.74.246.84:96/api/v1/list/stock-transfer-list";

        #endregion

        #endregion

        #endregion

        private AuthRoot Auth = new AuthRoot();

        public EONIntegrationDAL()
        {
            Auth = GetAuthToken();
        }

        #region Sale Data

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
purchaser_id varchar(100),
purchaser_mobile_no varchar(20),
territory varchar(100),
terms varchar(100),
due_date varchar(100),
officer_name varchar(100),
officer_mobile varchar(100),
depot_name varchar(100),
DiscountAmount decimal(25,7),
TotalDue decimal(25,7),
TotalOD decimal(25,7),

)";

                #endregion

                #region UpdateTypeVATRate

                string UpdateTypeVATRate = @"
----------- VAT Rate Update --------------
update #saletemp set #saletemp.VAT_Rate=Products.VATRate , #saletemp.UOM= Products.UOM, #saletemp.SD_Rate= Products.SD
from Products where #saletemp.Item_Code=Products.ProductCode

----------- Type Update --------------

update #saletemp set #saletemp.[Type]='VAT'
 where #saletemp.VAT_Rate='15'

 update #saletemp set #saletemp.[Type]='NonVAT'
 where #saletemp.VAT_Rate='0'

 update #saletemp set #saletemp.[Type]='OtherRate'
 where #saletemp.VAT_Rate!='0' and #saletemp.VAT_Rate!='15' 

 --update #saletemp set #saletemp.NBR_Price = ((((#saletemp.NBR_Price * #saletemp.Quantity)- #saletemp.Discount_Amount)/#saletemp.Quantity)
 --*100/(100+#saletemp.VAT_Rate))

 --update #saletemp set #saletemp.Discount_Amount=0

";

                if (code.ToLower() == "EON".ToLower())
                {
                    UpdateTypeVATRate += @"
                    --update #saletemp set #saletemp.NBR_Price = ((((#saletemp.NBR_Price * #saletemp.Quantity)- #saletemp.Discount_Amount)/#saletemp.Quantity)*100/(100+#saletemp.VAT_Rate))
                    
                    update #saletemp set #saletemp.Discount_Amount=0,#saletemp.DiscountAmount=0
                    ";
                }

                string UpdateCustomer = @"
----------- Type Customer --------------

update #saletemp set #saletemp.Customer_Code='10001' , #saletemp.Customer_Name='Cash Counter' where 1=1  and len( isnull(#saletemp.Customer_Name,''))<=3

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
,DiscountAmount Option1
,purchaser_id Option2
,purchaser_mobile_no Option3
,territory Option4
,terms Option5
,due_date Option6
,officer_name Option7
,officer_mobile Option8
,depot_name Option9
,TotalDue Option10
,TotalOD Option11
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
                //result = commonDal.BulkInsert("SalesTempData", dt, currConn, transaction);

                cmd.CommandText = GetZeroQty;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                if (code.ToLower() != "EON".ToLower())
                {
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
                }

                cmd.CommandText = UpdateTypeVATRate;
                cmd.ExecuteNonQuery();

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();


                if (code.ToLower() == "purofood")
                {
                    cmd.CommandText = UpdateCustomer;
                    cmd.ExecuteNonQuery();
                }

                #region VAT Calculation

                if (code.ToLower() == "EON".ToLower())
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
                string IntBranchCode = GetBranchcode(param.BranchId, connVM);

                DataRoot apiData;

                apiData = GetSaleApiData(Auth, param, "1", connVM, IntBranchCode);

                string total = "1";
                if (apiData.total != 0)
                {
                    total = apiData.total.ToString();
                }

                apiData = GetSaleApiData(Auth, param, total, connVM, IntBranchCode);


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
                dt.Columns.Add("purchaser_id");
                dt.Columns.Add("purchaser_mobile_no");
                dt.Columns.Add("territory");
                dt.Columns.Add("terms");
                dt.Columns.Add("due_date");
                dt.Columns.Add("officer_name");
                dt.Columns.Add("officer_mobile");
                dt.Columns.Add("depot_name");
                dt.Columns.Add("foc");
                dt.Columns.Add("DiscountAmount");

                dt.Columns.Add("TotalDue");
                dt.Columns.Add("TotalOD");

                #endregion

                if (apiData != null)
                {

                    if (param.TransactionType == "Credit")
                    {

                        #region Credit Note

                        foreach (var items in apiData.items)
                        {
                            string ID = items.order.invoice_no.ToString();
                            string Branch_Code = items.location.id.ToString();
                            string Invoice_Date_Time = items.order.invoice_date.ToString("yyyy-MM-dd HH:mm:ss");
                            string DeliveryDateTime = items.approved_at.ToString("yyyy-MM-dd HH:mm:ss");
                            string Customer_Code = items.order.customer.customer_code;
                            string Customer_Name = items.order.customer.company_name;
                            string ReasonOfReturn = items.description;
                            string Delivery_Address = items.location.delivery_address;

                            foreach (var Lineitems in items.sale_return_lines)
                            {

                                OrderLine orderLine = items.order.order_lines
                                    .FirstOrDefault(x => x.product.product_code == Lineitems.product.product_code);

                                DataRow dr = dt.NewRow();
                                dr["ID"] = ID;
                                dr["Branch_Code"] = Branch_Code;
                                dr["Customer_Code"] = Customer_Code;
                                dr["Customer_Name"] = Customer_Name;
                                dr["Invoice_Date_Time"] = Invoice_Date_Time;
                                dr["Reference_No"] = ID;
                                dr["Item_Code"] = Lineitems.product.product_code;
                                dr["Item_Name"] = Lineitems.product.name;
                                dr["UOM"] = Lineitems.product.unit.name;
                                dr["Quantity"] = Convert.ToDecimal(Lineitems.quantity);
                                ////dr["VAT_Rate"] = orderLine.vat;

                                decimal VATRate = 0;

                                if (orderLine.vat != 0)
                                {
                                    VATRate = 15;
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

                                dr["SubTotal"] = "0";
                                ////dr["SubTotal"] = orderLine.total_price;
                                dr["Discount_Amount"] = Lineitems.discount;
                                dr["Comments"] = "-";
                                dr["NBR_Price"] = Lineitems.rate;
                                dr["Delivery_Address"] = Delivery_Address;
                                dr["Promotional_Quantity"] = "0";
                                dr["Sale_Type"] = "New";
                                dr["Vehicle_No"] = "NA";
                                dr["VehicleType"] = "NA";

                                dr["SD_Rate"] = "0";
                                dr["SDAmount"] = "0";
                                dr["Is_Print"] = "N";
                                dr["Tender_Id"] = "0";
                                dr["Post"] = "N";
                                dr["LC_Number"] = "NA";
                                dr["Currency_Code"] = "BDT";
                                dr["CommentsD"] = "NA";

                                dr["Non_Stock"] = "N";
                                dr["Trading_MarkUp"] = "0";

                                #region Comments

                                ////if (orderLine.vat == 15)
                                ////{
                                ////    dr["Type"] = "VAT";
                                ////}
                                ////else if (orderLine.vat == 0)
                                ////{
                                ////    dr["Type"] = "NoNVAT";
                                ////}
                                ////else
                                ////{
                                ////    dr["Type"] = "OtherRate";
                                ////}

                                #endregion

                                dr["VAT_Name"] = "VAT 4.3";

                                dr["Previous_Invoice_No"] = ID;
                                dr["PreviousInvoiceDateTime"] = Invoice_Date_Time;

                                dr["PreviousNBRPrice"] = orderLine.rate;
                                dr["PreviousQuantity"] = orderLine.quantity;

                                dr["PreviousSubTotal"] = orderLine.total_price;
                                dr["PreviousVATAmount"] = "0";
                                dr["PreviousVATRate"] = orderLine.vat;
                                dr["PreviousSD"] = "0";
                                dr["PreviousSDAmount"] = "0";
                                dr["ReasonOfReturn"] = ReasonOfReturn;

                                dt.Rows.Add(dr);

                            }

                        }
                        #endregion

                    }
                    else
                    {
                        #region Sales

                        string InvoiceTime = DateTime.Now.ToString("HH:mm:ss");

                        foreach (var items in apiData.items)
                        {
                            string ID = items.invoice_no;
                            string Reference_No = ID;
                            string Branch_Code = items.location.id.ToString();
                            string Invoice_Date_Time = items.invoice_date.ToString("yyyy-MM-dd " + InvoiceTime);
                            string DeliveryDateTime = items.approved_at.ToString("yyyy-MM-dd " + InvoiceTime);
                            string Customer_Code = items.customer.customer_code;
                            string Customer_Name = items.customer.company_name;
                            decimal TotalDue = items.customer.total_due;
                            decimal TotalOD = items.customer.total_od;

                            //12 Oct 2022 Start
                            if (code.ToLower() == "purofood")
                            {
                                ID = items.id;
                                Reference_No = items.invoice_no;
                            }
                            //12 Oct 2022 End

                            if (string.IsNullOrEmpty(Customer_Name))
                            {
                                Customer_Name = "";
                            }
                            string Comments = items.description;
                            ////string Delivery_Address = items.location.delivery_address;
                            string Delivery_Address = items.customer.address_line1;

                            foreach (var Lineitems in items.order_lines)
                            {
                                #region Lineitems

                                DataRow dr = dt.NewRow();
                                dr["ID"] = ID;
                                dr["Branch_Code"] = Branch_Code;
                                dr["Customer_Code"] = Customer_Code;
                                dr["Customer_Name"] = Customer_Name;
                                dr["Invoice_Date_Time"] = Invoice_Date_Time;
                                dr["Reference_No"] = Reference_No;
                                dr["Item_Code"] = Lineitems.product.product_code;
                                dr["Item_Name"] = Lineitems.product.name;
                                dr["UOM"] = Lineitems.product.unit.name;

                                ////dr["VAT_Rate"] = Lineitems.vat;
                                dr["DiscountAmount"] = Lineitems.discount;
                                //////dr["SubTotal"] = Lineitems.total_price - Lineitems.discount;

                                if (code.ToLower() == "purofood")
                                {
                                    dr["Quantity"] = Convert.ToDecimal(Lineitems.quantity) + Convert.ToDecimal(Lineitems.foc);//
                                    dr["foc"] = Lineitems.foc;
                                    decimal subtotal = Lineitems.total_price;
                                    ////subtotal = subtotal - Lineitems.discount;
                                    subtotal = subtotal - Lineitems.vat;
                                    subtotal = subtotal - Lineitems.sd_amount;
                                    dr["SubTotal"] = subtotal;
                                    dr["VAT_Amount"] = Lineitems.vat;
                                    dr["Discount_Amount"] = "0";

                                }
                                else if (code.ToLower() == "eon")
                                {
                                    dr["Quantity"] = Convert.ToDecimal(Lineitems.quantity);//
                                    dr["foc"] = "0";
                                    dr["SubTotal"] = Lineitems.total_price;
                                    dr["Discount_Amount"] = Lineitems.discount;

                                }
                                else
                                {
                                    dr["Quantity"] = Convert.ToDecimal(Lineitems.quantity) + Convert.ToDecimal(Lineitems.foc);//
                                    dr["foc"] = Lineitems.foc;
                                    dr["SubTotal"] = Lineitems.gAmt - Lineitems.discount;
                                    dr["Discount_Amount"] = "0";

                                }

                                //////dr["SubTotal"] = Lineitems.total_price;
                                dr["TransactionType"] = param.TransactionType;
                                dr["CompanyCode"] = code;
                                dr["purchaser_id"] = Customer_Code;//items.purchaser_id;
                                dr["purchaser_mobile_no"] = items.purchaser_mobile_no;
                                dr["territory"] = items.territory;
                                dr["terms"] = items.terms;
                                dr["due_date"] = items.due_date;
                                dr["officer_name"] = items.officer_name;
                                dr["officer_mobile"] = items.officer_mobile;
                                dr["depot_name"] = items.depot_name;
                                dr["Comments"] = Comments;
                                dr["NBR_Price"] = Lineitems.rate;
                                dr["Delivery_Address"] = Delivery_Address;

                                dr["TotalDue"] = TotalDue;
                                dr["TotalOD"] = TotalOD;
                                dr["SD_Rate"] = "0";
                                ////dr["SDAmount"] = Lineitems.sd_amount;
                                decimal VATRate = 0;

                                if (Lineitems.vat != 0)
                                {
                                    VATRate = 15;
                                }
                                dr["VAT_Rate"] = VATRate;

                                if (VATRate == 15)
                                {
                                    dr["Type"] = "VAT";
                                }
                                else if (VATRate == 0)
                                {
                                    dr["Type"] = "NonVAT";
                                }
                                else
                                {
                                    dr["Type"] = "OtherRate";
                                }

                                ////dr["SubTotal"] = "0";

                                dr["Promotional_Quantity"] = "0";
                                dr["Sale_Type"] = "New";
                                dr["Vehicle_No"] = "NA";
                                dr["VehicleType"] = "NA";
                                ////dr["TransactionType"] = param.TransactionType;


                                dr["Is_Print"] = "N";
                                dr["Tender_Id"] = "0";
                                dr["Post"] = "N";
                                dr["LC_Number"] = "NA";
                                dr["Currency_Code"] = "BDT";
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

                                #endregion

                            }

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

        private DataRoot GetSaleApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");


            if (code.ToLower() == "purofood")
            {
                if (param.TransactionType == "Credit")
                {
                    url = PuroCNUrl;
                }
                else
                {
                    url = PurosaleUrl;
                }
            }
            else
            {
                if (param.TransactionType == "Credit")
                {
                    url = CNUrl;
                }
                else
                {
                    url = saleUrl;
                }
            }


            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            ////query["fromDate"] = "2021-03-01";
            ////query["toDate"] = "2021-03-31";
            query["limit"] = limit;
            query["page"] = "1";

            if (code.ToLower() == "purofood")
            {

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    query["date"] = param.FromDate;
                }

            }
            else
            {


                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    query["fromDate"] = param.FromDate;
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    query["toDate"] = param.ToDate;
                }

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    query["invoiceNo"] = param.RefNo;
                }
            }

            if (!string.IsNullOrEmpty(IntBranchCode))
            {
                query["locationId"] = IntBranchCode;
            }
            //query["locationId"] = param.RefNo;

            //locationId
            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth, connVM);

            DataRoot salesApiData = JsonConvert.DeserializeObject<DataRoot>(result);

            return salesApiData;
        }

        public ResultVM SaveSale_EON_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
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

        #region Production Issue Data

        public DataTable GetPIssueData(IntegrationParam param, SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string IsProcess = "N")
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

                dt = GetPIssueDataAPI(param);

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
create table #Issuetemp(
sl int identity(1,1), 
ID varchar(6000),
BranchCode varchar(100),
Issue_DateTime varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
UOM varchar(100),
Reference_No varchar(100),
Comments varchar(500),
Post varchar(100),
[Type] varchar(100),
IsProcess varchar(100)


)";

                #endregion

                #region Delete Finished Type Data

                string DeleteFinishedType = @"
----------- Delete Finished Type Data --------------
delete #Issuetemp where Type='Finished'
";

                #endregion

                #region UpdateProcesssFlag

                string UpdateIsProcessFlg = @"

update #Issuetemp set IsProcess='N'

update #Issuetemp set IsProcess='Y' from IssueHeaders where #Issuetemp.ID =IssueHeaders.SerialNo

";
                #endregion


                #region getAllData

                string getAllData = @"

select 
 ID
,BranchCode
,Issue_DateTime
,Item_Code
,Item_Name
,Quantity
,UOM
,Reference_No
,Comments
,Post
from #Issuetemp
where 1=1 

";

                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    getAllData += @" and IsProcess=@IsProcess";
                }

                getAllData += @" drop table #Issuetemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#Issuetemp", dt, currConn, transaction);

                cmd.CommandText = DeleteFinishedType;
                cmd.ExecuteNonQuery();

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();

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

            #region catch & finally

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
            #endregion

        }

        public DataTable GetPIssueDataAPI(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string IntBranchCode = GetBranchcode(param.BranchId, connVM);

                DataRoot apiData;

                apiData = GetPIssueApiData(Auth, param, "1", connVM, IntBranchCode);

                ////////string total = "1";
                ////////if (apiData.total != 0)
                ////////{
                ////////    total = apiData.total.ToString();
                ////////}

                ////////apiData = GetPIssueApiData(Auth, param, total);


                // convert api data to VAT system format 

                #region Create DataTable

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("ID");
                dt.Columns.Add("BranchCode");
                dt.Columns.Add("Issue_DateTime");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Post");
                dt.Columns.Add("Type");

                #endregion

                if (apiData.items != null)
                {

                    #region Issue

                    foreach (var items in apiData.items)
                    {
                        string ID = items.planningId.ToString();
                        string Branch_Code = items.location.id.ToString();
                        string Issue_DateTime = Convert.ToDateTime(items.date.date).ToString("yyyy-MM-dd HH:mm:ss");

                        foreach (var Lineitems in items.products)
                        {

                            DataRow dr = dt.NewRow();
                            dr["ID"] = ID;
                            dr["BranchCode"] = Branch_Code;
                            dr["Issue_DateTime"] = Issue_DateTime;
                            dr["Reference_No"] = ID;
                            dr["Item_Code"] = Lineitems.product.code;
                            dr["Item_Name"] = Lineitems.product.name;
                            dr["UOM"] = Lineitems.product.unit.name;
                            dr["Quantity"] = Convert.ToDecimal(Lineitems.qty);
                            dr["Type"] = Lineitems.product.type;
                            dr["Comments"] = "-";
                            dr["Post"] = "N";

                            dt.Rows.Add(dr);

                        }

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
        }

        private DataRoot GetPIssueApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");


            if (code.ToLower() == "purofood")
            {
                url = PuroPIssueUrl;
            }
            else
            {
                url = PIssueUrl;
            }
            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            ////query["limit"] = limit;
            ////query["page"] = "1";

            if (!string.IsNullOrEmpty(param.FromDate))
            {
                query["fromDate"] = param.FromDate;
            }

            if (!string.IsNullOrEmpty(param.ToDate))
            {
                query["toDate"] = param.ToDate;
            }

            if (!string.IsNullOrEmpty(param.RefNo))
            {
                query["productionId"] = param.RefNo;
            }

            if (!string.IsNullOrEmpty(IntBranchCode))
            {
                query["locationId"] = IntBranchCode;
            }

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth);

            DataRoot ApiData = JsonConvert.DeserializeObject<DataRoot>(result);

            return ApiData;
        }

        public ResultVM SavePIssue_EON_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            IssueDAL issueDal = new IssueDAL();
            try
            {

                DataTable PIssueData = GetPIssueData(paramVM, connVM);
                paramVM.Data = PIssueData;

                string[] result = issueDal.SaveIssue_Split(paramVM, connVM);


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

        #region Production Receive Data

        public DataTable GetPReceiveData(IntegrationParam param, SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string IsProcess = "N")
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

                string IntBranchCode = GetBranchcode(param.BranchId, connVM);

                DataRoot apiData;

                string limit = "1";

                apiData = GetPReceiveApiData(Auth, param, limit, connVM, IntBranchCode);

                //////string total = "1";
                //////if (apiData.total != 0)
                //////{
                //////    total = apiData.total.ToString();
                //////}

                //////apiData = GetPReceiveApiData(Auth, param, total);


                // convert api data to VAT system format 

                #region Create DataTable

                CommonDAL commonDal = new CommonDAL();
                string code = commonDal.settingValue("CompanyCode", "Code");
                string[] result = new[] { "Fail" };

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

                if (apiData.items != null)
                {

                    #region Receive

                    foreach (var items in apiData.items)
                    {
                        string ID = items.planningId.ToString();
                        string Branch_Code = items.location.id.ToString();
                        string Issue_DateTime = Convert.ToDateTime(items.received_at.date).ToString("yyyy-MM-dd HH:mm:ss");

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["BranchCode"] = Branch_Code;
                        dr["Receive_DateTime"] = Issue_DateTime;
                        dr["Reference_No"] = ID;
                        if (items.product_info != null)
                        {
                            //throw new ArgumentNullException("", "Product Not Found in API . ID : " + ID + " production_planning Id : " + items.production_planning.id);
                            dr["Item_Code"] = items.product_info.code;
                            dr["Item_Name"] = items.product_info.name;
                            dr["UOM"] = items.product_info.unit;
                            dr["Quantity"] = Convert.ToDecimal(items.rcv_qty);

                        }
                        dr["NBR_Price"] = items.rate;

                        dr["Comments"] = "-";
                        dr["Post"] = "N";

                        dr["Return_Id"] = "";
                        dr["With_Toll"] = "N";
                        dr["VAT_Name"] = "VAT 4.3";
                        dr["CustomerCode"] = "N/A";

                        dt.Rows.Add(dr);

                        #region Old

                        ////foreach (var Lineitems in items.production_planning.production_planning_lines)
                        ////{

                        ////    DataRow dr = dt.NewRow();
                        ////    dr["ID"] = ID;
                        ////    dr["BranchCode"] = Branch_Code;
                        ////    dr["Receive_DateTime"] = Issue_DateTime;
                        ////    dr["Reference_No"] = ID;
                        ////    dr["Item_Code"] = Lineitems.product.product_code;
                        ////    dr["Item_Name"] = Lineitems.product.name;
                        ////    dr["UOM"] = Lineitems.product.unit.name;
                        ////    dr["Quantity"] = Convert.ToDecimal(Lineitems.final_quantity);
                        ////    dr["Comments"] = "-";
                        ////    dr["Post"] = "N";

                        ////    dr["Return_Id"] = "";
                        ////    dr["With_Toll"] = "N";
                        ////    dr["NBR_Price"] = "0";
                        ////    dr["VAT_Name"] = "VAT 4.3";
                        ////    dr["CustomerCode"] = "N/A";

                        ////    dt.Rows.Add(dr);

                        ////}

                        #endregion

                    }

                    #endregion

                }

                //return dt;

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
create table #Receivetemp(
sl int identity(1,1), 
ID varchar(6000),
BranchCode varchar(100),
Receive_DateTime varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
UOM varchar(100),
Reference_No varchar(100),
Comments varchar(500),
Post varchar(100),
Return_Id varchar(100),
With_Toll varchar(100),
NBR_Price decimal(25, 9),
VAT_Name varchar(100),
CustomerCode varchar(100),
IsProcess varchar(100)


)";

                #endregion

                #region UpdateProcesssFlag

                string UpdateIsProcessFlg = @"

update #Receivetemp set IsProcess='N'

update #Receivetemp set IsProcess='Y' from ReceiveHeaders where #Receivetemp.ID =ReceiveHeaders.SerialNo

";
                #endregion


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
from #Receivetemp
where 1=1 

";

                if (!string.IsNullOrWhiteSpace(IsProcess) && IsProcess.ToLower() != "all")
                {
                    getAllData += @" and IsProcess=@IsProcess";
                }

                getAllData += @" drop table #Receivetemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#Receivetemp", dt, currConn, transaction);

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();

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
        }

        private DataRoot GetPReceiveApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");


            if (code.ToLower() == "purofood")
            {
                url = PuroPReceiveUrl;
            }
            else
            {
                url = PReceiveUrl;
            }
            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            ////query["limit"] = limit;
            ////query["page"] = "1";

            if (!string.IsNullOrEmpty(param.FromDate))
            {
                query["fromDate"] = param.FromDate;
            }

            if (!string.IsNullOrEmpty(param.ToDate))
            {
                query["toDate"] = param.ToDate;
            }

            if (!string.IsNullOrEmpty(param.RefNo))
            {
                ////query["id"] = param.RefNo;
                query["productionId"] = param.RefNo;
            }

            if (!string.IsNullOrEmpty(IntBranchCode))
            {
                ////query["id"] = param.RefNo;
                query["locationId"] = IntBranchCode;
            }
            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth);

            DataRoot ApiData = JsonConvert.DeserializeObject<DataRoot>(result);

            return ApiData;
        }

        public ResultVM SavePReceive_EON_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            ReceiveDAL receiveDal = new ReceiveDAL();
            try
            {

                DataTable PReceiveData = GetPReceiveData(paramVM, connVM);
                paramVM.Data = PReceiveData;

                string[] result = receiveDal.SaveReceive_Split(paramVM, connVM);


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

        #region Purchase Data

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

update #purchasetemp set Vendor_Code='N/A' where Vendor_Code is null 
or Vendor_Code='NA' or Vendor_Code='-' or Vendor_Code=' '

update #purchasetemp set Quantity='0.000001' where Quantity is null 
or Quantity=0

update #purchasetemp set Total_Price='0.000001' where Total_Price is null 
or Total_Price=0

";

                #endregion

                #region Update BE_Number

                string UpdateBE_Number = @"

WITH CTE AS (  SELECT ID,BE_Number,ROW_NUMBER() OVER (PARTITION BY ID ORDER BY (SELECT NULL)) AS RowNum
    FROM  #purchasetemp )
update #purchasetemp set BE_Number=u.BE_Number
from(
SELECT ID,BE_Number FROM CTE 
WHERE     RowNum = 1) as u
where #purchasetemp.ID=u.ID

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

                cmd.CommandText = UpdateIsProcessFlg;
                cmd.ExecuteNonQuery();

                cmd.CommandText = UpdateBE_Number;
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

                DataRoot apiData;

                string limit = "1";

                apiData = GetPurchaseApiData(Auth, param, limit, connVM, IntBranchCode);

                string total = "1";
                if (apiData.total != 0)
                {
                    total = apiData.total.ToString();
                }

                apiData = GetPurchaseApiData(Auth, param, total, connVM, IntBranchCode);

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

                if (apiData != null && apiData.items != null)
                {
                    if (param.TransactionType == "Import")
                    {
                        #region Import Purchase

                        foreach (var items in apiData.items)
                        {
                            string ID = items.id;
                            string Referance_No = items.id;
                            //////string Branch_Code = items.location.id.ToString();
                            string Invoice_Date = items.invoice_at.ToString("yyyy-MM-dd HH:mm:ss");
                            string Receive_Date = items.qc_approved_at.ToString("yyyy-MM-dd HH:mm:ss");

                            string Vendor_Code = items.purchase.customer.customer_code;
                            string Vendor_Name = items.purchase.customer.company_name;
                            string Comments = items.remark;

                            string CommercialId = items.commercial_invoice.id.ToString();

                            foreach (var Lineitems in items.purchase_receive_lines)
                            {

                                DataRow dr = dt.NewRow();

                                dr["ID"] = ID;
                                dr["BranchCode"] = Lineitems.purchase_line.requisition_line.requisition.location.id;
                                dr["Vendor_Code"] = Vendor_Code;
                                dr["Vendor_Name"] = Vendor_Name;

                                dr["BE_Number"] = "-";
                                dr["Receive_Date"] = Receive_Date;
                                dr["Invoice_Date"] = Invoice_Date;
                                dr["Referance_No"] = Referance_No;
                                dr["LC_No"] = "-";
                                dr["Item_Code"] = Lineitems.purchase_line.requisition_line.product.product_code;
                                dr["Item_Name"] = Lineitems.purchase_line.requisition_line.product.name;
                                dr["UOM"] = Lineitems.purchase_line.requisition_line.product.unit.name;

                                decimal Quantity = Lineitems.receive_quantity;
                                decimal chalan_quantity = Lineitems.chalan_quantity;

                                decimal Discount = 0;
                                if (Lineitems.invoice_discount != null)
                                {
                                    Discount = Lineitems.invoice_discount.Value;
                                }
                                decimal Rate = Lineitems.purchase_line.rate;
                                decimal VatRate = 0;

                                if (Lineitems.purchase_line.vat_percent != null)
                                {
                                    VatRate = Lineitems.purchase_line.vat_percent.Value;
                                }

                                decimal Total_Price = 0;
                                decimal VAT_Amount = 0;

                                if (chalan_quantity < Quantity)
                                {
                                    Total_Price = (chalan_quantity * Rate) - Discount;
                                }
                                else
                                {
                                    Total_Price = (Quantity * Rate) - Discount;
                                }

                                if (chalan_quantity < Quantity)
                                {
                                    VAT_Amount = (chalan_quantity * Rate) * VatRate;
                                }
                                else
                                {
                                    VAT_Amount = (Quantity * Rate) * VatRate;
                                }

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
                                //dr["VAT_Rate"] = VatRate;

                                dt.Rows.Add(dr);

                            }

                        }

                        #endregion
                    }
                    else
                    {
                        #region Purchase

                        foreach (var items in apiData.items)
                        {
                            string ID = items.id;
                            string Referance_No = items.id;
                            ////string Branch_Code = items.location.id.ToString();
                            string Invoice_Date = items.invoice_at.ToString("yyyy-MM-dd HH:mm:ss");
                            string Receive_Date = items.qc_approved_at.ToString("yyyy-MM-dd HH:mm:ss");

                            string Vendor_Code = items.purchase.customer.customer_code;
                            string Vendor_Name = items.purchase.customer.company_name;
                            string Comments = items.remark;

                            foreach (var Lineitems in items.purchase_receive_lines)
                            {

                                DataRow dr = dt.NewRow();
                                dr["ID"] = ID;
                                dr["BranchCode"] = Lineitems.purchase_line.requisition_line.requisition.location.id;
                                dr["Vendor_Code"] = Vendor_Code;
                                dr["Vendor_Name"] = Vendor_Name;
                                dr["BE_Number"] = Lineitems.chalan_no;
                                dr["Receive_Date"] = Receive_Date;
                                dr["Invoice_Date"] = Invoice_Date;
                                dr["Referance_No"] = Referance_No;
                                dr["LC_No"] = "-";
                                dr["Item_Code"] = Lineitems.purchase_line.requisition_line.product.product_code;
                                dr["Item_Name"] = Lineitems.purchase_line.requisition_line.product.name;
                                dr["UOM"] = Lineitems.purchase_line.requisition_line.product.unit.name;

                                decimal Quantity = Lineitems.receive_quantity;
                                decimal chalan_quantity = Lineitems.chalan_quantity;

                                decimal Discount = 0;
                                if (Lineitems.invoice_discount != null)
                                {
                                    Discount = Lineitems.invoice_discount.Value;
                                }
                                //////decimal Discount = Lineitems.invoice_discount;
                                decimal Rate = Lineitems.purchase_line.rate;
                                decimal VatRate = 0;

                                if (Lineitems.purchase_line.vat_percent != null)
                                {
                                    VatRate = Lineitems.purchase_line.vat_percent.Value;
                                }

                                decimal Total_Price = 0;
                                decimal VAT_Amount = 0;

                                if (chalan_quantity < Quantity)
                                {
                                    Total_Price = (chalan_quantity * Rate) - Discount;
                                }
                                else
                                {
                                    Total_Price = (Quantity * Rate) - Discount;
                                }

                                if (chalan_quantity < Quantity)
                                {
                                    VAT_Amount = (chalan_quantity * Rate) * VatRate / 100;
                                }
                                else
                                {
                                    VAT_Amount = (Quantity * Rate) * VatRate / 100;
                                }

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

                            }

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

        private DataRoot GetPurchaseApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {
            try
            {

                string url;

                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                if (code.ToLower() == "purofood")
                {
                    url = PuroPurchaseUrl;
                }
                else
                {
                    url = PurchaseUrl;

                    if (param.TransactionType == "Import")
                    {
                        url = ImportPurchaseUrl;
                    }
                }

                UriBuilder uriBuilder = new UriBuilder(url);

                NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["limit"] = limit;
                query["page"] = "1";

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    query["fromDate"] = param.FromDate;
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    query["toDate"] = param.ToDate;
                }

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    //query["id"] = param.RefNo;
                    query["purchase_id"] = param.RefNo;

                }

                if (!string.IsNullOrEmpty(IntBranchCode))
                {
                    query["locationId"] = IntBranchCode;
                }

                uriBuilder.Query = query.ToString();
                string result = GetData(uriBuilder.ToString(), auth);

                DataRoot ApiData = JsonConvert.DeserializeObject<DataRoot>(result);
                //DataSet ds = JsonConvert.DeserializeObject<DataSet>(result);

                return ApiData;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private DataRoot GetLcClearingApiData(AuthRoot auth, string CommercialId, string limit = "1", SysDBInfoVMTemp connVM = null)
        {

            string url;

            url = lcImportPurchaseUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["limit"] = limit;
            query["page"] = "1";

            if (!string.IsNullOrEmpty(CommercialId))
            {
                query["commInvoiceId"] = CommercialId;
            }

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth);

            DataRoot ApiData = JsonConvert.DeserializeObject<DataRoot>(result);

            return ApiData;

        }

        public ResultVM SavePurchase_EON_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            PurchaseDAL purchaseDal = new PurchaseDAL();
            try
            {

                DataTable PurchaseData = GetPurchaseData(paramVM, connVM);

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

                DataRoot apiData;

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

                if (apiData.items != null)
                {

                    #region Transfer

                    foreach (var items in apiData.items)
                    {
                        string ID = items.stockTransferId.ToString();
                        string BranchCode = items.from_location.id.ToString();
                        string TransactionDateTime = Convert.ToDateTime(items.transferred_at.date).ToString("yyyy-MM-dd HH:mm:ss");
                        string TransferToBranchCode = items.to_location.id.ToString();

                        string product_code = items.product_info.code;

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["BranchCode"] = BranchCode;
                        dr["TransferToBranchCode"] = TransferToBranchCode;
                        dr["TransactionDateTime"] = TransactionDateTime;
                        dr["ReferenceNo"] = ID;
                        dr["ProductCode"] = product_code;
                        dr["ProductName"] = items.product_info.name;
                        dr["UOM"] = items.product_info.unit;
                        dr["Quantity"] = Convert.ToDecimal(items.qty);
                        dr["CostPrice"] = Convert.ToDecimal(items.rate);

                        string type = items.product_info.type;

                        if (type == "Packaging")
                        {
                            type = "Raw";
                        }
                        dr["TransactionType"] = type;
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

        private DataRoot GetTransferApiData(AuthRoot auth, IntegrationParam param, string limit = "1", SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            if (code.ToLower() == "purofood")
            {
                url = PuroStockTransfer;
            }
            else
            {
                url = StockTransfer;
            }

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            ////query["limit"] = limit;
            ////query["page"] = "1";

            if (!string.IsNullOrEmpty(param.FromDate))
            {
                query["fromDate"] = param.FromDate;
            }

            if (!string.IsNullOrEmpty(param.ToDate))
            {
                query["toDate"] = param.ToDate;
            }

            if (!string.IsNullOrEmpty(param.RefNo))
            {
                query["stockTransferId"] = param.RefNo;
            }

            if (!string.IsNullOrEmpty(IntBranchCode))
            {
                query["fromLocationId"] = IntBranchCode;
            }

            ////if (!string.IsNullOrEmpty(IntBranchCode))
            ////{
            ////    query["toLocationId"] = IntBranchCode;
            ////}

            uriBuilder.Query = query.ToString();
            string result = GetData(uriBuilder.ToString(), auth);

            DataRoot ApiData = JsonConvert.DeserializeObject<DataRoot>(result);

            return ApiData;
        }

        public string[] SaveTransfer_EON_Pre(DataTable dtTransfer, string BranchCode, string transactionType, string CurrentUser,
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

        public ResultVM SaveTransferdata_EON_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SaleDAL salesDal = new SaleDAL();
            try
            {

                DataTable transferData = GetTransferData(paramVM, connVM);

                TableValidation_Transfer(transferData, connVM);

                string[] result = SaveTransfer_EON_Pre(transferData, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, Convert.ToInt32(paramVM.BranchId), () => { }, null, null, false, connVM, "", paramVM.CurrentUserId);

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

        #region Customer data

        public DataTable GetCustomerDataAPI(DataTable conInfo,SysDBInfoVMTemp connVM = null)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                ////List<DataRoot_DecathlonProduct> apiData;
                //DataRoot_DecathlonProduct apiData;

                //apiData = GetProductApiData(Auth);
                //string code = commonDal.settingValue("CompanyCode", "Code");
                //string IntBranchCode = GetBranchcode(param.BranchId, connVM);
                string IntBranchCode = GetBranchcode(conInfo.Rows[0]["BranchId"].ToString(), connVM);

                DataRoot apiData;

                apiData = GetCustomerApiData(Auth, connVM, IntBranchCode);

                // convert api data to VAT system format 

                #region Create DataTable

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("SL");
                dt.Columns.Add("CustomerCode");   
                dt.Columns.Add("CustomerName");
                dt.Columns.Add("Address");
                dt.Columns.Add("BIN_No");
                dt.Columns.Add("Comments");
                dt.Columns.Add("IsProcessed");
                dt.Columns.Add("IsUpdated");
                dt.Columns.Add("CompanyCode");
                dt.Columns.Add("Description");
                dt.Columns.Add("CustomerGroup");

                #endregion

                if (apiData != null)
                {


                    #region Products

                    foreach (var items in apiData.list)
                    {
                        //string SL = items.SKU.ToString();
                        string CustomerCode = items.customer_code;
                        string CustomerName = items.first_name+" "+items.last_name;
                        string CustomerGroup = "Local";
                        string Address = items.address_line1;
                        if (Address==null)
                        {
                            Address = "-";
                        }
                        string BIN_No = items.vat_reg_no;

                        if(BIN_No==null)
                        {
                            BIN_No = "-";
                        }
                        //string Address = items.address_line1;

                        DataRow dr = dt.NewRow();
                        dr["CustomerCode"] = CustomerCode;
                        dr["CustomerName"] = CustomerName;
                        dr["CustomerGroup"] = CustomerGroup;
                        dr["Address"] = Address;
                        dr["BIN_No"] = BIN_No;

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

        private DataRoot GetCustomerApiData(AuthRoot auth, SysDBInfoVMTemp connVM = null, string IntBranchCode = null)
        {

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");


            //if (code.ToLower() == "purofood")
            //{
            //    if (param.TransactionType == "Credit")
            //    {
            //        url = PuroCNUrl;
            //    }
            //    else
            //    {
            //        url = PurosaleUrl;
            //    }
            //}
            //else
            //{
            //    if (param.TransactionType == "Credit")
            //    {
            //        url = CNUrl;
            //    }
            //    else
            //    {
            //        url = saleUrl;
            //    }
            //}

            url = customerUrl;


            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            uriBuilder.Query = query.ToString();
            string result = GetDataCustomer(uriBuilder.ToString(), auth, connVM);

            DataRoot apiData = JsonConvert.DeserializeObject<DataRoot>(result);

            return apiData;
        }

        private string GetDataCustomer(string url, AuthRoot auth, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string CompanyId = "1";
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                if (code.ToLower() == "eahpl")
                {
                    CompanyId = "4";
                }
                else if (code.ToLower() == "eail")
                {
                    CompanyId = "5";
                }
                else if (code.ToLower() == "eeufl")
                {
                    CompanyId = "6";
                }
                else if (code.ToLower() == "exfl")
                {
                    CompanyId = "7";
                }

                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                request.Headers.Add("jw-token", "Bearer " + auth.token);
                ////request.Headers.Add("company-id", auth.company.FirstOrDefault().id.ToString());
                request.Headers.Add("company-id", CompanyId);

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

        private AuthRoot GetAuthToken(SysDBInfoVMTemp connVM = null)
        {
            AuthCredentials credentials = new AuthCredentials();

            string url;

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            if (code.ToLower() == "purofood")
            {
                //////credentials.UserName = "rest";
                //////credentials.Password = "123456";

                credentials.UserName = "vatuser";
                credentials.Password = "123456";


                url = PuroAuthUrl;
            }
            else
            {
                //credentials.UserName = "rest";
                //credentials.Password = "123456";

                credentials.UserName = "vatuser";
                credentials.Password = "123456";

                url = AuthUrl;

            }

            ////string result = PostData(AuthUrl, credentials.GetJson());
            string result = PostData(url, credentials.GetJson());
            AuthRoot auth = JsonConvert.DeserializeObject<AuthRoot>(result);
            return auth;
        }

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

        private string GetData(string url, AuthRoot auth, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string CompanyId = "1";
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");

                if (code.ToLower() == "eahpl")
                {
                    CompanyId = "4";
                }
                else if (code.ToLower() == "eail")
                {
                    CompanyId = "5";
                }
                else if (code.ToLower() == "eeufl")
                {
                    CompanyId = "6";
                }
                else if (code.ToLower() == "exfl")
                {
                    CompanyId = "7";
                }

                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                request.Headers.Add("jw-token", "Bearer " + auth.token);
                request.Headers.Add("company-id", CompanyId);

                ////////////WebResponse response = request.GetResponse();

                //////////////Stream datastream = response.GetResponseStream();

                //////////////StreamReader reader = new StreamReader(datastream);
                //////////////string responseMessage = reader.ReadToEnd();

                //////////////reader.Close();

                //////////////return responseMessage;

                WebResponse response = request.GetResponse();

                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    string responseMessage = reader.ReadToEnd();
                    reader.Close();

                    return responseMessage; // Return JSON response on success
                }
                else if (((HttpWebResponse)response).StatusCode == HttpStatusCode.NotFound)
                {
                    // Handle 404 status code here
                    return "404 Error: Resource not found";
                }
                else
                {
                    // Handle other status codes if needed
                    return "Unhandled status code: " + ((HttpWebResponse)response).StatusCode.ToString();
                }


            }
            catch (WebException e)
            {
                Stream datastream = e.Response.GetResponseStream();
                StreamReader reader = new StreamReader(datastream);
                string responseMessage = reader.ReadToEnd();
                reader.Close();

                // Return the JSON response for the 404 error from WebException
                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
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

        private NetworkCredential GetCredentials(SysDBInfoVMTemp connVM = null)
        {

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            //if (code.ToLower() == "purofood")
            //{
            //    ////return new NetworkCredential("rest", "123456");
            //    return new NetworkCredential("vatuser", "123456");

            //}
            //else
            //{
            //    //////// read from config file
            //    ////return new NetworkCredential("rest", "123456");
            //    return new NetworkCredential("vatuser", "123456");
            //}

            return new NetworkCredential("vatuser", "123456");


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

    }

}
