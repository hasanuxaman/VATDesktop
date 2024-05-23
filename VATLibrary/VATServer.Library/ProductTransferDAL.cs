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
using Newtonsoft.Json;
namespace VATServer.Library
{
    public class ProductTransferDAL
    {
        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        ProductDAL _ProductDAL = new ProductDAL();

        #endregion

        #region Navigation

        public NavigationVM Transfer_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

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


                #region SQL Statement

                #region SQL Text

                sqlText = "";
                sqlText = @"
------declare @Id as int = 14393;
------declare @FiscalYear as int = 2021;
------declare @TransactionType as varchar(50) = 'Other';
------declare @BranchId as int = 1;

";
                if (vm.ButtonName == "Current")
                {
                    #region Current Item

                    sqlText = sqlText + @"
--------------------------------------------------Current--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferCode InvoiceNo from ProductTransfers inv
where 1=1 
and inv.TransferCode=@InvoiceNo

";
                    #endregion
                }
                else if (vm.Id == 0 || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferCode InvoiceNo from ProductTransfers inv
where 1=1 
and BranchId = @BranchId
and TransactionType =@TransactionType
order by Id asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Last")
                {

                    #region Last Item

                    sqlText = sqlText + @"
--------------------------------------------------Last--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferCode InvoiceNo from ProductTransfers inv
where 1=1 
and BranchId = @BranchId
and TransactionType =@TransactionType
order by Id desc


";
                    #endregion

                }
                else if (vm.ButtonName == "Next")
                {

                    #region Next Item

                    sqlText = sqlText + @"
--------------------------------------------------Next--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferCode InvoiceNo from ProductTransfers inv
where 1=1 
and BranchId = @BranchId
and TransactionType =@TransactionType
and Id > @Id
order by Id asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Previous")
                {
                    #region Previous Item

                    sqlText = sqlText + @"
--------------------------------------------------Previous--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferCode InvoiceNo from ProductTransfers inv
where 1=1 
and BranchId = @BranchId
and TransactionType =@TransactionType
and Id < @Id
order by Id desc

";
                    #endregion
                }


                #endregion

                #region SQL Execution
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);


                if (vm.ButtonName == "Current")
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo);
                }
                else
                {

                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType);

                    if (vm.Id > 0)
                    {
                        cmd.Parameters.AddWithValue("@Id", vm.Id);
                    }
                }



                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    vm.InvoiceNo = dt.Rows[0]["InvoiceNo"].ToString();
                }
                else
                {
                    if (vm.ButtonName == "Previous" || vm.ButtonName == "Current")
                    {
                        vm.ButtonName = "First";
                        vm = Transfer_Navigation(vm, currConn, transaction);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = Transfer_Navigation(vm, currConn, transaction);

                    }
                }


                #endregion

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #region catch

            catch (Exception ex)
            {
                throw new ArgumentNullException("", ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return vm;

        }


        #endregion

        #region Basic Methods

        //currConn to VcurrConn 25-Aug-2020
        public string[] Insert(ProductTransfersVM Master, List<ProductTransfersDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null, string UserId = null)
        {
            #region Initializ

            var commonDAL = new CommonDAL();

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";


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

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransferDate).ToString("MMMM-yyyy");
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

                #region Validation for Header
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.TransferDate) < DateTime.MinValue || Convert.ToDateTime(Master.TransferDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");
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

                #region Fiscal Year Check

                string transactionDate = Master.TransferDate;

                string transactionYearCheck = Convert.ToDateTime(Master.TransferDate).ToString("yyyy-MM-dd");
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

                #region Invoice ID Create

                #region ProductTransfers ID Create For Other
                CommonDAL commonDal = new CommonDAL();
                Master.TransactionType = Master.GetTransactionType();
                if (Master.TransactionType.ToLower() == "RawCTC".ToLower())
                {
                    newID = commonDal.TransactionCode("TransferCTC", "TransferRaw", "ProductTransfers", "TransferCode",
                                             "TransferDate", Master.TransferDate, Master.BranchId.ToString(), currConn, transaction, connVM);
                }
                else if (Master.TransactionType.ToLower() == "WastageCTC".ToLower())
                {
                    newID = commonDal.TransactionCode("TransferCTC", "TransferWastage", "ProductTransfers", "TransferCode",
                                            "TransferDate", Master.TransferDate, Master.BranchId.ToString(), currConn, transaction, connVM);
                }
                else if (Master.TransactionType.ToLower() == "FinishCTC".ToLower())
                {
                    newID = commonDal.TransactionCode("TransferCTC", "TransferFinish", "ProductTransfers", "TransferCode",
                                            "TransferDate", Master.TransferDate, Master.BranchId.ToString(), currConn, transaction, connVM);
                }

                #endregion ProductTransfers ID Create For Other

                #endregion

                #region ID generated completed,Insert new Information in Header

                Master.TransferCode = newID;

                #region Find Transaction Exist
                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferCode) from ProductTransfers WHERE TransferCode='" + Master.TransferCode + "' ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                IDExist = (int)cmdExistTran.ExecuteScalar();
                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist

                retResults = TransferIssueInsertToMaster(Master, currConn, transaction, connVM);

                Id = retResults[4];
                if (retResults[0] != "Success")
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

                    //if (multiple == "N")
                    //{
                    //    sqlText = "";
                    //    sqlText += "select COUNT(TransferIssueNo) from ProductTransfersDetails WHERE TransferIssueNo='" + newID + "' ";
                    //    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    //    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    //    cmdFindId.Transaction = transaction;
                    //    IDExist = (int)cmdFindId.ExecuteScalar();
                    //    if (IDExist > 0)
                    //    {
                    //        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    //    }
                    //}


                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    //Item.TransferDate = Master.TransferDate;
                    //Item.FromBranchId = Master.;
                    Item.Post = Master.Post;
                    Item.ProductTransferId = Convert.ToInt32(Id);
                    Item.TransactionType = Master.GetTransactionType();

                    retResults = TransferIssueInsertToDetail(Item, currConn, transaction);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable
                }

                #endregion Insert Detail Table


                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.ProductTransfers WHERE TransferCode='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }
                #endregion Prefetch

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                retResults[4] = Id;
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
                if (currConn != null)
                {
                    transaction.Rollback();
                }
                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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

        public string[] Update(ProductTransfersVM Master, List<ProductTransfersDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = null)
        {

            #region Initializ

            string[] retResults = new string[6];
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

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransferDate).ToString("MMMM-yyyy");
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

                #region Validation for Header
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.TransferDate) < DateTime.MinValue || Convert.ToDateTime(Master.TransferDate) > DateTime.MaxValue)
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
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);
                #endregion open connection and transaction

                #region Fiscal Year Check
                string transactionDate = Master.TransferDate;
                string transactionYearCheck = Convert.ToDateTime(Master.TransferDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferCode) from ProductTransfers WHERE TransferCode='" + Master.TransferCode + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }
                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                retResults = TransferIssueUpdateToMaster(Master, currConn, transaction, connVM);

                if (retResults[0] != "Success")
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

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM ProductTransfersDetails WHERE ProductTransferId=@ProductTransferId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@ProductTransferId", Master.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Update Detail Table

                foreach (ProductTransfersDetailVM Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    // Insert
                    #region Insert only DetailTable
                    Item.ProductTransferId = Master.Id;
                    Item.Post = Master.Post;
                    Item.TransactionType = Master.GetTransactionType();
                    retResults = TransferIssueInsertToDetail(Item, currConn, transaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                    }
                    #endregion Insert only DetailTable

                    #endregion Find Transaction Mode Insert or Update

                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct Post from ProductTransfers WHERE TransferCode='" + Master.TransferCode + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
                }
                #endregion Prefetch

                #region Commit
                if (transaction != null)
                {

                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.TransferCode;
                retResults[3] = PostStatus;
                retResults[4] = Master.Id.ToString();
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
                if (currConn != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
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

        public string[] Post(ProductTransfersVM Master, List<ProductTransfersDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";// DB Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";
            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransferDate).ToString("MMMM-yyyy");
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


                #region Validation for Header
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.TransferDate) < DateTime.MinValue || Convert.ToDateTime(Master.TransferDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);
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

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'

select Id, Post from ProductTransfers
where 1=1 
and Id=@Id

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Id", Master.Id);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                if (currentPostStatus == "Y")
                {
                    throw new Exception("This Invoice Already Posted! Invoice No: " + Master.TransferCode);
                }

                #endregion

                #region Fiscal Year Check
                string transactionDate = Master.TransferDate;
                string transactionYearCheck = Convert.ToDateTime(Master.TransferDate).ToString("yyyy-MM-dd");
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

                //#region Avg Price Update

                //ProductDAL productDal = new ProductDAL();

                //foreach (ProductTransfersDetailVM transfersDetailVm in Details)
                //{
                //    if (transfersDetailVm.IsWastage=="F")
                //    {
                //        var product = productDal.SelectAll(transfersDetailVm.FromItemNo).FirstOrDefault();
                //        transfersDetailVm.IssuePrice = product.NBRPrice;
                //        transfersDetailVm.ReceivePrice = product.NBRPrice;
                //    }
                //    else
                //    {
                //        DataTable priceData = productDal.AvgPriceNew(transfersDetailVm.FromItemNo, transfersDetailVm.GetAvgPriceDate(), currConn, transaction, true,
                //       true, true, false, connVM, UserId);

                //        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                //        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                //        transfersDetailVm.SetIssuePrice(quan, amount);

                //        priceData = productDal.AvgPriceNew(transfersDetailVm.ToItemNo, transfersDetailVm.GetAvgPriceDate(), currConn, transaction, true,
                //            true, true, false, connVM, UserId);
                //        amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                //        quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                //        transfersDetailVm.SetReceivePrice(quan, amount);
                //    }

                //}

                //#endregion


                #region Find ID for Update
                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferCode) from ProductTransfers WHERE TransferCode='" + Master.TransferCode + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }
                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                sqlText = "";
                sqlText += @" Update  ProductTransfers                 set  Post ='Y'  , LastModifiedBy =@LastModifiedBy, LastModifiedOn =@LastModifiedOn   WHERE Id=@Id ";
                sqlText += @" Update  ProductTransfersDetails          set  Post ='Y'   WHERE ProductTransferId=@Id ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);

                transResult = cmdUpdate.ExecuteNonQuery();


                //sqlText =
                //    "Update ProductTransfersDetails  set IssuePrice=@IssuePrice, ReceivePrice=@ReceivePrice   " +
                //    "WHERE ProductTransferId=@Id  and FromItemNo=@FromItemNo and ToItemNo=@ToItemNo";
                //cmdUpdate.CommandText = sqlText;
                //foreach (ProductTransfersDetailVM productTransfersDetailVm in Details)
                //{
                //    cmdUpdate.Parameters.AddWithValueAndParamCheck("@IssuePrice",productTransfersDetailVm.IssuePrice);
                //    cmdUpdate.Parameters.AddWithValueAndParamCheck("@ReceivePrice", productTransfersDetailVm.ReceivePrice);
                //    cmdUpdate.Parameters.AddWithValueAndParamCheck("@FromItemNo", productTransfersDetailVm.FromItemNo);
                //    cmdUpdate.Parameters.AddWithValueAndParamCheck("@ToItemNo", productTransfersDetailVm.ToItemNo);

                //    cmdUpdate.ExecuteNonQuery();
                //}

                #endregion update Header
                #region Commonted
                //                #region Update Product Stocks Table
                //                if (Master.IsWastage == "Y" && transResult>0)


                //               foreach (var Item in Details.ToList())
                //                {
                //                    Decimal WastageQuantity = 0;
                //                    Decimal WastageToptalQuantity = 0;

                //                    Item.ToQuantity = Item.ToQuantity;
                //                    Item.BranchId = Item.BranchId;
                //                    Item.ToItemNo = Item.ToItemNo;

                //                    #region Find Transaction Mode Insert or Update

                //                    #region Find WastageTotalQuantity From ProductStocks

                //                    sqlText = "";
                //                    sqlText = @"
                //
                //          select [Id]
                //                ,[ItemNo]
                //                ,[BranchId]
                //                ,Isnull([StockQuantity],0) [StockQuantity]
                //                ,Isnull([StockValue],0) [StockValue]
                //                ,Isnull([Comments],'-')[Comments]
                //                ,Isnull([CurrentStock],0)[CurrentStock]
                //                ,Isnull([WastageTotalQuantity],0)[WastageTotalQuantity]
                //				from ProductStocks
                //                where 1=1 
                //                and ItemNo=@ItemNo
                //                and BranchId=@BranchId
                //
                //
                //";
                //                    SqlCommand cmdFind = new SqlCommand(sqlText, currConn, transaction);
                //                    cmdFind.Parameters.AddWithValue("@ItemNo", Item.ToItemNo);
                //                    cmdFind.Parameters.AddWithValue("@BranchId", Item.BranchId);

                //                    SqlDataAdapter DataAdapter = new SqlDataAdapter(cmdFind);
                //                    DataTable ProductStocks = new DataTable();
                //                    DataAdapter.Fill(ProductStocks);

                //                    if (ProductStocks != null && ProductStocks.Rows.Count > 0)
                //                    {
                //                        WastageQuantity = Convert.ToDecimal(ProductStocks.Rows[0]["WastageTotalQuantity"].ToString());
                //                    }
                //                    else
                //                    {

                //                        var Product = new ProductDAL().SelectAll("0", new[] { "Pr.ItemNo", "Pr.BranchId" }, new[] { Item.ToItemNo, Item.BranchId.ToString() }).FirstOrDefault();
                //                        #region Product Stock Id Check
                //                        sqlText = "";
                //                        sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  ProductStocks";
                //                        SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                //                        cmdNextId.Transaction = transaction;
                //                        int nextId = (int)cmdNextId.ExecuteScalar();
                //                        #endregion
                //                        #region Insert new Product Stocks
                //                        sqlText = "";
                //                        sqlText += "insert into ProductStocks";
                //                        sqlText += "(";
                //                        sqlText += "Id,";
                //                        sqlText += "ItemNo,";
                //                        sqlText += "BranchId,";
                //                        sqlText += "StockQuantity,";
                //                        sqlText += "StockValue,";
                //                        sqlText += "Comments,";
                //                        sqlText += "WastageTotalQuantity";
                //                        sqlText += ")";
                //                        sqlText += " values(";
                //                        sqlText += " @Id,";
                //                        sqlText += " @ItemNo,";
                //                        sqlText += " @BranchId,";
                //                        sqlText += " @StockQuantity,";
                //                        sqlText += " @StockValue,";
                //                        sqlText += " 'NA',";
                //                        sqlText += " '0'";
                //                        sqlText += ")SELECT SCOPE_IDENTITY()";


                //                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                //                        cmdInsert.Transaction = transaction;
                //                        cmdInsert.Parameters.AddWithValue("@Id", nextId);
                //                        cmdInsert.Parameters.AddWithValue("@ItemNo", Product.ItemNo);
                //                        cmdInsert.Parameters.AddWithValue("@BranchId", Product.BranchId);
                //                        cmdInsert.Parameters.AddWithValue("@StockQuantity", Product.OpeningBalance);
                //                        cmdInsert.Parameters.AddWithValue("@StockValue", Product.OpeningTotalCost);
                //                        transResult = (int)cmdInsert.ExecuteNonQuery();


                //                        #endregion 
                //                    }

                //                    WastageToptalQuantity = WastageQuantity + Item.ToQuantity;


                //                    #endregion

                //                    #region Update

                //                    sqlText = @"update ProductStocks set WastageTotalQuantity=@WastageTotalQuantity
                //                    where ItemNo=@ItemNo AND BranchId=@BranchId";
                //                    SqlCommand cmdupdate = new SqlCommand(sqlText, currConn);
                //                    cmdupdate.Transaction = transaction;
                //                    cmdupdate.Parameters.AddWithValue("@WastageTotalQuantity", WastageToptalQuantity);
                //                    cmdupdate.Parameters.AddWithValue("@ItemNo", Item.ToItemNo);
                //                    cmdupdate.Parameters.AddWithValue("@BranchId", Item.BranchId);
                //                    cmdupdate.ExecuteNonQuery();
                //                    #endregion Update


                //                }

                //                    #endregion Update Product Stocks Table
                //                #endregion Update Detail Table
                #endregion
                #endregion ID check completed,update Information in Header

                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct Post from ProductTransfers WHERE Id='" + Master.Id + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                }
                #endregion Prefetch

                #region Commit
                if (transaction != null && Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.TransferCode;
                retResults[3] = PostStatus;
                retResults[4] = Master.Id.ToString();
                #endregion SuccessResult
            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                //return retResults;

                FileLogger.Log("TransferIssueDAL", "Post", ex.ToString());

                throw;
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

        //        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = null)
        //        {

        //            #region Initializ
        //            string[] retResults = new string[4];
        //            retResults[0] = "Fail";
        //            retResults[1] = "Fail";
        //            retResults[2] = "";
        //            retResults[3] = "";
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            #endregion Initializ

        //            try
        //            {

        //                #region open connection and transaction

        //                currConn = _dbsqlConnection.GetConnection(connVM);
        //                if (currConn.State != ConnectionState.Open)
        //                {
        //                    currConn.Open();
        //                }

        //                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameUpdate);


        //                #endregion open connection and transaction

        //                for (int i = 0; i < Ids.Length - 1; i++)
        //                {
        //                    ProductTransfersVM Master = SelectAllList(Convert.ToInt32(Ids[i]), null, null, currConn, transaction, null).FirstOrDefault();
        //                    UserInformationVM userinformation = new UserInformationDAL().SelectAll(0, new[] { "ui.UserID" },
        //                           new[] { UserId }).FirstOrDefault();
        //                    Master.Post = "Y";
        //                    Master.IsTransfer = "N";
        //                    Master.SignatoryName = userinformation.FullName;
        //                    Master.SignatoryDesig = userinformation.Designation;
        //                    List<ProductTransfersDetailVM> Details = SelectDetail(Master.TransferIssueNo, null, null, currConn, transaction);
        //                    retResults = Post(Master, Details, currConn, transaction);

        //                    if (retResults[0] != "Success")
        //                    {
        //                        throw new Exception();
        //                    }
        //                }

        //                #region Commit
        //                if (transaction != null)
        //                {
        //                    transaction.Commit();
        //                }
        //                #endregion

        //            }
        //            catch (Exception ex)
        //            {
        //                retResults = new string[5];
        //                retResults[0] = "Fail";
        //                retResults[1] = ex.Message;
        //                retResults[2] = "0";
        //                retResults[3] = "N";
        //                retResults[4] = "0";
        //                throw ex;
        //            }
        //            finally
        //            {

        //            }

        //            #region Result
        //            return retResults;
        //            #endregion Result

        //        }

        public string[] PostTransfer(ProductTransfersVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";

            #endregion Initializ

            #region Try
            try
            {


                Master.Post = "Y";
                List<ProductTransfersDetailVM> Details = SelectDetail(Master.Id.ToString(), null, null, VcurrConn, Vtransaction, connVM);
                retResults = Post(Master, Details, VcurrConn, Vtransaction, connVM, UserId);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }

                #region Day End Process
                ////TollReceive
                ////TollReceiveRaw
                ////ClientRawReceive
                ////ClientFGReceiveWOBOM


                //UpdateAveragePrice(Master, currConn,transaction,commonDal,new List<PurchaseDetailVM>(),Master,"N");

                try
                {
                    CommonDAL commonDal = new CommonDAL();
                    string dayEnd = commonDal.settings("Purchase", "DayEndProcess", VcurrConn, Vtransaction, connVM);

                    if (dayEnd == "Y")
                    {
                        IssueDAL issueDAL = new IssueDAL();
                        AVGPriceVm priceVm = new AVGPriceVm();
                        priceVm.AvgDateTime = Master.TransferDate;
                        ResultVM resultVm = issueDAL.UpdateAvgPrice_New(priceVm, VcurrConn, Vtransaction);
                    }

                }
                catch (Exception e)
                {

                }


                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.TransferCode;
                //////retResults[3] = PostStatus;
                retResults[3] = retResults[3];
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
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
                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
            }
            finally
            {

            }
            #endregion Catch and Finall
            #region Result
            return retResults;
            #endregion Result
        }

        #endregion

        #region Plain Methods

        public string[] TransferIssueInsertToMaster(ProductTransfersVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += "  insert into ProductTransfers";
                sqlText += " (";
                sqlText += " TransferCode";
                sqlText += " ,TransferDate";
                sqlText += " ,BranchId";
                sqlText += " ,IsWastage";
                sqlText += " ,TransactionType";
                sqlText += " ,ActiveStatus";
                sqlText += " ,Post";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,LastModifiedBy";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@TransferCode";
                sqlText += ",@TransferDate";
                sqlText += ",@BranchId";
                sqlText += ",@IsWastage";
                sqlText += ",@TransactionType";
                sqlText += ",@ActiveStatus";
                sqlText += ",@Post";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedOn";
                sqlText += ",@LastModifiedBy";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferCode", Master.TransferCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferDate", OrdinaryVATDesktop.DateToDate(Master.TransferDate));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsWastage", Master.IsWastage);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", Master.ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.GetTransactionType());
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
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
                retResults[1] = "Requested insert into ProductTransfers successfully Added";
                retResults[2] = Master.TransferCode;
                ////retResults[3] = "" + PostStatus;
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

                throw new ArgumentNullException("", ex.Message.ToString());
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

        public string[] TransferIssueInsertToDetail(ProductTransfersDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
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

                #region Insert into Details(Insert complete in Header)

                #region Insert Detail Table

                #region Comments

                ////if (Detail.IsWastage == "F")
                ////{
                ////    var FromItemNoValue = productDal.GetBOMReferenceNo(Detail.FromItemNo, "VAT 4.3", Detail.TransferDate, currConn, transaction, "0", connVM).Rows[0]["NBRPrice"];
                ////    var ToItemNoValue = productDal.GetBOMReferenceNo(Detail.ToItemNo, "VAT 4.3", Detail.TransferDate, currConn, transaction, "0", connVM).Rows[0]["NBRPrice"];
                ////    Detail.IssuePrice = Convert.ToDecimal(FromItemNoValue) *Convert.ToDecimal(Detail.FromQuantity);
                ////    Detail.ReceivePrice = Convert.ToDecimal(ToItemNoValue) *Convert.ToDecimal(Detail.FromQuantity);

                ////    if (Detail.IssuePrice != Detail.ReceivePrice)
                ////    {
                ////        throw new ArgumentNullException("Transfer Rate","From and To Product Price not same for transfer");
                ////    }

                ////}
                ////else
                ////{
                ////    DataTable priceData = productDal.AVGStockNewMethod(Detail.FromItemNo, Detail.TransferDate, currConn, transaction, connVM);
                ////    if (priceData==null || priceData.Rows.Count<0)
                ////    {
                ////        throw new ArgumentNullException("Transfer Rate","From Product Ave Rate not Found to transfer");
                ////    }
                ////    //////////////decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                ////    //////////////decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                ////    //////////////var tt = amount / quan;
                ////    decimal avgRate = Convert.ToDecimal(priceData.Rows[0]["AvgRate"].ToString());

                ////    Detail.IssuePrice = avgRate * Detail.FromQuantity;
                ////    Detail.ReceivePrice = avgRate * Detail.FromQuantity; 

                ////}

                #endregion

                #region Insert only DetailTable ProductTransfersDetails

                sqlText = "";
                sqlText += " insert into ProductTransfersDetails(";
                sqlText += " ProductTransferId";
                sqlText += ",BranchId";
                sqlText += ",FromItemNo";
                sqlText += ",FromUOM";
                sqlText += ",FromQuantity";
                sqlText += ",FromUOMConversion";
                sqlText += ",ToItemNo";
                sqlText += ",ToUOM";
                sqlText += ",ToQuantity";
                sqlText += ",IsWastage";
                sqlText += ",ReceivePrice";
                sqlText += ",IssuePrice";
                sqlText += ",TransactionType";
                sqlText += ",TransferDate";
                sqlText += ",Post";
                sqlText += ",PackSize";
                sqlText += ",FromUnitPrice";
                sqlText += ",ToUnitPrice";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@ProductTransferId";
                sqlText += ",@BranchId";
                sqlText += ",@FromItemNo";
                sqlText += ",@FromUOM";
                sqlText += ",@FromQuantity";
                sqlText += ",@FromUOMConversion";
                sqlText += ",@ToItemNo";
                sqlText += ",@ToUOM";
                sqlText += ",@ToQuantity";
                sqlText += ",@IsWastage";
                sqlText += ",@ReceivePrice";
                sqlText += ",@IssuePrice";
                sqlText += ",@TransactionType";
                sqlText += ",@TransferDate";
                sqlText += ",@Post";
                sqlText += ",@PackSize";
                sqlText += ",@FromUnitPrice";
                sqlText += ",@ToUnitPrice";

                sqlText += ")	";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ProductTransferId", Detail.ProductTransferId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FromItemNo", Detail.FromItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FromUOM", Detail.FromUOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FromQuantity", Detail.FromQuantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FromUOMConversion", Detail.FromUOMConversion);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ToItemNo", Detail.ToItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ToUOM", Detail.ToUOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ToQuantity", Detail.ToQuantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@IsWastage", Detail.IsWastage);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@IssuePrice", Detail.IssuePrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ReceivePrice", Detail.ReceivePrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransferDate", OrdinaryVATDesktop.DateToDate(Detail.TransferDate));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", Detail.Post);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.TransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PackSize", Detail.PackSize);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FromUnitPrice", Detail.FromUnitPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ToUnitPrice", Detail.ToUnitPrice);

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgSaveNotSuccessfully);
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
                retResults[1] = "Requested insert into ProductTransfersDetails successfully Added";
                //retResults[2] = Detail.ProductTransferId;
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

                throw new ArgumentNullException("", ex.Message.ToString());
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

        public string[] TransferIssueUpdateToMaster(ProductTransfersVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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



                #region Update

                sqlText = "";
                sqlText += " update ProductTransfers set  ";

                sqlText += " TransferDate         =@TransferDate";
                sqlText += " ,BranchId            =@BranchId";
                sqlText += " ,IsWastage           =@IsWastage";
                sqlText += " ,Post                =@Post";
                sqlText += " ,Comments            =@Comments";
                sqlText += " ,ActiveStatus        =@ActiveStatus";
                sqlText += " ,TransactionType        =@TransactionType";
                sqlText += " ,CreatedBy           =@CreatedBy";
                sqlText += " ,CreatedOn           =@CreatedOn";
                sqlText += " ,LastModifiedBy      =@LastModifiedBy";
                sqlText += " ,LastModifiedOn      =@LastModifiedOn";
                sqlText += " where              TransferCode=@TransferCode";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferCode", Master.TransferCode);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferDate", OrdinaryVATDesktop.DateToDate(Master.TransferDate));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsWastage", Master.IsWastage);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ActiveStatus", Master.ActiveStatus);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.GetTransactionType());



                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
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
                retResults[1] = "Requested update into ProductTransfers successfully updated ";
                retResults[2] = "" + Master.TransferCode;
                retResults[3] = "" + PostStatus;
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

                throw new ArgumentNullException("", ex.Message.ToString());
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

        //        #region Search Methods

        public DataTable SearchTransfer(ProductTransfersVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchProductTransfer");
            #endregion
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
                #region SQL Statement
                sqlText = " ";
                sqlText = @" 
                   SELECT 
                   [Id]
                   ,[TransferCode]
                   ,[TransferDate]
                   ,[BranchId]
                   ,ISNULL([IsWastage],'Y')[IsWastage]
                   ,ISNULL([Post],'N') [Post]
                   ,ISNULL([Comments],'-') [Comments]
                 From [dbo].[ProductTransfers]
                            WHERE 1=1
                            AND ISNULL([ActiveStatus],'Y')='Y'
                            
                            
                            ";
                if (!string.IsNullOrWhiteSpace(vm.TransferCode))
                {
                    sqlText = sqlText + @" AND TransferCode= @TransferCode ";
                }
                if (!string.IsNullOrWhiteSpace(vm.TransactionType))
                {
                    sqlText = sqlText + @" AND TransactionType= @TransactionType ";
                }
                if (!string.IsNullOrWhiteSpace(vm.Post))
                {
                    sqlText = sqlText + @" AND Post= @Post ";
                }
                if (!string.IsNullOrWhiteSpace(vm.TransferDateFrom))
                {
                    sqlText = sqlText + @" AND TransferDate>= @DateTimeFrom ";
                }
                if (!string.IsNullOrWhiteSpace(vm.TransferDateTo))
                {
                    sqlText = sqlText + @" AND TransferDate <dateadd(d,1, @DateTimeTo)";
                }

                if (vm.BranchId > 0)
                {
                    sqlText = sqlText + @" AND BranchId=@BranchId";
                }




                #endregion
                #region SQL Command
                SqlCommand objCommHeader = new SqlCommand();
                objCommHeader.Connection = currConn;
                objCommHeader.Transaction = transaction;
                objCommHeader.CommandText = sqlText;
                objCommHeader.CommandType = CommandType.Text;
                #endregion
                #region Parameter

                if (!string.IsNullOrWhiteSpace(vm.TransferCode))
                {
                    objCommHeader.Parameters.AddWithValue("@TransferCode", vm.TransferCode);

                }
                if (!string.IsNullOrWhiteSpace(vm.TransactionType))
                {
                    objCommHeader.Parameters.AddWithValue("@TransactionType", vm.TransactionType);

                }
                if (!string.IsNullOrWhiteSpace(vm.Post))
                {
                    objCommHeader.Parameters.AddWithValue("@Post", vm.Post);
                }

                if (!string.IsNullOrWhiteSpace(vm.TransferDateTo))
                {
                    objCommHeader.Parameters.AddWithValue("@DateTimeTo", vm.TransferDateTo);
                }

                if (!string.IsNullOrWhiteSpace(vm.TransferDateFrom))
                {
                    objCommHeader.Parameters.AddWithValue("@DateTimeFrom", vm.TransferDateFrom);
                }

                if (vm.BranchId > 0)
                {
                    objCommHeader.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }




                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommHeader);
                dataAdapter.Fill(dataTable);

                #region Commit


                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }
            #endregion
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", ex.Message.ToString());
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
            return dataTable;
        }

        public DataTable SearchTransferDetail(string ProductTransferId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferDetail");
            #endregion

            #region Try

            try
            {
                string VAT6_5OrderByName = new CommonDAL().settingsDesktop("Reports", "VAT6_5OrderByProductName");

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

                #region SQL Statement
                sqlText = @"
SELECT 
       Pd.[ProductTransferId]
      ,Pd.[BranchId]
      ,Pd.[FromItemNo]
	  ,F.[ProductName] [FromItemName]
      ,Pd.[FromUOM]
      ,Pd.[FromQuantity]
      ,Pd.[FromUOMConversion]
      ,Pd.[ToItemNo]
      ,T.[ProductName] [ToItemName]
      ,Pd.[ToUOM]
      ,Pd.[ToQuantity]
      ,Pd.[IsWastage]
      ,Pd.[TransferDate]
      ,Pd.[Post]
      ,Pd.[PackSize]
      ,Pd.[FromUnitPrice]
      ,Pd.[ToUnitPrice]
      ,Pd.[IssuePrice]
      ,Pd.[ReceivePrice]
    from ProductTransfersDetails Pd
 left outer join Products F  on Pd.FromItemNo=F.ItemNo
 left outer join Products T  on Pd.ToItemNo=T.ItemNo

                            WHERE 1=1
                            and ( Pd.ProductTransferId = @ProductTransferId ) 
                            ";


                #endregion

                #region SQL Command
                SqlCommand objCommTransferIssueDetail = new SqlCommand();
                objCommTransferIssueDetail.Connection = currConn;
                objCommTransferIssueDetail.CommandText = sqlText;
                objCommTransferIssueDetail.Transaction = transaction;
                objCommTransferIssueDetail.CommandType = CommandType.Text;
                #endregion

                #region Parameter
                if (!objCommTransferIssueDetail.Parameters.Contains("@ProductTransferId"))
                { objCommTransferIssueDetail.Parameters.AddWithValue("@ProductTransferId", ProductTransferId); }
                else { objCommTransferIssueDetail.Parameters["@ProductTransferId"].Value = ProductTransferId; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransferIssueDetail);
                dataAdapter.Fill(dataTable);

                #region Commit


                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();

                    }
                }

                #endregion Commit
            }
            #endregion
            #region Catch and Finall

            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", ex.Message.ToString());
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
            return dataTable;
        }

        public List<ProductTransfersDetailVM> SelectDetail(string TransferIssueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductTransfersDetailVM> VMs = new List<ProductTransfersDetailVM>();
            ProductTransfersDetailVM vm;
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
                DataTable dt = new DataTable();
                dt = SelectDetail_DT(TransferIssueNo, conditionFields, conditionValues, currConn, transaction, connVM);

                string JSONString = JsonConvert.SerializeObject(dt);
                VMs = new List<ProductTransfersDetailVM>();

                VMs = JsonConvert.DeserializeObject<List<ProductTransfersDetailVM>>(JSONString);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
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
            return VMs;
        }

        public DataTable SelectDetail_DT(string ProductTransferId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
       Pd.[ProductTransferId]
      ,Pd.[BranchId]
      ,Pd.[FromItemNo]
	  ,F.[ProductName] [FromItemName]
      ,Pd.[FromUOM]
      ,Pd.[FromQuantity]
      ,Pd.[FromUOMConversion]
      ,Pd.[ToItemNo]
      ,T.[ProductName] [ToItemName]
      ,Pd.[ToUOM]
      ,Pd.[ToQuantity]
      ,Pd.[IsWastage]
      ,Pd.[TransferDate]
      ,Pd.[Post]
    from ProductTransfersDetails Pd
 left outer join Products F  on Pd.FromItemNo=F.ItemNo
 left outer join Products T  on Pd.ToItemNo=T.ItemNo

                            WHERE 1=1
";
                if (ProductTransferId != null)
                {
                    sqlText += "AND Pd.ProductTransferId=@ProductTransferId";
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

                if (ProductTransferId != null)
                {
                    objComm.Parameters.AddWithValue("@ProductTransferId", ProductTransferId);
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

                SqlDataAdapter da = new SqlDataAdapter(objComm);
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
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
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

        public List<ProductTransfersVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductTransfersVM> VMs = new List<ProductTransfersVM>();
            ProductTransfersVM vm;
            #endregion
            #region try

            try
            {

                #region sql statement

                #region SqlExecution


                int index = -1;
                if (conditionFields != null && conditionValues != null)
                {
                    index = Array.IndexOf(conditionFields, "SelectTop");

                }

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null, false, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (index >= 0)
                    {
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    }


                    foreach (DataRow dr in dt.Rows)
                    {
                        vm = new ProductTransfersVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.TransferCode = dr["TransferCode"].ToString();
                        vm.TransferDate = OrdinaryVATDesktop.DateTimeToDate(dr["TransferDate"].ToString());
                        vm.Post = dr["Post"].ToString();
                        string type = dr["IsWastage"].ToString();
                        if (type == "Y")
                        {
                            vm.IsWastage = "Wastage";
                            vm.TransactionType = "WastageCTC";
                        }
                        if (type == "R")
                        {
                            vm.IsWastage = "Raw";
                            vm.TransactionType = "RawCTC";
                        }
                        if (type == "F")
                        {
                            vm.IsWastage = "Finish";
                            vm.TransactionType = "FinishCTC";
                        }
                        //vm.IsWastage = dr["IsWastage"].ToString() == "Y" ? "Wastage" : "Raw";
                        vm.Comments = dr["Comments"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());

                        VMs.Add(vm);
                    }

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

                FileLogger.Log("ProductTransferDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("ProductTransferDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM paramVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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

                #region Select Top

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

                #endregion

                #region sql statement
                #region SqlText
                #region Sql Text

                if (count.ToLower() == "All".ToLower())
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"

 
                   [Id]
                   ,[TransferCode]
                   ,[TransferDate]
                   ,[BranchId]
                   ,ISNULL([IsWastage],'Y')[IsWastage]
                   ,ISNULL([Post],'N') [Post]
                   ,ISNULL([Comments],'-') [Comments]
                 From [dbo].[ProductTransfers]
                 WHERE  1=1
";

                #endregion SqlText

                sqlTextCount += @" 
select count(TransferCode)RecordCount
FROM ProductTransfers  
WHERE  1=1
";

                if (Id > 0)
                {
                    sqlTextParameter += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
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
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("ProductTransferDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("ProductTransferDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<ProductTransfersDetailVM> SelectTransferDetail(string ProductTransferId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool multipleIssue = false)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductTransfersDetailVM> VMs = new List<ProductTransfersDetailVM>();
            ProductTransfersDetailVM vm;
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

                sqlText = @"
SELECT 
       Pd.[ProductTransferId]
      ,Pd.[BranchId]
      ,Pd.[FromItemNo]
	  ,F.[ProductName] [FromItemName]
      ,Pd.[FromUOM]
      ,Pd.[FromQuantity]
      ,Pd.[FromUOMConversion]
      ,Pd.[ToItemNo]
      ,T.[ProductName] [ToItemName]
      ,Pd.[ToUOM]
      ,Pd.[ToQuantity]
      ,Pd.[IsWastage]
      ,Pd.[TransferDate]
      ,Pd.[Post]
    from ProductTransfersDetails Pd
 left outer join Products F  on Pd.FromItemNo=F.ItemNo
 left outer join Products T  on Pd.ToItemNo=T.ItemNo
 WHERE 1=1

";
                if (ProductTransferId != null)
                {
                    sqlText += "AND Pd.ProductTransferId=@ProductTransferId";
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

                if (ProductTransferId != null)
                {
                    objComm.Parameters.AddWithValue("@ProductTransferId", ProductTransferId);
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
                    vm = new ProductTransfersDetailVM();

                    vm.ProductTransferId = Convert.ToInt32(dr["ProductTransferId"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.FromItemNo = dr["FromItemNo"].ToString();
                    vm.FromUOM = dr["FromUOM"].ToString();
                    vm.FromItemName = dr["FromItemName"].ToString();
                    vm.FromQuantity = Convert.ToDecimal(dr["FromQuantity"].ToString());
                    vm.FromUOMConversion = Convert.ToDecimal(dr["FromUOMConversion"].ToString());
                    vm.ToItemNo = dr["ToItemNo"].ToString();
                    vm.ToItemName = dr["ToItemName"].ToString();
                    vm.ToUOM = dr["ToUOM"].ToString();
                    vm.ToQuantity = Convert.ToDecimal(dr["ToQuantity"].ToString());
                    vm.IsWastage = dr["IsWastage"].ToString() == "Y" ? "Wastage" : "Raw";
                    vm.TransferDate = dr["TransferDate"].ToString();
                    vm.Post = dr["Post"].ToString();

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
            #endregion

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("ProductTransferDAL", "SelectIssueDetail", ex.ToString() + "\n" + sqlText);
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
            return VMs;
        }
        //        #endregion


    }
}
