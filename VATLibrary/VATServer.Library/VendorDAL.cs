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
    public class VendorDAL : IVendor
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Vendor");

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

                sqlText = @"  SELECT
  row_number() OVER (ORDER BY v.[VendorID]) SL 
      
      ,v.[VendorCode] Code
      ,v.[VendorName]
    ,vg.GroupType VendorType
      ,vg.VendorGroupName VendorGroup
      ,v.[Address1]
      ,v.[Address2]
      ,v.[Address3]
      ,v.[City]
       , v.Country
      ,v.[TelephoneNo]
      ,v.[FaxNo]
      ,v.[Email]
      ,cast(v.[StartDateTime] as varchar(100)) StartDateTime
      ,v.[ContactPerson]
      ,v.[ContactPersonDesignation]
      ,v.[ContactPersonTelephone]
      ,v.[ContactPersonEmail]
      ,v.[VATRegistrationNo]
      ,v.[TINNo] TIN
      ,v.[Comments]
      ,v.[ActiveStatus]


  FROM Vendors v left outer join VendorGroups vg
  on v.VendorGroupID = vg.VendorGroupID
  where v.VendorCode in ";


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

        public string[] InsertToVendorAddress(VendorAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

            #endregion

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

                #region Customer  new id generation
                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  VendorsAddress";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Customers Address",
                                                    "Unable to create new Vendor Address");
                }


                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";
                sqlText += "insert into VendorsAddress";
                sqlText += "(";
                sqlText += "Id,";
                sqlText += "VendorID,";
                sqlText += "VendorAddress";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@Id,";
                sqlText += "@VendorID,";
                sqlText += "@VendorAddress";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("Id", nextId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("VendorID", vm.VendorID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("VendorAddress", vm.VendorAddress);
                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested vendor  Address successfully Added";
                    retResults[2] = "" + nextId;
                    retResults[3] = "" + nextId;
                }
                #region Commit
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested vendor  Address successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + nextId;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add vendor";
                            retResults[2] = "";
                            retResults[3] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add vendor ";
                        retResults[2] = "";
                        retResults[3] = "";
                    }
                }
                #endregion Commit




                #endregion Commit


                #endregion Inser new vendor

            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("VendorDAL", "InsertToVendorAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("VendorDAL", "InsertToVendorAddress", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateToVendorAddress(VendorAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

            #endregion

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Id.ToString()))
                {
                    throw new ArgumentNullException("UpdateToVendors",
                                                    "Invalid Vendor  Address");
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


                #region Customer  existence checking





                #endregion Customer Group existence checking

                #region Update new customer group
                sqlText = "";
                sqlText = "update VendorsAddress set";
                sqlText += " VendorAddress='" + vm.VendorAddress + "'";
                sqlText += " where id='" + vm.Id + "'";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
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
                            retResults[1] = "Requested vendors  Address successfully Update";
                            retResults[2] = vm.Id.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update vendor ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update vendor group";
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
                //////throw sqlex;

                FileLogger.Log("VendorDAL", "UpdateToVendorAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("VendorDAL", "UpdateToVendorAddress", ex.ToString() + "\n" + sqlText);

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

        public string[] DeleteVendorAddress(string VendorID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(Id))
                {
                    throw new ArgumentNullException("DeleteVendor",
                                "Could not find requested Address.");
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


                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText = "delete VendorsAddress where Id='" + Id + "'";
                }
                if (!string.IsNullOrEmpty(VendorID))
                {
                    sqlText = "delete VendorsAddress where VendorID='" + VendorID + "'";
                }
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Vendor  Address successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Vendors  Address successfully deleted";
                        retResults[2] = "";

                    }
                }

                #endregion Commit

            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("VendorDAL", "DeleteVendorAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("VendorDAL", "DeleteVendorAddress", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchVendorAddress(string VendorID, string Id, string address, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Vendor");

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

                //////                sqlText = @"
                //////SELECT 
                //////isnull(Id,'0')Id
                //////,isnull(CustomerID,'0')CustomerID
                //////,isnull(CustomerAddress,'-')CustomerAddress
                //////FROM CustomersAddress 
                //////WHERE 1=1
                //////
                //////";

                sqlText = @"
SELECT 
isnull(vad.Id,'0')Id
,isnull(vad.VendorID,'0')VendorID
,d.VendorCode
,d.VendorName
,isnull(vad.VendorAddress,'-')VendorAddress
,vg.VendorGroupName
FROM VendorsAddress vad
left outer join Vendors d on vad.VendorID=d.VendorID
left outer join VendorGroups vg on d.VendorGroupID=vg.VendorGroupID 
WHERE 1=1 and  d.ActiveStatus='Y'

";

                if (!string.IsNullOrEmpty(VendorID))
                {
                    sqlText += @"  and vad.VendorID=@VendorID";
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @"  and vad.Id=@Id";
                }

                if (!string.IsNullOrEmpty(address))
                {
                    sqlText += @"  and vad.VendorAddress like '%" + address + "%'";
                }
                sqlText += @"  order by  vad.VendorAddress";

                SqlCommand objCommVendorInformation = new SqlCommand();
                objCommVendorInformation.Connection = currConn;
                objCommVendorInformation.CommandText = sqlText;
                objCommVendorInformation.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(VendorID))
                {
                    if (!objCommVendorInformation.Parameters.Contains("@VendorID"))
                    { objCommVendorInformation.Parameters.AddWithValue("@VendorID", VendorID); }
                    else { objCommVendorInformation.Parameters["@VendorID"].Value = VendorID; }
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    if (!objCommVendorInformation.Parameters.Contains("@Id"))
                    { objCommVendorInformation.Parameters.AddWithValue("@Id", Id); }
                    else { objCommVendorInformation.Parameters["@Id"].Value = Id; }
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVendorInformation);
                dataAdapter.Fill(dataTable);



                #endregion
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("VendorDAL", "SearchVendorAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("VendorDAL", "SearchVendorAddress", ex.ToString() + "\n" + sqlText);

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

        #region web methods
        public string[] InsertToVendorNewSQL(VendorVM vm, bool autoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables


            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            string vendorCode = vm.VendorCode;

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

                if (string.IsNullOrEmpty(vm.VendorName))
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Please enter vendor name.");
                }

                string VATRegistrationNo = vm.VATRegistrationNo;

                if (string.IsNullOrWhiteSpace(VATRegistrationNo))
                {
                    VATRegistrationNo = "-";
                }
                string VATRegNo = VATRegistrationNo.Replace("-", "");


                ////if (!OrdinaryVATDesktop.IsNumeric(VATRegNo))
                ////{
                ////    throw new ArgumentNullException("InsertToVendorInformation",
                ////                                   "Please enter VATRegistrationNo only number.");
                    
                ////}

                ////if (VATRegNo.Length >= 14)
                ////{
                ////    throw new ArgumentNullException("InsertToVendorInformation",
                ////                                  "Please enter Valid VATRegistrationNo No/Not more than 13 digit.");

                ////}

                if (string.IsNullOrEmpty(vm.NIDNo))
                {
                    vm.NIDNo = "-";
                }

                if (vm.NIDNo != "-")
                {
                    if (!OrdinaryVATDesktop.IsNumber(vm.NIDNo))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                                 "Please Enter National ID  only number");
                    }

                    if (vm.NIDNo.Length >= 18)
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                               "Please Enter Valid National ID No./Not more than 17 digit.");

                    }

                }


                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Vendor") == "Y" ? true : false);
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

                #endregion
                 
                #region Insert Vendor Information


                sqlText = "select isnull(max(cast(VendorID as int)),0)+1 FROM  Vendors";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Vendor information Id";
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Unable to create new Vendor information Id");
                }
                #region Code
                if (Auto == false)
                {
                    if (autoSave)
                    {
                        vm.VendorGroupID = "1";
                        if (string.IsNullOrWhiteSpace(vm.VendorCode))
                        {
                            vendorCode = nextId.ToString();
                        }
                    }
                    else if (string.IsNullOrEmpty(vendorCode))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(VendorID) from Vendors where  VendorCode=@vendorCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@vendorCode", vendorCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToVendorInformation", "Same customer  Code('" + vendorCode + "') already exist");
                        }
                    }
                }
                else
                {
                    vendorCode = nextId.ToString();
                }
                #endregion Code


                sqlText = "";
                sqlText += "insert into Vendors";
                sqlText += "(";
                sqlText += "VendorID,";
                sqlText += "VendorName,";
                sqlText += "VendorGroupID,";
                sqlText += "Address1,";
                sqlText += "Address2,";
                sqlText += "Address3,";
                sqlText += "City,";
                sqlText += "TelephoneNo,";
                sqlText += "FaxNo,";
                sqlText += "Email,";
                sqlText += "StartDateTime,";
                sqlText += "ContactPerson,";
                sqlText += "ContactPersonDesignation,";
                sqlText += "ContactPersonTelephone,";
                sqlText += "ContactPersonEmail,";
                sqlText += "VATRegistrationNo,";
                sqlText += "NIDNo,";
                sqlText += "TINNo,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "Country,";
                sqlText += "VendorCode,";
                sqlText += "BusinessType,";
                sqlText += "BusinessCode,";
                sqlText += "IsVDSWithHolder,";
                sqlText += "IsRegister,";
                sqlText += "IsTurnover,";
                sqlText += "VDSPercent,";
                sqlText += "BranchId,";
                sqlText += "ShortName,";
                sqlText += "IsArchive";


                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@VendorName,";
                sqlText += "@VendorGroupID,";
                sqlText += "@Address1,";
                sqlText += "@Address2,";
                sqlText += "@Address3,";
                sqlText += "@City,";
                sqlText += "@TelephoneNo,";
                sqlText += "@FaxNo,";
                sqlText += "@Email,";
                sqlText += "@StartDateTime,";
                sqlText += "@ContactPerson,";
                sqlText += "@ContactPersonDesignation,";
                sqlText += "@ContactPersonTelephone,";
                sqlText += "@ContactPersonEmail,";
                sqlText += "@VATRegistrationNo,";
                sqlText += "@NIDNo,";
                sqlText += "@TINNo,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@Country,";
                sqlText += "@vendorCode,";
                sqlText += "@BusinessType,";
                sqlText += "@BusinessCode,";
                sqlText += "@IsVDSWithHolder,";
                sqlText += "@IsRegister,";
                sqlText += "@IsTurnover,";
                sqlText += "@VDSPercent,";
                sqlText += "@BranchId,";
                sqlText += "@ShortName,";
                sqlText += "@IsArchive";



                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@VendorName", vm.VendorName);
                cmdInsert.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@StartDateTime", OrdinaryVATDesktop.DateToDate(vm.StartDateTime) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@vendorCode", vendorCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsVDSWithHolder", vm.IsVDSWithHolder ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsRegister", vm.IsRegister ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsTurnover", vm.IsTurnover ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValue("@ShortName", vm.ShortName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Insert Vendor Information
                if (transResult > 0)
                {
                    VendorAddressVM cvm = new VendorAddressVM();
                    cvm.VendorID = nextId.ToString();
                    cvm.VendorAddress = vm.Address1;
                    retResults = InsertToVendorAddress(cvm, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", "SQL:" + sqlText);//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                    }

                }

                #region Commit


                if (transaction != null && Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested Vendor Information successfully added";
                retResults[2] = "" + nextId;
                retResults[3] = "" + vendorCode;


            }
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                ////transaction.Rollback();
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
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

        public string[] UpdateVendorNewSQL(VendorVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.VendorID;

            string vendorCode = vm.VendorCode;

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
                if (vm.VendorID.Trim() == "0")
                {
                    retResults[0] = "Fail";
                    retResults[1] = "This vendor information unable to update!";
                    retResults[2] = vm.VendorID;

                    return retResults;
                }
                if (string.IsNullOrEmpty(vm.VendorName))
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Please enter vendor name.");
                }


                string VATRegistrationNo = vm.VATRegistrationNo;
                string VATRegNo = VATRegistrationNo.Replace("-", "");


                if (!OrdinaryVATDesktop.IsNumeric(VATRegNo))
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                   "Please enter VATRegistrationNo only number.");

                }

                if (VATRegNo.Length >= 14)
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                  "Please enter Valid VATRegistrationNo No/Not more than 13 digit.");

                }

                if (string.IsNullOrEmpty(vm.NIDNo))
                {
                    vm.NIDNo = "-";
                }

                if (vm.NIDNo != "-")
                {
                    if (!OrdinaryVATDesktop.IsNumber(vm.NIDNo))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                                 "Please Enter National ID  only number");
                    }

                    if (vm.NIDNo.Length >= 18)
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                               "Please Enter Valid National ID No./Not more than 17 digit.");

                    }

                }



                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Vendor") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);

                transaction = currConn.BeginTransaction("InsertToVendorInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.VendorID))
                {


                    sqlText = "select count(VendorID) from Vendors where  VendorID=@VendorID ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@VendorID", vm.VendorID);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                                        "Could not find requested Vendor information ");
                    }

                }


                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(VendorID) from Vendors where  VendorCode=@VendorCode and VendorID <>@VendorID ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@VendorCode", vm.VendorCode);
                        cmdCodeExist.Parameters.AddWithValue("@VendorID", vm.VendorID);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToVendorInformation", "Same customer  Code('" + vendorCode + "') already exist");
                        }
                    }
                }
                else
                {
                    vendorCode = vm.VendorID;
                }
                #endregion Code


                #region Update Vendor Information



                sqlText = "";
                sqlText += "UPDATE  Vendors SET ";
                sqlText += " VendorName                 =@VendorName,";
                sqlText += " VendorGroupID              =@VendorGroupID,";
                sqlText += " Address1                   =@Address1,";
                sqlText += " Address2                   =@Address2,";
                sqlText += " Address3                   =@Address3,";
                sqlText += " City                       =@City,";
                sqlText += " TelephoneNo                =@TelephoneNo,";
                sqlText += " FaxNo                      =@FaxNo,";
                sqlText += " Email                      =@Email,";
                sqlText += " StartDateTime              =@StartDateTime,";
                sqlText += " ContactPerson              =@ContactPerson,";
                sqlText += " ContactPersonDesignation   =@ContactPersonDesignation,";
                sqlText += " ContactPersonTelephone     =@ContactPersonTelephone,";
                sqlText += " ContactPersonEmail         =@ContactPersonEmail,";
                sqlText += " VATRegistrationNo          =@VATRegistrationNo,";
                sqlText += " NIDNo                      =@NIDNo,";
                sqlText += " TINNo                      =@TINNo,";
                sqlText += " Comments                   =@Comments,";
                sqlText += " ActiveStatus               =@ActiveStatus,";
                //sqlText += " Option1                    =@Option1,";
                //sqlText += " Option2                    =@Option2,";
                //sqlText += " Option3                    =@Option3,";
                //sqlText += " Option4                    =@Option4,";
                sqlText += " LastModifiedBy             =@LastModifiedBy,";
                sqlText += " LastModifiedOn             =@LastModifiedOn,";
                sqlText += " Country                    =@Country,";
                sqlText += " ShortName                  =@ShortName,";
                sqlText += " VDSPercent                 =@VDSPercent,";
                sqlText += " BusinessType               =@BusinessType,";
                sqlText += " BusinessCode               =@BusinessCode,";
                sqlText += " IsVDSWithHolder            =@IsVDSWithHolder,";
                sqlText += " IsRegister                 =@IsRegister,";
                sqlText += " IsTurnover                 =@IsTurnover,";
                sqlText += " VendorCode                 =@vendorCode";
                sqlText += " where VendorID             =@VendorID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@VendorName", vm.VendorName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@StartDateTime", vm.StartDateTime);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ShortName", vm.ShortName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdInsert.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@vendorCode", vm.VendorCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsVDSWithHolder", vm.IsVDSWithHolder ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsTurnover", vm.IsTurnover ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsRegister", vm.IsRegister ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VendorID", vm.VendorID ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Vendor Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested Vendor Information successfully Updated";
                retResults[2] = "" + vm.VendorID;
                retResults[3] = "" + vm.VendorCode;


            }
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                ////transaction.Rollback();
                if (transaction != null) { transaction.Rollback(); }

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

        public List<VendorVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorVM> VMs = new List<VendorVM>();
            VendorVM vm;
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
'B' Sl, VendorID
, VendorName
FROM Vendors
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' VendorID
, 'ALL Product' VendorName  
FROM Vendors
)
AS a
order by Sl,VendorName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorVM();
                    vm.VendorID = dr["VendorID"].ToString(); ;
                    vm.VendorName = dr["VendorName"].ToString();
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

        public List<VendorVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorVM> VMs = new List<VendorVM>();
            VendorVM vm;
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
af.VendorID
,af.VendorName
FROM Vendors af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorVM();
                    vm.VendorID = dr["VendorID"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
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

        public List<VendorVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VendorVM> VMs = new List<VendorVM>();
            VendorVM vm;
            #endregion
            try
            {

                #region sql statement
                #region SqlExecution
                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new VendorVM();
                        vm.VendorID = dr["VendorID"].ToString();
                        vm.VendorCode = dr["VendorCode"].ToString();
                        vm.VendorName = dr["VendorName"].ToString();
                        vm.VendorGroupID = dr["VendorGroupID"].ToString();
                        vm.Address1 = dr["Address1"].ToString();
                        vm.Address2 = dr["Address2"].ToString();
                        vm.Address3 = dr["Address3"].ToString();
                        vm.City = dr["City"].ToString();
                        vm.TelephoneNo = dr["TelephoneNo"].ToString();
                        vm.FaxNo = dr["FaxNo"].ToString();
                        vm.Email = dr["Email"].ToString();
                        vm.StartDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["StartDateTime"].ToString());
                        vm.ContactPerson = dr["ContactPerson"].ToString();
                        vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                        vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                        vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                        vm.VATRegistrationNo = dr["VATRegistrationNo"].ToString();
                        vm.NIDNo = dr["NIDNo"].ToString();
                        vm.TINNo = dr["TINNo"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.Country = dr["Country"].ToString();
                        vm.ShortName = dr["ShortName"].ToString();
                        vm.Info2 = dr["Info2"].ToString();
                        vm.Info3 = dr["Info3"].ToString();
                        vm.Info4 = dr["Info4"].ToString();
                        vm.Info5 = dr["Info5"].ToString();
                        vm.VDSPercent = Convert.ToDecimal(dr["VDSPercent"]);
                        vm.BusinessType = dr["BusinessType"].ToString();
                        vm.BusinessCode = dr["BusinessCode"].ToString();
                        vm.IsVDSWithHolder = dr["IsVDSWithHolder"].ToString();
                        vm.IsRegister = dr["IsRegister"].ToString();
                        vm.IsTurnover = dr["IsTurnover"].ToString();

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

                sqlText = @"SELECT ";

                if (count == "All")
                {
                    sqlText = @"SELECT ";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"


v.VendorCode
,v.VendorName
,vg.VendorGroupName
,v.Address1
,v.City
,v.TelephoneNo
,v.FaxNo
,v.Email
,v.StartDateTime
,v.ContactPerson
,v.ContactPersonDesignation
,v.ContactPersonTelephone
,v.ContactPersonEmail
,v.VATRegistrationNo
,v.NIDNo
,v.TINNo
,v.Comments
,v.ActiveStatus
,v.CreatedBy
,v.CreatedOn
,v.LastModifiedBy
,v.LastModifiedOn
,v.Country
,v.Address2
,v.Address3
,v.Info2
,v.Info3
,v.Info4
,v.Info5
,ISNULL(v.VDSPercent,0) VDSPercent 
,ISNULL(v.ShortName,'-') ShortName 
,v.BusinessType
,v.BusinessCode
,isnull(v.IsRegister,'N') IsRegister
,isnull(v.IsTurnover,'N') IsTurnover
,ISNULL(v.IsVDSWithHolder,'N') IsVDSWithHolder
,v.VendorID
,v.VendorGroupID
,isnull(v.BranchId, 0) BranchId
FROM Vendors v left outer join VendorGroups vg on v.VendorGroupID=vg.VendorGroupID
WHERE  1=1 AND isnull(v.IsArchive,0) = 0   

";
                #region sqlTextCount
                sqlTextCount += @" select count(v.VendorID)RecordCount
FROM Vendors v left outer join VendorGroups vg on v.VendorGroupID=vg.VendorGroupID
WHERE  1=1 AND isnull(v.IsArchive,0) = 0
";
                #endregion

                if (Id > 0)
                {
                    sqlTextParameter += @" and v.VendorID=@VendorID";
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

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
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
                    da.SelectCommand.Parameters.AddWithValue("@VendorID", Id);
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

        public string[] Delete(VendorVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVendor"; //Method Name
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
                        sqlText = "update Vendors set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where VendorID=@VendorID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@VendorID", ids[i]);
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
                        throw new ArgumentNullException("Vendor Delete", vm.VendorID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Vendor Information Delete", "Could not found any item.");
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
