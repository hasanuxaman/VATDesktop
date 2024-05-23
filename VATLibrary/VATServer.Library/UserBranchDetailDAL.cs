using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class UserBranchDetailDAL : IUserBranchDetail
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        #endregion

        public string[] Insert(List<UserBranchDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

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
                #region Delete all previous branh
                foreach (var detail in Details)
                {
                    sqlText = "";
                    sqlText += "delete from UserBranchDetails where UserId=@UserId";
                    SqlCommand cmdChk = new SqlCommand(sqlText, currConn);
                    cmdChk.Transaction = transaction;
                    cmdChk.Parameters.AddWithValue("@UserId", detail.UserId);

                    int totalCount = (int)cmdChk.ExecuteNonQuery();
                }
                #endregion
                foreach (var detail in Details)
                {
                    #region Insert
                    #region SqlText
                    sqlText = "";
                    sqlText += " insert into UserBranchDetails";
                    sqlText += " (";
                    sqlText += " UserId         ";
                    sqlText += ",BranchId       ";
                    sqlText += ",Comments       ";
                    sqlText += ",CreatedBy      ";
                    sqlText += ",CreatedOn      ";
                    sqlText += ",LasatModifiedBy";
                    sqlText += ",LastModifiedOn ";
                    sqlText += " )";
                    sqlText += " values";
                    sqlText += " (";
                    sqlText += " @UserId";
                    sqlText += ",@BranchId";
                    sqlText += ",@Comments";
                    sqlText += ",@CreatedBy";
                    sqlText += ",@CreatedOn";
                    sqlText += ",@LastModifiedBy";
                    sqlText += ",@LastModifiedOn";
                    sqlText += ") SELECT SCOPE_IDENTITY()";
                    #endregion

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;

                    cmdInsert.Parameters.AddWithValueAndNullHandle("@UserId", detail.UserId);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", detail.BranchId);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", detail.Comments);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", detail.CreatedBy);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", detail.CreatedOn);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", detail.LastModifiedBy);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", detail.LastModifiedOn);

                    transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    retResults[4] = transResult.ToString();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }


                    #endregion
                }

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
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "" ;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public DataTable  SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
select 
 ub.Id
,ub.BranchId
,ub.UserId
,ub.Comments
,Ub.CreatedBy
,ub.CreatedOn
,ub.LasatModifiedBy
,ub.LastModifiedOn
,uf.UserName
,bp.BranchName
,bp.BranchCode
,bp.Address
 from UserBranchDetails ub 
 left outer join UserInformations uf on ub.UserId=uf.UserID 
 left outer join BranchProfiles bp on ub.BranchId=bp.BranchID

WHERE  1=1 and bp.IsArchive='0' and bp.ActiveStatus='Y'
";


                if (Id > 0)
                {
                    sqlText += @" and ub.UserId=@Id";
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
                sqlText += " order by ub.Id Asc";

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
                
                if (Id > 0)
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

        public List<UserBranchDetailVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UserBranchDetailVM> VMs = new List<UserBranchDetailVM>();
            UserBranchDetailVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new UserBranchDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.UserId = Convert.ToInt32(dr["UserId"].ToString());
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.Address = dr["Address"].ToString();
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


        public string[] InsertfromExcel(DataTable datatable, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string CreatedBy = null, string Createdon = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
    
            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

            DataTable dt = new DataTable();

            //List<UserBranchDetailVM> Details

            var Details = new List<UserBranchDetailVM>();


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

                for (var i = 0; i < datatable.Rows.Count; i++)
                {
                    string BranchCode = datatable.Rows[i]["BranchCode"].ToString();

                    string userName = datatable.Rows[i]["UserName"].ToString();
                    var vm = new UserBranchDetailVM();


                    string sqlselect = @"select BranchID from BranchProfiles where BranchCode=@BranchCode";
                    var cmd = new SqlCommand(sqlselect, currConn, transaction);
                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    vm.BranchId = (int)cmd.ExecuteScalar();


                    vm.BranchCode = datatable.Rows[i]["BranchCode"].ToString();
                    vm.BranchName = datatable.Rows[i]["BranchName"].ToString();
                    

                    sqlText = @"select UserID from UserInformations where UserName=@UserName";
                    var cmdUser = new SqlCommand(sqlText, currConn, transaction);
                    cmdUser.Parameters.AddWithValue("@UserName", userName);
                    vm.UserId =Convert.ToInt32(cmdUser.ExecuteScalar());
                    
                    Details.Add(vm);



                }

                //sqlText = @"select BranchID from BranchProfiles where BranchCode=@BranchCode";
                //var cmd = new SqlCommand(sqlText, currConn, transaction);

                //foreach (var detail in Details)
                //{

                //    cmd.Parameters.Clear();
                //    cmd.Parameters.AddWithValue("@BranchCode", detail.BranchCode);

                //    detail.BranchId = (int)cmd.ExecuteScalar();


                //    //SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                //    //da.SelectCommand.Transaction = transaction;
                //    //da.SelectCommand.Parameters.AddWithValue("@BranchCode", detail.BranchCode);
                //    //da.Fill(dt);

                //}

                //foreach (DataRow dr in dt.Rows)
                //{
                //    UserBranchDetailVM vm = new UserBranchDetailVM();
                   
                //    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    
                //    Details.Add(vm);
                //}




                //#region Delete all previous branh
                //foreach (var detail in Details)
                //{
                //    sqlText = "";
                //    sqlText += "delete from UserBranchDetails where UserId=@UserId";
                //    SqlCommand cmdChk = new SqlCommand(sqlText, currConn);
                //    cmdChk.Transaction = transaction;
                //    cmdChk.Parameters.AddWithValue("@UserId", detail.UserId);

                //    int totalCount = (int)cmdChk.ExecuteNonQuery();
                //}
                //#endregion

                foreach (var detail in Details)
                {

                    if (detail.BranchId > 0)
                    {

                        string isExit = @"select count(BranchID) from UserBranchDetails where BranchID=@BranchID  and UserId=@UserId";
                        var cmd = new SqlCommand(isExit, currConn, transaction);
                        cmd.Parameters.AddWithValue("@BranchID", detail.BranchId);
                        cmd.Parameters.AddWithValue("@UserId", detail.UserId);
                        var chackExit = (int)cmd.ExecuteScalar();
                        //SqlDataAdapter da = new SqlDataAdapter(isExit, currConn);
                        //da.SelectCommand.Transaction = transaction;
                        //da.SelectCommand.Parameters.AddWithValue("@BranchID", detail.BranchId);
                        //da.SelectCommand.Parameters.AddWithValue("@UserId", detail.UserId);


                        if (chackExit==0)
                        {
                            #region Insert
                            #region SqlText
                            sqlText = "";
                            sqlText += " insert into UserBranchDetails";
                            sqlText += " (";
                            sqlText += " UserId         ";
                            sqlText += ",BranchId       ";
                            sqlText += ",Comments       ";
                            sqlText += ",CreatedBy      ";
                            sqlText += ",CreatedOn      ";
                            sqlText += ",LasatModifiedBy";
                            sqlText += ",LastModifiedOn ";
                            sqlText += " )";
                            sqlText += " values";
                            sqlText += " (";
                            sqlText += " @UserId";
                            sqlText += ",@BranchId";
                            sqlText += ",@Comments";
                            sqlText += ",@CreatedBy";
                            sqlText += ",@CreatedOn";
                            sqlText += ",@LastModifiedBy";
                            sqlText += ",@LastModifiedOn";
                            sqlText += ") SELECT SCOPE_IDENTITY()";
                            #endregion

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;

                            cmdInsert.Parameters.AddWithValueAndNullHandle("@UserId", detail.UserId);
                            cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", detail.BranchId);
                            cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", detail.Comments);
                            cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", CreatedBy);
                            cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Createdon);
                            cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", detail.LastModifiedBy);
                            cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", detail.LastModifiedOn);

                            transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                            retResults[4] = transResult.ToString();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgSaveNotSuccessfully);
                            }


                            #endregion

                        }




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
                        }
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "";
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }



    }
}
