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
    public class DisposeDAL : IDispose
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        
        #region Search
        public DataTable SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("DisposeSearch");

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
                            DisposeNumber, convert (varchar,DisposeDate,120)DisposeDate,
                            isnull(RefNumber,'NA')RefNumber,
                            isnull(Remarks,'NA')Remarks,
                            isnull(TransactionType,'NA')TransactionType,
                            isnull(Post,'N')Post,
                            isnull(AppTotalPrice,'0')AppTotalPrice,
                            isnull(AppVATAmount,'0')AppVATAmount,
                            isnull(convert (varchar,AppDate,120),DisposeDate)AppDate,
                            isnull(AppRefNumber,'NA')AppRefNumber,
                            isnull(AppRemarks,'NA')AppRemarks,
                            isnull(AppVATAmountImport,'0')AppVATAmountImport,
                            isnull(AppTotalPriceImport,'0')AppTotalPriceImport,
                            isnull(BranchId,'0')BranchId

                            FROM         dbo.DisposeHeaders
                            WHERE 
                            (DisposeNumber  LIKE '%' +  @DisposeNumber   + '%' OR @DisposeNumber IS NULL) 
                            AND (DisposeDate>= @DisposeDateFrom OR @DisposeDateFrom IS NULL)
                            AND (DisposeDate <dateadd(d,1, @DisposeDateTo) OR @DisposeDateTo IS NULL)
                            AND (Post LIKE '%' + @Post + '%' OR @Post IS NULL)
                            AND (TransactionType=@TransactionType)";
                #endregion

                #region SQL Command

                if (BranchId > 0)
                {
                    sqlText = sqlText + @" and BranchId = @BranchId";
                }
                SqlCommand objCommDisposeHeader = new SqlCommand();
                objCommDisposeHeader.Connection = currConn;

                objCommDisposeHeader.CommandText = sqlText;
                objCommDisposeHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter
                if (BranchId > 0)
                {
                    objCommDisposeHeader.Parameters.AddWithValue("@BranchId", BranchId);
                }
                if (!objCommDisposeHeader.Parameters.Contains("@Post"))
                { objCommDisposeHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommDisposeHeader.Parameters["@Post"].Value = Post; }

                if (!objCommDisposeHeader.Parameters.Contains("@DisposeNumber"))
                { objCommDisposeHeader.Parameters.AddWithValue("@DisposeNumber", DisposeNumber); }
                else { objCommDisposeHeader.Parameters["@DisposeNumber"].Value = DisposeNumber; }
                if (DisposeDateFrom == "")
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateFrom"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateFrom", System.DBNull.Value); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateFrom"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateFrom", DisposeDateFrom); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateFrom"].Value = DisposeDateFrom; }
                }
                if (DisposeDateTo == "")
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateTo"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateTo", System.DBNull.Value); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateTo"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateTo", DisposeDateTo); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateTo"].Value = DisposeDateTo; }
                }


                if (!objCommDisposeHeader.Parameters.Contains("@transactionType"))
                { objCommDisposeHeader.Parameters.AddWithValue("@transactionType", transactionType); }
                else { objCommDisposeHeader.Parameters["@transactionType"].Value = transactionType; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommDisposeHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                FileLogger.Log("DisposeDAL", "SearchDisposeHeaderDTNew", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("DisposeDAL", "SearchDisposeHeaderDTNew", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }
        public DataTable SearchDisposeDetailDTNew(string DisposeNumber, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("DisposeSearch");

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
                            select 
LineNumber,
DD.ItemNo ItemNo,
p.ProductName ItemName,
p.ProductCode PCode,
dd.UOM UOM,
isnull(Quantity,0)Quantity,
isnull(QuantityImport,Quantity)QuantityImport,
isnull(RealPrice,0)RealPrice,
isnull(VATAmount,0)VATAmt,
SaleNumber SaleNumber,
PurchaseNumber PurchaseNumber,
isnull(PresentPrice,0)PresentPrice,
isnull(Post,'N')Post,
remarks Comments,
dd.VATRate,0 as Stock 
from DisposeDetails  DD left outer join
products P on DD.ItemNo= p.itemno
     
WHERE 
(DisposeNumber = @DisposeNumber ) ";
                #endregion

                #region SQL Command

                SqlCommand objCommDisposeHeader = new SqlCommand();
                objCommDisposeHeader.Connection = currConn;

                objCommDisposeHeader.CommandText = sqlText;
                objCommDisposeHeader.CommandType = CommandType.Text;
                
                #endregion

                #region Parameter

                if (!objCommDisposeHeader.Parameters.Contains("@DisposeNumber"))
                { objCommDisposeHeader.Parameters.AddWithValue("@DisposeNumber", DisposeNumber); }
                else { objCommDisposeHeader.Parameters["@DisposeNumber"].Value = DisposeNumber; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommDisposeHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                FileLogger.Log("DisposeDAL", "SearchDisposeDetailDTNew", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("DisposeDAL", "SearchDisposeDetailDTNew", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }
        
        #endregion 

        #region need to parameterize
        public string[] DisposeInsert(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null)
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

            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
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
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DisposeDate) < DateTime.MinValue || Convert.ToDateTime(Master.DisposeDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgCheckDate);

                }


                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNameInsert);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DisposeDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DisposeDate).ToString("yyyy-MM-dd");
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

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DisposeNumber) from DisposeHeaders " +
                          " where DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                CommonDAL commonDal = new CommonDAL();


                if (Master.TransactionType == "VAT26")
                {
                    newID = commonDal.TransactionCode("Dispose", "Raw", "DisposeHeaders", "DisposeNumber",
                                              "DisposeDate", Master.DisposeDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "VAT27")
                {
                    newID = commonDal.TransactionCode("Dispose", "Finish", "DisposeHeaders", "DisposeNumber",
                                             "DisposeDate", Master.DisposeDate, Master.BranchId.ToString(), currConn, transaction);
                }



                #endregion Purchase ID Create Not Complete
                #region ID generated completed,Insert new Information in Header

                sqlText = "";
                sqlText += " insert into DisposeHeaders";
                sqlText += " (";

                sqlText += " DisposeNumber,";
                sqlText += " DisposeDate,";
                sqlText += " RefNumber,";
                sqlText += " VATAmount,";
                sqlText += " Remarks,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " TransactionType,";
                sqlText += " Post,";
                sqlText += " FromStock,";
                sqlText += " ImportVATAmount,";
                sqlText += " TotalPrice,";
                sqlText += " TotalPriceImport,";
                sqlText += " AppVATAmount,";
                sqlText += " AppTotalPrice,";
                sqlText += " AppDate,";
                sqlText += " AppRefNumber,";
                sqlText += " AppRemarks,";
                sqlText += " AppVATAmountImport,";
                sqlText += " AppTotalPriceImport,";
                sqlText += " BranchId";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterDisposeDate,";
                sqlText += "@MasterRefNumber,";
                sqlText += "@MasterVATAmount,";
                sqlText += "@MasterRemarks,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterPost,";
                sqlText += "@MasterFromStock,";
                sqlText += "@MasterImportVATAmount,";
                sqlText += "@MasterTotalPrice,";
                sqlText += "@MasterTotalPriceImport,";
                sqlText += "@MasterAppVATAmount,";
                sqlText += "@MasterAppTotalPrice,";
                sqlText += "@MasterAppDate,";
                sqlText += "@MasterAppRefNumber,";
                sqlText += "@MasterAppRemarks,";
                sqlText += "@MasterAppVATAmountImport,";
                sqlText += "@MasterAppTotalPriceImport,";
                sqlText += "@BranchId";

                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDisposeDate", Master.DisposeDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterRefNumber", Master.RefNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterVATAmount", Master.VATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterRemarks", Master.Remarks);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterFromStock", Master.FromStock);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterImportVATAmount", Master.ImportVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalPrice", Master.TotalPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalPriceImport", Master.TotalPriceImport);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppVATAmount", Master.AppVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppTotalPrice", Master.AppTotalPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppDate", Master.AppDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppRefNumber", Master.AppRefNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppRemarks", Master.AppRemarks);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppVATAmountImport", Master.AppVATAmountImport);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterAppTotalPriceImport", Master.AppTotalPriceImport);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);


                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(DisposeNumber) from DisposeDetails" +
                               " WHERE DisposeNumber='" + newID + "' ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    #region USD calculate
                    ReceiveDAL reciveDal = new ReceiveDAL();
                    string[] usdResults = reciveDal.GetUSDCurrency(Item.RealPrice);
                    #endregion USD calculate


                    sqlText = "";
                    sqlText += " insert into DisposeDetails(";
                    sqlText += " DisposeNumber,";
                    sqlText += " LineNumber,";
                    sqlText += " ItemNo,";
                    sqlText += " Quantity,";
                    sqlText += " RealPrice,";
                    sqlText += " VATAmount,";
                    sqlText += " SaleNumber,";
                    sqlText += " PurchaseNumber,";
                    sqlText += " PresentPrice,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " Post,";
                    sqlText += " UOM,";
                    sqlText += " Remarks,";
                    sqlText += " DisposeDate,";
                    sqlText += " QuantityImport,";
                    sqlText += " TransactionType,";
                    sqlText += " FromStock,";
                    sqlText += " VATRate,";
                    sqlText += "DollarPrice";

                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "@ItemLineNumber,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemRealPrice,";
                    sqlText += "@ItemVATAmountD,";
                    sqlText += "@ItemSaleNumber,";
                    sqlText += "@ItemPurchaseNumber,";
                    sqlText += "@ItemPresentPrice,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterPost,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemRemarksD,";
                    sqlText += "@MasterDisposeDate,";
                    sqlText += "@ItemQuantityImport,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterFromStock,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "'" + usdResults[1] + "'";//DollerValue
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLineNumber", Item.LineNumber);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRealPrice", Item.RealPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmountD", Item.VATAmountD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSaleNumber", Item.SaleNumber);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPresentPrice", Item.PresentPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarksD", Item.RemarksD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDisposeDate", Master.DisposeDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantityImport", Item.QuantityImport);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterFromStock", Master.FromStock);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.DisposeHeaders " +
                          " where DisposeNumber='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgUnableCreatID);
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
                retResults[1] = MessageVM.disposeMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{

            //    transaction.Rollback();
            //    throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();

                FileLogger.Log("DisposeDAL", "DisposeInsert", ex.ToString() + "\n" + sqlText);

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] DisposeUpdate(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null)
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
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DisposeDate) < DateTime.MinValue || Convert.ToDateTime(Master.DisposeDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgCheckDate);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DisposeDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DisposeDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(DisposeNumber) from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DisposeHeaders set  ";

                sqlText += " DisposeDate        = @MasterDisposeDate ,";
                sqlText += " RefNumber          = @MasterRefNumber ,";
                sqlText += " VATAmount          = @MasterVATAmount ,";
                sqlText += " Remarks            = @MasterRemarks ,";
                sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                sqlText += " TransactionType    = @MasterTransactionType ,";
                sqlText += " Post               = @MasterPost ,";
                sqlText += " FromStock          = @MasterFromStock ,";
                sqlText += " ImportVATAmount    = @MasterImportVATAmount ,";
                sqlText += " TotalPrice         = @MasterTotalPrice ,";
                sqlText += " TotalPriceImport   = @MasterTotalPriceImport ,";
                sqlText += " AppVATAmount       = @MasterAppVATAmount ,";
                sqlText += " AppRefNumber       = @MasterAppRefNumber ,";
                sqlText += " AppTotalPrice      = @MasterAppTotalPrice ,";
                sqlText += " AppDate            = @MasterAppDate ,";
                sqlText += " AppRemarks         = @MasterAppRemarks ,";
                sqlText += " AppTotalPriceImport= @MasterAppTotalPriceImport ,";
                sqlText += " AppVATAmountImport = @MasterAppVATAmountImport ";
                sqlText += " where DisposeNumber= @MasterDisposeNumber ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDisposeDate", Master.DisposeDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterRefNumber", Master.RefNumber);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterVATAmount", Master.VATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterRemarks", Master.Remarks);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterFromStock", Master.FromStock);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterImportVATAmount", Master.ImportVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalPrice", Master.TotalPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalPriceImport", Master.TotalPriceImport);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppVATAmount", Master.AppVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppRefNumber", Master.AppRefNumber);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppTotalPrice", Master.AppTotalPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppDate", Master.AppDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppRemarks", Master.AppRemarks);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppTotalPriceImport", Master.AppTotalPriceImport);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterAppVATAmountImport", Master.AppVATAmountImport);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region Transaction Not Other


                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += " select COUNT(DisposeNumber) from DisposeDetails WHERE DisposeNumber=@MasterDisposeNumber ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    #region USD calculate
                    ReceiveDAL receiveDal = new ReceiveDAL();
                    string[] usdResults = receiveDal.GetUSDCurrency(Item.RealPrice);
                    #endregion USD calculate


                    if (IDExist <= 0)
                    {
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into DisposeDetails(";
                        sqlText += " DisposeNumber,";
                        sqlText += " LineNumber,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " RealPrice,";
                        sqlText += " VATAmount,";
                        sqlText += " SaleNumber,";
                        sqlText += " PurchaseNumber,";
                        sqlText += " PresentPrice,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " Post,";
                        sqlText += " UOM,";
                        sqlText += " Remarks,";
                        sqlText += " DisposeDate,";
                        sqlText += " QuantityImport,";
                        sqlText += " TransactionType,";
                        sqlText += " FromStock,";
                        sqlText += " VATRate,";
                        sqlText += " DollarPrice";

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterDisposeNumber,";
                        sqlText += "@ItemLineNumber,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemRealPrice,";
                        sqlText += "@ItemVATAmountD,";
                        sqlText += "@ItemSaleNumber,";
                        sqlText += "@ItemPurchaseNumber,";
                        sqlText += "@ItemPresentPrice,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterPost,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemRemarksD,";
                        sqlText += "@MasterDisposeDate,";
                        sqlText += "@ItemQuantityImport,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterFromStock,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "'" + usdResults[1] + "'";//DollerValue
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLineNumber", Item.LineNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRealPrice", Item.RealPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmountD", Item.VATAmountD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSaleNumber", Item.SaleNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPresentPrice", Item.PresentPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarksD", Item.RemarksD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDisposeDate", Master.DisposeDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantityImport", Item.QuantityImport);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterFromStock", Master.FromStock);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update DisposeDetails set ";
                        sqlText += " LineNumber         = @ItemLineNumber,";
                        sqlText += " Quantity           = @ItemQuantity,";
                        sqlText += " RealPrice          = @ItemRealPrice,";
                        sqlText += " VATAmount          = @ItemVATAmountD,";
                        sqlText += " SaleNumber         = @ItemSaleNumber,";
                        sqlText += " PurchaseNumber     = @ItemPurchaseNumber,";
                        sqlText += " PresentPrice       = @ItemPresentPrice,";
                        sqlText += " LastModifiedBy     = @MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn     = @MasterLastModifiedOn,";
                        sqlText += " DisposeDate        = @MasterDisposeDate,";
                        sqlText += " Post               = @MasterPost,";
                        sqlText += " UOM                = @ItemUOM,";
                        sqlText += " VATRate            = @ItemVATRate,";
                        sqlText += " Remarks            = @ItemRemarksD,";
                        sqlText += " TransactionType    = @MasterTransactionType,";
                        sqlText += " FromStock          = @MasterFromStock,";
                        sqlText += " QuantityImport     = @ItemQuantityImport,";
                        sqlText += "DollarPrice         = '" + usdResults[1] + "'";//DollerValue
                        sqlText += " where DisposeNumber= @MasterDisposeNumber";

                        sqlText += " and ItemNo = '" + Item.ItemNo + "'";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLineNumber", Item.LineNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRealPrice", Item.RealPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmountD", Item.VATAmountD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSaleNumber", Item.SaleNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPresentPrice", Item.PresentPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDisposeDate", Master.DisposeDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarksD", Item.RemarksD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterFromStock", Master.FromStock);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantityImport", Item.QuantityImport);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableCreatID);
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
                retResults[1] = MessageVM.disposeMsgUpdateSuccessfully;
                retResults[2] = Master.DisposeNumber;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{

            //    transaction.Rollback();
            //    throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";

                transaction.Rollback();

                FileLogger.Log("DisposeDAL", "DisposeUpdate", ex.ToString() + "\n" + sqlText);

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] DisposePost(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null)
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
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.DisposeDate) < DateTime.MinValue || Convert.ToDateTime(Master.DisposeDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DisposeDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DisposeDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(DisposeNumber) from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DisposeHeaders set  ";

                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where DisposeNumber= @MasterDisposeNumber ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += " select COUNT(DisposeNumber) from DisposeDetails WHERE DisposeNumber=@MasterDisposeNumber ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update DisposeDetails set ";
                        sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                        sqlText += " Post= @MasterPost ";
                        sqlText += " where DisposeNumber= @MasterDisposeNumber";
                        sqlText += " and ItemNo = @ItemItemNo";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterDisposeNumber", Master.DisposeNumber);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUnableCreatID);
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
                retResults[1] = MessageVM.disposeMsgSuccessfullyPost;
                retResults[2] = Master.DisposeNumber;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{

            //    transaction.Rollback();
            //    throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();

                FileLogger.Log("DisposeDAL", "DisposePost", ex.ToString() + "\n" + sqlText);

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        
        #endregion

    }
}
