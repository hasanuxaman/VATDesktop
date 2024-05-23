using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows.Forms;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library.Integration
{
    public class BritishCouncil_IntegrationDAL
    {

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #region Sales

        public DataTable GetSaleData(string xml, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
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
                DataTable dtapiData = new DataTable();
                DataSet ds = new DataSet();

                ds = GetSalesXMLData(xml);

                dt = ds.Tables[0];
                dtapiData = ds.Tables[1];

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
create table #saletemp(
sl int identity(1,1), 
ID varchar(6000),
Branch_Code varchar(100),
CustomerGroup varchar(100),
Customer_Name varchar(100),
Customer_Code varchar(100),
Delivery_Address varchar(500),
CustomerAddress varchar(500),
CustomerBIN varchar(200),
Invoice_Date_Time varchar(100),
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
Trading_MarkUp varchar(100),
[Type] varchar(100),
Discount_Amount decimal(25,7),
Promotional_Quantity decimal(25,7),
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

PreviousSubTotal decimal(25,7),
PreviousVATAmount decimal(25,7),
PreviousSDAmount decimal(25,7),
Option3 varchar(100),

CompanyCode varchar(100),
IsProcessed varchar(100),
ItemNo varchar(100),
CustomerId varchar(100),

)";

                #endregion

                #region UpdateTypeVATRate

                string UpdateTypeVATRate = @"

update #saletemp set ItemNo='0'
update #saletemp set CustomerId='0'


----------- Type Update --------------

update #saletemp set #saletemp.[Type]='VAT'  where #saletemp.VAT_Rate='15'

 update #saletemp set #saletemp.[Type]='NonVAT' where #saletemp.VAT_Rate='0'

 update #saletemp set #saletemp.[Type]='OtherRate' where #saletemp.VAT_Rate!='0' and #saletemp.VAT_Rate!='15' 


------------------- TransactionType Update-------------------

update #saletemp set #saletemp.TransactionType='ExportServiceNS', Sale_type = 'New' where #saletemp.TransactionType='Export'
update #saletemp set #saletemp.TransactionType='ServiceNS', Sale_type = 'New' where #saletemp.TransactionType='Local' or #saletemp.TransactionType='DOMESTIC'
update #saletemp set #saletemp.TransactionType='ServiceNS', Sale_type = 'credit' where #saletemp.TransactionType='credit'

update #saletemp set ItemNo=Products.ItemNo from
#saletemp inner join
Products on Products.ProductCode=#saletemp.Item_Code  and  (#saletemp.ItemNo = '0' or #saletemp.ItemNo is null or #saletemp.ItemNo='')
and #saletemp.Item_Code!='0' and #saletemp.Item_Code!='-'

update #saletemp set CustomerId=Customers.CustomerID from
#saletemp inner join
Customers on Customers.CustomerCode=#saletemp.Customer_Code  
and  (#saletemp.CustomerID = '0' or #saletemp.CustomerID is null or #saletemp.CustomerID='')
and #saletemp.Customer_Code!='0' and #saletemp.Customer_Code!='-'

";

                #endregion

                #region getAllNewMasterData

                string getAllNewMasterData = @"

select distinct count(Customer_Code) count, 'Customer' Type from #saletemp where CustomerID='0'
union all 
select distinct count(Customer_Code)count, 'Product' Type from #saletemp where ItemNo='0'

";

                #endregion

                #region getAllData

                string getAllData = @"

select 
 ID
,'001' Branch_Code
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
,Vehicle_No
,VehicleType
,CustomerGroup
,Delivery_Address
,Comments
,Sale_Type
,TransactionType
,Discount_Amount
,Promotional_Quantity
,SD_Rate
,SDAmount
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
,CustomerAddress
,Option3
,CompanyCode DataSource
from #saletemp
where 1=1 

order by ID,TransactionType

";

                getAllData += @" drop table #saletemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                string columns = dt.GetColumns();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                cmd.CommandText = UpdateTypeVATRate;
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllNewMasterData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();
                adapter.Fill(dt);

                SaveMasterData(dt, currConn, transaction, connVM);


                cmd.CommandText = getAllData;
                adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    if (dtapiData != null || dtapiData.Rows.Count != 0)
                    {
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

        public DataSet GetSalesXMLData(string xml)
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable();
            DataTable dtapiData = new DataTable();

            try
            {

                var dataSet = new DataSet();
                StringReader reader = new StringReader(xml);
                dataSet.ReadXml(reader);
                table = dataSet.Tables[0];

                dtapiData = table.Copy();

                table = OrdinaryVATDesktop.FormatSaleData(table);

                foreach (DataRow row in table.Rows)
                {
                    row["Invoice_Date_Time"] = Convert.ToDateTime(row["Invoice_Date_Time"]).ToString("yyyy/MM/dd HH:mm:ss");
                }

                ds.Tables.Add(table);
                ds.Tables.Add(dtapiData);
            }

            catch (ArgumentException e)
            {
                //FileLogger.Log("DHL form", "GetIBS", "Price can not be zero" + srcFileName);
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return ds;
        }

        private void TableValidation(string branchCode, DataTable table)
        {
            if (!table.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = branchCode };
                table.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = "API" };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };

            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            table.Columns.Add(column);
            table.Columns.Add(CreatedBy);
            table.Columns.Add(CreatedOn);

            if (!table.Columns.Contains("ReturnId"))
            {
                table.Columns.Add(ReturnId);
            }

            table.Columns.Add(BOMId);

            foreach (DataRow row in table.Rows)
            {
                if (row["TransactionType"].ToString() == "Local Sale")
                {
                    row["TransactionType"] = "Other";
                }

                if (Convert.ToDecimal(row["VAT_Rate"]) == 5)
                {
                    row["Type"] = "OtherRate";
                }


            }
        }

        public DataTable SelectSalesDataForAPI(DataTable dtIds, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                #region SqlText

                sqlText += @" SELECT
sih.SalesInvoiceNo InvoiceNo
,isnull(sih.ImportIDExcel,'')ID
,isnull(sih.Option3,'')Option3
,c.CustomerName Customer_Name
,sih.InvoiceDateTime Invoice_Date_Time
,isnull(sih.TotalAmount,0) TotalAmount 
,isnull(sih.TotalVATAmount,0) TotalVATAmount 
,'Success' Status
,'Data save successfully' Message
FROM SalesInvoiceHeaders sih 
left outer join Customers c on sih.CustomerID=c.CustomerID 
WHERE  1=1 
";
                #endregion SqlText

                string ids = "";

                foreach (DataRow dataRow in dtIds.Rows)
                {
                    ids += "'" + dataRow["ID"] + "'" + ",";
                }
                ids = ids.TrimEnd(',');

                sqlText += " AND sih.ImportIDExcel IN(" + ids + ")";

                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SelectSalesDataForAPI", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable SelectResponseDataForAPI(DataTable dtIds, string GetNumber, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                #region SqlText

                sqlText += @" 
SELECT
 InvoiceNo
,TransactionNumber
,FileId
,VendorCustomer
,InvoiceDateTime
,TotalAmount 
,TotalVATAmount 
,Status
,Message
FROM TempResponse 
WHERE  1=1 
and GetNumber=@GetNumber
";
                #endregion SqlText

                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@GetNumber", GetNumber);
                da.SelectCommand.CommandTimeout = 500;

                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SelectResponseDataForAPI", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        #endregion

        #region Purchase

        public DataTable GetPurchaseData(string xml, SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string IsProcess = "N")
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

                dt = GetPurchaseXMLData(xml);

                dtapiData = dt.Copy();

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
Referance_No varchar(100),
LC_No varchar(100),
BE_Number varchar(100),
Receive_Date varchar(100),
Invoice_Date varchar(100),
Post varchar(100),
With_VDS varchar(100),
Comments varchar(200),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
Total_Price decimal(25,7),
UOM varchar(100),
Type varchar(100),
Rebate_Rate decimal(25,7),
SD_Amount decimal(25,7),
VAT_Amount decimal(25,7),
CnF_Amount decimal(25,7),
Insurance_Amount decimal(25, 2),
Assessable_Value decimal(25, 2),
CD_Amount decimal(25,7),
RD_Amount decimal(25,7),
AITAmount decimal(25,7),
AT_Amount decimal(25,7),
Others_Amount decimal(25,7),
Remarks varchar(100),
Transection_Type varchar(100),
FileName varchar(100),
FileID varchar(100),
CompanyCode varchar(100),

)";

                #endregion

                string trnsTypeUpdate = @"
update #purchasetemp set Transection_Type='Other' where Transection_Type='Local'
";

                #region getAllData

                string getAllData = @"

select 
 ID
,'001' BranchCode
,Vendor_Code 
,Vendor_Name 
,Referance_No 
,LC_No 
,BE_Number 
,Receive_Date 
,Invoice_Date 
,Post 
,With_VDS
,Comments
,Item_Code
,Item_Name
,Quantity 
,Total_Price 
,UOM 
,Type
,Rebate_Rate
,SD_Amount 
,VAT_Amount
,CnF_Amount
,Insurance_Amount
,Assessable_Value
,CD_Amount
,RD_Amount
,AITAmount
,AT_Amount
,Others_Amount 
,Remarks 
,Transection_Type
,FileID FileName
,CompanyCode

from #purchasetemp
where 1=1 

";

                getAllData += @" drop table #purchasetemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#purchasetemp", dt, currConn, transaction);

                cmd.CommandText = trnsTypeUpdate;
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllData;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                try
                {
                    if (dtapiData != null || dtapiData.Rows.Count != 0)
                    {
                        var ProcessDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };

                        dtapiData.Columns.Add(ProcessDateTime);

                        commonDal.BulkInsert("PurchaseAudits", dtapiData, currConn, transaction);
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
        }

        public DataTable GetPurchaseXMLData(string xml)
        {

            DataTable table = new DataTable();

            try
            {

                DataSet dataSet = OrdinaryVATDesktop.GetDataSetFromXML(xml);

                table = dataSet.Tables[0];

                if (table == null || table.Rows.Count == 0)
                {
                    throw new ArgumentNullException("", "Purchase Import Fail");
                }

            }

            catch (ArgumentException e)
            {
                //FileLogger.Log("DHL form", "GetIBS", "Price can not be zero" + srcFileName);
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return table;
        }

        #region Get data for API Response

        public DataTable SelectPurchaseDataForAPI(DataTable dtIds, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                #region SqlText

                sqlText += @" SELECT
pih.PurchaseInvoiceNo InvoiceNo
,isnull(pih.ImportIDExcel,'')ID
,v.VendorName Vendor_Name
,pih.FileName FileName
,pih.InvoiceDateTime Invoice_Date
,isnull(pih.TotalAmount,0) TotalAmount 
,isnull(pih.TotalVATAmount,0) TotalVATAmount 
,'Success' Status
,'Data save successfully' Message

FROM PurchaseInvoiceHeaders pih 
left outer join Vendors v on pih.VendorID=v.VendorID 
WHERE  1=1 
";
                #endregion SqlText

                string ids = "";

                foreach (DataRow dataRow in dtIds.Rows)
                {
                    ids += "'" + dataRow["ID"] + "'" + ",";
                }
                ids = ids.TrimEnd(',');

                sqlText += " AND pih.ImportIDExcel IN(" + ids + ")";

                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SelectPurchaseDataForAPI", ex.ToString() + "\n" + sqlText, "Purchase");
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }


        #endregion

        #endregion

        public string[] SaveAPIData(DataTable InvoiceData, string Status = "", string Message = "", bool isPurchase = false, string GetNumber = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable apiData = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            string[] result = new string[6];

            #endregion

            #region try

            try
            {
                apiData = new DataTable();

                apiData = InvoiceData.Copy();

                #region Make Datatable

                if (!apiData.Columns.Contains("ID"))
                {
                    var columnName = new DataColumn("TransactionNumber") { DefaultValue = "" };
                    apiData.Columns.Add(columnName);
                }
                else
                {
                    apiData.Columns["ID"].ColumnName = "TransactionNumber";
                }

                if (!apiData.Columns.Contains("InvoiceNo"))
                {
                    var columnName = new DataColumn("InvoiceNo") { DefaultValue = "" };
                    apiData.Columns.Add(columnName);
                }

                if (isPurchase)
                {
                    #region Purchase

                    if (!apiData.Columns.Contains("Invoice_Date"))
                    {
                        var columnName = new DataColumn("InvoiceDateTime") { DefaultValue = "" };
                        apiData.Columns.Add(columnName);
                    }
                    else
                    {
                        apiData.Columns["Invoice_Date"].ColumnName = "InvoiceDateTime";
                    }

                    if (!apiData.Columns.Contains("Vendor_Name"))
                    {
                        var columnName = new DataColumn("VendorCustomer") { DefaultValue = "" };
                        apiData.Columns.Add(columnName);
                    }
                    else
                    {
                        apiData.Columns["Vendor_Name"].ColumnName = "VendorCustomer";
                    }

                    if (!apiData.Columns.Contains("FileName"))
                    {
                        var columnName = new DataColumn("FileId") { DefaultValue = "" };
                        apiData.Columns.Add(columnName);
                    }
                    else
                    {
                        apiData.Columns["FileName"].ColumnName = "FileId";
                    }

                    #endregion

                }
                else
                {
                    #region Sales

                    if (!apiData.Columns.Contains("Invoice_Date_Time"))
                    {
                        var columnName = new DataColumn("InvoiceDateTime") { DefaultValue = "" };
                        apiData.Columns.Add(columnName);
                    }
                    else
                    {
                        apiData.Columns["Invoice_Date_Time"].ColumnName = "InvoiceDateTime";
                    }

                    if (!apiData.Columns.Contains("Customer_Name"))
                    {
                        var columnName = new DataColumn("VendorCustomer") { DefaultValue = "" };
                        apiData.Columns.Add(columnName);
                    }
                    else
                    {
                        apiData.Columns["Customer_Name"].ColumnName = "VendorCustomer";
                    }

                    if (!apiData.Columns.Contains("Option3"))
                    {
                        var columnName = new DataColumn("FileId") { DefaultValue = "" };
                        apiData.Columns.Add(columnName);
                    }
                    else
                    {
                        apiData.Columns["Option3"].ColumnName = "FileId";
                    }

                    #endregion

                }

                if (!apiData.Columns.Contains("TotalAmount"))
                {
                    var columnName = new DataColumn("TotalAmount") { DefaultValue = "0" };
                    apiData.Columns.Add(columnName);
                }
                if (!apiData.Columns.Contains("TotalVATAmount"))
                {
                    var columnName = new DataColumn("TotalVATAmount") { DefaultValue = "0" };
                    apiData.Columns.Add(columnName);
                }

                if (!apiData.Columns.Contains("Status"))
                {
                    var columnName = new DataColumn("Status") { DefaultValue = Status };
                    apiData.Columns.Add(columnName);
                }
                if (!apiData.Columns.Contains("Message"))
                {
                    var columnName = new DataColumn("Message") { DefaultValue = Message };
                    apiData.Columns.Add(columnName);
                }
                if (!apiData.Columns.Contains("GetNumber"))
                {
                    var columnName = new DataColumn("GetNumber") { DefaultValue = GetNumber };
                    apiData.Columns.Add(columnName);
                }

                #endregion

                result = commonDal.BulkInsert("TempResponse", apiData, null, null, 10000, null, connVM);

            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SaveAPIData", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return result;
        }

        public DataTable SelectSalesDataForPDFExport(string ImportId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                #region SqlText

                sqlText += @" SELECT
sih.SalesInvoiceNo InvoiceNo
,isnull(sih.ImportIDExcel,'') ImportID
,c.CustomerName Customer_Name
,c.CustomerCode
,isnull(c.Email,'-') Email
,sih.InvoiceDateTime
,isnull(sih.IsPDFGenerated,'N') IsPDFGenerated
,isnull(sih.IsSendMail,'N') IsSendMail
,isnull(sih.TransactionType,'')TransactionType
FROM SalesInvoiceHeaders sih 
left outer join Customers c on sih.CustomerID=c.CustomerID 
WHERE  1=1 
";
                #endregion SqlText

                sqlText += " AND sih.ImportIDExcel IN('" + ImportId + "')";

                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SelectSalesDataForAPI", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public string[] SaveMasterData(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable apiData = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            string[] result = new string[6];

            #endregion

            #region try

            try
            {
                bool IsNewProduct = false;
                bool IsNewCustomer = false;
                bool IsNewVendor = false;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string Type = dr["Type"].ToString();

                        if (Type.ToLower() == "product")
                        {

                        }
                        else if (Type.ToLower() == "customer")
                        {
                            SaveNewCustomerData(VcurrConn, Vtransaction, connVM);
                        }
                        else if (Type.ToLower() == "vendor")
                        {

                        }


                    }

                }



            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SaveAPIData", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return result;
        }

        public string[] SaveNewCustomerData(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable apiData = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            string[] result = new string[6];

            #endregion

            #region try

            try
            {
                DataTable dt = new DataTable();

                DataTable dtORS = SaveCustomerMasterData(true, true);

                DataTable dtSRS = SaveCustomerMasterData(false, true);

                dt = dtORS.Copy();
                dt.Merge(dtSRS);



            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SaveAPIData", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return result;
        }

        public DataTable SaveCustomerMasterData(bool IsORS, bool IsWeb, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable apiData = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            string[] result = new string[6];

            #endregion

            #region try

            try
            {
                string path = "";

                if (IsWeb)
                {
                    path = HostingEnvironment.MapPath("~/Logs/Regular/Customers/"); ;
                }
                else
                {
                    path = Application.StartupPath + "\\Logs\\Customers";
                }

                ////string FileName = "ORS-20240415.csv";

                string FileDate = "2024-04-15" + " " + DateTime.Now.ToString("HH:mm:ss");

                DateTime dateTime = Convert.ToDateTime(FileDate);

                string fileType = "";
                string url = "";
                string FileFormate = ".csv";

                if (IsORS)
                {
                    path = Path.Combine(path, "ORS");
                    fileType = "ORS";
                    url = "https://pweintegrationsa.blob.core.windows.net/invoicing-staging/ORS/@HttpsFileName?sv=2023-11-03&si=invoicing-staging-read&sr=c&sig=tob9mERwkDoIzOd7eINm8ZZ3%2BVi70YGSboqq46jBlVo%3D";

                }
                else
                {
                    path = Path.Combine(path, "SRS");
                    fileType = "SRS";
                    url = "https://pweintegrationsa.blob.core.windows.net/invoicing-staging/ORS/@HttpsFileName?sv=2023-11-03&si=invoicing-staging-read&sr=c&sig=tob9mERwkDoIzOd7eINm8ZZ3%2BVi70YGSboqq46jBlVo%3D";

                }

                string HttpsFile = fileType + "-" + dateTime.ToString("yyyyMMdd") + FileFormate;

                url = url.Replace("@HttpsFileName", HttpsFile);

                string destinationFilePath = path;

                if (!Directory.Exists(destinationFilePath))
                {
                    Directory.CreateDirectory(destinationFilePath);
                }

                string NewFileName = fileType + "-" + dateTime.ToString("yyyyMMddHHmmss") + FileFormate;

                string filePath = Path.Combine(destinationFilePath, NewFileName);

                HttpsDataGet(url, filePath);

                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = filePath;

                DataSet ds = GetDataSetFromExcel(paramVm);

                if (ds.Tables.Count > 0) // Ensure there's at least one table in the DataSet
                {
                    apiData = ds.Tables[0];
                }


            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("BritishCouncil_IntegrationDAL", "SaveAPIData", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return apiData;
        }

        static void HttpsDataGet(string url, string filePath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Console.WriteLine("Starting HTTP GET request...");
                    HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine("HTTP GET request successful.");

                    byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    Console.WriteLine("Content read successfully.");

                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                    {
                        fs.WriteAsync(content, 0, content.Length).GetAwaiter().GetResult();
                        Console.WriteLine("File written successfully.");
                    }
                }
                catch (HttpRequestException e)
                {
                    // Log the exception or handle it accordingly
                    Console.Error.WriteLine("Request error: {e.Message}");
                    throw;
                }

                catch (Exception e)
                {
                    // Log the exception or handle it accordingly
                    Console.Error.WriteLine("Unexpected error: {e.Message}");
                    throw;
                }
            }
        }

        public DataSet GetDataSetFromExcel(ImportVM paramVM)
        {
            DataSet ds = new DataSet();

            #region try

            try
            {
                string FileName;
                string newPath;
                string Fullpath = ReadFile(paramVM, out FileName, out newPath);

                FileStream stream = File.Open(newPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;

                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else if (FileName.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else if (FileName.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else if (FileName.EndsWith(".csv"))
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream);
                }

                ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true },
                    UseColumnDataType = false
                });

                reader.Close();

                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    System.IO.File.Delete(Fullpath);
                }

                return ds;

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ImportDAL", "GetDataSetFromExcel", ex.ToString());

                throw ex;
            }
            #endregion

        }

        private string ReadFile(ImportVM paramVM, out string FileName, out string newPath)
        {
            string Fullpath = "";
            FileName = "";
            if (paramVM.File != null && paramVM.File.ContentLength > 0)
            {
                FileName = paramVM.File.FileName;
                Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                System.IO.File.Delete(Fullpath);
                paramVM.File.SaveAs(Fullpath);
            }
            else
            {
                Fullpath = paramVM.FilePath;
                FileName = Fullpath;
            }

            string copyPath = AppDomain.CurrentDomain.BaseDirectory + @"\Temp\";

            Directory.CreateDirectory(copyPath);

            newPath = copyPath + Path.GetFileName(Fullpath);

            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            File.Copy(Fullpath, newPath);
            return Fullpath;
        }



    }
}
