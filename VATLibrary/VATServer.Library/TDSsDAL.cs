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
using System.Reflection;
using VATServer.Interface;

namespace VATServer.Library
{
    public class TDSsDAL : ITDSs
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #region
        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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
                if (count == "All")
                {
                    sqlText = @"SELECT ";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";

                }

                sqlText += @"

 Id
,Code
,Description
,MinValue
,MaxValue
,Rate
,Section
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ISNULL(IsArchive,0) IsArchive
FROM TDSs   
WHERE  1=1  and isnull(IsArchive,0) = 0 

";
                #region sqlTextCount
                sqlTextCount += @" select count(Id)RecordCount
FROM TDSs   
WHERE  1=1  and isnull(IsArchive,0) = 0
";
                #endregion

                if (Id != null)
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
                #endregion SqlText
                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
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

                if (Id != null)
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
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("TDSsDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TDSsDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
       
        public string[] InsertToTDSsNew(TDSsVM vm, bool BranchProfileAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string Code = vm.Code;
            int nextId = 0;
            #region Try
            try
            {
                #region Validation

                //if (string.IsNullOrEmpty(vm.BranchName))
                //{
                //    throw new ArgumentNullException("InsertToBranchProfile",
                //                                    "Please enter BranchProfile group name.");
                //}


                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "TDSs") == "Y" ? true : false);
                #endregion settingsValue

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
                #region TDSs  name existence checking



                #endregion TDSs  name existence checking
                #region TDSs  new id generation
                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  TDSs";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToTDSs",
                                                    "Unable to create new TDSs No");
                }
                #region Code
                if (Auto == false)
                {
                    if (BranchProfileAutoSave)
                    {
                        if (string.IsNullOrWhiteSpace(vm.Code))
                        {
                            Code = nextId.ToString();
                        }
                        // BranchProfile Group Id
                    }
                    else if (string.IsNullOrEmpty(Code))
                    {
                        throw new ArgumentNullException("InsertToTDSs", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(Id) from TDSs where  Code=@Code and MaxValue=@MaxValue and MinValue=@MinValue and Section=@Section and Rate=@Rate";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@Code", Code);
                        cmdCodeExist.Parameters.AddWithValue("@MinValue", vm.MinValue);
                        cmdCodeExist.Parameters.AddWithValue("@MaxValue", vm.MaxValue);
                        cmdCodeExist.Parameters.AddWithValue("@Rate", vm.Rate);
                        cmdCodeExist.Parameters.AddWithValue("@Section", vm.Section);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToTDSs", "Same TDSs  Code('" + Code + "') already exist");
                        }
                    }
                }
                else
                {
                    Code = nextId.ToString();
                }
                #endregion Code

                #endregion TDSs  new id generation

                #region Inser new TDSs
                sqlText = "";

                sqlText += @" 
INSERT INTO TDSs(

Code
,Description
,MinValue
,MaxValue
,Rate
,Section
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
, IsArchive

) VALUES (

@Code
,@Description
,@MinValue
,@MaxValue
,@Rate
,@Section
,@Comments
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@IsArchive
) SELECT SCOPE_IDENTITY()
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@Code", Code);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MinValue", vm.MinValue);
                cmdInsert.Parameters.AddWithValue("@MaxValue", vm.MaxValue);
                cmdInsert.Parameters.AddWithValue("@Rate", vm.Rate);
                cmdInsert.Parameters.AddWithValue("@Section", vm.Section);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                nextId = transResult;

                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested TDSs  Information successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + Code;
                        }
                    }
                }

                #endregion Commit


                #endregion Inser new BranchProfile

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("TDSsDAL", "InsertToTDSsNew", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

            }
            #endregion

            #region finally
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
            return retResults;
        }
        
        public string[] UpdateToTDSsNew(TDSsVM vm, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.Id.ToString();

            string Code = vm.Code;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Id.ToString()))
                {
                    throw new ArgumentNullException("UpdateToTDSs",
                                                    "Invalid TDS ID");
                }
                //if (string.IsNullOrEmpty(vm.BranchName))
                //{
                //    throw new ArgumentNullException("UpdateToBranchProfiles",
                //                                    "Invalid BranchProfile Name.");
                //}
                //if (string.IsNullOrEmpty(vm.TelephoneNo))
                //{
                //    throw new ArgumentNullException("UpdateToBranchProfiles",
                //                                    "Please enter BranchProfile TelephoneNo");
                //}


                #endregion Validation


                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToTDSs");

                #endregion open connection and transaction

                #region BranchProfile  existence checking

                sqlText = "select count(Id) from TDSs where  Id=@Id";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@Id", vm.Id);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToTDSs",
                                "Could not find requested BranchProfiles  id.");
                }

                #endregion BranchProfile Group existence checking




                #region Update new TDSs
                sqlText = "";
                sqlText = "update TDSs set";
                sqlText += "  Code                      =@Code";
                sqlText += "  ,Description              =@Description";
                sqlText += "  ,MinValue                 =@MinValue";
                sqlText += "  ,MaxValue                 =@MaxValue";
                sqlText += "  ,Rate                     =@Rate";
                sqlText += "  ,Section                  =@Section";
                sqlText += "  ,Comments                 =@Comments";
                sqlText += "  ,LastModifiedBy            =@LastModifiedBy";
                sqlText += "  ,LastModifiedOn            =@LastModifiedOn";

                sqlText += " WHERE Id           =@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                cmdUpdate.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MinValue", vm.MinValue);
                cmdUpdate.Parameters.AddWithValue("@MaxValue", vm.MaxValue);
                cmdUpdate.Parameters.AddWithValue("@Rate", vm.Rate);
                cmdUpdate.Parameters.AddWithValue("@Section", vm.Section);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested TDS  Information successfully Update";
                        retResults[2] = vm.Id.ToString();
                        retResults[3] = Code;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update TDS ";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update TDS group";
                }

                #endregion Commit


                #endregion

            }
            #endregion
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("TDSsDAL", "UpdateToTDSsNew", ex.ToString() + "\n" + sqlText);

////                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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


            return retResults;
        }
        
        public string[] Delete(TDSsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTDS"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update TDSs set";

                        sqlText += " LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);

                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("TDSs Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("TDSs Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                if (Vtransaction == null) 
                {
                   transaction.Rollback(); 
                }

                FileLogger.Log("TDSsDAL", "Delete", ex.ToString() + "\n" + sqlText);

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }



        public DataTable CurrentTDSAmount(string PurchaseInvoiceNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable ds = new DataTable();
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
	--declare @PurchaseInvoiceNo as varchar(100)
	--set @PurchaseInvoiceNo='PUR-00000001/0919'

 select t.*, h.VendorID from PurchaseTDSs t
 left outer join PurchaseInvoiceHeaders h on t.PurchaseInvoiceNo=h.PurchaseInvoiceNo
 where t.PurchaseInvoiceNo=@PurchaseInvoiceNo
";


                #endregion SqlText
                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo);
                da.Fill(ds);
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
                FileLogger.Log("TDSsDAL", "CurrentTDSAmount", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TDSsDAL", "CurrentTDSAmount", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return ds;
        }

        public DataSet TDSAmount(string VendorID, string ReceiveDate, string TDSCode, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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

 
	--declare @VendorID as varchar(100)
	--declare @TDSCode as varchar(100)
	--declare @ReceiveDate as date
	--set @VendorID='10'
	--set @ReceiveDate='01-Oct-2019'
	--set @TDSCode='52A'

	declare @ReceiveDateF as date
	declare @ReceiveDateT as date

	select  @ReceiveDateF=  min(PeriodStart),@ReceiveDateT=max(PeriodEnd)  from FiscalYear
	where CurrentYear in(
	select CurrentYear from FiscalYear
	where @ReceiveDate between PeriodStart and PeriodEnd )
 
 
 select distinct h.VendorID,d.TDSCode,  sum( d.SubTotal)PreviousSubTotal  from PurchaseInvoiceDetails d
 left outer join PurchaseInvoiceHeaders h on d.PurchaseInvoiceNo=h.PurchaseInvoiceNo
 where h.VendorID=@VendorID
 and d.TDSCode=@TDSCode
	and d.ReceiveDate >DATEADD(d,-1,  @ReceiveDateF) and d.ReceiveDate<DATEADD(d,1,  @ReceiveDateT)
 and h.Post='Y'
 group by  h.VendorID,d.TDSCode
 
  select distinct h.VendorID,t.TDSCode,sum( t.TDSAmount)PreviousTDSAmount  from PurchaseTDSs t
 left outer join PurchaseInvoiceHeaders h on t.PurchaseInvoiceNo=h.PurchaseInvoiceNo
 where h.VendorID=@VendorID
 and t.TDSCode=@TDSCode
	and h.ReceiveDate >DATEADD(d,-1,  @ReceiveDateF) and h.ReceiveDate<DATEADD(d,1,  @ReceiveDateT)
 and t.Post='Y'
 group by  h.VendorID,t.TDSCode

     select Code,Description,MinValue,MaxValue,Rate from TDSs
    where Code in(@TDSCode)
";


                #endregion SqlText
                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@VendorID", VendorID);
                da.SelectCommand.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
                da.SelectCommand.Parameters.AddWithValue("@TDSCode", TDSCode);
                da.Fill(ds);
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
                FileLogger.Log("TDSsDAL", "TDSAmount", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TDSsDAL", "TDSAmount", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return ds;
        }

        public string[] UpdatePurchaseTDSs(string Id, decimal TDSAmount, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id.ToString();


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            int nextId = 0;
            #endregion

            #region Try
            try
            {
                #region Validation


                #endregion Validation


                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToTDSs");

                #endregion open connection and transaction


                #region Update new TDSs
                sqlText = "";
                sqlText = "update PurchaseTDSs set";
                sqlText += "  TDSAmount     =@TDSAmount";
                sqlText += " WHERE Id           =@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@Id", Id);
                cmdUpdate.Parameters.AddWithValue("@TDSAmount", TDSAmount);

                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested TDS  Information successfully Update";
                        retResults[2] = Id.ToString();
                        retResults[3] = "";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update TDS ";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update TDS group";
                }

                #endregion Commit


                #endregion

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("TDSsDAL", "UpdatePurchaseTDSs", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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


            return retResults;
        }

        #endregion
        #region Web Method
        public List<TDSsVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TDSsVM> VMs = new List<TDSsVM>();
            TDSsVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new TDSsVM();


                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.MinValue = Convert.ToDecimal(dr["MinValue"].ToString());
                    vm.MaxValue = Convert.ToDecimal(dr["MaxValue"].ToString());
                    vm.Rate = Convert.ToDecimal(dr["Rate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();


                    VMs.Add(vm);
                }

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("TDSsDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TDSsDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
    }
}
