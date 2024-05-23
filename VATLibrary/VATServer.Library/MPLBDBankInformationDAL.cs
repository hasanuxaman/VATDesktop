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

    public class MPLBDBankInformationDAL : IMPLBankInformation
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        #region web methods
        public List<MPLBankInformationVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLBankInformationVM> VMs = new List<MPLBankInformationVM>();
            MPLBankInformationVM vm;
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
'B' Sl, BankID
, BankName
FROM MPLBDBankInformations
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' BankID
, 'ALL Bank' BankName  
FROM MPLBDBankInformations
)
AS a
order by Sl,BankName
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLBankInformationVM();
                    vm.BankID = dr["BankID"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {

                FileLogger.Log("MPLBDBankInformationDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLBDBankInformationDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<MPLBankInformationVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLBankInformationVM> VMs = new List<MPLBankInformationVM>();
            MPLBankInformationVM vm;
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
af.BankID
,af.BankName
FROM MPLBDBankInformations af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLBankInformationVM();
                    vm.BankID = dr["BankID"].ToString();
                    vm.BankName = dr["BankName"].ToString();
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

                FileLogger.Log("MPLBDBankInformationDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("MPLBDBankInformationDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<MPLBankInformationVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<MPLBankInformationVM> VMs = new List<MPLBankInformationVM>();
            MPLBankInformationVM vm;
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
                    vm = new MPLBankInformationVM();
                    vm.BankID = dr["BankID"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vm.BankCode = dr["BankCode"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.AccountNumber = dr["AccountNumber"].ToString();
                    vm.Address1 = dr["Address1"].ToString();
                    vm.Address2 = dr["Address2"].ToString();
                    vm.Address3 = dr["Address3"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

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

                FileLogger.Log("MPLBDBankInformationDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("MPLBDBankInformationDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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
 BankID
,BranchId
,BankName
,BranchName
,AccountNumber
,Address1
,Address2
,Address3
,City
,TelephoneNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,BankCode
,isnull(BranchId, 0) BranchId
FROM MPLBDBankInformations  
WHERE  1=1 AND isnull(IsArchive,0) = 0 

";
                if (Id > 0)
                {
                    sqlText += @" and BankID=@BankID";
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
                    da.SelectCommand.Parameters.AddWithValue("@BankID", Id);
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
                FileLogger.Log("MPLBDBankInformationDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLBDBankInformationDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToMPLBankInformation(MPLBankInformationVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool IsIntegrationAutoCode = false)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            string bankCode = vm.BankCode;

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
                #region Validation

                if (string.IsNullOrWhiteSpace(vm.BankName))
                {
                    throw new ArgumentNullException("InsertToMPLBankInformation",
                                                    "Please enter bank name.");
                }
                if (string.IsNullOrWhiteSpace(vm.BranchName))
                {
                    throw new ArgumentNullException("InsertToMPLBankInformation",
                                                    "Please enter branch name.");
                }
                if (string.IsNullOrWhiteSpace(vm.AccountNumber))
                {
                    throw new ArgumentNullException("InsertToMPLBankInformation",
                                                    "Please enter Account Number.");
                }


                #endregion Validation

                #region settingsValue

                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Bank") == "Y" ? true : false);

                if (IsIntegrationAutoCode)
                {
                    Auto = true;
                }

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
                string[] cFields = { "BankName", "BranchName" };
                string[] cValues = new string[] { vm.BankName, vm.BranchName };
                var banks = SelectAllList(0, cFields, cValues, currConn, transaction, connVM);
                if (banks.Count > 0)
                {
                    retResults[1] = "Same branch already exists under the bank";
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion

                #region Insert Bank Information

                sqlText = "select isnull(max(cast(BankID as int)),0)+1 FROM  MPLBDBankInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToMPLBankInformation",
                                                    "Unable to create new Bank information Id");
                }
                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrWhiteSpace(bankCode))
                    {
                        throw new ArgumentNullException("InsertToMPLBankInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(BankID) from MPLBDBankInformations where  BankCode=@bankCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@bankCode", bankCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same Bank  Code('" + bankCode + "') already exist");
                        }
                    }
                }
                else
                {
                    bankCode = nextId.ToString();
                    vm.BankCode = bankCode;
                }
                #endregion Code

                vm.BankID = nextId.ToString();

                sqlText = "";
                sqlText += "insert into MPLBDBankInformations";
                sqlText += "(";
                sqlText += "BankID,";
                sqlText += "BankName,";
                sqlText += "BranchName,";
                sqlText += "AccountNumber,";
                sqlText += "Address1,";
                sqlText += "Address2,";
                sqlText += "Address3,";
                sqlText += "City,";
                sqlText += "TelephoneNo,";
                sqlText += "FaxNo,";
                sqlText += "Email,";
                sqlText += "ContactPerson,";
                sqlText += "ContactPersonDesignation,";
                sqlText += "ContactPersonTelephone,";
                sqlText += "ContactPersonEmail,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "BankCode,";
                sqlText += "BranchId,";
                sqlText += "IsArchive";


                sqlText += ")";
                sqlText += " values(";
                sqlText += "@BankID";
                sqlText += ",@BankName";
                sqlText += ",@BranchName";
                sqlText += ",@AccountNumber";
                sqlText += ",@Address1";
                sqlText += ",@Address2";
                sqlText += ",@Address3";
                sqlText += ",@City";
                sqlText += ",@TelephoneNo";
                sqlText += ",@FaxNo";
                sqlText += ",@Email";
                sqlText += ",@ContactPerson";
                sqlText += ",@ContactPersonDesignation";
                sqlText += ",@ContactPersonTelephone";
                sqlText += ",@ContactPersonEmail";
                sqlText += ",@Comments";
                sqlText += ",@ActiveStatus";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";
                sqlText += ",@BankCode";
                sqlText += ",@BranchId";
                sqlText += ",@IsArchive";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@BankID", vm.BankID);
                cmdInsert.Parameters.AddWithValue("@BankCode", vm.BankCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BankName", vm.BankName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BranchName", vm.BranchName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@AccountNumber", vm.AccountNumber ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Insert Bank Information

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
                retResults[1] = "Requested Bank Information successfully added";
                retResults[2] = "" + nextId;
                retResults[3] = "" + bankCode;

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = vm.BankID; //catch ex
                transaction.Rollback();

                FileLogger.Log("MPLBDBankInformationDAL", "InsertToBankInformation", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateMPLBankInformation(MPLBankInformationVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.BankID;

            string bankCode = vm.BankCode;

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
                #region Validation

                if (string.IsNullOrWhiteSpace(vm.BankID))
                {
                    throw new ArgumentNullException("UpdateMPLBankInformation",
                                                    "Please enter select bank information to update.");
                }
                if (string.IsNullOrWhiteSpace(vm.BankName))
                {
                    throw new ArgumentNullException("UpdateMPLBankInformation",
                                                    "Please enter bank name.");
                }
                if (string.IsNullOrWhiteSpace(vm.BranchName))
                {
                    throw new ArgumentNullException("UpdateMPLBankInformation",
                                                    "Please enter branch name.");
                }
                if (string.IsNullOrWhiteSpace(vm.AccountNumber))
                {
                    throw new ArgumentNullException("UpdateMPLBankInformation",
                                                    "Please enter Account Number.");
                }

                #endregion Validation
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


                sqlText = "select count(BankID) from MPLBDBankInformations where  BankID=@BankID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@BankID", vm.BankID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateMPLBankInformation", "Could not find requested bank information");

                }

                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrWhiteSpace(bankCode))
                    {
                        throw new ArgumentNullException("UpdateMPLBankInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(BankID) from MPLBDBankInformations where  BankCode=@BankCode" +
                                  " and BankID <>@BankID";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@BankCode", vm.BankCode);
                        cmdCodeExist.Parameters.AddWithValue("@BankID", vm.BankID);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("UpdateMPLBankInformation", "Same Bank  Code('" + bankCode + "') already exist");
                        }
                    }
                }
                else
                {
                    bankCode = vm.BankID;
                }
                #endregion Code

                #region Update Bank Information

                sqlText = "";
                sqlText = "update MPLBDBankInformations set";

                sqlText += "  BankName                  =@BankName";
                sqlText += " ,BranchName                =@BranchName";
                sqlText += " ,AccountNumber             =@AccountNumber";
                sqlText += " ,Address1                  =@Address1";
                sqlText += " ,Address2                  =@Address2";
                sqlText += " ,Address3                  =@Address3";
                sqlText += " ,City                      =@City";
                sqlText += " ,TelephoneNo               =@TelephoneNo";
                sqlText += " ,FaxNo                     =@FaxNo";
                sqlText += " ,Email                     =@Email";
                sqlText += " ,ContactPerson             =@ContactPerson";
                sqlText += " ,ContactPersonDesignation  =@ContactPersonDesignation";
                sqlText += " ,ContactPersonTelephone    =@ContactPersonTelephone";
                sqlText += " ,ContactPersonEmail        =@ContactPersonEmail";
                sqlText += " ,Comments                  =@Comments";
                sqlText += " ,ActiveStatus              =@ActiveStatus";
                sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                sqlText += " ,BankCode                  =@BankCode";

                sqlText += " where BankID               =@BankID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@BankName", vm.BankName);
                cmdUpdate.Parameters.AddWithValue("@BranchName", vm.BranchName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@AccountNumber", vm.AccountNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BankCode", vm.BankCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BankID", vm.BankID ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Bank Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Bank Information successfully updated";
                        retResults[2] = vm.BankID;
                        retResults[3] = bankCode;
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
                retResults[2] = vm.BankID; //catch ex

                transaction.Rollback();

                FileLogger.Log("MPLBDBankInformationDAL", "UpdateMPLBankInformation", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(MPLBankInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBank"; //Method Name
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
                        sqlText = "update MPLBDBankInformations set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where BankID=@BankID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@BankID", ids[i]);
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
                        throw new ArgumentNullException("Bank Delete", vm.BankID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Bank Information Delete", "Could not found any item.");
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
                FileLogger.Log("MPLBDBankInformationDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        private void ErrorReturn(MPLBankInformationVM vm, SysDBInfoVMTemp connVM = null)
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
