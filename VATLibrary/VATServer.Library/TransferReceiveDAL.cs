using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;
using Newtonsoft.Json;

namespace VATServer.Library
{
    public class TransferReceiveDAL : ITransferReceive
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        ProductDAL _ProductDAL = new ProductDAL();
        #endregion

        #region Navigation

        public NavigationVM TransferReceive_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

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
                    currConn = _dbsqlConnection.GetConnection();
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

                #region Check Point

                if (vm.FiscalYear == 0)
                {
                    DateTime now = DateTime.Now;
                    string startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MMM-dd");
                    FiscalYearVM varFiscalYearVM = new FiscalYearDAL().SelectAll(0, new[] { "PeriodStart" }, new[] { startDate }, currConn, transaction).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(varFiscalYearVM.PeriodID))
                    {
                        throw new ArgumentNullException("Fiscal Year Not Available for Date: " + now);
                    }

                    vm.FiscalYear = Convert.ToInt32(varFiscalYearVM.CurrentYear);

                }


                #endregion

                #region SQL Statement

                #region SQL Text

                sqlText = "";
                sqlText = @"
------declare @Id as int = 16530;
------declare @FiscalYear as int = 2021;
------declare @TransactionType as varchar(50) = 'Other';
------declare @BranchId as int = 1;

";
                if (vm.ButtonName == "Current")
                {
                    #region Current Item

                    sqlText = sqlText + @"
--------------------------------------------------Current--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferReceiveNo InvoiceNo from TransferReceives inv
where 1=1 
and inv.TransferReceiveNo=@InvoiceNo

";
                    #endregion
                }
                else if (vm.Id == 0 || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferReceiveNo InvoiceNo from TransferReceives inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
order by Id asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Last")
                {

                    #region Last Item

                    sqlText = sqlText + @"
--------------------------------------------------Last--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferReceiveNo InvoiceNo from TransferReceives inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
order by Id desc


";
                    #endregion

                }
                else if (vm.ButtonName == "Next")
                {

                    #region Next Item

                    sqlText = sqlText + @"
--------------------------------------------------Next--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferReceiveNo InvoiceNo from TransferReceives inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
and Id > @Id
order by Id asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Previous")
                {
                    #region Previous Item

                    sqlText = sqlText + @"
--------------------------------------------------Previous--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferReceiveNo InvoiceNo from TransferReceives inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
and Id < @Id
order by Id desc

";
                    #endregion
                }


                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (vm.ButtonName == "Current")
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    if (vm.Id > 0)
                    {
                        cmd.Parameters.AddWithValue("@Id", vm.Id);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    vm.InvoiceNo = dt.Rows[0]["InvoiceNo"].ToString();
                }
                else
                {
                    if (vm.ButtonName == "Previous" || vm.ButtonName == "Current")
                    {
                        vm.ButtonName = "First";
                        vm = TransferReceive_Navigation(vm, currConn, transaction, connVM);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = TransferReceive_Navigation(vm, currConn, transaction, connVM);

                    }
                }


                #endregion

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return vm;

        }


        #endregion

        #region Web Methods

        public List<TransferReceiveVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            ////SqlConnection currConn = null;
            ////SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferReceiveVM> VMs = new List<TransferReceiveVM>();
            TransferReceiveVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, true, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new TransferReceiveVM();
                    vm.Id = dr["Id"].ToString();
                    vm.TransferReceiveNo = dr["TransferReceiveNo"].ToString();
                    vm.TransactionDateTime = dr["ReceiveDateTime"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());

                    vm.TransferFromNo = dr["TransferFromNo"].ToString();
                    vm.TransferNo = dr["TransferNo"].ToString();
                    vm.TransferFrom = Convert.ToInt32(dr["TransferFrom"]);
                    vm.BranchName = dr["BranchName"].ToString();

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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
tr.Id
,ISNULL(tr.FiscalYear,0) FiscalYear
,tr.TransferReceiveNo
, convert (varchar,tr.TransactionDateTime,120)ReceiveDateTime
,isnull(tr.TotalAmount     ,'0')TotalAmount
,isnull(tr.TransactionType ,'N/A')TransactionType
,isnull(tr.SerialNo        ,'N/A')SerialNo
,isnull(tr.TransferFromNo        ,'N/A')TransferFromNo
,isnull(tr.TransferNo        ,'N/A')TransferNo
,isnull(tr.Comments        ,'N/A')Comments
,isnull(tr.Post            ,'N')Post
,isnull(tr.ReferenceNo     ,'N/A')ReferenceNo
,isnull(tr.TransferFrom      ,0)TransferFrom
,isnull(br.BranchName,'N/A') BranchName
,isnull(tr.TotalVATAmount       ,0)TotalVATAmount
,isnull(tr.TotalSDAmount       ,0)TotalSDAmount
,isnull(tr.CreatedBy       ,'N/A')CreatedBy
,isnull(tr.CreatedOn       ,'19000101')CreatedOn
,isnull(tr.LastModifiedBy  ,'N/A')LastModifiedBy
,isnull(tr.LastModifiedOn  ,'19000101')LastModifiedOn
FROM         dbo.TransferReceives tr
LEFT OUTER JOIN BranchProfiles br on tr.TransferFrom = br.BranchId
WHERE 1=1
";


                if (Id > 0)
                {
                    sqlText += @" and tr.Id=@Id";
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

        public List<TransferReceiveDetailVM> SelectDetail(string TransferReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferReceiveDetailVM> VMs = new List<TransferReceiveDetailVM>();
            TransferReceiveDetailVM vm;
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
 iss.TransferReceiveNo
,iss.ReceiveLineNo
,iss.ItemNo
,ISNULL(iss.Quantity,0) Quantity
,ISNULL(iss.CostPrice,0) CostPrice
,iss.UOM
,iss.TransferFrom
,ISNULL(iss.SubTotal,0) SubTotal
,iss.Comments
,iss.TransactionType
,iss.TransactionDateTime
,iss.Post
,ISNULL(iss.UOMQty,0) UOMQty
,ISNULL(iss.UOMPrice,0) UOMPrice
,ISNULL(iss.UOMc,0) UOMc
,iss.UOMn
,iss.CreatedBy
,iss.CreatedOn
,iss.LastModifiedBy
,iss.LastModifiedOn
,iss.TransferFromNo

,p.ProductCode
,p.ProductName
FROM TransferReceiveDetails iss left outer join Products p on iss.ItemNo=p.ItemNo
WHERE  1=1

";
                if (TransferReceiveNo != null)
                {
                    sqlText += "AND iss.TransferReceiveNo=@TransferReceiveNo";
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

                if (TransferReceiveNo != null)
                {
                    objComm.Parameters.AddWithValue("@TransferReceiveNo", TransferReceiveNo);
                }

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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new TransferReceiveDetailVM();
                    vm.TransferReceiveNo = dr["TransferReceiveNo"].ToString();
                    vm.ReceiveLineNo = dr["ReceiveLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.TransferFrom = Convert.ToInt32(dr["TransferFrom"]);
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.TransactionDateTime = dr["TransactionDateTime"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransferFromNo = dr["TransferFromNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ItemName = dr["ProductName"].ToString();

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

        #endregion

        #region Plain Methods

        public string[] TransferInsertToMaster(TransferReceiveVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            //string PostStatus = "";
            string newID = "";
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

                #region TransferIssues ID Create
                //if (string.IsNullOrEmpty(Master.TransactionType)) //start
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                //}
                //#endregion

                //#region TransferIssues ID Create For Other
                //CommonDAL commonDal = new CommonDAL();
                //if (Master.TransactionType == "61Out")
                //{
                //    newID = commonDal.TransactionCode("Transfer", "61Out", "TransferIssues", "TransferIssueNo",
                //                              "TransactionDateTime", Master.TransactionDateTime, currConn, transaction);
                //}
                //if (Master.TransactionType == "62Out")
                //{
                //    newID = commonDal.TransactionCode("Transfer", "62Out", "TransferIssues", "TransferIssueNo",
                //                              "TransactionDateTime", Master.TransactionDateTime, currConn, transaction);
                //}
                #endregion Purchase ID Create For Other

                #region Insert

                sqlText = "";
                sqlText += "  insert into TransferReceives";
                sqlText += " (";
                sqlText += " TransferReceiveNo";
                sqlText += " ,TransactionDateTime";
                sqlText += " ,TotalAmount";
                sqlText += " ,TransactionType";
                sqlText += " ,SerialNo";
                sqlText += " ,TransferFromNo";
                sqlText += " ,TransferNo";
                sqlText += " ,Comments";
                sqlText += " ,Post";
                sqlText += " ,ReferenceNo";
                sqlText += " ,TransferFrom";
                sqlText += " ,BranchId";

                sqlText += " ,TotalVATAmount";
                sqlText += " ,TotalSDAmount";

                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,LastModifiedBy";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@Master_TransferReceiveNo";
                sqlText += ",@Master_TransactionDateTime";
                sqlText += ",@Master_TotalAmount";
                sqlText += ",@Master_TransactionType";
                sqlText += ",@Master_SerialNo";
                sqlText += ",@Master_TransferFromNo";
                sqlText += ",@Master_TransferNo";
                sqlText += ",@Master_Comments";
                sqlText += ",@Master_Post";
                sqlText += ",@Master_ReferenceNo";
                sqlText += ",@Master_TransferFrom";
                sqlText += ",@BranchId";

                sqlText += ",@TotalVATAmount";
                sqlText += ",@TotalSDAmount";

                sqlText += ",@Master_CreatedBy";
                sqlText += ",@Master_CreatedOn";
                sqlText += ",@Master_LastModifiedOn";
                sqlText += ",@Master_LastModifiedBy";

                sqlText += ")  SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferReceiveNo", Master.TransferReceiveNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransactionDateTime", OrdinaryVATDesktop.DateToDate(Master.TransactionDateTime));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferFromNo", Master.TransferFromNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferNo", Master.TransferNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_ReferenceNo", Master.ReferenceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferFrom", Master.TransferFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
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
                retResults[1] = "Requested insert into TransferIssues successfully Added";
                retResults[2] = Master.TransferReceiveNo;
                //retResults[3] = "" + PostStatus;
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
                    if (transaction != null) transaction.Rollback();
                }

                FileLogger.Log("TransferReceiveDAL", "TransferInsertToMaster", ex.ToString() + "\n" + sqlText);

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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
        
        public string[] TransferInsertToDetail(TransferReceiveDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            ProductDAL productDal = new ProductDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string newID = "";
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

                #region Insert only DetailTable TransferReceiveDetails

                sqlText = "";
                sqlText += "  insert into TransferReceiveDetails(";
                sqlText += " TransferReceiveNo";
                sqlText += ",ReceiveLineNo";
                sqlText += ",ItemNo";
                sqlText += ",Quantity";
                sqlText += ",CostPrice";
                sqlText += ",UOM";
                sqlText += ",SubTotal";
                sqlText += ",Comments";
                sqlText += ",TransactionType";
                sqlText += ",TransactionDateTime";
                sqlText += ",TransferFrom";
                sqlText += ",Post";

                sqlText += ",VATRate";
                sqlText += ",VATAmount";
                sqlText += ",SDRate";
                sqlText += ",SDAmount";
                sqlText += ",UOMQty";
                sqlText += ",UOMPrice";
                sqlText += ",UOMc";
                sqlText += ",UOMn";
                sqlText += ",BranchId";
                sqlText += ",TransferFromNo";
                sqlText += ",Weight";

                sqlText += ",CreatedBy";
                sqlText += ",CreatedOn";
                sqlText += ",LastModifiedBy";
                sqlText += ",LastModifiedOn";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@Detail_TransferReceiveNo";
                sqlText += ",@Detail_ReceiveLineNo";
                sqlText += ",@Detail_ItemNo";
                sqlText += ",@Detail_Quantity";
                sqlText += ",@Detail_CostPrice";
                sqlText += ",@Detail_UOM";
                sqlText += ",@Detail_SubTotal";
                sqlText += ",@Detail_Comments";
                sqlText += ",@Detail_TransactionType";
                sqlText += ",@Detail_TransactionDateTime";
                sqlText += ",@Detail_TransferFrom";
                sqlText += ",@Detail_Post";

                sqlText += ",@VATRate";
                sqlText += ",@VATAmount";
                sqlText += ",@SDRate";
                sqlText += ",@SDAmount";
                sqlText += ",@UOMQty";
                sqlText += ",@UOMPrice";
                sqlText += ",@UOMc";
                sqlText += ",@UOMn";
                sqlText += ",@BranchId";
                sqlText += ",@TransferFromNo";
                sqlText += ",@Weight";

                sqlText += ",@Detail_CreatedBy";
                sqlText += ",@Detail_CreatedOn";
                sqlText += ",@Detail_LastModifiedBy";
                sqlText += ",@Detail_LastModifiedOn";


                sqlText += ")	";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferReceiveNo", Detail.TransferReceiveNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_ReceiveLineNo", Detail.ReceiveLineNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_ItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Quantity", Detail.Quantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CostPrice", Detail.CostPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOM", Detail.UOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_SubTotal", Detail.SubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Comments", Detail.Comments);


                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDRate", Detail.SDRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMQty", Detail.UOMQty);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMPrice", Detail.UOMPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMc", Detail.UOMc);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMn", Detail.UOMn);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransferFromNo", Detail.TransferFromNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Weight", Detail.Weight);


                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Post", Detail.Post);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransactionType", Detail.TransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransactionDateTime", OrdinaryVATDesktop.DateToDate(Detail.TransactionDateTime));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferFrom", Detail.TransferFrom);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_LastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_LastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                #endregion Insert only DetailTable

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
                retResults[1] = "Requested insert into TransferReceiveDetails successfully Added";
                retResults[2] = "" + newID;
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
                    if (transaction != null) transaction.Rollback();
                }
                FileLogger.Log("TransferReceiveDAL", "TransferInsertToDetail", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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
        public string[] TransferUpdateToMaster(TransferReceiveVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update TransferReceives set  ";

                sqlText += " TransactionDateTime =@TransactionDateTime";
                sqlText += " ,TotalAmount         =@TotalAmount";
                sqlText += " ,TransactionType     =@TransactionType";
                sqlText += " ,SerialNo            =@SerialNo";
                sqlText += " ,Comments            =@Comments";
                sqlText += " ,ReferenceNo         =@ReferenceNo";
                sqlText += " ,TransferFrom          =@TransferFrom";
                sqlText += " ,Post                =@Post";

                sqlText += " ,TotalVATAmount                =@TotalVATAmount";
                sqlText += " ,TotalSDAmount                =@TotalSDAmount";



                sqlText += " ,CreatedBy           =@CreatedBy";
                sqlText += " ,CreatedOn           =@CreatedOn";
                sqlText += " ,LastModifiedBy      =@LastModifiedBy";
                sqlText += " ,LastModifiedOn      =@LastModifiedOn";

                sqlText += " where              TransferReceiveNo=@TransferReceiveNo";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", Master.TransferReceiveNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Master.TransactionDateTime));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReferenceNo", Master.ReferenceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferFrom", Master.TransferFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));



                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
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
                retResults[1] = "Requested update into TransferIssues successfully updated ";
                retResults[2] = "" + Master.TransferReceiveNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
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
                FileLogger.Log("TransferReceiveDAL", "TransferUpdateToMaster", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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
        public string[] TransferUpdateToDetail(TransferReceiveDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update TransferReceiveDetails set  ";

                sqlText += " ReceiveLineNo               =@ReceiveLineNo";
                sqlText += " ,Quantity                 =@Quantity";
                sqlText += " ,CostPrice                =@CostPrice";
                sqlText += " ,UOM                      =@UOM";
                sqlText += " ,SubTotal                 =@SubTotal";
                sqlText += " ,Comments                 =@Comments";
                sqlText += " ,TransactionType          =@TransactionType";
                sqlText += " ,TransactionDateTime      =@TransactionDateTime";
                sqlText += " ,TransferFrom               =@TransferFrom";
                sqlText += " ,Post                     =@Post";


                sqlText += " ,VATRate                     =@VATRate";
                sqlText += " ,VATAmount                     =@VATAmount";
                sqlText += " ,SDRate                     =@SDRate";
                sqlText += " ,SDAmount                     =@SDAmount";

                sqlText += " ,UOMQty                     =@UOMQty";
                sqlText += " ,UOMPrice                     =@UOMPrice";
                sqlText += " ,UOMc                     =@UOMc";
                sqlText += " ,UOMn                     =@UOMn";
                sqlText += " ,Weight                     =@Weight";



                sqlText += " ,CreatedBy                =@CreatedBy";
                sqlText += " ,CreatedOn                =@CreatedOn";
                sqlText += " ,LastModifiedBy           =@LastModifiedBy";
                sqlText += " ,LastModifiedOn           =@LastModifiedOn";

                sqlText += " where      TransferReceiveNo=@TransferReceiveNo";
                sqlText += "  and                ItemNo=@ItemNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceiveLineNo", Detail.ReceiveLineNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Quantity", Detail.Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CostPrice", Detail.CostPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOM", Detail.UOM);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SubTotal", Detail.SubTotal);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Detail.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Detail.TransactionDateTime));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferFrom", Detail.TransferFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Detail.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", Detail.TransferReceiveNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ItemNo", Detail.ItemNo);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SDRate", Detail.SDRate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMQty", Detail.UOMQty);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMPrice", Detail.UOMPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMc", Detail.UOMc);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMn", Detail.UOMn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Weight", Detail.Weight);



                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Detail.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Detail.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));



                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
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
                retResults[1] = "Requested update into TransferReceiveDetails successfully updated ";
                retResults[2] = Detail.ReceiveLineNo;
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
                FileLogger.Log("TransferReceiveDAL", "TransferUpdateToDetail", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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
        #endregion

        #region Basic Methods

        public string[] MultipleSave(string[] Ids, string transactionType, int BranchId, string TransactionDateTime, string CurrentUser = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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

                string ttype = "";

                if (transactionType == "62Out")
                    ttype = "62In";
                else if (transactionType == "61Out")
                    ttype = "61In";

                for (int i = 0; i < Ids.Length; i++)
                {
                    string TransferIssueNo = Ids[i];

                    TransferIssueDAL transferIssueDal = new TransferIssueDAL();
                    TransferReceiveVM vm = new TransferReceiveVM();
                    DataTable transferDetailResult = new DataTable();
                    List<TransferReceiveDetailVM> TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();

                    #region Master data select

                    var _transferIssueVM = new TransferIssueVM();

                    _transferIssueVM.TransferIssueNo = TransferIssueNo;
                    _transferIssueVM.TransactionType = "";// transactionType;
                    _transferIssueVM.IssueDateFrom = "";
                    _transferIssueVM.IssueDateTo = "";
                    _transferIssueVM.Post = "";
                    _transferIssueVM.ReferenceNo = "";

                    DataTable transferResult = transferIssueDal.SearchTransferIssue(_transferIssueVM, currConn, transaction, connVM);

                    if (transferResult.Rows.Count > 0)
                    {

                        string TransferFromNo = transferResult.Rows[0]["TransferIssueNo"].ToString();

                        if (!string.IsNullOrWhiteSpace(TransferFromNo))
                        {
                            vm = new TransferReceiveDAL().SelectAllList(0, new[] { "tr.TransferFromNo" }, new[] { TransferFromNo }, currConn, transaction, connVM).FirstOrDefault();
                        }

                        if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                        {
                            throw new Exception("This Transfer From No Already Exist! Cannot Add!" + " " + "Transfer From No: " + vm.TransferFromNo);
                        }

                        vm = new TransferReceiveVM();
                        ////vm.TransferReceiveNo = string.Empty;
                        vm.TransactionDateTime = TransactionDateTime;
                        vm.TotalAmount = Convert.ToDecimal(transferResult.Rows[0]["TotalAmount"].ToString());
                        vm.SerialNo = "-";
                        vm.ReferenceNo = "-";
                        vm.Comments = "-";
                        vm.CreatedBy = CurrentUser;
                        vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        vm.LastModifiedBy = CurrentUser;
                        vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        vm.TransactionType = ttype;
                        vm.TransferNo = "";
                        vm.TransferFromNo = TransferFromNo;
                        vm.TransferFrom = Convert.ToInt32(transferResult.Rows[0]["BranchId"].ToString());
                        vm.Post = "N";
                        vm.BranchId = BranchId;

                    }

                    #endregion

                    #region Transfer Detail

                    if (!string.IsNullOrEmpty(TransferIssueNo))
                    {
                        transferDetailResult = transferIssueDal.SearchTransferDetail(TransferIssueNo, currConn, transaction, connVM);
                    }// Change 04

                    #region detail list
                    int j = 1;

                    foreach (DataRow item in transferDetailResult.Rows)
                    {
                        TransferReceiveDetailVM detail = new TransferReceiveDetailVM();
                        detail.TransferReceiveNo = string.Empty;
                        detail.ReceiveLineNo = j.ToString();
                        detail.ItemNo = item["ItemNo"].ToString();
                        detail.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                        detail.CostPrice = Convert.ToDecimal(item["CostPrice"].ToString());
                        detail.UOM = item["UOM"].ToString();
                        detail.SubTotal = Convert.ToDecimal(item["SubTotal"].ToString());
                        detail.Comments = "FromTransferReceive";
                        detail.TransactionDateTime = vm.TransactionDateTime;
                        detail.TransferFrom = vm.TransferFrom;
                        detail.UOMQty = Convert.ToDecimal(item["UOMQty"].ToString());
                        detail.UOMn = item["UOMn"].ToString();
                        detail.UOMc = Convert.ToDecimal(item["UOMc"].ToString());
                        detail.UOMPrice = Convert.ToDecimal(item["UOMPrice"].ToString());
                        detail.VATRate = Convert.ToDecimal(item["VATRate"].ToString());
                        detail.VATAmount = Convert.ToDecimal(item["VATAmount"].ToString());
                        detail.SDRate = Convert.ToDecimal(item["SDRate"].ToString());
                        detail.SDAmount = Convert.ToDecimal(item["SDAmount"].ToString());
                        detail.Weight = item["Weight"].ToString();
                        detail.BranchId = BranchId;
                        detail.TransferFromNo = vm.TransferFromNo;

                        TransferReceiveDetailVMs.Add(detail);

                        j = j + 1;
                    }
                    #endregion detail list

                    #endregion

                    retResults = Insert(vm, TransferReceiveDetailVMs, transaction, currConn, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new Exception(retResults[1]);
                    }
                }
                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
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
            #region Result
            return retResults;
            #endregion Result
        }


        public string[] ImportData(DataTable paramTransferIssueDT, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            //string PostStatus = "";
            string newID = "";
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

                TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();
                DataTable dtTransferIssue = new DataTable();
                DataTable dtTransferIssueDetail = new DataTable();
                string TransferIssueNo = "";
                string TransactionType = "";

                string[] cFields = { };
                string[] cValues = { };

                string TransferReceive_JSONString = "";
                string TransferReceiveDetail_JSONString = "";

                TransferReceiveVM vm = new TransferReceiveVM();
                List<TransferReceiveDetailVM> VMs = new List<TransferReceiveDetailVM>();

                List<TransferReceiveVM> masterVMs = new List<TransferReceiveVM>();

                int BranchId = 0;
                int TransferFrom = 0;
                foreach (DataRow dr in paramTransferIssueDT.Rows)
                {
                    BranchId = 0;
                    TransferFrom = 0;
                    vm = new TransferReceiveVM();
                    VMs = new List<TransferReceiveDetailVM>();
                    masterVMs = new List<TransferReceiveVM>();
                    TransferReceive_JSONString = "";
                    TransferReceiveDetail_JSONString = "";
                    dtTransferIssue = new DataTable();
                    dtTransferIssueDetail = new DataTable();
                    TransferIssueNo = "";

                    cFields = new string[] { "ti.ImportIDExcel" };
                    cValues = new string[] { dr["ReferenceNo"].ToString() };

                    dtTransferIssue = _TransferIssueDAL.SelectAll(0, cFields, cValues, currConn, transaction, true, null);
                    TransferIssueNo = dtTransferIssue.Rows[0]["TransferIssueNo"].ToString();
                    TransactionType = dtTransferIssue.Rows[0]["TransactionType"].ToString();
                    BranchId = Convert.ToInt32(dtTransferIssue.Rows[0]["TransferTo"]);
                    TransferFrom = Convert.ToInt32(dtTransferIssue.Rows[0]["BranchId"]);

                    if (TransactionType == "62Out")
                    {
                        TransactionType = "62IN";
                    }
                    else
                    {
                        TransactionType = "61IN";
                    }

                    dtTransferIssueDetail = _TransferIssueDAL.SelectDetail_DT(TransferIssueNo, null, null, currConn, transaction, null);
                    var dtTest = _TransferIssueDAL.SelectDetail(TransferIssueNo, null, null, currConn, transaction, null);

                    #region Assign Master
                    TransferReceive_JSONString = JsonConvert.SerializeObject(dtTransferIssue);
                    masterVMs = JsonConvert.DeserializeObject<List<TransferReceiveVM>>(TransferReceive_JSONString);
                    vm = masterVMs.FirstOrDefault();
                    vm.BranchId = BranchId;
                    vm.TransferFrom = TransferFrom;
                    vm.TransferFromNo = TransferIssueNo;
                    vm.TransactionType = TransactionType;

                    #endregion

                    #region Assign Detail

                    TransferReceiveDetail_JSONString = JsonConvert.SerializeObject(dtTransferIssueDetail);
                    VMs = JsonConvert.DeserializeObject<List<TransferReceiveDetailVM>>(TransferReceiveDetail_JSONString);

                    VMs.Select(c => { c.BranchId = BranchId; return c; }).ToList();
                    VMs.Select(c => { c.TransferFrom = TransferFrom; return c; }).ToList();
                    VMs.Select(c => { c.TransferFromNo = TransferIssueNo; return c; }).ToList();
                    VMs.Select(c => { c.TransactionType = TransactionType; return c; }).ToList();

                    int i = 1;
                    foreach (var dVM in VMs)
                    {
                        dVM.ReceiveLineNo = i.ToString();
                        i++;
                    }

                    #endregion

                    retResults = Insert(vm, VMs, transaction, currConn, null);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }

                }

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Requested insert into TransferIssues successfully Added";
                retResults[2] = "";
                //retResults[3] = "" + PostStatus;
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
                FileLogger.Log("TransferReceiveDAL", "ImportData", ex.ToString() + "\n" + sqlText);

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

        //currConn to VcurrConn 25-Aug-2020
        public string[] Insert(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null, bool CodeGenaration = true)
        {
            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            #region Check user from settings
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;


            #endregion Initializ

            #region Try
            try
            {
                CommonDAL commonDal = new CommonDAL();

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

                #region Find Month Lock
               
                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, currConn, transaction, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion Find Month Lock

                #region MultipleProduct Settings check
                var commonDAL = new CommonDAL();

                var multiple = commonDAL.settings("TransferIssue", "MultipleProduct", currConn, transaction, connVM);
                #endregion

                #region Validation for Header
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Please Check Issue Data and Time");

                }
                #endregion Validation for Header

                #region Fiscal Year Check

                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {
                    #region YearLock
                    sqlText = "";
                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where @transactionYearCheck between PeriodStart and PeriodEnd";

                                                   //" where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                    //BugsBD
                    SqlParameter parameter = new SqlParameter("@transactionYearCheck", SqlDbType.VarChar, 20);
                    parameter.Value = transactionYearCheck;
                    cmdIdExist.Parameters.Add(parameter);



                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock

                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
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
                    #endregion YearNotExist
                }

                #endregion Fiscal Year CHECK

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferReceiveNo) from TransferReceives WHERE TransferReceiveNo=@TransferReceiveNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.CommandTimeout = 500;
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", Master.TransferReceiveNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                }

                #endregion Find Transaction Exist                

                #region ID Create
                if (string.IsNullOrEmpty(Master.TransactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other
                if (CodeGenaration)
                {
                    if (Master.TransactionType.ToLower() == "61In".ToLower())
                    {
                        newID = commonDal.TransactionCode("Transfer", "61In", "TransferReceives", "TransferReceiveNo",
                                                  "TransactionDateTime", Master.TransactionDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    }
                    if (Master.TransactionType.ToLower() == "62In".ToLower())
                    {
                        newID = commonDal.TransactionCode("Transfer", "62In", "TransferReceives", "TransferReceiveNo",
                                                  "TransactionDateTime", Master.TransactionDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    }
                    Master.TransferReceiveNo = newID;

                }

                
                #endregion Purchase ID Create For Other

                #endregion

                #region ID generated completed,Insert new Information in Header

                retResults = TransferInsertToMaster(Master, currConn, transaction, connVM);
                Id = retResults[4];
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    if (multiple == "N")
                    {
                        #region Find Transaction Exist

                        sqlText = "";
                        //sqlText += "select COUNT(TransferReceiveNo) from TransferReceiveDetails WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
                        //sqlText += " AND ItemNo='" + Item.ItemNo + "'";

                        sqlText += "select COUNT(TransferReceiveNo) from TransferReceiveDetails WHERE TransferReceiveNo = @TransferReceiveNo";
                        sqlText += " AND ItemNo = @ItemNo";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);


                        //BugsBD
                        SqlParameter parameter3 = new SqlParameter("@TransferReceiveNo", SqlDbType.VarChar, 250);
                        parameter3.Value = Master.TransferReceiveNo;
                        cmdFindId.Parameters.Add(parameter3);

                        parameter3 = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                        parameter3.Value = Item.ItemNo;
                        cmdFindId.Parameters.Add(parameter3);


                        cmdFindId.Transaction = transaction;
                        IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                        }

                        #endregion Find Transaction Exist

                    }

                    #region Insert only DetailTable
                    Item.TransferReceiveNo = Master.TransferReceiveNo;
                    Item.TransferFrom = Master.TransferFrom;
                    Item.TransactionDateTime = Master.TransactionDateTime;
                    Item.Post = Master.Post;
                    Item.CreatedBy = Master.CreatedBy;
                    Item.CreatedOn = Master.CreatedOn;
                    Item.TransactionType = Master.TransactionType;
                    Item.LastModifiedBy = Master.LastModifiedBy;
                    Item.LastModifiedOn = Master.LastModifiedOn;
                    retResults = TransferInsertToDetail(Item, currConn, transaction, connVM);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }

                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #region Update only DetailTable

                //////sqlText = "";
                //////sqlText += @" update Transfers set  Post='Y'  where  TransferNo ='" + Master.TransferNo + "' ";
                //////sqlText += @" update TransferDetails set  Post='Y'  where  TransferNo ='" + Master.TransferNo + "' ";

                //////SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                //////transResult = (int)cmd.ExecuteNonQuery();


                //////if (transResult <= 0)
                //////{
                //////    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSuccessfully);
                //////}
                #endregion Update only DetailTable

                #region Update PeriodId and Fiscal Year

                sqlText = "";
                sqlText += @"

update  TransferReceives                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferReceiveNo = @TransferReceiveNo


update  TransferReceiveDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferReceiveNo = @TransferReceiveNo


update TransferReceives set FiscalYear=fyr.CurrentYear
From TransferReceives inv
left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@TransferReceiveNo", Master.TransferReceiveNo);

                transResult = cmdUpdate.ExecuteNonQuery();
                #endregion

                #region return Current ID and Post Status

                sqlText = "";
                //sqlText = sqlText + "select distinct  Post from dbo.TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "'";
                sqlText = sqlText + "select distinct  Post from dbo.TransferReceives WHERE TransferReceiveNo = @TransferReceiveNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);


                //BugsBD
                SqlParameter parameter4 = new SqlParameter("@TransferReceiveNo", SqlDbType.VarChar, 250);
                parameter4.Value = Master.TransferReceiveNo;
                cmdIPS.Parameters.Add(parameter4);


                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgUnableCreatID);
                }
                #endregion Prefetch

                #region Update Source / Transfer Issue


                //TransferIssueVM varTransferIssueVM = new TransferIssueVM();

                //TransferIssueDAL transferIssueDAl = new TransferIssueDAL();

                //string[] cFields = { "TransferIssueNo" };
                //string[] cVals = { Master.TransferFromNo };
                //varTransferIssueVM = transferIssueDAl.SelectAllList(0, cFields, cVals, currConn, transaction, connVM)
                //    .FirstOrDefault();
                ////varTransferIssueVM.Post = "Y";
                //varTransferIssueVM.IsTransfer = "Y";
                //List<TransferIssueDetailVM> TransferIssueDetailVMs = transferIssueDAl.SelectDetail(Master.TransferFromNo, null, null, currConn, transaction, null);
                //transferIssueDAl.Post(varTransferIssueVM, TransferIssueDetailVMs, currConn, transaction, null);

                string updateTransferFlag = "Update  TransferIssues set   IsTransfer='Y'    WHERE TransferIssueNo=@TransferIssueNo ";
                updateTransferFlag += " Update  TransferIssueTrackings set   IsTransfer='Y'    WHERE TransferIssueNo=@TransferIssueNo ";

                SqlCommand transferCommand = new SqlCommand(updateTransferFlag, currConn, transaction);

                transferCommand.Parameters.AddWithValue("@TransferIssueNo", Master.TransferFromNo);
                transferCommand.ExecuteNonQuery();

                #endregion

                #region Master Setting Update

                commonDal.UpdateProcessFlag(Master.TransferReceiveNo, Master.TransactionDateTime, currConn, transaction, connVM);

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
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
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
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                //if (transaction != null)
                //{
                //    transaction.Rollback();
                //}

                FileLogger.Log("TransferReceiveDAL", "Insert", ex.ToString());

                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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

        //currConn to VcurrConn 07-Dec-2023
        public string[] Update(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
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
            //DateTime MinDate = DateTime.MinValue;
            //DateTime MaxDate = DateTime.MaxValue;
            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                CommonDAL commonDal = new CommonDAL();

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                else if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                #endregion Find Month Lock

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, "Please Check Invoice Data and Time");
                }

                #endregion Validation for Header

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

                #region Current Status

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'

select TransferReceiveNo, Post from TransferReceives
where 1=1 
and TransferReceiveNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferReceiveNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                #endregion

                #region Current Items
                DataTable dtCurrentItems = new DataTable();

                if (currentPostStatus == "Y")
                {
                    sqlText = "";
                    sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'


select ItemNo, TransferReceiveNo from TransferReceiveDetails
where 1=1 
and TransferReceiveNo=@InvoiceNo

";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferReceiveNo);

                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtCurrentItems);

                }
                #endregion

                #endregion

                #region Fiscal Year Check

                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                TransferReceiveVM previousVm = SelectAllList(0, new[] { "tr.TransferReceiveNo" }, new[] { Master.TransferReceiveNo }, currConn, transaction, connVM).FirstOrDefault() ?? Master;

                List<TransferReceiveDetailVM> previousDetailVMs = SelectDetail(Master.TransferReceiveNo, null, null, currConn, transaction, connVM);

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferReceiveNo) from TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                string settingValue = commonDal.settingValue("EntryProcess", "UpdateOnPost", connVM, currConn, transaction);

                if (settingValue != "Y")
                {
                    if (Master.Post == "Y")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.ThisTransactionWasPosted);
                    }

                }

                retResults = TransferUpdateToMaster(Master, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }

                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(TransferReceiveNo) from TransferReceiveDetails WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable

                        retResults = TransferInsertToDetail(Item, currConn, transaction, connVM);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable

                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        Item.TransactionType = Master.TransactionType;

                        Item.Post = Master.Post;
                        Item.CreatedBy = Master.CreatedBy;
                        Item.CreatedOn = Master.CreatedOn;
                        Item.LastModifiedBy = Master.LastModifiedBy;
                        Item.LastModifiedOn = Master.LastModifiedOn;
                        Item.TransferReceiveNo = Master.TransferReceiveNo;
                        Item.ItemNo = Item.ItemNo;

                        retResults = TransferUpdateToDetail(Item, currConn, transaction, connVM);

                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from TransferReceiveDetails WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "'";

                dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM TransferReceiveDetails  ";
                        sqlText += " WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region Update PeriodId and Fiscal Year

                sqlText = "";
                sqlText += @"

update  TransferReceives                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferReceiveNo = @TransferReceiveNo


update  TransferReceiveDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferReceiveNo = @TransferReceiveNo


update TransferReceives set FiscalYear=fyr.CurrentYear
From TransferReceives inv
left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@TransferReceiveNo", Master.TransferReceiveNo);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableCreatID);
                }

                #endregion Prefetch

                #region Update Product Stock

                if (currentPostStatus == "Y" && dtCurrentItems != null && dtCurrentItems.Rows.Count > 0)
                {
                    ProductDAL productDal = new ProductDAL();
                    List<string> transactionTypes = new List<string>() { "62in", "61in" };

                    if (transactionTypes.Contains(Master.TransactionType.ToLower()))
                    {
                        DataTable dtItemNo = previousDetailVMs.Select(x => new { x.ItemNo, Quantity = x.UOMQty * -1 }).ToList().ToDataTable();
                        productDal.Product_IN_OUT(new ParameterVM() { dt = dtItemNo, BranchId = Master.BranchId }, currConn, transaction, connVM);
                    }
                    else
                    {

                        ResultVM rVM = new ResultVM();

                        ParameterVM paramVM = new ParameterVM();
                        paramVM.BranchId = Master.BranchId;
                        paramVM.InvoiceNo = Master.TransferReceiveNo;
                        paramVM.dt = dtCurrentItems;

                        paramVM.IDs = new List<string>();

                        foreach (DataRow dr in paramVM.dt.Rows)
                        {
                            paramVM.IDs.Add(dr["ItemNo"].ToString());
                        }

                        if (paramVM.IDs.Count > 0)
                        {
                            rVM = _ProductDAL.Product_Stock_Update(paramVM, currConn, transaction, connVM, UserId);
                        }
                    }
                }

                #endregion

                commonDal.UpdateProcessFlag(Master.TransferReceiveNo, previousVm.TransactionDateTime, currConn, transaction, connVM);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.receiveMsgUpdateSuccessfully;
                retResults[2] = Master.TransferReceiveNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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
        #region
//        public string[] Update(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "")
//        {
//            #region Initializ
//            string[] retResults = new string[4];
//            retResults[0] = "Fail";
//            retResults[1] = "Fail";
//            retResults[2] = "";
//            retResults[3] = "";

//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            int transResult = 0;
//            string sqlText = "";
//            //DateTime MinDate = DateTime.MinValue;
//            //DateTime MaxDate = DateTime.MaxValue;
//            string PostStatus = "";

//            #endregion Initializ

//            #region Try
//            try
//            {

//                #region Find Month Lock

//                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
//                string[] vValues = { PeriodName };
//                string[] vFields = { "PeriodName" };
//                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
//                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, null, null, connVM).FirstOrDefault();

//                if (varFiscalYearVM.VATReturnPost == "Y")
//                {
//                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
//                }

//                else if (varFiscalYearVM == null)
//                {
//                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
//                }

//                #endregion Find Month Lock

//                #region Validation for Header

//                if (Master == null)
//                {
//                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
//                }
//                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
//                {
//                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, "Please Check Invoice Data and Time");
//                }

//                #endregion Validation for Header

//                #region open connection and transaction

//                currConn = _dbsqlConnection.GetConnection(connVM);
//                if (currConn.State != ConnectionState.Open)
//                {
//                    currConn.Open();
//                }

//                #region Add BOMId

//                CommonDAL commonDal = new CommonDAL();

//                #endregion Add BOMId

//                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameUpdate);

//                #endregion open connection and transaction

//                #region Current Status

//                #region Post Status

//                string currentPostStatus = "N";

//                sqlText = "";
//                sqlText = @"
//----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'
//
//select TransferReceiveNo, Post from TransferReceives
//where 1=1 
//and TransferReceiveNo=@InvoiceNo
//
//";
//                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
//                cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferReceiveNo);

//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                if (dt != null && dt.Rows.Count > 0)
//                {
//                    currentPostStatus = dt.Rows[0]["Post"].ToString();
//                }

//                #endregion

//                #region Current Items
//                DataTable dtCurrentItems = new DataTable();

//                if (currentPostStatus == "Y")
//                {
//                    sqlText = "";
//                    sqlText = @"
//----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'
//
//
//select ItemNo, TransferReceiveNo from TransferReceiveDetails
//where 1=1 
//and TransferReceiveNo=@InvoiceNo
//
//";

//                    cmd = new SqlCommand(sqlText, currConn, transaction);
//                    cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferReceiveNo);

//                    da = new SqlDataAdapter(cmd);
//                    da.Fill(dtCurrentItems);

//                }
//                #endregion

//                #endregion

//                #region Fiscal Year Check

//                string transactionDate = Master.TransactionDateTime;
//                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
//                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
//                {

//                    #region YearLock
//                    sqlText = "";

//                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
//                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

//                    DataTable dataTable = new DataTable("ProductDataT");
//                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
//                    cmdIdExist.Transaction = transaction;
//                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
//                    reportDataAdapt.Fill(dataTable);

//                    if (dataTable == null)
//                    {
//                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
//                    }

//                    else if (dataTable.Rows.Count <= 0)
//                    {
//                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
//                    }
//                    else
//                    {
//                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
//                        {
//                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
//                        }
//                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
//                        {
//                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
//                        }
//                    }
//                    #endregion YearLock
//                    #region YearNotExist
//                    sqlText = "";
//                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

//                    DataTable dtYearNotExist = new DataTable("ProductDataT");

//                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
//                    cmdYearNotExist.Transaction = transaction;
//                    //countId = (int)cmdIdExist.ExecuteScalar();

//                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
//                    YearNotExistDataAdapt.Fill(dtYearNotExist);

//                    if (dtYearNotExist == null)
//                    {
//                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.msgFiscalYearNotExist);
//                    }

//                    else if (dtYearNotExist.Rows.Count < 0)
//                    {
//                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.msgFiscalYearNotExist);
//                    }
//                    else
//                    {
//                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
//                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
//                        {
//                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.msgFiscalYearNotExist);
//                        }
//                    }
//                    #endregion YearNotExist

//                }


//                #endregion Fiscal Year CHECK

//                TransferReceiveVM previousVm = SelectAllList(0, new[] { "tr.TransferReceiveNo" }, new[] { Master.TransferReceiveNo }, currConn, transaction, connVM).FirstOrDefault() ?? Master;

//                List<TransferReceiveDetailVM> previousDetailVMs = SelectDetail(Master.TransferReceiveNo, null, null, currConn, transaction, connVM);

//                #region Find ID for Update

//                sqlText = "";
//                sqlText = sqlText + "select COUNT(TransferReceiveNo) from TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
//                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
//                cmdFindIdUpd.Transaction = transaction;
//                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

//                if (IDExist <= 0)
//                {
//                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableFindExistID);
//                }

//                #endregion Find ID for Update

//                #region ID check completed,update Information in Header

//                #region update Header

//                string settingValue = commonDal.settingValue("EntryProcess", "UpdateOnPost", connVM, currConn, transaction);

//                if (settingValue != "Y")
//                {
//                    if (Master.Post == "Y")
//                    {
//                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.ThisTransactionWasPosted);
//                    }

//                }

//                retResults = TransferUpdateToMaster(Master, currConn, transaction, connVM);

//                if (retResults[0] != "Success")
//                {
//                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
//                }
//                #endregion update Header

//                #endregion ID check completed,update Information in Header

//                #region Update into Details(Update complete in Header)
//                #region Validation for Detail

//                if (Details.Count() < 0)
//                {
//                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
//                }

//                #endregion Validation for Detail

//                #region Update Detail Table

//                foreach (var Item in Details.ToList())
//                {
//                    #region Find Transaction Mode Insert or Update

//                    sqlText = "";
//                    sqlText += "select COUNT(TransferReceiveNo) from TransferReceiveDetails WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
//                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
//                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
//                    cmdFindId.Transaction = transaction;
//                    IDExist = (int)cmdFindId.ExecuteScalar();

//                    if (IDExist <= 0)
//                    {
//                        // Insert
//                        #region Insert only DetailTable

//                        retResults = TransferInsertToDetail(Item, currConn, transaction, connVM);
//                        if (retResults[0] != "Success")
//                        {
//                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
//                        }
//                        #endregion Insert only DetailTable

//                    }
//                    else
//                    {
//                        //Update

//                        #region Update only DetailTable

//                        Item.TransactionType = Master.TransactionType;

//                        Item.Post = Master.Post;
//                        Item.CreatedBy = Master.CreatedBy;
//                        Item.CreatedOn = Master.CreatedOn;
//                        Item.LastModifiedBy = Master.LastModifiedBy;
//                        Item.LastModifiedOn = Master.LastModifiedOn;
//                        Item.TransferReceiveNo = Master.TransferReceiveNo;
//                        Item.ItemNo = Item.ItemNo;

//                        retResults = TransferUpdateToDetail(Item, currConn, transaction, connVM);

//                        if (retResults[0] != "Success")
//                        {
//                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
//                        }
//                        #endregion Update only DetailTable

//                    }

//                    #endregion Find Transaction Mode Insert or Update
//                }//foreach (var Item in Details.ToList())

//                #region Remove row
//                sqlText = "";
//                sqlText += " SELECT  distinct ItemNo";
//                sqlText += " from TransferReceiveDetails WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "'";

//                dt = new DataTable("Previous");
//                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
//                cmdRIFB.Transaction = transaction;
//                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
//                dta.Fill(dt);
//                foreach (DataRow pItem in dt.Rows)
//                {
//                    var p = pItem["ItemNo"].ToString();

//                    //var tt= Details.Find(x => x.ItemNo == p);
//                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
//                    if (tt == 0)
//                    {
//                        sqlText = "";
//                        sqlText += " delete FROM TransferReceiveDetails  ";
//                        sqlText += " WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
//                        sqlText += " AND ItemNo='" + p + "'";
//                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
//                        cmdInsDetail.Transaction = transaction;
//                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
//                    }

//                }
//                #endregion Remove row

//                #endregion Update Detail Table

//                #endregion  Update into Details(Update complete in Header)

//                #region Update PeriodId and Fiscal Year

//                sqlText = "";
//                sqlText += @"
//
//update  TransferReceives                             
//set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
//WHERE TransferReceiveNo = @TransferReceiveNo
//
//
//update  TransferReceiveDetails                             
//set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
//WHERE TransferReceiveNo = @TransferReceiveNo
//
//
//update TransferReceives set FiscalYear=fyr.CurrentYear
//From TransferReceives inv
//left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID
//
//";

//                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
//                cmdUpdate.Parameters.AddWithValue("@TransferReceiveNo", Master.TransferReceiveNo);

//                transResult = cmdUpdate.ExecuteNonQuery();

//                #endregion

//                #region return Current ID and Post Status

//                sqlText = "";
//                sqlText = sqlText + "select distinct Post from TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "'";
//                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
//                cmdIPS.Transaction = transaction;
//                PostStatus = (string)cmdIPS.ExecuteScalar();
//                if (string.IsNullOrEmpty(PostStatus))
//                {
//                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableCreatID);
//                }

//                #endregion Prefetch

//                #region Update Product Stock

//                if (currentPostStatus == "Y" && dtCurrentItems != null && dtCurrentItems.Rows.Count > 0)
//                {
//                    ProductDAL productDal = new ProductDAL();
//                    List<string> transactionTypes = new List<string>() { "62in", "61in" };

//                    if (transactionTypes.Contains(Master.TransactionType.ToLower()))
//                    {
//                        DataTable dtItemNo = previousDetailVMs.Select(x => new { x.ItemNo, Quantity = x.UOMQty * -1 }).ToList().ToDataTable();
//                        productDal.Product_IN_OUT(new ParameterVM() { dt = dtItemNo, BranchId = Master.BranchId }, currConn, transaction, connVM);
//                    }
//                    else
//                    {

//                        ResultVM rVM = new ResultVM();

//                        ParameterVM paramVM = new ParameterVM();
//                        paramVM.BranchId = Master.BranchId;
//                        paramVM.InvoiceNo = Master.TransferReceiveNo;
//                        paramVM.dt = dtCurrentItems;

//                        paramVM.IDs = new List<string>();

//                        foreach (DataRow dr in paramVM.dt.Rows)
//                        {
//                            paramVM.IDs.Add(dr["ItemNo"].ToString());
//                        }

//                        if (paramVM.IDs.Count > 0)
//                        {
//                            rVM = _ProductDAL.Product_Stock_Update(paramVM, currConn, transaction, connVM, UserId);
//                        }
//                    }
//                }

//                #endregion

//                commonDal.UpdateProcessFlag(Master.TransferReceiveNo, previousVm.TransactionDateTime, currConn, transaction, connVM);

//                #region Commit

//                if (transaction != null)
//                {
//                    //if (transResult > 0)
//                    //{
//                    transaction.Commit();
//                    //}

//                }

//                #endregion Commit

//                #region SuccessResult

//                retResults[0] = "Success";
//                retResults[1] = MessageVM.receiveMsgUpdateSuccessfully;
//                retResults[2] = Master.TransferReceiveNo;
//                retResults[3] = PostStatus;
//                #endregion SuccessResult

//            }
//            #endregion Try

//            #region Catch and Finall
//            //catch (SqlException sqlex)
//            //{
//            //    transaction.Rollback();
//            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            //    //throw sqlex;
//            //}
//            catch (Exception ex)
//            {
//                retResults = new string[5];
//                retResults[0] = "Fail";
//                retResults[1] = ex.Message;
//                retResults[2] = "0";
//                retResults[3] = "N";
//                retResults[4] = "0";
//                if (currConn != null)
//                {
//                    transaction.Rollback();
//                }
//                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//                throw new ArgumentNullException("", ex.Message.ToString());
//                //throw ex;
//            }
//            finally
//            {
//                if (currConn != null)
//                {
//                    if (currConn.State == ConnectionState.Open)
//                    {
//                        currConn.Close();
//                    }
//                }

//            }
//            #endregion Catch and Finall

//            #region Result
//            return retResults;
//            #endregion Result

//        }
        #endregion

        public string[] Post(TransferReceiveVM Master, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

            string[] cFields = { "TransferReceiveNo" };
            string[] cVals = { Master.TransferReceiveNo };
            Master = SelectAllList(0, cFields, cVals, null, null, connVM).FirstOrDefault();
            Master.Post = "Y";
            List<TransferReceiveDetailVM> Details = SelectDetail(Master.TransferReceiveNo, null, null, null, null, connVM);
            #endregion Initializ

            #region Try

            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion Find Month Lock

                CommonDAL commonDal = new CommonDAL();

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgCheckDatePost);

                }

                #endregion Validation for Header

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

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'

select TransferReceiveNo, Post from TransferReceives
where 1=1 
and TransferReceiveNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferReceiveNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                if (currentPostStatus == "Y")
                {
                    throw new Exception("This Invoice Already Posted! Invoice No: " + Master.TransferReceiveNo);
                }

                #endregion

                #region Fiscal Year Check

                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
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
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferReceiveNo) from TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update TransferReceives set  ";
                sqlText += " LastModifiedBy='" + Master.LastModifiedBy + "' ,";
                sqlText += " LastModifiedOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,";
                sqlText += " Post= '" + Master.Post + "' ";
                sqlText += " where  TransferReceiveNo= '" + Master.TransferReceiveNo + "' ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSuccessfully);
                }
                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
              
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                }

                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(TransferReceiveNo) from TransferReceiveDetails WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "' ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update TransferReceiveDetails set ";
                        sqlText += " Post='" + Master.Post + "'";
                        sqlText += " where  TransferReceiveNo ='" + Master.TransferReceiveNo + "' ";
                        sqlText += " and ItemNo='" + Item.ItemNo + "'";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }
                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from TransferReceives WHERE TransferReceiveNo='" + Master.TransferReceiveNo + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSelect);
                }

                #endregion Prefetch

                #region TransferReceive_Product_IN_OUT

                ResultVM rVM = new ResultVM();

                ParameterVM paramVM = new ParameterVM();
                paramVM.InvoiceNo = Master.TransferReceiveNo;

                rVM = TransferReceive_Product_IN_OUT(paramVM, currConn, transaction, connVM);

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.receiveMsgSuccessfullyPost;
                retResults[2] = Master.TransferReceiveNo;
                retResults[3] = PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                //if (currConn != null)
                //{
                //    if (currConn.State == ConnectionState.Open)
                //    {
                //        currConn.Close();
                //    }
                //}
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


        public ResultVM TransferReceive_Product_IN_OUT(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

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

                #region Invoice Status

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'


select TransferReceiveNo, BranchId, Post from TransferReceives
where 1=1 
and TransferReceiveNo=@InvoiceNo
";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.InvoiceNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    paramVM.BranchId = Convert.ToInt32(dt.Rows[0]["BranchId"]);
                }

                #endregion

                #region Update Product Stock

                #region SQL Text

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1I-001/0001/1220'


select ItemNo, TransferReceiveNo, TransactionType, UOMQty as Quantity, Post from TransferReceiveDetails
where 1=1 
and TransferReceiveNo=@InvoiceNo

";
                #endregion

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.InvoiceNo);

                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                paramVM.dt = dt;

                rVM = _ProductDAL.Product_IN_OUT(paramVM, currConn, transaction, connVM);

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Product Stock Updated Successfully!";

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

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


        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion Initializ
            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameUpdate);


                #endregion open connection and transaction

                for (int i = 0; i < Ids.Length - 1; i++)
                {
                    TransferReceiveVM Master = SelectAllList(Convert.ToInt32(Ids[i]), null, null, currConn, transaction, connVM).FirstOrDefault();
                    Master.Post = "Y";

                    //List<TransferReceiveDetailVM> Details = SelectDetail(Master.TransferReceiveNo, null, null, currConn, transaction);
                    retResults = Post(Master);

                    if (retResults[0] != "Success")
                    {
                        throw new Exception();
                    }
                }
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

            }
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                throw ex;
            }
            finally
            {

            }
            #region Result
            return retResults;
            #endregion Result
        }

        #endregion

        //public DataTable SearchTransferReceive(string TransferReceiveNo, string DateTimeFrom,
        //    string DateTimeTo, string TransactionType, string Post)

        #region Search Methods

        public DataTable SearchTransferReceive(TransferReceiveVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferReceive");

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
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();

                #endregion Add BOMId
                #endregion open connection and transaction

                #region SQL Statement

                sqlText = " ";
                sqlText = @" 
SELECT 
tr.TransferReceiveNo
,tr.Id
,ISNULL(tr.FiscalYear,0) FiscalYear
, convert (varchar,tr.TransactionDateTime,120)ReceiveDateTime
,isnull(tr.TotalAmount     ,'0')TotalAmount
,isnull(tr.TransactionType ,'N/A')TransactionType
,isnull(tr.SerialNo        ,'N/A')SerialNo
,isnull(tr.TransferFromNo        ,'N/A')TransferFromNo
,isnull(tr.TransferNo        ,'N/A')TransferNo
,isnull(tr.Comments        ,'N/A')Comments
,isnull(tr.Post            ,'N')Post
,isnull(tr.ReferenceNo     ,'N/A')ReferenceNo
,isnull(tr.TransferFrom      ,0)TransferFrom
,isnull(tr.BranchId      ,0)BranchId
,isnull(br.BranchName,'N/A') BranchName
,isnull(tr.TotalVATAmount       ,0)TotalVATAmount
,isnull(tr.TotalSDAmount       ,0)TotalSDAmount
,isnull(tr.CreatedBy       ,'N/A')CreatedBy
,isnull(tr.CreatedOn       ,'19000101')CreatedOn
,isnull(tr.LastModifiedBy  ,'N/A')LastModifiedBy
,isnull(tr.LastModifiedOn  ,'19000101')LastModifiedOn
                            FROM         dbo.TransferReceives tr
							LEFT OUTER JOIN BranchProfiles br on tr.TransferFrom = br.BranchId
                            WHERE 1=1
                            AND (tr.TransferReceiveNo  LIKE '%' +  @TransferReceiveNo   + '%' OR @TransferReceiveNo IS NULL) 
                            AND (tr.TransferFromNo  LIKE '%' +  @TransferFromNo   + '%' OR @TransferFromNo IS NULL) 
                            AND (tr.TransferNo  LIKE '%' +  @TransferNo   + '%' OR tr.TransferNo IS NULL) 
                            AND (tr.TransactionDateTime>= @DateTimeFrom OR @DateTimeFrom IS NULL)
                            AND (tr.TransactionDateTime <dateadd(d,1, @DateTimeTo) OR @DateTimeTo IS NULL)
                            and (tr.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            AND (tr.transactionType=@transactionType) 
                            AND (tr.BranchId=@BranchId) 
                            AND (tr.TransferFrom=@ReceiveFrom) 
                            ";
                #endregion
                if (vm.TransferFrom == 0)
                {
                    sqlText = sqlText.Replace("=@ReceiveFrom", ">@ReceiveFrom");
                }
                #region SQL Command

                SqlCommand objCommHeader = new SqlCommand();
                objCommHeader.Connection = currConn;

                objCommHeader.CommandText = sqlText;
                objCommHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommHeader.Parameters.Contains("@Post"))
                { objCommHeader.Parameters.AddWithValue("@Post", vm.Post); }
                else { objCommHeader.Parameters["@Post"].Value = vm.Post; }

                if (!objCommHeader.Parameters.Contains("@TransferReceiveNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferReceiveNo", vm.TransferReceiveNo); }
                else { objCommHeader.Parameters["@TransferReceiveNo"].Value = vm.TransferReceiveNo; }

                if (!objCommHeader.Parameters.Contains("@TransferFromNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferFromNo", vm.TransferFromNo); }
                else { objCommHeader.Parameters["@TransferFromNo"].Value = vm.TransferFromNo; }

                if (!objCommHeader.Parameters.Contains("@TransferNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferNo", vm.TransferNo); }
                else { objCommHeader.Parameters["@TransferNo"].Value = vm.TransferNo; }

                if (vm.ReceiveDateFrom == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", "1753-01-01 00:00:00"); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = "1753-01-01 00:00:00"; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", vm.ReceiveDateFrom); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = vm.ReceiveDateFrom; }
                }
                if (vm.ReceiveDateTo == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", "9998-12-31 00:00:00"); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = "9998-12-31 00:00:00"; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", vm.ReceiveDateTo); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = vm.ReceiveDateTo; }
                }
                // IssueFromDate(MinDate) 1753-01-01 00:00:00 
                // IssueToDate (MaxDate)  9998-12-31 00:00:00

                if (!objCommHeader.Parameters.Contains("@transactionType"))
                { objCommHeader.Parameters.AddWithValue("@transactionType", vm.TransactionType); }
                else { objCommHeader.Parameters["@transactionType"].Value = vm.TransactionType; }

                if (!objCommHeader.Parameters.Contains("@BranchId"))
                { objCommHeader.Parameters.AddWithValue("@BranchId", vm.BranchId); }
                else { objCommHeader.Parameters["@BranchId"].Value = vm.BranchId; }


                if (!objCommHeader.Parameters.Contains("@ReceiveFrom"))
                { objCommHeader.Parameters.AddWithValue("@ReceiveFrom", vm.TransferFrom); }
                else { objCommHeader.Parameters["@ReceiveFrom"].Value = vm.TransferFrom; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }

        public DataTable SearchTransferDetail(string TransferReceiveNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferDetail");

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
tid.TransferReceiveNo
,tid.ReceiveLineNo
,tid.ItemNo
,tid.Quantity
,tid.CostPrice
,tid.UOM
,tid.SubTotal
,tid.Comments
,tid.TransactionType
,tid.TransactionDateTime ReceiveDateTime
,isnull(tid.TransferFrom, 0)TransferFrom
,isnull(br.BranchName,'N/A') BranchName
,tid.Post
,isnull(tid.UOMQty,isnull(tid.Quantity,0))UOMQty
,isnull(tid.UOMn,tid.UOM)UOMn
,isnull(tid.UOMc,1)UOMc
,isnull(tid.UOMPrice,isnull(tid.CostPrice,0))UOMPrice

,isnull(tid.VATRate,0)VATRate
,isnull(tid.VATAmount,0)VATAmount
,isnull(tid.SDRate ,0)SDRate 
,isnull(tid.SDAmount,0)SDAmount
,isnull(tid.Weight,'')Weight


,tid.CreatedBy
,tid.CreatedOn
,tid.LastModifiedBy
,tid.LastModifiedOn,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock,
Products.ProductName
from TransferReceiveDetails tid
left outer join Products on tid.ItemNo=Products.ItemNo
LEFT OUTER JOIN BranchProfiles br on tid.TransferFrom = br.BranchId
                            WHERE 1=1
                            and (TransferReceiveNo = @TransferReceiveNo ) 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommTransferReceiveDetail = new SqlCommand();
                objCommTransferReceiveDetail.Connection = currConn;

                objCommTransferReceiveDetail.CommandText = sqlText;
                objCommTransferReceiveDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommTransferReceiveDetail.Parameters.Contains("@TransferReceiveNo"))
                { objCommTransferReceiveDetail.Parameters.AddWithValue("@TransferReceiveNo", TransferReceiveNo); }
                else { objCommTransferReceiveDetail.Parameters["@TransferReceiveNo"].Value = TransferReceiveNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransferReceiveDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }

        public DataSet FormLoad(UOMVM UOMvm, ProductVM Productvm, string Name, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet ds = new DataSet("UOM_Product_Branch");
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sqlText
                sqlText = @"
--------------------------------UOM--------------------------------
select DISTINCT UOMId,
UOMFrom,
UOMTo,
Convertion,
CTypes,ActiveStatus
FROM UOMs	
WHERE 	(UOMId  LIKE '%' +  @UOMId + '%' OR @UOMId IS NULL) 
and (UOMFrom LIKE '%' + @UOMFrom + '%' OR @UOMFrom IS NULL)	
and (UOMTo LIKE '%' + @UOMTo + '%' OR @UOMTo IS NULL)
and (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
order by UOMFrom
--------------------------------Product--------------------------------
SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.IsVATRate,'N')IsVATRate,
                                    isnull(Products.IsSDRate,'N')IsSDRate
FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";
                sqlText += "  WHERE (Products.ItemNo LIKE '%' +'" + Productvm.ItemNo + "'+ '%'  OR Products.ItemNo IS NULL) AND";
                sqlText += " (Products.CategoryID  LIKE '%'  +'" + Productvm.CategoryID + "'+  '%' OR Products.CategoryID IS NULL) ";
                sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + Productvm.IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)  ";
                sqlText += " AND (ProductCategories.CategoryName LIKE '%'+'" + Productvm.CategoryName + "'+  '%' OR ProductCategories.CategoryName IS NULL)  ";
                sqlText += " AND (Products.ActiveStatus LIKE '%'+'" + Productvm.ActiveStatus + "'+  '%' OR Products.ActiveStatus IS NULL)";
                sqlText += " AND (Products.Trading LIKE '%' +'" + Productvm.Trading + "'+  '%' OR Products.Trading IS NULL)";
                sqlText += " AND (Products.NonStock LIKE '%' +'" + Productvm.NonStock + "'+  '%' OR Products.NonStock IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%'+'" + Productvm.ProductCode + "'+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";

                sqlText += @"
--------------------------------Branch--------------------------------
SELECT Id, Name, DBName,isnull(BrAddress,'-')BrAddress
FROM Branchs
WHERE 	(Name  LIKE '%' +  @Name + '%' OR @Name IS NULL) 
order by Name
";
                #endregion sqlText

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                #region Parameters for UOM
                if (!objComm.Parameters.Contains("@UOMId"))
                {
                    objComm.Parameters.AddWithValue("@UOMId", UOMvm.UOMID);
                }
                else
                {
                    objComm.Parameters["@UOMId"].Value = UOMvm.UOMID;
                }
                if (!objComm.Parameters.Contains("@UOMFrom"))
                {
                    objComm.Parameters.AddWithValue("@UOMFrom", UOMvm.UOMFrom);
                }
                else
                {
                    objComm.Parameters["@UOMFrom"].Value = UOMvm.UOMFrom;
                }
                if (!objComm.Parameters.Contains("@ActiveStatus"))
                {
                    objComm.Parameters.AddWithValue("@ActiveStatus", UOMvm.ActiveStatus);
                }
                else
                {
                    objComm.Parameters["@ActiveStatus"].Value = UOMvm.ActiveStatus;
                }
                if (!objComm.Parameters.Contains("@UOMTo"))
                {
                    objComm.Parameters.AddWithValue("@UOMTo", UOMvm.UOMTo);
                }
                else
                {
                    objComm.Parameters["@UOMTo"].Value = UOMvm.UOMTo;
                }
                #endregion Parameters for Branch
                #region Parameters for Branch
                if (!objComm.Parameters.Contains("@Name"))
                {
                    objComm.Parameters.AddWithValue("@Name", Name);
                }
                else
                {
                    objComm.Parameters["@Name"].Value = Name;
                }
                #endregion Parameters for Branch
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(ds);
            }
            #region Catch
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
            return ds;
        }

        #endregion

        public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

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
                #endregion open connection and transaction


                sqlText = @"
SELECT
      ti.TransferReceiveNo ID
	  , bp.BranchCode
      ,convert(varchar(50),ti.[TransactionDateTime],111) Transaction_Date
      ,convert(varchar(50),ti.[TransactionDateTime],108) Transaction_Time
      --,cast(ti.[TransactionDateTime] as varchar(100)) TransactionDateTime
	  ,pd.ProductCode 
	  , pd.ProductName
	  , tid.Quantity
	  , tid.UOM
      ,tid.Weight
	  ,tid.CostPrice
	  ,ti.[ReferenceNo]
	  ,ti.[TransactionType]
      ,ti.[Post]
	  , tid.VATRate VAT_Rate
      ,ti.[Comments]
	  , tid.Comments CommentsD

  FROM TransferReceives ti left outer join TransferReceiveDetails tid

  on ti.TransferReceiveNo = tid.TransferReceiveNo left outer join BranchProfiles bp
  on ti.BranchId = bp.BranchID left outer join Products pd 
  on tid.ItemNo = pd.ItemNo

  where ti.TransferReceiveNo in ( ";

                var len = invoiceList.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + invoiceList[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["TransactionType"] =
                        table.Rows[i]["TransactionType"].ToString() == "61In" ? "RAW" : "FINISH";
                }
                //Master.TransactionType = Master.TransactionType == "RAW" ? "61Out" : "62Out";
                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                throw ex;
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
            #endregion
        }

    }
}
