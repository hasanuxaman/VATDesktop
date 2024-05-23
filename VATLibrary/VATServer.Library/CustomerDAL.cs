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
using Excel;
using VATServer.Interface;

namespace VATServer.Library
{
    public class CustomerDAL : ICustomer
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;


        #region Navigation

        public NavigationVM Customer_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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
                    currConn = _dbsqlConnection.GetConnection();
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

                #region SQL Statement

                #region SQL Text

                sqlText = "";
                sqlText = @"
------declare @CustomerCode as varchar(100) = '1'
";
                if (vm.ButtonName == "Current")
                {
                    #region Current Item

                    sqlText = sqlText + @"
--------------------------------------------------Current--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 CustomerId as Id, CustomerCode as Code from Customers c
where 1=1 
and c.IsArchive = 0
and c.CustomerCode=@CustomerCode

";
                    #endregion
                }
                else if (string.IsNullOrWhiteSpace(vm.Code) || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 CustomerId as Id, CustomerCode as Code from Customers c
where 1=1 
and c.IsArchive = 0
and ISNULL(CustomerId, '0') <> '0'
order by CustomerCode asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Last")
                {

                    #region Last Item

                    sqlText = sqlText + @"
--------------------------------------------------Last--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 CustomerId as Id, CustomerCode as Code from Customers c
where 1=1 
and c.IsArchive = 0
and ISNULL(CustomerId, '0') <> '0'
order by CustomerCode desc

";
                    #endregion

                }
                else if (vm.ButtonName == "Next")
                {

                    #region Next Item

                    sqlText = sqlText + @"
--------------------------------------------------Next--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 CustomerId as Id, CustomerCode as Code from Customers c
where 1=1 
and c.IsArchive = 0
and c.CustomerCode > @CustomerCode
and ISNULL(CustomerId, '0') <> '0'
order by CustomerCode asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Previous")
                {
                    #region Previous Item

                    sqlText = sqlText + @"
--------------------------------------------------Previous--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 CustomerId as Id, CustomerCode as Code from Customers c
where 1=1 
and c.IsArchive = 0
and c.CustomerCode < @CustomerCode
and ISNULL(CustomerId, '0') <> '0'
order by CustomerCode desc

";
                    #endregion
                }

                #endregion

                #region SQL Execution
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);



                if (!string.IsNullOrWhiteSpace(vm.Code))
                {
                    cmd.Parameters.AddWithValue("@CustomerCode", vm.Code);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    vm.Code = dt.Rows[0]["Code"].ToString();
                }
                else
                {
                    if (vm.ButtonName == "Previous" || vm.ButtonName == "Current")
                    {
                        vm.ButtonName = "First";
                        vm = Customer_Navigation(vm, currConn, transaction);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = Customer_Navigation(vm, currConn, transaction);

                    }
                }


                #endregion

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "Customer_Navigation", ex.ToString() + "\n" + sqlText);

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

            return vm;

        }

        #endregion

        public DataTable SearchCountry(string customer, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";










            DataTable dataTable = new DataTable();
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

select Country from Customers where [CustomerName]=@customer";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@customer"))
                { objCommProductType.Parameters.AddWithValue("@customer", customer); }
                else { objCommProductType.Parameters["@customer"].Value = customer; }


                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CustomerDAL", "SearchCountry", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "SearchCountry", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchCustomerName(string customer, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";

            DataTable dataTable = new DataTable();
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

select CustomerName from Customers where [CustomerID]=@customer";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@customer"))
                { objCommProductType.Parameters.AddWithValue("@customer", customer); }
                else { objCommProductType.Parameters["@customer"].Value = customer; }


                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CustomerDAL", "SearchCustomerName", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "SearchCustomerName", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchCustomerByCode(string customerCode, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";

            DataTable dataTable = new DataTable();
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

select * from Customers where [CustomerCode]=@CustomerCode";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@CustomerCode"))
                { objCommProductType.Parameters.AddWithValue("@CustomerCode", customerCode); }
                else { objCommProductType.Parameters["@CustomerCode"].Value = customerCode; }


                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CustomerDAL", "SearchCustomerByCode", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "SearchCustomerByCode", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  CustomersAddress";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Customers Address",
                                                    "Unable to create new Customers Address");
                }


                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";
                sqlText += "insert into CustomersAddress";
                sqlText += "(";
                sqlText += "Id,";
                sqlText += "CustomerID,";
                sqlText += "CustomerAddress";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@Id,";
                sqlText += "@CustomerID,";
                sqlText += "@CustomerAddress";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("Id", nextId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("CustomerID", vm.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("CustomerAddress", vm.CustomerAddress);
                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Address successfully Added";
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
                            retResults[1] = "Requested customer  Address successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + nextId;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add customer";
                            retResults[2] = "";
                            retResults[3] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add customer ";
                        retResults[2] = "";
                        retResults[3] = "";
                    }
                }
                #endregion Commit
                #endregion Commit


                #endregion Inser new customer

            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {


                if (Vtransaction == null)
                {
                    if (transaction != null) transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("CustomerDAL", "InsertToCustomerAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {


                if (Vtransaction == null)
                {
                    if (transaction != null) transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "InsertToCustomerAddress", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Invalid Customer  Address");
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
                sqlText = "update CustomersAddress set";
                sqlText += " CustomerAddress='" + vm.CustomerAddress + "'";
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
                            retResults[1] = "Requested customers  Address successfully Update";
                            retResults[2] = vm.Id.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update customers ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update customer group";
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

                FileLogger.Log("CustomerDAL", "UpdateToCustomerAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "UpdateToCustomerAddress", ex.ToString() + "\n" + sqlText);

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

        public string[] DeleteCustomerAddress(string CustomerID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    sqlText = "delete CustomersAddress where Id='" + Id + "'";
                }
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    sqlText = "delete CustomersAddress where CustomerID='" + CustomerID + "'";
                }
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Address successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customers  Address successfully deleted";
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

                FileLogger.Log("CustomerDAL", "DeleteCustomerAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "DeleteCustomerAddress", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchCustomerAddress(string CustomerID, string Id, string address, SysDBInfoVMTemp connVM = null)
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
isnull(cad.Id,'0')Id
,isnull(cad.CustomerID,'0')CustomerID
,c.CustomerCode
,c.CustomerName
,isnull(cad.CustomerAddress,'-')CustomerAddress
,cg.CustomerGroupName
,isnull(cad.CustomerVATRegNo,c.VATRegistrationNo)CustomerVATRegNo
FROM CustomersAddress cad
left outer join Customers c on cad.CustomerID=c.CustomerID
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID 
WHERE 1=1 and  c.ActiveStatus='Y'

";

                if (!string.IsNullOrEmpty(CustomerID))
                {
                    sqlText += @"  and cad.CustomerID=@CustomerID";
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @"  and cad.Id=@Id";
                }

                if (!string.IsNullOrEmpty(address))
                {
                    sqlText += @"  and cad.CustomerAddress like '%' + @address + '%' ";
                }
                sqlText += @"  order by  cad.CustomerAddress";

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
                if (!string.IsNullOrEmpty(address))
                {
                    objCommCustomerInformation.Parameters["@address"].Value = address;
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

                FileLogger.Log("CustomerDAL", "SearchCustomerAddress", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "SearchCustomerAddress", ex.ToString() + "\n" + sqlText);

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

        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Customer");

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


                #region Sql Command

                sqlText = @" SELECT 
  row_number() OVER (ORDER BY c.CustomerID) SL
       ,c.[CustomerCode] Code
      ,c.[CustomerName]
      ,cg.CustomerGroupName CustomerGroup
      ,c.[Address1]
     -- ,c.[Address2]
     -- ,c.[Address3]
      ,c.[City]
	  ,c.[Country]
      ,c.[TelephoneNo]
      ,c.[FaxNo]
      ,c.[Email]
      ,cast(c.[StartDateTime] as varchar(100)) StartDateTime

      ,c.[ContactPerson]
      ,c.[ContactPersonDesignation]
      ,c.[ContactPersonTelephone]
      ,c.[ContactPersonEmail]
      ,c.[TINNo] TIN


      ,c.[VATRegistrationNo]
      ,c.[Comments]

      ,c.[ActiveStatus]
      ,c.[IsVDSWithHolder]
      ,isnull(c.[IsInstitution],'N') IsInstitution
      ,isnull(c.[IsSpecialRate],'N') IsSpecialRate
      ,isnull(c.[IsTax],'N') IsTax


  FROM [Customers] c left outer join CustomerGroups cg 
  on c.CustomerGroupID = cg.CustomerGroupID where c.[CustomerCode] in ";


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
                //    cmd.Parameters.AddWithValue("@code" + i, ids[i]);
                //}

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);


                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }


        public DataTable GetExcelAddress(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CustomerAdd");

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


                #region Sql Command

                sqlText = @" SELECT ROW_NUMBER() OVER (order by ca.CustomerID) SL
	 ,c.CustomerCode Code
	 ,c.CustomerName
      ,ca.[CustomerAddress] [Address]
  FROM CustomersAddress ca left outer join Customers c
  on c.CustomerID = ca.CustomerID
  where c.CustomerCode in  ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                if (ids.Count == 0)
                {
                    sqlText += "''";
                }

                sqlText = sqlText.TrimEnd(',') + ")";


                var cmd = new SqlCommand(sqlText, currConn, transaction);

                //for (int i = 0; i < len; i++)
                //{
                //    cmd.Parameters.AddWithValue("@code" + i, ids[i]);
                //}

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);


                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("CustomerDAL", "GetExcelAddress", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }

        #region web methods
        public List<CustomerVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
af.CustomerID
,af.CustomerName
,af.CustomerCode
FROM Customers af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerVM();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.CustomerCode = dr["CustomerCode"].ToString();
                    vm.CustomerName = vm.CustomerName + "(" + vm.CustomerCode + ")";

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

                FileLogger.Log("CustomerDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<CustomerVM> DropDownByCustomerID(int CustomerID = 0, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
af.CustomerID
,af.CustomerName
FROM Customers af
WHERE  1=1 AND af.ActiveStatus = 'Y' and af.CustomerID='" + CustomerID + "'";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerVM();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
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

                FileLogger.Log("CustomerDAL", "DropDownByCustomerID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDAL", "DropDownByCustomerID", ex.ToString() + "\n" + sqlText);

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

        public List<CustomerVM> DropDownByGroup(string groupId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
af.CustomerID
,af.CustomerName
FROM Customers af
WHERE  1=1 AND af.ActiveStatus = 'Y' AND af.CustomerGroupID=@groupId
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValueAndNullHandle("@groupId", groupId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerVM();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
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

                FileLogger.Log("CustomerDAL", "DropDownByGroup", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDAL", "DropDownByGroup", ex.ToString() + "\n" + sqlText);

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

        public List<CustomerVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
                        vm = new CustomerVM();
                        vm.CustomerID = dr["CustomerID"].ToString();

                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.CustomerGroupID = dr["CustomerGroupID"].ToString();
                        vm.Address1 = dr["Address1"].ToString();
                        vm.Address2 = dr["Address2"].ToString();
                        vm.Address3 = dr["Address3"].ToString();
                        vm.City = dr["City"].ToString();
                        vm.TelephoneNo = dr["TelephoneNo"].ToString();
                        vm.FaxNo = dr["FaxNo"].ToString();
                        vm.Email = dr["Email"].ToString();
                        vm.StartDateTime = dr["StartDateTime"].ToString();
                        vm.ContactPerson = dr["ContactPerson"].ToString();
                        vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                        vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                        vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                        vm.TINNo = dr["TINNo"].ToString();
                        vm.VATRegistrationNo = dr["VATRegistrationNo"].ToString();
                        vm.NIDNo = dr["NIDNo"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.Info2 = dr["Info2"].ToString();
                        vm.Info3 = dr["Info3"].ToString();
                        vm.Info4 = dr["Info4"].ToString();
                        vm.Info5 = dr["Info5"].ToString();
                        vm.Country = dr["Country"].ToString();
                        vm.VDSPercent = Convert.ToDecimal(dr["VDSPercent"]);
                        vm.BusinessType = dr["BusinessType"].ToString();
                        vm.BusinessCode = dr["BusinessCode"].ToString();
                        vm.IsVDSWithHolder = dr["IsVDSWithHolder"].ToString();
                        vm.IsExamted = dr["IsExamted"].ToString();
                        vm.IsInstitution = dr["IsInstitution"].ToString();
                        vm.IsTax = dr["IsTax"].ToString();
                        vm.IsTCS = dr["IsTCS"].ToString();
                        vm.ShortName = dr["ShortName"].ToString();
                        vm.IsCreditCustomer = dr["IsCreditCustomer"].ToString();
                        vm.SC = dr["SC"].ToString();
                        vm.RF = dr["RF"].ToString();
                        vm.SSLF = dr["SSLF"].ToString();
                        vm.DC = dr["DC"].ToString();

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

                FileLogger.Log("CustomerDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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

                if (count == "All")
                {
                    sqlText = @"SELECT ";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";

                }

                sqlText += @"

c.CustomerCode
,c.CustomerName
,c.Address1
,cg.CustomerGroupName
,cg.GroupType
 ,c.CustomerID
,c.City
,c.TelephoneNo
,c.FaxNo
,c.Email
,c.StartDateTime
,c.ContactPerson
,c.ContactPersonDesignation
,c.ContactPersonTelephone
,c.ContactPersonEmail
,c.TINNo
,c.VATRegistrationNo
,c.NIDNo
,c.Comments
,c.ActiveStatus
,c.CreatedBy
,c.CreatedOn
,c.LastModifiedBy
,c.LastModifiedOn
,c.CustomerGroupID
,c.Address2
,c.Address3
,c.Info2
,c.Info3
,c.Info4
,c.Info5
,c.Country
,ISNULL(c.VDSPercent,0) VDSPercent 
,c.BusinessType
,c.BusinessCode

,ISNULL(c.ShortName,'-') ShortName
,ISNULL(c.IsVDSWithHolder,'N') IsVDSWithHolder
,ISNULL(c.IsInstitution,'N') IsInstitution
,ISNULL(c.IsExamted,'N') IsExamted
,ISNULL(c.IsSpecialRate,'N') IsSpecialRate
,ISNULL(c.IsTax,'N') IsTax
,ISNULL(c.IsTCS,'N') IsTCS
,ISNULL(c.IsCreditCustomer,'N') IsCreditCustomer
,ISNULL(c.SC,0) SC 
,ISNULL(c.RF,0) RF 
,ISNULL(c.SSLF,0) SSLF 
,ISNULL(c.DC,0) DC 


,ISNULL(c.BranchId,0) BranchId

FROM Customers  c left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 AND  isnull(c.IsArchive,0) = 0 

";
                #region sqlTextCount
                sqlTextCount += @" select count(c.CustomerID)RecordCount
FROM Customers  c left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 AND  isnull(c.IsArchive,0) = 0
";
                #endregion


                if (!string.IsNullOrWhiteSpace(Id) && Id != "0")
                {
                    sqlTextParameter += @" and c.CustomerID=@CustomerID";
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


                //OrderBy
                //sqlTextOrderBy += " order by c.CustomerCode";


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

                if (!string.IsNullOrWhiteSpace(Id) && Id != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CustomerID", Id);
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("CustomerDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("CustomerDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToCustomerNew(CustomerVM vm, bool CustomerAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            string customerCode = vm.CustomerCode;
            int nextId = 0;

            #endregion

            #region Try
            try
            {
                #region settingsValue

                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Customer", null, VcurrConn, Vtransaction) == "Y");
                bool CustomerBINCheck = Convert.ToBoolean(commonDal.settingValue("Customer", "CustomerBINCheck", null, VcurrConn, Vtransaction) == "Y");

                #endregion settingsValue

                #region Validation

                if (string.IsNullOrEmpty(vm.CustomerName))
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Please enter customer group name.");
                }
                if (string.IsNullOrEmpty(vm.CustomerGroupID))
                {
                    throw new ArgumentNullException("UpdateToCustomer",
                                                    "Invalid Customer GroupID type.");
                }


                string VATRegistrationNo = vm.VATRegistrationNo;
                string VATRegNo = VATRegistrationNo.Replace("-", "");


                //if (!OrdinaryVATDesktop.IsNumeric(VATRegNo))
                //{
                //    throw new ArgumentNullException("InsertToVendorInformation",
                //                                   "Please enter VATRegistrationNo only number.");

                //}

                if (CustomerBINCheck)
                {
                    if (VATRegNo.Length < 9)
                    {
                        throw new ArgumentNullException("InsertToCustomerInformation", "Please enter Valid VATRegistrationNo No/Not more than 9 digit.");

                    }
                }


                if (string.IsNullOrEmpty(vm.NIDNo))
                {
                    vm.NIDNo = "-";
                }

                if (vm.NIDNo != "-")
                {
                    if (!OrdinaryVATDesktop.IsNumber(vm.NIDNo))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation", "Please Enter National ID  only number");
                    }

                    if (vm.NIDNo.Length >= 18)
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                               "Please Enter Valid National ID No./Not more than 17 digit.");

                    }

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

                #region Customer  name existence checking

                //select @Present = count(CustomerID) from Customers where CustomerID = @CustomerID;
                //sqlText = "select count(CustomerID) from Customers where  CustomerName='" + CustomerName + "'" +
                //          " and CustomerGroupID='" + CustomerGroupID + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //countId = (int)cmdNameExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("InsertToCustomer", "Same customer  name('" + CustomerName + "') already exist under same Group");
                //}

                #endregion Customer Group name existence checking

                #region Customer  new id generation
                sqlText = "select isnull(max(cast(CustomerID as int)),0)+1 FROM  Customers";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #region Code
                if (Auto == false)
                {
                    if (CustomerAutoSave)
                    {
                        if (string.IsNullOrWhiteSpace(vm.CustomerCode))
                        {
                            customerCode = nextId.ToString();
                        }
                        // Customer Group Id
                    }
                    else if (string.IsNullOrEmpty(customerCode))
                    {
                        throw new ArgumentNullException("InsertToCustomer", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(CustomerID) from Customers where  CustomerCode=@customerCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@customerCode", customerCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same customer  Code('" + customerCode + "') already exist");
                        }
                    }
                }
                else
                {
                    customerCode = nextId.ToString();
                }
                #endregion Code

                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";

                sqlText += @" 
INSERT INTO Customers(
 CustomerID
,CustomerCode
,CustomerName
,CustomerGroupID
,Address1
,Address2
,Address3
,City
,TelephoneNo
,FaxNo
,Email
,StartDateTime
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TINNo
,VATRegistrationNo
,NIDNo
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Country
,VDSPercent
,BusinessType
,BusinessCode
,IsVDSWithHolder
,IsInstitution
,IsExamted
,ShortName
,BranchId
,IsArchive
,IsTax
,IsSpecialRate
,IsTCS
,IsCreditCustomer
,SC
,RF
,SSLF
,DC
) VALUES (
 @CustomerID
,@CustomerCode
,@CustomerName
,@CustomerGroupID
,@Address1
,@Address2
,@Address3
,@City
,@TelephoneNo
,@FaxNo
,@Email
,@StartDateTime
,@ContactPerson
,@ContactPersonDesignation
,@ContactPersonTelephone
,@ContactPersonEmail
,@TINNo
,@VATRegistrationNo
,@NIDNo
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@Country
,@VDSPercent
,@BusinessType
,@BusinessCode 
,@IsVDSWithHolder 
,@IsInstitution 
,@IsExamted 
,@ShortName 
,@BranchId 
,@IsArchive  
,@IsTax
,@IsSpecialRate
,@IsTCS
,@IsCreditCustomer
,@SC
,@RF
,@SSLF
,@DC
) 
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@CustomerID", nextId.ToString());
                cmdInsert.Parameters.AddWithValue("@CustomerCode", customerCode);
                cmdInsert.Parameters.AddWithValue("@CustomerName", vm.CustomerName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@StartDateTime", OrdinaryVATDesktop.DateToDate(vm.StartDateTime));
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ?? "Y");
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdInsert.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsVDSWithHolder", vm.IsVDSWithHolder ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsInstitution", vm.IsInstitution ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsExamted", vm.IsExamted ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ShortName", vm.ShortName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@IsSpecialRate", vm.IsSpecialRate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsTax", vm.IsTax ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsTCS", vm.IsTCS ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsCreditCustomer", vm.IsCreditCustomer ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@SC", vm.SC ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@RF", vm.RF ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@SSLF", vm.SSLF ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@DC", vm.DC ?? Convert.DBNull);


                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    CustomerAddressVM cvm = new CustomerAddressVM();
                    cvm.CustomerID = nextId.ToString();
                    cvm.CustomerAddress = vm.Address1;
                    retResults = InsertToCustomerAddress(cvm, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", "SQL:" + sqlText);//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                    }

                }
                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested customer  Information successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + customerCode;
                        }
                    }
                }

                #endregion Commit


                #endregion Inser new customer

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                //"Unexpected erro to add customer";
                retResults[0] = "Fail";
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault();
                retResults[2] = "";
                retResults[3] = "";
                if (VcurrConn == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("CustomerDAL", "InsertToCustomerNew", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public string[] UpdateToCustomerNew(CustomerVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.CustomerID;

            string customerCode = vm.CustomerCode;

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
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Customer") == "Y" ? true : false);
                bool IsUnilever = OrdinaryVATDesktop.IsUnileverCompany((commonDal.settingValue("CompanyCode", "Code", connVM)));
                #endregion settingsValue

                #region Validation
                if (vm.CustomerID.Trim() == "0")
                {
                    retResults[0] = "Fail";
                    retResults[1] = "This customer information unable to update!";
                    retResults[2] = vm.CustomerID;

                    return retResults;
                }
                if (string.IsNullOrEmpty(vm.CustomerID))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Invalid Customer ID");
                }
                if (string.IsNullOrEmpty(vm.CustomerName))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Invalid Customer Name.");
                }
                if (string.IsNullOrEmpty(vm.TelephoneNo))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Please enter customer TelephoneNo");
                }


                if (!IsUnilever)
                {
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

                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToCustomers");

                #endregion open connection and transaction

                #region Customer  existence checking

                sqlText = "select count(CustomerID) from Customers where  CustomerID=@CustomerID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@CustomerID", vm.CustomerID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                "Could not find requested customers  id.");
                }

                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(customerCode))
                    {
                        throw new ArgumentNullException("InsertToCustomer", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(CustomerID) from Customers where  CustomerCode=@CustomerCode" +
                                  " and CustomerID <>@CustomerID";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@CustomerCode", vm.CustomerCode);
                        cmdCodeExist.Parameters.AddWithValue("@CustomerID", vm.CustomerID);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same customer  Code('" + customerCode + "') already exist");
                        }
                    }
                }
                else
                {
                    customerCode = vm.CustomerID;
                }
                #endregion Code
                #endregion Customer Group existence checking

                #region Customer  name existence checking
                //sqlText = "select count(CustomerName) from Customers ";
                //sqlText += " where  CustomerName='" + CustomerName + "'";
                //sqlText += " and CustomerGroupID ='" + CustomerGroupID + "'" +
                //           " and CustomerID <>'" + CustomerID + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //countId = (int)cmdNameExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("UpdateToCustomers",
                //                                    "Same customer name already exist");
                //}
                #endregion Customer  name existence checking



                #region Update new customer group
                sqlText = "";
                sqlText = "update Customers set";
                sqlText += "  CustomerCode              =@CustomerCode";
                sqlText += " ,CustomerName              =@CustomerName";
                sqlText += " ,CustomerBanglaName        =@CustomerBanglaName";
                sqlText += " ,CustomerGroupID           =@CustomerGroupID";
                sqlText += " ,Address1                  =@Address1";
                sqlText += " ,Address2                  =@Address2";
                sqlText += " ,Address3                  =@Address3";
                sqlText += " ,City                      =@City";
                sqlText += " ,TelephoneNo               =@TelephoneNo";
                sqlText += " ,FaxNo                     =@FaxNo";
                sqlText += " ,Email                     =@Email";
                sqlText += " ,StartDateTime             =@StartDateTime";
                sqlText += " ,ContactPerson             =@ContactPerson";
                sqlText += " ,ContactPersonDesignation  =@ContactPersonDesignation";
                sqlText += " ,ContactPersonTelephone    =@ContactPersonTelephone";
                sqlText += " ,ContactPersonEmail        =@ContactPersonEmail";
                sqlText += " ,VATRegistrationNo         =@VATRegistrationNo";
                sqlText += " ,NIDNo                     =@NIDNo";
                sqlText += " ,TINNo                     =@TINNo";
                sqlText += " ,Comments                  =@Comments";
                sqlText += " ,ActiveStatus              =@ActiveStatus";
                sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                sqlText += " ,Country                   =@Country";
                sqlText += " ,VDSPercent                =@VDSPercent";
                sqlText += " ,BusinessType              =@BusinessType";
                sqlText += " ,BusinessCode              =@BusinessCode";
                sqlText += " ,IsVDSWithHolder           =@IsVDSWithHolder";
                sqlText += " ,ShortName                 =@ShortName";
                sqlText += " ,IsInstitution             =@IsInstitution";
                sqlText += " ,IsExamted                 =@IsExamted";
                sqlText += " ,IsSpecialRate             =@IsSpecialRate";
                sqlText += " ,IsTax                     =@IsTax";
                sqlText += " ,IsTCS                     =@IsTCS";
                sqlText += " ,IsCreditCustomer                     =@IsCreditCustomer";
                sqlText += " ,SC                     =@SC";
                sqlText += " ,RF                     =@RF";
                sqlText += " ,SSLF                     =@SSLF";
                sqlText += " ,DC                     =@DC";
                sqlText += " WHERE CustomerID           =@CustomerID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@CustomerCode", vm.CustomerCode);
                cmdUpdate.Parameters.AddWithValue("@CustomerName", vm.CustomerName);
                cmdUpdate.Parameters.AddWithValue("@CustomerBanglaName", vm.CustomerBanglaName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);
                cmdUpdate.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@StartDateTime", OrdinaryVATDesktop.DateToDate(vm.StartDateTime) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdUpdate.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CustomerID", vm.CustomerID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsVDSWithHolder", vm.IsVDSWithHolder ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ShortName", vm.ShortName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsInstitution", vm.IsInstitution ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsExamted", vm.IsExamted ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsTax", vm.IsTax ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsTCS", vm.IsTCS ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsSpecialRate", vm.IsSpecialRate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsCreditCustomer", vm.IsCreditCustomer ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@SC", vm.SC ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@RF", vm.RF ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@SSLF", vm.SSLF ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@DC", vm.DC ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customers  Information successfully Update";
                        retResults[2] = vm.CustomerID;
                        retResults[3] = customerCode;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update customers ";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update customer group";
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

                ////transaction.Rollback();
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("CustomerDAL", "UpdateToCustomerNew", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public string[] Delete(CustomerVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCustomer"; //Method Name
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
                        sqlText = "update Customers set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where CustomerID=@CustomerID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@CustomerID", ids[i]);
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
                        throw new ArgumentNullException("Customer Delete", vm.CustomerID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Customer Information Delete", "Could not found any item.");
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

                FileLogger.Log("CustomerDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        public List<string> AutocompleteCustomer(string term, SysDBInfoVMTemp connVM = null)
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
CustomerCode
,CustomerName
FROM Customers  ";
                sqlText += @" WHERE CustomerName like '%" + term + "%'  and ActiveStatus='Y' ORDER BY CustomerName";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["CustomerName"].ToString());
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

        public string[] BulkInsertNewCustomers(DataTable Customerdt, int branchId = 1, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            ////string customerCode = vm.CustomerCode;
            int nextId = 0;
            DataTable dt = new DataTable();

            #endregion

            #region Try
            try
            {

                #region settingsValue

                CommonDAL commonDal = new CommonDAL();
                bool AutoCode = commonDal.settingValue("AutoCode", "Customer", null, VcurrConn, Vtransaction) == "Y";
                bool CustomerNameCheck = commonDal.settingValue("Sale", "CustomerNameCheck", null, currConn, transaction) == "Y";

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

                string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string CreatedOn = DateTime.Now.ToString();

                #region Delete Exits Customers

                sqlText = "";
                sqlText = @"
delete CustomerTempData where CustomerCode in(select CustomerCode from Customers)
";
                if (CustomerNameCheck)
                {
                    sqlText += @"
delete CustomerTempData where CustomerName in(select CustomerName from Customers)
";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                #endregion

                #region Delete Duplicat

                sqlText = @"
;with cte
as (
	
	select CustomerCode, row_number() over( partition by CustomerCode order by CustomerCode) RowNumber
	from CustomerTempData

)
delete from cte where RowNumber > 1
                ";

                if (AutoCode)
                {
                    sqlText = @"
;with cte
as (
	
	select customername, row_number() over( partition by customerName order by customerName) RowNumber
	from CustomerTempData

)
delete from cte where RowNumber > 1
                ";

                }

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                #endregion

                #region Customer Code Check

                if (!AutoCode)
                {

                    sqlText = @"
select distinct CustomerName from CustomerTempData 
where CustomerCode is null or CustomerCode = '' or CustomerCode = '-'

";
                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string customerNames = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            customerNames = customerNames + row["CustomerName"].ToString() + ",";
                        }

                        customerNames = customerNames.Trim(',');
                        throw new ArgumentNullException("", "Invalid customer code for  customer name(" + customerNames + ")");

                    }
                }

                #endregion

                #region Customer Name Check

                sqlText = @"
select distinct CustomerCode from CustomerTempData 
where CustomerName is null or CustomerName = '' or CustomerName = '-'

";
                cmd = new SqlCommand(sqlText, currConn, transaction);

                DataTable customerName = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(customerName);

                if (customerName != null && customerName.Rows.Count > 0)
                {
                    string customerCodes = "";
                    foreach (DataRow row in customerName.Rows)
                    {
                        customerCodes = customerCodes + row["CustomerCode"].ToString() + ",";
                    }

                    customerCodes = customerCodes.Trim(',');
                    throw new ArgumentNullException("", "Invalid customer name for  customer Code(" + customerCodes + ")");

                }

                #endregion

                #region Update Customer GroupID

                sqlText = @"
          --update CustomerTempData set BranchId=BranchProfiles.BranchID 
          --from BranchProfiles where BranchProfiles.BranchCode=CustomerTempData.Branch_Code or BranchProfiles.IntegrationCode = CustomerTempData.Branch_Code;
          --
          --update CustomerTempData set BranchId = BranchMapDetails.BranchID from BranchMapDetails 
          --where   BranchMapDetails.IntegrationCode = CustomerTempData.Branch_Code and (CustomerTempData.BranchId  is null or 
          --CustomerTempData.BranchId ='0' or CustomerTempData.BranchId = ''
          --)
                
                update CustomerTempData set CustomerGroupID=CustomerGroups.CustomerGroupID 
                from CustomerGroups where CustomerGroups.CustomerGroupName=CustomerTempData.CustomerGroup 
                and (CustomerTempData.CustomerGroupID  is null or CustomerTempData.CustomerGroupID  = 0) 
                and CustomerTempData.CustomerGroup  is not null
                
                ";

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                #endregion

                #region Select Customer Group

                sqlText = @"
select distinct CustomerGroupID, CustomerGroup from CustomerTempData 
where (CustomerGroupID is null or CustomerGroupID = 0)

";
                cmd = new SqlCommand(sqlText, currConn, transaction);

                DataTable customerGroup = new DataTable();
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(customerGroup);

                if (customerGroup != null && customerGroup.Rows.Count > 0)
                {
                    string Groups = "";
                    foreach (DataRow row in customerGroup.Rows)
                    {
                        Groups = Groups + row["CustomerGroup"].ToString() + ",";
                    }

                    Groups = Groups.Trim(',');
                    throw new ArgumentNullException("", "Customer Group Not Found in VAT system Group(" + Groups + ")");

                }

                #endregion

                #region update StartDateTime

                sqlText = @"
update CustomerTempData set StartDateTime=@StartDateTime,BranchId=@BranchId,CreatedOn=@CreatedOn

";
                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@StartDateTime", CurrentDate);
                cmd.Parameters.AddWithValue("@BranchId", branchId);
                cmd.Parameters.AddWithValue("@CreatedOn", CreatedOn);
                cmd.ExecuteNonQuery();

                #endregion

                #region Insert New Customers

                sqlText = @"
declare @max int
select @max=isnull(max(cast(CustomerID as int)),0) FROM  Customers

insert into Customers (CustomerID,CustomerName,CustomerCode,Address1,StartDateTime,ShortName,CustomerGroupID	
,IsArchive,ActiveStatus,BranchId,VATRegistrationNo,CreatedBy,CreatedOn,Address2,Address3,City,TelephoneNo	
,FaxNo,Email,ContactPerson,ContactPersonDesignation,ContactPersonTelephone,ContactPersonEmail,TINNo
,Comments,BusinessType,BusinessCode,IsVDSWithHolder)
select 
@max + ROW_NUMBER() over(order by Id) CustomerID
,CustomerName,CustomerCode,Address1,StartDateTime,CustomerName ShortName,CustomerGroupID
,isnull(IsArchive,0)IsArchive,isnull(ActiveStatus,'Y')ActiveStatus,BranchId
,isnull(VATRegistrationNo,'-')VATRegistrationNo,CreatedBy,CreatedOn
,'-','-','-','-','-','-','-','-','-','-','-','-','-','-','-'
from CustomerTempData
";

                if (AutoCode)
                {
                    sqlText = @"
declare @max int
select @max=isnull(max(cast(CustomerID as int)),0) FROM  Customers

insert into Customers (CustomerID,CustomerName,CustomerCode,Address1,StartDateTime,ShortName,CustomerGroupID	
,IsArchive,ActiveStatus,BranchId,VATRegistrationNo,CreatedBy,CreatedOn,Address2,Address3,City,TelephoneNo	
,FaxNo,Email,ContactPerson,ContactPersonDesignation,ContactPersonTelephone,ContactPersonEmail,TINNo
,Comments,BusinessType,BusinessCode,IsVDSWithHolder)
select 
@max + ROW_NUMBER() over(order by Id) CustomerID
,CustomerName
,@max + ROW_NUMBER() over(order by Id) CustomerCode
,Address1,StartDateTime,CustomerName ShortName,CustomerGroupID
,isnull(IsArchive,0)IsArchive,isnull(ActiveStatus,'Y')ActiveStatus,BranchId
,isnull(VATRegistrationNo,'-')VATRegistrationNo,CreatedBy,CreatedOn
,'-','-','-','-','-','-','-','-','-','-','-','-','-','-','-'
from CustomerTempData
";
                }

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[2] = "0";

                #endregion

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                //"Unexpected erro to add customer";
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "";
                retResults[3] = "";
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("CustomerDAL", "BulkInsertNewCustomers", ex.ToString() + "\n" + sqlText);

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


        public string[] ImportExcelIntegrationFile(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null,
            SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, string UserId = "")
        {

            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImportExcelFile"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion


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
                    transaction = currConn.BeginTransaction();
                }
                #endregion open connection and transaction

                #region Excel Reader

                string FileName = paramVM.File.FileName;
                DataSet ds = new DataSet();
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(paramVM.File.InputStream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(paramVM.File.InputStream);
                }

                if (reader != null)
                {
                    reader.IsFirstRowAsColumnNames = true;
                    ds = reader.AsDataSet();
                    reader.Close();
                }

                #endregion

                CommonDAL commonDal = new CommonDAL();
                string code = commonDal.settingValue("CompanyCode", "Code", null, currConn, transaction);

                
                if (code.ToLower() == "brac")
                {
                    if (!ds.Tables.Contains("CustomerPrice"))
                        throw new Exception("Correct sheet not found");

                    DataTable dtcustomerPrice = ds.Tables["CustomerPrice"];

                    sqlText = @"create table #tempData
(
Sl int identity(1,1) primary key
,Customer_Group varchar(50)
,Item_Code varchar(50)
,NBR_Price decimal(18,6)
,GroupId varchar(50)
)
";
                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.ExecuteNonQuery();

                    dtcustomerPrice.Columns.Remove("SL");

                    string[] result = commonDal.BulkInsert("#tempData", dtcustomerPrice, currConn, transaction);

                    sqlText = @"
update #tempData set GroupId = cg.CustomerGroupID
from #tempData td join 
CustomerGroups cg on td.Customer_Group = cg.CustomerGroupName


select distinct Customer_Group from #tempData where groupId is null";

                    cmd.CommandText = sqlText;
                    DataTable dtResult = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dtResult);


                    if (dtResult.Rows.Count > 0)
                    {
                        throw new Exception("Group Not Found - " + string.Join(",",
                            dtResult.AsEnumerable().Select(x => x["Customer_Group"].ToString())));
                    }


                    sqlText = @"
delete from GroupPriceMapping
from #tempData td join GroupPriceMapping gp 
on td.GroupId = gp.GroupId and td.Item_Code = gp.Item_Code

insert into GroupPriceMapping([Customer_Group]
      ,[Item_Code]
      ,[NBR_Price]
      ,[GroupId])

select [Customer_Group]
      ,[Item_Code]
      ,[NBR_Price]
      ,[GroupId]
	  from #tempData

";

                    cmd.CommandText = sqlText;
                    cmd.ExecuteNonQuery();


                }



                retResults[0] = "Success";//Success or Fail
                retResults[1] = "Success";// Success or Fail Message

                #region Commit
                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                #endregion Commit


            }
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString();
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("SaleDAL", "ImportExcelIntegrationFile", ex.ToString() + "\n" + sqlText);

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

            return retResults;

        }

    }

}
