using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;

namespace VATServer.Library
{
    public class TransferDAL
    {
        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        #region Plain Methods
        public string[] TransferInsertToMaster(TransferVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            //string PostStatus = "";
            string newID = "";
            #endregion Initializ

            #region Try
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

                #region Insert

                sqlText = "";
                sqlText += "  insert into Transfers";
                sqlText += " (";
                sqlText += " TransferNo";
                sqlText += " ,TransferFromNo";
                sqlText += " ,TransactionDateTime";
                sqlText += " ,TransactionType";
                sqlText += " ,SerialNo";
                sqlText += " ,Comments";
                sqlText += " ,Post";
                sqlText += " ,ReferenceNo";
                sqlText += " ,TransferFrom";
                sqlText += " ,BranchId";

                sqlText += " ,TotalVATAmount";
                sqlText += " ,TotalSDAmount";

                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,LastModifiedBy";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@Master_TransferNo";
                sqlText += ",@Master_TransferFromNo";
                sqlText += ",@Master_TransactionDateTime";
                sqlText += ",@Master_TransactionType";
                sqlText += ",@Master_SerialNo";
                sqlText += ",@Master_Comments";
                sqlText += ",@Master_Post";
                sqlText += ",@Master_ReferenceNo";
                sqlText += ",@Master_TransferFrom";
                sqlText += ",@BranchId";


                sqlText += ",@TotalVATAmount";
                sqlText += ",@TotalSDAmount";

                sqlText += ",@Master_CreatedBy";
                sqlText += ",@Master_CreatedOn";
                sqlText += ",@Master_LastModifiedOn";
                sqlText += ",@Master_LastModifiedBy";

                sqlText += ") ";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferNo", Master.TransferNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferFromNo", Master.TransferFromNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransactionDateTime", Master.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_ReferenceNo", Master.ReferenceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferFrom", Master.TransferFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);




                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_CreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_LastModifiedOn", Master.LastModifiedOn);


                transResult = cmdInsert.ExecuteNonQuery();
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Commit


                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Requested insert into Transfer successfully Added";
                retResults[2] = Master.TransferNo;
                //retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] TransferInsertToDetail(TransferDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            ProductDAL productDal = new ProductDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string newID = "";
            string PostStatus = "";

            #endregion Initializ

            #region Try
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
                #region Purchase ID Create
                //if (string.IsNullOrEmpty(Detail.TransactionType)) //start
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                //}
                //#endregion
                //#region Purchase ID Create For Other
                //CommonDAL commonDal = new CommonDAL();
                //if (Detail.TransactionType == "61Out")
                //{
                //    newID = commonDal.TransactionCode("Transfer", "61Out", "TransferIssues", "TransferIssueNo",
                //                              "TransactionDateTime", Detail.TransactionDateTime, currConn, transaction);
                //}
                //if (Detail.TransactionType == "62Out")
                //{
                //    newID = commonDal.TransactionCode("Transfer", "62Out", "TransferIssues", "TransferIssueNo",
                //                              "TransactionDateTime", Detail.TransactionDateTime, currConn, transaction);
                //}
                #endregion
                #region Insert into Details(Insert complete in Header)
                #region Insert Detail Table




                #region Insert only DetailTable PurchaseInvoiceDetails

                sqlText = "";
                sqlText += "  insert into TransferDetails(";
                sqlText += " TransferNo";
                sqlText += ",TransferLineNo";
                sqlText += ",ItemNo";
                sqlText += ",Quantity";
                sqlText += ",CostPrice";
                sqlText += ",UOM";
                sqlText += ",SubTotal";
                sqlText += ",Comments";
                sqlText += ",TransactionType";
                sqlText += ",TransactionDateTime";
                sqlText += ",TransferFrom";
                sqlText += ",BranchId";

                sqlText += ",Post";
                sqlText += ",UOMQty";
                sqlText += ",UOMPrice";
                sqlText += ",UOMc";
                sqlText += ",UOMn";


                sqlText += ",VATRate";
                sqlText += ",VATAmount";
                sqlText += ",SDRate";
                sqlText += ",SDAmount";


                sqlText += ",CreatedBy";
                sqlText += ",CreatedOn";
                sqlText += ",LastModifiedBy";
                sqlText += ",LastModifiedOn";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@Detail_TransferNo";
                sqlText += ",@Detail_TransferLineNo";
                sqlText += ",@Detail_ItemNo";
                sqlText += ",@Detail_Quantity";
                sqlText += ",@Detail_CostPrice";
                sqlText += ",@Detail_UOM";
                sqlText += ",@Detail_SubTotal";
                sqlText += ",@Detail_Comments";
                sqlText += ",@Detail_TransactionType";
                sqlText += ",@Detail_TransactionDateTime";
                sqlText += ",@Detail_TransferFrom";
                sqlText += ",@BranchId";

                sqlText += ",@Detail_Post";
                sqlText += ",@Detail_UOMQty";
                sqlText += ",@Detail_UOMPrice";
                sqlText += ",@Detail_UOMc";
                sqlText += ",@Detail_UOMn";


                sqlText += ",@VATRate";
                sqlText += ",@VATAmount";
                sqlText += ",@SDRate";
                sqlText += ",@SDAmount";



                sqlText += ",@Detail_CreatedBy";
                sqlText += ",@Detail_CreatedOn";
                sqlText += ",@Detail_LastModifiedBy";
                sqlText += ",@Detail_LastModifiedOn";


                sqlText += ")	";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferNo", Detail.TransferNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferLineNo", Detail.TransferLineNo);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_ItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Quantity", Detail.Quantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CostPrice", Detail.CostPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOM", Detail.UOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_SubTotal", Detail.SubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Comments", Detail.Comments);


                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDRate", Detail.SDRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Post", Detail.Post);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMQty", Detail.UOMQty);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMPrice", Detail.UOMPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMc", Detail.UOMc);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMn", Detail.UOMn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransactionType", Detail.TransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransactionDateTime", Detail.TransactionDateTime);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferFrom", Detail.TransferFrom);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);


                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CreatedOn", Detail.CreatedOn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_LastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_LastModifiedOn", Detail.LastModifiedOn);

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                #endregion Insert only DetailTable

                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region Commit


                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Requested insert into TransferDetails successfully Added";
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        #endregion

        public string[] Insert(TransferVM Master, List<TransferDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)////, string dbName)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            #region Check user from settings
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Please Check Issue Data and Time");
                }
                #endregion Validation for Header
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
                #region Fiscal Year Check
                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferNo) from Transfers WHERE TransferNo='" + Master.TransferNo + "' ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                IDExist = (int)cmdExistTran.ExecuteScalar();
                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                }
                #endregion Find Transaction Exist
                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.TransactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other
                CommonDAL commonDal = new CommonDAL();

                newID = commonDal.TransactionCode("Transfer", "BTB", "Transfers", "TransferNo",
                                              "TransactionDateTime", Master.TransactionDateTime, Master.BranchId.ToString(), currConn, transaction);

                #endregion Purchase ID Create For Other
                #endregion Purchase ID Create Not Complete
                #region ID generated completed,Insert new Information in Header

                Master.TransferNo = newID;
                retResults = TransferInsertToMaster(Master, currConn, transaction);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                }
                #endregion ID generated completed,Insert new Information in Header
                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail
                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }
                #endregion Validation for Detail
                #region Insert Detail Table
                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist
                    sqlText = "";
                    sqlText += "select COUNT(TransferNo) from TransferDetails WHERE TransferNo='" + newID + "' ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                    }
                    #endregion Find Transaction Exist
                    #region Insert only DetailTable
                    #region comment
                    //sqlText = "";
                    //sqlText += " insert into TransferDetails(";
                    //sqlText += "  TransferNo";
                    //sqlText += " ,TransferLineNo";
                    //sqlText += " ,ItemNo";
                    //sqlText += " ,Quantity";
                    //sqlText += " ,CostPrice";
                    //sqlText += " ,UOM";
                    //sqlText += " ,TransferFrom";
                    //sqlText += " ,SubTotal";
                    //sqlText += " ,Comments";
                    //sqlText += " ,TransactionType";
                    //sqlText += " ,TransactionDateTime";
                    //sqlText += " ,Post";
                    //sqlText += " ,UOMQty";
                    //sqlText += " ,UOMPrice";
                    //sqlText += " ,UOMc";
                    //sqlText += " ,UOMn";
                    //sqlText += " ,CreatedBy";
                    //sqlText += " ,CreatedOn";
                    //sqlText += " ,LastModifiedBy";
                    //sqlText += " ,LastModifiedOn      ";
                    //sqlText += " )";
                    //sqlText += " values(	";
                    //sqlText += "'" + newID + "',";
                    //sqlText += "'" + Item.TransferLineNo + "',";
                    //sqlText += "'" + Item.ItemNo + "',";
                    //sqlText += "'" + Item.Quantity + "',";
                    //sqlText += "'" + Item.CostPrice + "',";
                    //sqlText += "'" + Item.UOM + "',";
                    //sqlText += "'" + Master.TransferFrom + "',";
                    //sqlText += "'" + Item.SubTotal + "',";
                    //sqlText += "'" + Item.Comments + "',";
                    //sqlText += "'" + Master.TransactionType + "',";
                    //sqlText += "'" + Master.TransactionDateTime + "',";
                    //sqlText += "'" + Master.Post + "',";
                    //sqlText += "'" + Item.UOMQty + "',";
                    //sqlText += "'" + Item.UOMPrice + "',";
                    //sqlText += "'" + Item.UOMc + "',";
                    //sqlText += "'" + Item.UOMn + "',";
                    //sqlText += "'" + Master.CreatedBy + "',";
                    //sqlText += "'" + Master.CreatedOn + "',";
                    //sqlText += "'" + Master.LastModifiedBy + "',";
                    //sqlText += "'" + Master.LastModifiedOn + "'";
                    //sqlText += ")	";
                    //SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    //cmdInsDetail.Transaction = transaction;
                    //transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                    //}
                    #endregion
                    Item.TransferNo = newID;
                    Item.TransferFrom = Master.TransferFrom;
                    Item.TransactionType = Master.TransactionType;
                    Item.TransactionDateTime = Master.TransactionDateTime;
                    Item.Post = Master.Post;
                    Item.CreatedBy = Master.CreatedBy;
                    Item.CreatedOn = Master.CreatedOn;
                    Item.LastModifiedBy = Master.LastModifiedBy;
                    Item.LastModifiedOn = Master.LastModifiedOn;

                    retResults = TransferInsertToDetail(Item, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }
                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)
                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.Transfers WHERE TransferNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgUnableCreatID);
                }
                #endregion Prefetch
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
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult
            }
            #endregion Try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                return retResults;
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
            #region Result
            return retResults;
            #endregion Result
        }
        public string[] Update(TransferVM Master, List<TransferDetailVM> Details, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, "Please Check Invoice Data and Time");
                }
                #endregion Validation for Header
                #region open connection and transaction
               currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameUpdate);
                #endregion open connection and transaction
                #region Fiscal Year Check
                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferNo) from Transfers WHERE TransferNo='" + Master.TransferNo + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableFindExistID);
                }
                #endregion Find ID for Update
                #region ID check completed,update Information in Header
                #region update Header
                sqlText = "";
                sqlText += " update Transfers set  ";
                sqlText += " TransactionDateTime= '" + Master.TransactionDateTime + "' ,";
                sqlText += " TotalAmount= '" + Master.TotalAmount + "' ,";
                sqlText += " TransactionType= '" + Master.TransactionType + "' ,";
                sqlText += " SerialNo= '" + Master.SerialNo + "' ,";
                sqlText += " Comments= '" + Master.Comments + "' ,";
                sqlText += " Post= '" + Master.Post + "' ,";
                sqlText += " ReferenceNo= '" + Master.ReferenceNo + "' ,";
                sqlText += " TransferFrom= '" + Master.TransferFrom + "' ,";
                sqlText += " CreatedBy= '" + Master.CreatedBy + "' ,";
                sqlText += " CreatedOn= '" + Master.CreatedOn + "' ,";
                sqlText += " LastModifiedBy= '" + Master.LastModifiedBy + "' ,";
                sqlText += " LastModifiedOn= '" + Master.LastModifiedOn + "' ";
                sqlText += " where  TransferNo= '" + Master.TransferNo + "' ";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                }
                #endregion update Header
                #endregion ID check completed,update Information in Header
                #region Update into Details(Update complete in Header)
                #region Validation for Detail
                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }
                #endregion Validation for Detail
                #region Update Detail Table
                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update
                    sqlText = "";
                    sqlText += "select COUNT(TransferNo) from TransferDetails WHERE TransferNo='" + Master.TransferNo + "' ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable
                        sqlText = "";
                        sqlText += " insert into TransferDetails(";
                        sqlText += "  TransferNo";
                        sqlText += " ,TransferLineNo";
                        sqlText += " ,ItemNo";
                        sqlText += " ,Quantity";
                        sqlText += " ,CostPrice";
                        sqlText += " ,UOM";
                        sqlText += " ,TransferFrom";
                        sqlText += " ,SubTotal";
                        sqlText += " ,Comments";
                        sqlText += " ,TransactionType";
                        sqlText += " ,TransactionDateTime";
                        sqlText += " ,Post";
                        sqlText += " ,CreatedBy";
                        sqlText += " ,CreatedOn";
                        sqlText += " ,LastModifiedBy";
                        sqlText += " ,LastModifiedOn";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "'" + Master.TransferNo + "',";
                        sqlText += "'" + Item.TransferLineNo + "',";
                        sqlText += "'" + Item.ItemNo + "',";
                        sqlText += "'" + Item.Quantity + "',";
                        sqlText += "'" + Item.CostPrice + "',";
                        sqlText += "'" + Item.UOM + "',";
                        sqlText += "'" + Master.TransferFrom + "',";
                        sqlText += "'" + Item.SubTotal + "',";
                        sqlText += "'" + Item.Comments + "',";
                        sqlText += "'" + Master.TransactionType + "',";
                        sqlText += "'" + Master.TransactionDateTime + "',";
                        sqlText += "'" + Master.Post + "',";
                        sqlText += "'" + Master.CreatedBy + "',";
                        sqlText += "'" + Master.CreatedOn + "',";
                        sqlText += "'" + Master.LastModifiedBy + "',";
                        sqlText += "'" + Master.LastModifiedOn + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                    }
                    else
                    {
                        //Update
                        #region Update only DetailTable
                        sqlText = "";
                        sqlText += " update TransferDetails set ";
                        sqlText += " TransferLineNo='" + Item.TransferLineNo + "',";
                        sqlText += " Quantity='" + Item.Quantity + "',";
                        sqlText += " CostPrice='" + Item.CostPrice + "',";
                        sqlText += " UOM='" + Item.UOM + "',";
                        sqlText += " TransferFrom='" + Item.TransferFrom + "',";
                        sqlText += " SubTotal='" + Item.SubTotal + "',";
                        sqlText += " Comments='" + Item.Comments + "',";
                        sqlText += " TransactionType='" + Master.TransactionType + "',";
                        sqlText += " TransactionDateTime='" + Master.TransactionDateTime + "',";
                        sqlText += " Post='" + Master.Post + "',";
                        sqlText += " CreatedBy='" + Master.CreatedBy + "',";
                        sqlText += " CreatedOn='" + Master.CreatedOn + "',";
                        sqlText += " LastModifiedBy='" + Master.LastModifiedBy + "',";
                        sqlText += " LastModifiedOn='" + Master.LastModifiedOn + "'";
                        sqlText += " where  TransferNo ='" + Master.TransferNo + "' ";
                        sqlText += " and ItemNo='" + Item.ItemNo + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }
                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())
                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from TransferDetails WHERE TransferNo='" + Master.TransferNo + "'";
                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();
                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM TransferDetails  ";
                        sqlText += " WHERE TransferNo='" + Master.TransferNo + "' ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }
                }
                #endregion Remove row
                #endregion Update Detail Table
                #endregion  Update into Details(Update complete in Header)
                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct Post from Transfers WHERE TransferNo='" + Master.TransferNo + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableCreatID);
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
                retResults[1] = MessageVM.receiveMsgUpdateSuccessfully;
                retResults[2] = Master.TransferNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult
            }
            #endregion Try
            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
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
        public string[] Post(TransferVM Master, List<TransferDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "")
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
                string vNegStockAllow = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                #region Validation for Header
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgCheckDatePost);
                }
                #endregion Validation for Header
                #region open connection and transaction
               currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNamePost);
                #endregion open connection and transaction
                #region Fiscal Year Check
                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferNo) from Transfers WHERE TransferNo='" + Master.TransferNo + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgUnableFindExistIDPost);
                }
                #endregion Find ID for Update
                #region ID check completed,update Information in Header
                #region update Header
                sqlText = "";
                sqlText += " update Transfers set  ";
                sqlText += " LastModifiedBy='" + Master.LastModifiedBy + "' ,";
                sqlText += " LastModifiedOn='" + Master.LastModifiedOn + "' ,";
                sqlText += " Post= '" + Master.Post + "' ";
                sqlText += " where  TransferNo= '" + Master.TransferNo + "' ";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSuccessfully);
                }
                #endregion update Header
                #endregion ID check completed,update Information in Header
                #region Update into Details(Update complete in Header)
                #region Validation for Detail
                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                }
                #endregion Validation for Detail
                #region Update Detail Table
                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update
                    sqlText = "";
                    sqlText += "select COUNT(TransferNo) from TransferDetails WHERE TransferNo='" + Master.TransferNo + "' ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable
                        sqlText = "";
                        sqlText += " update TransferDetails set ";
                        sqlText += " Post='" + Master.Post + "'";
                        sqlText += " where  TransferNo ='" + Master.TransferNo + "' ";
                        sqlText += " and ItemNo='" + Item.ItemNo + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                        #region Update Item Qty
                        else
                        {
                            #region Find Quantity From Products
                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStock = productDal.StockInHand(Item.ItemNo, Master.IssueDateTime, currConn, transaction);
                            decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo, Master.TransactionDateTime,
                                                              currConn, transaction, true,true,true,true,connVM,UserId).Rows[0]["Quantity"].ToString());
                            #endregion Find Quantity From Products
                            #region Find Quantity From Transaction
                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from TransferDetails ";
                            sqlText += " WHERE ItemNo='" + Item.ItemNo + "' and TransferNo= '" + Master.TransferNo + "'";
                            SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                            cmdTranQty.Transaction = transaction;
                            decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();
                            #endregion Find Quantity From Transaction
                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQty > (oldStock + TranQty))
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgStockNotAvailablePost);
                                }
                            }
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
                sqlText = sqlText + "select distinct Post from Transfers WHERE TransferNo='" + Master.TransferNo + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSelect);
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
                retResults[1] = MessageVM.receiveMsgSuccessfullyPost;
                retResults[2] = Master.TransferNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult
            }
            #endregion Try
            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
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
        //public DataTable SearchTransfer(string TransferNo, string DateTimeFrom,
        //    string DateTimeTo, string TransactionType, string Post)
        public DataTable SearchTransfer(TransferVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransfer");
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
                #endregion Add BOMId
                #endregion open connection and transaction
                #region SQL Statement
                sqlText = " ";

                #region Old

//                sqlText = @" 
//SELECT 
//t.TransferNo
//,t.TransferFromNo
//, convert (varchar,t.TransactionDateTime,120)TransferDateTime
//,isnull(t.TotalAmount     ,'0')TotalAmount
//--,isnull(t.TransactionType ,'N/A')TransactionType
//,case when TransactionType='61Out' then '6.1 Out' else '6.2 Out' end TransactionType
//
//,isnull(t.SerialNo        ,'N/A')SerialNo
//,isnull(t.Comments        ,'N/A')Comments
//,isnull(t.Post            ,'N')Post
//,isnull(t.ReferenceNo     ,'N/A')ReferenceNo
//,isnull(t.TransferFrom      ,0)TransferFrom
//,isnull(br.BranchName, 'N/A') BranchName
//,isnull(t.BranchId      ,0)BranchId
//
//
//,isnull(t.TotalVATAmount       ,0)TotalVATAmount
//,isnull(t.TotalSDAmount       ,0)TotalSDAmount
//
//,isnull(t.CreatedBy       ,'N/A')CreatedBy
//,isnull(t.CreatedOn       ,'19900101')CreatedOn
//,isnull(t.LastModifiedBy  ,'N/A')LastModifiedBy
//,isnull(t.LastModifiedOn  ,'19900101')LastModifiedOn
//                            FROM         dbo.Transfers t
//LEFT OUTER JOIN BranchProfiles br on t.TransferFrom = br.BranchId
//                            WHERE 1=1
//                            AND (t.TransferNo  LIKE '%' +  @TransferNo   + '%' OR @TransferNo IS NULL) 
//                            AND (t.TransferFromNo  LIKE '%' +  @TransferFromNo   + '%' OR @TransferFromNo IS NULL) 
//                            AND (t.TransactionDateTime>= @DateTimeFrom OR @DateTimeFrom IS NULL)
//                            AND (t.TransactionDateTime <dateadd(d,1, @DateTimeTo) OR @DateTimeTo IS NULL)
//                            and (t.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
//                            AND (t.transactionType  LIKE '%' +  @transactionType   + '%' OR @transactionType IS NULL) 
//                          --  AND (t.transactionType=@transactionType) 
//                            ";

                #endregion

                sqlText = @"SELECT 
t.TransferNo
,t.TransferFromNo
, convert (varchar,t.TransactionDateTime,120)TransferDateTime
,isnull(t.TotalAmount     ,'0')TotalAmount
--,isnull(t.TransactionType ,'N/A')TransactionType
,case when TransactionType='61Out' then '6.1 Out' else '6.2 Out' end TransactionType

,isnull(t.SerialNo        ,'N/A')SerialNo
,isnull(t.Comments        ,'N/A')Comments
,isnull(t.Post            ,'N')Post
,isnull(t.ReferenceNo     ,'N/A')ReferenceNo
,isnull(t.TransferFrom      ,0)TransferFrom
,isnull(br.BranchName, 'N/A') BranchName
,isnull(t.BranchId      ,0)BranchId


,isnull(t.TotalVATAmount       ,0)TotalVATAmount
,isnull(t.TotalSDAmount       ,0)TotalSDAmount

,isnull(t.CreatedBy       ,'N/A')CreatedBy
,isnull(t.CreatedOn       ,'19900101')CreatedOn
,isnull(t.LastModifiedBy  ,'N/A')LastModifiedBy
,isnull(t.LastModifiedOn  ,'19900101')LastModifiedOn
                            FROM         dbo.Transfers t
LEFT OUTER JOIN BranchProfiles br on t.TransferFrom = br.BranchId
                            WHERE 1=1
                            AND (t.TransferNo  LIKE '%' +  @TransferNo   + '%' OR @TransferNo IS NULL) 
                            AND (t.TransferFromNo  LIKE '%' +  @TransferFromNo   + '%' OR @TransferFromNo IS NULL) 
                            AND (t.TransactionDateTime>= @DateTimeFrom OR @DateTimeFrom IS NULL)
                            AND (t.TransactionDateTime <dateadd(d,1, @DateTimeTo) OR @DateTimeTo IS NULL)
                            and (t.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            AND (t.transactionType  LIKE '%' +  @transactionType   + '%' OR @transactionType IS NULL) 
                          --  AND (t.transactionType=@transactionType) ";

                if (vm.BranchId > 0)
                {
                   // sqlText = sqlText+@" AND t.BranchId = @BranchId";

                    sqlText = sqlText + @" AND t.TransferTo = @BranchId";
                }

                #endregion
                #region SQL Command
                SqlCommand objCommHeader = new SqlCommand();
                objCommHeader.Connection = currConn;
                objCommHeader.CommandText = sqlText;
                objCommHeader.CommandType = CommandType.Text;
                #endregion
                #region Parameter
                if (!objCommHeader.Parameters.Contains("@Post"))
                { objCommHeader.Parameters.AddWithValue("@Post", vm.Post); }
                else { objCommHeader.Parameters["@Post"].Value = vm.Post; }




                if (!objCommHeader.Parameters.Contains("@TransferNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferNo", vm.TransferNo); }
                else { objCommHeader.Parameters["@TransferNo"].Value = vm.TransferNo; }

                if (!objCommHeader.Parameters.Contains("@TransferFromNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferFromNo", vm.TransferFromNo); }
                else { objCommHeader.Parameters["@TransferFromNo"].Value = vm.TransferFromNo; }

                if (vm.DateTimeFrom == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", System.DBNull.Value); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", vm.DateTimeFrom); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = vm.DateTimeFrom; }
                }
                if (vm.DateTimeTo == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", System.DBNull.Value); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", vm.DateTimeTo); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = vm.DateTimeTo; }
                }
                if (!objCommHeader.Parameters.Contains("@transactionType"))
                { objCommHeader.Parameters.AddWithValue("@transactionType", vm.TransactionType); }
                else { objCommHeader.Parameters["@transactionType"].Value = vm.TransactionType; }


                if (vm.BranchId > 0)
                {
                     objCommHeader.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }

                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommHeader);
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
        public DataTable SearchTransferDetail(string TransferNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferDetail");
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
tid.TransferNo
,tid.TransferLineNo
,tid.ItemNo
,tid.Quantity
,tid.CostPrice
,tid.UOM
,tid.SubTotal
,tid.Comments
,tid.TransactionType
,tid.TransactionDateTime
,isnull(tid.TransferFrom      ,0)TransferFrom
,isnull(br.BranchName, 'N/A') BranchName
,tid.Post
,isnull(tid.UOMQty,isnull(tid.Quantity,0))UOMQty
,isnull(tid.UOMn,tid.UOM)UOMn
,isnull(tid.UOMc,1)UOMc
,isnull(tid.UOMPrice,isnull(tid.CostPrice,0))UOMPrice



,isnull(tid.VATRate,0)VATRate
,isnull(tid.VATAmount,0)VATAmount
,isnull(tid.SDRate ,0)SDRate 
,isnull(tid.SDAmount,0)SDAmount


,tid.CreatedBy
,tid.CreatedOn
,tid.LastModifiedBy
,tid.LastModifiedOn,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock
from TransferDetails tid
left outer join Products on tid.ItemNo=Products.ItemNo
LEFT OUTER JOIN BranchProfiles br on tid.TransferFrom = br.BranchId
                            WHERE 1=1
                            and (TransferNo = @TransferNo ) 
                            ";
                #endregion
                #region SQL Command
                SqlCommand objCommTransferDetail = new SqlCommand();
                objCommTransferDetail.Connection = currConn;
                objCommTransferDetail.CommandText = sqlText;
                objCommTransferDetail.CommandType = CommandType.Text;
                #endregion
                #region Parameter
                if (!objCommTransferDetail.Parameters.Contains("@TransferNo"))
                { objCommTransferDetail.Parameters.AddWithValue("@TransferNo", TransferNo); }
                else { objCommTransferDetail.Parameters["@TransferNo"].Value = TransferNo; }
                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransferDetail);
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

        #region Web methods
        public List<TransferVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            ////SqlConnection currConn = null;
            ////SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferVM> VMs = new List<TransferVM>();
            TransferVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, true,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new TransferVM();

                    vm.TransferNo = dr["TransferNo"].ToString();
                    vm.TransferFromNo = dr["TransferFromNo"].ToString();
                    vm.TransactionDateTime = dr["TransactionDateTime"].ToString();
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.TransferFrom = Convert.ToInt32(dr["TransferFrom"]);
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["TransferTo"]);
                    VMs.Add(vm);
                }


                #endregion SqlExecution

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            //finally
            //{
            //}
            #endregion
            return VMs;
        }


        public DataTable SelectAll(string TransferNo = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
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

                sqlText = @"
SELECT 
 t.TransferNo
,t.TransferFromNo
,t.TransactionDateTime
,isnull(t.TotalAmount,0) TotalAmount
,t.TransactionType
,t.SerialNo
,t.Comments
,t.Post
,t.ReferenceNo
,t.TransferFrom
,t.CreatedBy
,t.CreatedOn
,t.LastModifiedBy
,t.LastModifiedOn
,isnull(t.TotalVATAmount,0) TotalVATAmount
,isnull(t.TotalSDAmount,0) TotalSDAmount
,isnull(br.BranchName, 'N/A') BranchName
,isnull(t.BranchId, 0) BranchId
from Transfers t
LEFT OUTER JOIN BranchProfiles br on t.TransferFrom = br.BranchId
WHERE 1=1
";


                if (TransferNo != null)
                {
                    sqlText += @" and TransferNo=@TransferNo";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }

                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                if (TransferNo != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@TransferNo", TransferNo);
                }
                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public List<TransferDetailVM> SelectDetail(string TransferNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferDetailVM> VMs = new List<TransferDetailVM>();
            TransferDetailVM vm;
            #endregion
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

                sqlText = @"
 SELECT

 iss.TransferNo
,iss.TransferLineNo
,iss.ItemNo
,isnull(iss.Quantity,0) Quantity
,isnull(iss.CostPrice,0) CostPrice
,iss.UOM
,isnull(iss.SubTotal,0) SubTotal
,iss.Comments
,iss.TransactionType
,iss.TransactionDateTime
,iss.Post
,iss.TransferFrom
,iss.UOMQty
,isnull(iss.UOMPrice,0) UOMPrice
,isnull(iss.UOMc,0) UOMc
,iss.UOMn
,iss.CreatedBy
,iss.CreatedOn
,iss.LastModifiedBy
,iss.LastModifiedOn
,iss.TransferFromNo
,isnull(iss.VATRate,0) VATRate
,isnull(iss.VATAmount,0) VATAmount
,isnull(iss.SDRate,0) SDRate
,isnull(iss.SDAmount,0) SDAmount

,p.ProductCode
,p.ProductName
FROM TransferDetails iss left outer join Products p on iss.ItemNo=p.ItemNo
WHERE  1=1

";
                if (TransferNo != null)
                {
                    sqlText += "AND iss.TransferNo=@TransferNo";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (TransferNo != null)
                {
                    objComm.Parameters.AddWithValue("@TransferNo", TransferNo);
                }

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new TransferDetailVM();

                    vm.TransferNo = dr["TransferNo"].ToString();
                    vm.TransferLineNo = dr["TransferLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.TransactionDateTime = dr["TransactionDateTime"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.TransferFrom = Convert.ToInt32(dr["TransferFrom"]);
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransferFromNo = dr["TransferFromNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SDRate = Convert.ToDecimal(dr["SDRate"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());


                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ItemName = dr["ProductName"].ToString();

                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return VMs;
        }

        #endregion
    }
}
