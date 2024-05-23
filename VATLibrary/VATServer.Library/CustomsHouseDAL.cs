using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using System.Reflection;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;

namespace VATServer.Library
{
    public class CustomsHouseDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        #region web methods

        public List<CustomsHouseVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomsHouseVM> VMs = new List<CustomsHouseVM>();
            CustomsHouseVM vm;
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
ch.ID
,ch.CustomsHouseName
FROM CustomsHouse ch
WHERE  1=1 AND ch.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomsHouseVM();
                    vm.ID = dr["ID"].ToString();
                    vm.CustomsHouseName = dr["CustomsHouseName"].ToString();
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

                FileLogger.Log("CustomsHouseDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomsHouseDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<CustomsHouseVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomsHouseVM> VMs = new List<CustomsHouseVM>();
            CustomsHouseVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement
                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new CustomsHouseVM();
                    vm.ID = dr["ID"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.CustomsHouseName = dr["CustomsHouseName"].ToString();
                    vm.CustomsHouseAddress = dr["CustomsHouseAddress"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = DateTime.Parse(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = DateTime.Parse(dr["LastModifiedOn"].ToString());


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
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("CustomsHouseDAL", "SelectAllLst", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomsHouseDAL", "SelectAllLst", ex.ToString() + "\n" + sqlText);

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

        //
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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
                #region SQLText
                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }
                sqlText += @"

 ID
,ISNULL(Code,'N/A')Code
,CustomsHouseName
,CustomsHouseAddress
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn



FROM CustomsHouse  
WHERE  1=1 
AND isnull(IsArchive,0) = 0 

";
                #endregion

                sqlTextCount += @" select count(ID) RecordCount
FROM CustomsHouse  
WHERE  1=1 
AND isnull(IsArchive,0) = 0 
";


                if (Id > 0)
                {
                    sqlTextParameter += @" and ID=@ID";
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

                sqlText = sqlText + " " + sqlTextParameter;
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

                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@ID", Id);
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("CustomsHouseDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomsHouseDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToCustomsHouse(CustomsHouseVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            string CustomsHCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int nextId = 0;
            #endregion

            #region try

            try
            {
                #region Validation


                #endregion Validation

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

                #region CustomsHouseCode existence checking

                sqlText = "select count(Code) from CustomsHouse where Code=@Code";
                SqlCommand CodeExist = new SqlCommand(sqlText, currConn);
                CodeExist.Transaction = transaction;
                CodeExist.Parameters.AddWithValue("@Code", vm.Code);
                countId = (int)CodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToCustomsHouse", "Same Code no('" + vm.Code + "') already exist.");
                }

                #endregion CustomsHouseCode existence checking

                #region CustomsHouseName existence checking

                sqlText = "select count(Code) from CustomsHouse where CustomsHouseName=@CustomsHouseName";
                SqlCommand NameExist = new SqlCommand(sqlText, currConn);
                NameExist.Transaction = transaction;
                NameExist.Parameters.AddWithValue("@CustomsHouseName", vm.CustomsHouseName);
                countId = (int)NameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToCustomsHouse", "Same Name('" + vm.CustomsHouseName + "') already exist.");
                }

                #endregion CustomsHouseName existence checking


                #region CustomsHouse new id generation

                sqlText = "select isnull(max(cast(ID as int)),0)+1 FROM  CustomsHouse";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToCustomsHouse", "Unable to create new CustomsHouse");
                }

                #endregion CustomsHouse new id generation

                vm.ID = nextId.ToString();

                #region Insert new CustomsHouse

                sqlText = "";
                sqlText += @" 
INSERT INTO CustomsHouse(
 ID
,Code
,CustomsHouseName
,CustomsHouseAddress
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,IsArchive

) VALUES (
 @ID
,@Code
,@CustomsHouseName
,@CustomsHouseAddress
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@IsArchive
         
) 
";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@ID", vm.ID);
                cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                cmdInsert.Parameters.AddWithValue("@CustomsHouseName", vm.CustomsHouseName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CustomsHouseAddress", vm.CustomsHouseAddress ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Customs House  Information successfully Added.";
                        retResults[2] = "" + nextId;

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add vehicle ";
                    retResults[2] = "";
                }

                #endregion Commit

                #endregion Insert new CustomsHouse

            }
            #endregion

            #region catch & Finally
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("CustomsHouseDAL", "FindCustomerId", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
            finally
            {
                if (currConn != null && VcurrConn == null)
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

        public string[] UpdateToCustomsHouse(CustomsHouseVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            string vehicleCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int nextId = 0;
            #endregion

            #region try

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
                transaction = currConn.BeginTransaction("UpdateToVehicle");

                #endregion open connection and transaction

                #region Vehicle existence checking by id

                sqlText = "select count(ID) from CustomsHouse where ID =@ID";
                SqlCommand IDExist = new SqlCommand(sqlText, currConn);
                IDExist.Transaction = transaction;
                IDExist.Parameters.AddWithValue("@ID", vm.ID);

                countId = (int)IDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToCustomsHouse", "Could not find requested CustomsHouse ID.");
                }

                #endregion CustomsHouse existence checking by id

                #region Code existence checking by id and requied field

                sqlText = "select count(Code) from CustomsHouse ";
                sqlText += " where  Code=@Code";
                sqlText += " and ID<>@ID";
                SqlCommand CodeExist = new SqlCommand(sqlText, currConn);
                CodeExist.Transaction = transaction;
                CodeExist.Parameters.AddWithValue("@Code", vm.Code);
                CodeExist.Parameters.AddWithValue("@ID", vm.ID);

                countId = (int)CodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToCustomsHouse", "Same Code already exist.");
                }

                #endregion Code existence checking by id and requied field

                #region Name existence checking by id and requied field

                sqlText = "select count(CustomsHouseName) from CustomsHouse ";
                sqlText += " where  CustomsHouseName=@CustomsHouseName";
                sqlText += " and ID<>@ID";
                SqlCommand NameExist = new SqlCommand(sqlText, currConn);
                NameExist.Transaction = transaction;
                NameExist.Parameters.AddWithValue("@CustomsHouseName", vm.CustomsHouseName);
                NameExist.Parameters.AddWithValue("@ID", vm.ID);

                countId = (int)NameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToCustomsHouse", "Customs House Name Code already exist.");
                }

                #endregion Name existence checking by id and requied field



                #region Update CustomsHouse

                sqlText = "";
                sqlText = "update CustomsHouse set";
                sqlText += "  Code=@Code";
                sqlText += " ,CustomsHouseName   =@CustomsHouseName";
                sqlText += " ,CustomsHouseAddress     =@CustomsHouseAddress";
                sqlText += " ,ActiveStatus  =@ActiveStatus";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";

                sqlText += " WHERE ID=@ID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                //cmdUpdate.Parameters.AddWithValue("@VehicleCode", vm.VehicleCode);
                cmdUpdate.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CustomsHouseName", vm.CustomsHouseName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CustomsHouseAddress", vm.CustomsHouseAddress ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ID", vm.ID);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested CustomsHouse Information Successfully Update.";
                        retResults[2] = "" + vm.ID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update CustomsHouse.";
                        retResults[2] = "" + vm.ID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update CustomsHouse";
                    retResults[2] = "" + vm.ID;
                }

                #endregion Commit

                #endregion Update CustomsHouse

            }
            #endregion

            #region catch finally

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = vm.ID.ToString(); //catch ex

                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("CustomsHouseDAL", "FindCustomerId", ex.ToString() + "\n" + sqlText);

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

            #endregion

            return retResults;
        }

        public string[] Delete(CustomsHouseVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVehicle"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {

                     #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update CustomsHouse set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where ID=@ID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@ID", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Vehicle Delete", vm.ID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("CustomsHouse Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("CustomsHouseDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        #endregion 


    }
}
