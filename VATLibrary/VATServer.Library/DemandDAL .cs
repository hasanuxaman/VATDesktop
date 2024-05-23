using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Interface;

namespace VATServer.Library
{
    public class DemandDAL : IDemand
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

        public DataTable SearchDemandHeaderDTNew(string DemandNo, string DemandDateFrom, string DemandDateTo, string TransactionType, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null)
        { 
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("demandSearchHeader");

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

                sqlText = " ";
                sqlText = @" SELECT  
                            DemandHeaders.DemandNo,
                            convert (varchar,DemandHeaders.DemandDateTime,120)DemandDate,
                            isnull(DemandHeaders.FiscalYear,'')FiscalYear,
                            isnull(DemandHeaders.MonthFrom,'')MonthFrom,
							isnull(DemandHeaders.MonthTo,'')MonthTo,
                            isnull(DemandHeaders.TotalQty,0)TotalQty ,
							isnull(DemandHeaders.Post,'N')Post,
							isnull(DemandHeaders.DemandReceiveID,'N/A')DemandReceiveID,
							convert (varchar,DemandHeaders.DemandReceiveDate,120)ReceiveDate,
							convert (varchar,DemandHeaders.RefDate,120)RefDate,
							isnull(DemandHeaders.RefNo,'N/A')RefNo,
							isnull(DemandHeaders.VehicleID,'N/A')VehicleID,
							isnull(Vehicles.VehicleType,'N/A')VehicleType,
							isnull(Vehicles.VehicleNo,'N/A')VehicleNo,
							isnull(Vehicles.DriverName,'N/A')DriverName,
                            isnull(DemandHeaders.BranchId, 0) BranchId

                
                            FROM DemandHeaders LEFT OUTER JOIN Vehicles ON DemandHeaders.VehicleID=Vehicles.VehicleID

                            WHERE

                            (DemandNo  LIKE '%' +  @DemandNo   + '%' OR @DemandNo IS NULL) 
                            AND (DemandDateTime>= @DemandDateFrom OR @DemandDateFrom IS NULL)
                            AND (DemandDateTime <dateadd(d,1, @DemandDateTo) OR @DemandDateTo IS NULL)
                            AND (Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            ";
               
                sqlText += " AND (transactionType='" + TransactionType + "') ";

                if (BranchId > 0)
                {
                    sqlText = sqlText + @" and DemandHeaders.BranchId = @BranchId";
                }
                sqlText += " order by DemandHeaders.DemandNo";

               
                #endregion

                #region SQL Command

                SqlCommand objCommdemandHeader = new SqlCommand();
                objCommdemandHeader.Connection = currConn;

                objCommdemandHeader.CommandText = sqlText;
                objCommdemandHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommdemandHeader.Parameters.Contains("@Post"))
                { objCommdemandHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommdemandHeader.Parameters["@Post"].Value = Post; }

                if (!objCommdemandHeader.Parameters.Contains("@demandNo"))
                { objCommdemandHeader.Parameters.AddWithValue("@demandNo", DemandNo); }
                else { objCommdemandHeader.Parameters["@demandNo"].Value = DemandNo; }

                if (DemandDateFrom == "")
                {
                    if (!objCommdemandHeader.Parameters.Contains("@demandDateFrom"))
                    { objCommdemandHeader.Parameters.AddWithValue("@demandDateFrom", System.DBNull.Value); }
                    else { objCommdemandHeader.Parameters["@demandDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommdemandHeader.Parameters.Contains("@demandDateFrom"))
                    { objCommdemandHeader.Parameters.AddWithValue("@demandDateFrom", DemandDateFrom); }
                    else { objCommdemandHeader.Parameters["@demandDateFrom"].Value = DemandDateFrom; }
                }
                if (DemandDateTo == "")
                {
                    if (!objCommdemandHeader.Parameters.Contains("@demandDateTo"))
                    { objCommdemandHeader.Parameters.AddWithValue("@demandDateTo", System.DBNull.Value); }
                    else { objCommdemandHeader.Parameters["@demandDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommdemandHeader.Parameters.Contains("@demandDateTo"))
                    { objCommdemandHeader.Parameters.AddWithValue("@demandDateTo", DemandDateTo); }
                    else { objCommdemandHeader.Parameters["@demandDateTo"].Value = DemandDateTo; }
                }
                if (BranchId > 0)
                {
                    objCommdemandHeader.Parameters.AddWithValue("@BranchId", BranchId);
                }


                //if (!objCommdemandHeader.Parameters.Contains("@transactionType"))
                //{ objCommdemandHeader.Parameters.AddWithValue("@transactionType", TransactionType); }
                //else { objCommdemandHeader.Parameters["@transactionType"].Value = TransactionType; }



                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommdemandHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DemandDAL", "SearchDemandHeaderDTNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DemandDAL", "SearchDemandHeaderDTNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        //
        public DataTable SearchDemandDetailDTNew(string DemandNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("demandSearchDetail");

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

                sqlText = @"
SELECT  
d.demandNo, 
d.demandLineNo,
isnull(p.ProductName,'N/A')ProductName,
isnull(p.ProductCode,'N/A')ProductCode,

isnull(NULLIF(bp.PackagingId,''),'')PackagingId,
isnull(NULLIF(pii.PackagingNature,''),'')PackagingName,
isnull(NULLIF(pii.PackagingCapacity,''),'')PackagingSize,
isnull(NULLIF(pii.UOM,''),'')PackagingUom,

isnull(NULLIF(bp.BanderolId,''),'')BanderolId,
isnull(NULLIF(b.BanderolName,''),'')BanderolName,
isnull(NULLIF(b.BanderolSize,''),'')BanderolSize,
isnull(NULLIF(b.UOM,''),'')BanderolUom,

isnull(d.Quantity,0)Quantity ,
isnull(d.UOM,'N/A')UOM ,
isnull(d.DemandQty,0)DemandQty ,

isnull(d.NBRPrice,0)NBRPrice,
isnull(d.Comments,'N/A')Comments,
d.BandProductId,bp.ItemNo,d.DemandReceiveID

FROM dbo.demandDetails d  left outer join
BanderolProducts bp on d.BandProductId=bp.BandProductId Left Outer Join Products p
on bp.ItemNo=p.ItemNo Left outer Join Banderols b
on bp.BanderolId=b.BanderolID Left outer join PackagingInformations pii
on bp.PackagingId=pii.PackagingId 

   WHERE 
(demandNo = @demandNo ) 
order by DemandNo 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommdemandDetail = new SqlCommand();
                objCommdemandDetail.Connection = currConn;

                objCommdemandDetail.CommandText = sqlText;
                objCommdemandDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommdemandDetail.Parameters.Contains("@demandNo"))
                { objCommdemandDetail.Parameters.AddWithValue("@demandNo", DemandNo); }
                else { objCommdemandDetail.Parameters["@demandNo"].Value = DemandNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommdemandDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DemandDAL", "SearchDemandDetailDTNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DemandDAL", "SearchDemandDetailDTNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        //
        public decimal BanderolStock(string itemNo, string BandProductID, string tranDate, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("Banderol", "There is No data to find Banderol");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

               
                #region STOCK
                sqlText = "  ";
                sqlText +=
                    @"

 --   DECLARE @BandProductID varchar(20)
 --   DECLARE @tranDate datetime

 --   SET @BandProductID ='2'
 --   SET @tranDate='2014-04-01'

                    DECLARE @ItemNo varchar(20)
                    DECLARE @BanderolID varchar(20)

                    Select @ItemNo = itemNo,@BanderolID = BanderolID from BanderolProducts where BandProductID = @BandProductID

                    SELECT ISNULL(SUM(ISNULL(Quantity,0)),0) Stock
                    FROM (
                    Select ISNULL(OpeningQty,0) Quantity  from BanderolProducts where BandProductID = @BandProductID and ItemNo=@ItemNo  and ActiveStatus = 'Y'

                    UNION ALL

                    Select ISNULL(SUM(ISNULL(Quantity,0)),0) DemandQty from DemandDetails where BandProductID = @BandProductID and Post = 'Y' 
                    and TransactionType = 'Receive' and TransactionDate <= @tranDate and ItemNo=@ItemNo 
 
                    UNION ALL

                    Select -ISNULL(Sum(ISNULL(s.Quantity,0))* ISNULL(bp.BUsedQty,0),0) as UsedQty 
                    from BanderolProducts bp LEFT OUTER JOIN SalesInvoiceDetails s on bp.ItemNo = s.ItemNo 
                    where bp.BanderolId = @BanderolID and bp.ActiveStatus = 'Y' and bp.ItemNo = @ItemNo
                    and s.Post = 'Y'  and s.InvoiceDateTime <= @tranDate
                    group by bp.BUsedQty

                    UNION ALL

                    Select -ISNULL(Sum(ISNULL(s.Quantity,0))* ISNULL(bp.BUsedQty,0) * ISNULL(bp.WastageQty,0),0) as WastageQty 
                    from BanderolProducts bp LEFT OUTER JOIN SalesInvoiceDetails s on bp.ItemNo = s.ItemNo 
                    where bp.BanderolId = @BanderolID and bp.ActiveStatus = 'Y' and bp.ItemNo = @ItemNo
                    and s.Post = 'Y'  and s.InvoiceDateTime <= @tranDate
                    group by bp.BUsedQty,bp.WastageQty

) as a ";



                SqlCommand cmdBandeStock = new SqlCommand();
                cmdBandeStock.Connection = currConn;
                cmdBandeStock.CommandText = sqlText;
                cmdBandeStock.CommandType = CommandType.Text;
                if (!cmdBandeStock.Parameters.Contains("@BandProductID"))
                {
                    cmdBandeStock.Parameters.AddWithValue("@BandProductID", BandProductID);
                }
                else
                {
                    cmdBandeStock.Parameters["@BandProductID"].Value = BandProductID;
                }
                if (!cmdBandeStock.Parameters.Contains("@tranDate"))
                {
                    cmdBandeStock.Parameters.AddWithValue("@tranDate", tranDate);
                }
                else
                {
                    cmdBandeStock.Parameters["@tranDate"].Value = tranDate;
                }

                //cmdBandeStock.Transaction = transaction;
                if (cmdBandeStock.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdBandeStock.ExecuteScalar();
                }

                #endregion STOCK

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DemandDAL", "BanderolStock", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DemandDAL", "BanderolStock", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Results

            return retResults;

            #endregion

        }

        //
        public DataTable DemandQty(string demandId, string bandeProId, string post, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            DataTable retResults = new DataTable();
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Return Qty

                sqlText = "  ";
                sqlText +=
                    @" Select DemandDetails.TransactionDate, ISNULL(DemandDetails.Quantity,0) DemandQty from DemandDetails";
                sqlText += " where BandProductId = @bandeProId and DemandNo = @demandId and Post = @post";

                //sqlText += " where BandProductId = '" + bandeProId + "' and DemandNo = '" + demandId + "' and Post = '" + post + "'";

                //BugsDD
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@bandeProId", SqlDbType.VarChar, 250);
                parameter.Value = bandeProId;
                cmdExist.Parameters.Add(parameter);
                parameter = new SqlParameter("@demandId", SqlDbType.VarChar, 250);
                parameter.Value = demandId;
                cmdExist.Parameters.Add(parameter);
                parameter = new SqlParameter("@post", SqlDbType.VarChar, 250);
                parameter.Value = post;
                cmdExist.Parameters.Add(parameter);
                
                //SqlCommand cmd = new SqlCommand(sqlText, currConn);
                //if (cmd.ExecuteScalar() == null)
                //{
                //    retResults = 0;
                //}
                //else
                //{
                //    retResults = (decimal)cmd.ExecuteScalar();
                //}


                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLastLIFOPriceNInvNo", "No row found ");
                }
                retResults = dataTable;
                
                #endregion Return Qty

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DemandDAL", "DemandQty", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DemandDAL", "DemandQty", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Results

            return retResults;

            #endregion
        }

        #region need to parameterize done

        //currConn to VcurrConn 25-Aug-2020
        public string[] DemandInsert(DemandMasterVM Master, List<DemandDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string vehicleId = "0";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            ////SqlConnection vcurrConn = VcurrConn;
            ////if (vcurrConn == null)
            ////{
            ////    VcurrConn = null;
            ////    Vtransaction = null;
            ////}

            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DemandDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DemandDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, "Please Check demand Data and Time");

                }


                #endregion Validation for Header

                #region Old connection

                #region open connection and transaction

                ////if (vcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }

                ////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.demandMsgMethodNameInsert);
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction(MessageVM.demandMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Fiscal Year Check

                //string transactionDate = Master.DemandDateTime;
                string transactionYearCheck = "";
                if (Master.TransactionType == "Demand")
                {
                    transactionYearCheck = Convert.ToDateTime(Master.DemandDateTime).ToString("yyyy-MM-dd");
                }
                else
                {
                    transactionYearCheck = Convert.ToDateTime(Master.ReceiveDate).ToString("yyyy-MM-dd");
                }

                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find Fiscal Year

                sqlText = "";
                sqlText = sqlText + "select distinct SUBSTRING(FiscalYearName,8,4)+SUBSTRING(FiscalYearName,22,6) AS FYear from fiscalyear ";
                sqlText += " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd"; ;
                SqlCommand cmdExistYear = new SqlCommand(sqlText, currConn);
                cmdExistYear.Transaction = transaction;
                string fiscalName = (string)cmdExistYear.ExecuteScalar();

                if (fiscalName != null)
                {
                    Master.FiscalYear = fiscalName.ToString();
                }

                #endregion Find Fiscal Year

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DemandNo) from DemandHeaders WHERE DemandNo=@MasterDemandNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Demand ID Create
                if (string.IsNullOrEmpty(Master.TransactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Demand ID Create For Other

                CommonDAL commonDal = new CommonDAL();

                if (Master.TransactionType == "Demand")
                {
                    newID = commonDal.TransactionCode("Demand", "Other", "DemandHeaders", "DemandNo",
                                              "DemandDateTime ", Master.DemandDateTime, Master.BranchId.ToString(), currConn, transaction);


                }
                if (Master.TransactionType == "DemandReturn")
                {
                    newID = commonDal.TransactionCode("Demand", "DemandReturn", "DemandHeaders", "DemandNo",
                                              "DemandDateTime ", Master.DemandDateTime, Master.BranchId.ToString(), currConn, transaction);


                }
                if (Master.TransactionType == "Receive")
                {
                    newID = commonDal.TransactionCode("Demand", "Receive", "DemandHeaders", "DemandNo",
                                              "DemandReceiveDate ", Master.ReceiveDate, Master.BranchId.ToString(), currConn, transaction);


                }

                #endregion Demand ID Create For Other



                #endregion Demand ID Create Not Complete

                #region VehicleId
                string vehicleID = "0";
                if (!string.IsNullOrEmpty(Master.VehicleNo))
                {

                    sqlText = "";
                    sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleNo=@MasterVehicleNo ";

                    SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                    cmdExistVehicleId.Transaction = transaction;
                    cmdExistVehicleId.Parameters.AddWithValueAndNullHandle("@MasterVehicleNo", Master.VehicleNo);

                    vehicleId = (string)cmdExistVehicleId.ExecuteScalar();


                    if (Convert.ToDecimal(vehicleId) <= 0 || string.IsNullOrEmpty(vehicleId))
                    {
                        sqlText = "";
                        sqlText = sqlText + "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles ";
                        SqlCommand cmdVehicleId = new SqlCommand(sqlText, currConn);
                        cmdVehicleId.Transaction = transaction;
                        int NewvehicleID = (int)cmdVehicleId.ExecuteScalar();
                        vehicleID = NewvehicleID.ToString();

                        sqlText = "";
                        sqlText +=
                            " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,DriverName,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
                        sqlText += "values(@vehicleID,";
                        sqlText += " @MasterVehicleType,";
                        sqlText += " @MasterVehicleNo,";
                        sqlText += " @MasterDriverName,";
                        sqlText += " 'NA',";
                        sqlText += " 'NA',";
                        if (Master.VehicleSaveInDB == true)
                        {
                            sqlText += " 'Y',";

                        }
                        else
                        {
                            sqlText += " 'N',";
                        }

                        sqlText += " @MasterCreatedBy ,";
                        sqlText += " @MasterCreatedOn ,";
                        sqlText += " @MasterLastModifiedBy ,";
                        sqlText += " @MasterLastModifiedOn)";
                        //sqlText += " from Vehicles;";

                        SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleIns.Transaction = transaction;

                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@vehicleID", vehicleID);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleType", Master.VehicleType);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleNo", Master.VehicleNo);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterDriverName", Master.DriverName);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);

                        transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            MessageVM.saleMsgUnableCreatID);
                        }
                        vehicleId = vehicleID.ToString();
                        if (string.IsNullOrEmpty(vehicleId))
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                        }
                    }
                    else
                    {
                        sqlText = "";
                        sqlText += " Update Vehicles Set DriverName = ";
                        sqlText += " @MasterDriverName ";
                        sqlText += " WHERE VehicleNo=@MasterVehicleNo ";

                        SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleIns.Transaction = transaction;
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterDriverName", Master.DriverName);
                        cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleNo", Master.VehicleNo);

                        transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            MessageVM.saleMsgUnableCreatID);
                        }
                    }
                }




                #endregion VehicleId

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into DemandHeaders(";
                sqlText += " DemandNo,";
                sqlText += " DemandDateTime,";
                sqlText += " FiscalYear,";
                sqlText += " MonthFrom,";
                sqlText += " MonthTo,";
                sqlText += " TotalQty,";
                sqlText += " DemandReceiveDate,";
                sqlText += " RefDate,";
                sqlText += " RefNo,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " TransactionType,";
                sqlText += " DemandReceiveID,";
                sqlText += " VehicleID,";
                sqlText += " BranchId,";
                sqlText += " Post";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterDemandDateTime,";
                sqlText += "@MasterFiscalYear,";
                sqlText += "@MasterMonthFrom,";
                sqlText += "@MasterMonthTo,";
                sqlText += "@MasterTotalQty,";
                sqlText += "@MasterReceiveDate,";
                sqlText += "@MasterRefDate,";
                sqlText += "@MasterRefNo,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterDemandReceiveID,";
                sqlText += "@vehicleId,";
                sqlText += "@BranchId,";
                sqlText += "@MasterPost";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDemandDateTime", Master.DemandDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterFiscalYear", Master.FiscalYear);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterMonthFrom", Master.MonthFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterMonthTo", Master.MonthTo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalQty", Master.TotalQty);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReceiveDate", Master.ReceiveDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterRefDate", Master.RefDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterRefNo", Master.RefNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDemandReceiveID", Master.DemandReceiveID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@vehicleId", vehicleId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(DemandNo) from DemandDetails WHERE DemandNo='" + newID + "' ";
                    sqlText += " AND BandProductId =@ItemBandProductId";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemBandProductId", Item.BandProductId);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into DemandDetails(";
                    sqlText += " DemandNo,";
                    sqlText += " DemandLineNo,";
                    sqlText += " BandProductId,";
                    sqlText += " ItemNo,";
                    sqlText += " Quantity,";
                    sqlText += " UOM,";
                    sqlText += " DemandQty,";
                    sqlText += " NBRPrice,";
                    sqlText += " TransactionDate,";
                    sqlText += " Comments,";
                    sqlText += " DemandReceiveID,";
                    sqlText += " TransactionType,";
                    sqlText += " VehicleID,";
                    sqlText += " BranchId,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";

                    sqlText += " Post";
                    sqlText += " )";

                    sqlText += " values(	";

                    sqlText += "@newID,";
                    sqlText += "@ItemDemandLineNo,";
                    sqlText += "@ItemBandProductId,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemDemandQty,";
                    sqlText += "@ItemNBRPrice,";
                    sqlText += "@ItemTransactionDate,";
                    sqlText += "@ItemComments,";
                    sqlText += "@MasterDemandReceiveID,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@vehicleId,";
                    sqlText += "@BranchId,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDemandLineNo", Item.DemandLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBandProductId", Item.BandProductId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDemandQty", Item.DemandQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTransactionDate", Item.TransactionDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemComments", Item.Comments);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDemandReceiveID", Master.DemandReceiveID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@vehicleId", vehicleId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.DemandHeaders WHERE DemandNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameInsert, MessageVM.demandMsgUnableCreatID);
                }

                #endregion Prefetch

                #region old Commit

                ////if (VcurrConn == null)
                ////{
                ////    if (Vtransaction != null)
                ////    {
                ////        if (transResult > 0)
                ////        {
                ////            Vtransaction.Commit();
                ////        }
                ////    }
                ////}

                #endregion Commit

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = MessageVM.demandMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                Vtransaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DemandDAL", "DemandInsert", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                Vtransaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DemandDAL", "DemandInsert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion Catch

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                ////if (vcurrConn == null)
                ////{
                ////    if (VcurrConn != null)
                ////    {
                ////        if (VcurrConn.State == ConnectionState.Open)
                ////        {
                ////            VcurrConn.Close();
                ////        }
                ////    }
                ////}

               

            }

            #endregion Finally

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }
        //
        public string[] DemandUpdate(DemandMasterVM Master, List<DemandDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DemandDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DemandDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.demandMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                //string transactionDate = Master.DemandDateTime;
                //string transactionYearCheck = Convert.ToDateTime(Master.DemandDateTime).ToString("yyyy-MM-dd");
                string transactionYearCheck = "";
                if (Master.TransactionType == "Demand")
                {
                    transactionYearCheck = Convert.ToDateTime(Master.DemandDateTime).ToString("yyyy-MM-dd");
                }
                else
                {
                    transactionYearCheck = Convert.ToDateTime(Master.ReceiveDate).ToString("yyyy-MM-dd");
                }
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find Fiscal Year

                sqlText = "";
                sqlText = sqlText + "select distinct SUBSTRING(FiscalYearName,8,4)+SUBSTRING(FiscalYearName,22,6) AS FYear from fiscalyear ";
                sqlText += " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd"; ;
                SqlCommand cmdExistYear = new SqlCommand(sqlText, currConn);
                cmdExistYear.Transaction = transaction;
                string fiscalName = (string)cmdExistYear.ExecuteScalar();

                if (fiscalName != null)
                {
                    Master.FiscalYear = fiscalName.ToString();
                }

                #endregion Find Fiscal Year

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DemandNo) from DemandHeaders WHERE DemandNo=@MasterDemandNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DemandHeaders set  ";

                sqlText += " DemandDateTime     = @MasterDemandDateTime ,";
                sqlText += " FiscalYear         = @MasterFiscalYear ,";
                sqlText += " MonthFrom          = @MasterMonthFrom ,";
                sqlText += " MonthTo            = @MasterMonthTo ,";
                sqlText += " TotalQty           = @MasterTotalQty ,";
                sqlText += " DemandReceiveDate  = @MasterReceiveDate ,";
                sqlText += " RefDate            = @MasterRefDate ,";
                sqlText += " RefNo              = @MasterRefNo ,";
                sqlText += " BranchId           = @BranchId ,";
                sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                sqlText += " TransactionType    = @MasterTransactionType ,";
                sqlText += " Post               = @MasterPost ";
                sqlText += " where  DemandNo    = @MasterDemandNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDemandDateTime", Master.DemandDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterFiscalYear", Master.FiscalYear);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterMonthFrom", Master.MonthFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterMonthTo", Master.MonthTo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalQty", Master.TotalQty);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveDate", Master.ReceiveDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterRefDate", Master.RefDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterRefNo", Master.RefNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(DemandNo) from DemandDetails WHERE DemandNo=@MasterDemandNo ";
                    sqlText += " AND BandProductId='" + Item.BandProductId + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into DemandDetails(";

                        sqlText += " DemandNo,";
                        sqlText += " DemandLineNo,";
                        sqlText += " BandProductId,";
                        //sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " NBRPrice,";

                        sqlText += " UOM,";
                        sqlText += " Comments,";
                        sqlText += " TransactionDate,";
                        sqlText += " BranchId,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";

                        sqlText += " TransactionType,";
                        sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";

                        sqlText += "@MasterDemandNo,";
                        sqlText += "@ItemDemandLineNo,";
                        sqlText += "@ItemBandProductId,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemComments,";
                        sqlText += "@ItemTransactionDate,";
                        sqlText += "@BranchId,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterPost";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterDemandNo", Master.DemandNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemDemandLineNo", Item.DemandLineNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemBandProductId", Item.BandProductId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemComments", Item.Comments);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("ItemTransactionDate", Item.TransactionDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("BranchId", Item.BranchId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterTransactionType", Master.TransactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("MasterPost", Master.Post);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update DemandDetails set ";

                        sqlText += " DemandLineNo       =@ItemDemandLineNo,";
                        sqlText += " Quantity           =@ItemQuantity,";
                        sqlText += " NBRPrice           =@ItemNBRPrice,";
                        sqlText += " DemandQty          =@ItemDemandQty,";
                        sqlText += " UOM                =@ItemUOM,";
                        sqlText += " Comments           =@ItemComments,";
                        sqlText += " BranchId           =@BranchId,";
                        sqlText += " TransactionDate    =@ItemTransactionDate,";
                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                        sqlText += " TransactionType    =@MasterTransactionType,";
                        sqlText += " Post               =@MasterPost";
                        sqlText += " where  DemandNo    =@MasterDemandNo ";
                        sqlText += " and BandProductId  =@ItemBandProductId";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDemandLineNo", Item.DemandLineNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDemandQty", Item.DemandQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemComments", Item.Comments);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTransactionDate", Item.TransactionDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBandProductId", Item.BandProductId);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct BandProductId";
                sqlText += " from DemandDetails WHERE DemandNo=@MasterDemandNo ";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["BandProductId"].ToString();

                    var tt = Details.Count(x => x.BandProductId.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM DemandDetails ";
                        sqlText += " WHERE DemandNo=@MasterDemandNo ";
                        sqlText += " AND BandProductId='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DemandHeaders WHERE DemandNo=@MasterDemandNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNameUpdate, MessageVM.demandMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.demandMsgUpdateSuccessfully;
                retResults[2] = Master.DemandNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DemandDAL", "DemandUpdate", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DemandDAL", "DemandUpdate", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
            return retResults;
            #endregion Result

        }
        //
        public string[] DemandPost(DemandMasterVM Master, List<DemandDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";


            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.DemandDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DemandDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.demandMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DemandDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.DemandDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DemandNo) from DemandHeaders WHERE DemandNo=@MasterDemandNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DemandHeaders set  ";
                sqlText += " LastModifiedBy = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn = @MasterLastModifiedOn ,";
                sqlText += " Post           = @MasterPost ";
                sqlText += " where DemandNo = @MasterDemandNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgPostNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(DemandNo) from DemandDetails WHERE DemandNo=@MasterDemandNo ";
                    sqlText += " AND BandProductId=@ItemBandProductId ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemBandProductId", Item.BandProductId);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update DemandDetails set ";
                        sqlText += " Post               =@MasterPost";
                        sqlText += " where  DemandNo    =@MasterDemandNo ";
                        sqlText += " and BandProductId  =@ItemBandProductId ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBandProductId", Item.BandProductId);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                        #region Update Item Qty
                        else
                        {
                            #region Find Quantity From Products
                            //ProductDAL productDal = new ProductDAL();
                            //decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.BandProductId, Master.DemandDateTime,
                            //                                  currConn, transaction,true).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            //sqlText = "";
                            //sqlText += "select isnull(Quantity ,0) from DemandDetails ";
                            //sqlText += " WHERE BandProductId='" + Item.BandProductId + "' and DemandNo= '" + Master.DemandNo + "'";
                            //SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                            //cmdTranQty.Transaction = transaction;
                            //decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                            //#endregion Find Quantity From Transaction

                            //#region Qty  check and Update
                            //if (NegStockAllow == false)
                            //{
                            //    if (TranQty > (oldStock + TranQty))
                            //    {
                            //        throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost,
                            //                                        MessageVM.demandMsgStockNotAvailablePost);
                            //    }
                            //}


                            #endregion Qty  check and Update
                        }

                        #endregion Qty  check and Update
                    }
                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DemandHeaders WHERE DemandNo=@MasterDemandNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterDemandNo", Master.DemandNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.demandMsgMethodNamePost, MessageVM.demandMsgPostNotSelect);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.demandMsgSuccessfullyPost;
                retResults[2] = Master.DemandNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DemandDAL", "DemandPost", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DemandDAL", "DemandPost", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
            return retResults;
            #endregion Result

        }
        //
        public decimal ReturnQty(string demandId, string bandeProId, string post, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Return Qty

                sqlText = "  ";
                sqlText +=
                    @" SELECT SUM(ISNULL(demand.DemandQty,0)) Quantity FROM
                       ( select ISNULL(DemandDetails.Quantity,0) DemandQty from DemandDetails";
                sqlText += " where BandProductId =@bandeProId and DemandNo =@demandId and Post =@post";
                sqlText +=
                    @" UNION ALL 
                             select -SUM(ISNULL(DemandDetails.Quantity,0)) ReceiveQty from DemandDetails ";
                sqlText += " where BandProductId = @bandeProId  and DemandReceiveID = @demandId and Post =@post";
                sqlText += "group by BandProductId ";
                sqlText += ") as demand ";






                //sqlText = " select Sum(isnull(DemandDetails.Quantity,0)) from DemandDetails ";
                //sqlText +=" where BandProductId = '" + bandeProId + "' and DemandReceiveID = '" + demandId + "' and Post = 'Y'";
                //sqlText +=" group by BandProductId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValueAndNullHandle("@demandId", demandId);
                cmd.Parameters.AddWithValueAndNullHandle("@post", post);
                cmd.Parameters.AddWithValueAndNullHandle("@demandId", demandId);
                if (cmd.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmd.ExecuteScalar();
                }

                #endregion Return Qty

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DemandDAL", "ReturnQty", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DemandDAL", "ReturnQty", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Results

            return retResults;

            #endregion
        }

        #endregion
    }
}

