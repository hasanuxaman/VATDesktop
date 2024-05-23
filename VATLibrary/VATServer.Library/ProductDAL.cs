using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class ProductDAL //:IProduct
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();

        #endregion

        public DataTable SelectAll_Specific(ProductVM vm, string[] conditionFields = null, string[] conditionValues = null, String[] specificColumns = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                dataTable = SelectProductDTAll(conditionFields, conditionValues, currConn, transaction, false, 0, "", "", null, connVM);

                DataView view = new DataView(dataTable);
                dataTable = view.ToTable("Selected", false, specificColumns);

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
                FileLogger.Log("ProductDAL", "SelectAll_Specific", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectAll_Specific", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////////throw ex;
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

        public NavigationVM Product_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region SQL Statement

                #region SQL Text

                sqlText = "";
                sqlText = @"
------declare @ProductCode as varchar(100) = '1007'
";
                if (vm.ButtonName == "Current")
                {
                    #region Current Item

                    sqlText = sqlText + @"
--------------------------------------------------Current--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 ItemNo, ProductCode from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 
and p.IsArchive = 0
and p.ProductCode=@ProductCode

";
                    #endregion
                }
                else if (string.IsNullOrWhiteSpace(vm.Code) || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 ItemNo, ProductCode from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 
and p.IsArchive = 0
and @Filter
order by ProductCode asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Last")
                {

                    #region Last Item

                    sqlText = sqlText + @"
--------------------------------------------------Last--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 ItemNo, ProductCode from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 
and p.IsArchive = 0
and  @Filter
order by ProductCode desc

";
                    #endregion

                }
                else if (vm.ButtonName == "Next")
                {

                    #region Next Item

                    sqlText = sqlText + @"
--------------------------------------------------Next--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 ItemNo, ProductCode from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1
and p.IsArchive = 0
and p.ProductCode > @ProductCode
and @Filter
order by ProductCode asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Previous")
                {
                    #region Previous Item

                    sqlText = sqlText + @"
--------------------------------------------------Previous--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 ItemNo, ProductCode from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1
and p.IsArchive = 0
and p.ProductCode < @ProductCode
and @Filter
order by ProductCode desc

";
                    #endregion
                }

                string FilterOverhead = "pc.IsRaw not in ('Overhead')";

                if (vm.ProductType == "Overhead")
                {
                    FilterOverhead = "pc.IsRaw in ('Overhead')";
                }

                sqlText = sqlText.Replace("@Filter", FilterOverhead);
                #endregion

                #region SQL Execution
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);



                if (!string.IsNullOrWhiteSpace(vm.Code))
                {
                    cmd.Parameters.AddWithValue("@ProductCode", vm.Code);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.ItemNo = dt.Rows[0]["ItemNo"].ToString();
                    vm.Code = dt.Rows[0]["ProductCode"].ToString();
                }
                else
                {
                    if (vm.ButtonName == "Previous" || vm.ButtonName == "Current")
                    {
                        vm.ButtonName = "First";
                        vm = Product_Navigation(vm, currConn, transaction, connVM);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = Product_Navigation(vm, currConn, transaction, connVM);

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
                FileLogger.Log("ProductDAL", "Product_Navigation", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return vm;

        }

        #region Backup - Before Feb04-2020

        //
        public decimal GetLastNBRPriceFromBOM_VatName(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);


                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                //sqlText += " FinishItemNo='" + itemNo + "' ";
                sqlText += " FinishItemNo=@itemNo ";

                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y'";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@CustomerID ";
                }
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@CustomerID", SqlDbType.VarChar, 20);
                parameter.Value = CustomerID;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno=@itemNo ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;

                    parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                    parameter.Value = itemNo;
                    cmdGetLastNBRPriceFromProducts.Parameters.Add(parameter);

                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }


        //Cavinkare BOMReference 
        public decimal CavinkareGetLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, string BOMReferenceName, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo='" + itemNo + "' ";
                sqlText += " and vatname='" + VatName + "' ";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                sqlText += " AND ReferenceNo='" + BOMReferenceName + "' ";

                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno='" + itemNo + "' ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        #endregion

        #region BOM Methods

        public DataTable SelectBOMRaw(string itemNo, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";

            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region open connection and transaction
                if (VcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region ProductExist

               


                //sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";

                sqlText = "select count(ItemNo) from Products where ItemNo=@ItemNo";


                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;


                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 250);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);



                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region BOMId

                sqlText = "  ";
                sqlText += " select top 1  BOMId from BOMRaws";
                sqlText += " where 1=1";
                sqlText += " and RawItemNo=@itemNo";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y'";

                sqlText += " order by effectdate desc ";


                SqlCommand cmdBOM = new SqlCommand(sqlText, VcurrConn, Vtransaction);

                cmdBOM.Parameters.AddWithValue("@itemNo", itemNo);
                cmdBOM.Parameters.AddWithValue("@effectDate", effectDate);

                SqlDataAdapter da = new SqlDataAdapter(cmdBOM);

                da.Fill(dt);

                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SelectBOMRaw", sqlex.ToString() + "\n" + sqlText);
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectBOMRaw", ex.ToString() + "\n" + sqlText);
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (VcurrConn.State == ConnectionState.Open)
                    {
                        VcurrConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return dt;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 OK
        public DataTable GetBOM(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dt = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 250);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) NBRPrice, BOMId from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y'";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@CustomerID ";
                }
                sqlText += " order by effectdate desc ";


                SqlCommand cmdBOM = new SqlCommand(sqlText, currConn);
                cmdBOM.Transaction = transaction;

                 parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmdBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@CustomerID", SqlDbType.VarChar, 20);
                parameter.Value = CustomerID;
                cmdBOM.Parameters.Add(parameter);

                SqlDataAdapter da = new SqlDataAdapter(cmdBOM);

                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {

                }
                else
                {
                    #region Comments

                    //////if (cmdBOM.ExecuteScalar() == null)
                    //////{
                    //////    retResults = 0;
                    //////}
                    //////else
                    //////{
                    //////    retResults = (decimal)cmdBOM.ExecuteScalar();
                    //////}
                    ////if (retResults == 0)
                    ////{

                    #endregion

                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno=@itemNo ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;

                     parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                    parameter.Value = itemNo;
                    cmdGetLastNBRPriceFromProducts.Parameters.Add(parameter);



                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }

                    DataRow dr = dt.NewRow();

                    dt.Rows.InsertAt(dr, 0);

                    dt.Rows[0]["NBRPrice"] = retResults.ToString();


                    ////}
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetBOM", sqlex.ToString() + "\n" + sqlText);

            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetBOM", ex.ToString() + "\n" + sqlText);

            }

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Comments 2020-12-15

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return dt;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public List<BOMNBRVM> DropDownBOMReferenceNo(string itemNo, string VatName, string effectDate, string CustomerID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string sqlText = "";
            List<BOMNBRVM> VMs = new List<BOMNBRVM>();
            BOMNBRVM vm;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region try

            try
            {
                #region Old connection

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}
                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region sql statement

                DataTable dt = new DataTable();

                string CustomerWiseBOM = new CommonDAL().settings("Sale", "CustomerWiseBOM", currConn, transaction, connVM);

                if (CustomerWiseBOM == "N")
                {
                    CustomerID = "0";
                }

                dt = GetBOMReferenceNo(itemNo, VatName, effectDate, currConn, transaction, CustomerID, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new BOMNBRVM();
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    VMs.Add(vm);
                }
                #endregion

            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "DropDownBOMReferenceNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DropDownBOMReferenceNo", ex.ToString() + "\n" + sqlText);

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

                #region Old

                if (VcurrConn != null)
                {
                    if (VcurrConn.State == ConnectionState.Open)
                    {
                        VcurrConn.Close();
                    }
                }
                #endregion

            }

            #endregion

            return VMs;

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public DataTable GetBOMReferenceNo(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            string sqlText = "";

            DataTable dt = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                //sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                sqlText = "select count(ItemNo) from Products where ItemNo=@ItemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);

                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += @" 
--------declare @FinishItemNo nvarchar(100) = '1263'
--------declare @EffectDate nvarchar(100) = '2019-Oct-06' 
--------declare @VATName nvarchar(100) = 'VAT 4.3' 
--------declare @CustomerId nvarchar(100) = '21'


SELECT ISNULL(NBRPrice,0) NBRPrice, BOMId, ISNULL(ReferenceNo,'NA') ReferenceNo
FROM BOMs
WHERE 1=1
AND FinishItemNo=@FinishItemNo
AND VATName=@VATName
AND EffectDate<=@EffectDate 
AND 1=1
AND Post='Y'
AND ISNULL(CustomerId,0)=@CustomerId

AND isnull(ReferenceNo,'-') in
(

SELECT distinct isnull(ReferenceNo,'-')
FROM BOMs 
WHERE 1=1
AND FinishItemNo=@FinishItemNo
AND EffectDate<=@EffectDate 
AND VATName=@VATName
and Post='Y'
AND 1=1
--ORDER BY EffectDate DESC
)
ORDER BY  EffectDate DESC ,BOMId
";

                if (CustomerID == "0" || string.IsNullOrWhiteSpace(CustomerID))
                {
                    sqlText = sqlText.Replace("ISNULL(CustomerId,0)=@CustomerId", "1=1");
                    //sqlText = sqlText.Replace("ISNULL(CustomerId,0)=@CustomerId", "ISNULL(CustomerId,0)=0");

                }
                else
                {

                }


                SqlCommand cmdBOM = new SqlCommand(sqlText, currConn);
                cmdBOM.Transaction = transaction;

           
                SqlDataAdapter da = new SqlDataAdapter(cmdBOM);

                da.SelectCommand.Parameters.AddWithValue("@FinishItemNo", itemNo);
                da.SelectCommand.Parameters.AddWithValue("@EffectDate", effectDate);
                da.SelectCommand.Parameters.AddWithValue("@VATName", VatName);
                da.SelectCommand.Parameters.AddWithValue("@CustomerId", CustomerID);

                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {

                }
                else
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno=@itemNo ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;

                    parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                    parameter.Value = itemNo;
                    cmdGetLastNBRPriceFromProducts.Parameters.Add(parameter);


                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }

                    DataRow dr = dt.NewRow();

                    dt.Rows.InsertAt(dr, 0);

                    dt.Rows[0]["NBRPrice"] = retResults.ToString();
                    dt.Rows[0]["BOMId"] = "0";
                    dt.Rows[0]["ReferenceNo"] = "NA";
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetBOMReferenceNo", sqlex.ToString() + "\n" + sqlText);

            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetBOMReferenceNo", ex.ToString() + "\n" + sqlText);

            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return dt;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public int GetBOMId_ByReferenceNo(string itemNo, string VatName, string effectDate, string CustomerID, string ReferenceNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            int BOMId = 0;
            DataTable dt = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Old connection

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region Last Price

                sqlText = "  ";
                sqlText += @" 
--------declare @FinishItemNo nvarchar(100) = '1263'
--------declare @EffectDate nvarchar(100) = '2019-Oct-06' 
--------declare @VATName nvarchar(100) = 'VAT 4.3' 
--------declare @CustomerId nvarchar(100) = '21'

SELECT Top 1 ISNULL(NBRPrice,0) NBRPrice, BOMId, ISNULL(ReferenceNo,'NA') ReferenceNo
FROM BOMs
WHERE 1=1
AND FinishItemNo=@FinishItemNo
AND EffectDate<=@EffectDate 
AND VATName=@VATName
AND ISNULL(CustomerId,0)=@CustomerId
AND ISNULL(ReferenceNo,'NA')=@ReferenceNo
AND Post='Y'

";

                if (CustomerID == "0" || string.IsNullOrWhiteSpace(CustomerID))
                {
                    sqlText = sqlText.Replace("ISNULL(CustomerId,0)=@CustomerId", "1=1");
                }

                sqlText = sqlText + " ORDER BY EffectDate DESC";

                SqlCommand cmdBOM = new SqlCommand(sqlText, currConn);
                cmdBOM.Transaction = transaction;

                SqlDataAdapter da = new SqlDataAdapter(cmdBOM);

                da.SelectCommand.Parameters.AddWithValue("@FinishItemNo", itemNo);
                da.SelectCommand.Parameters.AddWithValue("@EffectDate", effectDate);
                da.SelectCommand.Parameters.AddWithValue("@VATName", VatName);
                da.SelectCommand.Parameters.AddWithValue("@CustomerId", CustomerID);
                da.SelectCommand.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);

                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    BOMId = Convert.ToInt32(dt.Rows[0]["BOMId"]);
                }



                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetBOMId_ByReferenceNo", sqlex.ToString() + "\n" + sqlText);

            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetBOMId_ByReferenceNo", ex.ToString() + "\n" + sqlText);

            }

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return BOMId;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public decimal GetLastNBRPriceFromBOM(string itemNo, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);


                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                //sqlText += " and vatname='" + VatName + "' ";
                sqlText += " and effectdate<=@effectdate";
                sqlText += " and post='Y'";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@CustomerID ";
                }
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectdate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@CustomerID", SqlDbType.VarChar, 20);
                parameter.Value = CustomerID;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno=@itemNo ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);

                    parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                    parameter.Value = itemNo;
                    cmdGetLastNBRPriceFromProducts.Parameters.Add(parameter);

                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetLastNBRPriceFromBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetLastNBRPriceFromBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public decimal GetLastUseQuantityFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FinishItemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (effectDate < DateTime.MinValue || effectDate > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                string sEffectDate = OrdinaryVATDesktop.DateToDate(effectDate.ToString());

                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT TOP 1 ISNULL(isnull(UOMUQty,0)+isnull(UOMWQty,0),0)TotalQty FROM BOMRaws ";
                sqlText += " where ";
                sqlText += " FinishItemNo=@FinishItemNo ";
                sqlText += " and RawItemNo=@RawItemNo ";
                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@sEffectDate";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@FinishItemNo", SqlDbType.VarChar, 20);
                parameter.Value = FinishItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@RawItemNo", SqlDbType.VarChar, 20);
                parameter.Value = RawItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@sEffectDate", SqlDbType.VarChar, 20);
                parameter.Value = sEffectDate;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);


                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " SELECT TOP 1 ISNULL(isnull(UOMUQty,0)+isnull(UOMWQty,0),0)TotalQty FROM BOMRaws ";
                    sqlText += " where ";
                    sqlText += " FinishItemNo=@FinishItemNo ";
                    sqlText += " and RawItemNo=@RawItemNo ";
                    sqlText += " and vatname='VAT 1' ";
                    //sqlText += " and effectdate<='" + effectDate + "'";
                    sqlText += " and post='Y'";
                    sqlText += " order by effectdate desc ";


                    SqlCommand cmdGetLastNBRPriceFromBOM1 = new SqlCommand(sqlText, currConn);
                    parameter = new SqlParameter("@FinishItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = FinishItemNo;
                    cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                    parameter = new SqlParameter("@RawItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = RawItemNo;
                    cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                    cmdGetLastNBRPriceFromBOM1.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromBOM1.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromBOM1.ExecuteScalar();
                    }
                }



                #endregion Last UseQuantity

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetLastUseQuantityFromBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetLastUseQuantityFromBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public string GetBomIdFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FinishItemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (effectDate < DateTime.MinValue || effectDate > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT TOP 1 BOMID FROM BOMRaws ";
                sqlText += " where ";
                sqlText += " FinishItemNo=@FinishItemNo ";
                sqlText += " and RawItemNo=@RawItemNo ";
                sqlText += " and vatname=@VatName ";
                //sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@FinishItemNo", SqlDbType.VarChar, 20);
                parameter.Value = FinishItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@RawItemNo", SqlDbType.VarChar, 20);
                parameter.Value = RawItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = cmdGetLastNBRPriceFromBOM.ExecuteScalar().ToString();
                }
                if (retResults == "0")
                {
                    sqlText = "  ";
                    sqlText += " SELECT TOP 1 BOMID FROM BOMRaws ";

                    sqlText += " where ";
                    sqlText += " FinishItemNo=@FinishItemNo ";
                    sqlText += " and RawItemNo=@RawItemNo ";
                    sqlText += " and vatname=@VatName ";
                    //sqlText += " and effectdate<='" + effectDate + "'";
                    sqlText += " and post='Y'";
                    sqlText += " order by effectdate desc ";


                    SqlCommand cmdGetLastNBRPriceFromBOM1 = new SqlCommand(sqlText, currConn);

                    parameter = new SqlParameter("@FinishItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = FinishItemNo;
                    cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                    parameter = new SqlParameter("@RawItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = RawItemNo;
                    cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                    parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                    parameter.Value = VatName;
                    cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                    cmdGetLastNBRPriceFromBOM1.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromBOM1.ExecuteScalar() == null)
                    {
                        retResults = "0";
                    }
                    else
                    {
                        retResults = cmdGetLastNBRPriceFromBOM1.ExecuteScalar().ToString();
                    }
                }



                #endregion Last UseQuantity

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetBomIdFromBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetBomIdFromBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string GetBomIdFromBOMFinishItem(string FinishItemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FinishItemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT TOP 1 BOMID FROM BOMs ";
                sqlText += " where ";
                sqlText += " FinishItemNo=@FinishItemNo ";
                sqlText += " and vatname=@VatName ";
                //sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@FinishItemNo", SqlDbType.VarChar, 20);
                parameter.Value = FinishItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = cmdGetLastNBRPriceFromBOM.ExecuteScalar().ToString();
                }




                #endregion Last UseQuantity

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetBomIdFromBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetBomIdFromBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public decimal GetLastVatableFromBOM(string itemNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastVatableFromBOM", "There is No data to find Price");
                }

                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastVatableFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price
                //kkk
                sqlText = "  ";
                sqlText += " select top 1 isnull(UOMPrice,0) from BOMRaws";
                sqlText += " where ";
                sqlText += " rawItemNo=@itemNo ";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdGetLastVatableFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastVatableFromBOM.Transaction = transaction;

                parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdGetLastVatableFromBOM.Parameters.Add(parameter);

                if (cmdGetLastVatableFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastVatableFromBOM.ExecuteScalar();
                }



                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetLastVatableFromBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetLastVatableFromBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn != null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public decimal GetLastTollChargeFBOMOH(string HeadName, string VatName, string effectDate,
            SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(HeadName))
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction

                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@HeadName";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@HeadName", SqlDbType.VarChar, 20);
                parameter.Value = HeadName;
                cmdExist.Parameters.Add(parameter);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Get Last NBR Price From BOM

                //st
                sqlText = "  ";
                sqlText += " select top 1 isnull(RebateAmount,0) from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += " HeadId=@HeadName ";
                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM1 = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM1.Transaction = transaction;

                parameter = new SqlParameter("@HeadName", SqlDbType.VarChar, 20);
                parameter.Value = HeadName;
                cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmdGetLastNBRPriceFromBOM1.Parameters.Add(parameter);

                if (cmdGetLastNBRPriceFromBOM1.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM1.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    #region Last Price

                    //st
                    sqlText = "  ";
                    sqlText += " select top 1 isnull(AdditionalCost,0) from BOMCompanyOverhead";
                    sqlText += " where ";
                    sqlText += " HeadId=@HeadName ";
                    sqlText += " and vatname=@VatName ";
                    sqlText += " and effectdate<=@effectDate";
                    sqlText += " order by effectdate desc ";


                    SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                    parameter = new SqlParameter("@HeadName", SqlDbType.VarChar, 20);
                    parameter.Value = HeadName;
                    cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                    parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                    parameter.Value = VatName;
                    cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                    parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                    parameter.Value = effectDate;
                    cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                    if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                    }
                    if (retResults == 0)
                    {
                        sqlText = "  ";
                        sqlText += " select isnull(TollCharge,0) from products";
                        sqlText += " where ";
                        //sqlText += " ProductName='" + HeadName + "' ";
                        sqlText += " ProductName=@HeadName ";

                        

                        SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                        cmdGetLastNBRPriceFromProducts.Transaction = transaction;

                        parameter = new SqlParameter("@HeadName", SqlDbType.VarChar, 500);
                        parameter.Value = HeadName;
                        cmdGetLastNBRPriceFromProducts.Parameters.Add(parameter);

                        if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                        {
                            retResults = 0;
                        }
                        else
                        {
                            retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                        }
                    }

                    #endregion Last Price
                }
                #endregion

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetLastTollChargeFBOMOH", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetLastTollChargeFBOMOH", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 Ok
        public string GetFinishItemIdFromOH(string ItemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@ItemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = ItemNo;
                cmdExist.Parameters.Add(parameter);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price
                //1
                sqlText = "  ";
                sqlText += " select top 1 FinishItemNo from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += "  vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                //sqlText += " and HeadId='" + ItemNo + "'";
                sqlText += " and HeadId=@ItemNo";


                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = ItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = (string)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (string.IsNullOrEmpty(retResults))
                {
                    sqlText = "  ";
                    sqlText += " select isnull(TollCharge,0) from products";
                    sqlText += " where ";
                    sqlText += " ItemNo=@ItemNo ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;

                    parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = ItemNo;
                    cmdGetLastNBRPriceFromProducts.Parameters.Add(parameter);

                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = "0";
                    }
                    else
                    {
                        retResults = (string)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetFinishItemIdFromOH", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetFinishItemIdFromOH", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Old
                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
                #endregion

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public string GetBOMIdFromOH(string OverHeadItemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(OverHeadItemNo))
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction

                //////if (VcurrConn == null)
                //////{
                //////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                //////    if (VcurrConn.State != ConnectionState.Open)
                //////    {
                //////        VcurrConn.Open();
                //////    }
                //////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region ProductExist

                //sqlText = "select count(ItemNo) from Products where ItemNo='" + OverHeadItemNo + "'";
                sqlText = "select count(ItemNo) from Products where ItemNo=@OverHeadItemNo";

                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@OverHeadItemNo", SqlDbType.VarChar, 20);
                parameter.Value = OverHeadItemNo;
                cmdExist.Parameters.Add(parameter);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price
                //1
                sqlText = "  ";
                sqlText += " select top 1 BOMId from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += "  vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                //sqlText += " and HeadID='" + OverHeadItemNo + "'";
                sqlText += " and HeadID=@OverHeadItemNo ";


                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                parameter = new SqlParameter("@VatName", SqlDbType.VarChar, 50);
                parameter.Value = VatName;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@OverHeadItemNo", SqlDbType.VarChar, 20);
                parameter.Value = OverHeadItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = cmdGetLastNBRPriceFromBOM.ExecuteScalar().ToString();
                }



                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetBOMIdFromOH", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetBOMIdFromOH", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                //////if (VcurrConn == null)
                //////{
                //////    if (VcurrConn.State == ConnectionState.Open)
                //////    {
                //////        VcurrConn.Close();

                //////    }
                //////}
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        #endregion

        #region Stock and Price

        public ResultVM Product_IN_OUT(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int executionResult = 0;

            #endregion

            #region Try Statement

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

                int BranchId = 1;

                if (paramVM != null && paramVM.BranchId > 0)
                {
                    BranchId = paramVM.BranchId;
                }

                #region Update Stock / For Loop

                sqlText = "";
                sqlText = @"
----------declare @BranchId as int = 0
----------declare @ItemNo as varchar(100) ='846'
----------declare @ChangeStock as decimal(15,6)=-100


update ProductStocks set CurrentStock = ISNULL(CurrentStock,0)+ @ChangeStock 
from ProductStocks
where 1=1 
and ItemNo=@ItemNo
and BranchId=@BranchId

";
                if (paramVM != null && paramVM.dt != null && paramVM.dt.Rows.Count > 0)
                {
                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    foreach (DataRow dr in paramVM.dt.Rows)
                    {
                        decimal cStock = 0;
                        if (!string.IsNullOrWhiteSpace(dr["Quantity"].ToString()))
                        {
                            cStock = Convert.ToDecimal(dr["Quantity"]);
                        }

                        cmd = new SqlCommand(sqlText, currConn, transaction);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@ItemNo", dr["ItemNo"]);
                        cmd.Parameters.AddWithValue("@ChangeStock", cStock);

                        executionResult = cmd.ExecuteNonQuery();

                    }

                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Product Stock Updated Successfully!";

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "Product_IN_OUT", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM Product_Stock_Update(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int executionResult = 0;

            #endregion

            #region Try Statement

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

                int BranchId = 1;

                if (paramVM != null && paramVM.BranchId > 0)
                {
                    BranchId = paramVM.BranchId;
                }

                #region Product Stock Table insert
                ProductStockProcess(currConn, transaction, connVM);
                #endregion

                #region Get Products

                sqlText = "";
                sqlText = @"
----------declare @BranchId as int = 0


select isnull(p.ReportType,'-')ReportType, s.* from ProductStocks s
left outer join Products p on s.ItemNo=p.ItemNo
where 1=1 
and s.BranchId=@BranchId
";
                string IDs = string.Empty;
                if (paramVM != null)
                {
                    if (paramVM.IsDBMigration)
                    {
                        sqlText = sqlText + @" and ISNULL(s.CurrentStock,0)=0";
                    }
                    if (!string.IsNullOrWhiteSpace(paramVM.ItemNo))
                    {
                        sqlText = sqlText + @" and s.ItemNo=@ItemNo";
                    }

                    if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                    {
                        IDs = string.Join("','", paramVM.IDs);

                        //sqlText = sqlText + @" and s.ItemNo IN('" + IDs + "')";
                        sqlText = sqlText + @" and s.ItemNo IN('@IDs')";

                    }
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;
                cmd.Parameters.AddWithValue("@BranchId", BranchId);

                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    SqlParameter parameter = new SqlParameter("@IDs", SqlDbType.VarChar);
                    parameter.Value = IDs;
                    cmd.Parameters.Add(parameter);
                }


                if (paramVM != null)
                {
                    if (!string.IsNullOrWhiteSpace(paramVM.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", paramVM.ItemNo);
                    }

                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                #endregion

                #region Get Stock by Using Current Stock Method and Update Stock / For Loop

                if (dt != null && dt.Rows.Count > 0)
                {

                    string today = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    DataTable dtStock = new DataTable();

                    #region Update Stock

                    sqlText = "";
                    sqlText = @"
------declare @ItemNo as varchar(100) = '846'
------declare @CurrentStock as decimal(15,5) = 0
----------declare @BranchId as int = 0

update ProductStocks set CurrentStock=@CurrentStock
from ProductStocks
where 1=1 
and ItemNo=@ItemNo
and BranchId=@BranchId
";

                    #endregion

                    decimal CurrentStockQuantity = 0;
                    string ItemNo = "";
                    string ReportType = "";

                    foreach (DataRow dr in dt.Rows)
                    {

                        #region Call Stock

                        ItemNo = "";
                        ReportType = "";
                        CurrentStockQuantity = 0;
                        dtStock = new DataTable();

                        ItemNo = dr["ItemNo"].ToString();
                        ReportType = dr["ReportType"].ToString();

                        dtStock = AvgPriceNew(ItemNo, today, currConn, transaction, true, true, true, false, connVM,
                            UserId, true, ReportType);

                        if (dtStock != null && dtStock.Rows.Count > 0)
                        {
                            CurrentStockQuantity = Convert.ToDecimal(dtStock.Rows[0]["Quantity"]);
                        }

                        #endregion

                        #region Check Point

                        ////if (CurrentStockQuantity <= 0)
                        ////{
                        ////    continue;
                        ////}

                        #endregion

                        #region SQL Execution

                        cmd = new SqlCommand(sqlText, currConn, transaction);
                        cmd.CommandTimeout = 500;
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@ItemNo", ItemNo);
                        cmd.Parameters.AddWithValue("@CurrentStock", @CurrentStockQuantity);

                        executionResult = cmd.ExecuteNonQuery();

                        #endregion

                    }

                }
                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Product Stock Updated Successfully!";

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Product_Stock_Update", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public ResultVM ProductRefresh(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            ProductDAL productDAL = new ProductDAL();
            int executionResult = 0;

            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "update ProductStocks set CurrentStock = 0 where BranchId = @BranchId";

                if (!string.IsNullOrEmpty(paramVM.ItemNo))
                {
                    sqlText += " and ItemNo=@ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@BranchId", paramVM.BranchId);

                if (!string.IsNullOrEmpty(paramVM.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", paramVM.ItemNo);
                }

                cmd.ExecuteNonQuery();

                rVM = Product_Stock_Update(paramVM, currConn, transaction, connVM, UserId);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Product_Stock_Update", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public DataTable GetLIFOPurchaseInformation(string itemNo, string receiveDate, string PurchaseInvoiceNo = "", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            DataTable retResults = new DataTable();
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "There is No data in purchase");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }


                #endregion open connection and transaction

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y" ? true : false);

                #region Stock

                sqlText = "  ";

                sqlText +=
                    @"  
                DECLARE @costPrice AS DECIMAL(25,9);
                DECLARE @effectDate AS datetime;
                DECLARE @receiveDate AS datetime;

                SELECT TOP 1 @costPrice=  isnull(CostPrice,0),@effectDate=InputDate  FROM  Costing 
                ";
                sqlText += "  where  ItemNo=@itemNo and InputDate<=@receiveDate  ORDER BY InputDate DESC ";

                sqlText += @"
                SELECT TOP 1 receiveDate=@receiveDate FROM  PurchaseInvoiceDetails";
                sqlText += " where  ItemNo=@itemNo and Post='Y' and ReceiveDate<=@receiveDate ";
                if (!string.IsNullOrEmpty(PurchaseInvoiceNo))
                {
                    sqlText += " and PurchaseInvoiceNo=@PurchaseInvoiceNo";

                }
                sqlText += " ORDER BY receiveDate DESC";

                sqlText += "\n if @receiveDate!='' ";
                sqlText += "\n Begin";

                sqlText += @"  

                select top 1 PurchaseInvoiceNo,isnull(NBRPrice,0)CostPrice, isnull(UOMQty,0) PurchaseQuantity,";
                if (ImportCostingIncludeATV)
                {
                    sqlText += @"      CASE WHEN TransactionType='Import' OR TransactionType='InputServiceImport' OR TransactionType='ServiceImport' 
                OR TransactionType='ServiceNSImport' OR TransactionType='TradingImport' THEN
                isnull((isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(SDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0) 	
                ";
                }
                else
                {
                    sqlText += @"      CASE
                WHEN   TransactionType='InputServiceImport' OR TransactionType='ServiceImport' 
                OR TransactionType='ServiceNSImport' OR TransactionType='TradingImport' THEN
                isnull((isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(SDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(OthersAmount,0)),0)),0) 	
                
                WHEN TransactionType='Import'   THEN
                isnull((isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(SDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(OthersAmount,0)),0)),0) 	
              
";
                }


                sqlText += @"      ELSE 
                isnull(SubTotal,0)
                END AS  PurchaseCostPrice

                from PurchaseInvoiceDetails 

                ";

                sqlText += "  where  ItemNo=@itemNo  and Post='Y'  ";

                sqlText += " and ReceiveDate<= @receiveDate " ;

                sqlText += " and TransactionType in ('InputService','Import','Other','ServiceImport','Service','InputServiceImport','ServiceNonStock','ServiceNonStockImport') " ;
                


                if (!string.IsNullOrEmpty(PurchaseInvoiceNo))
                {
                    sqlText += " and PurchaseInvoiceNo=@PurchaseInvoiceNo";

                }
                sqlText += " ORDER BY receiveDate DESC";
                sqlText += @" EnD 
                ELSE
                BEGIN 
                select top 1 Id PurchaseInvoiceNo, isnull((costPrice-VATAmount),0) PurchaseCostPrice,quantity PurchaseQuantity,costPrice FROM  Costing ";
                sqlText += " where  ItemNo =@itemNo ORDER BY InputDate DESC ";

                sqlText += "END";



                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@receiveDate", SqlDbType.DateTime);
                parameter.Value = receiveDate;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@PurchaseInvoiceNo", SqlDbType.VarChar, 20);
                parameter.Value = PurchaseInvoiceNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);


                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmdGetLastNBRPriceFromBOM);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLastLIFOPriceNInvNo", "No row found ");
                }
                retResults = dataTable;
                return retResults;


                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetLIFOPurchaseInformation", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetLIFOPurchaseInformation", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public DataTable AVGStockNewMethod(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            decimal AvgRate = 0;
            decimal Quantity = 0;
            #endregion
            #region try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

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

                CommonDAL commonDal = new CommonDAL();
                string stockNewMethod = commonDal.settings("Product", "AVGStockNewMethod", currConn, transaction, connVM);

                if (stockNewMethod == "Y")
                {
                    PurchaseDAL purchaseDal = new PurchaseDAL();

                    DataSet dtavgPrice = purchaseDal.GetAvgPrice(itemNo, tranDate, currConn, transaction, connVM);

                    if (dtavgPrice != null && dtavgPrice.Tables[0].Rows.Count > 0)
                    {
                        AvgRate = Convert.ToDecimal(
                            string.IsNullOrEmpty(dtavgPrice.Tables[0].Rows[0]["AvgPrice"].ToString())
                                ? 0
                                : dtavgPrice.Tables[0].Rows[0]["AvgPrice"]);
                    }

                    if (dtavgPrice != null && dtavgPrice.Tables[1].Rows.Count > 0)
                    {
                        ////string currentStock = dtavgPrice.Tables[1]
                        ////    .Select("BranchId = '" + OrdinaryVATDesktop.BranchId + "'")[0]["CurrentStock"].ToString();

                        string currentStock = "0";
                        if (OrdinaryVATDesktop.BranchId != 0)
                        {
                            var dataRows = dtavgPrice.Tables[1]
                                .Select("BranchId = '" + OrdinaryVATDesktop.BranchId + "'");

                            if (dataRows.Length > 0)
                            {
                                if (dataRows[0]["CurrentStock"] != null && dataRows[0]["CurrentStock"].ToString() != "")
                                {
                                    currentStock = dataRows[0]["CurrentStock"].ToString();
                                }
                            }
                        }

                        Quantity = Convert.ToDecimal(
                            string.IsNullOrEmpty(currentStock)
                                ? 0
                                : Convert.ToDecimal(currentStock));
                    }
                    //Quantity = Quantity == 0 ? 1 : Quantity;
                    retResults.Columns.Add("Quantity");
                    retResults.Columns.Add("Amount");
                    retResults.Columns.Add("AvgRate");

                    retResults.Rows.Add(new object[] { Quantity, AvgRate * Quantity, AvgRate });

                }
            }
            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                FileLogger.LogWeb("ProductDAL", "AvgPriceNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
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
        public DataTable AvgPriceNew(string itemNo, string tranDate, SqlConnection VcurrConn
            , SqlTransaction Vtransaction, bool isPost, bool Vat16 = true, bool Vat17 = true, bool transfer = true
            , SysDBInfoVMTemp connVM = null, string UserId = "", bool stockProcess = false, string ReportType = "")
        {
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

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

                #region Variables

                DataSet AvgPriceVAT16 = new DataSet();
                DataSet AvgPriceVAT17 = new DataSet();
                DataSet transferStock = new DataSet();
                DataSet transferIssue = new DataSet();
                DataSet transferReceive = new DataSet();

                ProductDAL productDal = new ProductDAL();
                ReportDSDAL _reportDal = new ReportDSDAL();

                TransferIssueDAL _treportDal = new TransferIssueDAL();
                decimal Quantity = 0;
                decimal Quantity16 = 0;
                decimal Quantity17 = 0;
                decimal QuantityT = 0;

                decimal Amount = 0;
                decimal Amount16 = 0;
                decimal Amount17 = 0;
                decimal AmountT = 0;

                #endregion

                CommonDAL commonDal = new CommonDAL();
                string stockNewMethod = commonDal.settings("Product", "AVGStockNewMethod", currConn, transaction, connVM);

                if (stockNewMethod == "Y" && !stockProcess)
                {
                    retResults = AVGStockNewMethod(itemNo, tranDate, VcurrConn, Vtransaction, connVM);
                }
                else
                {
                    if (Vat16)
                    {

                        #region Parmeter Assign (VAT 6.1)

                        VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

                        varVAT6_1ParamVM.ItemNo = itemNo;
                        varVAT6_1ParamVM.UserName = "";
                        varVAT6_1ParamVM.StartDate = tranDate;
                        varVAT6_1ParamVM.EndDate = tranDate;
                        varVAT6_1ParamVM.Post1 = "Y";
                        varVAT6_1ParamVM.Post2 = "Y";
                        varVAT6_1ParamVM.ReportName = "";
                        varVAT6_1ParamVM.BranchId = OrdinaryVATDesktop.BranchId;
                        varVAT6_1ParamVM.Opening = true;
                        varVAT6_1ParamVM.OpeningFromProduct = false;
                        varVAT6_1ParamVM.UserId = UserId;
                        varVAT6_1ParamVM.ReportType = ReportType;

                        if (varVAT6_1ParamVM.ReportType.ToLower() == "VAT6_2_1".ToLower())
                        {
                            varVAT6_1ParamVM.VAT6_2_1 = true;
                        }

                        #endregion

                        if (varVAT6_1ParamVM.ReportType.ToLower() == "VAT6_1".ToLower()
                            || varVAT6_1ParamVM.ReportType.ToLower() == "VAT6_2_1".ToLower()
                            || varVAT6_1ParamVM.ReportType.ToLower() == "VAT6_1_And_6_2".ToLower()
                            )
                        {
                            AvgPriceVAT16 = _vatRegistersDAL.VAT6_1_WithConn(varVAT6_1ParamVM, currConn, transaction, connVM);
                        }

                        if (AvgPriceVAT16 != null && AvgPriceVAT16.Tables != null && AvgPriceVAT16.Tables.Count > 0)
                        {
                            if (AvgPriceVAT16.Tables[0].Rows.Count > 0)
                            {
                                Quantity16 = Convert.ToDecimal(AvgPriceVAT16.Tables[0].Rows[0]["Quantity"]);
                                Amount16 = Convert.ToDecimal(AvgPriceVAT16.Tables[0].Rows[0]["UnitCost"]);
                            }
                        }
                    }
                    if (Vat17)
                    {
                        #region Parmeter Assign (VAT 6.2)

                        VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                        varVAT6_2ParamVM.ItemNo = itemNo;
                        varVAT6_2ParamVM.StartDate = tranDate;
                        varVAT6_2ParamVM.EndDate = tranDate;
                        varVAT6_2ParamVM.Post1 = "Y";
                        varVAT6_2ParamVM.Post2 = "Y";
                        varVAT6_2ParamVM.BranchId = OrdinaryVATDesktop.BranchId;
                        //varVAT6_2ParamVM.Opening = true;
                        //varVAT6_2ParamVM.Opening6_2 = true;
                        varVAT6_2ParamVM.UserId = UserId;
                        varVAT6_2ParamVM.ReportType = ReportType;
                        if (varVAT6_2ParamVM.ReportType.ToLower() == "VAT6_2_1".ToLower())
                        {
                            varVAT6_2ParamVM.VAT6_2_1 = true;
                        }

                        #endregion
                        if (varVAT6_2ParamVM.ReportType.ToLower() == "VAT6_2".ToLower()
                          || varVAT6_2ParamVM.ReportType.ToLower() == "VAT6_2_1".ToLower()
                          || varVAT6_2ParamVM.ReportType.ToLower() == "VAT6_1_And_6_2".ToLower()
                          )
                        {
                            AvgPriceVAT17 = _vatRegistersDAL.VAT6_2(varVAT6_2ParamVM, currConn, transaction, connVM);
                        }
                        if (AvgPriceVAT17 != null && AvgPriceVAT17.Tables != null && AvgPriceVAT17.Tables.Count > 0)
                        {
                            if (AvgPriceVAT17.Tables[0].Rows.Count > 0)
                            {

                                Quantity17 = Convert.ToDecimal(AvgPriceVAT17.Tables[0].Rows[0]["Quantity"]);
                                Amount17 = Convert.ToDecimal(AvgPriceVAT17.Tables[0].Rows[0]["UnitCost"]);
                            }
                        }
                    }
                    if (true)
                    {
                        transferStock = _treportDal.TransferStock(itemNo, tranDate, OrdinaryVATDesktop.BranchId, currConn, transaction, connVM);
                        if (transferStock != null && transferStock.Tables != null && transferStock.Tables.Count > 0)
                        {
                            if (transferStock.Tables[0].Rows.Count > 0)
                            {

                                QuantityT = Convert.ToDecimal(transferStock.Tables[0].Rows[0]["Quantity"]);
                                AmountT = Convert.ToDecimal(transferStock.Tables[0].Rows[0]["Amount"]);
                            }
                        }
                    }

                    Quantity = Quantity16 + Quantity17 + QuantityT;
                    Amount = Amount16 + Amount17 + AmountT;

                    retResults.Columns.Add("Quantity");
                    retResults.Columns.Add("Amount");
                    retResults.Columns.Add("AvgRate");
                    retResults.Rows.Add(new object[] { Quantity, Amount, Quantity > 0 ? (Amount / Quantity) : 0 });
                }


                //StockMovement(itemNo, tranDate, null, null, true, null);
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                FileLogger.LogWeb("ProductDAL", "AvgPriceNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
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



        //  string itemNo, string tranDate, string tranDateTo, int BranchId, SqlConnection VcurrConn
        //, SqlTransaction Vtransaction, string Post1, string Post2, string ProdutType
        //  , string ProdutCategoryId, SysDBInfoVMTemp connVM = null, string gName = "", bool chkCategoryLike = false, string FormNumeric="2", string UserId = "")
        public DataSet StockMovement(StockMovementVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            DataSet retResults = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {
                string Company = new CommonDAL().settingValue("DayEnd", "BigDataProcess", connVM, null, null);
                string companyCode = new CommonDAL().settings("CompanyCode", "Code", null, null, connVM);

                #region Validation
                //if (string.IsNullOrEmpty(itemNo))
                //{
                //    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                //}
                //else 
                if (Convert.ToDateTime(vm.StartDate) < DateTime.MinValue || Convert.ToDateTime(vm.StartDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

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

                #region Variables
                CommonDAL _CDal = new CommonDAL();

                DataTable dt = new DataTable();
                DataSet AvgPriceVAT16 = new DataSet();
                DataSet AvgPriceVAT17 = new DataSet();
                DataSet transferStock = new DataSet();
                DataSet transferIssue = new DataSet();
                DataSet transferReceive = new DataSet();

                ProductDAL productDal = new ProductDAL();
                ReportDSDAL _reportDal = new ReportDSDAL();
                TransferIssueDAL _treportDal = new TransferIssueDAL();

                #endregion

                #region Reset Data

                _CDal.ExecuteQuery("delete from ProductStockMISs", currConn, transaction, connVM);
                //vm.Branchwise = true;
                #endregion


                SqlCommand command = new SqlCommand("", currConn, transaction);

                #region VAT 6.1 (Purchase Register)

                #region Parmeter Assign (VAT 6.1)

                VAT6_1ParamVM vat61ParamVm = new VAT6_1ParamVM();

                vat61ParamVm.ItemNo = vm.ItemNo;
                vat61ParamVm.UserName = "";
                vat61ParamVm.StartDate = vm.StartDate;
                vat61ParamVm.EndDate = vm.ToDate;
                vat61ParamVm.Post1 = vm.Post1;
                vat61ParamVm.Post2 = vm.Post2;
                vat61ParamVm.ReportName = "";
                vat61ParamVm.BranchId = vm.BranchId;
                vat61ParamVm.Opening = false;
                vat61ParamVm.OpeningFromProduct = true;
                vat61ParamVm.ProdutType = vm.ProductType;
                vat61ParamVm.ProdutCategoryId = vm.CategoryId;
                vat61ParamVm.VAT6_2_1 = false;
                vat61ParamVm.StockMovement = true;
                vat61ParamVm.ProdutGroupName = vm.ProductGroupName;
                vat61ParamVm.ProdutCategoryLike = vm.CategoryLike;
                vat61ParamVm.UserId = vm.CurrentUserID;
                vat61ParamVm.BranchWise = vm.Branchwise;
                //vat61ParamVm.VAT6_2_1 = vm.chkTrading == "Y" ? true : false;

                #endregion

                #region Comments

                ////AvgPriceVAT16 = _reportDal.VAT6_1_WithConn_Backup(itemNo, "", tranDate, tranDateTo, Post1, Post2, "", BranchId,
                ////   currConn, transaction, false, null, true, ProdutType, ProdutCategoryId, false, true);

                #endregion

                AvgPriceVAT16 = _vatRegistersDAL.VAT6_1_WithConn(vat61ParamVm, currConn, transaction, connVM);

                //////OrdinaryVATDesktop.SaveExcel(AvgPriceVAT16.Tables[0], "StockDataFrom6_1", "Stock");

                string[] DeleteColumnName =
                {
                    "SerialNo", "InvoiceDateTime", "StartingQuantity", "StartingAmount", "VendorName", "Address1",
                    "Address2", "Address3", "VATRegistrationNo", "ProductName", "ProductCodeA", "UOM", "VATRate", "SD",
                    "HSCodeNo", "BENumber", "CreateDateTime", "AvgRate", 
                    "RunningOpeningQuantity", "RunningOpeningValue"
                };

                // "RunningTotal", "RunningValue",

                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT16.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "StartDateTime", "TransactionDate");

                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningTotal", "ClosingQuantity");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningValue", "ClosingAmount");

                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_1", "string");
                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");
                string[] tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction);

                //                string update6_2 = @"
                //                    update ProductStockMISs set TransType='61out' where Remarks='Transfer Issue' and StockType='VAT6_1' 
                //                    update ProductStockMISs set TransType='61In' where Remarks='Transfer Receive' and StockType='VAT6_1'";

                //                command.CommandText = update6_2;
                //                command.ExecuteNonQuery();

                #region Parmeter Assign (6.2)

                VAT6_2ParamVM vat62ParamVm = new VAT6_2ParamVM();

                vat62ParamVm.ItemNo = vm.ItemNo;
                vat62ParamVm.StartDate = vm.StartDate;
                vat62ParamVm.EndDate = vm.ToDate;
                vat62ParamVm.Post1 = vm.Post1;
                vat62ParamVm.Post2 = vm.Post2;
                vat62ParamVm.BranchId = vm.BranchId;
                vat62ParamVm.Opening = false;
                vat62ParamVm.Opening6_2 = false;
                vat62ParamVm.ProdutType = vm.ProductType;
                vat62ParamVm.ProdutCategoryId = vm.CategoryId;
                vat62ParamVm.ProdutGroupName = vm.ProductGroupName;
                vat62ParamVm.ProdutCategoryLike = vm.CategoryLike;
                vat62ParamVm.StockMovement = true;
                vat62ParamVm.UserId = vm.CurrentUserID;
                vat62ParamVm.BranchWise = vm.Branchwise;
                //vat62ParamVm.VAT6_2_1 = vm.chkTrading == "Y" ? true : false;

                #endregion

                #endregion

                #region VAT 6.2 (Sale Register)

                if (Company.ToUpper() == "Y" && companyCode.ToLower() == "bata")
                {
                    AvgPriceVAT17 = _reportDal.VAT6_2_Permanent_DayWise(OrdinaryVATDesktop.CopyObject(vat62ParamVm), currConn, transaction, connVM);
                }
                else
                {
                    ////AvgPriceVAT17 = _reportDal.VAT6_2(OrdinaryVATDesktop.CopyObject(vat62ParamVm), currConn, transaction, null);
                    AvgPriceVAT17 = _vatRegistersDAL.VAT6_2(OrdinaryVATDesktop.CopyObject(vat62ParamVm), currConn, transaction, connVM);
                }

                DeleteColumnName = new string[]
                {
                    "SerialNo", "StartingQuantity", "StartingAmount", "CustomerName", "Address1", "Address2",
                    "Address3", "VATRegistrationNo", "ProductName", "ProductCode", "UOM", "VATRate", "SD", "HSCodeNo",
                    "BENumber", "CreatedDateTime", "UnitRate", "ClosingRate", "DeclaredPrice", "RunningTotalValue",
                     "RunningOpeningQuantityFinal","ReturnTransactionType","RunningOpeningValueFinal"
                };//"RunningOpeningValueFinal",
                dt = new DataTable();
                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT17.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "StartDateTime", "TransactionDate");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "remarks", "Remarks");

                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningTotalValueFinal", "ClosingAmount");
                //////dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningOpeningValueFinal", "ClosingAmount");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningTotal", "ClosingQuantity");

                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_2", "string");
                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");

                tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction);

                //                string update6_2 = @"
                //                    update ProductStockMISs set TransType='62out' where Remarks='Transfer Issue' and StockType='VAT6_2' 
                //                    update ProductStockMISs set TransType='62In' where Remarks='Transfer Receive' and StockType='VAT6_2'";

                //                command.CommandText = update6_2;
                //                command.ExecuteNonQuery();

                #endregion

                #region VAT 6.2.1 (Trading Register)


                AvgPriceVAT17 = _reportDal.VAT6_2_1(vat62ParamVm, currConn, transaction, connVM);

                DeleteColumnName = new string[]
                {
                    "SerialNo", "StartingQuantity", "StartingAmount", "CustomerName", "Address1", "Address2",
                    "Address3", "VATRegistrationNo", "ProductName", "ProductCode", "UOM", "VATRate", "SD", "HSCodeNo",
                    "BENumber", "CreatedDateTime", "UnitRate", "ClosingRate", "DeclaredPrice", "RunningTotalValue",
                    "RunningOpeningValueFinal", "RunningOpeningQuantityFinal","VendorName","InvoiceDateTime","BranchId","UserId"
                    ,"CustomerId","RunningOpeningQuantityValue","StockType","ReturnTransactionType"
                };
                dt = new DataTable();
                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT17.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "StartDateTime", "TransactionDate");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "remarks", "Remarks");

                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningTotalValueFinal", "ClosingAmount");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "RunningTotal", "ClosingQuantity");
                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_2_1", "string");


                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");

                tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction, 10000, null, connVM);



                #endregion



                #region Transfer Issue (FG/RM Out)

                if (!vm.Branchwise)
                {

                    //transferIssue = TransferIssue(vm.ItemNo,vm.StartDate,vm.ToDate, vm.Post1, vm.Post2, vm.BranchId, false, currConn,
                    //    transaction, null, vm.ProductType, vm.CategoryId,vm.ProductGroupName,vm.CategoryLike);

                    //if (transferIssue != null && transferIssue.Tables.Count > 0 && transferIssue.Tables[0].Rows.Count > 0)
                    //{

                    //    DeleteColumnName = new string[] { "SL" };
                    //    dt = new DataTable();
                    //    dt = OrdinaryVATDesktop.DtDeleteColumns(transferIssue.Tables[0], DeleteColumnName);
                    //    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionDateTime", "TransactionDate");
                    //    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "Amount", "UnitCost");
                    //    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionType", "TransType");
                    //    dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "transferIssue", "string");
                    //    tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction);

                    //}
                }

                #endregion

                #region Transfer Receive (FG/RM In)

                if (!vm.Branchwise)
                {
                    //transferReceive = TransferReceive(vm.ItemNo, vm.StartDate, vm.ToDate, vm.Post1, vm.Post2, vm.BranchId
                    //    , false, currConn, transaction, null, vm.ProductType, vm.CategoryId, vm.ProductGroupName, vm.CategoryLike);

                    //if (transferReceive != null && transferReceive.Tables.Count > 0 && transferReceive.Tables[0].Rows.Count > 0)
                    //{
                    //    DeleteColumnName = new string[] { "SL" };
                    //    dt = new DataTable();
                    //    dt = OrdinaryVATDesktop.DtDeleteColumns(transferReceive.Tables[0], DeleteColumnName);
                    //    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionDateTime", "TransactionDate");
                    //    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "Amount", "UnitCost");
                    //    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionType", "TransType");
                    //    dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "TransferReceive", "string");
                    //    tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction);
                    //}
                }

                #endregion



                #region Delete Receive-RawSale

                //string deleteSQL = @"delete from ProductStockMISs where TransType='Receive' and Remarks='RawSale'";
                //SqlCommand cmdDelete = new SqlCommand(deleteSQL, currConn, transaction);
                //cmdDelete.ExecuteNonQuery();
                string updateClosingValues = @"

 

update ProductStockMISs set closingAMountFinal= 0,ClosingQuantityFinal= 0 

update ProductStockMISs set closingAMountFinal= a.closingAmount,ClosingQuantityFinal= a.ClosingQuantity 
from (
select ProductStockMISs.* from  ProductStockMISs,(select distinct ItemNo , max(Id)id from ProductStockMISs
group by Itemno)a where ProductStockMISs.ItemNo=a.ItemNo and ProductStockMISs.Id = a.id
) as a
 where ProductStockMISs.ID=a.Id
";
                command.CommandText = updateClosingValues;
                command.ExecuteNonQuery();

                #endregion

                #region Delete Opening from purchase for trading Bussiness


                SqlCommand deletecmd = new SqlCommand("", currConn, transaction);


                string delete = @"
Delete  ps from ProductStockMISs as ps
left outer join  Products p on p.ItemNo=ps.ItemNo
where p.ReportType='VAT6_1_And_6_2' and ps.StockType='VAT6_1' and ps.Remarks='Opening'

";
                deletecmd = new SqlCommand(delete, currConn, transaction);

                deletecmd.ExecuteNonQuery();

                #endregion


                #region Opening Date

                SqlCommand cmd = new SqlCommand("", currConn, transaction);

                cmd.CommandText = @" select top 1 TransactionDate from ProductStockMISs where TransType<>'Opening'
order by TransactionDate
";
                var exec = cmd.ExecuteScalar();
                string OpeningDate = "1900-Jan-01";
                if (exec == null)
                {
                    OpeningDate = vm.StartDate;

                }

                if (Convert.ToDateTime(vm.ToDate) > Convert.ToDateTime("1900-Jan-01"))
                {
                    OpeningDate = vm.StartDate;
                }

                string update = @"
----------update ProductStockMISs set TransactionDate='1900/01/01' where TransType='Opening'

update ProductStockMISs set TransactionDate=@TransactionDate where TransType='Opening'

";
                cmd = new SqlCommand(update, currConn, transaction);
                cmd.Parameters.AddWithValue("@TransactionDate", OpeningDate);
                cmd.ExecuteNonQuery();

                #endregion


                #region Update Remarks

                cmd.CommandText = @" 
update ProductStockMISs set   Remarks=TransType
where 1=1 and Remarks='Other' and TransType='Receive'


update ProductStockMISs set   Remarks='Sale (Local)'
where 1=1 and Remarks='Other' and TransType='Sale'

update ProductStockMISs set   TransType='Sale'
where 1=1 and Remarks='RawSale'

update ProductStockMISs set   TransType='Receive', Remarks= 'Receive'
where 1=1 and TransType='Purchase'  and StockType='VAT6_2_1'

";
                if (vm.Branchwise)
                {
                    cmd.CommandText += @"
 update ProductStockMISs set   TransType='61out'
where 1=1 
and Remarks='Raw TransferIssue' 
and TransType='Issue'

update ProductStockMISs set   TransType='62In'
where 1=1 and Remarks='Raw TransferReceive' and TransType='Purchase'

update ProductStockMISs set   TransType='62out'
where 1=1 
and Remarks='Transfer Issue' 
and TransType='Sale'

update ProductStockMISs set   TransType='62In'
where 1=1 and Remarks='Transfer Receive' and TransType='Receive'

";
                }
                cmd.CommandText += @" update ProductStockMISs set ClosingQuantity=0,ClosingAmount=0,ClosingAmountFinal=0
,ClosingQuantityFinal=0,AdjustmentValue=0
where ItemNo in(
select itemno from Products
left outer join ProductCategories
on Products.CategoryID=ProductCategories.CategoryID
where ProductCategories.IsRaw in('NonInventory','Overhead','Service(NonStock)'))


";

                cmd.ExecuteNonQuery();


                #endregion


                #region Select Data


                string getDetails = @"

SELECT OpeningQuantity
     , OpeningValue
     , pc.IsRaw ProductType
     , pc.CategoryName
     , p.ProductName
     , p.ProductCode
     , p.ItemNo
     --,TransactionDate,TransID,TransType,Remarks,s.Quantity,ClosingAmount Unitcost,ClosingQuantity,ClosingAmount
     , TransactionDate
     , TransID
     , TransType
     , Remarks
     , s.Quantity
     , Unitcost
     , ClosingQuantity
     , ClosingAmount
     , ClosingQuantityFinal
     , ClosingAmountFinal
     , AdjustmentValue
FROM ProductStockMISs AS s
         LEFT OUTER JOIN Products p ON s.ItemNo = p.ItemNo
         LEFT OUTER JOIN ProductCategories pc ON p.CategoryID = pc.CategoryID
--- ORDER BY p.ProductName, TransactionDate

";


                if (!string.IsNullOrWhiteSpace(vm.OrderBy))
                {

                    getDetails += " order by " + " p." + vm.OrderBy + ",TransactionDate";

                }
                else
                {
                    getDetails += "ORDER BY p.ProductName, TransactionDate ";

                }




                string getStockInfo = @"

UPDATE ProductStockMISs
SET OpeningQuantity = TempStock.OpeningQty,
    OpeningValue    = TempStock.ClosingAmount
FROM (SELECT Id
           , ItemNo
           , LAG(ClosingQuantity, 1, 0) OVER (PARTITION BY ItemNo ORDER BY ItemNo, TransactionDate, Id) OpeningQty
           , LAG(ClosingAmount, 1, 0) OVER (PARTITION BY ItemNo ORDER BY ItemNo, TransactionDate, Id)   ClosingAmount
           , Quantity
           , ClosingQuantity

      FROM ProductStockMISs) AS TempStock
         INNER JOIN ProductStockMISs p ON p.ItemNo = TempStock.ItemNo AND p.Id = TempStock.Id

UPDATE ProductStockMISs
SET OpeningQuantity = ClosingQuantity,
    OpeningValue    = ClosingAmount
WHERE TransType = 'Opening'



@detailstext


UPDATE ProductStockMISs
SET closingAMountFinal= 0,
    ClosingQuantityFinal= 0

UPDATE ProductStockMISs
SET closingAMountFinal= a.closingAmount,
    ClosingQuantityFinal= a.ClosingQuantity,
    AdjustmentValue=a.AdjustmentValue
FROM (SELECT ProductStockMISs.*
      FROM ProductStockMISs,
           (SELECT DISTINCT ItemNo, MAX(Id) id
            FROM ProductStockMISs
            GROUP BY Itemno) a
      WHERE ProductStockMISs.ItemNo = a.ItemNo
        AND ProductStockMISs.Id = a.id) AS a
WHERE ProductStockMISs.ID = a.Id

UPDATE ProductStockMISs
SET AdjustmentValue = 0
WHERE closingAMountFinal = 0
  AND ClosingQuantityFinal = 0


-------------------------------------------------- Summary --------------------------------------------------
-------------------------------------------------------------------------------------------------------------
SELECT pc.IsRaw                                ProductType
     , pc.CategoryName
     , p.ProductName
     , p.ProductCode
     , p.ItemNo
     , s.OpeningQuantity
     , s.OpeningValue
     , s.PurchaseQuantity
     , s.PurchaseValue
     , s.IssueQuantity
     , s.IssueValue
     , s.ReceiveQuantity
     , s.ReceiveValue
     , s.SaleQuantity
     , s.SaleValue
     , s.TransferInQuantity
     , s.TransferInValue
     , s.TransferOutQuantity
     , s.TransferOutValue
     , s.ClosingQuantity1 - s.ClosingQuantity2 ClosingQuantity
     , s.ClosingValue1 - s.ClosingValue2       ClosingValue
     , (ISNULL(AdjustmentValue, 0))            AdjustmentValue
FROM (SELECT DISTINCT ItemNo
                    , SUM(CASE WHEN TransType = 'Opening' THEN OpeningQuantity ELSE 0 END)           OpeningQuantity
                    , SUM(CASE WHEN TransType = 'Opening' THEN OpeningValue ELSE 0 END)           OpeningValue
                    , SUM(CASE WHEN TransType = 'Purchase' THEN Quantity ELSE 0 END)          PurchaseQuantity
                    , SUM(CASE WHEN TransType = 'Purchase' THEN UnitCost ELSE 0 END)          PurchaseValue
                    , SUM(CASE WHEN TransType = 'Issue' THEN Quantity ELSE 0 END)             IssueQuantity
                    , SUM(CASE WHEN TransType = 'Issue' THEN UnitCost ELSE 0 END)             IssueValue
                    , SUM(CASE WHEN TransType = 'Receive' THEN Quantity ELSE 0 END)           ReceiveQuantity
                    , SUM(CASE WHEN TransType = 'Receive' THEN UnitCost ELSE 0 END)           ReceiveValue
                    , SUM(CASE WHEN TransType = 'Sale' THEN Quantity ELSE 0 END)              SaleQuantity
                    , SUM(CASE WHEN TransType = 'Sale' THEN UnitCost ELSE 0 END)              SaleValue
                    , SUM(CASE WHEN TransType IN ('62In', '61In') THEN Quantity ELSE 0 END)   TransferInQuantity
                    , SUM(CASE WHEN TransType IN ('62In', '61In') THEN UnitCost ELSE 0 END)   TransferInValue
                    , SUM(CASE WHEN TransType IN ('62Out', '61Out') THEN Quantity ELSE 0 END) TransferOutQuantity
                    , SUM(CASE WHEN TransType IN ('62Out', '61Out') THEN UnitCost ELSE 0 END) TransferOutValue
                    , SUM(ClosingQuantityFinal)                                               ClosingQuantity1
                    , SUM(ClosingAmountFinal)                                                 ClosingValue1
                    , 0                                                                       ClosingQuantity2
                    , 0                                                                       ClosingValue2
                    --,sum(case when TransType in('Issue','Sale','62Out','61Out') then  Quantity else 0 end )ClosingQuantity2
                    --,sum(case when TransType in('Issue','Sale','62Out','61Out') then  UnitCost else 0 end )ClosingValue2
                    , SUM(ISNULL(AdjustmentValue, 0))                                         AdjustmentValue

      FROM ProductStockMISs
      GROUP BY ItemNo) AS s
         LEFT OUTER JOIN Products p ON s.ItemNo = p.ItemNo
         LEFT OUTER JOIN ProductCategories pc ON p.CategoryID = pc.CategoryID
";

                //Company = "";


                if (!string.IsNullOrWhiteSpace(vm.OrderBy))
                {

                    getStockInfo += " order by " + " p." + vm.OrderBy;

                }
                else
                {
                    getStockInfo += "ORDER BY p.ProductName";

                }



                if (string.Equals(Company, "Y", StringComparison.OrdinalIgnoreCase) && vm.ExcelRptDetail)
                {
                    getStockInfo = getStockInfo.Replace("@detailstext", "");
                }
                else
                {
                    getStockInfo = getStockInfo.Replace("@detailstext", getDetails);
                }

                cmd.CommandText = getStockInfo;


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(retResults);

                #endregion

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "StockMovement", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "StockMovement", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

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

        public DataSet StockMovementMPL(StockMovementVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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


 
 SELECT distinct isnull( z.ZoneCode,'NA')ZoneCode ,isnull( bp.BranchCode,'NA')BranchCode ,isnull(pc.CategoryName,'NA')CategoryName  ,isnull(p.ProductCode,'NA')ProductCode
,sum(isnull(Opening.Opening,0))Opening,sum(isnull(b.Purchase,0))Purchase,sum(isnull(b.TransferIn,0))TransferIn
,sum(isnull(b.TransferOut,0))TransferOut,sum(isnull(b.Sale,0))Sale,sum(isnull(b.Transit,0))Transit from (
select distinct BranchId, ItemNo, sum(isnull(StockQuantity,0))Opening
from ProductStocks
group by BranchId, ItemNo
union all
select distinct BranchId, ItemNo , sum(isnull(UOMQty,0))Purchase
from PurchaseInvoiceDetails
where ReceiveDate <@DateFrom  
group by BranchId, ItemNo
union all
select distinct BranchId, ItemNo,sum(isnull(UOMQty,0)) TransferIn 
from TransferMPLReceiveDetails
where TransactionType in('62In') 
and TransferDateTime <@DateFrom  
group by BranchId, ItemNo
union all

select distinct BranchId, ItemNo,-1* sum(isnull(UOMQty,0)) TransferOut
from TransferMPLIssueDetails
where TransactionType in('62Out') 
and TransferDateTime <@DateFrom  
group by BranchId, ItemNo
union all

select distinct BranchId, ItemNo,-1* sum(isnull(UOMQty,0)) Sale
from SalesInvoiceDetails
where 1=1 
and InvoiceDateTime <@DateFrom  
group by BranchId, ItemNo
) as Opening
left outer join (

select BranchId, ItemNo,   sum(isnull(Purchase,0))Purchase,  sum(isnull(TransferIn,0))TransferIn
,   sum(isnull(TransferOut,0))TransferOut,   sum(isnull(Sale,0))Sale,  sum(isnull(Transit,0))Transit From(
select distinct BranchId, ItemNo, sum(isnull(UOMQty,0))Purchase,0 TransferIn, 0 TransferOut, 0 Sale,0 Transit
from PurchaseInvoiceDetails
where ReceiveDate >=@DateFrom and ReceiveDate< DATEADD(d,1,@DateTo)
group by BranchId, ItemNo
union all
select distinct BranchId, ItemNo, 0 Purchase,sum(isnull(UOMQty,0)) TransferIn, 0 TransferOut, 0 Sale,0 Transit
from TransferMPLReceiveDetails
where TransactionType in('62In') 
and TransferDateTime >=@DateFrom and TransferDateTime< DATEADD(d,1,@DateTo)
group by BranchId, ItemNo
union all

select distinct BranchId, ItemNo, 0 Purchase,0 TransferIn, sum(isnull(UOMQty,0)) TransferOut, 0 Sale,0 Transit
from TransferMPLIssueDetails
where TransactionType in('62Out') 
and TransferDateTime >=@DateFrom and TransferDateTime< DATEADD(d,1,@DateTo)
group by BranchId, ItemNo
union all

select distinct BranchId, ItemNo, 0 Purchase,0 TransferIn, 0 TransferOut, sum(isnull(UOMQty,0)) Sale,0 Transit
from SalesInvoiceDetails
where 1=1 
and InvoiceDateTime >=@DateFrom and InvoiceDateTime< DATEADD(d,1,@DateTo)
group by BranchId, ItemNo
union all

select distinct TransferTo BranchId, ItemNo, 0 Purchase,0 TransferIn, 0 TransferOut, 0 Sale
,sum(isnull(case when isnull(IsReceiveCompleted,'N') not in('Y') then  (isnull(UOMQty,0)-isnull(receivedQuantity,0)) else 0 end ,0)) Transit
from TransferMPLIssueDetails
where TransactionType in('62Out') 
and TransferDateTime< DATEADD(d,1,@DateTo)
group by TransferTo, ItemNo
) as a
group by BranchId, ItemNo
) as b on Opening.BranchId=b.BranchId and Opening.ItemNo=b.ItemNo
left outer join BranchProfiles bp on Opening.BranchId=bp.BranchId
left outer join Products p on Opening.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
left outer join MPLZoneBranchMapping zm on Opening.BranchId=zm.BranchId
left outer join MPLZoneProfiles z on zm.ZoneId=z.ZoneID
WHERE 1 = 1 
";
                if (!string.IsNullOrEmpty(vm.CategoryId))
                {
                    sqlText += @" AND  pc.CategoryID= @CategoryId ";
                }
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += @" AND  p.ItemNo= @ItemNo ";
                }
                if (!string.IsNullOrEmpty(vm.ZoneID))
                {
                    sqlText += @" AND  zm.ZoneId= @ZoneID ";
                }
                if (vm.BranchId > 0 && vm.BranchId != -1)
                {
                    sqlText += @" AND  zm.BranchId= @BranchId ";
                }

                sqlText += @" group by z.ZoneCode , bp.BranchCode ,pc.CategoryName, p.ProductCode ";


                if (vm.WithOutZero)
                {
                    sqlText += @" 
having 
sum(isnull(Opening.Opening,0))>0 or sum(isnull(b.Purchase,0))>0 or sum(isnull(b.TransferIn,0))>0
or sum(isnull(b.TransferOut,0))>0 or sum(isnull(b.Sale,0))>0 or sum(isnull(b.Transit,0))>0 ";

                }

                SqlCommand cmdStockMovementMPL = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@CategoryId", SqlDbType.VarChar, 20);
                parameter.Value = vm.CategoryId;
                cmdStockMovementMPL.Parameters.Add(parameter);

                parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = vm.ItemNo;
                cmdStockMovementMPL.Parameters.Add(parameter);

                parameter = new SqlParameter("@ZoneID", SqlDbType.Int);
                parameter.Value = vm.ZoneID;
                cmdStockMovementMPL.Parameters.Add(parameter);

                parameter = new SqlParameter("@BranchId", SqlDbType.Int);
                parameter.Value = vm.BranchId;
                cmdStockMovementMPL.Parameters.Add(parameter);


                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;


                if (!string.IsNullOrEmpty(vm.StartDate))
                {
                    da.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.StartDate);
                }
                if (!string.IsNullOrEmpty(vm.ToDate))
                {
                    da.SelectCommand.Parameters.AddWithValue("@DateTo", vm.ToDate);
                }

                da.Fill(ds);

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
                FileLogger.Log("ProductDAL", "StockMovementMPL", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "StockMovementMPL", ex.ToString() + "\n" + sqlText);

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
            return ds;
        }

        public DataSet TollStockMovement(StockMovementVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ


            string sqlText = "";
            DataSet retResults = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation

                if (Convert.ToDateTime(vm.StartDate) < DateTime.MinValue || Convert.ToDateTime(vm.StartDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

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

                #region Variables

                CommonDAL _CDal = new CommonDAL();

                DataTable dt = new DataTable();
                DataSet AvgPriceVAT16 = new DataSet();
                DataSet AvgPriceVAT17 = new DataSet();


                ProductDAL productDal = new ProductDAL();
                ReportDSDAL _reportDal = new ReportDSDAL();
                TransferIssueDAL _treportDal = new TransferIssueDAL();

                #endregion

                #region Reset Data

                var result = _CDal.ExecuteQuery("delete from ProductStockMISs", currConn, transaction, connVM);

                #endregion

                SqlCommand command = new SqlCommand("", currConn, transaction);

                #region VAT 6.1 (Purchase Register)

                #region Parmeter Assign (VAT 6.1)

                VAT6_1ParamVM vat61ParamVm = new VAT6_1ParamVM();

                vat61ParamVm.ItemNo = vm.ItemNo;
                vat61ParamVm.UserName = vm.CurrentUserName;
                vat61ParamVm.StartDate = vm.StartDate;
                vat61ParamVm.EndDate = vm.ToDate;
                vat61ParamVm.Post1 = vm.Post1;
                vat61ParamVm.Post2 = vm.Post2;
                vat61ParamVm.ReportName = "";
                vat61ParamVm.BranchId = vm.BranchId;
                vat61ParamVm.Opening = true;
                vat61ParamVm.OpeningFromProduct = true;
                vat61ParamVm.ProdutType = vm.ProductType;
                vat61ParamVm.ProdutCategoryId = vm.CategoryId;
                vat61ParamVm.VAT6_2_1 = false;
                vat61ParamVm.StockMovement = true;
                vat61ParamVm.ProdutGroupName = vm.ProductGroupName;
                vat61ParamVm.ProdutCategoryLike = vm.CategoryLike;
                vat61ParamVm.UserId = vm.CurrentUserID;
                vat61ParamVm.BranchWise = vm.Branchwise;
                vat61ParamVm.VAT6_2_1 = vm.chkTrading == "Y" ? true : false;

                #endregion

                #region Comments

                #endregion

                AvgPriceVAT16 = _reportDal.VAT6_1Toll(vat61ParamVm.ItemNo, vat61ParamVm.UserName,
                    vat61ParamVm.StartDate,
                    vat61ParamVm.EndDate, vat61ParamVm.Post1, vat61ParamVm.Post2, "",
                    vat61ParamVm.BranchId, connVM, true, currConn, transaction, vat61ParamVm);

                string[] DeleteColumnName =
                {
                     "InvoiceDateTime", "StartingQuantity", "StartingAmount", "VendorName", "Address1",
                    "Address2", "Address3", "VATRegistrationNo", "ProductName", "ProductCodeA", "UOM", "VATRate", "SD",
                    "HSCodeNo", "BENumber", "CreateDateTime", "AvgRate", 
                    "RunningOpeningQuantity", "RunningOpeningValue","VendorId"
                };


                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT16.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "StartDateTime", "TransactionDate");


                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_1(Toll)", "string");
                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");
                string[] tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction, 10000, null, connVM);


                #region Parmeter Assign (6.2)

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                varVAT6_2ParamVM.ItemNo = vm.ItemNo;
                varVAT6_2ParamVM.StartDate = vm.StartDate;
                varVAT6_2ParamVM.EndDate = vm.ToDate;
                varVAT6_2ParamVM.Post1 = vm.Post1;
                varVAT6_2ParamVM.Post2 = vm.Post2;
                varVAT6_2ParamVM.BranchId = vm.BranchId;
                varVAT6_2ParamVM.Opening = false;
                varVAT6_2ParamVM.Opening6_2 = false;
                varVAT6_2ParamVM.ProdutType = vm.ProductType;
                varVAT6_2ParamVM.ProdutCategoryId = vm.CategoryId;
                varVAT6_2ParamVM.ProdutGroupName = vm.ProductGroupName;
                varVAT6_2ParamVM.ProdutCategoryLike = vm.CategoryLike;
                varVAT6_2ParamVM.StockMovement = true;
                varVAT6_2ParamVM.UserId = vm.CurrentUserID;
                varVAT6_2ParamVM.BranchWise = vm.Branchwise;
                varVAT6_2ParamVM.VAT6_2_1 = vm.chkTrading == "Y" ? true : false;

                #endregion

                #endregion

                #region VAT 6.2 (Sale Register)


                AvgPriceVAT17 = _reportDal.VAT6_2Toll(varVAT6_2ParamVM.ItemNo, varVAT6_2ParamVM.StartDate,
                    varVAT6_2ParamVM.EndDate, varVAT6_2ParamVM.Post1, varVAT6_2ParamVM.Post2, varVAT6_2ParamVM.BranchId,
                    connVM, currConn, transaction, varVAT6_2ParamVM);

                DeleteColumnName = new string[]
                {
                     "StartingQuantity", "StartingAmount", "CustomerName", "Address1", "Address2",
                    "Address3", "VATRegistrationNo", "ProductName", "ProductCode", "UOM", "VATRate", "SD", "HSCodeNo",
                    "BENumber", "CreatedDateTime", "UnitRate", "ClosingRate", "DeclaredPrice", "RunningTotalValue",
                    "RunningOpeningValueFinal", "RunningOpeningQuantityFinal"
                };
                dt = new DataTable();
                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT17.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "StartDateTime", "TransactionDate");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "remarks", "Remarks");


                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_2(Toll)", "string");
                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");

                tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction, 10000, null, connVM);

                #endregion




                string stockUpdateQuries = @"


--insert into ProductStockMISs
--(
--[TransID]
--      ,[TransType]
--      ,[Quantity]
--      ,[UnitCost]
--      ,[ItemNo]
--      ,[TransactionDate]
--      ,[Remarks]
--      ,[StockType]
--      ,[AdjustmentValue]
--      ,[Day]
--      ,[ClosingQuantity]
--      ,[ClosingAmount]
--      ,[ClosingAmountFinal]
--      ,[ClosingQuantityFinal]
--      ,[PeriodName]
--      ,[SerialNo]
--      ,[OpeningQuantity]
--      ,[OpeningValue])
--
--
--
--SELECT 0 [TransID]
--      ,'Opening1' [TransType]
--      ,avg([Quantity])
--      ,avg([UnitCost])
--      ,[ItemNo]
--      ,min(Format([TransactionDate],'yyyy-MM-dd 00:00:00'))
--      ,'Opening1'[Remarks]
--      ,'Opening1'[StockType]
--      ,avg([AdjustmentValue])
--      ,null [Day]
--      ,avg([ClosingQuantity])
--      ,avg([ClosingAmount])
--      ,avg([ClosingAmountFinal])
--      ,avg([ClosingQuantityFinal])
--      ,null [PeriodName]
--      ,'A'[SerialNo]
--      ,avg([OpeningQuantity])
--      ,avg([OpeningValue])
--  FROM [ProductStockMISs]
--  where TransType = 'Opening'
--  group by ItemNo
--
--
--  delete from ProductStockMISs 
--  where TransType = 'Opening'
--
--
--  update ProductStockMISs set TransType = 'Opening', Remarks = 'Opening'
--  where TransType = 'Opening1'


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9)) -- ,BranchId int

insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost)
select Id,ItemNo,TransType,Quantity,UnitCost from ProductStockMISs
where 1=1 
order by  ItemNo,TransactionDate,SerialNo

update ProductStockMISs set  ClosingQuantity=RT.ClosingQuantity
from (SELECT id, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Issue','Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY SL) AS ClosingQuantity
FROM #Temp)RT
where RT.Id=ProductStockMISs.Id


";

                SqlCommand cmd = new SqlCommand(stockUpdateQuries, currConn, transaction);
                cmd.ExecuteNonQuery();


                string getData = @"


update ProductStockMISs set OpeningQuantity = Quantity, OpeningValue = UnitCost
where TransType = 'Opening'

update ProductStockMISs set OpeningQuantity = TempStock.OpeningQty, OpeningValue = TempStock.ClosingAmount
from 
(

select Id,ItemNo
,Lag(ClosingQuantity,1,0) over (partition by ItemNo order by ItemNo, TransactionDate, Id)OpeningQty
,Lag(ClosingAmount,1,0) over (partition by ItemNo order by ItemNo, TransactionDate, Id)ClosingAmount
, Quantity
, ClosingQuantity

from ProductStockMISs
) as TempStock
inner join ProductStockMISs p on p.ItemNo = TempStock.ItemNo and p.Id = TempStock.Id



select pc.IsRaw ProductType,pc.CategoryName,p.ProductName,p.ProductCode,p.ItemNo
 ,TransactionDate,s.OpeningQuantity,TransID,TransType,Remarks,Quantity,ClosingQuantity

 from ProductStockMISs as s left outer join Products p on  s.ItemNo=p.ItemNo
    left outer join ProductCategories pc on  p.CategoryID=pc.CategoryID
	order by p.ProductName,TransactionDate,s.SerialNo



update ProductStockMISs set closingAMountFinal= 0,ClosingQuantityFinal= 0

update ProductStockMISs set closingAMountFinal= a.closingAmount,ClosingQuantityFinal= a.ClosingQuantity,AdjustmentValue=a.AdjustmentValue
from (
select ProductStockMISs.* from  ProductStockMISs,(select distinct ItemNo , max(Id)id from ProductStockMISs
where TransType != 'Opening'
group by Itemno)a where ProductStockMISs.ItemNo=a.ItemNo and ProductStockMISs.Id = a.id
) as a
 where ProductStockMISs.ID=a.Id

update ProductStockMISs set AdjustmentValue = 0 where closingAMountFinal = 0 and ClosingQuantityFinal = 0

-------------------------------------------------- Summary --------------------------------------------------
-------------------------------------------------------------------------------------------------------------
SELECT pc.IsRaw ProductType,pc.CategoryName,p.ProductName,p.ProductCode,p.ItemNo
,s.OpeningQuantity
,s.PurchaseQuantity
,s.IssueQuantity
,s.ReceiveQuantity
,s.SaleQuantity
,s.TransferInQuantity
,s.TransferOutQuantity
,s.ClosingQuantity1-s.ClosingQuantity2 ClosingQuantity

 from (

SELECT distinct ItemNo
	,sum(case when TransType='Opening'  then  Quantity else 0 end )OpeningQuantity
	,sum(case when TransType='Opening'  then  UnitCost else 0 end )OpeningValue
	,sum(case when TransType='Purchase' then  Quantity else 0 end )PurchaseQuantity
	,sum(case when TransType='Purchase' then  UnitCost else 0 end )PurchaseValue
	,sum(case when TransType='Issue' then  Quantity else 0 end )IssueQuantity
	,sum(case when TransType='Issue' then  UnitCost else 0 end )IssueValue
	,sum(case when TransType='Receive' then  Quantity else 0 end )ReceiveQuantity
	,sum(case when TransType='Receive' then  UnitCost else 0 end )ReceiveValue
	,sum(case when TransType='Sale' then  Quantity else 0 end )SaleQuantity
	,sum(case when TransType='Sale' then  UnitCost else 0 end )SaleValue
	,sum(case when TransType in('62In','61In') then  Quantity else 0 end )TransferInQuantity
	,sum(case when TransType in('62In','61In') then  UnitCost else 0 end )TransferInValue
	,sum(case when TransType in('62Out','61Out') then  Quantity else 0 end )TransferOutQuantity
	,sum(case when TransType in('62Out','61Out') then  UnitCost else 0 end )TransferOutValue
	,sum(ClosingQuantityFinal) ClosingQuantity1
	,sum(ClosingAmountFinal) ClosingValue1
    ,0 ClosingQuantity2
    ,0 ClosingValue2
	--,sum(case when TransType in('Issue','Sale','62Out','61Out') then  Quantity else 0 end )ClosingQuantity2
	--,sum(case when TransType in('Issue','Sale','62Out','61Out') then  UnitCost else 0 end )ClosingValue2
    ,sum(isnull(AdjustmentValue,0))AdjustmentValue

	from ProductStockMISs  
	group by ItemNo

) as   s 
    left outer join Products p on  s.ItemNo=p.ItemNo
    left outer join ProductCategories pc on  p.CategoryID=pc.CategoryID

";
                cmd.CommandText = getData;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(retResults);

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "StockMovement", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
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
        public DataSet StockMovement6_2_1(string itemNo, string tranDate, string tranDateTo, int BranchId, SqlConnection VcurrConn
  , SqlTransaction Vtransaction, string Post1, string Post2, string ProdutType, string ProdutCategoryId, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ

            string sqlText = "";
            DataSet retResults = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(itemNo))
                //{
                //    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                //}
                //else 
                if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

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

                #region Variables

                DataSet AvgPriceVAT16 = new DataSet();
                DataSet AvgPriceVAT17 = new DataSet();
                DataSet transferStock = new DataSet();
                DataSet transferIssue = new DataSet();
                DataSet transferReceive = new DataSet();

                ProductDAL productDal = new ProductDAL();
                ReportDSDAL _reportDal = new ReportDSDAL();
                TransferIssueDAL _treportDal = new TransferIssueDAL();

                #endregion

                CommonDAL _CDal = new CommonDAL();

                _CDal.ExecuteQuery("delete from ProductStockMISKas", currConn, transaction, connVM);

                //////currConn.Close();


                DataTable dt = new DataTable();

                AvgPriceVAT16 = new DataSet();
                AvgPriceVAT17 = new DataSet();
                transferIssue = new DataSet();
                transferReceive = new DataSet();

                #region Parmeter Assign (VAT 6.1)

                VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

                varVAT6_1ParamVM.ItemNo = itemNo;
                varVAT6_1ParamVM.UserName = "";
                varVAT6_1ParamVM.StartDate = tranDate;
                varVAT6_1ParamVM.EndDate = tranDateTo;
                varVAT6_1ParamVM.Post1 = Post1;
                varVAT6_1ParamVM.Post2 = Post2;
                varVAT6_1ParamVM.ReportName = "";
                varVAT6_1ParamVM.BranchId = BranchId;
                varVAT6_1ParamVM.Opening = false;
                varVAT6_1ParamVM.OpeningFromProduct = true;
                varVAT6_1ParamVM.ProdutType = ProdutType;
                varVAT6_1ParamVM.ProdutCategoryId = ProdutCategoryId;
                varVAT6_1ParamVM.VAT6_2_1 = true;
                varVAT6_1ParamVM.UserId = UserId;
                varVAT6_1ParamVM.PermanentProcess = true;

                #endregion

                AvgPriceVAT16 = _vatRegistersDAL.VAT6_1_WithConn(varVAT6_1ParamVM);

                string[] DeleteColumnName = { "CreateDateTime", "ProductCodeA", "UOM", "AvgRate", "RunningTotal", "RunningValue", "RunningOpeningQuantity", "RunningOpeningValue" };

                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT16.Tables[0], DeleteColumnName);
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "StartDateTime", "TransactionDate");

                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_1", "string");

                List<string> columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToList();

                string columnList = string.Join("\n", columnNames);

                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");

                string[] tt = _CDal.BulkInsert("ProductStockMISKas", dt, currConn, transaction, 10000, null, connVM);

                #region Parmeter Assign (VAT 6.2)

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                varVAT6_2ParamVM.ItemNo = itemNo;
                varVAT6_2ParamVM.StartDate = tranDate;
                varVAT6_2ParamVM.EndDate = tranDateTo;
                varVAT6_2ParamVM.Post1 = Post1;
                varVAT6_2ParamVM.Post2 = Post2;
                varVAT6_2ParamVM.BranchId = BranchId;
                varVAT6_2ParamVM.Opening = false;
                varVAT6_2ParamVM.Opening6_2 = false;
                varVAT6_2ParamVM.ProdutType = ProdutType;
                varVAT6_2ParamVM.ProdutCategoryId = ProdutCategoryId;
                varVAT6_2ParamVM.VAT6_2_1 = true;
                varVAT6_2ParamVM.UserId = UserId;
                varVAT6_2ParamVM.MainProcess = true;



                #endregion

                AvgPriceVAT17 = _vatRegistersDAL.VAT6_2(varVAT6_2ParamVM, currConn, transaction, connVM);

                DeleteColumnName = new string[] { "CreatedDateTime", "UnitRate", "ProductCode", "UOM", "AdjustmentValue", "ClosingRate", "RunningTotal", "DeclaredPrice", "RunningTotalValue", "RunningTotalValueFinal", "RunningOpeningValueFinal", "RunningOpeningQuantityFinal" };
                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT17.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "CustomerName", "VendorName");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "remarks", "Remarks");

                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_2", "string");

                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");

                tt = _CDal.BulkInsert("ProductStockMISKas", dt, currConn, transaction, 10000, null, connVM);

                #region Comments / Sep-01-2020

                //transferIssue = TransferIssue(itemNo, tranDate, tranDateTo, Post1, Post2, BranchId, false, currConn,
                //    transaction, null, ProdutType, ProdutCategoryId);

                //DeleteColumnName = new string[] { "SL" };
                //dt = new DataTable();
                //dt = OrdinaryVATDesktop.DtDeleteColumns(transferIssue.Tables[0], DeleteColumnName);
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionDateTime", "TransactionDate");
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "Amount", "UnitCost");
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionType", "TransType");
                //dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "transferIssue", "string");
                //tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction);


                //transferReceive = TransferReceive(itemNo, tranDate, tranDateTo, Post1, Post2, BranchId, false, currConn, transaction, null, ProdutType, ProdutCategoryId);
                //DeleteColumnName = new string[] { "SL" };
                //dt = new DataTable();
                //dt = OrdinaryVATDesktop.DtDeleteColumns(transferReceive.Tables[0], DeleteColumnName);
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionDateTime", "TransactionDate");
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "Amount", "UnitCost");
                //dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "TransactionType", "TransType");
                //dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "TransferReceive", "string");
                //tt = _CDal.BulkInsert("ProductStockMISs", dt, currConn, transaction);

                #endregion

                sqlText = @"

update  ProductStockMISKas set Quantity=a.Quantity, UnitCost = a.UnitCost
from
(
select SUM(Quantity) Quantity, SUM(UnitCost) UnitCost from ProductStockMISKas
where 1=1 and TransID='0'
) as a
where 1=1 and StockType in('VAT6_2') and TransID='0'



delete from ProductStockMISKas where StockType in('VAT6_1') and TransType in('Opening')



----select mis.*, pih.BENumber 
update ProductStockMISKas set BENumber=pih.BENumber
--, InvoiceDateTime=StartDateTime
from ProductStockMISKas mis
left outer join PurchaseInvoiceHeaders pih on mis.TransID=pih.PurchaseInvoiceNo
where 1=1 and TransType='Purchase'



update ProductStockMISKas set BENumber= TransID, InvoiceDateTime=StartDateTime where 1=1 and TransType<>'Purchase' --------and StockType in('VAT6_2')


update ProductStockMISKas set StartingQuantity= Quantity, StartingAmount=UnitCost where 1=1 and StockType in('VAT6_2') and TransID='0'
update ProductStockMISKas set Remarks='Purchase' where Remarks like 'import'

";
                #region Comments / Sep-01-2020

                sqlText += @"
------    select pc.IsRaw ProductType,pc.CategoryName,p.ProductName, s.* from (
------    select distinct ItemNo,TransactionDate,TransID,TransType,Remarks,sum(Quantity)Quantity,sum(UnitCost)UnitCost,'Opening'StockType 
------    from ProductStockMISs where 1=1
------    --and stockType in('VAT6_1','VAT6_2') 
------    and transType in('Opening')
------
------    group by ItemNo,TransactionDate,TransID,TransType,Remarks
------    union all
------    select ItemNo,TransactionDate,TransID,TransType,Remarks,Quantity,UnitCost,StockType  from ProductStockMISs
------    where  id not in(select id from ProductStockMISs where 1=1 
------    --and stockType in('VAT6_1','VAT6_2') 
------    and transType in('Opening'))
------    ) as s 
------    left outer join Products p on  s.ItemNo=p.ItemNo
------    left outer join ProductCategories pc on  p.CategoryID=pc.CategoryID
------
-----------Summery
------	select pc.IsRaw ProductType,pc.CategoryName,p.ProductName,s.OpeningQuantity
------,s.OpeningValue
------,s.PurchaseQuantity
------,s.PurchaseValue
------,s.IssueQuantity
------,s.IssueValue
------,s.ReceiveQuantity
------,s.ReceiveValue
------,s.SaleQuantity
------,s.SaleValue
------,s.TransferInQuantity
------,s.TransferInValue
------,s.TransferOutQuantity
------,s.TransferOutValue
------,s.ClosingQuantity1-s.ClosingQuantity2 ClosingQuantity
------,s.ClosingValue1-s.ClosingValue2 ClosingValue
------ from (
------	SELECT distinct ItemNo
------	,sum(case when TransType='Opening' then  Quantity else 0 end )OpeningQuantity
------	,sum(case when TransType='Opening' then  UnitCost else 0 end )OpeningValue
------	,sum(case when TransType='Purchase' then  Quantity else 0 end )PurchaseQuantity
------	,sum(case when TransType='Purchase' then  UnitCost else 0 end )PurchaseValue
------	,sum(case when TransType='Issue' then  Quantity else 0 end )IssueQuantity
------	,sum(case when TransType='Issue' then  UnitCost else 0 end )IssueValue
------	,sum(case when TransType='Receive' then  Quantity else 0 end )ReceiveQuantity
------	,sum(case when TransType='Receive' then  UnitCost else 0 end )ReceiveValue
------	,sum(case when TransType='Sale' then  Quantity else 0 end )SaleQuantity
------	,sum(case when TransType='Sale' then  UnitCost else 0 end )SaleValue
------	,sum(case when TransType='62In' then  Quantity else 0 end )TransferInQuantity
------	,sum(case when TransType='62In' then  UnitCost else 0 end )TransferInValue
------	,sum(case when TransType='62Out' then  Quantity else 0 end )TransferOutQuantity
------	,sum(case when TransType='62Out' then  UnitCost else 0 end )TransferOutValue
------	,sum(case when TransType in('Opening','Purchase','Receive','62In') then  Quantity else 0 end )ClosingQuantity1
------	,sum(case when TransType in('Opening','Purchase','Receive','62In') then  UnitCost else 0 end )ClosingValue1
------
------	,sum(case when TransType in('Issue','Sale','62Out') then  Quantity else 0 end )ClosingQuantity2
------	,sum(case when TransType in('Issue','Sale','62Out') then  UnitCost else 0 end )ClosingValue2
------
------	from ProductStockMISs  
------	group by ItemNo
------	) as   s 
------    left outer join Products p on  s.ItemNo=p.ItemNo
------    left outer join ProductCategories pc on  p.CategoryID=pc.CategoryID
------
";
                #endregion

                sqlText += @"
select * from ProductStockMISKas
order by  StartDateTime,SerialNo 
";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(retResults);

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "StockMovement6_2_1", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "StockMovement6_2_1", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

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

        public string[] StockMovement6_2_1_PermanentProcess(VAT6_2ParamVM vm, SqlConnection VcurrConn = null
            , SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            DataSet retResults = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            string[] result = new string[10];
            #region Try
            try
            {

                #region Validation
                if (Convert.ToDateTime(vm.StartDate) < DateTime.MinValue || Convert.ToDateTime(vm.EndDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

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

                #region Variables

                DataSet AvgPriceVAT16 = new DataSet();
                DataSet AvgPriceVAT17 = new DataSet();
                DataSet transferStock = new DataSet();
                DataSet transferIssue = new DataSet();
                DataSet transferReceive = new DataSet();

                ProductDAL productDal = new ProductDAL();
                ReportDSDAL _reportDal = new ReportDSDAL();
                TransferIssueDAL _treportDal = new TransferIssueDAL();
                VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();
                #endregion

                CommonDAL _CDal = new CommonDAL();

                if (!vm.SkipOpening)
                {
                    _CDal.ExecuteQuery("delete from ProductStockMISKas where UserId='" + vm.UserId + "'", currConn, transaction, connVM);
                }


                DataTable dt = new DataTable();

                AvgPriceVAT16 = new DataSet();
                AvgPriceVAT17 = new DataSet();
                transferIssue = new DataSet();
                transferReceive = new DataSet();

                #region Parmeter Assign (VAT 6.1)

                VAT6_1ParamVM vat61ParamVm = new VAT6_1ParamVM();

                vat61ParamVm.ItemNo = vm.ItemNo;
                vat61ParamVm.UserName = "";
                vat61ParamVm.StartDate = vm.StartDate;
                vat61ParamVm.EndDate = vm.EndDate;
                vat61ParamVm.Post1 = vm.Post1;
                vat61ParamVm.Post2 = vm.Post2;
                vat61ParamVm.ReportName = "";
                vat61ParamVm.BranchId = vm.BranchId;
                vat61ParamVm.Opening = false;
                vat61ParamVm.OpeningFromProduct = true;
                vat61ParamVm.ProdutType = vm.ProdutType;
                vat61ParamVm.ProdutCategoryId = vm.ProdutCategoryId;
                vat61ParamVm.VAT6_2_1 = true;
                vat61ParamVm.UserId = vm.UserId;
                vat61ParamVm.Is6_1Permanent = true;
                vat61ParamVm.PermanentProcess = true;

                #endregion

                AvgPriceVAT16 = _vatRegistersDAL.VAT6_1_WithConn(vat61ParamVm, currConn, transaction, connVM);

                string[] DeleteColumnName = { "CreateDateTime", "ProductCodeA", "UOM", "AvgRate", "RunningTotal", "RunningValue", "RunningOpeningQuantity", "RunningOpeningValue" };

                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT16.Tables[0], DeleteColumnName);

                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_1", "string");

                List<string> columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToList();

                string columnList = string.Join("\n", columnNames);

                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");
                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "UserId", vm.UserId, "string");

                if (vm.SkipOpening)
                {
                    if (dt.Rows.Count > 1)
                    {
                        DataRow[] rows = dt.Select("TransType <> 'Opening'");

                        if (rows.Count() > 0)
                        {
                            dt = rows.CopyToDataTable();
                        }
                        else
                        {
                            dt.Clear();

                        }

                        ////////dataSetTable = dataSetTable.Select("TransType <> 'Opening' ").CopyToDataTable();
                    }
                    else
                    {
                        dt.Clear();
                    }
                }


                string[] tt = _CDal.BulkInsert("ProductStockMISKas", dt, currConn, transaction, 0, null, connVM);

                #region Parmeter Assign (VAT 6.2)

                VAT6_2ParamVM vat62ParamVm = new VAT6_2ParamVM();

                vat62ParamVm.ItemNo = vm.ItemNo;
                vat62ParamVm.StartDate = vm.StartDate;
                vat62ParamVm.EndDate = vm.EndDate;
                vat62ParamVm.Post1 = vm.Post1;
                vat62ParamVm.Post2 = vm.Post2;
                vat62ParamVm.BranchId = vm.BranchId;
                vat62ParamVm.Opening = false;
                vat62ParamVm.Opening6_2 = false;
                vat62ParamVm.ProdutType = vm.ProdutType;
                vat62ParamVm.ProdutCategoryId = vm.ProdutCategoryId;
                vat62ParamVm.VAT6_2_1 = true;
                vat62ParamVm.UserId = vm.UserId;
                vat62ParamVm.MainProcess = true;
                vat62ParamVm.PermanentProcess = vm.PermanentProcess;


                #endregion

                AvgPriceVAT17 = _vatRegistersDAL.VAT6_2(vat62ParamVm, currConn, transaction, connVM);

                DeleteColumnName = new string[]
                {
                    "CreatedDateTime", "UnitRate", "ProductCode", "UOM", "AdjustmentValue", "ClosingRate",
                    "RunningTotal", "DeclaredPrice", "RunningTotalValue", "RunningTotalValueFinal",
                    "RunningOpeningValueFinal", "RunningOpeningQuantityFinal","ReturnTransactionType"
                };
                dt = OrdinaryVATDesktop.DtDeleteColumns(AvgPriceVAT17.Tables[0], DeleteColumnName);
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "CustomerName", "VendorName");
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "remarks", "Remarks");

                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "StockType", "VAT6_2", "string");
                dt = OrdinaryVATDesktop.DtColumnAdd(dt, "UserId", vm.UserId, "string");

                OrdinaryVATDesktop.DtDeleteColumn(dt, "Day");


                if (vm.SkipOpening)
                {
                    if (dt.Rows.Count > 1)
                    {
                        DataRow[] rows = dt.Select("TransType <> 'Opening'");

                        if (rows.Count() > 0)
                        {
                            dt = rows.CopyToDataTable();
                        }
                        else
                        {
                            dt.Clear();

                        }

                    }
                    else
                    {
                        dt.Clear();
                    }
                }
                tt = _CDal.BulkInsert("ProductStockMISKas", dt, currConn, transaction, 0, null, connVM);

                sqlText = @"

--update  ProductStockMISKas set Quantity=a.Quantity, UnitCost = a.UnitCost
--from
--(
--select SUM(Quantity) Quantity, SUM(UnitCost) UnitCost from ProductStockMISKas
--where 1=1 and TransID='0'
--) as a
--where 1=1 and StockType in('VAT6_2') and TransID='0' and UserId=@UserId



update ProductStockMISKas set BENumber=pih.BENumber
--, InvoiceDateTime=StartDateTime
from ProductStockMISKas mis
left outer join PurchaseInvoiceHeaders pih on mis.TransID=pih.PurchaseInvoiceNo
where 1=1 and TransType='Purchase'



update ProductStockMISKas set BENumber= TransID, InvoiceDateTime=StartDateTime where 1=1 and TransType<>'Purchase' 
and UserId=@UserId

update ProductStockMISKas set BranchId=0 where 1=1 
and BranchId is null
and UserId=@UserId

update ProductStockMISKas set StartingQuantity= Quantity, StartingAmount=UnitCost where 1=1 and StockType in('VAT6_2') and TransID='0'
and UserId=@UserId

update ProductStockMISKas set Remarks='Purchase' where Remarks like 'import'
and UserId=@UserId

";


                //sqlText += Get6_2_1_PartitionQuery();

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                cmd.ExecuteNonQuery();

                if (false)
                {
                    transaction.Commit();
                }

                cmd.CommandText = Get6_2_1_PartitionQuery();

                cmd.ExecuteNonQuery();


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "StockMovement6_2_1", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
            }
            finally
            {


                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            #region Results

            return result;

            #endregion

        }

        private string Get6_2_1_PartitionQuery()
        {
            return @"

--declare @UserId varchar(50) = 10

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,8), EffectDate datetime,ToDate datetime, BranchId int)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,8),TotalCost decimal(25,8),BranchId int)

update ProductStockMISKas set CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.Salesinvoiceno=ProductStockMISKas.TransID and TransType='Sale' 
and UserId = @UserId

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null,0 ToDate from products
where ItemNo in(select distinct Itemno from ProductStockMISKas where 1=1 ) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,0 BranchId from BOMs
where FinishItemNo in(select distinct Itemno from ProductStockMISKas where 1=1 )
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 

----######################----------------
update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT Customerid,id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null  

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0
)RT
where RT.Id=#NBRPrive.Id  
and ToDate is null and isnull(nullif(customerid,''),0)<=0  
----######################----------------

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null 

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from ProductStockMISKas 
where 1=1 and UserId = @UserId
order by BranchId,ItemNo,StartDateTime,SerialNo


update ProductStockMISKas set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where 
RT.Id=ProductStockMISKas.Id
and RT.BranchId=ProductStockMISKas.BranchId
and UserId = @UserId

update ProductStockMISKas set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=ProductStockMISKas.Id
and RT.BranchId=ProductStockMISKas.BranchId
and UserId = @UserId

update ProductStockMISKas set DeclaredPrice =0 where 1=1 and UserId = @UserId

update ProductStockMISKas set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=ProductStockMISKas.ItemNo
and ProductStockMISKas.StartDateTime >=#NBRPrive.EffectDate and ProductStockMISKas.StartDateTime<#NBRPrive.ToDate
and ProductStockMISKas.CustomerID=#NBRPrive.CustomerId
and isnull(ProductStockMISKas.DeclaredPrice,0)=0
and UserId = @UserId

update ProductStockMISKas set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=ProductStockMISKas.ItemNo
and ProductStockMISKas.StartDateTime >=#NBRPrive.EffectDate and ProductStockMISKas.StartDateTime<#NBRPrive.ToDate
and isnull(ProductStockMISKas.DeclaredPrice,0)=0
and UserId = @UserId


update ProductStockMISKas set   RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 and UserId = @UserId
update ProductStockMISKas set AdjustmentValue=0 where 1=1 and UserId = @UserId
update ProductStockMISKas set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 and UserId = @UserId


update ProductStockMISKas set  RunningOpeningQuantityFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM ProductStockMISKas
)RT
where 
RT.Id=ProductStockMISKas.Id
and RT.BranchId=ProductStockMISKas.BranchId
and UserId = @UserId

 update ProductStockMISKas set  RunningOpeningValueFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotalValueFinal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM ProductStockMISKas
)RT
where 
RT.Id=ProductStockMISKas.Id
and RT.BranchId=ProductStockMISKas.BranchId
and UserId = @UserId


drop table #Temp
drop table #NBRPrive
";
        }

        public DataSet TransferIssue(string ItemNo, string StartDate, string EndDate,
            string post1, string post2, int BranchId = 0, bool Opening = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null, string ProdutType = "", string ProdutCategoryId = "", string gName = "", bool chkCategoryLike = false)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet dataSet = new DataSet("TransferIssue");
            //string ProdutType = "";
            //string ProdutCategoryId = "";
            #endregion

            #region Try

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


                #region SQL Statement

                sqlText = "";

                #region SQL Text

                sqlText += @"
                          
	--DECLARE @StartDate DATETIME;
	--DECLARE @EndDate DATETIME;
	--DECLARE @post1 VARCHAR(200);
	--DECLARE @post2 VARCHAR(200);
	--DECLARE @ItemNo VARCHAR(200);
    
	--SET @Itemno='46';
	--SET @post1='Y';
	--SET @post2='N';
	--SET @StartDate='2014-04-01';
	--SET @EndDate='2014-04-27';

   
  
";

                sqlText += @"  select * into #ProductReceive from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1

";
                if (!string.IsNullOrWhiteSpace(ProdutType))
                {
                    sqlText += @"  and IsRaw=@ProdutType";
                }
                else if (!string.IsNullOrWhiteSpace(ProdutCategoryId))
                {
                    if (chkCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @gName   + '%'";

                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }
                sqlText += @"  ) as a";


                sqlText += @"
  select * from (
select 'A'SL,'0' TransID,ItemNo,@StartDate TransactionDateTime ,Sum(UOMQty) Quantity,Sum(UOMQty*UOMPrice) Amount ,'Opening'TransactionType,'Opening'Remarks
from TransferIssueDetails as tt where 1=1
and ItemNo in(select distinct ItemNo from #ProductReceive)
and BranchId=@BranchId
and TransactionType in('61Out','62Out')and TransactionDateTime  <@StartDate  
AND (Post =@post1 or Post= @post2)
group by ItemNo
";

                if (Opening == false)
                {
                    sqlText += @"
union all
select 'B'SL,TransferIssueNo TransID,ItemNo,TransactionDateTime,UOMQty Quantity,UOMQty*UOMPrice Amount,TransactionType,TransactionType Remarks
from TransferIssueDetails where 1=1

and ItemNo in(select distinct ItemNo from #ProductReceive)

and TransactionType in('61Out','62Out')
and TransactionDateTime  >=@StartDate  and TransactionDateTime < DATEADD(d,1, @EndDate) 
and BranchId=@BranchId
AND (Post =@post1 or Post= @post2)

";
                }
                sqlText += @"
) as a
 order by TransactionDateTime,SL
 drop table #ProductReceive
                ";

                #endregion

                #endregion

                #region SQL Command
                if (BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }
                SqlCommand objCommVAT16 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT16.CommandTimeout = 500;
                #endregion

                #region Parameter
                objCommVAT16.Parameters.AddWithValue("@BranchId", BranchId);


                if (!string.IsNullOrWhiteSpace(ProdutType))
                {
                    objCommVAT16.Parameters.AddWithValue("@ProdutType", ProdutType);
                }
                else if (!string.IsNullOrWhiteSpace(ProdutCategoryId))
                {
                    if (chkCategoryLike == true)
                    {
                        objCommVAT16.Parameters.AddWithValue("@gName", gName);
                    }
                    else
                    {
                        objCommVAT16.Parameters.AddWithValue("@ProdutCategoryId", ProdutCategoryId);
                    }
                }

                else if (!string.IsNullOrWhiteSpace(ItemNo))
                {
                    objCommVAT16.Parameters.AddWithValue("@ItemNo", ItemNo);
                }


                if (StartDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", StartDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = StartDate;
                    }
                } // Common Filed
                if (EndDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", EndDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = EndDate;
                    }
                }

                if (!objCommVAT16.Parameters.Contains("@post1"))
                {
                    objCommVAT16.Parameters.AddWithValue("@post1", post1);
                }
                else
                {
                    objCommVAT16.Parameters["@post1"].Value = post1;
                }

                if (!objCommVAT16.Parameters.Contains("@post2"))
                {
                    objCommVAT16.Parameters.AddWithValue("@post2", post2);
                }
                else
                {
                    objCommVAT16.Parameters["@post2"].Value = post2;
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT16);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "TransferIssue", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "TransferIssue", ex.ToString() + "\n" + sqlText);

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

            return dataSet;
        }

        public DataSet TransferReceive(string ItemNo, string StartDate, string EndDate,
            string post1, string post2, int BranchId = 0, bool Opening = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string ProdutType = "", string ProdutCategoryId = "", string gName = "", bool chkCategoryLike = false)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet dataSet = new DataSet("TransferIssue");
            //string ProdutType = "";
            //string ProdutCategoryId = "";
            #endregion

            #region Try

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

                string top;


                #region SQL Statement

                sqlText = "";

                #region Backup

                sqlText += @"
                          
	--DECLARE @StartDate DATETIME;
	--DECLARE @EndDate DATETIME;
	--DECLARE @post1 VARCHAR(200);
	--DECLARE @post2 VARCHAR(200);
	--DECLARE @ItemNo VARCHAR(200);
    
	--SET @Itemno='46';
	--SET @post1='Y';
	--SET @post2='N';
	--SET @StartDate='2014-04-01';
	--SET @EndDate='2014-04-27';

    
   
";

                sqlText += @"  select * into #ProductReceive from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1

";
                if (!string.IsNullOrWhiteSpace(ProdutType))
                {
                    sqlText += @"  and IsRaw=@ProdutType";
                }
                else if (!string.IsNullOrWhiteSpace(ProdutCategoryId))
                {
                    if (chkCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @gName   + '%'";

                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }
                sqlText += @"  ) as a";


                sqlText += @"
 select * from (
select 'A'SL,'0' TransID,ItemNo,@StartDate TransactionDateTime ,Sum(UOMQty) Quantity,Sum(UOMQty*UOMPrice) Amount ,'Opening'TransactionType,'Opening' Remarks
from TransferReceiveDetails as tt where 1=1
and ItemNo in(select distinct ItemNo from #ProductReceive)
and BranchId=@BranchId
and TransactionType in('62In','61In')and TransactionDateTime  <@StartDate  
AND (Post =@post1 or Post= @post2)
group by ItemNo
";

                if (Opening == false)
                {
                    sqlText += @"
union all
select 'B'SL,TransferReceiveNo TransID,ItemNo,TransactionDateTime,UOMQty Quantity,UOMQty*UOMPrice Amount,TransactionType,TransactionType Remarks
from TransferReceiveDetails where 1=1

and ItemNo in(select distinct ItemNo from #ProductReceive)
and BranchId=@BranchId

and TransactionType in('62In','61In')
and TransactionDateTime  >=@StartDate  and TransactionDateTime < DATEADD(d,1, @EndDate) 
AND (Post =@post1 or Post= @post2)

";
                }
                sqlText += @" 
) as a
 order by TransactionDateTime,SL
drop table #ProductReceive
                ";

                #endregion
                top = "Go";

                #endregion

                #region SQL Command
                if (BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }
                SqlCommand objCommVAT16 = new SqlCommand(sqlText, currConn, transaction);

                #endregion

                #region Parameter
                objCommVAT16.Parameters.AddWithValue("@BranchId", BranchId);

                if (!string.IsNullOrWhiteSpace(ProdutType))
                {
                    objCommVAT16.Parameters.AddWithValue("@ProdutType", ProdutType);
                }
                else if (!string.IsNullOrWhiteSpace(ProdutCategoryId))
                {
                    if (chkCategoryLike == true)
                    {
                        objCommVAT16.Parameters.AddWithValue("@gName", gName);
                    }
                    else
                    {
                        objCommVAT16.Parameters.AddWithValue("@ProdutCategoryId", ProdutCategoryId);
                    }
                }

                else if (!string.IsNullOrWhiteSpace(ItemNo))
                {
                    objCommVAT16.Parameters.AddWithValue("@ItemNo", ItemNo);
                }


                if (StartDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", StartDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = StartDate;
                    }
                } // Common Filed
                if (EndDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", EndDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = EndDate;
                    }
                }

                if (!objCommVAT16.Parameters.Contains("@post1"))
                {
                    objCommVAT16.Parameters.AddWithValue("@post1", post1);
                }
                else
                {
                    objCommVAT16.Parameters["@post1"].Value = post1;
                }

                if (!objCommVAT16.Parameters.Contains("@post2"))
                {
                    objCommVAT16.Parameters.AddWithValue("@post2", post2);
                }
                else
                {
                    objCommVAT16.Parameters["@post2"].Value = post2;
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT16);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "TransferReceive", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "TransferReceive", ex.ToString() + "\n" + sqlText);

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

            return dataSet;
        }

        //currConn to VcurrConn 25-Aug-2020 ok
        //        public DataTable AvgPriceNewBackup(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, bool isPost, SysDBInfoVMTemp connVM = null)
        //        {
        //            #region Initializ

        //            string sqlText = "";
        //            DataTable retResults = new DataTable();
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;

        //            #endregion

        //            #region Try

        //            try
        //            {

        //                #region Validation
        //                if (string.IsNullOrEmpty(itemNo))
        //                {
        //                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
        //                }
        //                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
        //                {
        //                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

        //                }
        //                #endregion Validation

        //                #region Old connection

        //                #region open connection and transaction

        //                ////if (VcurrConn == null)
        //                ////{
        //                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
        //                ////    if (VcurrConn.State != ConnectionState.Open)
        //                ////    {
        //                ////        VcurrConn.Open();
        //                ////    }
        //                ////    Vtransaction = VcurrConn.BeginTransaction("AvgPprice");
        //                ////}

        //                #endregion open connection and transaction

        //                #endregion Old connection

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
        //                    transaction = currConn.BeginTransaction("AvgPprice");
        //                }

        //                #endregion open connection and transaction

        //                #region Get Setting Value

        //                ProductDAL productDal = new ProductDAL();

        //                var AvgPriceVAT16 = productDal.AvgPriceVAT16(itemNo, tranDate, currConn, transaction);
        //                var AvgPriceVAT17 = productDal.AvgPriceVAT17(itemNo, tranDate, currConn, transaction);
        //                decimal Quantity = Convert.ToDecimal(AvgPriceVAT16.Rows[0]["Quantity"]);
        //                decimal Amount = Convert.ToDecimal(AvgPriceVAT16.Rows[0]["Amount"]);

        //                Quantity = Quantity + Convert.ToDecimal(AvgPriceVAT17.Rows[0]["Quantity"]);
        //                Amount = Amount + Convert.ToDecimal(AvgPriceVAT17.Rows[0]["Amount"]);
        //                retResults.Columns.Add("Quantity");
        //                retResults.Columns.Add("Amount");
        //                retResults.Rows.Add(new object[] { Quantity, Amount });

        //                CommonDAL _cDal = new CommonDAL();
        //                bool ImportCostingIncludeATV = false;
        //                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y" ? true : false);

        //                #endregion

        //                #region ProductExist

        //                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
        //                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
        //                cmdExist.Transaction = transaction;

        //                int foundId = (int)cmdExist.ExecuteScalar();
        //                if (foundId <= 0)
        //                {
        //                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
        //                }

        //                #endregion ProductExist

        //                #region AvgPrice

        //                sqlText = "  ";
        //                sqlText += "  SELECT SUM(isnull(SubTotal,0)) Amount,SUM(isnull(stock,0))Quantity" +
        //                           " FROM(   ";
        //                sqlText += "  (SELECT  isnull(OpeningBalance,0) Stock, isnull(p.OpeningTotalCost,0) SubTotal  FROM Products p  WHERE p.ItemNo = '" + itemNo + "')";

        //                sqlText += "  UNION ALL   ";
        //                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
        //                sqlText +=
        //                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('other','Service','ServiceNS','InputService','Trading','TollReceive','TollReceive-WIP','TollReceiveRaw','PurchaseCN')";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                if (ImportCostingIncludeATV)
        //                {
        //                    sqlText += @"   UNION ALL  ( 
        //                SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
        //                isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal  
        //                FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport')  
        //                ";
        //                    if (!isPost)
        //                    {
        //                        sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
        //                    }
        //                    sqlText += " AND ItemNo = '" + itemNo + "') ";
        //                }
        //                else
        //                {
        //                    sqlText += @"    UNION ALL  ( 
        //               SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
        //                isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal  
        //                FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in( 'InputServiceImport','ServiceImport','ServiceNSImport','TradingImport')  
        //                ";
        //                    if (!isPost)
        //                    {
        //                        sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
        //                    }
        //                    sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                    sqlText += @"    UNION ALL  (  
        //                SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
        //                isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+  isnull(OthersAmount,0)),0)),0)SubTotal  
        //                FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import' )  
        //                ";
        //                    if (!isPost)
        //                    {
        //                        sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
        //                    }
        //                    sqlText += " AND ItemNo = '" + itemNo + "') ";
        //                }




        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
        //                sqlText +=
        //                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('PurchaseReturn','PurchaseDN')";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal  " +
        //                           "  FROM IssueDetails WHERE Post='Y' ";
        //                sqlText +=
        //                    "  and TransactionType IN('PackageProduction','Other','TollFinishReceive','Tender','WIP','TollReceive','InputService','InputServiceImport','Trading','TradingTender','ExportTrading','ExportTradingTender','Service','ExportService','InternalIssue','TollIssue')";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal    " +
        //                           "FROM IssueDetails WHERE Post='Y' and TransactionType IN('IssueReturn','ReceiveReturn')";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL  ";
        //                //sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   " +
        //                //           " FROM ReceiveDetails WHERE Post='Y' and TransactionType<>'ReceiveReturn' ";

        //                sqlText += " (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal " +
        //                                " FROM ReceiveDetails WHERE Post='Y' and TransactionType not in('ReceiveReturn','InternalIssue','Trading') "; //business is different for InternalIssue and Trading.
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL ";
        //                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal    FROM ReceiveDetails WHERE Post='Y'";
        //                sqlText += " and TransactionType='ReceiveReturn' ";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL ";
        //                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,-" +
        //                           "isnull(sum( SubTotal),0)SubTotal " +
        //                         "  FROM SalesInvoiceDetails ";
        //                //sqlText += "  WHERE Post='Y' " +
        //                //           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock','InternalIssue')";

        //                sqlText += "  WHERE Post='Y' " +
        //                           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock')";

        //                if (!isPost)
        //                {
        //                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,-isnull(sum( CurrencyValue),0)SubTotal " +
        //                            "   FROM SalesInvoiceDetails ";
        //                sqlText += "  WHERE Post='Y' " +
        //                           "AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') ";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,isnull(sum( SubTotal),0)SubTotal     FROM SalesInvoiceDetails ";
        //                sqlText += "  WHERE Post='Y'  AND TransactionType in( 'Credit') ";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";

        //                ////   New bussiness for transfer raw
        //                ////Transferred from item will be decrease stock
        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal     FROM TransferRawDetails  ";
        //                sqlText += "  WHERE Post='Y' ";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND TransferDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND TransFromItemNo = '" + itemNo + "') ";

        //                //----Transferred to item will be increase stock
        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal     FROM TransferRawDetails  ";
        //                sqlText += "  WHERE Post='Y' ";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND TransferDateTime<='" + tranDate + "' ";
        //                }
        //                sqlText += " AND ItemNo = '" + itemNo + "') ";



        //                sqlText += "  UNION ALL  ";
        //                sqlText += "  (select -isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty,";
        //                sqlText += "  isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)";
        //                sqlText += "  from DisposeDetails  LEFT OUTER JOIN ";
        //                sqlText += "  DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber  where ItemNo='" + itemNo + "' ";
        //                sqlText += "  AND (DisposeDetails.Post ='Y')    ";
        //                if (!isPost)
        //                {
        //                    sqlText += "  AND DisposeDetails.DisposeDate<='" + tranDate + "' ";
        //                }
        //                sqlText += "  and sih.FromStock in ('Y'))  ";
        //                sqlText += "  ) AS a";


        //                DataTable dataTable = new DataTable("UsableItems");
        //                SqlCommand cmd = new SqlCommand(sqlText, currConn);
        //                cmd.Transaction = transaction;
        //                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
        //                dataAdapt.Fill(dataTable);

        //                if (dataTable == null)
        //                {
        //                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "No row found ");
        //                }
        //                retResults = dataTable;


        //                #endregion Stock

        //            }

        //            #endregion try

        //            #region Catch and Finall
        //            catch (SqlException sqlex)
        //            {
        //                FileLogger.Log("ProductDAL", "AvgPriceNewBackup", sqlex.ToString() + "\n" + sqlText);

        //                throw new ArgumentNullException("", sqlex.Message.ToString());
        //                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //                //throw sqlex;
        //            }
        //            catch (Exception ex)
        //            {
        //                FileLogger.Log("ProductDAL", "AvgPriceNewBackup", ex.ToString() + "\n" + sqlText);

        //                throw new ArgumentNullException("", ex.Message.ToString());
        //                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //                //throw ex;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }

        //                #region Old

        //                //////if (VcurrConn == null)
        //                //////{
        //                //////    if (VcurrConn.State == ConnectionState.Open)
        //                //////    {
        //                //////        VcurrConn.Close();

        //                //////    }
        //                //////}
        //                #endregion

        //            }

        //            #endregion

        //            #region Results

        //            return retResults;

        //            #endregion

        //        }

        //currConn to VcurrConn 25-Aug-2020
        public DataTable AvgPriceVAT17(string ItemNo, string StartDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            //Delete all #VAT_17_0 information. It is not necessary for calculation. 
            #region Variables

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataSet = new DataTable("ReportVAT17");


            #endregion

            #region Try

            try
            {
                #region vat19 value

                string vExportInBDT = "";
                CommonDAL commonDal = new CommonDAL();
                vExportInBDT = commonDal.settings("VAT9_1", "ExportInBDT", VcurrConn, Vtransaction, connVM);

                #endregion vat19 value

                string IsExport = "No";

                if (vExportInBDT == "N")
                {
                    sqlText = "Select CASE WHEN pc.IsRaw = 'Export' THEN 'Yes' ELSE 'No' END AS IsExport ";
                    sqlText += "from ProductCategories pc join Products p on pc.CategoryID = p.CategoryID ";
                    sqlText += "where p.ItemNo = @ItemNo";

                    SqlCommand cmd = new SqlCommand(sqlText, VcurrConn, Vtransaction);

                    SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = ItemNo;
                    cmd.Parameters.Add(parameter);


                    object objItemNo = cmd.ExecuteScalar();
                    if (objItemNo == null)
                        IsExport = "No";
                    else
                        IsExport = objItemNo.ToString();
                }


                var top = "";
                sqlText = " ";

                #region SQL

                sqlText += @"
                
--DECLARE @StartDate DATETIME;
--DECLARE @EndDate DATETIME;
--DECLARE @post1 VARCHAR(2);
--DECLARE @post2 VARCHAR(2);
--DECLARE @ItemNo VARCHAR(20);

--DECLARE @IsExport VARCHAR(20);
--SET @IsExport ='No';

--SET @Itemno='24';
--SET @post1='Y';
--SET @post2='N';
--SET @StartDate='2014-04-01';
--SET @EndDate= '2014-04-27';

             
declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;


CREATE TABLE #VAT_17(
SerialNo  varchar (2) NULL,	 ItemNo   varchar (200) NULL,
 StartDateTime   datetime  NULL,	 StartingQuantity   decimal (25, 9) NULL,
 StartingAmount   decimal (25, 9) NULL,	 CustomerID   varchar (200) NULL,
 SD   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 Quantity   decimal (25, 9) NULL,
 UnitCost   decimal (25, 9) NULL,	 TransID   varchar (200) NULL,	 TransType   varchar (200) NULL,Remarks VARCHAR(200),CreatedDateTime   datetime  NULL)

CREATE TABLE #VATTemp_17(SerialNo  varchar (2) NULL,	 Dailydate   datetime  NULL,	 TransID   varchar (200) NULL,
 TransType   varchar (200) NULL,	 ItemNo   varchar (200) NULL,	 UnitCost   decimal (25, 9) NULL,
 Quantity   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 SD   decimal (25, 9) NULL,Remarks VARCHAR(200),CreatedDateTime   datetime  NULL) 
 
 

------end Disposee--------

select @OpeningDate = p.OpeningDate from Products p
WHERE ItemNo=@ItemNo

IF(@OpeningDate<@StartDate)
set @OpeningDate=@StartDate


insert into #VATTemp_17(SerialNo,Dailydate,TransID,VATRate,SD,remarks,TransType,ItemNo,Quantity,UnitCost)
SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo,
 SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost
	FROM (
		 
(SELECT @itemNo ItemNo,isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) Quantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS Amount
 FROM ReceiveDetails WHERE Post='Y'  AND ReceiveDateTime< @StartDate   
  and TransactionType not IN('ReceiveReturn') AND ItemNo = @itemNo ) 
UNION ALL
(SELECT @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM ReceiveDetails WHERE Post='Y'  AND ReceiveDateTime< @StartDate   
 and TransactionType IN('ReceiveReturn') AND ItemNo = @itemNo ) 
UNION ALL 
(SELECT  @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM SalesInvoiceDetails   WHERE Post='Y' AND InvoiceDateTime< @StartDate     
AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock','InternalIssue') AND ItemNo = @itemNo )  
UNION ALL  
(SELECT  @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(CurrencyValue,0)),0) END AS SubTotal
FROM SalesInvoiceDetails   WHERE Post='Y' AND InvoiceDateTime< @StartDate      
AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') AND ItemNo = @itemNo )  
UNION ALL
(SELECT @itemNo ItemNo,isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM SalesInvoiceDetails   WHERE Post='Y' AND InvoiceDateTime< @StartDate    
 AND TransactionType in( 'Credit') AND ItemNo = @itemNo )
   

) AS a GROUP BY a.ItemNo

insert into #VAT_17(SerialNo,ItemNo,StartDateTime,StartingQuantity,StartingAmount,
CustomerID,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime)
select SerialNo,ItemNo,Dailydate,0,0,0,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime
from #VATTemp_17
order by dailydate,SerialNo;

update #VAT_17 set 
CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.SalesInvoiceNo=#VAT_17.TransID 
and #VAT_17.TransType='Sale'
AND (SalesInvoiceHeaders.Post =@post1 or SalesInvoiceHeaders.Post= @post2)

select  
  #VAT_17.Quantity, #vat_17.UnitCost Amount
from #VAT_17  

DROP TABLE #VAT_17
DROP TABLE #VATTemp_17

                ";

                #endregion SQL

                top = "A";

                #region SQL Command

                SqlCommand objCommVAT17 = new SqlCommand();

                objCommVAT17.Connection = VcurrConn;
                objCommVAT17.Transaction = Vtransaction;

                objCommVAT17.CommandText = sqlText;
                objCommVAT17.CommandType = CommandType.Text;

                #endregion

                #region Parameter


                if (!objCommVAT17.Parameters.Contains("@IsExport"))
                {
                    objCommVAT17.Parameters.AddWithValue("@IsExport", IsExport);
                }
                else
                {
                    objCommVAT17.Parameters["@IsExport"].Value = IsExport;
                }

                if (!objCommVAT17.Parameters.Contains("@ItemNo"))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    objCommVAT17.Parameters["@ItemNo"].Value = ItemNo;
                }

                if (!objCommVAT17.Parameters.Contains("@StartDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@StartDate", StartDate);
                    objCommVAT17.Parameters.AddWithValue("@EndDate", StartDate);
                }
                else
                {
                    objCommVAT17.Parameters["@StartDate"].Value = StartDate;
                    objCommVAT17.Parameters["@EndDate"].Value = StartDate;
                }
                objCommVAT17.Parameters.AddWithValue("@post1", "Y");
                objCommVAT17.Parameters.AddWithValue("@post2", "Y");




                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT17);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceVAT17", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceVAT17", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                //if (currConn.State == ConnectionState.Open)
                //{
                //    currConn.Close();
                //}

            }

            #endregion

            return dataSet;
        }

        //currConn to VcurrConn 25-Aug-2020
        public DataTable AvgPriceVAT16(string ItemNo, string StartDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataSet = new DataTable("ReportVAT16");

            #endregion

            #region Try

            try
            {

                string top;

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV", VcurrConn, Vtransaction, connVM) == "Y" ? true : false);

                #region SQL Statement

                sqlText = "";

                #region Backup

                sqlText += @"
                          
	--DECLARE @StartDate DATETIME;
	--DECLARE @EndDate DATETIME;
	--DECLARE @post1 VARCHAR(200);
	--DECLARE @post2 VARCHAR(200);
	--DECLARE @ItemNo VARCHAR(200);
    
	--SET @Itemno='46';
	--SET @post1='Y';
	--SET @post2='N';
	--SET @StartDate='2014-04-01';
	--SET @EndDate='2014-04-27';

declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;

CREATE TABLE #VAT_16(	SerialNo [varchar] (2) NULL,
[ItemNo] [varchar](200) NULL,	[StartDateTime] [datetime] NULL,
[StartingQuantity] [decimal](25, 9) NULL,	[StartingAmount] [decimal](25, 9) NULL,
[VendorID] [varchar](200) NULL,	[SD] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[UnitCost] [decimal](25, 9) NULL,	[TransID] [varchar](200) NULL,
[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,[InvoiceDateTime] [datetime] NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL)

CREATE TABLE #VATTemp_16([SerialNo] [varchar] (2) NULL,[Dailydate] [datetime] NULL,[InvoiceDateTime] [datetime] NULL,
[TransID] [varchar](200) NULL,	[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,
[ItemNo] [varchar](200) NULL,	[UnitCost] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,	[SD] [decimal](25, 9) NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL) 

---- start purchase---
 
 

select @OpeningDate = p.OpeningDate from Products p
WHERE ItemNo=@ItemNo

IF(@OpeningDate<@StartDate)
set @OpeningDate=@StartDate

insert into #VATTemp_16(SerialNo,Dailydate,TransID,VATRate,SD,Remarks,TransType,ItemNo,Quantity,UnitCost,InvoiceDateTime,BENumber)
 		    
SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo, SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost,@OpeningDate InvoiceDateTime,'-' BENumber
	FROM (
				SELECT @itemNo ItemNo, isnull(OpeningBalance,0) Quantity, isnull(p.OpeningTotalCost,0) Amount  
FROM Products p  WHERE p.ItemNo = @itemNo 
UNION ALL (
		SELECT @itemNo ItemNo, isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal 
FROM ReceiveDetails WHERE Post='Y' 
and TransactionType in('WIP') 
AND ReceiveDateTime < @StartDate      AND ItemNo = @itemNo
 )   
UNION ALL (
		SELECT @itemNo ItemNo, isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal 
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('other','Service','ServiceNS','InputService','Trading', 'TollReceive-WIP','PurchaseCN') 

AND ReceiveDate < @StartDate      AND ItemNo = @itemNo
 )  	 UNION ALL (
	SELECT @itemNo ItemNo, isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,
	isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal 
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') 
AND ReceiveDate < @StartDate      AND ItemNo = @itemNo
 ) 	 UNION ALL 
(	SELECT  @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,
-isnull(sum(isnull(SubTotal,0)),0)SubTotal     FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('PurchaseReturn','PurchaseDN')  AND ReceiveDate< @StartDate     AND ItemNo = @itemNo ) 
 
 --Transfer to Raw
 UNION ALL (
	SELECT @itemNo ItemNo,isnull(sum(UOMQty),0) TransferQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal
FROM TransferRawDetails WHERE Post='Y'   AND TransferDateTime< @StartDate  
   AND ItemNo = @itemNo  AND (UOMQty>0)   
 ) 

UNION ALL 
(SELECT @itemNo ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)
FROM IssueDetails WHERE Post='Y'   AND IssueDateTime< @StartDate  
   and TransactionType IN('other','Receive','TollReceive','TollIssue')  AND ItemNo = @itemNo  AND (UOMQty>0))   
UNION ALL 
(SELECT @itemNo ItemNo,isnull(sum(UOMQty),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)
FROM SalesInvoiceDetails WHERE Post='Y'   AND InvoiceDateTime< @StartDate  
   and TransactionType IN('TollIssue ')  AND ItemNo = @itemNo  AND (UOMQty>0))   

UNION ALL 
(select @itemNo ItemNo,-isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty, 
isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)  
from DisposeDetails  LEFT OUTER JOIN   DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber 
 where ItemNo=@itemNo   
AND DisposeDetails.DisposeDate< @StartDate      AND (DisposeDetails.Post ='Y')      and sih.FromStock in ('Y'))    

	
) AS a GROUP BY a.ItemNo


insert into #VAT_16(SerialNo,ItemNo,StartDateTime,InvoiceDateTime,StartingQuantity,StartingAmount,
VendorID,Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,CreateDateTime)
select SerialNo,@ItemNo,Dailydate,InvoiceDateTime,0,0,0,
Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,CreateDateTime
from #VATTemp_16
order by dailydate,SerialNo

update #VAT_16 set 
VendorID=PurchaseInvoiceHeaders.VendorID
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_16.TransID
and #VAT_16.TransType='Purchase'

update #VAT_16 set 
StartingQuantity=0,
StartingAmount=0
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_16.TransID 
and PurchaseInvoiceHeaders.TransactionType IN('ServiceNS')
AND (PurchaseInvoiceHeaders.Post =@post1 or PurchaseInvoiceHeaders.Post= @post2)
and #VAT_16.TransType='Purchase'

select  
 #VAT_16.Quantity ,#VAT_16.UnitCost  Amount
from #VAT_16  
  

DROP TABLE #VAT_16
DROP TABLE #VATTemp_16


                
                ";

                #endregion



                top = "Go";

                #endregion

                #region SQL Command

                SqlCommand objCommVAT16 = new SqlCommand();
                objCommVAT16.Connection = VcurrConn;
                objCommVAT16.Transaction = Vtransaction;

                objCommVAT16.CommandText = sqlText;
                objCommVAT16.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                //objCommVAT16.CommandText = sqlText;
                //objCommVAT16.CommandType = CommandType.Text;

                #region Parameter

                //objCommVAT16.CommandText = sqlText;
                //objCommVAT16.CommandType = CommandType.Text;

                if (!objCommVAT16.Parameters.Contains("@ItemNo"))
                {
                    objCommVAT16.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    objCommVAT16.Parameters["@ItemNo"].Value = ItemNo;
                }


                if (StartDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", StartDate);
                        objCommVAT16.Parameters.AddWithValue("@EndDate", StartDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = StartDate;
                        objCommVAT16.Parameters["@EndDate"].Value = StartDate;
                    }

                } // Common Filed

                #endregion Parameter

                objCommVAT16.Parameters.AddWithValue("@post1", "Y");
                objCommVAT16.Parameters.AddWithValue("@post2", "Y");



                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT16);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceVAT16", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceVAT16", ex.ToString() + "\n" + sqlText);

                throw ex;
            }


            #endregion

            return dataSet;
        }

        public decimal AvgPriceTender(string ItemNo, string StartDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string sqlText = "";
            decimal result = 0;
            DataTable dt = new DataTable("ReportVAT16");
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

                string top;

                CommonDAL _cDal = new CommonDAL();

                #region SQL Statement

                sqlText = "";

                #region Backup

                sqlText += @"
                          
--DECLARE @StartDate DATETIME;
--DECLARE @ItemNo VARCHAR(200);
--
--SET @Itemno=7293;
--SET @StartDate='01/09/2013';

select isnull(sum(UOMQty),0)Qty, isnull(sum(SubTotal),0)Amount from (
select OpeningBalance UOMQty,OpeningTotalCost SubTotal from Products where ItemNo=@ItemNo  
union all
select isnull(sum(UOMQty),0)UOMQty, isnull(sum(SubTotal+SDAmount),0)SubTotal
 from PurchaseInvoiceDetails pd where ItemNo=@ItemNo and TransactionType in('other' ) 
 and Post='Y'
 and  ReceiveDate < DATEADD(d,1, @StartDate) 
union all

select  isnull(sum(UOMQty),0)UOMQty , isnull(sum((isnull(pd.AssessableValue,0)+ isnull(pd.CDAmount,0)+ isnull(pd.RDAmount,0)+ isnull(pd.TVBAmount,0)+ isnull(pd.TVAAmount,0)+ isnull(pd.ATVAmount,0)+isnull(pd.OthersAmount,0)+isnull(pd.SDAmount,0))),0)SubTotal
 from PurchaseInvoiceDetails pd where ItemNo=@ItemNo and TransactionType in('Import')
 and Post='Y'
 and  ReceiveDate < DATEADD(d,1, @StartDate) 
 ) as a


                
                ";

                #endregion



                top = "Go";

                #endregion

                #region SQL Command

                SqlCommand _obj = new SqlCommand();
                _obj.Connection = currConn;
                _obj.Transaction = transaction;

                _obj.CommandText = sqlText;
                _obj.CommandType = CommandType.Text;

                #endregion

                #region Parameter


                if (!_obj.Parameters.Contains("@ItemNo"))
                {
                    _obj.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    _obj.Parameters["@ItemNo"].Value = ItemNo;
                }


                if (!_obj.Parameters.Contains("@StartDate"))
                {
                    _obj.Parameters.AddWithValue("@StartDate", System.DBNull.Value);
                }
                else
                {
                    _obj.Parameters["@StartDate"].Value = System.DBNull.Value;
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(_obj);
                dataAdapter.Fill(dt);

                decimal Qty = Convert.ToDecimal(dt.Rows[0]["Qty"]);
                decimal Amount = Convert.ToDecimal(dt.Rows[0]["Amount"]);
                if (Qty > 0)
                {
                    result = Amount / Qty;
                }
                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceTender", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceTender", ex.ToString() + "\n" + sqlText);

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

            return result;
        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public DataTable AvgPriceForInternalSales(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, bool isPost, SysDBInfoVMTemp connVM = null)
        {
            // from Internal Sales,Trading, Service-Stock
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

                }
                #endregion Validation

                #region Old connection

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////    Vtransaction = VcurrConn.BeginTransaction("AvgPprice");
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction("AvgPprice");
                }

                #endregion open connection and transaction

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV", currConn, transaction, connVM) == "Y" ? true : false);

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);



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
                sqlText += "  (SELECT  isnull(OpeningBalance,0) Stock, isnull(p.OpeningTotalCost,0) SubTotal  FROM Products p  WHERE p.ItemNo = @itemNo)";

                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText +=
                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('other','Service','ServiceNS','InputService','Trading','TollReceive','TollReceive-WIP','TollReceiveRaw','PurchaseCN')";
                if (!isPost)
                {
                    //sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                    sqlText += "  AND ReceiveDate<=@tranDate ";


                    
                }
                sqlText += " AND ItemNo = @itemNo) ";

                if (ImportCostingIncludeATV)
                {
                    sqlText += "  UNION ALL   ";
                    sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity," +
                               "isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal   ";
                    sqlText +=
                        "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') ";

                }
                else
                {
                    sqlText += @"  
                    UNION ALL  
                    (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                    isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal 
                    FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') 
                    ";
                    sqlText += @"  
                    UNION ALL  
                    (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                    isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(OthersAmount,0)),0)),0)SubTotal 
                    FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import') 
                    ";
                }

                if (!isPost)
                {
                    //sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                    sqlText += "  AND ReceiveDate<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText +=
                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('PurchaseReturn','PurchaseDN')";
                if (!isPost)
                {
                    //sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                    sqlText += "  AND ReceiveDate<=@tranDate ";

                    
                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal  " +
                           "  FROM IssueDetails WHERE Post='Y' ";
                sqlText +=
                    "  and TransactionType IN('PackageProduction','Other','TollFinishReceive','Tender','WIP','TollReceive','InputService','InputServiceImport','Trading','TradingTender','ExportTrading','ExportTradingTender','Service','ExportService','InternalIssue','TollIssue')";
                if (!isPost)
                {
                    //sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
                    sqlText += "  AND IssueDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal    " +
                           "FROM IssueDetails WHERE Post='Y' and TransactionType IN('IssueReturn','ReceiveReturn')";
                if (!isPost)
                {
                    //sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
                    sqlText += "  AND IssueDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                //sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   " +
                //           " FROM ReceiveDetails WHERE Post='Y' and TransactionType<>'ReceiveReturn' ";

                sqlText += " (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal " +
                                " FROM ReceiveDetails WHERE Post='Y' and TransactionType not in('ReceiveReturn','InternalIssue','Trading') "; //business is different for InternalIssue and Trading.
                if (!isPost)
                {
                    //sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
                    sqlText += "  AND ReceiveDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal    FROM ReceiveDetails WHERE Post='Y'";
                sqlText += " and TransactionType='ReceiveReturn' ";
                if (!isPost)
                {
                    //sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
                    sqlText += "  AND ReceiveDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,-" +
                           "isnull(sum( SubTotal),0)SubTotal " +
                         "  FROM SalesInvoiceDetails ";
                //sqlText += "  WHERE Post='Y' " +
                //           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock','InternalIssue')";

                sqlText += "  WHERE Post='Y' " +
                           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock')";

                if (!isPost)
                {
                    //sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                    sqlText += "  AND InvoiceDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,-isnull(sum( CurrencyValue),0)SubTotal " +
                            "   FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' " +
                           "AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') ";
                if (!isPost)
                {
                    //sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                    sqlText += "  AND InvoiceDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,isnull(sum( SubTotal),0)SubTotal     FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y'  AND TransactionType in( 'Credit') ";
                if (!isPost)
                {
                    //sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                    sqlText += "  AND InvoiceDateTime<=@tranDate ";

                }
                sqlText += " AND ItemNo = @itemNo) ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (select -isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty,";
                sqlText += "  isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)";
                sqlText += "  from DisposeDetails  LEFT OUTER JOIN ";
                sqlText += "  DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber  where ItemNo=@itemNo ";
                sqlText += "  AND (DisposeDetails.Post ='Y')    ";
                if (!isPost)
                {
                    //sqlText += "  AND DisposeDetails.DisposeDate<='" + tranDate + "' ";
                    sqlText += "  AND DisposeDetails.DisposeDate<=@tranDate ";

                   
                }
                sqlText += "  and sih.FromStock in ('Y'))  ";
                sqlText += "  ) AS a";


                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;

                parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmd.Parameters.Add(parameter);

                parameter = new SqlParameter("@tranDate", SqlDbType.DateTime);
                parameter.Value = tranDate;
                cmd.Parameters.Add(parameter);


                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "No row found ");
                }
                retResults = dataTable;


                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceForInternalSales", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "AvgPriceForInternalSales", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public decimal PurchasePrice(string itemNo, String PurchaseNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion Validation

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }
                ////}

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
                    currConn = _dbsqlConnection.GetConnection(connVM);
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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdExist.Parameters.Add(parameter);


                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion ProductExist

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT ";
                sqlText += "  isnull(AssessableValue,0)+ isnull(CDAmount,0)+isnull(OthersAmount,0)" +
                           "+ isnull(RDAmount,0)+ isnull(TVBAmount,0)";
                //sqlText += "   FROM PurchaseInvoiceDetails id WHERE id.PurchaseInvoiceNo='" + PurchaseNo + "'";
                sqlText += "   FROM PurchaseInvoiceDetails id WHERE id.PurchaseInvoiceNo=@PurchaseNo";

                sqlText += "  and itemno=@itemNo ";


                SqlCommand cmdIssuePrice = new SqlCommand(sqlText, currConn);
                cmdIssuePrice.Transaction = transaction;

                parameter = new SqlParameter("@PurchaseNo", SqlDbType.VarChar, 20);
                parameter.Value = PurchaseNo;
                cmdIssuePrice.Parameters.Add(parameter);

                parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmdIssuePrice.Parameters.Add(parameter);

                if (cmdIssuePrice.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdIssuePrice.ExecuteScalar();
                    //object objDel = cmdDelete.ExecuteScalar();

                }

                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "PurchasePrice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "PurchasePrice", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        #endregion

        #region Search Product


        public DataTable SearchProductDT(string ItemNo, string ProductName, string CategoryID, string CategoryName, string UOM, string IsRaw, string SerialNo, string HSCodeNo, string ActiveStatus, string Trading, string NonStock, string ProductCode, string databaseName, string customerId = "0", string isvds = "", int BranchId = 0, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (!string.IsNullOrWhiteSpace(databaseName))
                {
                    currConn.ChangeDatabase(databaseName);
                }
                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
                                    Products.ItemNo,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(Products.ProductDescription,'N/A')ProductDescription,
                                    Products.CategoryID, 
                                    isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
                                    isnull(Products.UOM,'N/A')UOM,
                                    isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
                                    isnull(Products.CostPrice,0)CostPrice,
                                    isnull(Products.SalesPrice,0)SalesPrice,
                                    isnull(Products.NBRPrice,0)NBRPrice,
                                    isnull(ProductCategories.IsRaw,'N')IsRaw,
                                    isnull(Products.SerialNo,'N/A')SerialNo ,
                                    isnull(Products.HSCodeNo,'N/A')HSCodeNo,
                                    isnull(Products.VATRate,0)VATRate,
                                    isnull(Products.ActiveStatus,'N')ActiveStatus,
                                    isnull(Products.OpeningBalance,0)OpeningBalance,
                                    isnull(Products.Comments,'N/A')Comments,
                                    'N/A' HSDescription, 
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(Products.SD,0)SD, 
                                    isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
                                    Products.TradingMarkUp,Products.NonStock,
                                    isnull(Products.QuantityInHand,0)QuantityInHand,
                                    convert(varchar, Products.OpeningDate,120)OpeningDate,
                                    isnull(Products.ReceivePrice,0)ReceivePrice,
                                    isnull(Products.IssuePrice,0)IssuePrice,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.TollCharge,0)TollCharge,
                                    isnull(Products.VDSRate,0)VDSRate,
                                    isnull(Products.Banderol,'N')Banderol,

                                    isnull(Products.CDRate,0)CDRate,
                                    isnull(Products.RDRate,0)RDRate,
                                    isnull(Products.TVARate,0)TVARate,
                                    isnull(Products.ATVRate,0)ATVRate,
                                    isnull(Products.VATRate2,0)VATRate2,
                                    isnull(Products.FixedVATAmount,0)FixedVATAmount,

                                    isnull(Products.IsFixedVAT,'N')IsFixedVAT,
                                    isnull(Products.IsFixedSD,'N')IsFixedSD,
                                    isnull(Products.IsFixedCD,'N')IsFixedCD,
                                    isnull(Products.IsFixedRD,'N')IsFixedRD,
                                    isnull(Products.IsFixedAIT,'N')IsFixedAIT,
                                    isnull(Products.IsFixedVAT1,'N')IsFixedVAT1,
                                    isnull(Products.IsFixedAT,'N')IsFixedAT,
                                    isnull(Products.IsFixedOtherSD,'N')IsFixedOtherSD,
                                    isnull(Products.IsHouseRent,'N')IsHouseRent,
                                    isnull(Products.IsVDS,'Y')IsVDS,
                                    isnull(Products.HPSRate,'0')HPSRate,

                                    isnull(Products.AITRate,0)AITRate,
                                    isnull(Products.SDRate,0)SDRate,
                                    isnull(Products.VATRate3,0)VATRate3,
                                    

                                    isnull(Products.TollProduct,'N')TollProduct,
                                    isnull(Products.IsExempted,'N')IsExempted,
                                    isnull(Products.IsZeroVAT,'N')IsZeroVAT,
                                    isnull(Products.IsTransport,'N')IsTransport,
                                    isnull(Products.TDSCode,0)TDSCode,
                                    isnull(Products.BranchId,0)BranchId,
                                    isnull(Products.TransactionHoldDate,0)TransactionHoldDate,
                                    isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate

                                    

                                    FROM Products LEFT OUTER JOIN
                                    ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
                      
                                    WHERE (Products.IsArchive is null or  Products.IsArchive<> 1)
                                    AND (Products.ItemNo LIKE '%' + @ItemNo + '%'  OR @ItemNo IS NULL) 
                                    AND (Products.ProductName LIKE '%' + @ProductName + '%' OR  @ProductName IS NULL)
                                    AND (Products.CategoryID LIKE '%' + @CategoryID + '%' OR @CategoryID IS NULL) 
                                    AND (ProductCategories.CategoryName LIKE '%' + @CategoryName + '%' OR  @CategoryName IS NULL)
                                    AND (ProductCategories.IsRaw LIKE '%' + @IsRaw + '%' OR @IsRaw IS NULL)  
                                    AND (Products.SerialNo LIKE '%' + @SerialNo + '%' OR @SerialNo IS NULL)
                                    AND (Products.HSCodeNo LIKE '%' + @HSCodeNo + '%' OR @HSCodeNo IS NULL)
                                    AND (Products.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                                    
                                    AND (Products.ProductCode LIKE '%' + @ProductCode + '%' OR @ProductCode IS NULL)
                                    ";

                if (isvds != "")
                {
                    sqlText += " AND (ISNULL(Products.IsVDS, 'Y') = @IsVDS OR '' IS NULL)";
                }
                if (BranchId > 0)
                {
                    sqlText += " and Products.BranchId = @BranchId";
                }


                if (userBranchs != null)
                {
                    sqlText += " and Products.BranchId in (";

                    foreach (UserBranchDetailVM branch in userBranchs)
                    {
                        sqlText += branch.BranchId + ",";
                    }


                    sqlText = sqlText.TrimEnd(',') + ")";

                }



                sqlText += " order by Products.ProductCode";
                SqlCommand objCommProduct = new SqlCommand();


                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;


                if (BranchId > 0)
                {
                    objCommProduct.Parameters.AddWithValue("@BranchId", BranchId);
                }
                if (!objCommProduct.Parameters.Contains("@ItemNo"))
                { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }
                if (!objCommProduct.Parameters.Contains("@ProductName"))
                { objCommProduct.Parameters.AddWithValue("@ProductName", ProductName); }
                else { objCommProduct.Parameters["@ProductName"].Value = ProductName; }
                if (!objCommProduct.Parameters.Contains("@CategoryID"))
                { objCommProduct.Parameters.AddWithValue("@CategoryID", CategoryID); }
                else { objCommProduct.Parameters["@CategoryID"].Value = CategoryID; }
                if (!objCommProduct.Parameters.Contains("@CategoryName"))
                { objCommProduct.Parameters.AddWithValue("@CategoryName", CategoryName); }
                else { objCommProduct.Parameters["@CategoryName"].Value = CategoryName; }
                if (!objCommProduct.Parameters.Contains("@UOM"))
                { objCommProduct.Parameters.AddWithValue("@UOM", UOM); }
                else { objCommProduct.Parameters["@UOM"].Value = UOM; }
                if (!objCommProduct.Parameters.Contains("@IsRaw"))
                { objCommProduct.Parameters.AddWithValue("@IsRaw", IsRaw); }
                else { objCommProduct.Parameters["@IsRaw"].Value = IsRaw; }
                if (!objCommProduct.Parameters.Contains("@SerialNo"))
                { objCommProduct.Parameters.AddWithValue("@SerialNo", SerialNo); }
                else { objCommProduct.Parameters["@SerialNo"].Value = SerialNo; }
                if (!objCommProduct.Parameters.Contains("@HSCodeNo"))
                { objCommProduct.Parameters.AddWithValue("@HSCodeNo", HSCodeNo); }
                else { objCommProduct.Parameters["@HSCodeNo"].Value = HSCodeNo; }

                if (!objCommProduct.Parameters.Contains("@ActiveStatus"))
                { objCommProduct.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommProduct.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                if (!objCommProduct.Parameters.Contains("@Trading"))
                { objCommProduct.Parameters.AddWithValue("@Trading", Trading); }
                else { objCommProduct.Parameters["@Trading"].Value = Trading; }
                if (!objCommProduct.Parameters.Contains("@NonStock"))
                { objCommProduct.Parameters.AddWithValue("@NonStock", NonStock); }
                else { objCommProduct.Parameters["@NonStock"].Value = NonStock; }
                if (ProductCode == "")
                {
                    if (!objCommProduct.Parameters.Contains("@ProductCode"))
                    { objCommProduct.Parameters.AddWithValue("@ProductCode", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@ProductCode"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@ProductCode"))
                    { objCommProduct.Parameters.AddWithValue("@ProductCode", ProductCode); }
                    else { objCommProduct.Parameters["@ProductCode"].Value = ProductCode; }
                }
                if (isvds != "")
                {
                    objCommProduct.Parameters.AddWithValue("@IsVDS", isvds);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductDT", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductDT", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public DataTable SelectProductDTAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
                   , bool Dt = false, int BranchId = 0, string databaseName = "", string isvds = "", List<UserBranchDetailVM> userBranchs = null, SysDBInfoVMTemp connVM = null, string IsOverhead = null)
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

                #region SqlText

                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"
                            
Products.ItemNo
,isnull(Products.ProductCode,'N/A')                                          AS ProductCode
,isnull(Products.ProductName,'N/A')                                          AS ProductName
,isnull(Products.ProductName,'N/A')+'~'+isnull(Products.ProductCode,'N/A')        AS ProductNameCode
,isnull(Products.ProductDescription,'N/A')                                   AS ProductDescription
,isnull(ProductCategories.IsRaw,'N')                                    AS IsRaw
,isnull(ProductCategories.CategoryName,'N/A')                           AS CategoryName 
,isnull(Products.UOM,'N/A')                                                  AS UOM
,isnull(Products.NBRPrice,0)                                                 AS NBRPrice
,isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0)    AS Stock
,isnull(Products.OpeningTotalCost,0)                                         AS OpeningTotalCost
,isnull(Products.ActiveStatus,'N')                                           AS ActiveStatus
,isnull(Products.CostPrice,0)                                                AS CostPrice
,isnull(Products.SalesPrice,0)                                               AS SalesPrice
,isnull(Products.SerialNo,'N/A')                                             AS SerialNo 
,isnull(Products.HSCodeNo,'N/A')                                             AS HSCodeNo
,isnull(Products.VATRate,0)                                                  AS VATRate
,isnull(Products.OpeningBalance,0)                                           AS OpeningBalance
,isnull(Products.Comments,'N/A')                                             AS Comments
,'N/A'                                                                  AS HSDescription
,isnull(Products.SD,0)                                                       AS SD
,isnull(Products.Packetprice,0)                                              AS Packetprice
,Products.Trading                                                            AS Trading 
,Products.TradingMarkUp                                                      AS TradingMarkUp
,Products.NonStock                                                           AS NonStock
,isnull(Products.QuantityInHand,0)                                           AS QuantityInHand
,convert(varchar, Products.OpeningDate,120)                                  AS OpeningDate
,isnull(Products.ReceivePrice,0)                                             AS ReceivePrice
,isnull(Products.IssuePrice,0)                                               AS IssuePrice
,isnull(Products.ProductCode,'N/A')                                          AS ProductCode
,isnull(Products.RebatePercent,0)                                            AS RebatePercent
,isnull(Products.TollCharge,0)                                               AS TollCharge
,isnull(Products.VDSRate,0)                                                  AS VDSRate
,isnull(Products.Banderol,'N')                                               AS Banderol
,isnull(Products.CDRate,0)                                                   AS CDRate
,isnull(Products.RDRate,0)                                                   AS RDRate
,isnull(Products.TVARate,0)                                                  AS TVARate
,isnull(Products.ATVRate,0)                                                  AS ATVRate
,isnull(Products.VATRate2,0)                                                 AS VATRate2
,isnull(Products.FixedVATAmount,0)                                           AS FixedVATAmount
,isnull(Products.IsFixedVAT,'N')                                             AS IsFixedVAT
,isnull(Products.IsFixedSD,'N')                                              AS IsFixedSD
,isnull(Products.IsFixedCD,'N')                                              AS IsFixedCD
,isnull(Products.IsFixedRD,'N')                                              AS IsFixedRD
,isnull(Products.IsFixedAIT,'N')                                             AS IsFixedAIT
,isnull(Products.IsFixedVAT1,'N')                                            AS IsFixedVAT1
,isnull(Products.IsFixedAT,'N')                                              AS IsFixedAT
,isnull(Products.IsFixedOtherSD,'N')                                         AS IsFixedOtherSD
,isnull(Products.ShortName,'-')                                              AS ShortName
,isnull(Products.IsHouseRent,'N')                                            AS IsHouseRent
,isnull(Products.IsVDS,'Y')                                                  AS IsVDS
,isnull(Products.HPSRate,'0')                                                AS HPSRate
,isnull(Products.AITRate,0)                                                  AS AITRate
,isnull(Products.SDRate,0)                                                   AS SDRate
,isnull(Products.VATRate3,0)                                                 AS VATRate3
,isnull(Products.TollProduct,'N')                                            AS TollProduct
,isnull(Products.IsExempted,'N')                                             AS IsExempted
,isnull(Products.IsZeroVAT,'N')                                              AS IsZeroVAT
,isnull(Products.IsTransport,'N')                                            AS IsTransport
,isnull(Products.TDSCode,0)                                                  AS TDSCode
,isnull(TDSs.Section,'-')                                                    AS TDSSection  
,isnull(Products.GenericName,'-')                                            AS GenericName
,isnull(Products.DARNo,'-')                                                  AS DARNo
,Products.CategoryID                                                         AS CategoryID
,isnull(ProductCategories.IsRaw,'N')                                         AS ProductType
,isnull(ProductCategories.CategoryName,'N/A')                                AS ProductGroup
,isnull(Products.BranchId,0)                                                 AS BranchId
,isnull(Products.TradingSaleVATRate,0)                                       AS TradingSaleVATRate
,isnull(Products.TradingSaleSD,0)                                            AS TradingSaleSD
,isnull(Products.TransactionHoldDate,0)                                      AS TransactionHoldDate
,isnull(Products.IsFixedVATRebate,'N')                                       AS IsFixedVATRebate
,isnull(Products.TollOpeningQuantity,0)                                    AS TollOpeningQuantity


--,isnull(Products.WastageTotalQuantity,0)                                     AS WastageTotalQuantity
--,isnull(Products.WastageTotalValue,0)                                        AS WastageTotalValue

FROM Products 
LEFT OUTER JOIN ProductCategories ON Products.CategoryID = ProductCategories.CategoryID  
LEFT OUTER JOIN TDSs on TDSs.Code=Products.TDSCode                    
WHERE (Products.IsArchive is null or  Products.IsArchive<> 1)
";

                #endregion SqlText

                sqlTextCount += @" 
select count(Products.ItemNo)RecordCount
FROM Products
LEFT OUTER JOIN ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
WHERE (Products.IsArchive is null or  Products.IsArchive <> 1)";


                if (!string.IsNullOrWhiteSpace(IsOverhead))
                {
                    if (IsOverhead == "Y")
                    {
                        sqlTextParameter += " AND ISNULL(ProductCategories.IsRaw,'N') = 'Overhead'";
                    }
                    else
                    {
                        sqlTextParameter += " AND ISNULL(ProductCategories.IsRaw,'N') NOT IN('Overhead')";
                    }
                }


                if (isvds != "")
                {
                    sqlTextParameter += " AND (ISNULL(Products.IsVDS, 'Y') = @IsVDS OR '' IS NULL)";
                }
                if (BranchId > 0)
                {
                    sqlTextParameter += " and Products.BranchId = @BranchId";
                }


                if (userBranchs != null)
                {
                    sqlTextParameter += " and Products.BranchId in (";

                    foreach (UserBranchDetailVM branch in userBranchs)
                    {
                        sqlTextParameter += branch.BranchId + ",";
                    }


                    sqlText = sqlText.TrimEnd(',') + ")";

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
                        else if (conditionFields[i].ToLower().Contains("isnull"))
                        {
                            sqlText += " AND " + "isnull(" + cField + ",'Y')=" + " @" + cField + "";
                        }
                        else
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }


                #endregion SqlText

                sqlTextOrderBy += " order by Products.ProductCode";

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
                if (isvds != "")
                {
                    da.SelectCommand.Parameters.AddWithValue("@IsVDS", isvds);
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

                string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical", dt, connVM);

                if (IsPharmaceutical == "N")
                {
                    string[] columnNames = { "GenericName", "DARNo" };

                    dt = OrdinaryVATDesktop.DtDeleteColumns(dt, columnNames);
                }

                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", ex.ToString() + "\n" + sqlText);

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

        public DataTable ProductDTByItemNo(string ItemNo, string ProductName = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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
SELECT  top 1  Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(nullif(Products.ProductDescription,''),'N/A') ProductDescription,
Products.CategoryID, 
isnull(Products.UOM,'N/A')UOM,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull(Products.SerialNo,'N/A')SerialNo ,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(Products.ActiveStatus,'N')ActiveStatus,
isnull(Products.OpeningBalance,0)OpeningBalance,
isnull(Products.Comments,'N/A')Comments,
'N/A' HSDescription, 
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
Products.TradingMarkUp,Products.NonStock,
isnull(Products.QuantityInHand,0)QuantityInHand,
convert(varchar, Products.OpeningDate,120)OpeningDate,
isnull(Products.ReceivePrice,0)ReceivePrice,
isnull(Products.IssuePrice,0)IssuePrice,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.RebatePercent,0)RebatePercent,
isnull(Products.TollCharge,0)TollCharge,
isnull(Products.Banderol,'N')Banderol,
isnull(Products.IsTransport,'N')IsTransport,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsHouseRent,'N')IsHouseRent,
isnull(Products.IsFixedOtherSD,'N')IsFixedOtherSD,
isnull(Products.SD,0)SD, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.VATRate2,0)VATRate2,
isnull(Products.VDSRate,0)VDSRate, 
isnull(Products.CDRate,0)CDRate,
isnull(Products.RDRate,0)RDRate,
isnull(Products.SDRate,0)SDRate,
isnull(Products.VATRate3,0)VATRate3,
isnull(Products.ATVRate,0)ATVRate,
isnull(Products.AITRate,0)AITRate,
isnull(Products.IsFixedSD,'N')IsFixedSD,
isnull(Products.IsFixedCD,'N')IsFixedCD,
isnull(Products.IsFixedRD,'N')IsFixedRD,
isnull(Products.IsFixedAIT,'N')IsFixedAIT,
isnull(Products.IsFixedVAT1,'N')IsFixedVAT1,
isnull(Products.IsFixedAT,'N')IsFixedAT,
isnull(Products.IsVDS,'Y')IsVDS,
isnull(Products.HPSRate,'0')HPSRate,
isnull(Products.TVARate,0)TVARate,
isnull(Products.TollProduct,'N')TollProduct   ,
isnull(Products.TDSCode,'-')TDSCode  ,
isnull(Products.TransactionHoldDate,'0')TransactionHoldDate  ,
isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate  ,
isnull(TDSs.Section,'-')TDSSection   
FROM Products 
left outer join TDSs on TDSs.Code=Products.TDSCode
where 1=1 and Products.IsArchive = 0  

";
                if (string.IsNullOrEmpty(ProductName))
                {
                    sqlText += @"   and ItemNo= @ItemNo  ";
                }
                else
                {
                    sqlText += @"   and ProductName= @ProductName  ";
                }

                SqlCommand objCommProduct = new SqlCommand(sqlText, currConn, transaction);

                //SqlCommand objCommProduct = new SqlCommand();
                //objCommProduct.Connection = currConn;
                //objCommProduct.CommandText = sqlText;
                //objCommProduct.CommandType = CommandType.Text;
                if (string.IsNullOrEmpty(ProductName))
                {
                    if (!objCommProduct.Parameters.Contains("@ItemNo"))
                    { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                    else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@ProductName"))
                    { objCommProduct.Parameters.AddWithValue("@ProductName", ProductName); }
                    else { objCommProduct.Parameters["@ProductName"].Value = ProductName; }
                }



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchProductMiniDS(string ItemNo, string CategoryID, string IsRaw, string CategoryName,
            string ActiveStatus, string Trading, string NonStock, string ProductCode, SysDBInfoVMTemp connVM = null, List<string> IsRawList = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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
                sqlText += @" 


SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
isnull(Products.VDSRate,0)VDSRate,
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.CDRate,0)CDRate,
isnull(Products.RDRate,0)RDRate,
isnull(Products.TVARate,0)TVARate,
isnull(Products.ATVRate,0)ATVRate,
isnull(Products.VATRate2,0)VATRate2,
isnull(Products.IsTransport,'N')IsTransport,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent,
isnull(Products.TransactionHoldDate,0)TransactionHoldDate,
isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate

FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";


                sqlText += "  WHERE Products.ActiveStatus='Y'  and    (Products.ItemNo LIKE '%' + @ItemNo + '%'  OR Products.ItemNo IS NULL) ";
                if (!string.IsNullOrEmpty(CategoryID))
                    sqlText += "AND  (Products.CategoryID  = @CategoryID OR Products.CategoryID IS NULL) ";

                string IsRaws = string.Empty;
                if (IsRawList != null && IsRawList.Count > 0)
                {
                    IsRaws = string.Join("','", IsRawList);

                    sqlText += " AND ProductCategories.IsRaw IN(' @IsRaws ') ";
                }

                if (!string.IsNullOrEmpty(IsRaw))
                    sqlText += "AND  (ProductCategories.IsRaw  = @IsRaw OR ProductCategories.IsRaw  IS NULL) ";
                sqlText += " AND (ProductCategories.CategoryName LIKE '%' + @CategoryName + '%' OR ProductCategories.CategoryName IS NULL)  ";
                sqlText += " AND (Products.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR Products.ActiveStatus IS NULL)";
                sqlText += " AND (Products.Trading LIKE '%' + @Trading + '%' OR Products.Trading IS NULL)";
                sqlText += " AND (Products.NonStock LIKE '%' + @NonStock + '%' OR Products.NonStock IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%' + @ProductCode + '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;


                SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = ItemNo;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@CategoryID", SqlDbType.VarChar, 20);
                parameter.Value = CategoryID;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@IsRaw", SqlDbType.VarChar, 50);
                parameter.Value = IsRaw;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@CategoryName", SqlDbType.VarChar, 120);
                parameter.Value = CategoryName;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@ActiveStatus", SqlDbType.VarChar, 1);
                parameter.Value = ActiveStatus;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@Trading", SqlDbType.VarChar, 1);
                parameter.Value = Trading;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@NonStock", SqlDbType.VarChar, 1);
                parameter.Value = NonStock;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@ProductCode", SqlDbType.VarChar, 50);
                parameter.Value = ProductCode;
                objCommProduct.Parameters.Add(parameter);

                if (IsRawList != null && IsRawList.Count > 0)
                {
                    parameter = new SqlParameter("@IsRaws", SqlDbType.VarChar, 5000);
                    parameter.Value = IsRaws;
                    objCommProduct.Parameters.Add(parameter);
                }               



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductMiniDS", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductMiniDS", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchProductMiniDS_WithProductvm(ProductVM Productvm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("DT");
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
                sqlText += @" 
SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent,
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
                                    isnull(Products.IsTransport,'N')IsTransport,
                                    isnull(Products.IsVATRate,'N')IsVATRate,
                                    isnull(Products.IsSDRate,'N')IsSDRate,
                                    isnull(Products.TransactionHoldDate,'0')TransactionHoldDate
FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";
                sqlText += "  WHERE (Products.ItemNo LIKE '%' + @ItemNo + '%'  OR Products.ItemNo IS NULL) AND";
                sqlText += " (Products.CategoryID  LIKE '%'  + @CategoryID +  '%' OR Products.CategoryID IS NULL) ";
                sqlText += " AND (ProductCategories.IsRaw LIKE '%'+ @IsRaw +  '%' OR ProductCategories.IsRaw IS NULL)  ";
                sqlText += " AND (ProductCategories.CategoryName LIKE '%'+ @CategoryName +  '%' OR ProductCategories.CategoryName IS NULL)  ";
                sqlText += " AND (Products.ActiveStatus LIKE '%'+ @ActiveStatus +  '%' OR Products.ActiveStatus IS NULL)";
                sqlText += " AND (Products.Trading LIKE '%' +Trading+  '%' OR Products.Trading IS NULL)";
                sqlText += " AND (Products.NonStock LIKE '%' + @NonStock +  '%' OR Products.NonStock IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%'+@ProductCode+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = Productvm.ItemNo;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@CategoryID", SqlDbType.VarChar, 20);
                parameter.Value = Productvm.CategoryID;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@IsRaw", SqlDbType.VarChar, 50);
                parameter.Value = Productvm.IsRaw;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@CategoryName", SqlDbType.VarChar, 120);
                parameter.Value = Productvm.CategoryName;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@ActiveStatus", SqlDbType.VarChar, 1);
                parameter.Value = Productvm.ActiveStatus;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@Trading", SqlDbType.VarChar, 1);
                parameter.Value = Productvm.Trading;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@NonStock", SqlDbType.VarChar, 1);
                parameter.Value = Productvm.NonStock;
                objComm.Parameters.Add(parameter);

                parameter = new SqlParameter("@ProductCode", SqlDbType.VarChar, 50);
                parameter.Value = Productvm.ProductCode;
                objComm.Parameters.Add(parameter);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductMiniDS_WithProductvm", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductMiniDS_WithProductvm", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchProductFinder(string ProductName, string ProductCode, string IsRaw, string CustomerId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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
                sqlText += @" 


SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsHouseRent,'N')IsHouseRent,
Products.ShortName,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
                                    isnull(Products.VDSRate,0)VDSRate,
isnull(Products.SD,0)SD,
 isnull(Products.CDRate,0)CDRate,
                                    isnull(Products.RDRate,0)RDRate,
                                    isnull(Products.TVARate,0)TVARate,
                                    isnull(Products.ATVRate,0)ATVRate,
                                    isnull(Products.VATRate2,0)VATRate2,
                                    isnull(Products.IsTransport,'N')IsTransport,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent,
isnull(Products.TransactionHoldDate,0)TransactionHoldDate,
isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate

FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";
                sqlText += "  WHERE (1=1) ";
                sqlText += " AND (Products.ActiveStatus ='Y')";
                if (!string.IsNullOrEmpty(IsRaw))
                {
                    sqlText += " AND (ProductCategories.IsRaw = @IsRaw)  ";
                }
                //sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%'+@ProductCode+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " AND (Products.ProductName LIKE '%'+@ProductName+  '%' OR Products.ProductName IS NULL)";
                if (!string.IsNullOrEmpty(CustomerId) && CustomerId != "0")
                {
                    sqlText += "and Products.ItemNo in(select distinct FinishItemNo from BOMs where CustomerID =@CustomerId)   ";
                }

                sqlText += " order by Products.ItemNo ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                SqlParameter parameter = new SqlParameter("@IsRaw", SqlDbType.VarChar, 50);
                parameter.Value = IsRaw;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@ProductCode", SqlDbType.VarChar, 50);
                parameter.Value = ProductCode;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@ProductName", SqlDbType.VarChar);
                parameter.Value = ProductName;
                objCommProduct.Parameters.Add(parameter);

                parameter = new SqlParameter("@CustomerId", SqlDbType.VarChar, 20);
                parameter.Value = CustomerId;
                objCommProduct.Parameters.Add(parameter);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string GetProductUOMc(string uomFrom, string uomTo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(uomFrom))
                {
                    return retResults = string.Empty;

                    //throw new ArgumentNullException("GetProductUOMc", "Invalid UOM From");
                }
                if (string.IsNullOrEmpty(uomTo))
                {

                    return retResults = string.Empty;
                    //throw new ArgumentNullException("GetProductUOMc", "Invalid UOM To");
                }

                #endregion Validation



                #region Find UOM From

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMFrom=@uomFrom ";

                SqlCommand cmdUOMFrom = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@uomFrom", SqlDbType.VarChar, 50);
                parameter.Value = uomFrom;
                cmdUOMFrom.Parameters.Add(parameter);


                object objUOMFrom = cmdUOMFrom.ExecuteScalar();
                if (objUOMFrom == null)
                    throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomFrom + "' from found in conversion");

                #endregion Find UOM From

                #region Find UOM to

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMTo=@uomTo ";

                SqlCommand cmdUOMTo = new SqlCommand(sqlText, currConn);
                object objUOMTo = cmdUOMTo.ExecuteScalar();

                parameter = new SqlParameter("@uomTo", SqlDbType.VarChar, 50);
                parameter.Value = uomTo;
                cmdUOMTo.Parameters.Add(parameter);

                if (objUOMTo == null)
                    throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomTo + "' to found in conversion");

                #endregion Find UOM to

                #region Stock

                sqlText = "  ";

                sqlText = " SELECT top 1 u.Convertion FROM UOMs u ";
                sqlText += " where ";
                sqlText += " u.UOMFrom=@uomFrom ";
                sqlText += " and u.UOMTo=@uomTo ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                parameter = new SqlParameter("@uomFrom", SqlDbType.VarChar, 50);
                parameter.Value = uomFrom;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@uomTo", SqlDbType.VarChar, 50);
                parameter.Value = uomTo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    throw new ArgumentNullException("GetProductNo", "No conversion found from ='" + uomFrom + "'" +
                                                                    " to '" + uomTo + "'");

                retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductUOMc", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductUOMc", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string GetProductUOMn(string ProductName, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ProductName))
                {
                    throw new ArgumentNullException("GetProductNo", "Invalid product name");
                }
                if (string.IsNullOrEmpty(ProductGroup))
                {
                    throw new ArgumentNullException("GetProductNo", "Invalid product group");
                }

                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Stock

                sqlText = "  ";

                sqlText = " SELECT p.UOM ";
                sqlText += " FROM Products p LEFT OUTER JOIN ProductCategories pc ON p.CategoryID=pc.CategoryID";
                sqlText += " where ";
                sqlText += " ProductName=@ProductName";
                sqlText += " and Israw=@ProductGroup";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@ProductName", ProductName);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@ProductGroup", ProductGroup);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string GetProductCodeUOMn(string ProductCode, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation


                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Stock

                sqlText = "  ";

                sqlText = " SELECT p.UOM ";
                sqlText += " FROM Products p LEFT OUTER JOIN ProductCategories pc ON p.CategoryID=pc.CategoryID";
                sqlText += " where ";
                sqlText += " ProductCode=@ProductCode ";
                //sqlText += " and Israw='" + ProductGroup + "' ";
                sqlText += " and Israw=@ProductGroup ";


                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();

                SqlParameter parameter = new SqlParameter("@ProductCode", SqlDbType.VarChar, 50);
                parameter.Value = ProductCode;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@ProductGroup", SqlDbType.VarChar, 50);
                parameter.Value = ProductGroup;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                //if (objItemNo == null)
                //    throw new ArgumentNullException("GetProductNo", "No product found with product name ='" + ProductName + "' and group ='" + ProductGroup + "'");
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectProductDTAll", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string GetProductNoByGroup(string ProductName, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(ProductName))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product name");
                //}
                //if (string.IsNullOrEmpty(ProductGroup))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product group");
                //}

                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction


                #region Stock

                sqlText = "  ";

                sqlText = " select prd.ItemNo from Products prd ";
                sqlText += " inner join ProductCategories cat on cat.CategoryId=prd.CategoryId ";
                sqlText += " where ";
                sqlText += " ProductName=@ProductName";
                sqlText += " and Israw=@ProductGroup";
                // ProductName ProductGroup
                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@ProductName", ProductName);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@ProductGroup", ProductGroup);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                //if (objItemNo == null)
                //    throw new ArgumentNullException("GetProductNo", "No product found with product name ='" + ProductName + "' and group ='" + ProductGroup+"'");
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductNoByGroup", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductNoByGroup", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string GetProductIdByName(string ProductName, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation


                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction


                #region Stock

                sqlText = "  ";

                sqlText = " select ItemNo from Products ";
                sqlText += " where 1=1 ";
                sqlText += " and ProductName=@ProductName";////'" + ProductName + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@ProductName", ProductName);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                //if (objItemNo == null)
                //    throw new ArgumentNullException("GetProductNo", "No product found with product name ='" + ProductName + "' and group ='" + ProductGroup+"'");
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductIdByName", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductIdByName", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string GetProductNoByGroup_Code(string ProductCode, string ProductGroup, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(ProductCode))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product Code");
                //}
                //if (string.IsNullOrEmpty(ProductGroup))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product group");
                //}

                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction


                #region Stock

                sqlText = "  ";

                sqlText = " select prd.ItemNo from Products prd ";
                sqlText += " inner join ProductCategories cat on cat.CategoryId=prd.CategoryId ";
                sqlText += " where ";
                sqlText += " ProductCode=@ProductCode ";
                //sqlText += " and Israw='" + ProductGroup + "' ";
                sqlText += " and Israw=@ProductGroup ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();

                SqlParameter parameter = new SqlParameter("@ProductCode", SqlDbType.VarChar, 50);
                parameter.Value = ProductCode;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                parameter = new SqlParameter("@ProductGroup", SqlDbType.VarChar, 50);
                parameter.Value = ProductGroup;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductNoByGroup_Code", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductNoByGroup_Code", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020 ok
        public DataTable GetProductInfoByItemNo(string ItemNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            DataTable retResults = new DataTable();
            string sqlText = "";
            //SqlConnection currConn = null;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("GetProductCodeAndNameByItemNo", "Invalid product name");
                }


                #endregion Validation

                #region Old connection

                #region open connection and transaction
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();

                ////        if (Vtransaction == null)
                ////        {
                ////            Vtransaction = VcurrConn.BeginTransaction();

                ////        }
                ////    }
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                #region Stock

                sqlText = "  ";

                sqlText = @" select
                pc.CategoryName,pc.IsRaw
                ,p.* from Products p
                left outer join ProductCategories pc on p.CategoryID=pc.CategoryID";
                sqlText += " where ";
                sqlText += " ItemNo=@ItemNo ";

                DataTable dataTable = new DataTable("RIFB");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                parameter.Value = ItemNo;
                cmdRIFB.Parameters.Add(parameter);

                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                reportDataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetProductCodeAndNameByItemNo", "No product found ");
                }

                retResults = dataTable;
                return retResults;

                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductInfoByItemNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductInfoByItemNo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string GetProductTypeByItemNo(string ProductNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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
                ////if (VcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////        if (Vtransaction == null)
                ////        {
                ////            Vtransaction = VcurrConn.BeginTransaction("Product Type");

                ////        }
                ////    }
                ////}

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
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Product Type");
                }

                #endregion open connection and transaction

                #region ProductTYpe

                sqlText = "  ";
                sqlText += " SELECT DISTINCT IsRaw";
                sqlText += " FROM Products p LEFT OUTER JOIN";
                sqlText += " productCategories pc ON p.CategoryID=pc.CategoryID ";

                //sqlText += "  where p.ItemNo='" + ProductNo + "' ";
                sqlText += "  where p.ItemNo=@ProductNo ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                SqlParameter parameter = new SqlParameter("@ProductNo", SqlDbType.VarChar, 20);
                parameter.Value = ProductNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                else
                    retResults = objItemNo.ToString();

                #endregion ProductTYpe

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductTypeByItemNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductTypeByItemNo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////////throw ex;
            }

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                ////if (VcurrConn == null)
                ////{
                ////    if (VcurrConn.State == ConnectionState.Open)
                ////    {
                ////        VcurrConn.Close();

                ////    }
                ////}
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public DataTable SearchProductMiniDSDispose(string purchaseNumber, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("DisposeItem");
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
                            select PD.itemno,
isnull(Products1.ProductName,'N/A')ProductName,
isnull(Products1.ProductCode,'N/A')ProductCode,
Products1.CategoryID, 
isnull(Products1.CategoryName,'N/A')CategoryName ,
isnull(PD.uom,'N/A')UOM,
isnull(Products1.HSCodeNo,'N/A')HSCodeNo,
isnull(Products1.IsRaw,'N/A')IsRaw,
isnull(Products1.CostPrice,0)CostPrice,
isnull(Products1.OpeningBalance,0)OpeningBalance, 
isnull(Products1.SalesPrice,0)SalesPrice,
isnull(Products1.NBRPrice,0)NBRPrice,
isnull( Products1.ReceivePrice,0)ReceivePrice,
isnull( PD.costprice,0)IssuePrice,
isnull(Products1.Packetprice,0)Packetprice, 
isnull(Products1.TenderPrice,0)TenderPrice, 
isnull(Products1.ExportPrice,0)ExportPrice, 
isnull(Products1.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products1.TollIssuePrice,0)TollIssuePrice, 
isnull(Products1.TollCharge,0)TollCharge, 
isnull(PD.vatrate,0)VATRate,
isnull(Products1.SD,0)SD,
isnull(Products1.TradingMarkUp,0)TradingMarkUp,
isnull(PD.UOMQty,PD.quantity)as PurchaseQty,
isnull(Products1.Stock,0) as Stock,

isnull(Products1.QuantityInHand,0)QuantityInHand,
isnull(Products1.Trading,'N')Trading, 
isnull(Products1.FixedVATAmount,0)FixedVATAmount,
isnull(Products1.IsFixedVAT,'N')IsFixedVAT,
isnull(Products1.NonStock,'N')NonStock
from PurchaseInvoiceDetails PD left outer join
(SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
isnull(Products.vatrate,0)VATRate,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,

isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.TransactionHoldDate,'0')TransactionHoldDate,
isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate
FROM         Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
) Products1 on pd.itemno=Products1.itemno

 WHERE 
    (pd.PurchaseInvoiceNo = @purchaseNumber) ";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                if (!objCommProductType.Parameters.Contains("@purchaseNumber"))
                { objCommProductType.Parameters.AddWithValue("@purchaseNumber", purchaseNumber); }
                else { objCommProductType.Parameters["@purchaseNumber"].Value = purchaseNumber; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductMiniDSDispose", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductMiniDSDispose", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchProductbySaleInvoice(string SaleInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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
                sqlText += @"

SELECT s.[SalesInvoiceNo]      
      ,s.[ItemNo]
      ,s.[Quantity]   
      ,s.[UOM]   
      ,s.[SubTotal]
      ,p.[ProductCode]
      ,p.[ProductName]      
  FROM SalesInvoiceDetails s,
Products p
Where s.SalesInvoiceNo in(@SaleInvoiceNo)
and s.[ItemNo]=p.[ItemNo]

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;



                if (!objCommProduct.Parameters.Contains("@SaleInvoiceNo"))
                { objCommProduct.Parameters.AddWithValue("@SaleInvoiceNo", SaleInvoiceNo); }
                else { objCommProduct.Parameters["@SaleInvoiceNo"].Value = SaleInvoiceNo; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductbySaleInvoice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductbySaleInvoice", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public DataTable SearchProductbyMultipleSaleInvoice(string SaleInvoiceNos, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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

                string saleInvoiceNos = string.Empty;

                sqlText = "";
                sqlText += @"

SELECT distinct      
      s.[ItemNo]
      ,s.[UOM]   
      ,p.[ProductCode]
      ,p.[ProductName]   
      ,sum(s.[SubTotal])SubTotal
      ,sum(s.[Quantity])Quantity
   
  FROM SalesInvoiceDetails s,
Products p";
                sqlText += @" Where s.SalesInvoiceNo in('@SaleInvoiceNos')";
                sqlText += @" and s.[ItemNo]=p.[ItemNo]
group by 
      s.[ItemNo]
      ,s.[UOM]   
      ,p.[ProductCode]
      ,p.[ProductName] 
";



                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                SqlParameter parameter = new SqlParameter("@SaleInvoiceNos", SqlDbType.NVarChar);
                parameter.Value = SaleInvoiceNos;
                objCommProduct.Parameters.Add(parameter);


                //if (!objCommProduct.Parameters.Contains("@SaleInvoiceNo"))
                //{ objCommProduct.Parameters.AddWithValue("@SaleInvoiceNo", SaleInvoiceNo); }
                //else { objCommProduct.Parameters["@SaleInvoiceNo"].Value = SaleInvoiceNo; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductbyMultipleSaleInvoice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductbyMultipleSaleInvoice", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchOverheadForBOMNew(string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CompanyOverheads");

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

                sqlText = @"select Productname Headname,RebatePercent RebatePercent,ProductCode OHCode,ItemNo HeadID   
from  Products p LEFT OUTER JOIN
ProductCategories pc ON p.CategoryID=pc.CategoryID 
 WHERE 1=1 
AND pc.IsRaw IN('Overhead','Service(NonStock)')
and p.ActiveStatus=@ActiveStatus 
                          order by Productname";

                SqlCommand objCommOverhead = new SqlCommand();
                objCommOverhead.Connection = currConn;
                objCommOverhead.CommandText = sqlText;
                objCommOverhead.CommandType = CommandType.Text;

                #region param

                if (!objCommOverhead.Parameters.Contains("@ActiveStatus"))
                { objCommOverhead.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommOverhead.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchOverheadForBOMNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchOverheadForBOMNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchChassis(SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Chassis");
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
                        select Heading1 from Trackings
 where issale='Y'
                            order by Heading1
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                // Common Filed
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchChassis", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchChassis", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchEngine(SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Engine");
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
                        select Heading2 from Trackings
 where issale='Y'
                            order by Heading2
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                // Common Filed
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchEngine", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchEngine", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchBanderolProducts(SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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
                sqlText += @" 


SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
isnull(Products.RebatePercent,0)RebatePercent,
isnull(Products.TransactionHoldDate,0)TransactionHoldDate,
isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate

FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
Where Products.Banderol='Y'  ";


                //sqlText += "  WHERE (Products.ItemNo LIKE '%' +'" + ItemNo + "'+ '%'  OR Products.ItemNo IS NULL) AND";
                //sqlText += " (Products.CategoryID  LIKE '%'  +'" + CategoryID + "'+  '%' OR Products.CategoryID IS NULL) ";
                //sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)  ";
                //sqlText += " AND (ProductCategories.CategoryName LIKE '%'+'" + CategoryName + "'+  '%' OR ProductCategories.CategoryName IS NULL)  ";
                //sqlText += " AND (Products.ActiveStatus LIKE '%'+'" + ActiveStatus + "'+  '%' OR Products.ActiveStatus IS NULL)";
                //sqlText += " AND (Products.Trading LIKE '%' +'" + Trading + "'+  '%' OR Products.Trading IS NULL)";
                //sqlText += " AND (Products.NonStock LIKE '%' +'" + NonStock + "'+  '%' OR Products.NonStock IS NULL)";
                //sqlText += " AND (Products.ProductCode LIKE '%'+'" + ProductCode + "'+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchBanderolProducts", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchBanderolProducts", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchProductNames(string ItemNo, string Id, string Names, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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
isnull(Id,'0')Id
,isnull(ItemNo,'0')ItemNo
,isnull(ProductName,'-')ProductName
FROM ProductDetails 
WHERE 1=1

";
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @"  and Id=@Id";
                }

                if (!string.IsNullOrEmpty(Names))
                {
                    //sqlText += @"  and ProductName like '%" + Names + "%'";
                    sqlText += @"  and ProductName like '%' + @Names + '%'";

                }
                sqlText += @"  order by  ProductName";

                SqlCommand objCommCustomerInformation = new SqlCommand();
                objCommCustomerInformation.Connection = currConn;
                objCommCustomerInformation.CommandText = sqlText;
                objCommCustomerInformation.CommandType = CommandType.Text;

                SqlParameter parameter = new SqlParameter("@Names", SqlDbType.VarChar );
                parameter.Value = Names;
                objCommCustomerInformation.Parameters.Add(parameter);


                if (!string.IsNullOrEmpty(ItemNo))
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@ItemNo"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@ItemNo", ItemNo); }
                    else { objCommCustomerInformation.Parameters["@ItemNo"].Value = ItemNo; }
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
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchProductNames", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductNames", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        #region Enum Data


        public string[] productType = new string[]{
                                                    "Overhead",//0
                                                    "Raw",//1
                                                    "Pack",//2
                                                    "Finish",//3
                                                    "Service",//4
                                                    "Trading",//5
                                                    "WIP",//6
                                                    "Export",//7
                                                    "Service(NonStock)",//8
                                                    "NonInventory",//9
                                                    "Toll"//10
                                                };

        public string[] productTypeWithOutTrading = new string[]{
                                                    "Overhead",//0
                                                    "Raw",//1
                                                    "Pack",//2
                                                    "Finish",//3
                                                    "Service",//4
                                                    "WIP",//5
                                                    "Export",//6
                                                    "Service(NonStock)",//7
                                                    "NonInventory"//8
                                                };
        public string[] productTypeWithOutService = new string[]{
                                                    "Overhead",//0
                                                    "Raw",//1
                                                    "Pack",//2
                                                    "Finish",//3
                                                    "Trading",//4
                                                    "WIP",//5
                                                    "Export",//6
                                                };
        public string[] productTypeWithOutTradingService = new string[]{
                                                    "Overhead",//0
                                                    "Raw",//1
                                                    "Pack",//2
                                                    "Finish",//3
                                                    "WIP",//4
                                                    "Export",//5
                                                };
        public IList<string> ProductTypeList
        {
            get
            {
                return productType.ToList<string>();

            }
        }

        public IList<string> ProductTypeListOutTrading
        {
            get
            {
                return productTypeWithOutTrading.ToList<string>();

            }
        }

        public IList<string> ProductTypeListOutService
        {
            get
            {
                return productTypeWithOutService.ToList<string>();

            }
        }

        public IList<string> ProductTypeListOutTradingService
        {
            get
            {
                return productTypeWithOutTradingService.ToList<string>();

            }
        }

        public string[] productTypeWithOutOverhead = new string[]{
                                                    //0
                                                    "Raw",//1
                                                    "Pack",//2
                                                    "Finish",//3
                                                    "Service",//4
                                                    "Trading",//5
                                                    "WIP",//6
                                                    "Export",//7
                                                    "Service(NonStock)",//8
                                                    "NonInventory"//9
                                                };

        public IList<string> ProductTypeListWithOutOverhead
        {
            get
            {
                return productTypeWithOutOverhead.ToList<string>();

            }
        }

        #endregion

        public DataTable SearchHSCode(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("HSCode");

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
isnull(Id,'0')Id
,isnull(ItemNo,'0')ItemNo
,isnull(HSCode,'-')HSCode
FROM ProductHSCodes 
WHERE 1=1

";
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
                        else if (conditionFields[i].ToLower().Contains("isnull"))
                        {
                            sqlText += " AND " + "isnull(" + cField + ",'Y')=" + " @" + cField + "";
                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }

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

                da.Fill(dataTable);

                #endregion
            }

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchHSCode", ex.ToString() + "\n" + sqlText);

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

            return dataTable;

        }


        public string[] InsertToProductNames(ProductNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  ProductDetails";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Customers Address",
                                                    "Unable to create new Product Name");
                }


                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";
                sqlText += "insert into ProductDetails";
                sqlText += "(";
                sqlText += "ItemNo,";
                sqlText += "ProductName";
                sqlText += ")";
                sqlText += " values(";

                sqlText += " @itemNo,";
                sqlText += " @pName";
                sqlText += ")SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@itemNo", vm.ItemNo);
                cmdInsert.Parameters.AddWithValue("@pName", vm.ProductName);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                vm.Id = transResult;
                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Product Name successfully Added";
                    retResults[2] = "" + vm.Id;
                    retResults[3] = "" + vm.Id;
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
                            retResults[1] = "Requested Product Name successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + nextId;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add Product";
                            retResults[2] = "";
                            retResults[3] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Product ";
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
                    transaction.Rollback();

                }

                FileLogger.Log("ProductDAL", "InsertToProductNames", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    transaction.Rollback();

                }

                FileLogger.Log("ProductDAL", "InsertToProductNames", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] InserToCustomerRate(CustomerRateVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                sqlText = "select COUNT(BranchId) FROM  ProductCustomerRate where 1=1 and ItemNo=@ItemNo and CustomerId=@CustomerId";
                SqlCommand cmdIsExit = new SqlCommand(sqlText, currConn);
                cmdIsExit.Transaction = transaction;
                cmdIsExit.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdIsExit.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                int IsExit = (int)cmdIsExit.ExecuteScalar();
                if (IsExit > 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Customer Rate";
                    throw new ArgumentNullException("InserToCustomerRate", "This Product Rate Customer Already Exit");
                }
                #region Customer  new id generation
                //sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  ProductCustomerRate";
                //SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                //cmdNextId.Transaction = transaction;
                //int nextId = (int)cmdNextId.ExecuteScalar();
                //if (nextId <= 0)
                //{
                //    throw new ArgumentNullException("Customers Address",
                //                                    "Unable to create new Product Name");
                //}


                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";
                sqlText += "insert into ProductCustomerRate";
                sqlText += "(";
                sqlText += "CustomerId,";
                sqlText += "ItemNo,";
                sqlText += "NBRPrice,";
                sqlText += "BranchId,";
                sqlText += "TollCharge";
                sqlText += ")";
                sqlText += " values(";
                sqlText += " @CustomerId,";
                sqlText += " @itemNo,";
                sqlText += " @NBRPrice,";
                sqlText += " @BranchId,";
                sqlText += " @TollCharge";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                cmdInsert.Parameters.AddWithValue("@itemNo", vm.ItemNo);
                cmdInsert.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValue("@TollCharge", vm.TollCharge);
                //cmdInsert.Parameters.AddWithValue("@pName", vm.ProductName);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                //vm.Id = transResult;
                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Product Name successfully Added";
                    //retResults[2] = "" + vm.Id;
                    //retResults[3] = "" + vm.Id;
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
                            retResults[1] = "Requested Product Name successfully Added";
                            retResults[2] = "";
                            retResults[3] = "";
                            //retResults[2] = "" + nextId;
                            //retResults[3] = "" + nextId;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add Product";
                            retResults[2] = "";
                            retResults[3] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Product ";
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
                    transaction.Rollback();

                FileLogger.Log("ProductDAL", "InserToCustomerRate", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();

                FileLogger.Log("ProductDAL", "InserToCustomerRate", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] UpdateToProductNames(ProductNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException("UpdateToProduct",
                                                    "Invalid Product  Name");
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
                sqlText = "update ProductDetails set";
                //sqlText += " ProductName='" + vm.ProductName + "'";
                //sqlText += " where id='" + vm.Id + "'";
                sqlText += " ProductName=@ProductName";
                sqlText += " where id=@id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@ProductName", vm.ProductName);
                cmdUpdate.Parameters.AddWithValue("@id", vm.Id);
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
                            retResults[1] = "Requested Product  Name successfully Update";
                            retResults[2] = vm.Id.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update Product  Name ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update Product group";
                    }
                }




                #endregion Commit


                #endregion

            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "UpdateToProductNames", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {

                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "611435040", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] UpdateToExitProduct(ProductVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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


                #region Update new customer group
                sqlText = "";
                sqlText = "update Products set";
                sqlText += " ReportType=@ReportType";
                sqlText += " where CategoryID=@CategoryID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@ReportType", vm.ReportType);
                cmdUpdate.Parameters.AddWithValue("@CategoryID", vm.CategoryID);
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
                            retResults[1] = "Requested Product  Report Type successfully Update";
                            retResults[2] = vm.CategoryID.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update Product  Report Type";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update Product group";
                    }
                }

                #endregion Commit


                #endregion

            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "UpdateToExitProduct", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {

                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("UpdateToExitProduct", "611435040", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] DeleteProductNames(string itemNo, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

            #endregion Variables

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
                    sqlText = "delete ProductDetails where Id=@Id";
                }
                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText = "delete ProductDetails where itemNo=@itemNo";
                }
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                cmdDelete.Parameters.AddWithValue("@Id", Id);
                cmdDelete.Parameters.AddWithValue("@itemNo", itemNo);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Product successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Product  Address successfully deleted";
                        retResults[2] = "";

                    }
                }

                #endregion Commit

            }

            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "DeleteProductNames", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DeleteProductNames", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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
            #endregion finally

            return retResults;

        }

        public string[] DeleteToProductCustomerRate(string itemNo, string CustomerId, int BranchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = CustomerId;
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
                if (string.IsNullOrEmpty(CustomerId))
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

                sqlText = "delete ProductCustomerRate where CustomerId=@CustomerId and BranchId=@BranchId and itemNo=@itemNo";

                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                cmdDelete.Parameters.AddWithValue("@CustomerId", CustomerId);
                cmdDelete.Parameters.AddWithValue("@BranchId", BranchId);
                cmdDelete.Parameters.AddWithValue("@itemNo", itemNo);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer Product Rate successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customer Product Rate successfully deleted";
                        retResults[2] = "";

                    }
                }

                #endregion Commit

            }

            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "DeleteToProductCustomerRate", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DeleteToProductCustomerRate", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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
            #endregion finally

            return retResults;

        }

        public string GetExistingProductName(string ProductName, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                }

                #endregion open connection and transaction
                #region Stock

                sqlText = "  ";
                sqlText += " SELECT DISTINCT ProductName";
                sqlText += " FROM Products";
                sqlText += "  where ProductName=@ProductName ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@ProductName", ProductName);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                else
                    retResults = objItemNo.ToString();

                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetExistingProductName", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetExistingProductName", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

            #region Results

            return retResults;

            #endregion

        }

        public string TrackingStockCheck(string ItemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;

            #endregion

            #region Try

            try
            {


                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                }

                #endregion open connection and transaction
                #region Stock

                sqlText = "  ";
                sqlText += @" select COUNT(id)Stock from PurchaseSaleTrackings
                                where IsSold=0";
                sqlText += "  and  ItemNo=@ItemNo ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 50);
                parameter.Value = ItemNo;
                cmdGetLastNBRPriceFromBOM.Parameters.Add(parameter);


                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                else
                    retResults = objItemNo.ToString();

                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "TrackingStockCheck", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "TrackingStockCheck", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

            #region Results

            return retResults;

            #endregion

        }

        public DataTable SearchRawItemNo(string IssueNo, SysDBInfoVMTemp connVM = null)
        {
            // for TollReceive
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CompanyOverheads");

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
                sqlText = "Select ItemNo,CostPrice from IssueDetails ";
                sqlText += " where IssueNo=@IssueNo";

                SqlCommand objCommOverhead = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@IssueNo", SqlDbType.VarChar, 20);
                parameter.Value = IssueNo;
                objCommOverhead.Parameters.Add(parameter);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchRawItemNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchRawItemNo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchByItemNo(string ItemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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

                sqlText = @"SELECT
                                    Products.ItemNo,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(Products.ProductDescription,'N/A')ProductDescription,
                                    Products.CategoryID, 
                                    isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
                                    isnull(Products.UOM,'N/A')UOM,
                                    isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
                                    isnull(Products.CostPrice,0)CostPrice,
                                    isnull(Products.SalesPrice,0)SalesPrice,
                                    isnull(Products.NBRPrice,0)NBRPrice,
                                    isnull(ProductCategories.IsRaw,'N')IsRaw,
                                    isnull(Products.SerialNo,'N/A')SerialNo ,
                                    isnull(Products.HSCodeNo,'N/A')HSCodeNo,
                                    isnull(Products.VATRate,0)VATRate,
                                    isnull(Products.ActiveStatus,'N')ActiveStatus,
                                    isnull(Products.OpeningBalance,0)OpeningBalance,
                                    isnull(Products.Comments,'N/A')Comments,
                                    'N/A' HSDescription, 
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(Products.SD,0)SD, 
                                    isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
                                    Products.TradingMarkUp,Products.NonStock,
                                    isnull(Products.QuantityInHand,0)QuantityInHand,
                                    convert(varchar, Products.OpeningDate,120)OpeningDate,
                                    isnull(Products.ReceivePrice,0)ReceivePrice,
                                    isnull(Products.IssuePrice,0)IssuePrice,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.TollCharge,0)TollCharge,
                                    isnull(Products.Banderol,'N')Banderol,
                                    isnull(Products.IsTransport,'N')IsTransport,
                                    isnull(Products.TDSCode,'-')TDSCode,
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
isnull(Products.TollProduct,'N')TollProduct,
isnull(Products.TransactionHoldDate,'0')TransactionHoldDate,
isnull(Products.IsFixedVATRebate,'N')IsFixedVATRebate

                                    FROM Products LEFT OUTER JOIN
                                    ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
                      
                                    WHERE (Products.ItemNo = @ItemNo)  ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (!objCommProduct.Parameters.Contains("@ItemNo"))
                { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "SearchByItemNo", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchByItemNo", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string GetTransactionType(string itemNo, string effectDate, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {


                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                }

                #endregion open connection and transaction
                #region Stock

                sqlText = "  ";
                sqlText += " SELECT DISTINCT TransactionType,ReceiveDate";
                sqlText += " FROM PurchaseInvoiceDetails";
                sqlText += "  where  ItemNo=@itemNo  and Post='Y'  ";
                //sqlText += " and ReceiveDate<='" + effectDate + "' order by ReceiveDate desc ";
                sqlText += " and ReceiveDate<=@effectDate order by ReceiveDate desc ";




                SqlCommand cmbTransactionType = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@itemNo", SqlDbType.VarChar, 20);
                parameter.Value = itemNo;
                cmbTransactionType.Parameters.Add(parameter);

                parameter = new SqlParameter("@effectDate", SqlDbType.DateTime);
                parameter.Value = effectDate;
                cmbTransactionType.Parameters.Add(parameter);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmbTransactionType);
                dataAdapter.Fill(dt);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        retResults = dt.Rows[0][0].ToString();
                    }
                }


                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetTransactionType", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetTransactionType", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

            #region Results

            return retResults;

            #endregion

        }

        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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

                sqlText = @"
SELECT
	row_number() OVER (ORDER BY p.ItemNo) SL
	,p.[ProductCode] Code
      ,p.[ProductName]
      ,p.[ProductDescription] [Description]
      ,pc.IsRaw CategoryType
      ,pc.CategoryName [Group]
      ,p.[UOM]
      ,OpeningTotalCost	TotalPrice 
      ,p.[OpeningBalance] OpeningQuantity
      ,p.[SerialNo] RefNo
      ,p.[HSCodeNo] HSCode
	  ,P.NBRPrice
	  ,p.VATRate
	  ,p.Comments
	  ,p.ActiveStatus
	  ,p.SD SDRate
	  ,p.PacketPrice
	  ,p.Trading
	  ,p.TradingMarkUp
      ,CostPrice
	  ,p.NonStock
	  ,cast(p.OpeningDate as varchar(100)) OpeningDate
	  ,p.TollCharge
	  ,isnull(p.GenericName,'-')GenericName
	  ,isnull(p.DARNo,'-')DARNo
	  ,isnull(p.IsConfirmed,'Y')IsConfirmed
	  ,isnull(p.ReportType,'-')ReportType
	  ,isnull(p.TransactionHoldDate,0)TransactionHoldDate
	  ,isnull(p.IsFixedVATRebate,'N')IsFixedVATRebate
      
	  


  FROM [Products] p left outer join ProductCategories pc
  on p.CategoryID = pc.CategoryID
  where p.ItemNo!='ovh0' and p.ItemNo in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += " order by pc.IsRaw, p.ProductCode, p.ProductName";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;
                //for (int i = 0; i < len; i++)
                //{
                //    cmd.Parameters.AddWithValue("@itemNo" + i, ids[i]);
                //}

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);

                string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical");

                if (IsPharmaceutical == "N")
                {
                    string[] columnNames = { "GenericName", "DARNo" };

                    dataTable = OrdinaryVATDesktop.DtDeleteColumns(dataTable, columnNames);
                }

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

                FileLogger.Log("ProductDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

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

        public DataTable GetBOMData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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

                //string paramIDs = string.Join(",", ids.Select((id, index) => "@ID" + index));

                sqlText = @"

declare @overHeadName varchar(500)  = 'Salary and wages, admin, selling and distribution, warehousing service, transportation, fuel, maintenance, banking service, general overhead and profit'
declare @overHeadCode varchar(50)  = (select SettingValue from Settings where SettingGroup = 'BOM' and SettingName='DefaultOverHeadCode')

select * from (
select ProductCode FCode, ProductName FName,UOM FUOM, ProductName RName, ProductCode RCode, UOM RUOM, '-'CustomerName,'-'CustomerCode
, Format(GetDate(), 'yyyy-MM-dd') FirstSupplyDate, '-' ReferenceNo, Format(GetDate(), 'yyyy-MM-dd') EffectDate
, 'VAT 4.3' VATName, 1 TotalQuantity, 1 UseQuantity, 0 WastageQuantity, CostPrice Cost, 0 RebateRate, 'Y' IssueOnProduction
from Products 

where ItemNo in ('" + string.Join("','", ids) + @"')


union all 

select ProductCode FCode, ProductName FName,UOM FUOM,@overHeadName RName, @overHeadCode RCode, '-' RUOM, '-'CustomerName,'-'CustomerCode
, Format(GetDate(), 'yyyy-MM-dd') FirstSupplyDate, '-' ReferenceNo, Format(GetDate(), 'yyyy-MM-dd') EffectDate
, 'VAT 4.3' VATName, 1 TotalQuantity, 1 UseQuantity, 0 WastageQuantity, TradingMarkUp Cost, 0 RebateRate, 'N' IssueOnProduction
from Products 

where ItemNo in ('" + string.Join("','", ids) + @"')

) BOMs

order by FCode";




                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;

                //where ItemNo in (" + paramIDs + @")


                 //SqlParameter parameter = new SqlParameter("@IDs", SqlDbType.VarChar);
                 //parameter.Value = ids;
                 //cmd.Parameters.Add(parameter);

                //for (int i = 0; i < ids.Count; i++)
                //{
                //    cmd.Parameters.AddWithValue("@ID" + i, ids[i]);
                //}
                

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

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

                FileLogger.Log("ProductDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

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

        public DataTable GetTradingExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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
	row_number() OVER (ORDER BY p.ItemNo) SL
	,p.[ProductCode] Code
      ,p.[ProductName]
      ,p.[ProductDescription] [Description]
      ,pc.IsRaw CategoryType
      ,pc.CategoryName [Group]
      ,p.[UOM]
      ,OpeningTotalCost	TotalPrice 
      ,p.[OpeningBalance] OpeningQuantity
      ,p.[SerialNo] RefNo
      ,p.[HSCodeNo] HSCode
	  ,P.NBRPrice
	  ,p.VATRate
	  ,p.TradingSaleVATRate
	  ,p.TradingSaleSD
	  ,p.Comments
	  ,p.ActiveStatus
	  ,p.SD SDRate
	  ,p.PacketPrice
	  ,p.Trading
	  ,p.TradingMarkUp
      ,CostPrice
	  ,p.NonStock
	  ,cast(p.OpeningDate as varchar(100)) OpeningDate
	  ,p.TollCharge
	  ,isnull(p.GenericName,'-')GenericName
	  ,isnull(p.DARNo,'-')DARNo
	  ,isnull(p.IsConfirmed,'Y')IsConfirmed
	  ,isnull(p.ReportType,'-')ReportType
	  ,isnull(p.TransactionHoldDate,'0')TransactionHoldDate
	  ,isnull(p.IsFixedVATRebate,'N')IsFixedVATRebate


  FROM [Products] p left outer join ProductCategories pc
  on p.CategoryID = pc.CategoryID
  where p.ItemNo!='ovh0' and p.ItemNo in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += " order by pc.IsRaw, p.ProductCode, p.ProductName";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                //for (int i = 0; i < len; i++)
                //{
                //    cmd.Parameters.AddWithValue("@itemNo" + i, ids[i]);
                //}

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);

                string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical");

                if (IsPharmaceutical == "N")
                {
                    string[] columnNames = { "GenericName", "DARNo" };

                    dataTable = OrdinaryVATDesktop.DtDeleteColumns(dataTable, columnNames);
                }

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

                FileLogger.Log("ProductDAL", "GetTradingExcelData", ex.ToString() + "\n" + sqlText);

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

        public DataTable GetExcelProductDetails(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductD");

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

                sqlText = @"SELECT ROW_NUMBER() over (order by pd.ItemNo) SL

      ,p.[ProductCode] Code
      ,pd.[ProductName]

  FROM ProductDetails pd left outer join Products p
  on pd.ItemNo  = p.ItemNo
  where pd.ItemNo in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += " order by p.ProductCode,p.ProductName";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                //for (int i = 0; i < len; i++)
                //{
                //    cmd.Parameters.AddWithValue("@itemNo" + i, ids[i]);
                //}
                cmd.CommandTimeout = 500;
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

        public DataTable GetExcelProductStock(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductStock");

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
                var len = ids.Count;

                #region Sql Command

                sqlText = @"

    DECLARE @BranchCount INT;
    DECLARE @BranchCode varchar(100);

    select @BranchCount=Count(BranchID) from BranchProfiles;
    Create table #ProductStocks(id int identity(1,1),
    ItemNo varchar(100),BranchId varchar(100),StockQuantity decimal(18,5),StockValue  decimal(18,5),Comments varchar(500))
    WHILE 1 <=  @BranchCount
    BEGIN
    set @BranchCode='NA'

    select @BranchCode=BranchCode  from BranchProfiles where BranchID=@BranchCount
    if(@BranchCode!='NA') 
    begin
    insert into #ProductStocks(ItemNo,BranchId,StockQuantity,StockValue,Comments)
    select ItemNo,@BranchCount,StockQuantity,StockValue,Comments from (
    select ItemNo,BranchId,StockQuantity,StockValue,Comments from ProductStocks
    where BranchId=@BranchCount
    and ItemNo in (@AllItem)
    union all
    select ItemNo,@BranchCount,0 StockQuantity,0 StockValue,''Comments from Products where ActiveStatus='Y'
    and ItemNo not in
    (select ItemNo from ProductStocks where BranchId=@BranchCount
    and ItemNo in (@AllItem))

    and ItemNo in (@AllItem)

    ) as a
    end
    SET @BranchCount = @BranchCount - 1;
    END;


    select b.BranchCode,b.BranchName,pc.IsRaw CategoryType,pc.CategoryName,p.ProductCode, p.ProductName,ps.StockQuantity,ps.StockValue,ps.Comments
    from #ProductStocks ps 
    left outer join Products p on ps.ItemNo=p.ItemNo
    left outer join ProductCategories pc on  p.CategoryID=pc.CategoryID
    left outer join BranchProfiles b on ps.BranchId=b.BranchId 
    ";
                //sqlText += @" order by b.BranchCode,b.BranchName,pc.IsRaw ,pc.CategoryName,p.ProductCode
                sqlText += @" order by b.BranchCode,pc.IsRaw,p.ProductCode,p.ProductName
    drop table #ProductStocks ";

                string AllItem = "";

                for (int i = 0; i < len; i++)
                {
                    AllItem += "'" + ids[i] + "',";
                }

                AllItem = AllItem.TrimEnd(',') + "";


                sqlText = sqlText.Replace("@AllItem", AllItem);
                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;
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

                FileLogger.Log("ProductDAL", "GetExcelProductStock", ex.ToString() + "\n" + sqlText);

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

        #region WEB Methods

        public List<ProductVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
'B' Sl, ItemNo
, CategoryName
FROM Products
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' ItemNo
, 'ALL Product' CategoryName  
FROM Products
)
AS a
order by Sl,CategoryName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<ProductVM> DropDown(int CategoryID = 0, string IsRaw = "", SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
p.ItemNo
,p.ProductCode
,p.ProductName
,p.CategoryID
,pc.IsRaw
   FROM Products p
LEFT OUTER JOIN ProductCategories pc ON p.CategoryID = pc.CategoryID
WHERE  1=1 AND p.ActiveStatus = 'Y'
";
                if (CategoryID > 0)
                {
                    sqlText += @" AND p.CategoryID=@CategoryID";
                }
                if (!string.IsNullOrEmpty(IsRaw))
                {
                    sqlText += @" AND pc.IsRaw=@IsRaw";
                }
                sqlText += @" order by p.ProductName";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (CategoryID > 0)
                {
                    objComm.Parameters.AddWithValue("@CategoryID", CategoryID);
                }
                if (!string.IsNullOrEmpty(IsRaw))
                {
                    objComm.Parameters.AddWithValue("@IsRaw", IsRaw);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductName = vm.ProductName + "(" + vm.ProductCode + ")";
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                //// throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<ProductVM> GetProductByType(string type, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
select * from Products pr 
left outer join ProductCategories pc
on pr.CategoryID=pc.CategoryID
where pc.IsRaw=@type and pr.ActiveStatus = 'Y'
ORDER BY pr.ProductName;
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@type", type);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductName = vm.ProductName + "(" + vm.ProductCode + ")";
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "GetProductByType", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "GetProductByType", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public List<ProductVM> DropDownByCategory(string catId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
af.ItemNo
,af.ProductCode
,af.ProductName
   FROM Products af
WHERE  1=1 AND af.ActiveStatus = 'Y' AND af.CategoryID=@catId
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@catId", catId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductName = vm.ProductName + "(" + vm.ProductCode + ")";
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductDAL", "DropDownByCategory", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DropDownByCategory", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        private void SetDefaultValue(ProductVM vm, SysDBInfoVMTemp connVM = null)
        {
            //if (string.IsNullOrEmpty(vm.hsdescription))
            //{
            //    vm.ProductDescription = "-";
            //}
            if (string.IsNullOrEmpty(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrEmpty(vm.ProductDescription))
            {
                vm.ProductDescription = "-";
            }
            if (string.IsNullOrEmpty(vm.Comments))
            {
                vm.Comments = "-";
            }
            if (string.IsNullOrEmpty(vm.HSCodeNo))
            {
                vm.HSCodeNo = "NA";
            }
        }

        public string[] InsertToProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType, bool AutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool IsNestle = false)
        {
            #region Initializ

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
            string productCode = vm.ProductCode;
            string itemType = ItemType;
            bool Auto = false;
            string nextId = "0";
            #endregion Initializ

            #region Try
            try
            {
                #region Validation
                SetDefaultValue(vm);
                //////////if (string.IsNullOrEmpty(vm.ProductName))
                //////////{
                //////////    throw new ArgumentNullException("InsertToItem",
                //////////                                    "Please enter product name.");
                //////////}
                ////////////if (string.IsNullOrEmpty(vm.CategoryName))
                ////////////{
                ////////////    throw new ArgumentNullException("InsertToItem",
                ////////////                                    "Please enter product category.");
                ////////////}
                //////////if (string.IsNullOrEmpty(vm.UOM))
                //////////{
                //////////    throw new ArgumentNullException("InsertToItem",
                //////////                                    "Please enter Product UOM.");
                //////////}

                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                if (itemType == "Overhead")
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead", connVM, VcurrConn, Vtransaction) == "Y");

                }
                else
                {
                    Auto = commonDal.settingValue("AutoCode", "Item", connVM, VcurrConn, Vtransaction) == "Y";

                }

                if (!Auto)
                {
                    if (vm.ProductCode == "0" || vm.ProductCode == "-")
                    {
                        throw new ArgumentNullException("InsertToItem",
                            "Please enter product Code.");
                    }
                }


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

                #region check in product table
                //if (!string.IsNullOrEmpty(ProductName))
                //{
                //    sqlText = "select count(distinct ProductName) from Products where ProductName='" + ProductName + "'";
                //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //    cmdIdExist.Transaction = transaction;
                //    countId = (int)cmdIdExist.ExecuteScalar();
                //    if (countId > 0)
                //    {
                //        //throw new ArgumentNullException("InsertToProducts", "Product already exist.Do you want to save?");
                //        retResults[0] = "Exist";
                //        return retResults;
                //    }

                //}
                #endregion

                #region Report Type

                ProductCategoryDAL productcategorydal = new ProductCategoryDAL();
                ProductCategoryVM cVm = new ProductCategoryVM();

                cVm = productcategorydal.SelectAllList(Convert.ToInt32(vm.CategoryID), null, null, currConn, transaction, connVM).FirstOrDefault();

                vm.ReportType = cVm.ReportType;

                #endregion

                #region Product Name and Category Id not exist,Insert new Product

                #region ProductID

                if (itemType == "Overhead")
                {
                    //                    sqlText = @"select 'ovh'+ltrim(rtrim(str(isnull(max(substring(ItemNo,4,len(ItemNo))),0)+1))) from Products 
                    //                               LEFT outer JOIN ProductCategories pc ON Products.CategoryID=pc.CategoryID
                    //                                WHERE pc.IsRaw='Overhead'";
                    sqlText = "select 'ovh'+ltrim(rtrim(str(isnull(max(cast(substring(ItemNo,4,len(ItemNo))AS INT) ),0)+1))) from Products WHERE SUBSTRING(ItemNo,1,3)='ovh'";

                }
                else
                {
                    sqlText = "select isnull(max(cast(ItemNo as int)),0)+1 FROM  Products WHERE SUBSTRING(ItemNo,1,3)<>'ovh'";

                }

                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                object objNextId = cmdNextId.ExecuteScalar();
                if (objNextId == null)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Unable to create new Overhead information Id");
                }

                nextId = objNextId.ToString();
                if (string.IsNullOrEmpty(nextId))
                {


                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Unable to create new Overhead information Id");
                }
                #endregion ProductID

                #region Code
                if (Auto == false)
                {
                    if (AutoSave)
                    {
                        //vm.CategoryID = "2";
                        if (string.IsNullOrWhiteSpace(vm.ProductCode))
                        {
                            vm.ProductCode = nextId;
                        }
                    }
                    else if (string.IsNullOrEmpty(productCode))
                    {
                        throw new ArgumentNullException("InsertToItem", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(ItemNo) from Products where  ProductCode=@productCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;

                        SqlParameter parameter = new SqlParameter("@productCode", SqlDbType.VarChar, 50);
                        parameter.Value = productCode;
                        cmdCodeExist.Parameters.Add(parameter);


                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToItem", "Same Code('" + productCode + "') already exist");
                        }
                    }
                }
                else
                {
                    productCode = nextId;
                }
                #endregion Code

                //////var Id = _cDal.NextId("Products", null, null).ToString();

                #region Mapping Code Check

                bool MappingCodeCheck = Convert.ToBoolean(commonDal.settingValue("Products", "IsMappingCodeCheck", connVM, VcurrConn, Vtransaction) == "Y");

                if (MappingCodeCheck)
                {
                    ProductMappingCodeCheck(productCode, "0", false, currConn, transaction, connVM);
                }

                #endregion

                if (string.IsNullOrWhiteSpace(vm.IsChild))
                {
                    vm.IsChild = "N";
                }

                vm.ItemNo = nextId;
                if (IsNestle)
                {
                    productCode = nextId;
                }
                sqlText = "";

                sqlText += " INSERT into Products";
                sqlText += "(";
                sqlText += "ItemNo,";
                sqlText += "ProductCode,";
                sqlText += "ProductName,";
                sqlText += "ProductDescription,";
                sqlText += "CategoryID,";
                sqlText += "UOM,";
                sqlText += "CostPrice,";
                sqlText += "SalesPrice,";
                sqlText += "NBRPrice,";
                sqlText += "ReceivePrice,";
                sqlText += "IssuePrice,";
                sqlText += "TenderPrice,";
                sqlText += "ExportPrice,";
                sqlText += "InternalIssuePrice,";
                sqlText += "TollIssuePrice,";
                sqlText += "TollCharge,";
                sqlText += "OpeningBalance,";
                sqlText += "SerialNo,";
                sqlText += "HSCodeNo,";
                sqlText += "VATRate,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "SD,";
                sqlText += "PacketPrice,";
                sqlText += "Trading,";
                sqlText += "TradingMarkUp,";
                sqlText += "NonStock,";
                sqlText += "Quantityinhand,";
                sqlText += "RebatePercent,";
                sqlText += "OpeningTotalCost,";
                sqlText += "OpeningDate,";
                sqlText += "Banderol,";
                sqlText += "CDRate,";
                sqlText += "RDRate,";
                sqlText += "TVARate,";
                sqlText += "ATVRate,";
                sqlText += "VATRate2,";
                sqlText += "TradingSaleVATRate,";
                sqlText += "TradingSaleSD,";
                sqlText += "VDSRate,";
                sqlText += "IsExempted,";
                sqlText += "IsZeroVAT,";
                sqlText += "IsTransport,";
                sqlText += "TollProduct,";
                sqlText += "BranchId,";
                sqlText += "TDSCode,";
                sqlText += "IsFixedVAT,";
                sqlText += "IsFixedSD,";
                sqlText += "IsFixedCD,";
                sqlText += "IsFixedRD,";
                sqlText += "IsFixedAIT,";
                sqlText += "IsFixedVAT1,";
                sqlText += "IsFixedAT,";
                sqlText += "IsFixedOtherSD,";
                sqlText += "IsHouseRent,";
                sqlText += "ShortName,";
                sqlText += "IsVDS,";
                sqlText += "HPSRate,";
                sqlText += "FixedVATAmount,";
                sqlText += "AITRate,";
                sqlText += "SDRate,";
                sqlText += "VATRate3,";
                sqlText += "UOM2,";
                sqlText += "UOMConvertion,";
                sqlText += "IsArchive,";
                sqlText += "GenericName,";
                sqlText += "DARNo,";
                sqlText += "ReportType,";
                sqlText += "TransactionHoldDate,";
                sqlText += "IsFixedVATRebate,";
                sqlText += "IsConfirmed,";
                sqlText += "IsSample,";
                sqlText += "IsChild,";
                sqlText += "MasterProductItemNo,";
                sqlText += "Option1,";
                sqlText += "Volume,";
                sqlText += "VolumeUnit,";
                sqlText += "PackSize,";
                sqlText += "IsPackCal,";
                sqlText += "TollOpeningQuantity";

                ////sqlText += "WastageTotalQuantity,";
                ////sqlText += "WastageTotalValue";


                sqlText += ")";
                sqlText += " values(";
                sqlText += "@ItemNo,";
                sqlText += "@productCode,";
                sqlText += "@ProductName,";
                sqlText += "@ProductDescription,";
                sqlText += "@CategoryID,";
                sqlText += "@UOM,";
                sqlText += "@CostPrice,";//CostPrice
                sqlText += "@CostPrice,";//SalePrice
                sqlText += "@NBRPrice,";//NBRPrice
                sqlText += "@NBRPrice,";//ReceivePrice
                sqlText += "@NBRPrice,";//IssuePrice
                sqlText += "" + 0 + ",";//TenderPrice
                sqlText += "" + 0 + ",";//ExportPrice
                sqlText += "" + 0 + ",";//InternalIssuePrice
                sqlText += "" + 0 + ",";//TollIssueprice
                sqlText += "@TollCharge,";//TollCharge
                sqlText += "@OpeningBalance,";//OpeningBalance
                sqlText += "@SerialNo,";
                sqlText += "@HSCodeNo,";
                sqlText += "@VATRate,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@SD,";
                sqlText += "@Packetprice,";
                sqlText += "@Trading,";
                sqlText += "@TradingMarkUp,";
                sqlText += "@NonStock,";
                sqlText += "" + 0 + ",";//QuantityInHand
                sqlText += "@RebatePercent,";//QuantityInHand
                sqlText += "@OpeningTotalCost,";//QuantityInHand
                sqlText += "@OpeningDate,";//OpeningDate
                sqlText += "@Banderol,";//Banderol

                sqlText += "@CDRate,";
                sqlText += "@RDRate,";
                sqlText += "@TVARate,";
                sqlText += "@ATVRate,";
                sqlText += "@VATRate2,";
                sqlText += "@TradingSaleVATRate,";
                sqlText += "@TradingSaleSD,";
                sqlText += "@VDSRate,";

                sqlText += "@IsExempted,";
                sqlText += "@IsZeroVAT,";
                sqlText += "@IsTransport,";

                sqlText += "@TollProduct,";//TollProduct
                sqlText += "@BranchId,";
                sqlText += "@TDSCode,";
                sqlText += "@IsFixedVAT,";
                sqlText += "@IsFixedSD,";
                sqlText += "@IsFixedCD,";
                sqlText += "@IsFixedRD,";
                sqlText += "@IsFixedAIT,";
                sqlText += "@IsFixedVAT1,";
                sqlText += "@IsFixedAT,";
                sqlText += "@IsFixedOtherSD,";
                sqlText += "@IsHouseRent,";
                sqlText += "@ShortName,";

                sqlText += "@IsVDS,";
                sqlText += "@HPSRate,";

                sqlText += "@FixedVATAmount,";
                sqlText += "@AITRate,";
                sqlText += "@SDRate,";
                sqlText += "@VATRate3,";
                sqlText += "@UOM2,";
                sqlText += "@UOMConvertion,";

                sqlText += "@IsArchive,";
                sqlText += "@GenericName,";
                sqlText += "@DARNo,";
                sqlText += "@ReportType,";
                sqlText += "@TransactionHoldDate,";
                sqlText += "@IsFixedVATRebate,";
                sqlText += "@IsConfirmed,";
                sqlText += "@IsSample,";
                sqlText += "@IsChild,";
                sqlText += "@MasterProductItemNo,";
                sqlText += "@Option1,";
                sqlText += "@Volume,";
                sqlText += "@VolumeUnit,";
                sqlText += "@PackSize,";
                sqlText += "@IsPackCal,";
                sqlText += "@TollOpeningQuantity";
                ////sqlText += "@WastageTotalQuantity,";
                ////sqlText += "@WastageTotalValue";

                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@productCode", productCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ProductName", vm.ProductName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ProductDescription", vm.ProductDescription ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CategoryID", vm.CategoryID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOM", vm.UOM ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CostPrice", vm.CostPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NBRPrice", vm.NBRPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TollCharge", vm.TollCharge);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OpeningBalance", vm.OpeningBalance);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@HSCodeNo", vm.HSCodeNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate", vm.VATRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SD", vm.SD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Packetprice", vm.Packetprice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Trading", vm.Trading);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TradingMarkUp", vm.TradingMarkUp);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NonStock", vm.NonStock);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RebatePercent", vm.RebatePercent);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OpeningTotalCost", vm.OpeningTotalCost);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OpeningDate", vm.OpeningDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Banderol", vm.Banderol);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CDRate", vm.CDRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RDRate", vm.RDRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TVARate", vm.TVARate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ATVRate", vm.ATVRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate2", vm.VATRate2);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TradingSaleVATRate", vm.TradingSaleVATRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TradingSaleSD", vm.TradingSaleSD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VDSRate", vm.VDSRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsExempted", vm.IsExempted);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsZeroVAT", vm.IsZeroVAT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsTransport", vm.IsTransport);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TollProduct", vm.TollProduct);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TDSCode", vm.TDSCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FixedVATAmount", vm.FixedVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedVAT", vm.IsFixedVAT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedSD", vm.IsFixedSD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedCD", vm.IsFixedCD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedRD", vm.IsFixedRD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedAIT", vm.IsFixedAIT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedVAT1", vm.IsFixedVAT1);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedAT", vm.IsFixedAT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedOtherSD", vm.IsFixedOtherSD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsHouseRent", vm.IsHouseRent);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ShortName", vm.ShortName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsVDS", vm.IsVDS);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@HPSRate", vm.HPSRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AITRate", vm.AITRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SDRate", vm.SDRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate3", vm.VATRate3);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOM2", vm.UOM2);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMConvertion", vm.UOMConversion);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsArchive", false);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@GenericName", vm.GenericName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DARNo", vm.DARNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReportType", vm.ReportType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionHoldDate", vm.TransactionHoldDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsFixedVATRebate", vm.IsFixedVATRebate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsConfirmed", vm.IsConfirmed);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsSample", vm.IsSample);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsChild", vm.IsChild);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterProductItemNo", vm.MasterProductItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Option1", vm.Option1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Volume", vm.Volume);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VolumeUnit", vm.VolumeUnit ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PackSize", vm.PackSize ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsPackCal", vm.IsPackCal ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TollOpeningQuantity", vm.TollOpeningQuantity);

                ////cmdInsert.Parameters.AddWithValueAndNullHandle("@WastageTotalQuantity", vm.WastageTotalQuantity);
                ////cmdInsert.Parameters.AddWithValueAndNullHandle("@WastageTotalValue", vm.WastageTotalValue);

                transResult = cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    ProductNameVM pvm = new ProductNameVM();
                    pvm.ItemNo = nextId.ToString();
                    pvm.ProductName = vm.ProductName;
                    retResults = InsertToProductNames(pvm, currConn, transaction, connVM);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", "SQL:" + sqlText);//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                    }

                    PurchaseDAL purchaseDal = new PurchaseDAL();

                    purchaseDal.UpdateProductAVGprice(vm.ItemNo, "", 0, 0, currConn, transaction, 0, 0, "", "", connVM);

                }
                #endregion Product Name and Category Id not exist,Insert new Product

                #region Trackings
                if (Trackings != null && Trackings.Count() > 0)
                {
                    Trackings[0].ItemNo = nextId;

                    string trackingUpdate = string.Empty;
                    TrackingDAL trackingDal = new TrackingDAL();
                    trackingUpdate = trackingDal.TrackingInsert(Trackings, transaction, currConn, connVM);

                    if (trackingUpdate == "Fail")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                    }
                }
                #endregion
                #region Commit
                if (VcurrConn == null)
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

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Requested Item Information successfully Added";
                retResults[2] = vm.ItemNo;
                retResults[3] = "" + productCode;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                if (Vtransaction == null)
                {
                    transaction.Rollback();

                }

                FileLogger.Log("ProductDAL", "InsertToProduct", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                throw ex;
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public void ProductMappingCodeCheck(string productCode, string ItemNo = "0", bool IsUpdate = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int countId = 0;
            string sqlText = "";

            #endregion Initializ

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

                sqlText = "select count(ItemNo) from ProductMapDetails where  ProductMappingCode=@ProductCode";
                if (IsUpdate)
                {
                    sqlText += @" and ItemNo!=@ItemNo";
                }
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValue("@ProductCode", productCode);
                if (IsUpdate)
                {
                    cmd.Parameters.AddWithValue("@ItemNo", ItemNo);
                }

                countId = (int)cmd.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("ProductMappingCodeCheck", "Same Code('" + productCode + "') already exist as Product Mapping");
                }

            }

            #endregion Try

            #region Catch and Finall

            #region Catch

            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    transaction.Rollback();

                }

                FileLogger.Log("ProductDAL", "ProductMappingCodeCheck", ex.ToString() + "\n" + sqlText);

                throw ex;
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
            #endregion Catch and Finall

        }

        public string[] UpdateProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.ItemNo;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string productCode = vm.ProductCode;
            string itemType = ItemType;
            bool Auto = false;
            int nextId = 0;
            #endregion Initializ

            #region Try

            try
            {
                #region Validation

                if (vm.ItemNo.Trim() == "0")
                {
                    retResults[0] = "Fail";
                    retResults[1] = "This product information unable to update!";
                    retResults[2] = vm.ItemNo;
                    retResults[3] = vm.ProductCode;

                    return retResults;
                }

                SetDefaultValue(vm);


                //////if (string.IsNullOrEmpty(vm.ItemNo))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter product no.");
                //////}
                //////if (string.IsNullOrEmpty(vm.ProductName))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter product name.");
                //////}
                //////if (string.IsNullOrEmpty(vm.CategoryID))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter product category.");
                //////}
                //////if (string.IsNullOrEmpty(vm.UOM))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter Product UOM.");
                //////}

                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                if (itemType == "Overhead")
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead") == "Y" ? true : false);


                }
                else
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item") == "Y" ? true : false);

                }
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                transaction = currConn.BeginTransaction("UpdateItem");


                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/
                #region Report Type

                ProductCategoryDAL productcategorydal = new ProductCategoryDAL();
                ProductCategoryVM cVm = new ProductCategoryVM();

                cVm = productcategorydal.SelectAllList(Convert.ToInt32(vm.CategoryID), null, null, currConn, transaction, connVM).FirstOrDefault();

                vm.ReportType = cVm.ReportType;

                #endregion

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where  ItemNo =@ItemNo";
                SqlCommand cmdCIdExist = new SqlCommand(sqlText, currConn);
                cmdCIdExist.Transaction = transaction;
                cmdCIdExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                countId = (int)cmdCIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateItem",
                                                    "Unable to find requested product no");
                }
                #endregion ProductExist

                #region Product Exist or not
                /*Checking existance of provided bank Id information*/
                //if (!string.IsNullOrEmpty(ProductName))
                //{


                //    sqlText = "select count(ItemNo) from Products where  ProductName='" + ProductName +
                //              "' and CategoryID='" + CategoryID + "' and ItemNo <>'" + ItemNo + "'";
                //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //    cmdIdExist.Transaction = transaction;
                //    countId = (int)cmdIdExist.ExecuteScalar();
                //    if (countId > 0)
                //    {
                //        throw new ArgumentNullException("UpdateItem",
                //                                        "Same Item('" + ProductName + "' ) under same category already exist");
                //    }
                //}

                #endregion ProductExist

                #region Code
                ////if (Auto == false)
                ////{
                ////    if (string.IsNullOrEmpty(productCode))
                ////    {
                ////        throw new ArgumentNullException("UpdateItem", "Code generation is Manual, but Code not Issued");
                ////    }
                ////    else
                ////    {
                ////        sqlText = "select count(ItemNo) from Products where  ProductCode=@productCode and ItemNo <>@ItemNo ";
                ////        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                ////        cmdCodeExist.Transaction = transaction;
                ////        cmdCodeExist.Parameters.AddWithValue("@productCode", productCode);
                ////        cmdCodeExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                ////        countId = (int)cmdCodeExist.ExecuteScalar();
                ////        if (countId > 0)
                ////        {
                ////            throw new ArgumentNullException("UpdateItem", "Same Code('" + productCode + "') already exist");
                ////        }
                ////    }
                ////}
                ////else
                ////{
                ////    productCode = vm.ItemNo;
                ////}

                #endregion Code

                #region Update

                //vm.IsFixedSD = vm.IsFixedSDChecked ? "Y" : "N";
                //vm.IsFixedCD = vm.IsFixedCDChecked ? "Y" : "N";
                //vm.IsFixedRD = vm.IsFixedRDChecked ? "Y" : "N";
                //vm.IsFixedAIT = vm.IsFixedAITChecked ? "Y" : "N";
                //vm.IsFixedVAT1 = vm.IsFixedVAT1Checked ? "Y" : "N";
                //vm.IsFixedAT = vm.IsFixedATChecked ? "Y" : "N";


                sqlText = "";
                sqlText = "update Products set";

                sqlText += " ProductName        =@ProductName,";
                sqlText += " ProductDescription =@ProductDescription,";
                sqlText += " CategoryID         =@CategoryID,";
                sqlText += " UOM                =@UOM,";
                sqlText += " CostPrice          =@CostPrice,";
                sqlText += " OpeningBalance     =@OpeningBalance,";
                sqlText += " OpeningDate        =@OpeningDate,";
                sqlText += " SerialNo           =@SerialNo,";
                sqlText += " HSCodeNo           =@HSCodeNo,";
                sqlText += " VATRate            =@VATRate,";
                sqlText += " Comments           =@Comments,";
                sqlText += " ActiveStatus       =@ActiveStatus,";
                sqlText += " LastModifiedBy     =@LastModifiedBy,";
                sqlText += " LastModifiedOn     =@LastModifiedOn,";
                sqlText += " SD                 =@SD,";
                sqlText += " PacketPrice        =@Packetprice,";
                sqlText += " NBRPrice           =@NBRPrice,";
                sqlText += " receiveprice       =@NBRPrice,";
                sqlText += " Trading            =@Trading,";
                sqlText += " TradingMarkUp      =@TradingMarkUp,";
                sqlText += " NonStock           =@NonStock,";
                sqlText += " TollCharge         =@TollCharge,";
                sqlText += " RebatePercent      =@RebatePercent,";
                sqlText += " OpeningTotalCost   =@OpeningTotalCost,";
                ////sqlText += " ProductCode        =@productCode,";
                sqlText += " TollProduct        =@TollProduct,";
                sqlText += "CDRate              =@CDRate,";
                sqlText += "RDRate              =@RDRate,";
                sqlText += "TVARate             =@TVARate,";
                sqlText += "ATVRate             =@ATVRate,";
                sqlText += "VATRate2            =@VATRate2,";
                sqlText += "TradingSaleVATRate  =@TradingSaleVATRate,";
                sqlText += "TradingSaleSD       =@TradingSaleSD,";
                sqlText += "VDSRate             =@VDSRate,";
                sqlText += "AITRate             =@AITRate,";
                sqlText += "SDRate              =@SDRate,";
                sqlText += "VATRate3            =@VATRate3,";
                sqlText += "IsExempted          =@IsExempted,";
                sqlText += "IsZeroVAT           =@IsZeroVAT,";
                sqlText += "IsTransport         =@IsTransport,";
                sqlText += "TDSCode             =@TDSCode,";
                sqlText += "IsFixedVAT          =@IsFixedVAT,";
                sqlText += "IsFixedSD           =@IsFixedSD,";
                sqlText += "IsFixedCD           =@IsFixedCD,";
                sqlText += "IsFixedRD           =@IsFixedRD,";
                sqlText += "IsFixedAIT          =@IsFixedAIT,";
                sqlText += "IsFixedVAT1         =@IsFixedVAT1,";
                sqlText += "IsFixedAT           =@IsFixedAT,";
                sqlText += "IsFixedOtherSD      =@IsFixedOtherSD,";
                sqlText += "IsHouseRent      =@IsHouseRent,";
                sqlText += "ShortName      =@ShortName,";
                sqlText += "IsVDS               =@IsVDS,";
                sqlText += "HPSRate             =@HPSRate,";
                sqlText += "FixedVATAmount      =@FixedVATAmount,";
                sqlText += "UOM2                =@UOM2,";
                sqlText += "UOMConvertion       =@UOMConvertion,";
                sqlText += "GenericName       =@GenericName,";
                sqlText += "DARNo             =@DARNo,";
                sqlText += " Banderol           =@Banderol,";
                sqlText += " ReportType           =@ReportType,";
                sqlText += " TransactionHoldDate   =@TransactionHoldDate,";
                sqlText += " IsFixedVATRebate   =@IsFixedVATRebate,";
                sqlText += " IsConfirmed           =@IsConfirmed,";
                sqlText += " Option1           =@Option1,";
                sqlText += " Volume           =@Volume,";
                sqlText += " VolumeUnit           =@VolumeUnit,";
                sqlText += " PackSize           =@PackSize,";
                sqlText += " IsPackCal           =@IsPackCal,";
                sqlText += " TollOpeningQuantity  =@TollOpeningQuantity,";

                sqlText += " IsSample           =@IsSample";

                ////sqlText += " WastageTotalQuantity        =@WastageTotalQuantity,";
                ////sqlText += " WastageTotalValue           =@WastageTotalValue";

                sqlText += " where ItemNo       =@ItemNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@ProductName", vm.ProductName);
                cmdUpdate.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CategoryID", vm.CategoryID);
                cmdUpdate.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                cmdUpdate.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                cmdUpdate.Parameters.AddWithValue("@OpeningDate", vm.OpeningDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VATRate", vm.VATRate);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@SD", vm.SD);
                cmdUpdate.Parameters.AddWithValue("@Packetprice", vm.Packetprice);
                cmdUpdate.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                //////cmdUpdate.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                cmdUpdate.Parameters.AddWithValue("@Trading", vm.Trading ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TradingMarkUp", vm.TradingMarkUp);
                cmdUpdate.Parameters.AddWithValue("@NonStock", vm.NonStock ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TollCharge", vm.TollCharge);
                cmdUpdate.Parameters.AddWithValue("@RebatePercent", vm.RebatePercent);
                cmdUpdate.Parameters.AddWithValue("@OpeningTotalCost", vm.OpeningTotalCost);
                //////cmdUpdate.Parameters.AddWithValue("@productCode", productCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TollProduct", vm.TollProduct ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Banderol", vm.Banderol ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CDRate", vm.CDRate);
                cmdUpdate.Parameters.AddWithValue("@RDRate", vm.RDRate);
                cmdUpdate.Parameters.AddWithValue("@TVARate", vm.TVARate);
                cmdUpdate.Parameters.AddWithValue("@ATVRate", vm.ATVRate);
                cmdUpdate.Parameters.AddWithValue("@VATRate2", vm.VATRate2);
                cmdUpdate.Parameters.AddWithValue("@TradingSaleVATRate", vm.TradingSaleVATRate);
                cmdUpdate.Parameters.AddWithValue("@TradingSaleSD", vm.TradingSaleSD);
                cmdUpdate.Parameters.AddWithValue("@VDSRate", vm.VDSRate);
                cmdUpdate.Parameters.AddWithValue("@AITRate", vm.AITRate);
                cmdUpdate.Parameters.AddWithValue("@SDRate", vm.SDRate);
                cmdUpdate.Parameters.AddWithValue("@VATRate3", vm.VATRate3);
                cmdUpdate.Parameters.AddWithValue("@IsExempted", vm.IsExempted ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsZeroVAT", vm.IsZeroVAT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsTransport", vm.IsTransport ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TDSCode", vm.TDSCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedSD ", vm.IsFixedSD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedCD ", vm.IsFixedCD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedRD ", vm.IsFixedRD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedAIT", vm.IsFixedAIT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedVAT1", vm.IsFixedVAT1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedAT ", vm.IsFixedAT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedOtherSD ", vm.IsFixedOtherSD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsHouseRent ", vm.IsHouseRent ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ShortName ", vm.ShortName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsVDS ", vm.IsVDS ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@HPSRate ", vm.HPSRate);
                cmdUpdate.Parameters.AddWithValue("@UOM2 ", vm.UOM2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@UOMConvertion ", vm.UOMConversion);
                cmdUpdate.Parameters.AddWithValue("@FixedVATAmount", vm.FixedVATAmount);
                cmdUpdate.Parameters.AddWithValue("@IsConfirmed", vm.IsConfirmed);
                cmdUpdate.Parameters.AddWithValue("@IsSample", vm.IsSample);
                cmdUpdate.Parameters.AddWithValue("@ItemNo", vm.ItemNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@GenericName", vm.GenericName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@DARNo", vm.DARNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ReportType", vm.ReportType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TransactionHoldDate", vm.TransactionHoldDate);
                cmdUpdate.Parameters.AddWithValue("@Option1", vm.Option1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Volume", vm.Volume);
                cmdUpdate.Parameters.AddWithValue("@VolumeUnit", vm.VolumeUnit ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@PackSize", vm.PackSize ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsPackCal", vm.IsPackCal ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TollOpeningQuantity", vm.TollOpeningQuantity);

                cmdUpdate.Parameters.AddWithValue("@IsFixedVATRebate", vm.IsFixedVATRebate ?? Convert.DBNull);

                ////cmdUpdate.Parameters.AddWithValue("@WastageTotalQuantity", vm.WastageTotalQuantity);
                ////cmdUpdate.Parameters.AddWithValue("@WastageTotalValue", vm.WastageTotalValue);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update

                #region Tracking

                if (Trackings.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdateImportDyties);
                }

                foreach (var tracking in Trackings.ToList())
                {

                    #region Find Heading1 Existence

                    sqlText = "";
                    sqlText += "select COUNT(Heading1) from Trackings WHERE Heading1 = @Heading1";
                    sqlText += " AND ItemNo=@ItemNo";

                    SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, currConn);
                    cmdFindHeading1.Transaction = transaction;

                    SqlParameter parameter = new SqlParameter("@Heading1", SqlDbType.VarChar, 200);
                    parameter.Value = tracking.Heading1;
                    cmdFindHeading1.Parameters.Add(parameter);

                    parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = tracking.ItemNo;
                    cmdFindHeading1.Parameters.Add(parameter);


                    decimal IDExist = (int)cmdFindHeading1.ExecuteScalar();
                    if (IDExist <= 0)
                    {
                        #region Check Heading2

                        sqlText = "";
                        sqlText += "select COUNT(Heading2) from Trackings WHERE Heading2 = @Heading2";
                        sqlText += " AND ItemNo=@ItemNo";

                        SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, currConn);
                        cmdFindHeading2.Transaction = transaction;

                        parameter = new SqlParameter("@Heading2", SqlDbType.VarChar, 200);
                        parameter.Value = tracking.Heading2;
                        cmdFindHeading2.Parameters.Add(parameter);

                        parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                        parameter.Value = tracking.ItemNo;
                        cmdFindHeading2.Parameters.Add(parameter);

                        decimal IDExist2 = (int)cmdFindHeading2.ExecuteScalar();
                        #endregion
                        if (IDExist2 <= 0)
                        {
                            // Insert
                            #region Insert
                            sqlText = "";
                            sqlText += " insert into Trackings";
                            sqlText += " (";

                            sqlText += " PurchaseInvoiceNo,";
                            sqlText += " ItemNo,";
                            sqlText += " TrackLineNo,";
                            sqlText += " Heading1,";
                            sqlText += " Heading2,";
                            sqlText += " Quantity,";
                            sqlText += " UnitPrice,";

                            sqlText += " IsPurchase,";
                            sqlText += " IsIssue,";
                            sqlText += " IsReceive,";
                            sqlText += " IsSale,";
                            sqlText += " Post,";
                            sqlText += " ReceivePost,";
                            sqlText += " SalePost,";
                            sqlText += " IssuePost,";


                            sqlText += " CreatedBy,";
                            sqlText += " CreatedOn,";
                            sqlText += " LastModifiedBy,";
                            sqlText += " LastModifiedOn";

                            sqlText += " )";
                            sqlText += " values";
                            sqlText += " (";

                            sqlText += " '0',";
                            sqlText += "@trackingItemNo,";
                            sqlText += "@trackingTrackingLineNo,";
                            sqlText += "@trackingHeading1,";
                            sqlText += "@trackingHeading2,";
                            sqlText += "@trackingQuantity,";
                            sqlText += "@trackingUnitPrice,";
                            sqlText += "@trackingIsPurchase,";
                            sqlText += "@trackingIsIssue,";
                            sqlText += "@trackingIsReceive,";
                            sqlText += "@trackingIsSale,";
                            sqlText += "'Y',";
                            sqlText += "'N',";
                            sqlText += "'N',";
                            sqlText += "'N',";


                            sqlText += "@LastModifiedBy,";
                            sqlText += "@LastModifiedOn,";
                            sqlText += "@LastModifiedBy,";
                            sqlText += "@LastModifiedOn";

                            sqlText += ")";


                            SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, currConn);
                            cmdInsertTrackings.Transaction = transaction;
                            cmdInsertTrackings.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                            cmdInsertTrackings.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);

                            cmdInsertTrackings.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingTrackingLineNo", tracking.TrackingLineNo ?? Convert.DBNull);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2 ?? Convert.DBNull);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsPurchase", tracking.IsPurchase);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsIssue", tracking.IsIssue ?? Convert.DBNull);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsReceive", tracking.IsReceive ?? Convert.DBNull);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsSale", tracking.IsSale ?? Convert.DBNull);


                            transResult = (int)cmdInsertTrackings.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgSaveNotSuccessfully);
                            }


                            #endregion
                        }
                        else
                        {
                            //Update
                            #region Update
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " TrackLineNo=@trackingTrackingLineNo,";
                            sqlText += " Heading1   =@trackingHeading1,";
                            sqlText += " Heading2   =@trackingHeading2,";
                            sqlText += " Quantity   =@trackingQuantity,";
                            sqlText += " UnitPrice  =@trackingUnitPrice,";

                            sqlText += " Post= 'Y',";

                            sqlText += " LastModifiedBy = @LastModifiedBy,";
                            sqlText += " LastModifiedOn = @LastModifiedOn";

                            sqlText += " where ItemNo =@trackingItemNo ";
                            sqlText += " and Heading2 =@trackingHeading2 ";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;

                            cmdInsDetail.Parameters.AddWithValue("@trackingTrackingLineNo ", tracking.TrackingLineNo);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2 ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                            cmdInsDetail.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                            cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);
                            cmdInsDetail.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        //Update
                        #region Update
                        sqlText = "";
                        sqlText += " update Trackings set ";
                        sqlText += " TrackLineNo= @trackingTrackingLineNo,";
                        sqlText += " Heading1   = @trackingHeading1,";
                        sqlText += " Heading2   = @trackingHeading2,";
                        sqlText += " Quantity   = @trackingQuantity,";
                        sqlText += " UnitPrice  = @trackingUnitPrice,";
                        sqlText += " Post       = 'Y',";

                        sqlText += " LastModifiedBy = @LastModifiedBy,";
                        sqlText += " LastModifiedOn = @LastModifiedOn";


                        sqlText += " where ItemNo =@trackingItemNo ";
                        sqlText += " and Heading1 =@trackingHeading1 ";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@trackingTrackingLineNo ", tracking.TrackingLineNo);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2 ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                        cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }
                        #endregion
                    }


                    #endregion Find Heading1 Existence
                }

                #endregion Tracking

                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Requested information successfully updated";
                retResults[2] = vm.ItemNo;//vm.Id;
                retResults[3] = productCode;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("ProductDAL", "UpdateProduct", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

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
            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public string[] UpdateProductCode(ProductVM vm, List<TrackingVM> Trackings, string ItemType, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.ItemNo;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string productCode = vm.ProductCode;
            string itemType = ItemType;
            bool Auto = false;
            int nextId = 0;
            #endregion Initializ

            #region Try

            try
            {

                #region settingsValue

                CommonDAL commonDal = new CommonDAL();
                if (itemType == "Overhead")
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead") == "Y" ? true : false);


                }
                else
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item") == "Y" ? true : false);

                }
                #endregion settingsValue

                #region Validation

                if (string.IsNullOrEmpty(vm.ItemNo))
                {
                    throw new ArgumentNullException("UpdateItem", "Please enter product no.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                transaction = currConn.BeginTransaction("UpdateItem");


                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where  ItemNo =@ItemNo";
                SqlCommand cmdCIdExist = new SqlCommand(sqlText, currConn);
                cmdCIdExist.Transaction = transaction;
                cmdCIdExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                countId = (int)cmdCIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateItem", "Unable to find requested product no");
                }
                #endregion ProductExist

                #region Code
                //if (Auto == false)
                if (Auto == false || Auto == true)
                {
                    if (string.IsNullOrEmpty(productCode))
                    {
                        throw new ArgumentNullException("UpdateItem", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(ItemNo) from Products where  ProductCode=@productCode and ItemNo <>@ItemNo ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@productCode", productCode);
                        cmdCodeExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("UpdateItem", "Same Code('" + productCode + "') already exist");
                        }
                    }
                }
                else
                {
                    productCode = vm.ItemNo;
                }

                #endregion Code

                #region Mapping Code Check

                bool MappingCodeCheck = Convert.ToBoolean(commonDal.settingValue("Products", "IsMappingCodeCheck", connVM, currConn, transaction) == "Y");

                if (MappingCodeCheck)
                {
                    ProductMappingCodeCheck(productCode, vm.ItemNo, true, currConn, transaction, connVM);
                }

                #endregion

                #region Update

                sqlText = "";
                sqlText = "update Products set";
                sqlText += " ProductCode        =@ProductCode";
                sqlText += " where ItemNo       =@ItemNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@ProductCode", vm.ProductCode);
                cmdUpdate.Parameters.AddWithValue("@ItemNo", vm.ItemNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Requested information successfully updated";
                retResults[2] = vm.ItemNo;//vm.Id;
                retResults[3] = productCode;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = vm.ItemNo.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log("ProductDAL", "UpdateProductCode", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

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
            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public string[] Delete(ProductVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVehicle"; //Method Name
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
                        sqlText = "update Products set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where ItemNo=@ItemNo";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@ItemNo", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Product Delete", vm.ItemNo + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Product Information Delete", "Could not found any item.");
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
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        public List<ProductVM> SelectAll(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null, DataTable dtPCodes = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string count = "100";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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

                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }


                sqlText += @"

Pr.ItemNo
,Pr.ProductCode
,Pr.ProductName
,Pr.ProductDescription
,Pr.CategoryID
,Pr.UOM
,isnull(Pr.CostPrice,0) CostPrice
,isnull(Pr.SalesPrice,0) SalesPrice
,isnull(Pr.NBRPrice,0) NBRPrice
,isnull(Pr.ReceivePrice,0) ReceivePrice
,isnull(Pr.IssuePrice,0) IssuePrice
,isnull(Pr.TenderPrice,0) TenderPrice
,isnull(Pr.ExportPrice,0) ExportPrice
,isnull(Pr.InternalIssuePrice,0) InternalIssuePrice
,isnull(Pr.TollIssuePrice,0) TollIssuePrice
,isnull(Pr.TollCharge,0) TollCharge
,isnull(isnull(Pr.OpeningBalance,0)+isnull(Pr.QuantityInHand,0),0) Stock
,Pr.OpeningBalance
,Pr.SerialNo
,Pr.HSCodeNo
,isnull(Pr.VATRate,0) VATRate
,Pr.Comments
,isnull(Pr.SD,0) SD
,isnull(Pr.PacketPrice,0) PacketPrice
,Pr.Trading
,isnull(Pr.TradingMarkUp,0) TradingMarkUp
,Pr.NonStock
,isnull(Pr.QuantityInHand,0) QuantityInHand
,Pr.OpeningDate
,isnull(Pr.RebatePercent,0) RebatePercent
,isnull(Pr.TVBRate,0) TVBRate
,isnull(Pr.CnFRate,0) CnFRate
,isnull(Pr.InsuranceRate,0) InsuranceRate
,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.AITRate,0) AITRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.UOM2,Pr.UOM) UOM2
,isnull(Pr.UOMConvertion,1) UOMConvertion
,Pr.ActiveStatus
,Pr.CreatedBy
,Pr.CreatedOn
,Pr.LastModifiedBy
,Pr.LastModifiedOn
,isnull(Pr.OpeningTotalCost,0) OpeningTotalCost

,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.VATRate2,0) VATRate2
,isnull(Pr.TradingSaleVATRate,0) TradingSaleVATRate
,isnull(Pr.TradingSaleSD,0) TradingSaleSD
,isnull(Pr.VDSRate,0) VDSRate
,isnull(Pr.IsFixedVAT,'N') IsFixedVAT
,isnull(Pr.IsFixedSD,'N') IsFixedSD 
,isnull(Pr.IsFixedCD,'N') IsFixedCD 
,isnull(Pr.IsFixedRD,'N') IsFixedRD 
,isnull(Pr.IsFixedAIT,'N') IsFixedAIT
,isnull(Pr.IsFixedVAT1,'N') IsFixedVAT1
,isnull(Pr.IsFixedAT,'N') IsFixedAT 
,isnull(Pr.IsFixedOtherSD,'N') IsFixedOtherSD 
,isnull(Pr.FixedVATAmount,0) FixedVATAmount
,isnull(Pr.IsHouseRent,'N') IsHouseRent
,isnull(Pr.ShortName,'-') ShortName
,isnull(Pr.Banderol,'N')Banderol
,isnull(Pr.TollProduct,'N')TollProduct
,Pc.CategoryName
,Pc.IsRaw
,isnull(Pr.IsExempted,'N')IsExempted
,isnull(Pr.IsZeroVAT,'N')IsZeroVAT
,isnull(Pr.IsTransport,'N')IsTransport
,isnull(Pr.BranchId,'0')BranchId
,isnull(Pr.TDSCode,'-')TDSCode
,isnull(Pr.AITRate,'0')AITRate
,isnull(Pr.SDRate,'0')SDRate
,isnull(Pr.VATRate3,'0')VATRate3
,isnull(pr.IsVDS,'Y')IsVDS
,isnull(pr.HPSRate,'0')HPSRate
,isnull(pr.GenericName,'-')GenericName
,isnull(pr.DARNo,'-')DARNo
,isnull(pr.IsConfirmed,'Y')IsConfirmed
,isnull(pr.IsSample,'N')IsSample
,isnull(pr.ReportType,'-')ReportType
,isnull(pr.TransactionHoldDate,'0')TransactionHoldDate
,isnull(pr.IsFixedVATRebate,'N')IsFixedVATRebate
,isnull(pr.Option1,'-')Option1
,isnull(pr.Volume,1)Volume
,isnull(pr.VolumeUnit,'-')VolumeUnit
,isnull(pr.PackSize,'-')PackSize
,isnull(pr.TollOpeningQuantity,0)TollOpeningQuantity
,isnull(pr.IsPackCal,'N')IsPackCal
,isnull(pr.FixedVATAmountP,0)FixedVATAmountP
--,isnull(Pr.WastageTotalQuantity,0) WastageTotalQuantity
--,isnull(Pr.WastageTotalValue,0) WastageTotalValue


FROM Products Pr left outer join ProductCategories Pc on Pr.CategoryID=Pc.CategoryID
WHERE  1=1 AND Pr.IsArchive = 0
";


                if (ItemNo != "0" && !string.IsNullOrEmpty(ItemNo))
                {
                    sqlText += @" and Pr.ItemNo=@ItemNo";
                }


                //, DataTable dtPCodes = null
                if (dtPCodes != null && dtPCodes.Rows.Count > 0)
                {
                    sqlText += @" and pr.ItemNo in (";

                    foreach (DataRow row in dtPCodes.Rows)
                    {
                        sqlText += "'" + row["Code"].ToString() + "',";
                    }

                    sqlText = sqlText.TrimEnd(',');
                    sqlText += @")";

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

                        else if (conditionFields[i].ToLower().Contains("isnull"))
                        {
                            sqlText += " AND " + "isnull(" + cField + ",'Y')=" + " @" + cField + "";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else if (conditionFields[i].ToLower().Contains("!="))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.ProductName))
                    {
                        sqlText += " AND Pr.ProductName like @ProductName ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.ProductCode))
                    {
                        sqlText += " AND Pr.ProductCode like @ProductCode ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.HSCodeNo))
                    {
                        sqlText += " AND Pr.HSCodeNo like @HSCodeNo ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        sqlText += " AND Pr.SerialNo like @SerialNo ";
                    }
                }
                #endregion SqlText

                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
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
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.ProductName))
                    {
                        objComm.Parameters.AddWithValue("@ProductName", "%" + likeVM.ProductName + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.ProductCode))
                    {
                        objComm.Parameters.AddWithValue("@ProductCode", "%" + likeVM.ProductCode + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.HSCodeNo))
                    {
                        objComm.Parameters.AddWithValue("@HSCodeNo", "%" + likeVM.HSCodeNo + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        objComm.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
                    }
                }
                if (ItemNo != "0" && !string.IsNullOrEmpty(ItemNo))
                {
                    objComm.Parameters.AddWithValue("@ItemNo", ItemNo.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();

                    var FixedSD = dr["IsFixedSD"].ToString();
                    var FixedCD = dr["IsFixedCD"].ToString();
                    var FixedRD = dr["IsFixedRD"].ToString();
                    var FixedAIT = dr["IsFixedAIT"].ToString();
                    var FixedVAT1 = dr["IsFixedVAT1"].ToString();
                    var FixedAT = dr["IsFixedAT"].ToString();

                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.UOM2 = dr["UOM2"].ToString();
                    vm.UOMConversion = Convert.ToDecimal(dr["UOMConvertion"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.ReceivePrice = Convert.ToDecimal(dr["ReceivePrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.Trading = dr["Trading"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.OpeningDate = dr["OpeningDate"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                    vm.Banderol = dr["Banderol"].ToString();

                    vm.CDRate = Convert.ToDecimal(dr["CDRate"].ToString());
                    vm.RDRate = Convert.ToDecimal(dr["RDRate"].ToString());
                    vm.TVARate = Convert.ToDecimal(dr["TVARate"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString());
                    vm.VATRate2 = Convert.ToDecimal(dr["VATRate2"].ToString());
                    vm.TradingSaleVATRate = Convert.ToDecimal(dr["TradingSaleVATRate"].ToString());
                    vm.TradingSaleSD = Convert.ToDecimal(dr["TradingSaleSD"].ToString());

                    vm.VDSRate = Convert.ToDecimal(dr["VDSRate"].ToString());
                    vm.CnFRate = Convert.ToDecimal(dr["CnFRate"].ToString());

                    vm.AITRate = Convert.ToDecimal(dr["AITRate"].ToString());
                    vm.VATRate3 = Convert.ToDecimal(dr["VATRate3"].ToString());
                    vm.SDRate = Convert.ToDecimal(dr["SDRate"].ToString());
                    vm.Volume = Convert.ToDecimal(dr["Volume"].ToString());


                    vm.TollProduct = dr["TollProduct"].ToString();
                    vm.Stock = Convert.ToDecimal(dr["Stock"].ToString());
                    vm.ProductType = dr["IsRaw"].ToString();
                    vm.IsRaw = dr["IsRaw"].ToString();

                    vm.IsZeroVAT = dr["IsZeroVAT"].ToString();
                    vm.IsExempted = dr["IsExempted"].ToString();
                    vm.IsTransport = dr["IsTransport"].ToString();
                    vm.TDSCode = dr["TDSCode"].ToString();

                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                    vm.IsFixedSD = dr["IsFixedSD"].ToString();
                    vm.IsFixedCD = dr["IsFixedCD"].ToString();
                    vm.IsFixedRD = dr["IsFixedRD"].ToString();
                    vm.IsFixedAIT = dr["IsFixedAIT"].ToString();
                    vm.IsFixedVAT1 = dr["IsFixedVAT1"].ToString();
                    vm.IsFixedAT = dr["IsFixedAT"].ToString();
                    vm.IsFixedOtherSD = dr["IsFixedOtherSD"].ToString();
                    vm.IsHouseRent = dr["IsHouseRent"].ToString();
                    vm.ShortName = dr["ShortName"].ToString();
                    vm.IsConfirmed = dr["IsConfirmed"].ToString();
                    vm.IsSample = dr["IsSample"].ToString();
                    vm.IsVDS = dr["IsVDS"].ToString();
                    vm.HPSRate = Convert.ToDecimal(dr["HPSRate"]);
                    vm.TransactionHoldDate = Convert.ToDecimal(dr["TransactionHoldDate"]);
                    vm.IsFixedVATRebate = dr["IsFixedVATRebate"].ToString();
                    vm.Packetprice = Convert.ToDecimal(dr["Packetprice"]);

                    ////vm.WastageTotalQuantity = Convert.ToDecimal(dr["WastageTotalQuantity"]);
                    ////vm.WastageTotalValue = Convert.ToDecimal(dr["WastageTotalValue"]);

                    vm.IsFixedSDChecked = FixedSD == "Y";
                    vm.IsFixedCDChecked = FixedCD == "Y";
                    vm.IsFixedRDChecked = FixedRD == "Y";
                    vm.IsFixedAITChecked = FixedAIT == "Y";
                    vm.IsFixedVAT1Checked = FixedVAT1 == "Y";
                    vm.IsFixedATChecked = FixedAT == "Y";

                    vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());
                    vm.FixedVATAmountP = Convert.ToDecimal(dr["FixedVATAmountP"].ToString());

                    vm.GenericName = dr["GenericName"].ToString();
                    vm.DARNo = dr["DARNo"].ToString();
                    vm.ReportType = dr["ReportType"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Option1 = dr["Option1"].ToString();

                    vm.Volume = Convert.ToDecimal(dr["Volume"].ToString());
                    vm.VolumeUnit = dr["VolumeUnit"].ToString();
                    vm.PackSize = dr["PackSize"].ToString();
                    vm.IsPackCal = dr["IsPackCal"].ToString();
                    vm.TollOpeningQuantity = Convert.ToDecimal(dr["TollOpeningQuantity"].ToString());

                    VMs.Add(vm);
                }
                dr.Close();

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
                FileLogger.Log("ProductDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public List<ProductVM> SelectAllPaging(string ItemNo = "0", string[] conditionFields = null,
            string[] conditionValues = null, JQueryDataTableParamModel jqueryParam = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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

                sqlText += @"
SELECT
Pr.ItemNo
,Pr.ProductCode
,Pr.ProductName
,Pr.ProductDescription
,Pr.CategoryID
,Pr.UOM
,isnull(Pr.CostPrice,0) CostPrice
,isnull(Pr.SalesPrice,0) SalesPrice
,isnull(Pr.NBRPrice,0) NBRPrice
,isnull(Pr.ReceivePrice,0) ReceivePrice
,isnull(Pr.IssuePrice,0) IssuePrice
,isnull(Pr.TenderPrice,0) TenderPrice
,isnull(Pr.ExportPrice,0) ExportPrice
,isnull(Pr.InternalIssuePrice,0) InternalIssuePrice
,isnull(Pr.TollIssuePrice,0) TollIssuePrice
,isnull(Pr.TollCharge,0) TollCharge
,isnull(isnull(Pr.OpeningBalance,0)+isnull(Pr.QuantityInHand,0),0) Stock
,Pr.OpeningBalance
,Pr.SerialNo
,Pr.HSCodeNo
,isnull(Pr.VATRate,0) VATRate
,Pr.Comments
,isnull(Pr.SD,0) SD
,isnull(Pr.PacketPrice,0) PacketPrice
,Pr.Trading
,isnull(Pr.TradingMarkUp,0) TradingMarkUp
,Pr.NonStock
,isnull(Pr.QuantityInHand,0) QuantityInHand
,Pr.OpeningDate
,isnull(Pr.RebatePercent,0) RebatePercent
,isnull(Pr.TVBRate,0) TVBRate
,isnull(Pr.CnFRate,0) CnFRate
,isnull(Pr.InsuranceRate,0) InsuranceRate
,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.AITRate,0) AITRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.UOM2,Pr.UOM) UOM2
,isnull(Pr.UOMConvertion,1) UOMConvertion
,Pr.ActiveStatus
,Pr.CreatedBy
,Pr.CreatedOn
,Pr.LastModifiedBy
,Pr.LastModifiedOn
,isnull(Pr.OpeningTotalCost,0) OpeningTotalCost

,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.VATRate2,0) VATRate2
,isnull(Pr.TradingSaleVATRate,0) TradingSaleVATRate
,isnull(Pr.TradingSaleSD,0) TradingSaleSD
,isnull(Pr.VDSRate,0) VDSRate
,isnull(Pr.IsFixedVAT,'N') IsFixedVAT
,isnull(Pr.IsFixedSD,'N') IsFixedSD 
,isnull(Pr.IsFixedCD,'N') IsFixedCD 
,isnull(Pr.IsFixedRD,'N') IsFixedRD 
,isnull(Pr.IsFixedAIT,'N') IsFixedAIT
,isnull(Pr.IsFixedVAT1,'N') IsFixedVAT1
,isnull(Pr.IsFixedAT,'N') IsFixedAT 
,isnull(Pr.IsFixedOtherSD,'N') IsFixedOtherSD 
,isnull(Pr.FixedVATAmount,0) FixedVATAmount
,isnull(Pr.IsHouseRent,'N') IsHouseRent
,isnull(Pr.ShortName,'-') ShortName

,isnull(Pr.Banderol,'N')Banderol
,isnull(Pr.TollProduct,'N')TollProduct
,Pc.CategoryName
,Pc.IsRaw
,isnull(Pr.IsExempted,'N')IsExempted
,isnull(Pr.IsZeroVAT,'N')IsZeroVAT
,isnull(Pr.IsTransport,'N')IsTransport
,isnull(Pr.BranchId,'0')BranchId
,isnull(Pr.TDSCode,'-')TDSCode
,isnull(Pr.AITRate,'0')AITRate
,isnull(Pr.SDRate,'0')SDRate
,isnull(Pr.VATRate3,'0')VATRate3
,isnull(pr.IsVDS,'Y')IsVDS
,isnull(pr.HPSRate,'0')HPSRate
,isnull(pr.GenericName,'-')GenericName
,isnull(pr.DARNo,'-')DARNo
,isnull(pr.IsConfirmed,'Y')IsConfirmed
,isnull(pr.IsSample,'N')IsSample
,isnull(pr.ReportType,'-')ReportType
,isnull(pr.TransactionHoldDate,'0')TransactionHoldDate
,count(ItemNo) OVER () TotalCount
--,isnull(Pr.WastageTotalQuantity,0) WastageTotalQuantity
--,isnull(Pr.WastageTotalValue,0) WastageTotalValue
,isnull(Pr.IsFixedVATRebate,'N') IsFixedVATRebate


FROM Products Pr left outer join ProductCategories Pc on Pr.CategoryID=Pc.CategoryID
WHERE  1=1 AND Pr.IsArchive = 0
";


                if (ItemNo != "0")
                {
                    sqlText += @" and Pr.ItemNo=@ItemNo";
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

                        else if (conditionFields[i].ToLower().Contains("isnull"))
                        {
                            sqlText += " AND " + "isnull(" + cField + ",'Y')=" + " @" + cField + "";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else if (conditionFields[i].ToLower().Contains("!="))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }

                sqlText += " order by pr.ProductCode";

                sqlText += @" OFFSET  " + jqueryParam.iDisplayStart + @" ROWS
                FETCH NEXT " + jqueryParam.iDisplayLength + " ROWS ONLY";


                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
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
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }

                if (ItemNo != "0")
                {
                    objComm.Parameters.AddWithValue("@ItemNo", ItemNo.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();

                    var FixedSD = dr["IsFixedSD"].ToString();
                    var FixedCD = dr["IsFixedCD"].ToString();
                    var FixedRD = dr["IsFixedRD"].ToString();
                    var FixedAIT = dr["IsFixedAIT"].ToString();
                    var FixedVAT1 = dr["IsFixedVAT1"].ToString();
                    var FixedAT = dr["IsFixedAT"].ToString();

                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.UOM2 = dr["UOM2"].ToString();
                    vm.UOMConversion = Convert.ToDecimal(dr["UOMConvertion"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.Trading = dr["Trading"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.OpeningDate = dr["OpeningDate"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                    vm.Banderol = dr["Banderol"].ToString();

                    vm.CDRate = Convert.ToDecimal(dr["CDRate"].ToString());
                    vm.RDRate = Convert.ToDecimal(dr["RDRate"].ToString());
                    vm.TVARate = Convert.ToDecimal(dr["TVARate"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString());
                    vm.VATRate2 = Convert.ToDecimal(dr["VATRate2"].ToString());
                    vm.TradingSaleVATRate = Convert.ToDecimal(dr["TradingSaleVATRate"].ToString());
                    vm.TradingSaleSD = Convert.ToDecimal(dr["TradingSaleSD"].ToString());

                    vm.VDSRate = Convert.ToDecimal(dr["VDSRate"].ToString());
                    vm.CnFRate = Convert.ToDecimal(dr["CnFRate"].ToString());

                    vm.AITRate = Convert.ToDecimal(dr["AITRate"].ToString());
                    vm.VATRate3 = Convert.ToDecimal(dr["VATRate3"].ToString());
                    vm.SDRate = Convert.ToDecimal(dr["SDRate"].ToString());


                    vm.TollProduct = dr["TollProduct"].ToString();
                    vm.Stock = Convert.ToDecimal(dr["Stock"].ToString());
                    vm.ProductType = dr["IsRaw"].ToString();
                    vm.IsRaw = dr["IsRaw"].ToString();

                    vm.IsZeroVAT = dr["IsZeroVAT"].ToString();
                    vm.IsExempted = dr["IsExempted"].ToString();
                    vm.IsTransport = dr["IsTransport"].ToString();
                    vm.TDSCode = dr["TDSCode"].ToString();

                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                    vm.IsFixedSD = dr["IsFixedSD"].ToString();
                    vm.IsFixedCD = dr["IsFixedCD"].ToString();
                    vm.IsFixedRD = dr["IsFixedRD"].ToString();
                    vm.IsFixedAIT = dr["IsFixedAIT"].ToString();
                    vm.IsFixedVAT1 = dr["IsFixedVAT1"].ToString();
                    vm.IsFixedAT = dr["IsFixedAT"].ToString();
                    vm.IsFixedOtherSD = dr["IsFixedOtherSD"].ToString();
                    vm.IsHouseRent = dr["IsHouseRent"].ToString();
                    vm.ShortName = dr["ShortName"].ToString();
                    vm.IsConfirmed = dr["IsConfirmed"].ToString();
                    vm.IsSample = dr["IsSample"].ToString();
                    vm.IsVDS = dr["IsVDS"].ToString();
                    vm.HPSRate = Convert.ToDecimal(dr["HPSRate"]);
                    vm.TransactionHoldDate = Convert.ToDecimal(dr["TransactionHoldDate"]);
                    vm.IsFixedVATRebate = dr["IsFixedVATRebate"].ToString();
                    vm.Packetprice = Convert.ToDecimal(dr["Packetprice"]);

                    ////vm.WastageTotalQuantity = Convert.ToDecimal(dr["WastageTotalQuantity"]);
                    ////vm.WastageTotalValue = Convert.ToDecimal(dr["WastageTotalValue"]);

                    vm.IsFixedSDChecked = FixedSD == "Y";
                    vm.IsFixedCDChecked = FixedCD == "Y";
                    vm.IsFixedRDChecked = FixedRD == "Y";
                    vm.IsFixedAITChecked = FixedAIT == "Y";
                    vm.IsFixedVAT1Checked = FixedVAT1 == "Y";
                    vm.IsFixedATChecked = FixedAT == "Y";

                    vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());

                    vm.GenericName = dr["GenericName"].ToString();
                    vm.DARNo = dr["DARNo"].ToString();
                    vm.ReportType = dr["ReportType"].ToString();
                    vm.TotalCount = dr["TotalCount"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);



                    VMs.Add(vm);
                }
                dr.Close();


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAllAVGPrice(string Id = null, string[] conditionFields = null, string[] conditionValues = null
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

                //int index = -1;
                //if (conditionFields != null && conditionValues != null)
                //{
                //    index = Array.IndexOf(conditionFields, "SelectTop");
                //    if (index >= 0)
                //    {
                //        count = conditionValues[index].ToString();

                //        var field = conditionFields.ToList();
                //        var Values = conditionValues.ToList();
                //        field.RemoveAt(index);
                //        Values.RemoveAt(index);
                //        conditionFields = field.ToArray();
                //        conditionValues = Values.ToArray();
                //    }
                //}

                #region sql statement
                #region SqlText

                //if (count == "All")
                //{
                //    sqlText = @"SELECT ";
                //}
                //else
                //{
                //    sqlText = @"SELECT top " + count + " ";

                //}

                sqlText += @"

select distinct p.ItemNo,p.ProductCode,p.ProductName,c.CategoryName,c.IsRaw ProductType
from ProductAvgPrice r 
left outer join  Products p on p.ItemNo=r.ItemNo
left outer join  ProductCategories c on p.CategoryID=c.CategoryID
where 1=1
and AvgPrice<=0
and transactionType not in('Opening')
and c.IsRaw not in('Overhead')
order by c.IsRaw,c.CategoryName,p.ProductCode,p.ProductName

";
                #region sqlTextCount
                //                sqlTextCount += @" select count(c.CustomerID)RecordCount
                //FROM Customers  c left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
                //WHERE  1=1 AND  isnull(c.IsArchive,0) = 0
                //";
                #endregion


                //if (Id != null)
                //{
                //    sqlTextParameter += @" and c.CustomerID=@CustomerID";
                //}

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

                //if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                //{
                //    for (int j = 0; j < conditionFields.Length; j++)
                //    {
                //        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]) || conditionValues[j] == "0")
                //        {
                //            continue;
                //        }
                //        cField = conditionFields[j].ToString();
                //        cField = OrdinaryVATDesktop.StringReplacing(cField);
                //        if (conditionFields[j].ToLower().Contains("like"))
                //        {
                //            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                //        }
                //        else
                //        {
                //            da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                //        }
                //    }
                //}

                //if (Id != null)
                //{
                //    da.SelectCommand.Parameters.AddWithValue("@CustomerID", Id);
                //}
                da.Fill(ds);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                dt = ds.Tables[0].Copy();
                //if (index >= 0)
                //{
                //    dt.Rows.Add(ds.Tables[1].Rows[0][0]);
                //}

                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("ProductDAL", "SelectAllAVGPrice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("ProductDAL", "SelectAllAVGPrice", ex.ToString() + "\n" + sqlText);

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

        public DataSet SelectNegInventoryData(string VATType = "6_1", string[] conditionFields = null, string[] conditionValues = null
        , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            DataSet ds = new DataSet();

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
                if (VATType.ToString().ToLower() == "6_1")
                {
                    sqlText = "";
                    sqlText += @"
select distinct '6_1' ReportType,c.IsRaw ProductType,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName ,startDAteTime StartDateTime
from VAT6_1_Permanent v
left outer join Products p on v.ItemNo=p.ItemNo
left outer join ProductCategories c on c.CategoryID=p.CategoryID
where  RunningTotal<0 or RunningValue <0
--group by c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName
order by c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName

select distinct bf.BranchName,bf.BranchCode,'6_1' ReportType,c.IsRaw ProductType,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName ,startDAteTime StartDateTime
from VAT6_1_Permanent_Branch v
left outer join Products p on v.ItemNo=p.ItemNo
left outer join ProductCategories c on c.CategoryID=p.CategoryID
left outer join BranchProfiles bf on bf.BranchId=v.BranchId
where  RunningTotal<0 or RunningValue <0
--group by c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName
order by bf.BranchName,bf.BranchCode,c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName
";
                }
                else if (VATType.ToString().ToLower() == "6_2")
                {
                    sqlText = "";
                    sqlText += @"
select distinct '6_2' ReportType,c.IsRaw ProductType,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName ,startDAteTime StartDateTime
from VAT6_2_Permanent v
left outer join Products p on v.ItemNo=p.ItemNo
left outer join ProductCategories c on c.CategoryID=p.CategoryID
where  RunningTotal<0 or RunningTotalValueFinal <0
--group by c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName
order by c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName

select distinct bf.BranchName,bf.BranchCode,'6_2' ReportType,c.IsRaw ProductType,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName ,startDAteTime StartDateTime
from VAT6_2_Permanent_Branch v
left outer join Products p on v.ItemNo=p.ItemNo
left outer join ProductCategories c on c.CategoryID=p.CategoryID
left outer join BranchProfiles bf on bf.BranchId=v.BranchId
where  RunningTotal<0 or RunningTotalValueFinal <0
--group by c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName
order by bf.BranchName,bf.BranchCode,c.IsRaw ,c.CategoryName, p.ItemNo,p.ProductCode,p.ProductName

";
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


                da.Fill(ds);

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

                FileLogger.Log("ProductDAL", "SelectAllAVGPrice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("ProductDAL", "SelectAllAVGPrice", ex.ToString() + "\n" + sqlText);

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
            return ds;
        }

        public DataTable GetExcelDataAVGPrice(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductAVGPrice");

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

                sqlText = @"  select R.ItemNo,p.ProductCode,p.ProductName,c.CategoryName,c.IsRaw ProductType,agvPriceDate
,case when TransactionType in('Opening') then PurchaseQty else 0 end OpeningQty
,case when TransactionType in('Opening') then PurchaseValue else 0 end OpeningValue
,case when TransactionType in('Purchase') then PurchaseQty else 0 end   PurchaseQty
,case when TransactionType in('Purchase') then PurchaseValue else 0 end PurchaseValue
,case when TransactionType in('Issue') then PurchaseQty else 0 end   IssueQty
,case when TransactionType in('Issue') then PurchaseValue else 0 end IssueValue
,isnull(RuntimeQty,0)RuntimeQty,isnull( RuntimeTotal,0)RuntimeTotal,TransactionType
from ProductAvgPrice R
left outer join  Products p on p.ItemNo=r.ItemNo
left outer join  ProductCategories c on p.CategoryID=c.CategoryID
where R.itemno in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += @" order by c.IsRaw,c.CategoryName,p.ProductCode,p.ProductName, agvPriceDate";

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

                FileLogger.Log("ProductDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

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

        public List<ProductVM> SelectAllList(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
            #endregion
            try
            {
                #region sql statement

                #region SqlExecution
                DataTable dt = SelectProductDTAll(conditionFields, conditionValues, VcurrConn, Vtransaction, false, 0, "", "", null, connVM, null);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new ProductVM();
                        var FixedSD = dr["IsFixedSD"].ToString();
                        var FixedCD = dr["IsFixedCD"].ToString();
                        var FixedRD = dr["IsFixedRD"].ToString();
                        var FixedAIT = dr["IsFixedAIT"].ToString();
                        var FixedVAT1 = dr["IsFixedVAT1"].ToString();
                        var FixedAT = dr["IsFixedAT"].ToString();

                        vm.ItemNo = dr["ItemNo"].ToString();
                        vm.ProductCode = dr["ProductCode"].ToString();
                        vm.ProductName = dr["ProductName"].ToString();
                        vm.ProductDescription = dr["ProductDescription"].ToString();
                        vm.CategoryID = dr["CategoryID"].ToString();
                        vm.CategoryName = dr["CategoryName"].ToString();
                        vm.UOM = dr["UOM"].ToString();
                        vm.UOM2 = dr["UOM2"].ToString();
                        vm.UOMConversion = Convert.ToDecimal(dr["UOMConvertion"].ToString());
                        vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                        vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                        vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                        vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                        vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                        vm.SerialNo = dr["SerialNo"].ToString();
                        vm.HSCodeNo = dr["HSCodeNo"].ToString();
                        vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                        vm.Comments = dr["Comments"].ToString();
                        vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                        vm.Trading = dr["Trading"].ToString();
                        vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                        vm.NonStock = dr["NonStock"].ToString();
                        vm.OpeningDate = dr["OpeningDate"].ToString();
                        vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                        vm.Banderol = dr["Banderol"].ToString();

                        vm.CDRate = Convert.ToDecimal(dr["CDRate"].ToString());
                        vm.RDRate = Convert.ToDecimal(dr["RDRate"].ToString());
                        vm.TVARate = Convert.ToDecimal(dr["TVARate"].ToString());
                        vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString());
                        vm.VATRate2 = Convert.ToDecimal(dr["VATRate2"].ToString());
                        vm.TradingSaleVATRate = Convert.ToDecimal(dr["TradingSaleVATRate"].ToString());
                        vm.TradingSaleSD = Convert.ToDecimal(dr["TradingSaleSD"].ToString());

                        vm.VDSRate = Convert.ToDecimal(dr["VDSRate"].ToString());
                        vm.CnFRate = Convert.ToDecimal(dr["CnFRate"].ToString());

                        vm.AITRate = Convert.ToDecimal(dr["AITRate"].ToString());
                        vm.VATRate3 = Convert.ToDecimal(dr["VATRate3"].ToString());
                        vm.SDRate = Convert.ToDecimal(dr["SDRate"].ToString());


                        vm.TollProduct = dr["TollProduct"].ToString();
                        vm.Stock = Convert.ToDecimal(dr["Stock"].ToString());
                        vm.ProductType = dr["IsRaw"].ToString();
                        vm.IsRaw = dr["IsRaw"].ToString();

                        vm.IsZeroVAT = dr["IsZeroVAT"].ToString();
                        vm.IsExempted = dr["IsExempted"].ToString();
                        vm.IsTransport = dr["IsTransport"].ToString();
                        vm.TDSCode = dr["TDSCode"].ToString();

                        vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                        vm.IsFixedSD = dr["IsFixedSD"].ToString();
                        vm.IsFixedCD = dr["IsFixedCD"].ToString();
                        vm.IsFixedRD = dr["IsFixedRD"].ToString();
                        vm.IsFixedAIT = dr["IsFixedAIT"].ToString();
                        vm.IsFixedVAT1 = dr["IsFixedVAT1"].ToString();
                        vm.IsFixedAT = dr["IsFixedAT"].ToString();
                        vm.IsFixedOtherSD = dr["IsFixedOtherSD"].ToString();
                        vm.IsHouseRent = dr["IsHouseRent"].ToString();
                        vm.ShortName = dr["ShortName"].ToString();
                        vm.IsConfirmed = dr["IsConfirmed"].ToString();
                        vm.IsVDS = dr["IsVDS"].ToString();
                        vm.HPSRate = Convert.ToDecimal(dr["HPSRate"]);
                        vm.TransactionHoldDate = Convert.ToDecimal(dr["TransactionHoldDate"]);
                        vm.IsFixedVATRebate = dr["IsFixedVATRebate"].ToString();

                        vm.IsFixedSDChecked = FixedSD == "Y";
                        vm.IsFixedCDChecked = FixedCD == "Y";
                        vm.IsFixedRDChecked = FixedRD == "Y";
                        vm.IsFixedAITChecked = FixedAIT == "Y";
                        vm.IsFixedVAT1Checked = FixedVAT1 == "Y";
                        vm.IsFixedATChecked = FixedAT == "Y";

                        vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());
                        vm.TollOpeningQuantity = Convert.ToDecimal(dr["TollOpeningQuantity"].ToString());

                        vm.GenericName = dr["GenericName"].ToString();
                        vm.DARNo = dr["DARNo"].ToString();

                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);

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

        public List<ProductNameVM> SelectProductName(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductNameVM> VMs = new List<ProductNameVM>();
            ProductNameVM vm;
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
Pr.ItemNo
,Pr.ProductName
FROM ProductDetails Pr
WHERE  1=1
";


                if (ItemNo != "0")
                {
                    sqlText += @" and Pr.ItemNo=@ItemNo";
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
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.ProductName))
                    {
                        sqlText += " AND Pr.ProductName like @ProductName ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.ProductCode))
                    {
                        sqlText += " AND Pr.ProductCode like @ProductCode ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.HSCodeNo))
                    {
                        sqlText += " AND Pr.HSCodeNo like @HSCodeNo ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        sqlText += " AND Pr.SerialNo like @SerialNo ";
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
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
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.ProductName))
                    {
                        objComm.Parameters.AddWithValue("@ProductName", "%" + likeVM.ProductName + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.ProductCode))
                    {
                        objComm.Parameters.AddWithValue("@ProductCode", "%" + likeVM.ProductCode + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.HSCodeNo))
                    {
                        objComm.Parameters.AddWithValue("@HSCodeNo", "%" + likeVM.HSCodeNo + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        objComm.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
                    }
                }
                if (ItemNo != "0")
                {
                    objComm.Parameters.AddWithValue("@ItemNo", ItemNo.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductNameVM();




                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();



                    VMs.Add(vm);
                }
                dr.Close();

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
                FileLogger.Log("ProductDAL", "SelectProductName", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectProductName", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public List<ProductVM> SelectAllOverhead(string ItemNo = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
 Pr.ItemNo
,Pr.ProductCode
,Pr.ProductName
,Pr.ProductDescription
,Pr.CategoryID
,Pr.UOM
,isnull(Pr.CostPrice,0) CostPrice
,isnull(Pr.SalesPrice,0) SalesPrice
,isnull(Pr.NBRPrice,0) NBRPrice
,isnull(Pr.ReceivePrice,0) ReceivePrice
,isnull(Pr.IssuePrice,0) IssuePrice
,isnull(Pr.TenderPrice,0) TenderPrice
,isnull(Pr.ExportPrice,0) ExportPrice
,isnull(Pr.InternalIssuePrice,0) InternalIssuePrice
,isnull(Pr.TollIssuePrice,0) TollIssuePrice
,isnull(Pr.TollCharge,0) TollCharge
,Pr.OpeningBalance
,Pr.SerialNo
,Pr.HSCodeNo
,isnull(Pr.VATRate,0) VATRate
,Pr.Comments
,isnull(Pr.SD,0) SD
,isnull(Pr.PacketPrice,0) PacketPrice
,Pr.Trading
,isnull(Pr.TradingMarkUp,0) TradingMarkUp
,Pr.NonStock
,isnull(Pr.QuantityInHand,0) QuantityInHand
,Pr.OpeningDate
,isnull(Pr.RebatePercent,0) RebatePercent
,isnull(Pr.TVBRate,0) TVBRate
,isnull(Pr.CnFRate,0) CnFRate
,isnull(Pr.InsuranceRate,0) InsuranceRate
,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.AITRate,0) AITRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.IsFixedVAT,'N') IsFixedVAT
,isnull(Pr.FixedVATAmount,0) FixedVATAmount
,Pr.ActiveStatus
,Pr.CreatedBy
,Pr.CreatedOn
,Pr.LastModifiedBy
,Pr.LastModifiedOn
,isnull(Pr.OpeningTotalCost,0) OpeningTotalCost
,Pr.Banderol
,Pr.TollProduct
,Pc.CategoryName
,isnull(pr.TransactionHoldDate, '0') TransactionHoldDate
,isnull(pr.IsFixedVATRebate, 'N') IsFixedVATRebate
,isnull(pr.TDSCode, '-') TDSCode

FROM Products Pr left outer join ProductCategories Pc on Pr.CategoryID=Pc.CategoryID
WHERE  1=1 AND Pr.IsArchive = 0 and pc.IsRaw='Overhead'
";


                if (ItemNo != "0")
                {
                    sqlText += @" and Pr.ItemNo=@ItemNo";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        sqlText += " AND Pr." + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (ItemNo != "0")
                {
                    objComm.Parameters.AddWithValue("@ItemNo", ItemNo.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.Trading = dr["Trading"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.OpeningDate = dr["OpeningDate"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                    vm.Banderol = dr["Banderol"].ToString();
                    vm.TollProduct = dr["TollProduct"].ToString();
                    vm.TDSCode = dr["TDSCode"].ToString();
                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                    vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());
                    vm.TransactionHoldDate = Convert.ToDecimal(dr["TransactionHoldDate"].ToString());
                    vm.IsFixedVATRebate = dr["IsFixedVATRebate"].ToString();
                    vm.Packetprice = Convert.ToDecimal(dr["PacketPrice"].ToString());

                    VMs.Add(vm);
                }
                dr.Close();

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
                FileLogger.Log("ProductDAL", "SelectAllOverhead", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectAllOverhead", ex.ToString() + "\n" + sqlText);

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

        public ProductVM GetProductWithCostPrice(string productCode, string purchaseNo, string effectDate, SysDBInfoVMTemp connVM = null)
        {
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };
            var product = SelectAll("0", conditionalFields, conditionalValues, null, null, null, connVM).FirstOrDefault();
            decimal NewCostPrice = 0;
            var productGroup = new ProductCategoryDAL().SelectAllList(Convert.ToInt32(product.CategoryID), null, null, null, null, connVM).FirstOrDefault();
            string transType = GetTransactionType(product.ItemNo, effectDate, connVM);

            if (productGroup.IsRaw == "Raw")
            {
                if (transType == "TollReceiveRaw")
                {
                    NewCostPrice = 0;
                }
            }
            if (productGroup.IsRaw == "Raw" || productGroup.IsRaw == "Pack" || productGroup.IsRaw == "Trading")
            {
                DataTable dt = GetLIFOPurchaseInformation(product.ItemNo, effectDate, purchaseNo, connVM);

                if (dt.Rows.Count > 0)
                {
                    var PurchaseCostPrice = Convert.ToDecimal(dt.Rows[0]["PurchaseCostPrice"].ToString());
                    var PurchaseQuantity = Convert.ToDecimal(dt.Rows[0]["PurchaseQuantity"].ToString());
                    NewCostPrice = 0;
                    var PinvoiceNo = dt.Rows[0]["PurchaseInvoiceNo"].ToString();
                    if (PurchaseQuantity != 0)
                    {
                        NewCostPrice = PurchaseCostPrice / PurchaseQuantity;
                    }

                }
                else
                {
                    string[] retResult = { "Fail", "This Item has no price declaration" };
                    product.retResult = retResult;
                    //MessageBox.Show("This Item Name('" + txtRProductName.Text.Trim() + "'), Code('" + txtRProductCode.Text.Trim() + "') has no purchase price. ", this.Text);

                }
                if (transType == "TollReceiveRaw")
                {
                    NewCostPrice = 0;
                }
            }
            else
            {
                var nbrPrice = GetLastNBRPriceFromBOM_VatName(product.ItemNo, "VAT 1", effectDate, null, null, "0", connVM);
                if (nbrPrice == 0)
                {
                    nbrPrice = GetLastNBRPriceFromBOM_VatName(product.ItemNo, "VAT 1 Ka (Tarrif)", effectDate, null, null, "0", connVM);
                    NewCostPrice = nbrPrice;
                }
                else
                {
                    NewCostPrice = nbrPrice;
                }
                if (nbrPrice == 0)
                {
                    string[] retResult = { "Fail", "This Item has no price declaration" };
                    product.retResult = retResult;
                    //MessageBox.Show(
                    //    "This Item Name('" + txtRProductName.Text.Trim() + "'), Code('" + txtRProductCode.Text.Trim() + "') has no price declaration. ", this.Text);

                }
            }
            product.CostPrice = NewCostPrice;
            return product;
        }

        #endregion

        public string[] InserToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
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
            ImportDAL importDal = new ImportDAL();

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

                sqlText = "select COUNT(BranchId) FROM  ProductStocks where 1=1 and ItemNo=@ItemNo and BranchId=@BranchId";
                SqlCommand cmdIsExit = new SqlCommand(sqlText, currConn);
                cmdIsExit.Transaction = transaction;
                cmdIsExit.Parameters.AddWithValue("@ItemNo", psVM.ItemNo);
                cmdIsExit.Parameters.AddWithValue("@BranchId", psVM.BranchId);
                int IsExit = (int)cmdIsExit.ExecuteScalar();
                if (IsExit > 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Requested Product Stocks Already Exit";
                    retResults[2] = "";
                    retResults[3] = "";
                }
                if (IsExit == 0)
                {
                    #region Customer  new id generation

                    sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  ProductStocks";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();

                    if (nextId <= 0)
                    {
                        throw new ArgumentNullException("Unable to create new ProductStocks");
                    }


                    #endregion Customer  new id generation

                    if (psVM.BranchId == 1)
                    {
                        sqlText = "";
                        sqlText = "update Products set  ";
                        sqlText += " OpeningBalance=@OpeningBalance";
                        sqlText += " ,OpeningTotalCost=@OpeningTotalCost";
                        sqlText += " where ItemNo=@ItemNo";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        cmdUpdate.Parameters.AddWithValue("@OpeningBalance", psVM.StockQuantity);
                        cmdUpdate.Parameters.AddWithValue("@OpeningTotalCost", psVM.StockValue);
                        cmdUpdate.Parameters.AddWithValue("@ItemNo", psVM.ItemNo);
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                    }

                    #region Insert new Product Stocks
                    sqlText = "";
                    sqlText += "insert into ProductStocks";
                    sqlText += "(";
                    sqlText += "Id,";
                    sqlText += "ItemNo,";
                    sqlText += "BranchId,";
                    sqlText += "StockQuantity,";
                    sqlText += "StockValue,";
                    sqlText += "CurrentStock,";
                    sqlText += "Comments,";
                    sqlText += "WastageTotalQuantity";
                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += " @Id,";
                    sqlText += " @ItemNo,";
                    sqlText += " @BranchId,";
                    sqlText += " @StockQuantity,";
                    sqlText += " @StockValue,";
                    sqlText += " @CurrentStock,";
                    sqlText += " @Comments,";
                    sqlText += " @WastageTotalQuantity";
                    sqlText += ")SELECT SCOPE_IDENTITY()";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@Id", nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemNo", psVM.ItemNo);
                    cmdInsert.Parameters.AddWithValue("@BranchId", psVM.BranchId);
                    cmdInsert.Parameters.AddWithValue("@StockQuantity", psVM.StockQuantity);
                    cmdInsert.Parameters.AddWithValue("@StockValue", psVM.StockValue);
                    cmdInsert.Parameters.AddWithValue("@CurrentStock", psVM.CurrentStock);
                    cmdInsert.Parameters.AddWithValue("@Comments", psVM.Comments);
                    cmdInsert.Parameters.AddWithValue("@WastageTotalQuantity", psVM.WastageTotalQuantity);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    psVM.StockId = transResult;

                    if (transResult > 0)
                    {
                        retResults[0] = "Success";
                        retResults[1] = "Requested Product Stocks successfully Added";
                        retResults[2] = "" + psVM.StockId;
                        retResults[3] = "" + psVM.StockId;


                        importDal.UpdateStocksAVG(cmdInsert, psVM.ItemNo, connVM);
                    }

                    #region Update Current Stock

                    ResultVM rVM = new ResultVM();
                    ParameterVM paramVM = new ParameterVM();
                    paramVM.BranchId = psVM.BranchId;
                    paramVM.ItemNo = psVM.ItemNo;

                    rVM = Product_Stock_Update(paramVM, currConn, transaction, connVM, UserId);

                    #endregion

                    #endregion

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
                                retResults[1] = "Requested Product Stocks successfully Added";
                                retResults[2] = "" + nextId;
                                retResults[3] = "" + nextId;

                            }
                            else
                            {
                                transaction.Rollback();
                                retResults[0] = "Fail";
                                retResults[1] = "Unexpected erro to add Product Stocks";
                                retResults[2] = "";
                                retResults[3] = "";
                            }

                        }
                        else
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add Product Stocks";
                            retResults[2] = "";
                            retResults[3] = "";
                        }
                    }
                    #endregion Commit




                    #endregion Commit

                }

            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "InserToProductStock", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {

                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "InserToProductStock", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] UpdateToProductStockWeb(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
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
            ImportDAL importDal = new ImportDAL();

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

                sqlText = "select COUNT(BranchId) FROM  ProductStocks where 1=1 and ItemNo=@ItemNo and BranchId=@BranchId";
                SqlCommand cmdIsExit = new SqlCommand(sqlText, currConn);
                cmdIsExit.Transaction = transaction;
                cmdIsExit.Parameters.AddWithValue("@ItemNo", psVM.ItemNo);
                cmdIsExit.Parameters.AddWithValue("@BranchId", psVM.BranchId);
                int IsExit = (int)cmdIsExit.ExecuteScalar();
                if (IsExit > 0)
                {

                    retResults = UpdateToProductStock(psVM, currConn, Vtransaction, connVM);

                    retResults[0] = "Update";
                    retResults[1] = "Requested Product Stocks successfully Added";
                    retResults[2] = "";
                    retResults[3] = "";
                }
                else
                {
                    retResults = InserToProductStock(psVM, currConn, Vtransaction, connVM, UserId);
                }


            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "UpdateToProductStockWeb", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchProductStock(string ItemNo, string id, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null)
        {

            #region Declarations

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

            #endregion

            #region Try Statements

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
ps.Id
,ps.ItemNo
,ps.BranchId
,ps.StockQuantity
,ps.StockValue
,ISNULL(ps.CurrentStock,0) CurrentStock
,ps.Comments
,bp.BranchName
,ISNULL(ps.WastageTotalQuantity,0) WastageTotalQuantity

FROM ProductStocks ps left outer join BranchProfiles bp on ps.BranchId=bp.BranchID
where 1=1
";
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    sqlText += @"  and ps.ItemNo=@ItemNo";
                }

                if (userBranchs != null)
                {
                    sqlText += " and ps.BranchId in (";

                    foreach (UserBranchDetailVM branch in userBranchs)
                    {
                        sqlText += branch.BranchId + ",";
                    }
                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                var cmd = new SqlCommand(sqlText, currConn);

                cmd.Parameters.AddWithValue("@ItemNo", ItemNo);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);

                #endregion

            }

            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductStock", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }

        public DataTable SearchProductCustomerRate(string ItemNo, string BranchId, SysDBInfoVMTemp connVM = null, List<UserBranchDetailVM> userBranchs = null)
        {

            #region Declarations

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

            #endregion

            #region Try Statements

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
       p.[CustomerId]
      ,p.[ItemNo]
      ,p.[NBRPrice]
      ,p.[TollCharge]
      ,p.[BranchId]
	  ,c.[VendorName][CustomerName]
  FROM [ProductCustomerRate] p
  left outer join Vendors c on p.CustomerId=c.VendorID
where 1=1
";
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    sqlText += @"  and p.ItemNo=@ItemNo";
                }

                if (!string.IsNullOrEmpty(BranchId))
                {
                    sqlText += @"  and p.BranchId=@BranchId";
                }

                //if (userBranchs != null)
                //{
                //    sqlText += " and ps.BranchId in (";

                //    foreach (UserBranchDetailVM branch in userBranchs)
                //    {
                //        sqlText += branch.BranchId + ",";
                //    }
                //    sqlText = sqlText.TrimEnd(',') + ")";

                //}

                var cmd = new SqlCommand(sqlText, currConn);

                cmd.Parameters.AddWithValue("@ItemNo", ItemNo);
                cmd.Parameters.AddWithValue("@BranchId", BranchId);


                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);

                #endregion

            }

            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductCustomerRate", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }

        public string[] DeleteToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {


            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";


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

                sqlText = "delete ProductStocks where Id='" + psVM.StockId + "'";

                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Product Stocks successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Product Stocks successfully deleted";
                        retResults[2] = "";

                    }
                }

                #endregion Commit

            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "DeleteToProductStock", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
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

        public string[] UpdateToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(psVM.StockId.ToString()))
                {
                    throw new ArgumentNullException("UpdateToProduct",
                                                    "Invalid Stock  Id");
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

                #region Update new ProductStocks
                sqlText = "";
                sqlText = "update ProductStocks set";
                sqlText += " StockQuantity=@StockQuantity";
                sqlText += " ,StockValue=@StockValue";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,WastageTotalQuantity=@WastageTotalQuantity";
                sqlText += " where Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@StockQuantity", psVM.StockQuantity);
                cmdUpdate.Parameters.AddWithValue("@StockValue", psVM.StockValue);
                cmdUpdate.Parameters.AddWithValue("@Comments", psVM.Comments);
                cmdUpdate.Parameters.AddWithValue("@Id", psVM.StockId);
                cmdUpdate.Parameters.AddWithValue("@WastageTotalQuantity", psVM.WastageTotalQuantity);
                transResult = (int)cmdUpdate.ExecuteNonQuery();

                if (psVM.BranchId == 1)
                {
                    sqlText = "";
                    sqlText = "update Products set  ";
                    sqlText += " OpeningBalance=@OpeningBalance";
                    sqlText += " ,OpeningTotalCost=@OpeningTotalCost";
                    sqlText += " where ItemNo=@ItemNo";

                    cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.AddWithValue("@OpeningBalance", psVM.StockQuantity);
                    cmdUpdate.Parameters.AddWithValue("@OpeningTotalCost", psVM.StockValue);
                    cmdUpdate.Parameters.AddWithValue("@ItemNo", psVM.ItemNo);
                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                }

                ImportDAL importDal = new ImportDAL();

                importDal.UpdateStocksAVG(cmdUpdate, psVM.ItemNo, connVM);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested Product Stocks successfully Update";
                            retResults[2] = psVM.ItemNo;
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update Stock  ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update Stock";
                    }
                }




                #endregion Commit

                #endregion

            }
            #endregion
            #region Catch
            catch (SqlException sqlex)
            {
                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "UpdateToProductStock", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {

                if (Vtransaction == null) transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

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

        public DataSet GetReconsciliationData(string fromDate, string toDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var table = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

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



                sqlText = @"
--declare @DtFrom date='01/01/2019'
--declare @DtTo date='01/01/2029'

select  distinct p.ProductCode, p.ProductName,rd.UOM, sum(rd.Quantity)Quantity from ReceiveDetails rd 
left outer join products p on rd.itemno=p.itemno
where rd.Post='Y' 
and rd.ReceiveDateTime  >= @DtFrom and rd.ReceiveDateTime < dateadd(d,1,@DtTo)
group by p.productCode,p.ProductName,rd.UOM
order by p.productCode,p.ProductName,rd.UOM
--sum(  UseQuantity)-sum(  IssueQuantity)
select  a.ProductCode,a.ProductName,a.UOM,sum( UseQuantity)BOMWiseIssueQuantity,sum(  IssueQuantity) ActualIssueQuantity 
,sum(  IssueQuantity)-sum(  UseQuantity) Variance
--, 100-(sum(  IssueQuantity)*100/sum(  UseQuantity)) VariancePercent
 from (
select  distinct rp.productCode,rp.ProductName,br.UOM,sum( br.totalQuantity*rd.UOMQty)UseQuantity,0 IssueQuantity
from ReceiveDetails rd 
left outer join BOMRaws br on rd.bomid=br.bomid and br.RawItemType in('raw','Pack')
left outer join products rp on br.RawItemNo=rp.itemno
where rd.Post='Y' 
and rd.ReceiveDateTime >= @DtFrom and rd.ReceiveDateTime < dateadd(d,1,@DtTo)
group by rp.productCode,rp.ProductName,br.UOM
union all 
select  distinct p.productCode,p.ProductName,rd.UOM, 0 UseQuantity ,sum(rd.Quantity)IssueQuantity from issueDetails rd 
left outer join products p on rd.itemno=p.itemno
where rd.Post='Y' 
and rd.IssueDateTime  >= @DtFrom and rd.IssueDateTime < dateadd(d,1,@DtTo)
group by p.productCode,p.ProductName,rd.UOM
) as a
group by a.productCode,a.ProductName,a.UOM

order by a.productCode,a.ProductName,a.UOM";



                var cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@DtFrom", fromDate);
                cmd.Parameters.AddWithValue("@DtTo", toDate);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                if (Vtransaction == null) transaction.Rollback();

                FileLogger.Log("ProductDAL", "GetReconsciliationData", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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
            return table;
        }

        public DataTable GetProductStock(string itemNo, SqlConnection connection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = connection;
            SqlTransaction transaction = Vtransaction;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

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


                sqlText = @"select * from ProductStocks
where ItemNo = @ItemNo";
                ;

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);


                cmd.Parameters.AddWithValue("@ItemNo", itemNo);

                DataTable dtStock = new DataTable();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dtStock);


                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }



                return dtStock;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                ////2020-12-14
                FileLogger.Log("PurchaseDAL", "GetAvgPrice", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open && connection == null)
                {
                    currConn.Close();
                }
            }
            #endregion

        }

        private void ProductStockProcess(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {

            string sqlText = "";


            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
            cmd.CommandTimeout = 800;


            sqlText = @"


declare @maxStockId int = Isnull((select max(Id)+1 from ProductStocks),0)

create table  #tempBranches ( ID int identitY(1,1), BranchId int , BranchCode varchar(20))
create table #tempProductStock(ID int identitY(1,1),stockId int null, ItemNo varchar(50), BranchId int , StockQuantity decimal(25,9), StockValue decimal(25,9))

DBCC CHECKIDENT (#tempProductStock, RESEED, @maxStockId)


insert into #tempBranches (BranchId, BranchCode)
select BranchID, BranchCode from BranchProfiles

declare @minID int  = (Select min(ID) from #tempBranches)
declare @maxID int  = (Select max(ID) from #tempBranches)

declare @branchId int = 0

while @minID <= @maxID
begin

	select @branchId = branchId from #tempBranches
	where ID = @minID
	

	insert into #tempProductStock(ItemNo, BranchId, StockQuantity, StockValue)

	select ItemNo, @branchId, Isnull(OpeningBalance,0)OpeningBalance,Isnull(OpeningTotalCost,0)OpeningTotalCost from Products
	where ItemNo  in 
	(	
		select itemNo from Products

		except
		select itemno from ProductStocks
		where BranchId = @branchId
	)

	set @minID = @minID + 1
end


insert into ProductStocks(Id,ItemNo,BranchId,StockQuantity,StockValue,CurrentStock)
select ID,ItemNo,BranchId,StockQuantity,StockValue,0 from #tempProductStock

drop table #tempBranches
drop table #tempProductStock


";

            cmd.CommandText = sqlText;
            cmd.ExecuteNonQuery();


        }


        //WEB -JBR-5242021
        public DataTable GetExcelDataWeb(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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
	row_number() OVER (ORDER BY p.ItemNo) SL
	,p.[ProductCode] Code
      ,p.[ProductName]
      ,p.[ProductDescription] [Description]
      ,pc.IsRaw CategoryType
      ,pc.CategoryName [Group]
      ,p.[UOM]
      ,OpeningTotalCost	TotalPrice 
      ,p.[OpeningBalance] OpeningQuantity
      ,p.[SerialNo] RefNo
      ,p.[HSCodeNo] HSCode
	  ,P.NBRPrice
	  ,p.VATRate
	  ,p.Comments
	  ,p.ActiveStatus
	  ,p.SD SDRate
	  ,p.PacketPrice
	  ,p.Trading
	  ,p.TradingMarkUp
	  ,p.NonStock
	  ,cast(p.OpeningDate as varchar(100)) OpeningDate
	  ,p.TollCharge
	  ,isnull(p.GenericName,'-')GenericName
	  ,isnull(p.DARNo,'-')DARNo
	  ,isnull(p.IsConfirmed,'Y')IsConfirmed
	  ,isnull(p.TransactionHoldDate,'0')TransactionHoldDate
	  ,isnull(p.IsFixedVATRebate,'N')IsFixedVATRebate


  FROM [Products] p left outer join ProductCategories pc
  on p.CategoryID = pc.CategoryID
  where p.ItemNo!='ovh0' and p.[ProductCode] in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += " order by pc.IsRaw, p.ProductCode, p.ProductName";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                //for (int i = 0; i < len; i++)
                //{
                //    cmd.Parameters.AddWithValue("@itemNo" + i, ids[i]);
                //}

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);

                string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical");

                if (IsPharmaceutical == "N")
                {
                    string[] columnNames = { "GenericName", "DARNo" };

                    dataTable = OrdinaryVATDesktop.DtDeleteColumns(dataTable, columnNames);
                }

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

                FileLogger.Log("ProductDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

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

        public DataTable GetExcelProductDetailsWeb(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductD");

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

                sqlText = @"SELECT ROW_NUMBER() over (order by pd.ItemNo) SL

      ,p.[ProductCode] Code
      ,pd.[ProductName]

  FROM ProductDetails pd left outer join Products p
  on pd.ItemNo  = p.ItemNo
  where p.[ProductCode] in ";


                var len = ids.Count;

                sqlText += "( ";

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + ids[i] + "',";
                }

                sqlText = sqlText.TrimEnd(',') + ")";

                sqlText += " order by p.ProductCode,p.ProductName";

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


        public ResultVM Delete6_2_1Permanent(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_2_1_Permanent where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_2Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM Delete6_2_1Permanent_Branch(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_2_1_Permanent_Branch where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_2Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public ResultVM Delete6_2Permanent(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_2_Permanent where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }
                cmd.CommandTimeout = 500;

                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_2Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM Delete6_2Permanent_Branch(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_2_Permanent_Branch where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_2Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM Delete6_2Permanent_Branch_Day(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_2_Permanent_DayWise_Branch where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_2Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public ResultVM Delete6_2Permanent_Day(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_2_Permanent_DayWise where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_2Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public ResultVM Delete6_1Permanent(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_1_Permanent where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_1Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM Delete6_1Permanent_Branch(string itemNo, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            int executionResult = 0;
            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction
                commonDAL.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction, connVM);
                #endregion open connection and transaction

                sqlText = "delete from VAT6_1_Permanent_Branch where 1=1 ";

                if (!string.IsNullOrEmpty(itemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(itemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", itemNo);
                }

                cmd.ExecuteNonQuery();

                rVM.Message = "success";

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "Delete6_1Permanent", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public DataTable GetUprocessCount(SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDal = new CommonDAL();

            try
            {
                commonDal.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);

                string sqlText = @"
select 
sum(case when ReportType='VAT6_1' OR ReportType='VAT6_1_And_6_2' then 1 else 0 end) VAT6_1Count
,sum(case when ReportType='VAT6_2' OR ReportType='VAT6_1_And_6_2' then 1 else 0 end) VAT6_2Count 
,sum(case when ReportType='VAT6_2_1' then 1 else 0 end) VAT6_2_1Count 
from products where ProcessFlag='Y'

";
                SqlCommand command = new SqlCommand(sqlText, connection, transaction);
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(table);

                commonDal.TransactionCommit(ref currentTransaction, ref transaction, connVM);

                return table;
            }
            catch (Exception e)
            {
                commonDal.TransactionRollBack(ref currentTransaction, ref transaction, connVM);
                throw e;
            }
            finally
            {
                commonDal.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }


        public ResultVM ProcessFreshStockXX(ParameterVM parameterVm, SqlConnection currentConnection = null, SqlTransaction currentTransaction = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction);

                ProductStockProcess(connection, transaction);

                string resetStocktable = @"update ProductStocks set CurrentStock = 0
                where BranchId = @BranchId";

                string deleteVAT6_1 = @"delete from VAT6_1 where UserId = @UserId";
                string deleteVAT6_2 = @"delete from VAT6_2 where UserId = @UserId";

                string updateProductStock = @"
update ProductStocks set CurrentStock = VAT6_1.RunningTotal
from VAT6_1
where VAT6_1.ItemNo = ProductStocks.ItemNo and ProductStocks.BranchId = VAT6_1.BranchId and VAT6_1.UserId = @UserId

update ProductStocks set CurrentStock = VAT6_2.RunningTotal
from VAT6_2
where VAT6_2.ItemNo = ProductStocks.ItemNo and ProductStocks.BranchId = VAT6_2.BranchId and VAT6_2.UserId = @UserId


";

                SqlCommand cmd = new SqlCommand("", connection, transaction);

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();
                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                #region Settings

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = _cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y";
                bool IssueFrom6_1 = _cDal.settings("Toll6_4", "IssueFrom6_1") == "Y";
                bool TotalIncludeSD = _cDal.settings("VAT6_1", "TotalIncludeSD") == "Y";
                bool IncludeOtherAMT = _cDal.settings("VAT6_1", "IncludeOtherAMT") == "Y";
                bool TollReceiveNotWIP = _cDal.settings("IssueFromBOM", "TollReceive-NotWIP") == "Y";
                bool TollReceiveWithIssue = _cDal.settings("TollReceive", "WithIssue") == "Y";
                bool Permanent6_1 = _cDal.settings("VAT6_1", "6_1Permanent") == "Y";
                string vExportInBDT = _cDal.settings("VAT9_1", "ExportInBDT");
                string vAutoAdjustment = _cDal.settings("VAT6_2", "AutoAdjustment");
                string PDesc = _cDal.settingsDesktop("VAT6_2", "ProductDescription");

                #endregion


                VAT6_1ParamVM paramVm6_1 = new VAT6_1ParamVM();

                paramVm6_1.StartDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                paramVm6_1.EndDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

                paramVm6_1.Post1 = "Y";
                paramVm6_1.Post2 = "Y";
                paramVm6_1.BranchId = 0;
                paramVm6_1.PreviewOnly = false;
                paramVm6_1.InEnglish = "N";
                paramVm6_1.UOMConversion = 1;
                paramVm6_1.UOM = "";
                paramVm6_1.UOMTo = "";
                paramVm6_1.UserName = parameterVm.CurrentUser;
                paramVm6_1.ReportName = "";
                paramVm6_1.Opening = false;
                paramVm6_1.OpeningFromProduct = false;

                paramVm6_1.IsMonthly = false;
                paramVm6_1.IsTopSheet = false;
                paramVm6_1.IsGroupTopSheet = false;
                paramVm6_1.Is6_1Permanent = true;
                paramVm6_1.StockProcess = true;
                paramVm6_1.UserId = parameterVm.CurrentUserID;


                VAT6_2ParamVM paramVm6_2 = new VAT6_2ParamVM();
                paramVm6_2.StartDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                paramVm6_2.EndDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

                paramVm6_2.Post1 = "Y";
                paramVm6_2.Post2 = "Y";
                paramVm6_2.BranchId = 0;
                paramVm6_2.rbtnService = false;
                paramVm6_2.rbtnWIP = false;
                paramVm6_2.UOMTo = "";
                paramVm6_2.AutoAdjustment = vAutoAdjustment == "Y";
                paramVm6_2.PreviewOnly = false;
                paramVm6_2.InEnglish = "N";
                paramVm6_2.UOM = "";
                paramVm6_2.IsMonthly = false;
                paramVm6_2.IsTopSheet = false;

                paramVm6_2.UserId = parameterVm.CurrentUserID;


                if (!string.IsNullOrEmpty(parameterVm.ItemNo))
                {
                    paramVm6_1.ItemNo = parameterVm.ItemNo;
                    paramVm6_1.Is6_1Permanent = false;

                    paramVm6_2.ItemNo = parameterVm.ItemNo;

                    resetStocktable += " and ItemNo='" + paramVm6_1.ItemNo + "'";

                }

                for (var index = 0; index < branchList.Count; index++)
                {
                    BranchProfileVM branchProfileVm = branchList[index];

                    // reset the current stock
                    cmd.CommandText = resetStocktable;
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", branchProfileVm.BranchID);
                    paramVm6_1.BranchId = branchProfileVm.BranchID;
                    paramVm6_2.BranchId = branchProfileVm.BranchID;

                    cmd.CommandText = deleteVAT6_1 + " " + deleteVAT6_2;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", parameterVm.CurrentUserID);

                    string[] result = _vatRegistersDAL.Save6_1(paramVm6_1, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1,
                        TollReceiveNotWIP,
                        TollReceiveWithIssue, connection, transaction);

                    result = _vatRegistersDAL.Save6_2(paramVm6_2, vExportInBDT, connection, transaction, PDesc);


                    cmd.CommandText = updateProductStock;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", parameterVm.CurrentUserID);
                    cmd.ExecuteNonQuery();


                }

                commonDAL.TransactionCommit(ref currentTransaction, ref transaction);
                return new ResultVM() { Status = "success", Message = "success" };
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection);
            }
        }


        public ResultVM ProcessFreshStock(ParameterVM parameterVm, SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();

            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);

                ProductStockProcess(connection, transaction, connVM);

                string updateProductStock = @"

update ProductStocks set CurrentStock = 0


update ProductStocks set CurrentStock=isnull(VAT6_1.RunningTotal,0)
from(

select BranchId,ItemNo,RunningTotal from VAT6_1_Permanent_Branch
where id in(
select id from(
select distinct BranchId,ItemNo,MAX(id)Id from VAT6_1_Permanent_Branch 
where 1=1 @itemCondition1
group by BranchId,ItemNo) as a)
 ) 
as VAT6_1
 where ProductStocks.ItemNo=VAT6_1.ItemNo and ProductStocks.BranchId=VAT6_1.BranchId 
and ProductStocks.ItemNo in(
select ItemNo from Products
where CategoryID in (select CategoryID from ProductCategories where ReportType in('VAT6_1','VAT6_1_And_6_2'))
)


update ProductStocks set CurrentStock=isnull(VAT6_2.RunningTotal,0)
from(

select BranchId,ItemNo,RunningTotal from VAT6_2_Permanent_Branch
where id in(
select id from(
select distinct BranchId,ItemNo,MAX(id)Id from VAT6_2_Permanent_Branch 
where 1=1 @itemCondition1
group by BranchId,ItemNo) as a)
 ) as VAT6_2
 where ProductStocks.ItemNo=VAT6_2.ItemNo and ProductStocks.BranchId=VAT6_2.BranchId 
and ProductStocks.ItemNo in(
select ItemNo from Products
where CategoryID in (select CategoryID from ProductCategories where ReportType in('VAT6_2','VAT6_1_And_6_2'))
)



update ProductStocks set CurrentStock=isnull(VAT6_2_1.RunningTotal,0)
from(

select BranchId,ItemNo,RunningTotal from VAT6_2_1_Permanent_Branch
where id in(
select id from(
select distinct BranchId,ItemNo,MAX(id)Id from VAT6_2_1_Permanent_Branch 
where 1=1 @itemCondition1
group by BranchId,ItemNo) as a)
 ) as VAT6_2_1
 where ProductStocks.ItemNo=VAT6_2_1.ItemNo and ProductStocks.BranchId=VAT6_2_1.BranchId 
and ProductStocks.ItemNo in(
select ItemNo from Products
where CategoryID in (select CategoryID from ProductCategories where ReportType in('VAT6_2_1'))
)

";

                if (!string.IsNullOrEmpty(parameterVm.ItemNo))
                {
                    updateProductStock = updateProductStock.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                }
                else
                {
                    updateProductStock = updateProductStock.Replace("@itemCondition1", "");
                }

                SqlCommand cmd = new SqlCommand(updateProductStock, connection, transaction);

                cmd.CommandTimeout = 500;

                if (!string.IsNullOrEmpty(parameterVm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", parameterVm.ItemNo);
                }

                cmd.ExecuteNonQuery();


                commonDAL.TransactionCommit(ref currentTransaction, ref transaction, connVM);
                return new ResultVM() { Status = "success", Message = "success" };
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction, connVM);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }

        public DataTable GetTollStock(ParameterVM parameterVm, SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            ReportDSDAL reportDsdal = new ReportDSDAL();

            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);


                string sqlText = @"
select isnull(Sum(stockQuantity),0)stockQuantity from ProductStocks
where ItemNo = @ItemNo
";
                parameterVm.BranchId = 0; // forcely making to check all branch

                if (parameterVm.BranchId > 0)
                {
                    sqlText += " and BranchId = @BranchId";
                }


                SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@ItemNo", parameterVm.ItemNo);

                if (parameterVm.BranchId > 0)
                {
                    cmd.Parameters.AddWithValue("@BranchId", parameterVm.BranchId);
                }

                DataTable dtProductStock = new DataTable();

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(dtProductStock);

                string startDate = DateTime.Now.AddDays(1).ToString("yyyy-MMM-dd 00:00:00");
                string endDate = DateTime.Now.AddDays(1).ToString("yyyy-MMM-dd 23:59:59");

                DataSet dsToll6_1 = reportDsdal.VAT6_1Toll(parameterVm.ItemNo, parameterVm.CurrentUser,
                    startDate, endDate, "y", "y", "",
                    parameterVm.BranchId, connVM, false, connection, transaction, new VAT6_1ParamVM()
                    {
                        ItemNo = parameterVm.ItemNo,
                        StartDate = startDate,
                        EndDate = endDate,
                        Post1 = "y",
                        Post2 = "y",
                        BranchId = parameterVm.BranchId
                    });


                DataSet dsToll6_2 = reportDsdal.VAT6_2Toll(parameterVm.ItemNo,
                    startDate, endDate, "y",
                    "y",
                    parameterVm.BranchId, connVM, connection, transaction, new VAT6_2ParamVM()
                    {
                        ItemNo = parameterVm.ItemNo,
                        StartDate = startDate,
                        EndDate = endDate,
                        Post1 = "y",
                        Post2 = "y",
                        BranchId = parameterVm.BranchId
                    }, false);


                decimal finalStock = 0;

                if (dsToll6_1.Tables[0].Rows.Count > 0)
                {
                    finalStock += Convert.ToDecimal(dsToll6_1.Tables[0].Rows[0]["Quantity"]);
                }

                if (dsToll6_2.Tables[0].Rows.Count > 0)
                {
                    finalStock += Convert.ToDecimal(dsToll6_2.Tables[0].Rows[0]["Quantity"]);
                }

                if (dtProductStock.Rows.Count > 0)
                {
                    finalStock += Convert.ToDecimal(dtProductStock.Rows[0]["stockQuantity"]);
                }

                //finalStock = Convert.ToDecimal(dtProductStock.Rows[0]["stockQuantity"]) +
                //                    Convert.ToDecimal(dsToll6_1.Tables[0].Rows[0]["Quantity"]) +
                //                    Convert.ToDecimal(dsToll6_2.Tables[0].Rows[0]["Quantity"]);


                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Stock");

                dtResult.Rows.Add(dtResult.NewRow());
                dtResult.Rows[0][0] = finalStock;



                commonDAL.TransactionCommit(ref currentTransaction, ref transaction, connVM);
                return dtResult;
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction, connVM);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }


        public DataTable GetStock_Split(SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            ReportDSDAL reportDsdal = new ReportDSDAL();

            DataTable dtResult = new DataTable();
            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);


                string sqlText = @"
delete from StockUpdateTracking
insert into StockUpdateTracking(StockId)
SELECT top 10000 Id
FROM ProductStockMISs AS s
         LEFT OUTER JOIN Products p ON s.ItemNo = p.ItemNo
         LEFT OUTER JOIN ProductCategories pc ON p.CategoryID = pc.CategoryID

WHERE isnull(IsProcess,'N') = 'N'
ORDER BY p.ProductName, TransactionDate, Id


SELECT top 10000 OpeningQuantity
     , OpeningValue
     , pc.IsRaw ProductType
     , pc.CategoryName
     , p.ProductName
     , p.ProductCode
     , p.ItemNo
     --,TransactionDate,TransID,TransType,Remarks,s.Quantity,ClosingAmount Unitcost,ClosingQuantity,ClosingAmount
     , Format(TransactionDate,'yyyy-MM-dd')TransactionDate
     , TransID
     , TransType
     , Remarks
     , s.Quantity
     , Unitcost
     , ClosingQuantity
     , ClosingAmount
     , ClosingQuantityFinal
     , ClosingAmountFinal
     , AdjustmentValue
FROM ProductStockMISs AS s
         LEFT OUTER JOIN Products p ON s.ItemNo = p.ItemNo
         LEFT OUTER JOIN ProductCategories pc ON p.CategoryID = pc.CategoryID

WHERE isnull(IsProcess,'N') = 'N'
ORDER BY p.ProductName, TransactionDate, Id

";



                SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtResult);

                commonDAL.TransactionCommit(ref currentTransaction, ref transaction, connVM);

                return dtResult;
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction, connVM);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }

        public int GetStock_Count(SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            ReportDSDAL reportDsdal = new ReportDSDAL();

            DataTable dtResult = new DataTable();
            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);


                string sqlText = @"
SELECT CEILING(count(Id) / 10000.00)
FROM ProductStockMISs  
WHERE isnull(IsProcess,'N') = 'N'

";
                SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                commonDAL.TransactionCommit(ref currentTransaction, ref transaction, connVM);

                return count;
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction, connVM);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }

        public int Stock_Update(SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            ReportDSDAL reportDsdal = new ReportDSDAL();

            DataTable dtResult = new DataTable();
            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);


                string sqlText = @"
update ProductStockMISs set IsProcess = 'Y'
from ProductStockMISs pm inner join stockUpdateTracking st
on pm.Id = st.StockId



";
                SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                int result = Convert.ToInt32(cmd.ExecuteNonQuery());

                commonDAL.TransactionCommit(ref currentTransaction, ref transaction, connVM);

                return result;
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction, connVM);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }

        public ResultVM DayEndProcess(IntegrationParam processParam, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();
                CommonDAL commonDal = new CommonDAL();
                string VAT6_1ProcessDate = commonDal.settingsMaster("DayEnd", "VAT6_1ProcessDate", null, null, connVM);
                string VAT6_2ProcessDate = commonDal.settingsMaster("DayEnd", "VAT6_2ProcessDate", null, null, connVM);
                string VAT6_2_1ProcessDate = commonDal.settingsMaster("DayEnd", "VAT6_2_1ProcessDate", null, null, connVM);
                string DayEndProcess = commonDal.settingsMaster("DayEnd", "DayEndProcess", null, null, connVM);

                if (DayEndProcess == "N")
                {
                    throw new Exception("Dayend process not allowed");
                }

                #region Preparing Models

                VAT6_1ParamVM VAT6_1Model = new VAT6_1ParamVM();
                string endDate = DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy");

                VAT6_1Model.StartDate = Convert.ToDateTime(VAT6_1ProcessDate).ToString("dd-MMM-yyyy");
                VAT6_1Model.EndDate = endDate;

                VAT6_1Model.Post1 = "Y";
                VAT6_1Model.Post2 = "Y";
                VAT6_1Model.BranchId = 0;
                VAT6_1Model.PreviewOnly = false;
                VAT6_1Model.InEnglish = "N";
                VAT6_1Model.UOMConversion = 1;
                VAT6_1Model.UOM = "";
                VAT6_1Model.UOMTo = "";
                VAT6_1Model.UserName = processParam.CurrentUserName;//
                VAT6_1Model.ReportName = "";
                VAT6_1Model.Opening = false;
                VAT6_1Model.OpeningFromProduct = false;

                VAT6_1Model.IsMonthly = false;
                VAT6_1Model.IsTopSheet = false;
                VAT6_1Model.IsGroupTopSheet = false;
                VAT6_1Model.Is6_1Permanent = true;
                VAT6_1Model.FilterProcessItems = true;

                VAT6_1Model.UserId = processParam.CurrentUserId;//

                VAT6_2ParamVM VAT6_2Model = new VAT6_2ParamVM();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2Model.StartDate = Convert.ToDateTime(VAT6_2ProcessDate).ToString("dd-MMM-yyyy");
                VAT6_2Model.EndDate = endDate;

                VAT6_2Model.Post1 = "Y";
                VAT6_2Model.Post2 = "Y";
                VAT6_2Model.BranchId = 0;
                VAT6_2Model.rbtnService = false;
                VAT6_2Model.rbtnWIP = false;
                VAT6_2Model.UOMTo = "";
                VAT6_2Model.IsBureau = processParam.IsBureau; //
                VAT6_2Model.AutoAdjustment = vAutoAdjustment == "Y";
                VAT6_2Model.PreviewOnly = false;
                VAT6_2Model.InEnglish = "N";
                VAT6_2Model.UOM = "";
                VAT6_2Model.IsMonthly = false;
                VAT6_2Model.IsTopSheet = false;
                VAT6_2Model.FilterProcessItems = true; //
                VAT6_2Model.UserId = processParam.CurrentUserId;//

                VAT6_2ParamVM VAT6_2_1Model = new VAT6_2ParamVM();

                VAT6_2_1Model.StartDate = Convert.ToDateTime(VAT6_2_1ProcessDate).ToString("dd-MMM-yyyy");
                VAT6_2_1Model.EndDate = endDate;

                VAT6_2_1Model.Post1 = "Y";
                VAT6_2_1Model.Post2 = "Y";
                VAT6_2_1Model.BranchId = 0;
                VAT6_2_1Model.rbtnService = false;
                VAT6_2_1Model.rbtnWIP = false;
                VAT6_2_1Model.UOMTo = "";
                VAT6_2_1Model.IsBureau = processParam.IsBureau;
                VAT6_2_1Model.AutoAdjustment = vAutoAdjustment == "Y";
                VAT6_2_1Model.PreviewOnly = false;
                VAT6_2_1Model.InEnglish = "N";
                VAT6_2_1Model.UOM = "";
                VAT6_2_1Model.IsMonthly = false;
                VAT6_2_1Model.IsTopSheet = false;
                VAT6_2_1Model.FilterProcessItems = true; //
                VAT6_2_1Model.UserId = processParam.CurrentUserId;

                #endregion

                IssueDAL issueDal = new IssueDAL();

                AVGPriceVm priceVm = new AVGPriceVm();

                ProductDAL productDal = new ProductDAL();
                DataTable dtProductCount = productDal.GetUprocessCount(null, null, connVM);

                processParam.SetLabel("Avg price is processing");

                priceVm.AvgDateTime = VAT6_1Model.StartDate;
                ResultVM resultVm = issueDal.UpdateAvgPrice_New_Refresh(priceVm, null, null, connVM);

                processParam.SetLabel(dtProductCount.Rows[0][0] + " products of VAT6.1 is processing");

                _vatRegistersDAL.SaveVAT6_1_Permanent(VAT6_1Model, null, null, connVM);
                string[] result = _vatRegistersDAL.SaveVAT6_1_Permanent_Branch(VAT6_1Model, null, null, connVM);

                processParam.SetLabel(dtProductCount.Rows[0][1] + " products of VAT6.2 is processing");

                _vatRegistersDAL.SaveVAT6_2_Permanent(VAT6_2Model, null, null, connVM);
                result = _vatRegistersDAL.SaveVAT6_2_Permanent_Branch(VAT6_2Model, null, null, connVM);

                processParam.SetLabel(dtProductCount.Rows[0][2] + " products of VAT6.2_1 is processing");

                issueDal.SaveVAT6_2_1_Permanent(VAT6_2_1Model, null, null, connVM);
                result = issueDal.SaveVAT6_2_1_Permanent_Branch(VAT6_2_1Model, null, null, connVM);

                ProcessFreshStock(new ParameterVM());

                commonDal.settingsUpdateMaster("DayEnd", "DayEndProcess", "N", null, null, connVM);
                commonDal.settingsUpdateMaster("DayEnd", "VAT6_1ProcessDate", DateTime.Now.ToString("yyy-MM-dd"), null, null, connVM);
                commonDal.settingsUpdateMaster("DayEnd", "VAT6_2ProcessDate", DateTime.Now.ToString("yyy-MM-dd"), null, null, connVM);
                commonDal.settingsUpdateMaster("DayEnd", "VAT6_2_1ProcessDate", DateTime.Now.ToString("yyy-MM-dd"), null, null, connVM);
                commonDal.UpdateProcessFlag();

                resultVm = new ResultVM()
                {
                    Message = "Process Completed Successfully",
                    Status = "success"
                };


                return resultVm;
            }
            catch (Exception e)
            {
                FileLogger.Log("ProductDAL", "Day End Process", e.ToString());
                throw;
            }
        }

        #region Product Mapping

        public string[] InsertToProductChild(ProductMapVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Inser new ProductMapDetails

                ProductVM pvm = new ProductVM();

                pvm = SelectAll(vm.ItemNo, null, null, currConn, transaction, null, connVM).FirstOrDefault();
                pvm.ProductCode = vm.ProductMappingCode;
                pvm.ProductName = vm.ProductDescription;
                pvm.MasterProductItemNo = vm.ItemNo;
                pvm.IsChild = "Y";
                pvm.ProductDescription = vm.ProductDescription;

                retResults = InsertToProduct(pvm, null, null, false, currConn, transaction, connVM);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();

                    }

                }
                #endregion Commit

                #endregion Inser new ProductMapDetails

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "InsertToProductMapDetails", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToProductMapDetails(ProductMapVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Inser new ProductMapDetails

                #region Master chack exit

                sqlText = @"
                select isnull(count(ItemNo),0)ItemNo FROM  Products
where  ProductCode=@ProductCode
                ";
                SqlCommand cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                ////cmdisExitCode.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdisExitCode.Parameters.AddWithValue("@ProductCode", vm.ProductMappingCode);

                int isMasterExit = (int)cmdisExitCode.ExecuteScalar();

                if (isMasterExit != 0)
                {
                    throw new ArgumentNullException("", "Your Requested Product Code Already Exit in master table");
                }

                #endregion Master chack exit

                #region chack exit

                sqlText = @"
                select isnull(count(ItemNo),0)ItemNo FROM  ProductMapDetails
where  ProductMappingCode=@ProductMappingCode
                ";
                cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                ////cmdisExitCode.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdisExitCode.Parameters.AddWithValue("@ProductMappingCode", vm.ProductMappingCode);

                int isExit = (int)cmdisExitCode.ExecuteScalar();

                #endregion chack exit

                if (isExit == 0)
                {

                    #region sqlText

                    sqlText = "";
                    sqlText += "insert into ProductMapDetails";
                    sqlText += "(";
                    sqlText += "ItemNo";
                    sqlText += ",ProductMappingCode";
                    sqlText += ",ProductDescription";
                    sqlText += ",ProductCode";
                    sqlText += ")";
                    sqlText += " values(";

                    sqlText += "  @ItemNo";
                    sqlText += " ,@ProductMappingCode";
                    sqlText += " ,@ProductDescription";
                    sqlText += " ,@ProductCode";

                    sqlText += ")SELECT SCOPE_IDENTITY()";

                    #endregion sqlText

                    #region SqlCommand

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    cmdInsert.Parameters.AddWithValue("@ProductMappingCode", vm.ProductMappingCode);
                    cmdInsert.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription);
                    cmdInsert.Parameters.AddWithValue("@ProductCode", vm.ProductCode);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    #endregion SqlCommand

                    #region transResult

                    if (transResult > 0)
                    {
                        retResults[0] = "Success";
                        retResults[1] = " Your Requested successfully Added";
                        retResults[2] = transResult.ToString();
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

                        }

                    }
                    #endregion Commit

                    #endregion Commit

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = " Your Requested Product Mapping Code Already Exit";
                    retResults[2] = "";
                }

                #endregion Inser new ProductMapDetails

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "InsertToProductMapDetails", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateToProductMapDetails(ProductMapVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                if (string.IsNullOrEmpty(vm.SL.ToString()))
                {
                    throw new ArgumentNullException("UpdateToProductMapDetails", "Invalid Product Mapping Code");
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

                #region Master chack exit

                sqlText = @"
                select isnull(count(ItemNo),0)ItemNo FROM  Products
where  ProductCode=@ProductCode and ItemNo!=@ItemNo
                ";
                SqlCommand cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                cmdisExitCode.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdisExitCode.Parameters.AddWithValue("@ProductCode", vm.ProductMappingCode);

                int isMasterExit = (int)cmdisExitCode.ExecuteScalar();

                if (isMasterExit != 0)
                {
                    throw new ArgumentNullException("", "Your Requested Product Code Already Exit in master table");
                }

                #endregion Master chack exit

                #region chack exit

                sqlText = @"
                select isnull(count(ItemNo),0)ItemNo FROM  ProductMapDetails
where  ProductMappingCode=@ProductCode and ItemNo!=@ItemNo
                ";
                cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                cmdisExitCode.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdisExitCode.Parameters.AddWithValue("@ProductCode", vm.ProductMappingCode);

                int isExit = (int)cmdisExitCode.ExecuteScalar();

                if (isExit != 0)
                {
                    throw new ArgumentNullException("", "Your Requested Product Code Already Exit");
                }

                #endregion Master chack exit

                #region Update new Product Map Details

                sqlText = "";
                sqlText = "update ProductMapDetails set";

                sqlText += " ProductMappingCode=@ProductMappingCode";
                sqlText += " ,ProductDescription=@ProductDescription";
                sqlText += " where SL=@SL";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@ProductMappingCode", vm.ProductMappingCode);
                cmdUpdate.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription);
                cmdUpdate.Parameters.AddWithValue("@SL", vm.SL);
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
                            retResults[1] = "Requested Product successfully Update";
                            retResults[2] = vm.SL.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            if (Vtransaction == null && transaction != null)
                            {
                                transaction.Rollback();
                            }
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update Product Mapping ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update Product Mapping";
                    }
                }

                #endregion Commit

                #endregion

            }

            #endregion

            #region Catch

            catch (Exception ex)
            {

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("ProductDAL", "UpdateToProductMapDetails", ex.ToString() + "\n" + sqlText);

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

        public string[] DeleteProductMapDetails(ProductMapVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.SL.ToString();
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

                if (string.IsNullOrEmpty(vm.SL.ToString()))
                {
                    throw new ArgumentNullException("DeleteIItem", "Could not find requested Item.");
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

                if (!string.IsNullOrEmpty(vm.SL.ToString()))
                {
                    sqlText = "delete ProductMapDetails where SL=@SL";
                }

                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                cmdDelete.Parameters.AddWithValue("@SL", vm.SL);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Product Mapping Code successfully deleted";
                }

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

            #region Catch

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("ProductDAL", "DeleteProductMapDetails", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        public DataTable SearchProductMapDetails(ProductMapVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
select
SL
,ItemNo
,ProductMappingCode
,ProductDescription
,ProductCode
FROM  ProductMapDetails
where 1=1 

";
                if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  order by  SL";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);

                #endregion

            }
            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("ProductDAL", "SearchProductMapDetails", ex.ToString() + "\n" + sqlText);

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

        public DataTable SearchChildProducts(ProductMapVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            DataTable dataTable = new DataTable("ChildProducts");

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
select
ItemNo 
,ProductCode
,ProductName
FROM  Products
where 1=1  

";
                if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and MasterProductItemNo=@MasterProductItemNo";
                }

                sqlText += @"  order by  ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@MasterProductItemNo", vm.ItemNo);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);

                #endregion

            }

            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("ProductDAL", "SearchChildProducts", ex.ToString() + "\n" + sqlText);

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

        public List<ProductVM> SelectAllWithProductMapping(string ItemNo = "0", string productCode = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null, DataTable dtPCodes = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string count = "100";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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

                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }


                sqlText += @"

Pr.ItemNo
,Pr.ProductCode
,Pr.ProductName
,Pr.ProductDescription
,Pr.CategoryID
,Pr.UOM
,isnull(Pr.CostPrice,0) CostPrice
,isnull(Pr.SalesPrice,0) SalesPrice
,isnull(Pr.NBRPrice,0) NBRPrice
,isnull(Pr.ReceivePrice,0) ReceivePrice
,isnull(Pr.IssuePrice,0) IssuePrice
,isnull(Pr.TenderPrice,0) TenderPrice
,isnull(Pr.ExportPrice,0) ExportPrice
,isnull(Pr.InternalIssuePrice,0) InternalIssuePrice
,isnull(Pr.TollIssuePrice,0) TollIssuePrice
,isnull(Pr.TollCharge,0) TollCharge
,isnull(isnull(Pr.OpeningBalance,0)+isnull(Pr.QuantityInHand,0),0) Stock
,Pr.OpeningBalance
,Pr.SerialNo
,Pr.HSCodeNo
,isnull(Pr.VATRate,0) VATRate
,Pr.Comments
,isnull(Pr.SD,0) SD
,isnull(Pr.PacketPrice,0) PacketPrice
,Pr.Trading
,isnull(Pr.TradingMarkUp,0) TradingMarkUp
,Pr.NonStock
,isnull(Pr.QuantityInHand,0) QuantityInHand
,Pr.OpeningDate
,isnull(Pr.RebatePercent,0) RebatePercent
,isnull(Pr.TVBRate,0) TVBRate
,isnull(Pr.CnFRate,0) CnFRate
,isnull(Pr.InsuranceRate,0) InsuranceRate
,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.AITRate,0) AITRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.UOM2,Pr.UOM) UOM2
,isnull(Pr.UOMConvertion,1) UOMConvertion
,Pr.ActiveStatus
,Pr.CreatedBy
,Pr.CreatedOn
,Pr.LastModifiedBy
,Pr.LastModifiedOn
,isnull(Pr.OpeningTotalCost,0) OpeningTotalCost

,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.VATRate2,0) VATRate2
,isnull(Pr.TradingSaleVATRate,0) TradingSaleVATRate
,isnull(Pr.TradingSaleSD,0) TradingSaleSD
,isnull(Pr.VDSRate,0) VDSRate
,isnull(Pr.IsFixedVAT,'N') IsFixedVAT
,isnull(Pr.IsFixedSD,'N') IsFixedSD 
,isnull(Pr.IsFixedCD,'N') IsFixedCD 
,isnull(Pr.IsFixedRD,'N') IsFixedRD 
,isnull(Pr.IsFixedAIT,'N') IsFixedAIT
,isnull(Pr.IsFixedVAT1,'N') IsFixedVAT1
,isnull(Pr.IsFixedAT,'N') IsFixedAT 
,isnull(Pr.IsFixedOtherSD,'N') IsFixedOtherSD 
,isnull(Pr.FixedVATAmount,0) FixedVATAmount
,isnull(Pr.IsHouseRent,'N') IsHouseRent
,isnull(Pr.ShortName,'-') ShortName

,isnull(Pr.Banderol,'N')Banderol
,isnull(Pr.TollProduct,'N')TollProduct
,Pc.CategoryName
,Pc.IsRaw
,isnull(Pr.IsExempted,'N')IsExempted
,isnull(Pr.IsZeroVAT,'N')IsZeroVAT
,isnull(Pr.IsTransport,'N')IsTransport
,isnull(Pr.BranchId,'0')BranchId
,isnull(Pr.TDSCode,'-')TDSCode
,isnull(Pr.AITRate,'0')AITRate
,isnull(Pr.SDRate,'0')SDRate
,isnull(Pr.VATRate3,'0')VATRate3
,isnull(pr.IsVDS,'Y')IsVDS
,isnull(pr.HPSRate,'0')HPSRate
,isnull(pr.GenericName,'-')GenericName
,isnull(pr.DARNo,'-')DARNo
,isnull(pr.IsConfirmed,'Y')IsConfirmed
,isnull(pr.IsSample,'N')IsSample
,isnull(pr.ReportType,'-')ReportType
,isnull(pr.TransactionHoldDate,'0')TransactionHoldDate
,isnull(pr.IsFixedVATRebate,'N')IsFixedVATRebate

--,isnull(Pr.WastageTotalQuantity,0) WastageTotalQuantity
--,isnull(Pr.WastageTotalValue,0) WastageTotalValue


FROM Products Pr left outer join ProductCategories Pc on Pr.CategoryID=Pc.CategoryID
left outer join ProductMapDetails pmd on pmd.ItemNo=pr.ItemNo

WHERE  1=1 AND Pr.IsArchive = 0
";


                if (ItemNo != "0")
                {
                    sqlText += @" and Pr.ItemNo=@ItemNo";
                }
                if (productCode != "0")
                {
                    sqlText += @" and (pr.ProductCode=@ProductCode or pmd.ProductMappingCode=@ProductCode)";
                }


                //, DataTable dtPCodes = null
                if (dtPCodes != null && dtPCodes.Rows.Count > 0)
                {
                    sqlText += @" and pr.ItemNo in (";

                    foreach (DataRow row in dtPCodes.Rows)
                    {
                        sqlText += "'" + row["Code"].ToString() + "',";
                    }

                    sqlText = sqlText.TrimEnd(',');
                    sqlText += @")";

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

                        else if (conditionFields[i].ToLower().Contains("isnull"))
                        {
                            sqlText += " AND " + "isnull(" + cField + ",'Y')=" + " @" + cField + "";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else if (conditionFields[i].ToLower().Contains("!="))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.ProductName))
                    {
                        sqlText += " AND Pr.ProductName like @ProductName ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.ProductCode))
                    {
                        sqlText += " AND Pr.ProductCode like @ProductCode ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.HSCodeNo))
                    {
                        sqlText += " AND Pr.HSCodeNo like @HSCodeNo ";
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        sqlText += " AND Pr.SerialNo like @SerialNo ";
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
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
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.ProductName))
                    {
                        objComm.Parameters.AddWithValue("@ProductName", "%" + likeVM.ProductName + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.ProductCode))
                    {
                        objComm.Parameters.AddWithValue("@ProductCode", "%" + likeVM.ProductCode + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.HSCodeNo))
                    {
                        objComm.Parameters.AddWithValue("@HSCodeNo", "%" + likeVM.HSCodeNo + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        objComm.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
                    }
                }
                if (ItemNo != "0")
                {
                    objComm.Parameters.AddWithValue("@ItemNo", ItemNo.ToString());
                }
                if (productCode != "0")
                {
                    objComm.Parameters.AddWithValue("@ProductCode", productCode.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();

                    var FixedSD = dr["IsFixedSD"].ToString();
                    var FixedCD = dr["IsFixedCD"].ToString();
                    var FixedRD = dr["IsFixedRD"].ToString();
                    var FixedAIT = dr["IsFixedAIT"].ToString();
                    var FixedVAT1 = dr["IsFixedVAT1"].ToString();
                    var FixedAT = dr["IsFixedAT"].ToString();

                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.UOM2 = dr["UOM2"].ToString();
                    vm.UOMConversion = Convert.ToDecimal(dr["UOMConvertion"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.Trading = dr["Trading"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.OpeningDate = dr["OpeningDate"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                    vm.Banderol = dr["Banderol"].ToString();

                    vm.CDRate = Convert.ToDecimal(dr["CDRate"].ToString());
                    vm.RDRate = Convert.ToDecimal(dr["RDRate"].ToString());
                    vm.TVARate = Convert.ToDecimal(dr["TVARate"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString());
                    vm.VATRate2 = Convert.ToDecimal(dr["VATRate2"].ToString());
                    vm.TradingSaleVATRate = Convert.ToDecimal(dr["TradingSaleVATRate"].ToString());
                    vm.TradingSaleSD = Convert.ToDecimal(dr["TradingSaleSD"].ToString());

                    vm.VDSRate = Convert.ToDecimal(dr["VDSRate"].ToString());
                    vm.CnFRate = Convert.ToDecimal(dr["CnFRate"].ToString());

                    vm.AITRate = Convert.ToDecimal(dr["AITRate"].ToString());
                    vm.VATRate3 = Convert.ToDecimal(dr["VATRate3"].ToString());
                    vm.SDRate = Convert.ToDecimal(dr["SDRate"].ToString());


                    vm.TollProduct = dr["TollProduct"].ToString();
                    vm.Stock = Convert.ToDecimal(dr["Stock"].ToString());
                    vm.ProductType = dr["IsRaw"].ToString();
                    vm.IsRaw = dr["IsRaw"].ToString();

                    vm.IsZeroVAT = dr["IsZeroVAT"].ToString();
                    vm.IsExempted = dr["IsExempted"].ToString();
                    vm.IsTransport = dr["IsTransport"].ToString();
                    vm.TDSCode = dr["TDSCode"].ToString();

                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                    vm.IsFixedSD = dr["IsFixedSD"].ToString();
                    vm.IsFixedCD = dr["IsFixedCD"].ToString();
                    vm.IsFixedRD = dr["IsFixedRD"].ToString();
                    vm.IsFixedAIT = dr["IsFixedAIT"].ToString();
                    vm.IsFixedVAT1 = dr["IsFixedVAT1"].ToString();
                    vm.IsFixedAT = dr["IsFixedAT"].ToString();
                    vm.IsFixedOtherSD = dr["IsFixedOtherSD"].ToString();
                    vm.IsHouseRent = dr["IsHouseRent"].ToString();
                    vm.ShortName = dr["ShortName"].ToString();
                    vm.IsConfirmed = dr["IsConfirmed"].ToString();
                    vm.IsSample = dr["IsSample"].ToString();
                    vm.IsVDS = dr["IsVDS"].ToString();
                    vm.HPSRate = Convert.ToDecimal(dr["HPSRate"]);
                    vm.TransactionHoldDate = Convert.ToDecimal(dr["TransactionHoldDate"]);
                    vm.IsFixedVATRebate = dr["IsFixedVATRebate"].ToString();
                    vm.Packetprice = Convert.ToDecimal(dr["Packetprice"]);

                    ////vm.WastageTotalQuantity = Convert.ToDecimal(dr["WastageTotalQuantity"]);
                    ////vm.WastageTotalValue = Convert.ToDecimal(dr["WastageTotalValue"]);

                    vm.IsFixedSDChecked = FixedSD == "Y";
                    vm.IsFixedCDChecked = FixedCD == "Y";
                    vm.IsFixedRDChecked = FixedRD == "Y";
                    vm.IsFixedAITChecked = FixedAIT == "Y";
                    vm.IsFixedVAT1Checked = FixedVAT1 == "Y";
                    vm.IsFixedATChecked = FixedAT == "Y";

                    vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());

                    vm.GenericName = dr["GenericName"].ToString();
                    vm.DARNo = dr["DARNo"].ToString();
                    vm.ReportType = dr["ReportType"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        #region Product Price History

        public ResultVM InsertToProductPriceHistory(ProductPriceHistoryVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            ResultVM _rVM = new ResultVM();

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

                #region Inser new ProductPriceHistorys

                #region chack exit

                sqlText = @"
                select isnull(count(ItemNo),0)ItemNo FROM  Products where  ItemNo=@ItemNo
                ";
                SqlCommand cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                ////cmdisExitCode.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdisExitCode.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                int isMasterExit = (int)cmdisExitCode.ExecuteScalar();

                if (isMasterExit == 0)
                {
                    throw new ArgumentNullException("", "Your Requested Product not exit");
                }

                #endregion chack exit

                #region sqlText

                sqlText = "";
                sqlText += @"
insert into ProductPriceHistorys(
ItemNo
,ProductCode
,EffectDate
,VatablePrice
,CreatedBy
,CreatedOn
)
values
(
 @ItemNo
,@ProductCode
,@EffectDate
,@VatablePrice
,@CreatedBy
,@CreatedOn
)SELECT SCOPE_IDENTITY()
";


                #endregion sqlText

                #region SqlCommand

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdInsert.Parameters.AddWithValue("@ProductCode", vm.ProductCode);
                cmdInsert.Parameters.AddWithValue("@EffectDate", vm.EffectDate);
                cmdInsert.Parameters.AddWithValue("@VatablePrice", vm.VatablePrice);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion SqlCommand

                #region transResult

                if (transResult > 0)
                {
                    _rVM.Status = "Success";
                    _rVM.Message = " Your Request successfully Added";
                    _rVM.Id = transResult.ToString();
                }

                #endregion transResult

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                }
                #endregion Commit

                #endregion Inser new ProductPriceHistorys

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                _rVM.Status = "Fail";
                _rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ProductDAL", "InsertToProductPriceHistory", ex.ToString() + "\n" + sqlText, "ProductPriceHistory");

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

            return _rVM;

        }

        public ResultVM UpdateToProductPriceHistory(ProductPriceHistoryVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            ResultVM _rVM = new ResultVM();

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

                #region chack exit

                sqlText = @"
                select isnull(count(Id),0)ItemNo FROM  ProductPriceHistorys where  Id=@Id
                ";
                SqlCommand cmdisExitCode = new SqlCommand(sqlText, currConn);
                cmdisExitCode.Transaction = transaction;
                cmdisExitCode.Parameters.AddWithValue("@Id", vm.Id);

                int isExit = (int)cmdisExitCode.ExecuteScalar();

                if (isExit != 0)
                {
                    throw new ArgumentNullException("", "Your Requested information not exit");
                }

                #endregion Master chack exit

                #region Update new ProductPriceHistorys

                sqlText = "";
                sqlText = @"update ProductPriceHistorys set
ItemNo=@ItemNo
,ProductCode=@ProductCode
,EffectDate=@EffectDate
,VatablePrice=@VatablePrice
,LastModifiedBy=@LastModifiedBy
,LastModifiedOn=@LastModifiedOn
where Id=@Id
";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdUpdate.Parameters.AddWithValue("@ProductCode", vm.ProductCode);
                cmdUpdate.Parameters.AddWithValue("@EffectDate", vm.EffectDate);
                cmdUpdate.Parameters.AddWithValue("@VatablePrice", vm.VatablePrice);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();

                        _rVM.Status = "Success";
                        _rVM.Message = " Your Request successfully Update";
                        _rVM.Id = vm.Id.ToString();

                    }

                }

                #endregion Commit

                #endregion

            }

            #endregion

            #region Catch

            catch (Exception ex)
            {
                _rVM.Status = "Fail";
                _rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("ProductDAL", "UpdateToProductPriceHistory", ex.ToString() + "\n" + sqlText, "ProductPriceHistory");

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

            return _rVM;
        }

        public ResultVM DeleteProductPriceHistory(ProductPriceHistoryVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables
            ResultVM _rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion Variables

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

                if (!string.IsNullOrEmpty(vm.Id.ToString()))
                {
                    sqlText = "delete ProductPriceHistorys where Id=@Id";
                }

                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                cmdDelete.Parameters.AddWithValue("@Id", vm.Id);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    _rVM.Status = "Success";
                    _rVM.Message = " Your Request successfully deleted";

                }

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

            #region Catch

            catch (Exception ex)
            {
                _rVM.Status = "Fail";
                _rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("ProductDAL", "DeleteProductPriceHistory", ex.ToString() + "\n" + sqlText, "ProductPriceHistory");

                throw new ArgumentNullException("", ex.Message.ToString());

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

            return _rVM;
        }

        public DataTable SearchProductPriceHistory(ProductPriceHistoryVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
select
Id
,ItemNo
,ProductCode
,EffectDate
,VatablePrice
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
FROM  ProductPriceHistorys
where 1=1 

";
                if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  order by  Id";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);

                #endregion

            }
            #region catch

            catch (Exception ex)
            {

                FileLogger.Log("ProductDAL", "SearchProductPriceHistory", ex.ToString() + "\n" + sqlText, "ProductPriceHistory");

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

        public List<ProductPriceHistoryVM> SearchProductPriceHistoryList(string ItemNo = "0", string productCode = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null, SysDBInfoVMTemp connVM = null, DataTable dtPCodes = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            List<ProductPriceHistoryVM> VMs = new List<ProductPriceHistoryVM>();
            ProductPriceHistoryVM vm;
            #endregion
            try
            {

            }
            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("ProductDAL", "SearchProductPriceHistoryList", ex.ToString(), "ProductPriceHistory");

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

        public DataTable GetExcelProductPriceHistory(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductPriceHistory");

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

                var len = ids.Count;

                #region Sql Command

                sqlText = @"
select 
 pc.CategoryName ProductGroup
,p.ProductCode
,p.ProductName
,isnull(pph.EffectDate,'1900-01-01')EffectDate
,isnull(pph.VatablePrice,'0')VatablePrice
from Products p
left outer join ProductPriceHistorys pph on pph.ItemNo=p.ItemNo
left outer join ProductCategories pc on  p.CategoryID=pc.CategoryID
where 1=1 
and p.ItemNo in(@AllItem)
";
                sqlText += @" order by pc.IsRaw,p.ProductCode,p.ProductName";

                string AllItem = "";

                for (int i = 0; i < len; i++)
                {
                    AllItem += "'" + ids[i] + "',";
                }

                AllItem = AllItem.TrimEnd(',') + "";

                sqlText = sqlText.Replace("@AllItem", AllItem);
                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;

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

                FileLogger.Log("ProductDAL", "GetExcelProductPriceHistory", ex.ToString() + "\n" + sqlText, "ProductPriceHistory");

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


        #endregion
    }
}
