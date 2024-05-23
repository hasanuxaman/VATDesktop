
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
using Excel;

namespace VATServer.Library
{
    public class DepositDAL : IDeposit
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        public DataTable SearchVDSDT(string DepositNumber, SysDBInfoVMTemp connVM = null, bool chkPurchaseVDS = true)
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
                sqlText = @"SELECT VDS.VDSId, VDS.VendorId,";
                if (chkPurchaseVDS == true)
                {
                    sqlText += @"Vendors.VendorName,";

                }
                else
                {
                    sqlText += @"Customers.CustomerName VendorName,";
                }
                sqlText += @"
VDS.BillAmount,
VDS.BillNo,
VDS.VDSPercent,
VDS.BillDeductAmount VDSAmount,
convert (varchar,BillDate,120)PurchaseDate,
convert (varchar,IssueDate,120)IssueDate,
 VDS.Remarks,
VDS.DepositNumber,
VDS.IsPercent,
isnull(VDS.IsPurchase,'Y')IsPurchase,
isnull(VDS.VATAmount,0)VATAmount,
--, VDS.DepositDate, 
 VDS.PurchaseNumber
--, VendorGroups.VendorGroupName
FROM VDS LEFT OUTER JOIN";
                if (chkPurchaseVDS == true)
                {
                    sqlText += @"
Vendors ON VDS.VendorId = Vendors.VendorID LEFT OUTER JOIN
VendorGroups ON Vendors.VendorGroupID = VendorGroups.VendorGroupID";
                }
                else
                {
                    sqlText += @"
Customers  ON VDS.VendorId=Customers.CustomerID LEFT OUTER JOIN
CustomerGroups vg ON Customers.CustomerGroupID=vg.CustomerGroupID";
                }


                sqlText += @"
WHERE 	 (VDS.VDSId = @DepositNumber )";

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

                FileLogger.Log("DepositDAL", "SearchVDSDT", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("DepositDAL", "SearchVDSDT", ex.ToString() + "\n" + sqlText);

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
                    depositMaster.DepositDate = Convert.ToDateTime(OrdinaryVATDesktop.DateToDate(depositDate)).ToString("yyyy-MM-dd HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
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
                    depositMaster.ChequeDate = Convert.ToDateTime(OrdinaryVATDesktop.DateToDate(chequeDate)).ToString("yyyy-MM-dd HH:mm:ss");
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
                    depositMaster.BankDepositDate = dtDeposit.Rows[i]["BankDepositDate"].ToString().Trim();
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
                    string BillNo = "";
                    foreach (var row in VDSRaws)
                    {
                        BillNo = "";
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
                        BillNo = row["Bill_No"].ToString().Trim();

                        string billDate = OrdinaryVATDesktop.DateToDate(row["Bill_Date"].ToString().Trim());
                        string vdsAmt = row["VDS_Amount"].ToString().Trim();
                        string issueDate = OrdinaryVATDesktop.DateToDate(row["Issue_Date"].ToString().Trim());
                        string purchaseNo = row["Purchase_No"].ToString().Trim();
                        string remarks = cImport.ChecKNullValue(row["Remarks"].ToString().Trim());

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        //vdsDetail.DepositId = NextID.ToString();
                        vdsDetail.VendorId = vendorId;
                        vdsDetail.BillAmount = Convert.ToDecimal(billAmt);
                        vdsDetail.BillNo = BillNo;
                        vdsDetail.BillDate = Convert.ToDateTime(OrdinaryVATDesktop.DateToDate(billDate)).ToString("yyyy-MM-dd HH:mm:ss");
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

                    string[] sqlResults = DepositInsert(depositMaster, vdsMasterVMs, adjustmentHistory, transaction, currConn);
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
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositDAL", "ImportData", ex.ToString());

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

                FileLogger.Log("DepositDAL", "ReverseAmount", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

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

        public string[] ImportDataSingle(DataTable dtDeposit, DataTable dtVDS, int branchId = 1, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
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

                string tranType = dtDeposit.Rows[0]["Transection_Type"].ToString().Trim();

                for (int rows = 0; rows < MRowCount; rows++)
                {
                    #region Check Date

                    bool IsDepositDate, IsChequeDate;
                    //IsDepositDate = cImport.CheckDate(dtDeposit.Rows[rows]["Deposit_Date"].ToString().Trim());
                    //if (IsDepositDate != true)
                    //{
                    //    throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Deposit_Date field.");
                    //}
                    //if (!string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim()))
                    //{
                    //    IsChequeDate = cImport.CheckDate(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim());
                    //    if (IsChequeDate != true)
                    //    {
                    //        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Cheque_Date field.");
                    //    }
                    //}
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
                    if ((string.IsNullOrEmpty(dtDeposit.Rows[rows]["Account_No"].ToString().Trim()) || dtDeposit.Rows[rows]["Account_No"].ToString().Trim() == "-") && tranType != "SaleVDS")
                    {
                        throw new ArgumentNullException("Please insert valid value in Account_No field.");
                    }
                    #endregion Null value check
                    #region Bank Check
                    string bankName = dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim();
                    string branchName = dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim();
                    string accNo = dtDeposit.Rows[rows]["Account_No"].ToString().Trim();

                    //string bankId = cImport.CheckBankID(bankName, branchName, accNo, currConn, transaction);
                    //if (string.IsNullOrEmpty(bankId))
                    //{
                    //    throw new ArgumentNullException("FindBankId", "Bank '(" + bankName + ")' not in database");
                    //}
                    #endregion Bank Check

                    #region Cheque Information check
                    if (dtDeposit.Rows[rows]["Deposit_Type"].ToString().Trim().ToUpper() == "CHEQUE" && tranType != "SaleVDS")
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
                    //cImport.FindVendorId(dtVDS.Rows[i]["Vendor_Name"].ToString().Trim(),
                    //                       dtVDS.Rows[i]["Vendor_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindVendorId
                    #region Check Date

                    bool IsBillDate, IsIssueDate;
                    // BillDate=PurchaseDate
                    //IsBillDate = cImport.CheckDate(dtVDS.Rows[i]["Bill_Date"].ToString().Trim());
                    //if (IsBillDate != true)
                    //{
                    //    throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Bill_Date field.");
                    //}
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

                    var bankIdInDb = dtDeposit.Rows[i]["BankID"].ToString().Trim();
                    string bankId = "";
                    if ((string.IsNullOrEmpty(bankIdInDb) || bankIdInDb == "0") && tranType != "SaleVDS")
                    {
                        bankId = cImport.CheckBankID(bankName, branchName, accNo, currConn, transaction);
                    }
                    else
                    {
                        bankId = bankIdInDb;
                    }

                    #endregion Bank Check
                    string comment = cImport.ChecKNullValue(dtDeposit.Rows[i]["Comments"].ToString().Trim());
                    string dPerson = cImport.ChecKNullValue(dtDeposit.Rows[i]["Deposit_Person"].ToString().Trim());
                    string personDesg = cImport.ChecKNullValue(dtDeposit.Rows[i]["Person_Designation"].ToString().Trim());
                    string personContactNo = cImport.ChecKNullValue(dtDeposit.Rows[i]["Person_ContactNo"].ToString().Trim());
                    string personAddress = cImport.ChecKNullValue(dtDeposit.Rows[i]["Person_Address"].ToString().Trim());
                    string createdBy = dtDeposit.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtDeposit.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtDeposit.Rows[i]["Transection_Type"].ToString().Trim();

                    #region Deposit
                    depositMaster = new DepositMasterVM();
                    //depositMaster.DepositId = NextID.ToString();
                    depositMaster.TreasuryNo = treasuryNo;
                    depositMaster.DepositDate = depositDate; //Convert.ToDateTime(depositDate).ToString("yyyy-MM-dd HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    depositMaster.DepositType = depositType;
                    //depositMaster.DepositAmount = Convert.ToDecimal(vdsAmt);
                    depositMaster.ChequeNo = chequeNo;
                    depositMaster.ChequeBank = chequeBank;
                    depositMaster.ChequeBankBranch = chqBBrunch;
                    //depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");//dtpChequeDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (string.IsNullOrEmpty(chequeDate) || chequeDate == "-")
                    {
                        chequeDate = depositDate;
                    }
                    depositMaster.ChequeDate = Convert.ToDateTime(OrdinaryVATDesktop.DateToDate(chequeDate)).ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.BankId = bankId;
                    depositMaster.TreasuryCopy = "-";
                    depositMaster.DepositPerson = dPerson;
                    depositMaster.DepositPersonDesignation = personDesg;
                    depositMaster.DepositPersonContactNo = personContactNo;
                    depositMaster.DepositPersonAddress = personAddress;
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
                    depositMaster.ImportID = importID;
                    depositMaster.BankDepositDate = dtDeposit.Rows[i]["BankDepositDate"].ToString().Trim();
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
                        string vendorID = row["VendorID"].ToString().Trim();
                        #region FindVendorId

                        string vendorId = "";

                        if (tranType != "SaleVDS")
                        {
                            if (string.IsNullOrEmpty(vendorID) || vendorID == "0")
                            {
                                vendorId = cImport.FindVendorId(vendorName, vendorCode, currConn, transaction, true);
                            }
                            else
                            {
                                vendorId = vendorID;
                            }
                        }
                        else
                        {
                            vendorId = cImport.FindCustomerId(vendorName, vendorCode, currConn, transaction, true);
                        }

                        #endregion FindVendorId
                        string billAmt = row["Bill_Amount"].ToString().Trim();
                        string billDate = OrdinaryVATDesktop.DateToDate(row["Bill_Date"].ToString().Trim());
                        string vdsAmt = row["VDS_Amount"].ToString().Trim();
                        string issueDate = OrdinaryVATDesktop.DateToDate(row["Issue_Date"].ToString().Trim());
                        string purchaseNo = row["Purchase_No"].ToString().Trim();
                        string BillNo = row["BillNo"].ToString().Trim();
                        string remarks = cImport.ChecKNullValue(row["Remarks"].ToString().Trim());

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        //vdsDetail.DepositId = NextID.ToString();
                        vdsDetail.VendorId = vendorId;
                        vdsDetail.BillAmount = Convert.ToDecimal(billAmt);
                        vdsDetail.BillDate = Convert.ToDateTime(OrdinaryVATDesktop.DateToDate(billDate)).ToString("yyyy-MM-dd HH:mm:ss");
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
                        vdsDetail.BillNo = BillNo;
                        vdsMasterVMs.Add(vdsDetail);

                        dCounter++;
                        depositAmt = depositAmt + Convert.ToDecimal(vdsAmt);
                    }
                    #endregion
                    depositMaster.DepositAmount = depositAmt;

                    string[] sqlResults = DepositInsert(depositMaster, vdsMasterVMs, adjustmentHistory, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = "" + "1";
                retResults[3] = "" + "N";
            }

            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("DepositDAL", "ImportDataSingle", ex.ToString());

                throw ex;
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
            #endregion

            return retResults;
        }

        private string[] SaveVDS(DataTable dtTableResult, string BranchCode, string transactionType,
            string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            DataTable dtVDSM = new DataTable();
            DataTable dtVDSD = new DataTable();

            #endregion

            #region try

            try
            {

                if (!dtTableResult.Columns.Contains("Branch_Code"))
                {
                    var column = new DataColumn("Branch_Code") { DefaultValue = BranchCode };
                    dtTableResult.Columns.Add(column);
                }

                var dataView = new DataView(dtTableResult);

                dtVDSM = dataView.ToTable(true, "ID", "Deposit_Type", "Deposit_Date", "Treasury_No", "Cheque_No",
                    "Cheque_Date", "Cheque_Bank", "Cheque_Bank_Branch", "Bank_Name", "Branch_Code", "Branch_Name"
                    , "Account_No", "Deposit_Person", "Person_Designation", "Person_ContactNo", "Person_Address", "Post", "Comments", "BankID", "BankDepositDate");


                dtVDSD = dataView.ToTable(false,
                    "ID", "Vendor_Code", "Vendor_Name",
                    "Bill_Amount", "Bill_Date", "VDS_Amount",
                    "Issue_Date", "Purchase_No", "Remarks", "IsPurchase", "BillNo", "VendorID");

                dtVDSM = OrdinaryVATDesktop.DtColumnAdd(dtVDSM, "Transection_Type", transactionType, "string");
                dtVDSM = OrdinaryVATDesktop.DtColumnAdd(dtVDSM, "Created_By", CurrentUser, "string");
                dtVDSM = OrdinaryVATDesktop.DtColumnAdd(dtVDSM, "LastModified_By", CurrentUser, "string");

                dtVDSD = OrdinaryVATDesktop.DtDateCheck(dtVDSD, new string[] { "Bill_Date", "Issue_Date" });

                var result = ImportDataSingle(dtVDSM, dtVDSD, branchId, VcurrConn, Vtransaction);

                return result;
            }
            #endregion

            #region catch

            catch (Exception e)
            {
                FileLogger.Log("DepositDAL", "SaveVDS", e.ToString());

                throw e;
            }
            #endregion

        }

        public string[] SaveTempVDS(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
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

                var commonDal = new CommonDAL();
                bool AutoSave = commonDal.settingValue("AutoSave", "VDSBank") == "Y";

                sqlText = @"delete from TempVDSData;";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.ExecuteNonQuery();

                var bulkRes = commonDal.BulkInsert("TempVDSData", data, currConn, transaction);

                if (bulkRes[0].ToLower() == "fail")
                {
                    throw new Exception("Import Failed to Temp");
                }

                var updateVendorId =
                    @"update TempVDSData set VendorID = Vendors.VendorID from Vendors where Vendors.VendorCode = TempVDSData.Vendor_Code and  TempVDSData.VendorID is null
                      update TempVDSData set VendorID = Vendors.VendorID from Vendors where Vendors.VendorName = TempVDSData.Vendor_Name and  TempVDSData.VendorID is null;";

                var bankUpdate =
                    @"update TempVDSData set BankID = BankInformations.BankID from BankInformations where (BankInformations.BranchName like '%'+TempVDSData.Branch_Name+'%') and ( BankInformations.BankName like '%'+TempVDSData.Bank_Name+'%') and ( BankInformations.AccountNumber like '%'+TempVDSData.Account_No+'%' );";

                var branchUpdate = @"update TempVDSData set BranchId = BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode = TempVDSData.Branch_Code;";

                var getAll = @"select * from TempVDSData";

                cmd.CommandText = updateVendorId + " " + branchUpdate + " " + bankUpdate;

                cmd.ExecuteNonQuery();

                #region Auto Save new Bank

                if (AutoSave)
                {
                    string getBankTempvdsdata = @"select distinct Bank_Name,Branch_Name,Account_No from TempVDSData where BankID is null or BankID=''
and Bank_Name is not null and Bank_Name!='' and Bank_Name !='-'
";

                    DataTable dtBank = new DataTable();
                    cmd.CommandText = getBankTempvdsdata;
                    var ad = new SqlDataAdapter(cmd);
                    ad.Fill(dtBank);

                    if (dtBank != null && dtBank.Rows.Count > 0)
                    {
                        BankInformationDAL _bankInformationDal = new BankInformationDAL();

                        string[] result =
                            _bankInformationDal.ImportToBankInformation(dtBank, true, currConn, transaction, connVM, CurrentUser);

                        cmd.CommandText = bankUpdate;
                        cmd.ExecuteNonQuery();

                    }

                }

                #endregion

                var table = new DataTable();

                cmd.CommandText = getAll;
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                retResults = SaveVDS(table, BranchCode, transactionType, CurrentUser, branchId, currConn, transaction);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #region SuccessResult
                //retResults[0] = "Success";
                //retResults[1] = "Data Synchronized Successfully.";
                //retResults[2] = "";
                #endregion SuccessResult

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("DepositDAL", "SaveTempVDS", ex.ToString() + "\n" + sqlText);

                throw ex;
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVDS_Split(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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

                CommonDAL commonDal = new CommonDAL();

                #region delete and bulk insert to Source

                string deleteSource = @"delete from VAT_Source_VDS";
                SqlCommand cmd = new SqlCommand(deleteSource, currConn, transaction);
                cmd.ExecuteNonQuery();



                string[] result = commonDal.BulkInsert("VAT_Source_VDS", param.Data, currConn, transaction);

                #endregion

                #region delete duplicate

                string deleteDuplicate = @"
update  VAT_Source_VDS                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(Deposit_Date)) +  CONVERT(VARCHAR(4),YEAR(Deposit_Date)),6)
where PeriodId is null or PeriodId = ''

delete from VAT_Source_VDS where ID in (
                select v.ID from VAT_Source_VDS v inner join Deposits d
                on v.ID = d.ImportIDExcel and v.PeriodId = d.PeriodId)";

                cmd.CommandText = deleteDuplicate;
                cmd.ExecuteNonQuery();
                #endregion

                #region Loop counter

                string getLoopCount = @"select Ceiling(count(distinct ID)/5.00) from VAT_Source_VDS";

                cmd.CommandText = getLoopCount;
                int counter = Convert.ToInt32(cmd.ExecuteScalar());

                #endregion

                transaction.Commit();
                currConn.Close();

                DataTable sourceData = new DataTable();

                for (int i = 0; i < counter; i++)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                    cmd.Connection = currConn;
                    cmd.Transaction = transaction;

                    #region Create Temp tables

                    string tempTableCreate = @"create table #tempIds(sl int identity(1,1), ID varchar(500))";
                    cmd.CommandText = tempTableCreate;
                    cmd.ExecuteNonQuery();

                    #endregion

                    #region Get Top Rows

                    string insertIds = @"insert into #tempIds(ID)
select  distinct top 5 ID 
from VAT_Source_VDS
where isnull(IsProcessed,'N') = 'N'";

                    cmd.CommandText = insertIds;
                    cmd.ExecuteNonQuery();

                    string getData = @"SELECT 
       [ID]
      ,[Deposit_Type]
      ,[Deposit_Date]
      ,[Treasury_No]
      ,[Cheque_No]
      ,[Cheque_Date]
      ,[Cheque_Bank]
      ,[Cheque_Bank_Branch]
      ,[Bank_Name]
      ,[Branch_Code]
      ,[Account_No]
      ,[Deposit_Person]
      ,[Person_Designation]
      ,[Post]
      ,[Comments]
      ,[Vendor_Code]
      ,[Vendor_Name]
      ,[Bill_Amount]
      ,[Bill_Date]
      ,[VDS_Amount]
      ,[Issue_Date]
      ,[Purchase_No]
      ,[IsPurchase]
      ,[VendorID]
      ,[BranchId]
      ,[BomId]
      ,[Branch_Name]
      ,[Remarks]
      ,[BankID]
      ,[BankDepositDate]
      ,[BillNo]
      ,[Person_ContactNo]
      ,[Person_Address]
  FROM VAT_Source_VDS where ID in (select ID from #tempIds)";

                    cmd.CommandText = getData;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(sourceData);

                    //sourceData.Columns.Remove("IsProcessed");

                    #endregion

                    retResults = SaveTempVDS(sourceData, param.BranchCode, param.TransactionType, param.CurrentUser, param.DefaultBranchId,
                         currConn, transaction);

                    #region updateSourceTable

                    string updateSourceAndClearTemp = @"update VAT_Source_VDS set IsProcessed = 'Y' where ID  in (select ID from #tempIds);
                                            --delete from #tempIds;";

                    cmd.CommandText = updateSourceAndClearTemp;
                    cmd.ExecuteNonQuery();

                    #endregion

                    transaction.Commit();
                    currConn.Close();
                    transaction.Dispose();
                    currConn.Dispose();

                    sourceData.Clear();
                }

                #region Drop Temp table

                //currConn = _dbsqlConnection.GetConnection();
                //currConn.Open();
                //transaction = currConn.BeginTransaction();
                //cmd.Connection = currConn;
                //cmd.Transaction = transaction;

                //string dropTemp = @"drop table #tempIds";
                //cmd.CommandText = dropTemp;
                //cmd.ExecuteNonQuery();


                //transaction.Commit();
                //currConn.Close();
                //transaction.Dispose();
                //currConn.Dispose();

                #endregion

                return retResults;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log("DepositDAL", "SaveVDS_Split", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

            }
            #endregion
        }

        public string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null, string TransactionType = "")
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

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
                if (TransactionType.ToLower() == "salevds")
                {
                    sqlText = @"update SalesInvoiceHeaders set IsVDS = @flag where SalesInvoiceNo in (select PurchaseNumber from VDS where VDSId = @VDSId)";

                }
                else
                {

                    sqlText = @"update PurchaseInvoiceHeaders set IsVDSCompleted = @flag where PurchaseInvoiceNo in (select PurchaseNumber from VDS where VDSId = @VDSId)";
                }

                var cmd = new SqlCommand(sqlText, currConn);

                cmd.Parameters.AddWithValueAndNullHandle("@VDSId", VdsId);
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

                FileLogger.Log("DepositDAL", "UpdateVdsComplete", ex.ToString());

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

        public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
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

                sqlText = @"SELECT 
       dp.[DepositId] ID
      ,dp.[DepositType] Deposit_Type
      ,cast(dp.[DepositDateTime] as varchar(100)) Deposit_Date
	  ,cast(dp.BankDepositDate as varchar(100)) BankDepositDate
      ,dp.[TreasuryNo] Treasury_No
      ,dp.[ChequeNo] Cheque_No
	  ,cast(dp.[ChequeDate] as varchar(100)) Cheque_Date
      ,dp.[ChequeBank] Cheque_Bank
      ,dp.[ChequeBankBranch] Cheque_Bank_Branch
	  ,bi.BankName Bank_Name
	  ,bi.BranchName Branch_Name

      ,dp.[DepositPerson] Deposit_Person
      ,dp.[DepositPersonDesignation] Person_Designation
      ,dp.DepositPersonContactNo Person_ContactNo
      ,dp.DepositPersonAddress Person_Address
      ,dp.[Post]
      ,dp.[Comments]

	  ,v.VendorName Vendor_Name
	  ,v.VendorCode Vendor_Code

	  ,vd.BillAmount Bill_Amount
	  ,cast(vd.BillDate as varchar(100)) Bill_Date
	  ,vd.BillDeductAmount   VDS_Amount
	  ,cast(vd.IssueDate as varchar(100)) Issue_Date
	  ,vd.PurchaseNumber Purchase_No
	  ,vd.Remarks 
	  ,vd.IsPurchase
	  ,vd.BillNo
	  ,bi.AccountNumber Account_No

  FROM Deposits dp left outer join VDS vd
  on dp.DepositId = vd.DepositNumber
  left outer join BankInformations bi
  on dp.BankID = bi.BankID left outer join Vendors v
  on vd.VendorID = v.VendorID

  where dp.[DepositId] in ( ";

                var len = invoiceList.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + invoiceList[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("DepositDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        public DataTable GetExcelDataWithCustomer(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
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


                sqlText = @"SELECT 
       dp.[DepositId] ID
      ,dp.[DepositType] Deposit_Type
      ,cast(dp.[DepositDateTime] as varchar(100)) Deposit_Date
	  ,cast(dp.BankDepositDate as varchar(100)) BankDepositDate
      ,dp.[TreasuryNo] Treasury_No
      ,dp.[ChequeNo] Cheque_No
	  ,cast(dp.[ChequeDate] as varchar(100)) Cheque_Date
      ,dp.[ChequeBank] Cheque_Bank
      ,dp.[ChequeBankBranch] Cheque_Bank_Branch
	  ,bi.BankName Bank_Name
	  ,bi.BranchName Branch_Name

      ,dp.[DepositPerson] Deposit_Person
      ,dp.[DepositPersonDesignation] Person_Designation
      ,dp.DepositPersonContactNo Person_ContactNo
      ,dp.DepositPersonAddress Person_Address
      ,dp.[Post]
      ,dp.[Comments]

	  ,v.CustomerName Customer_Name
	  ,v.CustomerCode Customer_Code

	  ,vd.BillAmount Bill_Amount
	  ,cast(vd.BillDate as varchar(100)) Bill_Date
	  ,vd.BillDeductAmount   VDS_Amount
	  ,cast(vd.IssueDate as varchar(100)) Issue_Date
	  ,vd.PurchaseNumber Purchase_No

	  ,vd.Remarks 
	  ,vd.IsPurchase
	  ,bi.AccountNumber Account_No

  FROM Deposits dp left outer join VDS vd
  on dp.DepositId = vd.DepositNumber
  left outer join BankInformations bi
  on dp.BankID = bi.BankID left outer join Customers v
  on vd.VendorID = v.CustomerID

  where dp.[DepositId] in ( ";

                var len = invoiceList.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + invoiceList[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("DepositDAL", "GetExcelDataWithCustomer", ex.ToString() + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        #region web methods

        public List<DepositMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string transactionOpening = "")
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

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, false, connVM, transactionOpening);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new DepositMasterVM();
                        vm.Id = dr["Id"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.DepositId = dr["DepositId"].ToString();
                        vm.TreasuryNo = dr["TreasuryNo"].ToString();
                        vm.DepositDate = OrdinaryVATDesktop.DateTimeToDate(dr["DepositDateTime"].ToString());
                        vm.BankDepositDate = OrdinaryVATDesktop.DateTimeToDate(dr["BankDepositDate"].ToString());
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
                    catch (Exception e) { }
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

                FileLogger.Log("DepositDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string transactionOpening = "", string CashPayableSearch = "")
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            string count = "100";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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
                #region SqlText
                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"

 
d.DepositId
,d.TreasuryNo
,d.DepositDateTime
,d.DepositType
,d.DepositAmount
,d.ChequeNo
,isnull(d.BankDepositDate,'')BankDepositDate
,d.ChequeBank
,d.ChequeBankBranch
,d.ChequeDate
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
,isnull(d.DepositPersonContactNo,'N/A')DepositPersonContactNo
,isnull(d.DepositPersonAddress,'N/A')DepositPersonAddress 
,b.BankName
,b.BranchName
,b.AccountNumber
,b.City
,d.BankID
,d.ReverseDepositId
,isnull(d.BranchId,0)BranchId 
,d.Id

FROM Deposits d
LEFT OUTER JOIN BankInformations b ON d.BankID = b.BankID
WHERE  1=1
";

                #region Cash Payable

                if (!string.IsNullOrWhiteSpace(CashPayableSearch) && CashPayableSearch.ToLower() == "all")
                {
                    sqlText = sqlText + @" 
and d.TransactionType IN 
(
'WithoutBankPay'
,'DevelopmentSurcharge'
,'EnvironmentProtectionSurcharge'
,'ExciseDuty'
,'FineOrPenalty'
,'HelthCareSurcharge'
,'ICTDevelopmentSurcharge'
,'InterestOnOveredSD'
,'InterestOnOveredVAT'
,'FinePenaltyForNonSubmissionOfReturn'
)
";
                }
                #endregion

                #endregion

                sqlTextCount += @" select count(d.DepositId)RecordCount
               FROM Deposits d
LEFT OUTER JOIN BankInformations b ON d.BankID = b.BankID
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlTextParameter += @" and Id=@Id";
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
                if (transactionOpening != "")
                {
                    sqlTextParameter += @" or TransactionType=@TransactionType1";
                }
                #endregion SqlText
                #region SqlExecution
                sqlText = sqlText + " " + sqlTextParameter;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;



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
                if (transactionOpening != "")
                {
                    da.SelectCommand.Parameters.AddWithValue("@TransactionType1", transactionOpening);

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

        public string[] ImportExcelFile(DepositMasterVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImportExcelFile"; //Method Name
            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();


                #region Excel Reader

                string FileName = paramVM.File.FileName;
                //string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                //System.IO.File.Delete(Fullpath);
                //if (paramVM.File != null && paramVM.File.ContentLength > 0)
                //{
                //    paramVM.File.SaveAs(Fullpath);
                //}


                //FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);

                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(paramVM.File.InputStream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(paramVM.File.InputStream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                dt = ds.Tables[0];
                reader.Close();
                //System.IO.File.Delete(Fullpath);
                #endregion

                DataTable dtVdsM = new DataTable();
                dtVdsM = ds.Tables["VDSM"];

                //dtVdsM = OrdinaryVATDesktop.DtDateCheck(dtPurchaseM, new string[] { "Invoice_Date", "Receive_Date" });

                #region Data Insert

                //PurchaseDAL puchaseDal = new PurchaseDAL();
                //retResults = puchaseDal.SaveTempPurchase(dtPurchaseM, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, paramVM.BranchId, () => { }, null, null, connVM, paramVM.CurrentUser, true);

                dt = dtVdsM.Copy();

                dt.Columns["Effect_Date"].ColumnName = "Deposit_Date";


                if (paramVM.TransactionType == "SaleVDS")
                {
                    dt.Columns["VDS_Certificate_No"].ColumnName = "Treasury_No";
                    dt.Columns["VDS_Certificate_Date"].ColumnName = "BankDepositDate";
                    dt.Columns["Tax_Deposit_Account_Code"].ColumnName = "Cheque_No";
                    dt.Columns["Tax_Deposit_Date"].ColumnName = "Cheque_Date";
                    dt.Columns["Tax_Deposit_Serial_No"].ColumnName = "Cheque_Bank";
                    dt.Columns["Customer_Name"].ColumnName = "Vendor_Name";
                    dt.Columns["Customer_Code"].ColumnName = "Vendor_Code";

                }


                retResults = SaveTempVDS(dt, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser,
                                paramVM.BranchId, null, null, connVM);




                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[4]);
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                ////////retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("PurchaseDAL", "ImportExcelFile", ex.ToString() + "\n" + sqlText);
                throw ex;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        #endregion

        #region need to parameterize done

        //currConn to VcurrConn 25-Aug-2020
        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

            //////SqlConnection vcurrConn = VcurrConn;
            //////if (vcurrConn == null)
            //////{
            //////    VcurrConn = null;
            //////    Vtransaction = null;
            //////}

            int transResult = 0;
            string sqlText = "";
            string newIDCreate = "";
            string PostStatus = "";
            int IDExist = 0;

            #endregion Initializ

            #region Try

            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.DepositDate).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion Find Month Lock

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

                #region Find DepositType Exist

                if (Master.DepositType == "OpeningAdjustment" || Master.DepositType == "Opening" || Master.DepositType == "ClosingBalanceVAT(18.6)" || Master.DepositType == "ClosingBalanceSD(18.6)")
                {

                    IDExist = DepositTypeExist(Master.TransactionType, Master.DepositType, transaction, currConn, null);

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, "Transaction Type: " + Master.TransactionType + " Deposit Type: " + Master.DepositType + " Already Used!");
                    }

                }

                #endregion

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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId= @MasterId ";
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
                if (Master.TransactionType == "Treasury"
                    || Master.TransactionType == "Treasury-Opening"
                    || Master.TransactionType == "SD"
                    || Master.TransactionType == "SD-Opening"
                    )
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "Treasury", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }


                else if (Master.TransactionType == "VDS")
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "VDS", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "SaleVDS")
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "VDSSale", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "AdjCashPayble"
                    || Master.TransactionType == "WithoutBankPay"
                    || Master.TransactionType == "DevelopmentSurcharge"
                    || Master.TransactionType == "EnvironmentProtectionSurcharge"
                    || Master.TransactionType == "ExciseDuty"
                    || Master.TransactionType == "FineOrPenalty"
                    || Master.TransactionType == "HelthCareSurcharge"
                    || Master.TransactionType == "ICTDevelopmentSurcharge"
                    || Master.TransactionType == "InterestOnOveredSD"
                    || Master.TransactionType == "InterestOnOveredVAT"
                    || Master.TransactionType == "FinePenaltyForNonSubmissionOfReturn"
                    )
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "AdjCashPayble", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "Treasury-Credit" || Master.TransactionType == "Treasury-Opening-Credit")
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "Treasury-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }

                else if (Master.TransactionType == "VDS-Credit")
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "VDS-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "AdjCashPayble-Credit")
                {
                    newIDCreate = commonDal.TransactionCode("Deposit", "AdjCashPayble-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                Master.DepositId = newIDCreate;
                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into Deposits(";
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
                sqlText += " ImportIDExcel,";
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
                sqlText += " @ImportID,";
                sqlText += " @BranchId";

                sqlText += ") SELECT SCOPE_IDENTITY()";


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
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ImportID", Master.ImportID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                Id = transResult.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Deposit Insert

                retResults = DepositInsert2(Master, vds, adjHistory, transaction, currConn);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, retResults[1]);
                }

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"
UPDATE AdjustmentHistorys 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(AdjDate)) +  CONVERT(VARCHAR(4),YEAR(AdjDate)),6)
WHERE AdjHistoryID = @DepositId


UPDATE Deposits 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DepositDateTime)) +  CONVERT(VARCHAR(4),YEAR(DepositDateTime)),6)
WHERE DepositId = @DepositId

UPDATE VDS 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DepositDate)) +  CONVERT(VARCHAR(4),YEAR(DepositDate)),6)
WHERE VDSId = @DepositId

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@DepositId", Master.DepositId);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }

                ////if (vcurrConn == null)
                ////{
                ////    if (Vtransaction != null)
                ////    {
                ////        if (transResult > 0)
                ////        {
                ////            Vtransaction.Commit();
                ////        }
                ////    }
                ////}

                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = "" + Master.DepositId;
                retResults[3] = "N";
                retResults[4] = Id;

                #endregion SuccessResult

            }

            #endregion Try

            #region Catch and Finall

            #region Catch

            #region Comment before 28 Oct 2020

            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}

            #endregion

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositDAL", "DepositInsert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());
            }

            #endregion Catch

            #region Finally

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #region Comment before 28 Oct 2020

                ////if (vcurrConn == null)
                ////{
                ////    if (VcurrConn != null)
                ////    {
                ////        if (VcurrConn.State == ConnectionState.Open)
                ////        {
                ////            VcurrConn.Close();
                ////        }
                ////    }
                ////}

                #endregion

            }

            #endregion Finally

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public int DepositTypeExist(string TransactionType, string DepositType, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {

            int retResults = 0;

            #region try

            try
            {

                string sqlText = "";
                int IDExist = 0;

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + @"
------declare @TransactionType varchar(50) = 'Treasury-Opening'
------declare @DepositType varchar(50) = 'Opening'

select Count(DepositId) from Deposits 
WHERE 1=1 
and TransactionType=@TransactionType
and Post='Y' and DepositType=@DepositType
";
                SqlCommand cmdTType = new SqlCommand(sqlText, VcurrConn);
                cmdTType.Transaction = Vtransaction;
                cmdTType.Parameters.AddWithValueAndNullHandle("@TransactionType", TransactionType);
                cmdTType.Parameters.AddWithValueAndNullHandle("@DepositType", DepositType);
                IDExist = Convert.ToInt32(cmdTType.ExecuteScalar());
                if (IDExist > 0)
                {
                    retResults = 1;
                }


                #endregion Find Transaction Exist

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("DepositDAL", "DepositTypeExist", ex.ToString());

                throw ex;
            }
            #endregion

            #region finally
            finally
            {

            }
            #endregion

            #region Result

            return retResults;

            #endregion Result


        }

        //currConn to VcurrConn 25-Aug-2020
        public string[] DepositInsert2(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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
            int IDExist = 0;

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


                if (Master.TransactionType == "AdjCashPayble"
                    || Master.TransactionType == "AdjCashPayble-Credit"
                    || Master.TransactionType == "WithoutBankPay"
                    || Master.TransactionType == "DevelopmentSurcharge"
                    || Master.TransactionType == "EnvironmentProtectionSurcharge"
                    || Master.TransactionType == "ExciseDuty"
                    || Master.TransactionType == "FineOrPenalty"
                    || Master.TransactionType == "HelthCareSurcharge"
                    || Master.TransactionType == "ICTDevelopmentSurcharge"
                    || Master.TransactionType == "InterestOnOveredSD"
                    || Master.TransactionType == "InterestOnOveredVAT"
                    || Master.TransactionType == "FinePenaltyForNonSubmissionOfReturn"
                    )
                {
                    #region new id generation
                    sqlText = "select isnull(max(cast(AdjHistoryID as int)),0)+1 FROM  AdjustmentHistorys";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert, "Unable to create new Adjustment No");
                    }
                    #endregion product type new id generation


                    #region Insert

                    sqlText = "";



                    sqlText += "insert into AdjustmentHistorys";
                    sqlText += "(";

                    sqlText += " AdjHistoryID,";
                    sqlText += " AdjId,";
                    sqlText += " AdjDate,";
                    sqlText += " AdjAmount,";
                    sqlText += " AdjVATRate,";
                    sqlText += " AdjVATAmount,";
                    sqlText += " AdjSD,";
                    sqlText += " AdjSDAmount,";
                    sqlText += " AdjOtherAmount,";
                    sqlText += " AdjType,";
                    sqlText += " AdjDescription,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " AdjInputAmount,";
                    sqlText += " AdjInputPercent,";
                    sqlText += " AdjReferance,";
                    sqlText += " Post,";
                    sqlText += " ReverseAdjHistoryNo,";
                    sqlText += " AdjHistoryNo,";
                    sqlText += " BranchId";


                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@adjHistoryAdjId,";
                    sqlText += "@adjHistoryAdjDate,";
                    sqlText += "@adjHistoryAdjAmount,";
                    sqlText += "@adjHistoryAdjVATRate,";
                    sqlText += "@adjHistoryAdjVATAmount,";
                    sqlText += "@adjHistoryAdjSD,";
                    sqlText += "@adjHistoryAdjSDAmount,";
                    sqlText += "@adjHistoryAdjOtherAmount,";
                    sqlText += "@adjHistoryAdjType,";
                    sqlText += "@adjHistoryAdjDescription,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@adjHistoryAdjInputAmount,";
                    sqlText += "@adjHistoryAdjInputPercent,";
                    sqlText += "@adjHistoryAdjReferance,";
                    sqlText += "@MasterPost,";
                    sqlText += "@MasterReturnID,";
                    sqlText += "@newID,";
                    sqlText += "@BranchId";



                    sqlText += ")";

                    SqlCommand cmdInsert1 = new SqlCommand(sqlText, currConn);
                    cmdInsert1.Transaction = transaction;

                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@nextId", nextId);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjAmount", adjHistory.AdjAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjVATRate", adjHistory.AdjVATRate);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjVATAmount", adjHistory.AdjVATAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjSD", adjHistory.AdjSD);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjSDAmount", adjHistory.AdjSDAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjOtherAmount", adjHistory.AdjOtherAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDescription", adjHistory.AdjDescription);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjInputAmount", adjHistory.AdjInputAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjInputPercent", adjHistory.AdjInputPercent);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjReferance", adjHistory.AdjReferance);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@newID", Master.DepositId);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);


                    transResult = (int)cmdInsert1.ExecuteNonQuery();
                    #endregion Insert
                }

                else if (Master.TransactionType == "VDS"
                    || Master.TransactionType == "VDS-Credit"
                    || Master.TransactionType == "SaleVDS"

                    )
                {
                    #region Validation for Detail

                    if (vds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }


                    #endregion Validation for Detail

                    #region Insert Detail Table

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        if (Item.PurchaseNumber.Trim().ToUpper() != "NA")
                        {
                            sqlText = "";
                            sqlText += "select COUNT(VDSId) from VDS WHERE DepositNumber='" + Master.DepositId + "' ";
                            sqlText += " AND PurchaseNumber=@ItemPurchaseNumber";
                            SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                            cmdFindId.Transaction = transaction;
                            cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                            IDExist = (int)cmdFindId.ExecuteScalar();



                            //DataTable dataTable = new DataTable("");
                            //SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                            //cmdIdExist.Transaction = transaction;
                            //SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                            //reportDataAdapt.Fill(dataTable);


                            if (IDExist > 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                                MessageVM.dpMsgFindExistIDP);
                            }
                        }

                        #endregion Find Transaction Exist

                        if (string.IsNullOrEmpty(Item.DepositNumber))
                        {
                            Item.DepositNumber = Master.DepositId;

                        }

                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into VDS(";

                        sqlText += " VDSId";
                        sqlText += " ,VendorId";
                        sqlText += " ,BillAmount";
                        sqlText += " ,BillNo";

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
                        sqlText += " ,VATAmount";

                        sqlText += " )";

                        sqlText += " values(	";

                        sqlText += "'" + Master.DepositId + "',";
                        sqlText += "@ItemVendorId,";
                        sqlText += "@ItemBillAmount,";
                        sqlText += "@ItemBillNo,";

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
                        sqlText += "@VATAmount";


                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillAmount", Item.BillAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillNo", Item.BillNo);

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDate", Item.BillDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDepositNumber", Item.DepositNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseNumber", Item.PurchaseNumber);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositDate", Master.DepositDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRemarks", Item.Remarks);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIssueDate", Item.IssueDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVDSPercent", Item.VDSPercent);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPercent", Item.IsPercent);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemIsPurchase", Item.IsPurchase);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Item.VATAmount);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                            MessageVM.dpMsgSaveNotSuccessfully);
                        }

                        #endregion Insert only DetailTable


                    }


                    #endregion Insert Detail Table

                }

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = Master.DepositId;
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

                FileLogger.Log("DepositDAL", "DepositInsert2", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
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
            int DepositIDExist = 0;
            CommonDAL commonDal = new CommonDAL();

            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.DepositDate).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }
                else if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                #endregion Find Month Lock

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

                #region Find DepositType Exist

                if (Master.DepositType == "OpeningAdjustment" || Master.DepositType == "Opening" || Master.DepositType == "ClosingBalanceVAT(18.6)" || Master.DepositType == "ClosingBalanceSD(18.6)")
                {


                    DepositIDExist = DepositTypeExist(Master.TransactionType, Master.DepositType, transaction, currConn, null);

                    if (DepositIDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, "Transaction Type: " + Master.TransactionType + " Deposit Type: " + Master.DepositType + " Already Used!");
                    }




                }

                #endregion

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId=@MasterId ";
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

                string settingValue = commonDal.settingValue("EntryProcess", "UpdateOnPost");

                if (settingValue != "Y")
                {
                    if (Master.Post == "Y")
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.ThisTransactionWasPosted);
                    }

                }

                #region update VDS

                sqlText = @"update PurchaseInvoiceHeaders set IsVDSCompleted = 'N' where PurchaseInvoiceNo in (select PurchaseNumber from VDS where VDSId = @VDSId)";
                var cmdsVDS = new SqlCommand(sqlText, currConn);
                cmdsVDS.Parameters.AddWithValueAndNullHandle("@VDSId", Master.DepositId);
                cmdsVDS.Transaction = transaction;
                cmdsVDS.ExecuteNonQuery();


                #endregion

                #region update Header

                sqlText = "";

                sqlText += " update Deposits set  ";
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

                #endregion ID check completed,update Information in Header

                #region select Deposit

                string[] cFields = { "d.DepositId" };
                string[] cValues = { Master.DepositId };

                DepositMasterVM varDepositVM = new DepositDAL().SelectAllList(0, cFields, cValues, currConn, transaction, null).FirstOrDefault();

                #endregion

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM AdjustmentHistorys WHERE AdjHistoryNo=@MasterDepositId ";
                sqlText += @" delete FROM VDS WHERE VDSId=@MasterDepositId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion

                #region Deposit Insert

                Master.CreatedBy = varDepositVM.CreatedBy;
                Master.CreatedOn = varDepositVM.CreatedOn;

                retResults = DepositInsert2(Master, vds, adjHistory, transaction, currConn);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, retResults[1]);
                }

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"


UPDATE AdjustmentHistorys 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(AdjDate)) +  CONVERT(VARCHAR(4),YEAR(AdjDate)),6)
WHERE AdjHistoryID = @DepositId


UPDATE Deposits 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DepositDateTime)) +  CONVERT(VARCHAR(4),YEAR(DepositDateTime)),6)
WHERE DepositId = @DepositId

UPDATE VDS 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DepositDate)) +  CONVERT(VARCHAR(4),YEAR(DepositDate)),6)
WHERE VDSId = @DepositId

";

                cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@DepositId", Master.DepositId);

                transResult = cmdUpdate.ExecuteNonQuery();



                #endregion

                #region Commit

                if (transaction != null)
                {
                    //if (transResult > 0)
                    //{
                    transaction.Commit();
                    //}

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgUpdateSuccessfully;
                retResults[2] = Master.DepositId;
                retResults[3] = "N";
                #endregion SuccessResult

            }

            #endregion Try

            #region Catch and Finall

            #region Catch

            #region Comment before 28 Oct 2020

            //////catch (SqlException sqlex)
            //////{
            //////    transaction.Rollback();
            //////    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //////    //throw sqlex;
            //////}

            #endregion Catch and Finall

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (currConn != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("DepositDAL", "DepositUpdate", ex.ToString() + "\n" + sqlText);

                ////////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
            }

            #endregion Catch

            #region Finally

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

            #endregion Finally

            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
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

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.DepositDate).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion Find Month Lock

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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId=@MasterId";
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
                #region Post

                sqlText = "";
                sqlText += @" Update  AdjustmentHistorys             set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE AdjHistoryNo=@MasterDepositId ";
                sqlText += @" Update  Deposits             set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE DepositId=@MasterDepositId ";
                sqlText += @" Update  VDS               set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE VDSId=@MasterDepositId ";
                sqlText += @" Update  SalesInvoiceHeaders     set  IsVDSCompleted ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn     WHERE SalesInvoiceNo IN (select purchaseNumber from VDS  WHERE VDSId=@MasterDepositId) ";



                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterDepositId", Master.DepositId);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion


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
                retResults[1] = MessageVM.dpMsgPostSuccessfully;
                retResults[2] = Master.DepositId;
                retResults[3] = "Y";
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (currConn != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("DepositDAL", "DepositPost", ex.ToString() + "\n" + sqlText);

                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
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

            #region Result
            return retResults;
            #endregion Result

        }

        public ResultVM MultiplePost(DepositMasterVM paramVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

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

                #region Post

                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {

                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sSal.ID IN('" + IDs + "')";

                    sqlText = "";
                    sqlText += @" Update  AdjustmentHistorys             set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE AdjHistoryNo IN('" + IDs + "')";
                    sqlText += @" Update  Deposits             set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE DepositId IN('" + IDs + "')";
                    sqlText += @" Update  VDS               set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE VDSId IN('" + IDs + "')";

                    SqlCommand cmdPost = new SqlCommand(sqlText, currConn);
                    cmdPost.Transaction = transaction;

                    string LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    cmdPost.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);
                    cmdPost.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", paramVM.CurrentUser);

                    transResult = cmdPost.ExecuteNonQuery();

                }

                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();

                    }
                }

                #endregion Commit

                #region SuccessResult

                rVM.Status = "Success";
                rVM.Message = MessageVM.saleMsgSuccessfullyPost;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                rVM = new ResultVM();
                rVM.Message = ex.Message;
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("DepositDAL", "MultiplePost", ex.ToString() + "\n" + sqlText);

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
            return rVM;
            #endregion Result

        }

        #endregion

        #region UnUsed ////Before Jan-26-2020

        //currConn to VcurrConn 25-Aug-2020
        public string[] DepositInsertX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

                //////if (vcurrConn == null)
                //////{
                //////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                //////    if (VcurrConn.State != ConnectionState.Open)
                //////    {
                //////        VcurrConn.Open();
                //////    }

                //////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                //////}


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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId= @MasterId ";
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
                if (Master.TransactionType == "Treasury"
                    || Master.TransactionType == "SD"
                    || Master.TransactionType == "Treasury-Opening"
                    )
                {
                    newID = commonDal.TransactionCode("Deposit", "Treasury", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }


                else if (Master.TransactionType == "VDS")
                {
                    newID = commonDal.TransactionCode("Deposit", "VDS", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "SaleVDS")
                {
                    newID = commonDal.TransactionCode("Deposit", "VDS", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "AdjCashPayble"
                    || Master.TransactionType == "WithoutBankPay"
                    || Master.TransactionType == "DevelopmentSurcharge"
                    || Master.TransactionType == "EnvironmentProtectionSurcharge"
                    || Master.TransactionType == "ExciseDuty"
                    || Master.TransactionType == "FineOrPenalty"
                    || Master.TransactionType == "HelthCareSurcharge"
                    || Master.TransactionType == "ICTDevelopmentSurcharge"
                    || Master.TransactionType == "InterestOnOveredSD"
                    || Master.TransactionType == "InterestOnOveredVAT"
                    )
                {
                    newID = commonDal.TransactionCode("Deposit", "AdjCashPayble", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "Treasury-Credit" || Master.TransactionType == "Treasury-Opening-Credit")
                {
                    newID = commonDal.TransactionCode("Deposit", "Treasury-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }

                else if (Master.TransactionType == "VDS-Credit")
                {
                    newID = commonDal.TransactionCode("Deposit", "VDS-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }
                else if (Master.TransactionType == "AdjCashPayble-Credit")
                {
                    newID = commonDal.TransactionCode("Deposit", "AdjCashPayble-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, Master.BranchId.ToString(), currConn, transaction);
                }


                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into Deposits(";
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

                if (Master.TransactionType == "AdjCashPayble" || Master.TransactionType == "AdjCashPayble-Credit")
                {
                    #region new id generation
                    sqlText = "select isnull(max(cast(AdjHistoryID as int)),0)+1 FROM  AdjustmentHistorys";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert, "Unable to create new Adjustment No");
                    }
                    #endregion product type new id generation


                    #region Insert

                    sqlText = "";



                    sqlText += "insert into AdjustmentHistorys";
                    sqlText += "(";

                    sqlText += " AdjHistoryID,";
                    sqlText += " AdjId,";
                    sqlText += " AdjDate,";
                    sqlText += " AdjAmount,";
                    sqlText += " AdjVATRate,";
                    sqlText += " AdjVATAmount,";
                    sqlText += " AdjSD,";
                    sqlText += " AdjSDAmount,";
                    sqlText += " AdjOtherAmount,";
                    sqlText += " AdjType,";
                    sqlText += " AdjDescription,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " AdjInputAmount,";
                    sqlText += " AdjInputPercent,";
                    sqlText += " AdjReferance,";
                    sqlText += " Post,";
                    sqlText += " ReverseAdjHistoryNo,";
                    sqlText += " AdjHistoryNo,";
                    sqlText += " BranchId";


                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@adjHistoryAdjId,";
                    sqlText += "@adjHistoryAdjDate,";
                    sqlText += "@adjHistoryAdjAmount,";
                    sqlText += "@adjHistoryAdjVATRate,";
                    sqlText += "@adjHistoryAdjVATAmount,";
                    sqlText += "@adjHistoryAdjSD,";
                    sqlText += "@adjHistoryAdjSDAmount,";
                    sqlText += "@adjHistoryAdjOtherAmount,";
                    sqlText += "@adjHistoryAdjType,";
                    sqlText += "@adjHistoryAdjDescription,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@adjHistoryAdjInputAmount,";
                    sqlText += "@adjHistoryAdjInputPercent,";
                    sqlText += "@adjHistoryAdjReferance,";
                    sqlText += "@MasterPost,";
                    sqlText += "@MasterReturnID,";
                    sqlText += "@newID,";
                    sqlText += "@BranchId";



                    sqlText += ")";

                    SqlCommand cmdInsert1 = new SqlCommand(sqlText, currConn);
                    cmdInsert1.Transaction = transaction;

                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@nextId", nextId);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjAmount", adjHistory.AdjAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjVATRate", adjHistory.AdjVATRate);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjVATAmount", adjHistory.AdjVATAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjSD", adjHistory.AdjSD);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjSDAmount", adjHistory.AdjSDAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjOtherAmount", adjHistory.AdjOtherAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDescription", adjHistory.AdjDescription);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjInputAmount", adjHistory.AdjInputAmount);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjInputPercent", adjHistory.AdjInputPercent);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjReferance", adjHistory.AdjReferance);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@newID", newID);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);


                    transResult = (int)cmdInsert1.ExecuteNonQuery();
                    #endregion Insert
                }

                else if (Master.TransactionType == "VDS"
                    || Master.TransactionType == "VDS-Credit"
                    || Master.TransactionType == "SaleVDS"

                    )
                {
                    #region Validation for Detail

                    if (vds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }


                    #endregion Validation for Detail

                    #region Insert Detail Table

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        if (Item.PurchaseNumber.Trim().ToUpper() != "NA")
                        {
                            sqlText = "";
                            sqlText += "select COUNT(VDSId) from VDS WHERE DepositNumber='" + newID + "' ";
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
                        sqlText += " insert into VDS(";

                        sqlText += " VDSId";
                        sqlText += " ,VendorId";
                        sqlText += " ,BillAmount";
                        sqlText += " ,BillNo";

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

                        sqlText += " )";

                        sqlText += " values(	";

                        sqlText += "'" + newID + "',";
                        sqlText += "@ItemVendorId,";
                        sqlText += "@ItemBillAmount,";
                        sqlText += "@ItemBillNo,";

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
                        sqlText += "@BranchId";


                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVendorId", Item.VendorId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillAmount", Item.BillAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillNo", Item.BillNo);

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

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                            MessageVM.dpMsgSaveNotSuccessfully);
                        }

                        #endregion Insert only DetailTable


                    }


                    #endregion Insert Detail Table

                }





                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.Deposits WHERE DepositId='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch

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
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
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
                if (VcurrConn == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("DepositDAL", "DepositInsertX", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                //////if (vcurrConn == null)
                //////{
                //////    if (VcurrConn != null)
                //////    {
                //////        if (VcurrConn.State == ConnectionState.Open)
                //////        {
                //////            VcurrConn.Close();
                //////        }
                //////    }
                //////}

            }

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public string[] DepositPostX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null)
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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId=@MasterId";
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

                sqlText += " update Deposits set  ";
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
                    if (vds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        sqlText = "";
                        sqlText += "select COUNT(VDSId) from VDS WHERE VDSId=@MasterId ";
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
                            sqlText += " update VDS set ";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " Post=@MasterPost,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn";
                            sqlText += " where  VDSId  =@MasterId ";
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
                sqlText = sqlText + "select distinct Post from Deposits WHERE DepositId=@MasterId ";
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
                retResults[1] = MessageVM.dpMsgPostSuccessfully;
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

                FileLogger.Log("DepositDAL", "DepositPostX", ex.ToString() + "\n" + sqlText);

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

        #endregion UnUsed
    }
}
