using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATServer.Ordinary;
using Newtonsoft.Json;

namespace VATServer.Library
{
    public class DisposeFinishDAL
    {
        #region Global Declarations

        CommonDAL _CommonDAL = new CommonDAL();

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion 

        #region Method List

        // Select
        // SelectVM
        // SelectDetail
        // SelectDetailVM

        // Insert (Master)
        // InsertDetail (Detail)

        // Update (Master, Detail)
        // Post (Master, Detail)
        // Multiple Post

        #endregion

        public DataTable Select(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            DataTable dt = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";

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

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
SELECT
 df.Id
,df.BranchId
,df.DisposeNo
,df.TransactionDateTime
,df.Post
,df.FinishItemNo
,pro.ProductCode
,pro.ProductName

,df.Quantity
,df.UOM
,df.UnitPrice
,df.OfferUnitPrice
,df.IsSaleable
,df.BOMId
,df.ShiftId
,df.ReferenceNo
,df.SerialNo
,df.ImportIDExcel
,df.Comments
,df.TransactionType
,df.IsSynced
,df.FiscalYear
,df.AppVersion
,df.CreatedBy
,df.CreatedOn
,df.LastModifiedBy
,df.LastModifiedOn
,df.PeriodId



FROM DisposeFinishs df
left outer join Products pro on df.FinishItemNo=pro.ItemNo
WHERE 1=1

";

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

                sqlTextOrderBy = " order by df.TransactionDateTime desc";

                #endregion

                #region Sql Execution

                sqlText = sqlText + " " + sqlTextParameter + sqlTextOrderBy;

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

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
                            cmd.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                #endregion SqlExecution

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "Select", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

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

        public List<DisposeFinishMasterVM> SelectVM(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Declarations

            List<DisposeFinishMasterVM> VMs = new List<DisposeFinishMasterVM>();

            #endregion

            #region try

            try
            {
                DataTable dt = new DataTable();

                dt = Select(conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string JSONString = JsonConvert.SerializeObject(dt);
                    VMs = JsonConvert.DeserializeObject<List<DisposeFinishMasterVM>>(JSONString);

                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "SelectVM", ex.ToString());

                throw ex;
            }
            #endregion

            #region finally

            finally { }
            #endregion

            return VMs;

        }

        public DataTable SelectDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            DataTable dt = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";

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

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
SELECT
 dfd.Id
,dfd.DisposeNo
,dfd.DisposeLineNo
,dfd.BranchId
,dfd.TransactionDateTime
,dfd.TransactionType
,dfd.ItemNo
,dfd.UOM
,dfd.UsedQuantity
,dfd.PurchaseNo
,dfd.PurchaseQuantity
,dfd.VATRate
,dfd.VATAmount
,dfd.RebateRate
,dfd.RebateVATAmount
,dfd.Post
,dfd.Comments
,dfd.IsSynced
,dfd.CreatedBy
,dfd.CreatedOn
,dfd.LastModifiedBy
,dfd.LastModifiedOn
,dfd.PeriodId

,pro.ProductCode
,pro.ProductName

FROM DisposeFinishDetails dfd
left outer join Products pro on dfd.ItemNo=pro.ItemNo
WHERE 1=1

";

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

                sqlTextOrderBy = " order by dfd.TransactionDateTime desc";

                #endregion

                #region Sql Execution

                sqlText = sqlText + " " + sqlTextParameter + sqlTextOrderBy;

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

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
                            cmd.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                #endregion SqlExecution

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "SelectDetail", ex.ToString());

                throw ex;
            }

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

        public List<DisposeFinishDetailVM> SelectDetailVM(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Declarations

            List<DisposeFinishDetailVM> VMs = new List<DisposeFinishDetailVM>();

            #endregion

            #region try

            try
            {
                DataTable dt = new DataTable();

                dt = SelectDetail(conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string JSONString = JsonConvert.SerializeObject(dt);
                    VMs = JsonConvert.DeserializeObject<List<DisposeFinishDetailVM>>(JSONString);

                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "SelectDetailVM", ex.ToString());

                throw ex;
            }

            #endregion

            #region finally

            finally 
            {

            }

            #endregion

            return VMs;

        }

        public ResultVM Insert(DisposeFinishMasterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region Fiscal Year Check

                string transactionDate = vm.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MM-dd");

                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {
                    #region Year Lock

                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where 1=1 and '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dt = new DataTable();
                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else if (dt.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dt.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dt.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }

                    #endregion

                    #region Year Not Exist

                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    cmd = new SqlCommand(sqlText, currConn, transaction);

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmd);
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
                    #endregion
                }

                #endregion

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DisposeNo) from DisposeFinishs  where DisposeNo=@DisposeNo ";
                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndNullHandle("@DisposeNo", vm.DisposeNo ?? Convert.DBNull);
                int IDExist = Convert.ToInt32(cmd.ExecuteScalar());

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgFindExistID);
                }

                #endregion

                #region ID Generation

                if (string.IsNullOrWhiteSpace(vm.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }


                if (vm.TransactionType == "Other")
                {
                    vm.DisposeNo = _CommonDAL.TransactionCode("Dispose", "Finish", "DisposeFinishs", "DisposeNo", "TransactionDateTime", vm.TransactionDateTime, vm.BranchId.ToString(), currConn, transaction);
                }
                if (vm.TransactionType == "DisposeTrading")
                {
                    vm.DisposeNo = _CommonDAL.TransactionCode("Dispose", "Trading", "DisposeTrading", "DisposeNo", "TransactionDateTime", vm.TransactionDateTime, vm.BranchId.ToString(), currConn, transaction);
                }

                string PeriodId = Convert.ToDateTime(vm.TransactionDateTime).ToString("MMyyyy");

                vm.PeriodId = Convert.ToInt32(PeriodId);

                #endregion

                #region Master Data

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
INSERT INTO DisposeFinishs
(
 BranchId
,DisposeNo
,TransactionDateTime
,ShiftId
,FinishItemNo
,Quantity
,UOM
,UnitPrice
,OfferUnitPrice
,IsSaleable
,BOMId
,ReferenceNo
,SerialNo
,ImportIDExcel
,Comments
,TransactionType
,Post
,IsSynced
,FiscalYear
,AppVersion
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,PeriodId
)
VALUES
(
@BranchId
,@DisposeNo
,@TransactionDateTime
,@ShiftId
,@FinishItemNo
,@Quantity
,@UOM
,@UnitPrice
,@OfferUnitPrice
,@IsSaleable
,@BOMId
,@ReferenceNo
,@SerialNo
,@ImportIDExcel
,@Comments
,@TransactionType
,@Post
,@IsSynced
,@FiscalYear
,@AppVersion
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@PeriodId
)

";



                #endregion

                #region Sql Execution

                cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd.Parameters.AddWithValue("@DisposeNo", vm.DisposeNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(vm.TransactionDateTime) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@ShiftId", vm.ShiftId ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@FinishItemNo", vm.FinishItemNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Quantity", vm.Quantity);
                cmd.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);

                cmd.Parameters.AddWithValue("@UnitPrice", vm.UnitPrice);
                cmd.Parameters.AddWithValue("@OfferUnitPrice", vm.OfferUnitPrice);
                cmd.Parameters.AddWithValue("@IsSaleable", vm.IsSaleable ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@BOMId", vm.BOMId);
                cmd.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@ImportIDExcel", vm.ImportIDExcel ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Post", vm.Post ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@IsSynced", vm.IsSynced ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@AppVersion", vm.AppVersion ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@PeriodId", vm.PeriodId);

                executionResult = cmd.ExecuteNonQuery();


                #endregion SqlExecution

                #endregion

                #endregion

                #region Detail Data

                if (vm.Details != null && vm.Details.Count > 0)
                {
                    rVM = InsertDetail(vm, currConn, transaction, connVM);
                }

                #endregion


                #region Update PeriodId

                UpdatePeriodId(vm, currConn, transaction, out sqlText);

                #endregion


                #region Master Setting Update

                CommonDAL commonDal = new CommonDAL();
                commonDal.UpdateProcessFlag(vm.DisposeNo, vm.TransactionDateTime, currConn, transaction);

                #endregion


                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Data Saved Successfully!";

                rVM.InvoiceNo = vm.DisposeNo;
                rVM.Post = vm.Post;

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "Insert", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return rVM;
        }

        private void UpdatePeriodId(DisposeFinishMasterVM vm, SqlConnection currConn, SqlTransaction transaction,
            out string sqlText)
        {
            sqlText = "";
            sqlText += @"

UPDATE DisposeFinishDetails 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE DisposeNo = @DisposeNo

UPDATE DisposeFinishs 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE DisposeNo = @DisposeNo

";

            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
            cmdUpdate.CommandTimeout = 500;
            cmdUpdate.Parameters.AddWithValue("@DisposeNo", vm.DisposeNo);

            cmdUpdate.ExecuteNonQuery();
        }

        public ResultVM InsertDetail(DisposeFinishMasterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion


                #region Detail Data

                int PeriodId = vm.PeriodId;

                if (vm.Details != null && vm.Details.Count > 0)
                {
                    #region SQL Operation

                    #region SQL Text

                    sqlText = "";

                    sqlText = @"
INSERT INTO DisposeFinishDetails
(
DisposeNo
,DisposeLineNo
,BranchId
,TransactionDateTime
,TransactionType
,ItemNo
,UOM
,UsedQuantity
,PurchaseNo
,PurchaseQuantity
,VATRate
,VATAmount
,RebateRate
,RebateVATAmount
,Post
,Comments
,IsSynced
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,PeriodId
)
VALUES
(
 @DisposeNo
,@DisposeLineNo
,@BranchId
,@TransactionDateTime
,@TransactionType
,@ItemNo
,@UOM
,@UsedQuantity
,@PurchaseNo
,@PurchaseQuantity
,@VATRate
,@VATAmount
,@RebateRate
,@RebateVATAmount
,@Post
,@Comments
,@IsSynced
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@PeriodId
)

";

                    #endregion

                    #region Sql Execution

                    foreach (DisposeFinishDetailVM dVM in vm.Details)
                    {

                        cmd = new SqlCommand(sqlText, currConn, transaction);

                        cmd.Parameters.AddWithValue("@DisposeNo", vm.DisposeNo ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@DisposeLineNo", dVM.DisposeLineNo);
                        cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                        cmd.Parameters.AddWithValue("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(dVM.TransactionDateTime) ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@TransactionType", dVM.TransactionType ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@ItemNo", dVM.ItemNo ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@UOM", dVM.UOM ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@UsedQuantity", dVM.UsedQuantity);
                        cmd.Parameters.AddWithValue("@PurchaseNo", dVM.PurchaseNo ?? Convert.DBNull.ToString());
                        cmd.Parameters.AddWithValue("@PurchaseQuantity", dVM.PurchaseQuantity);
                        cmd.Parameters.AddWithValue("@VATRate", dVM.VATRate);
                        cmd.Parameters.AddWithValue("@VATAmount", dVM.VATAmount);
                        cmd.Parameters.AddWithValue("@RebateRate", dVM.RebateRate);
                        cmd.Parameters.AddWithValue("@RebateVATAmount", dVM.RebateVATAmount);
                        cmd.Parameters.AddWithValue("@Post", "N");
                        cmd.Parameters.AddWithValue("@Comments", dVM.Comments ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@IsSynced", dVM.IsSynced ?? Convert.DBNull);


                        cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@PeriodId", PeriodId);

                        executionResult = cmd.ExecuteNonQuery();

                    }


                    #endregion SqlExecution

                    #endregion
                }

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Data Saved Successfully!";

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "InsertDetail", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return rVM;

        }

        public ResultVM Update(DisposeFinishMasterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region Fiscal Year Check

                string transactionDate = vm.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MM-dd");

                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {
                    #region Year Lock

                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where 1=1 and '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dt = new DataTable();
                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else if (dt.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dt.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dt.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }

                    #endregion

                    #region Year Not Exist

                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    cmd = new SqlCommand(sqlText, currConn, transaction);

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmd);
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
                    #endregion
                }

                #endregion

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DisposeNo) from DisposeFinishs  where DisposeNo=@DisposeNo ";
                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndNullHandle("@DisposeNo", vm.DisposeNo ?? Convert.DBNull);
                int IDExist = Convert.ToInt32(cmd.ExecuteScalar());

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableFindExistID);
                }

                #endregion

                #region PeriodId

                string PeriodId = Convert.ToDateTime(vm.TransactionDateTime).ToString("MMyyyy");

                vm.PeriodId = Convert.ToInt32(PeriodId);

                #endregion

                #region Master Data

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
UPDATE DisposeFinishs SET
 BranchId             =@BranchId
,TransactionDateTime  =@TransactionDateTime
,ShiftId              =@ShiftId
,FinishItemNo         =@FinishItemNo
,Quantity             =@Quantity
,UOM                  =@UOM

,UnitPrice            =@UnitPrice
,OfferUnitPrice       =@OfferUnitPrice
,IsSaleable           =@IsSaleable
,BOMId                =@BOMId
,ReferenceNo          =@ReferenceNo
,SerialNo             =@SerialNo
,ImportIDExcel        =@ImportIDExcel
,Comments             =@Comments
,TransactionType      =@TransactionType
,Post                 =@Post
,IsSynced             =@IsSynced
,FiscalYear           =@FiscalYear
,AppVersion           =@AppVersion
,LastModifiedBy       =@LastModifiedBy
,LastModifiedOn       =@LastModifiedOn
,PeriodId             =@PeriodId

WHERE 1=1 AND DisposeNo=@DisposeNo
";



                #endregion

                #region Sql Execution

                cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@DisposeNo", vm.DisposeNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd.Parameters.AddWithValue("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(vm.TransactionDateTime) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@ShiftId", vm.ShiftId ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@FinishItemNo", vm.FinishItemNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Quantity", vm.Quantity);
                cmd.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);

                cmd.Parameters.AddWithValue("@UnitPrice", vm.UnitPrice);
                cmd.Parameters.AddWithValue("@OfferUnitPrice", vm.OfferUnitPrice);
                cmd.Parameters.AddWithValue("@IsSaleable", vm.IsSaleable ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@BOMId", vm.BOMId);
                cmd.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@ImportIDExcel", vm.ImportIDExcel ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Post", vm.Post ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@IsSynced", vm.IsSynced ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@AppVersion", vm.AppVersion ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@PeriodId", vm.PeriodId);

                executionResult = cmd.ExecuteNonQuery();

                #endregion SqlExecution

                #endregion

                #endregion

                #region Detail Data

                if (vm.Details != null && vm.Details.Count > 0)
                {
                    #region Delete Data

                    sqlText = "";

                    sqlText = @" DELETE DisposeFinishDetails WHERE 1=1 AND DisposeNo=@DisposeNo";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@DisposeNo", vm.DisposeNo ?? Convert.DBNull);

                    executionResult = cmd.ExecuteNonQuery();

                    #endregion

                    rVM = InsertDetail(vm, currConn, transaction, connVM);

                }

                #endregion

                UpdatePeriodId(vm, currConn, transaction, out sqlText);

                #region Master Setting Update

                CommonDAL commonDal = new CommonDAL();
                commonDal.UpdateProcessFlag(vm.DisposeNo, vm.TransactionDateTime, currConn, transaction);

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Data Updated Successfully!";

                rVM.InvoiceNo = vm.DisposeNo;
                rVM.Post = vm.Post;

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "Update", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return rVM;

        }

        public ResultVM Post(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
UPDATE DisposeFinishs SET Post='Y' WHERE 1=1 AND DisposeNo=@DisposeNo
UPDATE DisposeFinishDetails SET Post='Y' WHERE 1=1 AND DisposeNo=@DisposeNo

";
                #endregion

                #region Sql Execution

                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceNo) && paramVM.IDs == null)
                {
                    paramVM.IDs = new List<string>();
                    paramVM.IDs.Add(paramVM.InvoiceNo);
                }

                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    foreach (string ID in paramVM.IDs)
                    {


                        cmd = new SqlCommand(sqlText, currConn, transaction);

                        cmd.Parameters.AddWithValue("@DisposeNo", ID ?? Convert.DBNull);
                        executionResult = cmd.ExecuteNonQuery();

                    }

                }



                #endregion SqlExecution

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Data Posted Successfully!";

                rVM.InvoiceNo = paramVM.InvoiceNo;
                rVM.Post = "Y";

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "Post", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return rVM;

        }

        #region Web Method

        public DataTable SelectWeb(int Id = 0,string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            DataTable dt = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";

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

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
SELECT
 df.Id
,df.BranchId
,df.DisposeNo
,df.TransactionDateTime
,df.Post
,df.FinishItemNo
,pro.ProductCode
,pro.ProductName

,df.Quantity
,df.UOM
,df.UnitPrice
,df.OfferUnitPrice
,df.IsSaleable
,df.BOMId
,df.ShiftId
,df.ReferenceNo
,df.SerialNo
,df.ImportIDExcel
,df.Comments
,df.TransactionType
,df.IsSynced
,df.FiscalYear
,df.AppVersion
,df.CreatedBy
,df.CreatedOn
,df.LastModifiedBy
,df.LastModifiedOn
,df.PeriodId



FROM DisposeFinishs df
left outer join Products pro on df.FinishItemNo=pro.ItemNo
WHERE 1=1

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

                sqlTextOrderBy = " order by df.TransactionDateTime desc";

                #endregion

                #region Sql Execution

                sqlText = sqlText + " " + sqlTextParameter + sqlTextOrderBy;

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

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
                            cmd.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }


                SqlDataAdapter da = new SqlDataAdapter(cmd);

                if (Id > 0)
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                }

                da.Fill(dt);

                #endregion SqlExecution

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "SelectWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

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

        public List<DisposeFinishMasterVM> SelectVMWeb(int Id = 0,string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Declarations

            List<DisposeFinishMasterVM> VMs = new List<DisposeFinishMasterVM>();

            #endregion

            #region try

            try
            {
                DataTable dt = new DataTable();

                dt = SelectWeb(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string JSONString = JsonConvert.SerializeObject(dt);
                    VMs = JsonConvert.DeserializeObject<List<DisposeFinishMasterVM>>(JSONString);

                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("DisposeFinishDAL", "SelectVMWeb", ex.ToString());

                throw ex;
            }
            #endregion

            #region finally

            finally { }
            #endregion

            return VMs;

        }


        #endregion

    }
}
