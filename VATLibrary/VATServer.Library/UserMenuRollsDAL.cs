using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class UserMenuRollsDAL : IUserMenuRolls
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion


        //currConn to VcurrConn 25-Aug-2020
        public string UserMenuRollsInsert(string FormID,string UserID, string FormName, string Access, string PostAccess
            , string AddAccess, string EditAccess, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(FormID))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(UserID))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(FormName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(Access))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(PostAccess))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }

                #endregion Validation

                #region open connection and transaction

                if (VcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region SettingsExist

                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT FormID)FormID FROM UserMenuRolls ";
                sqlText += " WHERE FormID=@FormID";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;

                cmdExist.Parameters.AddWithValue("@FormID", FormID);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("UserMenuAllFinalRolls", "Please Input UserMenuAllFinalRolls Value");
                }

                #endregion ProductExist

                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        " INSERT INTO UserMenuRolls(FormID,UserID,FormName,Access,PostAccess,AddAccess,EditAccess,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += " @FormID,";
                    sqlText += " @UserID,";
                    sqlText += " @FormName,";
                    sqlText += " @Access,";
                    sqlText += " @PostAccess,";
                    sqlText += " @AddAccess,";
                    sqlText += " @EditAccess,";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    sqlText += " )";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;

                    cmdExist1.Parameters.AddWithValue("@FormID", FormID);
                    cmdExist1.Parameters.AddWithValue("@UserID", UserID);
                    cmdExist1.Parameters.AddWithValue("@FormName", FormName);
                    cmdExist1.Parameters.AddWithValue("@Access", Access);
                    cmdExist1.Parameters.AddWithValue("@PostAccess", PostAccess);
                    cmdExist1.Parameters.AddWithValue("@AddAccess", AddAccess);
                    cmdExist1.Parameters.AddWithValue("@EditAccess", EditAccess);

                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("UserMenuRolls", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("UserMenuRolls", "Please Input Settings Value");
                    }
                }
                #endregion Last Price
                #region insert data into settingRole table



                #endregion

            }

            #endregion try

            #region Catch and Finall
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
                //if (currConn == null)
                //{
                if (VcurrConn.State == ConnectionState.Open)
                {
                    VcurrConn.Close();

                }
                //}
            }


            #endregion

            #region Results

            return retResults;
            #endregion

        }
        public DataTable UserMenuRollsSelectAll(string UserID,string[] conditionFields = null
           , string[] conditionValues = null, SqlConnection VcurrConn = null , SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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

                sqlText = @"SELECT [FormID]
                                      ,[UserID]
                                      ,[FormName]
                                      ,  isnull(Access,0)Access
                                      ,    isnull(PostAccess,0)PostAccess
                                      ,   isnull(AddAccess,0)AddAccess
                                      ,   isnull(EditAccess,0)EditAccess
                                      FROM UserMenuRolls
                                
                                       WHERE  1=1 
";


                if (UserID != null && UserID != "0")
                {
                    sqlText += @" and UserID=@UserID";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
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
                sqlText += @" ORDER BY FormID";

                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
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

                if (UserID != null && UserID != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@UserID", UserID.ToString());
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
        public string[] UserMenuRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

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



                #region Last Settings


                sqlText = "";
                sqlText += " update UserMenuRolls set  ";

                sqlText += "  FormName=@FormName";
                sqlText += " ,Access=@Access";
                sqlText += " ,PostAccess=@PostAccess";
                sqlText += " ,AddAccess=@AddAccess";
                sqlText += " ,EditAccess=@EditAccess";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " where                FormID=@FormID";
                sqlText += " and                UserID=@UserID";

                foreach (DataRow item in dt.Rows)
                {


                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;

                    cmdExist1.Parameters.AddWithValue("@UserID", item["UserID"]);
                    cmdExist1.Parameters.AddWithValue("@FormID", item["FormID"]);
                    cmdExist1.Parameters.AddWithValue("@FormName", item["FormName"]);
                    cmdExist1.Parameters.AddWithValue("@Access", item["Access"]);
                    cmdExist1.Parameters.AddWithValue("@PostAccess", item["PostAccess"]);
                    cmdExist1.Parameters.AddWithValue("@AddAccess", item["AddAccess"]);
                    cmdExist1.Parameters.AddWithValue("@EditAccess", item["EditAccess"]);
                    cmdExist1.Parameters.AddWithValue("@LastModifiedBy", UserInfoVM.UserName);
                    cmdExist1.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    transResult = (int)cmdExist1.ExecuteNonQuery();

                }
                #endregion Last Price
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Adjustment";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update product type";

                }

                #endregion Commit

            }

            #endregion try

            #region Catch and Finall
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
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }
                //}
            }


            #endregion

            #region Results

            return retResults;
            #endregion

        }

        public string[] UserMenuRollsInsertByUser(string UserID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

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



                #region Last Settings


                sqlText = "";
                sqlText += @" insert into UserMenuRolls(FormID,UserID,FormName,Access,PostAccess,AddAccess,EditAccess)
SELECT FormID,@UserID,FormName,1,1,1,1
FROM UserMenuAllFinalRolls
WHERE  1=1 and  Access=1 and AccessType='Button'
and FormID not in(
SELECT [FormID]
FROM UserMenuRolls
WHERE  1=1 
and UserID=@UserID) ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.Parameters.AddWithValue("@UserID", UserID);

                    transResult = (int)cmdExist1.ExecuteNonQuery();

                #endregion Last Price
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Adjustment";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update product type";

                }

                #endregion Commit

            }

            #endregion try

            #region Catch and Finall
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
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }
                //}
            }


            #endregion

            #region Results

            return retResults;
            #endregion

        }

        public string[] ImportUserMenuRolls(List<UserMenuSettingsVM> UserMenu, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string bankCode;

            #endregion Initializ
            try
            {
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("ImportUserMenuRolls");

                #endregion open connection and transaction

                foreach (var Item in UserMenu.ToList())
                {



                    sqlText = "select count(distinct FormID) from UserMenuRolls where  FormID=@FormID and UserID=@UserID";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;

                    cmdNameExist.Parameters.AddWithValue("@FormID", Item.FormID);
                    cmdNameExist.Parameters.AddWithValue("@UserID", Item.UserID);

                    int countName = (int)cmdNameExist.ExecuteScalar();
                    if (countName > 0)
                    {

                        //throw new ArgumentNullException("InsertToBankInformation",
                        //                                "Requested Bank Name('" + Item.BankName + "') and Account number('" + Item.AccountNumber + "') is already exist");


                        sqlText = @"UPDATE [dbo].[UserMenuRolls]
                        SET 

                            [Access] = @Access
                            ,[PostAccess] = @PostAccess
                            ,[AddAccess] = @AddAccess
                            ,[EditAccess] = @EditAccess
                            ,[LastModifiedBy] = @LastModifiedBy
                            ,[LastModifiedOn] = @LastModifiedOn
               

                        WHERE FormID=@FormID and UserID=@UserID";


                        cmdNameExist.CommandText = sqlText;

                        cmdNameExist.Parameters.AddWithValue("@Access", Item.Access);
                        cmdNameExist.Parameters.AddWithValue("@PostAccess", Item.PostAccess);
                        cmdNameExist.Parameters.AddWithValue("@AddAccess", Item.AddAccess);
                        cmdNameExist.Parameters.AddWithValue("@EditAccess", Item.EditAccess);
                        cmdNameExist.Parameters.AddWithValue("@LastModifiedBy", Item.LastModifiedBy);
                        cmdNameExist.Parameters.AddWithValue("@LastModifiedOn", Item.LastModifiedOn);
                        //cmdNameExist.Parameters.AddWithValue("@UserID", Item.UserID);
                        //cmdNameExist.Parameters.AddWithValue("@FormID", Item.FormID);


                        transResult = cmdNameExist.ExecuteNonQuery();
                    }
                    else
                    {

                        #region Insert UserMenuRolls
                        sqlText = "select count(distinct FormID) from UserMenuAllFinalRolls where  FormID=@FormID And FormName=@FormName";
                        SqlCommand cmdFormCheck = new SqlCommand(sqlText, currConn);
                        cmdFormCheck.Transaction = transaction;
                        cmdFormCheck.Parameters.AddWithValue("@FormID", Item.FormID);
                        cmdFormCheck.Parameters.AddWithValue("@FormName", Item.FormName);
                        int nextId = (int)cmdFormCheck.ExecuteScalar();
                        if (nextId <= 0)
                        {

                            throw new ArgumentNullException("InsertToUserMenuRolls",
                                "Invalid FormID '" + Item.FormID + "' OR FormName '" + Item.FormName + "' Use In Excel");
                        }

                       

                        sqlText = "";
                        sqlText += "insert into UserMenuRolls";
                        sqlText += "(";
                        //sqlText += "LineID,";
                        sqlText += "FormID,";
                        sqlText += "UserID,";
                        sqlText += "FormName,";
                        sqlText += "Access,";
                        sqlText += "PostAccess,";
                        sqlText += "AddAccess,";
                        sqlText += "EditAccess,";
                        sqlText += "CreatedBy,";
                        sqlText += "CreatedOn";                     
                        sqlText += ")";
                        sqlText += " values(";
                        //sqlText += "@LineID,";
                        sqlText += "@FormID,";
                        sqlText += "@UserID,";
                        sqlText += "@FormName,";
                        sqlText += "@Access,";
                        sqlText += "@PostAccess,";
                        sqlText += "@AddAccess,";
                        sqlText += "@EditAccess,";
                        sqlText += "@CreatedBy,";
                        sqlText += "@CreatedOn";
                  
                        sqlText += ")";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;

                        //cmdInsert.Parameters.AddWithValueAndNullHandle("@nextId", LineID);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@FormID", Item.FormID);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@UserID", Item.UserID);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@Access", Item.Access);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@PostAccess", Item.PostAccess);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@AddAccess", Item.AddAccess);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@EditAccess", Item.EditAccess);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Item.CreatedOn);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Item.CreatedBy);
                        cmdInsert.Parameters.AddWithValueAndNullHandle("@FormName", Item.FormName);
             

                        transResult = (int)cmdInsert.ExecuteNonQuery();

                        if (transResult <= 0 || cmdInsert == null)
                        {

                            throw new ArgumentNullException("ImportUserMenuRolls",
                                "Unable to Insert FormID('" + Item.FormID + "')");
                        }

                        #endregion Insert
                    }
                }
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested ImportUserMenuRolls Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add ImportUserMenuRolls";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add ImportUserMenuRolls ";
                }

                #endregion COMMIT
            }

            #region Catch and Finall
            catch (SqlException sqlex)
            {

                transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
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

        public DataTable GetExcelUserMenuRolls(string UserID, string[] conditionFields = null
           , string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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

                            sqlText = @"SELECT [FormID]                                    
									  ,ui.UserName
                                      ,[FormName]
                                      ,isnull(Access,0)Access
                                      ,isnull(PostAccess,0)PostAccess
                                      ,isnull(AddAccess,0)AddAccess
                                      ,isnull(EditAccess,0)EditAccess
                                      FROM UserMenuRolls ur
                                      left outer join  UserInformations ui on ui.UserID=ur.UserID
                                      
                                      WHERE  1=1 
";


                if (UserID != null && UserID != "0")
                {
                    sqlText += @" and UserID=@UserID";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
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
                sqlText += @" ORDER BY FormID";

                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
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

                if (UserID != null && UserID != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@UserID", UserID.ToString());
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
   }
}
