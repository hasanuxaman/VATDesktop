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
    public class SaleExportDAL : ISaleExport
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        //currConn to VcurrConn 25-Aug-2020
        public string[] SaleExportInsert(SaleExportVM Master, List<SaleExportInvoiceVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            #region Check user from settings
            
            #endregion
            
            SqlConnection vcurrConn = VcurrConn;
            if (vcurrConn == null)
            {
                VcurrConn = null;
                Vtransaction = null;
            }
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
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.SaleExportDate) < DateTime.MinValue || Convert.ToDateTime(Master.SaleExportDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

                }


                #endregion Validation for Header
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }

                    Vtransaction = VcurrConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                }


                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.SaleExportDate;
                string transactionYearCheck = Convert.ToDateTime(Master.SaleExportDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, VcurrConn);
                    cmdIdExist.Transaction = Vtransaction;
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

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, VcurrConn);
                    cmdYearNotExist.Transaction = Vtransaction;
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
                sqlText = sqlText + "select COUNT(SaleExportNo) from SaleExports WHERE SaleExportNo=@MasterSaleExportNo  ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, VcurrConn);
                cmdExistTran.Transaction = Vtransaction;
                cmdExistTran.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist
                

                CommonDAL commonDal = new CommonDAL();


                newID = commonDal.TransactionCode("SaleExport", "SaleExport", "SaleExports", "SaleExportNo",
                                              "SaleExportDate", Master.SaleExportDate,Master.BranchId.ToString(), VcurrConn, Vtransaction);


                #region ID generated completed,Insert new Information in Header

                sqlText = "";
                sqlText += " insert into SaleExports(";
                sqlText += " SaleExportNo,";
                sqlText += " SaleExportDate,";
                sqlText += " Description,";
                sqlText += " Comments,";
                sqlText += " Quantity,";
                sqlText += " GrossWeight,";
                sqlText += " NetWeight,";
                sqlText += " NumberFrom,";
                sqlText += " NumberTo,";
                sqlText += " PortFrom,";
                sqlText += " PortTo,";
                sqlText += " Post,";
                sqlText += " BranchId,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "'" + newID + "',";
                sqlText += " @MasterSaleExportDate,";
                sqlText += " @MasterDescription,";
                sqlText += " @MasterComments,";
                sqlText += " @MasterQuantity,";
                sqlText += " @MasterGrossWeight,";
                sqlText += " @MasterNetWeight,";
                sqlText += " @MasterNumberFrom,";
                sqlText += " @MasterNumberTo,";
                sqlText += " @MasterPortFrom,";
                sqlText += " @MasterPortTo,";
                sqlText += " '0',";
                sqlText += " @BranchId,";
                sqlText += " @MasterCreatedBy,";
                sqlText += " @MasterCreatedOn";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, VcurrConn);
                cmdInsert.Transaction = Vtransaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSaleExportDate", Master.SaleExportDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDescription", Master.Description);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterQuantity", Master.Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterGrossWeight", Master.GrossWeight);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterNetWeight", Master.NetWeight);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterNumberFrom", Master.NumberFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterNumberTo", Master.NumberTo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPortFrom", Master.PortFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPortTo", Master.PortTo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(SaleExportNo) from SaleExportInvoices WHERE SaleExportNo='" + newID + "' and SalesInvoiceNo= '" + Item.SalesInvoiceNo + "' ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, VcurrConn);
                    cmdFindId.Transaction = Vtransaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into SaleExportInvoices(";
                    sqlText += " SaleExportNo,";
                    sqlText += " SL,";
                    sqlText += " SalesInvoiceNo";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "'" + newID + "',";
                    sqlText += "@ItemSL,";
                    sqlText += "@ItemSalesInvoiceNo";
                   
                    sqlText += ")	";

                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, VcurrConn);
                    cmdInsDetail.Transaction = Vtransaction;

                    cmdInsDetail.Parameters.AddWithValue("@ItemSL", Item.SL);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  SaleExportNo from SaleExports WHERE SaleExportNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, VcurrConn);
                cmdIPS.Transaction = Vtransaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch
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

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {

                    Vtransaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            return retResults;
            #endregion Result

        }

        public string[] SaleExportUpdate(SaleExportVM Master, List<SaleExportInvoiceVM> Details, SysDBInfoVMTemp connVM = null)
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
            //DateTime MinDate = DateTime.MinValue;
            //DateTime MaxDate = DateTime.MaxValue;
            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.SaleExportDate) < DateTime.MinValue || Convert.ToDateTime(Master.SaleExportDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.SaleExportDate;
                string transactionYearCheck = Convert.ToDateTime(Master.SaleExportDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(SaleExportNo) from SaleExports WHERE SaleExportNo=@MasterSaleExportNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();


                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update SaleExports set  ";
                sqlText += " SaleExportDate     = @MasterSaleExportDate ,";
                sqlText += " Description        = @MasterDescription ,";
                sqlText += " Comments           = @MasterComments ,";
                sqlText += " Quantity           = @MasterQuantity ,";
                sqlText += " GrossWeight        = @MasterGrossWeight ,";
                sqlText += " NetWeight          = @MasterNetWeight ,";
                sqlText += " NumberFrom         = @MasterNumberFrom ,";
                sqlText += " NumberTo           = @MasterNumberTo ,";
                sqlText += " PortFrom           = @MasterPortFrom ,";
                sqlText += " PortTo             = @MasterPortTo ,";
                sqlText += " BranchId           = @BranchId ,";
                sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     = @MasterLastModifiedOn ";
                sqlText += " where SaleExportNo = @MasterSaleExportNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterSaleExportDate", Master.SaleExportDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDescription", Master.Description);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterQuantity", Master.Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterGrossWeight", Master.GrossWeight);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterNetWeight", Master.NetWeight);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterNumberFrom", Master.NumberFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterNumberTo", Master.NumberTo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPortFrom", Master.PortFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPortTo", Master.PortTo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterSaleExportNo  ", Master.SaleExportNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                sqlText = "delete SaleExportInvoices where SaleExportNo=@MasterSaleExportNo";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Transaction = transaction;

                cmdDelete.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);

                cmdDelete.ExecuteNonQuery();
                

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update
                        // Insert
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into SaleExportInvoices(";
                        sqlText += " SaleExportNo,";
                        sqlText += " SL,";
                        sqlText += " SalesInvoiceNo";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterSaleExportNo,";
                        sqlText += "@ItemSL,";
                        sqlText += "@ItemSalesInvoiceNo ";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSL", Item.SL);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                       
                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())
 

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SaleExports WHERE SaleExportNo=@MasterSaleExportNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
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
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.SaleExportNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public string[] SaleExportPost(SaleExportVM Master, SysDBInfoVMTemp connVM = null)
        {
            //#region Initializ
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


            //#endregion Initializ

            //#region Try
            try
            {
                string vNegStockAllow = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                //#region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.SaleExportDate) < DateTime.MinValue || Convert.ToDateTime(Master.SaleExportDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);

                }



                //#endregion Validation for Header
                //#region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNamePost);

                //#endregion open connection and transaction
                //#region Fiscal Year Check

                string transactionDate = Master.SaleExportDate;
                string transactionYearCheck = Convert.ToDateTime(Master.SaleExportDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    //#region YearLock
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
                    //#endregion YearLock
                    //#region YearNotExist
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
                    //#endregion YearNotExist

                }


                //#endregion Fiscal Year CHECK

                //#region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(SaleExportNo) from SaleExports WHERE SaleExportNo=@MasterSaleExportNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                //#endregion Find ID for Update

                //#region ID check completed,update Information in Header

                //#region update Header
                sqlText = "";

                sqlText += " update SaleExports set  ";
                sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                sqlText += " Post= 'Y' ";
                sqlText += " where  SaleExportNo= @MasterSaleExportNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                }

                //#region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SaleExports WHERE SaleExportNo=@MasterSaleExportNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterSaleExportNo", Master.SaleExportNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                }


                //#endregion Prefetch
                //#region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                //#endregion Commit
                //#region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.SaleExportNo;
                retResults[3] = PostStatus;
                //#endregion SuccessResult

            }
          ////#endregion Try

            //#region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            //#endregion Catch and Finall

            //#region Result
            return retResults;
            //#endregion Result

        }

        public DataTable SearchSaleExportDTNew(string SaleExportNo, string SaleExportDateFrom, string SaleExportDateTo, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("IssueSearchHeader");

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
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();

                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion Add BOMId
                #endregion open connection and transaction

                #region SQL Statement

                sqlText = " ";
                sqlText = @"  
    select   sx.SaleExportNo
    ,isnull(sx.SaleExportDate,'1900/01/01')SaleExportDate
    ,isnull(sx.Description,'')Description
    ,isnull(sx.Comments		,'')Comments
    ,isnull(sx.Quantity		,'')Quantity
    ,isnull(sx.GrossWeight	,'')GrossWeight
    ,isnull(sx.NetWeight	,'')NetWeight
    ,isnull(sx.NumberFrom	,'')NumberFrom
    ,isnull(sx.NumberTo		,'')NumberTo
    ,isnull(sx.PortFrom		,'')PortFrom
    ,isnull(sx.PortTo		,'')PortTo
    ,isnull(sx.Post			,'N')Post
    ,isnull(sx.BranchId			,0)BranchId
    ,sx.CreatedBy
    ,sx.CreatedOn
    ,sx.LastModifiedBy
    ,sx.LastModifiedOn
    from SaleExports sx

    WHERE
    (sx.SaleExportNo  LIKE '%' +  @SaleExportNo   + '%' OR @SaleExportNo IS NULL) 
    AND (sx.SaleExportDate>= @SaleExportDateFrom OR @SaleExportDateFrom IS NULL)
    AND (sx.SaleExportDate <dateadd(d,1, @SaleExportDateTo) OR @SaleExportDateTo IS NULL)
    and (sx.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
    and sx.BranchId=@BranchId
                            ";
                

                #endregion

                #region SQL Command
               
                SqlCommand objCommIssueHeader = new SqlCommand();
                objCommIssueHeader.Connection = currConn;

                objCommIssueHeader.CommandText = sqlText;
                objCommIssueHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueHeader.Parameters.Contains("@Post"))
                { objCommIssueHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommIssueHeader.Parameters["@Post"].Value = Post; }

                if (!objCommIssueHeader.Parameters.Contains("@SaleExportNo"))
                { objCommIssueHeader.Parameters.AddWithValue("@SaleExportNo", SaleExportNo); }
                else { objCommIssueHeader.Parameters["@SaleExportNo"].Value = SaleExportNo; }

                if (SaleExportDateFrom == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@SaleExportDateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@SaleExportDateFrom", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@SaleExportDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@SaleExportDateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@SaleExportDateFrom", SaleExportDateFrom); }
                    else { objCommIssueHeader.Parameters["@SaleExportDateFrom"].Value = SaleExportDateFrom; }
                }
                if (SaleExportDateTo == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@SaleExportDateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@SaleExportDateTo", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@SaleExportDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@SaleExportDateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@SaleExportDateTo", SaleExportDateTo); }
                    else { objCommIssueHeader.Parameters["@SaleExportDateTo"].Value = SaleExportDateTo; }
                }
                if (!objCommIssueHeader.Parameters.Contains("@BranchId"))
                { objCommIssueHeader.Parameters.AddWithValue("@BranchId", BranchId); }
                else { objCommIssueHeader.Parameters["@BranchId"].Value = BranchId; }

                


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public DataTable SearchSaleExportDetailDTNew(string SaleExportNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("IssueSearchDetail");

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
sxi.SaleExportNo
,sxi.SL
,sxi.SalesInvoiceNo
,sih.InvoiceDateTime,c.CustomerName
 from SaleExportInvoices sxi
 left outer join SalesInvoiceHeaders sih on sih.SalesInvoiceNo=sxi.SalesInvoiceNo
 left outer join Customers c on sih.CustomerID=c.CustomerID

where 1=1 and sxi.SaleExportNo=@SaleExportNo
 order by sxi.SL
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommIssueDetail = new SqlCommand();
                objCommIssueDetail.Connection = currConn;

                objCommIssueDetail.CommandText = sqlText;
                objCommIssueDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueDetail.Parameters.Contains("@SaleExportNo"))
                { objCommIssueDetail.Parameters.AddWithValue("@SaleExportNo", SaleExportNo); }
                else { objCommIssueDetail.Parameters["@SaleExportNo"].Value = SaleExportNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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


    }
}
