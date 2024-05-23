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
    public class MPLDayEndClosingDAL 
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();

        #endregion



        public string[] InsertToMPLDayEndClosing(MPLDayEndClosingVM MasterVM, SqlTransaction Vtransaction = null,
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

            DataSet dataSet = new DataSet();
            DataTable sale = new DataTable();
            DataTable Bank = new DataTable();
            DataTable Other = new DataTable();


            #endregion Initializ

            #region Try

            try
            {

                #region Validation for Header


                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.DayEndDate) < DateTime.MinValue ||
                         Convert.ToDateTime(MasterVM.DayEndDate) > DateTime.MaxValue)
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

                #region Find Umpost Previuos transaction 

                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(post)post from MPLDayEndClosingHeaders where Post ='N'";
                SqlCommand cmdUnpost = new SqlCommand(sqlText, currConn, transaction);
            
                IDExist = (int)cmdUnpost.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgprevoisUnPostTransaction);
                }

                #endregion Find Transaction Exist

                #region Find Transaction Exist Same Date

                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(Code) from MPLDayEndClosingHeaders WHERE DayEndDate=@DayEndDate ";
                SqlCommand cmdExistDate = new SqlCommand(sqlText, currConn, transaction);

                cmdExistDate.Parameters.AddWithValueAndNullHandle("@DayEndDate",
                    MasterVM.DayEndDate);
                IDExist = (int)cmdExistDate.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindDayEndExistSameDate);
                }


                #endregion Find Transaction Exist


                #region Find Transaction Exist

                int transferIssueId = MasterVM.Id;
                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(Code) from MPLDayEndClosingHeaders WHERE Code=@Code ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@Code", MasterVM.Code);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Transaction ID Create

              

                #region Other


                newIDCreate = commonDal.TransactionCode("DayEnd", "Other", "MPLDayEndClosingHeaders",
                       "Code",
                       "DayEndDate", MasterVM.DayEndDate, MasterVM.BranchId.ToString(), currConn,
                       transaction);




                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(Code) FROM MPLDayEndClosingHeaders 
where Code = @Code  and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@Code", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("MPLDayEndClosingDAL", "Insert",
                        "Code " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText =
                    " select count(Code) from MPLDayEndClosingHeaders where Code = @Code";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Code", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("MPLDayEndClosingDAL", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.Code = newIDCreate;

                #region Call DayEnd Data

                dataSet = SelectDayEndData(MasterVM);
                sale = dataSet.Tables[0];
                Other = dataSet.Tables[1];

                Bank = dataSet.Tables[2];
                #endregion


                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                retResults = MPLDayEndDataInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


                var DayEndClosingHeaderId = new DataColumn("DayEndClosingHeaderId") { DefaultValue = Id };
       
                if (sale.Rows.Count>0)
                {
                    sale.Columns.Add(new DataColumn(DayEndClosingHeaderId.ColumnName, DayEndClosingHeaderId.DataType)
                    {
                        DefaultValue = Id
                    });
                    retResults = commonDal.BulkInsert("MPLDayEndClosingSales", sale, currConn, transaction);
                }

                if (Other.Rows.Count > 0)
                {
                    Other.Columns.Add(new DataColumn(DayEndClosingHeaderId.ColumnName, DayEndClosingHeaderId.DataType)
                    {
                        DefaultValue = Id
                    });

                    retResults = commonDal.BulkInsert("MPLDayEndClosingOther", Other, currConn, transaction);
                }
                if (Bank.Rows.Count > 0)
                {
                    Bank.Columns.Add(new DataColumn(DayEndClosingHeaderId.ColumnName, DayEndClosingHeaderId.DataType)
                    {
                        DefaultValue = Id
                    });

                    retResults = commonDal.BulkInsert("MPLDayEndClosingBank", Bank, currConn, transaction);
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
                retResults[2] = "";
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

                FileLogger.Log("MPLDayEndClosingDAL", "InsertToMPLBankDepositSlip", ex.ToString() + "\n" + sqlText);

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

        public string[] MPLDayEndDataInsertToMaster(MPLDayEndClosingVM Master, SqlConnection VcurrConn = null,
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
                sqlText += " INSERT INTO MPLDayEndClosingHeaders";
                sqlText += " (";
                sqlText += "  Code";
                sqlText += "  ,BranchId";
                sqlText += "  ,UserId";
                sqlText += "  ,DayEndDate";
                sqlText += "  ,Post";
                sqlText += "  ,CreatedBy";
                sqlText += "  ,CreatedOn";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "   @Code";
                sqlText += "   ,@BranchId";
                sqlText += "   ,@UserId";
                sqlText += "   ,@DayEndDate";
                sqlText += "   ,@Post";
                sqlText += "   ,@CreatedBy";
                sqlText += "   ,@CreatedOn";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);


                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserId", Master.UserId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DayEndDate", Master.DayEndDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
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
                retResults[2] = ""+ Master.Code;
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

                FileLogger.Log("MPLDayEndClosingDAL", "MPLBankDepositSlipInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] UpdateMPLDayEndData(MPLDayEndClosingVM MasterVM, SqlTransaction transaction,
            SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";

            DataSet dataSet = new DataSet();
            DataTable sale = new DataTable();
            DataTable Bank = new DataTable();
            DataTable Other = new DataTable();

            int transResult = 0;
            string sqlText = "";
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
                else if (Convert.ToDateTime(MasterVM.DayEndDate) < DateTime.MinValue ||
                         Convert.ToDateTime(MasterVM.DayEndDate) > DateTime.MaxValue)
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

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"

select top 1 Post from MPLDayEndClosingHeaders
where 1=1 
and Code=@Code

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;
                cmd.Parameters.AddWithValue("@Code", MasterVM.Code);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                if (currentPostStatus == "Y")
                {
                    throw new Exception("This Invoice Already Posted! Invoice No: " + MasterVM.Code);
                }

                #endregion

                #region Find Umpost Previuos transaction

                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(post)post from MPLDayEndClosingHeaders where Post ='N' and Id!=@Id";
                SqlCommand cmdUnpost = new SqlCommand(sqlText, currConn, transaction);
                cmdUnpost.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.Id);

                IDExist = (int)cmdUnpost.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgprevoisUnPostTransaction);
                }

                #endregion Find Transaction Exist

                #region Find Transaction Exist Same Date

                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(Code) from MPLDayEndClosingHeaders WHERE DayEndDate=@DayEndDate and Id!=@Id";
                SqlCommand cmdExistDate = new SqlCommand(sqlText, currConn, transaction);

                cmdExistDate.Parameters.AddWithValueAndNullHandle("@DayEndDate", MasterVM.DayEndDate);
                cmdExistDate.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.Id);
                IDExist = (int)cmdExistDate.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindDayEndExistSameDate);
                }


                #endregion Find Transaction Exist


                #region Call DayEnd Data

                dataSet = SelectDayEndData(MasterVM);
                sale = dataSet.Tables[0];
                Other = dataSet.Tables[1];

                Bank = dataSet.Tables[2];
                #endregion

                #region Update Information in Header

                MasterVM.DayEndDate = OrdinaryVATDesktop.DateToDate(MasterVM.DayEndDate);

                retResults = UpdateMPLDayEndDataMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText +=
                     @" DELETE FROM MPLDayEndClosingSales WHERE DayEndClosingHeaderId=@DayEndClosingHeaderId
                        DELETE FROM MPLDayEndClosingOther WHERE DayEndClosingHeaderId=@DayEndClosingHeaderId 
                        DELETE FROM MPLDayEndClosingBank WHERE DayEndClosingHeaderId=@DayEndClosingHeaderId 
                       ";
                     

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@DayEndClosingHeaderId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                var DayEndClosingHeaderId = new DataColumn("DayEndClosingHeaderId") { DefaultValue = Id };

                if (sale.Rows.Count > 0)
                {
                    sale.Columns.Add(new DataColumn(DayEndClosingHeaderId.ColumnName, DayEndClosingHeaderId.DataType)
                    {
                        DefaultValue = Id
                    });
                    retResults = commonDal.BulkInsert("MPLDayEndClosingSales", sale, currConn, transaction);
                }

                if (Other.Rows.Count > 0)
                {
                    Other.Columns.Add(new DataColumn(DayEndClosingHeaderId.ColumnName, DayEndClosingHeaderId.DataType)
                    {
                        DefaultValue = Id
                    });

                    retResults = commonDal.BulkInsert("MPLDayEndClosingOther", Other, currConn, transaction);
                }
                if (Bank.Rows.Count > 0)
                {
                    Bank.Columns.Add(new DataColumn(DayEndClosingHeaderId.ColumnName, DayEndClosingHeaderId.DataType)
                    {
                        DefaultValue = Id
                    });

                    retResults = commonDal.BulkInsert("MPLDayEndClosingBank", Bank, currConn, transaction);
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
                retResults[2] = "" ;
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

                FileLogger.Log("MPLDayEndClosingDAL", "TransReceiveMPLUpdate", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateMPLDayEndDataMaster(MPLDayEndClosingVM Master, SqlConnection VcurrConn = null,
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
                sqlText += " update MPLDayEndClosingHeaders SET  ";

                sqlText += " Code=@Code";
                sqlText += " ,BranchId=@BranchId";
                sqlText += " ,UserId=@UserId";
                sqlText += " ,DayEndDate=@DayEndDate";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";


                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserId", Master.UserId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DayEndDate", Master.DayEndDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
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

                FileLogger.Log("MPLDayEndClosingDAL", "UpdateMPLBankDepositSlipToMaster",
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

                FileLogger.Log("MPLDayEndClosingDAL", "TransReceiveMPLInsertToDetails",
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

 
       Ded.[Id]
      ,Ded.[DayEndDate]
      ,Ded.[BranchId]
      ,ISNULL(BP.BranchName, '') BranchName
      ,Ded.[UserId]
      ,Ded.[Code]
      ,isnull(Ded.[Post],'N')Post
      ,Ded.[CreatedBy]
      ,Ded.[CreatedOn]
      ,Ded.[LastModifiedBy]
      ,Ded.[LastModifiedOn]
      ,Ded.[PostedBy]
      ,Ded.[PostedOn]
  FROM [MPLDayEndClosingHeaders] Ded
LEFT OUTER JOIN BranchProfiles BP ON BP.BranchID = Ded.BranchId
  WHERE  1=1 ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT COUNT(Ded.Id) RecordCount
FROM MPLDayEndClosingHeaders  Ded WHERE  1=1 ";

               

                if (Id > 0)
                {
                    sqlTextParameter += @" AND Ded.Id=@Id";
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
                FileLogger.Log("MPLDayEndClosingDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLDayEndClosingDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<MPLDayEndClosingVM> SelectAllList(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y",
            string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLDayEndClosingVM> VMs = new List<MPLDayEndClosingVM>();
            MPLDayEndClosingVM vm;

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
                        vm = new MPLDayEndClosingVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.BranchName = dr["BranchName"].ToString();
                        vm.Code = dr["Code"].ToString();
                        vm.DayEndDate = OrdinaryVATDesktop.DateTimeToDate(dr["DayEndDate"].ToString());
                        vm.Post = dr["Post"].ToString();

                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.PostedBy = dr["PostedBy"].ToString();
                        vm.PostedOn = dr["PostedOn"].ToString();

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

                FileLogger.Log("MPLDayEndClosingDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLDayEndClosingDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }



        public DataSet SelectDayEndData(MPLDayEndClosingVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataSet = new DataSet("SelectDayEndData");

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


--declare @InvoiceDateTime varchar(100)='2024-01-02'


select 
SalesInvoiceNo InvoiceNo
, h.ReportType TransactionType
,FORMAT(CAST(h.InvoiceDateTime AS DATETIME), 'yyyy-MM-dd') AS InvoiceDateTime
,cg.CustomerGroupName
,p.ProductName
,p.ProductCode
,p.PackSize
,Quantity
,SubTotal
,VATAmount
,LineTotal

from SalesInvoiceMPLDetails d
left outer join SalesInvoiceMPLHeaders h on d.SalesInvoiceMPLHeaderId=h.Id
left outer join Customers c on h.CustomerID=c.CustomerID
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
left outer join Products p on d.ItemNo=p.ItemNo
 where h.InvoiceDateTime> dateadd(d,0,@InvoiceDateTime) and h.InvoiceDateTime< dateadd(d,1,@InvoiceDateTime)
    AND h.Post = 'Y'
    AND h.BranchId = @BranchId

order by SalesInvoiceNo,p.ProductName


SELECT 
   h.ReportType TransactionType
    ,V.ColName AS Type,
    SUM(
        CASE 
            WHEN ColName = 'SupplyVAT' THEN ISNULL(h.SupplyVAT, 0)
            WHEN ColName = 'TC' THEN ISNULL(h.TC, 0)
            WHEN ColName = 'LF' THEN ISNULL(h.LF, 0)
            WHEN ColName = 'RF' THEN ISNULL(h.RF, 0)
            WHEN ColName = 'SC' THEN ISNULL(h.SC, 0)
            WHEN ColName = 'ShortExcessAmnt' THEN ISNULL(h.ShortExcessAmnt, 0)
            WHEN ColName = 'Toll' THEN ISNULL(h.Toll, 0)
            WHEN ColName = 'DC' THEN ISNULL(h.DC, 0)
            WHEN ColName = 'ATV' THEN ISNULL(h.ATV, 0)
            WHEN ColName = 'LessFrightToPay' THEN ISNULL(h.LessFrightToPay, 0)
            WHEN ColName = 'OtherAmnt' THEN ISNULL(h.OtherAmnt, 0)
            ELSE 0 
        END
    ) AS Amount
FROM 
    SalesInvoiceMPLHeaders h
CROSS APPLY (
    VALUES ('SupplyVAT'), ('TC'), ('LF'), ('RF'), ('SC'), ('ShortExcessAmnt'), ('Toll'), ('DC'), ('ATV'), ('LessFrightToPay'), ('OtherAmnt')
) AS V(ColName)
 where h.InvoiceDateTime> dateadd(d,0,@InvoiceDateTime) and h.InvoiceDateTime< dateadd(d,1,@InvoiceDateTime)

    AND h.Post = 'Y'
    AND h.BranchId = @BranchId

GROUP BY 
    h.ReportType, V.ColName
ORDER BY 
 V.ColName

 Select Sbnk.BankName,h.Code DSCode,h.TransactionType,sum(D.Amount)Amount from MPLBankDepositSlipDetails D
left outer join MPLBankDepositSlipHeaders h on d.BankDepositSlipHeaderId=h.Id
left outer join MPLSelfBankInformations Sbnk on h.SelfBankId=Sbnk.BankID
WHERE 
 h.TransactionDateTime> dateadd(d,0,@InvoiceDateTime) and h.TransactionDateTime< dateadd(d,1,@InvoiceDateTime)
    AND h.Post = 'Y'
    AND h.BranchId =@BranchId

group by  Sbnk.BankName,h.Code,h.TransactionType

";



                #endregion

                #region SQL Command

                SqlCommand objCommSaleReport = new SqlCommand();
                objCommSaleReport.Connection = currConn;

                objCommSaleReport.CommandText = sqlText;
                objCommSaleReport.CommandType = CommandType.Text;

                #endregion

                #region Parameters
                objCommSaleReport.Parameters.AddWithValue("@InvoiceDateTime", vm.DayEndDate);
                objCommSaleReport.Parameters.AddWithValue("@BranchId", vm.BranchId);
                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleReport);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLDayEndClosingDAL", "SelectDayEndData", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLDayEndClosingDAL", "SelectDayEndData", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return dataSet;
        }




        public string[] Post(MPLDayEndClosingVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            int transResult = 0;
            string sqlText = "";

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
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

                #region Update 

                sqlText = "";
                sqlText = @"
UPDATE MPLDayEndClosingHeaders SET Post='Y' ,PostedBy=@PostedBy,PostedOn=@PostedOn WHERE 1=1 AND Id=@Id
";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.Id);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@PostedBy", MasterVM.PostedBy);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@PostedOn", MasterVM.PostedOn);

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
                retResults[4] = MasterVM.Id.ToString();
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
                FileLogger.Log("MPLDayEndClosingDAL", "Post", ex.ToString() + "\n" + sqlText);

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
