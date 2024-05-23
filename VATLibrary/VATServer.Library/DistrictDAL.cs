using System;
using System.Data.SqlClient;
using System.Data;
using VATViewModel.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VATServer.Ordinary;
using VATServer.Interface;


namespace VATServer.Library
{

    public class DistrictDAL : IDistrict
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


         #region web methods
        public List<DistrictVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<DistrictVM> VMs = new List<DistrictVM>();
            DistrictVM vm;
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
SELECT Id,Name FROM District WHERE  1=1 AND ActiveStatus = 'Y'  ORDER BY Name ASC";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DistrictVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
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

                FileLogger.Log("DistrictDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("DistrictDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<DistrictVM> DropDown(int? divisionId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<DistrictVM> VMs = new List<DistrictVM>();
            DistrictVM vm;
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
SELECT Id,Name FROM District WHERE  1=1 AND ActiveStatus = 'Y' ";

                if (divisionId !=null && divisionId > 0)
                {
                    sqlText += @" AND DivisionId="+divisionId+"";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DistrictVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
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

                FileLogger.Log("DistrictDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DistrictDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<DistrictVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<DistrictVM> VMs = new List<DistrictVM>();
            DistrictVM vm;
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
                #region

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, connVM);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new DistrictVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["Name"].ToString();
                    vm.DivisionId = Convert.ToInt32(dr["DivisionId"].ToString());
                    vm.DivisionName = dr["DivisionName"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.IsActive = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.Status = dr["ActiveStatus"].ToString() == "Y" ? "Active" : "Inactive";
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = OrdinaryVATDesktop.DateTimeToDate(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = OrdinaryVATDesktop.DateTimeToDate(dr["LastModifiedOn"].ToString());

                    VMs.Add(vm);
                }


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

                FileLogger.Log("DistrictDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DistrictDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string count = "100";
            DataTable dt = new DataTable();
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

                int index = -1;
                if (conditionFields != null && conditionValues != null)
                {
                    index = Array.IndexOf(conditionFields, "SelectTop");
                    if (index >= 0)
                    {
                        count = conditionValues[index].ToString();

                        var field = conditionFields.ToList();
                        var values = conditionValues.ToList();
                        field.RemoveAt(index);
                        values.RemoveAt(index);
                        conditionFields = field.ToArray();
                        conditionValues = values.ToArray();
                    }
                }

                #region SqlText

                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT TOP " + count + " ";
                }

                sqlText += @"


 District.Id
,DivisionId
,District.Name
,ISNULL(Division.Name,'') DivisionName
,ISNULL(District.ActiveStatus,'N') ActiveStatus
,District.CreatedBy
,ISNULL(District.CreatedOn,'') CreatedOn
,District.LastModifiedBy
,ISNULL(District.LastModifiedOn,'') LastModifiedOn

FROM District  
LEFT OUTER JOIN Division ON District.DivisionId = Division.Id
WHERE  1=1 ";

                if (Id > 0)
                {
                    sqlText += @" AND District.Id=@Id";
                }

                sqlText += @" ORDER BY District.Id DESC ";

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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("DistrictDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("DistrictDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertDistrict(DistrictVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                #endregion settingsValue


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
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("InsertDistrict");
                }
                #endregion open connection and transaction

                #region Exist

                sqlText = "select count(Id) from District where  DivisionId=@DivisionId AND Name=@Name";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@DivisionId", vm.DivisionId);
                cmdCodeExist.Parameters.AddWithValue("@Name", vm.Name);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertDistrict", "Same Info ('" + vm.Name + "') already exist");
                }
                #endregion Exist

                #region Insert District Information

                sqlText = "";
                sqlText += "INSERT INTO District";
                sqlText += "(";

                sqlText += "DivisionId,";
                sqlText += "Name,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn";


                sqlText += ")";
                sqlText += " VALUES (";

                sqlText += "  @DivisionId";
                sqlText += " ,@Name";
                sqlText += " ,@ActiveStatus";
                sqlText += " ,@CreatedBy";
                sqlText += " ,@CreatedOn";

                sqlText += ")  SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@DivisionId", vm.DivisionId);
                cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());

                retResults[4] = transResult.ToString();

                #endregion Insert District Information

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

                retResults[0] = "Success";
                retResults[1] = "Requested District Information successfully added.";
                retResults[2] = "" + vm.Id.ToString();
                retResults[3] = "" + vm.Name;

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.Id.ToString(); //catch ex
                transaction.Rollback();

                FileLogger.Log("DistrictDAL", "InsertDistrict", ex.ToString() + "\n" + sqlText);

                return retResults;
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

        public string[] UpdateDistrict(DistrictVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.Id.ToString();
            retResults[3] = "";
            retResults[4] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

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
                transaction = currConn.BeginTransaction("UpdateDistrict");

                #endregion open connection and transaction


                #region Exist
                sqlText = "select count(Id) from District where  DivisionId=@DivisionId AND Name=@Name" +
                          " and Id <>@Id";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@DivisionId", vm.DivisionId);
                cmdCodeExist.Parameters.AddWithValue("@Name", vm.Name);
                cmdCodeExist.Parameters.AddWithValue("@Id", vm.Id);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateDistrict", "Same Info ('" + vm.Name + "') already exist");
                }
                #endregion Exist

                #region Update Division Information

                sqlText = "";
                sqlText = "update District SET ";

                sqlText += "   DivisionId=@DivisionId";
                sqlText += "  ,Name=@Name";
                sqlText += "  ,ActiveStatus=@ActiveStatus";
                sqlText += "  ,LastModifiedBy=@LastModifiedBy";
                sqlText += "  ,LastModifiedOn=@LastModifiedOn";

                sqlText += " where Id=@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                cmdUpdate.Parameters.AddWithValue("@DivisionId", vm.DivisionId);
                cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Division Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested District Information successfully updated.";
                        retResults[2] = vm.Id.ToString();
                        retResults[3] = vm.Name;
                        retResults[4] = vm.Id.ToString();
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update District";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update District";
                }

                #endregion Commit

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.Id.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("DistrictDAL", "UpdateDistrict", ex.ToString() + "\n" + sqlText);

                return retResults;
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


        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] Delete(DistrictVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        
    }
}
