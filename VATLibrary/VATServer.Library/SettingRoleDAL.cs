using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class SettingRoleDAL : ISettingRole
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

        #region unused
        public void SettingsUpdate(string companyId, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            #endregion

            try
            {
                #region open connection and transaction

                CommonDAL commonDal = new CommonDAL();
                commonDal.DatabaseTableChanges();

                SaleDAL sdal = new SaleDAL();
                sdal.LoadIssueItems(connVM,UserId);

                #region Security 20140101
                commonDal.SetSecurity(companyId);
                #endregion Security 20140101



                #endregion open connection and transaction

            }
            #region catch

            catch (SqlException sqlex)
            {

                throw sqlex;
            }
            catch (ArgumentNullException sqlex)
            {

                throw sqlex;

            }
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion

        }
        //currConn to VcurrConn 25-Aug-2020
        private void IssuePriceUpdate(SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null,string UserId = "")
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

                if (VcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }
                    transaction = VcurrConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);

                }

                #endregion open connection and transaction
                #region find Null
                #region Update if Null

                sqlText = "  ";
                sqlText +=
                    " select  count(distinct Itemno) from IssueDetails WHERE UOMQty IS NULL ";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                transResult = (int)cmdExist.ExecuteScalar();
                if (transResult > 0)
                {
                    #region Update if Null

                    sqlText = "  ";
                    sqlText +=
                        " UPDATE IssueDetails SET UOMc = 1, UOMQty = Quantity,uomn=UOM,UOMPrice = CostPrice WHERE UOMQty IS NULL ";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
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
                SqlCommand cmdRIFB = new SqlCommand(sqlText, VcurrConn);
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
                        SqlCommand cmdInsert = new SqlCommand(sqlText, VcurrConn);
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

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, VcurrConn);
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
                if (VcurrConn != null)
                {
                    if (VcurrConn.State == ConnectionState.Open)
                    {
                        VcurrConn.Close();
                    }
                }

            }
            #endregion Catch and Finall
        }
        //currConn to VcurrConn 25-Aug-2020
        public string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue, string userId, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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
                else if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Select user");
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
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                sqlText += " WHERE SettingGroup=@settingGroup AND SettingName=@settingName AND SettingType=@settingType AND UserId=@userId";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;

                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingType", settingType);
                cmdExist.Parameters.AddWithValue("@userId", userId);

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
                    sqlText += " INSERT INTO SettingsRole(	UserID,SettingGroup,	SettingName,SettingValue,SettingType,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += " @userId,";
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

                    cmdExist1.Parameters.AddWithValueAndNullHandle("@userId", userId);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@settingGroup", settingGroup);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@settingName", settingName);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@settingValue", settingValue);
                    cmdExist1.Parameters.AddWithValueAndNullHandle("@settingType", settingType);

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
        //currConn to VcurrConn 25-Aug-2020
        public string ProductCategoryDataInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CategoryID)CategoryID FROM ProductCategories ";
                sqlText += " WHERE CategoryID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string ProductDataInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT ItemNo)ItemNo FROM Products ";
                sqlText += " WHERE ItemNo='ovh0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string BankDataInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT BankID)BankID FROM BankInformations ";
                sqlText += " WHERE BankID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string VendorGroupInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VendorGroupID)BankID FROM VendorGroups ";
                sqlText += " WHERE VendorGroupID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string VendorInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VendorID)VendorID FROM Vendors ";
                sqlText += " WHERE VendorID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string CustomerGroupInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CustomerGroupID)CustomerGroupID FROM CustomerGroups ";
                sqlText += " WHERE CustomerGroupID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string CustomerInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CustomerID)CustomerID FROM Customers ";
                sqlText += " WHERE CustomerID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string VehicleInsert(SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction



                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VehicleID)VehicleID FROM Vehicles ";
                sqlText += " WHERE VehicleID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;
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
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                    cmdExist1.Transaction = Vtransaction;
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
        public string settingsDataUpdate(string settingGroup, string settingName, string settingGroupNew, string settingNameNew, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
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

                #region ProductExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE SettingGroup='" + settingGroup + "' AND SettingName=@settingName ";
                SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                cmdExist.Transaction = Vtransaction;

                cmdExist.Parameters.AddWithValue("@settingName", settingName);

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
                    sqlText += " SettingName=@settingNameNew,";
                    sqlText += " SettingValue=@settingGroupNew";
                    sqlText += " where SettingGroup=@settingGroup and SettingName=@settingName";

                    //sqlText += " where SettingId='" + item.SettingId + "'" + " and SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "'";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, VcurrConn);
                    cmdUpdate.Transaction = Vtransaction;

                    cmdUpdate.Parameters.AddWithValue("@settingNameNew", settingNameNew);
                    cmdUpdate.Parameters.AddWithValue("@settingGroupNew", settingGroupNew);
                    cmdUpdate.Parameters.AddWithValue("@settingName", settingName);

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
        public string UpdateTablesData(SqlConnection VcurrConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction


                #region Last Settings


                //sqlText += " UPDATE ProductCategories SET 	IsRaw ='Service' where IsRaw = 'Non Stock' ";
                sqlText += " UPDATE AdjustmentHistorys SET 	AdjType ='Credit Payable' where AdjType = 'Credit Payble' ";
                sqlText += " UPDATE AdjustmentHistorys SET 	AdjType ='Cash Payable' where AdjType = 'Cash Payble'";



                SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
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

                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingName", settingName);

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

                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup);
                    cmdExist1.Parameters.AddWithValue("@settingName", settingName);

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
        public bool CheckUserAccess()
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
        public DataSet SearchSettingsRole(string UserId, string UserName, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettingsRole");
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                //transaction = currConn.BeginTransaction("InsertSettingsRoll");

                #endregion open connection and transaction

                #region sql statement search from settings

                sqlText = @" Select * from Settings
                                 ORDER BY SettingGroup,SettingName;
";

                SqlCommand cmdSettingRole = new SqlCommand();
                cmdSettingRole.Connection = currConn;
                cmdSettingRole.CommandText = sqlText;
                cmdSettingRole.CommandType = CommandType.Text;
                DataTable dt = new DataTable("Search Settings");

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdSettingRole);
                dataAdapter.Fill(dt);
                foreach (DataRow item in dt.Rows)
                {

                    sqlText = "  ";
                    sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                    sqlText += " WHERE SettingGroup='" + item["SettingGroup"].ToString() + "' AND SettingName='" + item["SettingName"].ToString() + "'";
                    sqlText += " AND SettingType='" + item["SettingType"].ToString() + "' AND UserId=@UserId";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;

                    cmdExist.Parameters.AddWithValue("@UserId", UserId);

                    object objfoundId = cmdExist.ExecuteScalar();
                    if (objfoundId == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int found = (int)objfoundId;
                    if (found <= 0)// not exist
                    {
                        sqlText = "  ";
                        sqlText +=
                            " INSERT INTO SettingsRole(	SettingGroup,SettingName,SettingValue,SettingType,UserID,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                        sqlText += " VALUES(";
                        sqlText += " '" + item["SettingGroup"].ToString() + "',";
                        sqlText += " '" + item["SettingName"].ToString() + "',";
                        sqlText += " '" + item["SettingValue"].ToString() + "',";
                        sqlText += " '" + item["SettingType"].ToString() + "',";
                        //sqlText += " '" + UserId + "',";
                        sqlText += " @UserId,";
                        sqlText += " 'Y',";
                        sqlText += " @UserName,";
                        sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        //sqlText += " '" + UserName + "',";
                        sqlText += " @UserName,";
                        sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " )";

                        SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                        cmdExist1.Transaction = transaction;
                        cmdExist1.Parameters.AddWithValue("@UserName", UserName);

                        //BugsBD
                        cmdExist1.Parameters.AddWithValue("@UserId", UserId);

                       
                        object objfoundId1 = cmdExist1.ExecuteNonQuery();
                        if (objfoundId1 == null)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                        transResult = (int)objfoundId1;
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                    }

                }

                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM SettingsRole where UserID=@userId
                                     ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM SettingsRole s ORDER BY s.SettingGroup;
";

                SqlCommand cmdSettingRole1 = new SqlCommand();
                cmdSettingRole1.Connection = currConn;
                cmdSettingRole1.CommandText = sqlText;
                cmdSettingRole1.CommandType = CommandType.Text;

                if (!cmdSettingRole1.Parameters.Contains("@userId"))
                { cmdSettingRole1.Parameters.AddWithValue("@userId", UserId); }
                else { cmdSettingRole1.Parameters["@userId"].Value = UserId; }



                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(cmdSettingRole1);
                dataAdapter1.Fill(dataSet);

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
        public string[] SettingsUpdate(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null, string userName = "", string userId = "")
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


                string currentUserId = UserInfoVM.UserId;
                string currentUserName = UserInfoVM.UserName;

                if (!string.IsNullOrEmpty(userName))
                {
                    currentUserName = userName;
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    currentUserId = userId;
                }


                if (settingsVM.Any())
                {
                    foreach (var item in settingsVM)
                    {
                        #region SettingsExist
                        sqlText = "  ";
                        sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                        //sqlText += " WHERE SettingGroup=@itemSettingGroup  AND SettingName=@itemSettingName AND SettingType=@itemSettingType AND UserId='" + currentUserId + "'";
                        sqlText += " WHERE SettingGroup=@itemSettingGroup  AND SettingName=@itemSettingName AND SettingType=@itemSettingType AND UserId = @CurrentUserId";
                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                        cmdExist.Transaction = transaction;

                        cmdExist.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                        cmdExist.Parameters.AddWithValue("@itemSettingName", item.SettingName);
                        cmdExist.Parameters.AddWithValue("@itemSettingType", item.SettingType);

                        //BugsBD
                        cmdExist.Parameters.AddWithValue("@CurrentUserId", currentUserId);
                        



                        object objfoundId = cmdExist.ExecuteScalar();
                        if (objfoundId == null)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                        #endregion SettingsExist


                        #region Last Settings

                        int foundId = (int)objfoundId;
                        if (foundId <= 0)
                        {
                            sqlText = "  ";
                            sqlText +=
                                " INSERT INTO SettingsRole(	SettingGroup,SettingName,SettingValue,SettingType,UserID,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                            sqlText += " VALUES(";
                            sqlText += "@itemSettingGroup,";
                            sqlText += "@itemSettingName,";
                            sqlText += "@itemSettingValue,";
                            sqlText += "@itemSettingType,";
                            sqlText += " '" + currentUserId + "',";
                            sqlText += " 'Y',";
                            //sqlText += " '" + currentUserName + "',";
                            sqlText += "@currentUserName,";
                            sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                            //sqlText += " '" + currentUserName + "',";
                            sqlText += "@currentUserName,";
                            sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            sqlText += " )";

                            SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                            cmdExist1.Transaction = transaction;

                            cmdExist1.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                            cmdExist1.Parameters.AddWithValue("@itemSettingName", item.SettingName);
                            cmdExist1.Parameters.AddWithValue("@itemSettingValue", item.SettingValue);
                            cmdExist1.Parameters.AddWithValue("@itemSettingType", item.SettingType);

                            //BugsBD
                            cmdExist1.Parameters.AddWithValue("@currentUserName", currentUserName);
                            


                            object objfoundId1 = cmdExist1.ExecuteNonQuery();
                            if (objfoundId1 == null)
                            {
                                throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                            }
                            transResult = (int)objfoundId1;
                        }
                        #endregion Last Price

                        else
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText += "update SettingsRole set";
                            sqlText += " SettingValue=@itemSettingValue,";
                            sqlText += " ActiveStatus=@itemActiveStatus";
                            //sqlText += " LastModifiedBy='" + item.LastModifiedBy + "',";
                            //sqlText += " LastModifiedOn='" + item.LastModifiedOn + "'";
                            sqlText += " where SettingGroup=@itemSettingGroup and SettingName=@itemSettingName " +                                         
                                            " AND UserId = @currentUserId";
                                            //" AND UserId='" + currentUserId + "'";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Transaction = transaction;

                            cmdUpdate.Parameters.AddWithValue("@itemSettingValue", item.SettingValue);
                            cmdUpdate.Parameters.AddWithValue("@itemActiveStatus", item.ActiveStatus);
                            cmdUpdate.Parameters.AddWithValue("@itemSettingName", item.SettingName);
                            cmdUpdate.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);

                            //BugsBD
                            cmdUpdate.Parameters.AddWithValue("@currentUserId", currentUserId);


                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            #endregion Update Settings

                        }


                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
                        }

                        #endregion Commit

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
        #endregion

        public DataTable SelectSettingRoleAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
SELECT SettingGroup,SettingName,SettingValue  FROM SettingsRole 

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

                sqlText += " ORDER BY SettingGroup ";

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

        public List<SettingsVM> SearchSettingsRoleList(string UserId, string UserName, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettingsRole");
            SqlTransaction transaction = null;
            List<SettingsVM> vms = new List<SettingsVM>();
            SettingsVM vm = new SettingsVM();

            DataTable dt = new DataTable();

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                //transaction = currConn.BeginTransaction("InsertSettingsRoll");

                #endregion open connection and transaction


                dataSet = SearchSettingsRole(UserId, UserName, connVM);

                if (dataSet.Tables.Count > 0)
                {
                    //DataTable dataTable = ds.Tables[0];
                    dt = dataSet.Tables[0].Copy();
                    foreach (DataRow dr in dt.Rows)
                    {
                      
                            vm = new SettingsVM();
                            vm.SettingId = dr["SettingId"].ToString();
                            vm.SettingGroup = dr["SettingGroup"].ToString();
                            vm.SettingName = dr["SettingName"].ToString();
                            vm.SettingValue = dr["SettingValue"].ToString();
                            vm.SettingType = dr["SettingType"].ToString();
                            vm.ActiveStatus = dr["ActiveStatus"].ToString();
                            //vmsr.CreatedBy = dr["CreatedBy"].ToString();
                            //vmsr.CreatedOn = dr["CreatedOn"].ToString();
                            //vmsr.LastModifiedBy = dr["LastModifiedBy"].ToString();
                            //vmsr.LastModifiedOn = dr["LastModifiedOn"].ToString();

                            vms.Add(vm);

                        }
                        

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

            return vms;
        }



    }
}
