using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VATServer.Interface;
using VATViewModel.DTOs;
namespace VATServer.Library
{
    public class BranchDAL : IBranch
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        //public string[] InsertBranchName(string Id, string Name, string DBName, string BrAddress)
    //{
        public string[] InsertBranchName(BranchVM vm, SysDBInfoVMTemp connVM = null)
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
                if (string.IsNullOrEmpty(vm.Name))
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameInsert,
                                                    "Please enter Branch name.");
                }
                #endregion Validation
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertBranchName");
                #endregion open connection and transaction
                #region name existence checking
                sqlText = "select count(Name) from Branchs where  Name='" + vm.Name + "'";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameInsert,
                                                    "Same Branch name already exist");
                }
                #endregion product type name existence checking
                #region new id generation
                sqlText = "select isnull(max(Id),0)+1 FROM  Branchs";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameInsert, "Unable to create new Branch No");
                }
                #endregion product type new id generation
                #region Insert new row to table
                sqlText = "";
                sqlText += "insert into Branchs";
                sqlText += "(";
                sqlText += " Id,";
                sqlText += " Name,";
                sqlText += " BrAddress,";
                sqlText += " DBName";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "'" + nextId + "',";
                sqlText += "'" + vm.Name + "',";
                sqlText += "'" + vm.BrAddress + "',";
                sqlText += "'" + vm.DBName + "'";
                sqlText += ") SELECT SCOPE_IDENTITY()";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Branch Information successfully Added";
                        retResults[2] = nextId.ToString();
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add Branch";
                        retResults[2] = "";
                    }
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add Branch";
                    retResults[2] = "";
                }
                #endregion Commit
                #endregion Inser new product type
            }
            #endregion try
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                retResults[0] = "Fail";
                retResults[1] = sqlex.Message;

                FileLogger.Log("BOMDAL", "InsertBranchName", sqlex.ToString() + "\n" + sqlText);

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                FileLogger.Log("BOMDAL", "LoadData", ex.ToString() + "\n" + sqlText);

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
        //public string[] UpdateBranchName(string Id, string Name, string DBName, string BrAddress)
        //{
        public string[] UpdateBranchName(BranchVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.Id.ToString();
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
                if (string.IsNullOrEmpty(vm.Id.ToString()))
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameUpdate,
                                                    "Invalid Branch id.");
                }
                if (string.IsNullOrEmpty(vm.Name))
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameUpdate,
                                                    "Please enter Branch name.");
                }
                #endregion Validation
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.BranchMsgMethodNameUpdate);
                #endregion open connection and transaction
                #region existence checking
                sqlText = "select count(Id) from Branchs where  Name = '" + vm.Name + "'";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameUpdate,
                                                    "Could not find requested Branch Id.");
                }
                #endregion
                #region same name existence checking
                sqlText = "select count(Id) from Branchs ";
                sqlText += " where  Name = '" + vm.Name + "'";
                sqlText += " and not Name = '" + vm.Name + "'";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameUpdate,
                                                    "Same Branch name already exist");
                }
                #endregion
                #region update existing row to table
                #region sql statement
                sqlText = "";
                sqlText += "UPDATE Branchs SET";
                sqlText += " Name ='" + vm.Name + "',";
                sqlText += " BrAddress ='" + vm.BrAddress + "',";
                sqlText += " DBName='" + vm.DBName + "'";
                sqlText += " WHERE Name = '" + vm.Name + "'";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                #endregion
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Branch Information successfully Updated";
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Branch";
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
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;

                retResults[0] = "Fail";
                retResults[1] = sqlex.Message;

                FileLogger.Log("BOMDAL", "UpdateBranchName", sqlex.ToString() + "\n" + sqlText);

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;

                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                FileLogger.Log("BOMDAL", "UpdateBranchName", ex.ToString() + "\n" + sqlText);

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
        public string[] Delete(string Name, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Name;
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
                //bool tranHas = commonDal.TransactionUsed("AdjustmentHistorys", "AdjId", Id,currConn);
                //if (tranHas==true)
                //{
                //    throw new ArgumentNullException("Used In Transaction", 
                //        "Requested information could not Deleted," +
                //         " This information is Used in AdjustmentHistorys");
                //}
                #endregion Transaction Used
                #region Validation
                if (string.IsNullOrEmpty(Name))
                {
                    throw new ArgumentNullException(MessageVM.BranchMsgMethodNameDelete, "Could not find requested Branch.");
                }
                #endregion Validation
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region existence checking
                sqlText = "select count(Id) from Branchs where Name=@Name";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValueAndNullHandle("@Name", Name);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Branch.";
                    return retResults;
                }
                #endregion
                #region delete
                sqlText = "DELETE Branchs WHERE Name = '" + Name + "'";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Branch Information successfully deleted";
                }
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//,"SQL:"+ sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;

                retResults[0] = "Fail";
                retResults[1] = sqlex.Message;

                FileLogger.Log("BOMDAL", "Delete", sqlex.ToString() + "\n" + sqlText);

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;

                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                FileLogger.Log("BOMDAL", "Delete", ex.ToString() + "\n" + sqlText);

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
        public DataTable SearchBranchName(string Name, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables
            string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("BranchName");
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
                            SELECT Id, Name, DBName,isnull(BrAddress,'-')BrAddress
                            FROM Branchs
                            WHERE 	(Name  LIKE '%' +  @Name + '%' OR @Name IS NULL) 
                            order by Name
                ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!objComm.Parameters.Contains("@Name"))
                {
                    objComm.Parameters.AddWithValue("@Name", Name);
                }
                else
                {
                    objComm.Parameters["@Name"].Value = Name;
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;

                FileLogger.Log("BOMDAL", "SearchBranchName", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;

                FileLogger.Log("BOMDAL", "SearchBranchName", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchBranchNameByParam(string Name = null, string DBName = null, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables
            string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("BranchName");
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
                            SELECT Id, Name, DBName,BrAddress
                            FROM Branchs
                            WHERE 	1=1                             
                ";
                if (!string.IsNullOrEmpty(Name))
                {
                    sqlText += "and Name=@Name";
                }
                if (!string.IsNullOrEmpty(DBName))
                {
                    sqlText += "and DBName=@DBName";
                }
                sqlText += " order by Name";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Name))
                {
                    objComm.Parameters.AddWithValue("@Name", Name);
                }
                if (!string.IsNullOrEmpty(DBName))
                {
                    objComm.Parameters.AddWithValue("@DBName", DBName);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;

                FileLogger.Log("BOMDAL", "SearchBranchNameByParam", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;

                FileLogger.Log("BOMDAL", "SearchBranchNameByParam", ex.ToString() + "\n" + sqlText);

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

        public List<BranchVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BranchVM> VMs = new List<BranchVM>();
            BranchVM vm;
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
af.Name
,af.DBName
FROM Branchs af order by af.Name
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BranchVM();
                    vm.DBName = dr["DBName"].ToString();
                    vm.Name = dr["Name"].ToString();
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

                FileLogger.Log("BOMDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("BOMDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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


    }
}
