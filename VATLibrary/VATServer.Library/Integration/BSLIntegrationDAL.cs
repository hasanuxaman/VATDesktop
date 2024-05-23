
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
using Excel;

namespace VATServer.Library
{
    public class BSLIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        public BSLIntegrationDAL()
        {
        }


        #region Purchase Data

        public void PurchaseTableValidation(DataTable purchaseData)
        {
            List<string> oldColumnNames = new List<string> { "Booking Date", "Document Date", "Document Number","Type", "Cost Center/Store", "Article Number", "Article Name", "QTY","Unit", "Transfer Cost Center/Store", "Supplier","Supplier Account","COS","COS Total","Sales Price","Sales Price Total","Invoiced","Item Group","Major Group","Over Group"};

            List<string> newColumnNames = new List<string> 
{
"BookingDate",
"DocumentDate",
"DocumentNumber",
"Type",
"CostCenter",
"ArticleNumber",
"ArticleName",
"QTY",
"Unit",
"TransferCostCenter",
"Supplier",
"SupplierAccount",
"COS",
"COSTotal",
"SalesPrice",
"SalesPriceTotal",
"Invoiced",
"ItemGroup",
"MajorGroup",
"OverGroup"
};

            purchaseData = OrdinaryVATDesktop.DtColumnNameChangeList(purchaseData, oldColumnNames, newColumnNames);

            if (purchaseData.Columns.Contains("ProcessDateTime"))
            {
                purchaseData.Columns.Remove("ProcessDateTime");
            }
            
            List<string> ColumnNames = new List<string> { "BIN", "User" };
            OrdinaryVATDesktop.DtDeleteColumns(purchaseData, ColumnNames);

        }

        public string[] SavePurchaseData(DataTable PurchaseData, SysDBInfoVMTemp connVm = null)
        {
            PurchaseDAL purchaseDal = new PurchaseDAL();
            CommonDAL commonDal = new CommonDAL();

            string[] result = new string[6];
            result[0] = "Fail";
            result[1] = "Fail";

            try
            {
                if (!PurchaseData.Columns.Contains("ProcessDateTime"))
                {
                    var processDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };
                    PurchaseData.Columns.Add(processDateTime);
                }

                result = commonDal.BulkInsert("PurchaseAudits", PurchaseData, null, null);

                PurchaseData = UpdateVatRateAmount(PurchaseData, null, null, null);

                AddNewColumn(PurchaseData);
                PurchaseTableMapping(PurchaseData);
                RemoveNegativeQuantityRows(PurchaseData);

                if (PurchaseData.Columns.Contains("VATRate"))
                {
                    PurchaseData.Columns.Remove("VATRate");
                }

                result = purchaseDal.SaveTempPurchase(PurchaseData, "", "", "API", 1, () => { }, null, null, connVm);

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
        }

        static void RemoveNegativeQuantityRows(DataTable dataTable)
        {
            for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dataTable.Rows[i];
                decimal quantity = Convert.ToDecimal(row["Quantity"].ToString());

                if (quantity < 0)
                {
                    dataTable.Rows.RemoveAt(i);
                }
            }
        }

        public void PurchaseTableMapping(DataTable purchaseData)
        {
            #region Remove UnusedColumn
            if (purchaseData.Columns.Contains("CostCenter"))
            {
                purchaseData.Columns.Remove("CostCenter");
            }
            if (purchaseData.Columns.Contains("Invoiced"))
            {
                purchaseData.Columns.Remove("Invoiced");
            }
            if (purchaseData.Columns.Contains("MajorGroup"))
            {
                purchaseData.Columns.Remove("MajorGroup");
            }
            if (purchaseData.Columns.Contains("ItemGroup"))
            {
                purchaseData.Columns.Remove("ItemGroup");
            }
            if (purchaseData.Columns.Contains("SalesPrice"))
            {
                purchaseData.Columns.Remove("SalesPrice");
            }
            if (purchaseData.Columns.Contains("ProcessDateTime"))
            {
                purchaseData.Columns.Remove("ProcessDateTime");
            }

            #endregion

            List<string> oldColumnNames = new List<string> {
                "BookingDate",
                "DocumentDate",
                "DocumentNumber",
                "Type",
                "ArticleNumber",
                "ArticleName",
                "QTY",
                "Unit",
                "TransferCostCenter",
                "Supplier",
                "SupplierAccount",
                "COS",
                "COSTotal",
                "SalesPriceTotal",
                "OverGroup",
            };

            List<string> newColumnNames = new List<string> 
            {
                "Invoice_Date",
                "Receive_Date",
                "ID",
                "Type",
                "Item_Code",
                "Item_Name",
                "Quantity",
                "UOM",
                "Custom_House",
                "Vendor_Name",
                "Vendor_Code",
                "CD_Amount",
                "Total_Price",
                "Assessable_Value",
                "Product_Group",
            };

            purchaseData = OrdinaryVATDesktop.DtColumnNameChangeList(purchaseData, oldColumnNames, newColumnNames);

        }

        public void AddNewColumn(DataTable PurchaseData)
        {
            if (!PurchaseData.Columns.Contains("Post"))
            {
                var post = new DataColumn("Post") { DefaultValue = "N" };
                PurchaseData.Columns.Add(post);
            }
            if (!PurchaseData.Columns.Contains("With_VDS"))
            {
                var With_VDS = new DataColumn("With_VDS") { DefaultValue = "N" };
                PurchaseData.Columns.Add(With_VDS);
            }
            if (!PurchaseData.Columns.Contains("Rebate_Rate"))
            {
                var Rebate_Rate = new DataColumn("Rebate_Rate") { DefaultValue = "0" };
                PurchaseData.Columns.Add(Rebate_Rate);
            }
            if (!PurchaseData.Columns.Contains("SD_Amount"))
            {
                var SD_Amount = new DataColumn("SD_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(SD_Amount);
            }
            if (!PurchaseData.Columns.Contains("Insurance_Amount"))
            {
                var Insurance_Amount = new DataColumn("Insurance_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(Insurance_Amount);
            }
            if (!PurchaseData.Columns.Contains("CnF_Amount"))
            {
                var CnF_Amount = new DataColumn("CnF_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(CnF_Amount);
            }
            if (!PurchaseData.Columns.Contains("RD_Amount"))
            {
                var RD_Amount = new DataColumn("RD_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(RD_Amount);
            }
            if (!PurchaseData.Columns.Contains("Others_Amount"))
            {
                var Others_Amount = new DataColumn("Others_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(Others_Amount);
            }
            if (!PurchaseData.Columns.Contains("CPCName"))
            {
                var CPCName = new DataColumn("CPCName") { DefaultValue = "NA" };
                PurchaseData.Columns.Add(CPCName);
            }
            if (!PurchaseData.Columns.Contains("FixedVATRebate"))
            {
                var FixedVATRebate = new DataColumn("FixedVATRebate") { DefaultValue = "N" };
                PurchaseData.Columns.Add(FixedVATRebate);
            }
            if (!PurchaseData.Columns.Contains("HSCode"))
            {
                var HSCode = new DataColumn("HSCode") { DefaultValue = "N" };
                PurchaseData.Columns.Add(HSCode);
            }
            if (!PurchaseData.Columns.Contains("IsRebate"))
            {
                var IsRebate = new DataColumn("IsRebate") { DefaultValue = "Y" };
                PurchaseData.Columns.Add(IsRebate);
            }
            if (!PurchaseData.Columns.Contains("RebateDate"))
            {
                var RebateDate = new DataColumn("RebateDate") { DefaultValue = DateTime.Now.ToString() };
                PurchaseData.Columns.Add(RebateDate);
            }
            if (!PurchaseData.Columns.Contains("VDSAmount"))
            {
                var VDSAmount = new DataColumn("VDSAmount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(VDSAmount);
            }
            if (!PurchaseData.Columns.Contains("AITAmount"))
            {
                var AITAmount = new DataColumn("AITAmount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(AITAmount);
            }
            if (!PurchaseData.Columns.Contains("AT_Amount"))
            {
                var AT_Amount = new DataColumn("AT_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(AT_Amount);
            }
            if (!PurchaseData.Columns.Contains("TDS_Amount"))
            {
                var TDS_Amount = new DataColumn("TDS_Amount") { DefaultValue = "0" };
                PurchaseData.Columns.Add(TDS_Amount);
            }
            if (!PurchaseData.Columns.Contains("TDSRate"))
            {
                var TDSRate = new DataColumn("TDSRate") { DefaultValue = "0" };
                PurchaseData.Columns.Add(TDSRate);
            }
            if (!PurchaseData.Columns.Contains("BE_Number"))
            {
                var BE_Number = new DataColumn("BE_Number") { DefaultValue = "NA" };
                PurchaseData.Columns.Add(BE_Number);
            }
        }

        public DataTable UpdateVatRateAmount(DataTable PurchaseData, SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null)
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

                if (PurchaseData.Rows.Count == 0)
                {
                    return PurchaseData;
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

                #region UpdateVATRate&Amount&Type

                string UpdateVATRate = @"
UPDATE PurchaseAudits SET PurchaseAudits.VATRate=Products.VATRate , PurchaseAudits.VAT_Amount=(Products.VATRate*PurchaseAudits.COSTotal)/100,
PurchaseAudits.Type=CASE WHEN PurchaseAudits.VATRate = 15 THEN 'VAT' WHEN PurchaseAudits.VATRate = 0 THEN 'NoNVAT' ELSE 'OtherRate' END
FROM Products WHERE PurchaseAudits.ArticleNumber=Products.ProductCode ;

";
                #endregion

                #region getAllData

                string getAllData = @"

SELECT  
 BookingDate
,DocumentDate
,DocumentNumber
,Type
,CostCenter
,ArticleNumber
,ArticleName
,QTY
,Unit
,TransferCostCenter
,Supplier
,SupplierAccount
,COS
,COSTotal
,SalesPrice
,SalesPriceTotal
,VATRate
,VAT_Amount
,Invoiced
,ItemGroup
,MajorGroup
,OverGroup
,Transection_Type
,CompanyCode
,BranchId
,UserId
,ProcessDateTime

FROM PurchaseAudits
WHERE 1=1 

";

                #endregion

                SqlCommand cmd = new SqlCommand(UpdateVATRate, currConn, transaction);
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();

                PurchaseData = new DataTable();

                adapter.Fill(PurchaseData);

                transaction.Commit();

            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw e;
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return PurchaseData;
        }        

        public string[] ImportExcelFile(PurchaseMasterVM paramVm, SysDBInfoVMTemp connVm = null)
        {
            #region Initializ
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = ""; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImportExcelFile"; //Method Name

            #endregion

            #region try

            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable dtPurchaseM = new DataTable();

                string code = new CommonDAL().settingValue("CompanyCode", "Code", connVm);

                #region Excel Reader

                string fileName = paramVm.File.FileName;

                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(paramVm.File.InputStream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(paramVm.File.InputStream);
                }
                else if (fileName.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(paramVm.File.InputStream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();

                dt = ds.Tables[0];
                reader.Close();
                #endregion

                
                dtPurchaseM = ds.Tables["Data"];

                if (!dtPurchaseM.Columns.Contains("BranchId"))
                {
                    var BranchId = new DataColumn("BranchId") { DefaultValue = paramVm.BranchId };
                    dtPurchaseM.Columns.Add(BranchId);
                }
                if (!dtPurchaseM.Columns.Contains("UserId"))
                {
                    var UserId = new DataColumn("UserId") { DefaultValue = paramVm.CurrentUser };
                    dtPurchaseM.Columns.Add(UserId);
                }
                if (!dtPurchaseM.Columns.Contains("Transection_Type"))
                {
                    var transection_Type = new DataColumn("Transection_Type") { DefaultValue = "Import" };
                    dtPurchaseM.Columns.Add(transection_Type);
                }
                if (!dtPurchaseM.Columns.Contains("CompanyCode"))
                {
                    var companyCode = new DataColumn("CompanyCode") { DefaultValue = code };
                    dtPurchaseM.Columns.Add(companyCode);
                }

                #region Data Insert

                PurchaseTableValidation(dtPurchaseM);

                retResults = SavePurchaseData(dtPurchaseM, connVm);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[4]);
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                #endregion SuccessResult
            }
            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString();
                FileLogger.Log("BSLIntegrationDAL", "ImportExcelFile", ex.ToString() + "\n");
            }
            #endregion

            return retResults;
        }

        #endregion

        #region Sales Data
        
        public ResultVM ImportSalesDataFile(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();

            try
            {                
                SaleDAL salesDal = new SaleDAL();
                DataTable salesData = new DataTable();

                var listData = ReadAttachedFile(paramVM.File.FileName);
                salesData = ConvertListToDataTable(listData);
                GetSaleData(salesData, null, null, connVM);

                string[] result = salesDal.SaveAndProcess(salesData, () => { }, Convert.ToInt32(paramVM.BranchId), "", connVM, paramVM, null, null, paramVM.CurrentUserId);

                rVM.Status = result[0];
                rVM.Message = result[1];
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;
                throw ex;
            }

            return rVM;
        }

        public DataTable GetSaleData(DataTable dt, SqlConnection vConnection = null, SqlTransaction vTransaction = null,SysDBInfoVMTemp connVM = null)
        {
            #region variable
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {

                CommonDAL commonDal = new CommonDAL();

                string[] result = new[] { "Fail" };

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
CREATE TABLE #saletemp(
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
ReasonOfReturn varchar(100)
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

                #endregion

                #region UpdateItemName

                string updateItemName = @"
----------- Item Name & Customer Update --------------
update #saletemp set #saletemp.Item_Name=Products.ProductName
from Products where #saletemp.Item_Code=Products.ItemNo

update #saletemp set #saletemp.Customer_Name=Customers.CustomerName
from Customers where #saletemp.Customer_Code=Customers.CustomerCode
";

                #endregion

                #region getAllData

                string getAllData = @"

SELECT 
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

FROM #saletemp
WHERE 1=1 AND Quantity > 0 ";

                getAllData += @" drop table #saletemp ";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandText = UpdateTypeVATRate;
                cmd.ExecuteNonQuery();
                cmd.CommandText = updateItemName;
                cmd.ExecuteNonQuery();

                cmd.CommandText = getAllData;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
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

        public DataTable UpdateUnitPrice(DataTable dt)
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

                if (dt.Rows.Count == 0)
                {
                    return dt;
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["SDAmount"] = (Convert.ToDecimal(dr["SubTotal"].ToString()) * Convert.ToDecimal(dr["SD_Rate"].ToString())) / 100; 
                        dr["NBR_Price"] = Convert.ToDecimal(dr["SubTotal"].ToString()) / Convert.ToDecimal(dr["Quantity"].ToString());
                        dr["VAT_Amount"] = ((Convert.ToDecimal(dr["SubTotal"].ToString()) + Convert.ToDecimal(dr["SDAmount"].ToString())) * Convert.ToDecimal(dr["VAT_Rate"].ToString())) / 100;

                        dr["VehicleType"] = "-";
                        dr["CustomerGroup"] = "-";
                        dr["Delivery_Address"] = "-";
                        dr["Currency_Code"] = "BDT";
                        dr["CommentsD"] = "-";
                        dr["Non_Stock"] = "0";
                        dr["Trading_MarkUp"] = "0";
                        dr["PreviousQuantity"] = "0";
                        dr["VAT_Name"] = "VAT 4.3";
                        dr["ReasonOfReturn"] = "-";
                        dr["Previous_Invoice_No"] = "-";
                        
                    }
                }

            }
            catch (Exception e)
            {                
                throw e;
            }

            return dt;
        }        

        #region Sun Data Read Methods

        public List<HotelData> ReadAttachedFile(string fullFilePath)
        {
            try
            {
                List<HotelData> lstEntity = new List<HotelData>();
                string branchCode = "";
                string line;
                using (StreamReader sr = new StreamReader(fullFilePath, Encoding.GetEncoding("UTF-8")))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (EncodingValues(line, 0, 7).ToLower() == "version")
                        {
                            continue;
                        }

                        HotelData entity = new HotelData();
                        entity.ID = EncodingValues(line, 0, 15).Trim();
                        string invoice_Date_Time = EncodingValues(line, 15, 17).Trim();
                        int startIndex = 7;
                        int desiredLength = Math.Min(8, invoice_Date_Time.Length - startIndex);
                        string extractedSubstring = invoice_Date_Time.Substring(startIndex, desiredLength);
                        entity.Invoice_Date_Time = extractedSubstring.Substring(0, 4) + "-" + extractedSubstring.Substring(4, 2) + "-" + invoice_Date_Time.Substring(6, 2);
                        entity.UOM = EncodingValues(line, 32, 15).Trim();
                        string subTotal = EncodingValues(line, 47, 20).Trim();
                        entity.SubTotal = string.IsNullOrEmpty(subTotal) ? 0 : Convert.ToDecimal(subTotal.Substring(0, 16) + "." + subTotal.Substring(16, 2));
                        entity.Vehicle_No = EncodingValues(line, 67, 10).Trim();
                        entity.ReasonOfReturn = EncodingValues(line, 77, 3).Trim();
                        string item_Code = EncodingValues(line, 80, 12).Trim();
                        entity.Item_Code = string.IsNullOrEmpty(item_Code) ? "10166" : item_Code;
                        entity.Comments = EncodingValues(line, 92, 149).Trim();
                        string quantity = EncodingValues(line, 241, 15).Trim();
                        entity.Quantity = string.IsNullOrEmpty(quantity) ? 0 : Convert.ToDecimal(quantity);
                        string branch = EncodingValues(line, 256, 120).Trim();
                        if (string.IsNullOrEmpty(branchCode) && branchCode != branch && !string.IsNullOrEmpty(branch)) 
                        { 
                            branchCode = branch; 
                        }
                        if (!string.IsNullOrEmpty(branchCode) && branchCode != branch && !string.IsNullOrEmpty(branch))
                        {
                            branchCode = branch;
                        }
                        if (string.IsNullOrEmpty(branch)) 
                        { 
                            branch = branchCode; 
                        }
                        entity.Branch_Code = branch;

                        lstEntity.Add(entity);
                    }
                }

                return lstEntity;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string EncodingValues(string stTarget, int iStart, int iByteSize)
        {
            try
            {
                Encoding hEncoding = Encoding.GetEncoding("UTF-8");
                byte[] btBytes = hEncoding.GetBytes(stTarget);

                return hEncoding.GetString(btBytes, iStart, iByteSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable ConvertListToDataTable<T>(List<T> list)
        {
            try
            {
                DataTable dataTable = new DataTable();

                // Get the properties of the type
                var properties = typeof(T).GetProperties();

                // Create columns in DataTable based on the properties of the type
                foreach (var property in properties)
                {
                    dataTable.Columns.Add(property.Name, property.PropertyType);
                }
                // Add rows to DataTable based on the data in the list
                foreach (var item in list)
                {
                    var row = dataTable.NewRow();
                    foreach (var property in properties)
                    {
                        row[property.Name] = property.GetValue(item);
                    }
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class HotelData
        {
            public string ID { get; set; }
            public string Invoice_Date_Time { get; set; }
            public string UOM { get; set; }
            public decimal SubTotal { get; set; }
            public string Vehicle_No { get; set; }
            public string ReasonOfReturn { get; set; }
            public string Item_Code { get; set; }
            public string Comments { get; set; }
            public decimal Quantity { get; set; }
            public string Branch_Code { get; set; }
        }

        #endregion

        public void AddNewSalesColumn(DataTable dt)
        {
            if (!dt.Columns.Contains("PreviousInvoiceDateTime"))
            {
                var Sale_Type = new DataColumn("PreviousInvoiceDateTime") { DefaultValue = "1900-01-01" };
                dt.Columns.Add(Sale_Type);
            }
            if (!dt.Columns.Contains("Sale_Type"))
            {
                var Sale_Type = new DataColumn("Sale_Type") { DefaultValue = "New" };
                dt.Columns.Add(Sale_Type);
            }
            if (!dt.Columns.Contains("Customer_Code"))
            {
                var Customer_Code = new DataColumn("Customer_Code") { DefaultValue = "2024" };
                dt.Columns.Add(Customer_Code);
            }
            if (!dt.Columns.Contains("Customer_Name"))
            {
                var Customer_Name = new DataColumn("Customer_Name") { DefaultValue = "N/A" };
                dt.Columns.Add(Customer_Name);
            }
            if (!dt.Columns.Contains("Reference_No"))
            {
                var Reference_No = new DataColumn("Reference_No") { DefaultValue = "-" };
                dt.Columns.Add(Reference_No);
            }
            if (!dt.Columns.Contains("LC_Number"))
            {
                var LC_Number = new DataColumn("LC_Number") { DefaultValue = "-" };
                dt.Columns.Add(LC_Number);
            }
            if (!dt.Columns.Contains("Post"))
            {
                var post = new DataColumn("Post") { DefaultValue = "N" };
                dt.Columns.Add(post);
            }
            if (!dt.Columns.Contains("NBR_Price"))
            {
                var NBR_Price = new DataColumn("NBR_Price") { DefaultValue = 0 };
                dt.Columns.Add(NBR_Price);
            }
            if (!dt.Columns.Contains("Discount_Amount"))
            {
                var Discount_Amount = new DataColumn("Discount_Amount") { DefaultValue = 0 };
                dt.Columns.Add(Discount_Amount);
            }
            if (!dt.Columns.Contains("VAT_Rate"))
            {
                var VAT_Rate = new DataColumn("VAT_Rate") { DefaultValue = 15 };
                dt.Columns.Add(VAT_Rate);
            }
            if (!dt.Columns.Contains("VAT_Amount"))
            {
                var VAT_Amount = new DataColumn("VAT_Amount") { DefaultValue = 0 };
                dt.Columns.Add(VAT_Amount);
            }

            if (!dt.Columns.Contains("SD_Rate"))
            {
                var SD_Rate = new DataColumn("SD_Rate") { DefaultValue = 0 };
                dt.Columns.Add(SD_Rate);
            }
            if (!dt.Columns.Contains("SDAmount"))
            {
                var SDAmount = new DataColumn("SDAmount") { DefaultValue = 0 };
                dt.Columns.Add(SDAmount);
            }
            if (!dt.Columns.Contains("Is_Print"))
            {
                var Is_Print = new DataColumn("Is_Print") { DefaultValue = "N" };
                dt.Columns.Add(Is_Print);
            }
            if (!dt.Columns.Contains("Tender_Id"))
            {
                var Tender_Id = new DataColumn("Tender_Id") { DefaultValue = "0" };
                dt.Columns.Add(Tender_Id);
            }
            if (!dt.Columns.Contains("PreviousNBRPrice"))
            {
                var PreviousNBRPrice = new DataColumn("PreviousNBRPrice") { DefaultValue = 0 };
                dt.Columns.Add(PreviousNBRPrice);
            }
            if (!dt.Columns.Contains("PreviousSubTotal"))
            {
                var PreviousSubTotal = new DataColumn("PreviousSubTotal") { DefaultValue = 0 };
                dt.Columns.Add(PreviousSubTotal);
            }
            if (!dt.Columns.Contains("PreviousVATAmount"))
            {
                var PreviousVATAmount = new DataColumn("PreviousVATAmount") { DefaultValue = 0 };
                dt.Columns.Add(PreviousVATAmount);
            }
            if (!dt.Columns.Contains("PreviousVATRate"))
            {
                var PreviousVATRate = new DataColumn("PreviousVATRate") { DefaultValue = 0 };
                dt.Columns.Add(PreviousVATRate);
            }
            if (!dt.Columns.Contains("PreviousSD"))
            {
                var PreviousSD = new DataColumn("PreviousSD") { DefaultValue = 0 };
                dt.Columns.Add(PreviousSD);
            }
            if (!dt.Columns.Contains("PreviousSDAmount"))
            {
                var PreviousSDAmount = new DataColumn("PreviousSDAmount") { DefaultValue = 0 };
                dt.Columns.Add(PreviousSDAmount);
            }

        }


        #endregion

    }

}
