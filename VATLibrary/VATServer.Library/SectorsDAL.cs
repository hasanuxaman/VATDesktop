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

    public class SectorsDAL : ISectors
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        #region web methods
        public List<SectorsVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SectorsVM> VMs = new List<SectorsVM>();
            SectorsVM vm;
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
'B' Sl, SectorID
, SectorName
FROM Sectors
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' SectorID
, 'ALL Sector' SectorName  
FROM Sectors
)
AS a
order by Sl,SectorName ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SectorsVM();
                    vm.SectorID = Convert.ToInt32(dr["SectorID"].ToString());
                    vm.SectorName = dr["SectorName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {

                FileLogger.Log("SectorsDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("SectorsDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<SectorsVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SectorsVM> VMs = new List<SectorsVM>();
            SectorsVM vm;
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
af.SectorID
,af.SectorName
FROM Sectors af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SectorsVM();
                    vm.SectorID = Convert.ToInt32(dr["SectorID"].ToString());
                    vm.SectorName = dr["SectorName"].ToString();
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

                FileLogger.Log("SectorsDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("SectorsDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<SectorsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SectorsVM> VMs = new List<SectorsVM>();
            SectorsVM vm;
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
                    vm = new SectorsVM();
                    vm.SectorID = Convert.ToInt32(dr["SectorID"].ToString());
                    vm.SectorCode = dr["SectorCode"].ToString();
                    vm.SectorName = dr["SectorName"].ToString();
                    vm.SectorLegalName = dr["SectorLegalName"].ToString();
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
                    vm.IsActive = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = OrdinaryVATDesktop.DateTimeToDate(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = OrdinaryVATDesktop.DateTimeToDate(dr["LastModifiedOn"].ToString());
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

                FileLogger.Log("SectorsDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("SectorsDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

SectorID
,SectorCode
,SectorName
,SectorLegalName
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
,ISNULL(ActiveStatus,'N') ActiveStatus
,CreatedBy
,ISNULL(CreatedOn,'') CreatedOn
,LastModifiedBy
,ISNULL(LastModifiedOn,'') LastModifiedOn
,ISNULL(IsArchive,0) IsArchive
,BanglaLegalName
,BanglaAddress

FROM Sectors  
WHERE  1=1 ";

                if (Id > 0)
                {
                    sqlText += @" AND SectorID=@SectorID";
                }
                sqlText += @" ORDER BY SectorID DESC ";

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
                    da.SelectCommand.Parameters.AddWithValue("@SectorID", Id);
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
                FileLogger.Log("SectorsDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("SectorsDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertSectors(SectorsVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                ErrorReturn(vm);

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
                    transaction = currConn.BeginTransaction("InsertSectors");
                }
                #endregion open connection and transaction

                #region Insert Sectors Information

                sqlText = "select count(SectorID) from Sectors where  SectorCode=@SectorCode";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@SectorCode", vm.SectorCode);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertSectors", "Same Sector  Code('" + vm.SectorCode + "') already exist");
                }

                sqlText = "";
                sqlText += "INSERT INTO Sectors";
                sqlText += "(";

                sqlText += "SectorCode,";
                sqlText += "SectorName,";
                sqlText += "SectorLegalName,";
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
                sqlText += " VALUES (";
                sqlText += "  @SectorCode";
                sqlText += " ,@SectorName";
                sqlText += " ,@SectorLegalName";
                sqlText += " ,@Address";
                sqlText += " ,@City";
                sqlText += " ,@ZipCode";
                sqlText += " ,@TelephoneNo";
                sqlText += " ,@FaxNo";
                sqlText += " ,@Email";
                sqlText += " ,@ContactPerson";
                sqlText += " ,@ContactPersonDesignation";
                sqlText += " ,@ContactPersonTelephone";
                sqlText += " ,@ContactPersonEmail";
                sqlText += " ,@VatRegistrationNo";
                sqlText += " ,@BIN";
                sqlText += " ,@TINNo";
                sqlText += " ,@Comments";
                sqlText += " ,@ActiveStatus";
                sqlText += " ,@CreatedBy";
                sqlText += " ,@CreatedOn";
                sqlText += " ,@IsArchive";
                sqlText += " ,@BanglaLegalName";
                sqlText += " ,@BanglaAddress";

                sqlText += ")  SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@SectorCode", vm.SectorCode);
                cmdInsert.Parameters.AddWithValue("@SectorName", vm.SectorName);
                cmdInsert.Parameters.AddWithValue("@SectorLegalName", vm.SectorLegalName);
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
                cmdInsert.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                cmdInsert.Parameters.AddWithValue("@BanglaLegalName", vm.BanglaLegalName);
                cmdInsert.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());

                retResults[4] = transResult.ToString();


                #endregion Insert Sectors Information

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
                retResults[1] = "Requested Sector Information successfully added.";
                retResults[2] = "" + vm.SectorID.ToString();
                retResults[3] = "" + vm.SectorName;

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.SectorID.ToString(); //catch ex
                transaction.Rollback();

                FileLogger.Log("SectorsDAL", "InsertSectors", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateSectors(SectorsVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.SectorID.ToString();
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
                ErrorReturn(vm);
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();

                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateSectors");

                #endregion open connection and transaction

                
                #region Code
                sqlText = "select count(SectorID) from Sectors where  SectorCode=@SectorCode" +
                          " and SectorID <>@SectorID";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@SectorCode", vm.SectorCode);
                cmdCodeExist.Parameters.AddWithValue("@SectorID", vm.SectorID);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateSectors", "Same Sector  Code('" + vm.SectorCode + "') already exist");
                }
                #endregion Code

                #region Update Sectors Information

                sqlText = "";
                sqlText = "update Sectors set";

                sqlText += "   SectorCode=@SectorCode";
                sqlText += "  ,SectorName=@SectorName";
                sqlText += "  ,SectorLegalName=@SectorLegalName";
                sqlText += "  ,Address=@Address";
                sqlText += "  ,City=@City";
                sqlText += "  ,ZipCode=@ZipCode";
                sqlText += "  ,TelephoneNo=@TelephoneNo";
                sqlText += "  ,FaxNo=@FaxNo";
                sqlText += "  ,Email=@Email";
                sqlText += "  ,ContactPerson=@ContactPerson";
                sqlText += "  ,ContactPersonDesignation=@ContactPersonDesignation";
                sqlText += "  ,ContactPersonTelephone=@ContactPersonTelephone";
                sqlText += "  ,ContactPersonEmail=@ContactPersonEmail";
                sqlText += "  ,VatRegistrationNo=@VatRegistrationNo";
                sqlText += "  ,BIN=@BIN";
                sqlText += "  ,TINNo=@TINNo";
                sqlText += "  ,Comments=@Comments";
                sqlText += "  ,LastModifiedBy=@LastModifiedBy";
                sqlText += "  ,LastModifiedOn=@LastModifiedOn";
                sqlText += "  ,BanglaLegalName=@BanglaLegalName";
                sqlText += "  ,BanglaAddress=@BanglaAddress";


                sqlText += " where SectorID=@SectorID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@SectorID", vm.SectorID);
                cmdUpdate.Parameters.AddWithValue("@SectorCode", vm.SectorCode);
                cmdUpdate.Parameters.AddWithValue("@SectorName", vm.SectorName);
                cmdUpdate.Parameters.AddWithValue("@SectorLegalName", vm.SectorLegalName);
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
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@BanglaLegalName", vm.BanglaLegalName);
                cmdUpdate.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress);


                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Sectors Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Sector Information successfully updated.";
                        retResults[2] = vm.SectorID.ToString();
                        retResults[3] = vm.SectorCode;
                        retResults[4] = vm.SectorID.ToString();
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Sector";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update Sector";
                }

                #endregion Commit

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.SectorID.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("SectorsDAL", "UpdateSectors", ex.ToString() + "\n" + sqlText);

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
        
        private void ErrorReturn(SectorsVM vm, SysDBInfoVMTemp connVM = null)
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
            if (string.IsNullOrWhiteSpace(vm.ZipCode))
            {
                vm.ZipCode = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.Address))
            {
                vm.Address = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.BanglaAddress))
            {
                vm.BanglaAddress = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.City))
            {
                vm.City = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.Email))
            {
                vm.Email = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.FaxNo))
            {
                vm.FaxNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.TelephoneNo))
            {
                vm.TelephoneNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }
        }

        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] Delete(SectorsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        
    }
}
