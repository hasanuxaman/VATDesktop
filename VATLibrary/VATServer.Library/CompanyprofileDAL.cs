using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;

namespace VATServer.Library
{
    public class CompanyprofileDAL : ICompanyprofile
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

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

                if (string.IsNullOrEmpty(companyProfiles.CompanyName))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter company name.");
                }
                if (string.IsNullOrEmpty(companyProfiles.CompanyLegalName))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter company legal name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateCompanyProfileNew");

                #endregion open connection and transaction

                #region CompanyProfile existence checking by id

                //select @Present = count(VehicleID) from Vehicles where VehicleID = @VehicleID;
                sqlText = "select count(CompanyID) from CompanyProfiles where CompanyID = '" + companyProfiles.CompanyID + "'";
                SqlCommand vhclIDExist = new SqlCommand(sqlText, currConn);
                vhclIDExist.Transaction = transaction;
                countId = (int)vhclIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Could not find requested Company Information.");
                }

                #endregion CompanyProfile existence checking by id

                #region companyProfiles existence checking by id and requied field

                //sqlText = "select count(CompanyName) from CompanyProfiles ";
                //sqlText += " where  CompanyID='" + companyProfiles.CompanyID + "'";
                //sqlText += " and CompanyName='" + companyProfiles.CompanyName + "'";
                //SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                //vhclNoExist.Transaction = transaction;
                //countId = (int)vhclNoExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("UpdateCompanyProfileNew", "Same company profile already exist.");
                //}

                #endregion companyProfiles existence checking by id and requied field

                #region Update company profile

                sqlText = "";
                sqlText = sqlText + "  update CompanyProfiles set ";
                //sqlText = sqlText + " CompanyID='" + NewCompanyID + "',";//"@CompanyName,";
                sqlText = sqlText + " CompanyName='" + companyProfiles.CompanyName + "',";//"@CompanyName,";
                sqlText = sqlText + " CompanyLegalName='" + companyProfiles.CompanyLegalName + "',";//CompanyLegalName,";
                sqlText = sqlText + " Address1='" + companyProfiles.Address1 + "',";//Address1,";
                sqlText = sqlText + " Address2='" + companyProfiles.Address2 + "',";//Address2,";
                sqlText = sqlText + " Address3='" + companyProfiles.Address3 + "',";//Address3,";
                sqlText = sqlText + " City='" + companyProfiles.City + "',";//City,";
                sqlText = sqlText + " ZipCode='" + companyProfiles.ZipCode + "',";//ZipCode,";
                sqlText = sqlText + " TelephoneNo='" + companyProfiles.TelephoneNo + "',";//TelephoneNo,";
                sqlText = sqlText + " FaxNo='" + companyProfiles.FaxNo + "',";//FaxNo,";
                sqlText = sqlText + " Email='" + companyProfiles.Email + "',";//Email,";
                sqlText = sqlText + " ContactPerson='" + companyProfiles.ContactPerson + "',";//ContactPerson,";
                sqlText = sqlText + " ContactPersonDesignation='" + companyProfiles.ContactPersonDesignation + "',";//ContactPersonDesignation,";
                sqlText = sqlText + " ContactPersonTelephone='" + companyProfiles.ContactPersonTelephone + "',";//ContactPersonTelephone,";
                sqlText = sqlText + " ContactPersonEmail='" + companyProfiles.ContactPersonEmail + "',";//ContactPersonEmail,";
                sqlText = sqlText + " TINNo='" + companyProfiles.TINNo + "',";//TINNo,";
                sqlText = sqlText + " VatRegistrationNo='" + companyProfiles.VatRegistrationNo + "',";//VatRegistrationNo,";
                sqlText = sqlText + " Section='" + companyProfiles.Section + "',";//VatRegistrationNo,";
                sqlText = sqlText + " CompanyType='" + companyProfiles.CompanyType + "',";//VatRegistrationNo,";
                sqlText = sqlText + " Comments='" + companyProfiles.Comments + "',";//Comments,";
                sqlText = sqlText + " ActiveStatus='" + companyProfiles.ActiveStatus + "',";//ActiveStatus,";
                sqlText = sqlText + " LastModifiedBy='" + companyProfiles.LastModifiedBy + "',";//LastModifiedBy,";
                sqlText = sqlText + " LastModifiedOn='" + companyProfiles.LastModifiedOn + "',";//LastModifiedOn,";

                sqlText = sqlText + " IsVDSWithHolder='" + companyProfiles.IsVDSWithHolder + "',";

                sqlText = sqlText + " BusinessNature='" + companyProfiles.BusinessNature + "',";
                sqlText = sqlText + " AccountingNature='" + companyProfiles.AccountingNature + "',";
                sqlText = sqlText + " BIN='" + companyProfiles.BIN + "',";
                sqlText = sqlText + " License='" + companyProfiles.License + "'";
                //sqlText = sqlText + " FYearStart='" + companyProfiles.FYearStart + "',";//FYearStart,";
                //sqlText = sqlText + " FYearEnd='" + companyProfiles.FYearEnd + "'";//FYearEnd
                if (!string.IsNullOrEmpty(companyProfiles.Tom))
                {
                    sqlText = sqlText + ", Tom='" + companyProfiles.Tom + "',";//"@CompanyName,";
                    sqlText = sqlText + " Jary='" + companyProfiles.Jary + "',";//CompanyLegalName,";
                    sqlText = sqlText + " Miki='" + companyProfiles.Miki + "',";//vat no";
                    sqlText = sqlText + " Mouse='" + companyProfiles.Mouse + "'";//processor id";
                }
                sqlText = sqlText + " where CompanyID='" + companyProfiles.CompanyID + "'";//CompanyID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Unable to Update Company Information ");
                }

                #region Update Sys DB Information

                string CompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string CompanyName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                string bin = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.BIN);

                sqlText = "";
                sqlText += " update CompanyInformations set " +
                           "CompanyName='" + CompanyName + "'" +
                           ",Bin='" + bin + "'" +
                           " where CompanyID='" + CompanyID + "'";
                currConn.ChangeDatabase("SymphonyVATSys");
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;

                transResult = (int)cmdPrefetch.ExecuteNonQuery();
                if (transResult < 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Unable to Update Company Information ");

                }
                #endregion Update Sys DB Information
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Company Profile Information Successfully Update.";
                        retResults[2] = "" + companyProfiles.CompanyID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update company profile.";
                        retResults[2] = "" + companyProfiles.CompanyID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update company profile.";
                    retResults[2] = "" + companyProfiles.CompanyID;
                }

                #endregion Commit

                #endregion Update company profile

            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CompanyprofileDAL", "UpdateCompanyProfileNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CompanyprofileDAL", "UpdateCompanyProfileNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
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

        #endregion

        #region Old Methods

        public DataTable SearchCompanyProfile(SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("CProfile");
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
CompanyId,
isnull(CompanyName,'N/A')CompanyName, 
isnull(CompanyLegalName,'N/A')CompanyLegalName ,
isnull(Address1,'N/A')Address1,
isnull(Address2,'N/A')Address2,
isnull(Address3,'N/A')Address3,
isnull(City,'N/A')City,
isnull(ZipCode,'N/A')ZipCode,
isnull(TelephoneNo,'N/A')TelephoneNo ,
isnull(FaxNo,'N/A')FaxNo ,
isnull(Email,'N/A')Email,
isnull(ContactPerson,'N/A')ContactPerson,
isnull(ContactPersonDesignation,'N/A')ContactPersonDesignation,
isnull(ContactPersonTelephone,'N/A')ContactPersonTelephone,
isnull(ContactPersonEmail ,'N/A')ContactPersonEmail,
isnull(VatRegistrationNo,'N/A')VatRegistrationNo,
isnull(TINNo,'N/A')TINNo,
isnull(CompanyType,'-')CompanyType,
isnull(Comments,'N/A')Comments,
isnull(ActiveStatus,'N')ActiveStatus,
convert(varchar, StartDateTime,120)StartDateTime,
convert(varchar, FYearStart,120)FYearStart,
convert(varchar, FYearEnd,120)FYearEnd,
isnull(IsVDSWithHolder,'N')IsVDSWithHolder,
isnull(BusinessNature,'N/A')BusinessNature,
isnull(AccountingNature,'N/A')AccountingNature,
isnull(BIN,'N/A')BIN,
isnull(Section,'-')Section,
isnull(CompanyType,'-')CompanyType,
License

FROM  CompanyProfiles";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CompanyprofileDAL", "SearchCompanyProfile", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CompanyprofileDAL", "SearchCompanyProfile", ex.ToString() + "\n" + sqlText);

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
            return dataTable;
        }

        #endregion

        #region web methods
        public List<CompanyProfileVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CompanyProfileVM> VMs = new List<CompanyProfileVM>();
            CompanyProfileVM vm;
            #endregion
            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new CompanyProfileVM();
                        vm.CompanyID = dr["CompanyID"].ToString();

                        vm.CompanyName = dr["CompanyName"].ToString();
                        vm.CompanyLegalName = dr["CompanyLegalName"].ToString();
                        vm.Address1 = dr["Address1"].ToString();
                        vm.Address2 = dr["Address2"].ToString();
                        vm.Address3 = dr["Address3"].ToString();
                        vm.City = dr["City"].ToString();
                        vm.ZipCode = dr["ZipCode"].ToString();
                        vm.TelephoneNo = dr["TelephoneNo"].ToString();
                        vm.FaxNo = dr["FaxNo"].ToString();
                        vm.Email = dr["Email"].ToString();
                        vm.ContactPerson = dr["ContactPerson"].ToString();
                        vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                        vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                        vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                        vm.TINNo = dr["TINNo"].ToString();
                        vm.VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.StartDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["StartDateTime"].ToString());
                        vm.FYearStart = OrdinaryVATDesktop.DateTimeToDate(dr["FYearStart"].ToString());
                        vm.FYearEnd = OrdinaryVATDesktop.DateTimeToDate(dr["FYearEnd"].ToString());
                        vm.Tom = dr["Tom"].ToString();
                        vm.Jary = dr["Jary"].ToString();
                        vm.Miki = dr["Miki"].ToString();
                        vm.Mouse = dr["Mouse"].ToString();
                        vm.IsVDSWithHolder = dr["IsVDSWithHolder"].ToString();
                        vm.BusinessNature = dr["BusinessNature"].ToString();
                        vm.AccountingNature = dr["AccountingNature"].ToString();
                        vm.BIN = dr["BIN"].ToString();
                        vm.Section = dr["Section"].ToString();
                        vm.CompanyType = dr["CompanyType"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {

                    }
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

                FileLogger.Log("CompanyprofileDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());


            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CompanyprofileDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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
SELECT top 1
 CompanyID
,CompanyName
,CompanyLegalName
,Address1
,Address2
,Address3
,City
,ZipCode
,TelephoneNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TINNo
,VatRegistrationNo
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,StartDateTime
,FYearStart
,FYearEnd
,Info4
,Info5
,Tom
,Jary
,Miki
,Mouse
,isnull(IsVDSWithHolder,'N') IsVDSWithHolder
,BusinessNature
,AccountingNature
,BIN
,isnull(AppVersion,'-')AppVersion
,isnull(Section,'-')Section
,isnull(CompanyType,'-')CompanyType

FROM CompanyProfiles  
WHERE  1=1 AND ActiveStatus = 'Y'

";
                if (Id != null)
                {
                    sqlText += @" and CompanyID=@CompanyID";
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

                if (Id != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@CompanyID", Id);
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
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("CompanyprofileDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CompanyprofileDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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
        #endregion

        public List<CompanyCategoryVM> GetCompanyTypes(SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, string CATEGORY_ID = "")
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();
            try
            {
                commonDal.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction);


                // Exempted80
                string sqlText = "select * from CompanyCategory where 1=1 ";

                if (!string.IsNullOrWhiteSpace(CATEGORY_ID))
                {
                    sqlText += " and  CATEGORY_ID=@CATEGORY_ID";
                }

                SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);

                if (!string.IsNullOrWhiteSpace(CATEGORY_ID))
                {
                    cmd.Parameters.AddWithValue("@CATEGORY_ID", CATEGORY_ID);
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dtTable = new DataTable();
                dataAdapter.Fill(dtTable);

                commonDal.TransactionCommit(ref currentTransaction, ref transaction);

                List<CompanyCategoryVM> list = dtTable.ToList<CompanyCategoryVM>();

                return list;

            }
            catch (Exception e)
            {
                commonDal.TransactionRollBack(ref currentTransaction, ref transaction);

                throw e;
            }
            finally
            {
                commonDal.CloseConnection(ref currentConnection, ref connection);
            }
        }
    }
}
