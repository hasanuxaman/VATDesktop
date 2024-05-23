using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using System.Reflection;
using VATServer.Interface;

namespace VATServer.Library
{
    public class BranchProfileDAL : IBranchProfile
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #region web methods

        public List<BranchProfileVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BranchProfileVM> VMs = new List<BranchProfileVM>();
            BranchProfileVM vm;
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
af.BranchID
,af.BranchName
FROM BranchProfiles af
WHERE  1=1 

And ActiveStatus='Y'

";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BranchProfileVM();
                    vm.BranchID = Convert.ToInt32(dr["BranchID"]);
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
                FileLogger.Log("BranchProfileDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "DropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());


                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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


        public List<BranchProfileVM> BranchDropDown(int Id =0,SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BranchProfileVM> VMs = new List<BranchProfileVM>();
            BranchProfileVM vm;
            DataTable dt = new DataTable();
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
select 
 ub.Id
,ub.BranchId
,ub.UserId
,ub.Comments
,Ub.CreatedBy
,ub.CreatedOn
,ub.LasatModifiedBy
,ub.LastModifiedOn
,uf.UserName
,bp.BranchName
,bp.BranchCode
,bp.Address
 from UserBranchDetails ub 
 left outer join UserInformations uf on ub.UserId=uf.UserID 
 left outer join BranchProfiles bp on ub.BranchId=bp.BranchID

WHERE  1=1 AND bp.ActiveStatus='Y' --and ub.BranchId  not in (2)
";


                if (Id > 0)
                {
                    sqlText += @" and ub.UserId=@Id";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BranchProfileVM();
                    vm.BranchID = Convert.ToInt32(dr["BranchID"]);
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
                FileLogger.Log("BranchProfileDAL", "BranchDropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "BranchDropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());


                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public List<BranchProfileVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BranchProfileVM> VMs = new List<BranchProfileVM>();
            BranchProfileVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {

                    vm = new BranchProfileVM();


                    vm.BranchID = Convert.ToInt32(dr["BranchID"]);
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.BranchLegalName = dr["BranchLegalName"].ToString();
                    vm.BranchBanglaLegalName = dr["BranchBanglaLegalName"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.BanglaAddress = dr["BanglaAddress"].ToString();
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
                    vm.IP = dr["IP"].ToString();
                    vm.DbName = dr["DbName"].ToString();
                    vm.Id = dr["Id"].ToString();
                    vm.Pass = dr["Pass"].ToString();
                    vm.IsWCF = dr["IsWCF"].ToString();

                    vm.IsCentral = dr["IsCentral"].ToString() == "Y" ? true : false ;

                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();


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
                FileLogger.Log("BranchProfileDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

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
 BranchID
,BranchCode
,BranchName
,BranchLegalName
,isnull(BranchBanglaLegalName,'-')BranchBanglaLegalName
,isnull(BanglaAddress,'-')BanglaAddress
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
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,IsArchive
 ,[IP]
 ,[DbName]
 ,[Id]
 ,[Pass]
, DbType
, isnull(IsWCF,'N')IsWCF
, isnull(IntegrationCode,'0')IntegrationCode
, isnull(IsCentral,'N')IsCentral

FROM BranchProfiles   
WHERE  1=1 AND ActiveStatus ='Y' and isnull(IsArchive,0) = 0 

";
                if (Id != null)
                {
                    sqlText += @" and BranchID=@BranchID";
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

                if (Id != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@BranchID", Id);
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
                FileLogger.Log("BranchProfileDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] InsertToBranchProfileNew(BranchProfileVM vm, bool BranchProfileAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            string sqlText = "";
            string BranchCode = vm.BranchCode;
            int nextId = 0;

            #endregion Variables

            #region Try
            try
            {
                #region Validation
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code", null, currConn, transaction);

                if (string.IsNullOrEmpty(vm.BranchName))
                {
                    throw new ArgumentNullException("InsertToBranchProfile",
                                                    "Please enter BranchProfile group name.");
                }


                #endregion Validation

                #region settingsValue

                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "BranchProfile") == "Y" ? true : false);
                #endregion settingsValue

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
                #region BranchProfile  name existence checking



                #endregion BranchProfile Group name existence checking
                #region BranchProfile  new id generation
                sqlText = "select isnull(max(cast(BranchID as int)),0)+1 FROM  BranchProfiles";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToBranchProfile",
                                                    "Unable to create new BranchProfile No");
                }
                #region Code
                if (Auto == false)
                {
                    if (BranchProfileAutoSave)
                    {
                        if (string.IsNullOrWhiteSpace(vm.BranchCode))
                        {
                            BranchCode = nextId.ToString();
                        }
                        // BranchProfile Group Id
                    }
                    else if (string.IsNullOrEmpty(BranchCode))
                    {
                        throw new ArgumentNullException("InsertToBranchProfile", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        if (code.ToUpper() == "JGHL")
                        {
                            if (BranchCode.Length > 8)
                            {
                                throw new ArgumentNullException("InsertToBranchProfile", "Branch code can not be more then 8 digit");
                            }
                        }
                        else
                        {
                            if (BranchCode.Length > 6)
                            {
                                throw new ArgumentNullException("InsertToBranchProfile", "Branch code can not be more then 4 digit");

                            }
                        }
                        

                        sqlText = "select count(BranchID) from BranchProfiles where  BranchCode=@BranchCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@BranchCode", BranchCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToBranchProfile", "Same BranchProfile  Code('" + BranchCode + "') already exist");
                        }
                    }
                }
                else
                {
                    BranchCode = nextId.ToString();
                }
                #endregion Code

                #endregion BranchProfile  new id generation

                #region Inser new BranchProfile
                sqlText = "";

                sqlText += @" 
INSERT INTO BranchProfiles(
 BranchID
,BranchCode
,BranchName
,BranchLegalName
,BranchBanglaLegalName
,Address
,BanglaAddress
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
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,IsArchive
,IsCentral
,IP
,DbName
,Id
,Pass

) 
VALUES (
 @BranchID
,@BranchCode
,@BranchName
,@BranchLegalName
,@BranchBanglaLegalName
,@Address
,@BanglaAddress
,@City
,@ZipCode
,@TelephoneNo
,@FaxNo
,@Email
,@ContactPerson
,@ContactPersonDesignation
,@ContactPersonTelephone
,@ContactPersonEmail
,@VatRegistrationNo
,@BIN
,@TINNo
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn  
,@IsArchive  
,@IsCentral
,@IP
,@DbName
,@Id
,@Pass
) 
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@BranchID", nextId.ToString());
                cmdInsert.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmdInsert.Parameters.AddWithValue("@BranchName", vm.BranchName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BranchLegalName", vm.BranchLegalName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BranchBanglaLegalName", vm.BranchBanglaLegalName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VatRegistrationNo", vm.VatRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BIN", vm.BIN ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@IsCentral", vm.IsCentral ? "Y" : "N");

                cmdInsert.Parameters.AddWithValue("@IP", vm.IP ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@DbName", vm.DbName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Id", vm.Id ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Pass", vm.Pass ?? Convert.DBNull);


                transResult = (int)cmdInsert.ExecuteNonQuery();


                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested BranchProfile  Information successfully Added";
                retResults[2] = "" + nextId;
                retResults[3] = "" + BranchCode;


                #endregion Inser new BranchProfile

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("BranchProfileDAL", "InsertToBranchProfileNew", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

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

        public string[] UpdateToBranchProfileNew(BranchProfileVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.BranchID.ToString();

            string BranchCode = vm.BranchCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            #endregion Variables

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.BranchID.ToString()))
                {
                    throw new ArgumentNullException("UpdateToBranchProfiles",
                                                    "Invalid BranchProfile ID");
                }
                if (string.IsNullOrEmpty(vm.BranchName))
                {
                    throw new ArgumentNullException("UpdateToBranchProfiles",
                                                    "Invalid BranchProfile Name.");
                }
                //if (string.IsNullOrEmpty(vm.TelephoneNo))
                //{
                //    throw new ArgumentNullException("UpdateToBranchProfiles",
                //                                    "Please enter BranchProfile TelephoneNo");
                //}


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToBranchProfiles");

                #endregion open connection and transaction

                if (vm.BranchCode.Length > 6)
                {
                    throw new ArgumentNullException("InsertToBranchProfile", "Branch code can not be more then 4 digit");
                }

                #region BranchProfile  existence checking

                sqlText = "select count(BranchID) from BranchProfiles where  BranchID=@BranchID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@BranchID", vm.BranchID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToBranchProfiles",
                                "Could not find requested BranchProfiles  id.");
                }

                #endregion BranchProfile Group existence checking

                #region Update new BranchProfile group
                sqlText = "";
                sqlText = "update BranchProfiles set";
                sqlText += "  BranchCode              =@BranchCode";
                sqlText += "  ,BranchName              =@BranchName";
                sqlText += "  ,BranchLegalName              =@BranchLegalName";
                sqlText += "  ,BranchBanglaLegalName              =@BranchBanglaLegalName";
                sqlText += "  ,Address              =@Address";
                sqlText += "  ,BanglaAddress              =@BanglaAddress";
                sqlText += "  ,City              =@City";
                sqlText += "  ,ZipCode              =@ZipCode";
                sqlText += "  ,TelephoneNo              =@TelephoneNo";
                sqlText += "  ,FaxNo              =@FaxNo";
                sqlText += "  ,Email              =@Email";
                sqlText += "  ,ContactPerson              =@ContactPerson";
                sqlText += "  ,ContactPersonDesignation              =@ContactPersonDesignation";
                sqlText += "  ,ContactPersonTelephone              =@ContactPersonTelephone";
                sqlText += "  ,ContactPersonEmail              =@ContactPersonEmail";
                sqlText += "  ,VatRegistrationNo              =@VatRegistrationNo";
                sqlText += "  ,BIN              =@BIN";
                sqlText += "  ,TINNo              =@TINNo";
                sqlText += "  ,Comments              =@Comments";
                sqlText += "  ,IsCentral              =@IsCentral";
                sqlText += "  ,ActiveStatus              =@ActiveStatus";
                sqlText += "  ,CreatedBy              =@CreatedBy";
                sqlText += "  ,CreatedOn              =@CreatedOn";
                sqlText += "  ,LastModifiedBy              =@LastModifiedBy";
                sqlText += "  ,LastModifiedOn              =@LastModifiedOn";

                sqlText += "  ,IP              =@IP";
                sqlText += "  ,DbName              =@DbName";
                sqlText += "  ,Id              =@Id";
                sqlText += "  ,Pass              =@Pass";

                sqlText += " WHERE BranchID           =@BranchID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@BranchID", vm.BranchID);
                cmdUpdate.Parameters.AddWithValue("@BranchCode", vm.BranchCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BranchName", vm.BranchName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BranchLegalName", vm.BranchLegalName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BranchBanglaLegalName", vm.BranchBanglaLegalName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VatRegistrationNo", vm.VatRegistrationNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BIN", vm.BIN ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsCentral", vm.IsCentral ? "Y" : "N");
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);

                cmdUpdate.Parameters.AddWithValue("@IP", vm.IP ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@DbName", vm.DbName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Pass", vm.Pass ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested BranchProfiles  Information successfully Update";
                        retResults[2] = vm.BranchID.ToString();
                        retResults[3] = BranchCode;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update BranchProfiles ";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update BranchProfile group";
                }

                #endregion Commit

                #endregion

            }
            #endregion
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("BranchProfileDAL", "UpdateToBranchProfileNew", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public string[] Delete(BranchProfileVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBranchProfile"; //Method Name
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
                        sqlText = "update BranchProfiles set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where BranchID=@BranchID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@BranchID", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("BranchProfile Delete", vm.BranchID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("BranchProfile Information Delete", "Could not found any item.");
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
                FileLogger.Log("BranchProfileDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAllAccountDetails(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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

SELECT [SL]
      ,[BranchId]
      ,[AccountCode]
      ,[AccountName]
      ,[Location]
      ,[LocationCode]
      ,[LocationAddress]
  FROM AccountDetails   
WHERE  1=1  

";
                if (Id != null)
                {
                    sqlText += @" and SL=@SL";
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

                if (Id != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@SL", Id);
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
                FileLogger.Log("BranchProfileDAL", "SelectAllAccountDetails", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "SelectAllAccountDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SelectAllChargeCodes(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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

SELECT [Id]
      ,[ChargeCode]
      ,[Description]
      ,isnull(type, 'IAS')Type

  FROM IASChargeCode
WHERE  1=1  

";
                if (Id != null)
                {
                    sqlText += @" and Id=@SL";
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

                if (Id != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@SL", Id);
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
                FileLogger.Log("BranchProfileDAL", "SelectAllChargeCodes", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "SelectAllChargeCodes", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        #endregion

        public string[] InsertToBranchMapDetails(BranchProfileVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            string sqlText = "";

            #endregion Variables

            #region Try

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

                #region BranchMapDetails  new id generation
                sqlText = "select isnull(max(cast(SL as int)),0)+1 FROM  BranchMapDetails";
                SqlCommand cmdnextId = new SqlCommand(sqlText, currConn);
                cmdnextId.Transaction = transaction;
                int nextId = (int)cmdnextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Branch Address",
                                                    "Unable to create new Branch");
                }
                vm.DetailsSL = nextId;

                #endregion BranchMapDetails  new id generation

                #region Inser new BranchMapDetails

                #region chack exit

                sqlText = "select isnull(count(BranchCode),0)BranchCode FROM  BranchMapDetails where BranchId=@BranchId and IntegrationCode=@IntegrationCode";
                SqlCommand cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                cmdisExitCode.Parameters.AddWithValue("@BranchId", vm.BranchID);
                cmdisExitCode.Parameters.AddWithValue("@IntegrationCode", vm.IntegrationCode);

                int isExit = (int)cmdisExitCode.ExecuteScalar();

                #endregion chack exit

                if (isExit == 0)
                {

                    #region sqlText

                    sqlText = "";
                    sqlText += "insert into BranchMapDetails";
                    sqlText += "(";
                    sqlText += "BranchCode";
                    sqlText += ",IntegrationCode";
                    sqlText += ",BranchId";
                    sqlText += ",Address";
                    sqlText += ",BranchName";
                    sqlText += ")";
                    sqlText += " values(";

                    sqlText += "  @BranchCode";
                    sqlText += " ,@IntegrationCode";
                    sqlText += " ,@BranchId";
                    sqlText += " ,@Address";
                    sqlText += " ,@BranchName";

                    sqlText += ")SELECT SCOPE_IDENTITY()";

                    #endregion sqlText

                    #region SqlCommand

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@BranchCode", vm.BranchCode);
                    cmdInsert.Parameters.AddWithValue("@IntegrationCode", vm.IntegrationCode);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchID);
                    cmdInsert.Parameters.AddWithValue("@Address", vm.DetailsAddress);
                    cmdInsert.Parameters.AddWithValue("@BranchName", vm.BranchName);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    #endregion SqlCommand

                    #region transResult

                    if (transResult > 0)
                    {
                        retResults[0] = "Success";
                        retResults[1] = " Your Requested successfully Added";
                        retResults[2] = vm.DetailsSL.ToString();
                        retResults[3] = vm.DetailsSL.ToString();
                    }

                    #endregion transResult

                    #region Commit

                    #region Commit
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            if (transResult > 0)
                            {
                                transaction.Commit();

                            }
                            else
                            {
                                transaction.Rollback();
                                retResults[0] = "Fail";
                                retResults[1] = "Unexpected erro to add Branch";
                                retResults[2] = "";
                                retResults[3] = "";
                            }

                        }
                        else
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add Branch ";
                            retResults[2] = "";
                            retResults[3] = "";
                        }
                    }
                    #endregion Commit

                    #endregion Commit

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = " Your Requested Branch Already Exit";
                    retResults[2] = vm.DetailsSL.ToString();
                }

                #endregion Inser new BranchMapDetails

            }

            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();

                FileLogger.Log("BranchProfileDAL", "InsertToBranchMapDetails", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                FileLogger.Log("BranchProfileDAL", "InsertToBranchMapDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
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

        public DataTable SearchBranchMapDetails(BranchProfileVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            DataTable dataTable = new DataTable("Branch");

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

                sqlText = @"
SELECT 
       SL
      ,BranchCode
      ,IntegrationCode
      ,BranchId
      ,Address
      ,BranchName
  FROM BranchMapDetails

  where 1=1

";
                if (vm.BranchID!=0)
                {
                    sqlText += @"  and BranchId=@BranchId";
                }

                sqlText += @"  order by  SL";

                SqlCommand objCommCustomerInformation = new SqlCommand();
                objCommCustomerInformation.Connection = currConn;
                objCommCustomerInformation.Transaction = transaction;
                objCommCustomerInformation.CommandText = sqlText;
                objCommCustomerInformation.CommandType = CommandType.Text;

                if (vm.BranchID != 0)
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@BranchId"))
                    {
                        objCommCustomerInformation.Parameters.AddWithValue("@BranchId", vm.BranchID); 
                    }
                    else 
                    {
                        objCommCustomerInformation.Parameters["@BranchId"].Value = vm.BranchID;
                    }
                }
                

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCustomerInformation);
                dataAdapter.Fill(dataTable);



                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BranchProfileDAL", "SearchBranchMapDetails", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {

                FileLogger.Log("BranchProfileDAL", "SearchBranchMapDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

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

        public string[] UpdateToBranchMapDetails(BranchProfileVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion Variables

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.DetailsSL.ToString()))
                {
                    throw new ArgumentNullException("UpdateToBranch",
                                                    "Invalid Branch");
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

                #region Update new Branch Map Details

                sqlText = "";
                sqlText = "update BranchMapDetails set";

                sqlText += " IntegrationCode=@IntegrationCode";
                sqlText += " ,Address=@Address";
                sqlText += " ,BranchName=@BranchName";
                sqlText += " where SL=@SL";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@IntegrationCode", vm.IntegrationCode);
                cmdUpdate.Parameters.AddWithValue("@Address", vm.DetailsAddress);
                cmdUpdate.Parameters.AddWithValue("@BranchName", vm.BranchName);
                cmdUpdate.Parameters.AddWithValue("@SL", vm.DetailsSL);
                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested Branch successfully Update";
                            retResults[2] = vm.DetailsSL.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update Branch ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update Branch";
                    }
                }

                #endregion Commit

                #endregion

            }

            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;

                FileLogger.Log("BranchProfileDAL", "UpdateToBranchMapDetails", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;

                FileLogger.Log("BranchProfileDAL", "UpdateToBranchMapDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        public string[] DeleteBranchMapDetails(BranchProfileVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.DetailsSL.ToString();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion Variables

            #region try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.DetailsSL.ToString()))
                {
                    throw new ArgumentNullException("DeleteIItem",
                                "Could not find requested Item.");
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


                if (!string.IsNullOrEmpty(vm.DetailsSL.ToString()))
                {
                    sqlText = "delete BranchMapDetails where SL=@SL";
                }
                
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                cmdDelete.Parameters.AddWithValue("@SL", vm.DetailsSL);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Branch successfully deleted";
                }
                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Branch  Address successfully deleted";
                        retResults[2] = "";

                    }
                }

                #endregion Commit

            }

            #endregion try

            #region Catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("BranchProfileDAL", "DeleteBranchMapDetails", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BranchProfileDAL", "DeleteBranchMapDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
            }
            #endregion

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

            return retResults;
        }

        public DataTable GetExcelData(List<string> BranchIdList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
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


                sqlText = @"SELECT 
       [BranchCode]
      ,[BranchName]
      ,[BranchLegalName]
      ,[Address]
      ,[City]
      ,[ZipCode]
      ,[TelephoneNo]
      ,[FaxNo]
      ,[Email]
      ,[ContactPerson]
      ,[ContactPersonDesignation]
      ,[ContactPersonTelephone]
      ,[ContactPersonEmail]
      ,[VatRegistrationNo]
      ,[BIN]
      ,[TINNo]
      ,[Comments]
      ,[IsCentral]
      ,[ActiveStatus]
  FROM [dbo].[BranchProfiles]
  where 1=1 ";

                if (BranchIdList.Count > 0)
                {
                    sqlText += " and [BranchID] in (";

                    var len = BranchIdList.Count;

                    for (int i = 0; i < len; i++)
                    {
                        sqlText += "'" + BranchIdList[i] + "'";

                        if (i != (len - 1))
                        {
                            sqlText += ",";
                        }
                    }

                    if (len == 0)
                    {
                        sqlText += "''";
                    }

                    sqlText += ")";
                }
                
                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("BranchProfileDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
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
        }

        public DataTable GetExcelBranchMapDetails(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BranchMapDetails");

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

                sqlText = @"SELECT

      b.[BranchCode] 
      ,bd.[IntegrationCode]
      ,bd.[Address]
      ,bd.[BranchName]

  FROM BranchMapDetails bd left outer join BranchProfiles b 
  on bd.BranchId  = b.BranchId
  where bd.BranchId in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += " order by b.BranchCode,b.BranchId";

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

                FileLogger.Log("ProductDAL", "GetExcelProductDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public List<string> AutocompleteBranch(string term, SysDBInfoVMTemp connVM = null)
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
                sqlText = @"SELECT Top 100
BranchCode
,BranchName
FROM BranchProfiles  ";

                sqlText += @" WHERE BranchName like '%" + term + "%'  and ActiveStatus='Y' ORDER BY BranchName";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["BranchName"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
                #endregion
            }
            #region catch

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

    }

}
