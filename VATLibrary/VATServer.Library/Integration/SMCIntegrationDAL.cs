using System;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;
using System.IO;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using System.Globalization;
using VATServer.Interface;
using VATServer.Ordinary;
using ExcelDataReader;


namespace VATServer.Library.Integration
{
    public class SMCIntegrationDAL
    {

        #region Declarations

        private string[] sqlResults;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        SaleDAL _SaleDAL = new SaleDAL();
        BranchProfileDAL _branchDAL = new BranchProfileDAL();
        ImportDAL _ImportDAL = new ImportDAL();
        TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

        #endregion

        #region SMC - Social Marketing Company

        ////SMC - SQL

        #region Sales

        public DataTable GetSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            DataTable dtSales = new DataTable();

            #endregion

            try
            {
                #region ConnectionInfo

                DataTable dtConnectionInfo = new DataTable();
                string CompanyCode = new CommonDAL().settings("CompanyCode", "Code", null, null, connVM);


                string BranchId = "1";

                if (CompanyCode.ToLower() != "smcholding")
                {

                    if (paramVM.DataSourceType == "Pharma")
                    {
                        BranchId = "339";
                    }
                    else
                    {
                        BranchId = "2";
                    }

                }


                dtConnectionInfo = _branchDAL.SelectAll(BranchId, null, null, null, null, true);

                #endregion

                #region Data Switching

                if (paramVM.DataSourceType == "Pharma")
                {
                    dtSales = GetSale_DBData_SMC_Pharma(paramVM.RefNo, paramVM.FromDate, paramVM.ToDate, dtConnectionInfo);
                    ////dtSales = GetSale_DBData_SMC_PharmaX(paramVM.RefNo, paramVM.FromDate, paramVM.ToDate, dtConnectionInfo);
                }
                else if (paramVM.DataSourceType.ToLower() == "primarysales")
                {
                    dtSales = GetSale_SMC_Consumer_PrimarySales(paramVM.RefNo, paramVM.FromDate, paramVM.ToDate, dtConnectionInfo);
                }
                else
                {
                    dtSales = GetSale_DBData_SMC_Consumer(paramVM.RefNo, paramVM.FromDate, paramVM.ToDate, dtConnectionInfo);
                }

                #endregion

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {



            }
            #endregion

            return dtSales;

        }

        public DataTable GetSale_DBData_SMC_PharmaX(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                sqlText = @"
SELECT  
req.IssueChalanNo															AS ID						  
, '001'																	    AS Branch_Code				  
, com.ComUnitName															AS Customer_Name			  
, ISNULL(com.ComUnitCode, '-')												AS Customer_Code			  
, com.Address																AS Delivery_Address			  
, '-'																		AS Vehicle_No
, '-' 																		AS VehicleType				  
, CAST(req.IssuChalanDate AS varchar(20)) 									AS Invoice_Date_Time		  
, CAST(req.IssuChalanDate AS varchar(20)) 									AS Delivery_Date_Time		  
, req.IssueChalanNo 														AS Reference_No				  
, '-' 																		AS Comments					  
, 'New' 																	AS Sale_Type				  
, '' 																		AS Previous_Invoice_No		  
, 'N' 																		AS Is_Print					  
, '0' 																		AS Tender_Id				  
, 'Y' 																		AS Post						  
, 'NA' 																		AS LC_Number				  
, 'BDT' 																	AS Currency_Code			  
, sit.ProductCode 															AS Item_Code				  
, sit.ProductName 															AS Item_Name				  
, ISNULL(NULLIF (sit.PackSize, '&nbsp;'), 'PACK') 							AS UOM						  
, sit.Quantity																AS Quantity					  
, (sit.PriceAmount+(sit.PriceAmount*0.16)) / sit.Quantity 											AS NBR_Price				  
, ROUND(ISNULL(sit.VATAmount, 0) / (sit.PriceAmount+(sit.PriceAmount*0.16)) * 100, 5) 				AS VAT_Rate					  
, 0 																		AS SD_Rate					  
, (sit.PriceAmount+(sit.PriceAmount*0.16)) 															AS SubTotal					  
, sit.VATAmount 															AS VAT_Amount				  
, sit.TotalPriceAmount 														AS TotalValue				  
, 'N' 																		AS Non_Stock				  
, 0 																		AS Trading_MarkUp
, CASE WHEN (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 >= 15 THEN 'VAT' 
WHEN (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 = 0 THEN 'NonVAT' 
WHEN (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 >= 0 AND (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 < 15 THEN 'OtherRate' 
END 																		AS Type						  
, 0 																		AS Discount_Amount			  
, 0 																		AS Promotional_Quantity		  
, 'VAT 4.3' 																AS VAT_Name	
, 'Other'                                                                   AS TransactionType	
, 'SMC'			                                                            AS CompanyCode
, 'Pharma'                                                                  AS DataSource
FROM	dbo.tblRequisition AS req 
LEFT OUTER JOIN	dbo.tblCompanyUnit AS com ON req.ComUnitId = com.ComUnitId 
LEFT OUTER JOIN dbo.tblStockInTransfar AS sit ON sit.ReqId = req.ReqId
WHERE        (1 = 1) AND (req.IssuChalanDate >= '2019-Nov-01')
AND ISNULL(sit.Quantity,0) > 0 
AND ISNULL(sit.PriceAmount,0) > 0

";
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" AND req.IssueChalanNo= @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" AND req.IssuChalanDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @" AND req.IssuChalanDate < dateadd(d,1,@toDate)";
                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 200;
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", SalesFromDate);
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", SalesToDate);
                }


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable GetSale_DBData_SMC_Pharma(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                sqlText = @"select * from GetVat(";

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" @fromDate";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @",dateadd(d,1,@toDate)";
                }

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" ,@SalesInvoiceNo";
                }
                else
                {
                    sqlText = sqlText + @",null";
                }
                sqlText = sqlText + @")";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 200;
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", SalesFromDate);
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", SalesToDate);
                }

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                if (table.Columns.Contains("Branch_Code"))
                {
                    table.Columns.Remove("Branch_Code");

                    var columnName = new DataColumn("Branch_Code") { DefaultValue = "018" };
                    table.Columns.Add(columnName);
                }
                else
                {
                    var columnName = new DataColumn("Branch_Code") { DefaultValue = "018" };
                    table.Columns.Add(columnName);
                }

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

               
                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable GetSale_DBData_SMC_ConsumerX(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();

            #endregion

            try
            {

                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction, connVM);

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                sqlText = @"
SELECT DISTINCT 
A.memo_no                                               AS ID
, s.id                                                  AS Branch_Code
, C.name                                                AS Customer_Name
, ISNULL(C.code, '-')                                   AS Customer_Code
, C.address                                             AS Delivery_Address
, '-'                                                   AS Vehicle_No
, '-' 													AS VehicleType	
, cast(A.memo_date as varchar(20))                      AS Invoice_Date_Time
, cast(A.memo_date as varchar(20))                      AS Delivery_Date_Time
, A.memo_no                                             AS Reference_No
, '-'                                                   AS Comments
, 'New'                                                 AS Sale_Type
, ''                                                    AS Previous_Invoice_No
, 'N'                                                   AS Is_Print
, '0'                                                   AS Tender_Id
, 'Y'                                                   AS Post
, 'NA'                                                  AS LC_Number
, 'BDT'                                                 AS Currency_Code
, D.product_code                                        AS Item_Code
, D.name                                                AS Item_Name
--, ISNULL(uom.name, 'Pcs')                             AS UOM
, 'Pcs'													AS UOM

, (CASE WHEN pm.qty_in_base IS NULL THEN 1 ELSE pm.qty_in_base END) * B.sales_qty                   AS Quantity
--, (B.price * 100) / (100 + ISNULL(E.vat, 0))                                                        AS NBR_Price
, (B.sales_qty * (B.price) * 100) / (100 + ISNULL(E.vat, 0))/
((CASE WHEN pm.qty_in_base IS NULL THEN 1 ELSE pm.qty_in_base END) * B.sales_qty)                    AS NBR_Price

, ISNULL(E.vat, 0)                                                                                  AS VAT_Rate
, 0                                                                                                 AS SD_Rate
, B.sales_qty * B.price                                                                             AS TotalValue
, (B.sales_qty * (B.price) * 100) / (100 + ISNULL(E.vat, 0))                                                AS SubTotal
, B.sales_qty * (B.price) - (B.sales_qty * (B.price) * 100) / (100 + ISNULL(E.vat, 0))                    AS VAT_Amount
, 'N'                                                                                               AS Non_Stock
, 0                                                                                                 AS Trading_MarkUp
, CASE 
WHEN isnull(E.vat, 0) = 15 THEN 'VAT' 
WHEN isnull(E.vat, 0) = 0 THEN 'NonVAT' 
WHEN isnull(E.vat, 0) > 0 AND isnull(E.vat, 0) < 15 THEN 'OtherRate' END                            AS Type
, 0                                                                                                 AS Discount_Amount
, 0                                                                                                 AS Promotional_Quantity
, 'VAT 4.3'                                                                                         AS VAT_Name
, 'Other'                                                                                           AS TransactionType
, 'SMC'			                                                                                    AS CompanyCode
, 'Consumer'                                                                                        AS DataSource
FROM            dbo.memos AS A LEFT OUTER JOIN
dbo.memo_details AS B ON A.id = B.memo_id LEFT OUTER JOIN
dbo.outlets AS C ON A.outlet_id = C.id LEFT OUTER JOIN
dbo.products AS D ON B.product_id = D.id LEFT OUTER JOIN
----dbo.product_prices AS E ON D.id = E.product_id AND E.vat IS NOT NULL LEFT OUTER JOIN
dbo.product_prices_v2 AS E ON D.id = E.product_id AND E.vat IS NOT NULL LEFT OUTER JOIN
dbo.measurement_units AS uom ON uom.id = B.measurement_unit_id LEFT OUTER JOIN
dbo.stores AS s ON A.territory_id = s.territory_id LEFT OUTER JOIN
dbo.product_measurements AS pm ON D.id = pm.product_id AND D.sales_measurement_unit_id = pm.measurement_unit_id
WHERE        (1 = 1) 
--AND (B.is_bonus = 1) 
AND (B.price > 0)
and a.memo_date >= '2019-Nov-01'
and ISNULL(s.id,0) > 0
and ((CASE WHEN pm.qty_in_base IS NULL THEN 1 ELSE pm.qty_in_base END) * B.sales_qty)  > 0
";
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" AND A.memo_no = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" AND A.memo_date >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @" AND A.memo_date < dateadd(d,1,@toDate)";
                }

                if (CompanyCode.ToLower() == "smcholding")
                {
                    sqlText = sqlText + @" AND d.source = 'smc'";
                }
                else
                {
                    sqlText = sqlText + @" AND d.source = 'smcel'";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 180;

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", SalesFromDate);
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", SalesToDate);
                }


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();

                }

                #endregion Commit

                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable GetSale_DBData_SMC_Consumer(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();

            #endregion

            try
            {

                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction, connVM);

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                sqlText = @"
SELECT DISTINCT 
ID
,Branch_Code
,Customer_Name
,Customer_Code
,Delivery_Address
,Vehicle_No
,VehicleType	
,Invoice_Date_Time
,Delivery_Date_Time
,Reference_No
,Comments
,Sale_Type
,Previous_Invoice_No
,Is_Print
,Tender_Id
,Post
,LC_Number
,Currency_Code
,Item_Code
,Item_Name
,UOM
,Quantity
,NBR_Price
,VAT_Rate
,SD_Rate
,TotalValue
,SubTotal
,VAT_Amount
,Non_Stock
,Trading_MarkUp
,Type
,Discount_Amount
,Promotional_Quantity
,VAT_Name
,TransactionType
,CompanyCode
,DataSource
from  [172.16.200.6].[smc_uat].[dbo].[VAT_Sales] sale where 1=1
";
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" AND sale.ID = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" AND sale.Invoice_Date_Time >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @" AND sale.Invoice_Date_Time < dateadd(d,1,@toDate)";
                }

                if (CompanyCode.ToLower() == "smcholding")
                {
                    sqlText = sqlText + @" AND sale.source = 'smc'";
                }
                else
                {
                    sqlText = sqlText + @" AND sale.source = 'smcel'";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 180;

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(SalesFromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(SalesToDate).ToString("yyyy-MM-dd"));
                }


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                //FileLogger.Log("SMC","Sales",sqlText + " "+ SalesFromDate + " " + SalesToDate);


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();

                }

                #endregion Commit

                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }


                throw ex;
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

        public ResultVM SaveSale_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    paramVM.SetSteps(1);
                    rVM = SaveSale(paramVM);
                    paramVM.callBack();
                }
                else
                {

                    DateTime FromDate = Convert.ToDateTime(paramVM.FromDate);
                    DateTime ToDate = Convert.ToDateTime(paramVM.ToDate);
                    Double Days = (ToDate - FromDate).TotalDays + 1;

                    if (paramVM.DataSourceType == "Pharma")
                    {
                        ToDate = ToDate.AddDays(-1);
                    }

                    DateTime CurrentDate = FromDate;

                    paramVM.SetSteps(Convert.ToInt32(Days));

                    int RowCount = 0;

                    for (int i = 0; i < Days; i++)
                    {
                        paramVM.FromDate = CurrentDate.ToString("yyyy-MMM-dd");
                        paramVM.ToDate = CurrentDate.ToString("yyyy-MMM-dd");
                        rVM = SaveSale(paramVM);

                        RowCount = RowCount + rVM.RowCount;

                        CurrentDate = CurrentDate.AddDays(1);
                        paramVM.callBack();

                    }

                    rVM.RowCount = RowCount;
                }

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

        public ResultVM SaveSale(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                DataTable dtSales = new DataTable();

                #region Get Data from Source DB

                dtSales = GetSource_SaleData(paramVM);

                if (dtSales == null || dtSales.Rows.Count == 0)
                {
                    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                #endregion

                DataTable BranchDt = GetNewBranch(dtSales.Copy(), null, null, connVM);

                #region Comments Aug-16-2020

                ////TableValidation(dtSales, paramVM);

                ////if (false)
                ////{
                ////    sqlResults = _SaleDAL.SaveAndProcess(dtSales, paramVM.callBack, paramVM.DefaultBranchId, null, null, paramVM);
                ////}

                #endregion


                #region Bulk Insert to Source Table

                sqlResults = _SaleDAL.ImportExcelIntegrationDataTable(dtSales, new SaleMasterVM(), null, null, null);

                #endregion

                #region Delete duplicate rows based on PeriodId and ID

                DeleteDuplicateSale(null, null);

                #endregion

                #region Bulk Insert to Sales

                DataTable dtVAT_Source_Sales = new DataTable();
                dtVAT_Source_Sales = Get_VAT_Source_Sales_IDs();

                if (dtVAT_Source_Sales == null || dtVAT_Source_Sales.Rows.Count == 0)
                {
                    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                    return rVM;
                }


                sqlResults = _ImportDAL.SaveSaleWithSteps_Split(dtVAT_Source_Sales, () => { }, paramVM.DefaultBranchId,
                    "", "", "", false, "", (s) => { });

                #endregion

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
                rVM.RowCount = dtSales.Rows.Count;
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

        public DataTable Get_VAT_Source_Sales_IDs(SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtVAT_Source_Sales = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }


                #endregion open connection and transaction

                sqlText = @"
SELECT 
DISTINCT 
'True' [Check]
,ID
FROM VAT_Source_Sales";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtVAT_Source_Sales);

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

            return dtVAT_Source_Sales;
        }

        public void DeleteDuplicateSale(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtVAT_Source_Sales = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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

                sqlText = @"

update  VAT_Source_Sales                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = ''


delete VAT_Source_Sales
FROM VAT_Source_Sales 
join SalesInvoiceHeaders on VAT_Source_Sales.ID = SalesInvoiceHeaders.ImportIDExcel 
and VAT_Source_Sales.PeriodId = SalesInvoiceHeaders.PeriodID
";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                int rows = cmd.ExecuteNonQuery();

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

                if (VcurrConn == null)
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
            #endregion

        }

        public DataTable GetSale_SMC_Consumer_PrimarySales(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();

            #endregion

            try
            {

                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction, connVM);

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"
SELECT 
ID
,Branch_Code
,Customer_Name
,Customer_Code
,Delivery_Address
,Vehicle_No
,VehicleType	
,Invoice_Date_Time
,Delivery_Date_Time
,Reference_No
,Comments
,Sale_Type
,Previous_Invoice_No
,Is_Print
,Tender_Id
,Post
,LC_Number
,Currency_Code
,Item_Code
,Item_Name
,UOM
,Quantity
,NBR_product_price NBR_Price
,VAT_Rate
,SD_Rate
,TotalValue
,SubTotal
,VAT_Amount
,Non_Stock
,Trading_MarkUp
,Type
,Discount_Amount
,Promotional_Quantity
,VAT_Name
,TransactionType
,CompanyCode
,'PrimarySales' DataSource
from  [172.16.200.6].[smc_uat].[dbo].[VAT_Primary_sales] sale where 1=1
";
                #endregion

                #region Condition

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" AND sale.ID = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" AND sale.Invoice_Date_Time >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @" AND sale.Invoice_Date_Time < dateadd(d,1,@toDate)";
                }

                if (CompanyCode.ToLower() == "smcholding")
                {
                    sqlText = sqlText + @" AND sale.source = 'smc'";
                }
                else
                {
                    sqlText = sqlText + @" AND sale.source = 'smcel'";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 180;

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(SalesFromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(SalesToDate).ToString("yyyy-MM-dd"));
                }

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #endregion Commit

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();
                }

                #endregion Commit

                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        #endregion

        #region Transfer

        public DataTable GetTransfer_DBData_SMC_ConsumerX(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            #endregion


            try
            {
                #region Credentials
                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction, connVM);

                BranchProfileDAL branchDAL = new BranchProfileDAL();
                string BranchId = "2";
                DataTable dt = branchDAL.SelectAll(BranchId, null, null, null, null, true);
                paramVM.dtConnectionInfo = dt;

                #endregion


                #region open connection and transaction


                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                sqlText = @"  
SELECT DISTINCT 
  c.challan_no																AS ID
, c.challan_date															AS TransactionDateTime
, c.challan_no																AS ReferenceNo
, c.remarks 																AS Comments
, cd.remarks 																AS CommentsD
, 'Y' 																		AS Post
, 'Finish' 																	AS TransactionType
, ISNULL(pr.product_code,'NA') 												AS ProductCode
, ISNULL(pr.name,'NA')  													AS ProductName
, cd.challan_qty * CASE WHEN pm.qty_in_base IS NULL THEN 1 ELSE pm.qty_in_base END AS Quantity
, '0' 																		AS CostPrice
, 'PCS' 																	AS UOM
, ISNULL(p.vat, 0) 															AS VAT_Rate
------, sendStore.name 													    AS SenderBranch
, ISNULL(sendStore.id,'0')  												AS BranchCode
-----, recStore.name 														AS ReceiverName
, ISNULL(recStore.id,'0')  													AS TransferToBranchCode
, 0 																		AS SDRate
, 'NA'                                                                      AS VehicleNo
, 'NA'                                                                      AS VehicleType



FROM dbo.challan_details AS cd 
LEFT OUTER JOIN dbo.challans AS c ON cd.challan_id = c.id 
LEFT OUTER JOIN dbo.product_prices AS p ON cd.product_id = p.id 
LEFT OUTER JOIN dbo.products AS pr ON cd.product_id = pr.id 
LEFT OUTER JOIN dbo.product_measurements AS pm ON cd.product_id = pm.product_id AND cd.measurement_unit_id = pm.measurement_unit_id 
LEFT OUTER JOIN dbo.stores AS sendStore ON c.sender_store_id = sendStore.id 
LEFT OUTER JOIN dbo.stores AS recStore ON c.receiver_store_id = recStore.id
WHERE        (1 = 1) AND (c.challan_date >= '2019-11-01')
and (ISNULL(sendStore.id,'0') != '0' or ISNULL(recStore.id,'0')=0)
";

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND c.challan_no = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND c.challan_date >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND c.challan_date < dateadd(d,1,@toDate)";
                }



                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 180;

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (CompanyCode.ToLower() == "smcholding")
                {
                    sqlText = sqlText + @" AND d.source = 'smc'";
                }
                else
                {
                    sqlText = sqlText + @" AND d.source = 'smcel'";
                }
                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();
                }

                #endregion Commit

                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable GetTransfer_DBData_SMC_Consumer(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            #endregion


            try
            {
                #region Credentials
                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction, connVM);

                BranchProfileDAL branchDAL = new BranchProfileDAL();
                string BranchId = "2";
                DataTable dt = branchDAL.SelectAll(BranchId, null, null, null, null, true);
                paramVM.dtConnectionInfo = dt;

                #endregion


                #region open connection and transaction


                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                sqlText = @"  
SELECT DISTINCT 
  ID
, TransactionDateTime
, ReferenceNo
, Comments
, CommentsD
, Post
, TransactionType
, ProductCode
, ProductName
, Quantity
, CostPrice
, UOM
, VAT_Rate
, BranchCode
, TransferToBranchCode
, SDRate
, VehicleNo
, VehicleType



from  [172.16.200.6].[smc_uat].[dbo].[VAT_Transfer6_5] where 1=1
";

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND TransactionDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND TransactionDateTime < dateadd(d,1,@toDate)";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 180;

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(paramVM.ToDate).ToString("yyyy-MM-dd"));
                }

                if (CompanyCode.ToLower() == "smcholding")
                {
                    sqlText = sqlText + @" AND source = 'smc'";
                }
                else
                {
                    sqlText = sqlText + @" AND source = 'smcel'";
                }
                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();
                }

                #endregion Commit

                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable Get_VAT_Source_Transfer(SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtVAT_Source_Sales = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }


                #endregion open connection and transaction

                sqlText = @"
SELECT [ID]
      ,[BranchCode]
      ,[TransferToBranchCode]
      ,[TransactionDateTime]
      , TransactionType
      ,isnull(VehicleNo,'NA')VehicleNo
      ,isnull(VehicleType,'NA')VehicleType
      ,[ProductCode]
      ,[ProductName]
      ,[UOM]
      ,[Quantity]
      ,((costprice*100)/(100+VAT_Rate)) CostPrice
      ,[Post]
      ,[VAT_Rate]
      ,[ReferenceNo]
      ,[Comments]
  FROM VAT_Source_TransferIssues";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtVAT_Source_Sales);

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

            return dtVAT_Source_Sales;
        }

        public void DeleteDuplicateTransfer(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtVAT_Source_Sales = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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

                sqlText = @"

update  VAT_Source_TransferIssues                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
where PeriodId is null or PeriodId = ''

delete from VAT_Source_TransferIssues where ID in(
select VAT_Source_TransferIssues.ID from 
VAT_Source_TransferIssues join TransferIssues on VAT_Source_TransferIssues.ID = TransferIssues.ImportIDExcel 
and VAT_Source_TransferIssues.PeriodId = TransferIssues.PeriodID
)";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                int rows = cmd.ExecuteNonQuery();

                string DeleteSampleData = @"
delete VAT_Source_TransferIssues where SL in (
select tpd.SL from 
VAT_Source_TransferIssues tpd
inner join products p on p.ProductCode = tpd.ProductCode
where isnull(p.IsSample,'N')='Y'
)
";

                cmd.CommandText = DeleteSampleData;
                cmd.ExecuteNonQuery();

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

                if (VcurrConn == null)
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
            #endregion

        }

        public ResultVM SaveTransfer_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    paramVM.SetSteps(1);
                    rVM = SaveTransfer(paramVM);
                    paramVM.callBack();
                }
                else
                {

                    DateTime FromDate = Convert.ToDateTime(paramVM.FromDate);
                    DateTime ToDate = Convert.ToDateTime(paramVM.ToDate);
                    Double Days = (ToDate - FromDate).TotalDays + 1;

                    DateTime CurrentDate = FromDate;

                    paramVM.SetSteps(Convert.ToInt32(Days));

                    for (int i = 0; i < Days; i++)
                    {
                        paramVM.FromDate = CurrentDate.ToString("yyyy-MMM-dd");
                        paramVM.ToDate = CurrentDate.ToString("yyyy-MMM-dd");
                        rVM = SaveTransfer(paramVM);

                        CurrentDate = CurrentDate.AddDays(1);
                        paramVM.callBack();

                    }
                }


            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;
                ////throw ex;
            }
            finally
            {


            }
            return rVM;
        }

        public ResultVM SaveTransfer(IntegrationParam paramVM, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            try
            {

                DataTable dtTransfers = new DataTable();

                #region Get Data from Source DB

                dtTransfers = GetTransfer_DBData_SMC_Consumer(paramVM);

                if (dtTransfers == null || dtTransfers.Rows.Count == 0)
                {
                    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                #endregion

                DataTable BranchDt = GetNewBranch(dtTransfers.Copy(), null, null, connVM);

                #region Cleare and Bulk Insert to Source Table

                DBSQLConnection dbsqlConnection = new DBSQLConnection();
                CommonDAL commonDal = new CommonDAL();

                connection = dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                string sqlText = "delete from VAT_Source_TransferIssues";
                SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                transResult = cmd.ExecuteNonQuery();
                sqlResults = commonDal.BulkInsert("VAT_Source_TransferIssues", dtTransfers, connection, transaction);

                transaction.Commit();
                transaction.Dispose();
                connection.Close();

                #endregion

                #region Delete duplicate rows based on PeriodId and ID

                DeleteDuplicateTransfer(null, null);

                #endregion

                #region Bulk Insert to Sales

                DataTable dtVAT_Source_Transfer = new DataTable();
                dtVAT_Source_Transfer = Get_VAT_Source_Transfer();


                if (dtVAT_Source_Transfer == null || dtVAT_Source_Transfer.Rows.Count == 0)
                {
                    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                    return rVM;
                }


                sqlResults = _TransferIssueDAL.SaveTempTransfer(dtVAT_Source_Transfer, paramVM.BranchCode,
                    paramVM.TransactionType, paramVM.CurrentUser, paramVM.DefaultBranchId, () => { }, null, null, false, null, "", UserId);

                #endregion


                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
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

        #region Table Validation

        private void TableValidation(DataTable dtSales, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            ////if (!dtSales.Columns.Contains("Branch_Code"))
            ////{
            ////    var columnName = new DataColumn("Branch_Code") { DefaultValue = param.DefaultBranchCode };
            ////    dtSales.Columns.Add(columnName);
            ////}

            ////var SL = new DataColumn("SL") { DefaultValue = "" };
            ////var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
            ////var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            ////var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            ////var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
            ////var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            ////if (!dtSales.Columns.Contains("SL"))
            ////{
            ////    dtSales.Columns.Add(SL);
            ////}
            ////if (!dtSales.Columns.Contains("CreatedBy"))
            ////{
            ////    dtSales.Columns.Add(CreatedBy);
            ////}
            ////if (!dtSales.Columns.Contains("CreatedOn"))
            ////{
            ////    dtSales.Columns.Add(CreatedOn);
            ////}
            ////if (!dtSales.Columns.Contains("ReturnId"))
            ////{
            ////    dtSales.Columns.Add(ReturnId);
            ////}

            ////if (!dtSales.Columns.Contains("TransactionType"))
            ////{
            ////    dtSales.Columns.Add(TransactionType);
            ////}
            ////if (!dtSales.Columns.Contains("BOMId"))
            ////{
            ////    dtSales.Columns.Add(BOMId);
            ////}
        }

        #endregion

        #region Backup

        public DataTable GetSale_DBData_SMC_Pharma_Backup(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                sqlText = @"
SELECT
req.IssueChalanNo ID
,com.ComUnitName Customer_Name, isnull(com.ComUnitCode,'-') Customer_Code
,com.Address Delivery_Address
, '-' Vehicle_No
,cast(req.IssuChalanDate as varchar(20)) Invoice_Date_Time
,cast(req.IssuChalanDate as varchar(20)) Delivery_Date_Time 
,req.IssueChalanNo  Reference_No  
,'-'Comments
,'New' Sale_Type
,'' Previous_Invoice_No
,'N' Is_Print
,'0' Tender_Id
,'Y' Post
,'NA' LC_Number
,'BDT' Currency_Code
,sit.ProductCode Item_Code
,sit.ProductName Item_Name
,isnull(NULLIF(sit.PackSize,'&nbsp;'),'PACK') UOM
,sit.Quantity Quantity
,(sit.PriceAmount/sit.Quantity) NBR_Price
,Round((isnull(sit.VATAmount,0)/sit.PriceAmount)*100, 5) VAT_Rate
,0 SD_Rate
,sit.PriceAmount SubTotal
,sit.VATAmount VAT_Amount
,sit.TotalPriceAmount TotalValue
,'N' Non_Stock
,0 Trading_MarkUp
,case 
when (isnull(sit.VATAmount,0)/sit.PriceAmount)*100 >= 15 then 'VAT'
when (isnull(sit.VATAmount,0)/sit.PriceAmount)*100 = 0 then 'NonVAT'
when (isnull(sit.VATAmount,0)/sit.PriceAmount)*100 >= 0 and (isnull(sit.VATAmount,0)/sit.PriceAmount)*100 < 15 then 'OtherRate'
end as Type
,0 Discount_Amount
,0 Promotional_Quantity
,'VAT 4.3' VAT_Name

from tblRequisition req 
left outer join tblCompanyUnit com on req.ComUnitId=com.ComUnitId
left outer join tblStockInTransfar sit on sit.ReqId=req.ReqId
where 1=1
";
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" and req.IssueChalanNo= @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" AND req.IssuChalanDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @" AND req.IssuChalanDate < dateadd(d,1,@toDate)";
                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", SalesFromDate);
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", SalesToDate);
                }


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable GetSale_DBData_SMC_Consumer_Backup(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                sqlText = @"
SELECT
A.memo_no ID
,C.Name Customer_Name, isnull(C.Code,'-') Customer_Code
,C.Address Delivery_Address, '-' Vehicle_No
,A.memo_date Invoice_Date_Time
,A.memo_date Delivery_Date_Time 
,A.memo_no  Reference_No  
,'-'Comments
,'New' Sale_Type
,'' Previous_Invoice_No
,'N' Is_Print
,'0' Tender_Id
,'Y' Post
,'NA' LC_Number
,'BDT' Currency_Code
,D.product_code Item_Code
,D.Name Item_Name
,isnull(uom.name,'PACK') UOM
,B.sales_qty Quantity
------,B.price NBR_Price
,(B.price*100)/(100+isnull(E.vat,0)) NBR_Price
,isnull(E.vat,0) VAT_Rate
,0 SD_Rate
,(B.sales_qty*B.price) TotalValue
,((B.sales_qty*B.price)*100)/(100+isnull(E.vat,0)) SubTotal
,(B.sales_qty*B.price)-((B.sales_qty*B.price)*100)/(100+isnull(E.vat,0)) VAT_Amount
,'N' Non_Stock
,0 Trading_MarkUp
,case 
when isnull(E.vat,0) = 15 then 'VAT'
when isnull(E.vat,0) = 0 then 'NonVAT'
when isnull(E.vat,0) > 0 and isnull(E.vat,0) < 15 then 'OtherRate'
end as Type
,0 Discount_Amount
,0 Promotional_Quantity
,'VAT 4.3' VAT_Name


FROM memos A
LEFT OUTER JOIN memo_details B ON A.id=B.memo_id 
LEFT OUTER JOIN outlets C ON A.outlet_id=C.id 
LEFT OUTER JOIN Products D ON B.product_id=D.id
LEFT OUTER JOIN product_prices E on D.id = E.product_id
LEFT OUTER JOIN measurement_units uom on uom.id = B.measurement_unit_id


WHERE 1=1 AND B.is_bonus=1
";
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    sqlText = sqlText + @" AND A.memo_no = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    sqlText = sqlText + @" AND A.memo_date >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    sqlText = sqlText + @" AND A.memo_date < dateadd(d,1,@toDate)";
                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);
                }

                if (!string.IsNullOrWhiteSpace(SalesFromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", SalesFromDate);
                }

                if (!string.IsNullOrWhiteSpace(SalesToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", SalesToDate);
                }


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        public DataTable GetTransfer_DBData_SMC_Consumer_Backup(string invoiceNo, string SalesFromDate, string SalesToDate, DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


            try
            {
                #region open connection and transaction


                currConn = _dbsqlConnection.GetDepoConnection(conInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                sqlText = @"  
 SELECT TOP 1000
  c.challan_no Id,
  c.challan_date TransactionDateTime,
  c.challan_no ReferenceNo,
  c.Remarks Comments,
  cd.Remarks CommentsD,
  'Y' Post,
  'Finish' TransactionType,
  pr.product_code Product_Code,
  pr.name Product_Name,
  (cd.challan_qty * case when pm.qty_in_base is null then 1 else pm.qty_in_base end) Quantity,
  '0' CostPrice,
  'PCS' UOM,
  ISNULL(p.vat, 0) VAT_Rate,
  sendStore.name SenderBranch,
  sendStore.id BranchCode,
  recStore.name ReceiverName,
  recStore.id TransferToBranchCode,
  0 SD

FROM challan_details cd
LEFT OUTER JOIN challans c
  ON cd.challan_id = c.id
LEFT OUTER JOIN product_prices p
  ON cd.product_id = p.id
LEFT OUTER JOIN products pr
  ON cd.product_id = pr.id
LEFT OUTER JOIN product_measurements pm
  ON cd.product_id = pm.product_id
  AND cd.measurement_unit_id = pm.measurement_unit_id
LEFT OUTER JOIN stores sendStore
  ON c.sender_store_id = sendStore.id
LEFT OUTER JOIN stores recStore
  ON c.receiver_store_id = recStore.id

WHERE 1=1 AND c.challan_no=@SalesInvoiceNo

"; //07/0007401

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@SalesInvoiceNo", invoiceNo);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit




                return table;


            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                throw ex;
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

        #endregion

        #region Purchase

        public DataTable GetPurchaseDataMaster_SMC(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchasemaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {
                if (paramVM.DataSourceType.ToLower() == "Trading".ToLower())
                {
                    dtPurchasemaster = GetPurchaseDataMaster_SMC_Trading(paramVM, connVM);
                }
                else
                {
                    dtPurchasemaster = GetSource_PurchaseData_Master_SMC(paramVM, connVM);
                }

                if (dtPurchasemaster != null && dtPurchasemaster.Rows.Count > 0)
                {
                    dtPurchasemaster = PurchaseDataMasterDuplicateDelete(dtPurchasemaster, paramVM);
                }

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseDataMaster_SMC", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dtPurchasemaster;

        }

        public DataTable PurchaseDataMasterDuplicateDelete(DataTable dtTable, IntegrationParam paramVM, SqlConnection vConnection = null, SqlTransaction vtransaction = null)
        {
            CommonDAL commonDal = new CommonDAL();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                dtTable.Columns.Remove("Selected");

                commonDal.ConnectionTransactionOpen(ref vConnection, ref vtransaction, ref currConn, ref transaction);

                string createTable = @"
create table #TempPurchase 
(

  ID varchar(200)
, VendorCode varchar(200)
, VendorName varchar(200)
, InvoiceDateTime datetime
, ReferenceNo varchar(200)
, BE_Number varchar(200)
, Receive_Date datetime
, TotalQuantity decimal(18,6)
, TotalValue decimal(18,6)
, BranchCode varchar(200)
, BranchName varchar(200)
)
";

                string deleteDuplicate = @"

delete from #TempPurchase where ID in (
select #TempPurchase.ID from 
#TempPurchase join PurchaseInvoiceHeaders on #TempPurchase.ID = PurchaseInvoiceHeaders.ImportIDExcel
)

";

                SqlCommand cmd = new SqlCommand(createTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("#TempPurchase", dtTable, currConn, transaction);

                cmd = new SqlCommand(deleteDuplicate, currConn, transaction);
                cmd.ExecuteNonQuery();

                string getData = @"
select 
  0	Selected
, ID
, VendorCode
, VendorName
, InvoiceDateTime
, ReferenceNo
, BE_Number
, Receive_Date
, TotalQuantity
, TotalValue
, BranchCode
, BranchName
from #TempPurchase

drop table #TempPurchase
";


                cmd.CommandText = getData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtResult = new DataTable();
                adapter.Fill(dtResult);

                commonDal.TransactionCommit(ref vtransaction, ref transaction);

                #region Selected Data

                if (dtResult != null && dtResult.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtResult.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtResult.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtResult = new DataTable();
                        dtResult = rows.CopyToDataTable();

                    }


                }

                #endregion

                return dtResult;

            }
            catch (Exception e)
            {
                commonDal.TransactionRollBack(ref vtransaction, ref transaction);
                throw e;

            }
            finally
            {
                commonDal.CloseConnection(ref vConnection, ref currConn);

            }
        }

        public DataTable GetPurchaseDataMaster_SMC_TradingX(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchasemaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(paramVM.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                CommonDAL commonDAL = new CommonDAL();

                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText

                sqlText = @"
SELECT DISTINCT
0							                                                        Selected
   , qsr.ReceiptNo AS ID
   , qsr.SupplierID AS VendorCode
   , VendorName=(SELECT TOP 1 [NAME] FROM BusinessPartner bp WHERE bp.BPID=qsr.SupplierID)
   , qsr.ReceiveDate AS InvoiceDateTime
   , qsr.ReceiptNo AS ReferenceNo
   , qsr.BillOfEntryNo AS BE_Number
   , qsr.ReceiveDate AS Receive_Date
   , SUM(qsri.DeclaredQty) AS TotalQuantity
   , SUM(qsri.DeclaredQty * qsri.PORate) TotalValue
   , qsr.StoreID AS BranchCode
   , BranchName=(select top 1 name from stores where storeid=qsr.StoreID)


FROM QuarantineStockReceiptItem AS qsri 
INNER JOIN dbo.QuarantineStockReceipt AS qsr ON qsr.QSTRID = qsri.QSTRID
INNER JOIN Stores AS st ON qsr.StoreID = st.StoreID
INNER JOIN SKUs AS sk ON qsri.SKUID = sk.SKUID
LEFT JOIN MaterialReceiptReport AS mrr ON qsr.QSTRID=mrr.ReferenceID AND mrr.ReferenceType IN (4)
LEFT JOIN POItem AS poi ON qsr.POID = poi.POID AND poi.SKUID=qsri.SKUID 
LEFT JOIN dbo.[Transaction] AS tr ON tr.ReferenceID= qsri.QSTRID AND tr.TransactionReferenceTypeID IN (4) AND tr.TransactionTypeID IN (1)
LEFT JOIN TransactionItem AS tri ON tr.TransactionID=tri.ParentID AND tri.ReferenceItemID = qsri.ItemID AND tri.SKUID=qsri.SKUID 
WHERE 1=1
 and sk.InventoryTypeID IN (43)
 and qsr.SupplierID not in ('100','1395')
----------------------------------------!Care on GROUP BY---------------------------------------- 

";
                #endregion

                #region Filtering

                if (paramVM.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and qsr.PurchaseMode = 0 ";
                }
                else if (paramVM.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and qsr.PurchaseMode = 1 ";
                }

                ////if (paramVM.TransactionType == "Other")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 1 ";
                ////}
                ////else if (paramVM.TransactionType == "Import")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 2 ";
                ////}

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    if (paramVM.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and qsr.ReceiptNo = @RefNo";

                    }
                    if (paramVM.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and qsr.BillOfEntryNo = @RefNo";

                    }

                }
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    sqlText += " and qsr.ReceiptNo in (";

                    foreach (string paramD in paramVM.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') <= @toDate";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
 qsr.ReceiptNo
,qsr.SupplierID
--,VendorName
--,qsr.BillOfEntryDate
--,tr.TransactionNo
,qsr.ReceiveDate
,qsr.StoreID
,qsr.BillOfEntryNo
";

                #endregion

                //sqlText += " ORDER BY Invoice_Date_Time";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", paramVM.RefNo);

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(paramVM.ToDate).ToString("yyyy-MM-dd"));
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchasemaster);

                #endregion

                #region Selected Data

                if (dtPurchasemaster != null && dtPurchasemaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtPurchasemaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtPurchasemaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtPurchasemaster = new DataTable();
                        dtPurchasemaster = rows.CopyToDataTable();

                    }

                }

                #endregion

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseDataMaster_SMC_Trading", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dtPurchasemaster;

        }

        public DataTable GetPurchaseDataMaster_SMC_Trading(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchasemaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                CommonDAL commonDAL = new CommonDAL();

                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText

                sqlText = @"
SELECT DISTINCT
   0	Selected
   , ID
   , Vendor_Code AS VendorCode
   , Vendor_Name AS VendorName
   , Invoice_Date AS InvoiceDateTime
   , Referance_No AS ReferenceNo
   , BE_Number
   , Receive_Date
   , SUM(Quantity) AS TotalQuantity
   , SUM(Total_price) TotalValue
   , BranchCode
   , BranchName


FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_PurchaseTrading
where 1=1
----------------------------------------!Care on GROUP BY---------------------------------------- 

";
                #endregion

                #region Filtering

                if (paramVM.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and PurchaseMode = 0 ";
                }
                else if (paramVM.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and PurchaseMode = 1 ";
                }

                ////if (paramVM.TransactionType == "Other")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 1 ";
                ////}
                ////else if (paramVM.TransactionType == "Import")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 2 ";
                ////}

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    if (paramVM.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and ID = @RefNo";

                    }
                    if (paramVM.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and BE_Number = @RefNo";

                    }

                }
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    sqlText += " and ID in (";

                    foreach (string paramD in paramVM.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') <= @toDate";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 

GROUP BY 
   ID
   , Vendor_Code
   , Vendor_Name
   , Invoice_Date
   , Referance_No
   , BE_Number
   , Receive_Date
   , BranchCode
   , BranchName

";

                #endregion

                //sqlText += " ORDER BY Invoice_Date_Time";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", paramVM.RefNo);

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(paramVM.ToDate).ToString("yyyy-MM-dd"));
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchasemaster);

                #endregion

                #region Selected Data

                if (dtPurchasemaster != null && dtPurchasemaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtPurchasemaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtPurchasemaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtPurchasemaster = new DataTable();
                        dtPurchasemaster = rows.CopyToDataTable();

                    }

                }

                #endregion

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseDataMaster_SMC_Trading", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dtPurchasemaster;

        }

        public DataTable GetSource_PurchaseData_Master_SMCX(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchasemaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(paramVM.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                CommonDAL commonDAL = new CommonDAL();

                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText

                sqlText = @"
SELECT
0							                                                        Selected
   , qsr.ReceiptNo AS ID
   , qsr.SupplierID AS VendorCode
   , VendorName=(SELECT TOP 1 [NAME] FROM BusinessPartner bp WHERE bp.BPID=qsr.SupplierID)
   --, qsr.BillOfEntryDate AS InvoiceDateTime
   , qsr.ReceiveDate AS InvoiceDateTime
   , qsr.ReceiptNo AS ReferenceNo
   , qsr.BillOfEntryNo AS BE_Number
--   , tr.TransactionNo AS ReferenceNo
   , qsr.ReceiveDate AS Receive_Date
   , SUM(qsri.DeclaredQty) AS TotalQuantity
   , SUM(qsri.DeclaredQty * qsri.PORate) TotalValue
   , qsr.StoreID AS BranchCode
   , BranchName=(select top 1 name from stores where storeid=qsr.StoreID)

FROM QuarantineStockReceiptItem AS qsri 
INNER JOIN dbo.QuarantineStockReceipt AS qsr ON qsr.QSTRID = qsri.QSTRID
INNER JOIN dbo.[Transaction] AS tr ON tr.ReferenceID= qsri.QSTRID AND tr.TransactionReferenceTypeID IN (4) AND tr.TransactionTypeID IN (1)
INNER JOIN TransactionItem AS tri ON tr.TransactionID=tri.ParentID AND tri.ReferenceItemID = qsri.ItemID AND tri.SKUID=qsri.SKUID 
INNER JOIN Stores AS st ON qsr.StoreID = st.StoreID
INNER JOIN SKUs AS sk ON qsri.SKUID = sk.SKUID
LEFT JOIN MaterialReceiptReport AS mrr ON qsr.QSTRID=mrr.ReferenceID AND mrr.ReferenceType IN (4)
LEFT JOIN POItem AS poi ON qsr.POID = poi.POID AND poi.SKUID=qsri.SKUID 

WHERE 1=1
and tr.TransactionTypeID = 1
AND sk.InventoryTypeID IN (38, 43, 49)
AND qsr.SupplierID not in ('1395','100')
----------------------------------------!Care on GROUP BY---------------------------------------- 

";
                #endregion

                #region Filtering

                if (paramVM.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and qsr.PurchaseMode = 0 ";
                }
                else if (paramVM.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and qsr.PurchaseMode = 1 ";
                }

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    if (paramVM.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and qsr.ReceiptNo = @RefNo";

                    }
                    if (paramVM.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and qsr.BillOfEntryNo = @RefNo";

                    }

                }
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    sqlText += " and qsr.ReceiptNo in (";

                    foreach (string paramD in paramVM.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') <= @toDate";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
 qsr.ReceiptNo
,qsr.SupplierID
--,VendorName
--,qsr.BillOfEntryDate
--,tr.TransactionNo
,qsr.ReceiveDate
,qsr.StoreID
,qsr.BillOfEntryNo
";

                #endregion

                //sqlText += " ORDER BY Invoice_Date_Time";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", paramVM.RefNo);

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(paramVM.ToDate).ToString("yyyy-MM-dd"));
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchasemaster);

                #endregion


                #region Selected Data

                if (dtPurchasemaster != null && dtPurchasemaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtPurchasemaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtPurchasemaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtPurchasemaster = new DataTable();
                        dtPurchasemaster = rows.CopyToDataTable();

                    }


                }



                #endregion

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetSource_PurchaseData_Master_SMC", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dtPurchasemaster;

        }

        public DataTable GetSource_PurchaseData_Master_SMC(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchasemaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                CommonDAL commonDAL = new CommonDAL();

                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText

                sqlText = @"
SELECT DISTINCT
   0	Selected
   , ID
   , Vendor_Code AS VendorCode
   , Vendor_Name AS VendorName
   , Invoice_Date AS InvoiceDateTime
   , Referance_No AS ReferenceNo
   , BE_Number
   , Receive_Date
   , SUM(Quantity) AS TotalQuantity
   , SUM(Total_price) TotalValue
   , BranchCode
   , BranchName

FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_Purchase

WHERE 1=1

----------------------------------------!Care on GROUP BY---------------------------------------- 

";
                #endregion

                #region Filtering

                if (paramVM.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and PurchaseMode = 0 ";
                }
                else if (paramVM.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and PurchaseMode = 1 ";
                }

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    if (paramVM.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and ID = @RefNo";

                    }
                    if (paramVM.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and BE_Number = @RefNo";

                    }

                }
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    sqlText += " and ID in (";

                    foreach (string paramD in paramVM.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') <= @toDate";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
   ID
   , Vendor_Code
   , Vendor_Name
   , Invoice_Date
   , Referance_No
   , BE_Number
   , Receive_Date
   , BranchCode
   , BranchName
";

                #endregion

                //sqlText += " ORDER BY Invoice_Date_Time";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", paramVM.RefNo);

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(paramVM.ToDate).ToString("yyyy-MM-dd"));
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchasemaster);

                #endregion


                #region Selected Data

                if (dtPurchasemaster != null && dtPurchasemaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtPurchasemaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtPurchasemaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtPurchasemaster = new DataTable();
                        dtPurchasemaster = rows.CopyToDataTable();

                    }


                }



                #endregion

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetSource_PurchaseData_Master_SMC", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dtPurchasemaster;

        }

        public DataTable GetPurchaseDataDetails_SMC(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null, bool isSave = false)
        {
            #region Initializ
            string sqlText = "";
            DataTable dt = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {
                dt = paramVM.DataSourceType.ToLower() == "Trading".ToLower()
                    ? GetPurchaseSMC_DetailsData_Trading(paramVM, connVM)
                    : GetPurchaseSMC_DbData_Web(paramVM, connVM);

                //string columNames = "";

                //for (var index = 0; index < dt.Columns.Count; index++)
                //{
                //    DataColumn dataColumn = dt.Columns[index];
                //    columNames += dataColumn.ColumnName + ", ";
                //}

                dt = ChangeType(dt, null, null, isSave);


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseDataDetails_SMC", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dt;

        }

        public DataTable ChangeType(DataTable dtTable, SqlConnection vConnection = null, SqlTransaction vtransaction = null, bool isSave = false)
        {
            CommonDAL commonDal = new CommonDAL();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {

                dtTable.Columns["VAT_Rate"].ColumnName = "ProductVATRate";

                commonDal.ConnectionTransactionOpen(ref vConnection, ref vtransaction, ref currConn, ref transaction);

                string createTable = @"
create table #TempPurchase 
(
 ID varchar(200)
, Vendor_Code varchar(200)
, BE_Number varchar(200)
, Invoice_Date datetime
, Vendor_Name varchar(200)
, Referance_No varchar(200)
, LC_No varchar(200)
, Receive_Date datetime
, Item_Code varchar(200)
, Item_Name varchar(200)
, Quantity decimal(18,6)
, Total_Price decimal(18,6)
, VAT_Amount decimal(18,6)
, UOM varchar(200)
, Type varchar(200)
, SD_Amount decimal(18,6)
, Assessable_Value decimal(18,6)
, CD_Amount decimal(18,6)
, RD_Amount decimal(18,6)
, AT_Amount decimal(18,6)
, AITAmount decimal(18,6)
, Others_Amount decimal(18,6)
, Remarks varchar(200)
, Post varchar(200)
, With_VDS varchar(200)
, Rebate_Rate decimal(18,6)
, ProductVATRate decimal(18,6)
, ItemNo varchar(20)
, BranchCode varchar(100)
)
";


                SqlCommand cmd = new SqlCommand(createTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("#TempPurchase", dtTable, currConn, transaction);

                ////                string updateVATRate = @"
                ////update #TempPurchase set ProductVATRate = isnull(Products.VATRate,0)
                ////from products
                ////where products.ProductCode = #TempPurchase.Item_Code
                ////
                ////";
                ////                cmd.CommandText = updateVATRate;
                ////                cmd.ExecuteNonQuery();


                string updateVATRateType = @"
update #TempPurchase set Type = Products.Option1
from products
where products.ProductCode = #TempPurchase.Item_Code
and Products.Option1 is not null and Products.Option1!=''

update #TempPurchase set Type=(case when isnull(ProductVATRate,0) = 15 then 'VAT' when isnull(ProductVATRate,0) = 0 then 'Exempted' else 'OtherRate'end) where 
Type is null or Type=''
";
                cmd.CommandText = updateVATRateType;
                cmd.ExecuteNonQuery();

                string DeleteSampleData = @"
delete TempPurchaseData where SL in (
select tpd.SL from 
TempPurchaseData tpd
inner join products p on p.ProductCode = tpd.Item_Code
where isnull(p.IsSample,'N')='Y'
)
";

                cmd.CommandText = DeleteSampleData;
                cmd.ExecuteNonQuery();

                if (isSave)
                {

                    //                    string sqlText = @"
                    //
                    //update #TempPurchase set ItemNo = Products.ItemNo
                    //from products
                    //where products.ProductCode = #TempPurchase.Item_Code
                    //
                    //select distinct Item_Name+'( '+Item_Code+' )' Item_NameCode from #TempPurchase where ItemNo='0' or ItemNo='' or ItemNo is null 
                    //
                    //";
                    //                    cmd.CommandText = sqlText;
                    //                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    //                    DataTable exitItem = new DataTable();
                    //                    da.Fill(exitItem);

                    //                    if (exitItem != null && exitItem.Rows.Count > 0)
                    //                    {
                    //                        string ItemNameCode = "";
                    //                        foreach (DataRow dr in exitItem.Rows)
                    //                        {
                    //                            ItemNameCode += "," + dr["Item_NameCode"].ToString();
                    //                        }

                    //                        ItemNameCode = ItemNameCode.Trim(',');

                    //                        throw new ArgumentNullException("FindItemId", "Item '(" + ItemNameCode + ")' not in database");

                    //                    }
                }


                string getData = @"
select ID
, Vendor_Code
, BE_Number
, Invoice_Date
, Vendor_Name
, Referance_No
, LC_No, Receive_Date
, Item_Code
, Item_Name
, Quantity
--, Total_Price
--, VAT_Amount
,Total_Price-(Total_Price*ProductVATRate/(100+ProductVATRate)) Total_Price
,Total_Price*ProductVATRate/(100+ProductVATRate) VAT_Amount
, UOM
, Type
--, (case when isnull(ProductVATRate,0) = 15 then 'VAT' when isnull(ProductVATRate,0) = 0 then 'Exempted' else 'OtherRate'end) Type
, SD_Amount
, Assessable_Value
, CD_Amount
, RD_Amount
, AT_Amount
, AITAmount
, Others_Amount
, Remarks
, Post
, With_VDS
,BranchCode
, Rebate_Rate from #TempPurchase

drop table #TempPurchase
";


                cmd.CommandText = getData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtResult = new DataTable();
                adapter.Fill(dtResult);

                commonDal.TransactionCommit(ref vtransaction, ref transaction);

                return dtResult;

            }
            catch (Exception e)
            {
                commonDal.TransactionRollBack(ref vtransaction, ref transaction);
                throw e;

            }
            finally
            {
                commonDal.CloseConnection(ref vConnection, ref currConn);

            }
        }

        public DataTable GetPurchaseSMC_DetailsData_TradingX(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"

SELECT
      qsr.ReceiptNo AS ID
   , qsr.SupplierID AS Vendor_Code
   , qsr.BillOfEntryNo AS BE_Number
   , isnull(qsr.BillOfEntryDate,qsr.ReceiveDate) AS Invoice_Date
   --, 0 AS VAT_Amount
   , Vendor_Name=(SELECT TOP 1 [NAME] FROM BusinessPartner bp WHERE bp.BPID=qsr.SupplierID)
   --, tr.TransactionNo AS Referance_No
   , qsr.ReceiptNo AS Referance_No
   , qsr.LCNo AS LC_No
   , qsr.ReceiveDate AS Receive_Date
   , sk.Code AS Item_Code
   , sk.Name AS Item_Name
   , qsri.DeclaredQty AS Quantity
   , ((qsri.DeclaredQty/((100+isnull(poi.VATPercentage,0))/100)) * qsri.PORate) Total_Price
   , (((qsri.DeclaredQty/((100+isnull(poi.VATPercentage,0))/100)) * qsri.PORate) * (isnull(poi.VATPercentage,0)/100) ) AS VAT_Amount

   --, (qsri.DeclaredQty * qsri.PORate) Total_Price
   , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=qsri.UnitID)
   ,(case when isnull(poi.VATPercentage,0) = 15 then 'VAT' when isnull(poi.VATPercentage,0) = 0 then 'Exempted' else 'OtherRate'end) [Type]
   , 0 AS SD_Amount
   ,0 AS Assessable_Value
   ,0 AS CD_Amount
   ,0 AS RD_Amount
   ,0 AS AT_Amount
   ,0 AS AITAmount
   ,0 AS Others_Amount
   ,'-' AS Remarks
   ,'N' AS Post
   ,'N' AS With_VDS
   , '0' Rebate_Rate
   , isnull(poi.VATPercentage,0) AS VAT_Rate
   --, qsr.LCNo AS LC_NO
   --, qsr.LCDate AS LC_Date
   --, qsr.BillOfEntryNo AS BE_number
   --, qsr.BillOfEntryDate AS BE_Date
   --, qsr.InvoiceNo
   --, sk.SKUID AS ProductID
   --, qsri.ReceiveQuantity AS Received_Quantity   
   --, qsri.PORate AS Unit_Price
   --, qsri.UnitID AS UOMID
   --, sk.QtyPerPack
   --, qsr.PurchaseType AS Transection_Type
   --, poi.VATPercentage AS VAT_Rate
   --, sk.InventoryTypeID AS InvTypeId
   --, InvTypeName=(SELECT TOP 1 [NAME] FROM InventoryTypes ivt WHERE ivt.InvtTypeID=sk.InventoryTypeID)
   --, Post=''
   --, Comment=''
   --, TransactionType=''

FROM QuarantineStockReceiptItem AS qsri 
INNER JOIN dbo.QuarantineStockReceipt AS qsr ON qsr.QSTRID = qsri.QSTRID
INNER JOIN Stores AS st ON qsr.StoreID = st.StoreID
INNER JOIN SKUs AS sk ON qsri.SKUID = sk.SKUID
LEFT JOIN MaterialReceiptReport AS mrr ON qsr.QSTRID=mrr.ReferenceID AND mrr.ReferenceType IN (4)
LEFT JOIN POItem AS poi ON qsr.POID = poi.POID AND poi.SKUID=qsri.SKUID 
LEFT JOIN dbo.[Transaction] AS tr ON tr.ReferenceID= qsri.QSTRID AND tr.TransactionReferenceTypeID IN (4) AND tr.TransactionTypeID IN (1)
LEFT JOIN TransactionItem AS tri ON tr.TransactionID=tri.ParentID AND tri.ReferenceItemID = qsri.ItemID AND tri.SKUID=qsri.SKUID 
WHERE 
 sk.InventoryTypeID IN (43)
 and qsr.SupplierID not in ('100','1395')
 ----and qsr.SupplierID in (SELECT BPID FROM BusinessPartner bp WHERE bp.Name in ('SMC FACTORY','SMC Hygiene Factory'))

----AND qsr.ReceiveDate >= CAST('01-JUL-2020' AS DATE) 
----AND qsr.ReceiveDate  <= CAST('30-JUN-2021' AS DATE)
 "; //07/0007401

                VendorDAL vendorDal = new VendorDAL();

                #region Filtering

                if (param.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and qsr.PurchaseMode = 0 ";
                }
                else if (param.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and qsr.PurchaseMode = 1 ";
                }

                ////if (param.TransactionType == "Other")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 1 ";
                ////}
                ////else if (param.TransactionType == "Import")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 2 ";
                ////}

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    if (param.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and qsr.ReceiptNo = @RefNo";

                    }
                    if (param.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and qsr.BillOfEntryNo = @RefNo";

                    }

                }

                if (param.IDs != null && param.IDs.Count > 0)
                {
                    sqlText += " and qsr.ReceiptNo in (";

                    foreach (string paramD in param.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') <= @toDate";
                }

                ////sqlText += "  and CompanyCode = @CompanyCode";

                #endregion

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Peram

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", param.RefNo);

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                }

                string code = commonDal.settingValue("CompanyCode", "Code");

                ////cmd.Parameters.AddWithValue("@CompanyCode", code);
                #endregion

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                table.Columns.Remove("VAT_Rate");
                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseSMC_DetailsData_Trading", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetPurchaseSMC_DetailsData_Trading(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"

SELECT
     ID
   , Vendor_Code
   , BE_Number
   , Invoice_Date
   , Vendor_Name
   , Referance_No
   , LC_No
   , Receive_Date
   , Item_Code
   , Item_Name
   , Quantity
   , Total_Price
   , VAT_Amount
   , UOM
   , [Type]
   , SD_Amount
   , Assessable_Value
   , CD_Amount
   , RD_Amount
   , AT_Amount
   , AITAmount
   , Others_Amount
   , Remarks
   , Post
   , With_VDS
   , Rebate_Rate
   , VAT_Rate
   , '' ItemNo
   , BranchCode

FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_PurchaseTrading
Where 1=1
 "; //07/0007401

                VendorDAL vendorDal = new VendorDAL();

                #region Filtering

                if (param.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and PurchaseMode = 0 ";
                }
                else if (param.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and PurchaseMode = 1 ";
                }

                ////if (param.TransactionType == "Other")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 1 ";
                ////}
                ////else if (param.TransactionType == "Import")
                ////{
                ////    sqlText += " and qsr.PurchaseType = 2 ";
                ////}

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    if (param.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and ID = @RefNo";

                    }
                    if (param.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and BE_Number = @RefNo";

                    }

                }

                if (param.IDs != null && param.IDs.Count > 0)
                {
                    sqlText += " and ID in (";

                    foreach (string paramD in param.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') <= @toDate";
                }

                ////sqlText += "  and CompanyCode = @CompanyCode";

                #endregion

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Peram

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", param.RefNo);

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                }

                string code = commonDal.settingValue("CompanyCode", "Code");

                ////cmd.Parameters.AddWithValue("@CompanyCode", code);
                #endregion

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                ////table.Columns.Remove("VAT_Rate");
                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseSMC_DetailsData_Trading", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetPurchaseSMC_DbData_WebX(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"

SELECT 
      qsr.ReceiptNo AS ID
   , qsr.SupplierID AS Vendor_Code
   , qsr.BillOfEntryNo AS BE_Number
   , isnull(qsr.BillOfEntryDate,qsr.ReceiveDate) AS Invoice_Date
   --, 0 AS VAT_Amount
   , Vendor_Name=(SELECT TOP 1 [NAME] FROM BusinessPartner bp WHERE bp.BPID=qsr.SupplierID)
   --, tr.TransactionNo AS Referance_No
   , qsr.ReceiptNo AS Referance_No
   , qsr.LCNo AS LC_No
   , qsr.ReceiveDate AS Receive_Date
   , sk.Code AS Item_Code
   , sk.Name AS Item_Name
   , qsri.DeclaredQty AS Quantity
   , ((qsri.DeclaredQty/((100+isnull(poi.VATPercentage,0))/100)) * qsri.PORate) Total_Price
   , (((qsri.DeclaredQty/((100+isnull(poi.VATPercentage,0))/100)) * qsri.PORate) * (isnull(poi.VATPercentage,0)/100) ) AS VAT_Amount

   --, (qsri.DeclaredQty * qsri.PORate) Total_Price
   , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=qsri.UnitID)
   ,(case when isnull(poi.VATPercentage,0) = 15 then 'VAT' when isnull(poi.VATPercentage,0) = 0 then 'Exempted' else 'OtherRate'end) [Type]
   , 0 AS SD_Amount
   ,0 AS Assessable_Value
   ,0 AS CD_Amount
   ,0 AS RD_Amount
   ,0 AS AT_Amount
   ,0 AS AITAmount
   ,0 AS Others_Amount
   ,'-' AS Remarks
   ,'N' AS Post
   ,'N' AS With_VDS
   , '0' Rebate_Rate
   , isnull(poi.VATPercentage,0) AS VAT_Rate

   --, qsr.BillOfEntryDate AS BE_Date
   
   
   --, qsri.ReceiveQuantity AS Received_Quantity
   --, qsri.PORate AS Unit_Price
   

   ----, st.Name AS BranchName
   ----, qsr.LCDate AS LC_Date

   ----, qsr.InvoiceNo
   ----, sk.SKUID AS ProductID
   ----, qsri.DeclaredQty AS Challan_Quantity
   ----, qsri.UnitID AS UOMID
   ----, sk.QtyPerPack
   ----, qsr.PurchaseType AS Transection_Type
   ----, sk.InventoryTypeID AS InvTypeId
   ----, InvTypeName=(SELECT TOP 1 [NAME] FROM InventoryTypes ivt WHERE ivt.InvtTypeID=sk.InventoryTypeID)
FROM QuarantineStockReceiptItem AS qsri 
INNER JOIN dbo.QuarantineStockReceipt AS qsr ON qsr.QSTRID = qsri.QSTRID
INNER JOIN dbo.[Transaction] AS tr ON tr.ReferenceID= qsri.QSTRID AND tr.TransactionReferenceTypeID IN (4) AND tr.TransactionTypeID IN (1)
INNER JOIN TransactionItem AS tri ON tr.TransactionID=tri.ParentID AND tri.ReferenceItemID = qsri.ItemID AND tri.SKUID=qsri.SKUID 
INNER JOIN Stores AS st ON qsr.StoreID = st.StoreID
INNER JOIN SKUs AS sk ON qsri.SKUID = sk.SKUID
LEFT JOIN MaterialReceiptReport AS mrr ON qsr.QSTRID=mrr.ReferenceID AND mrr.ReferenceType IN (4)
LEFT JOIN POItem AS poi ON qsr.POID = poi.POID AND poi.SKUID=qsri.SKUID 

WHERE 1=1 and tr.TransactionTypeID = 1
AND sk.InventoryTypeID IN (38, 43, 49)
AND qsr.SupplierID not in ('1395','100')
 "; //07/0007401

                VendorDAL vendorDal = new VendorDAL();

                if (param.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and qsr.PurchaseMode = 0 ";
                }
                else if (param.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and qsr.PurchaseMode = 1 ";
                }

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    if (param.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and qsr.ReceiptNo = @RefNo";

                    }
                    if (param.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and qsr.BillOfEntryNo = @RefNo";

                    }

                }

                if (param.IDs != null && param.IDs.Count > 0)
                {
                    sqlText += " and qsr.ReceiptNo in (";

                    foreach (string paramD in param.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    sqlText += " and Format(cast(qsr.ReceiveDate as DATE),'yyyy-MM-dd') <= @toDate";
                }

                ////sqlText += "  and CompanyCode = @CompanyCode";

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Peram

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", param.RefNo);

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                }

                string code = commonDal.settingValue("CompanyCode", "Code");

                ////cmd.Parameters.AddWithValue("@CompanyCode", code);
                #endregion

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region Comments

                ////ProductDAL dal = new ProductDAL();

                ////foreach (DataRow tableRow in table.Rows)
                ////{
                ////    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                ////    ProductVM vm = vms.FirstOrDefault();

                ////    if (vm == null)
                ////    {
                ////        continue;
                ////    }
                ////    decimal vatRate = Convert.ToDecimal(tableRow["VAT_Rate"]);
                ////    tableRow["UOM"] = vm.UOM;
                ////    decimal sdAmount = (Convert.ToDecimal(tableRow["Total_Price"]) * vm.SD) / 100;

                ////    tableRow["SD_Amount"] = sdAmount;

                ////    tableRow["VAT_Amount"] = (sdAmount + Convert.ToDecimal(tableRow["Total_Price"])) * vatRate / 100;

                ////}

                #endregion

                table.Columns.Remove("VAT_Rate");
                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseSMC_DbData_Web", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetPurchaseSMC_DbData_Web(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"


SELECT
     ID
   , Vendor_Code
   , BE_Number
   , Invoice_Date
   , Vendor_Name
   , Referance_No
   , LC_No
   , Receive_Date
   , Item_Code
   , Item_Name
   , Quantity
   , Total_Price
   , VAT_Amount
   , UOM
   , [Type]
   , SD_Amount
   , Assessable_Value
   , CD_Amount
   , RD_Amount
   , AT_Amount
   , AITAmount
   , Others_Amount
   , Remarks
   , Post
   , With_VDS
   , Rebate_Rate
   , VAT_Rate
   , '' ItemNo
   , BranchCode

FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_Purchase
Where 1=1
 "; //07/0007401

                VendorDAL vendorDal = new VendorDAL();

                if (param.TransactionType == "Other")
                {
                    ////sqlText += " and qsr.PurchaseType = 1 ";
                    sqlText += " and PurchaseMode = 0 ";
                }
                else if (param.TransactionType == "Import")
                {
                    ////sqlText += " and qsr.PurchaseType = 2 ";
                    sqlText += " and PurchaseMode = 1 ";
                }

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    if (param.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and ID = @RefNo";

                    }
                    if (param.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and BE_Number = @RefNo";

                    }

                }

                if (param.IDs != null && param.IDs.Count > 0)
                {
                    sqlText += " and ID in (";

                    foreach (string paramD in param.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') <= @toDate";
                }

                ////sqlText += "  and CompanyCode = @CompanyCode";

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseSMC_DbData_Web", "sqlText : " + sqlText);


                #region Peram

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", param.RefNo);

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                }

                string code = commonDal.settingValue("CompanyCode", "Code");

                ////cmd.Parameters.AddWithValue("@CompanyCode", code);
                #endregion

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region Comments

                ////ProductDAL dal = new ProductDAL();

                ////foreach (DataRow tableRow in table.Rows)
                ////{
                ////    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                ////    ProductVM vm = vms.FirstOrDefault();

                ////    if (vm == null)
                ////    {
                ////        continue;
                ////    }
                ////    decimal vatRate = Convert.ToDecimal(tableRow["VAT_Rate"]);
                ////    tableRow["UOM"] = vm.UOM;
                ////    decimal sdAmount = (Convert.ToDecimal(tableRow["Total_Price"]) * vm.SD) / 100;

                ////    tableRow["SD_Amount"] = sdAmount;

                ////    tableRow["VAT_Amount"] = (sdAmount + Convert.ToDecimal(tableRow["Total_Price"])) * vatRate / 100;

                ////}

                #endregion

                //////table.Columns.Remove("VAT_Rate");
                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("SMCIntegrationDAL", "GetPurchaseSMC_DbData_Web", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public ResultVM SaveSMCPurchase_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();

            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();
                CommonImportDAL cImport = new CommonImportDAL();

                DataTable dtPurchase = new DataTable();
                BranchProfileDAL branchProfile = new BranchProfileDAL();
                DataTable dtBranchInfo = branchProfile.SelectAll(null, new[] { "BranchCode" }, new[] { paramVM.BranchCode });
                paramVM.dtConnectionInfo = dtBranchInfo;

                ////dtPurchase = GetPurchaseSMC_DbData_Web(paramVM);
                dtPurchase = GetPurchaseDataDetails_SMC(paramVM, connVM, true);

                if (dtPurchase == null || dtPurchase.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                DataTable BranchDt = GetNewBranch(dtPurchase.Copy(), null, null, connVM);

                PurchaseDAL purchaseDal = new PurchaseDAL();

                ////if (!string.IsNullOrEmpty(paramVM.BranchCode))
                ////{
                ////    dtPurchase.Columns.Add(new DataColumn() { DefaultValue = paramVM.BranchCode, ColumnName = "BranchCode" });
                ////}

                sqlResults = purchaseDal.SaveTempPurchase(dtPurchase, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, 0, () => { });

                if (sqlResults[0].ToLower() == "success")
                {
                    rVM.Message = "Saved Successfully";
                }
                else
                {
                    rVM.Message = sqlResults[1];
                }

                rVM.Status = sqlResults[0];

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ImportDAL", "SaveACISale_Web", ex.ToString());

                rVM.Message = ex.Message;
            }
            #endregion

            #region finally

            finally
            {


            }
            #endregion

            return rVM;
        }

        #endregion

        #region Issue

        public DataTable GetIssueSMCDbDataWebX(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            string IDs = "";

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

--DECLARE @FROMDATE DATETIME =CAST('01-JUN-2021' AS DATE) ;
--DECLARE @ToDATE DATETIME =CAST('30-JUN-2021' AS DATE) ;
--DECLARE @ids varchar(150) ='MIP-21-011720' ;


Select 

    tt.TransactionNo AS ID
,0 Selected
   ,tt.FromStoreID AS BranchCode   
   , BranchName=(select top 1 name from stores where storeid=tt.FromStoreID)
   
   ,sum(ti.IssueQuantity) AS Quantity
   ,UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=ti.UnitID)
   ,Post='N'
FROM [Transaction] tt
INNER JOIN TransactionItem ti ON ti.ParentID=tt.TransactionID
INNER JOIN SKUs sk ON ti.SKUID=sk.SKUID
INNER JOIN Stores AS st ON tt.FromStoreID = st.StoreID
Where tt.IssueCategory IN (1,2,3) 
AND sk.InventoryTypeID IN (38,49,55)

";

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += @" AND tt.TransactionNo =@ids ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.FromDate))
                    {
                        sqlText += @" AND tt.Trandate >=  @FROMDATE ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.ToDate))
                    {
                        sqlText += @" AND tt.Trandate  <= @TODATE ";
                    }

                    sqlText += @"
 group by 

tt.TransactionNo 
   ,tt.FromStoreID 
   ,ti.UnitID

";


                    sqlText += @"

UNION
-------------------- Other Issue -------------------
Select 
    sa.TransactionNo AS ID
,0 Selected
  , sa.FromStoreID AS BranchCode    
  ,  BranchName=(select top 1 name from stores where storeid=sa.FromStoreID)
    
  , sum(sai.FinallyReceivingQuantity) AS Quantity
  , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=sai.UnitID) 
  , Post='N'
FROM StockAdjustment sa
INNER JOIN StockAdjustmentItem sai ON sai.ParentID=sa.TransactionID 
INNER JOIN Stores AS st ON sa.FromStoreID = st.StoreID
INNER JOIN SKUs sk ON sai.SKUID=sk.SKUID
Where sa.TranReferenceType=3 --Other Issue
AND sk.InventoryTypeID IN (38,49,55)";

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += @" AND sa.TransactionNo =@ids ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.FromDate))
                    {
                        sqlText += @" AND sa.Transactiondate >=  @FROMDATE ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.ToDate))
                    {
                        sqlText += @" AND sa.Transactiondate  <= @TODATE ";
                    }

                    sqlText += @"
group by
sa.TransactionNo
  , sa.FromStoreID
  ,sai.UnitID
";

                    sqlText += @"


--------------------Other Issue END-------------------
UNION

------'Issue to Packing'--------
Select 
   im.ChallanNo AS ID
,0 Selected
   ,im.IssuingStoreID AS BranchCode
   , BranchName=(select top 1 name from stores where storeid=im.IssuingStoreID)
    
   , sum(imi.IssueQty) AS Quantity   
   , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=imi.UnitID) 
   , Post='N'
FROM IssuingMaterialsForPacking im
INNER JOIN IssueMaterialItemsForPacking imi ON im.IMID=imi.IMID 
INNER JOIN Stores AS st ON im.IssuingStoreID = st.StoreID
INNER JOIN SKUs sk ON imi.SKUID=sk.SKUID
AND sk.InventoryTypeID IN (38,49,55)";

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += @" AND im.ChallanNo =@ids";
                    }
                    if (!string.IsNullOrWhiteSpace(param.FromDate))
                    {
                        sqlText += @" AND im.IssuingDate >=  @FROMDATE ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.ToDate))
                    {
                        sqlText += @" AND im.IssuingDate  <= @TODATE";
                    }

                    sqlText += @"

group by

im.ChallanNo
   ,im.IssuingStoreID
   ,imi.UnitID
";
                    //removed and IsProcessed = 'N' 	  ,IsProcessed JUBAYER 4152021


                    #endregion

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        cmd.Parameters.AddWithValue("@ids", param.RefNo);
                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        cmd.Parameters.AddWithValue("@FROMDATE", param.FromDate);
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        cmd.Parameters.AddWithValue("@ToDATE", param.ToDate);
                    }

                    ////cmd.Parameters.AddWithValue("@CompanyCode", code);

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit

                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetIssueSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetIssueSMCDbDataWeb(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            string IDs = "";

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetConnection(connVM);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"



Select 

    ID
   ,0 Selected
   ,BranchCode   
   ,BranchName
   ,sum(Quantity) AS Quantity
   ,'' UOM
   ,Post
FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_Issue
where 1=1

";

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += @" AND ID =@ids ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.FromDate))
                    {
                        sqlText += @" AND Issue_DateTime >=  @FROMDATE ";
                    }
                    if (!string.IsNullOrWhiteSpace(param.ToDate))
                    {
                        sqlText += @" AND Issue_DateTime  <= @TODATE ";
                    }

                    sqlText += @"
 group by 

   ID
  ,BranchCode
  ,BranchName
  ,UOM
  ,Post

";
                    #endregion

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    FileLogger.Log("SMCIntegrationDAL", "GetIssueSMCDbDataWeb", "SQL Text : " + sqlText);


                    #region Peram

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        cmd.Parameters.AddWithValue("@ids", param.RefNo);
                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        cmd.Parameters.AddWithValue("@FROMDATE", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd") + " 00:00:00");
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        cmd.Parameters.AddWithValue("@ToDATE", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd") + " 23:59:59");
                    }


                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit

                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetIssueSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetIssue_DetailSMCDbDataWebX(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            string IDs = "";

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

--DECLARE @FROMDATE DATETIME =CAST('01-JUN-2021' AS DATE) ;
--DECLARE @ToDATE DATETIME =CAST('30-JUN-2021' AS DATE) ;
--DECLARE @ids varchar(150) ='MIP-21-011720' ;


Select 
    tt.TransactionNo AS ID
   --,tt.FromStoreID AS BranchCode
   ,cast( tt.Trandate as varchar(200))Issue_DateTime
   ,sk.Code AS Item_Code
   ,sk.Name AS Item_Name
   ,ti.IssueQuantity AS Quantity
   ,UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=ti.UnitID)
   ,Post='N'
   ,tt.TransactionNo AS Reference_No
   ,Comments=tt.Remarks
FROM [Transaction] tt
INNER JOIN TransactionItem ti ON ti.ParentID=tt.TransactionID
INNER JOIN SKUs sk ON ti.SKUID=sk.SKUID
INNER JOIN Stores AS st ON tt.FromStoreID = st.StoreID
Where tt.IssueCategory IN (1,2,3) 
AND sk.InventoryTypeID IN (38,49,55)";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += @" AND tt.TransactionNo in(@ids)";
                    }

                    sqlText += @"

UNION
-------------------- Other Issue -------------------
Select 
    sa.TransactionNo AS ID
  --, sa.FromStoreID AS BranchCode 
  ,cast(sa.Transactiondate as varchar(200))Issue_DateTime
  , sk.Code AS Item_Code
  , sk.Name AS Item_Name
  , sai.FinallyReceivingQuantity AS Quantity
  , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=sai.UnitID) 
  , Post='N'
  ,sa.TransactionNo AS Reference_No
  , Comments=sa.Remarks
 
FROM StockAdjustment sa
INNER JOIN StockAdjustmentItem sai ON sai.ParentID=sa.TransactionID 
INNER JOIN Stores AS st ON sa.FromStoreID = st.StoreID
INNER JOIN SKUs sk ON sai.SKUID=sk.SKUID
Where sa.TranReferenceType=3 --Other Issue
AND sk.InventoryTypeID IN (38,49,55)";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += @" AND sa.TransactionNo in(@ids)";
                    }

                    sqlText += @"


--------------------Other Issue END-------------------
UNION

------'Issue to Packing'--------
Select 
   im.ChallanNo AS ID
  -- ,im.IssuingStoreID AS BranchCode   
   ,cast(im.IssuingDate as varchar(200))Issue_DateTime
   ,sk.Code AS Item_Code 
   ,sk.Name AS Item_Name
   ,imi.IssueQty AS Quantity   
   ,UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=imi.UnitID) 
   ,Post='N'
   ,im.ChallanNo AS Reference_No
   ,Comments=''
FROM IssuingMaterialsForPacking im
INNER JOIN IssueMaterialItemsForPacking imi ON im.IMID=imi.IMID 
INNER JOIN Stores AS st ON im.IssuingStoreID = st.StoreID
INNER JOIN SKUs sk ON imi.SKUID=sk.SKUID
AND sk.InventoryTypeID IN (38,49,55)";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += @" AND im.ChallanNo in(@ids)";
                    }

                    //removed and IsProcessed = 'N' 	  ,IsProcessed JUBAYER 4152021

                    IDs = "";
                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        foreach (string paramD in param.IDs)
                        {
                            IDs += "'" + paramD + "',";
                        }

                        IDs = IDs.TrimEnd(',');

                    }

                    #endregion

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText = sqlText.Replace("@ids", IDs);
                    }

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    //cmd.Parameters.AddWithValue("@ids", IDs);

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit
                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                ProductDAL dal = new ProductDAL();

                foreach (DataRow tableRow in table.Rows)
                {
                    var item_Code = tableRow["Item_Code"].ToString();
                    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                    var vm = vms.FirstOrDefault();

                    if (vm == null)
                    {
                        continue;
                        //throw new Exception("Product Code not found in system " +item_Code);
                    }

                    tableRow["UOM"] = vm.UOM;

                }
                //table.AcceptChanges();

                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetIssue_DetailSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetIssue_DetailSMCDbDataWeb(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            string IDs = "";

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetConnection(connVM);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"



Select 
     ID
   , cast( Issue_DateTime as varchar(200))Issue_DateTime
   , Item_Code
   , Item_Name
   , Quantity
   , UOM
   , Post
   , Reference_No
   , Comments
   ,BranchCode
FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_Issue 
where 1=1
and Issue_DateTime>='2022-05-01 00:00:00.000' 

";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += @" and ID in(@ids)";
                    }

                    IDs = "";
                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        foreach (string paramD in param.IDs)
                        {
                            IDs += "'" + paramD + "',";
                        }

                        IDs = IDs.TrimEnd(',');

                    }

                    #endregion

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText = sqlText.Replace("@ids", IDs);
                    }

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram


                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit
                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                ProductDAL dal = new ProductDAL();

                //foreach (DataRow tableRow in table.Rows)
                //{
                //    var item_Code = tableRow["Item_Code"].ToString();
                //    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                //    var vm = vms.FirstOrDefault();

                //    if (vm == null)
                //    {
                //        continue;
                //        //throw new Exception("Product Code not found in system " +item_Code);
                //    }

                //    tableRow["UOM"] = vm.UOM;

                //}
                //table.AcceptChanges();

                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetIssue_DetailSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public ResultVM SaveSMCIssue_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();

            #region try

            try
            {

                DataTable dtIssue = new DataTable();
                BranchProfileDAL branchProfile = new BranchProfileDAL();
                DataTable dtBranchInfo = branchProfile.SelectAll(null, new[] { "BranchCode" }, new[] { paramVM.BranchCode });
                paramVM.dtConnectionInfo = dtBranchInfo;

                dtIssue = GetIssue_DetailSMCDbDataWeb(paramVM);

                if (dtIssue == null || dtIssue.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                ////if (!string.IsNullOrEmpty(paramVM.BranchCode))
                ////{
                ////    dtIssue.Columns.Add(new DataColumn() { DefaultValue = paramVM.BranchCode, ColumnName = "BranchCode" });
                ////}

                DataTable BranchDt = GetNewBranch(dtIssue.Copy(), null, null, connVM);

                IntegrationParam param = new IntegrationParam
                {
                    Data = dtIssue,
                    callBack = () => { },
                    SetSteps = (s) => { },
                    DefaultBranchId = paramVM.DefaultBranchId,
                    TransactionType = paramVM.TransactionType,
                    CurrentUser = paramVM.CurrentUser,
                };
                IssueDAL IssueDal = new IssueDAL();

                sqlResults = IssueDal.SaveIssue_Split(param);

                rVM.Status = "success";
                rVM.Message = "Saved Successfully";

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "SaveSMCIssue_Web", ex.ToString());

                rVM.Message = ex.Message;
            }
            #endregion

            #region finally

            finally
            {


            }
            #endregion

            return rVM;
        }

        #endregion

        #region Receive

        #region Pharma Receive

        public DataTable GetReceive_DetailSMCDbDataWeb_Pharma(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetDepoConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

----Query
SELECT DISTINCT
req.IssueChalanNo   AS ID
, '018'      AS BranchCode
,Rcv.StockRcvDate AS  Receive_DateTime
, sit.ProductCode       AS Item_Code
, sit.ProductName       AS Item_Name
, Rcv.TotalQuantity   as Quantity  -- New Add(Receive Qty)
, ISNULL(NULLIF (sit.PackSize, '&nbsp;'), 'PACK')       AS UOM
, req.IssueChalanNo     AS Reference_No
, Post='Y'
, Comments='-'
, '' Return_Id
, 'N' With_Toll
, sit.PriceAmount / sit.Quantity      AS NBR_Price
, 'VAT 4.3' VAT_Name
, 'N/A' CustomerCode
----, ISNULL(com.ComUnitCode, '-')       AS CustomerCode
------, '001'      AS Branch_Code
----, com.ComUnitName      AS Customer_Name
----, ISNULL(com.ComUnitCode, '-')       AS Customer_Code
----, com.Address      AS Delivery_Address
----, '-'      AS Vehicle_No
----, '-'       AS VehicleType
----, CAST(req.IssuChalanDate AS varchar(20))       AS Invoice_Date_Time
----, CAST(req.IssuChalanDate AS varchar(20))       AS Delivery_Date_Time
----, '-'       AS Comments
----, 'New'     AS Sale_Type
----, ''      AS Previous_Invoice_No
----, 'N'       AS Is_Print
----, '0'       AS Tender_Id
----, 'Y'       AS Post
----, 'NA'        AS LC_Number
----, 'BDT'     AS Currency_Code

----, ISNULL(NULLIF (sit.PackSize, '&nbsp;'), 'PACK')       AS UOM
----, sit.Quantity       AS Quantity
----, ROUND(ISNULL(sit.VATAmount, 0) / sit.PriceAmount * 100, 5)      AS VAT_Rate
----, 0     AS SD_Rate
----, sit.PriceAmount       AS SubTotal
----, sit.VATAmount     AS VAT_Amount
----, sit.TotalPriceAmount        AS TotalValue
----, 'N'       AS Non_Stock
----, 0     AS Trading_MarkUp
----, CASE WHEN (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 >= 15 THEN 'VAT' 
----	WHEN (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 = 0 THEN 'NonVAT'
----	WHEN (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 >= 0 AND (isnull(sit.VATAmount, 0) / sit.PriceAmount) * 100 < 15 THEN 'OtherRate' END  AS Type
----, 0     AS Discount_Amount
----, 0     AS Promotional_Quantity
----, 'VAT 4.3'     AS VAT_Name
----, 'Other'     AS TransactionType
----, 'SMC'        AS CompanyCode
----, 'Pharma'     AS DataSource

FROM    dbo.tblRequisition AS req
LEFT OUTER JOIN    dbo.tblCompanyUnit AS com ON req.ComUnitId = com.ComUnitId
LEFT OUTER JOIN dbo.tblStockInTransfar AS sit ON sit.ReqId = req.ReqId
inner  JOIN dbo.tblDCStore AS Rcv ON Rcv.StockInTransfarId =sit.StockInTransfarId   --New Add

WHERE  (1 = 1) AND (req.IssuChalanDate >= '2019-Nov-01')
AND ISNULL(sit.Quantity,0) > 0 AND ISNULL(sit.PriceAmount,0) > 0
----and Rcv.StockRcvDate >= '2021-Jan-01'--changed
----and Rcv.StockRcvDate <= '2021-Jan-31'--changed
 ";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += " and req.IssueChalanNo in (";

                        foreach (string paramD in param.IDs)
                        {
                            sqlText += "'" + paramD + "',";
                        }

                        sqlText = sqlText.TrimEnd(',') + ")";

                    }

                    #endregion

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit

                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit

                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                ProductDAL dal = new ProductDAL();

                foreach (DataRow tableRow in table.Rows)
                {
                    string itemCode = tableRow["Item_Code"].ToString();

                    List<ProductVM> vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { itemCode });

                    ProductVM vm = vms.FirstOrDefault();

                    if (vm == null)
                        continue;

                    tableRow["UOM"] = vm.UOM;

                }
                table.AcceptChanges();

                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetReceive_DetailSMCDbDataWeb_Pharma", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetReceiveSMCDbDataWeb_Pharma(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetDepoConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

SELECT DISTINCT
req.IssueChalanNo   AS ID
,0 Selected
, '018'      AS BranchCode
,'-' AS BranchName
,Rcv.StockRcvDate AS  Receive_DateTime
, sum(Rcv.TotalQuantity)   as Quantity  -- New Add(Receive Qty)
, req.IssueChalanNo     AS Reference_No
, Post='Y'
, Comments='-'
, '' Return_Id
, 'N' With_Toll

FROM    dbo.tblRequisition AS req
LEFT OUTER JOIN    dbo.tblCompanyUnit AS com ON req.ComUnitId = com.ComUnitId
LEFT OUTER JOIN dbo.tblStockInTransfar AS sit ON sit.ReqId = req.ReqId
inner  JOIN dbo.tblDCStore AS Rcv ON Rcv.StockInTransfarId =sit.StockInTransfarId   --New Add

WHERE  (1 = 1) AND (req.IssuChalanDate >= '2019-Nov-01')
AND ISNULL(sit.Quantity,0) > 0 AND ISNULL(sit.PriceAmount,0) > 0

--and Rcv.StockRcvDate >= '2021-Jan-01'--changed
--and Rcv.StockRcvDate <= '2021-Jan-31'--changed

------------- ReceiveM-Finish -------------------

 "; //07/0007401


                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += " and req.IssueChalanNo = @rid";

                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        sqlText += " and Rcv.StockRcvDate >= @fromDate";
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        sqlText += " and Rcv.StockRcvDate <= @toDate";
                    }

                    #endregion

                    #region Group By

                    sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
   req.IssueChalanNo
   ,Rcv.StockRcvDate



";

                    #endregion


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        cmd.Parameters.AddWithValue("@rid", param.RefNo);

                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", param.FromDate);
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        cmd.Parameters.AddWithValue("@toDate", param.ToDate);
                    }

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit
                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }



                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetReceiveSMCDbDataWeb_Pharma", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        #endregion

        public DataTable GetReceiveDetailDataWeb_SMC(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ


            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM);

            #endregion

            #region try

            try
            {

                if (param.DataSourceType.ToLower() == "Pharma".ToLower())
                {
                    table = GetReceive_DetailSMCDbDataWeb_Pharma(param, connVM);
                }
                else
                {
                    table = GetReceive_DetailSMCDbDataWeb(param, connVM);
                }

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {

                FileLogger.Log("SMCIntegrationDAL", "GetReceiveDetailDataWeb_SMC", ex.ToString());

                throw ex;
            }
            finally
            {

            }
            #endregion
        }

        public DataTable GetReceiveDbDataWeb_SMC(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ


            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM);

            #endregion

            #region try

            try
            {
                if (param.DataSourceType.ToLower() == "Pharma".ToLower())
                {
                    table = GetReceiveSMCDbDataWeb_Pharma(param, connVM);
                }
                else
                {
                    table = GetReceiveSMCDbDataWeb(param, connVM);
                }

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {

                FileLogger.Log("SMCIntegrationDAL", "GetReceiveDbDataWeb_SMC", ex.ToString());

                throw ex;
            }
            finally
            {

            }
            #endregion
        }

        public DataTable GetReceiveSMCDbDataWebX(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

SELECT DISTINCT
    qsr.ReceiptNo AS ID
   ,0 Selected
   , qsr.StoreID AS BranchCode
   , BranchName=(select top 1 name from stores where storeid=qsr.StoreID)

   , qsr.ReceiveDate AS Receive_DateTime
   , sum(qsri.DeclaredQty) AS Quantity
   , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=qsri.UnitID)
   , Post='N'
   , '' Return_Id
   , 'N' With_Toll
   , '0' NBR_Price
   , 'VAT 4.3' VAT_Name
   , 'N/A' CustomerCode
   
   
FROM QuarantineStockReceiptItem AS qsri 
INNER JOIN dbo.QuarantineStockReceipt AS qsr ON qsr.QSTRID = qsri.QSTRID
INNER JOIN Stores AS st ON qsr.StoreID = st.StoreID
INNER JOIN SKUs AS sk ON qsri.SKUID = sk.SKUID
LEFT JOIN MaterialReceiptReport AS mrr ON qsr.QSTRID=mrr.ReferenceID AND mrr.ReferenceType IN (4)
LEFT JOIN POItem AS poi ON qsr.POID = poi.POID AND poi.SKUID=qsri.SKUID 
LEFT JOIN dbo.[Transaction] AS tr ON tr.ReferenceID= qsri.QSTRID AND tr.TransactionReferenceTypeID IN (4) AND tr.TransactionTypeID IN (1)
LEFT JOIN TransactionItem AS tri ON tr.TransactionID=tri.ParentID AND tri.ReferenceItemID = qsri.ItemID AND tri.SKUID=qsri.SKUID 
WHERE 
 sk.InventoryTypeID IN (43)
and qsr.SupplierID in ('100','1395')
 --and qsr.SupplierID in (SELECT BPID FROM BusinessPartner bp WHERE bp.Name in ('SMC FACTORY','SMC Hygiene Factory'))


----AND qsr.ReceiveDate >= CAST('01-JUL-2020' AS DATE) 
----AND qsr.ReceiveDate  <= CAST('30-JUN-2021' AS DATE)

------------- ReceiveM-Finish -------------------

 "; //07/0007401


                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += " and qsr.ReceiptNo = @rid";

                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        sqlText += " and qsr.ReceiveDate >= @fromDate";
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        sqlText += " and qsr.ReceiveDate <= @toDate";
                    }

                    #endregion

                    #region Group By

                    sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
     qsr.ReceiptNo
   , qsr.StoreID
   , qsr.ReceiveDate
   , qsri.UnitID


";

                    #endregion


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        cmd.Parameters.AddWithValue("@rid", param.RefNo);

                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", param.FromDate);
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        cmd.Parameters.AddWithValue("@toDate", param.ToDate);
                    }

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit
                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }



                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetReceiveSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetReceiveSMCDbDataWeb(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetConnection(connVM);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

SELECT DISTINCT
     ID
   ,0 Selected
   , BranchCode
   , BranchName
   , Receive_DateTime
   , sum(Quantity) AS Quantity
   ,'' UOM
   , Post
   , Return_Id
   , With_Toll
   , NBR_Price
   , VAT_Name
   , CustomerCode
   
   
FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_ProductionReceive
Where 1=1

------------- ReceiveM-Finish -------------------

 "; //07/0007401


                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        sqlText += " and ID = @rid";

                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        sqlText += " and Receive_DateTime >= @fromDate";
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        sqlText += " and Receive_DateTime <= @toDate";
                    }

                    #endregion

                    #region Group By

                    sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
     ID
   , BranchCode
   , BranchName
   , Receive_DateTime
   , Post
   , Return_Id
   , With_Toll
   , NBR_Price
   , VAT_Name
   , CustomerCode


";

                    #endregion


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    if (!string.IsNullOrEmpty(param.RefNo))
                    {
                        cmd.Parameters.AddWithValue("@rid", param.RefNo);

                    }

                    if (!string.IsNullOrEmpty(param.FromDate))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                    }

                    if (!string.IsNullOrEmpty(param.ToDate))
                    {
                        cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                    }

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit
                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }



                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetReceiveSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetReceive_DetailSMCDbDataWebX(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetSMCDSS_LiveConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

------------- ReceiveM-Finish -------------------
SELECT DISTINCT
     qsr.ReceiptNo AS ID
  -- , qsr.StoreID AS BranchCode
   , qsr.ReceiveDate AS Receive_DateTime
   , sk.Code AS Item_Code
   , sk.Name AS Item_Name
   , qsri.DeclaredQty AS Quantity
   , UOM=(SELECT TOP 1 [NAME] FROM UNIT un WHERE un.UnitID=qsri.UnitID)
   , tr.TransactionNo AS Reference_No
   , Post='N'
   , Comments='-'
   , '' Return_Id
   , 'N' With_Toll
   , qsri.PORate NBR_Price
   , 'VAT 4.3' VAT_Name
   , 'N/A' CustomerCode
   
   --, st.Name AS BranchName
   --, qsr.SupplierID AS Vendor_Code
   --, Vendor_Name=(SELECT TOP 1 [NAME] FROM BusinessPartner bp WHERE bp.BPID=qsr.SupplierID)
   --, qsr.LCNo AS LC_NO
   --, qsr.LCDate AS LC_Date
   --, qsr.BillOfEntryNo AS BE_number
   --, qsr.BillOfEntryDate AS BE_Date
   --, qsr.InvoiceNo
   --, sk.SKUID AS ProductID
   --, qsri.ReceiveQuantity AS Received_Quantity   
   --, qsri.PORate AS Unit_Price
   --, qsri.UnitID AS UOMID
   --, sk.QtyPerPack
   --, qsr.PurchaseType AS Transection_Type
   --, poi.VATPercentage AS VAT_Rate
   --, sk.InventoryTypeID AS InvTypeId
   --, InvTypeName=(SELECT TOP 1 [NAME] FROM InventoryTypes ivt WHERE ivt.InvtTypeID=sk.InventoryTypeID)
   --, Post=''
   --, Comment=''
   --, TransactionType=''

FROM QuarantineStockReceiptItem AS qsri 
INNER JOIN dbo.QuarantineStockReceipt AS qsr ON qsr.QSTRID = qsri.QSTRID
INNER JOIN Stores AS st ON qsr.StoreID = st.StoreID
INNER JOIN SKUs AS sk ON qsri.SKUID = sk.SKUID
LEFT JOIN MaterialReceiptReport AS mrr ON qsr.QSTRID=mrr.ReferenceID AND mrr.ReferenceType IN (4)
LEFT JOIN POItem AS poi ON qsr.POID = poi.POID AND poi.SKUID=qsri.SKUID 
LEFT JOIN dbo.[Transaction] AS tr ON tr.ReferenceID= qsri.QSTRID AND tr.TransactionReferenceTypeID IN (4) AND tr.TransactionTypeID IN (1)
LEFT JOIN TransactionItem AS tri ON tr.TransactionID=tri.ParentID AND tri.ReferenceItemID = qsri.ItemID AND tri.SKUID=qsri.SKUID 
WHERE 
 sk.InventoryTypeID IN (43)
 and qsr.SupplierID in ('100','1395')

----AND qsr.ReceiveDate >= CAST('01-JUL-2020' AS DATE) 
----AND qsr.ReceiveDate  <= CAST('30-JUN-2021' AS DATE)


 ";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += " and qsr.ReceiptNo in (";

                        foreach (string paramD in param.IDs)
                        {
                            sqlText += "'" + paramD + "',";
                        }

                        sqlText = sqlText.TrimEnd(',') + ")";

                    }

                    #endregion

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit

                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                ProductDAL dal = new ProductDAL();

                //table.Columns["Receive_DateTime"].ReadOnly = false;

                foreach (DataRow tableRow in table.Rows)
                {
                    string itemCode = tableRow["Item_Code"].ToString();

                    List<ProductVM> vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { itemCode });

                    ProductVM vm = vms.FirstOrDefault();

                    if (vm == null)
                        continue;

                    tableRow["UOM"] = vm.UOM;

                    DataTable dt = dal.GetBOMReferenceNo(vm.ItemNo, "VAT 4.3",
                        Convert.ToDateTime(tableRow["Receive_DateTime"]).ToString("yyyy-MMM-dd"), null, null, "", null);

                    ////////////////tableRow["Receive_DateTime"] =
                    ////////////////Convert.ToDateTime(tableRow["Receive_DateTime"]).ToString("yyyy-MMM-dd");

                    //tableRow["NBR_Price"] = string.IsNullOrEmpty(dt.Rows[0]["NBRPrice"].ToString())
                    //    ? "0"
                    //    : dt.Rows[0]["NBRPrice"].ToString();


                }
                table.AcceptChanges();

                return table;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetReceive_DetailSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetReceive_DetailSMCDbDataWeb(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            DataTable table = new DataTable();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

            #endregion

            #region try

            try
            {
                try
                {
                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetConnection(connVM);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"

------------- ReceiveM-Finish -------------------
SELECT DISTINCT
      ID
   ,  Receive_DateTime
   ,  Item_Code
   ,  Item_Name
   ,  Quantity
   ,  UOM
   ,  Reference_No
   ,  Post
   ,  Comments
   ,  Return_Id
   ,  With_Toll
   ,  NBR_Price
   ,  VAT_Name
   ,  CustomerCode
   ,  BranchCode

FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_ProductionReceive
where 1=1

 ";

                    if (param.IDs != null && param.IDs.Count > 0)
                    {
                        sqlText += " and ID in (";

                        foreach (string paramD in param.IDs)
                        {
                            sqlText += "'" + paramD + "',";
                        }

                        sqlText = sqlText.TrimEnd(',') + ")";

                    }

                    #endregion

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Peram

                    #endregion

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit

                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                }

                ProductDAL dal = new ProductDAL();

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("SMCIntegrationDAL", "GetReceive_DetailSMCDbDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public ResultVM SaveSMCReceive_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();

            #region try

            try
            {

                DataTable dtReceive = new DataTable();
                BranchProfileDAL branchProfile = new BranchProfileDAL();
                DataTable dtBranchInfo = branchProfile.SelectAll(null, new[] { "BranchCode" }, new[] { paramVM.BranchCode });
                paramVM.dtConnectionInfo = dtBranchInfo;

                //////dtReceive = GetReceive_DetailSMCDbDataWeb(paramVM);

                dtReceive = GetReceiveDetailDataWeb_SMC(paramVM);

                if (dtReceive == null || dtReceive.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                ////if (!string.IsNullOrEmpty(paramVM.BranchCode))
                ////{
                ////    dtReceive.Columns.Add(new DataColumn() { DefaultValue = paramVM.BranchCode, ColumnName = "BranchCode" });
                ////}

                DataTable BranchDt = GetNewBranch(dtReceive.Copy(), null, null, connVM);

                IntegrationParam param = new IntegrationParam
                {
                    Data = dtReceive,
                    callBack = () => { },
                    SetSteps = (s) => { },
                    DefaultBranchId = paramVM.DefaultBranchId,
                    TransactionType = paramVM.TransactionType,
                    CurrentUser = paramVM.CurrentUser,
                };
                ReceiveDAL ReceiveDal = new ReceiveDAL();

                sqlResults = ReceiveDal.SaveReceive_Split(param);

                rVM.Status = sqlResults[0];
                rVM.Message = "Saved Successfully";

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "SaveSMCReceive_Web", ex.ToString());

                rVM.Message = ex.Message;
            }
            #endregion

            #region finally

            finally
            {


            }
            #endregion

            return rVM;
        }

        #endregion

        #region Toll

        public DataTable GetTollReceiveDataMaster_SMC(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchasemaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            #region try

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                CommonDAL commonDAL = new CommonDAL();

                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText

                sqlText = @"
SELECT DISTINCT
   0	Selected
   , ID
   , Vendor_Code AS VendorCode
   , Vendor_Name AS VendorName
   , Invoice_Date AS InvoiceDateTime
   , Referance_No AS ReferenceNo
   , BE_Number
   , Receive_Date
   , SUM(Quantity) AS TotalQuantity
   , SUM(Total_price) TotalValue
   , BranchCode
   , BranchName


FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_PurchaseTrading_Toll
where 1=1
----------------------------------------!Care on GROUP BY---------------------------------------- 

";
                #endregion

                #region Filtering

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    if (paramVM.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and ID = @RefNo";

                    }
                    if (paramVM.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and BE_Number = @RefNo";

                    }

                }
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    sqlText += " and ID in (";

                    foreach (string paramD in paramVM.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') <= @toDate";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 

GROUP BY 
   ID
   , Vendor_Code
   , Vendor_Name
   , Invoice_Date
   , Referance_No
   , BE_Number
   , Receive_Date
   , BranchCode
   , BranchName

";

                #endregion

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (!string.IsNullOrEmpty(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", paramVM.RefNo);

                }

                if (!string.IsNullOrEmpty(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(paramVM.ToDate).ToString("yyyy-MM-dd"));
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchasemaster);

                #endregion

                #region Selected Data

                if (dtPurchasemaster != null && dtPurchasemaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtPurchasemaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtPurchasemaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtPurchasemaster = new DataTable();
                        dtPurchasemaster = rows.CopyToDataTable();

                    }

                }

                #endregion

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetTollReceiveDataMaster_SMC", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dtPurchasemaster;

        }

        public DataTable GetTollReceiveSMC_DetailsData(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"

SELECT
     ID
   , Vendor_Code
   , BranchCode
   , BE_Number
   , Invoice_Date
   , Vendor_Name
   , Referance_No
   , LC_No
   , Receive_Date
   , Item_Code
   , Item_Name
   , Quantity
   , Total_Price
   , VAT_Amount
   , UOM
   , [Type]
   , SD_Amount
   , Assessable_Value
   , CD_Amount
   , RD_Amount
   , AT_Amount
   , AITAmount
   , Others_Amount
   , Remarks
   , Post
   , With_VDS
   , Rebate_Rate
   , VAT_Rate
   , '' ItemNo

FROM [172.16.200.17].[SMCDSS_Live].[dbo].VAT_PurchaseTrading_Toll
Where 1=1
 ";

                VendorDAL vendorDal = new VendorDAL();

                #region Filtering

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    if (param.SearchField.ToLower() == "ref_no")
                    {
                        sqlText += " and ID = @RefNo";

                    }
                    if (param.SearchField.ToLower() == "be_no")
                    {
                        sqlText += " and BE_Number = @RefNo";

                    }

                }

                if (param.IDs != null && param.IDs.Count > 0)
                {
                    sqlText += " and ID in (";

                    foreach (string paramD in param.IDs)
                    {
                        sqlText += "'" + paramD + "',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') >= @fromDate";
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    sqlText += " and Format(cast(Receive_Date as DATE),'yyyy-MM-dd') <= @toDate";
                }

                #endregion

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Peram

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", param.RefNo);
                }

                if (!string.IsNullOrEmpty(param.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(param.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                }

                string code = commonDal.settingValue("CompanyCode", "Code");

                #endregion

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("SMCIntegrationDAL", "GetTollReceiveSMC_DetailsData_Trading", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable GetTollReceiveDataDetails_SMC(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null, bool isSave = false)
        {
            #region Initializ
            string sqlText = "";
            DataTable dt = new DataTable();

            SqlConnection currConn = null;

            #endregion

            #region try

            try
            {
                ////dt = paramVM.DataSourceType.ToLower() == "Trading".ToLower()
                ////    ? GetPurchaseSMC_DetailsData_Trading(paramVM, connVM)
                ////    : GetPurchaseSMC_DbData_Web(paramVM, connVM);

                dt = GetTollReceiveSMC_DetailsData(paramVM, connVM);

                dt = TollReceiveChangeType(dt, null, null, isSave);

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("SMCIntegrationDAL", "GetTollReceiveDataDetails_SMC", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dt;

        }

        public DataTable TollReceiveChangeType(DataTable dtTable, SqlConnection vConnection = null, SqlTransaction vtransaction = null, bool isSave = false)
        {
            CommonDAL commonDal = new CommonDAL();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {

                dtTable.Columns["VAT_Rate"].ColumnName = "ProductVATRate";

                commonDal.ConnectionTransactionOpen(ref vConnection, ref vtransaction, ref currConn, ref transaction);

                string createTable = @"
create table #TempPurchase 
(
 ID varchar(200)
, Vendor_Code varchar(200)
, BE_Number varchar(200)
, Invoice_Date datetime
, Vendor_Name varchar(200)
, Referance_No varchar(200)
, LC_No varchar(200)
, Receive_Date datetime
, Item_Code varchar(200)
, Item_Name varchar(200)
, Quantity decimal(18,6)
, Total_Price decimal(18,6)
, VAT_Amount decimal(18,6)
, UOM varchar(200)
, Type varchar(200)
, SD_Amount decimal(18,6)
, Assessable_Value decimal(18,6)
, CD_Amount decimal(18,6)
, RD_Amount decimal(18,6)
, AT_Amount decimal(18,6)
, AITAmount decimal(18,6)
, Others_Amount decimal(18,6)
, Remarks varchar(200)
, Post varchar(200)
, With_VDS varchar(200)
, Rebate_Rate decimal(18,6)
, ProductVATRate decimal(18,6)
, ItemNo varchar(20)
)
";

                SqlCommand cmd = new SqlCommand(createTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("#TempPurchase", dtTable, currConn, transaction);

                string DeleteSampleData = @"
delete TempPurchaseData where SL in (
select tpd.SL from 
TempPurchaseData tpd
inner join products p on p.ProductCode = tpd.Item_Code
where isnull(p.IsSample,'N')='Y'
)
";

                cmd.CommandText = DeleteSampleData;
                cmd.ExecuteNonQuery();

                if (isSave)
                {

                    ////                    string sqlText = @"
                    ////
                    ////update #TempPurchase set ItemNo = Products.ItemNo
                    ////from products
                    ////where products.ProductCode = #TempPurchase.Item_Code
                    ////
                    ////select distinct Item_Name+'( '+Item_Code+' )' Item_NameCode from #TempPurchase where ItemNo='0' or ItemNo='' or ItemNo is null 
                    ////
                    ////";
                    ////                    cmd.CommandText = sqlText;
                    ////                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ////                    DataTable exitItem = new DataTable();
                    ////                    da.Fill(exitItem);

                    ////                    if (exitItem != null && exitItem.Rows.Count > 0)
                    ////                    {
                    ////                        string ItemNameCode = "";
                    ////                        foreach (DataRow dr in exitItem.Rows)
                    ////                        {
                    ////                            ItemNameCode += "," + dr["Item_NameCode"].ToString();
                    ////                        }

                    ////                        ItemNameCode = ItemNameCode.Trim(',');

                    ////                        throw new ArgumentNullException("FindItemId", "Item '(" + ItemNameCode + ")' not in database");

                    ////                    }
                }


                string getData = @"
select ID
, Vendor_Code
, BE_Number
, Invoice_Date
, Vendor_Name
, Referance_No
, LC_No
, Receive_Date
, Item_Code
, Item_Name
, Quantity
,Total_Price-(Total_Price*ProductVATRate/(100+ProductVATRate)) Total_Price
,Total_Price*ProductVATRate/(100+ProductVATRate) VAT_Amount
, UOM
, (case when isnull(ProductVATRate,0) = 15 then 'VAT' when isnull(ProductVATRate,0) = 0 then 'Exempted' else 'OtherRate'end) Type
, SD_Amount
, Assessable_Value
, CD_Amount
, RD_Amount
, AT_Amount
, AITAmount
, Others_Amount
, Remarks
, Post
, With_VDS
, Rebate_Rate 
from #TempPurchase

drop table #TempPurchase
";


                cmd.CommandText = getData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtResult = new DataTable();
                adapter.Fill(dtResult);

                commonDal.TransactionCommit(ref vtransaction, ref transaction);

                return dtResult;

            }
            catch (Exception e)
            {
                commonDal.TransactionRollBack(ref vtransaction, ref transaction);
                throw e;

            }
            finally
            {
                commonDal.CloseConnection(ref vConnection, ref currConn);

            }
        }

        public ResultVM SaveSMCTollReceive_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();

            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();
                CommonImportDAL cImport = new CommonImportDAL();

                DataTable dtPurchase = new DataTable();
                BranchProfileDAL branchProfile = new BranchProfileDAL();
                DataTable dtBranchInfo = branchProfile.SelectAll(null, new[] { "BranchCode" }, new[] { paramVM.BranchCode });
                paramVM.dtConnectionInfo = dtBranchInfo;

                dtPurchase = GetTollReceiveDataDetails_SMC(paramVM, connVM, true);

                if (dtPurchase == null || dtPurchase.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                PurchaseDAL purchaseDal = new PurchaseDAL();

                //if (!string.IsNullOrEmpty(paramVM.BranchCode))
                //{
                //    dtPurchase.Columns.Add(new DataColumn() { DefaultValue = paramVM.BranchCode, ColumnName = "BranchCode" });
                //}

                DataTable BranchDt = GetNewBranch(dtPurchase.Copy(), null, null, connVM);

                sqlResults = purchaseDal.SaveTempPurchase(dtPurchase, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, 0, () => { });

                if (sqlResults[0].ToLower() == "success")
                {
                    rVM.Message = "Saved Successfully";
                }
                else
                {
                    rVM.Message = sqlResults[1];
                }

                rVM.Status = sqlResults[0];

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ImportDAL", "SaveACISale_Web", ex.ToString());

                rVM.Message = ex.Message;
            }
            #endregion

            #region finally

            finally
            {


            }
            #endregion

            return rVM;
        }


        #endregion

        #endregion

        #region Branch Check

        public DataTable GetNewBranch(DataTable dt, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null, string IsProcess = "N")
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            DataTable branchDataTable = new DataTable();
            #endregion variable

            try
            {
                CommonDAL commonDal = new CommonDAL();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }

                try
                {
                    if (dt.Columns.Contains("BranchCode"))
                    {
                        dt.Columns["BranchCode"].ColumnName = "Branch_Code";
                    }

                    if (dt.Columns.Contains("Branch_Code"))
                    {
                        //////dt = new DataTable();

                        DataView view = new DataView(dt);

                        ////DataView myDataView = new DataView(dt, "", "Branch_Code", DataViewRowState.CurrentRows);

                        branchDataTable = view.ToTable(true, "Branch_Code");

                    }

                }
                catch (Exception e)
                {
                    throw new ArgumentNullException("", "New branch found001. (" + e.Message + ")");
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
create table #tempBranch(
sl int identity(1,1)
,Branch_Code varchar(100)
,Branch_Name varchar(300)
)";

                #endregion

                string BranchDelete = @"
delete #tempBranch where Branch_Code in(Select BranchCode from BranchProfiles)
";

                #region GetZeroQty

                string GetNewBranch = @"

select 
Branch_Code
,Branch_Name
from #tempBranch

";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                try
                {
                    retResults = commonDal.BulkInsert("#tempBranch", branchDataTable, currConn, transaction);

                    if (retResults[0]=="Fail")
                    {
                        throw new ArgumentNullException("", "New branch BulkInsert 01. (" + retResults[4] + ")");
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentNullException("", "New branch BulkInsert 02. (" + e.Message + ")");
                }

                cmd = new SqlCommand(BranchDelete, currConn, transaction);
                cmd.ExecuteNonQuery();

                cmd.CommandText = GetNewBranch;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string IDs = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        IDs = IDs + row["Branch_Code"].ToString() + ",";
                    }

                    IDs = IDs.Trim(',');
                    throw new ArgumentNullException("", "New branch found.Please add branch before save data Branch Code (" + IDs + ")");

                }

                //return new DataTable();
            }

            #region  catch and finally
            catch (Exception e)
            {
                throw new ArgumentNullException("", "New branch found method(" + e.Message + ")");

                //throw e;
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

            return dt;
        }

        #endregion

    }
}
