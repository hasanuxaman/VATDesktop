using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Interface;


namespace VATServer.Library
{
    public class PackagingDAL : IPackaging
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public DataTable SearchPackage(string PackName, string PackgeSize, string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Pacakges");

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

                sqlText = @"           
                            SELECT isnull(NULLIF(p.PackagingID,''),0)PackagingID, 
                            isnull(NULLIF(p.PackagingNature,''),'')PackNature,
                            isnull(NULLIF(p.PackagingCapacity,''),'')PackCapacity,
                            isnull(NULLIF(p.UOM,''),'')UOM,
                            isnull(NULLIF(p.Description,''),'')Description,
                            isnull(NULLIF(p.ActiveStatus,''),'')ActiveStatus
                            FROM PackagingInformations p
                 
                            WHERE 
                                (p.PackagingNature  LIKE '%' +  @PackName  + '%' OR @PackName IS NULL) 
                            AND (p.PackagingCapacity  LIKE '%' +  @PackgeSize  + '%' OR @PackgeSize IS NULL) 
                            AND (p.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by p.PackagingID 
                            ";
                SqlCommand objCommPackage = new SqlCommand();
                objCommPackage.Connection = currConn;

                objCommPackage.CommandText = sqlText;
                objCommPackage.CommandType = CommandType.Text;


                if (!objCommPackage.Parameters.Contains("@PackName"))
                { objCommPackage.Parameters.AddWithValue("@PackName", PackName); }
                else { objCommPackage.Parameters["@PackName"].Value = PackName; }

                if (!objCommPackage.Parameters.Contains("@PackgeSize"))
                { objCommPackage.Parameters.AddWithValue("@PackgeSize", PackgeSize); }
                else { objCommPackage.Parameters["@PackgeSize"].Value = PackgeSize; }

                if (!objCommPackage.Parameters.Contains("@ActiveStatus"))
                { objCommPackage.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommPackage.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPackage);

                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("PackagingDAL", "SearchPackage", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("PackagingDAL", "SearchPackage", ex.ToString() + "\n" + sqlText);
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

        #region need to parameterize

        public string[] InsertToPackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

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

                if (string.IsNullOrEmpty(PackName))
                {
                    throw new ArgumentNullException("InsertToPackagingInformation", "Please enter Nature of Package.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToPackagingInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(PackID))
                {
                    sqlText = "";
                    sqlText = "select count(PackagingID) from PackagingInformations where  PackagingID=@PackID";
                    sqlText += " and PackagingNature=@PackName";
                    sqlText += " and PackagingCapacity=@PackSize";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;

                    cmdIdExist.Parameters.AddWithValue("@PackID", PackID);
                    cmdIdExist.Parameters.AddWithValue("@PackName", PackName);
                    cmdIdExist.Parameters.AddWithValue("@PackSize", PackSize);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToPackagingInformation", "Packaging information already exist");
                    }

                }

                #region Insert Packaging Information

                //sqlText = "select count(distinct PackagingNature) from PackagingInformations where  PackagingNature='" + PackName + "' and PackagingCapacity='" + PackSize + "'";
                sqlText = "select count(distinct PackagingNature) from PackagingInformations where  PackagingNature = @PackName  and PackagingCapacity = @PackSize";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);

                //BugsBD
                SqlParameter parameter = new SqlParameter("@PackName", SqlDbType.VarChar, 250);
                parameter.Value = PackName;
                cmdNameExist.Parameters.Add(parameter);

                parameter = new SqlParameter("@PackSize", SqlDbType.VarChar, 250);
                parameter.Value = PackSize;
                cmdNameExist.Parameters.Add(parameter);


                cmdNameExist.Transaction = transaction;
                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("InsertToPackagingInformation", "Requested package Name is already exist");
                }

                sqlText = "select isnull(max(cast(PackagingID as int)),0)+1 FROM  PackagingInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Packaging information Id";
                    throw new ArgumentNullException("InsertToPackagingInformation", "Unable to create new Packaging information Id");
                }


                sqlText = "";
                sqlText += "insert into PackagingInformations";
                sqlText += "(";
                sqlText += "PackagingID,";
                sqlText += "PackagingNature,";
                sqlText += "PackagingCapacity,";
                sqlText += "UOM,";
                sqlText += "Description,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "'" + nextId + "',";
                sqlText += "@PackName,";
                sqlText += "@PackSize,";
                sqlText += "@Uom,";
                sqlText += "@Description,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@PackName", PackName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PackSize", PackSize);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Uom", Uom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Description", Description);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information

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
                retResults[1] = "Requested Packaging Information successfully added";
                retResults[2] = "" + nextId;
            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("PackagingDAL", "InsertToPackage", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("PackagingDAL", "InsertToPackage", ex.ToString() + "\n" + sqlText);
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

            return retResults;

        }

        public string[] UpdatePackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

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

                if (string.IsNullOrEmpty(PackName))
                {
                    throw new ArgumentNullException("UpdatePackage", "Please enter Package name.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdatePackage");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(PackID))
                {
                    sqlText = "";
                    sqlText = "select count(PackagingID) from PackagingInformations where  PackagingID=@PackID";
                    sqlText += " and PackagingNature=@PackName and PackagingCapacity=@PackSize ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;

                    cmdIdExist.Parameters.AddWithValue("@PackID", PackID);
                    cmdIdExist.Parameters.AddWithValue("@PackName", PackName);
                    cmdIdExist.Parameters.AddWithValue("@PackSize", PackSize);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("UpdatePackage", "Could not find requested Packaging information ");
                    }

                }

                #region Update Packaging Information

                sqlText = "";
                sqlText += "UPDATE PackagingInformations SET ";
                sqlText += " PackagingNature    =@PackName,";
                sqlText += " PackagingCapacity  =@PackSize,";
                sqlText += " UOM                =@Uom,";
                sqlText += " Description        =@Description,";
                sqlText += " ActiveStatus       =@ActiveStatus,";
                sqlText += " LastModifiedBy     =@LastModifiedBy,";
                sqlText += " LastModifiedOn     =@LastModifiedOn";
                sqlText += " where PackagingID  =@PackID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@PackName", PackName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PackSize", PackSize);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Uom", Uom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Description", Description);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ActiveStatus", ActiveStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PackID", PackID);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Packaging Information

                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Packaging Information successfully Updated";

                    }

                }

                #endregion Commit
            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("PackagingDAL", "UpdatePackage", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("PackagingDAL", "UpdatePackage", ex.ToString() + "\n" + sqlText);
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

            return retResults;
        }

        public string[] DeletePackageInformation(string PackId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            //retResults[2] = PackId.ToString();
            retResults[2] = PackId;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(PackId.ToString()))
                {
                    throw new ArgumentNullException("DeletePackageInformation", "Could not find requested Package Id.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                sqlText = "select count(PackagingID) from PackagingInformations where PackagingID=@PackId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@PackId", PackId);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Package Information.";
                    return retResults;
                }

                sqlText = "delete PackagingInformations where PackagingID=@PackId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@PackId", PackId);


                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Package Information successfully deleted";
                }
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("PackagingDAL", "DeletePackageInformation", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("PackagingDAL", "DeletePackageInformation", ex.ToString() + "\n" + sqlText);
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

            return retResults;
        }
        #endregion
    }
}
