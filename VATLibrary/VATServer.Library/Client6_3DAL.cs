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
    public class Client6_3DAL
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
 cln.Id
,cln.InvoiceNo
,cln.Post
,vnd.VendorCode
,vnd.VendorName
,cln.Address
,cln.InvoiceDateTime
,cln.BranchId
,cln.SignatoryName
,cln.SignatoryDesig
,cln.TransactionType
,cln.PeriodId
,cln.Comments

,cln.CreatedBy
,cln.CreatedOn
,cln.LastModifiedBy
,cln.LastModifiedOn

,cln.VendorID
,vnd.Address1 VendorAddress

FROM Client6_3s cln
LEFT OUTER JOIN Vendors vnd on cln.VendorID=vnd.VendorID
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

                sqlTextOrderBy = " order by cln.InvoiceDateTime desc";

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

            #region Catch & Finally


            catch (Exception ex)
            {

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

        public List<Client6_3VM> SelectVM(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Declarations

            List<Client6_3VM> VMs = new List<Client6_3VM>();

            #endregion

            try
            {
                DataTable dt = new DataTable();

                dt = Select(conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string JSONString = JsonConvert.SerializeObject(dt);
                    VMs = JsonConvert.DeserializeObject<List<Client6_3VM>>(JSONString);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            finally { }

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
 clnd.Id
,clnd.InvoiceNo
,clnd.InvoiceLineNo
,clnd.ReceiveNo
,clnd.InvoiceDateTime
,clnd.ItemNo
,clnd.UOM
,clnd.Quantity
,clnd.UnitPrice
,clnd.Subtotal
,clnd.SDRate
,clnd.SDAmount
,clnd.VATRate
,clnd.VATAmount
,clnd.LineTotalAmount
,clnd.BranchId
,clnd.PeriodId
,clnd.TransactionType
,clnd.Comments
,clnd.Post
,clnd.CreatedBy
,clnd.CreatedOn
,clnd.LastModifiedBy
,clnd.LastModifiedOn

,pro.ProductCode
,pro.ProductName



FROM Client6_3Details clnd
LEFT OUTER JOIN Products pro ON clnd.ItemNo=pro.ItemNo
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

                sqlTextOrderBy = " order by clnd.InvoiceDateTime desc";

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

            #region Catch & Finally


            catch (Exception ex)
            {

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

        public List<Client6_3DetailVM> SelectDetailVM(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Declarations

            List<Client6_3DetailVM> VMs = new List<Client6_3DetailVM>();

            #endregion

            try
            {
                DataTable dt = new DataTable();

                dt = SelectDetail(conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string JSONString = JsonConvert.SerializeObject(dt);
                    VMs = JsonConvert.DeserializeObject<List<Client6_3DetailVM>>(JSONString);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            finally { }

            return VMs;

        }

        public ResultVM Insert(Client6_3VM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region Fiscal Year Check

                string transactionDate = vm.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(vm.InvoiceDateTime).ToString("yyyy-MM-dd");

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
                sqlText = sqlText + "select COUNT(InvoiceNo) from Client6_3s  where InvoiceNo=@InvoiceNo ";
                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndNullHandle("@InvoiceNo", vm.InvoiceNo ?? Convert.DBNull);
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
                    vm.InvoiceNo = _CommonDAL.TransactionCode("Toll", "Client6_3", "Client6_3s", "InvoiceNo", "InvoiceDateTime", vm.InvoiceDateTime, vm.BranchId.ToString(), currConn, transaction, connVM);
                }

                string PeriodId = Convert.ToDateTime(vm.InvoiceDateTime).ToString("MMyyyy");

                vm.PeriodId = PeriodId;

                #endregion

                #region Master Data

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
INSERT INTO Client6_3s
(
 InvoiceNo
,VendorID
,Address
,InvoiceDateTime
,BranchId
,PeriodId
,SignatoryName
,SignatoryDesig
,TransactionType
,Comments
,Post
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
)
VALUES
(
 @InvoiceNo
,@VendorID
,@Address
,@InvoiceDateTime
,@BranchId
,@PeriodId
,@SignatoryName
,@SignatoryDesig
,@TransactionType
,@Comments
,@Post
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
)

";

                #endregion

                #region Sql Execution

                cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@VendorID", vm.VendorID ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(vm.InvoiceDateTime) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd.Parameters.AddWithValue("@PeriodId", vm.PeriodId);
                cmd.Parameters.AddWithValue("@SignatoryName", vm.SignatoryName ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@SignatoryDesig", vm.SignatoryDesig ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Post", vm.Post ?? Convert.DBNull);

                cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);

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

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Data Saved Successfully!";

                rVM.InvoiceNo = vm.InvoiceNo;
                rVM.Post = vm.Post;

                #endregion
            }

            #region Catch & Finally

            catch (Exception ex)
            {

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

        public ResultVM InsertDetail(Client6_3VM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region Detail Data

                string PeriodId = vm.PeriodId;

                if (vm.Details != null && vm.Details.Count > 0)
                {
                    #region SQL Operation

                    #region SQL Text

                    sqlText = "";

                    sqlText = @"
INSERT INTO Client6_3Details
(
 InvoiceNo
,InvoiceLineNo
,ReceiveNo
,InvoiceDateTime
,ItemNo
,UOM
,Quantity
,UnitPrice
,Subtotal
,SDRate
,SDAmount
,VATRate
,VATAmount
,LineTotalAmount
,BranchId
,TransactionType
,PeriodId
,Comments
,Post
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
)
VALUES
(
 @InvoiceNo
,@InvoiceLineNo
,@ReceiveNo
,@InvoiceDateTime
,@ItemNo
,@UOM
,@Quantity
,@UnitPrice
,@Subtotal
,@SDRate
,@SDAmount
,@VATRate
,@VATAmount
,@LineTotalAmount
,@BranchId
,@TransactionType
,@PeriodId
,@Comments
,@Post
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
)

";

                    #endregion

                    #region Sql Execution

                    foreach (Client6_3DetailVM dVM in vm.Details)
                    {

                        cmd = new SqlCommand(sqlText, currConn, transaction);

                        cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@InvoiceLineNo", dVM.InvoiceLineNo);
                        cmd.Parameters.AddWithValue("@ReceiveNo", dVM.ReceiveNo ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(vm.InvoiceDateTime) ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@ItemNo", dVM.ItemNo);
                        cmd.Parameters.AddWithValue("@UOM", dVM.UOM);
                        cmd.Parameters.AddWithValue("@Quantity", dVM.Quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", dVM.UnitPrice);
                        cmd.Parameters.AddWithValue("@Subtotal", dVM.Subtotal);
                        cmd.Parameters.AddWithValue("@SDRate", dVM.SDRate);
                        cmd.Parameters.AddWithValue("@SDAmount", dVM.SDAmount);
                        cmd.Parameters.AddWithValue("@VATRate", dVM.VATRate);
                        cmd.Parameters.AddWithValue("@VATAmount", dVM.VATAmount);
                        cmd.Parameters.AddWithValue("@LineTotalAmount", dVM.LineTotalAmount);
                        cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                        cmd.Parameters.AddWithValue("@PeriodId", vm.PeriodId);
                        cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@Post", vm.Post ?? Convert.DBNull);

                        cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);

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

            #region Catch & Finally

            catch (Exception ex)
            {

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

        public ResultVM Update(Client6_3VM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            int executionResult = 0;

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

                #region Command Open

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region Fiscal Year Check

                string transactionDate = vm.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(vm.InvoiceDateTime).ToString("yyyy-MM-dd");

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
                sqlText = sqlText + "select COUNT(InvoiceNo) from Client6_3s  where InvoiceNo=@InvoiceNo ";
                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndNullHandle("@InvoiceNo", vm.InvoiceNo ?? Convert.DBNull);
                int IDExist = Convert.ToInt32(cmd.ExecuteScalar());

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableFindExistID);
                }

                #endregion

                #region PeriodId

                string PeriodId = Convert.ToDateTime(vm.InvoiceDateTime).ToString("MMyyyy");

                vm.PeriodId = PeriodId;

                #endregion

                #region Master Data

                #region SQL Operation

                #region SQL Text

                sqlText = "";

                sqlText = @"
UPDATE Client6_3s SET
VendorID=@VendorID
,Address=@Address
,InvoiceDateTime=@InvoiceDateTime
,BranchId=@BranchId
,SignatoryName=@SignatoryName
,SignatoryDesig=@SignatoryDesig
,TransactionType=@TransactionType
,Comments=@Comments
,Post=@Post
,LastModifiedBy=@LastModifiedBy
,LastModifiedOn=@LastModifiedOn

WHERE 1=1 AND InvoiceNo=@InvoiceNo
";



                #endregion

                #region Sql Execution

                cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@VendorID", vm.VendorID ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(vm.InvoiceDateTime));
                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd.Parameters.AddWithValue("@SignatoryName", vm.SignatoryName ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@SignatoryDesig", vm.SignatoryDesig ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Post", vm.Post ?? Convert.DBNull);


                cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);

                executionResult = cmd.ExecuteNonQuery();

                #endregion SqlExecution

                #endregion

                #endregion

                #region Detail Data

                if (vm.Details != null && vm.Details.Count > 0)
                {
                    #region Delete Data

                    sqlText = "";

                    sqlText = @" DELETE Client6_3Details WHERE 1=1 AND InvoiceNo=@InvoiceNo";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo ?? Convert.DBNull);

                    executionResult = cmd.ExecuteNonQuery();

                    #endregion

                    rVM = InsertDetail(vm, currConn, transaction, connVM);

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
                rVM.Message = "Data Updated Successfully!";

                rVM.InvoiceNo = vm.InvoiceNo;
                rVM.Post = vm.Post;

                #endregion
            }

            #region Catch & Finally

            catch (Exception ex)
            {

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
UPDATE Client6_3s SET Post='Y' ,SignatoryName=@SignatoryName ,SignatoryDesig=@SignatoryDesig WHERE 1=1 AND InvoiceNo=@InvoiceNo 
UPDATE Client6_3Details SET Post='Y' WHERE 1=1 AND InvoiceNo=@InvoiceNo


update PurchaseInvoiceHeaders set IsClients6_3Complete = Client6_3Details.Post 
from Client6_3Details 
where Client6_3Details.ReceiveNo = PurchaseInvoiceHeaders.PurchaseInvoiceNo 
and Client6_3Details.InvoiceNo =@InvoiceNo


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

                        cmd.Parameters.AddWithValue("@InvoiceNo", ID ?? Convert.DBNull);

                        cmd.Parameters.AddWithValue("@SignatoryName", paramVM.SignatoryName);
                        cmd.Parameters.AddWithValue("@SignatoryDesig", paramVM.SignatoryDesig);

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

            #region Catch & Finally

            catch (Exception ex)
            {

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

    }
}
