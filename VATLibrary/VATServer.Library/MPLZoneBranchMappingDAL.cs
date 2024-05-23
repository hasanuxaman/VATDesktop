using System;
using System.Data.SqlClient;
using System.Data;
using VATViewModel.DTOs;
using System.Collections.Generic;
using System.Reflection;
using VATServer.Ordinary;
using VATServer.Interface;


namespace VATServer.Library
{

    public class MPLZoneBranchMappingDAL : IMPLZoneBranchMapping
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        #region web methods
        public List<MPLZoneBranchMappingVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLZoneBranchMappingVM> VMs = new List<MPLZoneBranchMappingVM>();
            MPLZoneBranchMappingVM vm;
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
SELECT ZB.Id,Z.ZoneID,Z.ZoneName,B.BranchID,B.BranchName
FROM MPLZoneBranchMapping ZB
LEFT OUTER JOIN MPLZoneProfiles Z ON Z.ZoneID = ZB.ZoneId
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = ZB.BranchId ";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLZoneBranchMappingVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.ZoneId = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneName = dr["ZoneName"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchID"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {

                FileLogger.Log("MPLZoneBranchMappingDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLZoneBranchMappingDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<MPLZoneBranchMappingVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLZoneBranchMappingVM> VMs = new List<MPLZoneBranchMappingVM>();
            MPLZoneBranchMappingVM vm;
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
SELECT ZB.Id,Z.ZoneID,Z.ZoneName,B.BranchID,B.BranchName
FROM MPLZoneBranchMapping ZB
LEFT OUTER JOIN MPLZoneProfiles Z ON Z.ZoneID = ZB.ZoneId
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = ZB.BranchId ";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLZoneBranchMappingVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.ZoneId = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneName = dr["ZoneName"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchID"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {

                FileLogger.Log("MPLZoneBranchMappingDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLZoneBranchMappingDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<MPLZoneBranchMappingVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<MPLZoneBranchMappingVM> VMs = new List<MPLZoneBranchMappingVM>();
            MPLZoneBranchMappingVM vm;
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

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false, connVM);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new MPLZoneBranchMappingVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.ZoneId = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneName = dr["ZoneName"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchID"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();

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

                FileLogger.Log("MPLZoneBranchMappingDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLZoneBranchMappingDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
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
                #region SqlText

                sqlText = @"
SELECT ZB.Id,Z.ZoneID,Z.ZoneName,B.BranchID,B.BranchName
FROM MPLZoneBranchMapping ZB
LEFT OUTER JOIN MPLZoneProfiles Z ON Z.ZoneID = ZB.ZoneId
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = ZB.BranchId  
WHERE  1=1 ";
                if (Id > 0)
                {
                    sqlText += @" AND ZB.Id=@Id";
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
                FileLogger.Log("MPLZoneBranchMappingDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLZoneBranchMappingDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToMPLZoneBranchMapping(MPLZoneBranchMappingVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool IsIntegrationAutoCode = false)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            int Id = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region Validation

                if (vm.ZoneId < 0)
                {
                    throw new ArgumentNullException("InsertToMPLZoneBranchMapping",
                                                    "Please enter zone name.");
                }
                if (vm.BranchId < 0)
                {
                    throw new ArgumentNullException("InsertToMPLZoneBranchMapping",
                                                    "Please enter branch name.");
                }

                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                #endregion settingsValue

                #region Old connection

                #endregion

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
                    transaction = currConn.BeginTransaction("InsertToMPLBankInformation");
                }
                #endregion open connection and transaction

                #region existence check
                string[] cFields = { "ZB.ZoneId", "ZB.BranchId" };
                string[] cValues = new string[] { vm.ZoneId.ToString(), vm.BranchId.ToString() };
                var zone = SelectAllList(0, cFields, cValues, currConn, transaction, connVM);
                if (zone.Count > 0)
                {
                    retResults[1] = "Same branch already exists under the zone";
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion

                #region Insert Zone Information

                sqlText = "";
                sqlText += "insert into MPLZoneBranchMapping";
                sqlText += "(";
                sqlText += "ZoneId,";
                sqlText += "BranchId";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "@ZoneId";
                sqlText += ",@BranchId";
                sqlText += ") ";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@ZoneId", vm.ZoneId);
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }
                else
                {
                    cmdInsert.CommandText = "SELECT @@IDENTITY";
                    object lastInsertedId = cmdInsert.ExecuteScalar();
                    var id = lastInsertedId;
                    Id = Convert.ToInt32(id);
                }

                #endregion Insert Zone Information

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
                retResults[1] = "Requested Zone Information successfully added";
                retResults[2] = "" + Id.ToString();
                retResults[3] = "" + Id.ToString();

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.Id.ToString(); //catch ex
                transaction.Rollback();

                FileLogger.Log("MPLZoneBranchMappingDAL", "InsertToMPLZoneBranchMapping", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateMPLZoneBranchMapping(MPLZoneBranchMappingVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
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

                if (vm.ZoneId < 0)
                {
                    throw new ArgumentNullException("UpdateMPLZoneBranchMapping",
                        "Please enter zone name.");
                }
                if (vm.BranchId < 0)
                {
                    throw new ArgumentNullException("UpdateMPLZoneBranchMapping",
                        "Please enter branch name.");
                }

                #endregion Validation
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("BankInformationTransaction");

                #endregion open connection and transaction
                
                #region Update Zone Information

                sqlText = "";
                sqlText = "update MPLZoneBranchMapping set";

                sqlText += "  ZoneId                  =@ZoneId";
                sqlText += " ,BranchId                =@BranchId";
                sqlText += " where Id               =@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@ZoneId", vm.ZoneId);
                cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Zone Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Zone Information successfully updated";
                        retResults[2] = vm.Id.ToString();
                        retResults[3] = vm.Id.ToString();
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Zone";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update Zone";
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

                FileLogger.Log("MPLZoneBranchMappingDAL", "UpdateMPLZoneBranchMapping", ex.ToString() + "\n" + sqlText);

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


        public List<MPLZoneBranchMappingVM> GetZoneCodeWise(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            List<MPLZoneBranchMappingVM> VMs = new List<MPLZoneBranchMappingVM>();
            MPLZoneBranchMappingVM vm;
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
SELECT ZB.Id,Z.ZoneID,Z.ZoneName,B.BranchID,B.BranchName,b.Address BranchAddress
FROM MPLZoneBranchMapping ZB
LEFT OUTER JOIN MPLZoneProfiles Z ON Z.ZoneID = ZB.ZoneId
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = ZB.BranchId  
WHERE  1=1 ";
                if (Id > 0)
                {
                    sqlText += @" AND ZB.ZoneId=@Id";
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

                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new MPLZoneBranchMappingVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.MappingZoneId = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneName = dr["ZoneName"].ToString();
                    vm.MappingBranchId = Convert.ToInt32(dr["BranchID"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.BranchAddress = dr["BranchAddress"].ToString();

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
                FileLogger.Log("MPLZoneBranchMappingDAL", "GetZoneCodeWise", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLZoneBranchMappingDAL", "GetZoneCodeWise", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(MPLZoneBranchMappingVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        #endregion
        

    }
}
