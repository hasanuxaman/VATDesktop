using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using System.IO;
using Excel;
using System.Globalization;
using VATServer.Interface;
using Newtonsoft.Json;
using VATViewModel;

namespace VATServer.Library.Integration
{
    public class UNILEVERIntegrationDAL
    {
        #region UNILEVER

        private string[] sqlResults;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDAL = new CommonDAL();
        SaleDAL _SaleDAL = new SaleDAL();

        public ResultVM SaveUnileverSale_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            DataTable dtSale = new DataTable();
            CommonDAL commonDal = new CommonDAL();

            #region try

            try
            {

                string code = commonDal.settingValue("CompanyCode", "Code", paramVM.SysDbInfoVmTemp);
                if (paramVM.TransactionType.ToLower() == "other")
                {
                    dtSale = GetSource_SaleData_Details(paramVM, connVM);
                }
                else if (paramVM.TransactionType.ToLower() == "credit")
                {
                    dtSale = GetSource_SaleCNData_Details(paramVM, connVM);
                }
                else
                {
                    dtSale = GetSource_SaleDNData_Details(paramVM, connVM);
                }

                sqlResults = SaveSaleUnilever(dtSale, paramVM.DefaultBranchId, paramVM.TransactionType, paramVM.Token, paramVM.SysDbInfoVmTemp, paramVM);

                if (sqlResults[0].ToLower() == "success")
                {
                    if (paramVM.TransactionType.ToLower() == "other")
                    {
                        UpdateTransactions(dtSale, paramVM.dtConnectionInfo, "SaleInvoices", paramVM.TransactionType, connVM);
                    }

                    else
                    {
                        UpdateTransactions(dtSale, paramVM.dtConnectionInfo, "SaleInvoiceReturns", paramVM.TransactionType, connVM);
                    }
                }
               
                rVM.Status = sqlResults[0];
                rVM.Message = "Saved Successfully";

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("UNILEVERIntegrationDAL", "SaveUnileverSale_Web", ex.ToString());
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

        public DataTable GetSource_SaleData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {
                 CommonDAL commonDAL = new CommonDAL();

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

               //#region  Update Middleware
               // sqltext = " update SaleInvoices set ChallanNo=SaleCode  where ChallanNo=''";
               // var cmdUpdate = new SqlCommand(sqltext, currConn);
               // var rows = cmdUpdate.ExecuteNonQuery();
               //#endregion

                #region SQLText
                

                sqlText = @"
             SELECT Distinct 
              SSO
             ,Section
             ,CONVERT(varchar,InvoiceDate,111) as [Delivery Date]
             ,CASE
                 WHEN IsProcessed=0 THEN 'UnSaved'
                 WHEN IsProcessed=1 THEN 'Saved'
                 END AS Status
             ,ChallanNo
             ,Count(distinct SaleCode)[Memo Count]
             FROM [SaleInvoices]
             WHERE 1=1 and CompanyCode=@CompanyCode

            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    sqlText = sqlText + @" AND IsProcessed =@Processed";
                }
                
                #endregion


               

                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
ChallanNo
,SSO
,Section
,InvoiceDate
,IsProcessed
";

                #endregion

                sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                #region Add Parameter Values

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
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed=="Y" ? true:false);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public string[] SaveSaleUnilever(DataTable dtSaleM, int BranchId, string transactionType, string token, SysDBInfoVMTemp connVM = null, IntegrationParam paramVM = null, string UserId = "")
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
            retResults[5] = "ImportExcelFile"; //Method Name
            #endregion

            #region try
            try
            {
                DataTable dtSalesData = dtSaleM.Copy();

                TableValidation(dtSalesData, new IntegrationParam() { TransactionType = transactionType, BranchCode = paramVM.BranchCode, CurrentUser = paramVM.CurrentUser });
                dtSalesData.Columns.Add(new DataColumn() { ColumnName = "token", DefaultValue = token });
                retResults = new SaleDAL().SaveAndProcess(dtSalesData, () => { }, BranchId, String.Empty, paramVM.SysDbInfoVmTemp, paramVM, null, null, UserId);

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                FileLogger.Log("UNILEVERIntegrationDAL", "SaveSaleUnilever", ex.ToString() + "\n" + sqlText);
                throw ex;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public DataTable GetSource_SaleCNData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();
            CommonDAL commonDAL = new CommonDAL();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText


                sqlText = @"
             SELECT Distinct 
              SSO
             ,Section
             ,CONVERT(varchar,InvoiceDate,111) as [Delivery Date]
             ,CASE
                 WHEN IsProcessed=0 THEN 'UnSaved'
                 WHEN IsProcessed=1 THEN 'Saved'
                 END AS Status
             ,ChallanNo
             ,Count(distinct SaleCode)[Memo Count]
             FROM [SaleInvoiceReturns]
             WHERE 1=1 and CompanyCode=@CompanyCode
            and Quantity<0
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    sqlText = sqlText + @" AND IsProcessed = @Processed";
                }

                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
ChallanNo
,SSO
,Section
,InvoiceDate
,IsProcessed
";

                #endregion

                sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                #region Add Parameter Values

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
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed == "Y" ? true : false);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_SaleDNData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();
            CommonDAL commonDAL = new CommonDAL();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText


                sqlText = @"
             SELECT Distinct 
              SSO
             ,Section
             ,CONVERT(varchar,InvoiceDate,111) as [Delivery Date]
             ,CASE
                 WHEN IsProcessed=0 THEN 'UnSaved'
                 WHEN IsProcessed=1 THEN 'Saved'
                 END AS Status
             ,ChallanNo
             ,Count(distinct SaleCode)[Memo Count]
             FROM [SaleInvoiceReturns]
             WHERE 1=1 and CompanyCode=@CompanyCode
             and Quantity>0
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate < dateadd(d,1,@toDate)";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    sqlText = sqlText + @" AND IsProcessed = @Processed";
                }
                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
ChallanNo
,SSO
,Section
,InvoiceDate
,IsProcessed
";

                #endregion

                sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                #region Add Parameter Values

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
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed == "Y" ? true : false);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }



        public DataTable GetSource_SaleData_Details(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();
            CommonDAL commonDal = new CommonDAL();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                string code = commonDal.settingValue("CompanyCode", "Code", connVM);

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"    Select 
      [SaleCode] [ID]
      ,SSO
      ,Section
      ,ChallanNo
      ,BatchNo
      ,[CustomerName] [Customer_Name]
      ,[CustomerCode] [Customer_Code]
      ,[DeliveryAddress] [Delivery_Address] 
	  ,CONVERT(varchar,InvoiceDate,111)+' '+ CONVERT(VARCHAR,getdate(),108) [Invoice_Date_Time]
      ,[SaleCode] [Reference_No]
      ,[ProductCode] [Item_Code]
      ,[ProductName] [Item_Name]
      ,case when ISNULL(VATRate, 0) = 15 then 'VAT'
	   when ISNULL(VATRate, 0) = 15 then 'NonVAT'
	   else 'Other'	end [Type]
      ,[Quantity]
      ,[QuantityCtn] [Weight]
      ,'PC'[UOM]
	  ,ProductType
	  ,ProductBanglaName
      ,round((UnitNBRPrice/((100+VATRate)/100)),5)[NBR_Price]
      ---,[UnitNBRPrice] [UnitNBRPriceWithVAT]
      ,0 [SubTotal]
--    ,round((DiscountAmount/((100+VATRate)/100)),2)[Discount_Amount]
      ,round(DiscountAmount,2)[Discount_Amount]
      ,InvoiceDiscountAmount
      --,[DiscountAmount] [Discount_Amount]
      ,[FreeQuantity] [Promotional_Quantity]
      ,[VATRate] [VAT_Rate]
      ,[SDRate] [SD_Rate]
      ,'Y'[Post]
      ,TradeSkimComment [Comments]
      ,'N' [Non_Stock]
      ,'0' [Trading_MarkUp]
      ,'VAT 4.3' [VAT_Name]
      ,'BDT' [Currency_Code]
      ,'NEW' [Sale_Type]
      ,'' [Previous_Invoice_No]
      ,'N' [Is_Print]
      ,'0' [Tender_Id]
       FROM [SaleInvoices]
       WHERE 1=1 and CompanyCode=@CompanyCode
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }


                #endregion




                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                cmd.Parameters.AddWithValue("@CompanyCode", code);


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }
               


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_SaleCNData_Details(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM);

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"  Select 
      [SaleCode]+'-C' [ID]
      ,SSO
      ,Section
      ,ChallanNo
      ,BatchNo
      ,[CustomerName] [Customer_Name]
      ,[CustomerCode] [Customer_Code]
      ,[DeliveryAddress] [Delivery_Address] 
	  ,CONVERT(varchar,InvoiceDate,111)+' '+ CONVERT(VARCHAR,getdate(),108) [Invoice_Date_Time]
      ,[SaleCode]+'-C' [Reference_No]
      ,[ProductCode] [Item_Code]
      ,[ProductName] [Item_Name]
      ,'VAT'[Type]
      ,sum(Quantity*-1) Quantity
      ,[QuantityCtn][Weight]
      ,'PC'[UOM]
	  ,ProductType
      ,round((UnitNBRPrice/((100+VATRate)/100)),5)[NBR_Price]
      ---,[UnitNBRPrice] [UnitNBRPriceWithVAT]
      ,0 [SubTotal]
      ,round((DiscountAmount/((100+VATRate)/100)),2)[Discount_Amount]
      --,[DiscountAmount] [Discount_Amount]
      ,[FreeQuantity] [Promotional_Quantity]
      ,[VATRate] [VAT_Rate]
      ,[SDRate] [SD_Rate]
      ,'Y'[Post]
      ,[Comments]
      ,'N' [Non_Stock]
      ,'0' [Trading_MarkUp]
      ,'VAT 4.3' [VAT_Name]
      ,'BDT' [Currency_Code]
      ,'Credit' [Sale_Type]
      ,[SaleCode][Previous_Invoice_No]
      ,'N' [Is_Print]
      ,'0' [Tender_Id]
       FROM [SaleInvoiceReturns]
       WHERE 1=1 and CompanyCode=@CompanyCode
	   and [Quantity]<0
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }


                #endregion


                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
 [SaleCode]
      ,SSO
      ,Section
      ,ChallanNo
      ,BatchNo
      ,[CustomerName]
      ,[CustomerCode]
      ,[DeliveryAddress] 
	  ,InvoiceDate
      ,[ProductCode] 
      ,[ProductName] 
      ,[UOM]
	  ,ProductType
      ,round((UnitNBRPrice/((100+VATRate)/100)),5)
      ---,[UnitNBRPrice] [UnitNBRPriceWithVAT]
      ,[VATRate]
      ,round((DiscountAmount/((100+VATRate)/100)),2)
      --,[DiscountAmount] [Discount_Amount]
      ,[FreeQuantity] 
      ,[SDRate] 
      ,[Comments]
      ,[PreviousSalesInvoiceNo]
      ,[QuantityCtn]

";

                #endregion

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_SaleDNData_Details(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            string code = commonDal.settingValue("CompanyCode", "Code", connVM);
            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"  Select 
      [SaleCode]+'-D' [ID]
      ,SSO
      ,Section
      ,ChallanNo
      ,BatchNo
      ,[CustomerName] [Customer_Name]
      ,[CustomerCode] [Customer_Code]
      ,[DeliveryAddress] [Delivery_Address] 
	  ,CONVERT(varchar,InvoiceDate,111)+' '+ CONVERT(VARCHAR,getdate(),108) [Invoice_Date_Time]
      ,[SaleCode]+'-D' [Reference_No]
      ,[ProductCode] [Item_Code]
      ,[ProductName] [Item_Name]
      ,'VAT'[Type]
      , Sum(Quantity)Quantity
      ,[QuantityCtn] [Weight]
      ,'PC'[UOM]
	  ,ProductType
      ,round((UnitNBRPrice/((100+VATRate)/100)),5)[NBR_Price]
      ---,[UnitNBRPrice] [UnitNBRPriceWithVAT]
      ,0 [SubTotal]
      ,round((DiscountAmount/((100+VATRate)/100)),2)[Discount_Amount]
      --,[DiscountAmount] [Discount_Amount]
      ,[FreeQuantity] [Promotional_Quantity]
      ,[VATRate] [VAT_Rate]
      ,[SDRate] [SD_Rate]
      ,'Y'[Post]
      ,[Comments]
      ,'N' [Non_Stock]
      ,'0' [Trading_MarkUp]
      ,'VAT 4.3' [VAT_Name]
      ,'BDT' [Currency_Code]
      ,'Debit' [Sale_Type]
      ,[SaleCode][Previous_Invoice_No]
      ,'N' [Is_Print]
      ,'0' [Tender_Id]
       FROM [SaleInvoiceReturns]
       WHERE 1=1 and CompanyCode=@CompanyCode
	   and [Quantity]>0
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }


                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
 [SaleCode]
      ,SSO
      ,Section
      ,ChallanNo
      ,BatchNo
      ,[CustomerName]
      ,[CustomerCode]
      ,[DeliveryAddress] 
	  ,InvoiceDate
      ,[ProductCode] 
      ,[ProductName] 
      ,[UOM]
	  ,ProductType
      ,round((UnitNBRPrice/((100+VATRate)/100)),5)
      ---,[UnitNBRPrice] [UnitNBRPriceWithVAT]
      ,[VATRate]
      ,round((DiscountAmount/((100+VATRate)/100)),2)
      --,[DiscountAmount] [Discount_Amount]
      ,[FreeQuantity] 
      ,[SDRate] 
      ,[Comments]
      ,[PreviousSalesInvoiceNo]
      ,[QuantityCtn]
";

                #endregion


                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }




        public DataTable GetSource_SaleData_dis_Details(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"    Select  distinct 
0 Selected
,sh.Id
,sh.SalesInvoiceNo
,sh.TransactionType
,c.CustomerName [Outlet Name]
,c.CustomerCode
,sh.IsPrint
,sh.SerialNo [Memo No]
,Sum(sid.Quantity)[Total Quantity]
,Sum(Round ((sid.SubTotal+sid.VATAmount)-DiscountAmount,0))[Memo Value]
,sh.InvoiceDateTime,count(sid.itemNo)SKUCount
from SalesInvoiceHeaders sh 

left outer join SalesInvoiceDetails sid  on sh.SalesInvoiceNo = sid.SalesInvoiceNo
left outer join Customers c  on sh.CustomerID = c.CustomerID
       WHERE 1=1
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.TransactionType))
                {
                    sqlText = sqlText + @" AND sh.TransactionType = @TransactionType";
                }

                #endregion




                #endregion


                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
sh.SerialNo
,sh.TransactionType
,sh.Id
,c.CustomerName
,c.CustomerCode
,sh.IsPrint
,sh.SerialNo
,sh.SalesInvoiceNo
,sh.InvoiceDateTime
order by sh.SalesInvoiceNo
";

                #endregion


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@TransactionType", paramVM.TransactionType);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_SaleDataReturn_dis_Details(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"    Select 
0 Selected
,sh.Id
,sh.TransactionType
, sh.SalesInvoiceNo
,c.CustomerName [Outlet Name]
,c.CustomerCode,sh.IsPrint
,sh.SerialNo [Memo No]
,Sum(sid.Quantity)[Total Quantity]
,Sum(sid.SubTotal)[Memo Value]
,sh.InvoiceDateTime
,count(sid.itemNo)SKUCount
from SalesInvoiceHeaders sh 

left outer join SalesInvoiceDetails sid  on sh.SalesInvoiceNo = sid.SalesInvoiceNo
left outer join Customers c  on sh.CustomerID = c.CustomerID
       WHERE 1=1
            ";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ChallanNo = @SalesInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.TransactionType))
                {
                    sqlText = sqlText + @" AND sh.TransactionType = @TransactionType";
                }


                #endregion




                #endregion


                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
sh.SerialNo
,sh.TransactionType
,sh.Id
,c.CustomerName
,c.CustomerCode
,sh.IsPrint
,sh.SerialNo
,sh.SalesInvoiceNo
,sh.InvoiceDateTime
order by sh.SalesInvoiceNo
";

                #endregion


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.TransactionType))
                {
                    cmd.Parameters.AddWithValue("@TransactionType", paramVM.TransactionType);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }


        public DataTable GetSource_DistinctPurchaseData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchaseMaster = new DataTable();
            CommonDAL commonDAL = new CommonDAL();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText


                sqlText = @"
       Select Distinct
       [ReferanceNo] [ID]
      ,[VendorName] [Vendor_Name]
      ,[VendorCode] [Vendor_Code]
	  ,[InvoiceDate] [Invoice_Date]
      ,[ReceiveDate] [Receive_Date]
      ,[PurchaseCode] [Referance_No]
      ,[InvoiceNumber] [BE_Number]
      ,Sum([Quantity])[Quantity]
      ,[UOM]
     
 ,CASE
                 WHEN IsProcessed=0 THEN 'UnSaved'
                 WHEN IsProcessed=1 THEN 'Saved'
                 END AS Status
      ,'VAT'[Type]
      ,Sum(TotalPrice-(TotalPrice*15/115)) [Total_Price]
      ,Sum([SDAmount]) [SD_Amount]
      --,Sum(VATAmount-(VATAmount*15/115)) [VAT_Amount]
      ,Sum(TotalPrice*15/115) [VAT_Amount]
      ,'Y'[Post]
      ,'-'[Comments]
      ,'N'[With_VDS]
      ,'0' [Rebate_Rate]

  FROM [PurchaseInvoices]
   WHERE 1=1 and CompanyCode=@CompanyCode

";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ReferanceNo = @PurchaseInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    sqlText = sqlText + @" AND IsProcessed = @IsProcessed";
                }
               

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 

  group by [ReferanceNo]
      ,[PurchaseCode] 
      ,[VendorName] 
      ,[VendorCode] 
	  ,[InvoiceDate] 
      ,[ReceiveDate] 
      ,[PurchaseCode] 
      ,[InvoiceNumber] 
      ,[UOM]
	  ,[IsProcessed]
";

                #endregion

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);
                cmd.Parameters.AddWithValue("@IsProcessed", paramVM.Processed == "Y" ? true : false);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@PurchaseInvoiceNo", paramVM.RefNo);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchaseMaster);

                #endregion
                //ProductDAL dal = new ProductDAL();

                //foreach (DataRow tableRow in dtPurchaseMaster.Rows)
                //{
                //    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                //    ProductVM vm = vms.FirstOrDefault();

                //    if (vm == null)
                //    {
                //        continue;
                //    }

                //    decimal vatRate = 15;
                //    tableRow["Total_Price"] = (Convert.ToDecimal(tableRow["Quantity"])) * vm.NBRPrice;
                //    tableRow["VAT_Amount"] = (Convert.ToDecimal(tableRow["Total_Price"])) * vatRate / 100;

                //}


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

            return dtPurchaseMaster;

        }
        public DataTable GetSource_PurchaseDetailsData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtPurchaseMaster = new DataTable();
            CommonDAL commonDAL = new CommonDAL();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                #region SQLText
                

                sqlText = @"
       Select
       [ReferanceNo] [ID]
      ,[VendorName] [Vendor_Name]
      ,[VendorCode] [Vendor_Code]
	  ,[InvoiceDate] [Invoice_Date]
      ,[ReceiveDate] [Receive_Date]
      ,[PurchaseCode] [Referance_No]
      ,[InvoiceNumber] [BE_Number]
      ,[ProductCode] [Item_Code]
      ,[ProductName] [Item_Name]
      ,[Quantity]
      ,[UOM]
      , case when TotalPrice = 0 then 'P' else 'R' end   ProductType
 ,CASE
                 WHEN IsProcessed=0 THEN 'UnSaved'
                 WHEN IsProcessed=1 THEN 'Saved'
                 END AS Status
      ,'VAT'[Type]
      ,TotalPrice-(TotalPrice*15/115) [Total_Price]
      ,[SDAmount] [SD_Amount]
      ,(TotalPrice*15/115) [VAT_Amount]
      --,VATAmount-(VATAmount*15/115) [VAT_Amount]
      ,'Y'[Post]
      ,'-'[Comments]
      ,'N'[With_VDS]
      ,'0' [Rebate_Rate]

  FROM [PurchaseInvoices]
   WHERE 1=1 and CompanyCode=@CompanyCode

";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ReferanceNo = @PurchaseInvoiceNo";
                }


                #endregion


               
                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@PurchaseInvoiceNo", paramVM.RefNo);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchaseMaster);

                #endregion
                //ProductDAL dal = new ProductDAL();

                //foreach (DataRow tableRow in dtPurchaseMaster.Rows)
                //{
                //    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                //    ProductVM vm = vms.FirstOrDefault();

                //    if (vm == null)
                //    {
                //        continue;
                //    }

                //    decimal vatRate = 15;
                //    tableRow["Total_Price"] = (Convert.ToDecimal(tableRow["Quantity"])) * vm.NBRPrice;
                //    tableRow["VAT_Amount"] = (Convert.ToDecimal(tableRow["Total_Price"])) * vatRate / 100;

                //}


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

            return dtPurchaseMaster;

        }

        public ResultVM SaveUnileverPurchase_Web(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            CommonDAL commonDal = new CommonDAL();
            #region try

            try
            {

                DataTable dtPurchase = new DataTable();
               
                string code = commonDal.settingValue("CompanyCode", "Code", paramVM.SysDbInfoVmTemp);
                if(paramVM.TransactionType.ToLower()=="other")
                {
                    dtPurchase = GetSource_PurchaseDetailsData_Master(paramVM);
                }
                else
                {
                    dtPurchase = GetSource_PurchaseReturnDetailsData_Master(paramVM);
                }

                if (dtPurchase == null || dtPurchase.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }
                if (dtPurchase.Columns.Contains("Status"))
                {
                    dtPurchase.Columns.Remove("Status");

                }

                PurchaseDAL purchaseDal = new PurchaseDAL();
                sqlResults = purchaseDal.SaveTempPurchase(dtPurchase, paramVM.BranchCode, paramVM.TransactionType,
                    paramVM.CurrentUser,
                    0, () => { });


                if (sqlResults[0].ToLower() == "success")
                {
                    string[] results;
                    if (paramVM.TransactionType.ToLower() == "other")
                    {
                        results = new ImportDAL().UpdateUBLTransactions(dtPurchase, paramVM.dtConnectionInfo, connVM, "PurchaseInvoices");
                    }
                    else
                    {
                        results = new ImportDAL().UpdateUBLPurchaseReturnTransactions(dtPurchase, paramVM.dtConnectionInfo, connVM, "PurchaseInvoiceReturns");
                    }  


                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Id failed to Update to PurchaseInvoices Table\n";

                        foreach (DataRow row in dtPurchase.Rows)
                        {
                            message += row["ID"] + "\n";
                        }
                        rVM.Message = message;
                        FileLogger.Log("UNILEVERIntegrationDAL", "SaveUnileverPurchase_Web", message);

                    }
                    else
                    {
                        rVM.Message = "Saved Successfully";

                    }

                    

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
                FileLogger.Log("UNILEVERIntegrationDAL", "SaveUnileverPurchase_Web", ex.ToString());

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


        public DataTable GetSource_PurchaseReturnData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";
            DataTable dtPurchaseReturnMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
 Select 
       [ReferanceNo] [ID]
      ,[PreviousPurchaseCode][Previous_Purchase_No]
      ,[VendorName] [Vendor_Name]
      ,[VendorCode] [Vendor_Code]
	  ,[InvoiceDate] [Invoice_Date]
	  ,[ReceiveDate] [Receive_Date]
      ,[ReferanceNo] [Referance_No]
      ,[InvoiceNumber] [BE_Number]
      ,Sum([Quantity])[Quantity]
      ,[UOM]
      ,'VAT'[Type]
      --,[TotalPrice] [Total_Price]
     ,Sum(TotalPrice-(TotalPrice*15/115)) [Total_Price]
      ,Sum([SDAmount]) [SD_Amount]
      --,[VATAmount] [VAT_Amount]
      ,Sum(round((TotalPrice*15/115),4)) [VAT_Amount]
     --,Sum(round(((TotalPrice-(TotalPrice*15/115))*15/100),4)) [VAT_Amount]
      ,'Y'[Post]
      ,'-'[Comments]
      ,'N'[With_VDS]
      ,'0' [Rebate_Rate]
	  ,CASE
                 WHEN IsProcessed=0 THEN 'UnSaved'
                 WHEN IsProcessed=1 THEN 'Saved'
                 END AS Status

  FROM [PurchaseInvoiceReturns]
   WHERE 1=1


";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ReferanceNo = @PurchaseInvoiceNo";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.Processed))
                {
                    sqlText = sqlText + @" AND IsProcessed = @IsProcessed";
                }

                #endregion


                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 

   group by   [ReferanceNo] 
     ,[PreviousPurchaseCode]
      ,[VendorName] 
      ,[VendorCode] 
	  ,[InvoiceDate] 
      ,[ReceiveDate] 
      ,[PurchaseCode] 
      ,[InvoiceNumber] 
      ,[UOM]
	  ,[IsProcessed]
";

                #endregion
                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@IsProcessed", paramVM.Processed == "Y" ? true : false);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@PurchaseInvoiceNo", paramVM.RefNo);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchaseReturnMaster);

                #endregion

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

            return dtPurchaseReturnMaster;
        }
        public DataTable GetSource_PurchaseReturnDetailsData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";
            DataTable dtPurchaseReturnMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
       Select
       [ReferanceNo] [ID]
      ,[PreviousPurchaseCode][Previous_Purchase_No]
      ,[VendorName] [Vendor_Name]
      ,[VendorCode] [Vendor_Code]
	  ,[InvoiceDate] [Invoice_Date]
      ,[ReceiveDate] [Receive_Date]
      ,[ReferanceNo] [Referance_No]
      ,[InvoiceNumber] [BE_Number]
      ,[ProductCode] [Item_Code]
      ,[ProductName] [Item_Name]
      ,[Quantity]
      ,[UOM]
      ,'VAT'[Type]
      --,[TotalPrice] [Total_Price]
     ,TotalPrice-(TotalPrice*15/115) [Total_Price]
      ,[SDAmount] [SD_Amount]
      --,[VATAmount] [VAT_Amount]
      ,(round((TotalPrice*15/115),4)) [VAT_Amount]
    -- ,round(((TotalPrice-(TotalPrice*15/115))*15/100),4) [VAT_Amount]
      ,'Y'[Post]
      ,'-'[Comments]
      ,'N'[With_VDS]
      ,'0' [Rebate_Rate]
      ,'R'ProductType

  FROM [PurchaseInvoiceReturns]
   WHERE 1=1

";

                #region Filtering

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ReferanceNo = @PurchaseInvoiceNo";
                }


                #endregion



                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values




                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@PurchaseInvoiceNo", paramVM.RefNo);
                }



                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtPurchaseReturnMaster);

                #endregion

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

            return dtPurchaseReturnMaster;
        }


        public DataTable GetSaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {

                string TableName = "vat_sym_data"; // need to change GDIC

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "vat_sym_data";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"

SELECT
ID																			                                ID
,Post                                                                                                       Post
,BranchCode																	                                Branch_Code
,LTRIM(RTRIM(CustomerGroup))														                        CustomerGroup
,LTRIM(RTRIM(CustomerName))															                        Customer_Name
,LTRIM(RTRIM(CustomerCode))															                        Customer_Code
,DeliveryAddress																                            Delivery_Address
,''																					                        Vehicle_No
,'' 																				                        VehicleType
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										                            Invoice_Date_Time
,ReferenceNo  																                                Reference_No  
,''																					                        Comments
,'New'																				                        Sale_Type
,'' 																				                        Previous_Invoice_No
,'N'																				                        Is_Print
,'0'																				                        Tender_Id
,LCNumber																		                            LC_Number
,CurrencyCode																	                            Currency_Code
,LTRIM(RTRIM(ProductCode))															                        Item_Code
,LTRIM(RTRIM(ProductName))															                        Item_Name
,UOM																			                            UOM
,1																					                        Quantity
,UnitPrice																	                                NBR_Price
,VATRate																		                            VAT_Rate
,SDRate																		                                SD_Rate
,(1*UnitPrice)+(1*UnitPrice)*(VATRate/100)											                        TotalValue
,(1*UnitPrice)																		                        SubTotal
,(1*UnitPrice)*(VATRate/100) 														                        VAT_Amount
,'N'																				                        Non_Stock
,0																					                        Trading_MarkUp
,REPLACE(Type, ' ', '')																                        Type
,DiscountAmount																                                Discount_Amount
,PromotionalQuantity															                            Promotional_Quantity
,'VAT 4.3'																			                        VAT_Name
, case when TransactionType = 'Local' then 'Other' else TransactionType end                                 TransactionType
,'1900-Jan-01'																		                        PreviousInvoiceDateTime
,'0'																				                        PreviousNBRPrice
,'0'																				                        PreviousQuantity
,'0'																				                        PreviousUOM
,'0'																				                        PreviousSubTotal
,'0'																				                        PreviousSD
,'0'																				                        PreviousSDAmount
,'0'																				                        PreviousVATRate
,'0'																				                        PreviousVATAmount
,'NA'																				                        ReasonOfReturn
, IsLeader																			                        IsLeader
, case when ISNULL(IsLeader, 'NA') = 'Y' then ISNULL(LeaderAmount,0) else 0	end				                LeaderAmount
, case when ISNULL(IsLeader, 'NA') = 'Y' then ((1*LeaderAmount)*(VATRate/100)) else 0 end	                LeaderVATAmount
, case when ISNULL(IsLeader, 'NA') = 'N' then ISNULL(LeaderAmount,0) 
when ISNULL(IsLeader, 'NA') = 'Y' then (1*UnitPrice) - ISNULL(LeaderAmount,0) else 0 end	                NonLeaderAmount
, case when ISNULL(IsLeader, 'NA') = 'N' then (1*LeaderAmount)*(VATRate/100)
when ISNULL(IsLeader, 'NA') = 'Y' then ((1*UnitPrice) - ISNULL(LeaderAmount,0))*(VATRate/100) else 0 end    NonLeaderVATAmount


FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'
------AND IsProcessed='N'
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }


                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }


                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }



                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }




                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;


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
        }

        private void TableValidation(DataTable dtSales, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {


            var SL = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString("yyyy-MMM-dd") };

            if (!dtSales.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = param.DefaultBranchCode };
                dtSales.Columns.Add(columnName);
            }

            if (!dtSales.Columns.Contains("SL"))
            {
                dtSales.Columns.Add(SL);
            }
            if (!dtSales.Columns.Contains("CreatedBy"))
            {
                dtSales.Columns.Add(CreatedBy);
            }
            if (!dtSales.Columns.Contains("CreatedOn"))
            {
                dtSales.Columns.Add(CreatedOn);
            }
            if (!dtSales.Columns.Contains("ReturnId"))
            {
                dtSales.Columns.Add(ReturnId);
            }

            if (!dtSales.Columns.Contains("TransactionType"))
            {
                dtSales.Columns.Add(TransactionType);
            }
            if (!dtSales.Columns.Contains("BOMId"))
            {
                dtSales.Columns.Add(BOMId);
            }
        }

        public string[] UpdateTransactions(DataTable table, DataTable db, string tableName = "SaleInvoices", string TransactionType = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            CommonDAL commonDal = new CommonDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            #region try

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetDepoConnection(db);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }
                string code = commonDal.settingValue("CompanyCode", "Code", connVM);

                #endregion

                #region Sql Text

                var sqlText = @"";

                DataView dv = new DataView(table);

                table = dv.ToTable(true, "ChallanNo");
                #endregion

                #region Sql Command

                var len = table.Rows.Count;


                sqlText += " update " + tableName + " set IsProcessed = 1 where CompanyCode=@CompanyCode and ChallanNo in ( ";


                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + table.Rows[i]["ChallanNo"] + "',";
                }

                sqlText = sqlText.TrimEnd(',');

                sqlText += ")";
                if (!string.IsNullOrEmpty(TransactionType))
                {
                    if (TransactionType.ToLower() == "credit")
                    {
                        sqlText += "And Quantity<0";
                    }
                    else if (TransactionType.ToLower() == "debit")
                    {
                        sqlText += "And Quantity>0";
                    }
                }
               
                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@CompanyCode", code);

                var rows = cmd.ExecuteNonQuery();
                #endregion

                transaction.Commit();

                retResults[0] = rows > 0 ? "success" : "fail";

                return retResults;
            }
            #endregion

            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("ImportDAL", "UpdateACITransactions", ex.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public DataTable GetSource_DayEndClosingData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
            SELECT Distinct
             ChallanNo
             ,SaleCode
             ,ProductCode
			 ,sum(TotalQuantity+TotalFreeQuantity)DSQty
			 ,sum(TotalValue) DSValue
             FROM [DayEndClosing]
             WHERE 1=1
             AND CompanyCode=@CompanyCode

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
  ChallanNo
  ,SaleCode
  ,ProductCode
";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                cmd.Parameters.AddWithValue("@CompanyCode", paramVM.CompanyCode);
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }
        public DataTable GetSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
              SELECT Distinct 
              SaleCode 
              ,ProductCode
			 ,sum(Quantity+FreeQuantity)MSaleQty
			 ,sum((Quantity*UnitNBRPrice)) MSaleValue
             FROM [SaleInvoices]
             WHERE 1=1
			 and ProductType in ('R','P')
			 and IsProcessed=1
             and CompanyCode=@CompanyCode

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
SaleCode
,ProductCode
";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                cmd.Parameters.AddWithValue("@CompanyCode", paramVM.CompanyCode);
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_CraditData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
             SELECT Distinct 
              SaleCode 
              ,ProductCode
			 ,sum((Quantity*-1)+FreeQuantity)MCreditQty
			 ,sum(((Quantity*(-1))*UnitNBRPrice)) MCreditValue
             FROM [SaleInvoiceReturns]
             WHERE 1=1
			 and ProductType in ('R','P')
			 and IsProcessed=1
			 and Quantity<0
             and CompanyCode=@CompanyCode

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
 SaleCode 
,ProductCode
";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                cmd.Parameters.AddWithValue("@CompanyCode", paramVM.CompanyCode);
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }
        public DataTable GetSource_DebitData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
              SELECT Distinct 
              SaleCode 
              ,ProductCode
			 ,sum(Quantity+FreeQuantity)MDebitQty
			 ,sum((Quantity*UnitNBRPrice)) MDebitValue
             FROM [SaleInvoiceReturns]
             WHERE 1=1
			 and ProductType in ('R','P')
			 and IsProcessed=1
			 and Quantity>0
             and CompanyCode=@CompanyCode

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDate <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
 SaleCode 
,ProductCode
";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                cmd.Parameters.AddWithValue("@CompanyCode", paramVM.CompanyCode);
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }


        public DataTable GetSource_SalesDetailData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
          SELECT Distinct 
             ImportID SaleCode
             ,SalesInvoiceNo
             , Products.ProductCode
			 ,sum(Quantity+PromotionalQuantity)SaleQty
			 ,sum(SubTotal) SaleValue
             ,sum(VATAmount)SaleVAT
             FROM [SalesInvoiceDetails]
			 left outer join Products on Products.ItemNo=SalesInvoiceDetails.ItemNo
             WHERE 1=1
			 and ProductType in ('R','P')
			 and SalesInvoiceDetails.Post='Y'
			 and SalesInvoiceDetails.TransactionType='Other'

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND SalesInvoiceDetails.InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND SalesInvoiceDetails.InvoiceDateTime <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
  ImportID 
 ,SalesInvoiceNo
 ,ProductCode
";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_DebitDetailData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
          SELECT Distinct 
             SUBSTRING(ImportID,1,LEN(ImportID)-2) AS SaleCode
             ,SalesInvoiceNo
             ,Products.ProductCode
			 ,sum(Quantity+PromotionalQuantity)DebitQty
			 ,sum(SubTotal) DebitValue
             ,sum(VATAmount)DebitVAT
             FROM [SalesInvoiceDetails]
			 left outer join Products on Products.ItemNo=SalesInvoiceDetails.ItemNo
             WHERE 1=1
			 and ProductType in ('R','P')
			 and SalesInvoiceDetails.Post='Y'
			 and SalesInvoiceDetails.TransactionType='Debit'

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND SalesInvoiceDetails.InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND SalesInvoiceDetails.InvoiceDateTime <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
  ImportID 
 ,SalesInvoiceNo
 ,ProductCode";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetSource_CreditDetailData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            string sqltext = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText


                sqlText = @"
          SELECT Distinct 
             SUBSTRING(ImportID,1,LEN(ImportID)-2) AS SaleCode
             ,SalesInvoiceNo
             ,Products.ProductCode
			 ,sum(Quantity+PromotionalQuantity)CreditQty
			 ,sum(SubTotal) CreditValue
             ,sum(VATAmount)CreditVAT
             FROM [SalesInvoiceDetails]
			 left outer join Products on Products.ItemNo=SalesInvoiceDetails.ItemNo
             WHERE 1=1
			 and ProductType in ('R','P')
			 and SalesInvoiceDetails.Post='Y'
			 and SalesInvoiceDetails.TransactionType='Credit'

            ";

                #region Filtering


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND SalesInvoiceDetails.InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND SalesInvoiceDetails.InvoiceDateTime <=@toDate";
                }


                #endregion




                #endregion
                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
  ImportID 
 ,SalesInvoiceNo
 ,ProductCode";

                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values


                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }


                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }


        public string[] SaveNestleTempTable(DataTable dtNestle,DataTable dtMiddlewareSale,DataTable dtMiddlewareCredit,DataTable dtMiddlewareDebit,DataTable dtShamphanSale,DataTable dtShamphanCredit,DataTable dtShamphanDebit,SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null )
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[]bulkRes= new string[6];
            bulkRes[0] = "Fail";//Success or Fail
            bulkRes[1] = "Fail";// Success or Fail Message
            bulkRes[2] = Id.ToString();// Return Id
            bulkRes[3] = sqlText; //  SQL Query
            bulkRes[4] = "ex"; //catch ex
            bulkRes[5] = "Insert"; //Method Name
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

            #region try

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
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

                var commonDal = new CommonDAL();

                sqlText = @"delete from UBLTempTable;";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.ExecuteNonQuery();

                bulkRes = commonDal.BulkInsert("UBLTempTable", dtNestle, currConn, transaction);
                bulkRes = commonDal.BulkInsert("UBLTempTable", dtMiddlewareSale, currConn, transaction);
                bulkRes = commonDal.BulkInsert("UBLTempTable", dtMiddlewareCredit, currConn, transaction);
                bulkRes = commonDal.BulkInsert("UBLTempTable", dtMiddlewareDebit, currConn, transaction);
                bulkRes = commonDal.BulkInsert("UBLTempTable", dtShamphanSale, currConn, transaction);
                bulkRes = commonDal.BulkInsert("UBLTempTable", dtShamphanCredit, currConn, transaction);
                bulkRes = commonDal.BulkInsert("UBLTempTable", dtShamphanDebit, currConn, transaction);

                if (bulkRes[0].ToLower() == "fail")
                {
                    throw new Exception("Import Failed to Temp");
                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                //retResults[1] = "Data Synchronized Successfully.";
                //retResults[2] = "";
                #endregion SuccessResult
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("NESTLEIntegrationDAL", "SaveNestleTempTable", ex.ToString() + "\n" + sqlText);

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
            #region Results
            return retResults;
            #endregion
        }


        public DataTable GetTempData(SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"
     Select ProductCode 
 ,Sum(ISNULL(DSQty,0))DSQty
 ,Sum(ISNULL(DSValue,0))DSValue
,Sum(ISNULL(MSaleQty,0))MSaleQty
,Sum(ISNULL(MCreditQty,0))MCreditQty
,Sum(ISNULL(MDebitQty,0))MDebitQty
,Sum(ISNULL((MSaleQty),0)- ISNULL(MCreditQty,0)+ ISNULL(MDebitQty,0))MTotalSaleQty
,Sum(ISNULL((MSaleValue),0)- ISNULL(MCreditValue,0)+ ISNULL(MDebitValue,0))MTotalSaleValue
,Sum(ISNULL(SaleQty,0))SaleQty
,Sum(ISNULL(CreditQty,0))CreditQty
,Sum(ISNULL(DebitQty,0))DebitQty
,Sum(ISNULL((SaleQty),0)- ISNULL(CreditQty,0)+ ISNULL(DebitQty,0))TotalSaleQty
,round(Sum(ISNULL((SaleValue),0)+ISNULL((SaleVAT),0)- ISNULL(CreditValue,0)-ISNULL(CreditVAT,0)+ ISNULL(DebitValue,0)+ ISNULL(DebitVAT,0)),0)TotalSaleValue
,Sum(ISNULL(DSQty,0))-Sum(ISNULL((SaleQty),0)- ISNULL(CreditQty,0)+ ISNULL(DebitQty,0))DeffQty
,round(Sum(ISNULL(DSValue,0))-Sum(ISNULL((SaleValue),0)+ISNULL((SaleVAT),0)- ISNULL(CreditValue,0)-ISNULL(CreditVAT,0)+ ISNULL(DebitValue,0)+ ISNULL(DebitVAT,0)),0)TotaldeffValue

from UBLTempTable
group by ProductCode
order by ProductCode

            ";





                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }


        public DataTable GetTempDataDetails(SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                sqlText = @"create table #Temp
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    ProductCode Varchar(50), 
    SaleCode Varchar(50), 
    DSQty decimal(25, 9), 
    DSValue decimal(25, 9),
    MSaleQty decimal(25, 9),
    MCreditQty decimal(25, 9),
    MDebitQty decimal(25, 9),
    MTotalSaleQty decimal(25, 9),
    MTotalSaleValue decimal(25, 9),
    SaleQty decimal(25, 9),
    CreditQty decimal(25, 9),
    DebitQty decimal(25, 9),
    TotalSaleQty decimal(25, 9),
    TotalSaleValue decimal(25, 9),
    DeffQty decimal(25, 9),
    TotaldeffValue decimal(25, 9)
)";

                sqlText += @" 

INSERT INTO #Temp (ProductCode, SaleCode, DSQty,DSValue,MSaleQty,MCreditQty,MDebitQty,MTotalSaleQty,MTotalSaleValue,SaleQty,CreditQty,DebitQty,TotalSaleQty,TotalSaleValue,DeffQty,TotaldeffValue)

SELECT
 ProductCode 
,SaleCode
,Sum(ISNULL(DSQty,0))DSQty
,Sum(ISNULL(DSValue,0))DSValue
,Sum(ISNULL(MSaleQty,0))MSaleQty
,Sum(ISNULL(MCreditQty,0))MCreditQty
,Sum(ISNULL(MDebitQty,0))MDebitQty
,Sum(ISNULL((MSaleQty),0)- ISNULL(MCreditQty,0)+ ISNULL(MDebitQty,0))MTotalSaleQty
,Sum(ISNULL((MSaleValue),0)- ISNULL(MCreditValue,0)+ ISNULL(MDebitValue,0))MTotalSaleValue
,Sum(ISNULL(SaleQty,0))SaleQty
,Sum(ISNULL(CreditQty,0))CreditQty
,Sum(ISNULL(DebitQty,0))DebitQty
,Sum(ISNULL((SaleQty),0)- ISNULL(CreditQty,0)+ ISNULL(DebitQty,0))TotalSaleQty
,round(Sum(ISNULL((SaleValue),0)+ISNULL((SaleVAT),0)- ISNULL(CreditValue,0)-ISNULL(CreditVAT,0)+ ISNULL(DebitValue,0)+ ISNULL(DebitVAT,0)),0)TotalSaleValue
,Sum(ISNULL(DSQty,0))-Sum(ISNULL((SaleQty),0)- ISNULL(CreditQty,0)+ ISNULL(DebitQty,0))DeffQty
,round(Sum(ISNULL(DSValue,0))-Sum(ISNULL((SaleValue),0)+ISNULL((SaleVAT),0)- ISNULL(CreditValue,0)-ISNULL(CreditVAT,0)+ ISNULL(DebitValue,0)+ ISNULL(DebitVAT,0)),0)TotaldeffValue

from UBLTempTable

group by ProductCode,SaleCode
order by ProductCode


SELECT ProductCode, SalesInvoiceHeaders.SalesInvoiceNo,SaleCode, DSQty,DSValue,MSaleQty,MCreditQty,MDebitQty,MTotalSaleQty,MTotalSaleValue,SaleQty,CreditQty,DebitQty,TotalSaleQty,TotalSaleValue,DeffQty,TotaldeffValue from #Temp
left outer join SalesInvoiceHeaders on #Temp.SaleCode=SalesInvoiceHeaders.ImportIDExcel
where DeffQty !=0
and TotaldeffValue !=0


Drop Table #Temp
   
            
   
            ";





                #endregion

                //sqlText += " ORDER BY Status desc";


                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

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

            return dtSalesMaster;

        }

        public DataTable GetExcelData(SysDBInfoVMTemp connVM = null)
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

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM); // need to change


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Pharma Select

                sqlText = @"
create table #temp(

[ID] [int] IDENTITY(1,1) NOT NULL,
ProductCode varchar(500),
DeffQty decimal(25,9),
TotaldeffValue decimal(25,9),
)

insert into #temp
 Select ProductCode 
,Sum(ISNULL(DSQty,0))-Sum(ISNULL((SaleQty),0)- ISNULL(CreditQty,0)+ ISNULL(DebitQty,0))DeffQty
,round(Sum(ISNULL(DSValue,0))-Sum(ISNULL((SaleValue),0)+ISNULL((SaleVAT),0)- ISNULL(CreditValue,0)-ISNULL(CreditVAT,0)+ ISNULL(DebitValue,0)+ ISNULL(DebitVAT,0)),0)TotaldeffValue

from UBLTempTable
group by ProductCode
order by ProductCode

 select 
''ID
,'001'Branch_Code
,'' Customer_Code
,'' Customer_Name
,'2021/12/31' Invoice_Date
,'08:53:25' Invoice_Time
,'' Reference_No
, ProductCode Item_Code
,'' Item_Name
,DeffQty Quantity	
,TotaldeffValue Amount	
,'PC'UOM	
,''NBR_Price	
,''  Weight	
,''SubTotal	
,''Vehicle_No	
,''VehicleType	
,''CustomerGroup	
,''Delivery_Address	
,'Adjustment'Comments	
,'NEW'Sale_Type	
,'Other'TransactionType	
,'Y'Is_Print	
,''Tender_Id	
,'Y'Post	
,''LC_Number	
,'BDT'Currency_Code	
,''CommentsD	
,'0'SD_Rate
,'0'SDAmount	
,'15'VAT_Rate	
,''VAT_Amount	
,'N'Non_Stock	
,''Trading_MarkUp	
,'VAT'Type	
,'0'Discount_Amount	
,'0'Promotional_Quantity	
,'VAT 4.3'VAT_Name	
,''ProductDescription	
,''Previous_Invoice_No	
,''PreviousInvoiceDateTime	
,'0'PreviousNBRPrice	
,'0'PreviousQuantity	
,'0'PreviousSubTotal	
,'0'PreviousSD	
,'0'PreviousSDAmount	
,'0'PreviousVATRate	
,'0'PreviousVATAmount	
,''PreviousUOM	
,''ReasonOfReturn	
,'NA'BENumber	
,''ExpDescription	
,''ExpQuantity	
,''ExpGrossWeight	
,''ExpNetWeight	
,''ExpNumberFrom	
,''ExpNumberTo

  
  
  
from #temp  
where DeffQty <> 0
and TotaldeffValue <> 0

drop table #temp";

                #endregion



                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                ProductDAL productDal = new ProductDAL();

                foreach (DataRow tableRow in table.Rows)
                {

                    var vms = productDal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { tableRow["Item_Code"].ToString() });

                    ProductVM vm = vms.FirstOrDefault();
                    decimal SubTota = 0;
                    decimal VAT_Amount = 0;
                    decimal amount = Convert.ToDecimal(tableRow["Amount"].ToString());
                    decimal quantity = Convert.ToDecimal(tableRow["Quantity"].ToString());
                    decimal price = 0;
                    //if (quantity > 0)
                    //{
                    //    price = (amount / quantity);
                    //}
                    price = (amount / quantity);
                    price = price - (price * 15 / 115);
                    SubTota = (price * quantity);
                    VAT_Amount = (SubTota * 15 / 100);
                    tableRow["NBR_Price"] = Math.Round(price, 5);
                    tableRow["SubTotal"] = Math.Round(SubTota, 5);
                    tableRow["VAT_Amount"] = Math.Round(VAT_Amount, 5);
                    tableRow["Item_Name"] = vm.ProductName;

                }

                table.Columns.Remove("Amount");

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("ImportDAL", "GetTollACIDbData", ex.ToString() + "\n" + sqlText);

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

        public ResultVM DataProcess(IntegrationParam paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
           
            
            DataTable dtNestle = new DataTable();
            DataTable dtMiddlewareSale = new DataTable();
            DataTable dtMiddlewareCredit = new DataTable();
            DataTable dtMiddlewareDebit = new DataTable();
            DataTable dtShamphanSale = new DataTable();
            DataTable dtShamphanCredit = new DataTable();
            DataTable dtShamphanDebit = new DataTable();
            #endregion

            #region Try Statement

            try
            {

              
               dtNestle = GetSource_DayEndClosingData(paramVM, connVM);

              #region Data Load

                dtMiddlewareSale = GetSource_SaleData(paramVM, connVM);
                dtMiddlewareCredit = GetSource_CraditData(paramVM, connVM);
                dtMiddlewareDebit = GetSource_DebitData(paramVM, connVM);
                dtShamphanSale = GetSource_SalesDetailData(paramVM, connVM);
                dtShamphanCredit = GetSource_CreditDetailData(paramVM, connVM);
                dtShamphanDebit =GetSource_DebitDetailData(paramVM, connVM);

                var Result = SaveNestleTempTable(dtNestle, dtMiddlewareSale, dtMiddlewareCredit, dtMiddlewareDebit, dtShamphanSale, dtShamphanCredit, dtShamphanDebit, null, null, connVM);
                rVM.Status = Result[0];

                if (Result[0].ToLower() == "fail")
                {
                    rVM.Message = "Import Failed to Temp";
                }
                else
                {
                    rVM.Message = "Data Successfully Processed";
                }
              
                #endregion

            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                FileLogger.Log("UNILEVERIntegrationDAL", "DataProcess", ex.ToString());

                throw ex;
            }

            finally
            {
               
            }

            #endregion

            return rVM;
        }



        #endregion
    }
}
