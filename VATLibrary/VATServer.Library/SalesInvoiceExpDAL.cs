using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class SalesInvoiceExpDAL : ISalesInvoiceExp
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region web Method
//        public List<HSCodeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            List<HSCodeVM> VMs = new List<HSCodeVM>();
//            HSCodeVM vm;
//            #endregion
//            try
//            {

//                #region sql statement

//                #region SqlExecution

//                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false);

//                foreach (DataRow dr in dt.Rows)
//                {
//                    vm = new HSCodeVM();

//                    vm.Id = Convert.ToInt32(dr["Id"]);
//                    vm.Code = dr["Code"].ToString();
//                    vm.HSCode = dr["HSCode"].ToString();
//                    vm.Description = dr["Description"].ToString();
//                    vm.AIT = Convert.ToDecimal(dr["AIT"].ToString());
//                    vm.AT = Convert.ToDecimal(dr["AT"].ToString());
//                    vm.CD = Convert.ToDecimal(dr["CD"].ToString());
//                    vm.OtherSD = Convert.ToDecimal(dr["OtherSD"].ToString());
//                    vm.OtherVAT = Convert.ToDecimal(dr["OtherVAT"].ToString());
//                    vm.RD = Convert.ToDecimal(dr["RD"].ToString());
//                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
//                    vm.VAT = Convert.ToDecimal(dr["VAT"].ToString());
//                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();

//                    vm.IsFixedSD = dr["IsFixedSD"].ToString();
//                    vm.IsFixedCD = dr["IsFixedCD"].ToString();
//                    vm.IsFixedRD = dr["IsFixedRD"].ToString();
//                    vm.IsFixedAIT = dr["IsFixedAIT"].ToString();
//                    vm.IsFixedVAT1 = dr["IsFixedVAT1"].ToString();
//                    vm.IsFixedAT = dr["IsFixedAT"].ToString();

//                    vm.Comments = dr["Comments"].ToString();
//                    vm.CreatedBy = dr["CreatedBy"].ToString();
//                    vm.CreatedOn = dr["CreatedOn"].ToString();
//                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
//                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();


//                    VMs.Add(vm);
//                }

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
//            return VMs;
//        }

        public List<SalesInvoiceExpVM> SelectAllList(int Id, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SalesInvoiceExpVM> VMs = new List<SalesInvoiceExpVM>();
            SalesInvoiceExpVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction,false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new SalesInvoiceExpVM();


                    vm.ID = Convert.ToInt32(dr["ID"]);
                    vm.LCNumber = dr["LCNumber"].ToString();
                    vm.LCBank = dr["LCBank"].ToString();
                    vm.LCDate = dr["LCDate"].ToString();
                    vm.PINo = dr["PINo"].ToString();
                    vm.PIDate = dr["PIDate"].ToString();
                    vm.PortFrom = dr["PortFrom"].ToString();
                    vm.PortTo = dr["PortTo"].ToString();
                    vm.EXPNo = dr["EXPNo"].ToString();
                    vm.EXPDate = dr["EXPDate"].ToString();
                    vm.IsArchive = dr["IsArchive"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
        public DataTable SelectAll(int ID = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
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
ID
,LCDate
,LCBank
,PINo
,PIDate
,EXPNo
,EXPDate
,PortFrom
,PortTo
,isnull(LCNumber,'N/A')LCNumber
,Remarks
,IsArchive
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn


FROM SalesInvoiceExps  
WHERE  1=1 AND isnull(IsArchive,0) = 0 

";
                if (ID > 0)
                {
                    sqlText += @" and ID=@ID";
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

                if (ID > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@ID", ID);
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


        public string[] InsertToSalesInvoiceExp(SalesInvoiceExpVM vm, SysDBInfoVMTemp connVM = null)
        {
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
            int nextId = 0;

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.PINo))
                {
                    throw new ArgumentNullException("InsertToSalesInvoiceExp",
                                                    "Please enter SalesInvoiceExp PI No ");
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

                #region SalesInvoiceExp  new id generation
                sqlText = "select isnull(max(cast(ID as int)),0)+1 FROM  SalesInvoiceExps";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToSalesInvoiceExps",
                                                    "Unable to create new SalesInvoiceExps No");
                }
                #endregion

                #region Insert SalesInvoiceExps Information



                vm.ID = nextId;
                sqlText = "";
                sqlText += "insert into SalesInvoiceExps";
                sqlText += "(";
                sqlText += "ID,";
                sqlText += "LCDate,";
                sqlText += "LCBank,";
                sqlText += "PINo,";
                sqlText += "PIDate,";
                sqlText += "EXPNo,";
                sqlText += "EXPDate,";
                sqlText += "PortFrom,";
                sqlText += "PortTo,";
                sqlText += "LCNumber,";
                sqlText += "Remarks,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "IsArchive";


                sqlText += ")";
                sqlText += " values(";
                sqlText += "@ID";
                sqlText += ",@LCDate";
                sqlText += ",@LCBank";
                sqlText += ",@PINo";
                sqlText += ",@PIDate";
                sqlText += ",@EXPNo";
                sqlText += ",@EXPDate";
                sqlText += ",@PortFrom";
                sqlText += ",@PortTo";
                sqlText += ",@LCNumber";
                sqlText += ",@Remarks";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@IsArchive";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@ID", vm.ID);
                cmdInsert.Parameters.AddWithValue("@LCDate", vm.LCDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LCBank", vm.LCBank ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PINo", vm.PINo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PIDate", vm.PIDate);
                cmdInsert.Parameters.AddWithValue("@EXPNo", vm.EXPNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@EXPDate", vm.EXPDate);
                cmdInsert.Parameters.AddWithValue("@PortFrom", vm.PortFrom ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PortTo", vm.PortTo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LCNumber", vm.LCNumber ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                transResult = (int)cmdInsert.ExecuteNonQuery();

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
                retResults[1] = "Requested SalesInvoiceExps Information successfully added";
                retResults[2] = "" + vm.ID;
               



            }
            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                //retResults[2] = vm.Id; //catch ex
                transaction.Rollback();

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
        public string[] UpdateSalesInvoiceExps(SalesInvoiceExpVM vm, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Convert.ToString(vm.ID);


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {

                if (string.IsNullOrEmpty(vm.PINo))
                {
                    throw new ArgumentNullException("InsertToSalesInvoiceExp",
                                                    "Please enter SalesInvoiceExp PI No ");
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("SalesInvoiceExps");

                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/
                sqlText = "select count(Id) from SalesInvoiceExps where  ID=@ID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@ID", vm.ID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateSalesInvoiceExpsInformation", "Could not find requested bank information");

                }



                #region Update HSCodes Information

                sqlText = "";
                sqlText = "update SalesInvoiceExps set";

                sqlText += "  LCDate                       =@LCDate";
                sqlText += " ,LCBank                       =@LCBank";
                sqlText += " ,PINo                         =@PINo";
                sqlText += " ,PIDate                       =@PIDate";
                sqlText += " ,EXPNo                        =@EXPNo";
                sqlText += " ,EXPDate                      =@EXPDate";
                sqlText += " ,PortFrom                     =@PortFrom";
                sqlText += " ,PortTo                       =@PortTo";
                sqlText += " ,LCNumber                     =@LCNumber";
                sqlText += " ,Remarks                      =@Remarks";
                sqlText += " ,LastModifiedBy               =@LastModifiedBy";
                sqlText += " ,LastModifiedOn               =@LastModifiedOn";
                sqlText += " where ID                      =@ID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@LCDate", vm.LCDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LCBank", vm.LCBank ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@PINo", vm.PINo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@PIDate", vm.PIDate);
                cmdUpdate.Parameters.AddWithValue("@EXPNo", vm.EXPNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@EXPDate", vm.EXPDate);
                cmdUpdate.Parameters.AddWithValue("@PortFrom", vm.PortFrom ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@PortTo", vm.PortTo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LCNumber", vm.LCNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ID", vm.ID);


                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Bank Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested SalesInvoiceExps Information successfully updated";
                        retResults[2] = Convert.ToString(vm.ID);
                     
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update SalesInvoiceExps";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update SalesInvoiceExps";
                }



                #endregion Commit

            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = Convert.ToString(vm.ID); //catch ex

                transaction.Rollback();

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
        public string[] Delete(SalesInvoiceExpVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalesInvoiceExp"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                        sqlText = "update SalesInvoiceExps set";
                        sqlText += " LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where ID=@ID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@ID", ids[i]);
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
                        throw new ArgumentNullException("SalesInvoiceExps Delete", vm.ID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("SalesInvoiceExps Information Delete", "Could not found any item.");
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
