using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class AdjustmentDAL : IAdjustment
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Method
       //
        public string[] DeleteAdjustmentName(string AdjId, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = AdjId;

            SqlConnection currConn = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";
            #endregion

            #region try

            try
            {
                #region Transaction Used
                CommonDAL commonDal = new CommonDAL();
                bool tranHas = commonDal.TransactionUsed("AdjustmentHistorys", "AdjId", AdjId,currConn);
                if (tranHas==true)
                {
                    throw new ArgumentNullException("Used In Transaction", 
                        "Requested information could not Deleted," +
                         " This information is Used in AdjustmentHistorys");
                }
                #endregion Transaction Used
                

                #region Validation
                if (string.IsNullOrEmpty(AdjId))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameDelete, "Could not find requested Adjustment.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region product type existence checking

                sqlText = "select count(AdjId) from AdjustmentName where AdjId=@AdjId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValueAndNullHandle("@AdjId", AdjId);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Adjustment.";
                    return retResults;
                }
                #endregion
               

                #region delete product type

                sqlText = "DELETE AdjustmentName WHERE AdjId = @AdjId ";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValueAndNullHandle("@AdjId", AdjId);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Adjustment Information successfully deleted";
                }
               
                #endregion
            }

            #endregion

            #region catch
            
           catch (SqlException sqlex)
            {
                FileLogger.Log("AdjustmentDAL", "DeleteAdjustmentName", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//,"SQL:"+ sqlText + FieldDelimeter + sqlex.Message.ToString());

                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("AdjustmentDAL", "DeleteAdjustmentName", ex.ToString() + "\n" + sqlText);

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

            return retResults;

        }

        #endregion Method
        
        #region web methods
        public List<AdjustmentVM> DropDown( SysDBInfoVMTemp connVM=null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AdjustmentVM> VMs = new List<AdjustmentVM>();
            AdjustmentVM vm;
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
AdjId,AdjName

FROM AdjustmentName 
WHERE  1=1 AND ActiveStatus = 'Y'
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AdjustmentVM();
                    vm.AdjId = dr["AdjId"].ToString();
                    vm.AdjName = dr["AdjName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("AdjustmentDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("AdjustmentDAL", "DropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
       //
        public string[] InsertAdjustmentName(AdjustmentVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.AdjName))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert,
                                                    "Please enter Adjustment name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToAdjName");

                #endregion open connection and transaction

                #region name existence checking

                sqlText = "select count(AdjName) from AdjustmentName where  AdjName=@AdjName";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@AdjName", vm.AdjName);
                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert,
                                                    "Same Adjustment name already exist");
                }
                #endregion product type name existence checking

                #region new id generation
                sqlText = "select isnull(max(cast(AdjId as int)),0)+1 FROM  AdjustmentName";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert, "Unable to create new Adjustment No");
                }
                #endregion product type new id generation

                #region Insert new row to table
                sqlText = "";

                sqlText += @" 
INSERT INTO AdjustmentName(
 AdjId
,AdjName
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Branchid

) VALUES (
 @AdjId
,@AdjName
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn        
,@Branchid
) 
";
                #region oldquery

                //sqlText += "insert into AdjustmentName";
                //sqlText += "(";
                //sqlText += " AdjId,";
                //sqlText += " AdjName,";
                //sqlText += " Comments,";
                //sqlText += " ActiveStatus,";
                //sqlText += " CreatedBy,";
                //sqlText += " CreatedOn,";
                //sqlText += " LastModifiedBy,";
                //sqlText += " LastModifiedOn";
                //sqlText += ")";
                //sqlText += " values(";
                //sqlText += "'" + nextId + "',";
                //sqlText += "'" + AdjName + "',";
                //sqlText += "'" + Comments + "',";
                //sqlText += "'" + ActiveStatus + "',";
                //sqlText += "'" + CreatedBy + "',";
                //sqlText += "'" + CreatedOn + "',";
                //sqlText += "'" + LastModifiedBy + "',";
                //sqlText += "'" + LastModifiedOn + "'";
                //sqlText += ")";
                #endregion oldquery

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Parameters.AddWithValue("@AdjId", nextId);
                cmdInsert.Parameters.AddWithValue("@AdjName", vm.AdjName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Branchid", vm.Branchid);


                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Adjustment Information successfully Added";
                        retResults[2] = nextId.ToString();

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add Adjustment";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add Adjustment";
                    retResults[2] = "";
                }

                #endregion Commit


                #endregion Inser new product type
            }
            #endregion try

            #region catch

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

                FileLogger.Log("AdjustmentDAL", "InsertAdjustmentName", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //////throw ex;
            }

            #endregion catch

            #region Finally

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
       //
        public string[] UpdateAdjustmentName(AdjustmentVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.AdjId;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.AdjId))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Invalid Adjustment id.");
                }
                if (string.IsNullOrEmpty(vm.AdjName))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Please enter Adjustment name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.AdjmsgMethodNameUpdate);

                #endregion open connection and transaction

                #region product type existence checking

                sqlText = "select count(AdjId) from AdjustmentName where  AdjId =@AdjId";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                cmdIdExist.Transaction = transaction;
                //adding parameter
                cmdIdExist.Parameters.AddWithValue("@AdjId", vm.AdjId);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Could not find requested Adjustment id.");
                }
                #endregion

                #region same name product type existence checking
                sqlText = "select count(AdjId) from AdjustmentName ";
                sqlText += " where  AdjName = @AdjName";
                sqlText += " and not AdjId = @AdjId";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;

                //adding parameter
                cmdNameExist.Parameters.AddWithValue("@AdjName", vm.AdjName);
                cmdNameExist.Parameters.AddWithValue("@AdjId", vm.AdjId);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Same Adjustment name already exist");
                }
                #endregion



                #region update existing row to table

                #region sql statement

                sqlText = "";
                sqlText += "UPDATE AdjustmentName SET";
                sqlText += " AdjName =@AdjName";
                sqlText += ",Comments=@Comments";
                sqlText += ",ActiveStatus=@ActiveStatus";
                sqlText += ",LastModifiedBy=@LastModifiedBy";
                sqlText += ",LastModifiedOn=@LastModifiedOn";

                sqlText += " WHERE AdjId =@AdjId";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                //adding parameter
                cmdUpdate.Parameters.AddWithValue("@AdjId", vm.AdjId);
                cmdUpdate.Parameters.AddWithValue("@AdjName", vm.AdjName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Adjustment Information successfully Updated";

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

                #endregion
            }
            #endregion try

            #region catch

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

                FileLogger.Log("AdjustmentDAL", "UpdateAdjustmentName", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //////throw ex;
            }

            #endregion catch

            #region Finally

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

        public List<AdjustmentVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<AdjustmentVM> VMs = new List<AdjustmentVM>();
            AdjustmentVM vm;
            #endregion

            #region try
            
            try
            {
                
                #region sql statement
                
                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false);


                foreach (DataRow dr in dt.Rows)
                {
                    vm = new AdjustmentVM();
                    vm.AdjId = dr["AdjId"].ToString();
                    vm.AdjName = dr["AdjName"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();

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
                FileLogger.Log("AdjustmentDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("AdjustmentDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

       // 
        public DataTable SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
 AdjId
,AdjName
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn

FROM AdjustmentName 
WHERE  1=1 
";


                if (Id!=null && Id != "0")
                {
                    sqlText += @" and AdjId=@AdjId";
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

                if (Id != null && Id != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@AdjId", Id.ToString());
                }
                SqlDataReader dr;
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
                FileLogger.Log("AdjustmentDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("AdjustmentDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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
            return dt;
        }
        
        #endregion 
 
   }
}
