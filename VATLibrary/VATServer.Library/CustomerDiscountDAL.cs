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
    public class CustomerDiscountDAL : ICustomerDiscount
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #region
        public DataTable SearchCustomerDiscount(string CustomerID, string Id, string address, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Customer");

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

 cd.Id
,cd.Description
,cd.MinValue
,cd.MaxValue
,cd.Rate
,cd.CustomerID
,c.CustomerName
,cd.Comments
,cd.CreatedBy
,cd.CreatedOn
,cd.LastModifiedBy
,cd.LastModifiedOn
,ISNULL(cd.IsArchive,0) IsArchive
FROM CustomerDiscounts cd  
left outer join Customers c on cd.CustomerID=c.CustomerID
WHERE  1=1  and isnull(cd.IsArchive,0) = 0 


";
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    sqlText += @"  and cd.CustomerID=@CustomerID";
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @"  and Id=@Id";
                }

             

                SqlCommand objCommCustomerInformation = new SqlCommand();
                objCommCustomerInformation.Connection = currConn;
                objCommCustomerInformation.CommandText = sqlText;
                objCommCustomerInformation.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(CustomerID))
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@CustomerID"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@CustomerID", CustomerID); }
                    else { objCommCustomerInformation.Parameters["@CustomerID"].Value = CustomerID; }
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@Id"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@Id", Id); }
                    else { objCommCustomerInformation.Parameters["@Id"].Value = Id; }
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCustomerInformation);
                dataAdapter.Fill(dataTable);



                #endregion
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CustomerDiscountDAL", "SearchCustomerDiscount", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDiscountDAL", "SearchCustomerDiscount", ex.ToString() + "\n" + sqlText);

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
SELECT 

 cd.Id
,cd.Description
,cd.MinValue
,cd.MaxValue
,cd.Rate
,cd.CustomerID
,c.CustomerName
,cd.Comments
,cd.CreatedBy
,cd.CreatedOn
,cd.LastModifiedBy
,cd.LastModifiedOn
,ISNULL(cd.IsArchive,0) IsArchive
FROM CustomerDiscounts cd  
left outer join Customers c on cd.CustomerID=c.CustomerID
WHERE  1=1  and isnull(cd.IsArchive,0) = 0 

";
                if (Id != null)
                {
                    sqlText += @" and cd.Id=@Id";
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
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("CustomerDiscountDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDiscountDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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
        public string[] InsertToCustomerDiscountNew(CustomerDiscountVM vm, bool BranchProfileAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            string Code = vm.Code;
            int nextId = 0;

            #endregion

            #region Try
            try
            {
                #region Validation

                //if (string.IsNullOrEmpty(vm.BranchName))
                //{
                //    throw new ArgumentNullException("InsertToBranchProfile",
                //                                    "Please enter BranchProfile group name.");
                //}


                #endregion Validation
                //#region settingsValue
                //CommonDAL commonDal = new CommonDAL();
                //bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "TDSs") == "Y" ? true : false);
                //#endregion settingsValue

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
                #region CustomerDiscount  new id generation
                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  CustomerDiscounts";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomerDiscount",
                                                    "Unable to create new TDSs No");
                }


                #region CustomerID Exist Checck
                sqlText = "select count(Id) from CustomerDiscounts where  CustomerID=@CustomerID and MaxValue=@MaxValue and MinValue=@MinValue and Rate=@Rate";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@CustomerID", vm.CustomerID);
                cmdExist.Parameters.AddWithValue("@MaxValue", vm.MaxValue);
                cmdExist.Parameters.AddWithValue("@MinValue", vm.MinValue);
                cmdExist.Parameters.AddWithValue("@Rate", vm.Rate);
                countId = (int)cmdExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToCustomerDiscounts", "Same CustomerDiscounts  CustomerID('" + vm.CustomerID + "') already exist");
                }
                #endregion 
               
               



                #endregion TDSs  new id generation

                #region Inser new CustomerDiscount
                sqlText = "";

                sqlText += @" 
INSERT INTO CustomerDiscounts(


Description
,MinValue
,MaxValue
,Rate
,CustomerID
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
, IsArchive

) VALUES (

@Description
,@MinValue
,@MaxValue
,@Rate
,@CustomerID
,@Comments
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@IsArchive
) SELECT SCOPE_IDENTITY()
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
               
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MinValue", vm.MinValue );
                cmdInsert.Parameters.AddWithValue("@MaxValue", vm.MaxValue);
                cmdInsert.Parameters.AddWithValue("@Rate", vm.Rate);
                cmdInsert.Parameters.AddWithValue("@CustomerID", vm.CustomerID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive",false);


                transResult =Convert.ToInt32(cmdInsert.ExecuteScalar());
                nextId = transResult;

                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested CustomerDiscount  Information successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + Code;
                        }
                    }
                }

                #endregion Commit


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

                FileLogger.Log("CustomerDiscountDAL", "InsertToCustomerDiscountNew", ex.ToString() + "\n" + sqlText);

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
        public string[] UpdateToCustomerDiscountNew(CustomerDiscountVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.Id.ToString();

            string Code = vm.Code;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            #endregion

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Id.ToString()))
                {
                    throw new ArgumentNullException("UpdateTo Customer Discounts",
                                                    "Invalid CustomerDiscounts ID");
                }
                //if (string.IsNullOrEmpty(vm.BranchName))
                //{
                //    throw new ArgumentNullException("UpdateToBranchProfiles",
                //                                    "Invalid BranchProfile Name.");
                //}
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
                transaction = currConn.BeginTransaction("UpdateToCustomerDiscount");

                #endregion open connection and transaction

                #region CustomerDiscounts  existence checking

                sqlText = "select count(Id) from CustomerDiscounts where  Id=@Id";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@Id", vm.Id);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToCustomerDiscount",
                                "Could not find requested CustomerDiscount  id.");
                }

                #endregion BranchProfile Group existence checking

                #region CustomerID Exist Checck
                sqlText = "select count(Id) from CustomerDiscounts where  CustomerID=@CustomerID and MaxValue=@MaxValue and MinValue=@MinValue and Rate=@Rate and Id!=@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@CustomerID", vm.CustomerID);
                cmdExist.Parameters.AddWithValue("@MaxValue", vm.MaxValue);
                cmdExist.Parameters.AddWithValue("@MinValue", vm.MinValue);
                cmdExist.Parameters.AddWithValue("@Rate", vm.Rate);
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                countId = (int)cmdExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToCustomerDiscounts", "Same CustomerDiscounts  CustomerID('" + vm.CustomerID + "') already exist");
                }
                #endregion
                




                #region Update new CustomerDiscounts
                sqlText = "";
                sqlText = "update CustomerDiscounts set";
                sqlText += "  Description              =@Description";
                sqlText += "  ,MinValue                 =@MinValue";
                sqlText += "  ,MaxValue                 =@MaxValue";
                sqlText += "  ,Rate                     =@Rate";
                sqlText += "  ,CustomerID               =@CustomerID";
                sqlText += "  ,Comments                 =@Comments";
                sqlText += "  ,LastModifiedBy           =@LastModifiedBy";
                sqlText += "  ,LastModifiedOn           =@LastModifiedOn";

                sqlText += " WHERE Id           =@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MinValue", vm.MinValue );
                cmdUpdate.Parameters.AddWithValue("@MaxValue", vm.MaxValue );
                cmdUpdate.Parameters.AddWithValue("@Rate", vm.Rate);
                cmdUpdate.Parameters.AddWithValue("@CustomerID", vm.CustomerID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested CustomerDiscount  Information successfully Update";
                        retResults[2] = vm.Id.ToString();
                        retResults[3] = Code;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update CustomerDiscount ";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update CustomerDiscount group";
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
                retResults[2] = vm.Id.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("CustomerDiscountDAL", "UpdateToCustomerDiscountNew", ex.ToString() + "\n" + sqlText);

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
        public string[] Delete(CustomerDiscountVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCustomerDiscount"; //Method Name
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
                        sqlText = "update CustomerDiscounts set";
                       
                        sqlText += " LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                       
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
                        throw new ArgumentNullException("CustomerDiscount Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("CustomerDiscount Information Delete", "Could not found any item.");
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

                FileLogger.Log("CustomerDiscountDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        public string[] DeleteCustomerDiscount(string CustomerID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException("DeleteCustomer",
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
                    sqlText = "delete CustomerDiscounts where Id='" + Id + "'";
                }
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    sqlText = "delete CustomerDiscounts where CustomerID='" + CustomerID + "'";
                }
               
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested CustomerDiscount  Address successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested CustomerDiscount  Address successfully deleted";
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

                FileLogger.Log("CustomerDiscountDAL", "DeleteCustomerDiscount", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDiscountDAL", "DeleteCustomerDiscount", ex.ToString() + "\n" + sqlText);

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

//        public DataTable CurrentTDSAmount(string PurchaseInvoiceNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            DataTable ds = new DataTable();
//            #endregion
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
//                #region sql statement
//                #region SqlText

//                sqlText = @"
//	--declare @PurchaseInvoiceNo as varchar(100)
//	--set @PurchaseInvoiceNo='PUR-00000001/0919'
//
// select t.*, h.VendorID from PurchaseTDSs t
// left outer join PurchaseInvoiceHeaders h on t.PurchaseInvoiceNo=h.PurchaseInvoiceNo
// where t.PurchaseInvoiceNo=@PurchaseInvoiceNo
//";


//                #endregion SqlText
//                #region SqlExecution
//                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
//                da.SelectCommand.Transaction = transaction;
//                da.SelectCommand.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo);
//                da.Fill(ds);
//                #endregion SqlExecution

//                if (Vtransaction == null && transaction != null)
//                {
//                    transaction.Commit();
//                }
//                #endregion
//            }
//            #region catch
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
//            return ds;
//        }

//        public DataSet TDSAmount(string VendorID, string ReceiveDate, string TDSCode, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            DataSet ds = new DataSet();
//            #endregion
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
//                #region sql statement
//                #region SqlText

//                sqlText = @"
//
// 
//	--declare @VendorID as varchar(100)
//	--declare @TDSCode as varchar(100)
//	--declare @ReceiveDate as date
//	--set @VendorID='10'
//	--set @ReceiveDate='01-Oct-2019'
//	--set @TDSCode='52A'
//
//	declare @ReceiveDateF as date
//	declare @ReceiveDateT as date
//
//	select  @ReceiveDateF=  min(PeriodStart),@ReceiveDateT=max(PeriodEnd)  from FiscalYear
//	where CurrentYear in(
//	select CurrentYear from FiscalYear
//	where @ReceiveDate between PeriodStart and PeriodEnd )
// 
// 
// select distinct h.VendorID,d.TDSCode,  sum( d.SubTotal)PreviousSubTotal  from PurchaseInvoiceDetails d
// left outer join PurchaseInvoiceHeaders h on d.PurchaseInvoiceNo=h.PurchaseInvoiceNo
// where h.VendorID=@VendorID
// and d.TDSCode=@TDSCode
//	and d.ReceiveDate >DATEADD(d,-1,  @ReceiveDateF) and d.ReceiveDate<DATEADD(d,1,  @ReceiveDateT)
// and h.Post='Y'
// group by  h.VendorID,d.TDSCode
// 
//  select distinct h.VendorID,t.TDSCode,sum( t.TDSAmount)PreviousTDSAmount  from PurchaseTDSs t
// left outer join PurchaseInvoiceHeaders h on t.PurchaseInvoiceNo=h.PurchaseInvoiceNo
// where h.VendorID=@VendorID
// and t.TDSCode=@TDSCode
//	and h.ReceiveDate >DATEADD(d,-1,  @ReceiveDateF) and h.ReceiveDate<DATEADD(d,1,  @ReceiveDateT)
// and t.Post='Y'
// group by  h.VendorID,t.TDSCode
//
//     select Code,Description,MinValue,MaxValue,Rate from TDSs
//    where Code in(@TDSCode)
//";


//                #endregion SqlText
//                #region SqlExecution
//                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
//                da.SelectCommand.Transaction = transaction;
//                da.SelectCommand.Parameters.AddWithValue("@VendorID", VendorID);
//                da.SelectCommand.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
//                da.SelectCommand.Parameters.AddWithValue("@TDSCode", TDSCode);
//                da.Fill(ds);
//                #endregion SqlExecution

//                if (Vtransaction == null && transaction != null)
//                {
//                    transaction.Commit();
//                }
//                #endregion
//            }
//            #region catch
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
//            return ds;
//        }

        //public string[] UpdatePurchaseTDSs(string Id, decimal TDSAmount, SysDBInfoVMTemp connVM = null)
        //{
        //    string[] retResults = new string[4];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";
        //    retResults[2] =  Id.ToString();


        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    string sqlText = "";
        //    int nextId = 0;

        //    #region Try
        //    try
        //    {
        //        #region Validation
                

        //        #endregion Validation


        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection(connVM);
        //        if (currConn.State != ConnectionState.Open)
        //        {
        //            currConn.Open();
        //        }
        //        transaction = currConn.BeginTransaction("UpdateToTDSs");

        //        #endregion open connection and transaction


        //        #region Update new TDSs
        //        sqlText = "";
        //        sqlText = "update CustomerDiscount set";
        //        sqlText += "  TDSAmount     =@TDSAmount";
        //        sqlText += " WHERE Id           =@Id";

        //        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
        //        cmdUpdate.Transaction = transaction;
        //        cmdUpdate.Parameters.AddWithValue("@Id", Id);
        //        cmdUpdate.Parameters.AddWithValue("@TDSAmount", TDSAmount);

        //        transResult = (int)cmdUpdate.ExecuteNonQuery();


        //        #region Commit


        //        if (transaction != null)
        //        {
        //            if (transResult > 0)
        //            {
        //                transaction.Commit();
        //                retResults[0] = "Success";
        //                retResults[1] = "Requested TDS  Information successfully Update";
        //                retResults[2] = Id.ToString();
        //                retResults[3] = "";

        //            }
        //            else
        //            {
        //                transaction.Rollback();
        //                retResults[0] = "Fail";
        //                retResults[1] = "Unexpected error to update TDS ";
        //            }

        //        }
        //        else
        //        {
        //            retResults[0] = "Fail";
        //            retResults[1] = "Unexpected error to update TDS group";
        //        }

        //        #endregion Commit


        //        #endregion

        //    }
        //    #endregion
        //    #region Catch
        //    catch (Exception ex)
        //    {

        //        retResults[0] = "Fail";//Success or Fail
        //        retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
        //        retResults[2] = nextId.ToString(); //catch ex

        //        transaction.Rollback();

        //        FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
        //    }
        //    #endregion
        //    #region finally
        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {

        //                currConn.Close();

        //            }
        //        }

        //    }
        //    #endregion


        //    return retResults;
        //}
        
        #endregion
        #region Web Method
        public List<CustomerDiscountVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerDiscountVM> VMs = new List<CustomerDiscountVM>();
            CustomerDiscountVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new CustomerDiscountVM();


                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.MinValue =Convert.ToDecimal( dr["MinValue"].ToString());
                    vm.MaxValue =Convert.ToDecimal( dr["MaxValue"].ToString());
                    vm.Rate = Convert.ToDecimal(dr["Rate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerCode = dr["CustomerCode"].ToString();
                    vm.Comments = dr["Comments"].ToString();
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("CustomerDiscountDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDiscountDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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
      
        #endregion
    }
}
