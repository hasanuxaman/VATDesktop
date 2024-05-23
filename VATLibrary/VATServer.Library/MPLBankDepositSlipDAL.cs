using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using SymphonySofttech.Utilities;
using VATServer.Ordinary;
using System.IO;
using Excel;
using VATViewModel.DTOs;
using System.Data.OracleClient;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using VATServer.Interface;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VATServer.Library.Integration;

namespace VATServer.Library
{
    public class MPLBankDepositSlipDAL : IMPLBankDepositSlip
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();

        #endregion


        public string[] InsertToMPLBankDepositSlip(MPLBankDepositSlipHeaderVM MasterVM, SqlTransaction Vtransaction = null,
            SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

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
            string newIDCreate = "";
            string PostStatus = "";
            int IDExist = 0;

            #endregion Initializ

            #region Try

            try
            {

                #region Validation for Header


                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransactionDateTime) < DateTime.MinValue ||
                         Convert.ToDateTime(MasterVM.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        "Please Check Transaction Data and Time");

                }

                #endregion Validation for Header

                CommonDAL commonDal = new CommonDAL();

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(null, null);
                }

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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction


                #region Find Transaction Exist

                int transferIssueId = MasterVM.Id;
                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(Code) from MPLBankDepositSlipHeaders WHERE Code=@Code ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@Code",
                    MasterVM.Code);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Transaction ID Create

                if (string.IsNullOrEmpty(MasterVM.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        MessageVM.msgTransactionNotDeclared);
                }

                #region Other

                if (MasterVM.TransactionType=="SA-1")
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "BankDepositSA1", "MPLBankDepositSlipHeaders",
                        "Code",
                        "TransactionDateTime", MasterVM.TransactionDateTime, MasterVM.BranchId.ToString(), currConn,
                        transaction);
                }
                else
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "BankDepositSA2", "MPLBankDepositSlipHeaders",
                       "Code",
                       "TransactionDateTime", MasterVM.TransactionDateTime, MasterVM.BranchId.ToString(), currConn,
                       transaction);
                }
                

               

                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(Code) FROM MPLBankDepositSlipHeaders 
where Code = @Code and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@Code", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("MPLBankDepositSlipDAL", "Insert",
                        "Code " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText =
                    " select count(Code) from MPLBankDepositSlipHeaders where Code = @Code";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Code", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("MPLBankDepositSlipDAL", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.Code = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                retResults = MPLBankDepositSlipInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Insert into Details(Insert complete in Header)

                if (MasterVM.MPLBankDepositSlipDetailVMs != null)
                {
                    if (MasterVM.MPLBankDepositSlipDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert,
                            MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (MPLBankDepositSlipDetailVM collection in MasterVM.MPLBankDepositSlipDetailVMs)
                    {
                        collection.BankDepositSlipHeaderId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        retResults = MPLBankDepositSlipInsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion Insert into Details(Insert complete in Header)

                
                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + MasterVM.Code;
                retResults[3] = "N";
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

                FileLogger.Log("MPLBankDepositSlipDAL", "InsertToMPLBankDepositSlip", ex.ToString() + "\n" + sqlText);

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

        public string[] MPLBankDepositSlipInsertToMaster(MPLBankDepositSlipHeaderVM Master, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
        {
            #region Initializ

            string[] retResults = new string[5];
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

                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO MPLBankDepositSlipHeaders";
                sqlText += " (";
                sqlText += "  Code";
                sqlText += "  ,BranchId";
                sqlText += "  ,SelfBankId";
                sqlText += "  ,TransactionDateTime";
                sqlText += "  ,TransactionType";
                sqlText += "  ,BankSlipType";
                sqlText += "  ,Comments";
                sqlText += "  ,CreatedBy";
                sqlText += "  ,CreatedOn";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "   @Code";
                sqlText += "   ,@BranchId";
                sqlText += "   ,@SelfBankId";
                sqlText += "   ,@TransactionDateTime";
                sqlText += "   ,@TransactionType";
                sqlText += "   ,@BankSlipType";
                sqlText += "   ,@Comments";
                sqlText += "   ,@CreatedBy";
                sqlText += "   ,@CreatedOn";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);


                cmdInsert.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SelfBankId", Master.SelfBankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankSlipType", Master.BankSlipType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        MessageVM.saleMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Commit

                if (Vtransaction == null)
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
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + Master.Code;
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

                FileLogger.Log("MPLBankDepositSlipDAL", "MPLBankDepositSlipInsertToMaster", ex.ToString() + "\n" + sqlText);
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
        
        public string[] UpdateMPLBankDepositSlip(MPLBankDepositSlipHeaderVM MasterVM, SqlTransaction transaction,
            SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";


            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion Initializ

            #region Try

            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransactionDateTime) < DateTime.MinValue ||
                         Convert.ToDateTime(MasterVM.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        "Please Check Invoice Data and Time");

                }

                #endregion Validation for Header

                CommonDAL commonDal = new CommonDAL();

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(null, null);
                }

                #region open connection and transaction

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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction


                #region Update Information in Header

                MasterVM.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);

                retResults = UpdateMPLBankDepositSlipToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText +=
                    @" DELETE FROM MPLBankDepositSlipDetails WHERE BankDepositSlipHeaderId=@BankDepositSlipHeaderId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@BankDepositSlipHeaderId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.MPLBankDepositSlipDetailVMs != null)
                {
                    if (MasterVM.MPLBankDepositSlipDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert,
                            MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (MPLBankDepositSlipDetailVM collection in MasterVM.MPLBankDepositSlipDetailVMs)
                    {
                        collection.BankDepositSlipHeaderId = MasterVM.Id;
                        collection.BranchId = MasterVM.BranchId;
                        collection.InstrumentDate = OrdinaryVATDesktop.DateTimeToDate(collection.InstrumentDate);
                        if (!string.IsNullOrEmpty(collection.InstrumentDate))
                        { collection.InstrumentDate = Convert.ToDateTime(collection.InstrumentDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }

                        retResults = MPLBankDepositSlipInsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Insert into Details(Insert complete in Header)

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + MasterVM.Code;
                retResults[3] = "N";
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

                FileLogger.Log("MPLBankDepositSlipDAL", "TransReceiveMPLUpdate", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result
        }

        public string[] UpdateMPLBankDepositSlipToMaster(MPLBankDepositSlipHeaderVM Master, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " update MPLBankDepositSlipHeaders SET  ";

                sqlText += " Code=@Code";
                sqlText += " ,BranchId=@BranchId";
                sqlText += " ,SelfBankId=@SelfBankId";
                sqlText += " ,TransactionDateTime=@TransactionDateTime";
                sqlText += " ,TransactionType=@TransactionType";
                sqlText += " ,BankSlipType=@BankSlipType";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";


                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SelfBankId", Master.SelfBankId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BankSlipType", Master.BankSlipType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgUpdateNotSuccessfully,
                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                }
                else
                {
                    #region Update Transfer Issue Data
                    sqlText = "";

                    sqlText += @"  update SalesInvoiceMPLBankPayments set IsUsedDS=0 where id in(SELECT RefId FROM MPLBankDepositSlipDetails where TType='P' and  BankDepositSlipHeaderId=@Id)
                            update MPLBankPaymentReceives set IsUsedDS=0 where id in(SELECT RefId FROM MPLBankDepositSlipDetails where TType='R' and  BankDepositSlipHeaderId=@Id)";
                    

                    SqlCommand cmdupdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdupdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                    transResult = cmdupdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                            MessageVM.saleMsgSaveNotSuccessfully);
                    }

                    #endregion
                }
                #endregion ID generated completed,Insert new Information in Header

                #region Commit


                if (Vtransaction == null)
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
                retResults[2] = "" + Master.Id;
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

                FileLogger.Log("MPLBankDepositSlipDAL", "UpdateMPLBankDepositSlipToMaster",
                    ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());
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

        public string[] MPLBankDepositSlipInsertToDetails(MPLBankDepositSlipDetailVM details,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,
            DataTable settingsDt = null)
        {
            #region Initializ

            string[] retResults = new string[5];
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

                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO MPLBankDepositSlipDetails";
                sqlText += " (";
                sqlText += "BankDepositSlipHeaderId ";
                sqlText += ",BranchId ";
                sqlText += ",BDBankId ";
                sqlText += ",SalesInvoiceMPLHeaderId ";
                sqlText += ",MPLBankPaymentReceivesId ";
                sqlText += ",ModeOfPayment ";
                sqlText += ",InstrumentNo ";
                sqlText += ",InstrumentDate ";
                sqlText += ",Amount ";
                sqlText += ",TType ";
                sqlText += ",RefId ";



                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @BankDepositSlipHeaderId";
                sqlText += " ,@BranchId";
                sqlText += " ,@BDBankId";
                sqlText += " ,@SalesInvoiceMPLHeaderId";
                sqlText += " ,@MPLBankPaymentReceivesId";
                sqlText += " ,@ModeOfPayment";
                sqlText += " ,@InstrumentNo";
                sqlText += " ,@InstrumentDate";
                sqlText += " ,@Amount";
                sqlText += " ,@TType";
                sqlText += " ,@RefId";



                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankDepositSlipHeaderId", details.BankDepositSlipHeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BDBankId", details.BDBankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceMPLHeaderId", details.SalesInvoiceMPLHeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MPLBankPaymentReceivesId", details.MPLBankPaymentReceivesId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ModeOfPayment", details.ModeOfPayment);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentNo", details.InstrumentNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentDate", details.InstrumentDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Amount", details.Amount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TType", details.TType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RefId", details.RefId);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        MessageVM.saleMsgSaveNotSuccessfully);
                }
                else
                {
                    #region Update  Data
                    sqlText = "";
                    if (details.TType.ToLower()=="p")
                    {
                        sqlText += @"  update SalesInvoiceMPLBankPayments set IsUsedDS=1 where id=@Id ";
                    }
                    else
                    {
                        sqlText += @"  update MPLBankPaymentReceiveDetails set IsUsedDS=1 where id=@Id ";
                    }

                    SqlCommand cmdupdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdupdate.Parameters.AddWithValueAndNullHandle("@Id", details.RefId);

                    transResult = cmdupdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                            MessageVM.saleMsgSaveNotSuccessfully);
                    }

                    #endregion
                }
                #endregion ID generated completed,Insert new Information in Header

                #region Commit

                if (Vtransaction == null)
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
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + details.BankDepositSlipHeaderId;
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

                FileLogger.Log("MPLBankDepositSlipDAL", "TransReceiveMPLInsertToDetails",
                    ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,
            string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string count = SelectTop;

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

 
DSH.Id
,DSH.Code
,DSH.BranchId
,DSH.SelfBankId
,SDB.BankName SelfBankName
,ISNULL(BP.BranchName, '') BranchName
,DSH.TransactionDateTime
,DSH.TransactionType
,ISNULL(SUM(DSHD.Amount),0) TotalAmount
,DSH.BankSlipType
,DSH.Comments
,DSH.CreatedBy
,DSH.CreatedOn
,DSH.LastModifiedBy
,DSH.LastModifiedOn
,Isnull(DSH.Post,'N')Post

FROM MPLBankDepositSlipHeaders DSH
LEFT OUTER JOIN MPLBankDepositSlipDetails DSHD ON DSHD.BankDepositSlipHeaderId = DSH.Id
LEFT OUTER JOIN MPLSelfBankInformations SDB ON SDB.BankID = DSH.SelfBankId
LEFT OUTER JOIN BranchProfiles BP ON BP.BranchID = DSH.BranchId
WHERE  1=1 ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT COUNT(DSH.Id) RecordCount
FROM MPLBankDepositSlipHeaders DSH WHERE  1=1 ";

                if (ids != null && ids.Length > 0)
                {
                    int len = ids.Count();
                    string sqlText2 = "";

                    for (int i = 0; i < len; i++)
                    {
                        sqlText2 += "'" + ids[i] + "'";

                        if (i != (len - 1))
                        {
                            sqlText2 += ",";
                        }
                    }

                    if (len == 0)
                    {
                        sqlText2 += "''";
                    }

                    sqlText += " AND DSH.TransferReceiveNo IN(" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND DSH.Id=@Id";
                }

                if (!String.IsNullOrEmpty(transactionType))
                {
                    sqlTextParameter += @" AND DSH.TransactionType IN (@transactionType)";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]) ||
                            conditionValues[i] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " '%'+ " + " @" +
                                                cField.Replace("like", "").Trim() + " +'%'";
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

                sqlTextOrderBy += "  GROUP BY  DSH.Id,DSH.Code,DSH.BranchId,DSH.SelfBankId,SDB.BankName,BP.BranchName,DSH.TransactionDateTime,DSH.TransactionType,DSH.Comments,DSH.BankSlipType," +
                  "DSH.CreatedBy,DSH.CreatedOn,DSH.LastModifiedBy,DSH.LastModifiedOn,DSH.Post ";

                if (Orderby == "N")
                {
                    sqlTextOrderBy += " order by DSH.Id desc";
                }
                else
                {
                    sqlTextOrderBy += " order by DSH.Id desc";
                }

                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]) ||
                            conditionValues[j] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(),
                                conditionValues[j]);
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

                if (!String.IsNullOrEmpty(transactionType))
                {
                    da.SelectCommand.Parameters.AddWithValue("@transactionType", transactionType);
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
                FileLogger.Log("MPLBankDepositSlipDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<MPLBankDepositSlipHeaderVM> SelectAllList(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y",
            string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLBankDepositSlipHeaderVM> VMs = new List<MPLBankDepositSlipHeaderVM>();
            MPLBankDepositSlipHeaderVM vm;

            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids,
                    transactionType, Orderby, SelectTop);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new MPLBankDepositSlipHeaderVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.Code = dr["Code"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.SelfBankId = Convert.ToInt32(dr["SelfBankId"].ToString());
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                        vm.TransactionDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransactionDateTime"].ToString());
                        vm.SelfBankName = dr["SelfBankName"].ToString();
                        vm.BranchName = dr["BranchName"].ToString();
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.BankSlipType = dr["BankSlipType"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {
                        //
                    }
                }

                #endregion SqlExecution

                #endregion
            }

            #endregion

            #region catch

            catch (SqlException sqlex)
            {

                FileLogger.Log("MPLBankDepositSlipDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLBankDepositSlipDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }


        public List<MPLBankDepositSlipDetailVM> SearchMPLBankDepositSlipDetailList(string bankDepositSlipHeaderId,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLBankDepositSlipDetailVM> lst = new List<MPLBankDepositSlipDetailVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("MPLBankDepositSlipDetailVMs");

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
 DSD.Id
,DSD.BankDepositSlipHeaderId
,ISNULL(SI.SalesInvoiceNo,'') SalesInvoiceNo
,DSD.BranchId
,DSD.BDBankId
,BDB.BankName BDBankName
,BDB.BankCode
,DSD.SalesInvoiceMPLHeaderId
,DSD.MPLBankPaymentReceivesId
,DSD.ModeOfPayment
,DSD.InstrumentNo
,ISNULL(FORMAT(DSD.InstrumentDate,'dd-MMM-yyyy'),'') InstrumentDate
,DSD.Amount
,TType,RefId
,c.CustomerName

FROM MPLBankDepositSlipDetails DSD
LEFT OUTER join SalesInvoiceMPLBankPayments p on DSD.RefId=p.Id and DSD.TType='P'
LEFT OUTER JOIN SalesInvoiceMPLHeaders SI ON   p.SalesInvoiceMPLHeaderId =SI.Id
LEFT OUTER join MPLBankPaymentReceiveDetails R on DSD.RefId=r.Id and DSD.TType='r'
LEFT OUTER JOIN MPLBDBankInformations BDB ON BDB.BankID = DSD.BDBankId 
LEFT OUTER JOIN Customers c ON c.CustomerID=(case when DSD.TType='P' then  SI.CustomerID else r.CustomerId end )

WHERE 1=1  ";

                if (!string.IsNullOrEmpty(bankDepositSlipHeaderId))
                {
                    if (Convert.ToInt32(bankDepositSlipHeaderId) > 0)
                    {
                        sqlText += @" AND DSD.BankDepositSlipHeaderId=@bankDepositSlipHeaderId";
                    }
                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                if (!string.IsNullOrEmpty(bankDepositSlipHeaderId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@bankDepositSlipHeaderId", bankDepositSlipHeaderId);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLBankDepositSlipDetailVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "SearchTMPLBankDepositSlipDetailList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "SearchTMPLBankDepositSlipDetailList",
                    ex.ToString() + "\n" + sqlText);
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

            return lst;
        }

        public List<MPLBankDepositSlipDetailVM> GetMPLBankDepositSlipDetailList(MPLBankDepositSlipHeaderVM vm, string[] conditionFields = null, string[] conditionValues = null,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLBankDepositSlipDetailVM> lst = new List<MPLBankDepositSlipDetailVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("MPLBankDepositSlipDetailVMs");

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
SELECT d.SL TType,d.Id,bb.BankCode,bb.BankID BDBankId,bb.BankName BDBankName,c.CustomerName,d.ModeOfPayment,d.InstrumentNo,d.InstrumentDate,d.Amount, 
ISNULL(SalesInvoiceMPLHeaderId,0) SalesInvoiceMPLHeaderId ,d.SalesInvoiceNo 
FROM (
SELECT 'R' SL, rd.Id,rd.BranchId,rd.BDBankId,r.CustomerID,rd.ModeOfPayment,rd.InstrumentNo,ISNULL(FORMAT(rd.InstrumentDate,'dd-MMM-yyyy'),'') InstrumentDate,rd.Amount,0 SalesInvoiceMPLHeaderId,'' SalesInvoiceNo
FROM MPLBankPaymentReceiveDetails rd
left outer join MPLBankPaymentReceives rh on rd.BankPaymentReceiveId=rh.Id
left outer join SalesInvoiceMPLHeaders r on r.Id=rd.BankPaymentReceiveId
WHERE isnull(rd.IsUsedDS,0)=0  AND rd.Post='Y' AND r.BranchId= @branch
";
                if (!string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    sqlText += @"
                         And rh.TransactionDateTime>=@DateFrom";
                }
                if (!string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    sqlText += @"
                         And rh.TransactionDateTime<=@DateTo";
                }
                if (!string.IsNullOrWhiteSpace(vm.ModeOfPayment))
                {
                    sqlText += @"
                         And rh.ModeOfPayment=@ModeOfPayment";
                }
                if (!string.IsNullOrWhiteSpace(vm.InstrumentNo))
                {
                    sqlText += @"
                         And rd.InstrumentNo=@InstrumentNo";
                }
                if (!string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    sqlText += @"
                         And r.SalesInvoiceNo=''";
                }

sqlText += @"

UNION ALL

";

sqlText += @"

SELECT 'P' SL,p.Id,p.BranchId,p.BankId,h.CustomerID,ModeOfPayment,InstrumentNo,ISNULL(FORMAT(InstrumentDate,'dd-MMM-yyyy'),'') InstrumentDate,Amount,p.SalesInvoiceMPLHeaderId,h.SalesInvoiceNo
FROM SalesInvoiceMPLBankPayments p
left outer join SalesInvoiceMPLHeaders h on p.SalesInvoiceMPLHeaderId=h.Id
WHERE isnull(p.IsUsedDS,0)=0 and  h.Post='Y' and p.Amount>0 and p.ModeOfPayment !='TR' and h.BranchId= @branch and h.ReportType=@ReportType";

                if (!string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    sqlText += @"
                         And h.InvoiceDateTime>=@DateFrom";
                }
                if (!string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    sqlText += @"
                         And h.InvoiceDateTime<=@DateTo";
                }
                if (!string.IsNullOrWhiteSpace(vm.ModeOfPayment))
                {
                    sqlText += @"
                         And p.ModeOfPayment=@ModeOfPayment";
                }
                if (!string.IsNullOrWhiteSpace(vm.InstrumentNo))
                {
                    sqlText += @"
                         And InstrumentNo=@InstrumentNo";
                }
                if (!string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    sqlText += @"
                         And h.SalesInvoiceNo=@SalesInvoiceNo";
                }
sqlText += @"

) as d
left outer join MPLBDBankInformations bb on d.BDBankId=bb.BankID
left outer join Customers c on d.CustomerId=c.CustomerID ";
                

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);


                if (!String.IsNullOrEmpty(vm.BranchId.ToString()))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@branch", vm.BranchId.ToString());
                } 

                if (!String.IsNullOrEmpty(vm.TransactionType.ToString()))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@ReportType", vm.TransactionType.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.FromDate.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@DateTo", vm.ToDate.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.InstrumentNo))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@InstrumentNo", vm.InstrumentNo.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@SalesInvoiceNo", vm.SalesInvoiceNo.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.ModeOfPayment))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@ModeOfPayment", vm.ModeOfPayment.ToString());
                }
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLBankDepositSlipDetailVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "SearchTMPLBankDepositSlipDetailList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "SearchTMPLBankDepositSlipDetailList",
                    ex.ToString() + "\n" + sqlText);
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

            return lst;
        }
        
        public string[] Delete(MPLBankDepositSlipHeaderVM vm, string[] ids, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public List<MPLBankDepositSlipDetailVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLBankDepositSlipDetailVM> VMs = new List<MPLBankDepositSlipDetailVM>();
            MPLBankDepositSlipDetailVM vm;
            #endregion
            #region try

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
                sqlText = @" Select Id,SalesInvoiceNo From SalesInvoiceMPLHeaders WHERE  1=1 AND Post = 'Y' ";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLBankDepositSlipDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLBankDepositSlipDAL", "DropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
            return VMs;
        }

        public string[] MPLBankDepositSlipPost(TransferMPLIssueVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            string ids = "";

            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }

                for (int i = 0; i < MasterVM.IDs.Count; i++)
                {
                    if (string.IsNullOrEmpty(MasterVM.IDs[i]))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(ids))
                    {
                        ids = MasterVM.IDs[i];
                    }
                    else
                    {
                        ids += "," + MasterVM.IDs[i];
                    }
                }


                #endregion Validation for Header

                #region open connection and transaction

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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction

                #region Update  Data

                sqlText = "";
                sqlText +=
                 @" UPDATE MPLBankDepositSlipHeaders SET Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=GETDATE() ";
                sqlText += @"  WHERE Id IN (" + ids + ") ";
                sqlText +=
                    @" UPDATE MPLBankDepositSlipDetails SET Post='Y'  ";
                sqlText += @"  WHERE BankDepositSlipHeaderId IN (" + ids + ") ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);

                transResult = cmdDeleteDetail.ExecuteNonQuery();
   

                #endregion
               
                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSuccessfullyPost;
                retResults[2] = "" + MasterVM.TransferIssueNo;
                retResults[3] = "N";
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
                FileLogger.Log("MPLBankDepositSlipDAL", "MPLBankDepositSlipPost", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public List<MPLBankDepositSlipHeaderVM> BankSlipTypeDropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLBankDepositSlipHeaderVM> VMs = new List<MPLBankDepositSlipHeaderVM>();
            MPLBankDepositSlipHeaderVM vm;
            #endregion
            #region try

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
                sqlText = @" Select Id,Code From EnumBankSlipType WHERE  1=1 AND IsActive = 'Y' ";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLBankDepositSlipHeaderVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Code = dr["Code"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLBankDepositSlipDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLBankDepositSlipDAL", "DropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
            return VMs;
        }

    }
}
