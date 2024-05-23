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
    public class MPLTradeChallanDAL 
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();

        #endregion


        public string[] InsertToMPLTradeChallan(MPLTradeChallanVM MasterVM, SqlTransaction Vtransaction = null,
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
                          "select COUNT(Code) from MPLTradeChallan WHERE Code=@Code ";
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


                newIDCreate = commonDal.TransactionCode("Sale", "TradeChallan", "MPLTradeChallan",
                        "Code",
                        "TransactionDateTime", MasterVM.TransactionDateTime, MasterVM.BranchId.ToString(), currConn,
                        transaction);
               

               

                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.insert,
                        "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(Code) FROM MPLTradeChallan 
where Code = @Code and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@Code", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("MPLTradeChallanDAL", "Insert",
                        "Code " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText =
                    " select count(Code) from MPLTradeChallan where Code = @Code";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Code", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("MPLTradeChallanDAL", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.Code = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                retResults = MPLTradeChallanInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Update Data

                sqlText = "";
                sqlText +=
                    @" Update SalesInvoiceMPLHeaders  set IsUsedTradeChallan=1 WHERE  Id=@Id ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.SalesInvoiceRefId);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion


                
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

                FileLogger.Log("MPLTradeChallanDAL", "InsertToMPLBankDepositSlip", ex.ToString() + "\n" + sqlText);

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

        public string[] MPLTradeChallanInsertToMaster(MPLTradeChallanVM Master, SqlConnection VcurrConn = null,
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
                sqlText += " INSERT INTO MPLTradeChallan";
                sqlText += " (";
                sqlText += "  Code";
                sqlText += "  ,BranchId";
                sqlText += "  ,TransactionDateTime";
                sqlText += "  ,SalesInvoiceRefId";
                sqlText += "  ,AgainstSupplyOrderNo";
                sqlText += "  ,Consignee";
                sqlText += "  ,ContractOrATNo";
                sqlText += "  ,AgreementDate";
                sqlText += "  ,Post";
                sqlText += "  ,CreatedBy";
                sqlText += "  ,CreatedOn";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "   @Code";
                sqlText += "   ,@BranchId";
                sqlText += "   ,@TransactionDateTime";
                sqlText += "   ,@SalesInvoiceRefId";
                sqlText += "   ,@AgainstSupplyOrderNo";
                sqlText += "   ,@Consignee";
                sqlText += "   ,@ContractOrATNo";
                sqlText += "   ,@AgreementDate";
                sqlText += "   ,@Post";
                sqlText += "   ,@CreatedBy";
                sqlText += "   ,@CreatedOn";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);


                cmdInsert.Parameters.AddWithValueAndNullHandle("@Code", Master.Code);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceRefId", Master.SalesInvoiceRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AgainstSupplyOrderNo", Master.AgainstSupplyOrderNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Consignee", Master.Consignee);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ContractOrATNo", Master.ContractOrATNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AgreementDate", Master.AgreementDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", "N");
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

                FileLogger.Log("MPLTradeChallanDAL", "MPLBankDepositSlipInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] UpdateMPLTradeChallan(MPLTradeChallanVM MasterVM, SqlTransaction transaction,
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

                #region Update Data

                sqlText = "";
                sqlText +=
                    @" Update SalesInvoiceMPLHeaders  set IsUsedTradeChallan=0 WHERE  Id=@Id ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.PreviousSalesInvoiceRefId);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion



                #region Update Information in Header

                MasterVM.TransactionDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransactionDateTime);
                MasterVM.AgreementDate = OrdinaryVATDesktop.DateToDate(MasterVM.AgreementDate);

                retResults = UpdateTradeChallanToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #region Update Data SalesInvoiceMPLHeaders

                sqlText = "";
                sqlText +=
                    @" Update SalesInvoiceMPLHeaders  set IsUsedTradeChallan=1 WHERE  Id=@Id ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.SalesInvoiceRefId);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

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

                FileLogger.Log("MPLTradeChallanDAL", "TransReceiveMPLUpdate", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateTradeChallanToMaster(MPLTradeChallanVM Master, SqlConnection VcurrConn = null,
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
                sqlText += " update MPLTradeChallan SET  ";

                sqlText += " TransactionDateTime=@TransactionDateTime";
                sqlText += " ,AgreementDate=@AgreementDate";
                sqlText += " ,Consignee=@Consignee";
                sqlText += " ,ContractOrATNo=@ContractOrATNo";
                sqlText += " ,AgainstSupplyOrderNo=@AgainstSupplyOrderNo";
                sqlText += " ,SalesInvoiceRefId=@SalesInvoiceRefId";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";


                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", Master.TransactionDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@AgreementDate", Master.AgreementDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@AgainstSupplyOrderNo", Master.AgainstSupplyOrderNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Consignee", Master.Consignee);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ContractOrATNo", Master.ContractOrATNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SalesInvoiceRefId", Master.SalesInvoiceRefId);
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

                FileLogger.Log("MPLTradeChallanDAL", "UpdateMPLBankDepositSlipToMaster",
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

     
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,
            string SelectTop = "100")
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

 
   TRC.[Id]
      ,TRC.[Code]
      ,TRC.[BranchId]
      ,TRC.[TransactionDateTime]
      ,TRC.[SalesInvoiceRefId]
      ,SIH.[SalesInvoiceNo]
      ,TRC.[AgainstSupplyOrderNo]
      ,TRC.[Consignee]
      ,TRC.[ContractOrATNo]
      ,TRC.[AgreementDate]
      ,C.CustomerName
      ,C.CustomerCode
      ,Isnull(TRC.[Post],'N')Post
  FROM [MPLTradeChallan]  TRC
  left outer join SalesInvoiceMPLHeaders SIH on SIH.Id=TRC.SalesInvoiceRefId
  left outer join Customers C on SIH.CustomerID=C.CustomerID

WHERE  1=1 ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT COUNT(Id) RecordCount
FROM [MPLTradeChallan] TRC  WHERE  1=1 ";

               
                if (Id > 0)
                {
                    sqlTextParameter += @" AND TRC.Id=@Id";
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
                FileLogger.Log("MPLTradeChallanDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLTradeChallanDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<MPLTradeChallanVM> SelectAllList(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null,string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLTradeChallanVM> VMs = new List<MPLTradeChallanVM>();
            MPLTradeChallanVM vm;

            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, SelectTop);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new MPLTradeChallanVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.Code = dr["Code"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.TransactionDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransactionDateTime"].ToString());
                        vm.AgreementDate = OrdinaryVATDesktop.DateTimeToDate(dr["AgreementDate"].ToString());
                        vm.Post = dr["Post"].ToString();
                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.SalesInvoiceRefId = Convert.ToInt32(dr["SalesInvoiceRefId"].ToString());
                        vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                        vm.AgainstSupplyOrderNo = dr["AgainstSupplyOrderNo"].ToString();
                        vm.Consignee = dr["Consignee"].ToString();
                        vm.ContractOrATNo = dr["ContractOrATNo"].ToString();
                        

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

                FileLogger.Log("MPLTradeChallanDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLTradeChallanDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }


      

        public List<MPLTradeChallanDetilsVM> GetMPLCreditInvoiceList(MPLTradeChallanVM vm, string[] conditionFields = null, string[] conditionValues = null,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLTradeChallanDetilsVM> lst = new List<MPLTradeChallanDetilsVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("GetMPLCreditInvoiceList");

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



                sqlText = @"   Select
  SH.SalesInvoiceNo
 ,SH.Id
 ,FORMAT(SH.InvoiceDateTime,'dd-MMM-yyyy')InvoiceDateTime
 ,C.CustomerCode
 ,C.CustomerName
  ,isnull (SH.CustomerOrder,'-') AgainstSupplyOrderNo
 ,isnull (SH.Comments,'-') ContractOrATNo

  from SalesInvoiceMPLBankPayments  SBP

  left outer join SalesInvoiceMPLHeaders SH on SBP.SalesInvoiceMPLHeaderId =SH.Id
    left outer join Customers c on SH.CustomerId=c.CustomerID

  where SBP.ModeOfPayment='Credit'
  And SH.Post='Y'
  And isnull(SH.IsUsedTradeChallan,0)=0
";

                if (!string.IsNullOrWhiteSpace(vm.SearchField) && !string.IsNullOrWhiteSpace(vm.SearchValue))
                {
                    sqlText += @"
                         And SH.@SearchField=@SearchValue";

                    sqlText = sqlText.Replace("@SearchField", vm.SearchField);

                }
                if (!string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    sqlText += @"
                         And SH.InvoiceDateTime>=@DateFrom";
                }
                if (!string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    sqlText += @"
                         And SH.InvoiceDateTime<=@DateTo";
                }
                if (vm.BranchId>0)
                {
                    sqlText += @"
                         And SH.BranchId=@BranchId";
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
                if (vm.BranchId > 0)
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId.ToString());
                }
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLTradeChallanDetilsVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLTradeChallanDAL", "SearchTMPLBankDepositSlipDetailList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLTradeChallanDAL", "SearchTMPLBankDepositSlipDetailList",
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


        public List<MPLTradeChallanDetilsVM> GetMPLCreditItemList(MPLTradeChallanVM vm, string[] conditionFields = null, string[] conditionValues = null,
         SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<MPLTradeChallanDetilsVM> lst = new List<MPLTradeChallanDetilsVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("GetMPLCreditItemList");

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



                sqlText = @"  SELECT

     P.ProductName 
    , P.ProductCode
    , SID.Quantity
    , isnull (SIH.VehicleNo,'-')VehicleNo
    , isnull (SIH.VehicleType,'-')VehicleType


  FROM SalesInvoiceMPLDetails SID
  left outer join PurchaseInvoiceMPLHeaders SIH on SIH.Id=SID.SalesInvoiceMPLHeaderId

  left outer join Products P on SID.ItemNo=P.ItemNo
  where 1=1 
and SID.SalesInvoiceMPLHeaderId=@SalesInvoiceMPLHeaderId
";

               
                
               
                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);

              
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@SalesInvoiceMPLHeaderId", vm.Id.ToString());
               
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<MPLTradeChallanDetilsVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLTradeChallanDAL", "GetMPLCreditItemList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLTradeChallanDAL", "GetMPLCreditItemList",
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
        
        public string[] Delete(MPLTradeChallanVM vm, string[] ids, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }


    }
}
