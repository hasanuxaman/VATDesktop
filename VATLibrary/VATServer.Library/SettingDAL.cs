using SymphonySofttech.Utilities;
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
    public class SettingDAL : ISetting
    { 
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string EnKey = DBConstant.EnKey;
        private static string PassPhrase = DBConstant.PassPhrase;


        #endregion

        #region Methods

        public DataSet SearchSettings(SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettings");


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
                //SettingsUpdate
                #region sql statement

                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM Settings
                                      ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM Settings s ORDER BY s.SettingGroup;
";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
                dataAdapter.Fill(dataSet);

                #endregion
            }
            #region catch
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

            return dataSet;
        }

        public DataSet SearchSettingsMaster(SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettings");


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
                //SettingsUpdate
                #region sql statement

                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM SettingsMaster
                                      ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM SettingsMaster s ORDER BY s.SettingGroup;
";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
                dataAdapter.Fill(dataSet);

                #endregion
            }
            #region catch
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

            return dataSet;
        }

        public string[] SettingsUpdatelist(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null)
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

            bool iSTransSuccess = false;

            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction("UpdateToSettings");

                #endregion open connection and transaction
                //int tt = 0;

                if (settingsVM.Any())
                {
                    foreach (var item in settingsVM)
                    {
                        //tt++;
                        //Debug.WriteLine(tt.ToString());
                        #region Update Settings
                        sqlText = "";
                        sqlText += "update Settings set";
                        sqlText += " SettingValue=@itemSettingValue,";
                        sqlText += " ActiveStatus=@itemActiveStatus,";
                        //sqlText += " LastModifiedBy='" + UserInfoVM.UserName + "',";
                        sqlText += " LastModifiedBy= @LastModifiedBy,";
                        sqlText += " LastModifiedOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " where SettingGroup=@itemSettingGroup and SettingName=@itemSettingName";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;

                        cmdUpdate.Parameters.AddWithValue("@itemSettingValue", item.SettingValue);
                        cmdUpdate.Parameters.AddWithValue("@itemActiveStatus", item.ActiveStatus);
                        cmdUpdate.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                        cmdUpdate.Parameters.AddWithValue("@itemSettingName", item.SettingName);

                        //BugsBD
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", UserInfoVM.UserName);


                        transResult = (int)cmdUpdate.ExecuteNonQuery();

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                    }

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SettingsUpdate", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Requested Settings Information Successfully Updated.";
                    retResults[2] = "";
                }
                else
                {
                    transaction.Rollback();
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update settings.";
                    retResults[2] = "";
                }
                UpdateQuery();
            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }

        public string[] SettingsUpdatelistMaster(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null)
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

            bool iSTransSuccess = false;

            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction("UpdateToSettings");

                #endregion open connection and transaction
                //int tt = 0;

                if (settingsVM.Any())
                {
                    foreach (var item in settingsVM)
                    {
                        //tt++;
                        //Debug.WriteLine(tt.ToString());
                        #region Update Settings
                        sqlText = "";
                        sqlText += "update settingsmaster set";
                        sqlText += " SettingValue=@itemSettingValue,";
                        sqlText += " ActiveStatus=@itemActiveStatus,";
                        //sqlText += " LastModifiedBy='" + UserInfoVM.UserName + "',";
                        sqlText += " LastModifiedBy= @UserName,";
                        sqlText += " LastModifiedOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " where SettingGroup=@itemSettingGroup and SettingName=@itemSettingName";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;

                        cmdUpdate.Parameters.AddWithValue("@itemSettingValue", item.SettingValue);
                        cmdUpdate.Parameters.AddWithValue("@itemActiveStatus", item.ActiveStatus);
                        cmdUpdate.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                        cmdUpdate.Parameters.AddWithValue("@itemSettingName", item.SettingName);

                        //BugsBD
                        cmdUpdate.Parameters.AddWithValue("@UserName", UserInfoVM.UserName);

                        transResult = (int)cmdUpdate.ExecuteNonQuery();

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                    }

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SettingsUpdate", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Requested Settings Information Successfully Updated.";
                    retResults[2] = "";
                }
                else
                {
                    transaction.Rollback();
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update settings.";
                    retResults[2] = "";
                }
                UpdateQuery();
            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }

        public void UpdateQuery(SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            try
            {
                #region open connection and transaction
                #region Settings
                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }
                string VAT18_6Adjustment = new CommonDAL().settings("VAT9_1", "VAT18_6Adjustment", currConn, transaction);
                #endregion
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                transaction = currConn.BeginTransaction("UpdateItem");


                #endregion open connection and transaction

                #region Update


                sqlText = " ";
                #region Comments

                //////                sqlText = @"
                //////UPDATE BOMCompanyOverhead set VATName = 'VAT 4.3' WHERE VATName <> 'VAT 4.3'
                //////UPDATE BOMRaws set VATName = 'VAT 4.3' WHERE VATName <> 'VAT 4.3'
                //////UPDATE BOMs set VATName = 'VAT 4.3' WHERE VATName <> 'VAT 4.3'
                //////UPDATE ReceiveDetails set VATName = 'VAT 4.3' WHERE VATName <> 'VAT 4.3'
                //////UPDATE SalesInvoiceDetails set VATName = 'VAT 4.3' WHERE VATName <> 'VAT 4.3'
                //////";

                sqlText = sqlText + @"
                update Products set IsVDS='N'

                update Products set IsVDS='Y'
                where (VATRate>0 and VATRate<15) or IsFixedVAT='Y'

                 Update VATReturnV2Notes set Description='Due to VAT Deducted at Source by Supply receiver'
                 where NoteNo=24
                 
                 Update VATReturnV2Notes set Description='Remaining Balance (VAT) from Mushak-18.6,[Rule 118(5)]'
                 where NoteNo=54
                 
                 Update VATReturnV2Notes set Description='Remaining Balance (SD) from Mushak-18.6,[Rule 118(5)]'
                 where NoteNo=55

               Update VATReturnV2Notes set Description='Decreasing Adjustment for Note 54 (up to @VAT18_6Adjustment% of Note 34)'
                 where NoteNo=56

                 Update VATReturnV2Notes set Description='Decreasing Adjustment for Note 55 (up to @VAT18_6Adjustment% of Note 36)'
                 where NoteNo=57


                 Update PurchaseInvoiceDetails set AssessableValue=SubTotal
                 where  [AssessableValue]<=0 and InsuranceAmount<=0 and CnFAmount <=0

                ";

                #endregion
                sqlText = sqlText.Replace("@VAT18_6Adjustment", VAT18_6Adjustment);

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VAT18_6Adjustment", VAT18_6Adjustment);
                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion


                #region Commit


                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }
            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {
                transaction.Rollback();

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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


        }

        public void UserMenuAdd(SysDBInfoVMTemp connVM = null)
        {
            #region Objects and Variables

            SettingDAL settingDal = new SettingDAL();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string transResult = "";

            #endregion

            #region try

            try
            {
                #region Connection and Transaction

                currConn = _dbsqlConnection.GetConnectionNoTimeOut();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);

                #endregion

                #region UserMenuAdd
                transResult = settingDal.UserMenuAllRollsInsert("220110140", "SCBL/SCBLMIS/Sale Summary (AllShift)", "0", "rbtnSaleSummary", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnSaleSummary", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110150", "SCBL/SCBLMIS/Sale Summary by Product", "0", "rbtnSaleSummarybyProduct", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnSaleSummarybyProduct", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110160", "SCBL/SCBLMIS/Import Data", "0", "rbtnImportData", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnImportData", currConn, transaction);
                #endregion
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + "sqlText" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

        }

        #region Code Generation

        public string[] UpdateCode(string CodeGroup, string CodeName, int LastId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "0";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

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

                #region Update
                sqlText = "";

                sqlText = @"
Update Codes
SET LastId = @LastId
WHERE 1=1
AND CodeGroup=@CodeGroup
AND CodeName=@CodeName
";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@LastId", LastId);
                cmdUpdate.Parameters.AddWithValue("@CodeGroup", CodeGroup);
                cmdUpdate.Parameters.AddWithValue("@CodeName", CodeName);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[1] = "Requested Information successfully Executed";
                retResults[2] = "";
                retResults[3] = "";

                #endregion Commit



            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = "0"; //catch ex

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

            return retResults;
        }

        public string[] UpdateCodeGeneration(int BranchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "0";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

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

                #region Update
                sqlText = "";

                sqlText = @"
----declare @CurrentYear as varchar(4)
----declare @BranchId as varchar(4)


INSERT INTO CodeGenerations
SELECT distinct  @CurrentYear CurrentYear, @BranchId BranchId, Prefix, LastId 
FROM Codes
WHERE 1=1
AND Prefix NOT IN 
(
SELECT DISTINCT Prefix FROM CodeGenerations
WHERE 1=1
AND CurrentYear=@CurrentYear
AND BranchId=@BranchId
)


UPDATE CodeGenerations
SET CodeGenerations.LastId = Codes.LastId
FROM Codes
WHERE 1=1 
AND Codes.prefix = CodeGenerations.Prefix
AND CodeGenerations.CurrentYear = @CurrentYear
AND ISNULL(Codes.LastId,0) <> ISNULL(CodeGenerations.LastId,0)
AND BranchId=@BranchId

";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                string CurrentYear = DateTime.Now.ToString("yyyy");
                cmdUpdate.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                cmdUpdate.Parameters.AddWithValue("@BranchId", BranchId);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[1] = "Requested Information successfully Executed";
                retResults[2] = "";
                retResults[3] = "";

                #endregion Commit



            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[2] = "0"; //catch ex

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();

                }


                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

            return retResults;
        }




        private ResultVM CodeGeneration(int BranchId = 0, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string newIDCreate = "";
            string sqlText = "";
            CommonDAL commonDal = new CommonDAL();
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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction


                #region CodeGenerations

                #region Table Create


                sqlText = "";

                sqlText += @" 
SELECT * FROM sys.objects
WHERE object_id = OBJECT_ID(N'CodeGenerations') AND type in (N'U')
";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                DataTable dt = new DataTable();

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);




                if (dt != null && dt.Rows.Count > 0)
                {
                    rVM.Status = "Success";
                    rVM.Message = "Table Already Exist! No need to create!";
                    return rVM;
                }

                sqlText = "";
                sqlText = @"
CREATE TABLE [dbo].[CodeGenerations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrentYear] [varchar](4) NULL,
	[BranchId] [int] NULL,
	[Prefix] [varchar](3) NULL,
	[LastId] [int] NULL,
 CONSTRAINT [PK_CodeGenerations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                commonDal.NewTableAdd("CodeGenerations", sqlText, currConn, transaction);

                #endregion
                #endregion



                #region Sale

                int LastId = 0; string newId = "";
                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Other", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                                  "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);

                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-'))
                    {
                        LastId = Convert.ToInt32(newId.Split('-')[1]);
                        LastId = LastId - 1;
                    }

                    retResults = UpdateCode("Sale", "Other", LastId, currConn, transaction);
                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "RawSale", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                              "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "RawSale", LastId, currConn, transaction);
                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Trading", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-'))
                    {
                        LastId = Convert.ToInt32(newId.Split('-')[1]);
                        LastId = LastId - 1;
                    }

                    retResults = UpdateCode("Sale", "Trading", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("VAT11GaGa", "VAT11GaGa", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("VAT11GaGa", "VAT11GaGa", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Debit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Debit", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Credit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Credit", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Export", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Export", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("InternalIssue", "InternalIssue", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("InternalIssue", "InternalIssue", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Service", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Service", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "ServiceNS", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "ServiceNS", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Tender", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Tender", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("TollIssue", "TollIssue", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("TollIssue", "TollIssue", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("TollFinishIssue", "TollFinishIssue", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("TollFinishIssue", "TollFinishIssue", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Package", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Package", LastId, currConn, transaction);
                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "CommercialImporter", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                              "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "CommercialImporter", LastId, currConn, transaction);
                }
                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "Delivery", "SalesInvoiceHeaders", "DeliveryChallanNo",
                                                   "DeliveryDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "Delivery", LastId, currConn, transaction);
                }
                {
                    newIDCreate = commonDal.TransactionCodeX("SaleExport", "SaleExport", "SaleExports", "SaleExportNo",
                                             "SaleExportDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("SaleExport", "SaleExport", LastId, currConn, transaction);
                }
                #endregion
                #region bureau Sale


                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "ExportServiceNS", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "ExportServiceNS", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Sale", "ExportServiceNSCredit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Sale", "ExportServiceNSCredit", LastId, currConn, transaction);

                }

                #endregion
                #region DepositTDS

                {
                    // newIDCreate = commonDal.TransactionCodeX("TDS", "TDS", "DepositTDSs", "DepositId",
                    //                          "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"),currConn,transaction);
                    // LastId = 0; newId = "";
                    // if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    //if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    // retResults = UpdateCode("TDS", "TDS", LastId,currConn,transaction);
                }
                #endregion
                #region Despose


                {
                    newIDCreate = commonDal.TransactionCodeX("Dispose", "Raw", "DisposeHeaders", "DisposeNumber",
                                              "DisposeDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Dispose", "Raw", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Dispose", "Finish", "DisposeHeaders", "DisposeNumber",
                                             "DisposeDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Dispose", "Finish", LastId, currConn, transaction);
                }


                #endregion
                #region DutyDrawBack
                {
                    newIDCreate = commonDal.TransactionCodeX("DDB", "DDB", "DutyDrawBackHeader", "DDBackNo",
                                              "DDBackDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);

                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("DDB", "DDB", LastId, currConn, transaction);
                }
                #endregion
                #region Toll6_3Invoice
                {
                    newIDCreate = new CommonDAL().TransactionCodeX("Toll", "Invoice6_3", "Toll6_3Invoices", "TollNo",
                                              "TollDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);

                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Toll", "Invoice6_3", LastId, currConn, transaction);
                }
                #endregion
                #region Transfer
                {
                    newIDCreate = commonDal.TransactionCodeX("Transfer", "BTB", "Transfers", "TransferNo",
                                              "TransactionDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Transfer", "BTB", LastId, currConn, transaction);
                }
                #endregion
                #region TransferIssue

                {
                    newIDCreate = commonDal.TransactionCodeX("Transfer", "61Out", "TransferIssues", "TransferIssueNo",
                                              "TransactionDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Transfer", "61Out", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Transfer", "62Out", "TransferIssues", "TransferIssueNo",
                                              "TransactionDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Transfer", "62Out", LastId, currConn, transaction);
                }
                #endregion
                #region TransferRaw
                {
                    newIDCreate = commonDal.TransactionCodeX("TransferRaw", "TransferRaw", "TransferRawHeaders", "TransferId",
                        "TransferDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("TransferRaw", "TransferRaw", LastId, currConn, transaction);
                }
                #endregion
                #region TransferReceive

                {
                    newIDCreate = commonDal.TransactionCodeX("Transfer", "61In", "TransferReceives", "TransferReceiveNo",
                                              "TransactionDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);

                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Transfer", "61In", LastId, currConn, transaction);


                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Transfer", "62In", "TransferReceives", "TransferReceiveNo",
                                              "TransactionDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Transfer", "62In", LastId, currConn, transaction);



                }
                #endregion
                #region VAT7
                {
                    newIDCreate = commonDal.TransactionCodeX("VAT7", "VAT7", "VAT7", "VAT7No",
                        "Vat7Date", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("VAT7", "VAT7", LastId, currConn, transaction);
                }
                #endregion
                #region AdjustmentHistorys
                newIDCreate = commonDal.TransactionCodeX("Adjustment", "Both", "AdjustmentHistorys", "AdjHistoryNo",
                                              "AdjDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                LastId = 0; newId = "";
                if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                retResults = UpdateCode("Adjustment", "Both", LastId, currConn, transaction);
                #endregion
                #region Demand

                {
                    newIDCreate = commonDal.TransactionCodeX("Demand", "Other", "DemandHeaders", "DemandNo",
                                              "DemandDateTime ", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Demand", "Other", LastId, currConn, transaction);

                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Demand", "Receive", "DemandHeaders", "DemandNo",
                                              "DemandReceiveDate ", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Demand", "Receive", LastId, currConn, transaction);
                }
                #endregion
                #region Deposit
                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "Treasury", "Deposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "Treasury", LastId, currConn, transaction);
                }



                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "VDS", "Deposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "VDS", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "VDSSale", "Deposits", "DepositId",
                        "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "VDSSale", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "AdjCashPayble", "Deposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "AdjCashPayble", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "Treasury-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "Treasury-Credit", LastId, currConn, transaction);
                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "VDS-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "VDS-Credit", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Deposit", "AdjCashPayble-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Deposit", "AdjCashPayble-Credit", LastId, currConn, transaction);
                }
                #endregion
                #region IssueBom

                {
                    newIDCreate = commonDal.TransactionCodeX("Issue", "Other", "IssueHeaderBOMs", "IssueNo",
                                              "IssueDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Issue", "Other", LastId, currConn, transaction);


                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Issue", "IssueReturn", "IssueHeaderBOMs", "IssueNo",
                                              "IssueDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Issue", "IssueReturn", LastId, currConn, transaction);


                }
                #endregion

                #region IssueHeaders

                {
                    newIDCreate = commonDal.TransactionCodeX("Issue", "Other", "IssueHeaders", "IssueNo",
                        "IssueDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Issue", "Other", LastId, currConn, transaction);


                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Issue", "IssueReturn", "IssueHeaders", "IssueNo",
                        "IssueDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Issue", "IssueReturn", LastId, currConn, transaction);


                }
                #endregion

                #region Receive

                {
                    newIDCreate = commonDal.TransactionCodeX("Receive", "Other", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Receive", "Other", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Receive", "WIP", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Receive", "WIP", LastId, currConn, transaction);
                }
                {
                    {
                        newIDCreate = commonDal.TransactionCodeX("Receive", "Package", "ReceiveHeaders", "ReceiveNo",
                                                  "ReceiveDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                        LastId = 0; newId = "";
                        if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                        if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                        retResults = UpdateCode("Receive", "Package", LastId, currConn, transaction);
                    }
                }
                {
                    newIDCreate = commonDal.TransactionCodeX("TollFinishReceive", "TollFinishReceive", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("TollFinishReceive", "TollFinishReceive", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Receive", "ReceiveReturn", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Receive", "ReceiveReturn", LastId, currConn, transaction);
                }
                #endregion
                #region SDDeposit

                {
                    newIDCreate = commonDal.TransactionCodeX("SDDeposit", "Treasury", "SDDeposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("SDDeposit", "Treasury", LastId, currConn, transaction);
                }

                {
                    newIDCreate = commonDal.TransactionCodeX("SDDeposit", "Treasury-Credit", "SDDeposits", "DepositId",
                                             "DepositDateTime", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("SDDeposit", "Treasury-Credit", LastId, currConn, transaction);
                }
                #endregion
                #region Purchase

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "Other", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "Other", LastId, currConn, transaction);


                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "PurchaseDN", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "PurchaseDN", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "PurchaseCN", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "PurchaseCN", LastId, currConn, transaction);

                }


                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "Trading", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "Trading", LastId, currConn, transaction);

                }


                {
                    newIDCreate = commonDal.TransactionCodeX("TollReceive", "TollReceive", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                             "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("TollReceive", "TollReceive", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "Import", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "Import", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "InputService", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "InputService", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("TollReceiveRaw", "TollReceiveRaw", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("TollReceiveRaw", "TollReceiveRaw", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "PurchaseReturn", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "PurchaseReturn", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "Service", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "Service", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "ServiceNS", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "ServiceNS", LastId, currConn, transaction);

                }

                {
                    newIDCreate = commonDal.TransactionCodeX("Purchase", "CommercialImporter", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", DateTime.Now.ToString("dd/MMM/yyyy"), currConn, transaction);
                    LastId = 0; newId = "";
                    if (newIDCreate.Contains('/')) { newId = newIDCreate.Split('/')[0]; }
                    if (newId.Contains('-')) { LastId = Convert.ToInt32(newId.Split('-')[1]); LastId = LastId - 1; }

                    retResults = UpdateCode("Purchase", "CommercialImporter", LastId, currConn, transaction);

                }
                #endregion
                #region Update Code Generation
                retResults = UpdateCodeGeneration(BranchId, currConn, transaction);

                #endregion

                if (transaction != null)
                {
                    transaction.Commit();
                }

                rVM.Status = "Success";
                rVM.Message = "All Data Update Successfully!";

            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;
                if (transaction != null) transaction.Rollback();
                throw ex;

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
            return rVM;
        }

        #endregion

        #region Temporary Field Add / CommonDAL was Checked Out / Temporarily Code is Kept Here

        public void TemporaryFieldAdd(SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            try
            {
                currConn = _dbsqlConnection.GetConnectionNoTimeOut();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);


                if (transResult < 0)
                {
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + "sqlText" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
        }

        #endregion

        public void SettingsUpdate(string companyId, int BranchId = 0, SysDBInfoVMTemp connVM = null, Version version = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            #endregion

            try
            {
                CommonDAL commonDal = new CommonDAL();
                commonDal.DatabaseTableChanges();

                #region Temporary

                //////TemporaryFieldAdd();

                #endregion

                CodeGeneration(BranchId);

                #region Update Query

                UpdateQuery();

                #endregion

                #region UserMenuAdd
                //UserMenuAdd();

                if (version != null)
                {
                    commonDal.Update_AppVersion(version, null, null);
                }
                #endregion

                SaleDAL sdal = new SaleDAL();

                #region Security 20140101
                commonDal.SetSecurity(companyId);
                #endregion Security 20140101

            }
            #region catch
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion

        }

        private void IssuePriceUpdate(SqlConnection currConn, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlTransaction transaction = null;
            int transResult = 0;

            try
            {
            #endregion

                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);

                }

                #endregion open connection and transaction
                #region find Null
                #region Update if Null

                sqlText = "  ";
                sqlText +=
                    " select  count(distinct Itemno) from IssueDetails WHERE UOMQty IS NULL ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                transResult = (int)cmdExist.ExecuteScalar();
                if (transResult > 0)
                {
                    #region Update if Null

                    sqlText = "  ";
                    sqlText +=
                        " UPDATE IssueDetails SET UOMc = 1, UOMQty = Quantity,uomn=UOM,UOMPrice = CostPrice WHERE UOMQty IS NULL ";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    transResult = (int)cmdExist1.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                        MessageVM.receiveMsgSaveNotSuccessfully);
                    }

                    #endregion ProductExist
                }

                #endregion ProductExist
                #endregion find Null


                #region

                sqlText = "";
                sqlText +=
                    "   SELECT IssueNo,ItemNo,IssueLineNo,isnull(UOMPrice,0)UOMPrice,isnull(CostPrice,0)CostPrice,isnull(uomc,0)uomc,isnull(SubTotal,0)SubTotal,IssueDateTime,isnull(Quantity,0)Quantity";
                sqlText += " FROM IssueDetails ";
                //sqlText += " where  IssueNo='REC-0034/0713' ";

                DataTable dataTable = new DataTable("RIFB");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                reportDataAdapt.Fill(dataTable);

                if (dataTable != null || dataTable.Rows.Count > 0)
                {

                    ProductDAL productDal = new ProductDAL();
                    foreach (DataRow BRItem in dataTable.Rows)
                    {

                        string vIssueNo = BRItem["IssueNo"].ToString();
                        string vItemNo = BRItem["ItemNo"].ToString();
                        string vIssueLineNo = BRItem["IssueLineNo"].ToString();
                        //DateTime vIssueDateTime =Convert.ToDateTime(BRItem["IssueDateTime"].ToString());
                        string vIssueDateTime = BRItem["IssueDateTime"].ToString();

                        decimal vSubTotal = Convert.ToDecimal(BRItem["SubTotal"].ToString());
                        decimal vUomc = Convert.ToDecimal(BRItem["uomc"].ToString());
                        decimal vCostPrice = Convert.ToDecimal(BRItem["CostPrice"].ToString());
                        decimal vUOMPrice = Convert.ToDecimal(BRItem["UOMPrice"].ToString());
                        decimal vQuantity = Convert.ToDecimal(BRItem["Quantity"].ToString());
                        //decimal vUOMPrice1 = productDal.AvgPrice(vItemNo, vIssueDateTime, currConn, transaction);
                        decimal vUOMPrice1 = 0;
                        DataTable priceData = productDal.AvgPriceNew(vItemNo, vIssueDateTime, null, null, false,true,true,true,connVM,UserId);


                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                        if (quantity > 0)
                        {
                            vUOMPrice1 = amount / quantity;
                        }
                        else
                        {
                            vUOMPrice1 = 0;
                        }

                        decimal vCostPrice1 = vUOMPrice1 * vUomc;
                        decimal vSubTotal1 = vCostPrice1 * vQuantity;

                        #region Update UnitCost

                        sqlText = "  ";
                        sqlText += " UPDATE IssueDetails SET ";
                        sqlText += " UOMPrice='" + vUOMPrice1 + "',";
                        sqlText += " CostPrice='" + vCostPrice1 + "',";
                        sqlText += " SubTotal = '" + vSubTotal1 + "'";
                        sqlText += " WHERE IssueNo='" + vIssueNo + "'";
                        sqlText += " AND ItemNo='" + vItemNo + "'";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;
                        transResult = (int)cmdInsert.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgSaveNotSuccessfully);
                        }
                        #region Update Issue Header

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo= '" + vIssueNo + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgUnableToSaveIssue);
                        }

                        #endregion Update Issue Header
                        #endregion Update UnitCost


                    }
                }

                #endregion
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

            }
            #region Catch and Finall
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
        }

        #endregion

        public string UpdateInternalIssueValue(SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string result = "";
            CommonDAL commDal = new CommonDAL();


            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                #endregion open connection and transaction

                #region sql statement
                #region Issue

                sqlText = @"SELECT [ItemNo]
                                  ,[IssueDateTime]
                                  ,[IssueNo]
                                  ,[Quantity]
                                  ,[Transactiontype]
                                   from IssueDetails where Transactiontype = 'InternalIssue' and IssueDateTime > '2013-10-08';
                                ";

                DataTable dt = new DataTable();
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                SqlDataAdapter adptInterIssue = new SqlDataAdapter(cmdIdExist);
                adptInterIssue.Fill(dt);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    //throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                }

                else
                {
                    ProductDAL proDal = new ProductDAL();
                    string itemNo, issueDate, issueNo = string.Empty;
                    DateTime issueDateTime = DateTime.Now;
                    DataTable avgPriceData = new DataTable();
                    decimal AvgRate, NBRPrice, Qty = 0;

                    foreach (DataRow item in dt.Rows)
                    {
                        itemNo = item["ItemNo"].ToString();
                        issueDateTime = Convert.ToDateTime(item["IssueDateTime"].ToString());
                        issueNo = item["IssueNo"].ToString();
                        Qty = Convert.ToDecimal(item["Quantity"].ToString());

                        #region Find Avg
                        issueDate = issueDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                        avgPriceData = proDal.AvgPriceNew(itemNo, issueDate, currConn, transaction, false,true,true,true,connVM,UserId);
                        decimal amount = Convert.ToDecimal(avgPriceData.Rows[0]["Amount"].ToString());
                        decimal quantity = Convert.ToDecimal(avgPriceData.Rows[0]["Quantity"].ToString());

                        if (quantity > 0)
                        {
                            AvgRate = amount / quantity;
                        }
                        else
                        {
                            AvgRate = 0;
                        }
                        #endregion Find Avg

                        #region Issue Settings

                        int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                        int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                        Qty = FormatingNumeric(Qty, IssuePlaceQty);

                        #endregion Issue Settings


                        #region Find NBR Price
                        //NBRPrice = proDal.GetLastNBRPriceFromBOM(itemNo, "VAT 4.3 (Internal Issue)", issueDate, currConn, transaction);
                        #endregion Find NBR Price

                        #region Update Issue Details

                        sqlText = "";
                        sqlText += " UPDATE IssueDetails SET NBRPrice ='0', ";
                        sqlText += " CostPrice ='" + AvgRate + "', ";
                        sqlText += " SubTotal ='" + FormatingNumeric(AvgRate * Qty, IssuePlaceAmt) + "', ";
                        sqlText += " UOMPrice ='" + AvgRate + "' ";
                        sqlText += " where ItemNo='" + itemNo + "' and IssueNo='" + issueNo + "'  and  Transactiontype = 'InternalIssue' ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }

                        #endregion Update Issue Details
                    }
                }
                #endregion  Issue
                #region Receive

                sqlText = " ";
                sqlText += @"SELECT [ItemNo]
                                  ,[ReceiveNo]
                                  ,[CostPrice]
                                  ,[Quantity]
                                  ,[Transactiontype]
                                   from ReceiveDetails where Transactiontype = 'InternalIssue'
                                ";

                dt = new DataTable();
                cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                SqlDataAdapter adptInterRecei = new SqlDataAdapter(cmdIdExist);
                adptInterRecei.Fill(dt);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    //throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                }

                else
                {
                    string itemNo, receiveNo = string.Empty;
                    decimal CostPrice, Qty = 0;




                    foreach (DataRow item in dt.Rows)
                    {
                        itemNo = item["ItemNo"].ToString();
                        receiveNo = item["ReceiveNo"].ToString();
                        Qty = Convert.ToDecimal(item["Quantity"].ToString());
                        CostPrice = Convert.ToDecimal(item["CostPrice"].ToString());

                        int ReceivePlaceQty = Convert.ToInt32(commDal.settings("Receive", "Quantity"));
                        int ReceivePlaceAmt = Convert.ToInt32(commDal.settings("Receive", "Amount"));
                        CostPrice = FormatingNumeric(CostPrice, ReceivePlaceAmt);
                        Qty = FormatingNumeric(Qty, ReceivePlaceQty);

                        #region Update Receive Details
                        sqlText = "";
                        sqlText += " UPDATE ReceiveDetails SET ";
                        sqlText += " SubTotal ='" + FormatingNumeric(CostPrice * Qty, ReceivePlaceAmt) + "' ";
                        sqlText += " where ItemNo='" + itemNo + "' and ReceiveNo='" + receiveNo + "'  and  Transactiontype = 'InternalIssue' ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                    }

                        #endregion Update Receive Details
                }

                #endregion Receive

                #endregion

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        result = "Success";
                    }
                }

                #endregion Commit
            }
            #region catch

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

            return result;
        }

        public decimal FormatingNumeric(decimal value, int DecPlace, SysDBInfoVMTemp connVM = null)
        {
            object outPutValue = 0;
            string decPointLen = "";
            try
            {

                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }
                if (value < 1000)
                {
                    var a = "0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    outPutValue = value.ToString(a);

                }


            }
            #region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            catch (Exception ex)
            {
                //string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            #endregion Catch

            return Convert.ToDecimal(outPutValue);
        }

        #region web methods
        public List<SettingsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SettingsVM> VMs = new List<SettingsVM>();
            SettingsVM vm;
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
 SettingId
,SettingGroup
,SettingName
,SettingValue
,SettingType
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
FROM Settings
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlText += @" and SettingId=@SettingId";
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
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]))
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
                    objComm.Parameters.AddWithValue("@SettingId", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettingsVM();
                    vm.SettingId = dr["SettingId"].ToString();
                    vm.SettingGroup = dr["SettingGroup"].ToString();
                    vm.SettingName = dr["SettingName"].ToString();
                    vm.SettingValue = dr["SettingValue"].ToString();
                    vm.SettingType = dr["SettingType"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
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

        public List<SettingsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SettingsVM> VMs = new List<SettingsVM>();
            SettingsVM vm;
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
 SettingId
,SettingGroup
,SettingName
,SettingValue
,SettingType
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
FROM SettingsMaster
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlText += @" and SettingId=@SettingId";
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
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]))
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
                    objComm.Parameters.AddWithValue("@SettingId", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettingsVM();
                    vm.SettingId = dr["SettingId"].ToString();
                    vm.SettingGroup = dr["SettingGroup"].ToString();
                    vm.SettingName = dr["SettingName"].ToString();
                    vm.SettingValue = dr["SettingValue"].ToString();
                    vm.SettingType = dr["SettingType"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
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

        public string[] settingsDataUpdate(SettingsVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Bank Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            int nextId = 0;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToBank"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update Settings set";
                    sqlText += " SettingValue=@SettingValue";
                    sqlText += " where SettingGroup=@SettingGroup and  SettingName=@SettingName";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@SettingGroup", vm.SettingGroup.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingName", vm.SettingName.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingValue", vm.SettingValue.Trim());
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", BankVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Setting Update", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update Setting.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region Catch and Finally
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex
                if (VcurrConn == null) { transaction.Rollback(); }
                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
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
            #endregion
            return retResults;
        }

        public List<SettingsVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SettingsVM> VMs = new List<SettingsVM>();
            SettingsVM vm;
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
SELECT distinct SettingGroup from Settings union select 'AllGroup' SettingGroup from Settings
WHERE  1=1
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettingsVM();
                    vm.SettingGroup = dr["SettingGroup"].ToString(); ;
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

        public List<SettingsVM> DropDownSettingMaster(SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SettingsVM> VMs = new List<SettingsVM>();
            SettingsVM vm;
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
SELECT distinct SettingGroup from SettingsMaster union select 'AllGroup' SettingGroup from SettingsMaster
WHERE  1=1
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettingsVM();
                    vm.SettingGroup = dr["SettingGroup"].ToString(); ;
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
        #endregion

        #region unused
        public string ProductCategoryDataInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CategoryID)CategoryID FROM ProductCategories ";
                sqlText += " WHERE CategoryID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";

                    sqlText += " INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'0', N'NA', N'NA', N'NA', N'Overhead', N'0.00', CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'admin', CAST(0x0000A16400F8CA3C AS DateTime), N'admin', CAST(0x0000A1A30106ECFC AS DateTime), CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'NA', N'NA')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
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
        public string ProductDataInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT ItemNo)ItemNo FROM Products ";
                sqlText += " WHERE ItemNo='ovh0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";

                    sqlText += " INSERT [dbo].[Products] ([ItemNo], [ProductCode], [ProductName], [ProductDescription], [CategoryID], [UOM], [CostPrice], [SalesPrice], [NBRPrice], [ReceivePrice], [IssuePrice], [TenderPrice], [ExportPrice], [InternalIssuePrice], [TollIssuePrice], [TollCharge], [OpeningBalance], [SerialNo], [HSCodeNo], [VATRate], [Comments], [SD], [PacketPrice], [Trading], [TradingMarkUp], [NonStock], [QuantityInHand], [OpeningDate], [RebatePercent], [TVBRate], [CnFRate], [InsuranceRate], [CDRate], [RDRate], [AITRate], [TVARate], [ATVRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [OpeningTotalCost]) VALUES (N'ovh0', N'ovh0', N'Margin', N'-', N'0', N'-', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'-', N'', CAST(0.000000000 AS Decimal(25, 9)), N'', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), CAST(0x0000A1A40105ED84 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Y', N'admin', CAST(0x0000A1A401060044 AS DateTime), N'admin', CAST(0x0000A1A401224A74 AS DateTime), NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
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
        public string BankDataInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT BankID)BankID FROM BankInformations ";
                sqlText += " WHERE BankID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [BankInformations] ([BankID], [BankCode], [BankName], [BranchName], [AccountNumber], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'0', N'NA', N'NA', N'NA', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'Y', N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), NULL, NULL, NULL, NULL, NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
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
        public string VendorGroupInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VendorGroupID)BankID FROM VendorGroups ";
                sqlText += " WHERE VendorGroupID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input VendorGroups Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A''N/A', N'N/A', NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input VendorGroups Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input VendorGroups Value");
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
        public string VendorInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VendorID)VendorID FROM Vendors ";
                sqlText += " WHERE VendorID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input Vendors Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Vendors] ([VendorID], [VendorCode], [VendorName], [VendorGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [VATRegistrationNo], [TINNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Country], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vendors Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vendors Value");
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
        public string CustomerGroupInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CustomerGroupID)CustomerGroupID FROM CustomerGroups ";
                sqlText += " WHERE CustomerGroupID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input CustomerGroups Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'N/A', N'N/A', N'Local', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A17500C8DF0C AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input CustomerGroups Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input CustomerGroups Value");
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
        public string CustomerInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CustomerID)CustomerID FROM Customers ";
                sqlText += " WHERE CustomerID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input Customers Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [CustomerGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [TINNo], [VATRegistrationNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info2], [Info3], [Info4], [Info5], [Country]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Customers Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Customers Value");
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
        public string VehicleInsert(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VehicleID)VehicleID FROM Vehicles ";
                sqlText += " WHERE VehicleID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input Vehicles Value");
                }
                #endregion ProductExist
                #region Insert

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vehicles Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vehicles Value");
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
        public string UpdateTablesData(SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

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


                #region Last Settings


                //sqlText += " UPDATE ProductCategories SET 	IsRaw ='Service' where IsRaw = 'Non Stock' ";
                sqlText += " UPDATE AdjustmentHistorys SET 	AdjType ='Credit Payable' where AdjType = 'Credit Payble' ";
                sqlText += " UPDATE AdjustmentHistorys SET 	AdjType ='Cash Payable' where AdjType = 'Cash Payble'";



                SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                cmdExist1.Transaction = transaction;
                object objfoundId1 = cmdExist1.ExecuteNonQuery();
                if (objfoundId1 == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                int save = (int)objfoundId1;
                if (save <= 0)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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
        public string settingsDataUpdate(string settingGroup, string settingName, string settingGroupNew, string settingNameNew, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE SettingGroup='" + settingGroup + "' AND SettingName='" + settingName + "' ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Price

                int foundId = (int)objfoundId;
                if (foundId > 0)
                {
                    sqlText = "";
                    sqlText = "update Settings set";
                    sqlText += " SettingName='" + settingNameNew + "',";
                    sqlText += " SettingValue='" + settingGroupNew + "'";
                    sqlText += " where SettingGroup='" + settingGroup + "' and SettingName='" + settingName + "'";

                    //sqlText += " where SettingId='" + item.SettingId + "'" + " and SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "'";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    object objfoundId1 = cmdUpdate.ExecuteNonQuery();

                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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
        public bool CheckUserAccess(SysDBInfoVMTemp connVM = null)
        {
            bool isAlloweduser = false;
            CommonDAL commonDal = new CommonDAL();

            bool isAccessTransaction =
                Convert.ToBoolean(commonDal.settings("Transaction", "AccessTransaction") == "Y" ? true : false);
            if (!isAccessTransaction)
            {
                string userName = commonDal.settings("Transaction", "AccessUser");
                if (userName.ToLower() == UserInfoVM.UserName.ToLower())
                {
                    isAlloweduser = true;
                }
            }
            else
            {
                isAlloweduser = true;
            }
            return isAlloweduser;
        }

        #endregion

        #region need to parameterize

        //currConn to VcurrConn 25-Aug-2020
        public string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingType))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingValue))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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

                #region SettingsExist

                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE SettingGroup=@settingGroup AND SettingName=@settingName AND SettingType=@settingType";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;

                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingType", settingType);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }

                #endregion ProductExist

                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        " INSERT INTO Settings(	SettingGroup,	SettingName,SettingValue,SettingType,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += " @settingGroup,";
                    sqlText += " @settingName,";
                    sqlText += " @settingValue,";
                    sqlText += " @settingType,";
                    sqlText += " 'Y',";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    sqlText += " )";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;

                    cmdExist1.Parameters.AddWithValue("@settingName", settingName);
                    cmdExist1.Parameters.AddWithValue("@settingValue", settingValue);
                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup);
                    cmdExist1.Parameters.AddWithValue("@settingType", settingType);

                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                }
                #endregion Last Price
                #region insert data into settingRole table

                //if (!string.IsNullOrEmpty(userId))
                //{

                //    SettingRoleDAL settingRoleDal = new SettingRoleDAL();
                //    settingRoleDal.settingsDataInsert(settingGroup, settingName, settingType, settingValue, userId,
                //                                      currConn, transaction);
                //}

                #endregion

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
                //if (currConn == null)
                //{
                if (VcurrConn.State == ConnectionState.Open)
                {
                    VcurrConn.Close();

                }
                //}
            }


            #endregion

            #region Results

            return retResults;
            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string settingsDataDelete(string settingGroup, string settingName, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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

                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE REPLACE(SettingGroup,' ','')=@settingGroup AND SettingName=@settingName";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;

                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " DELETE FROM Settings";
                    sqlText += " WHERE REPLACE(SettingGroup,' ','')=@settingGroup ";
                    sqlText += " AND SettingName=@settingName";


                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;

                    cmdExist1.Parameters.AddWithValue("@settingName", settingName);
                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup);

                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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

        public DataTable SearchUserMenuAllRolls(SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataTable dataSet = new DataTable("SearchUserMenuAllRolls");


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

                sqlText = @"SELECT *
                                      FROM UserMenuAllFinalRolls
                                      ORDER BY FormID;
";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
                dataAdapter.Fill(dataSet);

                #endregion
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

            return dataSet;
        }

        //currConn to VcurrConn 25-Aug-2020
        public string UserMenuAllRollsInsert(string FormId, string FormName, string Access, string RibbonName
            , string AccessType, int Len, string TabName, string PanelName, string ButtonName, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                if (string.IsNullOrEmpty(FormId))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(FormName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(Access))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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


                #region SettingsExist

                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT FormID)FormID FROM UserMenuAllFinalRolls ";
                sqlText += " WHERE FormID=@FormID";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                cmdExist.Parameters.AddWithValue("@FormID", FormId);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("UserMenuAllFinalRolls", "Please Input UserMenuAllFinalRolls Value");
                }

                #endregion ProductExist
                #region
                var tt = FormId + "~" + Access;
                string AccessRoll = Converter.DESEncrypt(PassPhrase, EnKey, tt);
                #endregion
                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        @" INSERT INTO UserMenuAllFinalRolls(
 FormId
,FormName
,RibbonName
,Access
,AccessRoll
,AccessType
,Len
,TabName
,PanelName
,ButtonName
,LastModifiedBy
,LastModifiedOn
)";
                    sqlText += " VALUES(";
                    sqlText += " @FormId,";
                    sqlText += " @FormName,";
                    sqlText += " @RibbonName,";
                    sqlText += " @Access,";
                    sqlText += " @AccessRoll,";
                    sqlText += " @AccessType,";
                    sqlText += " @Len,";
                    sqlText += " @TabName,";
                    sqlText += " @PanelName,";
                    sqlText += " @ButtonName,";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    sqlText += " )";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;

                    cmdExist1.Parameters.AddWithValue("@FormId", FormId);
                    cmdExist1.Parameters.AddWithValue("@FormName", FormName);
                    cmdExist1.Parameters.AddWithValue("@RibbonName", RibbonName);
                    cmdExist1.Parameters.AddWithValue("@Access", Access);
                    cmdExist1.Parameters.AddWithValue("@AccessRoll", AccessRoll);
                    cmdExist1.Parameters.AddWithValue("@AccessType", AccessType);
                    cmdExist1.Parameters.AddWithValue("@Len", Len);
                    cmdExist1.Parameters.AddWithValue("@TabName", TabName);
                    cmdExist1.Parameters.AddWithValue("@PanelName", PanelName);
                    cmdExist1.Parameters.AddWithValue("@ButtonName", ButtonName);

                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("UserMenuAllFinalRolls", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("UserMenuAllFinalRolls", "Please Input Settings Value");
                    }
                    if (Vtransaction == null && transaction != null)
                    {
                        transaction.Commit();
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
        public string UserMenuAllRollsDelete(string FormID, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FormID))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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

                #region SettingsExist

                #endregion ProductExist
                #region Last Settings

                sqlText = "  ";
                sqlText += " DELETE FROM UserMenuAllFinalRolls";
                sqlText += " WHERE FormID=@FormID";

                SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                cmdExist1.Transaction = Vtransaction;

                cmdExist1.Parameters.AddWithValue("@FormID", FormID);

                object objfoundId1 = cmdExist1.ExecuteNonQuery();




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

        //currConn to VcurrConn 25-Aug-2020
        public string settingsDataDeleteBulk(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation


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


                sqlText = "";
                sqlText += @"  

    Update SettingsRole set SettingName='A4VAT4_3Digit7' where SettingName='A4VAT1Digit7' and  SettingGroup ='BOM'
    Update SettingsRole set SettingName='LegalVAT4_3Digit7' where SettingName='LegalVAT1Digit7' and  SettingGroup ='BOM'
    Update SettingsRole set SettingName='VAT4_3(TollIssue)WithRaw' where SettingName='VAT1(TollIssue)WithRaw' and  SettingGroup ='BOM'
    Update SettingsRole set SettingName='VAT4_3Digit7' where SettingName='VAT1Digit7' and  SettingGroup ='BOM'
    Update SettingsRole set SettingName='LocalInVAT4_3' where SettingName='LocalInVAT1' and  SettingGroup ='PriceDeclaration'
    Update SettingsRole set SettingName='LocalInVAT4_3Ka(Tarrif)' where SettingName='LocalInVAT1Ka(Tarrif)' and  SettingGroup ='PriceDeclaration'
    Update SettingsRole set SettingName='TenderInVAT4_3' where SettingName='TenderInVAT1' and  SettingGroup ='PriceDeclaration'
    Update SettingsRole set SettingName='TenderInVAT4_3(Tender)' where SettingName='TenderInVAT1(Tender)' and  SettingGroup ='PriceDeclaration'
    Update SettingsRole set SettingName='VAT6_3SCBLA5' where SettingName='VAT11SCBLA5' and  SettingGroup ='Reports'
    Update SettingsRole set SettingName='VAT6_3English' where SettingName='VAT11English' and  SettingGroup ='Sale'
    Update SettingsRole set SettingName='VAT6_3Letter' where SettingName='VAT11Letter' and  SettingGroup ='Sale'
    Update SettingsRole set SettingName='VAT6_3Legal' where SettingName='VAT11Legal' and  SettingGroup ='Sale'
    Update SettingsRole set SettingName='VAT6_3A5' where SettingName='VAT11A5' and  SettingGroup ='Sale'
    Update SettingsRole set SettingName='VAT6_3A4' where SettingName='VAT11A4' and  SettingGroup ='Sale'
    Update SettingsRole set SettingName='VAT6_3' where SettingName='VAT11' and  SettingGroup ='Reports'
    Update SettingsRole set SettingName='AttachedWithVAT6_1' where SettingName='AttachedWithVAT16' and  SettingGroup ='TollItemReceive'
    Update SettingsRole set SettingGroup='VAT6_1' where SettingName='AutoAdjustment' and  SettingGroup ='VAT16'
    Update SettingsRole set SettingGroup='VAT6_1' where SettingName='Report3Digits' and  SettingGroup ='VAT16'
    Update SettingsRole set SettingGroup='VAT6_2' where SettingName='AutoAdjustment' and  SettingGroup ='VAT17'
    Update SettingsRole set SettingGroup='VAT6_2' where SettingName='Report3Digits' and  SettingGroup ='VAT17'
    Update SettingsRole set SettingGroup='VAT9_1' where SettingName='ExportInBDT' and  SettingGroup ='VAT19'

        Update Settings set SettingName='A4VAT4_3Digit7' where SettingName='A4VAT1Digit7' and  SettingGroup ='BOM'
        Update Settings set SettingName='LegalVAT4_3Digit7' where SettingName='LegalVAT1Digit7' and  SettingGroup ='BOM'
        Update Settings set SettingName='VAT4_3(TollIssue)WithRaw' where SettingName='VAT1(TollIssue)WithRaw' and  SettingGroup ='BOM'
        Update Settings set SettingName='VAT4_3Digit7' where SettingName='VAT1Digit7' and  SettingGroup ='BOM'
        Update Settings set SettingName='LocalInVAT4_3' where SettingName='LocalInVAT1' and  SettingGroup ='PriceDeclaration'
        Update Settings set SettingName='LocalInVAT4_3Ka(Tarrif)' where SettingName='LocalInVAT1Ka(Tarrif)' and  SettingGroup ='PriceDeclaration'
        Update Settings set SettingName='TenderInVAT4_3' where SettingName='TenderInVAT1' and  SettingGroup ='PriceDeclaration'
        Update Settings set SettingName='TenderInVAT4_3(Tender)' where SettingName='TenderInVAT1(Tender)' and  SettingGroup ='PriceDeclaration'
        Update Settings set SettingName='VAT6_3SCBLA5' where SettingName='VAT11SCBLA5' and  SettingGroup ='Reports'
        Update Settings set SettingName='VAT6_3English' where SettingName='VAT11English' and  SettingGroup ='Sale'
        Update Settings set SettingName='VAT6_3Letter' where SettingName='VAT11Letter' and  SettingGroup ='Sale'
        Update Settings set SettingName='VAT6_3Legal' where SettingName='VAT11Legal' and  SettingGroup ='Sale'
        Update Settings set SettingName='VAT6_3A5' where SettingName='VAT11A5' and  SettingGroup ='Sale'
        Update Settings set SettingName='VAT6_3A4' where SettingName='VAT11A4' and  SettingGroup ='Sale'
        Update Settings set SettingName='VAT6_3' where SettingName='VAT11' and  SettingGroup ='Reports'
        Update Settings set SettingName='AttachedWithVAT6_1' where SettingName='AttachedWithVAT16' and  SettingGroup ='TollItemReceive'
        Update Settings set SettingGroup='VAT6_1' where SettingName='AutoAdjustment' and  SettingGroup ='VAT16'
        Update Settings set SettingGroup='VAT6_1' where SettingName='Report3Digits' and  SettingGroup ='VAT16'
        Update Settings set SettingGroup='VAT6_2' where SettingName='AutoAdjustment' and  SettingGroup ='VAT17'
        Update Settings set SettingGroup='VAT6_2' where SettingName='Report3Digits' and  SettingGroup ='VAT17'
        Update Settings set SettingGroup='VAT9_1' where SettingName='ExportInBDT' and  SettingGroup ='VAT19'

    ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, VcurrConn);
                cmdUpdate.Transaction = Vtransaction;

                cmdUpdate.ExecuteNonQuery();

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

        //settingsDataInsert1("110110110", "Setup/ItemInformation/Group", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110110120", "Setup/ItemInformation/Product", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110110130", "Setup/ItemInformation/Overhead", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110110140", "Setup/ItemInformation/TDS", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110110150", "Setup/ItemInformation/HSCode", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110120110", "Setup/Vedor/Group", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110120120", "Setup/Vedor/Vendor", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110130110", "Setup/Customer/Group", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110130120", "Setup/Customer/Customer", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110140110", "Setup/BankVehicle/Bank", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110140120", "Setup/BankVehicle/Vehicle", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110150110", "Setup/PriceDeclaration/BOM", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110150120", "Setup/PriceDeclaration/Service", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110150130", "Setup/PriceDeclaration/Tender", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110160110", "Setup/Company/CommpanyProfile", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110160120", "Setup/Company/BranchProfile", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110170110", "Setup/FiscalYear/FiscalYear", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110180110", "Setup/Configuration/Settings", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110180120", "Setup/Configuration/Prefix", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110180130", "Setup/Configuration/Shift", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110190110", "Setup/ImportSync/Import", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110190120", "Setup/ImportSync/Sync", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110190130", "Setup/ImportSync/Update/Delete query", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110200110", "Setup/Measurment/Name", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110200120", "Setup/Measurment/Conversion", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110210110", "Setup/Currency/Currency", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110210120", "Setup/Currency/Conversion", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110220110", "Setup/Banderol/Banderol", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110220120", "Setup/Banderol/Packaging", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("110220130", "Setup/Banderol/Product", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("120120120", "Purchase/Purchase/Local", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("120120130", "Purchase/Purchase/Import", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("120120140", "Purchase/Purchase/InputService", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("120120150", "Purchase/Purchase/PurchaseReturn", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("120120160", "Purchase/Purchase/Service Stock", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("120120170", "Purchase/Purchase/Service Non Stock", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130130130", "Production/Issue/Issue", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130130140", "Production/Issue/Return", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130130150", "Production/Issue/Issue WithOut BOM", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130130160", "Production/Issue/Wastage", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130130170", "Production/Issue/Transfer", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130140130", "Production/Receive/WIP", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130140140", "Production/Receive/FGReceive", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130140150", "Production/Receive/Return", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("130140160", "Production/Receive/Package", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140140", "Sale/Sale/Local", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140150", "Sale/Sale/Service Stock", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140160", "Sale/Sale/Service Non Stock", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140170", "Sale/Sale/Trading", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140180", "Sale/Sale/Export", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140190", "Sale/Sale/Tender", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140140200", "Sale/Sale/RawSale", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140150140", "Sale/package/package", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140160140", "Sale/Transfer  IssueRecieve/RM In", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140160150", "Sale/Transfer  IssueRecieve/FG In", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140160160", "Sale/Transfer IssueRecieve/RM  Out", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140160170", "Sale/Transfer IssueRecieve/FG Out", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("140170140", "Sale/EXP/EXP", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150150150", "Deposit/Treasury/Treasury", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150160150", "Deposit/VDS/Purchage", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150160160", "Deposit/VDS/Sale", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150170150", "Deposit/SD/SD", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150180150", "Deposit/CashPayble/CashPayble", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150190150", "Deposit/Reverse/Reverse", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("150200150", "Deposit/TDS/TDS", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160160160", "Toll/Client/Issue 6.4", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160160170", "Toll/Client/FGReceive", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160160180", "Toll/Client/VAT11GAGA", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160170160", "Toll/Contractor/RawReceive", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160170170", "Toll/Contractor/FGProduction", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160170180", "Toll/Contractor/FGIssue", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160170190", "Toll/Contractor/Toll 6.3", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160180160", "Toll/Toll Register/Toll 6.1", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("160180170", "Toll/Toll Register/Toll 6.2", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170170170", "Adjustment/AdjustmentHead/Head", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170170180", "Adjustment/AdjustmentHead/Transaction", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170180170", "Adjustment/Purchase/DN", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170180180", "Adjustment/Purchase/CN", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170190170", "Adjustment/Sale/CN", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170190180", "Adjustment/Sale/DN", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170200170", "Adjustment/Dispose/26", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170200180", "Adjustment/Dispose/27", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170210170", "Adjustment/DDB/DDB", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170220170", "Adjustment/VAT & SD Adjutment/VAT", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("170220180", "Adjustment/VAT & SD Adjutment/SD", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180180180", "NBRReport/VAT4.3/VAT4.3", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180190180", "NBRReport/VAT 6.1/VAT 6.1", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180200180", "NBRReport/VAT 6.2/VAT 6.2", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180210180", "NBRReport/VAT 9.1/VAT 9.1", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180220180", "NBRReport/VAT 6.10/VAT 6.10", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180230180", "NBRReport/SDReport/SDReport", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180240180", "NBRReport/VAT 6.3/VAT 6.3", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180250180", "NBRReport/VAT 6.5/VAT 6.5", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180260180", "NBRReport/VAT 7/VAT 7", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180270180", "NBRReport/VAT 20/VAT 20", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180280180", "NBRReport/Banderol/Form 4", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180280190", "NBRReport/Banderol/Form 5", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180290180", "NBRReport/Summery-Current Account/Summery-Current Account", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180290190", "NBRReport/Summery-Current Account/Breakdwon-Current Account", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180300180", "NBRReport/Chak/Chak Ka", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("180300190", "NBRReport/Chak/Chak kha", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190190190", "MISReport/Purchase/Purchase", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190200190", "MISReport/Production/Issue", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190200200", "MISReport/Production/Receive", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190210190", "MISReport/Sale/Sale", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190220190", "MISReport/Stock/Stock", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190220200", "MISReport/Stock/Receive Sale", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190220210", "MISReport/Stock/Reconsciliation", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190220220", "MISReport/Stock/Branch Stock Movement", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190230190", "MISReport/Deposit/Deposit", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190230200", "MISReport/Deposit/Current Account", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190240190", "MISReport/Other/Adjustment", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190240200", "MISReport/Other/Co-Efficient", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190240210", "MISReport/Other/Wastage", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190240220", "MISReport/Other/Value Change", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190240230", "MISReport/Other/Serial Stock", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190240240", "MISReport/Other/Purchage LC", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190250190", "MISReport/Sale C/E/Sale C/E", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("190260190", "MISReport/Comparision Satement", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200200200", "UserAccount/NewAccount/NewAccount", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200210200", "UserAccount/PasswordChange/PasswordChange", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200220200", "UserAccount/UserRole/UserRole", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200230200", "UserAccount/SettingsRole/SettingsRole", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200240200", "UserAccount/Log Out/Log Out", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200250200", "UserAccount/Log In/Log In", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200260200", "UserAccount/Logs/Logs", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200270200", "UserAccount/Close All/Close All", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("200280200", "UserAccount/User Branch/User Branch", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("210210210", "Banderol/Demand/Demand", "Y", "Y", "Y", "Y", null, null);
        //settingsDataInsert1("210220210", "Banderol/Receive/Receive", "Y", "Y", "Y", "Y", null, null);


        #endregion

        public DataTable SelectSettingAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
SELECT SettingGroup,SettingName,SettingValue  FROM Settings 
WHERE  1=1 
";
                if (Id > 0)
                {
                    sqlText += @" and SettingId=@SettingId";
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

                sqlText += " ORDER BY SettingGroup";

                #endregion SqlText

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
                    da.SelectCommand.Parameters.AddWithValue("@SettingId", Id);
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




    }
}
