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
    public class VehicleDAL : IVehicle
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        #region web methods

        public List<VehicleVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VehicleVM> VMs = new List<VehicleVM>();
            VehicleVM vm;
            #endregion
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
af.VehicleID
,af.VehicleNo
FROM Vehicles af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VehicleVM();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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

        public List<VehicleVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VehicleVM> VMs = new List<VehicleVM>();
            VehicleVM vm;
            #endregion
            try
            {

                #region sql statement
                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new VehicleVM();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.Code = dr["VehicleCode"].ToString();
                    vm.VehicleType = dr["VehicleType"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = DateTime.Parse(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = DateTime.Parse(dr["LastModifiedOn"].ToString());
                    vm.Info1 = dr["Info1"].ToString();
                    vm.Info2 = dr["Info2"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
                    vm.DriverName = dr["DriverName"].ToString();

                    VMs.Add(vm);
                }

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

 
ISNULL(VehicleCode,'N/A')VehicleCode
,VehicleType
,VehicleNo
,Description
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info1
,Info2
,Info3
,Info4
,Info5
,DriverName
,VehicleID

FROM Vehicles  
WHERE  1=1 
AND isnull(IsArchive,0) = 0 

";
                #endregion

                sqlTextCount += @" select count(VehicleID)RecordCount
FROM Vehicles  
WHERE  1=1 
AND isnull(IsArchive,0) = 0 
";


                if (Id > 0)
                {
                    sqlTextParameter += @" and VehicleID=@VehicleID";
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
                    da.SelectCommand.Parameters.AddWithValue("@VehicleID", Id);
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

        public string[] InsertToVehicle(VehicleVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.VehicleType))
                {
                    throw new ArgumentNullException("InsertToVehicle", "Please select one vehicle type.");
                }
                if (string.IsNullOrEmpty(vm.VehicleNo))
                {
                    throw new ArgumentNullException("InsertToVehicle", "Please enter vehicle no.");
                }

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

                #region VehicleNo existence checking

                //select @Present = count(distinct VehicleNo) from Vehicles where VehicleNo = @VehicleNo;
                sqlText = "select count(VehicleNo) from Vehicles where VehicleNo =@VehicleNo and VehicleType=@vehicleType";
                SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                vhclNoExist.Transaction = transaction;
                vhclNoExist.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo);
                vhclNoExist.Parameters.AddWithValue("@vehicleType", vm.VehicleType);
                countId = (int)vhclNoExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToVehicle", "Same vehicle no('" + vm.VehicleNo + "') already exist.");
                }

                #endregion VehicleNo existence checking

                #region Vehicle new id generation

                //select @VehicleID= isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles;
                sqlText = "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToVehicle", "Unable to create new vehicle");
                }

                #endregion Vehicle new id generation

                //vehicleCode = nextId.ToString();
                vm.VehicleID = nextId.ToString();

                #region Insert new vehicle

                sqlText = "";
                sqlText += @" 
INSERT INTO Vehicles(
 VehicleID
,VehicleCode
,VehicleType
,VehicleNo
,Description
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,DriverName
,IsArchive

) VALUES (
 @VehicleID
,@VehicleCode
,@VehicleType
,@VehicleNo
,@Description
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@DriverName
,@IsArchive
         
) 
";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@VehicleID", vm.VehicleID);
                cmdInsert.Parameters.AddWithValue("@VehicleCode", vm.Code);
                cmdInsert.Parameters.AddWithValue("@VehicleType", vm.VehicleType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@DriverName", vm.DriverName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested vehicle  Information successfully Added.";
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

                #endregion Insert new vehicle

            }
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

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public string[] UpdateToVehicle(VehicleVM vm, SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Validation

                if (vm.VehicleID.Trim() == "0")
                {
                    retResults[0] = "Fail";
                    retResults[1] = "This Vehicle information unable to update!";
                    retResults[2] = vm.VehicleID;

                    return retResults;
                }
                if (string.IsNullOrEmpty(vm.VehicleType))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please select one vehicle type.");
                }
                if (string.IsNullOrEmpty(vm.VehicleNo))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter vehicle no.");
                }

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

                //select @Present = count(VehicleID) from Vehicles where VehicleID = @VehicleID;
                sqlText = "select count(VehicleID) from Vehicles where VehicleID =@VehicleID";
                SqlCommand vhclIDExist = new SqlCommand(sqlText, currConn);
                vhclIDExist.Transaction = transaction;
                vhclIDExist.Parameters.AddWithValue("@VehicleID", vm.VehicleID);

                countId = (int)vhclIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Could not find requested vehicle id.");
                }

                #endregion Vehicle existence checking by id

                #region VehicleNo existence checking by id and requied field

                sqlText = "select count(VehicleNo) from Vehicles ";
                sqlText += " where  VehicleNo=@VehicleNo";
                sqlText += " and VehicleType=@VehicleType";
                sqlText += " and VehicleId<>@VehicleId";
                SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                vhclNoExist.Transaction = transaction;
                vhclNoExist.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo);
                vhclNoExist.Parameters.AddWithValue("@VehicleType", vm.VehicleType);
                vhclNoExist.Parameters.AddWithValue("@VehicleId", vm.VehicleID);

                countId = (int)vhclNoExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Same vehicle no name already exist.");
                }

                #endregion VehicleNo existence checking by id and requied field

                vehicleCode = vm.VehicleID;

                #region Update vehicle

                sqlText = "";
                sqlText = "update Vehicles set";
                //sqlText += " VehicleCode=@VehicleCode";
                sqlText += "  VehicleType   =@VehicleType";
                sqlText += " ,VehicleCode   =@VehicleCode";
                sqlText += " ,VehicleNo     =@VehicleNo";
                sqlText += " ,Description   =@Description";
                sqlText += " ,Comments      =@Comments";
                sqlText += " ,ActiveStatus  =@ActiveStatus";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,DriverName    =@DriverName";

                sqlText += " WHERE VehicleID=@VehicleID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                //cmdUpdate.Parameters.AddWithValue("@VehicleCode", vm.VehicleCode);
                cmdUpdate.Parameters.AddWithValue("@VehicleType", vm.VehicleType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VehicleCode", vm.Code ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@DriverName", vm.DriverName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VehicleID", vm.VehicleID);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Vehicle Information Successfully Update.";
                        retResults[2] = "" + vm.VehicleID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update vehicles.";
                        retResults[2] = "" + vm.VehicleID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update vehicles";
                    retResults[2] = "" + vm.VehicleID;
                }

                #endregion Commit

                #endregion Update vehicle

            }
            #region catch
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public string[] Delete(VehicleVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                        sqlText = "update Vehicles set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where VehicleID=@VehicleID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@VehicleID", ids[i]);
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
                        throw new ArgumentNullException("Vehicle Delete", vm.VehicleID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Vehicle Information Delete", "Could not found any item.");
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
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
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


        public List<string> AutocompleteVehicle(string term, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
            #endregion
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
                sqlText = "";
                sqlText = @"SELECT
VehicleID
,VehicleNo
FROM Vehicles  ";
                sqlText += @" WHERE VehicleNo like '%" + term + "%'  and ActiveStatus='Y' ORDER BY VehicleNo";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["VehicleNo"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return vms;
        }




        #endregion



        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Vehicles");

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


                #region Sql Command

                sqlText = @" SELECT 
  row_number() OVER (ORDER BY [VehicleID]) SL
  
      ,[VehicleCode] Code
      ,[VehicleType]
      ,[VehicleNo]
      ,[Description]
      ,[Comments]
      ,[ActiveStatus]


  FROM Vehicles

  where VehicleID in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";


                var cmd = new SqlCommand(sqlText, currConn, transaction);

                //for (int i = 0; i < len; i++)
                //{
                //    cmd.Parameters.AddWithValue("@itemNo" + i, ids[i]);
                //}

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);


                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #region catch
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

            return dataTable;
        }

//        public DataTable GetExcelAddress(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
//        {
//            #region Variables

//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            int transResult = 0;
//            int countId = 0;
//            string sqlText = "";

//            DataTable dataTable = new DataTable("Vehicles");

//            #endregion

//            #region try

//            try
//            {
//                #region open connection and transaction
//                #region New open connection and transaction
//                if (VcurrConn != null)
//                {
//                    currConn = VcurrConn;
//                }
//                if (Vtransaction != null)
//                {
//                    transaction = Vtransaction;
//                }
//                #endregion New open connection and transaction
//                if (currConn == null)
//                {
//                    currConn = _dbsqlConnection.GetConnection(connVM);
//                    if (currConn.State != ConnectionState.Open)
//                    {
//                        currConn.Open();
//                    }
//                }
//                if (transaction == null)
//                {
//                    transaction = currConn.BeginTransaction("");
//                }
//                #endregion open connection and transaction


//                #region Sql Command

//                sqlText = @" SELECT ROW_NUMBER() OVER (order by ca.CustomerID) SL
//	 ,c.CustomerCode Code
//	 ,c.CustomerName
//      ,ca.[CustomerAddress] [Address]
//  FROM CustomersAddress ca left outer join Customers c
//  on c.CustomerID = ca.CustomerID
//  where c.CustomerCode in  ";


//                var len = ids.Count;

//                sqlText += "( ";

//                for (int i = 0; i < len; i++)
//                {
//                    sqlText += "'" + ids[i] + "',";
//                }

//                if (ids.Count == 0)
//                {
//                    sqlText += "''";
//                }

//                sqlText = sqlText.TrimEnd(',') + ")";


//                var cmd = new SqlCommand(sqlText, currConn, transaction);

//                for (int i = 0; i < len; i++)
//                {
//                    cmd.Parameters.AddWithValue("@code" + i, ids[i]);
//                }

//                var adapter = new SqlDataAdapter(cmd);

//                adapter.Fill(dataTable);


//                if (transaction != null && Vtransaction == null)
//                {
//                    transaction.Commit();
//                }

//                #endregion

//            }
//            #endregion

//            #region catch
//            catch (Exception ex)
//            {
//                if (transaction != null && Vtransaction == null)
//                {
//                    transaction.Rollback();
//                }
//                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//                ////throw ex;

//                FileLogger.Log("CustomerDAL", "GetExcelAddress", ex.ToString() + "\n" + sqlText);

//                throw new ArgumentNullException("", ex.Message.ToString());

//            }
//            #endregion
//            #region finally

//            finally
//            {
//                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
//                {
//                    currConn.Close();
//                }
//            }

//            #endregion

//            return dataTable;
//        }
    }
}
