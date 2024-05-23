using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class CommonImportDAL : ICommonImport
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

        public string FindVatName(string vatName, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";

            #endregion

            #region Validation
            if (string.IsNullOrEmpty(vatName))
            {
                throw new ArgumentNullException("FindVatName", "Invalid Vat Name.");
            }

            #endregion Validation
            #region Find Vat Name
            VATName vname = new VATName();

            for (int i = 0; i < vname.VATNameList.Count; i++)
            {
                if (vatName == vname.VATNameList[i])
                {
                    retResults = vatName;
                    break;
                }
            }

            if (retResults != vatName)
            {
                throw new ArgumentNullException("FindVatName", "VAT Name  '" + vatName + "' to found in database");

            }

            #endregion Find Vat Name

            #region Results
            return retResults;
            #endregion

        }

        #region bool

        public bool CheckYN(string Post)
        {
            bool result = true;

            if (string.IsNullOrEmpty(Post))
            {
                result = false;
            }
            else
            {
                if (Post.ToUpper() == "Y")
                {
                    result = true;
                }
                else if (Post.ToUpper() == "N")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public bool CheckNumericBool(string input)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    Convert.ToDecimal(input);
                    result = true;
                }
                else
                {
                    result = false;
                }

                return result;
            }
            #region Catch
            catch (Exception ex)
            {
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                FileLogger.Log("CommonImportDAL", "CheckNumericBool", ex.ToString());

                return result;

            }
            #endregion Catch
        }

        public bool CheckDate(string input)
        {
            bool result = false;
            //var nInput = input.Trim();
            #region try

            try
            {
                if (input.Contains("/") || input.Contains("-") || input.Trim().Contains(" "))
                {

                    if (!string.IsNullOrEmpty(input))
                    {
                        //////Correct format "MM/dd/yyyy","MM/dd/yy","dd/MMM/yy"
                        Convert.ToDateTime(input);
                        result = true;

                    }
                }
                else
                {
                    double d = double.Parse(input);
                    DateTime conv = DateTime.FromOADate(d);
                    result = true;
                }


                return result;
            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                //////FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                FileLogger.Log("CommonImportDAL", "CheckDate", ex.ToString());

                return result;

            }
            #endregion Catch
        }

        public string ChecKNullValue(string input)
        {
            string result = "-";

            if (string.IsNullOrEmpty(input))
            {
                result = "-";

            }
            else
            {
                result = input;
            }

            return result;
        }

        #endregion bool

        #region Burear

        //currConn to VcurrConn 25-Aug-2020

        public string FindCustomerId(string customerName, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation

                if (string.IsNullOrEmpty(customerName))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }

                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CustomerID ";
                sqlText += " from Customers";
                sqlText += " where ";
                sqlText += " CustomerName='" + customerName + "' ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCustomerId", "Customer '(" + customerName + ")' not in database");
                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindCustomerId", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindCustomerId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion Catch

            #region finally

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public bool CheckBureauDate(string input)
        {
            bool result = false;
            #region try

            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    Convert.ToDateTime(input);
                    //Convert.ToDateTime("24/9/2013");
                    result = true;

                }


                return result;
            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);

                FileLogger.Log("CommonImportDAL", "CheckBureauDate", ex.ToString());

                return result;

            }
            #endregion Catch
        }

        //currConn to VcurrConn 25-Aug-2020

        public DataTable GetProductInfo(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            // for TollReceive
            #region Variables

            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            DataTable dataTable = new DataTable("ProductInfo");

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //        }
                //        Vtransaction = VcurrConn.BeginTransaction("Products");
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Products");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "";
                sqlText = @" 
Select Top 1 ItemNo,ProductCode,ProductName,Products.SD,Products.UOM from Products,ProductCategories 
where Products.CategoryID = ProductCategories.CategoryID 
and IsRaw IN('Service','Service(NonStock)') 
and CategoryName='Service Non Stock'
order by Products.CreatedOn desc
";

                SqlCommand cmd = new SqlCommand(sqlText, VcurrConn);
                cmd.Transaction = Vtransaction;
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

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
            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "GetProductInfo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "GetProductInfo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion
            #region finally

            finally
            {

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}


                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dataTable;
        }

        public string CheckCellValue(string cellValue)
        {
            string results = "Y";
            if (string.IsNullOrEmpty(cellValue))
            {
                results = "N";
                return results;
            }
            else
            {
                return results;
            }

        }

        public string CheckNumericValue(string input)
        {
            string result = "N";
            string amtValue = input;

            #region try

            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    if (input.StartsWith("$"))
                    {
                        amtValue = input.Substring(1).ToString();
                    }

                    Convert.ToDecimal(amtValue);
                    result = "Y";
                }

                return result;
            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                //////FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);

                FileLogger.Log("CommonImportDAL", "CheckNumericValue", ex.ToString());

                return result;

            }
            #endregion Catch
        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindCurrencyRateFromBDTForBureau(string currencyId, string convertionDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string bdtId = "";
            string sqlText = "";

            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction


                #region Validation

                if (string.IsNullOrEmpty(currencyId))
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Invalid Currency Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode='BDT'";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency 'BDT' not in database");

                }
                else
                {
                    bdtId = obj1.ToString();

                }

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyRate ";
                sqlText += " from CurrencyConversion";
                sqlText += " where ";
                sqlText += " CurrencyCodeFrom='" + currencyId + "' ";
                sqlText += " and  CurrencyCodeTo='" + bdtId + "' ";
                sqlText += " and  ConversionDate<='" + convertionDate + "' ";
                sqlText += " order by conversionDate desc ";

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                object obj2 = cmd2.ExecuteScalar();
                if (obj2 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency '(" + currencyId + ")' not in database");

                }
                else
                {
                    retResults = obj2.ToString();

                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyRateFromBDTForBureau", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyRateFromBDTForBureau", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindSaleInvoiceNo(string customerId, string invoiceNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(customerId))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " Select SalesInvoiceNo";
                sqlText += " from BureauSalesInvoiceDetails";
                sqlText += " where ";
                sqlText += " CustomerId='" + customerId + "' ";
                sqlText += " and InvoiceName='" + invoiceNo + "' ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Sale information not in database");
                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindSaleInvoiceNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindSaleInvoiceNo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public bool CheckDepositType(string DepositType, SysDBInfoVMTemp connVM = null)
        {
            bool result = true;

            if (string.IsNullOrEmpty(DepositType))
            {
                result = false;
            }
            else
            {
                if (DepositType.ToUpper() == "CASH" || DepositType.ToUpper() == "CHEQUE" || DepositType.ToUpper() == "BEFTN" || DepositType.ToUpper() == "NPSB" || DepositType.ToUpper() == "NOTDEPOSITED")
                {
                    result = true;
                }
                //else if (DepositType.ToUpper() == "CHEQUE")
                //{
                //    result = true;
                //}
                else
                {
                    result = false;
                }
            }

            return result;
        }

        //currConn to VcurrConn 25-Aug-2020
        public string CheckBankID(string BankName, string BranchName, string AccountNumber, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            string result = "";


            DataTable dataTable = new DataTable("Bank");

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                sqlText = @"SELECT 
                            BankID,
                            isnull(BankName,'N/A')BankName,
                            isnull(BranchName,'N/A')BranchName,
                            isnull(AccountNumber,'N/A')AccountNumber
                            FROM dbo.BankInformations
                            WHERE (BankName LIKE '%' + @BankName + '%' OR @BankName IS NULL)
                            AND (BranchName LIKE '%' + @BranchName	 + '%' OR @BranchName	 IS NULL) 
                            AND (AccountNumber LIKE '%' + @AccountNumber	 + '%' OR @AccountNumber	 IS NULL)
                            order by BankName"
                            ;

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.Transaction = transaction;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;

                if (!objCommBankInformation.Parameters.Contains("@BankName"))
                { objCommBankInformation.Parameters.AddWithValue("@BankName", BankName); }
                else { objCommBankInformation.Parameters["@BankName"].Value = BankName; }
                if (!objCommBankInformation.Parameters.Contains("@BranchName"))
                { objCommBankInformation.Parameters.AddWithValue("@BranchName", BranchName); }
                else { objCommBankInformation.Parameters["@BranchName"].Value = BranchName; }
                if (!objCommBankInformation.Parameters.Contains("@AccountNumber"))
                { objCommBankInformation.Parameters.AddWithValue("@AccountNumber", AccountNumber); }
                else { objCommBankInformation.Parameters["@AccountNumber"].Value = AccountNumber; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);

                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    result = dataTable.Rows[0]["BankID"].ToString();
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckBankID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckBankID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            return result;

        }

        #endregion

        #region unused
        public string PostCheck(string Post)
        {
            string result = "";
            if (string.IsNullOrEmpty(Post))
            {
                throw new ArgumentNullException("PostCheck", "Please input Y/N for Post Information");
            }
            if (Post != "Y")
            {
                if (Post != "N")
                {
                    throw new ArgumentNullException("PostCheck", "Please input Y/N for Post Information");
                }
                else
                {
                    result = Post;
                }
            }
            else
            {
                result = Post;
            }
            return result;
        }
        public string YesNoCheck(string YesNo)
        {
            string result = "";
            if (string.IsNullOrEmpty(YesNo))
            {
                throw new ArgumentNullException("YesNoCheck", "Please input Y/N for Post Information");
            }
            if (YesNo != "Y")
            {
                if (YesNo != "N")
                {
                    throw new ArgumentNullException("YesNoCheck", "Please input Y/N for Post Information");
                }
                else
                {
                    result = YesNo;
                }
            }
            else
            {
                result = YesNo;
            }
            return result;
        }
        public void CheckNumeric(string input)
        {
            try
            {
                if (input != string.Empty)
                {

                    if (Convert.ToDecimal(input) < 0)
                    {
                        //throw new ArgumentNullException("CheckNumeric", "Please input number Information");

                    }
                }


            }
            #region Catch
            catch (Exception ex)
            {
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                FileLogger.Log("CommonImportDAL", "CheckNumeric", ex.ToString());

            }
            #endregion Catch
        }
        public string NonStockCheck(string NonStock)
        {
            string result = "";
            if (string.IsNullOrEmpty(NonStock))
            {
                throw new ArgumentNullException("NonStockCheck", "Please input Y/N for NonStock Information");
            }
            if (NonStock != "Y")
            {
                if (NonStock != "N")
                {
                    throw new ArgumentNullException("NonStockCheck", "Please input Y/N for NonStock Information");
                }
                else
                {
                    result = NonStock;
                }
            }
            else
            {
                result = NonStock;
            }
            return result;
        }
        public string SaleDetailTypeCheck(string Type)
        {
            string result = "";
            if (string.IsNullOrEmpty(Type))
            {
                throw new ArgumentNullException("SaleTypeCheck", "Please input VAT/Non VAT for Post Information");
            }
            if (Type != "VAT")
            {
                if (Type != "Non VAT")
                {
                    throw new ArgumentNullException("SaleTypeCheck", "Please input VAT/Non VAT for Post Information");
                }
                else
                {
                    result = Type;
                }
            }
            else
            {
                result = Type;
            }
            return result;
        }
        public string SaleMasterTypeCheck(string Type)
        {
            string result = "";
            if (string.IsNullOrEmpty(Type))
            {
                throw new ArgumentNullException("SaleMasterTypeCheck", "Please input New/Debit/Credit VAT for Post Information");
            }
            if (Type != "New")
            {
                if (Type != "Debit")
                {
                    if (Type != "Credit")
                    {
                        throw new ArgumentNullException("SaleMasterTypeCheck",
                                                        "Please input New/Debit/Credit VAT for Post Information");

                    }
                    else
                    {
                        result = Type;
                    }
                }
                else
                {
                    result = Type;
                }
            }
            else
            {
                result = Type;
            }
            return result;
        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindTenderId(string TenderId, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string retResults = "";
            string sqlText = "";

            //SqlConnection vcurrConn = currConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try
            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{

                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation

                if (string.IsNullOrEmpty(TenderId))
                {
                    //throw new ArgumentNullException("FindTenderId", "Invalid Tender Information");
                    return sqlText;
                }

                #endregion Validation

                #region Find

                if (TenderId == "0")
                {
                    retResults = TenderId;
                }
                else
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 TenderId ";
                    sqlText += " from TenderHeaders";
                    sqlText += " where ";
                    sqlText += " TenderId='" + TenderId + "' ";

                    SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                    cmd1.Transaction = transaction;
                    object obj1 = cmd1.ExecuteScalar();
                    if (obj1 == null)
                    {
                        throw new ArgumentNullException("FindTenderId", "Tender '(" + TenderId + ")' not in database");
                    }
                    else
                    {
                        retResults = obj1.ToString();
                    }
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindTenderId", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////throw new ArgumentNullException("", sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindTenderId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////throw new ArgumentNullException("", sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();
                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results
            return retResults;
            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string CheckPreInvoiceID(string InvoiceID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(InvoiceID))
                {
                    return results;
                }

                if (InvoiceID == "0")
                {
                    return results;
                }


                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //}

                #endregion open connection and transaction

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

                #region Validation



                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 SalesInvoiceNo";
                sqlText += " from SalesInvoiceHeaders";
                sqlText += " where ";
                sqlText += " SalesInvoiceNo='" + InvoiceID + "' ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Previous Invoice ID", "Previous invoice no. '(" + InvoiceID + ")' is not in database");

                }
                else
                {
                    results = InvoiceID;
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckPreInvoiceID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckPreInvoiceID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results

            return results;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string CheckTenderID(string TenderID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(TenderID))
                {
                    return results;
                }

                if (TenderID == "0")
                {
                    return results;
                }


                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //        Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation



                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 TenderId";
                sqlText += " from TenderHeaders";
                sqlText += " where ";
                sqlText += " TenderId='" + TenderID + "' ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Tender ID", "Tender id '(" + TenderID + ")' is not in database");

                }
                else
                {
                    results = TenderID;
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckTenderID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckTenderID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        #endregion

        #region need to parameterize

        public string FindCustomerId(string customerName, string customerCode, SqlConnection VcurrConn, SqlTransaction Vtransaction
            , bool AutoSave = false, string customerGroup = null, int branchId = 1, SysDBInfoVMTemp connVM = null
            , string BIN = "-", bool onlyCode = false, string address = "-", bool noValidation = false, bool checkWithAddress = false, bool skipCRAT = false)
        {
            #region Initializ
            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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

                bool customerAutoCode = new CommonDAL().settingValue("AutoCode", "Customer", null, currConn, transaction) == "Y";

                #region Validation
                if (string.IsNullOrEmpty(customerName) && string.IsNullOrEmpty(customerCode))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }
                CommonDAL commonDal = new CommonDAL();

                string vcustomerCode = customerCode;
                string vcustomerName = customerName;

                if (customerCode == "0" || customerCode == "-")
                {
                    customerCode = "";
                }
                if (customerName == "-")
                {
                    customerName = "";
                }
                bool AutoCode = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Customer", null, currConn, transaction) == "Y");

                #endregion Validation

                #region Find

                object obj1 = null;

                if (!noValidation)
                {
                    if (!string.IsNullOrWhiteSpace(customerCode))
                    {

                        sqlText = " ";
                        sqlText = " SELECT top 1 CustomerID ";
                        sqlText += " from Customers";
                        sqlText += " where ";
                        sqlText += " CustomerCode=@customerCode";

                        SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                        cmd1.Transaction = transaction;
                        cmd1.Parameters.AddWithValueAndNullHandle("@customerCode", customerCode);
                        obj1 = cmd1.ExecuteScalar();
                    }

                }

                if (obj1 == null)
                {
                    object obj2 = null;
                    if (!onlyCode)
                    {
                        if (!string.IsNullOrWhiteSpace(customerName))
                        {
                            sqlText = " ";
                            sqlText = " SELECT top 1 CustomerID ";
                            sqlText += " from Customers";
                            sqlText += " where ";
                            sqlText += " CustomerName=@customerName  ";

                            if (skipCRAT)
                            {
                                sqlText += " and  BranchId != 2  ";

                            }

                            SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                            cmd2.Transaction = transaction;
                            cmd2.Parameters.AddWithValueAndNullHandle("@customerName", customerName);
                            obj2 = cmd2.ExecuteScalar();
                        }
                    }

                    if (checkWithAddress)
                    {
                        if (!string.IsNullOrWhiteSpace(customerName))
                        {
                            sqlText = " ";
                            sqlText = " SELECT top 1 CustomerID ";
                            sqlText += " from Customers";
                            sqlText += " where ";
                            sqlText += " CustomerName=@customerName  ";
                            sqlText += "and Address1=@addrs  ";

                            SqlCommand checkCommand = new SqlCommand(sqlText, currConn);
                            checkCommand.Transaction = transaction;
                            checkCommand.Parameters.AddWithValueAndNullHandle("@customerName", customerName);
                            checkCommand.Parameters.AddWithValueAndNullHandle("@addrs", address);
                            obj2 = checkCommand.ExecuteScalar();
                        }
                    }


                    if (obj2 == null)
                    {
                        if (AutoSave)
                        {

                            if (string.IsNullOrEmpty(customerName))
                            {
                                throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information customer Code : " + vcustomerCode + " customer Name : " + vcustomerName);
                            }

                            if (!AutoCode)
                            {
                                if (string.IsNullOrEmpty(customerCode))
                                {
                                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information customer Code : " + vcustomerCode + " customer Name : " + vcustomerName);
                                }
                            }


                            // Insert into customer

                            CustomerVM vm = new CustomerVM();
                            vm.CustomerName = customerName;
                            vm.CustomerCode = customerCode;
                            vm.Address1 = address;
                            vm.Address2 = "-";
                            vm.Address3 = "-";
                            vm.City = "-";
                            vm.TelephoneNo = "-";
                            vm.FaxNo = "-";
                            vm.Email = "-";
                            vm.StartDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                            vm.ContactPerson = "-";
                            vm.ContactPersonDesignation = "-";
                            vm.ContactPersonTelephone = "-";
                            vm.ContactPersonEmail = "-";
                            vm.TINNo = "-";
                            vm.VATRegistrationNo = BIN;
                            vm.Comments = "-";
                            vm.BusinessType = "-";
                            vm.BusinessCode = "-";
                            vm.IsVDSWithHolder = "-";
                            vm.ShortName = customerName;

                            #region getting default group id

                            string defaultGroupName = "";

                            if (!string.IsNullOrEmpty(customerGroup))
                            {
                                defaultGroupName = customerGroup;
                            }
                            else
                            {
                                defaultGroupName = commonDal.settingValue("AutoSave", "DefaultCustomerGroup", null, currConn, transaction);
                            }

                            sqlText = " ";
                            sqlText = " SELECT top 1 CustomerGroupID ";
                            sqlText += " from CustomerGroups";
                            sqlText += " where ";
                            sqlText += " CustomerGroupName=@CustomerGroupName  ";

                            SqlCommand cmd3 = new SqlCommand(sqlText, currConn);
                            cmd3.Transaction = transaction;
                            cmd3.Parameters.AddWithValueAndNullHandle("@CustomerGroupName", defaultGroupName);
                            var cmd3result = cmd3.ExecuteScalar();
                            if (cmd3result == null)
                            {

                                if (!string.IsNullOrEmpty(customerGroup))
                                {
                                    var groupDal = new CustomerGroupDAL();

                                    var groupVM = new CustomerGroupVM();

                                    groupVM.CustomerGroupName = defaultGroupName;
                                    groupVM.CustomerGroupDescription = "-";
                                    groupVM.ActiveStatus = "Y";
                                    groupVM.GroupType = "LOCAL";
                                    groupVM.Comments = "-";
                                    groupVM.Info1 = "-";
                                    groupVM.Info2 = "-";
                                    groupVM.Info3 = "-";
                                    groupVM.Info4 = "-";
                                    groupVM.Info5 = "-";
                                    groupVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    var results = groupDal.InsertToCustomerGroupNew(groupVM, null, currConn, transaction);

                                    cmd3result = results[2];
                                }
                                else
                                {
                                    throw new Exception("Could not find Customer group " + defaultGroupName);

                                }


                            }
                            string customerGroupId = cmd3result.ToString();
                            #endregion

                            vm.CustomerGroupID = customerGroupId;
                            vm.ActiveStatus = "Y";
                            vm.BranchId = branchId;
                            CustomerDAL _cdal = new CustomerDAL();
                            string[] result = _cdal.InsertToCustomerNew(vm, AutoSave, currConn, transaction);

                            if (result[0].ToLower() == "fail")
                            {
                                throw new ArgumentNullException("FindCustomerId", "Customer '(" + customerName + ")' failed to insert.");
                            }

                            retResults = result[2];
                        }
                        else
                        {
                            throw new ArgumentNullException("FindCustomerId", "Customer '(" + customerName + ")' not in database");
                        }
                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("CommonImportDAL", "FindCustomerId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindBranchId(string branchCode, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(branchCode))
                {
                    throw new ArgumentNullException("FindBranchId", "Invalid Branch Information");
                }
                CommonDAL commonDal = new CommonDAL();

                #endregion Validation

                #region Find
                sqlText = " ";
                sqlText = " SELECT top 1 BranchId ";
                sqlText += " from BranchProfiles";
                sqlText += " where ";
                sqlText += " BranchCode=@BranchCode";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@BranchCode", branchCode);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = obj1.ToString();
                }
                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindBranchId", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindBranchId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindVendorId(string vendorName, string vendorCode, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool AutoSave = false, SysDBInfoVMTemp connVM = null, int branchId = 1)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region Try

            try
            {

                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(vendorName) && string.IsNullOrEmpty(vendorCode))
                {
                    throw new ArgumentNullException("FindVendorId", "Invalid Vendor Information");
                }
                string companyCode = new CommonDAL().settingValue("CompanyCode", "Code", connVM);
                string vVendorCode = vendorCode;
                string vVendorName = vendorName;

                if (vendorCode == "0" || vendorCode == "-")
                {
                    vendorCode = "";
                }
                if (vendorName == "-")
                {
                    vendorName = "";
                }
                if (companyCode.ToLower() == "bsl")
                {
                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        vendorCode = "N/A";
                    }
                }

                #endregion Validation

                #region Find

                object obj1 = null;
                object obj2 = null;

                #region Find by vendorCode

                if (!string.IsNullOrEmpty(vendorCode))
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 VendorID ";
                    sqlText += " from Vendors";
                    sqlText += " where ";
                    sqlText += " VendorCode=@vendorCode ";

                    SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                    cmd1.Transaction = transaction;
                    cmd1.Parameters.AddWithValueAndNullHandle("@vendorCode", vendorCode);

                    //////object obj1 = cmd1.ExecuteScalar();
                    obj1 = cmd1.ExecuteScalar();
                }

                #endregion

                if (obj1 == null)
                {
                    #region Find by vendorName
                    if (!string.IsNullOrEmpty(vendorName))
                    {
                        sqlText = " ";
                        sqlText = " SELECT top 1 VendorID ";
                        sqlText += " from Vendors";
                        sqlText += " where ";
                        sqlText += " VendorName=@vendorName ";

                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Transaction = transaction;
                        cmd2.Parameters.AddWithValueAndNullHandle("@vendorName", vendorName);

                        //////object obj2 = cmd2.ExecuteScalar();
                        obj2 = cmd2.ExecuteScalar();
                    }

                    #endregion

                    if (obj2 == null)
                    {
                        #region Find by vendorName

                        if (AutoSave)
                        {

                            bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Vendor") == "Y" ? true : false);
                            if (Auto == true)
                            {
                                if (string.IsNullOrEmpty(vendorName))
                                {
                                    throw new ArgumentNullException("FindVendorId", "Invalid Vendor Information Vendor Code : " + vVendorCode + " Vendor Name : " + vVendorName);
                                }

                            }

                            else
                            {
                                if (string.IsNullOrEmpty(vendorName) || string.IsNullOrEmpty(vendorCode))
                                {
                                    throw new ArgumentNullException("FindVendorId", "Invalid Vendor Information Vendor Code : " + vVendorCode + " Vendor Name : " + vVendorName);
                                }
                            }


                            //insert new vendor
                            VendorVM vm = new VendorVM();
                            vm.VendorName = vendorName;
                            vm.VendorCode = vendorCode;

                            #region getting default group id

                            string defaultGroupName = commonDal.settingValue("AutoSave", "DefaultVendorGroup");
                            sqlText = " ";
                            sqlText = " SELECT top 1 VendorGroupID ";
                            sqlText += " from VendorGroups";
                            sqlText += " where ";
                            sqlText += " VendorGroupName=@VendorGroupName  ";

                            SqlCommand cmd3 = new SqlCommand(sqlText, currConn);
                            cmd3.Transaction = transaction;
                            cmd3.Parameters.AddWithValueAndNullHandle("@VendorGroupName", defaultGroupName);
                            var cmd3result = cmd3.ExecuteScalar();
                            if (cmd3result == null)
                            {
                                throw new Exception("Could not find Vendor group " + defaultGroupName);
                            }
                            string GroupId = cmd3result.ToString();

                            #endregion

                            vm.VendorGroupID = GroupId;
                            vm.ActiveStatus = "Y";
                            vm.ShortName = vendorName;
                            vm.BranchId = branchId;
                            VendorDAL _vDal = new VendorDAL();
                            var result = _vDal.InsertToVendorNewSQL(vm, AutoSave, currConn, transaction);
                            retResults = result[2];

                        }
                        else
                        {
                            ////throw new ArgumentNullException("FindVendorId", "Vendor '(" + vendorName + ")' not in database");
                            throw new ArgumentNullException("FindVendorId", "Vendor '(" + vVendorName + ")' not in database");
                        }

                        #endregion

                    }
                    else
                    {
                        retResults = obj2.ToString();
                    }

                }
                else
                {
                    retResults = obj1.ToString();
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindVendorId", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindVendorId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindItemId(string productName, string productCode, SqlConnection VcurrConn,
            SqlTransaction Vtransaction, bool AutoSave = false, string uom = "-", int branchId = 1, decimal vatRate = 0,
            decimal NbrPrice = 0, SysDBInfoVMTemp connVM = null, string Product_Group = "-", bool ProductNameCheck = false)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            bool ProductAutoSave = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                #endregion

                CommonDAL commonDal = new CommonDAL();
                ProductAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleProduct", null, currConn, transaction) == "Y");

                #region Validation
                if (string.IsNullOrEmpty(productName) && string.IsNullOrEmpty(productCode))
                {
                    throw new ArgumentNullException("FindItemId", "Invalid Item Information");
                }

                string vProductCode = productCode;
                string vProductName = productName;

                //if (productCode == "0" || productCode == "-")
                //{
                //    productCode = "";
                //}
                //if (productName == "-")
                //{
                //    productName = "";
                //}

                #endregion Validation

                #region Find

                object obj1 = null;
                object obj2 = null;

                #region Find by productCode

                if (!string.IsNullOrEmpty(productCode))
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 ItemNo ";
                    sqlText += " from Products";
                    sqlText += " where ";
                    sqlText += " ProductCode=@productCode ";

                    SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                    cmd1.Transaction = transaction;
                    cmd1.Parameters.AddWithValueAndNullHandle("@productCode", productCode);
                    //////object obj1 = cmd1.ExecuteScalar();
                    obj1 = cmd1.ExecuteScalar();
                }

                #endregion

                if (obj1 == null)
                {
                    #region Find by productName

                    if (ProductNameCheck)
                    {

                        if (!string.IsNullOrEmpty(productName))
                        {
                            sqlText = " ";
                            sqlText = " SELECT top 1 ItemNo ";
                            sqlText += " from Products";
                            sqlText += " where ";
                            sqlText += " ProductName=@productName ";

                            SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                            cmd2.Transaction = transaction;
                            cmd2.Parameters.AddWithValueAndNullHandle("@productName", productName);

                            //////object obj2 = cmd2.ExecuteScalar();
                            obj2 = cmd2.ExecuteScalar();

                        }

                    }

                    #endregion

                    if (obj2 == null)
                    {
                        #region Product Save

                        //insert product
                        if (AutoSave)
                        {

                            //if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(productCode))
                            //{
                            //    throw new ArgumentNullException("FindItemId", "Invalid Item Information Product Code : " + vProductCode + " product Name : " + vProductName);
                            //}

                            ProductVM vm = new ProductVM();
                            vm.ProductName = productName;
                            vm.ProductCode = productCode;

                            #region getting default group id

                            string defaultGroupName = commonDal.settingValue("AutoSave", "DefaultProductCategory", null, currConn, transaction);

                            if (Product_Group != "-" && !string.IsNullOrEmpty(Product_Group))
                            {
                                defaultGroupName = Product_Group;
                            }

                            sqlText = " ";
                            sqlText = " SELECT top 1 CategoryID ";
                            sqlText += " from ProductCategories";
                            sqlText += " where ";
                            sqlText += " CategoryName=@CategoryName  ";

                            SqlCommand cmd3 = new SqlCommand(sqlText, currConn);
                            cmd3.Transaction = transaction;
                            cmd3.Parameters.AddWithValueAndNullHandle("@CategoryName", defaultGroupName);
                            var cmd3result = cmd3.ExecuteScalar();
                            if (cmd3result == null)
                            {
                                throw new Exception("Could not find Product group " + defaultGroupName + " " + productName);
                            }
                            string GroupId = cmd3result.ToString();
                            #endregion
                            vm.CategoryID = GroupId;
                            vm.ActiveStatus = "Y";
                            vm.UOM = uom;
                            vm.ProductCode = productCode;
                            vm.BranchId = branchId;
                            vm.IsTransport = "N";
                            vm.TDSCode = "NA";
                            vm.IsZeroVAT = "N";
                            vm.Banderol = "N";
                            vm.TollProduct = "N";
                            vm.Trading = "N";
                            vm.NonStock = "N";
                            vm.HSCodeNo = "NA";
                            vm.VATRate2 = 0;
                            vm.Comments = "NA";
                            //vm.IsVDS = "N";
                            vm.ProductDescription = "NA";
                            vm.VATRate = vatRate;
                            vm.NBRPrice = NbrPrice;
                            vm.OpeningDate = "01/01/1901";
                            vm.ShortName = productName;

                            ProductDAL _pDal = new ProductDAL();
                            var result = _pDal.InsertToProduct(vm, null, null, true, currConn, transaction, connVM);

                            retResults = result[2];
                        }
                        else
                        {
                            ////throw new ArgumentNullException("FindItemId", "Item '(" + productName + " " + productCode + ")' not in database");
                            throw new ArgumentNullException("FindItemId", "Item '(" + vProductName + " " + vProductCode + ")' not in database");

                        }

                        #endregion

                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit


            }
            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindItemId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindItemId_(string productName, string productCode, SqlConnection VcurrConn,
            SqlTransaction Vtransaction, bool AutoSave = false, string uom = "-", int branchId = 1, decimal vatRate = 0,
            decimal NbrPrice = 0, SysDBInfoVMTemp connVM = null, string Product_Group = "-")
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            bool ProductAutoSave = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                #endregion

                CommonDAL commonDal = new CommonDAL();
                ProductAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleProduct", null, currConn, transaction) == "Y");

                #region Validation
                if (string.IsNullOrEmpty(productName) && string.IsNullOrEmpty(productCode))
                {
                    throw new ArgumentNullException("FindItemId", "Invalid Item Information");
                }

                string vProductCode = productCode;
                string vProductName = productName;

                //if (productCode == "0" || productCode == "-")
                //{
                //    productCode = "";
                //}
                //if (productName == "-")
                //{
                //    productName = "";
                //}

                #endregion Validation

                #region Find

                object obj1 = null;
                object obj2 = null;

                #region Find by productCode

                if (!string.IsNullOrEmpty(productCode))
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 ItemNo ";
                    sqlText += " from Products";
                    sqlText += " where ";
                    sqlText += " ProductCode=@productCode ";

                    SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                    cmd1.Transaction = transaction;
                    cmd1.Parameters.AddWithValueAndNullHandle("@productCode", productCode);
                    //////object obj1 = cmd1.ExecuteScalar();
                    obj1 = cmd1.ExecuteScalar();
                }

                #endregion

                if (obj1 == null)
                {
                    //#region Find by productName

                    //if (!string.IsNullOrEmpty(productName))
                    //{
                    //    sqlText = " ";
                    //    sqlText = " SELECT top 1 ItemNo ";
                    //    sqlText += " from Products";
                    //    sqlText += " where ";
                    //    sqlText += " ProductName=@productName ";

                    //    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    //    cmd2.Transaction = transaction;
                    //    cmd2.Parameters.AddWithValueAndNullHandle("@productName", productName);

                    //    //////object obj2 = cmd2.ExecuteScalar();
                    //    obj2 = cmd2.ExecuteScalar();

                    //}

                    //#endregion


                    if (obj2 == null)
                    {
                        #region Product Save

                        //insert product
                        if (AutoSave)
                        {

                            //if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(productCode))
                            //{
                            //    throw new ArgumentNullException("FindItemId", "Invalid Item Information Product Code : " + vProductCode + " product Name : " + vProductName);
                            //}

                            ProductVM vm = new ProductVM();
                            vm.ProductName = productName;
                            vm.ProductCode = productCode;

                            #region getting default group id

                            string defaultGroupName = commonDal.settingValue("AutoSave", "DefaultProductCategory", null, currConn, transaction);

                            if (Product_Group != "-" && !string.IsNullOrEmpty(Product_Group))
                            {
                                defaultGroupName = Product_Group;
                            }

                            sqlText = " ";
                            sqlText = " SELECT top 1 CategoryID ";
                            sqlText += " from ProductCategories";
                            sqlText += " where ";
                            sqlText += " CategoryName=@CategoryName  ";

                            SqlCommand cmd3 = new SqlCommand(sqlText, currConn);
                            cmd3.Transaction = transaction;
                            cmd3.Parameters.AddWithValueAndNullHandle("@CategoryName", defaultGroupName);
                            var cmd3result = cmd3.ExecuteScalar();
                            if (cmd3result == null)
                            {
                                throw new Exception("Could not find Product group " + defaultGroupName + " " + productName);
                            }
                            string GroupId = cmd3result.ToString();
                            #endregion
                            vm.CategoryID = GroupId;
                            vm.ActiveStatus = "Y";
                            vm.UOM = uom;
                            vm.ProductCode = productCode;
                            vm.BranchId = branchId;
                            vm.IsTransport = "N";
                            vm.TDSCode = "NA";
                            vm.IsZeroVAT = "N";
                            vm.Banderol = "N";
                            vm.TollProduct = "N";
                            vm.Trading = "N";
                            vm.NonStock = "N";
                            vm.HSCodeNo = "NA";
                            vm.VATRate2 = 0;
                            vm.Comments = "NA";
                            //vm.IsVDS = "N";
                            vm.ProductDescription = "NA";
                            vm.VATRate = vatRate;
                            vm.NBRPrice = NbrPrice;
                            vm.OpeningDate = "01/01/1901";
                            vm.ShortName = productName;

                            ProductDAL _pDal = new ProductDAL();
                            var result = _pDal.InsertToProduct(vm, null, null, true, currConn, transaction, connVM);

                            retResults = result[2];
                        }
                        else
                        {
                            ////throw new ArgumentNullException("FindItemId", "Item '(" + productName + " " + productCode + ")' not in database");
                            throw new ArgumentNullException("FindItemId", "Item '(" + vProductName + " " + vProductCode + ")' not in database");

                        }

                        #endregion

                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit


            }
            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindItemId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Results

            return retResults;

            #endregion

        }


        public string FindNestleItemId(string productName, string BanglaName, string ProductType, SqlConnection VcurrConn,
           SqlTransaction Vtransaction, bool AutoSave = false, string uom = "-", int branchId = 1, decimal vatRate = 0,
           decimal NbrPrice = 0, SysDBInfoVMTemp connVM = null, string Product_Group = "-")
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            string NewProductName = "";
            //SqlConnection vcurrConn = VcurrConn;

            bool ProductAutoSave = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                #endregion

                CommonDAL commonDal = new CommonDAL();

                #region Validation
                if (string.IsNullOrEmpty(productName) && string.IsNullOrEmpty(productName))
                {
                    throw new ArgumentNullException("FindItemId", "Invalid Item Information");
                }
                if (ProductType == "E")
                {
                    NewProductName = productName + '~' + "E";

                }
                else if (ProductType == "D")
                {
                    NewProductName = productName + '~' + "D";
                }

                else
                {
                    NewProductName = productName + '~' + "C";
                }

                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 ItemNo ";
                sqlText += " from Products";
                sqlText += " where ";
                sqlText += " ProductName=@productName ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@productName", NewProductName);
                object obj1 = cmd1.ExecuteScalar();

                if (obj1 == null)
                {
                    //insert product

                    ProductVM vm = new ProductVM();
                    vm.ProductName = productName;
                    //vm.ProductCode = productCode;

                    #region getting default group id

                    string defaultGroupName = "NonInventory";

                    sqlText = " ";
                    sqlText = " SELECT top 1 CategoryID ";
                    sqlText += " from ProductCategories";
                    sqlText += " where ";
                    sqlText += " CategoryName=@CategoryName  ";

                    SqlCommand cmd3 = new SqlCommand(sqlText, currConn);
                    cmd3.Transaction = transaction;
                    cmd3.Parameters.AddWithValueAndNullHandle("@CategoryName", defaultGroupName);
                    var cmd3result = cmd3.ExecuteScalar();
                    if (cmd3result == null)
                    {
                        throw new Exception("Could not find Product group " + defaultGroupName + " " + productName);
                    }
                    string GroupId = cmd3result.ToString();
                    #endregion
                    vm.CategoryID = GroupId;
                    vm.ActiveStatus = "N";
                    vm.UOM = uom;
                    //vm.ProductCode = productCode;
                    vm.BranchId = branchId;
                    vm.IsTransport = "N";
                    vm.TDSCode = "NA";
                    vm.IsZeroVAT = "N";
                    vm.Banderol = "N";
                    vm.TollProduct = "N";
                    vm.Trading = "N";
                    vm.NonStock = "N";
                    vm.HSCodeNo = "NA";
                    vm.VATRate2 = 0;
                    vm.Comments = "NA";
                    //vm.IsVDS = "N";
                    vm.ProductDescription = "NA";
                    vm.VATRate = vatRate;
                    vm.NBRPrice = NbrPrice;
                    vm.OpeningDate = "01/01/1901";
                    vm.ShortName = BanglaName;
                    vm.ProductName = NewProductName;

                    ProductDAL _pDal = new ProductDAL();
                    var result = _pDal.InsertToProduct(vm, null, null, true, currConn, transaction, null, true);

                    retResults = result[2];

                }

                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit


            }
            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindNestleItemId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020   ok
        public string FindUOMn(string ItemNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindUOMn", "Invalid Item Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 uom ";
                sqlText += " from Products";
                sqlText += " where ";
                sqlText += " ItemNo=@ItemNo ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@ItemNo", ItemNo);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindUOMn", "Item '(" + ItemNo + ")' not in database");

                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindUOMn", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindUOMn", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindLastNBRPrice(string ItemNo, string VATName, string RequestDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Item Information");
                }
                else if (Convert.ToDateTime(RequestDate) == DateTime.MinValue || Convert.ToDateTime(RequestDate) == DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Request Date ");
                }
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid VATName ");
                }

                #endregion Validation

                #region FindLastNBRPrice
                ProductDAL productDal = new ProductDAL();
                retResults = productDal.GetLastNBRPriceFromBOM_VatName(ItemNo, VATName, RequestDate,
                                            VcurrConn, Vtransaction).ToString();


                #endregion FindLastNBRPrice


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindLastNBRPrice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindLastNBRPrice", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }


        public string FindAvgPrice(string ItemNo, string RequestDate, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            #endregion

            #region Try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Item Information");
                }
                else if (Convert.ToDateTime(RequestDate) == DateTime.MinValue || Convert.ToDateTime(RequestDate) == DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Request Date ");
                }

                #endregion Validation
                #region FindAvgPrice
                ProductDAL productDal = new ProductDAL();

                //retResults = productDal.AvgPrice(ItemNo, RequestDate, null, null).ToString();
                //retResults = productDal.AvgPriceNew(ItemNo, RequestDate, null, null).ToString();

                DataTable priceData = productDal.AvgPriceNew(ItemNo, RequestDate, null, null, false, true, true, true, null, UserId);

                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                if (quantity > 0)
                {
                    retResults = (amount / quantity).ToString();
                }
                else
                {
                    retResults = "0";
                }



                #endregion FindAvgPrice
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindAvgPrice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindAvgPrice", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindUOMc(string uomFrom, string uomTo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null, string itemNo = "", string productName = "", string productCode = "")
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                if (uomFrom.ToLower() == uomTo.ToLower())
                {
                    retResults = "1";
                    return retResults;
                }

                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(uomFrom))
                {
                    throw new ArgumentNullException("FindUOMc", "Invalid UOM From " + uomFrom);
                }
                if (string.IsNullOrEmpty(uomTo))
                {
                    throw new ArgumentNullException("FindUOMc", "Invalid UOM To");
                }

                #endregion Validation

                #region Find UOM From

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMFrom=@uomFrom ";

                SqlCommand cmdUOMFrom = new SqlCommand(sqlText, currConn);
                cmdUOMFrom.Transaction = transaction;
                cmdUOMFrom.Parameters.AddWithValueAndNullHandle("@uomFrom", uomFrom);
                object objUOMFrom = cmdUOMFrom.ExecuteScalar();
                if (objUOMFrom == null)
                {
                    retResults = "1";
                    return retResults;
                }

                //throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomFrom + "' from found in conversion");

                #endregion Find UOM From

                #region Find UOM to

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMTo=@uomTo ";

                SqlCommand cmdUOMTo = new SqlCommand(sqlText, currConn);
                cmdUOMTo.Transaction = transaction;
                cmdUOMTo.Parameters.AddWithValueAndNullHandle("@uomTo", uomTo);

                object objUOMTo = cmdUOMTo.ExecuteScalar();
                if (objUOMTo == null)
                    //throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomTo + "', to found in conversion - " + itemNo);
                    throw new Exception("No Conversion Found Between '" + uomFrom + "' and '" + uomTo + "',Product Name: '" + productName + "',Product Code: '" + productCode + "', to found in conversion - " + itemNo);


                #endregion Find UOM to

                #region UOMc

                sqlText = "  ";

                sqlText = " SELECT top 1 u.Convertion FROM UOMs u ";
                sqlText += " where ";
                sqlText += " u.UOMFrom=@uomFrom ";
                sqlText += " and u.UOMTo=@uomTo ";

                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValueAndNullHandle("@uomFrom", uomFrom);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValueAndNullHandle("@uomTo", uomTo);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    throw new ArgumentNullException("GetProductNo", "No conversion found from ='" + uomFrom + "'" +
                                                                    " to '" + uomTo + "'");

                retResults = objItemNo.ToString();
                #endregion UOMc

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindUOMc", "uomFrom : " + uomFrom + "uomTo : " + uomTo + sqlex.ToString() + "\n" + sqlText);

                ////FileLogger.Log("CommonImportDAL", "FindUOMc", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindUOMc", "uomFrom : " + uomFrom + "uomTo : " + uomTo + ex.ToString() + "\n" + sqlText);


                ////FileLogger.Log("CommonImportDAL", "FindUOMc", ex.ToString() + "\n" + sqlText);

                // throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw ex;
            }

            #endregion

            #region finally

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results
            return retResults;



            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindCurrencyId(string currencyCode, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region open connection and transaction

                //if (vcurrConn == null)
                //{

                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(currencyCode))
                {
                    throw new ArgumentNullException("FindCurrencyId", "Invalid Currency Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode=@currencyCode ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@currencyCode", currencyCode);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyId", "Currency '(" + currencyCode + ")' not in database");

                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion


            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyId", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyId", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindCurrencyRateFromBDT(string currencyId, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string bdtId = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(currencyId))
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Invalid Currency Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode='BDT'";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency 'BDT' not in database");

                }
                else
                {
                    bdtId = obj1.ToString();

                }

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyRate ";
                sqlText += " from CurrencyConversion";
                sqlText += " where ";
                sqlText += " CurrencyCodeFrom=@currencyId ";
                sqlText += " and  CurrencyCodeTo='" + bdtId + "' ";


                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValueAndNullHandle("@currencyId", currencyId);
                object obj2 = cmd2.ExecuteScalar();
                if (obj2 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency '(" + currencyId + ")' not in database");

                }
                else
                {
                    retResults = obj2.ToString();

                }
                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyRateFromBDT", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyRateFromBDT", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {

                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindCurrencyRateBDTtoUSD(string currencyId, string convertionDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string bdtId = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(currencyId))
                {
                    throw new ArgumentNullException("FindCurrencyRateFromUSD", "Invalid Currency Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode='BDT'";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromUSD", "Currency 'BDT' not in database");
                }
                else
                {
                    bdtId = obj1.ToString();
                }

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyRate ";
                sqlText += " from CurrencyConversion";
                sqlText += " where ";
                sqlText += " CurrencyCodeFrom=@currencyId ";
                sqlText += " and  CurrencyCodeTo='" + bdtId + "' ";
                sqlText += " and  ConversionDate<=@convertionDate ";
                sqlText += " order by conversionDate desc ";

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValueAndNullHandle("@currencyId", currencyId);
                cmd2.Parameters.AddWithValueAndNullHandle("@convertionDate", convertionDate);
                object obj2 = cmd2.ExecuteScalar();
                if (obj2 == null)
                {
                    throw new ArgumentNullException("FindCurrencyId", "Currency '(" + currencyId + ")' not in database");
                }
                else
                {
                    retResults = obj2.ToString();
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyRateBDTtoUSD", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindCurrencyRateBDTtoUSD", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020   ok
        public string CheckPrePurchaseNo(string PurchaseID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(PurchaseID) || PurchaseID == "-")
                {
                    return results;
                }

                if (PurchaseID == "0")
                {
                    return results;
                }


                #region open connection and transaction

                //if (vcurrConn == null)
                //{

                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //        }
                //    }
                //}

                #endregion open connection and transaction

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

                #region Validation



                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 PurchaseInvoiceNo";
                sqlText += " from PurchaseInvoiceHeaders";
                sqlText += " where ";
                sqlText += " PurchaseInvoiceNo=@PurchaseID ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@PurchaseID", PurchaseID);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Previous Purchase ID", "Previous purchase no. '(" + PurchaseID + ")' is not in database");

                }
                else
                {
                    results = PurchaseID;
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckPrePurchaseNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckPrePurchaseNo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string CheckReceiveReturnID(string ReturnID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(ReturnID))
                {
                    return results;
                }

                if (ReturnID == "0" || ReturnID == "-")
                {
                    return results;
                }


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

                #region Old

                //if (vcurrConn == null)
                //{

                //    if (currConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //        }
                //    }
                //}

                #endregion

                #endregion open connection and transaction


                #region Validation



                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 ReceiveNo";
                sqlText += " from ReceiveHeaders";
                sqlText += " where ";
                sqlText += " ReceiveNo=@ReturnID ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn, transaction);
                cmd1.Parameters.AddWithValueAndNullHandle("@ReturnID", ReturnID);
                object obj1 = cmd1.ExecuteScalar();

                if (obj1 == null)
                {
                    throw new ArgumentNullException("Return ID", "Previous receive no. '(" + ReturnID + ")' is not in database");

                }
                else
                {
                    results = ReturnID;
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckReceiveReturnID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckReceiveReturnID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string CheckIssueReturnID(string ReturnID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(ReturnID))
                {
                    return results;
                }

                if (ReturnID == "0")
                {
                    return results;
                }

                #region old connection

                #region open connection and transaction

                //if (vcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //        Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //    }
                //}

                #endregion open connection and transaction

                #endregion

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation



                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 IssueNo";
                sqlText += " from IssueHeaders";
                sqlText += " where ";
                sqlText += " IssueNo=@ReturnID  ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@ReturnID", ReturnID);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Return ID", "Previous issue no. '(" + ReturnID + ")' is not in database");

                }
                else
                {
                    results = ReturnID;
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckIssueReturnID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckIssueReturnID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public DataTable FindAvgPriceImport(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();
            //SqlConnection vcurrConn = VcurrConn;


            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #endregion

            #region Try

            try
            {


                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Item Information");
                }
                else if (Convert.ToDateTime(tranDate) == DateTime.MinValue || Convert.ToDateTime(tranDate) == DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Request Date ");
                }
                //if (string.IsNullOrEmpty(itemNo))
                //{
                //    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                //}
                //else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                //{
                //    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

                //}
                #endregion Validation

                #region Old connection

                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnection();
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //    Vtransaction = VcurrConn.BeginTransaction("AvgPprice");
                //}

                #endregion open connection and transaction

                #endregion

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
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion ProductExist

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT SUM(isnull(SubTotal,0)) Amount,SUM(isnull(stock,0))Quantity" +
                           " FROM(   ";
                sqlText += "  (SELECT  isnull(OpeningBalance,0) Stock, isnull(p.OpeningTotalCost,0) SubTotal  FROM Products p  WHERE p.ItemNo = '" + itemNo + "')";

                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText += "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('other','Service','ServiceNS','InputService','Trading','TollReceive','TollReceive-WIP','TollReceiveRaw','PurchaseCN')" +
                           "  AND ReceiveDate<=@tranDate " +
                           "AND ItemNo = @itemNo ) ";


                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity," +
                           "isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal   ";
                sqlText += "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') " +
                           "  AND ReceiveDate<=@tranDate " +
                           "AND ItemNo =@itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText += "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('PurchaseReturn','PurchaseDN')  AND ReceiveDate<=@tranDate  " +
                           "AND ItemNo = @itemNo) ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal  " +
                           "  FROM IssueDetails WHERE Post='Y' ";
                sqlText += "  AND IssueDateTime<=@tranDate  and TransactionType IN('Other','TollFinishReceive','Tender','WIP','TollReceive','InputService','InputServiceImport','Trading','TradingTender','ExportTrading','ExportTradingTender','Service','ExportService','InternalIssue','TollIssue')" +
                           "  AND ItemNo = @itemNo )  ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal    " +
                           "FROM IssueDetails WHERE Post='Y' ";
                sqlText += "  AND IssueDateTime<=@tranDate  and TransactionType IN('IssueReturn','ReceiveReturn')  AND ItemNo = @itemNo )  ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   " +
                           " FROM ReceiveDetails WHERE Post='Y'";
                sqlText += "  AND ReceiveDateTime<=@tranDate and TransactionType<>'ReceiveReturn' AND ItemNo = @itemNo )";
                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal    FROM ReceiveDetails WHERE Post='Y'";
                sqlText += "  AND ReceiveDateTime<=@tranDate and TransactionType='ReceiveReturn' AND ItemNo = @itemNo )";
                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT  -isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleNewQuantity,-" +
                           "isnull(sum( SubTotal),0)SubTotal " +
                "  FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' AND InvoiceDateTime<=@tranDate   " +
                           "AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','Credit','TollFinishIssue','ServiceStock','InternalIssue')" +
                           " AND ItemNo = @itemNo )";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,-isnull(sum( CurrencyValue),0)SubTotal " +
                "   FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' AND InvoiceDateTime<=@tranDate   " +
                           "AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') AND ItemNo = @itemNo )";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,isnull(sum( SubTotal),0)SubTotal     FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' AND InvoiceDateTime<=@tranDate   AND TransactionType in( 'Credit') AND ItemNo = @itemNo ) ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (select -isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty,";
                sqlText += "  isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)";
                sqlText += "  from DisposeDetails  LEFT OUTER JOIN ";
                sqlText += "  DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber  where ItemNo=@itemNo  ";
                sqlText += "  AND DisposeDetails.DisposeDate<=@tranDate   AND (DisposeDetails.Post ='Y')    ";
                sqlText += "  and sih.FromStock in ('Y'))  ";
                sqlText += "  ) AS a";


                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValueAndNullHandle("@tranDate", tranDate);
                cmd.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);

                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "No row found ");
                }
                retResults = dataTable;


                #endregion Stock

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindAvgPriceImport", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindAvgPriceImport", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////////throw ex;
            }
            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();
                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public decimal FindLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            CommonDAL cmDal = new CommonDAL();

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnection();
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //        Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //    }
                //}

                #endregion open connection and transaction

                #endregion

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
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction


                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdFindLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPriceFromBOM.Transaction = transaction;
                cmdFindLastNBRPriceFromBOM.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmdFindLastNBRPriceFromBOM.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                cmdFindLastNBRPriceFromBOM.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);

                if (cmdFindLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdFindLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno=@itemNo ";

                    SqlCommand cmdFindLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPriceFromProducts.Transaction = transaction;
                    cmdFindLastNBRPriceFromProducts.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);

                    if (cmdFindLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdFindLastNBRPriceFromProducts.ExecuteScalar();
                        retResults = cmDal.FormatingDecimal(retResults.ToString());

                    }
                }

                #endregion Last Price

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindLastNBRPriceFromBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindLastNBRPriceFromBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }


            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string FindCustGroupID(string custGrpName, string custGrpID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region Old connection

                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn == null)
                //    {
                //        VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //        if (VcurrConn.State != ConnectionState.Open)
                //        {
                //            VcurrConn.Open();
                //            Vtransaction = VcurrConn.BeginTransaction("Import Data");
                //        }
                //    }
                //}

                #endregion open connection and transaction

                #endregion

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
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Data");
                }

                #endregion open connection and transaction

                #region Validation
                if (string.IsNullOrEmpty(custGrpName) && string.IsNullOrEmpty(custGrpID))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }


                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CustomerGroupID ";
                sqlText += " from CustomerGroups";
                sqlText += " where ";
                sqlText += " CustomerGroupName=@custGrpName ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@custGrpName", custGrpName);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 CustomerGroupID ";
                    sqlText += " from CustomerGroups";
                    sqlText += " where ";
                    sqlText += " CustomerGroupID=@custGrpID ";

                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValueAndNullHandle("@custGrpName", custGrpName);

                    object obj2 = cmd2.ExecuteScalar();
                    if (obj2 == null)
                    {
                        throw new ArgumentNullException("FindCustomerId", "Customer Group '(" + custGrpName + ")' not in database");

                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find


                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "FindCustGroupID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "FindCustGroupID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string CheckSaleImportIdExist(string ImportId, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            //SqlConnection vcurrConn = VcurrConn;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(ImportId))
                {
                    return results;
                }

                if (ImportId == "0")
                {
                    return results;
                }

                #region Old connection

                #region open connection and transaction
                //if (vcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //}

                #endregion open connection and transaction

                #endregion

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

                #region Validation

                #endregion Validation

                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 SalesInvoiceNo";
                sqlText += " from SalesInvoiceHeaders";
                sqlText += " where ";
                sqlText += " ImportIDExcel=@ImportId ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValueAndNullHandle("@ImportId", ImportId);
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 != null)
                {
                    results = "Exist";
                }
                else
                {
                    results = "NotExist";
                }

                #endregion Find

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("CommonImportDAL", "CheckSaleImportIdExist", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("CommonImportDAL", "CheckSaleImportIdExist", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                //if (vcurrConn == null)
                //{
                //    if (VcurrConn.State == ConnectionState.Open)
                //    {
                //        VcurrConn.Close();

                //    }
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        #endregion
    }
}
