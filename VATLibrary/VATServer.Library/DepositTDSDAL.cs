
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
    public class DepositTDSDAL : IDepositTDS
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion 

        public DataTable SearchVDSDT(string DepositNumber, SysDBInfoVMTemp connVM = null)
        {

            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("VDS");
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
                sqlText = @"SELECT v.TDSId, v.VendorId, Vendors.VendorName,
v.BillAmount,
v.VDSPercent,
v.BillDeductAmount VDSAmount,
convert (varchar,BillDate,120)PurchaseDate,
convert (varchar,IssueDate,120)IssueDate,
 v.Remarks,
v.DepositNumber,
v.IsPercent,
isnull(v.IsPurchase,'Y')IsPurchase,
--, v.DepositDate, 
 v.PurchaseNumber,
 isnull(v.PaymentDate,'1900/01/01')PaymentDate
--, VendorGroups.VendorGroupName
FROM DepositTDSDetails v LEFT OUTER JOIN
Vendors ON v.VendorId = Vendors.VendorID LEFT OUTER JOIN
VendorGroups ON Vendors.VendorGroupID = VendorGroups.VendorGroupID

WHERE 	 (v.TDSId = @DepositNumber )";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                if (!objCommProductType.Parameters.Contains("@DepositNumber"))
                { objCommProductType.Parameters.AddWithValue("@DepositNumber", DepositNumber); }
                else { objCommProductType.Parameters["@DepositNumber"].Value = DepositNumber; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DepositTDSDAL", "SearchVDSDT", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositTDSDAL", "SearchVDSDT", ex.ToString() + "\n" + sqlText);

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

        public string[] ImportData(DataTable dtDeposit, DataTable dtVDS, int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            DepositMasterVM depositMaster;
            List<VDSMasterVM> vdsMasterVMs = new List<VDSMasterVM>();
            AdjustmentHistoryVM adjustmentHistory = new AdjustmentHistoryVM();



            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable
            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                }

                #region RowCount
                int MRowCount = 0;
                int MRow = dtDeposit.Rows.Count;
                for (int i = 0; i < dtDeposit.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtDeposit.Rows[i]["ID"].ToString()))
                    {
                        MRowCount++;
                    }

                }
                if (MRow != MRowCount)
                {
                    throw new ArgumentNullException("you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.");
                }
                #endregion RowCount
                #region ID in Master or Detail table

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtDeposit.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtVDS.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                        }

                    }

                }

                #endregion
                #region Double ID in Master

                for (int i = 0; i < MRowCount; i++)
                {
                    string id = dtDeposit.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtDeposit.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master

                CommonImportDAL cImport = new CommonImportDAL();

                #region checking from database is exist the information(NULL Check)
                #region Deposit


                for (int rows = 0; rows < MRowCount; rows++)
                {
                    #region Check Date

                    bool IsDepositDate, IsChequeDate;
                    IsDepositDate = cImport.CheckDate(dtDeposit.Rows[rows]["Deposit_Date"].ToString().Trim());
                    if (IsDepositDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Deposit_Date field.");
                    }
                    if (!string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim()))
                    {
                        IsChequeDate = cImport.CheckDate(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim());
                        if (IsChequeDate != true)
                        {
                            throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Cheque_Date field.");
                        }
                    }
                    #endregion Check Date
                    #region Deposit type check

                    bool depositType;
                    depositType = cImport.CheckDepositType(dtDeposit.Rows[rows]["Deposit_Type"].ToString().Trim());
                    if (depositType != true)
                    {
                        throw new ArgumentNullException("Please insert Cash/Cheque in Deposit_Type field.");
                    }

                    #endregion Deposit type check
                    #region Yes no check

                    bool post;
                    post = cImport.CheckYN(dtDeposit.Rows[rows]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    #endregion Yes no check
                    #region Null value check
                    //if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim()) || dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim() == "-")
                    //{
                    //   throw new ArgumentNullException("Please insert valid value in Bank_Name field.");  
                    //}
                    //if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim()) || dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim() == "-")
                    //{
                    //    throw new ArgumentNullException("Please insert valid value in Branch_Name field.");
                    //}
                    if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Account_No"].ToString().Trim()) || dtDeposit.Rows[rows]["Account_No"].ToString().Trim() == "-")
                    {
                        throw new ArgumentNullException("Please insert valid value in Account_No field.");
                    }
                    #endregion Null value check
                    #region Bank Check
                    string bankName = dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim();
                    string branchName = dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim();
                    string accNo = dtDeposit.Rows[rows]["Account_No"].ToString().Trim();

                    string bankId = cImport.CheckBankID(bankName, branchName, accNo, currConn, transaction);
                    //if (string.IsNullOrEmpty(bankId))
                    //{
                    //    throw new ArgumentNullException("FindBankId", "Bank '(" + bankName + ")' not in database");
                    //}
                    #endregion Bank Check

                    #region Cheque Information check
                    if (dtDeposit.Rows[rows]["Deposit_Type"].ToString().Trim().ToUpper() == "CHEQUE")
                    {
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_No"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_No"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_No field.");
                        }
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_Date field.");
                        }
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Bank"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_Bank"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_Bank field.");
                        }
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Bank_Branch"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_Bank_Branch"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_Bank_Branch field.");
                        }
                    }
                    #endregion Cheque Information check
                }
                #endregion Deposit

                #region VDS
                #region Row count for vds table
                int DRowCount = 0;
                for (int i = 0; i < dtVDS.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtVDS.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for vds table

                for (int i = 0; i < DRowCount; i++)
                {
                    #region FindVendorId
                    cImport.FindVendorId(dtVDS.Rows[i]["Vendor_Name"].ToString().Trim(),
                                           dtVDS.Rows[i]["Vendor_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindVendorId
                    #region Check Date

                    bool IsBillDate, IsIssueDate;
                    // BillDate=PurchaseDate
                    IsBillDate = cImport.CheckDate(dtVDS.Rows[i]["Bill_Date"].ToString().Trim());
                    if (IsBillDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Bill_Date field.");
                    }
                    //IsIssueDate = cImport.CheckDate(dtVDS.Rows[i]["Issue_Date"].ToString().Trim());
                    //if (IsIssueDate != true)
                    //{
                    //    throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    //}

                    #endregion Check Date
                    #region Numeric value check
                    bool IsBillAmt, IsVDSAmt;
                    IsBillAmt = cImport.CheckNumericBool(dtVDS.Rows[i]["Bill_Amount"].ToString().Trim());
                    if (IsBillAmt != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Bill_Amount field.");
                    }
                    IsVDSAmt = cImport.CheckNumericBool(dtVDS.Rows[i]["VDS_Amount"].ToString().Trim());
                    if (IsVDSAmt != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in VDS_Amount field.");
                    }
                    #endregion Numeric value check
                }
                #endregion VDS

                #endregion checking from database is exist the information(NULL Check)

                #region Connection
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data.");
                }
                #endregion

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtDeposit.Rows[i]["ID"].ToString().Trim();
                    string depositType = dtDeposit.Rows[i]["Deposit_Type"].ToString().Trim();
                    string depositDate = dtDeposit.Rows[i]["Deposit_Date"].ToString().Trim();
                    string post = dtDeposit.Rows[i]["Post"].ToString().Trim();
                    string treasuryNo = cImport.ChecKNullValue(dtDeposit.Rows[i]["Treasury_No"].ToString().Trim());
                    string chequeNo = cImport.ChecKNullValue(dtDeposit.Rows[i]["Cheque_No"].ToString().Trim());
                    string chequeDate = dtDeposit.Rows[i]["Cheque_Date"].ToString().Trim();
                    string chequeBank = cImport.ChecKNullValue(dtDeposit.Rows[i]["Cheque_Bank"].ToString().Trim());
                    string chqBBrunch = cImport.ChecKNullValue(dtDeposit.Rows[i]["Cheque_Bank_Branch"].ToString().Trim());
                    string bankName = cImport.ChecKNullValue(dtDeposit.Rows[i]["Bank_Name"].ToString().Trim());
                    string branchName = cImport.ChecKNullValue(dtDeposit.Rows[i]["Branch_Name"].ToString().Trim());
                    string accNo = dtDeposit.Rows[i]["Account_No"].ToString().Trim();
                    #region Bank Check
                    string bankId = cImport.CheckBankID(bankName, branchName, accNo, currConn, transaction);
                    #endregion Bank Check
                    string comment = cImport.ChecKNullValue(dtDeposit.Rows[i]["Comments"].ToString().Trim());
                    string dPerson = cImport.ChecKNullValue(dtDeposit.Rows[i]["Deposit_Person"].ToString().Trim());
                    string personDesg = cImport.ChecKNullValue(dtDeposit.Rows[i]["Person_Designation"].ToString().Trim());
                    string createdBy = dtDeposit.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtDeposit.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtDeposit.Rows[i]["Transection_Type"].ToString().Trim();

                    #region Deposit
                    depositMaster = new DepositMasterVM();
                    //depositMaster.DepositId = NextID.ToString();
                    depositMaster.TreasuryNo = treasuryNo;
                    depositMaster.DepositDate = Convert.ToDateTime(depositDate).ToString("yyyy-MM-dd HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    depositMaster.DepositType = depositType;
                    //depositMaster.DepositAmount = Convert.ToDecimal(vdsAmt);
                    depositMaster.ChequeNo = chequeNo;
                    depositMaster.ChequeBank = chequeBank;
                    depositMaster.ChequeBankBranch = chqBBrunch;
                    //depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");//dtpChequeDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (string.IsNullOrEmpty(chequeDate))
                    {
                        chequeDate = depositDate;
                    }
                    depositMaster.ChequeDate = Convert.ToDateTime(chequeDate).ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.BankId = bankId;
                    depositMaster.TreasuryCopy = "-";
                    depositMaster.DepositPerson = dPerson;
                    depositMaster.DepositPersonDesignation = personDesg;
                    depositMaster.Comments = comment;
                    depositMaster.CreatedBy = createdBy;
                    depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.LastModifiedBy = createdBy;
                    depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.TransactionType = transactionType;
                    //depositMaster.Info2 = "Info2";
                    //depositMaster.Info3 = "Info3";
                    //depositMaster.Info4 = "Info4";
                    //depositMaster.Info5 = "Info5";
                    depositMaster.Post = post;
                    depositMaster.BranchId = branchId;
                    #endregion Deposit

                    #region VDS

                    DataRow[] VDSRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        VDSRaws = dtVDS.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        VDSRaws = null;
                    }

                    #endregion MAtch

                    int dCounter = 1;
                    decimal depositAmt = 0;
                    vdsMasterVMs = new List<VDSMasterVM>();
                    foreach (var row in VDSRaws)
                    {

                        //}
                        //for (int vdsRow = 0; vdsRow < dgvVDS.RowCount; vdsRow++)
                        //{
                        string IsPurchase = row["IsPurchase"].ToString().Trim();
                        string vendorName = row["Vendor_Name"].ToString().Trim();
                        string vendorCode = row["Vendor_Code"].ToString().Trim();
                        #region FindVendorId
                        string vendorId = cImport.FindVendorId(vendorName, vendorCode, currConn, transaction);
                        #endregion FindVendorId
                        string billAmt = row["Bill_Amount"].ToString().Trim();
                        string billDate = OrdinaryVATDesktop.DateToDate(row["Bill_Date"].ToString().Trim());
                        string vdsAmt = row["VDS_Amount"].ToString().Trim();
                        string issueDate = OrdinaryVATDesktop.DateToDate(row["Issue_Date"].ToString().Trim());
                        string purchaseNo = row["Purchase_No"].ToString().Trim();
                        string remarks = cImport.ChecKNullValue(row["Remarks"].ToString().Trim());

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        //vdsDetail.DepositId = NextID.ToString();
                        vdsDetail.VendorId = vendorId;
                        vdsDetail.BillAmount = Convert.ToDecimal(billAmt);
                        vdsDetail.BillDate = Convert.ToDateTime(billDate).ToString("yyyy-MM-dd HH:mm:ss");
                        vdsDetail.BillDeductedAmount = Convert.ToDecimal(vdsAmt);
                        vdsDetail.Remarks = remarks;
                        vdsDetail.IssueDate = issueDate;//Convert.ToDateTime(issueDate).ToString("yyyy-MM-dd HH:mm:ss"); // temp change
                        if (string.IsNullOrEmpty(purchaseNo))
                        {
                            purchaseNo = "NA";
                        }
                        vdsDetail.PurchaseNumber = purchaseNo;
                        decimal VDSPercentRate = (Convert.ToDecimal(vdsAmt) * 100) / Convert.ToDecimal(billAmt);
                        vdsDetail.VDSPercent = VDSPercentRate;
                        vdsDetail.IsPercent = "N";
                        vdsDetail.BranchId = branchId;
                        vdsDetail.IsPurchase = IsPurchase;
                        vdsMasterVMs.Add(vdsDetail);

                        dCounter++;
                        depositAmt = depositAmt + Convert.ToDecimal(vdsAmt);
                    }
                    #endregion
                    depositMaster.DepositAmount = depositAmt;

                    string[] sqlResults = DepositInsertX(depositMaster, vdsMasterVMs, adjustmentHistory, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }
            }

            #endregion try
            #region catch & final
            catch (SqlException sqlex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "ImportData", sqlex.ToString());

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (ArgumentNullException aeg)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "ImportData", aeg.ToString());

                throw new ArgumentNullException("", aeg.Message.ToString());

            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "ImportData", ex.ToString());

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion catch & final

            return retResults;
        }

        public decimal ReverseAmount(string reverseDepositId, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            string sqlText = "";
            SqlConnection currConn = null;
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

                #region Return Amount

                sqlText = "  ";

                sqlText = " Select Sum(isnull(Deposits.DepositAmount,0)) from Deposits ";
                sqlText += " where ReverseDepositId =@reverseDepositId";
                sqlText += " and Post = 'Y'";
                sqlText += " group by ReverseDepositId";


                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValueAndNullHandle("@reverseDepositId", reverseDepositId);
                if (cmd.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmd.ExecuteScalar();
                }

                #endregion Return Amount

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("DepositTDSDAL", "ReverseAmount", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositTDSDAL", "ReverseAmount", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        public string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            #region try

            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                    transaction = currConn.BeginTransaction();
                }


                var sqlText = @"update PurchaseInvoiceHeaders set IsTDSCompleted = @flag where PurchaseInvoiceNo in (select PurchaseNumber from DepositTDSDetails where TDSId = @TDSId)";

                var cmd = new SqlCommand(sqlText, currConn);

                cmd.Parameters.AddWithValueAndNullHandle("@TDSId", VdsId);
                cmd.Parameters.AddWithValueAndNullHandle("@flag", flag);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                transaction.Commit();
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "";
                retResults[2] = "" + "1";
                retResults[3] = "" + "N";
                #endregion SuccessResult

                return retResults;
            }
            #endregion

            #region catch & final

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "UpdateVdsComplete", ex.ToString());

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion catch & final
        }

        #region web methods

        public List<DepositMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<DepositMasterVM> VMs = new List<DepositMasterVM>();
            DepositMasterVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, false);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new DepositMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.DepositId = dr["DepositId"].ToString();
                    vm.TreasuryNo = dr["TreasuryNo"].ToString();
                    vm.DepositDate = OrdinaryVATDesktop.DateTimeToDate(dr["DepositDateTime"].ToString());
                    vm.DepositType = dr["DepositType"].ToString();
                    vm.DepositAmount = Convert.ToDecimal(dr["DepositAmount"].ToString());
                    vm.ChequeNo = dr["ChequeNo"].ToString();
                    vm.ChequeBank = dr["ChequeBank"].ToString();
                    vm.ChequeBankBranch = dr["ChequeBankBranch"].ToString();
                    vm.ChequeDate = OrdinaryVATDesktop.DateTimeToDate(dr["ChequeDate"].ToString());
                    vm.BankId = dr["BankID"].ToString();
                    vm.TreasuryCopy = dr["TreasuryCopy"].ToString();
                    vm.DepositPerson = dr["DepositPerson"].ToString();
                    vm.DepositPersonDesignation = dr["DepositPersonDesignation"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();

                    vm.BankName = dr["BankName"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.AccountNumber = dr["AccountNumber"].ToString();
                    ////reading newly added fields
                    vm.DepositPersonContactNo = dr["DepositPersonContactNo"].ToString();
                    vm.DepositPersonAddress = dr["DepositPersonAddress"].ToString();
                    ////newly added fields
                    vm.ReverseDepositId = dr["ReverseDepositId"].ToString();

                    VMs.Add(vm);
                }


                #endregion SqlExecution

                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
                //}
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            //finally
            //{
            //    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            //    {
            //        currConn.Close();
            //    }
            //}
            #endregion
            return VMs;
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 d.Id
,d.DepositId
,d.TreasuryNo
,d.DepositDateTime
,d.DepositType
,d.DepositAmount
,d.ChequeNo
,isnull(d.BankDepositDate,'')BankDepositDate
,d.ChequeBank
,d.ChequeBankBranch
,d.ChequeDate
,d.BankID
,d.TreasuryCopy
,d.DepositPerson
,d.DepositPersonDesignation
,d.Comments
,d.CreatedBy
,d.CreatedOn
,d.LastModifiedBy
,d.LastModifiedOn
,d.TransactionType
,d.Post
,d.ReverseDepositId
,isnull(d.DepositPersonContactNo,'N/A')DepositPersonContactNo
,isnull(d.DepositPersonAddress,'N/A')DepositPersonAddress 
,b.BankName
,b.BranchName
,b.AccountNumber
,isnull(d.BranchId,0)BranchId 

FROM DepositTDSs d
LEFT OUTER JOIN BankInformations b ON d.BankID = b.BankID
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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

        #endregion

        #region need to parameterize done

        //currConn to VcurrConn 25-Aug-2020
        public string[] DepositInsertX(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            ////SqlConnection vcurrConn = VcurrConn;
            ////if (vcurrConn == null)
            ////{
            ////    currConn = null;
            ////    Vtransaction = null;
            ////}

            int transResult = 0;
            string sqlText = "";
            string newID = "";
            string PostStatus = "";
            int IDExist = 0;

            #endregion Initializ

            #region Try

            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.depMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgFiscalYearNotExist);
                }


                #endregion Validation for Header


                #region Old connection

                #region open connection and transaction

                ////if (vcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }

                ////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock

                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from DepositTDSs WHERE DepositId= @MasterId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create

                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgTransactionNotDeclared);
                }

                CommonDAL commonDal = new CommonDAL();
                if (Master.TransactionType == "TDS"
                    )
                {
                    newID = commonDal.TransactionCode("TDS", "TDS", "DepositTDSs", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into DepositTDSs(";
                sqlText += " DepositId,	";
                sqlText += " TreasuryNo,";
                sqlText += " DepositDateTime,";
                sqlText += " DepositType,";
                sqlText += " DepositAmount,";
                sqlText += " ChequeNo,";
                sqlText += " BankDepositDate,";
                sqlText += " ChequeBank,";
                sqlText += " ChequeBankBranch,";
                sqlText += " ChequeDate,";
                sqlText += " BankID,";
                sqlText += " TreasuryCopy,";
                sqlText += " DepositPerson,";
                sqlText += " DepositPersonDesignation,";
                sqlText += " DepositPersonContactNo,";
                sqlText += " DepositPersonAddress,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " TransactionType,";
                sqlText += " ReverseDepositId,";
                sqlText += " Post,";
                sqlText += " BranchId";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += " @newID,";
                sqlText += " @MasterTreasuryNo,";
                sqlText += " @MasterDepositDate,";
                sqlText += " @MasterDepositType,";
                sqlText += " @MasterDepositAmount,";
                sqlText += " @MasterChequeNo,";
                sqlText += " @BankDepositDate,";
                sqlText += " @MasterChequeBank,";
                sqlText += " @MasterChequeBankBranch,";
                sqlText += " @MasterChequeDate,";
                sqlText += " @MasterBankId,";
                sqlText += " @MasterTreasuryCopy,";
                sqlText += " @MasterDepositPerson,";
                sqlText += " @MasterDepositPersonDesignation,";
                sqlText += " @MasterDepositPersonContactNo,";
                sqlText += " @MasterDepositPersonAddress,";
                sqlText += " @MasterComments,";
                sqlText += " @MasterCreatedBy,";
                sqlText += " @MasterCreatedOn,";
                sqlText += " @MasterLastModifiedBy,";
                sqlText += " @MasterLastModifiedOn,";
                sqlText += " @MasterTransactionType,";
                sqlText += " @MasterReturnID,";
                sqlText += " @MasterPost,";
                sqlText += " @BranchId";

                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTreasuryNo", Master.TreasuryNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositType", Master.DepositType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositAmount", Master.DepositAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeNo", Master.ChequeNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankDepositDate", Master.BankDepositDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeBank", Master.ChequeBank);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeBankBranch", Master.ChequeBankBranch);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeDate", Master.ChequeDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterBankId", Master.BankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTreasuryCopy", Master.TreasuryCopy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPerson", Master.DepositPerson);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonDesignation", Master.DepositPersonDesignation);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonContactNo", Master.DepositPersonContactNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonAddress", Master.DepositPersonAddress);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);


                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Validation for Detail

                if (tds.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in tds.ToList())
                {
                    #region Find Transaction Exist

                    if (Item.PurchaseNumber.Trim().ToUpper() != "NA")
                    {
                        sqlText = "";
                        sqlText += "select COUNT(TDSId) from DepositTDSDetails WHERE DepositNumber='" + newID + "' ";
                        sqlText += " AND PurchaseNumber=@ItemPurchaseNumber";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                            MessageVM.dpMsgFindExistIDP);
                        }
                    }

                    #endregion Find Transaction Exist

                    if (string.IsNullOrEmpty(Item.DepositNumber))
                    {
                        Item.DepositNumber = newID;

                    }
                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into DepositTDSDetails(";

                    sqlText += " TDSId";
                    sqlText += " ,VendorId";
                    sqlText += " ,BillAmount";
                    sqlText += " ,BillDate";
                    sqlText += " ,BillDeductAmount";
                    sqlText += " ,DepositNumber";
                    sqlText += " ,PurchaseNumber";
                    sqlText += " ,DepositDate";
                    sqlText += " ,Remarks";
                    sqlText += " ,IssueDate";
                    sqlText += " ,CreatedBy";
                    sqlText += " ,CreatedOn";
                    sqlText += " ,LastModifiedBy";
                    sqlText += " ,LastModifiedOn";
                    sqlText += " ,VDSPercent";
                    sqlText += " ,IsPercent";
                    sqlText += " ,IsPurchase";
                    sqlText += " ,Post";
                    sqlText += " ,ReverseVDSId";
                    sqlText += " ,BranchId";
                    sqlText += " ,PaymentDate";

                    sqlText += " )";

                    sqlText += " values(	";

                    sqlText += "'" + newID + "',";
                    sqlText += "@ItemVendorId,";
                    sqlText += "@ItemBillAmount,";
                    sqlText += "@ItemBillDate,";
                    sqlText += "@ItemBillDeductedAmount,";
                    sqlText += "@ItemDepositNumber,";
                    sqlText += "@ItemPurchaseNumber,";
                    sqlText += "@MasterDepositDate,";
                    sqlText += "@ItemRemarks,";
                    sqlText += "@ItemIssueDate,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemVDSPercent,";
                    sqlText += "@ItemIsPercent,";
                    sqlText += "@ItemIsPurchase,";
                    sqlText += "@MasterPost,";
                    sqlText += "@MasterReturnID,";
                    sqlText += "@BranchId,";
                    sqlText += "@PaymentDate";


                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillAmount", Item.BillAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDate", Item.BillDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDepositNumber", Item.DepositNumber);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarks", Item.Remarks);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIssueDate", Item.IssueDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVDSPercent", Item.VDSPercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPercent", Item.IsPercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPurchase", Item.IsPurchase);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PaymentDate", Item.PaymentDate);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                        MessageVM.dpMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable

                    #region Update PurchaseTDSs

                    sqlText = "";

                    sqlText += " update PurchaseTDSs set PaymentDate=DepositTDSDetails.PaymentDate ";
                    sqlText += " from DepositTDSDetails ";
                    sqlText += " where DepositTDSDetails.PurchaseNumber=PurchaseTDSs.PurchaseInvoiceNo ";
                    sqlText += " and DepositTDSDetails.PurchaseNumber=@PurchaseNumber ";


                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;
                    cmdUpdateReceive.Parameters.AddWithValueAndNullHandle("@PurchaseNumber", Item.PurchaseNumber);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgUnableToSaveReceive);
                    }

                    #endregion Update PurchaseTDSs
                }


                #endregion Insert Detail Table


                #region Commit

                if (Vtransaction == null)
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
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Inserted";
                retResults[2] = "" + newID;
                retResults[3] = "N";
                retResults[4] = Id;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (currConn == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "DepositInsertX", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        //currConn to VcurrConn 25-Aug-2020
        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> tds, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            
            ////SqlConnection vcurrConn = VcurrConn;
            ////if (vcurrConn == null)
            ////{
            ////    VcurrConn = null;
            ////    Vtransaction = null;
            ////}

            int transResult = 0;
            string sqlText = "";
            string newIDCreate = "";
            int IDExist = 0;

            #endregion Initializ

            #region Try

            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.depMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgFiscalYearNotExist);
                }


                #endregion Validation for Header

                #region Old connection

                #region open connection and transaction

                ////if (vcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }

                ////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                ////}

                #endregion open connection and transaction

                #endregion Old connection

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
                    transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                }

                #endregion open connection and transaction


                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from DepositTDSs WHERE DepositId= @MasterId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create

                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgTransactionNotDeclared);
                }

                CommonDAL commonDal = new CommonDAL();
                if (Master.TransactionType == "TDS"
                    )
                {
                    newIDCreate = commonDal.TransactionCode("TDS", "TDS", "DepositTDSs", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }

                #endregion Purchase ID Create Not Complete
                Master.DepositId = newIDCreate;
                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into DepositTDSs(";
                sqlText += " DepositId,	";
                sqlText += " TreasuryNo,";
                sqlText += " DepositDateTime,";
                sqlText += " DepositType,";
                sqlText += " DepositAmount,";
                sqlText += " ChequeNo,";
                sqlText += " BankDepositDate,";
                sqlText += " ChequeBank,";
                sqlText += " ChequeBankBranch,";
                sqlText += " ChequeDate,";
                sqlText += " BankID,";
                sqlText += " TreasuryCopy,";
                sqlText += " DepositPerson,";
                sqlText += " DepositPersonDesignation,";
                sqlText += " DepositPersonContactNo,";
                sqlText += " DepositPersonAddress,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " TransactionType,";
                sqlText += " ReverseDepositId,";
                sqlText += " Post,";
                sqlText += " BranchId";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += " @newID,";
                sqlText += " @MasterTreasuryNo,";
                sqlText += " @MasterDepositDate,";
                sqlText += " @MasterDepositType,";
                sqlText += " @MasterDepositAmount,";
                sqlText += " @MasterChequeNo,";
                sqlText += " @BankDepositDate,";
                sqlText += " @MasterChequeBank,";
                sqlText += " @MasterChequeBankBranch,";
                sqlText += " @MasterChequeDate,";
                sqlText += " @MasterBankId,";
                sqlText += " @MasterTreasuryCopy,";
                sqlText += " @MasterDepositPerson,";
                sqlText += " @MasterDepositPersonDesignation,";
                sqlText += " @MasterDepositPersonContactNo,";
                sqlText += " @MasterDepositPersonAddress,";
                sqlText += " @MasterComments,";
                sqlText += " @MasterCreatedBy,";
                sqlText += " @MasterCreatedOn,";
                sqlText += " @MasterLastModifiedBy,";
                sqlText += " @MasterLastModifiedOn,";
                sqlText += " @MasterTransactionType,";
                sqlText += " @MasterReturnID,";
                sqlText += " @MasterPost,";
                sqlText += " @BranchId";

                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", Master.DepositId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTreasuryNo", Master.TreasuryNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositType", Master.DepositType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositAmount", Master.DepositAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeNo", Master.ChequeNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankDepositDate", Master.BankDepositDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeBank", Master.ChequeBank);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeBankBranch", Master.ChequeBankBranch);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterChequeDate", Master.ChequeDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterBankId", Master.BankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTreasuryCopy", Master.TreasuryCopy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPerson", Master.DepositPerson);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonDesignation", Master.DepositPersonDesignation);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonContactNo", Master.DepositPersonContactNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonAddress", Master.DepositPersonAddress);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);


                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Validation for Detail

                if (tds.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail
                retResults = DepositInsert2(Master, tds, transaction, currConn);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }



                #region Commit

                if (Vtransaction == null)
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
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Inserted";
                retResults[2] = "" + Master.DepositId;
                retResults[3] = "N";
                retResults[4] = Id;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (currConn == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "DepositInsert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

        //currConn to VcurrConn 25-Aug-2020
        public string[] DepositInsert2(DepositMasterVM Master, List<VDSMasterVM> tds, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";

            int transResult = 0;
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction


                #region Insert Detail Table

                foreach (var Item in tds.ToList())
                {

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into DepositTDSDetails(";

                    sqlText += " TDSId";
                    sqlText += " ,VendorId";
                    sqlText += " ,BillAmount";
                    sqlText += " ,BillDate";
                    sqlText += " ,BillDeductAmount";
                    sqlText += " ,DepositNumber";
                    sqlText += " ,PurchaseNumber";
                    sqlText += " ,DepositDate";
                    sqlText += " ,Remarks";
                    sqlText += " ,IssueDate";
                    sqlText += " ,CreatedBy";
                    sqlText += " ,CreatedOn";
                    sqlText += " ,LastModifiedBy";
                    sqlText += " ,LastModifiedOn";
                    sqlText += " ,VDSPercent";
                    sqlText += " ,IsPercent";
                    sqlText += " ,IsPurchase";
                    sqlText += " ,Post";
                    sqlText += " ,ReverseVDSId";
                    sqlText += " ,BranchId";
                    sqlText += " ,PaymentDate";

                    sqlText += " )";

                    sqlText += " values(	";

                    sqlText += "'" + Master.DepositId + "',";
                    sqlText += "@ItemVendorId,";
                    sqlText += "@ItemBillAmount,";
                    sqlText += "@ItemBillDate,";
                    sqlText += "@ItemBillDeductedAmount,";
                    sqlText += "@ItemDepositNumber,";
                    sqlText += "@ItemPurchaseNumber,";
                    sqlText += "@MasterDepositDate,";
                    sqlText += "@ItemRemarks,";
                    sqlText += "@ItemIssueDate,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemVDSPercent,";
                    sqlText += "@ItemIsPercent,";
                    sqlText += "@ItemIsPurchase,";
                    sqlText += "@MasterPost,";
                    sqlText += "@MasterReturnID,";
                    sqlText += "@BranchId,";
                    sqlText += "@PaymentDate";


                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillAmount", Item.BillAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDate", Item.BillDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDepositNumber", Master.DepositId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarks", Item.Remarks);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIssueDate", Item.IssueDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVDSPercent", Item.VDSPercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPercent", Item.IsPercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPurchase", Item.IsPurchase);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PaymentDate", Item.PaymentDate);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                        MessageVM.dpMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable

                    #region Update PurchaseTDSs

                    sqlText = "";

                    sqlText += " update PurchaseTDSs set PurchaseTDSs.PaymentDate=DepositTDSDetails.PaymentDate,PurchaseTDSs.DepositId=DepositTDSDetails.TDSId ";
                    sqlText += " from DepositTDSDetails ";
                    sqlText += " where DepositTDSDetails.PurchaseNumber=PurchaseTDSs.PurchaseInvoiceNo ";
                    sqlText += " and DepositTDSDetails.PurchaseNumber=@PurchaseNumber ";


                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;
                    cmdUpdateReceive.Parameters.AddWithValueAndNullHandle("@PurchaseNumber", Item.PurchaseNumber);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgUnableToSaveReceive);
                    }

                    #endregion Update PurchaseTDSs
                }

                #endregion Insert Detail Table

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Inserted";
                retResults[2] = "" + Master.DepositId;
                retResults[3] = "N";
                retResults[4] = Id;

                #endregion SuccessResult

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositTDSDAL", "DepositInsert2", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> tds, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";



            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                        "Please Check Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from DepositTDSs WHERE DepositId=@MasterId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header
                #region update Header
                sqlText = "";

                sqlText += " update DepositTDSs set  ";
                sqlText += " TreasuryNo                 = @MasterTreasuryNo ,";
                sqlText += " DepositDateTime            = @MasterDepositDate ,";
                sqlText += " DepositType                = @MasterDepositType ,";
                sqlText += " DepositAmount              = @MasterDepositAmount ,";
                sqlText += " ChequeNo                   = @MasterChequeNo ,";
                sqlText += " BankDepositDate            = @BankDepositDate ,";
                sqlText += " ChequeBank                 = @MasterChequeBank ,";
                sqlText += " ChequeBankBranch           = @MasterChequeBankBranch ,";
                sqlText += " ChequeDate                 = @MasterChequeDate ,";
                sqlText += " BankID                     = @MasterBankId ,";
                sqlText += " TreasuryCopy               = @MasterTreasuryCopy ,";
                sqlText += " DepositPerson              = @MasterDepositPerson ,";
                sqlText += " DepositPersonDesignation   = @MasterDepositPersonDesignation ,";
                sqlText += " DepositPersonContactNo     = @MasterDepositPersonContactNo ,";
                sqlText += " DepositPersonAddress       = @MasterDepositPersonAddress ,";
                sqlText += " Comments                   = @MasterComments ,";
                sqlText += " LastModifiedBy             = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn             = @MasterLastModifiedOn ,";
                sqlText += " TransactionType            = @MasterTransactionType ,";
                sqlText += " ReverseDepositId           = @MasterReturnID ,";
                sqlText += " Post                       = @MasterPost ";
                sqlText += " where DepositId            = @MasterId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTreasuryNo", Master.TreasuryNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositType", Master.DepositType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositAmount", Master.DepositAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeNo", Master.ChequeNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BankDepositDate", Master.BankDepositDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeBank", Master.ChequeBank);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeBankBranch", Master.ChequeBankBranch);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeDate", Master.ChequeDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterBankId", Master.BankId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTreasuryCopy", Master.TreasuryCopy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPerson", Master.DepositPerson);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonDesignation", Master.DepositPersonDesignation);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonContactNo", Master.DepositPersonContactNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonAddress", Master.DepositPersonAddress);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header


                if (tds.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }

                #region Purchase/Receive/Issue Data

                sqlText = "";
                sqlText += @" delete FROM DepositTDSDetails WHERE TDSId=@DepositId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@DepositId", Master.DepositId);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion

                retResults = DepositInsert2(Master, tds, transaction, currConn);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                #endregion ID check completed,update Information in Header

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Updated";
                retResults[2] = Master.DepositId;
                retResults[3] = "N";
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositTDSDAL", "DepositUpdate", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] DepositUpdateX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                        "Please Check Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from DepositTDSs WHERE DepositId=@MasterId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header
                #region update Header
                sqlText = "";

                sqlText += " update DepositTDSs set  ";
                sqlText += " TreasuryNo                 = @MasterTreasuryNo ,";
                sqlText += " DepositDateTime            = @MasterDepositDate ,";
                sqlText += " DepositType                = @MasterDepositType ,";
                sqlText += " DepositAmount              = @MasterDepositAmount ,";
                sqlText += " ChequeNo                   = @MasterChequeNo ,";
                sqlText += " BankDepositDate            = @BankDepositDate ,";
                sqlText += " ChequeBank                 = @MasterChequeBank ,";
                sqlText += " ChequeBankBranch           = @MasterChequeBankBranch ,";
                sqlText += " ChequeDate                 = @MasterChequeDate ,";
                sqlText += " BankID                     = @MasterBankId ,";
                sqlText += " TreasuryCopy               = @MasterTreasuryCopy ,";
                sqlText += " DepositPerson              = @MasterDepositPerson ,";
                sqlText += " DepositPersonDesignation   = @MasterDepositPersonDesignation ,";
                sqlText += " DepositPersonContactNo     = @MasterDepositPersonContactNo ,";
                sqlText += " DepositPersonAddress       = @MasterDepositPersonAddress ,";
                sqlText += " Comments                   = @MasterComments ,";
                sqlText += " LastModifiedBy             = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn             = @MasterLastModifiedOn ,";
                sqlText += " TransactionType            = @MasterTransactionType ,";
                sqlText += " ReverseDepositId           = @MasterReturnID ,";
                sqlText += " Post                       = @MasterPost ";
                sqlText += " where DepositId            = @MasterId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTreasuryNo", Master.TreasuryNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositType", Master.DepositType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositAmount", Master.DepositAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeNo", Master.ChequeNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BankDepositDate", Master.BankDepositDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeBank", Master.ChequeBank);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeBankBranch", Master.ChequeBankBranch);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterChequeDate", Master.ChequeDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterBankId", Master.BankId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTreasuryCopy", Master.TreasuryCopy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPerson", Master.DepositPerson);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonDesignation", Master.DepositPersonDesignation);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonContactNo", Master.DepositPersonContactNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositPersonAddress", Master.DepositPersonAddress);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region AdjCashPayble
                if (Master.TransactionType == "AdjCashPayble" || Master.TransactionType == "AdjCashPayble-Credit")
                {
                    #region Find ID for Update

                    int countId = 0;
                    #region product type existence checking

                    sqlText = "select count(AdjHistoryNo) from AdjustmentHistorys where  AdjHistoryNo = @MasterId ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Could not find requested Adjustment id.");
                    }
                    #endregion
                    #region same name product type existence checking
                    countId = 0;
                    sqlText = "select count(AdjHistoryNo) from AdjustmentHistorys where  AdjId=@adjHistoryAdjId";
                    sqlText += " and AdjDate            =@adjHistoryAdjDate";
                    sqlText += " and AdjType            =@adjHistoryAdjType ";
                    sqlText += " and not AdjHistoryNo   =@MasterId ";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                    countId = (int)cmdNameExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Same Adjustment name already exist in same date");
                    }
                    #endregion
                    #endregion Find ID for Update

                    #region sql statement

                    sqlText = "";
                    sqlText += "UPDATE AdjustmentHistorys SET";
                    sqlText += " AdjId              =@adjHistoryAdjId,";
                    sqlText += " AdjDate            =@adjHistoryAdjDate,";
                    sqlText += " AdjAmount          =@adjHistoryAdjAmount,";
                    sqlText += " AdjVATRate         =@adjHistoryAdjVATRate,";
                    sqlText += " AdjVATAmount       =@adjHistoryAdjVATAmount,";
                    sqlText += " AdjSD              =@adjHistoryAdjSD,";
                    sqlText += " AdjSDAmount        =@adjHistoryAdjSDAmount,";
                    sqlText += " AdjOtherAmount     =@adjHistoryAdjOtherAmount,";
                    sqlText += " AdjType            =@adjHistoryAdjType,";
                    sqlText += " AdjDescription     =@adjHistoryAdjDescription,";
                    sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                    sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                    sqlText += " AdjInputAmount     =@adjHistoryAdjInputAmount,";
                    sqlText += " AdjInputPercent    =@adjHistoryAdjInputPercent,";
                    sqlText += " AdjReferance       =@adjHistoryAdjReferance,";
                    sqlText += " ReverseAdjHistoryNo=@MasterReturnID,";
                    sqlText += " Post               =@MasterPost";
                    sqlText += " WHERE AdjHistoryNo =@MasterDepositId";

                    SqlCommand cmdUpdateACP = new SqlCommand(sqlText, currConn);
                    cmdUpdateACP.Transaction = transaction;

                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjAmount", adjHistory.AdjAmount);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjVATRate", adjHistory.AdjVATRate);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjVATAmount", adjHistory.AdjVATAmount);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjSD", adjHistory.AdjSD);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjSDAmount", adjHistory.AdjSDAmount);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjOtherAmount", adjHistory.AdjOtherAmount);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDescription", adjHistory.AdjDescription);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjInputAmount", adjHistory.AdjInputAmount);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjInputPercent", adjHistory.AdjInputPercent);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjReferance", adjHistory.AdjReferance);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);

                    transResult = (int)cmdUpdateACP.ExecuteNonQuery();

                    #endregion
                }
                #endregion AdjCashPayble

                #endregion ID check completed,update Information in Header
                #region VDS
                if (Master.TransactionType == "VDS" || Master.TransactionType == "VDS-Credit")
                {
                    if (vds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        sqlText = "";
                        sqlText += "select COUNT(TDSId) from DepositTDSDetails WHERE TDSId=@MasterId  ";
                        sqlText += " AND PurchaseNumber=@ItemPurchaseNumber ";
                        sqlText += " AND VendorId=@ItemVendorId ";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);

                        IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist <= 0)
                        {
                            #region Insert only DetailTable

                            sqlText = "";
                            sqlText += " insert into DepositTDSDetails(";

                            sqlText += " TDSId";
                            sqlText += " ,VendorId";
                            sqlText += " ,BillAmount";
                            sqlText += " ,BillDate";
                            sqlText += " ,BillDeductAmount";
                            sqlText += " ,DepositNumber";
                            sqlText += " ,PurchaseNumber";
                            sqlText += " ,DepositDate";
                            sqlText += " ,Remarks";
                            sqlText += " ,IssueDate";
                            sqlText += " ,CreatedBy";
                            sqlText += " ,CreatedOn";
                            sqlText += " ,LastModifiedBy";
                            sqlText += " ,LastModifiedOn";
                            sqlText += " ,VDSPercent";
                            sqlText += " ,IsPercent";
                            sqlText += " ,IsPurchase";
                            sqlText += " ,ReverseVDSId";
                            sqlText += " ,Post";
                            sqlText += " ,PaymentDate";
                            sqlText += " )";

                            sqlText += " values(	";

                            sqlText += "@MasterDepositId,";
                            sqlText += "@ItemVendorId,";
                            sqlText += "@ItemBillAmount,";
                            sqlText += "@ItemBillDate,";
                            sqlText += "@ItemBillDeductedAmount,";
                            sqlText += "@ItemDepositNumber,";
                            sqlText += "@ItemPurchaseNumber,";
                            sqlText += "@MasterDepositDate,";
                            sqlText += "@ItemRemarks,";
                            sqlText += "@ItemIssueDate,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@ItemVDSPercent,";
                            sqlText += "@ItemIsPercent,";
                            sqlText += "@ItemIsPurchase,";
                            sqlText += "@MasterReturnID,";
                            sqlText += "@MasterPost,";
                            sqlText += "@PaymentDate";

                            sqlText += ")	";


                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;

                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillAmount", Item.BillAmount);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDate", Item.BillDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDepositNumber", Item.DepositNumber);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarks", Item.Remarks);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIssueDate", Item.IssueDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVDSPercent", Item.VDSPercent);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPercent", Item.IsPercent);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPurchase", Item.IsPurchase);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PaymentDate", Item.PaymentDate);



                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                                                                MessageVM.dpMsgUpdateNotSuccessfully);
                            }

                            #endregion Insert only DetailTable
                        }
                        else
                        {


                            sqlText = "";
                            sqlText += " update DepositTDSDetails set ";

                            sqlText += " BillAmount         =@ItemBillAmount ,";
                            sqlText += " BillDate           =@ItemBillDate ,";
                            sqlText += " BillDeductAmount   =@ItemBillDeductedAmount ,";
                            sqlText += " DepositNumber      =@MasterDepositId ,";
                            sqlText += " PurchaseNumber     =@ItemPurchaseNumber ,";
                            sqlText += " DepositDate        =@MasterDepositDate ,";
                            sqlText += " Remarks            =@ItemRemarks ,";
                            sqlText += " IssueDate          =@ItemIssueDate ,";
                            sqlText += " LastModifiedBy     =@MasterLastModifiedBy ,";
                            sqlText += " LastModifiedOn     =@MasterLastModifiedOn ,";
                            sqlText += " VDSPercent         =@ItemVDSPercent, ";
                            sqlText += " ReverseVDSId       =@MasterReturnID, ";
                            sqlText += " IsPercent          =@ItemIsPercent,";
                            sqlText += " Post               =@MasterPost,";
                            sqlText += " IsPurchase         =@ItemIsPurchase";
                            sqlText += " PaymentDate         =@PaymentDate";
                            sqlText += " where  TDSId       =@MasterDepositId ";
                            sqlText += " and PurchaseNumber =@ItemPurchaseNumber ";
                            sqlText += " and VendorId       =@ItemVendorId ";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;

                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillAmount", Item.BillAmount);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDate", Item.BillDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarks", Item.Remarks);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIssueDate", Item.IssueDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVDSPercent", Item.VDSPercent);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPercent", Item.IsPercent);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PaymentDate", Item.PaymentDate);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPurchase", Item.IsPurchase);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                                                                MessageVM.dpMsgUpdateNotSuccessfully);
                            }
                        }

                        #endregion Find Transaction Exist
                    }

                    #region Remove row
                    sqlText = "";
                    sqlText += " SELECT  distinct VendorId";
                    sqlText += " from DepositTDSDetails WHERE TDSId=@MasterId  ";

                    DataTable dt = new DataTable("Previous");
                    SqlCommand cmdPrevious = new SqlCommand(sqlText, currConn);
                    cmdPrevious.Transaction = transaction;
                    cmdPrevious.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                    SqlDataAdapter dta = new SqlDataAdapter(cmdPrevious);
                    dta.Fill(dt);
                    foreach (DataRow pVId in dt.Rows)
                    {
                        var p = pVId["VendorId"].ToString();

                        //var tt = Details.Find(x => x.ItemNo == p);
                        var tt = vds.Count(x => x.VendorId.Trim() == p.Trim());
                        if (tt == 0)
                        {
                            sqlText = "";
                            sqlText += " delete FROM DepositTDSDetails ";
                            sqlText += " WHERE TDSId=@MasterId ";
                            sqlText += " AND VendorId='" + p + "'";
                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        }

                    }
                    #endregion Remove row
                }
                #endregion VDS
                #region Update PurchaseTDSs

                sqlText = "";

                sqlText += " update PurchaseTDSs set PaymentDate=DepositTDSDetails.PaymentDate ";
                sqlText += " from DepositTDSDetails ";
                sqlText += " where DepositTDSDetails.PurchaseNumber=PurchaseTDSs.PurchaseInvoiceNo ";
                sqlText += " and DepositTDSDetails.PurchaseInvoiceNo='' ";


                SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                cmdUpdateReceive.Transaction = transaction;
                transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgUnableToSaveReceive);
                }

                #endregion Update PurchaseTDSs

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DepositTDSs WHERE DepositId=@MasterId ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Updated";
                retResults[2] = Master.DepositId;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositTDSDAL", "DepositUpdateX", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                        "Please Check Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from DepositTDSs WHERE DepositId=@MasterId";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DepositTDSs set  ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where DepositId=@MasterDepositId ";

                sqlText += " update DepositTDSDetails set  ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where TDSId=@MasterDepositId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", "Y");
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header


                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Posted";
                retResults[2] = Master.DepositId;
                retResults[3] = "Y";
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositTDSDAL", "DepositPost", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] DepositPostX(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                        "Please Check Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from DepositTDSs WHERE DepositId=@MasterId";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DepositTDSs set  ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where DepositId=@MasterDepositId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", "Y");
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region AdjCashPayble
                if (Master.TransactionType == "AdjCashPayble" || Master.TransactionType == "AdjCashPayble-Credit")
                {
                    #region Find ID for Update

                    countId = 0;
                    #region product type existence checking

                    sqlText = "select count(AdjHistoryNo) from AdjustmentHistorys where  AdjHistoryNo = @MasterId";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Could not find requested Adjustment id.");
                    }
                    #endregion
                    #region same name product type existence checking
                    countId = 0;
                    sqlText = "select count(AdjHistoryID) from AdjustmentHistorys where  AdjId=@adjHistoryAdjId";
                    sqlText += " and AdjDate=@adjHistoryAdjDate";
                    sqlText += " and AdjType=@adjHistoryAdjType";
                    sqlText += " and not AdjHistoryNo =@MasterId";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;

                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                    countId = (int)cmdNameExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Same Adjustment name already exist in same date");
                    }
                    #endregion
                    #endregion Find ID for Update

                    #region sql statement

                    sqlText = "";
                    sqlText += "UPDATE AdjustmentHistorys SET";
                    sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                    sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                    sqlText += " Post           =@MasterPost";
                    sqlText += " WHERE AdjHistoryNo =@MasterId";

                    SqlCommand cmdUpdateACP = new SqlCommand(sqlText, currConn);
                    cmdUpdateACP.Transaction = transaction;
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterPost", "Y");
                    cmdUpdateACP.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                    transResult = (int)cmdUpdateACP.ExecuteNonQuery();

                    #endregion
                }
                #endregion AdjCashPayble

                #region VDS
                if (Master.TransactionType == "VDS"
                    || Master.TransactionType == "VDS-Credit"
                    || Master.TransactionType == "SaleVDS"
                    || Master.TransactionType == "PurchaseVDS"
                    )
                {
                    if (tds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }

                    foreach (var Item in tds.ToList())
                    {
                        #region Find Transaction Exist

                        sqlText = "";
                        sqlText += "select COUNT(TDSId) from DepositTDSDetails WHERE TDSId=@MasterId ";
                        sqlText += " AND PurchaseNumber=@ItemPurchaseNumber";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);

                        }
                        else
                        {


                            sqlText = "";
                            sqlText += " update DepositTDSDetails set ";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " Post=@MasterPost,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn";
                            sqlText += " where  TDSId  =@MasterId ";
                            sqlText += " and PurchaseNumber=@ItemPurchaseNumber ";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", "Y");
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                                                                MessageVM.dpMsgUpdateNotSuccessfully);
                            }
                        }

                        #endregion Find Transaction Exist
                    }
                }
                #endregion VDS

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DepositTDSs WHERE DepositId=@MasterId ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "TDS Deposit Successfully Posted";
                retResults[2] = Master.DepositId;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositTDSDAL", "DepositPostX", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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

            #region Result
            return retResults;
            #endregion Result

        }

        #endregion
    }
}
