using System;
using System.Collections.Generic;
using System.Data;
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
using VATServer.Interface;

namespace VATServer.Library
{
    public class Toll6_3InvoiceDAL : IToll6_3Invoice
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        public List<Toll6_3InvoiceVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string TransactionType = "")
        {
            #region Variables
            ////SqlConnection currConn = null;
            ////SqlTransaction transaction = null;
            string sqlText = "";
            List<Toll6_3InvoiceVM> VMs = new List<Toll6_3InvoiceVM>();
            Toll6_3InvoiceVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, false,null,TransactionType);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new Toll6_3InvoiceVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.TollNo = dr["TollNo"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.VendorID = Convert.ToInt32(dr["VendorID"].ToString());
                    vm.Address = dr["Address"].ToString();
                    vm.TollDateTime = dr["TollDateTime"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();

                    ////if (TransactionType == "Client63")
                    ////{
                    ////}
                    ////else
                    ////{
                    ////}
                    vm.Post = dr["Post"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    VMs.Add(vm);
                }


                #endregion SqlExecution

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
            //finally
            //{
            //}
            #endregion
            return VMs;
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string TransactionType="")
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
 ti.Id
,ti.TollNo
,ti.CustomerID
,c.CustomerName
,ti.VendorID
,V.VendorName
,ti.Address
,ti.TollDateTime
,ISNULL(ti.Post,'N') Post
,ti.Comments
,ti.CreatedBy
,ti.CreatedOn
,ti.LastModifiedBy
,ti.LastModifiedOn
,ISNULL(ti.BranchId,0) BranchId

,ISNULL(ti.TransactionType,'Contractor63')TransactionType
FROM Toll6_3Invoices ti 
LEFT OUTER JOIN Vendors V ON V.VendorID = ti.VendorID
LEFT OUTER JOIN Customers c ON c.CustomerID = ti.CustomerID

WHERE  1=1
";

                #region TransactionType (Client63)
                
                if (TransactionType == "Client63")
                {
                   
                }

                #endregion

                #region TransactionType (Contractor63)

                else
                {

////                    sqlText = @"
////SELECT
//// ti.Id
////,ti.TollNo
////,ti.CustomerID
////,ti.Address
////,ti.TollDateTime
////,ISNULL(ti.Post,'N') Post
////,ti.Comments
////,ti.CreatedBy
////,ti.CreatedOn
////,ti.LastModifiedBy
////,ti.LastModifiedOn
////,ISNULL(ti.BranchId,0) BranchId
////,c.CustomerName
////,ISNULL(ti.TransactionType,'Contractor63')TransactionType
////FROM Toll6_3Invoices ti 
////LEFT OUTER JOIN Customers c ON c.CustomerID = ti.CustomerID
////WHERE  1=1 
////";
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(TransactionType))
                {
                    sqlText += @" and ISNULL(ti.TransactionType,'Contractor63') =@TransactionType";
                }

                if (Id > 0)
                {
                    sqlText += @" and ti.Id=@Id";
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

                #endregion SqlText

                sqlText += " order by ti.TollNo desc";

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
                if (!string.IsNullOrWhiteSpace(TransactionType))
                {
                    da.SelectCommand.Parameters.AddWithValue("@TransactionType", TransactionType);                
                }
                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }
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
            return dt;
        }

        public DataTable TollSearch(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
 ti.Id
,ti.TollNo
,ti.CustomerID
,ti.Address
,ti.TollDateTime
,ISNULL(ti.Post,'N') Post
,ti.Comments
,ti.CreatedBy
,ti.CreatedOn
,ti.LastModifiedBy
,ti.LastModifiedOn
,c.CustomerName
,si.TransactionType
FROM Toll6_3Invoices ti 
LEFT OUTER JOIN Customers c ON c.CustomerID = ti.CustomerID
left outer join Toll6_3InvoiceDetails tid on 
ti.TollNo = tid.TollNo left outer join SalesInvoiceHeaders si
on tid.SalesInvoiceNo = si.SalesInvoiceNo
WHERE  1=1 or si.Is6_3TollCompleted is null
";


                if (Id > 0)
                {
                    sqlText += @" and ti.Id=@Id";
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
                #endregion SqlText
                sqlText += " order by ti.TollNo desc";

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
            return dt;
        }

        public List<Toll6_3InvoiceDetailVM> SelectDetail(string TollNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Toll6_3InvoiceDetailVM> VMs = new List<Toll6_3InvoiceDetailVM>();
            Toll6_3InvoiceDetailVM vm;
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
tid.Id
,tid.TollNo
,tid.TollLineNo
,tid.SalesInvoiceNo
,tid.InvoiceDateTime
,ISNULL(tid.Post,'N') Post

,tid.Comments
,tid.CreatedBy
,tid.CreatedOn
,tid.LastModifiedBy
,tid.LastModifiedOn


FROM Toll6_3InvoiceDetails tid
WHERE  1=1

";

                if (!string.IsNullOrWhiteSpace(TollNo))
                {
                    sqlText += "AND tid.TollNo=@TollNo";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]))
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

                if (!string.IsNullOrWhiteSpace(TollNo))
                {
                    objComm.Parameters.AddWithValue("@TollNo", TollNo);
                }
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]))
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
                    vm = new Toll6_3InvoiceDetailVM();

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.TollNo = dr["TollNo"].ToString();
                    vm.TollLineNo = dr["TollLineNo"].ToString();
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    vm.InvoiceDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                    
                    vm.Post = dr["Post"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

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

        public string[] InsertMaster(Toll6_3InvoiceVM MasterVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " insert into Toll6_3Invoices";
                sqlText += " (";
                sqlText += " TollNo";
                sqlText += " ,CustomerID";
                sqlText += " ,VendorID";
                sqlText += " ,Address";
                sqlText += " ,TollDateTime";
                sqlText += " ,Post";
                sqlText += " ,BranchId";
                sqlText += " ,SignatoryName";
                sqlText += " ,SignatoryDesig";
                sqlText += " ,TransactionType";                
                sqlText += " ,Comments";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";

                sqlText += " )";

                sqlText += " values";

                sqlText += " (";

                sqlText += "@TollNo";
                sqlText += ",@CustomerID";
                sqlText += ",@VendorID";
                sqlText += ",@Address";
                sqlText += ",@TollDateTime";
                sqlText += ",@Post";
                sqlText += ",@BranchId";
                sqlText += ",@SignatoryName";
                sqlText += ",@SignatoryDesig";
                sqlText += ",@TransactionType";
                sqlText += ",@Comments";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";

                sqlText += ")  SELECT SCOPE_IDENTITY()";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TollNo", MasterVM.TollNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID", MasterVM.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VendorID", MasterVM.VendorID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Address", MasterVM.Address);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TollDateTime", MasterVM.TollDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", MasterVM.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryName", MasterVM.SignatoryName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", MasterVM.SignatoryDesig);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", MasterVM.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", MasterVM.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", MasterVM.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", MasterVM.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", MasterVM.LastModifiedOn);

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
                retResults[2] = "" + MasterVM.Id;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

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

        public string[] InsertDetailList(List<Toll6_3InvoiceDetailVM> DetailVMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Insert

                sqlText = "";
                sqlText += " insert into Toll6_3InvoiceDetails(";

                sqlText += " TollNo";
                sqlText += " ,TollLineNo";
                sqlText += " ,SalesInvoiceNo";
                sqlText += " ,InvoiceDateTime";
                sqlText += " ,TransactionType";
                sqlText += " ,Post";
                sqlText += " ,BranchId";
                sqlText += " ,Comments";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@TollNo";
                sqlText += ",@TollLineNo";
                sqlText += ",@SalesInvoiceNo";
                sqlText += ",@InvoiceDateTime";
                sqlText += ",@TransactionType";
                sqlText += ",@Post";
                sqlText += ",@BranchId";
                sqlText += ",@Comments";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";

                sqlText += ")	";


                foreach (Toll6_3InvoiceDetailVM vm in DetailVMs)
                {

                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TollNo", vm.TollNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TollLineNo", vm.TollLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", vm.SalesInvoiceNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", vm.InvoiceDateTime);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", vm.TransactionType);

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", "N");
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", vm.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Comments", vm.Comments);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", vm.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", vm.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", vm.LastModifiedOn);

                    transResult = cmdInsDetail.ExecuteNonQuery();

                }

                #endregion Insert only DetailTable

                #region Commit
                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult


                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + DetailVMs.FirstOrDefault().SalesInvoiceNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

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

        public string[] InsertDetail(Toll6_3InvoiceDetailVM DetailVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Insert

                sqlText = "";
                sqlText += " insert into Toll6_3InvoiceDetails(";

                sqlText += "  Id";
                sqlText += " ,TollNo";
                sqlText += " ,TollLineNo";
                sqlText += " ,SalesInvoiceNo";
                sqlText += " ,InvoiceDateTime";
                
                sqlText += " ,Post";
                sqlText += " ,Comments";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += " @Id";
                sqlText += ",@TollNo";
                sqlText += ",@TollLineNo";
                sqlText += ",@SalesInvoiceNo";
                sqlText += ",@InvoiceDateTime";
                sqlText += ",@Post";
                sqlText += ",@Comments";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";

                sqlText += ")	";



                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Id", DetailVM.Id);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TollNo", DetailVM.TollNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TollLineNo", DetailVM.TollLineNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", DetailVM.SalesInvoiceNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", DetailVM.InvoiceDateTime);
                
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", "N");
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Comments", DetailVM.Comments);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", DetailVM.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", DetailVM.CreatedOn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", DetailVM.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", DetailVM.LastModifiedOn);


                transResult = cmdInsDetail.ExecuteNonQuery();

                #endregion Insert only DetailTable

                #region Commit
                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult


                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + DetailVM.SalesInvoiceNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

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

        public string[] UpdateMaster(Toll6_3InvoiceVM MasterVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update Toll6_3Invoices set  ";

                sqlText += " TollNo=@TollNo";
                sqlText += " ,CustomerID=@CustomerID";
                sqlText += " ,VendorID=@VendorID";
                sqlText += " ,Address=@Address";
                sqlText += " ,TollDateTime=@TollDateTime";
                sqlText += " ,TransactionType=@TransactionType";

                sqlText += " ,Post=@Post";
                sqlText += " ,BranchId=@BranchId";
                sqlText += " ,SignatoryName=@SignatoryName";
                sqlText += " ,SignatoryDesig=@SignatoryDesig";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,CreatedBy=@CreatedBy";
                sqlText += " ,CreatedOn=@CreatedOn";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";

                sqlText += " where Id=@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.Id);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TollNo", MasterVM.TollNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", MasterVM.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VendorID", MasterVM.VendorID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Address", MasterVM.Address);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TollDateTime", MasterVM.TollDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", MasterVM.TransactionType);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", "N");
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", MasterVM.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryName", MasterVM.SignatoryName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", MasterVM.SignatoryDesig);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", MasterVM.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", MasterVM.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", MasterVM.CreatedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", MasterVM.LastModifiedOn);

                transResult = cmdUpdate.ExecuteNonQuery();


                #endregion ID generated completed,Insert new Information in Header

                #region Commit


                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "" + MasterVM.Id;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

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

        public string[] UpdateDetail(Toll6_3InvoiceDetailVM DetailVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update 3InvoiceDetails set  ";

                sqlText += " TollNo=@TollNo";
                sqlText += " ,TollLineNo=@TollLineNo";
                sqlText += " ,SalesInvoiceNo=@SalesInvoiceNo";
                sqlText += " ,InvoiceDateTime=@InvoiceDateTime";

                sqlText += " ,Post=@Post";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,CreatedBy=@CreatedBy";
                sqlText += " ,CreatedOn=@CreatedOn";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";

                sqlText += " where Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", DetailVM.Id);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TollNo", DetailVM.TollNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TollLineNo", DetailVM.TollLineNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", DetailVM.SalesInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", DetailVM.InvoiceDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post","N");
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", DetailVM.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", DetailVM.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", DetailVM.CreatedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", DetailVM.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", DetailVM.LastModifiedOn);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion ID generated completed,Insert new Information in Header

                #region Commit


                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "" + DetailVM.Id;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

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
        //currConn to VcurrConn 25-Aug-2020
        public string[] Insert(Toll6_3InvoiceVM MasterVM, List<Toll6_3InvoiceDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            ////SqlConnection vcurrConn = VcurrConn;
            ////if (vcurrConn == null)
            ////{
            ////    VcurrConn = null;
            ////    Vtransaction = null;
            ////}
            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                ////if (vcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }

                ////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                ////}

                #endregion open connection and transaction

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

                if (MasterVM.TransactionType == "Contractor63")
                {
                    newID = new CommonDAL().TransactionCode("Toll", "Invoice6_3", "Toll6_3Invoices", "TollNo",
                                                  "TollDateTime", MasterVM.TollDateTime, MasterVM.BranchId.ToString(), currConn, transaction);

                    MasterVM.TollNo = newID;
                }
                else if (MasterVM.TransactionType == "Client63")
                {
                    newID = new CommonDAL().TransactionCode("Toll", "Client6_3", "Toll6_3Invoices", "TollNo",
                                                  "TollDateTime", MasterVM.TollDateTime, MasterVM.BranchId.ToString(), currConn, transaction);

                    MasterVM.TollNo = newID;
                }

                retResults = InsertMaster(MasterVM, currConn, transaction);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #region Insert only DetailTable

                foreach (Toll6_3InvoiceDetailVM dVM in DetailVMs)
                {
                    dVM.TollNo = newID;
                    dVM.CreatedBy = MasterVM.CreatedBy;
                    dVM.CreatedOn = MasterVM.CreatedOn;
                    dVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    dVM.LastModifiedOn = MasterVM.LastModifiedOn;

                }

                retResults = InsertDetailList(DetailVMs, currConn, transaction);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }

                #endregion Insert only DetailTable

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

update  Toll6_3Invoices                             
set PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TollDateTime)) +  CONVERT(VARCHAR(4),YEAR(TollDateTime)),6)
WHERE TollNo = @TollNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@TollNo", newID);

                transResult = cmdUpdate.ExecuteNonQuery();
                #endregion

                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                retResults[4] = Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
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

        public string[] Update(Toll6_3InvoiceVM MasterVM, List<Toll6_3InvoiceDetailVM> DetailVMs, SysDBInfoVMTemp connVM = null)
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

            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameUpdate);
                #endregion open connection and transaction

                #region update Header

                string[] cFields = new string[] { "ti.TollNo" };
                string[] cVals = new string[] { MasterVM.TollNo };
                Toll6_3InvoiceVM mVM = SelectAllList(0, cFields, cVals, currConn, transaction,null,MasterVM.TransactionType).FirstOrDefault();

                mVM.CustomerID = MasterVM.CustomerID;
                mVM.VendorID = MasterVM.VendorID;
                mVM.Address = MasterVM.Address;
                mVM.TollDateTime = MasterVM.TollDateTime;
                mVM.TransactionType = MasterVM.TransactionType;

                mVM.Comments = MasterVM.Comments;
                mVM.LastModifiedBy = MasterVM.LastModifiedBy;
                mVM.LastModifiedOn = MasterVM.LastModifiedOn;
                mVM.BranchId = MasterVM.BranchId;
                mVM.SignatoryName = MasterVM.SignatoryName;
                mVM.SignatoryDesig = MasterVM.SignatoryDesig;

                retResults = UpdateMaster(mVM, currConn, transaction);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                }


                #endregion update Header

                #region Insert only DetailTable

                foreach (Toll6_3InvoiceDetailVM dVM in DetailVMs)
                {
                    dVM.CreatedBy = MasterVM.CreatedBy;
                    dVM.CreatedOn = MasterVM.CreatedOn;
                    dVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    dVM.LastModifiedOn = MasterVM.LastModifiedOn;
                }

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM Toll6_3InvoiceDetails WHERE TollNo=@TollNo ";
                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@TollNo", MasterVM.TollNo);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                retResults = InsertDetailList(DetailVMs, currConn, transaction);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }

                #endregion Insert only DetailTable

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

update  Toll6_3Invoices                             
set PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TollDateTime)) +  CONVERT(VARCHAR(4),YEAR(TollDateTime)),6)
WHERE TollNo = @TollNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@TollNo", MasterVM.TollNo);

                transResult = cmdUpdate.ExecuteNonQuery();
                #endregion

                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();

                }

                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + MasterVM.TollNo;
                retResults[3] = "" + "N";
                retResults[4] = MasterVM.Id.ToString();
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                transaction.Rollback();
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

        public string[] Post(Toll6_3InvoiceVM MasterVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update Toll6_3Invoices set   SignatoryName=@SignatoryName,SignatoryDesig=@SignatoryDesig,  Post='Y'  where 1=1 AND TollNo=@TollNo";
                sqlText += " update Toll6_3InvoiceDetails set    Post='Y'  where 1=1 AND TollNo=@TollNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TollNo", MasterVM.TollNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryName", MasterVM.SignatoryName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", MasterVM.SignatoryDesig);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion ID generated completed,Insert new Information in Header

                #region Commit


                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.purchaseMsgSuccessfullyPost;
                retResults[2] = "" + MasterVM.Id;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

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

        public string[] UpdateTollCompleted(string flag, string tollNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                var getInvoice = "select SalesInvoiceNo from Toll6_3InvoiceDetails where TollNo = @tollNo";

                sqlText = "";
                sqlText += " update SalesInvoiceHeaders set    Is6_3TollCompleted=@flag  where 1=1 AND SalesInvoiceNo in (" + getInvoice + ")";

                var SalesInvoiceNo = "";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                //cmdUpdate.Parameters.AddWithValueAndNullHandle("@tollNo", tollNo);

                //var adatapter = new SqlDataAdapter(cmdUpdate);
                //var table = new DataTable();
                //adatapter.Fill(table);


                //foreach (DataRow row in table.Rows)
                //{
                    
                //}

                //SalesInvoiceNo = (string)cmdUpdate.ExecuteScalar();

                //cmdUpdate.CommandText = sqlText;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@tollNo", tollNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@flag", flag);

                transResult = cmdUpdate.ExecuteNonQuery();


                #endregion ID generated completed,Insert new Information in Header

                #region Commit


                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";

                #endregion SuccessResult
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

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

    }
}





