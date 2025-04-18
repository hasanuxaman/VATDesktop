﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;

namespace VATServer.Library
{
    public class ProductCategoryDAL : IProductCategory
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        #region Old Methods


//        public DataTable SearchProductCategory(string CategoryID, string CategoryName, string IsRaw, string HSCodeNo, decimal VATRateFrom, decimal VATRateTo, string ActiveStatus, decimal SD, string Trading, string NonStock, string databaseName)
//        {
//            SqlConnection currConn = null;
//            int transResult = 0;
//            int countId = 0;
//            string sqlText = "";

//            DataTable dataTable = new DataTable("ProductCategory");

//            try
//            {
//                #region open connection and transaction

//                currConn = _dbsqlConnection.GetConnection(connVM);
//                if (currConn.State != ConnectionState.Open)
//                {
//                    currConn.Open();
//                }

//                #endregion open connection and transaction
//                sqlText = @"
//SELECT 
//PC.CategoryID,
//isnull(PC.CategoryName,'N/A')CategoryName,
//isnull(PC.Description,'N/A')Description,
//isnull(PC.Comments,'N/A')Comments,
//isnull(PC.IsRaw,'R')IsRaw,
//isnull(PC.HSCodeNo,'N/A')HSCodeNo,
//isnull(PC.VATRate,0)VATRate,
//isnull(PC.PropergatingRate,'N')PropergatingRate,
//isnull(PC.ActiveStatus,'N')ActiveStatus,
//'N/A' HSDescription,
//isnull(PC.SD,0)SD,
//PC.Trading,pc.NonStock
//
//FROM         dbo.ProductCategories as PC 
//                 
//WHERE 
//    (CategoryID  LIKE '%' +  @CategoryID  + '%' OR @CategoryID IS NULL) 
//AND (CategoryName LIKE '%' + @CategoryName + '%' OR @CategoryName IS NULL)
//AND (IsRaw LIKE '%' + @IsRaw + '%' OR @IsRaw IS NULL) 
//AND (PC.HSCodeNo LIKE '%' + @HSCodeNo + '%' OR @HSCodeNo IS NULL)
//AND (PC.VATRate >= @VATRateFrom  OR @VATRateFrom =0) 
//AND (PC.VATRate <= @VATRateTo OR @VATRateTo =0) 
//AND (PC.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
//AND (PC.SD >= @SD OR @SD =0)
//AND (PC.Trading LIKE '%' + @Trading + '%' OR @Trading IS NULL)
//AND (PC.NonStock LIKE '%' + @NonStock + '%' OR @NonStock IS NULL)
//order by CategoryName
//
//";

//                SqlCommand objCommBankInformation = new SqlCommand();
//                objCommBankInformation.Connection = currConn;
//                objCommBankInformation.CommandText = sqlText;
//                objCommBankInformation.CommandType = CommandType.Text;

//                if (!objCommBankInformation.Parameters.Contains("@CategoryID"))
//                { objCommBankInformation.Parameters.AddWithValue("@CategoryID", CategoryID); }
//                else { objCommBankInformation.Parameters["@CategoryID"].Value = CategoryID; }
//                if (!objCommBankInformation.Parameters.Contains("@CategoryName"))
//                { objCommBankInformation.Parameters.AddWithValue("@CategoryName", CategoryName); }
//                else { objCommBankInformation.Parameters["@CategoryName"].Value = CategoryName; }
//                if (!objCommBankInformation.Parameters.Contains("@IsRaw"))
//                { objCommBankInformation.Parameters.AddWithValue("@IsRaw", IsRaw); }
//                else { objCommBankInformation.Parameters["@IsRaw"].Value = IsRaw; }
//                if (!objCommBankInformation.Parameters.Contains("@HSCodeNo"))
//                { objCommBankInformation.Parameters.AddWithValue("@HSCodeNo", HSCodeNo); }
//                else { objCommBankInformation.Parameters["@HSCodeNo"].Value = HSCodeNo; }
//                if (!objCommBankInformation.Parameters.Contains("@VATRateFrom"))
//                { objCommBankInformation.Parameters.AddWithValue("@VATRateFrom", VATRateFrom); }
//                else { objCommBankInformation.Parameters["@VATRateFrom"].Value = VATRateFrom; }
//                if (!objCommBankInformation.Parameters.Contains("@VATRateTo"))
//                { objCommBankInformation.Parameters.AddWithValue("@VATRateTo", VATRateTo); }
//                else { objCommBankInformation.Parameters["@VATRateTo"].Value = VATRateTo; }
//                if (!objCommBankInformation.Parameters.Contains("@ActiveStatus"))
//                { objCommBankInformation.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
//                else { objCommBankInformation.Parameters["@ActiveStatus"].Value = ActiveStatus; }
//                // Common Filed
//                if (!objCommBankInformation.Parameters.Contains("@SD"))
//                { objCommBankInformation.Parameters.AddWithValue("@SD", SD); }
//                else { objCommBankInformation.Parameters["@SD"].Value = SD; }
//                if (!objCommBankInformation.Parameters.Contains("@Trading"))
//                { objCommBankInformation.Parameters.AddWithValue("@Trading", Trading); }
//                else { objCommBankInformation.Parameters["@Trading"].Value = Trading; }
//                if (!objCommBankInformation.Parameters.Contains("@NonStock"))
//                { objCommBankInformation.Parameters.AddWithValue("@NonStock", NonStock); }
//                else { objCommBankInformation.Parameters["@NonStock"].Value = NonStock; }


//                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);

//                dataAdapter.Fill(dataTable);


//            }
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

//                //throw sqlex;
//            }
//            catch (Exception ex)
//            {

//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

//                //throw ex;
//            }
//            finally
//            {
//                if (currConn != null)
//                {
//                    if (currConn.State == ConnectionState.Open)
//                    {

//                        currConn.Close();

//                    }
//                }

//            }

//            return dataTable;



//        }




        #endregion

        #region web methods

        public string[] InsertToProductCategory(ProductCategoryVM vm,SysDBInfoVMTemp ConnVm =null)
        {

            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id

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

                if (string.IsNullOrEmpty(vm.CategoryName))
                {
                    throw new ArgumentNullException(this.GetType().Name,
                                                    "Please enter category name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(ConnVm);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToCategoryInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.CategoryID))
                {
                    sqlText = "select count(CategoryID) from ProductCategories where  CategoryID=@CategoryID";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@CategoryID", vm.CategoryID);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToCategoryInformation", "Category information already used!");
                    }


                }

                #region Insert Category Information

                sqlText = "select count(distinct CategoryName) from ProductCategories where  CategoryName=@CategoryName";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValueAndNullHandle("@CategoryName", vm.CategoryName);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {

                    throw new ArgumentNullException("InsertToCategoryInformation",
                                                    "Requested Category Name  is already exist");


                }

                sqlText = "select isnull(max(cast(CategoryID as int)),0)+1 FROM  ProductCategories";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Category information Id";
                    throw new ArgumentNullException("InsertToCategoryInformation",
                                                    "Unable to create new Category information Id");
                }
                vm.CategoryID = nextId.ToString();

                sqlText = "";

                sqlText += @" 
INSERT INTO ProductCategories(
 CategoryID
,CategoryName
,Description
,Comments
,ReportType
,IsRaw
,HSCodeNo
,VATRate
,PropergatingRate
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,SD
,Trading
,NonStock
,IsArchive
) VALUES (
@CategoryID
,@CategoryName
,@Description
,@Comments
,@ReportType
,@IsRaw
,@HSCodeNo
,@VATRate
,@PropergatingRate
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@SD
,@Trading
,@NonStock  
,@IsArchive           
) 
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CategoryID", vm.CategoryID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CategoryName", vm.CategoryName.Trim());
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Description", vm.Description);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", vm.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReportType", vm.ReportType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsRaw", vm.IsRaw);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@HSCodeNo", vm.HSCodeNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate", vm.VATRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PropergatingRate", vm.PropergatingRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SD", vm.SD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Trading", vm.Trading);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NonStock", vm.NonStock);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsArchive",false);
                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Insert Category Information


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
                retResults[1] = "Requested Category Information successfully added";
                retResults[2] = "" + nextId;

            }

            #endregion

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = vm.CategoryID; //catch ex

                transaction.Rollback();

                FileLogger.Log("ProductCategoryDAL", "InsertToProductCategory", ex.ToString() + "\n" + sqlText);

                //FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public string[] UpdateProductCategory(ProductCategoryVM vm, SysDBInfoVMTemp connVM = null)
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

                if (vm.CategoryID.Trim() == "0")
                {
                    retResults[0] = "Fail";
                    retResults[1] = "This product category information unable to update!";
                    retResults[2] = vm.CategoryID;

                    return retResults;
                }

                if (string.IsNullOrEmpty(vm.CategoryName))
                {
                    throw new ArgumentNullException("InsertToCategoryInformation",
                                                    "Please enter category name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToCategoryInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.CategoryID))
                {
                    sqlText = "select count(CategoryID) from ProductCategories where  CategoryID=@CategoryID";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("CategoryID", vm.CategoryID);
                    cmdIdExist.Transaction = transaction;
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToCategoryInformation", "Could not find requested Category information ");
                    }

                    #region Exist Check
                    //bool exist = false;
                    //CommonDAL _cDAL = new CommonDAL();
                    //string[] cFields = { "CategoryName", "CategoryID!" };
                    //string[] cValues = { vm.CategoryName, vm.CategoryID };
                    //exist = _cDAL.ExistCheck("ProductCategories", "CategoryID", cFields, cValues, currConn, transaction);
                    //if (exist)
                    //{
                    //    throw new ArgumentNullException("InsertToCategoryInformation", "Category information already used!");
                    //}
                    #endregion

                }

                #region Update Category Information



                sqlText = "";
                sqlText += "UPDATE  ProductCategories SET";
                sqlText += " CategoryName       =@CategoryName";
                sqlText += ", Description       =@Description";
                sqlText += ", Comments          =@Comments";
                sqlText += ", ReportType        =@ReportType";
                sqlText += ", IsRaw             =@IsRaw";
                sqlText += ", HSCodeNo          =@HSCodeNo";
                sqlText += ", VATRate           =@VATRate";
                sqlText += ", PropergatingRate  =@PropergatingRate";
                sqlText += ", ActiveStatus      =@ActiveStatus";
                sqlText += ", LastModifiedBy    =@LastModifiedBy";
                sqlText += ", LastModifiedOn    =@LastModifiedOn";
                sqlText += ", SD                =@SD";
                sqlText += ", Trading           =@Trading";
                sqlText += ", NonStock          =@NonStock";
                sqlText += " WHERE CategoryID   =@CategoryID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CategoryID", vm.CategoryID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CategoryName", vm.CategoryName.Trim());
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Description", vm.Description);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", vm.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReportType", vm.ReportType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsRaw", vm.IsRaw);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@HSCodeNo", vm.HSCodeNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate", vm.VATRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PropergatingRate", vm.PropergatingRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SD", vm.SD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Trading", vm.Trading);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NonStock", vm.NonStock);

                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Update Category Information

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
                retResults[1] = "Requested Category Information successfully updated";
                retResults[2] = "" + vm.CategoryID;

            }

            #endregion

            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = vm.CategoryID; //catch ex

                transaction.Rollback();

                FileLogger.Log("ProductCategoryDAL", "UpdateProductCategory", ex.ToString() + "\n" + sqlText);

                //FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

        public List<ProductCategoryVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductCategoryVM> VMs = new List<ProductCategoryVM>();
            ProductCategoryVM vm;
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
'B' Sl, CategoryID
, CategoryName
FROM ProductCategories
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Product' CategoryName  
FROM ProductCategories
)
AS a
order by Sl,CategoryName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductCategoryVM();
                    vm.CategoryID = dr["CategoryID"].ToString(); ;
                    vm.CategoryName = dr["CategoryName"].ToString();
                    //vm.Code = dr["Code"].ToString();
                    //vm.Name = vm.Name + " ( " + vm.Code + " )";
                    //vm.Code = vm.Code + "~" + vm.Id;
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductCategoryDAL", "DropDownAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductCategoryDAL", "DropDownAll", ex.ToString() + "\n" + sqlText);

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

        public List<ProductCategoryVM> DropDown(string IsRaw, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductCategoryVM> VMs = new List<ProductCategoryVM>();
            ProductCategoryVM vm;
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
pc.CategoryID
,pc.CategoryName
,pc.IsRaw
FROM ProductCategories pc
WHERE  1=1 AND pc.ActiveStatus = 'Y'
";

                if (!string.IsNullOrEmpty(IsRaw))
                {
                    sqlText += @" AND pc.IsRaw = @IsRaw";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (!string.IsNullOrEmpty(IsRaw))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@IsRaw", IsRaw);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductCategoryVM();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    //vm.Code = dr["Code"].ToString();
                    //vm.Name = vm.Name + " ( " + vm.Code + " )";
                    //vm.Code = vm.Code + "~" + vm.Id;
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductCategoryDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductCategoryDAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public List<IdName> DropDownProductType(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<IdName> VMs = new List<IdName>();
            IdName vm;
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
select distinct IsRaw from ProductCategories";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new IdName();
                    vm.Id = dr["IsRaw"].ToString();
                    vm.Name = dr["IsRaw"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("ProductCategoryDAL", "DropDownProductType", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductCategoryDAL", "DropDownProductType", ex.ToString() + "\n" + sqlText);

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

        public List<ProductCategoryVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductCategoryVM> VMs = new List<ProductCategoryVM>();
            ProductCategoryVM vm;
            #endregion
            try
            {
                #region sql statement
                
                #region SqlExecution
                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, false,"",connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new ProductCategoryVM();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.IsRaw = dr["IsRaw"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"]);
                    vm.PropergatingRate = dr["PropergatingRate"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"]);
                    vm.Trading = dr["Trading"].ToString();
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
                    vm.ReportType = dr["ReportType"].ToString();


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
                FileLogger.Log("ProductCategoryDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductCategoryDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, string DbName = "", SysDBInfoVMTemp connVM = null)
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
                            if (!string.IsNullOrWhiteSpace(DbName))
                            {
                                    currConn.ChangeDatabase(DbName);
                            }
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
 CategoryID
,CategoryName
,Description
,Comments
,ReportType
,IsRaw
,HSCodeNo
,VATRate
,PropergatingRate
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,SD
,Trading
,NonStock
,Info4
,Info5


FROM ProductCategories  
WHERE  1=1 AND ActiveStatus = 'Y' and isnull(IsArchive,0) = 0 

";


                if (Id > 0)
                {
                    sqlText += @" and CategoryID=@CategoryID";
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

                AddValueToAdapter(da, conditionValues, conditionFields);

                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValueAndNullHandle("@CategoryID", Id);
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
                FileLogger.Log("ProductCategoryDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductCategoryDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        public void AddValueToAdapter(SqlDataAdapter adapter, string[] conditionValues, string[] conditionFields, SysDBInfoVMTemp connVM = null)
        {

            string cField;
            if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
            {
                for (int j = 0; j < conditionFields.Length; j++)
                {
                    if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) ||
                        conditionValues[j] == "0")
                    {
                        continue;
                    }

                    cField = conditionFields[j].ToString();
                    cField = OrdinaryVATDesktop.StringReplacing(cField);
                    if (conditionFields[j].ToLower().Contains("like"))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(),
                            conditionValues[j]);
                    }
                    else
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
            }
        }

        public string[] Delete(ProductCategoryVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteProductCategory"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //string retVal = "";
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
                        sqlText = "update ProductCategories set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where CategoryId=@CategoryId";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@CategoryId", ids[i]);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn));
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("ProductCategory Delete", vm.CategoryID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ProductCategory Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            #endregion

            #region catch finally

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                if (Vtransaction == null) 
                { 
                    transaction.Rollback(); 
                }

                FileLogger.Log("ProductCategoryDAL", "Delete", ex.ToString() + "\n" + sqlText);

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
