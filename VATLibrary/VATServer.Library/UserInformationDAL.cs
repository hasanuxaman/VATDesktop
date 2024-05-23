using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VATServer.Library
{
    public class UserInformationDAL : IUserInformation
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

        public string[] InsertUserInformation(string UserID, string UserName, string UserPassword, string FullName
            , string Designation
            , string ContactNo
            , string Email
            , string Address
            , string ActiveStatus, string IsMainSettings, string LastLoginDateTime, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null, string NationalID = null, byte[] bimage = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user name.");
                }

                Regex regexItem = new Regex("[^A-Za-z0-9]");

                if (regexItem.IsMatch(UserName))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "User name only Alphabets & Numeric.Space not allowed");
                }

                if (string.IsNullOrEmpty(UserPassword))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user password.");
                }

                //if (!OrdinaryVATDesktop.IsNumber(NationalID))
                //{
                //    throw new ArgumentNullException("InsertToUserInformationNew", "Please Enter National ID  only number.");

                //}

                //if (NationalID.Length >= 18)
                //{
                //    throw new ArgumentNullException("InsertToUserInformationNew", "Please Enter Valid National ID No/Not more than 17 digit.");                 
                //}
                //var regexItem1 = new Regex("[0-9]");

                ////if (regexItem1.IsMatch(NationalID))
                ////{
                ////    throw new ArgumentNullException("InsertToUserInformationNew", "NationalID only Numeric.");
                ////}

                #endregion Validation
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #region Load Settings
                CommonDAL commonDal = new CommonDAL();

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(currConn, transaction, connVM);
                }
                UserAuditVM UserAudit = new UserAuditVM();

                UserAudit.MinimumLengthCheck = Convert.ToBoolean(commonDal.settingsCache("Password", "MinimumLengthCheck", settings).ToString().ToLower() == "y" ? true : false);
                UserAudit.MinimumLength = Convert.ToInt32(commonDal.settingsDesktop("Password", "MinimumLength", settings));
                UserAudit.MixPasswordCheck = Convert.ToBoolean(commonDal.settingsCache("Password", "MixPasswordCheck", settings).ToString().ToLower() == "y" ? true : false);
                UserAudit.ChangeDate = Convert.ToInt32(commonDal.settingsDesktop("Password", "ChangeDate", settings));
                UserAudit.MaxWrongLoginTime = Convert.ToInt32(commonDal.settingsDesktop("Password", "MaxWrongLoginTime", settings));

                UserAudit.UserPassword = Converter.DESDecrypt(PassPhrase, EnKey, UserPassword);


                #endregion

                #endregion open connection and transaction

                #region UserName existence checking

                //select @Present = count(distinct UserName) from UserInformations where  UserName=@UserName;
                sqlText = "select count(distinct UserName) from UserInformations where  UserName =@UserName ";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                userNameExist.Parameters.AddWithValueAndNullHandle("@UserName", UserName);
                countId = (int)userNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Same user name already exist.");
                }

                #endregion UserName existence checking

                #region NationalID existence checking

                //select @Present = count(distinct UserName) from UserInformations where  UserName=@UserName;
                sqlText = "select count(distinct NationalID) from UserInformations where  NationalID = @NationalID ";
                SqlCommand nationalidExist = new SqlCommand(sqlText, currConn);
                nationalidExist.Transaction = transaction;
                nationalidExist.Parameters.AddWithValueAndNullHandle("@NationalID", NationalID);

                countId = (int)nationalidExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Same National ID already exist.");
                }

                #endregion NationalID existence checking

                #region Check Security Related issues
                UserValidationResult result = Validate(UserAudit);
                if (!result.IsValid)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", result.ErrorMessage);
                }
                #endregion

                #region User new id generation

                //select @UserID= isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations;
                sqlText = "select isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                }

                #endregion User new id generation

                #region Insert new user

                sqlText = "";
                sqlText += "insert into UserInformations";
                sqlText += "(";
                sqlText += "UserID,";
                sqlText += "UserName,";
                sqlText += "UserPassword,";
                sqlText += "FullName,";
                sqlText += "Designation,";
                sqlText += "ContactNo,";
                sqlText += "Email,";
                sqlText += "NationalID,";
                sqlText += "Address,";
                sqlText += "ActiveStatus,";
                sqlText += "LastLoginDateTime,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "IsMainSettings,";
                sqlText += "LastPasswordChangeDate,";
                sqlText += "Signature";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@UserID,";
                sqlText += "@UserName,";
                sqlText += "@UserPassword,";
                sqlText += "@FullName,";
                sqlText += "@Designation,";
                sqlText += "@ContactNo,";
                sqlText += "@Email,";
                sqlText += "@NationalID,";
                sqlText += "@Address,";
                sqlText += "@ActiveStatus,";
                sqlText += "@LastLoginDateTime,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@IsMainSettings,";
                sqlText += "@LastPasswordChangeDate,";
                sqlText += "@bimage";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserID", nextId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserName", UserName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserPassword", UserPassword);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FullName", FullName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Designation", Designation);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ContactNo", ContactNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Email", Email);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NationalID", NationalID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Address", Address);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastLoginDateTime", LastLoginDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsMainSettings", IsMainSettings);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastPasswordChangeDate", CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@bimage", bimage);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information successfully Added.";
                        retResults[2] = "" + nextId;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add user";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add user ";
                    retResults[2] = "";
                }

                #endregion Commit

                #endregion Insert new user

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw sqlex;
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

            return retResults;
        }

        public string InsertUserLogin(string LogID, string ComputerName, string ComputerLoginUserName,
            string ComputerIPAddress, string SoftwareUserId, string SessionDate, string LogInDateTime,
            string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                string tt = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog

                //commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn); //tablename,fieldName, datatype

                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);

                #endregion UserLog

                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction
                sqlText = "";
                sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                sqlText += " WHERE ComputerName=@ComputerName";
                sqlText += " and ComputerLoginUserName=@ComputerLoginUserName";
                sqlText += " and ComputerIPAddress=@ComputerIPAddress";
                sqlText += " and SoftwareUserId=@SoftwareUserId";
                sqlText += " and SessionDate=@SessionDate";
                sqlText += " and LogInDateTime=@LogInDateTime";
                sqlText += " and LogOutDateTime=@LogOutDateTime";

                SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                cmdFindId.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                parameter.Value = ComputerName;
                cmdFindId.Parameters.Add(parameter);
                parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                parameter.Value = ComputerLoginUserName;
                cmdFindId.Parameters.Add(parameter);
                parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                parameter.Value = ComputerIPAddress;
                cmdFindId.Parameters.Add(parameter);
                parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                parameter.Value = SoftwareUserId;
                cmdFindId.Parameters.Add(parameter);
                parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                parameter.Value = SessionDate;
                cmdFindId.Parameters.Add(parameter);
                parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                parameter.Value = LogInDateTime;
                cmdFindId.Parameters.Add(parameter);
                parameter = new SqlParameter("@LogOutDateTime", SqlDbType.DateTime);
                parameter.Value = LogOutDateTime;
                cmdFindId.Parameters.Add(parameter);

                int IDExist = (int)cmdFindId.ExecuteScalar();

                if (IDExist > 0)//update
                {
                    #region Update

                    sqlText = "";
                    sqlText += " UPDATE UserAuditLogs";
                    sqlText += " SET LogOutDateTime = @LogOutDateTime";
                    sqlText += " WHERE LogID=@LogID";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;

                    parameter = new SqlParameter("@LogOutDateTime", SqlDbType.DateTime);
                    parameter.Value = LogOutDateTime;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@LogID", SqlDbType.VarChar, 50);
                    parameter.Value = LogID;
                    cmdInsert.Parameters.Add(parameter);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();

                            retResults = "" + LogID;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults = "0";

                        }

                    }
                    else
                    {
                        retResults = "0";
                    }

                    #endregion Commit

                    #endregion Insert new user
                }
                else // insert 
                {



                    #region User new id generation


                    sqlText = "select isnull(max(cast(LogID as int)),0)+1 FROM  UserAuditLogs";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int vLogID = (int)cmdNextId.ExecuteScalar();
                    if (vLogID <= 0)
                    {
                        throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                    }

                    #endregion User new id generation

                    #region Insert new user

                    sqlText = "";
                    sqlText += " INSERT INTO UserAuditLogs";
                    sqlText += " (	LogID,";
                    sqlText += " 	ComputerName,";
                    sqlText += " 	ComputerLoginUserName,";
                    sqlText += " 	ComputerIPAddress,";
                    sqlText += " 	SoftwareUserId,";
                    sqlText += " 	SessionDate,";
                    sqlText += " 	LogInDateTime,";
                    sqlText += " 	LogOutDateTime";
                    sqlText += " )";
                    sqlText += "   VALUES";
                    sqlText += " (";
                    sqlText += "@vLogID,";
                    sqlText += "@ComputerName,";
                    sqlText += "@ComputerLoginUserName,";
                    sqlText += "@ComputerIPAddress,";
                    sqlText += "@SoftwareUserId,";
                    sqlText += "@SessionDate,";
                    sqlText += "@LogInDateTime,";
                    sqlText += "@LogOutDateTime";
                    sqlText += " )";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;

                    parameter = new SqlParameter("@vLogID", SqlDbType.VarChar, 50);
                    parameter.Value = vLogID;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                    parameter.Value = ComputerName;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                    parameter.Value = ComputerLoginUserName;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                    parameter.Value = ComputerIPAddress;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                    parameter.Value = SoftwareUserId;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                    parameter.Value = SessionDate;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                    parameter.Value = LogInDateTime;
                    cmdInsert.Parameters.Add(parameter);
                    parameter = new SqlParameter("@LogOutDateTime", SqlDbType.DateTime);
                    parameter.Value = LogOutDateTime;
                    cmdInsert.Parameters.Add(parameter);


                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    #endregion Insert new user
                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();

                            retResults = "" + vLogID;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults = "0";

                        }

                    }
                    else
                    {
                        retResults = "0";
                    }

                    #endregion Commit
                }

            }
            #region catch finally

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }

        public string InsertUserLogin(List<UserLogsVM> Details, string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                string vDatabaseName = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog

                commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction); //tablename,fieldName, datatype

                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);

                #endregion UserLog

                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                foreach (UserLogsVM Item in Details.ToList())
                {
                    #region IfSameDatabase
                    if (Item.DataBaseName == vDatabaseName)
                    {

                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName=@ComputerName";
                        sqlText += " and ComputerLoginUserName=@ComputerLoginUserName";
                        sqlText += " and ComputerIPAddress=@ComputerIPAddress";
                        sqlText += " and SoftwareUserId=@SoftwareUserId";
                        sqlText += " and SessionDate=@SessionDate";
                        sqlText += " and LogInDateTime=@LogInDateTime";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;


                        SqlParameter parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerName;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerLoginUserName;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerIPAddress;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                        parameter.Value = Item.SoftwareUserId;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                        parameter.Value = Item.SessionDate;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                        parameter.Value = Item.LogInDateTime;
                        cmdFindId.Parameters.Add(parameter);

                        int IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist <= 0)//update
                        {
                            #region User new id generation


                            sqlText = "select isnull(max(cast(LogID as int)),0)+1 FROM  UserAuditLogs";
                            SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                            cmdNextId.Transaction = transaction;
                            int vLogID = (int)cmdNextId.ExecuteScalar();
                            if (vLogID <= 0)
                            {
                                throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                            }

                            #endregion User new id generation

                            #region Insert new user

                            sqlText = "";
                            sqlText += " INSERT INTO UserAuditLogs";
                            sqlText += " (	LogID,";
                            sqlText += " 	ComputerName,";
                            sqlText += " 	ComputerLoginUserName,";
                            sqlText += " 	ComputerIPAddress,";
                            sqlText += " 	SoftwareUserId,";
                            sqlText += " 	SessionDate,";
                            sqlText += " 	LogInDateTime";
                            sqlText += " )";
                            sqlText += "   VALUES";
                            sqlText += " (";
                            sqlText += "@vLogID,";
                            sqlText += "@ComputerName,";
                            sqlText += "@ComputerLoginUserName,";
                            sqlText += "@ComputerIPAddress,";
                            sqlText += "@SoftwareUserId,";
                            sqlText += "@SessionDate,";
                            sqlText += "@LogInDateTime,";
                            sqlText += " )";


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;

                            parameter = new SqlParameter("@vLogID", SqlDbType.VarChar, 50);
                            parameter.Value = vLogID;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerName;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerLoginUserName;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerIPAddress;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                            parameter.Value = Item.SoftwareUserId;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                            parameter.Value = Item.SessionDate;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                            parameter.Value = Item.LogInDateTime;
                            cmdInsert.Parameters.Add(parameter);

                            transResult = (int)cmdInsert.ExecuteNonQuery();
                            #endregion Insert new user
                            #region Commit

                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();

                                    retResults = "" + vLogID;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";

                                }

                            }
                            else
                            {
                                retResults = "0";
                            }

                            #endregion Commit
                        }

                    }
                    #endregion IfSameDatabase
                    #region IfNotSameDatabase
                    else
                    {

                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName=@ComputerName";
                        sqlText += " and ComputerLoginUserName=@ComputerLoginUserName";
                        sqlText += " and ComputerIPAddress=@ComputerIPAddress";
                        sqlText += " and SoftwareUserId=@SoftwareUserId";
                        sqlText += " and SessionDate=@SessionDate";
                        sqlText += " and LogInDateTime=@LogInDateTime";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;

                        SqlParameter parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerName;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerLoginUserName;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerIPAddress;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                        parameter.Value = Item.SoftwareUserId;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                        parameter.Value = Item.SessionDate;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                        parameter.Value = Item.LogInDateTime;
                        cmdFindId.Parameters.Add(parameter);

                        int IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist > 0) //update
                        {
                            #region Update

                            sqlText = "";
                            sqlText += " UPDATE UserAuditLogs";
                            sqlText += " SET LogOutDateTime = @LogOutDateTime";
                            sqlText += " WHERE ComputerName=@ComputerName";
                            sqlText += " and ComputerLoginUserName=@ComputerLoginUserName";
                            sqlText += " and ComputerIPAddress=@ComputerIPAddress";
                            sqlText += " and SoftwareUserId=@SoftwareUserId";
                            sqlText += " and SessionDate=@SessionDate";
                            sqlText += " and LogInDateTime=@LogInDateTime";


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerName;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerLoginUserName;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerIPAddress;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                            parameter.Value = Item.SoftwareUserId;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                            parameter.Value = Item.SessionDate;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                            parameter.Value = Item.LogInDateTime;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@LogOutDateTime", SqlDbType.DateTime);
                            parameter.Value = Item.LogOutDateTime;
                            cmdInsert.Parameters.Add(parameter);


                            transResult = (int)cmdInsert.ExecuteNonQuery();

                            #region Commit

                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();

                                    retResults = "" + Item.LogID;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";

                                }

                            }
                            else
                            {
                                retResults = "0";
                            }

                            #endregion Commit

                            #endregion Insert new user
                        }

                    }
                    #endregion IfSameDatabase

                }

            }
            #region catch finally

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }
        public string InsertUserLogOutByList(List<UserLogsVM> Details, string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                string vDatabaseName = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog

                commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction); //tablename,fieldName, datatype

                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);

                #endregion UserLog

                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                foreach (UserLogsVM Item in Details.ToList())
                {
                    if (Item.DataBaseName == vDatabaseName)
                    {

                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName=@ComputerName";
                        sqlText += " and ComputerLoginUserName=@ComputerLoginUserName";
                        sqlText += " and ComputerIPAddress=@ComputerIPAddress";
                        sqlText += " and SoftwareUserId=@SoftwareUserId";
                        sqlText += " and SessionDate=@SessionDate";
                        sqlText += " and LogInDateTime=@LogInDateTime";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;

                        SqlParameter parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerName;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerLoginUserName;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                        parameter.Value = Item.ComputerIPAddress;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                        parameter.Value = Item.SoftwareUserId;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                        parameter.Value = Item.SessionDate;
                        cmdFindId.Parameters.Add(parameter);
                        parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                        parameter.Value = Item.LogInDateTime;
                        cmdFindId.Parameters.Add(parameter);


                        int IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist > 0) //update
                        {
                            #region Update

                            sqlText = "";
                            sqlText += " UPDATE UserAuditLogs";
                            sqlText += " SET LogOutDateTime = @LogOutDateTime";
                            sqlText += " WHERE ComputerName=@ComputerName";
                            sqlText += " and ComputerLoginUserName=@ComputerLoginUserName";
                            sqlText += " and ComputerIPAddress=@ComputerIPAddress";
                            sqlText += " and SoftwareUserId=@SoftwareUserId";
                            sqlText += " and SessionDate=@SessionDate";
                            sqlText += " and LogInDateTime=@LogInDateTime";


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;

                            parameter = new SqlParameter("@ComputerName", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerName;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerLoginUserName", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerLoginUserName;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@ComputerIPAddress", SqlDbType.VarChar, 200);
                            parameter.Value = Item.ComputerIPAddress;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@SoftwareUserId", SqlDbType.VarChar, 200);
                            parameter.Value = Item.SoftwareUserId;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@SessionDate", SqlDbType.DateTime);
                            parameter.Value = Item.SessionDate;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@LogInDateTime", SqlDbType.DateTime);
                            parameter.Value = Item.LogInDateTime;
                            cmdInsert.Parameters.Add(parameter);
                            parameter = new SqlParameter("@LogOutDateTime", SqlDbType.DateTime);
                            parameter.Value = Item.LogOutDateTime;
                            cmdInsert.Parameters.Add(parameter);

                            transResult = (int)cmdInsert.ExecuteNonQuery();

                            #region Commit

                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();

                                    retResults = "" + Item.LogID;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";

                                }

                            }
                            else
                            {
                                retResults = "0";
                            }

                            #endregion Commit

                            #endregion Insert new user
                        }
                    }
                }



            }
            #region catch finally

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }

        public string InsertUserLogOut(string LogID, string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {


                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                #region Update

                sqlText = "";
                sqlText += " UPDATE UserAuditLogs";
                sqlText += " SET LogOutDateTime = '" + LogOutDateTime + "'";
                sqlText += " WHERE LogID='" + LogID + "'";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                        retResults = "" + LogID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults = "0";

                    }

                }
                else
                {
                    retResults = "0";
                }

                #endregion Commit

                #endregion Insert new user

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }
        public DataTable SearchUserLog(string ComputerLoginUserName, string SoftwareUserName, string ComputerName, string StartDate, string EndDate, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchtUserLog");

            #endregion

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

                sqlText = @"SELECT 
LogID,
SoftwareUserId ,
ui.UserName SoftwareUserName,
convert (DATETIME,SessionDate,101)SessionDate,
convert (DATETIME,LogInDateTime,101)LogInDateTime,
convert (DATETIME,isnull(LogOutDateTime,'1900/01/01'),101)LogOutDateTime,
ComputerName,
ComputerLoginUserName,
ComputerIPAddress
FROM UserAuditLogs ul 
LEFT OUTER JOIN UserInformations ui  ON ul.SoftwareUserId=ui.UserID 
                            WHERE 
                            (ComputerLoginUserName LIKE '%' + @ComputerLoginUserName	 + '%' OR @ComputerLoginUserName	 IS NULL) 
                            AND (ui.UserName LIKE '%' + @SoftwareUserName + '%' OR @SoftwareUserName IS NULL)
                            AND (ComputerName LIKE '%' + @ComputerName + '%' OR @ComputerName IS NULL)
                            AND (LogInDateTime>= @StartDate OR @StartDate IS NULL)
                            AND (LogInDateTime <dateadd(d,1, @EndDate) OR @EndDate IS NULL)
                            order by username";

                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;

                if (!objCommUser.Parameters.Contains("@ComputerLoginUserName"))
                { objCommUser.Parameters.AddWithValue("@ComputerLoginUserName", ComputerLoginUserName); }
                else { objCommUser.Parameters["@ComputerLoginUserName"].Value = ComputerLoginUserName; }

                if (!objCommUser.Parameters.Contains("@SoftwareUserName"))
                { objCommUser.Parameters.AddWithValue("@SoftwareUserName", SoftwareUserName); }
                else { objCommUser.Parameters["@SoftwareUserName"].Value = SoftwareUserName; }

                if (!objCommUser.Parameters.Contains("@ComputerName"))
                { objCommUser.Parameters.AddWithValue("@ComputerName", ComputerName); }
                else { objCommUser.Parameters["@ComputerName"].Value = ComputerName; }

                if (!objCommUser.Parameters.Contains("@StartDate"))
                { objCommUser.Parameters.AddWithValue("@StartDate", StartDate); }
                else { objCommUser.Parameters["@StartDate"].Value = StartDate; }

                if (!objCommUser.Parameters.Contains("@EndDate"))
                { objCommUser.Parameters.AddWithValue("@EndDate", EndDate); }
                else { objCommUser.Parameters["@EndDate"].Value = EndDate; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

            return dataTable;

        }

        public string[] UpdateUserInformation(string UserID, string UserName, string FullName, string Designation
              , string ContactNo
              , string Email
            , string Address
            , string ActiveStatus, string IsMainSettings, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null, string NationalID = null, bool IsLock = false, byte[] bimage = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(UserID))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user id.");
                }
                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user name.");
                }
                //if (string.IsNullOrEmpty(UserPassword))
                //{
                //    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user password.");
                //}

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("Update To User Information New");

                #endregion open connection and transaction

                #region UserID existence checking by id

                //select @Present = count(UserID) from UserInformations where  UserID=@UserID;
                sqlText = "select count(UserID) from UserInformations where  UserID = @UserID ";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                userIDExist.Parameters.AddWithValueAndNullHandle("@UserID", UserID);
                countId = (int)userIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Could not find requested user id.");
                }

                #endregion UserID existence checking by id

                #region NationalID existence checking

                //select @Present = count(distinct UserName) from UserInformations where  UserName=@UserName;
                sqlText = "select count(distinct NationalID) from UserInformations where  NationalID = @NationalID and UserID!=@UserID";
                SqlCommand nationalidExist = new SqlCommand(sqlText, currConn);
                nationalidExist.Parameters.AddWithValueAndNullHandle("@NationalID", NationalID);
                nationalidExist.Parameters.AddWithValueAndNullHandle("@UserID", UserID);

                nationalidExist.Transaction = transaction;
                countId = (int)nationalidExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Same National ID already exist.");
                }

                #endregion NationalID existence checking

                #region UserName existence checking by id and requied field

                //sqlText = "select count(UserName) from UserInformations ";
                //sqlText += " where  UserID='" + UserID + "'";
                //sqlText += " and UserName ='" + UserName + "'";
                //SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                //userNameExist.Transaction = transaction;
                //countId = (int)userNameExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("UpdateToUserInformationNew", "Same user name already exist.");
                //}

                #endregion UserName existence checking by id and requied field

                #region Update user

                sqlText = "";
                sqlText = " update UserInformations set";
                sqlText += " FullName=@FullName";
                sqlText += " ,Designation=@Designation";
                sqlText += " ,ContactNo=@ContactNo";
                sqlText += " ,Email=@Email";
                sqlText += " ,NationalID=@NationalID";
                sqlText += " ,Address=@Address";
                sqlText += " ,ActiveStatus=@ActiveStatus";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,IsMainSettings=@IsMainSettings";

                sqlText += " ,IsLock=@IsLock";
                sqlText += " ,Signature=@Signature";
                sqlText += " where                UserID=@UserID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FullName", FullName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Designation", Designation);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ContactNo", ContactNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Email", Email);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@NationalID", NationalID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Address", Address);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ActiveStatus", ActiveStatus);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserID", UserID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsMainSettings", IsMainSettings);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsLock", IsLock);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Signature", bimage != null ? bimage : new byte[0]);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information Successfully Update.";
                        retResults[2] = "" + UserID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserID;
                }

                #endregion Commit

                #endregion Update user

            }
            #region catch

            catch (SqlException sqlex)
            {

                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {


                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }

        public string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Please enter user name.");
                }
                if (string.IsNullOrEmpty(UserPassword))
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Please enter user password.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToUserInformationNew");

                #endregion open connection and transaction



                #region Load Settings
                CommonDAL commonDal = new CommonDAL();

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(currConn, transaction, connVM);
                }
                UserAuditVM UserAudit = new UserAuditVM();

                UserAudit.MinimumLengthCheck = Convert.ToBoolean(commonDal.settingsCache("Password", "MinimumLengthCheck", settings).ToString().ToLower() == "y" ? true : false);
                UserAudit.MinimumLength = Convert.ToInt32(commonDal.settingsDesktop("Password", "MinimumLength", settings));
                UserAudit.MixPasswordCheck = Convert.ToBoolean(commonDal.settingsCache("Password", "MixPasswordCheck", settings).ToString().ToLower() == "y" ? true : false);
                UserAudit.ChangeDate = Convert.ToInt32(commonDal.settingsDesktop("Password", "ChangeDate", settings));
                UserAudit.MaxWrongLoginTime = Convert.ToInt32(commonDal.settingsDesktop("Password", "MaxWrongLoginTime", settings));

                UserAudit.UserPassword = Converter.DESDecrypt(PassPhrase, EnKey, UserPassword);





                #endregion

                #region Check Security Related issues
                UserValidationResult result = Validate(UserAudit);
                if (!result.IsValid)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", result.ErrorMessage);
                }
                #endregion
                #region UserName existence checking by id


                //select @Present = count(UserID) from UserInformations where  UserID=@UserID;
                sqlText = "select count(UserName) from UserInformations where  UserName = @UserName";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                userIDExist.Parameters.AddWithValueAndNullHandle("@UserName", UserName);
                countId = (int)userIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Could not find requested user name.");
                }

                #endregion UserName existence checking by id

                #region Update user

                sqlText = "";
                sqlText = "update UserInformations set";
                sqlText += " UserPassword=@UserPassword";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,LastPasswordChangeDate=@LastPasswordChangeDate";
                sqlText += " where  UserName=@UserName";



                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserName", UserName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserPassword", UserPassword);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastPasswordChangeDate", LastModifiedOn);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Password Information Successfully Update.";
                        retResults[2] = "" + UserName;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserName;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserName;
                }

                #endregion Commit

                #endregion Update user

            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {


                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;

        }

        //==================Search User=================
        /// <summary>
        /// Search User with separate SQL
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="ActiveStatus"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataTable SearchUserDataTable(string UserID, string UserName, string ActiveStatus, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("User Search");

            #endregion

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

                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword,isnull(FullName,'NA')FullName
,isnull(Designation,'NA')Designation
,isnull(ContactNo,'NA')ContactNo
,isnull(Email,'NA')Email
,isnull(NationalID,'NA')NationalID
,isnull(Address,'NA')Address
,isnull(IsMainSettings,'Y')IsMainSettings
,isnull(IsLock,0)IsLock,Signature
                            FROM UserInformations
                            WHERE 
                            (UserID LIKE '%' + @UserID	 + '%' OR @UserID	 IS NULL) 
                            AND (UserName LIKE '%' + @UserName + '%' OR @UserName IS NULL)
                            AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by username";

                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;

                if (!objCommUser.Parameters.Contains("@UserID"))
                { objCommUser.Parameters.AddWithValue("@UserID", UserID); }
                else { objCommUser.Parameters["@UserID"].Value = UserID; }
                if (!objCommUser.Parameters.Contains("@UserName"))
                { objCommUser.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUser.Parameters["@UserName"].Value = UserName; }
                if (!objCommUser.Parameters.Contains("@ActiveStatus"))
                { objCommUser.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommUser.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

            return dataTable;

        }

        //==================Search User Has=================
        /// <summary>
        /// Search User Has with separate SQL
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataTable SearchUserHasNew(string UserName, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchUserHas");

            #endregion

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

                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword
                            FROM         UserInformations                
                            WHERE (UserName = @UserName )
                            order by username";

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;

                if (!objCommBankInformation.Parameters.Contains("@UserName"))
                { objCommBankInformation.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommBankInformation.Parameters["@UserName"].Value = UserName; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

            return dataTable;

        }

        #endregion

        public string[] InsertToUserRoll(List<UserRollVM> userRollVMs, string databaseName, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string userId = "";


            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserRoll");

                #endregion open connection and transaction

                #region id existence checking

                if (userRollVMs.Any())
                {
                    foreach (UserRollVM item in userRollVMs)
                    {
                        if (!string.IsNullOrEmpty(item.UserID))
                        {
                            userId = item.UserID;
                        }
                        break;
                    }

                }

                sqlText = "select  count(FormID) from UserRolls where  UserID = @userId";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValueAndNullHandle("@userId", userId);
                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId > 0)
                {
                    sqlText = "delete from UserRolls where  UserID = @UserId";
                    SqlCommand cmdIdExist1 = new SqlCommand(sqlText, currConn);
                    cmdIdExist1.Transaction = transaction;
                    cmdIdExist1.Parameters.AddWithValueAndNullHandle("@UserId", userId);

                    cmdIdExist1.ExecuteScalar();

                }

                #endregion

                if (userRollVMs.Any())
                {
                    int j = 0;
                    foreach (UserRollVM item in userRollVMs)
                    {
                        Debug.WriteLine(j.ToString());
                        j++;
                        #region Update Settings
                        sqlText = "";
                        //sqlText += "declare @Present numeric";
                        //sqlText +=" select @Present = count(FormID) from UserRolls ";
                        //sqlText += " where  UserID = '" + item.UserID + "' and FormID='" + item.FormID + "'; ";
                        //                sqlText +=" if(@Present <=0 ) ";
                        //                sqlText +=" BEGIN ";
                        sqlText += " insert into UserRolls( ";
                        sqlText += " UserID, ";
                        sqlText += " FormID, ";
                        sqlText += " Access, ";
                        sqlText += " CreatedBy, ";
                        sqlText += " CreatedOn, ";
                        sqlText += " LastModifiedBy, ";
                        sqlText += " LastModifiedOn, ";
                        sqlText += " AddAccess,EditAccess, ";
                        sqlText += " LineID,FormName,PostAccess) ";

                        sqlText += " values( ";

                        sqlText += " @UserID, ";
                        sqlText += " @FormID, ";
                        sqlText += " @Access, ";
                        sqlText += " @CreatedBy, ";
                        sqlText += " @CreatedOn, ";
                        sqlText += " @LastModifiedBy, ";
                        sqlText += " @LastModifiedOn, ";
                        sqlText += " @AddAccess, ";
                        sqlText += " @EditAccess, ";
                        sqlText += " @LineID, ";
                        sqlText += " @FormName, ";
                        sqlText += " @PostAccess ) ";    


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UserID", item.UserID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FormID", item.FormID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Access", item.Access);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", item.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", item.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", item.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", item.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@AddAccess", item.AddAccess);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@EditAccess", item.EditAccess);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LineID", item.LineID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FormName", item.FormName);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PostAccess", item.PostAccess);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                        #endregion Update Settings
                    }

                }
                else
                {
                    throw new ArgumentNullException("InsertToUserRoll", "Could not found any item.");
                }

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Roll Information Successfully Updated.";
                    }

                }

                #endregion Commit

            }
            #region catch

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string SearchUserRoll(string UserID, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            //string userRoll = string.Empty;
            string decryptedData = string.Empty;
            string encriptedData = string.Empty;


            try
            {
                #region open connection

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection
                #region MyRegion

                CommonDAL commonDal = new CommonDAL();
                int insertCol = 0;
                SqlTransaction transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                insertCol = commonDal.TableFieldAdd("UserRolls", "AddAccess", "varchar(1)", currConn, transaction);
                insertCol = commonDal.TableFieldAdd("UserRolls", "EditAccess", "varchar(1)", currConn, transaction);

                if (insertCol < 0)
                {
                    transaction.Commit();
                }

                #endregion

                sqlText = @"
                SELECT LineID,UserID,FormID,isnull(Access,'N')Access,isnull(PostAccess,'N')PostAccess,isnull(AddAccess,'N')AddAccess,isnull(EditAccess,'N')EditAccess 
                FROM dbo.UserRolls

                WHERE (UserID  = @UserID ) 
                order by UserID,LineID
                ";

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;


                //objCommUser.CommandText = sqlText;
                //objCommUser.CommandType = CommandType.Text;


                if (!objCommBankInformation.Parameters.Contains("@UserID"))
                { objCommBankInformation.Parameters.AddWithValue("@UserID", UserID); }
                else { objCommBankInformation.Parameters["@UserID"].Value = UserID; }

                SqlDataReader reader = objCommBankInformation.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        decryptedData = decryptedData + FieldDelimeter + reader[j].ToString();
                    }
                    decryptedData = decryptedData + LineDelimeter;
                }
                reader.Close();

                encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, decryptedData);
                //return decryptedData;
                return encriptedData;

            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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


            return encriptedData;
        }

        public DataTable SearchUserHas(string UserName, SysDBInfoVMTemp connVM = null, string password = "")
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable("UserHas");
            bool SuccessResult = false;
            CommonDAL commonDal = new CommonDAL();
            string[] retResults = new string[3];
            try
            {
                #region open connection



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

                #endregion open connection
                #region AD Check
                string IsAdCheck = commonDal.settingsMaster("User", "IsAdCheck", currConn, transaction);

                if (!string.IsNullOrEmpty(IsAdCheck) && IsAdCheck == "Y")
                {
                    retResults = CheckADAuth(UserName, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("User", retResults[1].ToString());
                    }
                    retResults = CheckADAPI(UserName, password, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("User", retResults[1].ToString());
                    }
                }

                #endregion AD Check

                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword,GETDATE() ServerDate,isnull(IsLock,0)IsLock,isnull([LastPasswordChangeDate],GETDATE())[LastPasswordChangeDate]
                            FROM         UserInformations                
                            WHERE (UserName = @UserName )
                            order by username";

                SqlCommand objCommUI = new SqlCommand();
                objCommUI.Connection = currConn;
                objCommUI.Transaction = transaction;
                objCommUI.CommandText = sqlText;
                objCommUI.CommandType = CommandType.Text;


                if (!objCommUI.Parameters.Contains("@UserName"))
                { objCommUI.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUI.Parameters["@UserName"].Value = UserName; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUI);
                dataAdapter.Fill(dt);

                SuccessResult = true;



                if (transaction != null)
                {
                    if (SuccessResult)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                }

            }
            #region catch and final
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            #endregion catch and final

            return dt;
        }

        private string[] CheckADAPI(string userName, string password, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Message";
            retResults[2] = "";
            try
            {
                CommonDAL commonDal = new CommonDAL();


                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || userName.ToLower() == "admin")
                {
                    retResults[1] = "User not in AD";
                    return retResults;
                }
                string url = commonDal.settingsMaster("User", "AdAPI", VcurrConn, Vtransaction);
                if (string.IsNullOrEmpty(url))
                {
                    retResults[1] = "No AdAPI Url Found";
                    return retResults;
                }
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";


                byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { userName, password }));
                request.ContentLength = byteArray.Length;

                request.ContentType = "application/json;charset=UTF-8";

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);

                string responseMessage = reader.ReadToEnd();

                reader.Close();

                ADAPIResult result = JsonConvert.DeserializeObject<ADAPIResult>(responseMessage);

                if (!result.IsValid)
                {
                    retResults[1] = "User's AD authentication failed";
                    //throw new Exception("User's AD authentication failed");
                }
                else
                {
                    retResults[0] = "Success";
                    retResults[1] = "User's AD authentication Success";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return retResults;

        }

        private string[] CheckADAuth(string UserName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Message";
            retResults[2] = "";
            try
            {
                CommonDAL commonDal = new CommonDAL();

                string AdUrl = commonDal.settingsMaster("User", "AdUrl", VcurrConn, Vtransaction);
                if (string.IsNullOrWhiteSpace(AdUrl))
                {
                    retResults[1] = "AD Url not Exist";
                    return retResults;
                }
                if (string.Equals(UserName, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    retResults[1] = "AD User not Exist";
                    return retResults;
                }
                if (CheckUserinAD(AdUrl, UserName))
                {
                    PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, AdUrl);
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, UserName);

                    if (userPrincipal != null)
                    {
                        DirectoryEntry dirEntry = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                        bool status = IsAccountDisabled(dirEntry);

                        if (status)
                        {
                            retResults[1] = "User has been disabled";
                            return retResults;
                        }
                    }
                }
                else
                {
                    retResults[1] = "User not found in AD";
                    return retResults;
                }
            }
            catch (Exception e)
            {
                FileLogger.Log("UserinfoDAl", "CheckADAuth", e.ToString());

                throw;
            }
            return retResults;
        }

            #endregion

        #region web methods
        public List<UserInformationVM> SelectForLogin(LoginVM Logvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            //From Web
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UserInformationVM> VMs = new List<UserInformationVM>();
            UserInformationVM vm;
            CommonDAL commonDal = new CommonDAL();
            string[] retResults = new string[3];
            #endregion
            try
            {

                //CheckADAuth(Logvm.UserName);
                //CheckADAPI(Logvm.UserName,Logvm.UserPassword);

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

                    currConn = _dbsqlConnection.GetConnectionForLogin(Logvm.DatabaseName);
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

                #region AD Check
                string IsAdCheck = commonDal.settingsMaster("User", "IsAdCheck", currConn, transaction);

                if (!string.IsNullOrEmpty(IsAdCheck) && IsAdCheck == "Y")
                {
                    retResults = CheckADAuth(Logvm.UserName, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("User", retResults[1].ToString());
                    }
                    retResults = CheckADAPI(Logvm.UserName, Logvm.UserPassword, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("User", retResults[1].ToString());
                    }
                }

                #endregion AD Check

                #region sql statement
                #region SqlText

                sqlText = @"
    SELECT top 1

UserID
,UserName
,FullName
,Designation
,UserPassword
,ActiveStatus
,isnull(IsLock,0)IsLock
,isnull([LastPasswordChangeDate],GETDATE())[LastPasswordChangeDate]
    FROM UserInformations 
    WHERE  1=1    and ActiveStatus='Y' and   UserName=@UserName   and UserPassword=@UserPassword

";


                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@UserName", Logvm.UserName);
                Logvm.UserPassword = Converter.DESEncrypt(PassPhrase, EnKey, Logvm.UserPassword);

                objComm.Parameters.AddWithValue("@UserPassword", Logvm.UserPassword);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UserInformationVM();
                    vm.UserID = dr["UserID"].ToString();
                    vm.UserName = dr["UserName"].ToString();
                    vm.FullName = dr["FullName"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.UserPassword = dr["UserPassword"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.IsLock = Convert.ToBoolean(dr["IsLock"].ToString());
                    vm.LastPasswordChangeDate = dr["LastPasswordChangeDate"].ToString();
                    //vm.IsAdmin = dr["IsAdmin"].ToString() == "Y" ? true : false;

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

            catch (Exception ex)
            {
                FileLogger.Log("UserinfoDAl", "SelectForLogin", ex.ToString());

                throw;
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

        public List<UserInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UserInformationVM> VMs = new List<UserInformationVM>();
            UserInformationVM vm;
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
 ui.UserID
,ui.UserName
,ui.UserPassword
,ui.ActiveStatus
,ui.LastLoginDateTime
,ui.CreatedBy
,ui.CreatedOn
,ui.LastModifiedBy
,ui.LastModifiedOn
,ui.FullName
,ui.Designation
,ui.ContactNo
,ui.Address
,ui.Email
,ui.ContactNo Mobile
,ui.NationalId NationalId
,isnull(ui.IsMainSettings,'N')IsMainSettings
,ui.GroupID
,ug.GroupName

FROM UserInformations ui left outer join UserGroups ug on ui.GroupID=ug.GroupID
WHERE  1=1 

";
                if (Id > 0)
                {
                    sqlText += @" and ui.UserID=@UserID";
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

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@UserID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UserInformationVM();
                    vm.UserID = dr["UserID"].ToString();
                    vm.UserName = dr["UserName"].ToString();
                    vm.UserPassword = dr["UserPassword"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.LastLoginDateTime = dr["LastLoginDateTime"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.GroupID = dr["GroupID"].ToString();
                    vm.GroupName = dr["GroupName"].ToString();
                    vm.FullName = dr["FullName"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    //vm.ContactNo = dr["ContactNo"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.NationalId = dr["NationalId"].ToString();
                    vm.IsMainSettings = dr["IsMainSettings"].ToString();




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

        public string[] InsertToUserInformationNew(UserInformationVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UserName))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user name.");
                }
                if (string.IsNullOrEmpty(vm.UserPassword))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user password.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                #region UserName existence checking

                //select @Present = count(distinct UserName) from UserInformations where  UserName=@UserName;
                sqlText = "select count(distinct UserName) from UserInformations where  UserName =@UserName ";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                userNameExist.Parameters.AddWithValue("@UserName", vm.UserName);

                countId = (int)userNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Same user name already exist.");
                }

                #endregion UserName existence checking

                #region User new id generation

                //select @UserID= isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations;
                sqlText = "select isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                }

                #endregion User new id generation

                #region Insert new user

                sqlText = "";
                sqlText += "insert into UserInformations";
                sqlText += "(";
                sqlText += "UserID,";
                sqlText += "UserName,";
                sqlText += "UserPassword,";
                sqlText += "ActiveStatus,";
                sqlText += "LastLoginDateTime,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@UserName,";
                sqlText += "@UserPassword,";
                sqlText += "@ActiveStatus,";
                sqlText += "@LastLoginDateTime,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@UserName", vm.UserName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UserPassword", vm.UserPassword ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                cmdInsert.Parameters.AddWithValue("@LastLoginDateTime", vm.LastLoginDateTime ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information successfully Added.";
                        retResults[2] = "" + nextId;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add user";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add user ";
                    retResults[2] = "";
                }

                #endregion Commit

                #endregion Insert new user

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw sqlex;
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

            return retResults;
        }

        public List<UserInformationVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<UserInformationVM> VMs = new List<UserInformationVM>();
            UserInformationVM vm;
            #endregion
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
                sqlText = @"
SELECT
 ui.UserID
,ui.UserName
FROM UserInformations ui WHERE  1=1 AND ui.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UserInformationVM();
                    vm.UserID = dr["UserID"].ToString();
                    vm.UserName = dr["UserName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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

        #endregion

        public bool CheckUserinAD(string domain, string username)
        {
            using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domain))
            {
                using (UserPrincipal user = new UserPrincipal(domainContext))
                {
                    user.SamAccountName = username;

                    using (PrincipalSearcher pS = new PrincipalSearcher())
                    {
                        pS.QueryFilter = user;

                        using (PrincipalSearchResult<Principal> results = pS.FindAll())
                        {
                            if (results != null && results.Count() > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public DataTable UserSecurityLog(SqlConnection Vcon = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection con = null;
            SqlTransaction transaction = null;

            string sqlselect = "";
            DataTable dt = new DataTable();
            //            try
            //            {
            //                if (Vcon != null)
            //                {
            //                    con = Vcon;

            //                }
            //                if (Vtransaction != null)
            //                {
            //                    transaction = Vtransaction;
            //                }

            //                if (con == null)
            //                {
            //                    con = _dbsqlConnection.GetConnection();
            //                    if (con.State != ConnectionState.Open)
            //                    {
            //                        con.Open();
            //                    }
            //                }
            //                if (transaction == null)
            //                {
            //                    transaction = con.BeginTransaction();
            //                }



            //                SqlCommand cmd = new SqlCommand(sqlselect, con, transaction);

            //                SqlDataAdapter da = new SqlDataAdapter(cmd);
            //                da.Fill(dt);
            //                if (transaction != null && Vtransaction == null)
            //                {
            //                    transaction.Commit();
            //                }


            //            }
            //catch (Exception ex)
            // {
            //     if (transaction != null && Vtransaction == null)
            //     {
            //         transaction.Rollback();
            //     }
            //     throw new ArgumentNullException("", "SQL:" + sqlselect + FieldDelimeter + ex.Message.ToString());

            // }
            //finally
            //{
            //    if (Vcon == null)
            //    {
            //        if (con != null)
            //        {
            //            if (con.State == ConnectionState.Open)
            //            {
            //                con.Close();
            //            }
            //        }
            //    }

            // } 
            return dt;
        }

        public bool IsAccountDisabled(DirectoryEntry user)
        {
            const string uac = "userAccountControl";
            if (user.NativeGuid == null) return false;

            if (user.Properties[uac] != null && user.Properties[uac].Value != null)
            {
                UserFlags userFlags = (UserFlags)user.Properties[uac].Value;
                return userFlags.Contains(UserFlags.AccountDisabled);
            }

            return false;
        }

        public DataTable GetExcelData(List<string> ids, SqlConnection Vcon = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection con = null;
            SqlTransaction transaction = null;

            string sqlselect = "";

            DataTable dt = new DataTable();
            try
            {
                if (Vcon != null)
                {
                    con = Vcon;

                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (con == null)
                {
                    con = _dbsqlConnection.GetConnection();
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = con.BeginTransaction();
                }

                sqlselect = @"select UI.UserName, BP.BranchCode, BP.BranchName from UserInformations UI 
left outer join UserBranchDetails UD 
on UI.UserID = UD.UserId left outer join BranchProfiles BP
on BP.BranchID = UD.BranchId where UI.UserID in ";

                int len = ids.Count;


                sqlselect += "(";
                for (int i = 0; i < len; i++)
                {
                    sqlselect += "'" + ids[i] + "',";
                }

                sqlselect = sqlselect.TrimEnd(',') + ")";

                SqlCommand cmd = new SqlCommand(sqlselect, con, transaction);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }


            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlselect + FieldDelimeter + ex.Message.ToString());

            }
            finally
            {
                if (Vcon == null)
                {
                    if (con != null)
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }

            }

            return dt;



        }

        public class UserValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }

        public UserValidationResult Validate(UserAuditVM UserAudit)
        {



            if (UserAudit.MinimumLengthCheck)
            {
                if (UserAudit.UserPassword.Length < UserAudit.MinimumLength)
                {
                    return new UserValidationResult { IsValid = false, ErrorMessage = "Please Enter Password At Least " + UserAudit.MinimumLength + " Digit" };
                }
            }

            if (UserAudit.MixPasswordCheck)
            {
                if (!CheckPasswordCharacters(UserAudit.UserPassword))
                {
                    return new UserValidationResult { IsValid = false, ErrorMessage = "Please Enter Password At least one uppercase letter, one lowercase letter, one digit, and one special character." };
                }

            }

            return new UserValidationResult { IsValid = true, ErrorMessage = null };
        }

        public static bool CheckPasswordCharacters(string password)
        {
            // Check if the password contains at least one uppercase letter
            bool containsUppercase = password.Any(char.IsUpper);

            // Check if the password contains at least one lowercase letter
            bool containsLowercase = password.Any(char.IsLower);

            // Check if the password contains at least one digit
            bool containsDigit = password.Any(char.IsDigit);

            // Check if the password contains at least one special character
            bool containsSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            // Check if all character types are present
            return containsUppercase && containsLowercase && containsDigit && containsSpecialChar;
        }

        public void WorngAttemptProcess(bool IsUser = false, string UserName = "", SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            int WromgAttemptCount = 0;
            string sqlText = "";

            #endregion
            UserAuditVM UserAudit = new UserAuditVM();

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("WorngAttemptProcess");

                #endregion open connection and transaction

                if (!IsUser)
                {

                }
                else
                {
                    #region Load Settings
                    CommonDAL commonDal = new CommonDAL();

                    DataTable settings = new DataTable();
                    if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                    {
                        settings = settingVM.SettingsDTUser;
                    }
                    else
                    {
                        settings = new CommonDAL().SettingDataAll(currConn, transaction, connVM);
                    }

                    UserAudit.MaxWrongLoginTime = Convert.ToInt32(commonDal.settingsDesktop("Password", "MaxWrongLoginTime", settings) ?? "4");

                    #endregion

                    #region Wromg Attempt value Find

                    sqlText = "select ISNULL(SUM(ISNULL(WromgAttempt,0)),0) from UserInformations where  UserName = @User ";
                    SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                    userIDExist.Transaction = transaction;
                    userIDExist.Parameters.AddWithValueAndNullHandle("@User", UserName);
                    WromgAttemptCount = (int)userIDExist.ExecuteScalar();
                    WromgAttemptCount = WromgAttemptCount + 1;
                    #endregion Wromg Attempt value Find

                    #region Update user

                    sqlText = "";
                    sqlText = "update UserInformations set";
                    sqlText += " WromgAttempt=@WromgAttempt";
                    if (WromgAttemptCount > UserAudit.MaxWrongLoginTime)
                    {
                        sqlText += " ,IsLock=@IsLock";
                    }

                    sqlText += " where  UserName=@UserName";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserName", UserName);
                    cmdUpdate.Parameters.AddWithValueAndNullHandle("@WromgAttempt", WromgAttemptCount);

                    if (WromgAttemptCount > UserAudit.MaxWrongLoginTime)
                    {
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsLock", true);
                    }


                    transResult = (int)cmdUpdate.ExecuteNonQuery();

                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested User Information Successfully Update.";
                            retResults[2] = "" + UserName;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update user.";
                            retResults[2] = "" + UserName;
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user group";
                        retResults[2] = "" + UserName;
                    }

                    #endregion Commit

                    #endregion Update user
                }



            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            //return retResults;

        }

        public void UpdateUserWromgAttempt(string UserName = "", SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            string sqlText = "";

            #endregion

            try
            {


                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("WorngAttemptProcess");

                #endregion open connection and transaction

                #region Update user

                sqlText = "";
                sqlText = "update UserInformations set";
                sqlText += " WromgAttempt=@WromgAttempt";

                sqlText += " where  UserName=@UserName";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserName", UserName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@WromgAttempt", 0);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information Successfully Update.";
                        retResults[2] = "" + UserName;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserName;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserName;
                }

                #endregion Commit

                #endregion Update user

            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            //return retResults;

        }

        #region IVAS User Create

        public string[] InsertIVASUserInformation(UserInformationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UserName))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user name.");
                }

                //////Regex regexItem = new Regex("[^A-Za-z0-9]");

                //////if (regexItem.IsMatch(vm.UserName))
                //////{
                //////    throw new ArgumentNullException("InsertToUserInformationNew", "User name only Alphabets & Numeric.Space not allowed");
                //////}

                if (string.IsNullOrEmpty(vm.UserPassword))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user password.");
                }


                #endregion Validation

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
                    transaction = currConn.BeginTransaction("InsertIVASUserInformation");
                }
                #endregion open connection and transaction

                #region UserName existence checking

                sqlText = "select count(distinct UserName) from UserInformationIVAS where  UserName = @UserName";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                userNameExist.Parameters.AddWithValueAndNullHandle("@UserName", vm.UserName);
                countId = (int)userNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToIVASUserInformationNew", "Same user name already exist.");
                }

                #endregion UserName existence checking

                #region User new id generation

                sqlText = "select isnull(max(cast(UserID as int)),0)+1 FROM  UserInformationIVAS";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Insert To IVAS User Information New", "Unable to create new user");
                }

                #endregion User new id generation

                #region Insert new user

                sqlText = "";
                sqlText += "insert into UserInformationIVAS";
                sqlText += "(";
                sqlText += "UserID,";
                sqlText += "UserName,";
                sqlText += "UserPassword,";
                sqlText += "FullName,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@UserID,";
                sqlText += "@UserName,";
                sqlText += "@UserPassword,";
                sqlText += "@FullName,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserID", nextId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserName", vm.UserName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserPassword", vm.UserPassword);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FullName", vm.FullName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", vm.CreatedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert new user

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Requested User Information successfully Added.";
                    retResults[2] = "" + nextId;

                }

                #endregion Commit

            }
            #region catch

            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] UpdateIVASUserInformation(UserInformationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UserID))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user id.");
                }
                if (string.IsNullOrEmpty(vm.UserName))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user name.");
                }

                #endregion Validation

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
                    transaction = currConn.BeginTransaction("UpdateIVASUserInformation");
                }
                #endregion open connection and transaction

                #region UserID existence checking by id

                sqlText = "select count(UserID) from UserInformationIVAS where  UserID = @UserID ";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                userIDExist.Parameters.AddWithValueAndNullHandle("@UserID", vm.UserID);
                countId = (int)userIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToUserInformation", "Could not find requested user id.");
                }

                #endregion UserID existence checking by id

                #region Update user

                sqlText = "";
                sqlText = " update UserInformationIVAS set";
                sqlText += " FullName=@FullName";
                sqlText += " ,ActiveStatus=@ActiveStatus";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " where UserID=@UserID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FullName", vm.FullName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UserID", vm.UserID);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update user

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Requested User Information Successfully Update.";
                    retResults[2] = "" + vm.UserID;

                }

                #endregion Commit

            }
            #region catch

            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        public DataTable SearchIVASUserDataTable(string UserID, string UserName, string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("User Search");

            #endregion

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

                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword,isnull(FullName,'NA')FullName
                            FROM UserInformationIVAS
                            WHERE 
                            (UserID LIKE '%' + @UserID	 + '%' OR @UserID	 IS NULL) 
                            AND (UserName LIKE '%' + @UserName + '%' OR @UserName IS NULL)
                            AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by username";

                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;

                if (!objCommUser.Parameters.Contains("@UserID"))
                { objCommUser.Parameters.AddWithValue("@UserID", UserID); }
                else { objCommUser.Parameters["@UserID"].Value = UserID; }
                if (!objCommUser.Parameters.Contains("@UserName"))
                { objCommUser.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUser.Parameters["@UserName"].Value = UserName; }
                if (!objCommUser.Parameters.Contains("@ActiveStatus"))
                { objCommUser.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommUser.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;

        }

        public DataTable SearchIVASUserHas(string UserName, string password = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable("UserHas");
            bool SuccessResult = false;
            CommonDAL commonDal = new CommonDAL();
            string[] retResults = new string[3];
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
                    transaction = currConn.BeginTransaction("SearchIVASUser");
                }
                #endregion open connection and transaction

                sqlText = @"
SELECT 
UserID
,UserName
,ActiveStatus
,UserPassword                          
FROM         UserInformationIVAS                
WHERE (UserName = @UserName )
order by username
";

                SqlCommand objCommUI = new SqlCommand();
                objCommUI.Connection = currConn;
                objCommUI.Transaction = transaction;
                objCommUI.CommandText = sqlText;
                objCommUI.CommandType = CommandType.Text;


                if (!objCommUI.Parameters.Contains("@UserName"))
                { objCommUI.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUI.Parameters["@UserName"].Value = UserName; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUI);
                dataAdapter.Fill(dt);

                SuccessResult = true;

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }


            }
            #region catch and final

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {

                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }

            }
            #endregion catch and final

            return dt;
        }


        #endregion

    }
}
