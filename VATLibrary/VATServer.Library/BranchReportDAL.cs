using System;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class BranchReportDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        //public string[] Insert(string Id, string Name, string DBName)
        //{
        public string[] Insert(BranchReportVM vm, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameInsert,
                                                    "Please enter BranchReport name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("Insert");

                #endregion open connection and transaction

                #region name existence checking

                sqlText = "select count(Name) from BranchReports where  Name='" + vm.Name + "'";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameInsert,
                                                    "Same BranchReport name already exist");
                }
                #endregion product type name existence checking

                #region new id generation
                sqlText = "select isnull(max(Id),0)+1 FROM  BranchReports";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameInsert, "Unable to create new BranchReport No");
                }
                #endregion product type new id generation

                #region Insert new row to table
                sqlText = "";



                sqlText += "insert into BranchReports";
                sqlText += "(";
                sqlText += " Id,";
                sqlText += " Name";
                sqlText += " ,DBName";
                sqlText += " ,IsSelf";
                sqlText += " ,IsHeadOffice";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "'" + nextId + "'";
                sqlText += ",'" + vm.Name + "'";
                sqlText += ",'" + vm.DBName + "'";
                sqlText += ",'" + (vm.IsSelf==true?"Y":"N" )+ "'";
                sqlText += ",'" + (vm.IsHeadOffice == true ? "Y" : "N") + "'";
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
                        retResults[1] = "Requested BranchReport Information successfully Added";
                        retResults[2] = nextId.ToString();

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add BranchReport";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add BranchReport";
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

                FileLogger.Log("BranchReportDAL", "Insert", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BranchReportDAL", "Insert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        //public string[] Update(string Id, string Name, string DBName)
        //{ 
        public string[] Update(BranchReportVM vm, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameUpdate,
                                                    "Invalid BranchReport id.");
                }
                if (string.IsNullOrEmpty(vm.Name))
                {
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameUpdate,
                                                    "Please enter BranchReport name.");
                }

                #endregion Validation
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.BranchReportMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region existence checking

                sqlText = "select count(Id) from BranchReports where  Name = '" + vm.Name + "'";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameUpdate,
                                                    "Could not find requested BranchReport id.");
                }
                #endregion
                #region same name existence checking
                sqlText = "select count(Id) from BranchReports ";
                sqlText += " where  Name = '" + vm.Name + "'";
                sqlText += " and not Name = '" + vm.Name + "'";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameUpdate,
                                                    "Same BranchReport name already exist");
                }
                #endregion
                #region update existing row to table

                #region sql statement

                sqlText = "";
                sqlText += "UPDATE BranchReports SET";
                sqlText += " Name ='" + vm.Name + "'";
                sqlText += " ,DBName='" + vm.DBName + "'";
                sqlText += " ,IsSelf='" + (vm.IsSelf == true ? "Y" : "N") + "'";
                sqlText += " ,IsHeadOffice='" + (vm.IsHeadOffice == true ? "Y" : "N") + "'";

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
                        retResults[1] = "Requested BranchReport Information successfully Updated";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update BranchReport";

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
                //////throw sqlex;

                FileLogger.Log("BranchReportDAL", "Update", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BranchReportDAL", "Update", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
                    throw new ArgumentNullException(MessageVM.BranchReportMsgMethodNameDelete, "Could not find requested Branch.");
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

                sqlText = "select count(Id) from BranchReports where Name=@Name";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValueAndNullHandle("@Name", Name);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested BranchReport.";
                    return retResults;
                }
                #endregion
                #region delete

                sqlText = "DELETE BranchReports WHERE Name = @Name ";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValueAndNullHandle("@Name", Name);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested BranchReport Information successfully deleted";
                }

                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                retResults[0] = "Fail";
                retResults[1] = sqlex.Message;
                FileLogger.Log("BranchReportDAL", "Delete", sqlex.ToString() + "\n" + sqlText);

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//,"SQL:"+ sqlText + FieldDelimeter + sqlex.Message.ToString());

                ////////throw sqlex;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                FileLogger.Log("BranchReportDAL", "Delete", ex.ToString() + "\n" + sqlText);


                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                ////////throw ex;
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

        public DataTable SearchBranchReport(string Name, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("BranchReport");
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
                            SELECT Id, Name, DBName,isnull(IsSelf,'N')IsSelf,isnull(IsHeadOffice,'N')IsHeadOffice
                            FROM BranchReports
                            WHERE 	(Name  LIKE '%' +  @Name + '%' OR @Name IS NULL) 
                              order by isnull(IsSelf,'N') desc";

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
                FileLogger.Log("BranchReportDAL", "SearchBranchReport", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchReportDAL", "SearchBranchReport", ex.ToString() + "\n" + sqlText);

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
            return dataTable;
        }

    }
}
