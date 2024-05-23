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
using VATServer.Interface;

namespace VATServer.Library
{
    public class ShiftDAL : IShift
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        public string[] InsertToShiftNew(ShiftVM vm, SysDBInfoVMTemp connVM = null)
        {


            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";


            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.ShiftName))
                {
                    throw new ArgumentNullException("InsertToShift",
                                                    "Please enter Shift name.");
                }
 

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToShift");

                #endregion open connection and transaction


                #region Customer Group name existence checking 


                sqlText = "select count(Id) from Shifts where  ShiftName='" + vm.ShiftName + "'";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;
                    countId = (int)cmdNameExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToShift",
                                                        "Same ShiftName already exist");
                    }
                #endregion Customer Group name existence checking
                
                
                #region Customer Group new id generation
                    sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  Shifts";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {

                        throw new ArgumentNullException("InsertToShift",
                                                        "Unable to create new Shift");
                    }
                #endregion Customer Group new id generation


                #region Inser new customer group
                    sqlText = "";
                    sqlText += "insert into Shifts";
                    sqlText += "(";
                    sqlText += "ShiftName,";
                    sqlText += "ShiftStart,";
                    sqlText += "ShiftEnd,";
                    sqlText += "Remarks,";
                    sqlText += "NextDay,";
                    sqlText += "Sl";

                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "'" + vm.ShiftName + "',";
                    sqlText += "'" + vm.ShiftStart + "',";
                    sqlText += "'" + vm.ShiftEnd + "',";
                    sqlText += "'" + vm.Remarks + "',";
                    sqlText += "'" + vm.NextDay + "',";
                    sqlText += "'" + vm.Sl + "'";
                    sqlText += ")";
                    sqlText += " select SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    transResult =Convert.ToInt32( cmdInsert.ExecuteScalar());


                    #region Commit


                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested Shift Information successfully Added";
                            retResults[2] = "" + transResult;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add Shiftp";
                            retResults[2] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Shift";
                        retResults[2] = "";
                    }

                    #endregion Commit


                #endregion Inser new customer group

            }
            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
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
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            return retResults;
        }


        public string[] UpdateToShiftNew(ShiftVM vm, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.Id.ToString();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #region try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Id))
                {
                    throw new ArgumentNullException("Update Shift ",
                                                    "Invalid Shift Id, Shift Not Found.");
                }
                
                if (string.IsNullOrEmpty(vm.ShiftName))
                {
                    throw new ArgumentNullException("UpdateToShift",
                                                    "Please enter Shift name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToShift");

                #endregion open connection and transaction

                #region Customer Group existence checking

                sqlText = "select count(Id) from Shifts where  Id='" + vm.Id + "'";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToShift",
                                "Could not find requested Shift id.");
                }

                #endregion Customer Group existence checking

                #region Customer Group name existence checking
                sqlText = "select count(ShiftName) from Shifts ";
                sqlText += " where  ShiftName='" + vm.ShiftName + "'";
                sqlText += " and not Id='" + vm.Id + "'";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToShift",
                                                    "Same Shift name already exist");
                }
                #endregion Customer Group name existence checking

                #region Inser new customer group
                sqlText = "";
                sqlText = "update Shifts set";
                sqlText += " ShiftName='" + vm.ShiftName + "',";
                sqlText += " ShiftStart='" + vm.ShiftStart + "',";
                sqlText += " ShiftEnd='" + vm.ShiftEnd + "',";
                sqlText += " Remarks='" + vm.Remarks + "',";
                sqlText += " NextDay='" + vm.NextDay + "',";
                sqlText += " Sl='" + vm.Sl + "'";

                sqlText += " where Id='" + vm.Id + "'";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Shift Information successfully Update";


                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to update Shift";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to update Shift";
                }

                #endregion Commit


                #endregion Inser new customer group

            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

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

          return retResults;
        }


        public string[] DeleteShiftNew(string Id, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id;

            SqlConnection currConn = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Transaction Used
                CommonDAL commonDal = new CommonDAL();
                 
                #endregion Transaction Used
                #region Validation
                if (string.IsNullOrEmpty(Id))
                {
                    throw new ArgumentNullException("DeleteShift",
                                "Could not find requested Shift.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                //sqlText = "select count(Id) from Shifts where Id='" + Id + "'";
                sqlText = "select count(Id) from Shifts where Id= @Id ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);

                //BugsBD
                SqlParameter parameter = new SqlParameter("@Id", SqlDbType.VarChar, 250);
                parameter.Value = Id;
                cmdExist.Parameters.Add(parameter);



                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Shift.";
                    return retResults;
                }

                //sqlText = "delete Shifts where Id='" + Id + "'";
                sqlText = "delete Shifts where Id= @Id";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);


                //BugsBD
                SqlParameter parameter2 = new SqlParameter("@Id", SqlDbType.VarChar, 250);
                parameter2.Value = Id;
                cmdDelete.Parameters.Add(parameter2);


                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Shift Information successfully deleted";
                }
            }
            #region catch
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


        public DataTable SearchShift(string ShiftName, int Id = 0, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Shifts");
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
Id,isnull(ShiftName,'NA')ShiftName
,isnull(ShiftStart,'12:00')ShiftStart
,isnull(ShiftEnd,'12:00')ShiftEnd
,isnull(Remarks,'NA')Remarks
,isnull(Sl,'0')Sl
,isnull(NextDay,'N')NextDay
FROM         Shifts
WHERE 	(1=1) 
AND (ShiftName LIKE '%' + @ShiftName + '%' OR @ShiftName IS NULL)

";
                if (Id>0)
	{
        sqlText += @" and Id=@Id ";
	}
                sqlText += @"order by ShiftName ";


               

                

                SqlCommand objCommShift = new SqlCommand();
                objCommShift.Connection = currConn;
                objCommShift.CommandText = sqlText;
                objCommShift.CommandType = CommandType.Text;


                if (Id > 0)
                {
                    if (!objCommShift.Parameters.Contains("@Id"))
                    { objCommShift.Parameters.AddWithValue("@Id", Id); }
                    else { objCommShift.Parameters["@Id"].Value = Id; }
                }
                if (!objCommShift.Parameters.Contains("@ShiftName"))
                { objCommShift.Parameters.AddWithValue("@ShiftName", ShiftName); }
                else { objCommShift.Parameters["@ShiftName"].Value = ShiftName; }
 

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommShift);
                dataAdapter.Fill(dataTable);

                // Common Filed
                #endregion
            }
            #endregion
            #region catch
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


        public DataTable SearchForTime(string time, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Shifts");
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
--declare @Time as time
--set @Time='06:00'
 
 select * from (
select * from (
SELECT   * FROM         Shifts
where    @time between  shiftStart and ShiftEnd
and isnull(NextDay,'N')='N'

union all
SELECT   * FROM         Shifts
where   1=1 
and  ((@time between  shiftStart and '23:59:59') 
or (@time between  '00:00:01' and ShiftEnd) )
and NextDay='Y'
) as a
union all

 select * from Shifts
 where id not in(
 select id from (
SELECT   * FROM         Shifts
where    @time between  shiftStart and ShiftEnd
and isnull(NextDay,'N')='N'

union all
SELECT   * FROM         Shifts
where   1=1 
and  ((@time between  shiftStart and '23:59:59') 
or (@time between  '00:00:01' and ShiftEnd) )
and NextDay='Y'
) as a
)
) as b
";
                

                SqlCommand objCommShift = new SqlCommand();
                objCommShift.Connection = currConn;
                objCommShift.CommandText = sqlText;
                objCommShift.CommandType = CommandType.Text;



                if (!objCommShift.Parameters.Contains("@time"))
                    { objCommShift.Parameters.AddWithValue("@time", time); }
                else { objCommShift.Parameters["@time"].Value = time; }
               
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommShift);
                dataAdapter.Fill(dataTable);

                // Common Filed
                #endregion
            }
            #endregion
            #region catch
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
        public List<ShiftVM> DropDown(int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ShiftVM> VMs = new List<ShiftVM>();
            ShiftVM vm;
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
s.Id
,s.ShiftName Name
   FROM Shifts s
WHERE  1=1 
";

                ////if (branchId > 0)
                ////{
                ////    sqlText += @" and d.BranchId=@BranchId";
                ////}
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                ////if (branchId > 0)
                ////{
                ////    objComm.Parameters.AddWithValue("@BranchId", branchId);
                ////}
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ShiftVM();
                    vm.Id = dr["Id"].ToString();
                    vm.ShiftName = dr["Name"].ToString();
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

        public List<ShiftVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ShiftVM> VMs = new List<ShiftVM>();
            ShiftVM vm;
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
 Id
,ShiftName
,ShiftStart
,ShiftEnd
,Remarks
,Sl
,NextDay

FROM Shifts  
WHERE  1=1

";


                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new ShiftVM();
                    vm.Id = dr["Id"].ToString();
                    vm.ShiftName = Convert.ToString(dr["ShiftName"]);
                    vm.ShiftStart = (Convert.ToString(dr["ShiftStart"]));//OrdinaryVATDesktop.StringToTimeN );
                    vm.ShiftEnd = Convert.ToString(dr["ShiftEnd"]);//OrdinaryVATDesktop.StringToTimeN ();
                    vm.Remarks = Convert.ToString(dr["Remarks"]);
                    vm.Sl = dr["Sl"].ToString();
                    vm.NextDay = Convert.ToString(dr["NextDay"]);

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
        #endregion


    }
}
