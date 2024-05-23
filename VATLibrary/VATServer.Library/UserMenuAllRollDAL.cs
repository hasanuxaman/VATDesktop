using SymphonySofttech.Utilities;
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
    public class UserMenuAllRollDAL : IUserMenuAllRoll
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        #endregion
        public void UserMenuAllRollsSettingChange(SysDBInfoVMTemp connVM = null)
        {
            string sqlResultssettings;
            SettingDAL settingDal = new SettingDAL();

            //sqlResultssettings = settingDal.UserMenuAllRollsInsert("110110110", "Setup/ItemInformation/Group", "1", "1", "1", "1", null, null, connVM);
            

        }

        public DataTable UserMenuAllRollsSelectAll(string FormID = "0", string[] conditionFields = null
            , string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , bool Dt = false, SysDBInfoVMTemp connVM = null)
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
                                      ,    isnull(AccessType,'')AccessType
                                      ,[FormName]
                                      ,  isnull(Access,0)Access
                                      ,  isnull(AccessRoll,0)AccessRoll
                                      FROM UserMenuAllFinalRolls
                                      
                                       WHERE  1=1 
";


                if (FormID != null && FormID != "0")
                {
                    sqlText += @" and FormID=@FormID";
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

                if (FormID != null && FormID != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@FormID", FormID.ToString());
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
        public string[] UserMenuAllRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                sqlText += " update UserMenuAllFinalRolls set  ";

                sqlText += " Access=@Access";
                sqlText += " ,AccessRoll=@AccessRoll";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " where                FormID=@FormID";

                foreach (DataRow item in dt.Rows)
                {


                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@FormID", item["FormID"]);

                    var tt = item["FormID"].ToString().Trim() + "~" + item["Access"].ToString().Trim();
                    string AccessRoll = Converter.DESEncrypt(PassPhrase, EnKey, tt);

                    cmdExist1.Parameters.AddWithValueAndNullHandle("@Access", item["Access"]);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@AccessRoll", AccessRoll);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", UserInfoVM.UserName);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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
        public string[] UserMenuAllFinalRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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


                dt = new DataTable();
                #region Last Settings
                sqlText = @" select * from UserMenuAllFinalRolls";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.Fill(dt);


                sqlText = "";
                sqlText += " update UserMenuAllFinalRolls set  ";
                sqlText += " AccessRoll=@Access";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " where                FormID=@FormID";

                foreach (DataRow item in dt.Rows)
                {


                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@FormID", item["FormID"]);

                    var tt = item["FormID"].ToString().Trim() + "~" + item["Access"].ToString().Trim();
                    string Access = Converter.DESEncrypt(PassPhrase, EnKey, tt);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@Access", Access);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", UserInfoVM.UserName);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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


    }
}
