using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using System.Linq;

namespace VATServer.Library
{
    public class CPCDetailsDAL : ICPCDetails
    {

        #region Global Variables

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        bool Auto = false;

        #endregion

        #region web Method

        public List<CPCDetailsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CPCDetailsVM> VMs = new List<CPCDetailsVM>();
            CPCDetailsVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new CPCDetailsVM();

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Type = dr["Type"].ToString();
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

                FileLogger.Log("HSCodeDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("HSCodeDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

Id
,Code
,Name
,Type
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
FROM CPCDetails  
WHERE  1=1 

";
                #region sqlTextCount
                sqlTextCount += @" select count(Id)RecordCount
FROM CPCDetails  
WHERE  1=1 
";
                #endregion

                if (Id > 0)
                {
                    sqlTextParameter += @" and Id=@Id";
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
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
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

                FileLogger.Log("HSCodeDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("HSCodeDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public string[] InsertToCPCDetails(CPCDetailsVM vm,bool AutoSave = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            //string Code = vm.Code;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string nextId = "";
            string sqlText = "";
            string Code = vm.Code;
            #endregion

            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();


                #region Validation

                if (string.IsNullOrWhiteSpace(vm.Name))
                {
                    throw new ArgumentNullException("InsertTo Name",
                                                    "Please enter Name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToCPCDetails");


                #endregion open connection and transaction
                Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "CPC") == "Y" ? true : false);
                #region ProductID

               
                    sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  CPCDetails";

                

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
                        if (string.IsNullOrWhiteSpace(vm.Code))
                        {
                            vm.Code = nextId;
                        }
                    }
                    else if (string.IsNullOrEmpty(Code))
                    {
                        throw new ArgumentNullException("InsertToItem", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(Code) from CPCDetails where  Code='" + Code + "'";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToItem", "Same Code('" + Code + "') already exist");
                        }
                    }
                }
                else
                {
                    Code = nextId;
                }
                #endregion Code


                #region Insert CPCDetails Information

                vm.Code = Code;
                sqlText = "";
                sqlText += "insert into CPCDetails";
                sqlText += "(";
                sqlText += "Code,";
                sqlText += "Name,";
                sqlText += "Type,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "@Code";
                sqlText += ",@Name";
                sqlText += ",@Type";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";
                sqlText += ") SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Name", vm.Name ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Type", vm.Type ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[3] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.CPCmsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                vm.Id = transResult;

                #endregion Insert Bank Information

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
                retResults[1] = "Requested CPCDetails Information successfully added";
                retResults[2] = "" + vm.Id;
                retResults[3] = "" + vm.Code;

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                //retResults[2] = vm.Id; //catch ex
                transaction.Rollback();

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("HSCodeDAL", "InsertToHSCode", ex.ToString() + "\n" + sqlText);

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
        public string[] UpdateCPCDetails(CPCDetailsVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Convert.ToString(vm.Id);


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string Code = vm.Code;

            #endregion

            #region try

            try
            {

                #region Validation

                if (string.IsNullOrWhiteSpace(vm.Code))
                {
                    throw new ArgumentNullException("InsertToCPCDetailsInformation",
                                                    "Please enter CPCDetails .");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("CPCDetails");

                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/
                sqlText = "select count(Id) from CPCDetails where  Id=@Id";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@Id", vm.Id);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateCPCDetailsInformation", "Could not find requested CPC information");

                }
                #region Code
                CommonDAL commonDal = new CommonDAL();

                Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "CPC") == "Y" ? true : false);
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(Code))
                    {
                        throw new ArgumentNullException("UpdateItem", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(Code) from CPCDetails where  Code=@Code and Id <>@Id ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@Code", Code);
                        cmdCodeExist.Parameters.AddWithValue("@Id", vm.Id);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("UpdateItem", "Same Code('" + Code + "') already exist");
                        }
                    }
                }
                else
                {
                    Code = vm.Code;
                }

                #endregion Code

                #region Update CPCDetails Information

                sqlText = "";
                sqlText = "update CPCDetails set";

                sqlText += "  Code                      =@Code";
                sqlText += " ,Name                      =@Name";
                sqlText += " ,Type                      =@Type";
                sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                sqlText += " where Id                   =@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@Code", Code ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Name", vm.Name ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Type", vm.Type ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update CPCDetails Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested CPCDetails Information successfully updated";
                        retResults[2] = Convert.ToString(vm.Id);
                        retResults[3] = vm.Code;
                    }

                    else

                    {

                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update CPCDetails";
                        transaction.Rollback();


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
                retResults[2] = Convert.ToString(vm.Id); //catch ex
              
                    transaction.Rollback();

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("CPCDetailsAL", "UpdateCPCDetails", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(CPCDetailsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteHSCode"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                        sqlText = "update CPCDetails set";
                        sqlText += " LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
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
                        throw new ArgumentNullException("CPCDetails Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("CPCDetails Information Delete", "Could not found any item.");
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

                FileLogger.Log("HSCodeDAL", "Delete", ex.ToString() + "\n" + sqlText);

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

        public List<CPCDetailsVM> DropDown(string Type = "", SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CPCDetailsVM> VMs = new List<CPCDetailsVM>();
            CPCDetailsVM vm;
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
Code
,Name
   FROM CPCDetails
WHERE  1=1 
";

                if (!string.IsNullOrEmpty(Type))
                {
                    sqlText += @" AND Type=@Type";
                }
                sqlText += @" order by Name";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (!string.IsNullOrEmpty(Type))
                {
                    objComm.Parameters.AddWithValue("@Type", Type);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CPCDetailsVM();
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("CPCDetailsDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

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
        public string[] InsertfromExcel(CPCDetailsVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            //retResults[2] = Convert.ToString(vm.Id);


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

                if (string.IsNullOrWhiteSpace(vm.Code))
                {
                    throw new ArgumentNullException("InsertToHSCodeInformation",
                                                    "Please enter HSCode .");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("HSCode");

                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/
                sqlText = "select count(HSCode) from CPCDetails where  HSCode=@HSCode";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@Code", vm.Code);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    #region Insert CPCDetails Information



                    string Code = vm.Code.Replace(".", "");
                    vm.Code = Code;
                    sqlText = "";
                    sqlText += "insert into CPCDetails";
                    sqlText += "(";
                    sqlText += "Code,";
                    sqlText += "Name,";
                    sqlText += "Type,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";

                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@Code";
                    sqlText += ",@Name";
                    sqlText += ",@Type";
                    sqlText += ",@CreatedBy";
                    sqlText += ",@CreatedOn";
                    sqlText += ") SELECT SCOPE_IDENTITY()";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Type", vm.Type ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn ?? Convert.DBNull);
                    transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    retResults[3] = transResult.ToString();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                    vm.Id = transResult;

                    #endregion Insert Bank Information

                }
                else
                {

                    #region Update CPCDetails Information

                    sqlText = "";
                    sqlText = "update CPCDetails set";

                    //sqlText += "  HSCode                    =@HSCode";
                    sqlText += " Description               =@Description";
                    sqlText += " ,Comments                  =@Comments";
                    sqlText += " ,CD                        =@CD";
                    sqlText += " ,SD                        =@SD";
                    sqlText += " ,VAT                       =@VAT";
                    sqlText += " ,AIT                       =@AIT";
                    sqlText += " ,RD                        =@RD";
                    sqlText += " ,AT                        =@AT";
                    sqlText += " ,OtherSD                   =@OtherSD";
                    sqlText += " ,OtherVAT                  =@OtherVAT";
                    sqlText += " ,IsFixedVAT               =@IsFixedVAT";
                    sqlText += " ,IsFixedSD                =@IsFixedSD ";
                    sqlText += " ,IsFixedCD                =@IsFixedCD ";
                    sqlText += " ,IsFixedRD                =@IsFixedRD ";
                    sqlText += " ,IsFixedAIT               =@IsFixedAIT";
                    sqlText += " ,IsFixedAT                =@IsFixedAT ";
                    sqlText += " ,IsFixedOtherVAT          =@IsFixedOtherVAT";
                    sqlText += " ,IsFixedOtherSD           =@IsFixedOtherSD";
                    sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                    sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                    sqlText += " where HSCode                   =@HSCode";


                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;

                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Type", vm.Type ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);


                    transResult = (int)cmdUpdate.ExecuteNonQuery();

                    #endregion Update Bank Information

                }





                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Import HSCode Information successfully";
                        retResults[2] = Convert.ToString(vm.Id);
                        retResults[3] = vm.Code;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Import HSCode Information Fail";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Import HSCode Information Fail";
                }



                #endregion Commit

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = Convert.ToString(vm.Id); //catch ex

                transaction.Rollback();

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("HSCodeDAL", "InsertfromExcel", ex.ToString() + "\n" + sqlText);

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

    }
}
