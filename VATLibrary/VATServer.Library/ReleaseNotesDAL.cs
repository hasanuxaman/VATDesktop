using System;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class ReleaseNotesDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        //

        public DataSet SearchNotes(SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettings");


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

                sqlText = @"SELECT [Id]
      ,[SL]
      ,[Version]
      ,[Date]
      ,[Name]
      ,[Issue]
      ,[Description]
  FROM [ReleaseNotes]

  select distinct [Version] from ReleaseNotes
";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
                dataAdapter.Fill(dataSet);

                #endregion
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("ReleaseNotesDAL", "SearchNotes", ex.ToString() + "\n" + sqlText);
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

            return dataSet;
        }

        public string NotesDataInsert(string sl, string version, string date, string name,string issue, string desc, SqlConnection currConn = null, SqlTransaction transaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(sl))
                {
                    throw new ArgumentNullException("settinNotesDataInsertgsDataInsert", "Please Input Settings Value");
                }

                if (string.IsNullOrEmpty(version))
                {
                    throw new ArgumentNullException("NotesDataInsert", "Please Input Settings Value");
                }

                if (string.IsNullOrEmpty(date))
                {
                    throw new ArgumentNullException("NotesDataInsert", "Please Input Settings Value");
                }

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("NotesDataInsert", "Please Input Settings Value");
                }

                if (string.IsNullOrEmpty(issue))
                {
                    throw new ArgumentNullException("NotesDataInsert", "Please Input Settings Value");
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

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion open connection and transaction

                #region NoteExist

                sqlText = "  ";
                sqlText += " select count(Id) from ReleaseNotes where [Version] = @Version and SL = @sl ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                cmdExist.Parameters.AddWithValue("@Version", version);
                cmdExist.Parameters.AddWithValue("@sl", sl);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("NotesDataInsert", "Please Input Settings Value");
                }

                #endregion NoteExist

                #region InsertNotes

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        @" INSERT INTO [dbo].[ReleaseNotes]
                            ([SL]
                        ,[Version]
                        ,[Date]
                        ,[Name]
                        ,[Issue]
                        ,[Description])";

                    sqlText += " VALUES(";
                    sqlText += " @sl,";
                    sqlText += " @version,";
                    sqlText += " @Date,";
                    sqlText += " @name,";
                    sqlText += " @issue,";
                    sqlText += " @desc";

                    sqlText += " )";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;

                    cmdExist1.Parameters.AddWithValue("@sl", sl);
                    cmdExist1.Parameters.AddWithValue("@version", version);
                    cmdExist1.Parameters.AddWithValue("@Date", date);
                    cmdExist1.Parameters.AddWithValue("@name", name);
                    cmdExist1.Parameters.AddWithValue("@issue", issue);
                    cmdExist1.Parameters.AddWithValue("@desc", desc);

                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("NotesDataInsert", "Please Input Settings Value");
                    }
                }
                #endregion 

                transaction.Commit();
            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("SearchNotes", "NotesDataInsert", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            finally
            {
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }
                //}
            }


            #endregion

            #region Results

            return retResults;
            #endregion

        }

        public void Insert()
        {
            //NotesDataInsert(string sl, string Releaseversion, string Releasedate, string ReleaseName,string ReleaseIssue, string ReleaseDesc)
            
            //NotesDataInsert("1.1-10/APR/2020", "1.1", "10/APR/2020", "Sale", "none", "none");

            NotesDataInsert("1", "1.1", "10/APR/2020", @"Sale", @"none", @"noneasdfadfa");
            NotesDataInsert("2", "1.2", "11/APR/2020", "Issue", "none","none");
            NotesDataInsert("3", "1.3", "10/APR/2020", "Receive", "none","none");
            NotesDataInsert("4", "1.1", "10/APR/2020", "Sale", "none","none");
            NotesDataInsert("5", "1.1", "10/APR/2020", "Purchase", "none","none");

            DeleteNotes("5");

        }

        //currConn to VcurrConn 25-Aug-2020
        public string DeleteNotes(string sl, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region open connection and transaction
                if (VcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }

                }

                if (Vtransaction == null)
                {
                    Vtransaction = VcurrConn.BeginTransaction();
                }

                #endregion open connection and transaction

                sqlText = "  ";
                sqlText += " DELETE FROM ReleaseNotes";
                sqlText += " WHERE SL=@sl ";



                SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                cmdExist1.Transaction = Vtransaction;

                cmdExist1.Parameters.AddWithValue("@sl", sl);

                object objfoundId1 = cmdExist1.ExecuteNonQuery();

                int save = (int)objfoundId1;
                retResults = save.ToString();
                if (save <= 0)
                {
                    throw new ArgumentNullException("Release Notes", "Please give sl");
                }

                Vtransaction.Commit();

            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message);//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("SearchNotes", "DeleteNotes", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

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

            return retResults;
            #endregion
        }


    }
}