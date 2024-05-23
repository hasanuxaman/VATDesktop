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
    public class HSCodeDAL : IHSCode
    {

        #region Global Variables

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region web Method

        public List<HSCodeVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<HSCodeVM> VMs = new List<HSCodeVM>();
            HSCodeVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                if (conditionFields != null && conditionValues != null)
                {
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new HSCodeVM();

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.HSCode = dr["HSCode"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.AIT = Convert.ToDecimal(dr["AIT"].ToString());
                    vm.AT = Convert.ToDecimal(dr["AT"].ToString());
                    vm.CD = Convert.ToDecimal(dr["CD"].ToString());
                    vm.OtherSD = Convert.ToDecimal(dr["OtherSD"].ToString());
                    vm.OtherVAT = Convert.ToDecimal(dr["OtherVAT"].ToString());
                    vm.RD = Convert.ToDecimal(dr["RD"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.VAT = Convert.ToDecimal(dr["VAT"].ToString());
                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();

                    vm.IsFixedSD = dr["IsFixedSD"].ToString();
                    vm.IsFixedCD = dr["IsFixedCD"].ToString();
                    vm.IsFixedRD = dr["IsFixedRD"].ToString();
                    vm.IsFixedAIT = dr["IsFixedAIT"].ToString();
                    vm.IsFixedAT = dr["IsFixedAT"].ToString();
                    vm.IsFixedOtherVAT = dr["IsFixedOtherVAT"].ToString();
                    vm.IsFixedOtherSD = dr["IsFixedOtherSD"].ToString();

                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.IsVDS = dr["IsVDS"].ToString();
                    vm.FiscalYear = dr["FiscalYear"].ToString();

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
,HSCode
,FiscalYear
,Description
,CD
,SD
,VAT
,AIT
,RD
,AT
,OtherSD
,OtherVAT
,IsFixedVAT
,isnull(IsFixedSD,'N')IsFixedSD
,isnull(IsFixedCD,'N')IsFixedCD
,isnull(IsFixedRD,'N')IsFixedRD
,isnull(IsFixedAIT,'N')IsFixedAIT
,isnull(IsFixedAT,'N')IsFixedAT
,isnull(IsFixedOtherVAT,'N')IsFixedOtherVAT
,isnull(IsFixedOtherSD,'N')IsFixedOtherSD
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,IsArchive
,IsVDS
FROM HSCodes  
WHERE  1=1 AND isnull(IsArchive,0) = 0 

";
                #region sqlTextCount
                sqlTextCount += @" select count(Id)RecordCount
FROM HSCodes  
WHERE  1=1 AND isnull(IsArchive,0) = 0
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

        public string[] InsertToHSCode(HSCodeVM vm, SysDBInfoVMTemp connVM = null)
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
            string sqlText = "";
            string Code = "";
            #endregion

            #region try

            try
            {

                #region Validation

                if (string.IsNullOrWhiteSpace(vm.HSCode))
                {
                    throw new ArgumentNullException("InsertTo HSCode",
                                                    "Please enter HSCode.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToHSCodes");


                #endregion open connection and transaction

                #region Insert HSCodes Information

                Code = vm.HSCode.Replace(".", "");
                vm.Code = Code;
                sqlText = "";
                sqlText += "insert into HSCodes";
                sqlText += "(";
                sqlText += "Code,";
                sqlText += "HSCode,";
                sqlText += "FiscalYear,";
                sqlText += "Description,";
                sqlText += "Comments,";
                sqlText += "CD,";
                sqlText += "SD,";
                sqlText += "VAT,";
                sqlText += "AIT,";
                sqlText += "RD,";
                sqlText += "AT,";
                sqlText += "OtherSD,";
                sqlText += "OtherVAT,";
                sqlText += "IsFixedVAT,";
                sqlText += "IsFixedSD,";
                sqlText += "IsFixedCD,";
                sqlText += "IsFixedRD,";
                sqlText += "IsFixedAIT,";
                sqlText += "IsFixedAT,";
                sqlText += "IsFixedOtherVAT,";
                sqlText += "IsFixedOtherSD,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "IsArchive,";
                sqlText += "IsVDS";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "@Code";
                sqlText += ",@HSCode";
                sqlText += ",@FiscalYear";
                sqlText += ",@Description";
                sqlText += ",@Comments";
                sqlText += ",@CD";
                sqlText += ",@SD";
                sqlText += ",@VAT";
                sqlText += ",@AIT";
                sqlText += ",@RD";
                sqlText += ",@AT";
                sqlText += ",@OtherSD";
                sqlText += ",@OtherVAT";
                sqlText += ",@IsFixedVAT";
                sqlText += ",@IsFixedSD";
                sqlText += ",@IsFixedCD";
                sqlText += ",@IsFixedRD";
                sqlText += ",@IsFixedAIT";
                sqlText += ",@IsFixedAT";
                sqlText += ",@IsFixedOtherVAT";
                sqlText += ",@IsFixedOtherSD";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";
                sqlText += ",@IsArchive";
                sqlText += ",@IsVDS";
                sqlText += ") SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@HSCode", vm.HSCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CD", vm.CD);
                cmdInsert.Parameters.AddWithValue("@SD", vm.SD);
                cmdInsert.Parameters.AddWithValue("@VAT", vm.VAT);
                cmdInsert.Parameters.AddWithValue("@AIT", vm.AIT);
                cmdInsert.Parameters.AddWithValue("@RD", vm.RD);
                cmdInsert.Parameters.AddWithValue("@AT", vm.AT);
                cmdInsert.Parameters.AddWithValue("@OtherSD", vm.OtherSD);
                cmdInsert.Parameters.AddWithValue("@OtherVAT", vm.OtherVAT);
                cmdInsert.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedSD", vm.IsFixedSD ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedCD", vm.IsFixedCD ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedRD", vm.IsFixedRD ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedAIT", vm.IsFixedAIT ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedAT ", vm.IsFixedAT ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedOtherVAT", vm.IsFixedOtherVAT ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsFixedOtherSD", vm.IsFixedOtherSD ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", OrdinaryVATDesktop.DateToDate(vm.CreatedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@IsVDS", vm.IsVDS);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[3] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
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
                retResults[1] = "Requested HSCodes Information successfully added";
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
        public string[] UpdateHSCode(HSCodeVM vm, SysDBInfoVMTemp connVM = null)
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

            #endregion

            #region try

            try
            {

                #region Validation

                if (string.IsNullOrWhiteSpace(vm.HSCode))
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
                sqlText = "select count(Id) from HSCodes where  Id=@Id";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@Id", vm.Id);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateHSCodeInformation", "Could not find requested bank information");

                }

                #region Update HSCodes Information

                sqlText = "";
                sqlText = "update HSCodes set";

                sqlText += "  HSCode                    =@HSCode";
                sqlText += " ,FiscalYear                =@FiscalYear";
                sqlText += " ,Description               =@Description";
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
                sqlText += " ,IsVDS                     =@IsVDS";

                sqlText += " where Id                   =@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@HSCode", vm.HSCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CD", vm.CD);
                cmdUpdate.Parameters.AddWithValue("@SD", vm.SD);
                cmdUpdate.Parameters.AddWithValue("@VAT", vm.VAT);
                cmdUpdate.Parameters.AddWithValue("@AIT", vm.AIT);
                cmdUpdate.Parameters.AddWithValue("@RD", vm.RD);
                cmdUpdate.Parameters.AddWithValue("@AT", vm.AT);
                cmdUpdate.Parameters.AddWithValue("@OtherSD", vm.OtherSD);
                cmdUpdate.Parameters.AddWithValue("@OtherVAT", vm.OtherVAT);
                cmdUpdate.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedSD", vm.IsFixedSD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedCD", vm.IsFixedCD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedRD", vm.IsFixedRD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedAIT", vm.IsFixedAIT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedAT ", vm.IsFixedAT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedOtherVAT", vm.IsFixedOtherVAT ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@IsFixedOtherSD", vm.IsFixedOtherSD ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(vm.LastModifiedOn) ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                cmdUpdate.Parameters.AddWithValue("@IsVDS", vm.IsVDS);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Bank Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested HSCode Information successfully updated";
                        retResults[2] = Convert.ToString(vm.Id);
                        retResults[3] = vm.Code;
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
                retResults[2] = Convert.ToString(vm.Id); //catch ex

                transaction.Rollback();

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("HSCodeDAL", "UpdateHSCode", ex.ToString() + "\n" + sqlText);

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
        public string[] Delete(HSCodeVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                        sqlText = "update HSCodes set";
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
                        throw new ArgumentNullException("HSCodes Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("HSCodes Information Delete", "Could not found any item.");
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

        public DataTable GetExcelData(List<string> HSCode, SqlConnection VcurrConn = null, SqlTransaction VTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlselect = "";

            DataTable dt = new DataTable();

            #endregion

            #region try

            try
            {

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;

                }
                if (VTransaction != null)
                {
                    transaction = VTransaction;
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
                    transaction = currConn.BeginTransaction();
                }

                sqlselect = @" SELECT

HSCode
,Description
,CD
,SD
,VAT
,AIT
,RD
,AT
,OtherSD
,OtherVAT
,IsFixedVAT
,isnull(IsFixedSD,'N')IsFixedSD
,isnull(IsFixedCD,'N')IsFixedCD
,isnull(IsFixedRD,'N')IsFixedRD
,isnull(IsFixedAIT,'N')IsFixedAIT
,isnull(IsFixedAT,'N')IsFixedAT
,isnull(IsFixedOtherVAT,'N')IsFixedOtherVAT
,isnull(IsFixedOtherSD,'N')IsFixedOtherSD
,Comments
FROM HSCodes  
WHERE  1=1 AND isnull(IsArchive,0) = 0 and HSCode in ";

                var len = HSCode.Count;

                sqlselect += "(";
                for (int i = 0; i < len; i++)
                {
                    sqlselect += "'" + HSCode[i] + "',";
                }
                sqlselect = sqlselect.TrimEnd(',') + ")";

                SqlCommand cmdSelect = new SqlCommand(sqlselect, currConn, transaction);

                SqlDataAdapter da = new SqlDataAdapter(cmdSelect);
                da.Fill(dt);

                if (transaction != null && VTransaction == null)
                {
                    transaction.Commit();
                }





            }
            #endregion

            catch (Exception ex)
            {
                if (transaction != null && VTransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("HSCodeDAL", "GetExcelData", ex.ToString() + "\n" + sqlselect);
                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlselect + FieldDelimeter + ex.Message.ToString());

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

            return dt;



        }

        public string[] InsertfromExcel(HSCodeVM vm, SysDBInfoVMTemp connVM = null)
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

                if (string.IsNullOrWhiteSpace(vm.HSCode))
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
                sqlText = "select count(HSCode) from HSCodes where  HSCode=@HSCode";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@HSCode", vm.HSCode);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    #region Insert HSCodes Information



                    string Code = vm.HSCode.Replace(".", "");
                    vm.Code = Code;
                    sqlText = "";
                    sqlText += "insert into HSCodes";
                    sqlText += "(";
                    sqlText += "Code,";
                    sqlText += "HSCode,";
                    sqlText += "Description,";
                    sqlText += "Comments,";
                    sqlText += "CD,";
                    sqlText += "SD,";
                    sqlText += "VAT,";
                    sqlText += "AIT,";
                    sqlText += "RD,";
                    sqlText += "AT,";
                    sqlText += "OtherSD,";
                    sqlText += "OtherVAT,";
                    sqlText += "IsFixedVAT,";
                    sqlText += "IsFixedSD,";
                    sqlText += "IsFixedCD,";
                    sqlText += "IsFixedRD,";
                    sqlText += "IsFixedAIT,";
                    sqlText += "IsFixedAT,";
                    sqlText += "IsFixedOtherVAT,";
                    sqlText += "IsFixedOtherSD,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";

                    sqlText += "IsArchive";


                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@Code";
                    sqlText += ",@HSCode";
                    sqlText += ",@Description";
                    sqlText += ",@Comments";
                    sqlText += ",@CD";
                    sqlText += ",@SD";
                    sqlText += ",@VAT";
                    sqlText += ",@AIT";
                    sqlText += ",@RD";
                    sqlText += ",@AT";
                    sqlText += ",@OtherSD";
                    sqlText += ",@OtherVAT";
                    sqlText += ",@IsFixedVAT";
                    sqlText += ",@IsFixedSD";
                    sqlText += ",@IsFixedCD";
                    sqlText += ",@IsFixedRD";
                    sqlText += ",@IsFixedAIT";
                    sqlText += ",@IsFixedAT";
                    sqlText += ",@IsFixedOtherVAT";
                    sqlText += ",@IsFixedOtherSD";


                    sqlText += ",@CreatedBy";
                    sqlText += ",@CreatedOn";

                    sqlText += ",@IsArchive";
                    sqlText += ") SELECT SCOPE_IDENTITY()";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@HSCode", vm.HSCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CD", vm.CD);
                    cmdInsert.Parameters.AddWithValue("@SD", vm.SD);
                    cmdInsert.Parameters.AddWithValue("@VAT", vm.VAT);
                    cmdInsert.Parameters.AddWithValue("@AIT", vm.AIT);
                    cmdInsert.Parameters.AddWithValue("@RD", vm.RD);
                    cmdInsert.Parameters.AddWithValue("@AT", vm.AT);
                    cmdInsert.Parameters.AddWithValue("@OtherSD", vm.OtherSD);
                    cmdInsert.Parameters.AddWithValue("@OtherVAT", vm.OtherVAT);
                    cmdInsert.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT);
                    cmdInsert.Parameters.AddWithValue("@IsFixedSD", vm.IsFixedSD);
                    cmdInsert.Parameters.AddWithValue("@IsFixedCD", vm.IsFixedCD);
                    cmdInsert.Parameters.AddWithValue("@IsFixedRD", vm.IsFixedRD);
                    cmdInsert.Parameters.AddWithValue("@IsFixedAIT", vm.IsFixedAIT);
                    cmdInsert.Parameters.AddWithValue("@IsFixedAT ", vm.IsFixedAT);
                    cmdInsert.Parameters.AddWithValue("@IsFixedOtherVAT", vm.IsFixedOtherVAT);
                    cmdInsert.Parameters.AddWithValue("@IsFixedOtherSD", vm.IsFixedOtherSD);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);

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

                    #region Update HSCodes Information

                    sqlText = "";
                    sqlText = "update HSCodes set";

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

                    cmdUpdate.Parameters.AddWithValue("@HSCode", vm.HSCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@CD", vm.CD);
                    cmdUpdate.Parameters.AddWithValue("@SD", vm.SD);
                    cmdUpdate.Parameters.AddWithValue("@VAT", vm.VAT);
                    cmdUpdate.Parameters.AddWithValue("@AIT", vm.AIT);
                    cmdUpdate.Parameters.AddWithValue("@RD", vm.RD);
                    cmdUpdate.Parameters.AddWithValue("@AT", vm.AT);
                    cmdUpdate.Parameters.AddWithValue("@OtherSD", vm.OtherSD);
                    cmdUpdate.Parameters.AddWithValue("@OtherVAT", vm.OtherVAT);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedSD", vm.IsFixedSD);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedCD", vm.IsFixedCD);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedRD", vm.IsFixedRD);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedAIT", vm.IsFixedAIT);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedAT ", vm.IsFixedAT);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedOtherVAT", vm.IsFixedOtherVAT);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedOtherSD", vm.IsFixedOtherSD);
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
