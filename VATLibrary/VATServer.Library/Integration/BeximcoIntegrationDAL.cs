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

namespace VATServer.Library.Integration
{
    public class BeximcoIntegrationDAL 
    {
        private string[] sqlResults;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        SaleDAL _SaleDAL = new SaleDAL();
        TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

        #region BCL - Beximco Communication Ltd.

        #region Sales (and Credit Note) Methods

        public DataTable GetSource_SaleData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

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

                #region SQLText
                string TableName = "BCL_Trading_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                sqlText = @"

SELECT
0							                                                        Selected
,InvoiceNo																			InvoiceNo
,ID																			        ID
,Post                                                                               Post
,IsPrint                                                                            IsPrint
,ReferenceNo                                                                        ReferenceNo
,BranchCode																	        BranchCode
,CustomerName																	    CustomerName
,CustomerCode																	    CustomerCode
,cast(InvoiceDateTime as varchar(20)) 										        InvoiceDateTime
,DeliveryAddress																    DeliveryAddress
, VehicleNo 																		VehicleNo
, VehicleType 																		VehicleType
,SUM((Quantity*UnitPrice)+(Quantity*UnitPrice)*(VATRate/100))	                    TotalValue
,SUM(Quantity)																		TotalQuantity
,SUM((Quantity*UnitPrice)*(VATRate/100)) 									        TotalVATAmount
, IsProcessed																		Processed

FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'

----------------------------------------!Care on GROUP BY---------------------------------------- 
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


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @SalesInvoiceNo";
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

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
InvoiceNo
,ID
,BranchCode
,CustomerName
,CustomerCode
,InvoiceDateTime
,DeliveryAddress
,VehicleNo
,VehicleType
,IsProcessed
,Post
,IsPrint
,ReferenceNo


";

                #endregion

                sqlText += " ORDER BY InvoiceDateTime";

                #endregion

                #region SQLExecution

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
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion


                #region Selected Data

                if (dtSalesMaster != null && dtSalesMaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtSalesMaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtSalesMaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtSalesMaster = new DataTable();
                        dtSalesMaster = rows.CopyToDataTable();

                    }


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

        public DataTable GetSaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {

                string AutoPost = "Y";
                AutoPost = _cDAL.settingValue("Integration", "FromDBSaleAutoPost");

                string TableName = "BCL_Trading_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"

------declare @AutoPost as varchar(1) = 'Y'

SELECT
ID																			        ID
,Post                                                                               Post
,BranchCode																	        Branch_Code
,LTRIM(RTRIM(CustomerName))															Customer_Name
,LTRIM(RTRIM(CustomerCode))															Customer_Code
,DeliveryAddress																    Delivery_Address
,VehicleNo 																			Vehicle_No
,VehicleType 																		VehicleType
,cast(InvoiceDateTime as varchar(20)) 										        Invoice_Date_Time
,cast(InvoiceDateTime as varchar(20))										        Delivery_Date_Time
,ReferenceNo  																        Reference_No  
,Comments																		    Comments
,'New'																				Sale_Type
,'' 																				Previous_Invoice_No
,'N'																				Is_Print
,'0'																				Tender_Id
,LCNumber																		    LC_Number
,CurrencyCode																	    Currency_Code
,LTRIM(RTRIM(ProductCode))															Item_Code
,LTRIM(RTRIM(ProductName))															Item_Name
,UOM																			    UOM
,Quantity																		    Quantity
,UnitPrice																	        NBR_Price
,VATRate																		    VAT_Rate
,SDRate																		        SD_Rate
,(Quantity*UnitPrice)+(Quantity*UnitPrice)*(VATRate/100)	                        TotalValue
,(Quantity*UnitPrice)													            SubTotal
,(Quantity*UnitPrice)*(VATRate/100) 									            VAT_Amount
,'N'																				Non_Stock
,0																					Trading_MarkUp
,REPLACE(Type, ' ', '')																Type
,DiscountAmount																        Discount_Amount
,PromotionalQuantity															    Promotional_Quantity
,'VAT 4.3'																			VAT_Name
, case when TransactionType = 'Local' then 'Other' else TransactionType end         TransactionType
,PreviousInvoiceDateTime													        PreviousInvoiceDateTime
,PreviousNBRPrice															        PreviousNBRPrice
,PreviousQuantity															        PreviousQuantity
,PreviousUOM																        PreviousUOM
,(PreviousQuantity*PreviousNBRPrice)										        PreviousSubTotal
,PreviousSD																	        PreviousSD
,PreviousSDAmount															        PreviousSDAmount
,PreviousVATRate															        PreviousVATRate
,(PreviousQuantity*PreviousNBRPrice)*(PreviousVATRate/100)					        PreviousVATAmount
,ReasonOfReturn																        ReasonOfReturn


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


                ////if (!string.IsNullOrWhiteSpace(paramVM.Processed) && paramVM.Processed.ToLower() == "ALL".ToLower())
                ////{
                ////    sqlText = sqlText.Replace("IsProcessed='N'", "1=1");
                ////}

                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

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



                ////if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                ////{
                ////    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                ////}



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

        public ResultVM SaveSale_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                DataTable dtSale = new DataTable();
                dtSale = GetSaleData(paramVM);

                if (dtSale == null || dtSale.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                string Setp = "N";

                Setp = _cDAL.settingValue("SaleWeb", "Setps");

                if (Setp == "N")
                {
                    rVM = SaveSale(dtSale, paramVM,connVM);
                    rVM.Step = Setp;

                }
                else
                {
                    ////string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + paramVM.CurrentUser;

                    sqlResults = new SaleDAL().SaveToTemp(dtSale, paramVM.DefaultBranchId, paramVM.TransactionType, paramVM.Token, null, paramVM);

                    rVM.Status = sqlResults[0];
                    rVM.Message = "Saved to Temp";
                    rVM.Step = Setp;

                }

            }
            catch (Exception ex)
            {

                rVM.Message = ex.Message;
            }
            finally
            {


            }
            return rVM;
        }

        public ResultVM SaveSale_Setp2(SaleMasterVM vm, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            ResultVM rVM = new ResultVM();
            try
            {

                sqlResults = new SaleDAL().SaveToTransactionTables(() => { }, vm.BranchId, "", null, vm.Token,UserId);
                rVM.Status = sqlResults[0];
                rVM.Message = "Transaction Saved Successfully";

                DataTable dtTemp = new DataTable();
                dtTemp = _SaleDAL.GetMasterData(null, null, "",connVM, null, false, vm.Token);

                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    DataView dv = new DataView(dtTemp);
                    DataTable dtSales = dv.ToTable(true, "Id");

                    string SalesIDs = "";
                    SalesIDs = JsonConvert.SerializeObject(dtSales);
                    IntegrationParam paramVM = new IntegrationParam();
                    paramVM.IDs = JsonConvert.DeserializeObject<List<string>>(SalesIDs);

                    UpdateSource_SaleData(paramVM);
                }

            }
            catch (Exception ex)
            {

                rVM.Message = ex.Message;
            }
            finally
            {

            }
            return rVM;
        }

        public ResultVM SaveSale(DataTable dtSales, IntegrationParam paramVM, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            ResultVM rVM = new ResultVM();
            try
            {
                SaleDAL salesDal = new SaleDAL();

                TableValidation(dtSales, paramVM);
                sqlResults = salesDal.SaveAndProcess(dtSales, () => { }, paramVM.DefaultBranchId, null, null, paramVM,null,null,UserId);

                rVM = UpdateSource_SaleData(paramVM);

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";

                if (paramVM != null && !string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    SaleMasterVM varSaleMasterVM = new SaleMasterVM();
                    varSaleMasterVM = _SaleDAL.SelectAllList(0, new[] { "sih.ImportIDExcel" }, new[] { paramVM.RefNo }, null, null, null, null).FirstOrDefault();

                    rVM.InvoiceNo = varSaleMasterVM.SalesInvoiceNo;
                }



            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;
            }
            finally { }
            return rVM;
        }

        public ResultVM SaveCredit_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                DataTable dtCredit = new DataTable();
                dtCredit = GetSaleData(paramVM);

                if (dtCredit == null || dtCredit.Rows.Count == 0)
                {
                    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                {
                    paramVM.IDs = new List<string>();
                    paramVM.IDs.Add(paramVM.RefNo);

                }


                rVM = SaveSale(dtCredit, paramVM);


            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

            }
            finally
            {


            }
            return rVM;
        }

        private void TableValidation(DataTable dtSales, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            if (!dtSales.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = param.DefaultBranchCode };
                dtSales.Columns.Add(columnName);
            }

            var SL = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

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

        public ResultVM UpdateSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                string TableName = "BCL_Trading_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE @TableName SET 
  IsProcessed = 'Y'
, InvoiceNo=sih.SalesInvoiceNo, Post=sih.Post   
FROM @TableName sSal 
INNER JOIN SalesInvoiceHeaders sih on sSal.ID=sih.ImportIDExcel
WHERE 1=1
";
                sqlText = sqlText.Replace("@TableName", TableName);
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sSal.ID IN('" + IDs + "')";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = "Success";
                rVM.Message = "Source Data Updated Successfully!";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public ResultVM PostSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                SaleMasterVM varSaleMasterVM = new SaleMasterVM();
                varSaleMasterVM.IDs = paramVM.IDs;

                rVM = _SaleDAL.Multiple_SalePost(varSaleMasterVM, transaction, currConn, connVM);

                string TableName = "BCL_Trading_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE " + TableName +
@" SET 
Post = 'Y'
FROM " + TableName + " sSal" +
@"
INNER JOIN SalesInvoiceHeaders sih on sSal.InvoiceNo=sih.SalesInvoiceNo
WHERE 1=1
";
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sSal.InvoiceNo IN('" + IDs + "')";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = rows > 0 ? "Success" : "Fail";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public ResultVM UpdateSource_TransferData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                string TableName = "BCL_Trading_VAT_Source_DB.dbo.TransferIssues";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_TransferIssues";
                }

                string sqlText = @"";
                sqlText += @" 

UPDATE @TableName SET 
  IsProcessed = 'Y'
, InvoiceNo=tih.TransferIssueNo, Post=tih.Post   
FROM @TableName sTransfer 
INNER JOIN TransferIssues tih on sTransfer.ID=tih.ImportIDExcel
WHERE 1=1
";
                sqlText = sqlText.Replace("@TableName", TableName);
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sTransfer.ID IN('" + IDs + "')";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = "Success";
                rVM.Message = "Source Data Updated Successfully!";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public ResultVM PostSource_TransferData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                SaleMasterVM varSaleMasterVM = new SaleMasterVM();
                varSaleMasterVM.IDs = paramVM.IDs;

                rVM = _TransferIssueDAL.Multiple_Post(paramVM, transaction, currConn, connVM);

                string TableName = "BCL_Trading_VAT_Source_DB.dbo.TransferIssues";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_TransferIssues";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE " + TableName +
@" SET 
Post = 'Y'
FROM " + TableName + " sTransfer" +
@"
INNER JOIN TransferIssues tih on sTransfer.InvoiceNo=tih.TransferIssueNo
WHERE 1=1
";
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sTransfer.InvoiceNo IN('" + IDs + "')";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = "Success";
                rVM.Message = "Source Data Updated Successfully!";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public ResultVM PrintSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                string TableName = "BCL_Trading_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE " + TableName +
@" SET 
IsPrint = 'Y'
FROM " + TableName + " sSal" +
@"
INNER JOIN SalesInvoiceHeaders sih on sSal.InvoiceNo=sih.SalesInvoiceNo
WHERE 1=1
";
                if (!string.IsNullOrWhiteSpace(paramVM.MulitipleInvoice))
                {
                    if (!paramVM.MulitipleInvoice.Contains("'"))
                    {
                        paramVM.MulitipleInvoice = "'" + paramVM.MulitipleInvoice + "'";
                    }
                    sqlText = sqlText + @" AND sSal.InvoiceNo IN(" + paramVM.MulitipleInvoice + ")";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = "Print Flag Update SuccessFully!";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public ResultVM PrintSource_TransferData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                string TableName = "BCL_Trading_VAT_Source_DB.dbo.TransferIssues";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_TransferIssues";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE " + TableName +
@" SET 
IsPrint = 'Y'
FROM " + TableName + " sTransfer" +
@"
INNER JOIN TransferIssues tih on sTransfer.InvoiceNo=tih.TransferIssueNo
WHERE 1=1
";

                if (!string.IsNullOrWhiteSpace(paramVM.MulitipleInvoice))
                {
                    if (!paramVM.MulitipleInvoice.Contains("'"))
                    {
                        paramVM.MulitipleInvoice = "'" + paramVM.MulitipleInvoice + "'";
                    }
                    sqlText = sqlText + @" AND sTransfer.InvoiceNo IN(" + paramVM.MulitipleInvoice + ")";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = "Success";
                rVM.Message = "Source Data Updated Successfully!";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }


        #endregion


        #region Transfer Methods

        public DataTable GetSource_TransferData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtTransferMaster = new DataTable();

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

                #region SQLText
                string TableName = "BCL_Trading_VAT_Source_DB.dbo.TransferIssues";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_TransferIssues";
                }

                sqlText = @"

SELECT
0		                                                        Selected
,InvoiceNo												        InvoiceNo
,ID														        ID
,Post                                                           Post
,IsPrint                                                        IsPrint
,ReferenceNo                                                    ReferenceNo
,BranchCode												        BranchCode
,TransferToBranchCode									        TransferToBranchCode
,cast(TransactionDateTime as varchar(20)) 				        TransactionDateTime
, VehicleNo 											        VehicleNo
, VehicleType 											        VehicleType
,SUM(Quantity)											        TotalQuantity
,SUM(CostPrice)											        TotalCostPrice
, IsProcessed											        Processed

FROM " + TableName +
@"
WHERE 1=1


----------------------------------------!Care on GROUP BY---------------------------------------- 
";

                #region Filtering

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


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND TransactionDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND TransactionDateTime < dateadd(d,1,@toDate)";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode=@BranchCode";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
InvoiceNo
,ID
,BranchCode
,TransferToBranchCode
,TransactionDateTime
,VehicleNo
,VehicleType
,IsProcessed
,Post
,ReferenceNo
,IsPrint

";

                #endregion

                sqlText += " ORDER BY TransactionDateTime";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Parameter Value

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

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtTransferMaster);

                #endregion

                #region Selected Data

                if (dtTransferMaster != null && dtTransferMaster.Rows.Count > 0 && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtTransferMaster.Copy();

                    string IDs = string.Join("','", paramVM.IDs);

                    DataRow[] rows = dtTransferMaster.Select("ID  IN ('" + IDs + "')");

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (DataRow dr in rows)
                        {
                            dr["Selected"] = 1;
                        }

                        dtTransferMaster = new DataTable();
                        dtTransferMaster = rows.CopyToDataTable();

                    }


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

                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
            #endregion

            return dtTransferMaster;

        }

        public DataTable GetTransferData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {
                string AutoPost = "Y";
                AutoPost = _cDAL.settingValue("Integration", "FromDBTransferAutoPost",connVM);

                DataTable dt = new BranchProfileDAL().SelectAll(paramVM.DefaultBranchId.ToString(),null,null,null,null,true,connVM);
                paramVM.dtConnectionInfo = dt;

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SqlText

                sqlText = @"
SELECT
    0									SL
  , Id									ID
  , BranchCode							BranchCode
  , TransactionDateTime					TransactionDateTime
  , ProductType							TransactionType
  , ProductCode							ProductCode
  , ProductName							ProductName
  , UOM									UOM
  , Quantity							Quantity
  ,	CostPrice							CostPrice
  , TransferToBranchCode				TransferToBranchCode
  , Post							    Post
  , VATRate								VAT_Rate
  , ReferenceNo							ReferenceNo
  , Comments							Comments
  , 0									ItemNo
  , 0									BranchId
  , 0									BomId
  , 0									TransferToBranchId
  , ''									CommentsD
  , ''									Weight
  , VehicleNo							VehicleNo
  , VehicleType 						VehicleType

FROM TransferIssues			
							
WHERE 1=1 					
------AND IsProcessed='N'		

";
                #region Filtering

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
                    sqlText = sqlText + @" AND TransactionDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND TransactionDateTime < dateadd(d,1,@toDate)";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode=@BranchCode";
                }

                #endregion


                #endregion

                #region SQLExecution


                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Parameter Value

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

                DataTable dtTransfer = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtTransfer);


                #endregion

                return dtTransfer;
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

        public ResultVM SaveTransfer_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                DataTable dtTransfer = new DataTable();
                dtTransfer = GetTransferData(paramVM);

                if (dtTransfer == null || dtTransfer.Rows.Count == 0)
                {
                    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                {
                    paramVM.IDs = new List<string>();
                    paramVM.IDs.Add(paramVM.RefNo);

                }

                rVM = SaveTransfer(dtTransfer, paramVM);


            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

            }
            finally
            {


            }
            return rVM;
        }

        public ResultVM SaveTransfer(DataTable dtTransfer, IntegrationParam paramVM, string UserId = "")
        {
            ResultVM rVM = new ResultVM();

            try
            {


                TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

                TableValidation_Transfer(dtTransfer, paramVM);

                sqlResults = _TransferIssueDAL.SaveTempTransfer(dtTransfer, null, null, paramVM.CurrentUser, 0, () => { }, null, null, true,null,"",UserId);


                rVM = UpdateSource_TransferData(paramVM);

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";

                if (paramVM != null && !string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    TransferIssueVM varTransferIssueVM = new TransferIssueVM();
                    varTransferIssueVM = _TransferIssueDAL.SelectAllList(0, new[] { "ti.ImportIDExcel" }, new[] { paramVM.RefNo }, null, null, null).FirstOrDefault();

                    rVM.InvoiceNo = varTransferIssueVM.TransferIssueNo;
                }

            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

            }

            finally { }
            return rVM;

        }

        private void TableValidation_Transfer(DataTable dtTransfer, IntegrationParam param)
        {
            if (!dtTransfer.Columns.Contains("SL"))
            {
                DataColumn varDataColumn = new DataColumn("SL") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }
            if (!dtTransfer.Columns.Contains("BranchId"))
            {
                DataColumn varDataColumn = new DataColumn("BranchId") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }
            if (!dtTransfer.Columns.Contains("TransferToBranchId"))
            {
                DataColumn varDataColumn = new DataColumn("TransferToBranchId") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }
            if (!dtTransfer.Columns.Contains("VehicleNo"))
            {
                DataColumn varDataColumn = new DataColumn("VehicleNo") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }

            if (!dtTransfer.Columns.Contains("BomId"))
            {
                DataColumn varDataColumn = new DataColumn("BomId") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }

            if (!dtTransfer.Columns.Contains("ItemNo"))
            {
                DataColumn varDataColumn = new DataColumn("ItemNo") { DefaultValue = "" };
                dtTransfer.Columns.Add(varDataColumn);
            }

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


        #endregion
    }



}
