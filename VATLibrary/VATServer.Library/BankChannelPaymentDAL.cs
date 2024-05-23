using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class BankChannelPaymentDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        ProductDAL _ProductDAL = new ProductDAL();

        private DataTable DtPurchaseD;

        CommonDAL commonDal = new CommonDAL();

        #endregion
         
        public string[] BankChannelInsertList(List<BankChannelPaymentVM> BankChannelVMList, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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


            int transResult = 0;
            string sqlText = "";
            string newIDCreate1 = "";
            int IDExist = 0;

            #endregion Initializ

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

                #region Insert Detail Table

                foreach (var Item in BankChannelVMList.ToList())
                {

                    #region Bank Channel

                    #region Make Vm

                    BankChannelPaymentVM bVM = new BankChannelPaymentVM();

                    bVM.Id = Item.Id;
                    bVM.PurchaseInvoiceNo = Item.PurchaseInvoiceNo;
                    bVM.BankID = Item.BankID;
                    bVM.PaymentDate = Item.PaymentDate;
                    bVM.PaymentAmount = Item.PaymentAmount;
                    bVM.VATAmount = Item.VATAmount;
                    bVM.Remarks = Item.Remarks;
                    bVM.PaymentType = Item.PaymentType;
                    bVM.CreatedBy = Item.CreatedBy;
                    bVM.CreatedOn = Item.CreatedOn;
                    bVM.LastModifiedBy = Item.LastModifiedBy;
                    bVM.LastModifiedOn = Item.LastModifiedOn;
                    bVM.Post = Item.Post;

                    #endregion

                    #region Find ID

                    sqlText = "";
                    sqlText = @"select COUNT(PurchaseInvoiceNo) from BankChannelPayment WHERE PurchaseInvoiceNo=@PurchaseInvoiceNo ";
                    SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                    cmdFindIdUpd.Transaction = transaction;
                    cmdFindIdUpd.Parameters.AddWithValue("@PurchaseInvoiceNo", bVM.PurchaseInvoiceNo);

                    int IDExistP = (int)cmdFindIdUpd.ExecuteScalar();

                    if (IDExistP <= 0)
                    {

                        retResults = Insert(bVM, currConn, transaction);

                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, retResults[1]);
                        }

                    }
                    else
                    {
                        retResults = Update(bVM, currConn, transaction);

                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, retResults[1]);
                        }

                    }

                    #endregion Find ID for Update

                    #endregion

                }

                #endregion Insert Detail Table

                #region Commit

                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {

                        retResults[0] = "Success";
                        retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;

                        transaction.Commit();

                    }
                }

                #endregion Commit

            }

            #region Catch and Finall

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

                FileLogger.Log("BankChannelPaymentDAL", "BankChannelInsertList", ex.ToString() + "\n" + sqlText);

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

            return retResults;

        }

        public string[] Insert(BankChannelPaymentVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Fiscal Year Check

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.PaymentDate).ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion Find Month Lock

                string transactionDate = Master.PaymentDate;
                string transactionYearCheck = Convert.ToDateTime(Master.PaymentDate).ToString("yyyy-MM-dd");
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

                #region Find ID

                sqlText = "";
                sqlText = @"select COUNT(PurchaseInvoiceNo) from BankChannelPayment WHERE PurchaseInvoiceNo=@PurchaseInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@PurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                int IDExistP = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExistP > 0)
                {
                    throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNameUpdate, MessageVM.PurchasemsgFindExistID);
                }

                #endregion Find ID for Update

                #region Insert

                sqlText = "";
                sqlText += " insert into BankChannelPayment";
                sqlText += " (";
                sqlText += " PurchaseInvoiceNo,";
                sqlText += " BankID,";
                sqlText += " PaymentDate,";
                sqlText += " PaymentAmount,";
                sqlText += " VATAmount,";
                sqlText += " Remarks,";
                sqlText += " PaymentType,";
                sqlText += " Post,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@PurchaseInvoiceNo,";
                sqlText += "@BankID,";
                sqlText += "@PaymentDate,";
                sqlText += "@PaymentAmount,";
                sqlText += "@VATAmount,";
                sqlText += "@Remarks,";
                sqlText += "@PaymentType,";
                sqlText += "@Post,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn";

                sqlText += ") SELECT SCOPE_IDENTITY()";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@PurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankID", Master.BankID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PaymentDate", OrdinaryVATDesktop.DateToDate(Master.PaymentDate));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PaymentAmount", Master.PaymentAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATAmount", Master.VATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PaymentType", Master.PaymentType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Remarks", Master.Remarks);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());

                retResults[4] = transResult.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
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
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = Master.PurchaseInvoiceNo;
                retResults[3] = PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

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

                FileLogger.Log("BankChannelPaymentDAL", "Insert", ex.ToString() + "\n" + sqlText);

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

        public string[] Update(BankChannelPaymentVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Fiscal Year Check

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.PaymentDate).ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion Find Month Lock

                string transactionDate = Master.PaymentDate;
                string transactionYearCheck = Convert.ToDateTime(Master.PaymentDate).ToString("yyyy-MM-dd");
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
                sqlText = @"select COUNT(PurchaseInvoiceNo) from BankChannelPayment WHERE PurchaseInvoiceNo=@PurchaseInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@PurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                int IDExistP = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExistP <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNameUpdate, MessageVM.PurchasemsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region Update

                sqlText = "";
                sqlText += " update BankChannelPayment set  ";
                sqlText += "   BankID   =@BankID";
                sqlText += "  ,PaymentDate   =@PaymentDate";
                sqlText += "  ,PaymentAmount   =@PaymentAmount";
                sqlText += "  ,VATAmount   =@VATAmount";
                sqlText += "  ,Remarks   =@Remarks";
                sqlText += "  ,PaymentType   =@PaymentType";
                sqlText += "  ,Post   =@Post";
                sqlText += "  ,LastModifiedBy   =@LastModifiedBy";
                sqlText += "  ,LastModifiedOn   =@LastModifiedOn";

                sqlText += " where PurchaseInvoiceNo=@PurchaseInvoiceNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BankID", Master.BankID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PaymentDate", OrdinaryVATDesktop.DateToDate(Master.PaymentDate));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PaymentAmount", Master.PaymentAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATAmount", Master.VATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Remarks", Master.Remarks);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PaymentType", Master.PaymentType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                ////cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNameUpdate, MessageVM.PurchasemsgSaveNotSuccessfully);
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
                retResults[1] = MessageVM.PurchasemsgUpdateSuccessfully;
                retResults[2] = Master.PurchaseInvoiceNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }

            #endregion Try

            #region Catch and Finall

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

                //////2020-12-13
                FileLogger.Log("BankChannelPaymentDAL", "Update", ex.ToString() + "\n" + sqlText);

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

        public string[] BankChannelPostList(List<BankChannelPaymentVM> BankChannelVMList, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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

            int transResult = 0;
            string sqlText = "";
            string newIDCreate1 = "";
            int IDExist = 0;

            #endregion Initializ

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

                #region Insert Detail Table

                foreach (var Item in BankChannelVMList.ToList())
                {

                    #region Bank Channel

                    #region Make Vm

                    BankChannelPaymentVM bVM = new BankChannelPaymentVM();

                    bVM.Id = Item.Id;
                    bVM.PurchaseInvoiceNo = Item.PurchaseInvoiceNo;
                    bVM.BankID = Item.BankID;
                    bVM.PaymentDate = Item.PaymentDate;
                    bVM.PaymentAmount = Item.PaymentAmount;
                    bVM.VATAmount = Item.VATAmount;
                    bVM.Remarks = Item.Remarks;
                    bVM.CreatedBy = Item.CreatedBy;
                    bVM.CreatedOn = Item.CreatedOn;
                    bVM.LastModifiedBy = Item.LastModifiedBy;
                    bVM.LastModifiedOn = Item.LastModifiedOn;
                    bVM.Post = Item.Post;

                    #endregion

                    #region Find ID

                    sqlText = "";
                    sqlText = @"select COUNT(PurchaseInvoiceNo) from BankChannelPayment WHERE PurchaseInvoiceNo=@PurchaseInvoiceNo ";
                    SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                    cmdFindIdUpd.Transaction = transaction;
                    cmdFindIdUpd.Parameters.AddWithValue("@PurchaseInvoiceNo", bVM.PurchaseInvoiceNo);

                    int IDExistP = (int)cmdFindIdUpd.ExecuteScalar();

                    if (IDExistP <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNameUpdate, MessageVM.PurchasemsgUnableFindExistID);
                    }

                    retResults = Post(bVM,transaction,currConn);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost, retResults[1]);
                    }

                    #endregion Find ID for Update

                    #endregion

                }

                #endregion Insert Detail Table

                #region Commit

                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {

                        retResults[0] = "Success";
                        retResults[1] = MessageVM.purchaseMsgSuccessfullyPost;
                        retResults[3] = "Y";
                        //retResults[4] = "";

                        transaction.Commit();

                    }
                }

                #endregion Commit


            }

            #region Catch and Finall

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

                FileLogger.Log("BankChannelPaymentDAL", "BankChannelInsertList", ex.ToString() + "\n" + sqlText);

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

            return retResults;

        }

        public string[] Post(BankChannelPaymentVM Master, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            IssueDAL issDal = new IssueDAL();
            ReceiveDAL recDal = new ReceiveDAL();
            int transResult = 0;
            string sqlText = "";


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNamePost, MessageVM.purchaseMsgNoDataToPost);
                }

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
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.PaymentDate).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");

                }

                #endregion Find Month Lock

                #region Fiscal Year Check
                string PeriodIdCheck = Convert.ToDateTime(Master.PaymentDate).ToString("MMyyyy");

                string transactionDate = Master.PaymentDate;
                string transactionYearCheck = Convert.ToDateTime(Master.PaymentDate).ToString("yyyy-MM-dd");
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

                #region Find ID for Post

                sqlText = "";

                sqlText = @"select COUNT(PurchaseInvoiceNo) from BankChannelPayment WHERE PurchaseInvoiceNo=@PurchaseInvoiceNo";

                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@PurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BankChannelmsgMethodNamePost,
                                                    MessageVM.purchaseMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region Update Header

                string[] cFields = { "bcp.PurchaseInvoiceNo" };
                string[] cvals = { Master.PurchaseInvoiceNo };

                BankChannelPaymentVM pmVM = SelectAllList(0, cFields, cvals, currConn, transaction, null, null).FirstOrDefault();

                string settingValue = commonDal.settingValue("EntryProcess", "UpdateOnPost");

                if (settingValue != "Y")
                {
                    if (pmVM.Post == "Y")
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.ThisTransactionWasPosted);
                    }

                }

                #endregion update Header

                #region Post Data

                sqlText = "";
                sqlText += @" Update  BankChannelPayment set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE PurchaseInvoiceNo=@PurchaseInvoiceNo ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@PurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.purchaseMsgSuccessfullyPost;
                retResults[2] = Master.PurchaseInvoiceNo;
                retResults[3] = "Y";
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("BankChannelPaymentDAL", "Post", ex.ToString() + "\n" + sqlText);

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

        public List<BankChannelPaymentVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<BankChannelPaymentVM> VMs = new List<BankChannelPaymentVM>();
            BankChannelPaymentVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id.ToString(), conditionFields, conditionValues, VcurrConn, Vtransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new BankChannelPaymentVM();
                    vm.Id = dr["Id"].ToString();
                    vm.PurchaseInvoiceNo = dr["PurchaseInvoiceNo"].ToString();

                    vm.BankID = dr["BankID"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    vm.PaymentDate = OrdinaryVATDesktop.DateTimeToDate(dr["PaymentDate"].ToString());
                    vm.PaymentAmount = Convert.ToDecimal(dr["PaymentAmount"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

                    VMs.Add(vm);

                }


                #endregion SqlExecution

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("BankChannelPaymentDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string PurchaseInvoiceNo = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

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

                #region sql statement

                #region SqlText

                sqlText = @"
SELECT bcp.Id
      ,bcp.PurchaseInvoiceNo
      ,bcp.BankID
      ,bcp.PaymentDate
      ,bcp.PaymentAmount
      ,bcp.VATAmount
      ,bcp.Remarks
      ,bcp.PaymentType
      ,bcp.Post
      ,bcp.CreatedBy
      ,bcp.CreatedOn
      ,bcp.LastModifiedBy
      ,bcp.LastModifiedOn
	  ,bnk.BankName
  FROM BankChannelPayment bcp
  left outer join BankInformations bnk on bcp.BankID=bnk.BankID where 1=1

";
                if (!string.IsNullOrWhiteSpace(Id) && Id != "0")
                {
                    sqlText += @" and bcp.Id=@Id";
                }

                if (!string.IsNullOrWhiteSpace(PurchaseInvoiceNo))
                {
                    sqlText += @" and bcp.PurchaseInvoiceNo in(" + PurchaseInvoiceNo + ")";
                }

                #region cField

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
                #endregion

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

                if (!string.IsNullOrWhiteSpace(Id) && Id != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }
                if (!string.IsNullOrWhiteSpace(PurchaseInvoiceNo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo);                
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
                FileLogger.Log("BankChannelPaymentDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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


    }
}
