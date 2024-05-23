using System;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using VATViewModel.DTOs;
using OracleCommand = Oracle.DataAccess.Client.OracleCommand;
using OracleDataAdapter = Oracle.DataAccess.Client.OracleDataAdapter;
using OracleParameter = System.Data.OracleClient.OracleParameter;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using VATServer.Library;
using VATServer.Ordinary;

namespace VATServer.Library.Integration
{
    public class BataIntegrationDAL
    {
        private string[] sqlResults = new string[5];
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        SaleDAL _SaleDAL = new SaleDAL();
        BranchProfileDAL _branchDAL = new BranchProfileDAL();
        ImportDAL _ImportDAL = new ImportDAL();


        public ResultVM SaveSale(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            try
            {

                DataTable dtSales = new DataTable();
                DBSQLConnection dbsqlConnection = new DBSQLConnection();

                #region Save Sale

                int count = GetCount(paramVM) + 1;

                int updatedRows = 0;
                int insertedRows = 0;
                for (int i = 0; i < count; i++)
                {


                    // get top 1000 distinct Id with line items
                    dtSales = GetTopRows(paramVM);

                    if (dtSales.Rows.Count == 0)
                    {

                        break;
                    }

                    insertedRows += dtSales.Rows.Count;


                    //dtSales.Columns.Add(new DataColumn() { ColumnName = "BranchCode", DefaultValue = "001" });
                    // insert to source table

                    FileLogger.Log("BataSale", "SaveSale", "loop entered");


                    #region Bulk Insert to Source and Delete Duplicate


                    connection = dbsqlConnection.GetConnection();
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    sqlResults = _SaleDAL.ImportExcelIntegrationDataTable(dtSales, new SaleMasterVM(), null, connection, transaction);
                    DeleteDuplicateSale(connection, transaction);

                    if (paramVM.TransactionType.ToLower() == "credit")
                    {
                        string negativeUpdate =
                            @"update VAT_Source_Sales set Quantity = Quantity * -1, SourcePaidQuantity = SourcePaidQuantity * -1, Subtotal = Subtotal*-1, DiscountAmount = -1*DiscountAmount";
                        SqlCommand cmd = new SqlCommand(negativeUpdate, connection, transaction);
                        cmd.ExecuteNonQuery();
                    }


                    transaction.Commit();
                    connection.Close();

                    #endregion


                    DataTable dtVAT_Source_Sales = new DataTable();
                    dtVAT_Source_Sales = Get_VAT_Source_Sales_IDs();


                    if (dtVAT_Source_Sales.Rows.Count == 0)
                        continue;

                    sqlResults = _ImportDAL.SaveSaleWithSteps_Split(dtVAT_Source_Sales, paramVM.callBack, paramVM.DefaultBranchId,
                        "", "", "", false, "", paramVM.SetSteps);

                    int rows = UpdateTopRows(paramVM);

                    updatedRows += rows;
                }

                // Get Exempted data


                #region Bulk Insert to Source and Delete Duplicate

                DataTable exemptedProducts = GetExemptedValue(paramVM);

                try
                {
                    connection = dbsqlConnection.GetBataMiddlewareConnection();
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    CommonDAL commonDal = new CommonDAL();

                    string sqlText = "delete from ProductExempted";
                    string updateItemNo = @"update productexempted set ItemNo = Products.ItemNo
                                        from BATA2012_Demo_DB.dbo.Products
                                        where replace(Products.ProductCode,'-','') = productexempted.productcode";

                    string updateSaleNonVAT = @"


update ProductExempted set channel = 'ERD'
where channel = 'FRANCHISE'


update ProductExempted set channel = 'ECOM'
where channel = 'E-COMMERCE'

update BATA2012_Demo_DB.dbo.SalesInvoiceDetails
set SubTotal = SubTotal - isnull((ProductExempted.vatamount*Quantity),0), CurrencyValue = CurrencyValue - isnull((ProductExempted.vatamount*Quantity),0)
,Option2 = ProductExempted.vatamount*Quantity
from ProductExempted
where ProductExempted.itemno = BATA2012_Demo_DB.dbo.SalesInvoiceDetails.ItemNo
and ProductExempted.channel = BATA2012_Demo_DB.dbo.SalesInvoiceDetails.DataSource
and BATA2012_Demo_DB.dbo.SalesInvoiceDetails.Type = 'nonvat'
and isnull(BATA2012_Demo_DB.dbo.SalesInvoiceDetails.Option2,0) = 0
and BATA2012_Demo_DB.dbo.SalesInvoiceDetails.DataSource != 'Retail'


update BATA2012_Demo_DB.dbo.SalesInvoiceDetails
set Option2 = ProductExempted.vatamount*Quantity
from ProductExempted
where ProductExempted.itemno = BATA2012_Demo_DB.dbo.SalesInvoiceDetails.ItemNo
and ProductExempted.channel = BATA2012_Demo_DB.dbo.SalesInvoiceDetails.DataSource
and BATA2012_Demo_DB.dbo.SalesInvoiceDetails.Type = 'OtherRate'
and isnull(BATA2012_Demo_DB.dbo.SalesInvoiceDetails.Option2,0) = 0
and BATA2012_Demo_DB.dbo.SalesInvoiceDetails.DataSource = 'Retail'



update BATA2012_Demo_DB.dbo.SalesInvoiceDetails
set Type = 'Retail'
from BATA2012_Demo_DB.dbo.SalesInvoiceDetails
where  BATA2012_Demo_DB.dbo.SalesInvoiceDetails.Type = 'OtherRate'
and BATA2012_Demo_DB.dbo.SalesInvoiceDetails.DataSource = 'Retail'


";


                    SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                    cmd.ExecuteNonQuery();

                    sqlResults = commonDal.BulkInsert("ProductExempted", exemptedProducts, connection, transaction);

                    cmd.CommandText = updateItemNo;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = updateSaleNonVAT;
                    cmd.ExecuteNonQuery();


                    transaction.Commit();
                }
                catch (Exception e)
                {
                    if (transaction != null) transaction.Rollback();

                    throw e;
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
                #endregion

                #endregion

                #region Comments

                //updatedRows = updatedRows;
                //insertedRows = insertedRows;
                //dtSales = GetTopRows(paramVM);


                //dtSales.Columns.Add(new DataColumn() { ColumnName = "BranchCode", DefaultValue = "001" });

                //sqlResults = _SaleDAL.ImportExcelIntegrationDataTable(dtSales, new SaleMasterVM(), null, null, null);

                //DataTable dtVAT_Source_Sales = new DataTable();
                //dtVAT_Source_Sales = Get_VAT_Source_Sales_IDs();

                //sqlResults = _ImportDAL.SaveSaleWithSteps(dtVAT_Source_Sales, paramVM.callBack, paramVM.DefaultBranchId,
                //    "", "", "", false, DateTime.Now.ToString("yyyy-MM-dd"), paramVM.SetSteps);

                //int rows = UpdateTopRows(paramVM);

                #endregion



                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataSale", "SaveSale", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }

        public ResultVM SaveSale_NoCalc(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            try
            {

                DataTable dtSales = new DataTable();
                DBSQLConnection dbsqlConnection = new DBSQLConnection();

                #region Save Sale
                FileLogger.Log("BataSale", "SaveSale_NoCalc", "TransactionType :" + paramVM.TransactionType.ToString());

                int count = GetCount(paramVM) + 1;
                //int count = 1;

                int updatedRows = 0;
                int insertedRows = 0;
                for (int i = 0; i < count; i++)
                {


                    // get top 1000 distinct Id with line items
                    dtSales = GetTopRows_NoCalc(paramVM);
                    FileLogger.Log("BataSale", "SaveSale_NoCalc", "TableRecord - " + dtSales.Rows.Count.ToString());

                    if (dtSales.Rows.Count == 0)
                    {

                        break;
                    }

                    insertedRows += dtSales.Rows.Count;


                    //dtSales.Columns.Add(new DataColumn() { ColumnName = "BranchCode", DefaultValue = "001" });
                    // insert to source table

                    FileLogger.Log("BataSale", "SaveSale", "loop entered");


                    #region Bulk Insert to Source and Delete Duplicate


                    connection = dbsqlConnection.GetConnection();
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    sqlResults = _SaleDAL.ImportExcelIntegrationDataTable(dtSales, new SaleMasterVM(), null, connection, transaction);
                    DeleteDuplicateSale(connection, transaction);

                    if (paramVM.TransactionType.ToLower() == "credit")
                    {
                        string negativeUpdate =
                            @"update VAT_Source_Sales set Quantity = Quantity * -1, VAT_Amount = VAT_Amount*-1, SourcePaidQuantity = SourcePaidQuantity * -1, Subtotal = Subtotal*-1, DiscountAmount = -1*DiscountAmount, Option2 = -1*Option2";
                        SqlCommand cmd = new SqlCommand(negativeUpdate, connection, transaction);
                        cmd.ExecuteNonQuery();
                    }


                    transaction.Commit();
                    connection.Close();

                    #endregion


                    DataTable dtVAT_Source_Sales = new DataTable();
                    dtVAT_Source_Sales = Get_VAT_Source_Sales_IDs();


                    if (dtVAT_Source_Sales.Rows.Count == 0)
                        continue;

                    sqlResults = _ImportDAL.SaveSaleWithSteps_Split(dtVAT_Source_Sales, paramVM.callBack, paramVM.DefaultBranchId,
                        "", "", "", false, "", paramVM.SetSteps);

                    int rows = UpdateTopRows(paramVM);

                    updatedRows += rows;
                }

                #endregion


                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataSale", "SaveSale", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
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

                currConn = _dbsqlConnection.GetConnection();

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

        public int GetCount(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction

                string transactionType = GetTransactionType(param);


                sqlText = @"select ceil((Count(distinct ID)/2000)) as ""Count"" from onlinevat.MUSHAK_63_DATA 
                where IsProcessed = 'N' and TransactionType = '" + transactionType + "'";

                //IsProcessed = 'N'
                if (!string.IsNullOrWhiteSpace(param.RefNo))
                {
                    sqlText = sqlText + @" AND ID= :SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(param.FromDate) && !string.IsNullOrWhiteSpace(param.ToDate))
                {
                    //sqlText += @" and TO_DATE (invoicedate, 'dd-MON-yy HH24:MI:SS') BETWEEN TO_DATE (:fromDate, 'dd-MON-yy HH24:MI:SS')
                    //AND   TO_DATE (:toDate, 'dd-MON-yy HH24:MI:SS')";
                    // to_date(substr(invoicedate,1,9),'dd-mm-rrrr')
                    sqlText += @"  and invoicedate BETWEEN  :fromDate
                                AND   :toDate";
                }


                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(sqlText, connection);
                cmd.Transaction = transaction;


                if (!string.IsNullOrWhiteSpace(param.RefNo))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("SalesInvoiceNo", param.RefNo));
                }

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }


                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    count = Convert.ToInt32(result.ToString());
                }


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return count;
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

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        private string GetTransactionType(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            string transactionType = "";

            if (param.TransactionType.ToLower() == "other")
            {
                transactionType = "Local";
            }
            else if (param.TransactionType.ToLower() == "credit")
            {
                transactionType = "Credit";
            }

            return transactionType;
        }

        public int GetTransCount(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction

                sqlText = @"select ceil((Count(distinct ID)/50)) as ""Count"" from ONLINEVAT.transferissues 
                where IsProcessed = 'N' ";

                //IsProcessed = 'N'
                if (!string.IsNullOrWhiteSpace(param.RefNo))
                {
                    sqlText = sqlText + @" AND ID= :transferId";
                }

                if (!string.IsNullOrWhiteSpace(param.FromDate) && !string.IsNullOrWhiteSpace(param.ToDate))
                {
                    //TO_DATE(substr(transactiondate,1,9), 'dd-mm-rrrr')
                    sqlText += @" and transactiondate BETWEEN :fromDate
                                AND  :toDate ";
                }


                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(sqlText, connection);
                cmd.Transaction = transaction;


                if (!string.IsNullOrWhiteSpace(param.RefNo))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("transferId", param.RefNo));
                }

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }


                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    count = Convert.ToInt32(result.ToString());
                }


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return count;
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("BataDAL", "TransferCount", ex.ToString() + "\n" + sqlText);
                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public DataTable GetTopRows(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();
                FileLogger.Log("BataDAL", "Process_Step", "connection String - " + connection.ConnectionString);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                FileLogger.Log("BataDAL", "Process_Step", "Connection Opened");

                #endregion open connection and transaction

                string transactionType = GetTransactionType(param);

                #region Sql Text

                string deleteSaleIds = @"
delete from saleids

 ";
                //onlinevat.MUSHAK_63_DATA.isprocessed = 'N' to_date(substr(invoicedate,1,9),'dd-mm-rrrr')
                string insertToIdtable = @"
insert into saleids
select  ID from (select distinct ID from onlinevat.MUSHAK_63_DATA
 where  onlinevat.MUSHAK_63_DATA.isprocessed = 'N' and invoicedate BETWEEN  :fromDate
                                AND   :toDate

and TransactionType = '" + transactionType +
                                         "' @condition)sale where ROWNUM <=2000";

                #region Comments


                ////                string selectRows = @"select    
                ////      ID 
                ////      ,NVL(CustomerName,'-') as ""CustomerName""
                ////                    ,CustomerCode as ""CustomerCode""
                ////                    ,CustomerGroup as ""CustomerGroup""
                ////
                ////                    ,branchcode as ""BranchCode""
                ////                    ,DeliveryAddress as ""DeliveryAddress""
                ////                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                ////                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                ////                    , Post as ""Post""
                ////                    ,ProductCode as  ""ProductCode"" 
                ////                    ,ProductName as ""ProductName""
                ////                    , Quantity as ""Quantity""
                ////                    ,NVL(SourcePaidQuantity,0) as ""SourcePaidQuantity""
                ////                    ,UnitPrice as ""UnitPrice"" 
                ////                    , UOM as ""UOM""
                ////                    ,VATRate  as ""VATRate""
                ////                    ,SDRate as ""SDRate"" 
                ////
                ////                    ,DiscountAmount as ""DiscountAmount""
                ////                    ,PromotionalQuantity as ""PromotionalQuantity"" 
                ////
                ////                    ,LCNumber as ""LCNumber"" 
                ////                    ,CurrencyCode as ""CurrencyCode"" 
                ////
                ////
                ////                    ,ID as ""ReferenceNo""
                ////                    , SubTotal as ""SubTotal""
                ////                    ,Type as ""Type""
                ////                    ,channel as ""DataSource""
                ////                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                ////                 from onlinevat.MUSHAK_63_DATA where ID in(
                ////select ID from Saleids
                ////                    )  and TransactionType = '" + transactionType + "'";

                #endregion


                string selectRows = @"



select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    , Quantity as ""Quantity""
                    ,sourcepaidquantity as ""SourcePaidQuantity""
                    ,UnitPrice as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate"" 

                    ,DiscountAmount as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""
                    , SubTotal as ""SubTotal""
                    ,Type as ""Type""
                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  
                    and TransactionType = '" + transactionType + @"' 
                    and (NVL(sourcepaidquantity,0)=0) 
                    and Quantity != 0
UNION ALL 

select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    , Quantity as ""Quantity""
                    ,sourcepaidquantity as ""SourcePaidQuantity""
                    ,(discountamount+subtotal)/quantity as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate"" 

                    ,DiscountAmount as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""
                    , SubTotal as ""SubTotal""
                    ,Type as ""Type""
                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  
                    and TransactionType = '" + transactionType + @"' 
                    and (quantity=sourcepaidquantity)
                    and Quantity != 0 
UNION ALL 




select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    , Quantity-NVL(sourcepaidquantity,0) as ""Quantity""
                    ,0 as ""SourcePaidQuantity""
                    ,UnitPrice as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate""
                    ,((discountamount/quantity)*(quantity-NVL(sourcepaidquantity,0))) as ""DiscountAmount""
                    --,DiscountAmount as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""
                    , SubTotal as ""SubTotal""
                    ,Type as ""Type""
                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  and TransactionType = '" + transactionType + @"' 
                    and quantity > sourcepaidquantity 
                    and sourcepaidquantity != 0
                    and Quantity != 0

UNION ALL 

select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    ,NVL(sourcepaidquantity,0) as ""Quantity""
                    ,NVL(sourcepaidquantity,0) as ""SourcePaidQuantity""
                    ,(discountamount+subtotal)/quantity as ""UnitPrice"" 
                    --,UnitPrice as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate"" 

                    ,(DiscountAmount/quantity)*NVL(sourcepaidquantity,0) as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""
                    , SubTotal as ""SubTotal""
                    ,Type as ""Type""
                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  and TransactionType = '" + transactionType + @"' 
                    and quantity > sourcepaidquantity 
                    and sourcepaidquantity != 0 
                    and Quantity != 0
";

                #endregion

                #region delete sale ids

                //FileLogger.Log("BataDAL", "Process_Step", "before cmd " + deleteSaleIds);

                OracleCommand cmd = new OracleCommand(deleteSaleIds, connection);
                cmd.Transaction = transaction;


                int rows = cmd.ExecuteNonQuery();



                #endregion

                #region inser to Sale Ids

                string conditionText = "";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    conditionText += " and onlinevat.MUSHAK_63_DATA.ID='" + param.RefNo + "'";
                }

                if (param.DataSourceType.ToLower() != "all")
                {
                    conditionText += " and onlinevat.MUSHAK_63_DATA.channel='" + param.DataSourceType + "'";
                }

                //insertToIdtable = !string.IsNullOrEmpty(param.RefNo)
                //    ? insertToIdtable.Replace("@condition", " and onlinevat.MUSHAK_63_DATA.ID='" + param.RefNo + "'")
                //    : insertToIdtable.Replace("@condition", "");

                insertToIdtable = insertToIdtable.Replace("@condition", conditionText);

                cmd.CommandText = insertToIdtable;

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {

                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));



                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));


                }


                //FileLogger.Log("BataDAL", "GetTopRows", "Before Insert id");

                rows = cmd.ExecuteNonQuery();



                #endregion

                cmd.CommandText = selectRows;

                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                adapter.Fill(table);

                FileLogger.Log("BataDAL", "GetTopRows", table.Rows.Count + " \n " + selectRows);




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
                if (transaction != null)
                {
                    transaction.Rollback();
                    //transaction.Commit();

                }
                FileLogger.Log("BataDAL", "SelectQuearyLog", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public DataTable GetTopRows_NoCalc(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction
               
                connection = _dbsqlConnection.GetBataConnection();
                FileLogger.Log("BataDAL", "Process_Step", "connection String - " + connection.ConnectionString);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                FileLogger.Log("BataDAL", "Process_Step", "Connection Opened");

                #endregion open connection and transaction

                string transactionType = GetTransactionType(param);

                #region Sql Text

                string deleteSaleIds = @"
delete from saleids

 ";
                //onlinevat.MUSHAK_63_DATA.isprocessed = 'N' to_date(substr(invoicedate,1,9),'dd-mm-rrrr')
                string insertToIdtable = @"
insert into saleids
select  ID from (select distinct ID from onlinevat.MUSHAK_63_DATA
 where  onlinevat.MUSHAK_63_DATA.isprocessed = 'N' and invoicedate BETWEEN  :fromDate
                                AND   :toDate

and TransactionType = '" + transactionType +
                                         "' @condition)sale where ROWNUM <=2000";


                // Subtotal
                // VATamount

                string selectRows = @"
select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    , Quantity as ""Quantity""
                    ,sourcepaidquantity as ""SourcePaidQuantity""
                    ,UnitPrice as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate"" 

                    ,DiscountAmount as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""

                    , case 
                    when VATRate=15 then Total_NET_15_Per 
                    when VATRate=5 then Total_Trade_NET
                    when VATRate = 0 then Refund_input_net else 0 end as ""SubTotal""
                    ,case
                         when channel='RETAIL' and Type = 'OtherRate' then 'RETAIL'
                         when channel='CLEARANCE WSALE' and Type = 'OtherRate' then 'RETAIL' 
                     else Type end as ""Type""
                    , case 
                    when VATRate=15 then Total_VAT_15_Per 
                    when VATRate=5 then Total_Trade_VAT
                    when VATRate = 0 then 0 end ""VAT_Amount""
                    ,Refund_input_VAT as ""Option2""

                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  
                    and TransactionType = '" + transactionType + @"' 
                    and (NVL(sourcepaidquantity,0)=0) 
                    and Quantity != 0
UNION ALL 

select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    , Quantity as ""Quantity""
                    ,sourcepaidquantity as ""SourcePaidQuantity""
                    ,(discountamount+subtotal)/quantity as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate"" 

                    ,DiscountAmount as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""


                    , case 
                    when VATRate=15 then Total_NET_15_Per 
                    when VATRate=5 then Total_Trade_NET
                    when VATRate = 0 then Refund_input_net else 0 end as ""SubTotal""
                     ,case
                         when channel='RETAIL' and Type = 'OtherRate' then 'RETAIL'
                         when channel='CLEARANCE WSALE' and Type = 'OtherRate' then 'RETAIL' 
                     else Type end as ""Type""
                    , case 
                    when VATRate=15 then Total_VAT_15_Per 
                    when VATRate=5 then Total_Trade_VAT
                    when VATRate = 0 then 0 end ""VAT_Amount""
                    ,Refund_input_VAT as ""Option2""


                    ,channel as ""DataSource""

                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  
                    and TransactionType = '" + transactionType + @"' 
                    and (quantity=sourcepaidquantity)
                    and Quantity != 0 
UNION ALL 




select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    , Quantity-NVL(sourcepaidquantity,0) as ""Quantity""
                    ,0 as ""SourcePaidQuantity""
                    ,UnitPrice as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate""
                    ,((discountamount/quantity)*(quantity-NVL(sourcepaidquantity,0))) as ""DiscountAmount""
                    --,DiscountAmount as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""

                    , case 
                    when VATRate=15 then Total_NET_15_Per 
                    when VATRate=5 then Total_Trade_NET
                    when VATRate = 0 then Refund_input_net else 0 end as ""SubTotal""
                    ,case
                         when channel='RETAIL' and Type = 'OtherRate' then 'RETAIL'
                         when channel='CLEARANCE WSALE' and Type = 'OtherRate' then 'RETAIL' 
                     else Type end as ""Type""
                    , case 
                    when VATRate=15 then Total_VAT_15_Per 
                    when VATRate=5 then Total_Trade_VAT
                    when VATRate = 0 then 0 end ""VAT_Amount""
                    ,Refund_input_VAT as ""Option2""

                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  and TransactionType = '" + transactionType + @"' 
                    and quantity > sourcepaidquantity 
                    and sourcepaidquantity != 0
                    and Quantity != 0

UNION ALL 

select    
      ID 
      ,NVL(CustomerName,CustomerCode) as ""CustomerName""
                    ,CustomerCode as ""CustomerCode""
                    ,CustomerGroup as ""CustomerGroup""

                    ,branchcode as ""BranchCode""
                    ,DeliveryAddress as ""DeliveryAddress""
                    ,invoicedate || ' ' || invoicetime as ""Invoice_Date_Time""
                    ,invoicedate || ' ' || invoicetime as ""DeliveryDateTime""
                    , Post as ""Post""
                    ,ProductCode as  ""ProductCode"" 
                    ,ProductName as ""ProductName""
                    ,NVL(sourcepaidquantity,0) as ""Quantity""
                    ,NVL(sourcepaidquantity,0) as ""SourcePaidQuantity""
                    ,(discountamount+subtotal)/quantity as ""UnitPrice"" 
                    --,UnitPrice as ""UnitPrice"" 
                    , UOM as ""UOM""
                    ,VATRate  as ""VATRate""
                    ,SDRate as ""SDRate"" 

                    ,(DiscountAmount/quantity)*NVL(sourcepaidquantity,0) as ""DiscountAmount""
                    ,PromotionalQuantity as ""PromotionalQuantity"" 

                    ,LCNumber as ""LCNumber"" 
                    ,CurrencyCode as ""CurrencyCode"" 
                    ,ID as ""ReferenceNo""

                    , case 
                    when VATRate=15 then Total_NET_15_Per 
                    when VATRate=5 then Total_Trade_NET
                    when VATRate = 0 then Refund_input_net else 0 end as ""SubTotal""
                     ,case
                         when channel='RETAIL' and Type = 'OtherRate' then 'RETAIL'
                         when channel='CLEARANCE WSALE' and Type = 'OtherRate' then 'RETAIL' 
                     else Type end as ""Type""
                    , case 
                    when VATRate=15 then Total_VAT_15_Per 
                    when VATRate=5 then Total_Trade_VAT
                    when VATRate = 0 then 0 end ""VAT_Amount""
                    ,Refund_input_VAT as ""Option2""

                    ,channel as ""DataSource""
                    ,(case when TransactionType = 'Local' then 'Other' else TransactionType end) as ""TransactionType""
                    ,(case when TransactionType = 'Credit' then REFERENCENO else '' end) as ""Previous_Invoice_No""

                 from onlinevat.MUSHAK_63_DATA where ID in(
select ID from Saleids
                    )  and TransactionType = '" + transactionType + @"' 
                    and quantity > sourcepaidquantity 
                    and sourcepaidquantity != 0 
                    and Quantity != 0
";

                #endregion

                #region delete sale ids

                //FileLogger.Log("BataDAL", "Process_Step", "before cmd " + deleteSaleIds);

                OracleCommand cmd = new OracleCommand(deleteSaleIds, connection);
                cmd.Transaction = transaction;


                int rows = cmd.ExecuteNonQuery();



                #endregion

                #region inser to Sale Ids

                string conditionText = "";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    conditionText += " and onlinevat.MUSHAK_63_DATA.ID='" + param.RefNo + "'";
                }

                if (param.DataSourceType.ToLower() != "all")
                {
                    conditionText += " and onlinevat.MUSHAK_63_DATA.channel='" + param.DataSourceType + "'";
                }

                //insertToIdtable = !string.IsNullOrEmpty(param.RefNo)
                //    ? insertToIdtable.Replace("@condition", " and onlinevat.MUSHAK_63_DATA.ID='" + param.RefNo + "'")
                //    : insertToIdtable.Replace("@condition", "");

                insertToIdtable = insertToIdtable.Replace("@condition", conditionText);

                cmd.CommandText = insertToIdtable;

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {

                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));



                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));


                }


                FileLogger.Log("BataDAL", "GetTopRows", "Before Insert id");

                rows = cmd.ExecuteNonQuery();

                FileLogger.Log("BataDAL", "GetTopRows", "After Insert id");


                #endregion

                cmd.CommandText = selectRows;

                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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
                if (transaction != null)
                {
                    transaction.Rollback();
                    //transaction.Commit();

                }
                FileLogger.Log("BataDAL", "SelectQuearyLog", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public DataTable GetExemptedValue(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                #endregion open connection and transaction


                #region Sql Text


                string selectRows = @"

select * from ONLINEVAT.product_master
where (cat_code = 40 or cat_code = 42) and mrp <= 150
";

                #endregion



                OracleCommand cmd = new OracleCommand(selectRows, connection);
                cmd.Transaction = transaction;

                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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
                if (transaction != null)
                {
                    //transaction.Rollback();
                    transaction.Commit();

                }
                FileLogger.Log("BataDAL", "SelectQuearyLog", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public DataTable GetTopTransferDataTable(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction

                #region Sql Text

                string deleteSaleIds = @"
delete from transferIds

 ";
                //onlinevat.MUSHAK_63_DATA.isprocessed = 'N'
                string insertToIdtable = @"
insert into transferIds
select  ID from (select distinct ID from ONLINEVAT.transferissues
 where isprocessed = 'N'  
and transactiondate BETWEEN :fromDate
                                AND  :toDate  @condition )sale where ROWNUM <=50";


                string selectRows = @"SELECT ID
      ,BranchCode as ""BranchCode""
                    ,TransferToBranchCode as ""TransferToBranchCode""
                    ,TransactionDate || ' ' || transactiontime as ""TransactionDateTime"" 
                    ,ProductType  as ""TransactionType""
                    ,ProductCode as ""ProductCode""
                    ,ProductName as ""ProductName""
                    ,UOM as ""UOM""
                    ,Quantity as ""Quantity""
                    ,CostPrice as ""CostPrice""
                    ,Post as ""Post""
                    ,VATRate as ""VAT_Rate""
                    ,ID as ""ReferenceNo""
                    ,Comments as ""Comments""
                FROM Onlinevat.TransferIssues where IsProcessed = 'N' 
                and ID in (select ID from transferids)";

                #endregion


                #region delete sale ids

                OracleCommand cmd = new OracleCommand(deleteSaleIds, connection);
                cmd.Transaction = transaction;

                int rows = cmd.ExecuteNonQuery();

                #endregion

                #region inserto Sale Ids
                //'" + param.RefNo + "'"
                insertToIdtable = !string.IsNullOrEmpty(param.RefNo)
                    ? insertToIdtable.Replace("@condition", " and ID='" + param.RefNo + "'")
                    : insertToIdtable.Replace("@condition", "");

                cmd.CommandText = insertToIdtable;

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }



                rows = cmd.ExecuteNonQuery();

                #endregion


                cmd.CommandText = selectRows;
                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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

                FileLogger.Log("BataDAL", "SaveTransfer", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public int UpdateTopRows(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction


                string transactionType = GetTransactionType(param);

                //and invoicedate >= @fromDate and invoicedate <= @toDate
                //invoicedate >= '01-Jan-2020' and invoicedate <= '31-Jan-2020'
                sqlText =
                    @" update onlinevat.MUSHAK_63_DATA set IsProcessed = 'Y' where ID in(select distinct ID from Saleids) and TransactionType = '" +
                    transactionType + "'";


                #region Conditions

                //if (!string.IsNullOrWhiteSpace(param.RefNo))
                //{
                //    sqlText = sqlText + @" AND ID= :SalesInvoiceNo";
                //}

                //if (!string.IsNullOrWhiteSpace(param.FromDate))
                //{
                //    sqlText = sqlText + @" AND invoicedate  >= :fromDate ";
                //}

                //if (!string.IsNullOrWhiteSpace(param.ToDate))
                //{
                //    sqlText = sqlText + @" AND invoicedate <= :toDate";
                //}


                //sqlText += @") disID where RowNum <= 3500)";

                #endregion

                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(sqlText, connection);
                cmd.Transaction = transaction;



                #region Params

                //if (!string.IsNullOrWhiteSpace(param.RefNo))
                //{
                //    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("SalesInvoiceNo", param.RefNo));
                //}

                //if (!string.IsNullOrWhiteSpace(param.FromDate))
                //{
                //    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate", Convert.ToDateTime(param.FromDate).ToString("dd-MM-yyyy")));

                //}

                //if (!string.IsNullOrWhiteSpace(param.ToDate))
                //{
                //    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate", Convert.ToDateTime(param.ToDate).ToString("dd-MM-yyyy")));
                //}

                #endregion


                int rows = cmd.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return rows;
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

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public int UpdateTransferTopRows(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction

                //and invoicedate >= @fromDate and invoicedate <= @toDate
                //invoicedate >= '01-Jan-2020' and invoicedate <= '31-Jan-2020'
                sqlText = @" update onlinevat.transferissues 
set IsProcessed = 'Y' 
where ID in(select distinct ID from transferids)  ";


                #region Conditions



                #endregion

                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(sqlText, connection);
                cmd.Transaction = transaction;



                #region Params



                #endregion


                int rows = cmd.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return rows;
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

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public void DeleteDuplicateSale(SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

--delete VAT_Source_Sales from 
--VAT_Source_Sales vs join SalesInvoiceHeaders sh 
--on vs.ID = sh.ImportIDExcel 
--and vs.PeriodId = sh.PeriodID and vs.TransactionType = sh.TransactionType


DELETE b FROM VAT_Source_Sales b
WHERE  EXISTS (
    SELECT 1 FROM SalesInvoiceHeaders s
    WHERE s.ImportIDExcel = b.ID 
	and s.PeriodID = b.PeriodId 
	and b.TransactionType = s.TransactionType
	);
";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;
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
                FileLogger.Log("BataSale", "SaveSale", ex.Message + "\n" + ex.StackTrace);

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

        public void DeleteDuplicateTransfer(SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

        public ResultVM SaveTransferIssue(IntegrationParam paramVM, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            try
            {

                DataTable dtTransfers = new DataTable();

                TransferIssueDAL transferIssueDal = new TransferIssueDAL();


                #region Split Loop

                int count = GetTransCount(paramVM) + 1;

                int updatedRows = 0;
                int insertedRows = 0;
                for (int i = 0; i < count; i++)
                {
                    // get top 100 distinct Id with line items
                    dtTransfers = GetTopTransferDataTable(paramVM);

                    if (dtTransfers.Rows.Count == 0)
                    {
                        break;
                    }

                    insertedRows += dtTransfers.Rows.Count;


                    #region Bulk Insert to Source and Delete Duplicate

                    DBSQLConnection dbsqlConnection = new DBSQLConnection();
                    CommonDAL commonDal = new CommonDAL();

                    connection = dbsqlConnection.GetConnection();
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    string sqlText = "delete from VAT_Source_TransferIssues";
                    SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                    cmd.ExecuteNonQuery();
                    sqlResults = commonDal.BulkInsert("VAT_Source_TransferIssues", dtTransfers, connection, transaction);

                    DeleteDuplicateTransfer(connection, transaction);

                    transaction.Commit();
                    connection.Close();

                    #endregion


                    // get all data from source data
                    DataTable dtVAT_Source_transfer = new DataTable();
                    dtVAT_Source_transfer = Get_VAT_Source_Transfer();

                    if (dtVAT_Source_transfer.Rows.Count == 0)
                        continue;

                    sqlResults = transferIssueDal.SaveTempTransfer(dtVAT_Source_transfer, paramVM.BranchCode,
                        paramVM.TransactionType, paramVM.CurrentUser, paramVM.DefaultBranchId, () => { }, null, null, false, null, "", UserId);


                    int rows = UpdateTransferTopRows(paramVM);

                    updatedRows += rows;
                }

                #endregion




                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                FileLogger.Log("BataDAL", "SaveTransfer", ex.ToString());


                rVM.Message = ex.Message;

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }

        public ResultVM SaleDataProcess(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            try
            {

                DataTable dtSales = new DataTable();

                #region Save Sale

                int count = GetCount(paramVM) + 1;

                int updatedRows = 0;
                int insertedRows = 0;
                for (int i = 0; i < count; i++)
                {

                    // get top 1000 distinct Id with line items
                    dtSales = GetData_Oracle(paramVM);

                    if (dtSales.Rows.Count == 0)
                    {

                        break;
                    }

                    insertedRows += dtSales.Rows.Count;

                    #region Bulk Insert to Source and Delete Duplicate

                    DBSQLConnection dbsqlConnection = new DBSQLConnection();

                    #region open connection and transaction

                    connection = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    transaction = connection.BeginTransaction();

                    #endregion open connection and transaction

                    CommonDAL commonDal = new CommonDAL();

                    sqlResults = commonDal.BulkInsert("sales", dtSales, connection, transaction);

                    ////DeleteDuplicateSale(connection, transaction);

                    transaction.Commit();
                    connection.Close();

                    #endregion

                    int rows = UpdateTopRows(paramVM);

                    updatedRows += rows;
                }

                #endregion


                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataSale", "GetSaleData", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }

        public DataTable GetData_Oracle(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();
                FileLogger.Log("BataDAL", "Process_Step", "connection String - " + connection.ConnectionString);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                FileLogger.Log("BataDAL", "Process_Step", "Connection Opened");

                #endregion open connection and transaction


                string transactionType = GetTransactionType(param);

                #region Sql Text

                string deleteSaleIds = @"
delete from saleids

 ";
                //onlinevat.MUSHAK_63_DATA.isprocessed = 'N' to_date(substr(invoicedate,1,9),'dd-mm-rrrr')
                string insertToIdtable = @"
insert into saleids
select  ID from (select distinct ID from onlinevat.MUSHAK_63_DATA
 where  onlinevat.MUSHAK_63_DATA.isprocessed = 'N' and invoicedate BETWEEN  :fromDate
                                AND   :toDate

and TransactionType = '" + transactionType +
                                         "' @condition)sale where ROWNUM <=2000";




                string selectRows = @"

select    
BRANCHCODE 
,ID 
,CUSTOMERGROUP 
,CUSTOMERCODE 
,CUSTOMERNAME 
,DELIVERYADDRESS 
,INVOICEDATE 
,INVOICETIME 
,DELIVERYDATE 
,DELIVERYTIME 
,REFERENCENO
,COMMENTS
,PRODUCTCODE 
,PRODUCTNAME 
,UOM 
,QUANTITY 
,SOURCEPAIDQUANTITY 
,UNITPRICE 
,SUBTOTAL 
,VATRATE 
,SDRATE 
,DISCOUNTAMOUNT 
,PROMOTIONALQUANTITY 
,POST
,LCNUMBER
,CURRENCYCODE
,LINECOMMENTS
,TYPE 
,TRANSACTIONTYPE 
,ISPROCESSED 
,COMPANYCODE 
,CHANNEL
,TOTAL_TURNOVER  
,TOTAL_NET_15_PER
,TOTAL_VAT_15_PER
,TOTAL_TRADE_NET
,TOTAL_TRADE_VAT
,REFUND_INPUT_NET
,REFUND_INPUT_VAT
,TOTAL_NET_AMOUNT
,TOTAL_VAT_AMOUNT
 

   
 from onlinevat.MUSHAK_63_DATA 
where ID in(
select ID from Saleids
                   )  
                   and TransactionType = '" + transactionType + @"' 

";

                #endregion


                #region delete sale ids

                OracleCommand cmd = new OracleCommand(deleteSaleIds, connection);
                cmd.Transaction = transaction;

                int rows = cmd.ExecuteNonQuery();

                #endregion

                #region inser to Sale Ids
                //'" + param.RefNo + "'"
                insertToIdtable = !string.IsNullOrEmpty(param.RefNo)
                    ? insertToIdtable.Replace("@condition", " and onlinevat.MUSHAK_63_DATA.ID='" + param.RefNo + "'")
                    : insertToIdtable.Replace("@condition", "");

                cmd.CommandText = insertToIdtable;

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {

                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));

                }

                rows = cmd.ExecuteNonQuery();

                #endregion
                cmd.CommandText = selectRows;

                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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
                if (transaction != null)
                {
                    //transaction.Rollback();
                    transaction.Commit();

                }
                FileLogger.Log("BataDAL", "SelectQuearyLog", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public ResultVM TransferIssueDataProcess(IntegrationParam paramVM, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            try
            {

                DataTable dtTransfers = new DataTable();

                TransferIssueDAL transferIssueDal = new TransferIssueDAL();

                FileLogger.Log("BataIntegrationDAL", "TransferIssueDataProcess", "Entered");

                #region Split Loop

                int count = GetTransCount(paramVM) + 1;

                FileLogger.Log("BataIntegrationDAL", "TransferIssueDataProcess", "AfterCount");

                int updatedRows = 0;
                int insertedRows = 0;
                for (int i = 0; i < count; i++)
                {
                    // get top 100 distinct Id with line items
                    dtTransfers = GetTransferIssueData_Oracle(paramVM);

                    if (dtTransfers.Rows.Count == 0)
                    {
                        break;
                    }

                    insertedRows += dtTransfers.Rows.Count;


                    #region Bulk Insert to Source and Delete Duplicate

                    DBSQLConnection dbsqlConnection = new DBSQLConnection();

                    #region open connection and transaction

                    connection = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    transaction = connection.BeginTransaction();

                    #endregion open connection and transaction

                    CommonDAL commonDal = new CommonDAL();

                    sqlResults = commonDal.BulkInsert("TransferIssues", dtTransfers, connection, transaction);

                    ////DeleteDuplicateSale(connection, transaction);

                    transaction.Commit();
                    connection.Close();

                    #endregion

                    int rows = UpdateTransferTopRows(paramVM);

                    updatedRows += rows;
                }

                #endregion

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                FileLogger.Log("BataIntegrationDAL", "TransferIssueDataProcess", ex.ToString());


                rVM.Message = ex.Message;

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }

        public DataTable GetTransferIssueData_Oracle(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction

                #region Sql Text

                string deleteSaleIds = @"
delete from transferIds

 ";
                //onlinevat.MUSHAK_63_DATA.isprocessed = 'N'
                string insertToIdtable = @"
insert into transferIds
select  ID from (select distinct ID from ONLINEVAT.transferissues
 where isprocessed = 'N'  
and transactiondate BETWEEN :fromDate
                                AND  :toDate  @condition )sale where ROWNUM <=50";


                string selectRows = @"SELECT 
ID, 
BRANCHCODE, 
TRANSFERTOBRANCHCODE, 
TRANSACTIONDATE, 
TRANSACTIONTIME, 
PRODUCTTYPE, 
PRODUCTCODE, 
PRODUCTNAME, 
UOM, 
QUANTITY, 
COSTPRICE, 
POST, 
VATRATE, 
REFERENCENO, 
COMMENTS, 
ISPROCESSED, 
COMPANYCODE
                FROM Onlinevat.TransferIssues where IsProcessed = 'N' 
                and ID in (select ID from transferids)";

                #endregion


                #region delete sale ids

                OracleCommand cmd = new OracleCommand(deleteSaleIds, connection);
                cmd.Transaction = transaction;

                int rows = cmd.ExecuteNonQuery();

                #endregion

                #region inserto Sale Ids
                //'" + param.RefNo + "'"
                insertToIdtable = !string.IsNullOrEmpty(param.RefNo)
                    ? insertToIdtable.Replace("@condition", " and ID='" + param.RefNo + "'")
                    : insertToIdtable.Replace("@condition", "");

                cmd.CommandText = insertToIdtable;

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }



                rows = cmd.ExecuteNonQuery();

                #endregion


                cmd.CommandText = selectRows;
                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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

                FileLogger.Log("BataDAL", "SaveTransfer", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }



        public DataTable Get4_3Data_Db(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();


                #endregion open connection and transaction


                sqlText = @"SELECT
                FGArticleCode as ""FCode""	
                    ,'-' as ""FName""
                    ,'pr' as ""FUOM""	
                    ,'-' as ""RName""
                    ,rawpackcode as ""RCode""	
                    ,UOM as ""RUOM""
                    ,'-' as ""CustomerName""
                    ,'-' as ""CustomerCode""
                    ,'1900-01-0' as ""FirstSupplyDate""
                    ,'-' as ""ReferenceNo""	
                    ,'1900-01-0' as ""EffectDate""
                    ,'VAT 4.3' as ""VATName""	
                    ,TotalQuantity as ""TotalQuantity""
                    ,totalquantity - (TotalQuantity * (perchantage/100)) as ""UseQuantity""	
                    ,TotalQuantity * (perchantage/100) as ""WastageQuantity""	
                    ,purchasevalue as ""Cost""	
                    ,100.00 as ""RebateRate""	
                    ,'Y' as ""IssueOnProduction""
                    ,SELLPRICE as ""Price""
                    FROM ONLINEVAT.mushak_43 where 1=1 ";


                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and FGArticleCode=:FGArticleCode";
                }


                OracleCommand cmd = new OracleCommand(sqlText, connection);
                cmd.Transaction = transaction;

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("FGArticleCode",
                        param.RefNo));
                }


                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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
                if (transaction != null)
                {
                    transaction.Rollback();
                    //transaction.Commit();

                }
                FileLogger.Log("BataDAL", "SelectQuearyLog", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }


        public DataTable Get4_3Data(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                // Get Data
                DataTable dtRawData =  Get4_3Data_Db(param, connVM);


                if (dtRawData.Rows.Count > 0)
                {
                    decimal price = Convert.ToDecimal(dtRawData.Rows[0]["Price"]);

                    decimal vatExcludedPrice = (price * 100) / (100 + 15);

                    // Get expense Details based on transaction type

                    DataTable dtExpense = GetExpenseInfo(param);

                    //decimal expsenseValue = dtExpense.Rows.Cast<DataRow>().Sum(dtExpenseRow =>
                    //    (vatExcludedPrice - (vatExcludedPrice * Convert.ToDecimal(dtExpenseRow["ExpensePercent"]) / 100)));

                    decimal expsenseValue = vatExcludedPrice;

                    foreach (DataRow dtExpenseRow in dtExpense.Rows)
                    {
                        expsenseValue -= (vatExcludedPrice * Convert.ToDecimal(dtExpenseRow["ExpensePercent"]) / 100);
                    }


                    foreach (DataRow dtExpenseRow in dtRawData.Rows)
                    {
                        expsenseValue -= Convert.ToDecimal(dtExpenseRow["Cost"]);
                    }


                    //FileLogger.Log("BataDAL", "Get4_3Data",
                    //    price + " " + vatExcludedPrice + " " + expsenseValue + " " + sumTotal + " " + sumOverheads);

                    param.ExpsenseValue = expsenseValue;
                    param.FinishItemName = dtRawData.Rows[0]["FCode"].ToString();

                    DataTable dtOverheads = GetOverheadInfo(param);

                    dtRawData.Merge(dtOverheads);
                }

                
                return dtRawData;
            }
            catch (Exception e)
            {
                FileLogger.Log("BataDAL", "Get4_3Data", e.ToString());
                throw;
            }
        }

        private DataTable GetExpenseInfo(IntegrationParam param)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DBSQLConnection dbsqlConnection = new DBSQLConnection();

            try
            {
                connection = dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();


                string sqlText = @"
select * from OverheadExpenses where 1=1 
and TransactionType = @TransactionType
and HeadType in (
'Commission'
,'Profit'
,'Royalty'
)
";

                SqlCommand command = new SqlCommand(sqlText,connection, transaction);
                command.Parameters.AddWithValue("@TransactionType",param.TransactionType);

                DataTable dtExpense = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dtExpense);


                transaction.Commit();

                return dtExpense;

            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private DataTable GetOverheadInfo(IntegrationParam param)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DBSQLConnection dbsqlConnection = new DBSQLConnection();

            try
            {
                connection = dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();


                string sqlText = @"
select 

@FName FCode	
, '-' FName	
,'pr' FUOM	
,HeadName RName	
,'-' RCode	
,'-' RUOM	
,'-' CustomerName	
,'-' CustomerCode	
,'1900-01-01' FirstSupplyDate	
,'-'ReferenceNo	
,'1900-01-01' EffectDate	
,'VAT 4.3' VATName	
,1.00 TotalQuantity	
,1.00 UseQuantity	
,0.00 WastageQuantity	
,(@expsense*ExpensePercent)/100 Cost	
,(case when HeadType = 'Rebatable' then 100.00 else 0.00 end ) RebateRate	
,'N' IssueOnProduction


from OverheadExpenses where 1=1
and TransactionType = @TransactionType
and HeadType not in (
'Commission'
,'Profit'
,'Royalty'
)
";

                SqlCommand command = new SqlCommand(sqlText,connection, transaction);
                command.Parameters.AddWithValue("@TransactionType", param.TransactionType);
                command.Parameters.AddWithValue("@FName", param.FinishItemName);
                command.Parameters.AddWithValue("@expsense", param.ExpsenseValue);

                //FileLogger.Log("BataDAL", "Get4_3Data", param.TransactionType);
                //FileLogger.Log("BataDAL", "Get4_3Data", param.FinishItemName);


                DataTable dtExpense = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dtExpense);


                transaction.Commit();

                return dtExpense;

            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        #region Toll Issue 6.4


        public ResultVM TollIssueDataGet(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();

            try
            {
                FileLogger.Log("BataIntegrationDAL", "TollIssueDataGet", "Start Data Get and Save");


                DataTable dtToll = new DataTable();

                #region Save Sale

                int count = GetCount(paramVM) + 1;

                int updatedRows = 0;
                int insertedRows = 0;

                for (int i = 0; i < count; i++)
                {

                    // get top 1000 distinct Id with line items
                    dtToll = GetTollissueData_Oracle(paramVM);

                    if (dtToll.Rows.Count == 0)
                    {
                        break;
                    }

                    insertedRows += dtToll.Rows.Count;

                    #region Bulk Insert to Source and Delete Duplicate

                    DBSQLConnection dbsqlConnection = new DBSQLConnection();

                    #region open connection and transaction

                    connection = _dbsqlConnection.GetConnection(connVM);

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    transaction = connection.BeginTransaction();

                    FileLogger.Log("BataIntegrationDAL", "TollIssueDataGet-1 Process_Step", "connection String - " + connection.ConnectionString);


                    #endregion open connection and transaction

                    CommonDAL commonDal = new CommonDAL();

                    #region temp Table

                    string tempTable = @"

create table #temp(
  PO_NO varchar(200)
, PO_DATE varchar(100)
, SUPL_CODE varchar(200)
, SUPP_NAME varchar(200)
, PLANT_NAME varchar(200)
, PRODUCT_NAME varchar(200)
, JOB_NAME varchar(200)
, GATE_PASS_NO varchar(200)
, GP_DATE varchar(100)
, ITEM_CODE varchar(200)
, ITEM_NAME varchar(200)
, UNIT varchar(200)
, QNTY decimal(18,9)
, PLANTCODE varchar(200)
, ISPROCESSED varchar(20)
)

";
                    #endregion

                    #region Delete Duplicat

                    string DeleteDuplicat = @"
delete from #temp where PO_NO in (select PO_NO from TempTollIssue6_4) 
and GATE_PASS_NO In(select GATE_PASS_NO from TempTollIssue6_4)
";

                    #endregion

                    #region Select

                    string getAll = @"
select   PO_NO
, PO_DATE
, SUPL_CODE
, SUPP_NAME
, PLANT_NAME
, PRODUCT_NAME
, JOB_NAME
, GATE_PASS_NO
, GP_DATE
, ITEM_CODE
, ITEM_NAME
, UNIT
, QNTY
, QNTY RestQNTY
, PLANTCODE
, ISPROCESSED
from #temp

drop table #temp 

";

                    #endregion


                    SqlCommand cmd = new SqlCommand(tempTable, connection, transaction);
                    cmd.ExecuteNonQuery();

                    sqlResults = commonDal.BulkInsert("#temp", dtToll, connection, transaction);

                    cmd = new SqlCommand(DeleteDuplicat, connection, transaction);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(getAll, connection, transaction);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dt);

                    sqlResults = commonDal.BulkInsert("TempTollIssue6_4", dt, connection, transaction);

                    transaction.Commit();
                    connection.Close();

                    #endregion

                    int rows = UpdateTollTopRows(paramVM);

                    updatedRows += rows;
                }

                #endregion

                #region RestQNTY Update for Bata

                BataIntegrationDAL bDal = new BataIntegrationDAL();

                bDal.UpdateRestQNTY(connVM);

                #endregion

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataIntegrationDAL", "TollIssueDataGet", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }

        public DataTable GetTollissueData_Oracle(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-1 Process_Step", "connection String - " + connection.ConnectionString);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();

                FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-2 - Process_Step", "Connection Opened");

                #endregion open connection and transaction

                ////string transactionType = GetTransactionType(param);

                FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-3 ", " PO_DATE : " + Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yy")+
                    " toDate : " + Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yy") + " GP_DATE : " + Convert.ToDateTime(param.GpDateFrom).ToString("dd-MMM-yy") +
                    " GP_DATE_To : " + Convert.ToDateTime(param.GpDateTo).ToString("dd-MMM-yy")
                    );


                #region Sql Text

                string deleteSaleIds = @"
delete from TOLLIDS

 ";
                //onlinevat.MUSHAK_63_DATA.isprocessed = 'N' to_date(substr(invoicedate,1,9),'dd-mm-rrrr')

                string insertToIdtable = @"
insert into TOLLIDS
select  PO_NO ID from (select distinct PO_NO from onlinevat.abu_materails_requisition
 where  onlinevat.abu_materails_requisition.isprocessed = 'N' and PO_DATE BETWEEN  :fromDate  AND   :toDate
and GP_DATE BETWEEN  :GPfromDate  AND   :GPtoDate
@condition)toll where ROWNUM <=2000";

                string selectRows = @"

select    
PO_NO
, PO_DATE
, SUPL_CODE
, SUPP_NAME
, PLANT_NAME
, PRODUCT_NAME
, JOB_NAME
, GATE_PASS_NO
, GP_DATE
, ITEM_CODE
, ITEM_NAME
, UNIT
, QNTY
, PLANTCODE
, ISPROCESSED     
 from onlinevat.abu_materails_requisition 
where PO_NO in(select ID from TOLLIDS)
";

                #endregion

                #region delete sale ids

                OracleCommand cmd = new OracleCommand(deleteSaleIds, connection);
                cmd.Transaction = transaction;

                int rows = cmd.ExecuteNonQuery();

                #endregion

                string sqlcondition = "";

                if (!string.IsNullOrWhiteSpace(param.GatePassNo))
                {
                    sqlcondition = @" and onlinevat.abu_materails_requisition.GATE_PASS_NO='" + param.GatePassNo + "'";
                }
                if (!string.IsNullOrWhiteSpace(param.RefNo))
                {
                    sqlcondition = @" and onlinevat.abu_materails_requisition.PO_NO='" + param.RefNo + "'";
                }

                #region inser to Sale Ids
                //'" + param.RefNo + "'"

                if (!string.IsNullOrWhiteSpace(sqlcondition))
                {
                    insertToIdtable = insertToIdtable.Replace("@condition", sqlcondition);
                }
                else
                {
                    insertToIdtable = insertToIdtable.Replace("@condition", "");
                }

                cmd.CommandText = insertToIdtable;

                if (!string.IsNullOrWhiteSpace(param.FromDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yy")));
                }

                if (!string.IsNullOrWhiteSpace(param.ToDate))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yy")));
                }

                if (!string.IsNullOrWhiteSpace(param.GpDateFrom))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("GPfromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yy")));
                }

                if (!string.IsNullOrWhiteSpace(param.GpDateTo))
                {
                    cmd.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("GPtoDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yy")));
                }

                rows = cmd.ExecuteNonQuery();

                #endregion

                cmd.CommandText = selectRows;

                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                adapter.Fill(table);

                ////FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-4 ", " deleteSaleIds Queary : " + deleteSaleIds);

                ////FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-5 ", " insertToIdtable Queary : " + insertToIdtable);

                ////FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-6 ", " selectRows Queary : " + selectRows);

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                ////FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle-7 ", " TollissueData_Oracle Total Get Data : " + table.Rows.Count.ToString());

                return table;

            }

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null)
                {
                    //transaction.Rollback();
                    transaction.Commit();

                }
                FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();

                        ////FileLogger.Log("BataIntegrationDAL", "GetTollissueData_Oracle(finally) ", "connection.Close()");

                    }

                }
            }
            #endregion
        }

        public int UpdateTollTopRows(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name


            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                #endregion open connection and transaction

                ////string transactionType = GetTransactionType(param);

                //and invoicedate >= @fromDate and invoicedate <= @toDate
                //invoicedate >= '01-Jan-2020' and invoicedate <= '31-Jan-2020'

                sqlText =
                    @" update onlinevat.abu_materails_requisition set IsProcessed = 'Y' where PO_NO in(select distinct ID from TOLLIDS)";

                #region Conditions

                #endregion

                Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(sqlText, connection);
                cmd.Transaction = transaction;

                #region Params

                #endregion

                int rows = cmd.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return rows;
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

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
            #endregion
        }

        public DataTable SelectAllTempData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string count = "100";
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

                #region Backup SqlText
                

////                sqlText += @"
////select   PO_NO
////, PO_DATE
////, GATE_PASS_NO
////, ITEM_CODE
////, ITEM_NAME
////, UNIT
////, QNTY
////, isnull(RestQNTY,QNTY) RestQNTY
////, SUPL_CODE
////, SUPP_NAME
////, PLANTCODE
////, PLANT_NAME
////, PRODUCT_NAME
////, JOB_NAME
////, GP_DATE
////--, ISPROCESSED
////from TempTollIssue6_4
////WHERE  1=1
////";
                #endregion

                sqlText = @"
select 
t.GATE_PASS_NO  
, GP_DATE
, ITEM_CODE
, ITEM_NAME
, UNIT
, QNTY TotalQuantity
, 0.00 TransactionQuantity
,isnull(v.ProcessQuantity,0)ProcessQuantity
,QNTY-isnull(v.ProcessQuantity,0)RestQNTY
, PO_NO
, PO_DATE
, SUPL_CODE
, SUPP_NAME
, PLANTCODE
, PLANT_NAME
, PRODUCT_NAME
, JOB_NAME


--, ISPROCESSED
from TempTollIssue6_4 t
left outer join (
select distinct ImportIDExcel GATE_PASS_NO,p.ProductCode,sum(d.quantity)ProcessQuantity from SalesInvoiceDetails d
left outer join SalesInvoiceHeaders h on d.SalesInvoiceNo=h.SalesInvoiceNo
left outer join Products p on d.ItemNo=p.ItemNo
where h.TransactionType='Tollissue'
group by ImportIDExcel,ProductCode) as v  on t.GATE_PASS_NO=v.GATE_PASS_NO and t.ITEM_CODE=v.ProductCode
WHERE  1=1

";

                #endregion SqlText

                #region parameter

                if (!string.IsNullOrWhiteSpace(paramVM.GatePassNo))
                {
                    sqlText += @" and t.GATE_PASS_NO=@GatePassNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText += @" and t.PO_NO=@RefNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText += @" and t.PO_DATE>=@FromDate";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText += @" and t.PO_DATE<=@ToDate";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateFrom))
                {
                    sqlText += @" and t.GP_DATE>=@GpDateFrom";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateTo))
                {
                    sqlText += @" and t.GP_DATE<=@GpDateTo";
                }

                #endregion

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (!string.IsNullOrWhiteSpace(paramVM.GatePassNo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GatePassNo", paramVM.GatePassNo);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@RefNo", paramVM.RefNo);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    da.SelectCommand.Parameters.AddWithValue("@FromDate", paramVM.FromDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    da.SelectCommand.Parameters.AddWithValue("@ToDate", paramVM.ToDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GpDateFrom", paramVM.GpDateFrom);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GpDateTo", paramVM.GpDateTo);
                }

                da.Fill(dt);

                FileLogger.Log("BataIntegrationDAL", " SelectAllTempData-2 ", " Data Count : " + dt.Rows.Count.ToString());


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
                FileLogger.Log("BataIntegrationDAL", "SelectAllTempData", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectTempData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string count = "100";
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

                sqlText += @"
select   
 GATE_PASS_NO ID
, PO_NO Reference_No
, SUPL_CODE Customer_Code
, SUPP_NAME Customer_Name
, cus.Address1 Delivery_Address
, JOB_NAME Comments
, GP_DATE Delivery_Date_Time
, GP_DATE Invoice_Date_Time
, ITEM_CODE Item_Code
, ITEM_NAME Item_Name
, UNIT UOM
, isnull(RestQNTY,QNTY) Quantity
, '-' Vehicle_No
,'New'Sale_Type
,'0'Previous_Invoice_No
, 'VAT 4.3' VAT_Name
, '' [Type]
, 0 NBR_Price
, 0 VAT_Rate
, 'N' Post
, 0 SD_Rate
,'BDT' Currency_Code
,0 SubTotal
from TempTollIssue6_4 tti
left outer join Customers cus on tti.SUPL_CODE=cus.CustomerCode
WHERE  1=1

";
                #endregion SqlText

                #region parameter

                if (!string.IsNullOrWhiteSpace(paramVM.GatePassNo))
                {
                    sqlText += @" and GATE_PASS_NO=@GatePassNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText += @" and PO_NO=@RefNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText += @" and PO_DATE>=@FromDate";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText += @" and PO_DATE<=@ToDate";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateFrom))
                {
                    sqlText += @" and GP_DATE>=@GpDateFrom";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateTo))
                {
                    sqlText += @" and GP_DATE<=@GpDateTo";
                }

                #endregion

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (!string.IsNullOrWhiteSpace(paramVM.GatePassNo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GatePassNo", paramVM.GatePassNo);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@RefNo", paramVM.RefNo);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    da.SelectCommand.Parameters.AddWithValue("@FromDate", paramVM.FromDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    da.SelectCommand.Parameters.AddWithValue("@ToDate", paramVM.ToDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GpDateFrom", paramVM.GpDateFrom);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.GpDateTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@GpDateTo", paramVM.GpDateTo);
                }

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
                FileLogger.Log("BataIntegrationDAL", "SelectAllTempData", ex.ToString() + "\n" + sqlText);
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

        public ResultVM SaveTollIssue(IntegrationParam paramVM,DataTable TollData, SysDBInfoVMTemp connVM = null, string GetAppVersion = "")
        {
            #region Variable
            
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            DataTable dtTableResult = new DataTable();
            var salesDal = new SaleDAL();
            ProductDAL productDal = new ProductDAL();
            CustomerDAL customerDAL = new CustomerDAL();

            string isExempted = "N";

            #endregion

            try
            {

                #region Filtering
                
                var Tollrow = TollData.Select("TransactionQuantity > 0");

                if (Tollrow.Length > 0)
                {
                    dtTableResult = Tollrow.CopyToDataTable();
                }

                #endregion

                #region Table rows Check

                if (dtTableResult.Rows.Count <= 0)
                {
                    throw new ArgumentNullException("", "There is no data to save");

                }

                #endregion

                #region RestQNTY Check

                foreach (DataRow row in dtTableResult.Rows)
                {
                    decimal TransactionQuantity = Convert.ToDecimal(row["TransactionQuantity"]);
                    decimal RestQNTY = Convert.ToDecimal(row["RestQNTY"]);
                    string pCode = row["ITEM_CODE"].ToString();

                    if (RestQNTY < TransactionQuantity)
                    {
                        throw new ArgumentNullException("", MessageVM.msgRestQnotavailable + " (Product Code : " + pCode+")");
                    }

                }

                #endregion

                #region Change Column Name

                dtTableResult.Columns["GATE_PASS_NO"].ColumnName = "ID";
                dtTableResult.Columns["PO_NO"].ColumnName = "Reference_No";
                dtTableResult.Columns["SUPL_CODE"].ColumnName = "Customer_Code";
                dtTableResult.Columns["SUPP_NAME"].ColumnName = "Customer_Name";
                dtTableResult.Columns["JOB_NAME"].ColumnName = "Comments";
                //dtTableResult.Columns["GP_DATE"].ColumnName = "Delivery_Date_Time";
                dtTableResult.Columns["GP_DATE"].ColumnName = "Invoice_Date_Time";
                dtTableResult.Columns["ITEM_CODE"].ColumnName = "Item_Code";
                dtTableResult.Columns["ITEM_NAME"].ColumnName = "Item_Name";
                dtTableResult.Columns["UNIT"].ColumnName = "UOM";
                dtTableResult.Columns["TransactionQuantity"].ColumnName = "Quantity";


                #endregion

                #region TableValidation

                TableValidation(dtTableResult, paramVM);

                #endregion

                #region Remove Columns

                List<string> deleteColumnList = new List<string>();

                deleteColumnList.Add("PO_DATE");
                deleteColumnList.Add("TotalQuantity");
                deleteColumnList.Add("ProcessQuantity");
                deleteColumnList.Add("RestQNTY");
                deleteColumnList.Add("PLANTCODE");
                deleteColumnList.Add("PLANT_NAME");
                deleteColumnList.Add("PRODUCT_NAME");
                deleteColumnList.Add("Tender_Id");                

                OrdinaryVATDesktop.DtDeleteColumns(dtTableResult, deleteColumnList);

                #endregion

                //dtTableResult = SelectTempData(paramVM);

                #region Update Vat Rate and Price

                var filterdData = new DataTable();

                foreach (DataColumn column in dtTableResult.Columns)
                {
                    filterdData.Columns.Add(column.ColumnName);
                }


                foreach (DataRow row in dtTableResult.Rows)
                {

                    var currentRows = filterdData.Select("Item_Code= '" + row["Item_Code"] +
                                                         "' and Customer_Code= '" + row["Customer_Code"] + "'");

                    if (currentRows.Length == 0)
                    {
                        var rows = dtTableResult.Select("Item_Code= '" + row["Item_Code"] +
                                                        "' and Customer_Code= '" + row["Customer_Code"] + "'");

                        decimal quantity = 0;
                        decimal subTotal = 0;
                        foreach (DataRow dataRow in rows)
                        {
                            quantity += Convert.ToDecimal(dataRow["Quantity"]);
                            subTotal += Convert.ToDecimal(dataRow["SubTotal"]);
                        }

                        row["Quantity"] = quantity;
                        row["SubTotal"] = subTotal;

                        var items = row.ItemArray;

                        filterdData.Rows.Add(items);
                    }

                }


                var salesData = filterdData;

                ProductDAL pDal = new ProductDAL();

                //string customerName = salesData.Rows[0]["Customer_Name"].ToString().Trim();
                ////string deliveryAddress = salesData.Rows[0]["Delivery_Address"].ToString().Trim('\r', '\n').Trim();
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                foreach (DataRow row in salesData.Rows)
                {

                    #region Date_Time
                    
                    string Invoice_Date_Time = row["Invoice_Date_Time"].ToString();
                    row["Delivery_Date_Time"] = Invoice_Date_Time;

                    row["Delivery_Date_Time"] = time;
                    row["Invoice_Date_Time"] = time;

                    #endregion

                    #region customer

                    string customerCode = salesData.Rows[0]["Customer_Code"].ToString().Trim();
                    CustomerVM customer = customerDAL.SelectAllList(null, new[] { "CustomerCode" }, new[] { customerCode }, null, null, connVM).FirstOrDefault();

                    if (customer != null)
                    {
                        row["Delivery_Address"] = customer.Address1;
                    }
                    
                    #endregion

                    #region Product
                    
                    List<ProductVM> vms = pDal.SelectAll("0", new[] { "ProductCode" }, new[] { row["Item_Code"].ToString() }, null, null, null, connVM);

                    string itemNo = "";

                    if (vms != null && vms.Any())
                    {
                        var vm = vms.FirstOrDefault();
                        itemNo = vm.ItemNo;

                        decimal sd = Convert.ToDecimal(string.Format("{0:0.0000}", vm.SD));
                        decimal vRate = Convert.ToDecimal(string.Format("{0:0.0000}", vm.VATRate));

                        decimal subtotal = Convert.ToDecimal(string.Format("{0:0.0000}", row["SubTotal"]));
                        decimal quantity = Convert.ToDecimal(string.Format("{0:0.0000}", row["Quantity"]));

                        row["VAT_Rate"] = vm.VATRate;
                        row["UOM"] = vm.UOM;
                        row["SD_Rate"] = vm.SD;

                        if (vm.VATRate == 15)
                        {
                            row["Type"] = "VAT";
                        }

                        if (row["VAT_Rate"].ToString() == "0")
                        {
                            row["Type"] = "NonVAT";

                        }

                        if (vm.VATRate != 15 && vm.VATRate != 0)
                        {
                            row["Type"] = "OtherRate";
                        }


                        if (string.IsNullOrEmpty(row["Type"].ToString()))
                        {
                            row["Type"] = "NonVAT";
                        }

                    }
                    else
                    {
                        throw new Exception("This  " + row["Item_Name"].ToString().Trim() + " ('" + row["Item_Code"].ToString().Trim() + "') Product Not Found in Vat System Please Add Product");
                    }

                    #endregion

                    #region Avg Price
                    
                    DataTable priceData = productDal.AvgPriceNew(itemNo, Convert.ToDateTime(Convert.ToDateTime(row["Invoice_Date_Time"])).ToString("yyyy-MM-dd") +
                        DateTime.Now.ToString(" HH:mm:00"), null, null, true, true, true, false, null, paramVM.CurrentUser);

                    decimal Price = 0;
                    if (priceData != null && priceData.Rows.Count != 0)
                    {
                        Price = Convert.ToDecimal(priceData.Rows[0]["AvgRate"].ToString());
                    }

                    if (Price <= 0)
                    {
                        Price = 0001;
                    }

                    row["NBR_Price"] = Price;

                    decimal qty = Convert.ToDecimal(string.Format("{0:0.0000}", row["Quantity"]));

                    row["SubTotal"] = qty * Price;

                    #endregion

                }

                #endregion

                ////TableValidation(salesData, paramVM);

                #region SaveAndProcessIntegration
                
                sqlResults = salesDal.SaveAndProcessIntegration(salesData, () => { }, Convert.ToInt32(paramVM.BranchId), GetAppVersion, connVM, paramVM.CurrentUser);
               
                #endregion

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";

            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataIntegrationDAL", "TollIssueDataGet", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }


        public ResultVM SaveTollIssueData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null, string GetAppVersion="")
        {
            ResultVM rVM = new ResultVM();
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            DataTable dtTableResult = new DataTable();
            var salesDal = new SaleDAL();
            ProductDAL productDal = new ProductDAL();

            string isExempted = "N";

            try
            {


                dtTableResult = SelectTempData(paramVM);


                #region Update Vat Rate and Price

                var filterdData = new DataTable();

                foreach (DataColumn column in dtTableResult.Columns)
                {
                    filterdData.Columns.Add(column.ColumnName);
                }


                foreach (DataRow row in dtTableResult.Rows)
                {

                    var currentRows = filterdData.Select("Item_Code= '" + row["Item_Code"] +
                                                         "' and Customer_Code= '" + row["Customer_Code"] + "'");

                    if (currentRows.Length == 0)
                    {
                        var rows = dtTableResult.Select("Item_Code= '" + row["Item_Code"] +
                                                        "' and Customer_Code= '" + row["Customer_Code"] + "'");

                        decimal quantity = 0;
                        decimal subTotal = 0;
                        foreach (DataRow dataRow in rows)
                        {
                            quantity += Convert.ToDecimal(dataRow["Quantity"]);
                            subTotal += Convert.ToDecimal(dataRow["SubTotal"]);
                        }

                        row["Quantity"] = quantity;
                        row["SubTotal"] = subTotal;

                        var items = row.ItemArray;

                        filterdData.Rows.Add(items);
                    }

                }


                var salesData = filterdData;

                ProductDAL pDal = new ProductDAL();

                string customerCode = salesData.Rows[0]["Customer_Code"].ToString().Trim();
                string customerName = salesData.Rows[0]["Customer_Name"].ToString().Trim();
                string deliveryAddress = salesData.Rows[0]["Delivery_Address"].ToString().Trim('\r', '\n').Trim();
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                CustomerDAL customerDAL = new CustomerDAL();

                CustomerVM customer = customerDAL.SelectAllList(null, new[] { "CustomerCode" }, new[] { customerCode }, null, null, connVM).FirstOrDefault();

                foreach (DataRow row in salesData.Rows)
                {
                    List<ProductVM> vms = pDal.SelectAll("0", new[] { "ProductCode" }, new[] { row["Item_Code"].ToString() }, null, null, null, connVM);

                    row["Delivery_Date_Time"] = time;
                    row["Invoice_Date_Time"] = time;

                    string itemNo = "";

                    if (vms != null && vms.Any())
                    {
                        var vm = vms.FirstOrDefault();
                        itemNo = vm.ItemNo;

                        decimal sd = Convert.ToDecimal(string.Format("{0:0.0000}", vm.SD));
                        decimal vRate = Convert.ToDecimal(string.Format("{0:0.0000}", vm.VATRate));

                        decimal subtotal = Convert.ToDecimal(string.Format("{0:0.0000}", row["SubTotal"]));
                        decimal quantity = Convert.ToDecimal(string.Format("{0:0.0000}", row["Quantity"]));

                        row["VAT_Rate"] = vm.VATRate;
                        row["UOM"] = vm.UOM;
                        row["SD_Rate"] = vm.SD;
                        
                        if (vm.VATRate == 15)
                        {
                            row["Type"] = "VAT";
                        }

                        if (row["VAT_Rate"].ToString() == "0")
                        {
                            row["Type"] = "NonVAT";

                        }

                        if (vm.VATRate != 15 && vm.VATRate != 0)
                        {
                            row["Type"] = "OtherRate";
                        }


                        if (string.IsNullOrEmpty(row["Type"].ToString()))
                        {
                            row["Type"] = "NonVAT";
                        }

                    }

                    DataTable priceData = productDal.AvgPriceNew(itemNo, Convert.ToDateTime(Convert.ToDateTime(row["Invoice_Date_Time"])).ToString("yyyy-MM-dd") +
                        DateTime.Now.ToString(" HH:mm:00"), null, null, true, true, true, false, null, paramVM.CurrentUser);

                    decimal Price = 0;
                    if (priceData != null && priceData.Rows.Count != 0)
                    {
                        Price = Convert.ToDecimal(priceData.Rows[0]["AvgRate"].ToString());
                    }

                    row["NBR_Price"] = Price;

                    row["Delivery_Address"] = deliveryAddress;
                    row["Customer_Name"] = customerName;

                    decimal qty = Convert.ToDecimal(string.Format("{0:0.0000}", row["Quantity"]));

                    row["SubTotal"] = qty * Price;

                    ////if (customerCode != row["Customer_Code"].ToString().Trim())
                    ////{
                    ////    throw new Exception("Multiple customer can not be add to a single invoice");
                    ////}

                }

                #endregion


                TableValidation(salesData, paramVM);

                sqlResults = salesDal.SaveAndProcessIntegration(salesData, () => { }, Convert.ToInt32(paramVM.BranchId), GetAppVersion, connVM, paramVM.CurrentUser);

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";

            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataIntegrationDAL", "TollIssueDataGet", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return rVM;
        }

        private void TableValidation(DataTable salesData, IntegrationParam paramVM)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = paramVM.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = paramVM.CurrentUserName };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = paramVM.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };
            DataColumn userId = new DataColumn("UserId") { DefaultValue = paramVM.CurrentUser };

            DataColumn Vehicle_No = new DataColumn("Vehicle_No") { DefaultValue = "-" };
            DataColumn Sale_Type = new DataColumn("Sale_Type") { DefaultValue = "New" };
            DataColumn Previous_Invoice_No = new DataColumn("Previous_Invoice_No") { DefaultValue = "0" };
            DataColumn VAT_Name = new DataColumn("VAT_Name") { DefaultValue = "VAT 4.3" };
            DataColumn Type = new DataColumn("Type") { DefaultValue = "" };
            DataColumn NBR_Price = new DataColumn("NBR_Price") { DefaultValue = "0" };
            DataColumn VAT_Rate = new DataColumn("VAT_Rate") { DefaultValue = "0" };
            DataColumn Post = new DataColumn("Post") { DefaultValue = "N" };
            DataColumn SD_Rate = new DataColumn("SD_Rate") { DefaultValue = "0" };
            DataColumn Currency_Code = new DataColumn("Currency_Code") { DefaultValue = "BDT" };
            DataColumn SubTotal = new DataColumn("SubTotal") { DefaultValue = "0" };
            DataColumn Delivery_Date_Time = new DataColumn("Delivery_Date_Time") { DefaultValue = "" };
            DataColumn Delivery_Address = new DataColumn("Delivery_Address") { DefaultValue = "" };
            DataColumn BENumber = new DataColumn("BENumber") { DefaultValue = "" };


            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);
            salesData.Columns.Add(userId);

            salesData.Columns.Add(Vehicle_No);
            salesData.Columns.Add(Sale_Type);
            salesData.Columns.Add(Previous_Invoice_No);
            salesData.Columns.Add(VAT_Name);
            salesData.Columns.Add(Type);
            salesData.Columns.Add(NBR_Price);
            salesData.Columns.Add(VAT_Rate);
            salesData.Columns.Add(Post);
            salesData.Columns.Add(SD_Rate);
            salesData.Columns.Add(Currency_Code);
            salesData.Columns.Add(SubTotal);
            salesData.Columns.Add(Delivery_Date_Time);
            salesData.Columns.Add(Delivery_Address);
            salesData.Columns.Add(BENumber);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);
        }

        public ResultVM UpdateRestQNTY(SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();

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

                CommonDAL commonDal = new CommonDAL();

                #region temp Table

                string sqlText = @"
create table #tempToll(
  ImportIDExcel varchar(200)
, ProductCode varchar(100)
, Quantity decimal(18,9)

)
insert into #tempToll 
select sh.ImportIDExcel,p.ProductCode,sum(sd.Quantity)Quantity from SalesInvoiceHeaders sh
left outer join SalesInvoiceDetails sd on sd.SalesInvoiceNo=sh.SalesInvoiceNo 
left outer join Products p on p.ItemNo=sd.ItemNo 
where 1=1 and  sh.ImportIDExcel!=''
and sh.TransactionType='TollIssue'
group by
sh.ImportIDExcel
,p.ProductCode

--select * from #tempToll

update TempTollIssue6_4 set RestQNTY= QNTY- (#tempToll.Quantity) from #tempToll where 1=1 
and ITEM_CODE = #tempToll.ProductCode  and GATE_PASS_NO = #tempToll.ImportIDExcel

drop table #tempToll 

";
                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                rVM.Status = "Success";
                rVM.Message = "Data Saved Successfully";
            }

            #endregion

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("BataIntegrationDAL", "UpdateRestQNTY", ex.Message + "\n" + ex.StackTrace);

            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            return rVM;
        }

        public DataTable SelectAllGatePassNo_Oracle(SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            int count = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = count.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            Oracle.DataAccess.Client.OracleConnection connection = null;
            Oracle.DataAccess.Client.OracleTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                connection = _dbsqlConnection.GetBataConnection();

                FileLogger.Log("BataIntegrationDAL", "SelectAllGatePassNo_Oracle Process_Step-1", "connection String - " + connection.ConnectionString);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                transaction = connection.BeginTransaction();

                FileLogger.Log("BataIntegrationDAL", "SelectAllGatePassNo_Oracle - Process_Step-2", "Connection Opened");

                #endregion open connection and transaction

                #region Sql Text

                string selectRows = @"

select distinct GATE_PASS_NO, GP_DATE,PO_NO,PO_DATE from onlinevat.abu_materails_requisition
";

                #endregion

                OracleCommand cmd = new OracleCommand(selectRows, connection);
                cmd.Transaction = transaction;

                DataTable table = new DataTable();

                Oracle.DataAccess.Client.OracleDataAdapter adapter = new OracleDataAdapter(cmd);
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
                if (transaction != null)
                {
                    //transaction.Rollback();
                    transaction.Commit();

                }
                FileLogger.Log("BataIntegrationDAL", "SelectAllGatePassNo_Oracle", ex.ToString());

                throw ex;
            }
            finally
            {

                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();

                    }

                }
            }
            #endregion

        }



        #endregion


    }
}
