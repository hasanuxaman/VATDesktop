using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class CodeDAL : ICode
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       
        #endregion

        #region Methods

        public DataSet SearchCodes(SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataTable dataTable = new DataTable("Search Codes");
            DataSet dataSet = new DataSet("Search Codes");

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

                sqlText = @"SELECT
CodeId,
CodeGroup,
CodeName,
prefix,
prefix prefixOld,
Lenth,
ActiveStatus
                                      FROM Codes
                                      ORDER BY CodeGroup

SELECT DISTINCT CodeGroup FROM Codes order by CodeGroup

";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
                dataAdapter.Fill(dataSet);

                #endregion
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CodeDAL", "SearchCodes", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

               ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

               //// //throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CodeDAL", "SearchCodes", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //////throw ex;
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

            return dataSet;
        }
        
        #endregion 

        #region need to parameterize
        public string[] CodeUpdate(List<CodeVM> codeVMs, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            bool iSTransSuccess = false;

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
                transaction = currConn.BeginTransaction("UpdateToCode");

                #endregion open connection and transaction

                if (codeVMs.Any())
                {
                    foreach (var item in codeVMs)
                    {
                        #region Update Settings

                        sqlText = "";
                        sqlText = "update Codes set";
                        sqlText += " prefix='" + item.prefix + "',";
                        sqlText += " Lenth='" + item.Lenth + "'";

                        sqlText += " where CodeName='" + item.CodeName + "'" + " and CodeGroup='" + item.CodeGroup + "'";

                        sqlText += " IF  EXISTS (SELECT * FROM sys.objects ";
                        sqlText += " WHERE object_id = OBJECT_ID(N'" + "CodeGenerations" + "') AND type in (N'U'))";

                        sqlText += " BEGIN";
                        sqlText += " update CodeGenerations set Prefix = '" + item.prefix + "' where Prefix ='" + item.prefixOld + "'";
                        sqlText += " END";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("CodeUpdate", item.CodeName + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                    }

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("CodeUpdate", "Could not found any item.");
                }

                if (iSTransSuccess == true)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Requested Code Information Successfully Updated.";
                    retResults[2] = "";

                }
                else
                {
                    transaction.Rollback();
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update code.";
                    retResults[2] = "";
                }

            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                retResults[0] = "Fail";
                retResults[1] = sqlex.Message;

                transaction.Rollback();

                FileLogger.Log("CodeDAL", "CodeUpdate", sqlex.ToString() + "\n" + sqlText);

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                transaction.Rollback();

                FileLogger.Log("CodeDAL", "CodeUpdate", ex.ToString() + "\n" + sqlText);

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

            return retResults;
        }

        //currConn to VcurrConn 25-Aug-2020
        public string CodeDataInsert(string CodeGroup, string CodeName, string prefix, string Lenth, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(CodeGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(CodeName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(prefix))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(Lenth))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }

                #endregion Validation

                #region open connection and transaction
                //if (VcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //}

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


                #region ProductExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CodeId)CodeId FROM Codes ";
                sqlText += " WHERE CodeGroup='" + CodeGroup + "' AND CodeName='" + CodeName + "' AND prefix='" + prefix + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("CodesDataInsert", "Please Input Codes Value");
                }
                #endregion ProductExist

                #region Last Price

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText += " INSERT INTO Codes(	CodeGroup,	CodeName,prefix,Lenth,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += " '" + CodeGroup + "',";
                    sqlText += " '" + CodeName + "',";
                    sqlText += " '" + prefix + "',";
                    sqlText += " '" + Lenth + "',";
                    sqlText += " 'Y',";
                    sqlText += " 'admin',";
                    sqlText += " '1900-01-01',";
                    sqlText += " 'admin',";
                    sqlText += " '1900-01-01'";
                    sqlText += " )";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("CodeDataInsert", "Please Input Code Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("CodeDataInsert", "Please Input Code Value");
                    }


                }

                #endregion Last Price

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                transaction.Rollback();

                FileLogger.Log("CodeDAL", "CodeDataInsert", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                FileLogger.Log("CodeDAL", "CodeDataInsert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #endregion

            #region Results

            return retResults;
            #endregion


        }
        
        #endregion
    }
}
