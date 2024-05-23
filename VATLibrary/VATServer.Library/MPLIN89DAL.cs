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
    public class MPLIN89DAL : IMPLIN89
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

        #endregion


        public string[] MPLIN89Insert(MPLIN89VM MasterVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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
            MPLIN89VM IN89VM = new MPLIN89VM();
            List<MPLIN89DetailsVM> details = new List<MPLIN89DetailsVM>();
            List<MPLIN89IssueDetailsVM> issueDetails = new List<MPLIN89IssueDetailsVM>();

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.insert, "Please Check Transaction Date and Time");
                }

                #endregion Validation for Header

                #region commonDal

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

                #endregion commonDal

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

                sqlText = "";
                sqlText = sqlText + " SELECT COUNT(Id) FROM MPLIN89Headers WHERE Code=@Code ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@Code", MasterVM.Code);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindExistSame + ". " + MasterVM.Code);
                }

                #endregion Find Transaction Exist

                #region IN89 ID Create

                if (string.IsNullOrEmpty(MasterVM.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgTransactionNotDeclared);
                }

                #region Other

                if (MasterVM.TransactionType == "Other")
                {
                    newIDCreate = commonDal.TransactionCode("Inventory", "INE", "MPLIN89Headers", "Code",
                                              "TransactionDateTime", MasterVM.TransactionDateTime, MasterVM.BranchId.ToString(), currConn, transaction);
                }

                #endregion

                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,"ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
                SELECT COUNT(Id) FROM MPLIN89Headers WHERE Code = @Code and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@Code", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("MPLIN89DAL", "Insert", "Code " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText = " SELECT COUNT(Id) from MPLIN89Headers where Code = @Code";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Code", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("MPLIN89DAL", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.Code = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                MasterVM.DIP = Convert.ToDecimal(commonDal.decimal259(MasterVM.DIP));
                MasterVM.Temperature = Convert.ToDecimal(commonDal.decimal259(MasterVM.Temperature));
                MasterVM.SP_Gravity = Convert.ToDecimal(commonDal.decimal259(MasterVM.SP_Gravity));
                MasterVM.IssueNaturalQuantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.IssueNaturalQuantity));
                MasterVM.Issueat30Quantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.Issueat30Quantity));
                MasterVM.ReceiveNaturalQuantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.ReceiveNaturalQuantity));
                MasterVM.Receiveat30Quantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.Receiveat30Quantity));
                MasterVM.GainNaturalQuantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.GainNaturalQuantity));
                MasterVM.Gainat30Quantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.Gainat30Quantity));

                MasterVM.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);
                MasterVM.Post = "N";
                if (MasterVM.GainNaturalQuantity > 0 || MasterVM.Gainat30Quantity > 0)
                {
                    MasterVM.IsLoss = true;
                }

                retResults = MPLIN89InsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Insert into Details(Insert complete in Header)
                int lineNumber = 0;
                if (MasterVM.MPLIN89DetailsVMs != null)
                {
                    if (MasterVM.MPLIN89DetailsVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                    }
                    
                    foreach (MPLIN89DetailsVM collection in MasterVM.MPLIN89DetailsVMs)
                    {
                        collection.MPLIN89HeaderId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        collection.Post = "N";
                        collection.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);
                        collection.TransactionType = MasterVM.TransactionType;
                        lineNumber++;
                        collection.LineNumber = lineNumber;

                        retResults = MPLIN89InsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion Insert into Details(Insert complete in Header)

                #region Insert into Issue Details(Insert complete in Header)

                if (MasterVM.MPLIN89IssueDetailsVMs != null)
                {
                    if (MasterVM.MPLIN89IssueDetailsVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                    }

                    lineNumber = 0;
                    foreach (MPLIN89IssueDetailsVM collection in MasterVM.MPLIN89IssueDetailsVMs)
                    {
                        collection.MPLIN89HeaderId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        collection.Post = "N";
                        collection.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);
                        collection.TransactionType = MasterVM.TransactionType;
                        lineNumber++;
                        collection.LineNumber = lineNumber;

                        retResults = MPLIN89IssueInsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }               

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion Insert into Issue Details(Insert complete in Header)


                #region update receive flag

                foreach (MPLIN89DetailsVM collection in MasterVM.MPLIN89DetailsVMs)
                {
                    sqlText = "";
                    sqlText +=
                        @" UPDATE TransferMPLReceiveDetails SET UsedStatus='Used in 89'  ";
                    sqlText += @"  WHERE Id IN (" + collection.TransferMPLReceiveDetailId + ") ";

                    SqlCommand cmdPost = new SqlCommand(sqlText, currConn, transaction);
                    cmdPost.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);

                    int res = cmdPost.ExecuteNonQuery();
                }

                #endregion  update receive flag

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

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
                FileLogger.Log("MPLIN89DAL", "MPLIN89Insert", ex.ToString() + "\n" + sqlText);

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

        public string[] MPLIN89InsertToMaster(MPLIN89VM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO MPLIN89Headers";
                sqlText += " (";
                sqlText += "  Code";
                sqlText += " ,BranchId";
                sqlText += " ,TransactionDateTime";
                sqlText += " ,TransactionType";
                sqlText += " ,Comments";
                sqlText += " ,Post";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,SignatoryDesig";
                sqlText += " ,DIP";
                sqlText += " ,Temperature";
                sqlText += " ,SP_Gravity";
                sqlText += " ,IssueNaturalQuantity";
                sqlText += " ,Issueat30Quantity";
                sqlText += " ,ReceiveNaturalQuantity";
                sqlText += " ,Receiveat30Quantity";
                sqlText += " ,GainNaturalQuantity";
                sqlText += " ,Gainat30Quantity";
                sqlText += " ,IsLoss";
                sqlText += " ,ItemNo";


                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @Code";
                sqlText += "  ,@BranchId";
                sqlText += "  ,@TransactionDateTime";
                sqlText += "  ,@TransactionType";
                sqlText += "  ,@Comments";
                sqlText += "  ,@Post";
                sqlText += "  ,@CreatedBy";
                sqlText += "  ,@CreatedOn";
                sqlText += "  ,@SignatoryDesig";
                sqlText += "  ,@DIP";
                sqlText += "  ,@Temperature";
                sqlText += "  ,@SP_Gravity";
                sqlText += "  ,@IssueNaturalQuantity";
                sqlText += "  ,@Issueat30Quantity";
                sqlText += "  ,@ReceiveNaturalQuantity";
                sqlText += "  ,@Receiveat30Quantity";
                sqlText += "  ,@GainNaturalQuantity";
                sqlText += "  ,@Gainat30Quantity";
                sqlText += "  ,@IsLoss";
                sqlText += "  ,@ItemNo";



                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DIP", Master.DIP);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Temperature", Master.Temperature);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SP_Gravity", Master.SP_Gravity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IssueNaturalQuantity", Master.IssueNaturalQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Issueat30Quantity", Master.Issueat30Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveNaturalQuantity", Master.ReceiveNaturalQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Receiveat30Quantity", Master.Receiveat30Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@GainNaturalQuantity", Master.GainNaturalQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Gainat30Quantity", Master.Gainat30Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsLoss", Master.IsLoss);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", Master.ItemNo);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
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

                FileLogger.Log("MPLIN89DAL", "SalesMPLInsertToMaster", ex.ToString() + "\n" + sqlText);
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


        public string[] MPLIN89InsertToDetails(MPLIN89DetailsVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO MPLIN89Details";
                sqlText += " (";
                sqlText += " BranchId ";
                sqlText += ",MPLIN89HeaderId ";
                sqlText += ",TransactionDateTime ";
                sqlText += ",TransactionType ";
                sqlText += ",Post ";
                sqlText += ",LineNumber ";
                sqlText += ",TransferMPLReceiveDetailId ";
                sqlText += ",TransferMPLReceiveId ";
                sqlText += ",ItemNo ";
                sqlText += ",ReceiveNaturalQuantity ";
                sqlText += ",WagonNo ";



                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @BranchId";
                sqlText += " ,@MPLIN89HeaderId";
                sqlText += " ,@TransactionDateTime";
                sqlText += " ,@TransactionType";
                sqlText += " ,@Post";
                sqlText += " ,@LineNumber";
                sqlText += " ,@TransferMPLReceiveDetailId";
                sqlText += " ,@TransferMPLReceiveId";
                sqlText += " ,@ItemNo";
                sqlText += " ,@ReceiveNaturalQuantity";
                sqlText += " ,@WagonNo";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MPLIN89HeaderId", details.MPLIN89HeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", details.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", details.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", details.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LineNumber", details.LineNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferMPLReceiveDetailId", details.TransferMPLReceiveDetailId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferMPLReceiveId", details.TransferMPLReceiveId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", details.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveNaturalQuantity", details.ReceiveNaturalQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@WagonNo", details.WagonNo);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
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
                retResults[2] = "" + details.MPLIN89HeaderId;
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

                FileLogger.Log("SaleMPLDAL", "MPLIN89InsertToDetails", ex.ToString() + "\n" + sqlText);
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

        public string[] MPLIN89IssueInsertToDetails(MPLIN89IssueDetailsVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO MPLIN89IssueDetails";
                sqlText += " (";
                sqlText += " BranchId ";
                sqlText += ",MPLIN89HeaderId ";
                sqlText += ",TransactionDateTime ";
                sqlText += ",TransactionType ";
                sqlText += ",LineNumber ";
                sqlText += ",Post ";
                sqlText += ",TransferIssueMasterRefId ";
                sqlText += ",TransferIssueDetailsRefId ";
                sqlText += ",ItemNo ";
                sqlText += ",Temperature ";
                sqlText += ",SP_Gravity ";
                sqlText += ",DIP ";
                sqlText += ",WagonNo ";
                sqlText += ",IssueNaturalQuantity ";
                sqlText += ",Issueat30Quantity ";
                sqlText += ",TransferFrom ";



                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @BranchId";
                sqlText += " ,@MPLIN89HeaderId";
                sqlText += " ,@TransactionDateTime";
                sqlText += " ,@TransactionType";
                sqlText += " ,@LineNumber";
                sqlText += " ,@Post";
                sqlText += " ,@TransferIssueMasterRefId";
                sqlText += " ,@TransferIssueDetailsRefId";
                sqlText += " ,@ItemNo";
                sqlText += " ,@Temperature";
                sqlText += " ,@SP_Gravity";
                sqlText += " ,@DIP";
                sqlText += " ,@WagonNo";
                sqlText += " ,@IssueNaturalQuantity";
                sqlText += " ,@Issueat30Quantity";
                sqlText += " ,@TransferFrom";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MPLIN89HeaderId", details.MPLIN89HeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", details.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", details.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LineNumber", details.LineNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", details.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferIssueMasterRefId", details.TransferIssueMasterRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferIssueDetailsRefId", details.TransferIssueDetailsRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", details.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Temperature", details.Temperature);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SP_Gravity", details.SP_Gravity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DIP", details.DIP);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@WagonNo", details.WagonNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IssueNaturalQuantity", details.IssueNaturalQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Issueat30Quantity", details.Issueat30Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferFrom", details.TransferFrom);



                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
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
                retResults[2] = "" + details.MPLIN89HeaderId;
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

                FileLogger.Log("SaleMPLDAL", "MPLIN89IssueInsertToDetails", ex.ToString() + "\n" + sqlText);
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


        public string[] MPLIN89Update(MPLIN89VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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

            MPLIN89VM IN89VM = new MPLIN89VM();
            List<MPLIN89DetailsVM> details = new List<MPLIN89DetailsVM>();
            List<MPLIN89IssueDetailsVM> issueDetails = new List<MPLIN89IssueDetailsVM>();

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.insert, "Please Check Transaction Date and Time");

                }

                #endregion Validation for Header

                #region commonDal

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

                #endregion commonDal

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
                    transaction = currConn.BeginTransaction(MessageVM.insert);
                }


                #endregion open connection and transaction


                #region Update Information in Header

                MasterVM.DIP = Convert.ToDecimal(commonDal.decimal259(MasterVM.DIP));
                MasterVM.Temperature = Convert.ToDecimal(commonDal.decimal259(MasterVM.Temperature));
                MasterVM.SP_Gravity = Convert.ToDecimal(commonDal.decimal259(MasterVM.SP_Gravity));
                MasterVM.IssueNaturalQuantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.IssueNaturalQuantity));
                MasterVM.Issueat30Quantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.Issueat30Quantity));
                MasterVM.ReceiveNaturalQuantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.ReceiveNaturalQuantity));
                MasterVM.Receiveat30Quantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.Receiveat30Quantity));
                MasterVM.GainNaturalQuantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.GainNaturalQuantity));
                MasterVM.Gainat30Quantity = Convert.ToDecimal(commonDal.decimal259(MasterVM.Gainat30Quantity));
               
                MasterVM.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);

                retResults = MPLIN89UpdateToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" DELETE FROM MPLIN89Details WHERE MPLIN89HeaderId=@MPLIN89HeaderId ";
                sqlText += @" DELETE FROM MPLIN89IssueDetails WHERE MPLIN89HeaderId=@MPLIN89HeaderId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MPLIN89HeaderId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.MPLIN89DetailsVMs != null)
                {
                    if (MasterVM.MPLIN89DetailsVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (MPLIN89DetailsVM collection in MasterVM.MPLIN89DetailsVMs)
                    {
                        collection.MPLIN89HeaderId = MasterVM.Id;
                        collection.BranchId = MasterVM.BranchId;
                        collection.Post = "N";
                        collection.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);
                        collection.TransactionType = MasterVM.TransactionType;

                        retResults = MPLIN89InsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion Insert into Details(Insert complete in Header)

                #region Insert into Issue Details(Insert complete in Header)

                if (MasterVM.MPLIN89IssueDetailsVMs != null)
                {
                    if (MasterVM.MPLIN89IssueDetailsVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (MPLIN89IssueDetailsVM collection in MasterVM.MPLIN89IssueDetailsVMs)
                    {
                        collection.MPLIN89HeaderId = MasterVM.Id;
                        collection.BranchId = MasterVM.BranchId;
                        collection.Post = "N";
                        collection.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);
                        collection.TransactionType = MasterVM.TransactionType;

                        retResults = MPLIN89IssueInsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion Insert into Issue Details(Insert complete in Header)
                
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

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
                FileLogger.Log("MPLIN89DAL", "MPLIN89Update", ex.ToString() + "\n" + sqlText);

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


        public string[] MPLIN89UpdateToMaster(MPLIN89VM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " UPDATE MPLIN89Headers SET  ";

                sqlText += "  TransactionDateTime=@TransactionDateTime";
                sqlText += " ,TransactionType=@TransactionType";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,SignatoryDesig=@SignatoryDesig";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,DIP=@DIP";
                sqlText += " ,Temperature=@Temperature";
                sqlText += " ,SP_Gravity=@SP_Gravity";
                sqlText += " ,IssueNaturalQuantity=@IssueNaturalQuantity";
                sqlText += " ,Issueat30Quantity=@Issueat30Quantity";
                sqlText += " ,ReceiveNaturalQuantity=@ReceiveNaturalQuantity";
                sqlText += " ,Receiveat30Quantity=@Receiveat30Quantity";
                sqlText += " ,GainNaturalQuantity=@GainNaturalQuantity";
                sqlText += " ,Gainat30Quantity=@Gainat30Quantity";
                sqlText += " ,IsLoss=@IsLoss";
                sqlText += " ,ItemNo=@ItemNo";



                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DIP", Master.DIP);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Temperature", Master.Temperature);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SP_Gravity", Master.SP_Gravity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueNaturalQuantity", Master.IssueNaturalQuantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Issueat30Quantity", Master.Issueat30Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceiveNaturalQuantity", Master.ReceiveNaturalQuantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Receiveat30Quantity", Master.Receiveat30Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@GainNaturalQuantity", Master.GainNaturalQuantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Gainat30Quantity", Master.Gainat30Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsLoss", Master.IsLoss);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ItemNo", Master.ItemNo);




                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgUpdateNotSuccessfully, MessageVM.PurchasemsgUpdateNotSuccessfully);
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
                FileLogger.Log("MPLIN89DAL", "MPLIN89UpdateToMaster", ex.ToString() + "\n" + sqlText);
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


        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
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
                    sqlText = @"SELECT TOP  " + count + " ";
                }

                sqlText += @"
 H.Id
,ISNULL(H.Code,'') Code
,ISNULL(H.BranchId,'0') BranchId
,ISNULL(FORMAT(H.TransactionDateTime,'dd-MMM-yyyy'),'') TransactionDateTime
,ISNULL(H.TransactionType,'') TransactionType
,ISNULL(H.Comments,'') Comments
,ISNULL(H.Post,'N') Post
,ISNULL(H.CreatedBy,'') CreatedBy
,ISNULL(H.CreatedOn,'') CreatedOn
,ISNULL(H.LastModifiedBy,'') LastModifiedBy
,ISNULL(H.SignatoryDesig,'') SignatoryDesig
,ISNULL(H.LastModifiedOn,'') LastModifiedOn
,ISNULL(H.DIP,'0') DIP
,ISNULL(H.Temperature,'0') Temperature
,ISNULL(H.SP_Gravity,'0') SP_Gravity
,ISNULL(H.IssueNaturalQuantity,'0') IssueNaturalQuantity
,ISNULL(H.Issueat30Quantity,'0') Issueat30Quantity
,ISNULL(H.ReceiveNaturalQuantity,'0') ReceiveNaturalQuantity
,ISNULL(H.Receiveat30Quantity,'0') Receiveat30Quantity
,ISNULL(H.GainNaturalQuantity,'0') GainNaturalQuantity
,ISNULL(H.Gainat30Quantity,'0') Gainat30Quantity
,ISNULL(H.IsLoss,'0') IsLoss

,ISNULL(H.ItemNo,'0') ItemNo
,ISNULL(P.ProductCode,'') ProductCode
,ISNULL(P.ProductName,'') ProductName

FROM MPLIN89Headers H
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = H.BranchId
LEFT OUTER JOIN Products P ON H.ItemNo = P.ItemNo
WHERE  1=1 ";
                #endregion SqlText

                sqlTextCount += @" 
SELECT COUNT(H.Id) RecordCount
FROM MPLIN89Headers H WHERE  1=1 ";

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
                    sqlText += " AND H.Code IN (" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND H.Id=@Id";
                }
                if (!String.IsNullOrEmpty(transactionType))
                {
                    sqlTextParameter += @" AND H.TransactionType IN (@transactionType)";
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

                sqlTextOrderBy += " order by H.Id desc";

                #region SqlExecution
                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

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
                FileLogger.Log("MPLMPLIN89DAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLMPLIN89DAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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


        public List<MPLIN89VM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLIN89VM> VMs = new List<MPLIN89VM>();
            MPLIN89VM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids, transactionType, Orderby, SelectTop);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new MPLIN89VM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.Code = dr["Code"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.TransactionDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransactionDateTime"].ToString());
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.ItemNo = dr["ItemNo"].ToString();
                        vm.ProductCode = dr["ProductCode"].ToString();
                        vm.ProductName = dr["ProductName"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.Status = vm.Post == "Y" ? "Posted" : "Not Posted";
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = OrdinaryVATDesktop.DateTimeToDate(dr["CreatedOn"].ToString());
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                        vm.LastModifiedOn = OrdinaryVATDesktop.DateTimeToDate(dr["LastModifiedOn"].ToString());
                        vm.DIP = Convert.ToDecimal(dr["DIP"].ToString());
                        vm.Temperature = Convert.ToDecimal(dr["Temperature"].ToString());
                        vm.SP_Gravity = Convert.ToDecimal(dr["SP_Gravity"].ToString());
                        vm.IssueNaturalQuantity = Convert.ToDecimal(dr["IssueNaturalQuantity"].ToString());
                        vm.Issueat30Quantity = Convert.ToDecimal(dr["Issueat30Quantity"].ToString());
                        vm.ReceiveNaturalQuantity = Convert.ToDecimal(dr["ReceiveNaturalQuantity"].ToString());
                        vm.Receiveat30Quantity = Convert.ToDecimal(dr["Receiveat30Quantity"].ToString());
                        vm.GainNaturalQuantity = Convert.ToDecimal(dr["GainNaturalQuantity"].ToString());
                        vm.Gainat30Quantity = Convert.ToDecimal(dr["Gainat30Quantity"].ToString());
                        vm.IsLoss = Convert.ToBoolean(dr["IsLoss"].ToString());

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
                FileLogger.Log("MPLMPLIN89DAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLMPLIN89DAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            #endregion
            return VMs;
        }

        
        public string[] MPLIN89Post(MPLIN89VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            string ids = "";
            string RecDetailsIds = "";
            string ExistingDetailsIds = "";


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

                #region Select Receive Data
                //DataTable dt = new DataTable();
                //sqlText = "";
                //sqlText +=
                //    @" SELECT STRING_AGG(TransferMPLReceiveDetailId, ', ') AS ConcatenatedIds FROM MPLIN89Details ";
                //sqlText += @"  WHERE MPLIN89HeaderId IN (" + ids + ") ";

                //SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                //da.SelectCommand.Transaction = transaction;
                //da.SelectCommand.CommandTimeout = 500;
                //da.Fill(dt);

                //RecDetailsIds = dt.Rows[0][0].ToString();

                #endregion  Select Receive Data

                #region Check Existing data

                //DataTable dTable = new DataTable();
                //sqlText = "";
                //sqlText +=
                //    @" SELECT TransferMPLReceiveDetailId,Post FROM MPLIN89Details ";
                //sqlText += @"  WHERE TransferMPLReceiveDetailId IN (" + RecDetailsIds + ") ";

                //SqlDataAdapter sda = new SqlDataAdapter(sqlText, currConn);
                //sda.SelectCommand.Transaction = transaction;
                //sda.SelectCommand.CommandTimeout = 500;
                //sda.Fill(dTable);

                //foreach (DataRow dr in dTable.Rows)
                //{
                //   if(!string.IsNullOrEmpty(dr["TransferMPLReceiveDetailId"].ToString()) && dr["Post"].ToString() == "Y")
                //    {
                //         throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindExistSame);
                //    }
                //}

                #endregion   Check Existing data

                #region Update Transfer Issue Data

                sqlText = "";
                sqlText +=
                 @" UPDATE MPLIN89Headers SET Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=GETDATE() ";
                sqlText += @"  WHERE Id IN (" + ids + ") ";
                sqlText +=
                    @" UPDATE MPLIN89Details SET Post='Y'  ";
                sqlText += @"  WHERE MPLIN89HeaderId IN (" + ids + ") ";
                sqlText +=
                    @" UPDATE MPLIN89IssueDetails SET Post='Y'  ";
                sqlText += @"  WHERE MPLIN89HeaderId IN (" + ids + ") ";

                //sqlText +=
                //    @" UPDATE TransferMPLReceiveDetails SET UsedStatus='Used in 89'  ";
                //sqlText += @"  WHERE Id IN (" + RecDetailsIds + ") ";

                SqlCommand cmdPost = new SqlCommand(sqlText, currConn, transaction);
                cmdPost.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);

                transResult = cmdPost.ExecuteNonQuery();
                
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
                FileLogger.Log("MPLIN89DAL", "MPLIN89Post", ex.ToString() + "\n" + sqlText);

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

        public List<MPLIN89VM> DropDown(string branchId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLIN89VM> VMs = new List<MPLIN89VM>();
            MPLIN89VM vm;
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
                sqlText = @" SELECT Id,Code FROM MPLIN89Headers WHERE  1=1  AND BranchId = @BranchId ";
                
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValueAndNullHandle("@BranchId", branchId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLIN89VM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
                FileLogger.Log("MPLIN89DAL", "DropDown", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLIN89DAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(MPLIN89VM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        
        public List<MPLIN89VM> TransReceiveIndex(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            #region Variables

            List<MPLIN89VM> lst = new List<MPLIN89VM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("TransReceiveIndex");

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

SELECT DISTINCT
 ISNULL(D.Id,'0') TransferIssueMasterRefId
,ISNULL(D.TransferIssueDetailsRefId,'0') TransferIssueDetailsRefId
,ISNULL(R.TransferReceiveNo,'') TransferReceiveNo
,ISNULL(FORMAT(D.ReceiveDateTime,'dd-MMM-yyyy'),'') ReceiveDateTime
,ISNULL(D.WagonNo,'') WagonNo
,ISNULL(D.Quantity,'0') Quantity

,ISNULL(D.ItemNo,'0') ItemNo
,ISNULL(P.ProductCode,'') ProductCode
,ISNULL(P.ProductName,'') ProductName
,ISNULL(B.BranchName,'') TransferFrom

FROM  TransferMPLReceiveDetails D

LEFT OUTER JOIN Products P ON D.ItemNo = P.ItemNo
LEFT OUTER JOIN TransferMPLReceives R ON D.TransferMPLReceiveId = R.Id
LEFT OUTER JOIN BranchProfiles B ON D.TransferFrom = B.BranchID

WHERE 1=1  AND D.Post = 'Y' AND R.TransferType = 'IN-43' AND D.UsedStatus IS NULL ";

                if (Convert.ToInt32(Id) > 0)
                {
                    sqlText += @" AND D.Id=@Id";
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

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter
                

                if (Convert.ToInt32(Id) > 0)
                {
                    objCommSaleDetail.Parameters.AddWithValue("@Id", Id);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);

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
                            dataAdapter.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            dataAdapter.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }

                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLIN89VM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLIN89DAL", "TransReceiveIndex",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLIN89DAL", "TransReceiveIndex",
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


        public List<MPLIN89DetailsVM> SearchMPLIN89DetailsList(string mplIN89HeaderId,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLIN89DetailsVM> lst = new List<MPLIN89DetailsVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("MPLIN89Details");

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
SELECT DISTINCT
 D.Id
,ISNULL(D.BranchId,'0') BranchId
,ISNULL(D.MPLIN89HeaderId,'0') MPLIN89HeaderId
,ISNULL(FORMAT(D.TransactionDateTime,'dd-MMM-yyyy'),'') TransactionDateTime
,ISNULL(D.TransactionType,'') TransactionType
,ISNULL(D.Post,'N') Post
,ISNULL(D.LineNumber,'') LineNumber
,ISNULL(D.WagonNo,'') WagonNo
,ISNULL(D.TransferMPLReceiveDetailId,'0') TransferMPLReceiveDetailId
,ISNULL(D.TransferMPLReceiveId,'0') TransferMPLReceiveId
,ISNULL(D.ItemNo,'0') ItemNo
,ISNULL(D.ReceiveNaturalQuantity,'0') ReceiveNaturalQuantity

,ISNULL(R.TransferReceiveNo,'') TransferReceiveNo
,ISNULL(P.ProductCode,'') ProductCode
,ISNULL(P.ProductName,'') ProductName
,ISNULL(B.BranchName,'') BranchName

FROM  MPLIN89Details D
LEFT OUTER JOIN Products P ON D.ItemNo = P.ItemNo
LEFT OUTER JOIN TransferMPLReceives R ON D.TransferMPLReceiveId = R.Id
LEFT OUTER JOIN TransferMPLReceiveDetails RD ON D.TransferMPLReceiveDetailId = RD.Id
LEFT OUTER JOIN BranchProfiles B ON D.BranchId = B.BranchID

WHERE 1=1  ";

                if (!string.IsNullOrEmpty(mplIN89HeaderId))
                {
                    if (Convert.ToInt32(mplIN89HeaderId) > 0)
                    {
                        sqlText += @" AND D.MPLIN89HeaderId=@MPLIN89HeaderId";
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

                if (!string.IsNullOrEmpty(mplIN89HeaderId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@MPLIN89HeaderId", mplIN89HeaderId);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLIN89DetailsVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchMPLIN89DetailsList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchMPLIN89DetailsList",
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

       
        public List<MPLIN89IssueDetailsVM> SearchMPLIN89IssueDetailsList(string mplIN89HeaderId,
         SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLIN89IssueDetailsVM> lst = new List<MPLIN89IssueDetailsVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("MPLIN89IssueDetailsVM");

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
SELECT DISTINCT
 D.Id
,ISNULL(D.BranchId,'0') BranchId
,ISNULL(D.MPLIN89HeaderId,'0') MPLIN89HeaderId
,ISNULL(FORMAT(D.TransactionDateTime,'dd-MMM-yyyy'),'') TransactionDateTime
,ISNULL(D.TransactionType,'') TransactionType
,ISNULL(D.LineNumber,'') LineNumber
,ISNULL(D.Post,'N') Post
,ISNULL(D.TransferIssueMasterRefId,'0') TransferIssueMasterRefId
,ISNULL(D.TransferIssueDetailsRefId,'0') TransferIssueDetailsRefId
,ISNULL(D.ItemNo,'0') ItemNo
,ISNULL(D.Temperature,'0') Temperature
,ISNULL(D.SP_Gravity,'0') SP_Gravity
,ISNULL(D.DIP,'0') DIP
,ISNULL(D.WagonNo,'0') WagonNo
,ISNULL(D.IssueNaturalQuantity,'0') IssueNaturalQuantity
,ISNULL(D.Issueat30Quantity,'0') Issueat30Quantity
,ISNULL(D.TransferFrom,'') TransferFrom


,ISNULL(I.TransferIssueNo,'') TransferIssueNo
,ISNULL(P.ProductCode,'') ProductCode
,ISNULL(P.ProductName,'') ProductName
,ISNULL(B.BranchName,'') BranchName

FROM  MPLIN89IssueDetails D
LEFT OUTER JOIN Products P ON D.ItemNo = P.ItemNo
LEFT OUTER JOIN TransferMPLIssues I ON D.TransferIssueMasterRefId = I.Id
LEFT OUTER JOIN TransferMPLIssueDetails ID ON D.TransferIssueDetailsRefId = ID.Id
LEFT OUTER JOIN BranchProfiles B ON D.BranchId = B.BranchID

WHERE 1=1  ";

                if (!string.IsNullOrEmpty(mplIN89HeaderId))
                {
                    if (Convert.ToInt32(mplIN89HeaderId) > 0)
                    {
                        sqlText += @" AND D.MPLIN89HeaderId=@MPLIN89HeaderId";
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

                if (!string.IsNullOrEmpty(mplIN89HeaderId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@MPLIN89HeaderId", mplIN89HeaderId);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLIN89IssueDetailsVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchMPLIN89IssueDetailsList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchMPLIN89IssueDetailsList",
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


        public List<MPLIN89IssueDetailsVM> SearchTransferMPLIssuesList(string ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLIN89IssueDetailsVM> lst = new List<MPLIN89IssueDetailsVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("MPLIN89IssueDetailsVM");

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
SELECT DISTINCT
D.Id TransferIssueDetailsRefId
,ISNULL(D.TransferMPLIssueId,0) TransferIssueMasterRefId
,ISNULL(D.ItemNo,'0') ItemNo
--,ISNULL(D.Quantity,0) IssueNaturalQuantity
,ISNULL(D.RequestedVolumn,0) RequestedVolumn
,ISNULL(D.UOMQty,0) IssueNaturalQuantity
,ISNULL(D.UOMQty,0) UOMQty
,ISNULL(D.Temperature,0) Temperature
,ISNULL(D.SP_Gravity,0) SP_Gravity
,ISNULL(D.DIP,0) DIP
,ISNULL(D.WagonNo,'') WagonNo
,ISNULL(D.QtyAt30Temperature,0) Issueat30Quantity

,ISNULL(I.TransferIssueNo,'') TransferIssueNo
,ISNULL(P.ProductName,'') ProductName
,ISNULL(P.ProductCode,'') ProductCode
,ISNULL(B.BranchName,'') TransferFrom



FROM  TransferMPLIssueDetails D
LEFT OUTER JOIN Products P ON D.ItemNo = P.ItemNo
LEFT OUTER JOIN TransferMPLIssues I ON D.TransferMPLIssueId = I.Id
LEFT OUTER JOIN BranchProfiles B ON D.BranchId = B.BranchID


WHERE 1=1  AND D.Post = 'Y'  ";

                if (!string.IsNullOrEmpty(ids))
                {
                    sqlText += @" AND D.Id IN (" + ids + ")";
                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLIN89IssueDetailsVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchTransferMPLIssuesList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchTransferMPLIssuesList",
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

        public List<MPLIN89DetailsVM> SearchTransferMPLReceivesList(string ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLIN89DetailsVM> lsts = new List<MPLIN89DetailsVM>();
            MPLIN89DetailsVM lst = new MPLIN89DetailsVM();
                SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("MPLIN89DetailsVM");

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
SELECT DISTINCT
 ISNULL(D.Id,0) TransferMPLReceiveDetailId 
,ISNULL(D.TransferIssueMasterRefId,0) TransferMPLReceiveId
,ISNULL(R.TransferReceiveNo,'') TransferReceiveNo
,ISNULL(CONVERT(NVARCHAR, FORMAT(D.ReceiveDateTime,'dd-MMM-yyyy'), 120), '') AS ReceiveDateTime
,ISNULL(D.WagonNo,'') WagonNo
--,ISNULL(D.Quantity,0) ReceiveNaturalQuantity
,ISNULL(D.RequestedVolumn,0) RequestedVolumn
,ISNULL(D.UOMQty,0) ReceiveNaturalQuantity
,ISNULL(D.UOMQty,0) UOMQty
,ISNULL(D.SP_Gravity,0) SP_Gravity
,ISNULL(D.Temperature,0) Temperature
,ISNULL(D.QtyAt30Temperature,0) QtyAt30Temperature

,ISNULL(D.ItemNo,'0') ItemNo
,ISNULL(P.ProductCode,'') ProductCode
,ISNULL(P.ProductName,'') ProductName
,ISNULL(B.BranchName,'') TransferFrom

FROM  TransferMPLReceiveDetails D

LEFT OUTER JOIN Products P ON D.ItemNo = P.ItemNo
LEFT OUTER JOIN TransferMPLReceives R ON D.TransferMPLReceiveId = R.Id
LEFT OUTER JOIN BranchProfiles B ON D.TransferFrom = B.BranchID

WHERE 1=1  AND D.Post = 'Y' ";

              
                if (!string.IsNullOrEmpty(ids))
                {
                    sqlText += @" AND D.Id IN (" + ids + ")";
                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);
                foreach (DataRow dr in dataTable.Rows)
                {
                    try
                    {
                        lst = new MPLIN89DetailsVM();
                         
                        lst.TransferMPLReceiveDetailId = Convert.ToInt32(dr["TransferMPLReceiveDetailId"].ToString());
                        lst.TransferMPLReceiveId = Convert.ToInt32(dr["TransferMPLReceiveId"].ToString());
                        lst.TransferReceiveNo = dr["TransferReceiveNo"].ToString();
                        lst.ReceiveDateTime = dr["ReceiveDateTime"].ToString();
                        lst.WagonNo = dr["WagonNo"].ToString();
                        lst.ReceiveNaturalQuantity = Convert.ToDecimal(dr["ReceiveNaturalQuantity"].ToString());
                        lst.RequestedVolumn = Convert.ToDecimal(dr["RequestedVolumn"].ToString());
                        lst.UOMQty =Convert.ToDecimal( dr["UOMQty"].ToString());
                        lst.SP_Gravity =Convert.ToDecimal( dr["SP_Gravity"].ToString());
                        lst.Temperature = Convert.ToDecimal(dr["Temperature"].ToString());
                        lst.QtyAt30Temperature =Convert.ToDecimal( dr["QtyAt30Temperature"].ToString());
                        lst.ItemNo = dr["ItemNo"].ToString();
                        lst.ProductCode = dr["ProductCode"].ToString();
                        lst.ProductName = dr["ProductName"].ToString();
                        lst.TransferFrom = dr["TransferFrom"].ToString();

                        lsts.Add(lst);
                    }
                    catch (Exception e)
                    {
                        //
                    }
                }
                //lsts = dataTable.ToList<MPLIN89DetailsVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchTransferMPLReceivesList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLIN89DAL", "SearchTransferMPLReceivesList",
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

            return lsts;
        }
    }
}
