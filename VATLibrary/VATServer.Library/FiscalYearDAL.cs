using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using System.Reflection;
using VATServer.Ordinary;
using VATServer.Interface;
using Newtonsoft.Json;

namespace VATServer.Library
{
    public class FiscalYearDAL : IFiscalYear
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public DataTable SearchYear(SysDBInfoVMTemp connVM = null)
        {

            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductType");
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
                sqlText = @"
                            SELECT CurrentYear FROM 
(
SELECT DISTINCT CurrentYear FROM  FiscalYear 
union 
select max(CurrentYear)+1 FROM FiscalYear 
UNION 
                                 
SELECT DATEPART(yyyy, FYearEnd) FROM CompanyProfiles
) AS a
WHERE CurrentYear IS NOT null ORDER BY CurrentYear";

                SqlCommand objCommYear = new SqlCommand();
                objCommYear.Connection = currConn;
                objCommYear.CommandText = sqlText;
                objCommYear.CommandType = CommandType.Text;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommYear);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("FiscalYearDAL", "SearchYear", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("FiscalYearDAL", "SearchYear", ex.ToString() + "\n" + sqlText);

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
            return dataTable;

        }

        public string[] FiscalYearUpDate(List<FiscalYearVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


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
                transaction = currConn.BeginTransaction(MessageVM.FYMsgMethodNameUpdate);

                #endregion open connection and transaction



                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.FYMsgMethodNameUpdate, MessageVM.FYMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText = sqlText + "select COUNT(PeriodID) from FiscalYear" +
                              " WHERE  PeriodID=@ItemPeriodID ";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;

                    cmdExistTran.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);

                    int IDExist = (int)cmdExistTran.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgNotExist);
                    }

                    #endregion Find Transaction Exist

                    #region Update only DetailTable

                    sqlText = "";


                    sqlText += " update FiscalYear set ";

                    sqlText += " PeriodLock     =@ItemPeriodLock,";
                    sqlText += " GLLock         =@ItemGLLock,";
                    sqlText += " LastModifiedBy =@ItemLastModifiedBy,";
                    sqlText += " LastModifiedOn =@ItemLastModifiedOn";
                    sqlText += " where  PeriodID=@ItemPeriodID";

                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemGLLock", Item.GLLock);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodID", Item.PeriodID);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameUpdate, MessageVM.FYMsgUpdateNotSuccessfully);
                    }
                    #endregion Update only DetailTable


                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


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
                retResults[1] = MessageVM.FYMsgUpdateSuccessfully;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("FiscalYearDAL", "FiscalYearUpDate", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("FiscalYearDAL", "FiscalYearUpDate", ex.ToString() + "\n" + sqlText);

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public DataTable LoadYear(string CurrentYear, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Year");
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
                sqlText = @"
                            SELECT    
FiscalYearName,
CurrentYear,
PeriodID,
PeriodName,
convert(varchar, PeriodStart,120)PeriodStart,
convert(varchar, PeriodEnd,120)PeriodEnd, 
PeriodLock,
isnull(GLLock,'N')GLLock 

FROM         FiscalYear
WHERE 	(CurrentYear  =  @CurrentYear ) 

ORDER BY PeriodStart";

                SqlCommand objCommYear = new SqlCommand();
                objCommYear.Connection = currConn;
                objCommYear.CommandText = sqlText;
                objCommYear.CommandType = CommandType.Text;

                if (!objCommYear.Parameters.Contains("@CurrentYear"))
                {
                    objCommYear.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                }
                else
                {
                    objCommYear.Parameters["@CurrentYear"].Value = CurrentYear;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommYear);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("FiscalYearDAL", "LoadYear", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("FiscalYearDAL", "LoadYear", ex.ToString() + "\n" + sqlText);

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
            return dataTable;
        }


        #region web methods
        public string[] FiscalYearUpdate(List<FiscalYearVM> Details, string modifiedBy, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            int nextId = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


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
                transaction = currConn.BeginTransaction(MessageVM.FYMsgMethodNameUpdate);

                #endregion open connection and transaction



                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.FYMsgMethodNameUpdate, MessageVM.FYMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText = sqlText + "select COUNT(PeriodID) from FiscalYear" +
                              " WHERE  PeriodID=@ItemPeriodID";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    cmdExistTran.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);

                    int IDExist = (int)cmdExistTran.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgNotExist);
                    }

                    #endregion Find Transaction Exist

                    #region Update only DetailTable

                    sqlText = "";


                    sqlText += " update FiscalYear set ";

                    sqlText += " PeriodLock     =@ItemPeriodLock,";
                    sqlText += " GLLock         =@ItemGLLock,";
                    sqlText += " LastModifiedBy =@ItemLastModifiedBy,";
                    sqlText += " LastModifiedOn =@ItemLastModifiedOn";
                    sqlText += " where  PeriodID=@ItemPeriodID";

                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemGLLock", Item.GLLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", modifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn", DateTime.Now);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameUpdate, MessageVM.FYMsgUpdateNotSuccessfully);
                    }
                    #endregion Update only DetailTable


                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


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
                retResults[1] = MessageVM.FYMsgUpdateSuccessfully;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();
                FileLogger.Log("FiscalYearDAL", "FiscalYearUpdate", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public List<FiscalYearVM> SelectAll(int PeriodID = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
            FiscalYearVM vm;
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

                #region sql statement

                #region SqlText

                sqlText = @"
SELECT
 FiscalYearName
,CurrentYear
,PeriodID
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,ISNULL(VATReturnPost, 'N') VATReturnPost
,GLLock
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn

FROM FiscalYear  
WHERE  1=1

";
                if (PeriodID > 0)
                {
                    sqlText += @" and PeriodID=@PeriodID";
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

                sqlText += @" ORDER BY PeriodStart";

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

                if (PeriodID > 0)
                {
                    objComm.Parameters.AddWithValue("@PeriodID", PeriodID);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearVM();
                    vm.FiscalYearName = dr["FiscalYearName"].ToString();
                    vm.CurrentYear = dr["CurrentYear"].ToString();
                    vm.PeriodID = dr["PeriodID"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = OrdinaryVATDesktop.DateTimeToDate(dr["PeriodStart"].ToString());
                    vm.PeriodEnd = OrdinaryVATDesktop.DateTimeToDate(dr["PeriodEnd"].ToString());
                    vm.PeriodLock = dr["PeriodLock"].ToString();
                    vm.VATReturnPost = dr["VATReturnPost"].ToString();
                    vm.GLLock = dr["GLLock"].ToString();
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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
            return VMs;
        }

        public List<FiscalYearVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
            FiscalYearVM vm;
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
                //                sqlText = @"
                //select distinct CurrentYear from FiscalYear union select 'All' CurrentYear from FiscalYear
                //WHERE  1=1
                //";
                sqlText = @" select distinct CurrentYear from FiscalYear WHERE  1=1";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearVM();
                    vm.CurrentYear = dr["CurrentYear"].ToString(); ;
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<FiscalYearVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
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

                sqlText = @" 
SELECT 
CurrentYear	
,PeriodID	
,PeriodName	
from FiscalYear WHERE  1=1
order by CurrentYear desc, PeriodStart desc
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    VMs = JsonConvert.DeserializeObject<List<FiscalYearVM>>(json);
                }

                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "DropDown", ex.ToString() + "\n" + sqlText);
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

        public int LockChek(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            int Unlocked = 0;
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

                sqlText += "select COUNT(PeriodID) from FiscalYear where PeriodLock='N'";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                Unlocked = (int)objComm.ExecuteScalar();
                return Unlocked;

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("FiscalYearDAL", "LockChek", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            #endregion

        }

        public string MaxDate(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            string maxDate = "";
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

                sqlText += "select MaxDate=max(PeriodEnd) from FiscalYear";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    maxDate = dr["MaxDate"].ToString();
                }

                return maxDate;

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("FiscalYearDAL", "MaxDate", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            #endregion

        }

        public FiscalYearPeriodVM StartEndPeriod(string year, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            FiscalYearPeriodVM vm = new FiscalYearPeriodVM();
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
                sqlText = @"select min(PeriodStart) PeriodStart,max(PeriodEnd) PeriodEnd from FiscalYear where CurrentYear=@CurrentYear";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@CurrentYear", year);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "StartEndPeriod", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("FiscalYearDAL", "StartEndPeriod", ex.ToString() + "\n" + sqlText);
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
            return vm;
        }
        #endregion

        #region need to parameterize
        public string[] FiscalYearInsert(List<FiscalYearVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";


            int IDExist = 0;


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
                transaction = currConn.BeginTransaction(MessageVM.FYMsgMethodNameInsert);

                #endregion open connection and transaction

                if (!Details.Any()) throw new ArgumentNullException("FiscalYearInsert", "Sorry,No item found to add.");

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(PeriodID) from FiscalYear" +
                          " WHERE  PeriodID=@PeriodID";//'" + Details[0].PeriodID + "'";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@PeriodID", Details[0].PeriodID);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException("FiscalYearInsert", "Fiscal Year aleady exist.");
                }

                #endregion Find Transaction Exist

                #region Find Previous Year Lock

                sqlText = "";
                sqlText = sqlText + "select COUNT(periodlock) from FiscalYear" +
                          " WHERE currentyear='" + Details[0].CurrentYear + "'-1  and periodlock='Y'";
                SqlCommand cmdPreYearLock = new SqlCommand(sqlText, currConn);
                cmdPreYearLock.Transaction = transaction;
                IDExist = (int)cmdPreYearLock.ExecuteScalar();

                if (IDExist < 12)
                {
                    throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgPreviouseYearNotLock);
                }

                #endregion Find Previous Year Lock

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into FiscalYear(";
                    sqlText += " FiscalYearName,";
                    sqlText += " CurrentYear,";
                    sqlText += " PeriodID,";
                    sqlText += " PeriodName,";
                    sqlText += " PeriodStart,";
                    sqlText += " PeriodEnd,";
                    sqlText += " PeriodLock,";
                    sqlText += " GLLock,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";

                    sqlText += "@ItemFiscalYearName,";
                    sqlText += "@ItemCurrentYear,";
                    sqlText += "@ItemPeriodID,";
                    sqlText += "@ItemPeriodName,";
                    sqlText += "@ItemPeriodStart,";
                    sqlText += "@ItemPeriodEnd,";
                    sqlText += "@ItemPeriodLock,";
                    sqlText += "@ItemGLLock,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFiscalYearName", Item.FiscalYearName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCurrentYear", Item.CurrentYear);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodID", Item.PeriodID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodName", Item.PeriodName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodStart", Item.PeriodStart);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodEnd", Item.PeriodEnd);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemGLLock", Item.GLLock);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedBy", Item.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }
                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

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
                retResults[1] = MessageVM.FYMsgSaveSuccessfully;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

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
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //////throw ex;

                FileLogger.Log("FiscalYearDAL", "FiscalYearInsert", ex.ToString() + "\n" + sqlText);
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        #endregion

        public List<FiscalYearVM> SelectLastPeriod(int PeriodID = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
            FiscalYearVM vm;
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

                #region sql statement

                #region SqlText

                sqlText = @"
SELECT
 FiscalYearName
,CurrentYear
,PeriodID
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,ISNULL(VATReturnPost, 'N') VATReturnPost
,GLLock
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn

FROM FiscalYear  
WHERE  1=1
and PeriodStart = (select max(PeriodStart) from FiscalYear)

";
                if (PeriodID > 0)
                {
                    sqlText += @" and PeriodID=@PeriodID";
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

                sqlText += @" ORDER BY PeriodStart";

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

                if (PeriodID > 0)
                {
                    objComm.Parameters.AddWithValue("@PeriodID", PeriodID);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearVM();
                    vm.FiscalYearName = dr["FiscalYearName"].ToString();
                    vm.CurrentYear = dr["CurrentYear"].ToString();
                    vm.PeriodID = dr["PeriodID"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = OrdinaryVATDesktop.DateTimeToDate(dr["PeriodStart"].ToString());
                    vm.PeriodEnd = OrdinaryVATDesktop.DateTimeToDate(dr["PeriodEnd"].ToString());
                    vm.PeriodLock = dr["PeriodLock"].ToString();
                    vm.VATReturnPost = dr["VATReturnPost"].ToString();
                    vm.GLLock = dr["GLLock"].ToString();
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
            #endregion

            #region catch
            
            catch (Exception ex)
            {
                FileLogger.Log("FiscalYearDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

            return VMs;
        }


    }
}
