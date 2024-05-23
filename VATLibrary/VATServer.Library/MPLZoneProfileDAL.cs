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

    public class MPLZoneProfileDAL : IMPLZoneProfile
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        #region web methods
        public List<MPLZoneProfileVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLZoneProfileVM> VMs = new List<MPLZoneProfileVM>();
            MPLZoneProfileVM vm;
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
SELECT * FROM(
SELECT 
'B' Sl, ZoneID
, ZoneName
FROM MPLZoneProfiles
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' ZoneID
, 'ALL Bank' ZoneName  
FROM MPLZoneProfiles
)
AS a
order by Sl,ZoneName ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLZoneProfileVM();
                    vm.ZoneID = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneName = dr["ZoneName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {

                FileLogger.Log("MPLZoneProfileDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLZoneProfileDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<MPLZoneProfileVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLZoneProfileVM> VMs = new List<MPLZoneProfileVM>();
            MPLZoneProfileVM vm;
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
af.ZoneID
,af.ZoneName
FROM MPLZoneProfiles af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLZoneProfileVM();
                    vm.ZoneID = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneName = dr["ZoneName"].ToString();
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

                FileLogger.Log("MPLZoneProfileDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("MPLZoneProfileDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<MPLZoneProfileVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<MPLZoneProfileVM> VMs = new List<MPLZoneProfileVM>();
            MPLZoneProfileVM vm;
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
                    vm = new MPLZoneProfileVM();
                    vm.ZoneID = Convert.ToInt32(dr["ZoneID"].ToString());
                    vm.ZoneCode = dr["ZoneCode"].ToString();
                    vm.ZoneName = dr["ZoneName"].ToString();
                    vm.ZoneLegalName = dr["ZoneLegalName"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.ZipCode = dr["ZipCode"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                    vm.BIN = dr["BIN"].ToString();
                    vm.TINNo = dr["TINNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"].ToString());
                    vm.BanglaLegalName = dr["BanglaLegalName"].ToString();
                    vm.BanglaAddress = dr["BanglaAddress"].ToString();

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

                FileLogger.Log("MPLZoneProfileDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("MPLZoneProfileDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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
SELECT 
ZoneID
,ZoneCode
,ZoneName
,ZoneLegalName
,Address
,City
,ZipCode
,TelephoneNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,VatRegistrationNo
,BIN
,TINNo
,Comments
,ActiveStatus
,CreatedBy
,ISNULL(CreatedOn,'') CreatedOn
,LastModifiedBy
,ISNULL(LastModifiedOn,'') LastModifiedOn
,IsArchive
,BanglaLegalName
,BanglaAddress

FROM MPLZoneProfiles  
WHERE  1=1 AND isnull(IsArchive,0) = 0 ";
                if (Id > 0)
                {
                    sqlText += @" AND ZoneID=@ZoneID";
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
                    da.SelectCommand.Parameters.AddWithValue("@ZoneID", Id);
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
                FileLogger.Log("MPLZoneProfileDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLZoneProfileDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToMPLZoneProfile(MPLZoneProfileVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool IsIntegrationAutoCode = false)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            string zoneCode = vm.ZoneCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                ErrorReturn(vm);

                #region settingsValue

                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Bank") == "Y" ? true : false);

                if (IsIntegrationAutoCode)
                {
                    Auto = true;
                }

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
                    transaction = currConn.BeginTransaction("InsertToMPLBankInformation");
                }
                #endregion open connection and transaction
                
                #region Insert Zone Information

                sqlText = "select count(ZoneID) from MPLZoneProfiles where  ZoneCode=@zoneCode";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@zoneCode", vm.ZoneCode);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToCustomer", "Same Zone  Code('" + vm.ZoneCode + "') already exist");
                }

                sqlText = "";
                sqlText += "insert into MPLZoneProfiles";
                sqlText += "(";
                sqlText += "ZoneCode,";
                sqlText += "ZoneName,";
                sqlText += "ZoneLegalName,";
                sqlText += "Address,";
                sqlText += "City,";
                sqlText += "ZipCode,";
                sqlText += "TelephoneNo,";
                sqlText += "FaxNo,";
                sqlText += "Email,";
                sqlText += "ContactPerson,";
                sqlText += "ContactPersonDesignation,";
                sqlText += "ContactPersonTelephone,";
                sqlText += "ContactPersonEmail,";
                sqlText += "VatRegistrationNo,";
                sqlText += "BIN,";
                sqlText += "TINNo,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "IsArchive,";
                sqlText += "BanglaLegalName,";
                sqlText += "BanglaAddress";


                sqlText += ")";
                sqlText += " values(";
                sqlText += " @ZoneCode";
                sqlText += ",@ZoneName";
                sqlText += ",@ZoneLegalName";
                sqlText += ",@Address";
                sqlText += ",@City";
                sqlText += ",@ZipCode";
                sqlText += ",@TelephoneNo";
                sqlText += ",@FaxNo";
                sqlText += ",@Email";
                sqlText += ",@ContactPerson";
                sqlText += ",@ContactPersonDesignation";
                sqlText += ",@ContactPersonTelephone";
                sqlText += ",@ContactPersonEmail";
                sqlText += ",@VatRegistrationNo";
                sqlText += ",@BIN";
                sqlText += ",@TINNo";
                sqlText += ",@Comments";
                sqlText += ",@ActiveStatus";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@IsArchive";
                sqlText += ",@BanglaLegalName";
                sqlText += ",@BanglaAddress";

                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@ZoneCode", vm.ZoneCode);
                cmdInsert.Parameters.AddWithValue("@ZoneName", vm.ZoneName);
                cmdInsert.Parameters.AddWithValue("@ZoneLegalName", vm.ZoneLegalName);
                cmdInsert.Parameters.AddWithValue("@Address", vm.Address);
                cmdInsert.Parameters.AddWithValue("@City", vm.City);
                cmdInsert.Parameters.AddWithValue("@ZipCode", vm.ZipCode);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail);
                cmdInsert.Parameters.AddWithValue("@VatRegistrationNo", vm.VatRegistrationNo);
                cmdInsert.Parameters.AddWithValue("@BIN", vm.BIN);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@BanglaLegalName", vm.BanglaLegalName);
                cmdInsert.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress);


                transResult = (int)cmdInsert.ExecuteNonQuery();

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
                retResults[2] = "" + vm.ZoneID.ToString();
                retResults[3] = "" + vm.ZoneCode;

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.ZoneID.ToString(); //catch ex
                transaction.Rollback();

                FileLogger.Log("MPLZoneProfileDAL", "InsertToMPLZoneProfile", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateMPLZoneProfile(MPLZoneProfileVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.ZoneID.ToString();

            string zoneCode = vm.ZoneCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                ErrorReturn(vm);
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Bank") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("BankInformationTransaction");

                #endregion open connection and transaction

                
                #region Code
                sqlText = "select count(ZoneID) from MPLZoneProfiles where  ZoneCode=@ZoneCode" +
                          " and ZoneID <>@ZoneID";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@ZoneCode", vm.ZoneCode);
                cmdCodeExist.Parameters.AddWithValue("@ZoneID", vm.ZoneID);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateMPLZoneProfile", "Same Zone  Code('" + vm.ZoneCode + "') already exist");
                }
                #endregion Code

                #region Update Zone Information

                sqlText = "";
                sqlText = "update MPLZoneProfiles set";

                sqlText += "  ZoneCode                 =@ZoneCode";
                sqlText += "  ,ZoneName                 =@ZoneName";
                sqlText += "  ,ZoneLegalName                 =@ZoneLegalName";
                sqlText += "  ,Address                 =@Address";
                sqlText += "  ,City                 =@City";
                sqlText += "  ,ZipCode                 =@ZipCode";
                sqlText += "  ,TelephoneNo                 =@TelephoneNo";
                sqlText += "  ,FaxNo                 =@FaxNo";
                sqlText += "  ,Email                 =@Email";
                sqlText += "  ,ContactPerson                 =@ContactPerson";
                sqlText += "  ,ContactPersonDesignation                 =@ContactPersonDesignation";
                sqlText += "  ,ContactPersonTelephone                 =@ContactPersonTelephone";
                sqlText += "  ,ContactPersonEmail                 =@ContactPersonEmail";
                sqlText += "  ,VatRegistrationNo                 =@VatRegistrationNo";
                sqlText += "  ,BIN                 =@BIN";
                sqlText += "  ,TINNo                 =@TINNo";
                sqlText += "  ,Comments                 =@Comments";
                sqlText += "  ,ActiveStatus                 =@ActiveStatus";
                sqlText += "  ,LastModifiedBy                 =@LastModifiedBy";
                sqlText += "  ,LastModifiedOn                 =@LastModifiedOn";
                sqlText += "  ,IsArchive                 =@IsArchive";
                sqlText += "  ,BanglaLegalName                 =@BanglaLegalName";
                sqlText += "  ,BanglaAddress                 =@BanglaAddress";



                sqlText += " where ZoneID               =@ZoneID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@ZoneCode", vm.ZoneCode);
                cmdUpdate.Parameters.AddWithValue("@ZoneName", vm.ZoneName);
                cmdUpdate.Parameters.AddWithValue("@ZoneLegalName", vm.ZoneLegalName);
                cmdUpdate.Parameters.AddWithValue("@Address", vm.Address);
                cmdUpdate.Parameters.AddWithValue("@City", vm.City);
                cmdUpdate.Parameters.AddWithValue("@ZipCode", vm.ZipCode);
                cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo);
                cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo);
                cmdUpdate.Parameters.AddWithValue("@Email", vm.Email);
                cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail);
                cmdUpdate.Parameters.AddWithValue("@VatRegistrationNo", vm.VatRegistrationNo);
                cmdUpdate.Parameters.AddWithValue("@BIN", vm.BIN);
                cmdUpdate.Parameters.AddWithValue("@TINNo", vm.TINNo);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                cmdUpdate.Parameters.AddWithValue("@BanglaLegalName", vm.BanglaLegalName);
                cmdUpdate.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress);
                cmdUpdate.Parameters.AddWithValue("@ZoneID", vm.ZoneID);

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
                        retResults[2] = vm.ZoneID.ToString();
                        retResults[3] = vm.ZoneCode;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Bank";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update Bank";
                }



                #endregion Commit

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.ZoneID.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("MPLZoneProfileDAL", "UpdateMPLZoneProfile", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(MPLZoneProfileVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteZone"; //Method Name
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
                        sqlText = "update MPLZoneProfiles set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where ZoneID=@ZoneID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@ZoneID", ids[i]);
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
                        throw new ArgumentNullException("Zone Delete", vm.ZoneID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Zone Information Delete", "Could not found any item.");
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
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("MPLZoneProfileDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        private void ErrorReturn(MPLZoneProfileVM vm, SysDBInfoVMTemp connVM = null)
        {
            if (string.IsNullOrWhiteSpace(vm.ContactPerson))
            {
                vm.ContactPerson = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonDesignation))
            {
                vm.ContactPersonDesignation = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonTelephone))
            {
                vm.ContactPersonTelephone = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail))
            {
                vm.ContactPersonEmail = "-";
            }
            
        }

        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
