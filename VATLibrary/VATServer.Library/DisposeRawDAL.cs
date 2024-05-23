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

namespace VATServer.Library
{
    public class DisposeRawDAL
    {
        #region Declarations

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        CommonDAL _CommonDAL = new CommonDAL();

        #endregion
         
        #region Search Methods

        public DataTable SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("DisposeSearch");

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
                            DisposeNumber, convert (varchar,DisposeDate,120)DisposeDate,
                            isnull(RefNumber,'NA')RefNumber,
                            isnull(Remarks,'NA')Remarks,
                            isnull(TransactionType,'NA')TransactionType,
                            isnull(Post,'N')Post,
                            isnull(AppTotalPrice,'0')AppTotalPrice,
                            isnull(AppVATAmount,'0')AppVATAmount,
                            isnull(convert (varchar,AppDate,120),DisposeDate)AppDate,
                            isnull(AppRefNumber,'NA')AppRefNumber,
                            isnull(AppRemarks,'NA')AppRemarks,
                            isnull(AppVATAmountImport,'0')AppVATAmountImport,
                            isnull(AppTotalPriceImport,'0')AppTotalPriceImport,
                            isnull(BranchId,'0')BranchId

                            FROM         dbo.DisposeHeaders
                            WHERE 
                            (DisposeNumber  LIKE '%' +  @DisposeNumber   + '%' OR @DisposeNumber IS NULL) 
                            AND (DisposeDate>= @DisposeDateFrom OR @DisposeDateFrom IS NULL)
                            AND (DisposeDate <dateadd(d,1, @DisposeDateTo) OR @DisposeDateTo IS NULL)
                            AND (Post LIKE '%' + @Post + '%' OR @Post IS NULL)
                            AND (TransactionType=@TransactionType)";
                #endregion

                #region SQL Command

                if (BranchId > 0)
                {
                    sqlText = sqlText + @" and BranchId = @BranchId";
                }
                SqlCommand objCommDisposeHeader = new SqlCommand();
                objCommDisposeHeader.Connection = currConn;

                objCommDisposeHeader.CommandText = sqlText;
                objCommDisposeHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter
                if (BranchId > 0)
                {
                    objCommDisposeHeader.Parameters.AddWithValue("@BranchId", BranchId);
                }
                if (!objCommDisposeHeader.Parameters.Contains("@Post"))
                { objCommDisposeHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommDisposeHeader.Parameters["@Post"].Value = Post; }

                if (!objCommDisposeHeader.Parameters.Contains("@DisposeNumber"))
                { objCommDisposeHeader.Parameters.AddWithValue("@DisposeNumber", DisposeNumber); }
                else { objCommDisposeHeader.Parameters["@DisposeNumber"].Value = DisposeNumber; }
                if (DisposeDateFrom == "")
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateFrom"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateFrom", System.DBNull.Value); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateFrom"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateFrom", DisposeDateFrom); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateFrom"].Value = DisposeDateFrom; }
                }
                if (DisposeDateTo == "")
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateTo"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateTo", System.DBNull.Value); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateTo"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateTo", DisposeDateTo); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateTo"].Value = DisposeDateTo; }
                }


                if (!objCommDisposeHeader.Parameters.Contains("@transactionType"))
                { objCommDisposeHeader.Parameters.AddWithValue("@transactionType", transactionType); }
                else { objCommDisposeHeader.Parameters["@transactionType"].Value = transactionType; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommDisposeHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                FileLogger.Log("DisposeRawDAL", "SearchDisposeHeaderDTNew", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("DisposeRawDAL", "SearchDisposeHeaderDTNew", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public DataTable SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            string count = "100";

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

                #region Select Top Condition Remove

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

                #endregion

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

 Id
,BranchId
,DisposeNo
,TransactionDateTime
,Post
,ShiftId
,ReferenceNo
,SerialNo
,ImportIDExcel
,Comments
,TransactionType
,IsSynced
,FiscalYear
,AppVersion
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
  FROM DisposeRaws
WHERE  1=1
";
                #endregion

                sqlTextCount += @" 
select count(DisposeNo)RecordCount
FROM DisposeRaws 
WHERE  1=1
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

                sqlTextOrderBy = " order by TransactionDateTime desc";


                #endregion SqlText

                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + sqlTextOrderBy;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

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



                da.Fill(ds);

                #endregion SqlExecution

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

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

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DisposeRawDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DisposeRawDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<DisposeRawsMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ReceiveMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<DisposeRawsMasterVM> VMs = new List<DisposeRawsMasterVM>();
            DisposeRawsMasterVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(conditionFields, conditionValues, VcurrConn, Vtransaction, null);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new DisposeRawsMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = dr["BranchId"].ToString();
                    vm.DisposeNo = dr["DisposeNo"].ToString();
                    vm.TransactionDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransactionDateTime"].ToString());
                    vm.ShiftId = dr["ShiftId"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.ImportIDExcel = dr["ImportIDExcel"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.IsSynced = dr["IsSynced"].ToString();
                    vm.FiscalYear = dr["FiscalYear"].ToString();
                    vm.AppVersion = dr["AppVersion"].ToString();

                    VMs.Add(vm);
                }


                #endregion SqlExecution

                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
                //}
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DisposeRawDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DisposeRawDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            //finally
            //{
            //    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            //    {
            //        currConn.Close();
            //    }
            //}
            #endregion
            return VMs;
        }

        public DataTable Select_DisposeRawDetail(string DisposeNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("DisposeSearch");

            #endregion

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

                #region SQL Statement

                #region SQL Text


                sqlText = @"
SELECT
DRD.Id
,DRD.BranchId
,DRD.DisposeNo
,isnull(DRD.PurchaseNo,'0')PurchaseNo
,isnull(DRD.SaleNo,'0')SaleNo
,DRD.DisposeLineNo
,DRD.ItemNo
,pro.ProductCode
,pro.ProductName
,isnull(DRD.Quantity,'0')Quantity
,DRD.UOM
,DRD.TransactionType
,DRD.TransactionDateTime
,isnull(DRD.UnitPrice,'0')UnitPrice
,isnull(DRD.SD,'0')SD
,isnull(DRD.SDAmount,'0')SDAmount
,isnull(DRD.VATRate,'0')VATRate
,isnull(DRD.VATAmount,'0')VATAmount
,isnull(DRD.ATAmount,'0')ATAmount
,isnull(DRD.SubTotal,'0')SubTotal      
,isnull(DRD.OfferUnitPrice,'0')OfferUnitPrice      
,isnull(DRD.Post,'N')Post      
,isnull(DRD.IsSaleable,'N')IsSaleable      
,DRD.Comments
,DRD.IsSynced
,DRD.CreatedBy
,DRD.CreatedOn
,DRD.LastModifiedBy
,DRD.LastModifiedOn
FROM DisposeRawDetails DRD 
LEFT OUTER JOIN Products pro ON DRD.ItemNo = pro.ItemNo 
  
WHERE 1=1 
";

                if (!string.IsNullOrWhiteSpace(DisposeNo))
                {
                    sqlText += @" and DRD.DisposeNo=@DisposeNo";
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

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(DisposeNo))
                {
                    cmd.Parameters.AddWithValue("@DisposeNo", DisposeNo);
                }

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


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);

                #endregion

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                FileLogger.Log("DisposeRawDAL", "Select_DisposeRawDetail", ex.ToString() + "\n" + sqlText);

                throw ex;
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

            return dataTable;
        }

        public List<DisposeRawsDetailVM> Select_DisposeRawDetailVM(string DisposeNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<DisposeRawsDetailVM> VMs = new List<DisposeRawsDetailVM>();
            DisposeRawsDetailVM vm;
            #endregion

            #region try

            try
            {
                #region SqlExecution

                DataTable dt = Select_DisposeRawDetail(DisposeNo, conditionFields, conditionValues, VcurrConn, Vtransaction, null);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new DisposeRawsDetailVM();

                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = dr["BranchId"].ToString();
                    vm.DisposeNo = dr["DisposeNo"].ToString();
                    vm.PurchaseNo = dr["PurchaseNo"].ToString();
                    vm.SaleNo = dr["SaleNo"].ToString();
                    vm.DisposeLineNo = dr["DisposeLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.TransactionDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransactionDateTime"].ToString());
                    vm.UnitPrice = Convert.ToDecimal(dr["UnitPrice"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.ATAmount = Convert.ToDecimal(dr["ATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.OfferUnitPrice = Convert.ToDecimal(dr["OfferUnitPrice"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.IsSynced = dr["IsSynced"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();

                    VMs.Add(vm);
                }


                #endregion SqlExecution
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DisposeRawDAL", "Select_DisposeRawDetailVM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DisposeRawDAL", "Select_DisposeRawDetailVM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            finally
            {
                //if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                //{
                //    currConn.Close();
                //}
            }
            #endregion
            return VMs;
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
            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameUpdate);
                CommonDAL commonDal = new CommonDAL();

                #endregion open connection and transaction

                for (int i = 0; i < Ids.Length - 1; i++)
                {
                    DisposeRawsMasterVM master = SelectAllList(Convert.ToInt32(Ids[i]), null, null, currConn, transaction, null).FirstOrDefault();
                    master.Post = "Y";
                    retResults = DisposeRawsPost(master, null);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion


            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                FileLogger.Log("DisposeRawDAL", "MultiplePost", ex.ToString());

                throw ex;
            }
            #endregion

            #region finally

            finally
            {

            }
            #endregion

            #region Result
            return retResults;
            #endregion Result
        }

        #endregion

        #region Basic Methods

        public string[] DisposeRawsInsert(DisposeRawsMasterVM Master, List<DisposeRawsDetailVM> Details, SysDBInfoVMTemp connVM = null)
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

            string newID = "";
            string PostStatus = "";
            int IDExist = 0;

            string PeriodId = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgCheckDate);

                }


                #endregion Validation for Header

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNameInsert);

                #endregion open connection and transaction

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

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DisposeNo) from DisposeRaws " +
                          " where DisposeNo=@MasterDisposeNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterDisposeNo", Master.DisposeNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Generate ID

                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }


                if (Master.TransactionType == "Other")
                {
                    newID = _CommonDAL.TransactionCode("Dispose", "Raw", "DisposeRaws", "DisposeNo", "TransactionDateTime", Master.TransactionDateTime, Master.BranchId.ToString(), currConn, transaction);
                }

                PeriodId = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMyyyy");

                #endregion

                #region Master

                sqlText = "";
                sqlText += " insert into DisposeRaws";
                sqlText += " (";

                sqlText += "  DisposeNo ";
                sqlText += " ,BranchId ";
                sqlText += " ,TransactionDateTime ";
                sqlText += " ,ShiftId ";
                sqlText += " ,ReferenceNo ";
                sqlText += " ,SerialNo ";
                sqlText += " ,Comments ";
                sqlText += " ,TransactionType";
                sqlText += " ,PeriodId";
                sqlText += " ,Post ";
                sqlText += " ,IsSynced ";
                sqlText += " ,FiscalYear";
                sqlText += " ,AppVersion";
                sqlText += " ,CreatedBy ";
                sqlText += " ,CreatedOn ";
                sqlText += " ,LastModifiedBy ";
                sqlText += " ,LastModifiedOn ";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += " @newID";
                sqlText += " ,@BranchId";
                sqlText += " ,@TransactionDateTime";
                sqlText += " ,@ShiftId";
                sqlText += " ,@ReferenceNo";
                sqlText += " ,@SerialNo";
                sqlText += " ,@Comments";
                sqlText += " ,@TransactionType";
                sqlText += " ,@PeriodId";
                sqlText += " ,@Post";
                sqlText += " ,@IsSynced";
                sqlText += " ,@FiscalYear";
                sqlText += " ,@AppVersion";
                sqlText += " ,@CreatedBy";
                sqlText += " ,@CreatedOn";
                sqlText += " ,@LastModifiedBy";
                sqlText += " ,@LastModifiedOn";



                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Master.TransactionDateTime));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReferenceNo", Master.ReferenceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PeriodId", PeriodId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsSynced", Master.IsSynced);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FiscalYear", Master.FiscalYear);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AppVersion", Master.AppVersion);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));


                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Details

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into DisposeRawDetails(";
                    sqlText += " DisposeNo	";
                    sqlText += " ,BranchId";
                    sqlText += " ,PurchaseNo";
                    sqlText += " ,SaleNo";
                    sqlText += " ,DisposeLineNo";
                    sqlText += " ,ItemNo";
                    sqlText += " ,Quantity";
                    sqlText += " ,UOM";
                    sqlText += " ,TransactionType";
                    sqlText += " ,TransactionDateTime";
                    sqlText += " ,UnitPrice	";
                    sqlText += " ,SD";
                    sqlText += " ,SDAmount	";
                    sqlText += " ,VATRate	";
                    sqlText += " ,VATAmount	";
                    sqlText += " ,ATAmount	";
                    sqlText += " ,SubTotal	";
                    sqlText += " ,OfferUnitPrice";
                    sqlText += " ,PeriodId";
                    sqlText += " ,Post";
                    sqlText += " ,IsSaleable	";
                    sqlText += " ,Comments	";
                    sqlText += " ,CreatedBy	";
                    sqlText += " ,CreatedOn	";
                    sqlText += " ,LastModifiedBy";
                    sqlText += " ,LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "  @BranchId	";
                    sqlText += " ,@PurchaseNo";
                    sqlText += " ,@SaleNo";
                    sqlText += " ,@DisposeLineNo";
                    sqlText += " ,@ItemNo";
                    sqlText += " ,@Quantity";
                    sqlText += " ,@UOM	";
                    sqlText += " ,@TransactionType";
                    sqlText += " ,@TransactionDateTime";
                    sqlText += " ,@UnitPrice";
                    sqlText += " ,@SD";
                    sqlText += " ,@SDAmount	";
                    sqlText += " ,@VATRate	";
                    sqlText += " ,@VATAmount";
                    sqlText += " ,@ATAmount	";
                    sqlText += " ,@SubTotal	";
                    sqlText += " ,@OfferUnitPrice";
                    sqlText += " ,@PeriodId";
                    sqlText += " ,@Post	";
                    sqlText += " ,@IsSaleable	";
                    sqlText += " ,@Comments	";
                    sqlText += " ,@CreatedBy";
                    sqlText += " ,@CreatedOn";
                    sqlText += " ,@LastModifiedBy";
                    sqlText += " ,@LastModifiedOn";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PurchaseNo", Item.PurchaseNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SaleNo", Item.SaleNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DisposeLineNo", Item.DisposeLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", Item.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Quantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOM", Item.UOM);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Item.TransactionType);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Item.TransactionDateTime));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UnitPrice", Item.UnitPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SD", Item.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDAmount", Item.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Item.VATAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ATAmount", Item.ATAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OfferUnitPrice", Item.OfferUnitPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PeriodId", PeriodId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", Item.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@IsSaleable", Item.IsSaleable);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Comments", Item.Comments);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", Item.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Item.CreatedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Item.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Item.LastModifiedOn));

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                    }

                    #endregion
                }

                #endregion

                #endregion

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.DisposeRaws " +
                          " where DisposeNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgUnableCreatID);
                }


                #endregion Prefetch

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.disposeMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{

            //    transaction.Rollback();
            //    throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();
                FileLogger.Log("DisposeRawDAL", "DisposeRawsInsert", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public string[] DisposeRawsUpdate(DisposeRawsMasterVM Master, List<DisposeRawsDetailVM> Details, SysDBInfoVMTemp connVM = null)
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

            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            string PostStatus = "";

            string PeriodId = "";


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgCheckDate);

                }



                #endregion Validation for Header

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region Fiscal Year Check

                PeriodId = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMyyyy");


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
                sqlText = sqlText + "select COUNT(DisposeNo) from DisposeRaws WHERE DisposeNo=@MasterDisposeNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterDisposeNo", Master.DisposeNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DisposeRaws set  ";

                sqlText += " BranchId                   = @BranchId ,";
                sqlText += " TransactionDateTime        = @TransactionDateTime ,";
                sqlText += " ShiftId                    = @ShiftId ,";
                sqlText += " ReferenceNo                = @ReferenceNo ,";
                sqlText += " SerialNo                   = @SerialNo ,";
                sqlText += " Comments                   = @Comments ,";
                sqlText += " TransactionType            = @TransactionType ,";
                sqlText += " PeriodId                   = @PeriodId ,";
                sqlText += " Post                       = @Post ,";
                sqlText += " FiscalYear                 = @FiscalYear ,";
                sqlText += " AppVersion                 = @AppVersion ,";
                sqlText += " LastModifiedBy             = @LastModifiedBy ,";
                sqlText += " LastModifiedOn             = @LastModifiedOn ";

                sqlText += " where DisposeNo= @DisposeNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DisposeNo", Master.DisposeNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Master.TransactionDateTime));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReferenceNo", Master.ReferenceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PeriodId", PeriodId);


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FiscalYear", Master.FiscalYear);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@AppVersion", Master.AppVersion);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #endregion

                #region Update into Details(Update complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Delete Existing Detail Data
                sqlText = @" delete FROM DisposeRawDetails WHERE DisposeNo=@DisposeNo ";


                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@DisposeNo", Master.DisposeNo);

                transResult = cmdDeleteDetail.ExecuteNonQuery();
                #endregion

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into DisposeRawDetails(";
                    sqlText += " DisposeNo	";
                    sqlText += " ,BranchId";
                    sqlText += " ,PurchaseNo";
                    sqlText += " ,SaleNo";
                    sqlText += " ,DisposeLineNo";
                    sqlText += " ,ItemNo";
                    sqlText += " ,Quantity";
                    sqlText += " ,UOM";
                    sqlText += " ,TransactionType";
                    sqlText += " ,TransactionDateTime";
                    sqlText += " ,UnitPrice	";
                    sqlText += " ,SD";
                    sqlText += " ,SDAmount	";
                    sqlText += " ,VATRate	";
                    sqlText += " ,VATAmount	";
                    sqlText += " ,ATAmount	";
                    sqlText += " ,SubTotal	";
                    sqlText += " ,OfferUnitPrice";
                    sqlText += " ,PeriodId";
                    sqlText += " ,Post";
                    sqlText += " ,IsSaleable";
                    sqlText += " ,Comments	";
                    sqlText += " ,CreatedBy	";
                    sqlText += " ,CreatedOn	";
                    sqlText += " ,LastModifiedBy";
                    sqlText += " ,LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "  @BranchId	";
                    sqlText += " ,@PurchaseNo";
                    sqlText += " ,@SaleNo";
                    sqlText += " ,@DisposeLineNo";
                    sqlText += " ,@ItemNo";
                    sqlText += " ,@Quantity";
                    sqlText += " ,@UOM	";
                    sqlText += " ,@TransactionType";
                    sqlText += " ,@TransactionDateTime";
                    sqlText += " ,@UnitPrice";
                    sqlText += " ,@SD";
                    sqlText += " ,@SDAmount	";
                    sqlText += " ,@VATRate	";
                    sqlText += " ,@VATAmount";
                    sqlText += " ,@ATAmount	";
                    sqlText += " ,@SubTotal	";
                    sqlText += " ,@OfferUnitPrice";
                    sqlText += " ,@PeriodId";
                    sqlText += " ,@Post	";
                    sqlText += " ,@IsSaleable	";
                    sqlText += " ,@Comments	";
                    sqlText += " ,@CreatedBy";
                    sqlText += " ,@CreatedOn";
                    sqlText += " ,@LastModifiedBy";
                    sqlText += " ,@LastModifiedOn";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@newID", Master.DisposeNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PurchaseNo", Item.PurchaseNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SaleNo", Item.SaleNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DisposeLineNo", Item.DisposeLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", Item.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Quantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOM", Item.UOM);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Item.TransactionType);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Item.TransactionDateTime));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UnitPrice", Item.UnitPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SD", Item.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDAmount", Item.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Item.VATAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ATAmount", Item.ATAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OfferUnitPrice", Item.OfferUnitPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PeriodId", PeriodId);

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", Item.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@IsSaleable", Item.IsSaleable);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Comments", Item.Comments);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", Item.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Item.CreatedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Item.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Item.LastModifiedOn));

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable
                }

                #endregion

                #endregion

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DisposeRaws WHERE DisposeNo=@DisposeNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@DisposeNo", Master.DisposeNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableCreatID);
                }


                #endregion Prefetch

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.disposeMsgUpdateSuccessfully;
                retResults[2] = Master.DisposeNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{

            //    transaction.Rollback();
            //    throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";

                transaction.Rollback();
                FileLogger.Log("DisposeRawDAL", "DisposeRawsUpdate", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public string[] DisposeRawsPost(DisposeRawsMasterVM Master, SysDBInfoVMTemp connVM = null)
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

            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgCheckDatePost);

                }



                #endregion Validation for Header

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNamePost);

                #endregion open connection and transaction

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
                sqlText = sqlText + "select COUNT(DisposeNo) from DisposeRaws WHERE DisposeNo=@DisposeNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@DisposeNo", Master.DisposeNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region Post Data

                sqlText = "";

                sqlText += @" 
update DisposeRaws set   LastModifiedBy= @LastModifiedBy , LastModifiedOn= @LastModifiedOn , Post= 'Y' 
where DisposeNo= @DisposeNo 

update DisposeRawDetails set  LastModifiedBy= @LastModifiedBy, LastModifiedOn= @LastModifiedOn, Post= 'Y' 
where DisposeNo= @DisposeNo

";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DisposeNo", Master.DisposeNo);
                transResult = cmdUpdate.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUpdateNotSuccessfully);
                }

                #endregion

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DisposeRaws WHERE DisposeNo=@DisposeNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@DisposeNo", Master.DisposeNo);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUnableCreatID);
                }


                #endregion Prefetch

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.disposeMsgSuccessfullyPost;
                retResults[2] = Master.DisposeNo;
                retResults[3] = PostStatus;
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

                transaction.Rollback();
                FileLogger.Log("DisposeRawDAL", "DisposeRawsPost", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        #endregion
    }
}
