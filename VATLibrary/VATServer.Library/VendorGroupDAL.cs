using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using System.Reflection;
using VATServer.Ordinary;
using VATServer.Interface;

namespace VATServer.Library
{
    public class VendorGroupDAL : IVendorGroup
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #region web methods
        public string[] InsertToVendorGroup(VendorGroupVM vm, SysDBInfoVMTemp connVM = null)
       {
           string[] retResults = new string[3];
           retResults[0] = "Fail";
           retResults[1] = "Fail";
           retResults[2] = "";

           SqlConnection currConn = null;
           SqlTransaction transaction = null;
           int transResult = 0;
           int countId = 0;
           string sqlText = "";
           int nextId = 0;

           try
           {
               #region Validation

               if (string.IsNullOrEmpty(vm.VendorGroupName))
               {
                   throw new ArgumentNullException("InsertToVendorGroup",
                                                   "Please enter Vendor group name.");
               }
               if (string.IsNullOrEmpty(vm.GroupType))
               {
                   throw new ArgumentNullException("UpdateToVendorGroup",
                                                   "Invalid Vendor group type.");
               }

               #endregion Validation

               #region open connection and transaction

               currConn = _dbsqlConnection.GetConnection(connVM);
               if (currConn.State != ConnectionState.Open)
               {
                   currConn.Open();
               }
               transaction = currConn.BeginTransaction("InsertToVendorGroup");

               #endregion open connection and transaction


               #region Vendor Group name existence checking


               sqlText = "select count(VendorGroupID) from VendorGroups where  VendorGroupName=@VendorGroupName ";
               SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
               cmdNameExist.Transaction = transaction;
               cmdNameExist.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName);

               countId = (int)cmdNameExist.ExecuteScalar();
               if (countId > 0)
               {
                   throw new ArgumentNullException("InsertToVendorGroup",
                                                   "Same Vendor group name already exist");
               }
               #endregion Vendor Group name existence checking


               #region Vendor Group new id generation
               sqlText = "select isnull(max(cast(VendorGroupID as int)),0)+1 FROM  VendorGroups";
               SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
               cmdNextId.Transaction = transaction;
               nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
               if (nextId <= 0)
               {

                   throw new ArgumentNullException("InsertToVendorGroup",
                                                   "Unable to create new Product No");
               }
               #endregion Customer Group new id generation


               #region Inser new Vendor group
               sqlText = "";
               sqlText += "insert into VendorGroups";
               sqlText += "(";
               sqlText += "VendorGroupID,";
               sqlText += "VendorGroupName,";
               sqlText += "VendorGroupDescription,";
               sqlText += "Comments,";
               sqlText += "ActiveStatus,";
               sqlText += "CreatedBy,";
               sqlText += "CreatedOn,";
               sqlText += "LastModifiedBy,";
               sqlText += "LastModifiedOn,";
               sqlText += "IsArchive,";
               sqlText += "GroupType";
               //sqlText += "Info2,";
               //sqlText += "Info3,";
               //sqlText += "Info4,";
               //sqlText += "Info5";

               sqlText += ")";
               sqlText += " values(";
               sqlText += "'" + nextId + "',";
               sqlText += "@VendorGroupName,";
               sqlText += "@VendorGroupDescription,";
               sqlText += "@Comments,";
               sqlText += "@ActiveStatus,";
               sqlText += "@CreatedBy,";
               sqlText += "@CreatedOn,";
               sqlText += "@LastModifiedBy,";
               sqlText += "@LastModifiedOn,";
               sqlText += "@IsArchive,";
               sqlText += "@GroupType";
               sqlText += ")";

               SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
               cmdInsert.Transaction = transaction;
               cmdInsert.Parameters.AddWithValueAndNullHandle("@VendorGroupName", vm.VendorGroupName);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@VendorGroupDescription ", vm.VendorGroupDescription ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", vm.Comments ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", vm.ActiveStatus);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn));
               cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
               cmdInsert.Parameters.AddWithValueAndNullHandle("@IsArchive", false);
               cmdInsert.Parameters.AddWithValueAndNullHandle("@GroupType", vm.GroupType ?? Convert.DBNull);

               transResult = (int)cmdInsert.ExecuteNonQuery();


               #region Commit


               if (transaction != null)
               {
                   if (transResult > 0)
                   {
                       transaction.Commit();
                       retResults[0] = "Success";
                       retResults[1] = "Requested Vendor group Information successfully Added";
                       retResults[2] = "" + nextId;

                   }
                   else
                   {
                       transaction.Rollback();
                       retResults[0] = "Fail";
                       retResults[1] = "Unexpected erro to add Vendor group";
                       retResults[2] = "";
                   }

               }
               else
               {
                   retResults[0] = "Fail";
                   retResults[1] = "Unexpected erro to add customer group";
                   retResults[2] = "";
               }

               #endregion Commit


               #endregion Inser new Vendor group

           }
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
           ///////////////////////////////////////////////////////////////////////////////////////////////////

           return retResults;
       }

        public string[] UpdateToVendorGroup(VendorGroupVM vm, SysDBInfoVMTemp connVM = null)
       {
           string[] retResults = new string[3];
           retResults[0] = "Fail";
           retResults[1] = "Fail";
           retResults[2] = vm.VendorGroupID;

           SqlConnection currConn = null;
           SqlTransaction transaction = null;
           int transResult = 0;
           int countId = 0;
           string sqlText = "";
           int nextId = 0;

           try
           {
               #region Validation
               if (vm.VendorGroupID.Trim() == "0")
               {
                   retResults[0] = "Fail";
                   retResults[1] = "This vendor group information unable to update!";
                   retResults[2] = vm.VendorGroupID;

                   return retResults;
               }
               if (string.IsNullOrEmpty(vm.VendorGroupID))
               {
                   throw new ArgumentNullException("InsertToVendorGroup",
                                                   "Please enter Vendor group id.");
               }
               if (string.IsNullOrEmpty(vm.GroupType))
               {
                   throw new ArgumentNullException("UpdateToVendorGroup",
                                                   "Invalid Vendor group type.");
               }

               #endregion Validation

               #region open connection and transaction

               currConn = _dbsqlConnection.GetConnection(connVM);
               if (currConn.State != ConnectionState.Open)
               {
                   currConn.Open();
               }
               transaction = currConn.BeginTransaction("UpdateToVendorGroup");

               #endregion open connection and transaction

               #region Vendor Group name existence checking

               sqlText = "select count(VendorGroupID) from VendorGroups where  VendorGroupID=@VendorGroupID";
               SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
               cmdIdExist.Transaction = transaction;
               cmdIdExist.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID);

               countId = (int)cmdIdExist.ExecuteScalar();
               if (countId <= 0)
               {
                   throw new ArgumentNullException("Update VendorGroup",
                               "Could not find requested VendorGroup  id.");
               }
               #endregion Vendor Group name existence checking

               #region Vendor Group  update
               sqlText = "select count(VendorGroupName) from VendorGroups ";
               sqlText += " where 1=1 AND  VendorGroupName =@VendorGroupName ";
               sqlText += " and not VendorGroupID  =@VendorGroupID ";
               SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
               cmdNameExist.Transaction = transaction;
               cmdNameExist.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName ?? Convert.DBNull);
               cmdNameExist.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID ?? Convert.DBNull);

               countId = (int)cmdNameExist.ExecuteScalar();
               if (countId > 0)
               {
                   throw new ArgumentNullException("InsertToVendorGroup",
                                                  "VendorGroup Already Exist!");
               }
               #endregion

               #region Update Vendor group
               sqlText = "";
               sqlText += "update VendorGroups set";
               sqlText += " VendorGroupName        =@VendorGroupName,";
               sqlText += " VendorGroupDescription =@VendorGroupDescription,";
               sqlText += " Comments               =@Comments,";
               sqlText += " ActiveStatus           =@ActiveStatus,";
               sqlText += " LastModifiedBy         =@LastModifiedBy,";
               sqlText += " LastModifiedOn         =@LastModifiedOn,";
               sqlText += " GroupType              =@GroupType";
               sqlText += " where VendorGroupID    =@VendorGroupID";

               SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
               cmdInsert.Transaction = transaction;
               cmdInsert.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValue("@VendorGroupDescription", vm.VendorGroupDescription ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
               cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
               cmdInsert.Parameters.AddWithValue("@GroupType", vm.GroupType ?? Convert.DBNull);
               cmdInsert.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID ?? Convert.DBNull);

               transResult = (int)cmdInsert.ExecuteNonQuery();


               #region Commit


               if (transaction != null)
               {
                   if (transResult > 0)
                   {
                       transaction.Commit();
                       retResults[0] = "Success";
                       retResults[1] = "Requested Vendor group Information successfully Updated.";


                   }
                   else
                   {
                       transaction.Rollback();
                       retResults[0] = "Fail";
                       retResults[1] = "Unexpected error to update vendor group";

                   }

               }
               else
               {
                   retResults[0] = "Fail";
                   retResults[1] = "Unexpected error to update vendor group";

               }

               #endregion Commit


               #endregion Inser new Vendor group

           }
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
           return retResults;
       }

        public List<VendorGroupVM> DropDownAll( SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorGroupVM> VMs = new List<VendorGroupVM>();
            VendorGroupVM vm;
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
SELECT * FROM(
SELECT 
'B' Sl, VendorGroupID
, VendorGroupName
FROM VendorGroups
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Product' VendorGroupName  
FROM VendorGroups
)
AS a
order by Sl,VendorGroupName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorGroupVM();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString(); ;
                    vm.VendorGroupName = dr["VendorGroupName"].ToString();
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

        public List<VendorGroupVM> DropDown( SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorGroupVM> VMs = new List<VendorGroupVM>();
            VendorGroupVM vm;
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
af.VendorGroupID
,af.VendorGroupName
FROM VendorGroups af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorGroupVM();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString();
                    vm.VendorGroupName = dr["VendorGroupName"].ToString();
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

        public List<VendorGroupVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VendorGroupVM> VMs = new List<VendorGroupVM>();
            VendorGroupVM vm;
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
                
                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new VendorGroupVM();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString();
                    vm.VendorGroupName = dr["VendorGroupName"].ToString();
                    vm.VendorGroupDescription = dr["VendorGroupDescription"].ToString();
                    vm.GroupType = dr["GroupType"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
                    vm.Info2 = dr["Info2"].ToString();

                    VMs.Add(vm);
                }
                #endregion SqlExecution

                #endregion

                #region Commit
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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt=new DataTable();
            
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
 VendorGroupID
,VendorGroupName
,VendorGroupDescription
,GroupType
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info3
,Info4
,Info5
,Info2

FROM VendorGroups  
WHERE  1=1 AND isnull(IsArchive,0) = 0 

";


                if (Id > 0)
                {
                    sqlText += @" and VendorGroupID=@VendorGroupID";
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
                    da.SelectCommand.Parameters.AddWithValue("@VendorGroupID", Id);
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

        public string[] Delete(VendorGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVendorGroup"; //Method Name
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
                        sqlText = "update VendorGroups set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where VendorGroupID=@VendorGroupID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@VendorGroupID", ids[i]);
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
                        throw new ArgumentNullException("VendorGroup Delete", vm.VendorGroupID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("VendorGroup Information Delete", "Could not found any item.");
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
        #endregion
    }
}
