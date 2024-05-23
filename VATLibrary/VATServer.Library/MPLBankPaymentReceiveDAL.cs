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
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using VATServer.Interface;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VATServer.Library.Integration;

namespace VATServer.Library
{
    public class MPLBankPaymentReceiveDAL : IMPLBankPaymentReceive
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();

        #endregion


        public string[] InsertToMPLBankPaymentReceive(MPLBankPaymentReceiveVM MasterVM, SqlTransaction Vtransaction = null,
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

                MasterVM.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);


                #region Find Transaction Exist

                int transferIssueId = MasterVM.Id;
                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(Code) from MPLBankPaymentReceives WHERE Code=@Code ";
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

               

                #region Other


                newIDCreate = commonDal.TransactionCode("Deposit", "BankPaymentReceive", "MPLBankPaymentReceives",
                        "Code",
                        "InstrumentDate", MasterVM.TransactionDateTime, MasterVM.BranchId.ToString(), currConn,
                        transaction);



                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(Code) FROM MPLBankPaymentReceives 
where Code = @Code  and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@Code", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("MPLBankPaymentReceives", "Insert",
                        "Code " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText =
                    " select count(Code) from MPLBankPaymentReceives where Code = @Code";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Code", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("MPLBankPaymentReceives", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.Code = newIDCreate;

                #endregion BankPaymentReceives ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                retResults = MPLBankPaymentReceiveInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.BankPaymentReceiveDetailsVMs != null)
                {
                    if (MasterVM.BankPaymentReceiveDetailsVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert,
                            MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (MPLBankPaymentReceiveDetailsVM collection in MasterVM.BankPaymentReceiveDetailsVMs)
                    {
                        collection.BankPaymentReceiveId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        collection.CustomerId = MasterVM.CustomerId;
                        collection.InstrumentDate = OrdinaryVATDesktop.DateTimeToDate(collection.InstrumentDate);
                        if (!string.IsNullOrEmpty(collection.InstrumentDate))
                        { collection.InstrumentDate = Convert.ToDateTime(collection.InstrumentDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }

                        retResults = MPLBankPaymentReceiveInsertToDetails(collection, currConn, transaction, null, settings);
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
                retResults[2] = "" + MasterVM.Id.ToString();
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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "InsertToMPLBankPaymentReceive", ex.ToString() + "\n" + sqlText);

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

        public string[] MPLBankPaymentReceiveInsertToMaster(MPLBankPaymentReceiveVM Master, SqlConnection VcurrConn = null,
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
                sqlText += " INSERT INTO MPLBankPaymentReceives";
                sqlText += " (";
                sqlText += "  Code";
                sqlText += "  ,BranchId";
                sqlText += "  ,CustomerId";
                sqlText += "  ,TransactionDateTime";
                sqlText += "  ,Post";
               

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "    @Code";
                sqlText += "   ,@BranchId";
                sqlText += "   ,@CustomerId";
                sqlText += "   ,@TransactionDateTime";
                sqlText += "   ,@Post";
              


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);


                cmdInsert.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerId", Master.CustomerId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", "N");
           

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
               
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.insert,
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

           
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + Master.Code;
                retResults[3] = "N";
                retResults[4] =transResult.ToString();
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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "MPLBankPaymentReceiveInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] UpdateMPLBankPaymentReceive(MPLBankPaymentReceiveVM MasterVM, SqlTransaction transaction,
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

                MasterVM.InstrumentDate = OrdinaryVATDesktop.DateToDate(MasterVM.InstrumentDate);

                retResults = UpdateMPLBankPaymentReceiveToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }



                #endregion Update Information in Header


                #region Delete Existing Detail Data

                sqlText = "";
                sqlText +=
                    @" DELETE FROM MPLBankPaymentReceiveDetails WHERE BankPaymentReceiveId=@BankPaymentReceiveId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@BankPaymentReceiveId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.BankPaymentReceiveDetailsVMs != null)
                {
                    if (MasterVM.BankPaymentReceiveDetailsVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert,
                            MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (MPLBankPaymentReceiveDetailsVM collection in MasterVM.BankPaymentReceiveDetailsVMs)
                    {
                        collection.BankPaymentReceiveId = MasterVM.Id;
                        collection.BranchId = MasterVM.BranchId;
                        collection.CustomerId = MasterVM.CustomerId;
                        collection.InstrumentDate = OrdinaryVATDesktop.DateTimeToDate(collection.InstrumentDate);
                        if (!string.IsNullOrEmpty(collection.InstrumentDate))
                        { collection.InstrumentDate = Convert.ToDateTime(collection.InstrumentDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }

                        retResults = MPLBankPaymentReceiveInsertToDetails(collection, currConn, transaction, null, settings);
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
                retResults[2] = "" + MasterVM.Id.ToString();
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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "UpdateMPLBankPaymentReceive", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateMPLBankPaymentReceiveToMaster(MPLBankPaymentReceiveVM Master, SqlConnection VcurrConn = null,
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
                sqlText += " update MPLBankPaymentReceives SET  ";

                sqlText += " BranchId=@BranchId";
                sqlText += " ,CustomerId=@CustomerId";
                sqlText += " ,TransactionDateTime=@TransactionDateTime";
                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerId", Master.CustomerId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgUpdateNotSuccessfully,
                        MessageVM.PurchasemsgUpdateNotSuccessfully);
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
                retResults[2] = "" + Master.Id.ToString();
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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "UpdateMPLBankPaymentReceiveToMaster",
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

        public string[] MPLBankPaymentReceiveInsertToDetails(MPLBankPaymentReceiveDetailsVM details,
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
                sqlText += " INSERT INTO MPLBankPaymentReceiveDetails";
                sqlText += " (";
                sqlText += "BankPaymentReceiveId ";
                sqlText += ",BranchId ";
                sqlText += ",BDBankId ";
                sqlText += ",CustomerId ";
                sqlText += ",ModeOfPayment ";
                sqlText += ",InstrumentNo ";
                sqlText += ",InstrumentDate ";
                sqlText += ",Amount ";
                sqlText += ",IsUsed ";
                sqlText += ",IsUsedDS ";
                sqlText += ",Post ";



                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @BankPaymentReceiveId";
                sqlText += " ,@BranchId";
                sqlText += " ,@BDBankId";
                sqlText += " ,@CustomerId";
                sqlText += " ,@ModeOfPayment";
                sqlText += " ,@InstrumentNo";
                sqlText += " ,@InstrumentDate";
                sqlText += " ,@Amount";
                sqlText += " ,@IsUsed";
                sqlText += " ,@IsUsedDS";
                sqlText += " ,@Post";



                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankPaymentReceiveId", details.BankPaymentReceiveId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BDBankId", details.BDBankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerId", details.CustomerId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ModeOfPayment", details.ModeOfPayment);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentNo", details.InstrumentNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentDate", details.InstrumentDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Amount", details.Amount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsUsed", false);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsUsedDS", false);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", "N");


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
                retResults[2] = "" + details.BankPaymentReceiveId;
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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "MPLBankPaymentReceiveInsertToDetails",
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
            string[] ids = null,string Orderby = "Y", string SelectTop = "100")
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextGroupBy = "";
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

 BPR.Id
,BPR.Code
,ISNULL(c.CustomerName, '') CustomerName
,ISNULL(c.CustomerCode, '') CustomerCode
,BPR.CustomerId
,BPR.BranchId
,BPR.TransactionDateTime
,ISNULL(BPR.Post, 'N') Post

,Sum (BPRD.Amount)Amount


FROM MPLBankPaymentReceiveDetails BPRD
LEFT OUTER JOIN MPLBankPaymentReceives BPR ON BPR.Id = BPRD.BankPaymentReceiveId

LEFT OUTER JOIN MPLBDBankInformations BDB ON BDB.BankID = BPR.BDBankId
LEFT OUTER JOIN BranchProfiles BP ON BP.BranchID = BPR.BranchId
LEFT OUTER JOIN Customers c on c.CustomerID=BPR.CustomerId 

WHERE  1=1 ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT COUNT(BPR.Id) RecordCount
FROM MPLBankPaymentReceives BPR WHERE  1=1 ";

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

                    sqlText += " AND BPR.Id IN (" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND BPR.Id=@Id";
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

                sqlTextGroupBy += @" group by BPR.Id,BPR.Code,BPR.CustomerId,BPR.BranchId,c.CustomerName,c.CustomerCode,BPR.TransactionDateTime,BPR.Post";

                if (Orderby == "N")
                {
                    sqlTextOrderBy += " order by BPR.Id desc";
                }
                else
                {
                    sqlTextOrderBy += " order by BPR.Id desc";
                }

               
                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextGroupBy + " " +sqlTextOrderBy;
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
                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<MPLBankPaymentReceiveVM> SelectAllList(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null, string[] ids = null, string Orderby = "Y",
            string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLBankPaymentReceiveVM> VMs = new List<MPLBankPaymentReceiveVM>();
            MPLBankPaymentReceiveVM vm;

            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids,
                     Orderby, SelectTop);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new MPLBankPaymentReceiveVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.Code = dr["Code"].ToString();
                        vm.CustomerId = dr["CustomerId"].ToString();
                        vm.TransactionDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransactionDateTime"].ToString());
                        vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.Post = dr["Post"].ToString();

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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }


        public DataTable SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,
           string[] ids = null, string Orderby = "Y", string SelectTop = "100")
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


BPRD.Id
,BPRD.BankPaymentReceiveId
,BPRD.BranchId
,BPRD.BDBankId
,BDB.BankName BDBankName
,BDB.BankCode BDBankCode
,ISNULL(BP.BranchName, '') BranchName
,ISNULL(c.CustomerName, '') CustomerName
,BPRD.CustomerId
,BPRD.ModeOfPayment
,BPRD.InstrumentNo
,FORMAT(BPRD.InstrumentDate,'dd-MMM-yyyy' )InstrumentDate
,BPRD.Amount
,isnull(BPRD.IsUsed,0)IsUsed
,isnull(BPRD.IsUsedDS,0)IsUsedDS
,isnull(BPRD.Post,'N')Post

FROM MPLBankPaymentReceiveDetails BPRD
LEFT OUTER JOIN MPLBDBankInformations BDB ON BDB.BankID = BPRD.BDBankId
LEFT OUTER JOIN BranchProfiles BP ON BP.BranchID = BPRD.BranchId
LEFT OUTER JOIN Customers c on c.CustomerID=BPRD.CustomerId 

WHERE  1=1 ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT COUNT(BPRD.Id) RecordCount
FROM MPLBankPaymentReceiveDetails BPRD WHERE  1=1 ";

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

                    sqlText += " AND BPRD.BankPaymentReceiveId IN (" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND BPRD.BankPaymentReceiveId=@Id";
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

                if (Orderby == "N")
                {
                    sqlTextOrderBy += " order by BPRD.Id desc";
                }
                else
                {
                    sqlTextOrderBy += " order by BPRD.Id desc";
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
                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectAllDetails", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectAllDetails", ex.ToString() + "\n" + sqlText);
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



        public List<MPLBankPaymentReceiveDetailsVM> SelectDetailsList(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y",
            string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLBankPaymentReceiveDetailsVM> VMs = new List<MPLBankPaymentReceiveDetailsVM>();
            MPLBankPaymentReceiveDetailsVM vm;

            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAllDetails(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids,
                    Orderby, SelectTop);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new MPLBankPaymentReceiveDetailsVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.BankPaymentReceiveId = Convert.ToInt32(dr["BankPaymentReceiveId"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.BDBankId = Convert.ToInt32(dr["BDBankId"].ToString());
                        vm.ModeOfPayment = dr["ModeOfPayment"].ToString();
                        vm.InstrumentNo = dr["InstrumentNo"].ToString();
                        vm.InstrumentDate = dr["InstrumentDate"].ToString();
                        vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                        vm.IsUsed = Convert.ToBoolean(dr["IsUsed"].ToString());
                        vm.IsUsedDS = Convert.ToBoolean(dr["IsUsedDS"].ToString());
                        vm.BDBankName = dr["BDBankName"].ToString();
                        vm.BDBankCode = dr["BDBankCode"].ToString();
                        vm.BranchName = dr["BranchName"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.Post = dr["Post"].ToString();
                     

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

                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectDetailsList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectDetailsList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }

        public List<MPLBankPaymentReceiveVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] Delete(MPLBankPaymentReceiveVM vm, string[] ids, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }


        public List<MPLBankPaymentReceiveVM> SelectBankPaymentReceiveList(MPLBankPaymentReceiveVM vm, string[] conditionFields = null, string[] conditionValues = null,
        SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLBankPaymentReceiveVM> lst = new List<MPLBankPaymentReceiveVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SelectBankPaymentReceiveListVMs");

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
Select 
BPR.code
,BPRD.Id
,BDB.BankID  BDBankId
,BDB.BankCode BDBankCode
,BDB.BankName BDBankName
,BPRD.ModeOfPayment
,BPRD.InstrumentNo
,FORMAT(BPRD.InstrumentDate,'dd-MMM-yyyy' )InstrumentDate
,BPRD.Amount
,isnull(BPRD.IsUsedDS,0)IsUsedDS

from MPLBankPaymentReceiveDetails BPRD
left outer join MPLBankPaymentReceives  BPR on  BPR.Id=BPRD.BankPaymentReceiveId
left outer join MPLBDBankInformations BDB on  BPRD.BDBankId=BDB.BankID
WHERE isnull(BPRD.IsUsed,0)=0
";

                if (!string.IsNullOrWhiteSpace(vm.SearchField) && !string.IsNullOrWhiteSpace(vm.SearchValue))
                {
                    sqlText += @"
                         And BPR.@SearchField=@SearchValue";

                    sqlText = sqlText.Replace("@SearchField", vm.SearchField);

                }
                if (!string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    sqlText += @"
                         And BPRD.InstrumentDate>=@DateFrom";
                }
                if (!string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    sqlText += @"
                         And BPRD.InstrumentDate<=@DateTo";
                }
                if (!string.IsNullOrWhiteSpace(vm.CustomerId))
                {
                    sqlText += @"
                         And BPR.CustomerId=@CustomerId";
                }


               


                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);


                if (!string.IsNullOrWhiteSpace(vm.SearchField) && !string.IsNullOrWhiteSpace(vm.SearchValue))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@SearchValue", vm.SearchValue.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.FromDate.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@DateTo", vm.ToDate.ToString());
                }
                if (!string.IsNullOrWhiteSpace(vm.CustomerId))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId.ToString());
                }
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLBankPaymentReceiveVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectBankPaymentReceiveList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBankPaymentReceiveDAL", "SelectBankPaymentReceiveList",
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

        public string[] MPLBankPaymentReceivePost(MPLBankPaymentReceiveVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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
                 @" UPDATE MPLBankPaymentReceives SET Post='Y'";
                sqlText += @"  WHERE Id IN (" + ids + ") ";
                sqlText +=
                    @" UPDATE MPLBankPaymentReceiveDetails SET Post='Y'  ";
                sqlText += @"  WHERE BankPaymentReceiveId IN (" + ids + ") ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);

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
                FileLogger.Log("MPLBankPaymentReceiveDAL", "MPLBankDepositSlipPost", ex.ToString() + "\n" + sqlText);

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

    }
}
