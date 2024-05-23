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
    public class TanksMPLDAL : ITanksMPL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();

        #endregion


        public string[] TanksMPLInsert(TankMPLsVM MasterVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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


                #region Find TankCode Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(TankCode) from TankMPLs WHERE TankCode=@TankCode AND BranchId = @BranchId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@TankCode", MasterVM.TankCode);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@BranchId", MasterVM.BranchId);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find TankCode Exist


                #region ID generated completed,Insert new Information in Header

                retResults = TanksMPLInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


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
                retResults[2] = "" + MasterVM.TankCode;
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
                FileLogger.Log("SaleDAL", "SalesInsert", ex.ToString() + "\n" + sqlText);

            }
            finally
            {

                #region Comment 28 Oct 2020


                #endregion

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

        public string[] TanksMPLInsertToMaster(TankMPLsVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO TankMPLs";
                sqlText += " (";
                sqlText += " BranchId";
                sqlText += " ,TankCode";
                sqlText += " ,ItemNo";
                sqlText += " ,Comments";
                sqlText += " ,ActiveStatus";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,OwnerName";
                sqlText += " ,OwnerAddress";
                sqlText += " ,THeight";
                sqlText += " ,TIntDiameter";
                sqlText += " ,MaxLoadHeight";
                sqlText += " ,SafeLoadHeght";
                sqlText += " ,MaxLoadCap";
                sqlText += " ,QtyZeroLevelDip";


                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @BranchId";
                sqlText += "  ,@TankCode";
                sqlText += "  ,@ItemNo";
                sqlText += "  ,@Comments";
                sqlText += "  ,@ActiveStatus";
                sqlText += "  ,@CreatedBy";
                sqlText += "  ,@CreatedOn";
                sqlText += "  ,@LastModifiedBy";
                sqlText += "  ,@LastModifiedOn";
                sqlText += "  ,@OwnerName";
                sqlText += "  ,@OwnerAddress";
                sqlText += "  ,@THeight";
                sqlText += "  ,@TIntDiameter";
                sqlText += "  ,@MaxLoadHeight";
                sqlText += "  ,@SafeLoadHeght";
                sqlText += "  ,@MaxLoadCap";
                sqlText += "  ,@QtyZeroLevelDip";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TankCode", Master.TankCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", Master.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", Master.ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OwnerName", Master.OwnerName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OwnerAddress", Master.OwnerAddress);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@THeight", Master.THeight);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TIntDiameter", Master.TIntDiameter);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MaxLoadHeight", Master.MaxLoadHeight);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SafeLoadHeght", Master.SafeLoadHeght);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MaxLoadCap", Master.MaxLoadCap);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@QtyZeroLevelDip", Master.QtyZeroLevelDip);



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
                retResults[2] = "" + Master.TankCode;
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

                FileLogger.Log("SaleMPLDAL", "SalesMPLInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] TanksMPLUpdate(TankMPLsVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
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

                #region Find TankCode Exist
                int IDExist = 0;
                sqlText = "";
                sqlText = sqlText + "SELECT COUNT(TankCode) from TankMPLs WHERE Id !=@Id AND TankCode=@TankCode AND BranchId = @BranchId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.Id);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@TankCode", MasterVM.TankCode);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@BranchId", MasterVM.BranchId);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgFindExistID);
                }

                #endregion Find TankCode Exist

                #region Update Information in Header

                retResults = TanksMPLUpdateToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

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
                retResults[2] = "" + MasterVM.TankCode;
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
                FileLogger.Log("SaleDAL", "SalesInsert", ex.ToString() + "\n" + sqlText);

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

        public string[] TanksMPLUpdateToMaster(TankMPLsVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " update TankMPLs SET  ";

                sqlText += " BranchId=@BranchId";
                sqlText += " ,TankCode=@TankCode";
                sqlText += " ,ItemNo=@ItemNo";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,ActiveStatus=@ActiveStatus";
                sqlText += " ,CreatedBy=@CreatedBy";
                sqlText += " ,CreatedOn=@CreatedOn";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,OwnerName=@OwnerName";
                sqlText += " ,OwnerAddress=@OwnerAddress";
                sqlText += " ,THeight=@THeight";
                sqlText += " ,TIntDiameter=@TIntDiameter";
                sqlText += " ,MaxLoadHeight=@MaxLoadHeight";
                sqlText += " ,SafeLoadHeght=@SafeLoadHeght";
                sqlText += " ,MaxLoadCap=@MaxLoadCap";
                sqlText += " ,QtyZeroLevelDip=@QtyZeroLevelDip";


                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TankCode", Master.TankCode);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ItemNo", Master.ItemNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ActiveStatus", Master.ActiveStatus);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@OwnerName", Master.OwnerName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@OwnerAddress", Master.OwnerAddress);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@THeight", Master.THeight);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TIntDiameter", Master.TIntDiameter);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MaxLoadHeight", Master.MaxLoadHeight);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SafeLoadHeght", Master.SafeLoadHeght);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MaxLoadCap", Master.MaxLoadCap);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@QtyZeroLevelDip", Master.QtyZeroLevelDip);

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
                FileLogger.Log("SaleDAL", "SalesUpdateToMaster", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string ActiveStatus = "", string SelectTop = "100")
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


TD.Id,
TD.TankCode,
TD.BranchId,
ISNULL(B.BranchName,'') BranchName,
TD.ItemNo,
P.ProductName ItemName, 
TD.Comments,
TD.ActiveStatus,
TD.CreatedBy,
ISNULL(TD.CreatedOn,'') CreatedOn,
TD.LastModifiedBy,
ISNULL(TD.LastModifiedOn,'') LastModifiedOn
,TD.Comments
,TD.OwnerName
,TD.OwnerAddress
,ISNULL(TD.THeight,'0') THeight
,ISNULL(TD.TIntDiameter,'0') TIntDiameter
,ISNULL(TD.MaxLoadHeight,'0') MaxLoadHeight
,ISNULL(TD.SafeLoadHeght,'0') SafeLoadHeght
,ISNULL(TD.MaxLoadCap,'0') MaxLoadCap
,ISNULL(TD.QtyZeroLevelDip,'0') QtyZeroLevelDip

FROM TankMPLs TD
LEFT OUTER JOIN Products P ON P.ItemNo = TD.ItemNo
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = TD.BranchId
WHERE  1=1 ";
                #endregion SqlText

                sqlTextCount += @" 
SELECT count(TD.TankCode) RecordCount
FROM TankMPLs TD WHERE  1=1 ";

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
                    sqlText += " AND TD.TankCode IN (" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND TD.Id=@Id";
                }
                if (!string.IsNullOrEmpty(ActiveStatus))
                {
                    sqlTextParameter += @" AND TD.ActiveStatus=@ActiveStatus";
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

                sqlTextOrderBy += " order by TD.Id desc";

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
                if (!string.IsNullOrEmpty(ActiveStatus))
                {
                    da.SelectCommand.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
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
                FileLogger.Log("TanksMPLDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("TanksMPLDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<TankMPLsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string ActiveStatus = "", string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<TankMPLsVM> VMs = new List<TankMPLsVM>();
            TankMPLsVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids, ActiveStatus, SelectTop);
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new TankMPLsVM();

                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.BranchName = dr["BranchName"].ToString();
                        vm.ItemName = dr["ItemName"].ToString();
                        vm.TankCode = dr["TankCode"].ToString();
                        vm.ItemNo = dr["ItemNo"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.IsActive = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                        vm.Status = dr["ActiveStatus"].ToString() == "Y" ? "Active" : "Inactive";
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = OrdinaryVATDesktop.DateTimeToDate(dr["CreatedOn"].ToString());
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = OrdinaryVATDesktop.DateTimeToDate(dr["LastModifiedOn"].ToString());
                        vm.OwnerName = dr["OwnerName"].ToString();
                        vm.OwnerAddress = dr["OwnerAddress"].ToString();
                        vm.THeight = Convert.ToInt32(dr["THeight"].ToString());
                        vm.TIntDiameter = Convert.ToDecimal(dr["TIntDiameter"].ToString());
                        vm.MaxLoadHeight = Convert.ToDecimal(dr["MaxLoadHeight"].ToString());
                        vm.SafeLoadHeght = Convert.ToDecimal(dr["SafeLoadHeght"].ToString());
                        vm.MaxLoadCap = Convert.ToDecimal(dr["MaxLoadCap"].ToString());
                        vm.QtyZeroLevelDip = Convert.ToDecimal(dr["QtyZeroLevelDip"].ToString());

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

                FileLogger.Log("TanksMPLDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("TanksMPLDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            #endregion
            return VMs;
        }

        public List<TankMPLsVM> DropDown(int branchId, string ItemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<TankMPLsVM> VMs = new List<TankMPLsVM>();
            TankMPLsVM vm;
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
                sqlText = @" SELECT Id,TankCode,ItemNo FROM TankMPLs WHERE  1=1  AND ActiveStatus = 'Y' AND BranchId = @BranchId ";

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    sqlText += @" AND ItemNo = @ItemNo";
                }
                
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValueAndNullHandle("@BranchId", branchId);
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@ItemNo", ItemNo);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new TankMPLsVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.TankCode = dr["TankCode"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("TanksMPLDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TanksMPLDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(TankMPLsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

    }
}
