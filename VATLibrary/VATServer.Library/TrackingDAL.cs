using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATServer.Ordinary;

namespace VATServer.Library
{
    public class TrackingDAL : ITracking
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public DataTable FindTrackingItems(string fItemNo, string vatName, string effectDate, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("TrackingDetail");

            #endregion

            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                string BomId = string.Empty;
                DateTime BOMDate = DateTime.MinValue;
                string bomDate = "";
                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                sqlText += " where ";
                //sqlText += " FinishItemNo='" + fItemNo + "' ";
                sqlText += " FinishItemNo = @fItemNo";
                //sqlText += " and vatname='" + vatName + "' ";
                sqlText += " and vatname = @vatName";
                //sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and effectdate <= @effectDate";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                //cmdBomId.Transaction = transaction;

                //BugsBD
                SqlParameter parameter = new SqlParameter("@fItemNo", SqlDbType.VarChar, 250);
                parameter.Value = fItemNo;
                cmdBomId.Parameters.Add(parameter);

                parameter = new SqlParameter("@vatName", SqlDbType.VarChar, 250);
                parameter.Value = vatName;
                cmdBomId.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.VarChar, 250);
                parameter.Value = effectDate;
                cmdBomId.Parameters.Add(parameter);


                if (cmdBomId.ExecuteScalar() == null)
                {
                    throw new ArgumentNullException("Tracking Info", "No Data found for this item");
                    BomId = "0";
                }
                else
                {
                    BomId = (string)cmdBomId.ExecuteScalar();
                }

                #endregion Last BOMId

                #region Last BOMDate

                sqlText = "  ";
                sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                //sqlText += " where FinishItemNo='" + fItemNo + "' ";
                sqlText += " where FinishItemNo = @fItemNo";
                //sqlText += " and vatname='" + vatName + "' ";
                sqlText += " and vatname = @vatName";
                //sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and effectdate <= @effectDate";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                //cmdBomEDate.Transaction = transaction;

                //BugsBD
                SqlParameter parameter2 = new SqlParameter("@fItemNo", SqlDbType.VarChar, 250);
                parameter2.Value = fItemNo;
                cmdBomEDate.Parameters.Add(parameter2);

                parameter2 = new SqlParameter("@vatName", SqlDbType.VarChar, 250);
                parameter2.Value = vatName;
                cmdBomEDate.Parameters.Add(parameter2);

                parameter2 = new SqlParameter("@effectDate", SqlDbType.VarChar, 250);
                parameter2.Value = effectDate;
                cmdBomEDate.Parameters.Add(parameter2);

                if (cmdBomEDate.ExecuteScalar() == null)
                {
                    throw new ArgumentNullException("TrackingInfo", "No Data found for this item");
                    BOMDate = DateTime.MinValue;
                    bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                }
                else
                {
                    BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                    bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                }

                #endregion Last BOMDate

                #region Find Raw Item From BOM  and update Stock

                sqlText = "";
                sqlText +=
                    " SELECT Distinct  b.RawItemNo,b.UseQuantity,b.WastageQuantity,b.UOMUQty,b.UOMWQty from BOMRaws b,Trackings t  ";
                sqlText += " where b.RawItemNo = t.ItemNo ";
                //sqlText += " and b.FinishItemNo='" + fItemNo + "' ";
                sqlText += " and b.FinishItemNo = @fItemNo";
                //sqlText += " and b.vatname='" + vatName + "' ";
                sqlText += " and b.vatname = @vatName";
                //sqlText += " and b.effectdate='" + bomDate + "'";
                sqlText += " and b.effectdate = @bomDate";
                sqlText += " and b.post='Y' ";
                sqlText += "   and (rawitemtype='raw' or rawitemtype='finish') ";

                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                //cmdRIFB.Transaction = transaction;

                //BugsBD
                SqlParameter parameter3 = new SqlParameter("@fItemNo", SqlDbType.VarChar, 250);
                parameter3.Value = fItemNo;
                cmdRIFB.Parameters.Add(parameter3);

                parameter3 = new SqlParameter("@vatName", SqlDbType.VarChar, 250);
                parameter3.Value = vatName;
                cmdRIFB.Parameters.Add(parameter3);

                parameter3 = new SqlParameter("@bomDate", SqlDbType.VarChar, 250);
                parameter3.Value = bomDate;
                cmdRIFB.Parameters.Add(parameter3);



                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                reportDataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("TrackingInfo", "There is no data to Post");

                }
                else if (dataTable.Rows.Count <= 0)
                {
                    throw new ArgumentNullException("TrackingInfo","No Data found for the Item Code (" + fItemNo + ")");
                                                    
                }

                #endregion Find Raw Item From BOM and update Stock



                #endregion

            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }

        public DataTable SearchTrackingItems(string itemNo, string isIssue, string isReceive, string isSale, string finishItemNo,
            string receiveNo, string issueNo, string saleInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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

                #region sql statement
                #region Parameter
//                Declare @itemNo varchar(100) 
//Declare @FinishItemNo varchar(100) 
//Declare @saleInvoiceNo varchar(100) 
//Declare @isSale varchar(100) 
//Declare @isReceive varchar(100) 
//Declare @receiveNo varchar(100) 
//Declare @issueNo varchar(100) 
//Declare @isIssue varchar(100)

//SET @itemNo='36'
//SET @FinishItemNo =''
//SET @saleInvoiceNo=''
//SET @isSale =''
//SET @isReceive ='N'
//SET @receiveNo =''
//SET @issueNo =''
//SET @isIssue =''
                #endregion


                sqlText = "";
                sqlText += @"

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]

  FROM Trackings t,Products pr

WHERE t.[ItemNo]=pr.[ItemNo] And t.IsPurchase='Y' And t.Post='Y'
           AND (t.ItemNo LIKE '%' +  @itemNo + '%' OR @itemNo IS NULL) 
		   AND ((IsIssue =@isIssue )  OR (IssueNo= @issueNo))
		   AND ((IsReceive =@isReceive )  OR (ReceiveNo= @receiveNo))
		   AND ((IsSale =@isSale )  OR (SaleInvoiceNo= @saleInvoiceNo))
		   AND (FinishItemNo LIKE '%' +  @FinishItemNo + '%' OR @FinishItemNo IS NULL)


  --       AND ((IsSale LIKE '%' +  @isIssue + '%' OR @isIssue IS NULL)  
				--OR (IssueNo LIKE '%' +  @issueNo + '%' OR @issueNo IS NULL))  
    --       AND ((IsReceive LIKE '%' +  @isReceive + '%' OR @isReceive IS NULL)  
				--OR (ReceiveNo LIKE '%' +  @receiveNo + '%' OR @receiveNo IS NULL)  )
		  
    --       AND ((IsSale LIKE '%' +  @isSale + '%' OR @isSale IS NULL)  
				--OR (SaleInvoiceNo LIKE '%' +  @saleInvoiceNo + '%' OR @saleInvoiceNo IS NULL) )                       

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

               
                if (string.IsNullOrEmpty(isIssue))
                {
                    if (!objCommProduct.Parameters.Contains("@isIssue"))
                    { objCommProduct.Parameters.AddWithValue("@isIssue", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isIssue"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isIssue"))
                    { objCommProduct.Parameters.AddWithValue("@isIssue", isIssue); }
                    else { objCommProduct.Parameters["@isIssue"].Value = isIssue; }
                }

                if (string.IsNullOrEmpty(issueNo))
                {
                    if (!objCommProduct.Parameters.Contains("@issueNo"))
                    { objCommProduct.Parameters.AddWithValue("@issueNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@issueNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@issueNo"))
                    { objCommProduct.Parameters.AddWithValue("@issueNo", issueNo); }
                    else { objCommProduct.Parameters["@issueNo"].Value = issueNo; }
                }

                if (string.IsNullOrEmpty(isReceive))
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isReceive"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", isReceive); }
                    else { objCommProduct.Parameters["@isReceive"].Value = isReceive; }
                }

                if (string.IsNullOrEmpty(receiveNo))
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", receiveNo); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = receiveNo; }
                }

                if (string.IsNullOrEmpty(isSale))
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isSale"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", isSale); }
                    else { objCommProduct.Parameters["@isSale"].Value = isSale; }
                }

                if (string.IsNullOrEmpty(saleInvoiceNo))
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = saleInvoiceNo; }
                }

                if (finishItemNo == "" || string.IsNullOrEmpty(finishItemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@finishItemNo"))
                    { objCommProduct.Parameters.AddWithValue("@finishItemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@finishItemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@finishItemNo"))
                    { objCommProduct.Parameters.AddWithValue("@finishItemNo", finishItemNo); }
                    else { objCommProduct.Parameters["@finishItemNo"].Value = finishItemNo; }
                }



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally

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

            return dataTable;

        }


        public List<TrackingVM> GetTrackingsWeb(string itemNo, string isTransaction, string transactionId, string type, SysDBInfoVMTemp connVM = null)
        {
            List<TrackingVM> SaleTrackings = new List<TrackingVM>();

            //foreach (PurchaseDetailVM purchaseDetailVm in DetailVMs)
            //{
                DataTable trackingInfoDt = SearchReceiveTrackItems(itemNo,isTransaction,transactionId,type);
                if (trackingInfoDt.Rows.Count > 0)
                {
                    for (int i = 0; i < trackingInfoDt.Rows.Count; i++)
                    {
                        TrackingVM trackingVm = new TrackingVM();
                        trackingVm.ItemNo = trackingInfoDt.Rows[i]["ItemNo"].ToString();
                        trackingVm.Heading1 = trackingInfoDt.Rows[i]["Heading1"].ToString();
                        trackingVm.Heading2 = trackingInfoDt.Rows[i]["Heading2"].ToString();
                        trackingVm.ProductCode = trackingInfoDt.Rows[i]["ProductCode"].ToString();
                        trackingVm.ProductName = trackingInfoDt.Rows[i]["ProductName"].ToString();
                        trackingVm.SaleInvoiceNo = trackingInfoDt.Rows[i]["SaleInvoiceNo"].ToString();
                        trackingVm.TrackingLineNo = trackingInfoDt.Rows[i]["TrackLineNo"].ToString();
                        trackingVm.FinishItemNo = trackingInfoDt.Rows[i]["FinishItemNo"].ToString();
                        trackingVm.IsSale = trackingInfoDt.Rows[i]["IsSale"].ToString();
                        trackingVm.IsIssue = trackingInfoDt.Rows[i]["IsIssue"].ToString();
                        trackingVm.IssueNo = trackingInfoDt.Rows[i]["IssueNo"].ToString();
                        trackingVm.IsReceive = trackingInfoDt.Rows[i]["IsReceive"].ToString();
                        trackingVm.ReceiveNo = trackingInfoDt.Rows[i]["ReceiveNo"].ToString();
                        trackingVm.IsPurchase = trackingInfoDt.Rows[i]["IsPurchase"].ToString();

                        if (type.ToLower() == "Receive".ToLower())
                        {
                            if (trackingVm.IsReceive == "Y" && transactionId == trackingVm.ReceiveNo)
                            {
                                trackingVm.IsSelect = true;
                            }
                        }
                        else
                        {
                            if (trackingVm.IsSale == "Y" && transactionId == trackingVm.SaleInvoiceNo)
                            {
                                trackingVm.IsSelect = true;
                            }
                        }
                        


                        SaleTrackings.Add(trackingVm);
                    }
                }
            //}

                return SaleTrackings;
        }


        public DataTable SearchReceiveTrackItems(string itemNo, string isTransaction, string transactionId, string type, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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

                #region sql statement
                #region Parameter

                #endregion

                sqlText = "";
                sqlText += @"

--Declare @itemNo varchar(100) 
--Declare @isTransaction varchar(100) 
--Declare @transactionId varchar(100) 

--SET @itemNo='19'
--SET @isTransaction='N'
--SET @transactionId =''

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]

  FROM Trackings t,Products pr
  WHERE t.[ItemNo]=pr.[ItemNo] 
--And (t.IsPurchase='Y' OR t.Post='Y') 
And t.Post='Y' 
AND t.ItemNo=@itemNo

";

                if (type.ToLower()=="receive" )
                {
                    sqlText += " AND (ReturnPurchase = 'N' OR ReturnPurchase IS NULL)";
                    if (string.IsNullOrEmpty(transactionId))
                    {
                        sqlText += " AND (IsReceive LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL)"; //NEW Receive
                    }
                    else
                    {
                        sqlText += " AND ((ReceiveNo = @transactionId )"; //Existing Receive
                        sqlText += " OR (IsReceive LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL))";
                        
                    }
                }
                else if (type.ToLower() == "sale")
                {
                    sqlText += " AND t.ReceivePost='Y'";
                    sqlText += " AND (ReturnReceive = 'N' OR ReturnReceive IS NULL)";
                    if (string.IsNullOrEmpty(transactionId))
                    {
                        sqlText += @"   AND ((IsSale LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL) or saleinvoiceno in(select PreviousSalesInvoiceNo  from salesInvoiceHeaders
where 1=1
and saleType='credit'))";  
                    }
                    else
                    {
                        sqlText += " AND ((SaleInvoiceNo = @transactionId )"; //Existing Receive
                        //sqlText += " OR (IsSale LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL))";
                        sqlText += " OR (IsSale=@isTransaction OR @isTransaction IS NULL))";
                    }
                }
                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

                if (string.IsNullOrEmpty(isTransaction))
                {
                    if (!objCommProduct.Parameters.Contains("@isTransaction"))
                    { objCommProduct.Parameters.AddWithValue("@isTransaction", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isTransaction"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isTransaction"))
                    { objCommProduct.Parameters.AddWithValue("@isTransaction", isTransaction); }
                    else { objCommProduct.Parameters["@isTransaction"].Value = isTransaction; }
                }

                if (string.IsNullOrEmpty(transactionId))
                {
                    if (!objCommProduct.Parameters.Contains("@transactionId"))
                    { objCommProduct.Parameters.AddWithValue("@transactionId", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@transactionId"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@transactionId"))
                    { objCommProduct.Parameters.AddWithValue("@transactionId", transactionId); }
                    else { objCommProduct.Parameters["@transactionId"].Value = transactionId; }
                }


                objCommProduct.CommandTimeout = 500;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally

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

            return dataTable;

        }



        public DataTable SearchExistingTrackingItems(string isReceive, string receiveNo, string isSale, string saleInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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

                #region sql statement
                #region Parameter
                //                Declare @itemNo varchar(100) 
                //Declare @FinishItemNo varchar(100) 
                //Declare @saleInvoiceNo varchar(100) 
                //Declare @isSale varchar(100) 
                //Declare @isReceive varchar(100) 
                //Declare @receiveNo varchar(100) 
                //Declare @issueNo varchar(100) 
                //Declare @isIssue varchar(100)

                //SET @itemNo='36'
                //SET @FinishItemNo =''
                //SET @saleInvoiceNo=''
                //SET @isSale =''
                //SET @isReceive ='N'
                //SET @receiveNo =''
                //SET @issueNo =''
                //SET @isIssue =''
                #endregion


                sqlText = "";
                sqlText += @"

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]

  FROM Trackings t,Products pr

WHERE t.[ItemNo]=pr.[ItemNo] And t.Post='Y'
           AND ((IsReceive =@isReceive )  AND (ReceiveNo= @receiveNo))
		   OR ((IsSale =@isSale ) AND  (SaleInvoiceNo= @saleInvoiceNo))

		  ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

               
                if (string.IsNullOrEmpty(isReceive))
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isReceive"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", isReceive); }
                    else { objCommProduct.Parameters["@isReceive"].Value = isReceive; }
                }

                if (string.IsNullOrEmpty(receiveNo))
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", receiveNo); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = receiveNo; }
                }

                if (string.IsNullOrEmpty(isSale))
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isSale"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", isSale); }
                    else { objCommProduct.Parameters["@isSale"].Value = isSale; }
                }

                if (string.IsNullOrEmpty(saleInvoiceNo))
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = saleInvoiceNo; }
                }

               
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally

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

            return dataTable;

        }
        //currConn to VcurrConn 25-Aug-2020
        public string TrackingUpdate(List<TrackingVM> Trackings, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string retResult = "Fail";
                       
            SqlConnection vcurrConn = VcurrConn;
            if (vcurrConn == null)
            {
                VcurrConn = null;
                Vtransaction = null;
            }
            int transResult = 0;
            string sqlText = "";
           
            #endregion Initializ
            #region Try
            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }
                    Vtransaction = VcurrConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                }
                #endregion open connection and transaction
                #region Tracking

                if (Trackings.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdateImportDyties);
                }

                foreach (var tracking in Trackings.ToList())
                {

                    #region Find Heading1 Existence

                    sqlText = "";
                    sqlText += "Select COUNT(Heading1) from Trackings  ";
                    sqlText += " WHERE ItemNo='" + tracking.ItemNo + "'";
                    sqlText += " AND Heading1 = '" + tracking.Heading1 + "'";

                    SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, VcurrConn);
                    cmdFindHeading1.Transaction = Vtransaction;
                    decimal IDExist = (int)cmdFindHeading1.ExecuteScalar();
                    if (IDExist > 0)
                    {
                        //Update


                        if (tracking.transactionType == "Purchase_Return")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " ReturnType='" + tracking.ReturnType + "',";
                            sqlText += " ReturnPurchase= '" + tracking.ReturnPurchase + "',";
                            sqlText += " ReturnPurchaseID='" + Trackings[0].ReturnPurchaseID + "',";
                            sqlText += " ReturnPurDate='" + tracking.ReturnPurDate + "'";
                            
                            sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                            sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, VcurrConn);
                            cmdReturnUpdate.Transaction = Vtransaction;
                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Receive_Return")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " ReturnType='" + tracking.ReturnType + "',";
                            sqlText += " ReturnReceive= '" + tracking.ReturnReceive + "',";
                            sqlText += " ReturnReceiveID='" + tracking.ReturnReceiveID + "'";
                           
                            sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                            sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, VcurrConn);
                            cmdReturnUpdate.Transaction = Vtransaction;
                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Sale_Return")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " ReturnType='" + tracking.ReturnType + "',";
                            sqlText += " ReturnSale= '" + tracking.ReturnSale + "',";
                            sqlText += " ReturnSaleID='" + tracking.ReturnSaleID + "'";

                            sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                            sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, VcurrConn);
                            cmdReturnUpdate.Transaction = Vtransaction;
                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Receive")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " IsReceive='" + tracking.IsReceive + "',";
                            sqlText += " ReceiveNo= '" + tracking.ReceiveNo + "',";
                            sqlText += " ReceiveDate= '" + tracking.ReceiveDate + "',";
                            //sqlText += " ReceivePost= '" + tracking.Post + "',";
                            sqlText += " FinishItemNo= '" + tracking.FinishItemNo + "'";

                            sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                            sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, VcurrConn);
                            cmdReturnUpdate.Transaction = Vtransaction;
                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Sale")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " IsSale='" + tracking.IsSale + "',";
                            sqlText += " SaleInvoiceNo= '" + tracking.SaleInvoiceNo + "',";
                            //sqlText += " SalePost= '" + tracking.Post + "',";
                            sqlText += " FinishItemNo= '" + tracking.FinishItemNo + "'";

                            sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                            sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, VcurrConn);
                            cmdReturnUpdate.Transaction = Vtransaction;
                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        //else if (tracking.transactionType == "Sale_Return")
                        //{
                        //    #region Update Return
                        //    sqlText = "";
                        //    sqlText += " update Trackings set ";
                        //    sqlText += " ReturnType='" + tracking.ReturnType + "',";
                        //    sqlText += " ReturnSale= '" + tracking.ReturnSale + "',";
                        //    sqlText += " ReturnSaleID='" + tracking.ReturnSaleID + "'";

                        //    sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                        //    sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                        //    SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, currConn);
                        //    cmdReturnUpdate.Transaction = transaction;
                        //    transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                        //    if (transResult <= 0)
                        //    {
                        //        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate,
                        //                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                        //    }
                        //    #endregion Update Return
                        //}
                        else
                        {
                            #region Update
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " IsIssue='" + tracking.IsIssue + "',";
                            sqlText += " IssueNo= '" + tracking.IssueNo + "',";
                            sqlText += " IsReceive='" + tracking.IsReceive + "',";
                            sqlText += " ReceiveNo= '" + tracking.ReceiveNo + "',";
                            sqlText += " ReceiveDate= '" + tracking.ReceiveDate + "',";
                            sqlText += " IsSale='" + tracking.IsSale + "',";
                            sqlText += " SaleInvoiceNo= '" + tracking.SaleInvoiceNo + "',";
                            sqlText += " FinishItemNo= '" + tracking.FinishItemNo + "'";


                            sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                            sqlText += " and Heading1 = '" + tracking.Heading1 + "'";


                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, VcurrConn);
                            cmdInsDetail.Transaction = Vtransaction;
                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion
                        }

                       

                       
                    }


                    #endregion Find Heading1 Existence
                }

                #endregion Tracking
                #region Commit


                if (vcurrConn == null)
                {
                    if (Vtransaction != null)
                    {
                        if (transResult > 0)
                        {
                            Vtransaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResult = "Success";
                
                #endregion SuccessResult
            }

            #endregion
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (vcurrConn == null)
                {
                    if (VcurrConn != null)
                    {
                        if (VcurrConn.State == ConnectionState.Open)
                        {
                            VcurrConn.Close();
                        }
                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResult;
            #endregion Result
        }
        //currConn to VcurrConn 25-Aug-2020
        public string TrackingInsert(List<TrackingVM> Trackings, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string retResult = "Fail";

            SqlConnection vcurrConn = VcurrConn;
            if (vcurrConn == null)
            {
                VcurrConn = null;
                Vtransaction = null;
            }
            
            int transResult = 0;
            string sqlText = "";
            int IDExist = 0;

            #endregion Initializ
            #region Try
            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }
                    Vtransaction = VcurrConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                }
                #endregion open connection and transaction
                #region Tracking

                #region Tracking

                if (Trackings.Count() > 0)
                {
                    foreach (var tracking in Trackings.ToList())
                    {

                        #region Find Heading1 Existence

                        sqlText = "";
                        sqlText += "select COUNT(Heading1) from Trackings WHERE Heading1 = @Heading1 ";
                        SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, VcurrConn);
                        cmdFindHeading1.Transaction = Vtransaction;
                        cmdFindHeading1.Parameters.AddWithValueAndNullHandle("@Heading1", tracking.Heading1);

                         object objIDExist = cmdFindHeading1.ExecuteScalar();
                        if (objIDExist != null)
                        {
                            IDExist = Convert.ToInt32(objIDExist);
                        }

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Requested Tracking Information ( " + tracking.Heading1 + " ) already added.");
                        }

                        #endregion Find Heading1 Existence

                        #region Find Heading2 Existence

                        sqlText = "";
                        sqlText += "select COUNT(Heading2) from Trackings WHERE Heading2 = @Heading2 ";
                        SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, VcurrConn);
                        cmdFindHeading2.Transaction = Vtransaction;
                        cmdFindHeading2.Parameters.AddWithValueAndNullHandle("@Heading2", tracking.Heading2);

                        objIDExist = cmdFindHeading2.ExecuteScalar();
                        if (objIDExist != null)
                        {
                            IDExist = Convert.ToInt32(objIDExist);
                        }

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Requested Tracking Information ( " + tracking.Heading2 + " ) already added.");
                        }

                        #endregion Find Heading2 Existence


                        sqlText = "";
                        sqlText += " insert into Trackings";
                        sqlText += " (";

                        sqlText += " PurchaseInvoiceNo,";
                        sqlText += " ItemNo,";
                        sqlText += " TrackLineNo,";
                        sqlText += " Heading1,";
                        sqlText += " Heading2,";
                        sqlText += " Quantity,";
                        sqlText += " UnitPrice,";
                        
                        sqlText += " IsPurchase,";
                        sqlText += " IsIssue,";
                        sqlText += " IsReceive,";
                        sqlText += " IsSale,";
                        sqlText += " Post,";
                        sqlText += " ReceivePost,";
                        sqlText += " SalePost,";
                        sqlText += " IssuePost,";


                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn";

                        sqlText += " )";
                        sqlText += " values";
                        sqlText += " (";

                        sqlText += "'0',";
                        sqlText += "'" + Trackings[0].ItemNo + "',";
                        sqlText += "" + tracking.TrackingLineNo + ",";
                        sqlText += "'" + tracking.Heading1 + "',";
                        sqlText += "'" + tracking.Heading2 + "',";
                        sqlText += "'" + tracking.Quantity + "',";
                        sqlText += "'" + tracking.UnitPrice + "',";
                        
                        sqlText += "'" + tracking.IsPurchase + "',";
                        sqlText += "'" + tracking.IsIssue + "',";
                        sqlText += "'" + tracking.IsReceive + "',";
                        sqlText += "'" + tracking.IsSale + "',";
                        sqlText += "'Y',";
                        sqlText += "'N',";
                        sqlText += "'N',";
                        sqlText += "'N',";


                        sqlText += "'" + UserInfoVM.UserName + "',";
                        sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        sqlText += "'" + UserInfoVM.UserName + "',";
                        sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                        sqlText += ")";


                        SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, VcurrConn);
                        cmdInsertTrackings.Transaction = Vtransaction;
                        transResult = (int)cmdInsertTrackings.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                    }
                }

                #endregion Tracking

                #endregion Tracking
                #region Commit


                if (vcurrConn == null)
                {
                    if (Vtransaction != null)
                    {
                        if (transResult > 0)
                        {
                            Vtransaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResult = "Success";

                #endregion SuccessResult
            }

            #endregion
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (vcurrConn == null)
                {
                    if (VcurrConn != null)
                    {
                        if (VcurrConn.State == ConnectionState.Open)
                        {
                            VcurrConn.Close();
                        }
                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResult;
            #endregion Result
        }

        public DataTable SearchTrackings(string itemNo, SysDBInfoVMTemp connVM = null) // use for product opening
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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

                #region sql statement
                #region Parameter
                //                Declare @itemNo varchar(100) 
                //Declare @FinishItemNo varchar(100) 
                //Declare @saleInvoiceNo varchar(100) 
                //Declare @isSale varchar(100) 
                //Declare @isReceive varchar(100) 
                //Declare @receiveNo varchar(100) 
                //Declare @issueNo varchar(100) 
                //Declare @isIssue varchar(100)

                //SET @itemNo='36'
                //SET @FinishItemNo =''
                //SET @saleInvoiceNo=''
                //SET @isSale =''
                //SET @isReceive ='N'
                //SET @receiveNo =''
                //SET @issueNo =''
                //SET @isIssue =''
                #endregion


                sqlText = "";
                sqlText += @"
SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]
      ,t.[Quantity]
      ,ISNULL(t.[UnitPrice],0) UnitPrice

  FROM Trackings t,Products pr

WHERE t.[ItemNo]=pr.[ItemNo] And t.Post='Y' 
           AND (t.ItemNo LIKE '%' +  @itemNo + '%' OR @itemNo IS NULL) 
           AND (t.IsPurchase LIKE '%' +  @isPurchase + '%' OR @isPurchase IS NULL) 

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }
                if (!objCommProduct.Parameters.Contains("@isPurchase"))
                { objCommProduct.Parameters.AddWithValue("@isPurchase", "N"); }
                else { objCommProduct.Parameters["@isPurchase"].Value = "N"; }
               

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally

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

            return dataTable;

        }

        public DataTable SearchTrackingForReturn(string transactionType, string itemNo, string transactionID, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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

                #region sql statement



                sqlText = "";
                sqlText += @"

SELECT t.[PurchaseInvoiceNo]
      ,pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[Heading1]
      ,t.[Heading2]
      ,t.[Quantity]
      ,t.[IsPurchase]
      ,t.[Post]
	  ,t.ReturnPurchase
	  ,t.ReturnPurchaseID
	  ,t.ReturnReceive
	  ,t.ReturnReceiveID
	  ,t.ReturnSale
	  ,t.ReturnSaleID
	  ,t.ReturnType
,t.IsIssue
,t.IsReceive
,t.IsSale
,t.ReceiveNo
,t.IssueNo
,t.SaleInvoiceNo
      ,ISNULL(t.[UnitPrice],0) UnitPrice

FROM Trackings t,Products pr
where t.[ItemNo]=pr.[ItemNo]
and t.ItemNo=@itemNo

";

                if (transactionType == "Purchase")
                {
                    sqlText += "and t.PurchaseInvoiceNo = @transactionID ";
                    sqlText += "and t.ISReceive='N' and t.IsSale='N' ";
                }
                else if (transactionType == "Receive")
                {
                    sqlText += "and t.ReceiveNo = @transactionID ";
                    sqlText += "and t.IsSale='N' ";
                }
                else if (transactionType == "Sale")
                {
                    sqlText += "and t.SaleInvoiceNo = @transactionID ";
                    
                }

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;



                if (!objCommProduct.Parameters.Contains("@transactionID"))
                { objCommProduct.Parameters.AddWithValue("@transactionID", transactionID); }
                else { objCommProduct.Parameters["@transactionID"].Value = transactionID; }

                if (string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

                objCommProduct.CommandTimeout = 200;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally

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

            return dataTable;

        }

        public string TrackingDelete(List<string> Headings, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string retResult = "Fail";
            SqlConnection currConn = null;
            int transResult = 0;
            string sqlText = "";

            #endregion Initializ
            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }


                #endregion open connection and transaction
                #region Statement
                if (Headings.Count > 0)
                {
                    for (int i = 0; i < Headings.Count; i++)
                    {
                        sqlText = "";
                        sqlText += " select count(Heading1) from Trackings Where Heading1 = @Headings ";
                        sqlText += " AND IsReceive ='N'";

                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                        cmdExist.Parameters.AddWithValueAndNullHandle("@Headings", Headings[i].ToString());
                        int IDExist = (int)cmdExist.ExecuteScalar();                       

                        if (IDExist > 0)
                        {
                            sqlText = "";
                            sqlText += " Delete Trackings Where Heading1 ='" + Headings[i].ToString() + "' ";
                            sqlText += " AND IsReceive ='N'";

                            SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                            transResult = (int)cmdDelete.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException("DeleteTracking", "Already used in production");
                            }
                            retResult = "Success";
                        }
                        
                    }
                }

                #endregion Statement
            }
            #endregion Try
            #region Catch and Finall

            catch (SqlException sqlex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            #endregion Catch and Finall

            #region Result
            return retResult;
            #endregion Result


        }

        public string TransferIssueTrackingInsert(List<TransferIssueTrackingVM> Trackings, TransferIssueVM Master, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string retResult = "Fail";

            SqlConnection vcurrConn = VcurrConn;
            if (vcurrConn == null)
            {
                VcurrConn = null;
                Vtransaction = null;
            }

            int transResult = 0;
            string sqlText = "";
            int IDExist = 0;

            #endregion Initializ
            #region Try
            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }
                    Vtransaction = VcurrConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                }
                #endregion open connection and transaction
                #region Tracking

                #region Tracking

                if (Trackings.Count() > 0)
                {
                    foreach (var tracking in Trackings.ToList())
                    {

                        #region Find Heading1 Existence

                        sqlText = "";
                        sqlText += "select COUNT(Heading1) from TransferIssueTrackings WHERE Heading1 = @Heading1 ";
                        SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, VcurrConn);
                        cmdFindHeading1.Transaction = Vtransaction;
                        cmdFindHeading1.Parameters.AddWithValueAndNullHandle("@Heading1", tracking.Heading1);

                        object objIDExist = cmdFindHeading1.ExecuteScalar();
                        if (objIDExist != null)
                        {
                            IDExist = Convert.ToInt32(objIDExist);
                        }

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Requested Tracking Information ( " + tracking.Heading1 + " ) already added.");
                        }

                        #endregion Find Heading1 Existence

                        #region Find Heading2 Existence

                        sqlText = "";
                        sqlText += "select COUNT(Heading2) from TransferIssueTrackings WHERE Heading2 = @Heading2 ";
                        SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, VcurrConn);
                        cmdFindHeading2.Transaction = Vtransaction;
                        cmdFindHeading2.Parameters.AddWithValueAndNullHandle("@Heading2", tracking.Heading2);

                        objIDExist = cmdFindHeading2.ExecuteScalar();
                        if (objIDExist != null)
                        {
                            IDExist = Convert.ToInt32(objIDExist);
                        }

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Requested Tracking Information ( " + tracking.Heading2 + " ) already added.");
                        }

                        #endregion Find Heading2 Existence

                        sqlText = "";
                        sqlText += " insert into TransferIssueTrackings(";
                        sqlText += "  ItemNo";
                        sqlText += " ,TrackLineNo";
                        sqlText += " ,Heading1";
                        sqlText += " ,Heading2";
                        sqlText += " ,Quantity";
                        sqlText += " ,TransferIssueNo";
                        sqlText += " ,Post";
                        sqlText += " ,IsTransfer";
                        sqlText += " ,FinishItemNo";
                        sqlText += " ,CreatedBy";
                        sqlText += " ,CreatedOn";
                        sqlText += " ,LastModifiedBy";
                        sqlText += " ,LastModifiedOn";                      
                        sqlText += " ,BranchId";
                        sqlText += " )";
                        sqlText += " values( ";
                        sqlText += "@ItemNo";
                        sqlText += ",@TrackLineNo";
                        sqlText += ",@Heading1";
                        sqlText += ",@Heading2";
                        sqlText += ",@Quantity";
                        sqlText += ",@TransferIssueNo";
                        sqlText += ",@Post";
                        sqlText += ",@IsTransfer";
                        sqlText += ",@FinishItemNo";
                        sqlText += ",@CreatedBy";
                        sqlText += ",@CreatedOn";
                        sqlText += ",@LastModifiedBy";
                        sqlText += ",@LastModifiedOn";
                        sqlText += ",@BranchId";
                        sqlText += ")";

                        SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, VcurrConn);
                        cmdInsertTrackings.Transaction = Vtransaction;

                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@ItemNo", tracking.ItemNo);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@TrackLineNo", tracking.TrackingLineNo);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@Heading1", tracking.Heading1);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@Heading2", tracking.Heading2);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@Quantity", tracking.Quantity);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", tracking.TransferIssueNo);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@Post", "N");
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@IsTransfer", "N");
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@FinishItemNo", tracking.FinishItemNo);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                        cmdInsertTrackings.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                      
                        //SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, VcurrConn);
                        //cmdInsertTrackings.Transaction = Vtransaction;
                        transResult = (int)cmdInsertTrackings.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                    }
                }

                #endregion Tracking

                #endregion Tracking
                #region Commit


                if (vcurrConn == null)
                {
                    if (Vtransaction != null)
                    {
                        if (transResult > 0)
                        {
                            Vtransaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResult = "Success";

                #endregion SuccessResult
            }

            #endregion
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (vcurrConn == null)
                {
                    if (VcurrConn != null)
                    {
                        if (VcurrConn.State == ConnectionState.Open)
                        {
                            VcurrConn.Close();
                        }
                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResult;
            #endregion Result
        }
       

    }
}
