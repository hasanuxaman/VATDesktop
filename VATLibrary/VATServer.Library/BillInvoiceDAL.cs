
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
using VATServer.Interface;
using Excel;

namespace VATServer.Library
{
    public class BillInvoiceDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        public string[] BillInvoiceInsert(BillInvoiceMasterVM Master, List<BillInvoiceDetailVM> BillInvoices, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            //////SqlConnection vcurrConn = VcurrConn;
            //////if (vcurrConn == null)
            //////{
            //////    VcurrConn = null;
            //////    Vtransaction = null;
            //////}

            int transResult = 0;
            string sqlText = "";
            string sqlText1 = "";
            string newIDCreate = "";
            string PostStatus = "";
            int IDExist = 0;

            #endregion Initializ

            #region Try

            try
            {

              
                #region Validation for Header


                #endregion Validation for Header

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
                    transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Fiscal Year Check


                string transactionDate = Master.BillDate;
                string transactionYearCheck = Convert.ToDateTime(Master.BillDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(BillNo) from BillInvoiceHeaders WHERE BillNo= @MasterId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterId", Master.BillNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region BillInvoice ID Create

                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgTransactionNotDeclared);
                }

                CommonDAL commonDal = new CommonDAL();
                
                if (Master.TransactionType == "Other")
                {
                    newIDCreate = commonDal.TransactionCode("BillInvoice", "BillInvoice", "BillInvoiceHeaders", "BillNo",
                                             "BillDate", Master.BillDate, Master.BranchId.ToString(), currConn, transaction);
                }
                Master.BillNo = newIDCreate;
                #endregion BillInvoice ID Create Complete

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into BillInvoiceHeaders(";
                sqlText += " BillNo,";
                sqlText += " BranchId,";
                sqlText += " BillDate,";
                sqlText += " CustomerID,";
                sqlText += " PONo,";
                sqlText += " ChallanNo,";    
                sqlText += " TransactionType,";
                sqlText += " Post,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn";



                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += " @newID,";
                //sqlText += " @MasterBillNo,";
                sqlText += " @BranchId,";
                sqlText += " @MasterBillDate,";
                sqlText += " @MasterCustomerID,";
                sqlText += " @MasterPONo,";
                sqlText += " @MasterChallanNo,";
                sqlText += " @MasterTransactionType,";
                sqlText += " @MasterPost,";
                sqlText += " @MasterCreatedBy,";
                sqlText += " @MasterCreatedOn,";
                sqlText += " @MasterLastModifiedBy,";
                sqlText += " @MasterLastModifiedOn";

                sqlText += ") SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", Master.BillNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterBillDate", Master.BillDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPONo", Master.PONo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChallanNo", Master.ChallanNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                Id = transResult.ToString();
                Master.Id = Id;

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region BillInvoice Insert

                retResults = BillInvoiceInsert2(Master, BillInvoices, transaction, currConn);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.billinvoiceMsgMethodNameUpdate, retResults[1]);
                }

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"
UPDATE BillInvoiceHeaders 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(BillDate)) +  CONVERT(VARCHAR(4),YEAR(BillDate)),6)
WHERE BillNo = @BillNo

UPDATE BillInvoiceDetails 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(BillDate)) +  CONVERT(VARCHAR(4),YEAR(BillDate)),6)
WHERE BillNo = @BillNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@BillNo", Master.BillNo);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Update Sales IsBillCompleted

                sqlText1 = "";
                sqlText1 += @"
  update SalesInvoiceHeaders set IsBillCompleted='Y' where SalesInvoiceNo=@ChallanNo


";

                SqlCommand cmdUpdate1 = new SqlCommand(sqlText1, currConn, transaction);
                cmdUpdate1.Parameters.AddWithValue("@ChallanNo", Master.ChallanNo);

                transResult = cmdUpdate1.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }

                ////if (vcurrConn == null)
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

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = "" + Master.BillNo;
                retResults[3] = "N";
                retResults[4] = Id;

                #endregion SuccessResult

            }

            #endregion Try

            #region Catch and Finall

            #region Catch

            #region Comment before 28 Oct 2020

            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}

            #endregion

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositDAL", "DepositInsert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
            }

            #endregion Catch

            #region Finally

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }


            }

            #endregion Finally

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public string[] BillInvoiceInsert2(BillInvoiceMasterVM Master, List<BillInvoiceDetailVM> BillInvoices, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";

            int transResult = 0;
            string sqlText = "";
            int IDExist = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                 #region Validation for Detail

                if (BillInvoices.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }


                    #endregion Validation for Detail

                 #region Insert Detail Table

                foreach (var Item in BillInvoices.ToList())
                    {
                        #region Find Transaction Exist

                        //if (Item.PurchaseNumber.Trim().ToUpper() != "NA")
                        //{
                        //    sqlText = "";
                        //    sqlText += "select COUNT(VDSId) from VDS WHERE DepositNumber='" + Master.DepositId + "' ";
                        //    sqlText += " AND PurchaseNumber=@ItemPurchaseNumber";
                        //    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        //    cmdFindId.Transaction = transaction;
                        //    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        //    IDExist = (int)cmdFindId.ExecuteScalar();

                        //    if (IDExist > 0)
                        //    {
                        //        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                        //                                        MessageVM.dpMsgFindExistIDP);
                        //    }
                        //}

                        #endregion Find Transaction Exist

                        if (string.IsNullOrEmpty(Item.BillNo))
                        {
                            Item.BillNo = Master.BillNo;

                        }

                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into BillInvoiceDetails(";

                        sqlText += " BillId";
                        sqlText += " ,BillNo";
                        sqlText += " ,ItemNo";
                        sqlText += " ,BillDate";
                        sqlText += " ,BranchId";

                        sqlText += " ,Quantity";
                        sqlText += " ,VATRate";
                        sqlText += " ,NBRPrice";
                        sqlText += " ,SubTotal";
                        sqlText += " ,VATAmount";
                        sqlText += " ,Post";
                        sqlText += " ,TransactionType";
                        sqlText += " ,CreatedBy";
                        sqlText += " ,CreatedOn";
                        sqlText += " ,LastModifiedBy";
                        sqlText += " ,LastModifiedOn";

                        sqlText += " )";

                        sqlText += " values(	";

                        //sqlText += "'" + Master.Id + "',";
                        sqlText += "@MasterBillId,";
                        sqlText += "@MasterBillNo,";
                        sqlText += "@ItemNo,";
                        sqlText += "@MasterBillDate,";
                        sqlText += "@BranchId,";
                        sqlText += "@Quantity,";
                        sqlText += "@VATRate,";
                        sqlText += "@NBRPrice,";
                        sqlText += "@SubTotal,";
                        sqlText += "@VATAmount,";
                        sqlText += "@MasterPost,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn";

                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterBillId", Master.Id);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterBillNo", Master.BillNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", Item.ItemNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterBillDate", Master.BillDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Quantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@NBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                            MessageVM.dpMsgSaveNotSuccessfully);
                        }

                        #endregion Insert only DetailTable


                    }


                    #endregion Insert Detail Table

                

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = Master.BillNo;
                retResults[3] = "N";
                retResults[4] = Id;

                #endregion SuccessResult

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";

                FileLogger.Log("DepositDAL", "DepositInsert2", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public List<BillInvoiceMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<BillInvoiceMasterVM> VMs = new List<BillInvoiceMasterVM>();
            BillInvoiceMasterVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, false, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new BillInvoiceMasterVM();
                        vm.Id = dr["Id"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.BillNo = dr["BillNo"].ToString();
                        vm.BillDate = OrdinaryVATDesktop.DateTimeToDate(dr["BillDate"].ToString());
                        vm.ChallanNo = dr["ChallanNo"].ToString();
                        vm.PONo = dr["PONo"].ToString();
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e) { }
                }


                #endregion SqlExecution

                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
                //}
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("BillInvoiceDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("BillInvoiceDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            //finally
            //{
            //    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            //    {
            //        currConn.Close();
            //    }
            //}
            #endregion
            return VMs;
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            string count = "100";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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
                int index = -1;
                if (conditionFields != null && conditionValues != null)
                {
                    index = Array.IndexOf(conditionFields, "SelectTop");
                    if (index >= 0)
                    {
                        count = conditionValues[index].ToString();

                        var field = conditionFields.ToList();
                        var Values = conditionValues.ToList();
                        field.RemoveAt(index);
                        Values.RemoveAt(index);
                        conditionFields = field.ToArray();
                        conditionValues = Values.ToArray();
                    }
                }
                #region sql statement
                #region SqlText
                #region SqlText
                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"
b.BillNo
,b.BranchId
,b.BillDate
,b.CustomerID
,b.PONo
,b.ChallanNo
,b.TransactionType
,b.Post
,b.PeriodID
,b.CreatedBy
,b.CreatedOn
,b.LastModifiedBy
,b.LastModifiedOn
,b.Id

FROM BillInvoiceHeaders b
WHERE  1=1
";

                #endregion

                if (Id > 0)
                {
                    sqlTextParameter += @" and Id=@Id";
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
                            sqlTextParameter += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
              
                #endregion SqlText
                #region SqlExecution
                sqlText = sqlText + " " + sqlTextParameter;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;



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
                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }

                da.Fill(ds);
                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                dt = ds.Tables[0].Copy();
                if (index >= 0)
                {
                    dt.Rows.Add(ds.Tables[1].Rows[0][0]);
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

        public List<BillInvoiceDetailVM> SelectAllListdetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<BillInvoiceDetailVM> VMs = new List<BillInvoiceDetailVM>();
            BillInvoiceDetailVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAllDetails(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, false, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new BillInvoiceDetailVM();
                        //vm.BillId = dr["BillId"].ToString();
                        vm.BillId = Convert.ToInt32(dr["BillId"]);
                        vm.BillNo = dr["BillNo"].ToString();
                        vm.BillDate = OrdinaryVATDesktop.DateTimeToDate(dr["BillDate"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                        vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                        vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                        vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.Id = dr["Id"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e) { }
                }


                #endregion SqlExecution

                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
                //}
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("BillInvoiceDetailsDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("BillInvoiceDetailsDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            //finally
            //{
            //    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            //    {
            //        currConn.Close();
            //    }
            //}
            #endregion
            return VMs;
        }

        public DataTable SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            string count = "100";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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
                int index = -1;
                if (conditionFields != null && conditionValues != null)
                {
                    index = Array.IndexOf(conditionFields, "SelectTop");
                    if (index >= 0)
                    {
                        count = conditionValues[index].ToString();

                        var field = conditionFields.ToList();
                        var Values = conditionValues.ToList();
                        field.RemoveAt(index);
                        Values.RemoveAt(index);
                        conditionFields = field.ToArray();
                        conditionValues = Values.ToArray();
                    }
                }
                #region sql statement
                #region SqlText
                #region SqlText
                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"
bd.BillId
,bd.BillNo
,bd.BillDate
,bd.BranchId
,bd.Quantity
,bd.VATRate
,bd.SubTotal
,bd.VATAmount
,bd.Post
,bd.PeriodID
,bd.TransactionType
,bd.CreatedBy
,bd.CreatedOn
,bd.LastModifiedBy
,bd.LastModifiedOn
,bd.Id

FROM BillInvoiceDetails bd
WHERE  1=1
";

                #endregion

                if (Id > 0)
                {
                    sqlTextParameter += @" and BillId=@Id";
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
                            sqlTextParameter += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }

                #endregion SqlText
                #region SqlExecution
                sqlText = sqlText + " " + sqlTextParameter;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;



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
                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }

                da.Fill(ds);
                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                dt = ds.Tables[0].Copy();
                if (index >= 0)
                {
                    dt.Rows.Add(ds.Tables[1].Rows[0][0]);
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
        
    }
}
