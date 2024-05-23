// ---------form //
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
////
using System.Security.Cryptography;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using SymphonySofttech.Utilities;
//
using VATClient.ReportPages;
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Threading;
//
using VATViewModel.DTOs;
using VATServer.Library;
using System.Data.OleDb;
using VATServer.License;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormDeposit : Form
    {
        #region Constructors

        public FormDeposit()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DepositMasterVM depositMaster;
        private AdjustmentHistoryVM adjustmentHistory;
        private List<VDSMasterVM> vdmMaster;

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DataTable VDSResult;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private DataSet ReportResult;
        private DataSet ReportSubResult;
        private string transId = string.Empty;
        string transactionType = string.Empty;
        public string InEnglish = string.Empty;
        List<AdjustmentVM> adjNames = new List<AdjustmentVM>();
        CommonDAL commonDal = new CommonDAL();
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        private int searchBranchId = 0;
        private bool ChangeData = false;
        string NextID = string.Empty;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string company;
        private string DepositData;
        private string VDSData;
        private string BankInformationData;
        private string[] BankInformationLines;
        private string result;
        private string[] DepositResultFields;
        private DataTable BankInformationResult;
        private DataTable AdjTypeResult;
        private decimal VDSPercentRate = 0;
        private bool Add = false;
        private bool Edit = false;
        #endregion


        #region Methods 01

        //public string VFIN = "151";
        private void BankSearch()
        {
            try
            {
                //BankInformationDAL bankInformationDal = new BankInformationDAL();
                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);
                BankInformationResult = bankInformationDal.SelectAll(0, null, null, null, null, true, connVM);

                cmbBankName.Items.Clear();
                foreach (DataRow item in BankInformationResult.Rows)
                {
                    cmbBankName.Items.Add(item[1].ToString());
                }
                cmbBankName.SelectedIndex = 0;

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BankSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BankSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BankSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "BankSearch", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankSearch", exMessage);
            }
            #endregion
        }

        private void BankDetailsInfo()
        {

            try
            {
                DataRow[] dr = BankInformationResult.Select("BankName = '" + cmbBankName.Text.Trim() + "'");

                txtBranchName.Text = dr[0][2].ToString();
                txtBankID.Text = dr[0][0].ToString();
                txtAccountNumber.Text = dr[0][3].ToString();
                txtDistrict.Text = dr[0][7].ToString();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BankDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BankDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BankDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "BankDetailsInfo", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankDetailsInfo", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankDetailsInfo", exMessage);
            }
            #endregion
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                #region Save

                #region Checkpoint

                //if (Add == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotAddAccess, this.Text);
                //    return;
                //}
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (txtTreasuryNo.Text.Trim() == "")
                {
                    txtTreasuryNo.Text = "-";
                }
                if (Program.CheckLicence(dtpDepositDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDepositId.Text.Trim();
                }


                if (txtDepositAmount.Text == "")
                {
                    txtDepositAmount.Text = "0.00";
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                //BankDetailsInfo();

                #region Error Check

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                #endregion

                #region Check Point

                if (rbtnVDS.Checked)
                {
                    if (dgvVDS.Rows.Count <= 0)
                    {
                        MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                        return;
                    }

                }

                #endregion

                #region Value Assign

                #region Deposit
                depositMaster = new DepositMasterVM();
                depositMaster.DepositId = NextID.ToString();
                depositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                depositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankDepositDate = dtpBankDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpBankDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.DepositType = lstDepositType.SelectedValue.ToString();
                depositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                depositMaster.ChequeNo = txtChequeNo.Text.Trim();
                depositMaster.ChequeBank = txtChequeBank.Text.Trim();
                depositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");//dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankId = txtBankID.Text.Trim();
                depositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                depositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                depositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();

                depositMaster.DepositPersonContactNo = txtContactNo.Text.Trim();
                depositMaster.DepositPersonAddress = txtAddress.Text.Trim();

                depositMaster.Comments = txtComments.Text.Trim();
                depositMaster.CreatedBy = Program.CurrentUser;
                depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                depositMaster.LastModifiedBy = Program.CurrentUser;
                depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                if (lstDepositType.SelectedValue.ToString().ToLower() == "opening")
                {
                    depositMaster.TransactionType = transactionType.Trim() + "-Opening";

                }
                else
                {
                    depositMaster.TransactionType = transactionType;
                }

                //depositMaster.TransactionType = Convert.ToString(chkVDS.Checked ? "VDS" : "Treasury");

                depositMaster.Info2 = "Info2";
                depositMaster.Info3 = "Info3";
                depositMaster.Info4 = "Info4";
                depositMaster.Info5 = "Info5";
                depositMaster.Post = "N";
                depositMaster.BranchId = Program.BranchId;
                #endregion Deposit

                #region AdjCashPayble
                if (rbtnAdjCashPayble.Checked)
                {
                    adjustmentHistory = new AdjustmentHistoryVM();
                    adjustmentHistory.AdjId = cmbAdjId.SelectedValue.ToString();
                    adjustmentHistory.AdjDate =
                        dtpAdjDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    adjustmentHistory.AdjInputAmount = Convert.ToDecimal(txtInputAmount.Text.Trim());
                    adjustmentHistory.AdjInputPercent = Convert.ToDecimal(txtInputPercent.Text.Trim());
                    adjustmentHistory.AdjAmount = Convert.ToDecimal(txtAdjAmount.Text.Trim());
                    adjustmentHistory.AdjVATRate = 0;
                    adjustmentHistory.AdjVATAmount = 0;
                    adjustmentHistory.AdjSD = 0;
                    adjustmentHistory.AdjSDAmount = 0;
                    adjustmentHistory.AdjOtherAmount = 0;
                    adjustmentHistory.AdjType = cmbAdjType.SelectedValue.ToString();
                    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                    adjustmentHistory.BranchId = Program.BranchId;

                }

                #endregion AdjCashPayble

                #region VDS
                if (rbtnVDS.Checked)
                {
                    vdmMaster = new List<VDSMasterVM>();

                    for (int i = 0; i < dgvVDS.RowCount; i++)
                    {

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        vdsDetail.Id = NextID.ToString();
                        vdsDetail.VendorId = dgvVDS.Rows[i].Cells["VendorId"].Value.ToString();
                        vdsDetail.BillAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["BillAmount"].Value.ToString());
                        vdsDetail.BillNo = dgvVDS.Rows[i].Cells["BillNo"].Value.ToString();

                        vdsDetail.BillDate =
                              Convert.ToDateTime(dgvVDS.Rows[i].Cells["PurchaseDate"].Value.ToString()).ToString(
                                    "yyyy-MMM-dd HH:mm:ss");
                        vdsDetail.BillDeductedAmount =
                            Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSAmount"].Value.ToString());
                        vdsDetail.Remarks = dgvVDS.Rows[i].Cells["Remarks"].Value.ToString();
                        vdsDetail.IssueDate =

                               Convert.ToDateTime(dgvVDS.Rows[i].Cells["IssueDate"].Value.ToString()).ToString(
                                    "yyyy-MMM-dd HH:mm:ss");

                        vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                        vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                        vdsDetail.IsPercent = dgvVDS.Rows[i].Cells["IsPercent"].Value.ToString();
                        vdsDetail.IsPurchase = dgvVDS.Rows[i].Cells["IsPurchase"].Value.ToString();
                        vdsDetail.VATAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VATAmount"].Value);
                        vdsDetail.BranchId = Program.BranchId;
                        vdmMaster.Add(vdsDetail);

                    }


                    //if (rbtnVDS.Checked)
                    //{
                    if (vdmMaster.Count <= 0)
                    {
                        MessageBox.Show("Please insert Details information for transaction", this.Text,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;

                    }
                    //}
                }

                #endregion VDS

                #endregion

                #endregion Save

                #region Background Workder

                bgwSave.RunWorkerAsync();

                #endregion

                #region Button Stats

                this.btnAdd.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion

        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement


                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DepositDAL depositDal = new DepositDAL();

                IDeposit depositDal = OrdinaryVATDesktop.GetObject<DepositDAL, DepositRepo, IDeposit>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.DepositInsert(depositMaster, vdmMaster, adjustmentHistory, null, null, connVM); // Change 04

                if (sqlResults[0].ToLower() == "success")
                    depositDal.UpdateVdsComplete("Y", sqlResults[2], connVM, depositMaster.TransactionType);

                SAVE_DOWORK_SUCCESS = true;

                // End DoWork

                #endregion
            }

            #endregion

            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                #region Success

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDepositId.Text = sqlResults[2].ToString();
                            txtDepositId1.Text = sqlResults[2].ToString();
                            txtDepositId2.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }
                ChangeData = false;
                #endregion Success
            }

            #endregion

            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
            }
            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                #region Check Point

                #region Comment before 28 Oct 2020

                //////if (Edit == false)
                //////{
                //////    MessageBox.Show(MessageVM.msgNotEditAccess, this.Text);
                //////    return;
                //////}

                #endregion

                #region Comment 28 Oct 2020

                ////if (IsPost == true)
                ////{
                ////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                ////    return;
                ////}

                #endregion

                if (txtTreasuryNo.Text.Trim() == "")
                {
                    txtTreasuryNo.Text = "-";
                }
                if (Program.CheckLicence(dtpDepositDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    NextID = txtDepositId.Text.Trim();
                }


                if (txtDepositAmount.Text == "")
                {
                    txtDepositAmount.Text = "0.00";
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Error Return

                //BankDetailsInfo();

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                #endregion

                #region Details check

                if (rbtnVDS.Checked)
                {
                    if (dgvVDS.Rows.Count <= 0)
                    {
                        MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                        return;
                    }

                }

                #endregion

                #region Master Value Assign

                depositMaster = new DepositMasterVM();
                depositMaster.DepositId = NextID.ToString();
                depositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                depositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankDepositDate = dtpBankDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                depositMaster.DepositType = lstDepositType.SelectedValue.ToString();
                depositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                depositMaster.DepositPersonContactNo = txtContactNo.Text.Trim();
                depositMaster.DepositPersonAddress = txtAddress.Text.Trim();
                depositMaster.ChequeNo = txtChequeNo.Text.Trim();
                depositMaster.ChequeBank = txtChequeBank.Text.Trim();
                depositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankId = txtBankID.Text.Trim();
                depositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                depositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                depositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();
                depositMaster.Comments = txtComments.Text.Trim();
                depositMaster.CreatedBy = Program.CurrentUser;
                depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                depositMaster.LastModifiedBy = Program.CurrentUser;
                depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                if (lstDepositType.SelectedValue.ToString().ToLower() == "opening")
                {
                    depositMaster.TransactionType = transactionType.Trim() + "-Opening";

                }
                else
                {
                    depositMaster.TransactionType = transactionType.Trim();
                }



                //depositMaster.TransactionType = Convert.ToString(chkVDS.Checked ? "VDS" : "Treasury");
                depositMaster.Info2 = "Info2";
                depositMaster.Info3 = "Info3";
                depositMaster.Info4 = "Info4";
                depositMaster.Info5 = "Info5";
                depositMaster.Post = "N";
                depositMaster.BranchId = Program.BranchId;

                #endregion Master Value Assign

                #region AdjCashPayble
                if (rbtnAdjCashPayble.Checked)
                {
                    adjustmentHistory = new AdjustmentHistoryVM();
                    adjustmentHistory.AdjId = cmbAdjId.SelectedValue.ToString();
                    adjustmentHistory.AdjDate =
                        dtpAdjDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    adjustmentHistory.AdjInputAmount = Convert.ToDecimal(txtInputAmount.Text.Trim());
                    adjustmentHistory.AdjInputPercent = Convert.ToDecimal(txtInputPercent.Text.Trim());
                    adjustmentHistory.AdjAmount = Convert.ToDecimal(txtAdjAmount.Text.Trim());
                    adjustmentHistory.AdjVATRate = 0;
                    adjustmentHistory.AdjVATAmount = 0;
                    adjustmentHistory.AdjSD = 0;
                    adjustmentHistory.AdjSDAmount = 0;
                    adjustmentHistory.AdjOtherAmount = 0;
                    adjustmentHistory.AdjType = cmbAdjType.SelectedValue.ToString();
                    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                }

                #endregion AdjCashPayble

                #region Detail Value Assign

                if (rbtnVDS.Checked)
                {
                    vdmMaster = new List<VDSMasterVM>();

                    for (int i = 0; i < dgvVDS.RowCount; i++)
                    {

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        vdsDetail.Id = NextID.ToString();
                        vdsDetail.VendorId = dgvVDS.Rows[i].Cells["VendorId"].Value.ToString();
                        vdsDetail.BillAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["BillAmount"].Value.ToString());
                        vdsDetail.BillNo = dgvVDS.Rows[i].Cells["BillNo"].Value.ToString();

                        //format test
                        vdsDetail.BillDate = Convert.ToDateTime(dgvVDS.Rows[i].Cells["PurchaseDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                        vdsDetail.BillDeductedAmount =
                            Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSAmount"].Value.ToString());
                        vdsDetail.Remarks = dgvVDS.Rows[i].Cells["Remarks"].Value.ToString();
                        vdsDetail.IssueDate =
                                Convert.ToDateTime(dgvVDS.Rows[i].Cells["IssueDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");

                        vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                        vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                        vdsDetail.IsPercent = dgvVDS.Rows[i].Cells["IsPercent"].Value.ToString();
                        vdsDetail.IsPurchase = dgvVDS.Rows[i].Cells["IsPurchase"].Value.ToString();
                        vdsDetail.BranchId = Program.BranchId;
                        vdmMaster.Add(vdsDetail);

                    }


                    if (rbtnVDS.Checked)
                    {
                        if (vdmMaster.Count <= 0)
                        {
                            MessageBox.Show("Please insert Details information for transaction", this.Text,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                            return;

                        }
                    }
                }

                #endregion

                #region Background Worker Update

                bgwUpdate.RunWorkerAsync();

                #endregion

                #region Button Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion

        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DepositDAL depositDal = new DepositDAL();

                IDeposit depositDal = OrdinaryVATDesktop.GetObject<DepositDAL, DepositRepo, IDeposit>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.DepositUpdate(depositMaster, vdmMaster, adjustmentHistory, connVM); // Change 04

                if (sqlResults[0].ToLower() == "success")
                    depositDal.UpdateVdsComplete("Y", sqlResults[2], connVM, depositMaster.TransactionType);

                UPDATE_DOWORK_SUCCESS = true;

                // End DoWork

                #endregion

            }
            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDepositId.Text = sqlResults[2].ToString();
                            txtDepositId1.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }
                ChangeData = false;
                #endregion

            }
            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        public int ErrorReturn()
        {
            if (chkPurchaseVDS.Checked == false)
            {
                lstDepositType.SelectedValue = "Cash";
            }
            if (lstDepositType.SelectedValue.ToString().ToLower() == "select")
            {
                MessageBox.Show("Please select Deposit Type.", this.Text);
                lstDepositType.Focus();
                return 1;
            }
            if (lstDepositType.SelectedValue.ToString().ToLower() == "opening")
            {
                txtBankID.Text = "0";
            }


            if (txtTreasuryCopy.Text == "")
            {
                txtTreasuryCopy.Text = "-";
            }
            if (txtChequeNo.Text == "")
            {
                txtChequeNo.Text = "-";
            }
            if (txtChequeBank.Text == "")
            {
                if (lstDepositType.SelectedValue.ToString().ToLower() == "cheque")
                {

                    MessageBox.Show("Please enter Deposit Cheque Bank Name!", this.Text);
                    txtChequeBank.Focus();
                    return 1;
                }
                else
                {
                    txtChequeBank.Text = "-";

                }
            }
            if (txtChequeBankBranch.Text == "")
            {
                if (lstDepositType.SelectedValue.ToString().ToLower() == "cheque")
                {

                    MessageBox.Show("Please enter Deposit Cheque Bank Branch Name!", this.Text);
                    txtChequeBankBranch.Focus();
                    return 1;
                }
                else
                {
                    txtChequeBankBranch.Text = "-";

                }
            }
            if (lstDepositType.SelectedValue.ToString() == "Cash" || lstDepositType.SelectedValue.ToString().ToLower() == "cheque")
            {
                if (lstDepositType.SelectedValue.ToString() != "Cash")
                {
                    if (txtBankID.Text.Trim() == "")
                    {
                        if (transactionType != "SaleVDS")
                        {
                            MessageBox.Show("Please enter Deposit Bank Name.", this.Text);
                            cmbBankName.Focus();
                            //txtBankID.Focus();
                            return 1;
                        }

                    }
                }




                if (txtDepositPerson.Text == "" || txtDepositPerson.Text == "-")
                {
                    if (transactionType != "SaleVDS")
                    {
                        MessageBox.Show("Please enter Deposit Person Name!", this.Text);
                        txtDepositPerson.Focus();
                        return 1;
                    }
                }

                if (txtDepositPersonDesignation.Text == "")
                {
                    if (transactionType != "SaleVDS")
                    {
                        MessageBox.Show("Please enter Deposit Person Designation!", this.Text);
                        txtDepositPersonDesignation.Focus();
                        return 1;
                    }

                }

                if (txtContactNo.Text == "")
                {
                    if (transactionType != "SaleVDS")
                    {
                        MessageBox.Show("Please enter Deposit Person ContactNo!", this.Text);
                        txtContactNo.Focus();
                        return 1;
                    }
                }
                if (txtAddress.Text == "")
                {
                    if (transactionType != "SaleVDS")
                    {
                        MessageBox.Show("Please enter Deposit Person Address!", this.Text);
                        txtAddress.Focus();
                        return 1;
                    }
                }

                if (txtComments.Text == "")
                {

                    if (transactionType != "SaleVDS")
                    {
                        MessageBox.Show("Please enter  Remarks !", this.Text);
                        txtComments.Focus();
                        return 1;
                    }
                }


            }


            return 0;
        }

        private void ClearAll()
        {
            cmbBankName.Text = "";
            txtDepositId.Text = "";
            txtDepositId1.Text = "";
            txtTreasuryNo.Text = "";
            //txtDepositType.Text = "";
            txtDepositAmount.Text = "";
            txtChequeNo.Text = "";
            txtChequeBank.Text = "";
            txtChequeBankBranch.Text = "";
            txtBankID.Text = "";
            txtBankName.Text = "";
            txtBranchName.Text = "";
            txtAccountNumber.Text = "";
            txtTreasuryCopy.Text = "";
            txtDepositPerson.Text = "";
            txtDepositPersonDesignation.Text = "";
            txtContactNo.Text = "";
            txtAddress.Text = "";
            txtComments.Text = "";
            dtpChequeDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dtpDepositDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            //VDS Info:
            txtPurchaseNumber.Text = "";
            txtVendorName.Text = "";
            txtVDSComments.Text = "";
            txtBillAmount.Text = "";
            txtBillNo.Text = "";
            txtVDSPercent.Text = "";
            txtVDSAmount.Text = "";
            dtpPurchaseDate.Value = Program.SessionDate;
            dtpIssueDate.Value = Program.SessionDate;
            dgvVDS.Rows.Clear();
            txtBillTotal.Text = "";
            txtVDSTotal.Text = "";
            txtAdjHistoryID.Text = string.Empty;
            txtAdjAmount.Text = "0";
            txtAdjDescription.Text = string.Empty;
            dtpAdjDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            if (cmbAdjType.SelectedIndex != -1)
            {
                cmbAdjType.SelectedIndex = 0;
            }

            if (lstDepositType.SelectedIndex != -1)
            {
                lstDepositType.SelectedIndex = 0;
            }


            txtDepositId2.Text = string.Empty;
            txtInputAmount.Text = "0";
            txtInputPercent.Text = "0";
            txtAdjReferance.Text = string.Empty;
            IsPost = false;
            txtDistrict.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picBankID_Click(object sender, EventArgs e)
        {

        }

        private void picDepositID_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 02

        #region Textbox KeyDown Event

        private void txtDepositId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTreasuryNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        //private void txtDepositType_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        //}

        private void txtDepositAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDepositDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpChequeDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeBank_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeBankBranch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBankID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                BankLoad();
            }
        }

        private void txtBranchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTreasuryCopy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtDepositPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                UserLoad();
            }
        }

        private void txtDepositPersonDesignation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { btnAdd.Focus(); }
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {

        }

        #endregion

        private void txtDepositAmount_Leave(object sender, EventArgs e)
        {
            txtDepositAmount.Text = Program.ParseDecimalObject(txtDepositAmount.Text.Trim()).ToString();

            //Program.FormatTextBox(txtDepositAmount, "Deposit Amount");
        }

        private void txtDepositAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                TransactionTypes();
                FormRptDepositTransaction frmRptDepositInformation = new FormRptDepositTransaction();

                if (txtDepositId.Text == "~~~ New ~~~")
                {
                    frmRptDepositInformation.txtDepositNo.Text = string.Empty;

                }
                else
                {
                    frmRptDepositInformation.txtDepositNo.Text = txtDepositId.Text.Trim();

                }

                frmRptDepositInformation.txtTransactionType.Text = transactionType;
                frmRptDepositInformation.Show();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "Search", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion
        }

        private void cmbBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BankDetailsInfo();
            ChangeData = true;
        }

        private void btnPrintGrid_Click(object sender, EventArgs e)
        {
            ReportShow();
        }

        private void ReportShow()
        {
            try
            {
                string ReportData =

                    FieldDelimeter +
                    FieldDelimeter +
                    FieldDelimeter +


                    //dtpDepositToDate.Value + FieldDelimeter +
                    txtChequeNo.Text.Trim() + FieldDelimeter +
                    txtBankName.Text.Trim() + FieldDelimeter +

                    LineDelimeter;

                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);

                //ReportSoapClient ShowReport = new ReportSoapClient();

                //string ReportResult = ShowReport.Deposit(encriptedData.ToString());

                //string decriptedSData = Converter.DESDecrypt(PassPhrase, EnKey, ReportResult);

                //string[] ReportLines = decriptedSData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //TextWriter Report1 = new StreamWriter(Program.TempFilePath + @"\TreasuryDepositTransaction.txt");
                //Report1.WriteLine(@"DepositId,TreasuryChallanNo,DepositType,DepositDate,DepositAmount,ChequeNo,ChequeDate,BankName,DepositPerson");


                //for (int j = 0; j < Convert.ToInt32(ReportLines.Length); j++)
                //{
                //    string[] ReportFields = ReportLines[j].Split(FieldDelimeter.ToCharArray());

                //    try
                //    {
                //        string txtBank =
                //            ReportFields[1].ToString() + "," +
                //            ReportFields[2].ToString() + "," +
                //            ReportFields[3].ToString() + "," +
                //            ReportFields[4].ToString() + "," +
                //            ReportFields[5].ToString() + "," +
                //            ReportFields[6].ToString() + "," +
                //            ReportFields[7].ToString() + "," +
                //            ReportFields[8].ToString() + "," +
                //            ReportFields[9].ToString();

                //        Report1.WriteLine(txtBank);
                //    }
                //    catch (Exception)
                //    {
                //        MessageBox.Show("Error occurred.", "Deposit Report");
                //        return;
                //    }
                //}

                //Report1.Close();

                Thread.Sleep(1000);
                FormReport Reports = new FormReport(); //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'"; objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                Reports.crystalReportViewer1.Refresh();
                Reports.crystalReportViewer1.ReportSource = @"c:\report\DepositGrid.rpt";
                ParameterField CompanyName1 = new ParameterField();
                CompanyName1.Name = "CompanyName";
                ParameterDiscreteValue CompanyNameValue = new ParameterDiscreteValue(); // CrystalDecisions.Shared.
                CompanyNameValue.Value = DBConstant.CompanyName;
                CompanyName1.CurrentValues.Add(CompanyNameValue);
                Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyName1);

                ParameterField CompanyAddress1 = new ParameterField();
                CompanyAddress1.Name = "CompanyAddress";
                ParameterDiscreteValue CompanyAddressValue = new ParameterDiscreteValue(); // CrystalDecisions.Shared.
                CompanyAddressValue.Value = DBConstant.CompanyAddress;
                CompanyAddress1.CurrentValues.Add(CompanyAddressValue);
                Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyAddress1);

                ParameterField CompanyNumber1 = new ParameterField();
                CompanyNumber1.Name = "CompanyNumber";
                ParameterDiscreteValue CompanyNumberValue = new ParameterDiscreteValue(); // CrystalDecisions.Shared.
                CompanyNumberValue.Value = DBConstant.CompanyContactNumber;
                CompanyNumber1.CurrentValues.Add(CompanyNumberValue);
                Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyNumber1);
                Reports.Text = "Deposit";
                Reports.Show();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ReportShow", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", exMessage);
            }
            #endregion
        }

        private void searchExist()
        {

            //DepositDAL IsDal = new DepositDAL();
            IDeposit DepositDal = OrdinaryVATDesktop.GetObject<DepositDAL, DepositRepo, IDeposit>(OrdinaryVATDesktop.IsWCF);
            DataTable dataTable = new DataTable("SearchDepositHeader");

            try
            {
                TransactionTypes();
                if (rbtnAdjCashPayble.Checked)
                    transactionType = "AdjCashPayble";
                transId = string.Empty;
                Program.fromOpen = "Me";
                DataGridViewRow selectedRow = FormDepositSearch.SelectOne(transactionType);

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    string[] cValues = { selectedRow.Cells["DepositId"].Value.ToString() };
                    string[] cFields = { "d.DepositId like" };
                    dataTable = DepositDal.SelectAll(0, cFields, cValues, null, null, true, connVM);

                    txtDepositId.Text = dataTable.Rows[0]["DepositId"].ToString();
                    txtDepositId1.Text = dataTable.Rows[0]["DepositId"].ToString();
                    txtDepositId2.Text = dataTable.Rows[0]["DepositId"].ToString();
                    transId = dataTable.Rows[0]["DepositId"].ToString();
                    txtTreasuryNo.Text = dataTable.Rows[0]["TreasuryNo"].ToString();
                    dtpDepositDate.Value = Convert.ToDateTime(dataTable.Rows[0]["DepositDateTime"].ToString());
                    dtpBankDepositDate.Value = Convert.ToDateTime(dataTable.Rows[0]["BankDepositDate"].ToString());
                    lstDepositType.SelectedValue = dataTable.Rows[0]["DepositType"].ToString();
                    txtDepositAmount.Text = Program.ParseDecimalObject(dataTable.Rows[0]["DepositAmount"].ToString());
                    txtChequeNo.Text = dataTable.Rows[0]["ChequeNo"].ToString();
                    txtChequeBank.Text = dataTable.Rows[0]["ChequeBank"].ToString();
                    txtChequeBankBranch.Text = dataTable.Rows[0]["ChequeBankBranch"].ToString();
                    dtpChequeDate.Value = Convert.ToDateTime(dataTable.Rows[0]["ChequeDate"].ToString());
                    txtBankID.Text = dataTable.Rows[0]["BankID"].ToString();
                    txtBankName.Text = dataTable.Rows[0]["BankName"].ToString();
                    cmbBankName.Text = dataTable.Rows[0]["BankName"].ToString();

                    txtBranchName.Text = dataTable.Rows[0]["BranchName"].ToString();
                    txtAccountNumber.Text = dataTable.Rows[0]["AccountNumber"].ToString();
                    txtDistrict.Text = dataTable.Rows[0]["City"].ToString();
                    txtDepositPerson.Text = dataTable.Rows[0]["DepositPerson"].ToString();
                    txtDepositPersonDesignation.Text = dataTable.Rows[0]["DepositPersonDesignation"].ToString();
                    txtContactNo.Text = dataTable.Rows[0]["DepositPersonContactNo"].ToString();
                    txtAddress.Text = dataTable.Rows[0]["DepositPersonAddress"].ToString();
                    txtComments.Text = dataTable.Rows[0]["Comments"].ToString();
                    IsPost = Convert.ToString(dataTable.Rows[0]["Post"].ToString()) == "Y" ? true : false;
                    searchBranchId = Convert.ToInt32(dataTable.Rows[0]["BranchId"].ToString());
                    if (rbtnVDS.Checked)
                    {
                        if (txtDepositId.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwVDSSearch.RunWorkerAsync();

                        }
                        else if (txtDepositId1.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwVDSSearch.RunWorkerAsync();
                        }
                    }
                    else if (rbtnAdjCashPayble.Checked)
                    {
                        if (txtDepositId.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwACPSearch.RunWorkerAsync();

                        }
                        else if (txtDepositId2.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwACPSearch.RunWorkerAsync();
                        }
                    }


                    //btnAdd.Text = "&Save";
                    IsUpdate = true;



                    ChangeData = false;
                    btnUpdateSeven.Visible = false;


                }



            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchDepositNo_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void btnSearchDepositNo_Click(object sender, EventArgs e)
        {
            searchExist();

        }

        private void bgwVDSSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                VDSResult = new DataTable();
                //DepositDAL depositDal = new DepositDAL();
                IDeposit depositDal = OrdinaryVATDesktop.GetObject<DepositDAL, DepositRepo, IDeposit>(OrdinaryVATDesktop.IsWCF);
                VDSResult = depositDal.SearchVDSDT(txtDepositId.Text, connVM, chkPurchaseVDS.Checked);


                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwVDSSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                //dgvVDS.DataSource = null;
                //if (VDSResult != null)
                //{
                //    dgvVDS.DataSource = VDSResult;
                //}
                dgvVDS.Rows.Clear();
                int j = 0;
                foreach (DataRow item in VDSResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvVDS.Rows.Add(NewRow);
                    dgvVDS.Rows[j].Cells["LineNo"].Value = item["VDSId"].ToString();
                    dgvVDS.Rows[j].Cells["VendorId"].Value = item["VendorId"].ToString();
                    dgvVDS.Rows[j].Cells["VendorName"].Value = item["VendorName"].ToString();
                    dgvVDS.Rows[j].Cells["BillAmount"].Value = Program.ParseDecimalObject(item["BillAmount"].ToString());
                    dgvVDS.Rows[j].Cells["BillNo"].Value = item["BillNo"].ToString();
                    dgvVDS.Rows[j].Cells["VDSPercent"].Value = Program.ParseDecimalObject(item["VDSPercent"].ToString());
                    dgvVDS.Rows[j].Cells["VDSAmount"].Value = Program.ParseDecimalObject(item["VDSAmount"].ToString());
                    dgvVDS.Rows[j].Cells["PurchaseDate"].Value = item["PurchaseDate"].ToString();
                    dgvVDS.Rows[j].Cells["IssueDate"].Value = item["IssueDate"].ToString();
                    dgvVDS.Rows[j].Cells["Remarks"].Value = item["Remarks"].ToString();
                    dgvVDS.Rows[j].Cells["PurchaseNumber"].Value = item["PurchaseNumber"].ToString();
                    dgvVDS.Rows[j].Cells["IsPercent"].Value = item["IsPercent"].ToString();
                    dgvVDS.Rows[j].Cells["IsPurchase"].Value = item["IsPurchase"].ToString();
                    dgvVDS.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                    j = j + 1;
                }

                Rowcalculate();

                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVDSSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnSearchDepositNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnSearchBankName_Click(object sender, EventArgs e)
        {

            try
            {
                Program.fromOpen = "Me";
                string result = FormBankInformationSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] BankInformationinfo = result.Split(FieldDelimeter.ToCharArray());
                    txtBankID.Text = BankInformationinfo[0];
                    cmbBankName.Text = BankInformationinfo[1];
                    txtBankName.Text = BankInformationinfo[1];
                    txtBranchName.Text = BankInformationinfo[2];
                    txtAccountNumber.Text = BankInformationinfo[3];
                }
                // BankSearch();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchBankName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchBankName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchBankName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearchBankName_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchBankName_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchBankName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchBankName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchBankName_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void txtTreasuryNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        //private void txtDepositType_TextChanged(object sender, EventArgs e)
        //{
        //    ChangeData = true;
        //}

        private void dtpDepositDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtBranchName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 03

        private void txtAccountNumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTreasuryCopy_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDepositPersonDesignation_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDepositPerson_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtChequeBankBranch_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtChequeBank_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void dtpChequeDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtChequeNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void TransactionTypes()
        {
            #region Transaction Type

            transactionType = string.Empty;

            if (rbtnTreasury.Checked)
            {
                transactionType = "Treasury";
            }


            else if (rbtnVDS.Checked)
            {
                if (chkPurchaseVDS.Checked)
                {
                    transactionType = "VDS";
                }
                else
                {
                    transactionType = "SaleVDS";
                }
            }
            else if (rbtnAdjCashPayble.Checked)
            {
                btn6_6.Visible = false;
                transactionType = cmbAdjType.SelectedValue.ToString();
            }
            else if (rbtnSD.Checked)
            {
                transactionType = "SD";
            }
            #endregion Transaction Type
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (rbtnAdjCashPayble.Checked)
                {
                    IAdjustment adjustmentDal = OrdinaryVATDesktop.GetObject<AdjustmentDAL, AdjustmentRepo, IAdjustment>(OrdinaryVATDesktop.IsWCF);
                    string[] cFields = new string[] { "ActiveStatus like" };
                    string[] cValues = new string[] { "Y" };
                    AdjTypeResult = adjustmentDal.SelectAll(null, cFields, cValues, null, null, false, connVM);

                }

                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);
                BankInformationResult = bankInformationDal.SelectAll(0, null, null, null, null, true, connVM);

            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (rbtnAdjCashPayble.Checked)
                {
                    #region AdjustmentHead

                    cmbAdjId.Items.Clear();
                    foreach (DataRow item2 in AdjTypeResult.Rows)
                    {
                        var pType = new AdjustmentVM();
                        pType.AdjId = item2["AdjId"].ToString();
                        pType.AdjName = item2["AdjName"].ToString();
                        pType.Comments = item2["Comments"].ToString();
                        pType.ActiveStatus = item2["ActiveStatus"].ToString();
                        adjNames.Add(pType);
                    }
                    if (AdjTypeResult == null)
                    {
                        MessageBox.Show("There is no Adjustment Head", this.Text);
                        return;
                    }
                    else if (AdjTypeResult.Rows.Count <= 0)
                    {
                        MessageBox.Show("There is no Adjustment Head", this.Text);
                        return;
                    }
                    cmbAdjId.DataSource = AdjTypeResult;
                    cmbAdjId.ValueMember = "AdjId";
                    cmbAdjId.DisplayMember = "AdjName";
                    cmbAdjId.SelectedIndex = 0;

                    #endregion AdjustmentHead
                }

                #region Bank DropDown

                cmbBankName.Items.Clear();

                foreach (DataRow item in BankInformationResult.Rows)
                {
                    cmbBankName.Items.Add(item[1].ToString());
                }

                cmbBankName.SelectedIndex = 0;

                #endregion

                if (cmbBankName.Items.Count <= 0)
                {
                    MessageBox.Show("Please input Bank first", this.Text);
                    return;
                }

            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                ChangeData = false;
            }

        }

        private void FormDeposit_Load(object sender, EventArgs e)
        {
            try
            {
                #region Tool Tip

                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchBankName, "Bank");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchDepositNo, "Existing information");

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Reset Fields / Elements

                ClearAll();

                txtDepositId.Text = "~~~ New ~~~";
                txtDepositId1.Text = "~~~ New ~~~";
                txtDepositId2.Text = "~~~ New ~~~";

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                #endregion

                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion

                #region Settings Data

                company = commonDal.settingsDesktop("Reports", "TR6", null, connVM);

                #endregion

                #region Flag Update

                ChangeData = false;

                #endregion

            }
            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_Load", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void UserLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();
                string SqlText = @" select UserID,UserName,FullName,Designation,ContactNo,Address from UserInformations
            where 1=1 and ActiveStatus='Y' ";

                string SQLTextRecordCount = @" select count(UserID)RecordNo from UserInformations";
                string[] shortColumnName = { "UserID", "UserName", "FullName", "Designation", "ContactNo", "Address" };
                string tableName = "";
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtDepositPerson.Text = selectedRow.Cells["FullName"].Value.ToString();
                    txtDepositPersonDesignation.Text = selectedRow.Cells["Designation"].Value.ToString();
                    txtContactNo.Text = selectedRow.Cells["ContactNo"].Value.ToString();
                    txtAddress.Text = selectedRow.Cells["Address"].Value.ToString();
                    //dtpInvoiceDate.Focus();

                    //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UserLoad", exMessage);
            }
        }

        private void BankLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();
                string SqlText = @" select BankID,BankName,BranchName,AccountNumber,Address1,City from BankInformations
            where 1=1 and ActiveStatus='Y' ";

                string SQLTextRecordCount = @" select count(BankID)RecordNo from BankInformations";
                string[] shortColumnName = { "BankID", "BankName", "BranchName", "AccountNumber", "Address1", "City" };
                string tableName = "";
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtBankID.Text = selectedRow.Cells["BankID"].Value.ToString();
                    txtBankName.Text = selectedRow.Cells["BankName"].Value.ToString();
                    txtBranchName.Text = selectedRow.Cells["BranchName"].Value.ToString();
                    txtAccountNumber.Text = selectedRow.Cells["AccountNumber"].Value.ToString();
                    txtDistrict.Text = selectedRow.Cells["City"].Value.ToString();
                    //dtpInvoiceDate.Focus();

                    //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankLoad", exMessage);
            }
        }

        #endregion

        #region Methods 04

        private void FormMaker()
        {
            #region Button Import Integration Lisence
            if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
            {
                btnImport.Visible = false;
            }
            #endregion
            txtDepositAmount.ReadOnly = false;

            btnAddNew.Visible = true;
            btnAddNewVDS.Visible = false;
            btnUpdateSeven.Visible = false;

            if (rbtnVDS.Checked)
            {
                txtDepositAmount.ReadOnly = true;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage2);
                tabDeposit.TabPages.Add(tabPage1);
                this.Text = "VDS Deposit Entry";
                btnAddNew.Visible = false;
                btnAddNewVDS.Visible = true;
                dtpPurchaseDate.Enabled = true;
                if (chkPurchaseVDS.Checked)
                {

                }
                else
                {
                    btn6_6.Visible = false;
                    chkSingleTR6.Visible = false;
                    btnPrintTR.Visible = false;
                    groupBox3.Visible = false;
                    label14.Text = "VDS Certificate No";
                    label38.Text = "VDS Certificate Date";
                    label6.Text = "Deposite Account Code";
                    label9.Text = "Tax Deposit Date";
                    label7.Text = "Tax Deposite Serial No";
                    label20.Text = "Sale Date";
                }

                DepositTypeLoad("");

            }
            else if (rbtnTreasury.Checked)
            {
                btnPrintVDS.Visible = false;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage1);

                DepositTypeLoad("Treasury");


                ////lstDepositType.Items.Insert(1, "Opening");
                ////lstDepositType.Items.Insert(2, "OpeningAdjustment");
                ////lstDepositType.Items.Insert(3, "ClosingBalanceVAT(18.6)");
                ////lstDepositType.Items.Insert(4, "RequestedAmountForRefundVAT");

                this.Text = "Deposit Entry";


            }
            else if (rbtnSD.Checked)
            {
                btnPrintVDS.Visible = false;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage1);

                ////lstDepositType.Items.Insert(1, "Opening");
                ////lstDepositType.Items.Insert(2, "ClosingBalanceSD(18.6)");
                ////lstDepositType.Items.Insert(3, "RequestedAmountForRefundSD");

                DepositTypeLoad("SD");

                this.Text = "SD Deposit Entry";


            }
            else if (rbtnAdjCashPayble.Checked)
            {
                btnPrintVDS.Visible = false;
                txtDepositAmount.ReadOnly = true;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage3);
                tabDeposit.TabPages.Add(tabPage1);
                btnAddNew.Visible = false;
                btnAddNewVDS.Visible = false;

                DepositTypeLoad("");
                AdjustmentTypeLoad();

                this.Text = "Cash Payable Adjustment Entry";


            }


            ////lstDepositType.Items.Insert(0, "Select");

            ////if (lstDepositType.SelectedIndex != -1)
            ////{
            ////    lstDepositType.SelectedIndex = 0;
            ////}


        }

        private void DepositTypeLoad(string TransactionType)
        {
            CommonDAL commonDal = new CommonDAL();
            Dictionary<string, string> dicDepositType = new Dictionary<string, string>();
            lstDepositType.DataSource = null;
            lstDepositType.Items.Clear();

            dicDepositType = commonDal.DepositType(TransactionType);

            if (dicDepositType != null && dicDepositType.Count > 0)
            {
                lstDepositType.DataSource = new BindingSource(dicDepositType, null);
                lstDepositType.DisplayMember = "Key";
                lstDepositType.ValueMember = "Value";
                lstDepositType.SelectedIndex = 0;

            }

        }

        private void AdjustmentTypeLoad()
        {
            CommonDAL commonDal = new CommonDAL();
            Dictionary<string, string> dicAdjustmentTypeLoad = new Dictionary<string, string>();
            cmbAdjType.DataSource = null;
            cmbAdjType.Items.Clear();

            dicAdjustmentTypeLoad = commonDal.AdjustmentType();

            if (dicAdjustmentTypeLoad != null && dicAdjustmentTypeLoad.Count > 0)
            {
                cmbAdjType.DataSource = new BindingSource(dicAdjustmentTypeLoad, null);
                cmbAdjType.DisplayMember = "Key";
                cmbAdjType.ValueMember = "Value";
                cmbAdjType.SelectedIndex = 0;


            }

        }

        private void FormDeposit_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                if (ChangeData == true)
                {
                    if (DialogResult.Yes != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to close without saving?",
                        this.Text,

                        MessageBoxButtons.YesNo,

                        MessageBoxIcon.Question,

                        MessageBoxDefaultButton.Button2))
                    {
                        e.Cancel = true;
                    }

                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            #endregion
        }

        private void cmbBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void addNew()
        {
            try
            {

                IsPost = false;
                if (rbtnAdjCashPayble.Checked)
                {
                    cmbAdjId.SelectedIndex = 0;
                }
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + "Want to add new without saving?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2))
                    {
                        BankSearch();
                        ClearAll();
                        //btnAdd.Text = "&Add";
                        txtDepositId.Text = "~~~ New ~~~";
                        txtDepositId1.Text = "~~~ New ~~~";
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {
                    BankSearch();
                    ClearAll();
                    //btnAdd.Text = "&Add";
                    txtDepositId.Text = "~~~ New ~~~";
                    txtDepositId1.Text = "~~~ New ~~~";
                    ChangeData = false;
                }
                IsUpdate = false;
                btnUpdateSeven.Visible = false;
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            #endregion
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            addNew();
        }

        private void btnVAT18_Click(object sender, EventArgs e)
        {
            try
            {

                if (Program.CheckLicence(dtpDepositDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT18 frmRptVAT18 = new FormRptVAT18();


                //mdi.RollDetailsInfo("8401");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                frmRptVAT18.dtpFromDate.Value = dtpDepositDate.Value;
                frmRptVAT18.dtpToDate.Value = dtpDepositDate.Value;

                frmRptVAT18.ShowDialog();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }
            #endregion
        }

        private void btnPost_Click(object sender, EventArgs e)
        {

            #region try

            try
            {

                #region Post Message

                if (MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                #endregion

                #region Comment 28 Oct 2020

                ////if (IsPost == true)
                ////{
                ////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                ////    return;
                ////}

                #endregion Comment 28 Oct 2020

                #region Check Point

                if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpDepositDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Check Point

                if (transactionType != "SaleVDS")
                {
                    if (txtTreasuryNo.Text.Trim() == "-" || txtTreasuryNo.Text.Trim() == "")
                    {
                        if (lstDepositType.SelectedValue.ToString().ToLower() != "notdeposited")
                        {
                            MessageBox.Show("Please enter Treasury Number.", this.Text);
                            txtTreasuryNo.Focus();
                            return;
                        }
                    }
                }

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDepositId.Text.Trim();
                }
                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtDepositAmount.Text == "")
                {
                    txtDepositAmount.Text = "0.00";
                }

                if (rbtnVDS.Checked)
                {
                    if (dgvVDS.Rows.Count <= 0)
                    {
                        MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                        return;
                    }

                }

                #endregion

                #region Master Value Assign

                depositMaster = new DepositMasterVM();
                depositMaster.DepositId = NextID.ToString();
                depositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                depositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankDepositDate = dtpBankDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                depositMaster.DepositType = lstDepositType.SelectedValue.ToString();
                depositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                depositMaster.ChequeNo = txtChequeNo.Text.Trim();
                depositMaster.ChequeBank = txtChequeBank.Text.Trim();
                depositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankId = txtBankID.Text.Trim();
                depositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                depositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                depositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();
                depositMaster.Comments = txtComments.Text.Trim();
                depositMaster.CreatedBy = Program.CurrentUser;
                depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                depositMaster.LastModifiedBy = Program.CurrentUser;
                depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                if (lstDepositType.SelectedValue.ToString().ToLower() == "opening")
                {
                    depositMaster.TransactionType = transactionType + "-Opening";

                }
                else
                {
                    depositMaster.TransactionType = transactionType;
                }

                //////depositMaster.TransactionType = Convert.ToString(chkVDS.Checked ? "VDS" : "Treasury");
                depositMaster.Info2 = "Info2";
                depositMaster.Info3 = "Info3";
                depositMaster.Info4 = "Info4";
                depositMaster.Info5 = "Info5";
                depositMaster.Post = "Y";

                #endregion

                #region Detail Value Assign

                vdmMaster = new List<VDSMasterVM>();
                for (int i = 0; i < dgvVDS.RowCount; i++)
                {

                    VDSMasterVM vdsDetail = new VDSMasterVM();

                    vdsDetail.Id = NextID.ToString();
                    vdsDetail.VendorId = dgvVDS.Rows[i].Cells["VendorId"].Value.ToString();
                    vdsDetail.BillAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["BillAmount"].Value.ToString());
                    vdsDetail.BillNo = dgvVDS.Rows[i].Cells["BillNo"].Value.ToString();

                    vdsDetail.BillDate = dgvVDS.Rows[i].Cells["PurchaseDate"].Value.ToString();
                    vdsDetail.BillDeductedAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSAmount"].Value.ToString());
                    vdsDetail.Remarks = dgvVDS.Rows[i].Cells["Remarks"].Value.ToString();
                    vdsDetail.IssueDate = Convert.ToDateTime(dgvVDS.Rows[i].Cells["IssueDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                    vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                    vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                    vdmMaster.Add(vdsDetail);

                }

                #endregion

                #region AdjCashPayble
                if (rbtnAdjCashPayble.Checked)
                {
                    adjustmentHistory = new AdjustmentHistoryVM();
                    adjustmentHistory.AdjId = cmbAdjId.SelectedValue.ToString();
                    adjustmentHistory.AdjDate =
                        dtpAdjDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    adjustmentHistory.AdjInputAmount = Convert.ToDecimal(txtInputAmount.Text.Trim());
                    adjustmentHistory.AdjInputPercent = Convert.ToDecimal(txtInputPercent.Text.Trim());
                    adjustmentHistory.AdjAmount = Convert.ToDecimal(txtAdjAmount.Text.Trim());
                    adjustmentHistory.AdjVATRate = 0;
                    adjustmentHistory.AdjVATAmount = 0;
                    adjustmentHistory.AdjSD = 0;
                    adjustmentHistory.AdjSDAmount = 0;
                    adjustmentHistory.AdjOtherAmount = 0;
                    adjustmentHistory.AdjType = cmbAdjType.SelectedValue.ToString();
                    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                }

                #endregion AdjCashPayble

                #region Background Worker Save

                bgwPost.RunWorkerAsync();

                #endregion

                #region Button Stats

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

            }

            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            #endregion

        }

        private void btnPrintVDS_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvVDS.RowCount > 0)
                {

                    if (Program.CheckLicence(dtpIssueDate.Value) == true)
                    {
                        MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                        return;
                    }
                    //MDIMainInterface mdi = new MDIMainInterface();

                    FormRptVDS frmRptDispose = new FormRptVDS();

                    //mdi.RollDetailsInfo(frmRptDispose.VFIN);
                    //if (Program.Access != "Y")
                    //{
                    //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}

                    frmRptDispose.txtVendorName.Text = dgvVDS.CurrentRow.Cells["VendorName"].Value.ToString();
                    frmRptDispose.txtVendorId.Text = dgvVDS.CurrentRow.Cells["VendorID"].Value.ToString();
                    frmRptDispose.txtDepositNumber.Text = txtDepositId.Text.Trim();
                    frmRptDispose.txtPurchaseNumber.Text = dgvVDS.CurrentRow.Cells["PurchaseNumber"].Value.ToString();
                    frmRptDispose.dtpBillDateFrom.Value = dtpPurchaseDate.Value;
                    frmRptDispose.dtpBillDateTo.Value = dtpPurchaseDate.Value;
                    frmRptDispose.dtpDepositDateFrom.Value = dtpDepositDate.Value;
                    frmRptDispose.dtpDepositDateTo.Value = dtpDepositDate.Value;
                    frmRptDispose.dtpIssueDateFrom.Value = dtpIssueDate.Value;
                    frmRptDispose.dtpIssueDateTo.Value = dtpIssueDate.Value;
                    frmRptDispose.chkPurchaseVDS.Checked = dgvVDS.CurrentRow.Cells["IsPurchase"].Value.ToString() == "Y" ? true : false;

                    frmRptDispose.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No data to progress", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }

            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
            }
            #endregion

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormVendorSearch frm = new FormVendorSearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (chkPurchaseVDS.Checked)
                {

                    DataGridViewRow selectedRow = new DataGridViewRow();

                    selectedRow = FormVendorSearch.SelectOne();
                    if (selectedRow != null && selectedRow.Selected == true)
                    {
                        txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                        txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                        txtVDSPercent.Text = selectedRow.Cells["VDSPercent"].Value.ToString();

                    }

                    #region Old

                    //string result = FormVendorSearch.SelectOne();
                    //if (result == "")
                    //{
                    //    return;
                    //}
                    //else //if (result == ""){return;}else//if (result != "")
                    //{
                    //    string[] VendorInfo = result.Split(FieldDelimeter.ToCharArray());

                    //    txtVendorID.Text = VendorInfo[0];
                    //    txtVendorName.Text = VendorInfo[1];
                    //    txtVDSPercent.Text = VendorInfo[22];
                    //}

                    #endregion

                }
                else
                {
                    DataGridViewRow selectedRow = new DataGridViewRow();

                    selectedRow = FormCustomerSearch.SelectOne();
                    if (selectedRow != null && selectedRow.Selected == true)
                    {
                        txtVendorID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                        txtVendorName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                        txtVDSPercent.Text = "0";

                    }

                    #region Old
                    //string result = FormCustomerSearch.SelectOne();
                    //if (result == "")
                    //{
                    //    return;
                    //}
                    //else //if (result == ""){return;}else//if (result != "")
                    //{
                    //    string[] VendorInfo = result.Split(FieldDelimeter.ToCharArray());

                    //    txtVendorID.Text = VendorInfo[0];
                    //    txtVendorName.Text = VendorInfo[1];
                    //    txtVDSPercent.Text = "0";
                    //}
                    #endregion

                }
                if (chkVDSPercent.Checked == true)
                {
                    txtVDSPercent.Text = "0";
                }

                txtVDSAmount.Text = "0";
                txtBillAmount.Text = "0";
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button1_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion
        }

        //private void txtVDSAmount_TextChanged(object sender, EventArgs e)
        //{
        //    //txtDepositAmount.Text = txtVDSAmount.Text;
        //}

        //private void textBox2_TextChanged(object sender, EventArgs e)
        //{
        //    ChangeVDSAmount();
        //}

        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Me";

                #region Conditonal Actions

                if (chkPurchaseVDS.Checked)
                {
                    var table = FormPurchaseSearch.SelectMultiple("All", true, true);

                    if (table.Rows.Count == 0)
                    {
                        return;
                    }
                    if (IsUpdate == false)
                    {
                        dgvVDS.Rows.Clear();

                    }
                    dtpPurchaseDate.Enabled = false;
                    PopulateGrid(table);

                }
                else
                {
                    DataGridViewRow selectedRow = FormSaleSearch.SelectOne("All", 0, true);
                    if (selectedRow != null && selectedRow.Selected == true)
                    {
                        txtPurchaseNumber.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                        txtVendorID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                        txtVendorName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                        txtBillAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalSubtotal"].Value.ToString()).ToString();//"0,0.00");
                        txtVDSPercent.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
                        txtVDSAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
                        txtSaleVAT.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
                        dtpPurchaseDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDateTime"].Value.ToString());
                        dtpPurchaseDate.Enabled = false;
                    }
                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion
        }

        private void btnAddGrid_Click(object sender, EventArgs e)
        {
            try
            {

                #region Conditional Values

                if (txtVDSComments.Text == "")
                {
                    txtVDSComments.Text = "NA";
                }
                if (txtPurchaseNumber.Text.Trim() == "")
                {
                    txtPurchaseNumber.Text = "NA";
                }
                if (txtVendorID.Text.Trim() == "")
                {
                    MessageBox.Show("Please Select the Vendor");
                    return;
                }

                #endregion

                #region ChangeVDSAmount Function

                ChangeVDSAmount();

                #endregion

                #region Value Assign

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvVDS.Rows.Add(NewRow);
                dgvVDS["VDSAmount", dgvVDS.RowCount - 1].Value = txtVDSAmount.Text.Trim();
                dgvVDS["Remarks", dgvVDS.RowCount - 1].Value = txtVDSComments.Text.Trim();

                #region Conditional Values

                if (chkVDSPercent.Checked == false)
                {
                    dgvVDS["VDSPercent", dgvVDS.RowCount - 1].Value = txtVDSPercent.Text.Trim();
                }
                else
                {
                    dgvVDS["VDSPercent", dgvVDS.RowCount - 1].Value = VDSPercentRate;
                }

                #endregion

                dgvVDS["VendorId", dgvVDS.RowCount - 1].Value = txtVendorID.Text.Trim();
                dgvVDS["VendorName", dgvVDS.RowCount - 1].Value = txtVendorName.Text.Trim();
                dgvVDS["IssueDate", dgvVDS.RowCount - 1].Value = dtpIssueDate.Value;
                dgvVDS["PurchaseDate", dgvVDS.RowCount - 1].Value = dtpPurchaseDate.Value;
                dgvVDS["PurchaseNumber", dgvVDS.RowCount - 1].Value = txtPurchaseNumber.Text.Trim();
                dgvVDS["BillAmount", dgvVDS.RowCount - 1].Value = txtBillAmount.Text.Trim();
                dgvVDS["BillNo", dgvVDS.RowCount - 1].Value = txtBillNo.Text.Trim();
                dgvVDS["IsPercent", dgvVDS.RowCount - 1].Value = chkVDSPercent.Checked == true ? "N" : "Y";
                dgvVDS["IsPurchase", dgvVDS.RowCount - 1].Value = chkPurchaseVDS.Checked == true ? "Y" : "N";

                #region Conditional Values

                if (!chkPurchaseVDS.Checked)
                {
                    dgvVDS["VATAmount", dgvVDS.RowCount - 1].Value = txtSaleVAT.Text.Trim();
                }
                else
                {
                    dgvVDS["VATAmount", dgvVDS.RowCount - 1].Value = "0";
                }

                #endregion

                #endregion

                #region Rowcalculate Function

                Rowcalculate();

                #endregion

                #region selectLastRow Function

                selectLastRow();

                #endregion

                #region Reset Values

                txtVendorID.Text = "";
                txtSaleVAT.Text = "0";

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button2_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            #endregion
        }

        private void PopulateGrid(DataTable table)
        {
            try
            {
                #region For Loop

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    #region dgvVDS Grid Load

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvVDS.Rows.Add(NewRow);

                    dgvVDS["VDSAmount", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["VDSAmount"].ToString();
                    dgvVDS["Remarks", dgvVDS.Rows.Count - 1].Value = txtVDSComments.Text.Trim();
                    decimal billAmount = Convert.ToDecimal(table.Rows[i]["TotalAmount"].ToString());
                    decimal VDSAmount = Convert.ToDecimal(table.Rows[i]["VDSAmount"].ToString());
                    decimal TotalSubtotal = Convert.ToDecimal(table.Rows[i]["TotalSubtotal"].ToString());

                    //////VDSPercentRate = VDSAmount * 100 / billAmount;

                    VDSPercentRate = VDSAmount * 100 / TotalSubtotal;

                    dgvVDS["VDSPercent", dgvVDS.Rows.Count - 1].Value = VDSPercentRate;
                    dgvVDS["VendorId", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["VendorID"].ToString();
                    dgvVDS["VendorName", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["VendorName"].ToString();
                    dgvVDS["IssueDate", dgvVDS.Rows.Count - 1].Value = dtpIssueDate.Value;
                    dgvVDS["PurchaseDate", dgvVDS.Rows.Count - 1].Value = Convert.ToDateTime(table.Rows[i]["InvoiceDateTime"].ToString());
                    dgvVDS["PurchaseNumber", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["PurchaseInvoiceNo"].ToString(); //PurchaseInvoiceNo
                    dgvVDS["BillAmount", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["TotalAmount"].ToString(); //TotalAmount
                    dgvVDS["BillNo", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["BENumber"].ToString(); //TotalAmount
                    dgvVDS["IsPercent", dgvVDS.Rows.Count - 1].Value = chkVDSPercent.Checked == true ? "N" : "Y";
                    dgvVDS["IsPurchase", dgvVDS.Rows.Count - 1].Value = chkPurchaseVDS.Checked == true ? "Y" : "N";
                    dgvVDS["VATAmount", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["TotalVATAmount"].ToString();

                    #endregion

                }

                #endregion

                #region Row Calculate

                Rowcalculate();

                #endregion

                #region Select Last Row

                selectLastRow();

                #endregion

                #region Reset Form Elements

                txtVendorID.Text = "";

                #endregion

            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 05

        private void Rowcalculate()
        {
            try
            {
                decimal SumVDSTotal = 0;
                decimal SumBillTotal = 0;
                for (int i = 0; i < dgvVDS.RowCount; i++)
                {
                    dgvVDS[0, i].Value = i + 1;

                    SumVDSTotal = SumVDSTotal + Convert.ToDecimal(dgvVDS["VDSAmount", i].Value);
                    SumBillTotal = SumBillTotal + Convert.ToDecimal(dgvVDS["BillAmount", i].Value);
                }

                txtVDSTotal.Text = Convert.ToDecimal(SumVDSTotal).ToString("0,0.00");
                txtBillTotal.Text = Convert.ToDecimal(SumBillTotal).ToString("0,0.00");
                txtDepositAmount.Text = txtVDSTotal.Text;
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            #endregion
        }

        private void selectLastRow()
        {
            try
            {
                if (dgvVDS.Rows.Count > 0)
                {

                    dgvVDS.Rows[dgvVDS.Rows.Count - 1].Selected = true;
                    dgvVDS.CurrentCell = dgvVDS.Rows[dgvVDS.Rows.Count - 1].Cells[1];

                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "selectLastRow", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }
            #endregion
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtVDSComments.Text == "")
                {
                    txtVDSComments.Text = "NA";
                }
                if (txtPurchaseNumber.Text.Trim() == "")
                {
                    txtPurchaseNumber.Text = "NA";
                }
                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }
                ChangeVDSAmount();
                dgvVDS["BillAmount", dgvVDS.CurrentRow.Index].Value = txtBillAmount.Text.Trim();
                dgvVDS["BillNo", dgvVDS.CurrentRow.Index].Value = txtBillNo.Text.Trim();
                if (chkVDSPercent.Checked == false)
                {
                    dgvVDS["VDSPercent", dgvVDS.CurrentRow.Index].Value = txtVDSPercent.Text.Trim();
                }
                else
                {
                    dgvVDS["VDSPercent", dgvVDS.CurrentRow.Index].Value = VDSPercentRate;
                }
                dgvVDS["VDSAmount", dgvVDS.CurrentRow.Index].Value = txtVDSAmount.Text.Trim(); // txtUOM.Text.Trim();
                dgvVDS["PurchaseDate", dgvVDS.CurrentRow.Index].Value = dtpPurchaseDate.Value; // txtUOM.Text.Trim();
                dgvVDS["IssueDate", dgvVDS.CurrentRow.Index].Value = dtpIssueDate.Value; // txtUOM.Text.Trim();
                dgvVDS["Remarks", dgvVDS.CurrentRow.Index].Value = txtVDSComments.Text.Trim(); // txtUOM.Text.Trim();
                //dgvSale["PurchaseNumber", dgvSale.CurrentRow.Index].Value = txtPurchaseNumber.Text.Trim();// txtUOM.Text.Trim();

                dgvVDS.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvVDS.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
                dgvVDS["IsPercent", dgvVDS.RowCount - 1].Value = chkVDSPercent.Checked == true ? "N" : "Y";
                dgvVDS["IsPurchase", dgvVDS.RowCount - 1].Value = chkPurchaseVDS.Checked == true ? "Y" : "N";

                Rowcalculate();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            #endregion
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            #region old
            //try
            //{
            //    dgvVDS.CurrentRow.Cells["BillAmount"].Value = 0.00;
            //    dgvVDS.CurrentRow.Cells["VDSPercent"].Value = 0.00;
            //    dgvVDS.CurrentRow.Cells["VDSAmount"].Value = 0.00;

            //    dgvVDS.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
            //    dgvVDS.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
            //    Rowcalculate();
            //}
            #endregion old
            try
            {
                if (dgvVDS.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nVendor Name: " + dgvVDS.CurrentRow.Cells["VendorName"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvVDS.Rows.RemoveAt(dgvVDS.CurrentRow.Index);
                        Rowcalculate();
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }
            #endregion
        }

        private void txtVDSTotal_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
            //txtDepositAmount.Text = txtVDSAmount.Text;
        }

        private void dgvVDS_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvVDS.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                txtVendorID.Text = dgvVDS.CurrentRow.Cells["VendorID"].Value.ToString();
                txtVendorName.Text = dgvVDS.CurrentRow.Cells["VendorName"].Value.ToString();
                txtBillAmount.Text = dgvVDS.CurrentRow.Cells["BillAmount"].Value.ToString();
                txtBillNo.Text = dgvVDS.CurrentRow.Cells["BillNo"].Value.ToString();



                //txtVDSPercent.Text = dgvVDS.CurrentRow.Cells["VDSPercent"].Value.ToString();
                txtVDSAmount.Text = dgvVDS.CurrentRow.Cells["VDSAmount"].Value.ToString();
                dtpPurchaseDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["PurchaseDate"].Value);
                dtpIssueDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["IssueDate"].Value);
                txtVDSComments.Text = dgvVDS.CurrentRow.Cells["Remarks"].Value.ToString();
                txtPurchaseNumber.Text = dgvVDS.CurrentRow.Cells["PurchaseNumber"].Value.ToString();

                chkVDSPercent.Checked = dgvVDS.CurrentRow.Cells["IsPercent"].Value.ToString() == "Y" ? false : true;
                chkPurchaseVDS.Checked = dgvVDS.CurrentRow.Cells["IsPurchase"].Value.ToString() == "Y" ? true : false;

                if (chkVDSPercent.Checked == false)
                {
                    txtVDSPercent.Text = dgvVDS.CurrentRow.Cells["VDSPercent"].Value.ToString();
                }
                else
                {
                    txtVDSPercent.Text = dgvVDS.CurrentRow.Cells["VDSAmount"].Value.ToString();
                }

                btnUpdateSeven.Visible = true;
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvVDS_DoubleClick", exMessage);
            }
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DepositDAL depositDal = new DepositDAL();

                IDeposit depositDal = OrdinaryVATDesktop.GetObject<DepositDAL, DepositRepo, IDeposit>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.DepositPost(depositMaster, vdmMaster, adjustmentHistory, connVM); // Change 04

                //if (sqlResults[0].ToLower() == "success")
                //    depositDal.UpdateVdsComplete("Y", depositMaster.DepositId);

                POST_DOWORK_SUCCESS = true;

                // End DoWork

                #endregion

            }
            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                #region Statement

                if (POST_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;

                        }

                        if (result == "Success")
                        {
                            txtDepositId.Text = sqlResults[2].ToString();
                            txtDepositId1.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }

                #endregion
                ChangeData = false;
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void ChangeVDSAmount()
        {
            try
            {
                string strBillAmount = txtBillAmount.Text;
                decimal vBillAmount = 0;
                string strVDSPercent = txtVDSPercent.Text;
                decimal vVDSPercent = 0;

                if (!string.IsNullOrEmpty(strBillAmount))
                    vBillAmount = Convert.ToDecimal(strBillAmount);

                if (!string.IsNullOrEmpty(strVDSPercent))
                    vVDSPercent = Convert.ToDecimal(strVDSPercent);
                if (chkVDSPercent.Checked == false)
                { txtVDSAmount.Text = Program.ParseDecimalObject(vBillAmount * vVDSPercent / 100).ToString(); }
                else
                {
                    txtVDSAmount.Text = Program.ParseDecimalObject(vVDSPercent).ToString();
                    VDSPercentRate = Convert.ToDecimal(vVDSPercent * 100 / (vBillAmount - vVDSPercent));
                }

                ////txtVDSAmount.Text =
                ////    Convert.ToDecimal(Convert.ToDecimal(txtBillAmount.Text)*Convert.ToDecimal(txtVDSPercent.Text)/100).
                ////        ToString("0,0.00");
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }

            #endregion
        }

        private void txtBillAmount_Leave(object sender, EventArgs e)
        {
            ChangeVDSAmount();
        }

        private void txtVDSPercent_Leave(object sender, EventArgs e)
        {
            ChangeVDSAmount();
        }

        private void txtBillAmount_TextChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void lstDepositType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpPurchaseDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpIssueDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        #endregion

        #region Methods 06

        private void lstDepositType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;


            if (lstDepositType.SelectedValue.ToString().ToLower() == "cheque")
            {
                label46.Visible = true;
                label47.Visible = true;
                label45.Visible = true;
            }
            else
            {
                label46.Visible = false;
                label47.Visible = false;
                label45.Visible = false;
            }

            if (lstDepositType.SelectedValue.ToString().ToLower() == "cheque" || lstDepositType.SelectedValue.ToString() == "Cash")
            {
                label40.Visible = true;
                label41.Visible = true;
                label42.Visible = true;
                label43.Visible = true;
                label44.Visible = true;
            }
            else
            {
                label40.Visible = false;
                label41.Visible = false;
                label42.Visible = false;
                label43.Visible = false;
                label44.Visible = false;
            }


        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btnAddNewVDS_Click(object sender, EventArgs e)
        {
            addNew();
        }

        private void txtVDSPercent_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            searchExist();

        }

        private void txtVDSComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkVDSPercent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkVDSPercent.Checked)
            {
                chkVDSPercent.Text = "VDS(Amt)";
            }
            else
            {
                chkVDSPercent.Text = "VDS(%)";
            }
        }

        private void txtBillTotal_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnPrintTR_Click(object sender, EventArgs e)
        {
            try
            {
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                if (txtDepositId.Text == "")
                {
                    MessageBox.Show("you have to fill up Auto Number");
                    return;
                }

                if (Program.CheckLicence(dtpDepositDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                btnPrintTR.Enabled = false;
                progressBar1.Visible = true;
                bgwTR.RunWorkerAsync();



            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrintTR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrintTR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrintTR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnPrintTR_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintTR_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintTR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintTR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintTR_Click", exMessage);
            }
            #endregion
        }

        private void bgwTR_DoWork(object sender, DoWorkEventArgs e)
        {
            // this.progressBar1.Visible = true;

            try
            {
                #region Statement

                ReportResult = new DataSet();
                ReportSubResult = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();
                IReport depositDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);


                if (rbtnVDS.Checked)
                {
                    if (chkSingleTR6.Checked)
                    {
                        ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim(), connVM);
                    }
                    else
                    {
                        //ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim(), connVM);
                        ReportResult = reportDsdal.VDSDepositNew(txtDepositId.Text.Trim(), connVM);
                    }
                }
                else
                {
                    ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim(), connVM);
                }
                //ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim());


                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwTR_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwTR_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwTR_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwTR_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTR_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTR_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTR_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTR_DoWork", exMessage);
            }
            #endregion


        }

        private void bgwTR_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                ReportClass objrpt = new ReportClass();                // Start Complete
                //ReportDocument objrpt = new ReportDocument();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }
                string TR_Landscape = commonDal.settingsDesktop("Reports", "RptTR_Landscape", settingsDt, connVM);

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!rbtnVDS.Checked)
                {
                    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                    if (company.ToLower() == "scbl")
                    {
                        objrpt = new RptTR_SCBL();

                    }
                    else
                    {
                        if (TR_Landscape == "Y")
                        {
                            objrpt = new RptTR_Landscape();

                        }
                        else
                        {
                            objrpt = new RptTR();

                        }
                    }
                    //objrpt.Load(Program.ReportAppPath + @"\RptTR.rpt");
                    objrpt.SetDataSource(ReportResult);
                    objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'N'";
                }
                else
                {
                    if (chkSingleTR6.Checked)
                    {
                        ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                        if (company.ToLower() == "scbl")
                        {
                            objrpt = new RptTR_SCBL();
                            //objrpt.Load(Program.ReportAppPath + @"\RptTR_SCBL.rpt");
                        }
                        else
                        {

                            if (TR_Landscape == "Y")
                            {
                                objrpt = new RptTR_Landscape();

                            }
                            else
                            {
                                objrpt = new RptTR();

                            }
                        }
                        //objrpt.Load(Program.ReportAppPath + @"\RptTR.rpt");

                        objrpt.SetDataSource(ReportResult);
                        objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'Y'";

                    }
                    else
                    {
                        //ReportSubResult.Tables[0].TableName = "DsVDSDeposit";
                        ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                        if (company.ToLower() == "scbl")
                        {
                            objrpt = new RptTRVDS_SCBL();
                            //objrpt.Load(Program.ReportAppPath + @"\RptTRVDS_SCBL.rpt");

                        }
                        else
                        {
                            if (TR_Landscape == "Y")
                            {
                                objrpt = new RptTRVDS_Landscape();

                            }
                            else
                            {
                                objrpt = new RptTRVDS();

                            }

                        }
                        objrpt.SetDataSource(ReportResult);
                        //objrpt.Subreports[0].SetDataSource(ReportSubResult);
                        //objrpt.Subreports[1].SetDataSource(ReportSubResult);
                    }
                }

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["VATCircle"].Text = "'" + Program.Comments + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                string copiesNo = "";
                int cpno = 0;
                #region CopyName

                for (int i = 1; i <= 3; i++)
                {
                    cpno = i;
                    if (cpno == 1)
                    {
                        copiesNo = cpno + " st copy";
                    }
                    else if (cpno == 2)
                    {
                        copiesNo = cpno + " nd copy";
                    }
                    else if (cpno == 3)
                    {
                        copiesNo = cpno + " rd copy";
                    }
                    else
                    {
                        copiesNo = cpno + " copy";
                    }

                }

                #endregion CopyName
                //objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                FormReport reports = new FormReport();

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                btnPrintTR.Enabled = true;
                progressBar1.Visible = false;
            }

        }

        private void txtVDSAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAdjAmount_Leave(object sender, EventArgs e)
        {
            txtAdjAmount.Text = Program.ParseDecimalObject(txtAdjAmount.Text.Trim()).ToString();


        }

        private void txtInputAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtInputAmount, "txtInputAmount");
            txtInputAmount.Text = Program.ParseDecimalObject(txtInputAmount.Text.Trim()).ToString();

            PercentCalculation();
        }

        private void txtInputPercent_Leave(object sender, EventArgs e)
        {
            txtInputPercent.Text = Program.ParseDecimalObject(txtInputPercent.Text.Trim()).ToString();

            //Program.FormatTextBox(txtInputAmount, "txtInputAmount");
            PercentCalculation();
        }

        private void PercentCalculation()
        {

            string strInputAmount = txtInputAmount.Text.Trim();
            string strPercent = txtInputPercent.Text.Trim();

            decimal inputAmount = 0;
            decimal inputPercent = 0;
            decimal amount = 0;

            if (!string.IsNullOrEmpty(strInputAmount))
            {
                inputAmount = Convert.ToDecimal(strInputAmount);
            }
            if (!string.IsNullOrEmpty(strPercent))
            {
                inputPercent = Convert.ToDecimal(strPercent);
            }


            amount = inputAmount * inputPercent / 100;

            txtAdjAmount.Text = Program.ParseDecimalObject(amount).ToString();
            txtDepositAmount.Text = Program.ParseDecimalObject(amount).ToString();

        }

        #endregion

        #region Methods 07

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchExist();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            addNew();
        }

        private void bgwACPSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                // transId = string.Empty;
                // Start DoWork
                //AdjustmentHistoryDAL adjustmentDal = new AdjustmentHistoryDAL();
                IAdjustmentHistory adjustmentDal = OrdinaryVATDesktop.GetObject<AdjustmentHistoryDAL, AdjustmentHistoryRepo, IAdjustmentHistory>(OrdinaryVATDesktop.IsWCF);
                AdjTypeResult = adjustmentDal.SearchAdjustmentHistoryForDeposit(transId, connVM);

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwACPSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                cmbAdjId.Text = AdjTypeResult.Rows[0]["AdjName"].ToString();//selectedRow.Cells["AdjName"].Value.ToString();
                dtpAdjDate.Value = Convert.ToDateTime(Convert.ToDateTime(AdjTypeResult.Rows[0]["AdjDate"]).ToString("dd/MMM/yyyy"));
                cmbAdjType.SelectedValue = AdjTypeResult.Rows[0]["AdjType"].ToString();//selectedRow.Cells["AdjType1"].Value.ToString();
                txtInputAmount.Text = Program.ParseDecimalObject(AdjTypeResult.Rows[0]["AdjInputAmount"].ToString());//selectedRow.Cells["AdjInputAmount"].Value.ToString();
                txtInputPercent.Text = AdjTypeResult.Rows[0]["AdjInputPercent"].ToString();//selectedRow.Cells["AdjInputPercent"].Value.ToString();
                txtAdjAmount.Text = Program.ParseDecimalObject(AdjTypeResult.Rows[0]["AdjAmount"].ToString());//selectedRow.Cells["AdjAmount"].Value.ToString();
                txtAdjDescription.Text = AdjTypeResult.Rows[0]["AdjDescription"].ToString();//selectedRow.Cells["AdjDescription"].Value.ToString();
                txtAdjHistoryID.Text = AdjTypeResult.Rows[0]["AdjHistoryID"].ToString();// selectedRow.Cells["AdjHistoryID"].Value.ToString();

                txtDepositId2.Text = AdjTypeResult.Rows[0]["AdjHistoryNo"].ToString();//selectedRow.Cells["AdjHistoryNo"].Value.ToString();
                txtAdjReferance.Text = AdjTypeResult.Rows[0]["AdjReferance"].ToString();//selectedRow.Cells["AdjReferance"].Value.ToString();


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwACPSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnSearchDepositNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //string fileName = "";
            //if (chkSame.Checked == false)
            //{
            //    BrowsFile();
            //    fileName = Program.ImportFileName;
            //    if (string.IsNullOrEmpty(fileName))
            //    {
            //        MessageBox.Show("Please select the right file for import");
            //        return;
            //    }
            //}
            //chkSame.Checked = true;
            TransactionTypes();
            FormMasterImport fmi = new FormMasterImport();
            fmi.preSelectTable = "VDS";
            fmi.transactionType = transactionType;
            fmi.Show();
            #region new process for bom import

            //string[] extention = fileName.Split(".".ToCharArray());
            //string[] retResults = new string[4];
            //if (extention[extention.Length - 1] == "txt")
            //{
            //    //retResults = ImportFromText();
            //}
            //else
            //{
            //    retResults = ImportFromExcel();
            //}

            ////string[] sqlResults = Import();
            //string result = retResults[0];
            //string message = retResults[1];
            //if (string.IsNullOrEmpty(result))
            //{
            //    throw new ArgumentNullException("Purchase Ipmort",
            //                                    "Unexpected error.");
            //}
            //else if (result == "Success" || result == "Fail")
            //{
            //    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}

            #endregion new process for bom import
        }

        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";


                //fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                //BugsBD
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*|CSV files (*.csv*)|*.csv*";


                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    Program.ImportFileName = fdlg.FileName;
                }
            }
            #endregion try

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "BrowsFile", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

            #endregion
        }

        private string[] ImportFromExcel()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";
            #endregion Close1

            #region try
            OleDbConnection theConnection = null;
            TransactionTypes();

            try
            {
                //string vdateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                #region Load Excel
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return sqlResults;
                }
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                                          + "Data Source=" + fileName + ";"
                                          + "Extended Properties=" + "\""
                                          + "Excel 12.0;HDR=YES;" + "\"";
                theConnection = new OleDbConnection(connectionString);
                theConnection.Open();

                OleDbDataAdapter adVDSM = new OleDbDataAdapter("SELECT * FROM [VDSM$]", theConnection);
                DataTable dtVDSM = new DataTable();
                adVDSM.Fill(dtVDSM);

                OleDbDataAdapter adVDSD = new OleDbDataAdapter("SELECT * FROM [VDSD$]", theConnection);
                DataTable dtVDSD = new System.Data.DataTable();
                adVDSD.Fill(dtVDSD);

                theConnection.Close();
                #endregion Load Excel

                dtVDSM.Columns.Add("Transection_Type");
                dtVDSM.Columns.Add("Created_By");
                dtVDSM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtVDSM.Rows)
                {
                    row["Transection_Type"] = transactionType;
                    row["Created_By"] = Program.CurrentUser;
                    row["LastModified_By"] = Program.CurrentUser;

                }
                SAVE_DOWORK_SUCCESS = false;

                //DepositDAL depositDal = new DepositDAL();
                IDeposit depositDal = OrdinaryVATDesktop.GetObject<DepositDAL, DepositRepo, IDeposit>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.ImportData(dtVDSM, dtVDSD, 0, connVM);
                SAVE_DOWORK_SUCCESS = true;
            }


            #endregion try
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VDSImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", exMessage);
            }

            #endregion catch
            return sqlResults;
        }

        private void chkPurchaseVDS_CheckedChanged(object sender, EventArgs e)
        {
            chkPurchaseVDS.Text = "Sale";
            l15.Text = "Cust Name";
            label23.Text = "Sale Number";
            btnVAT18.Visible = true;
            if (chkPurchaseVDS.Checked)
            {
                chkPurchaseVDS.Text = "Purchase";
                l15.Text = "Vendor Name";
                label23.Text = "Pur. Number";
                btnVAT18.Visible = false;

            }

        }

        private void dgvVDS_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnImportPurchaseDrtails_Click(object sender, EventArgs e)
        {
            if (chkPurchaseVDS.Checked)
            {
                dgvVDS.Rows.Clear();

                ImportPurchaseDetailFromExcel();
            }
            else
            {
                MessageBox.Show("Please select Purchase for Import");
                return;
            }
        }

        private void ImportPurchaseDetailFromExcel()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";
            #endregion Close1

            string fileName = "";
            if (chkSame.Checked == false)
            {
                BrowsFile();
                fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return;
                }
            }



            #region try
            OleDbConnection theConnection = null;

            try
            {
                //string vdateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                #region Load Excel
                fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                }
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                                          + "Data Source=" + fileName + ";"
                                          + "Extended Properties=" + "\""
                                          + "Excel 12.0;HDR=YES;" + "\"";
                theConnection = new OleDbConnection(connectionString);
                theConnection.Open();

                OleDbDataAdapter adVDSM = new OleDbDataAdapter("SELECT * FROM [PurchaseVDS$]", theConnection);
                DataTable dtVDSM = new DataTable();
                adVDSM.Fill(dtVDSM);

                theConnection.Close();
                #endregion Load Excel

                CommonImportDAL cImport = new CommonImportDAL();
                //CommonImportDAL cImportD = new CommonImportDAL();
                ICommonImport cImportD = OrdinaryVATDesktop.GetObject<CommonImportDAL, CommonImportRepo, ICommonImport>(OrdinaryVATDesktop.IsWCF);


                foreach (DataRow row in dtVDSM.Rows)
                {
                    #region FindVendorId
                    string VendorId = cImportD.FindVendorId(row["VendorName"].ToString().Trim(), row["VendorCode"].ToString().Trim(), null, null, false, connVM);
                    #endregion FindVendorId

                    bool IsPurchaseDate;
                    bool IsIssueDate;
                    bool IsBillAmount;
                    bool IsVDSAmount;
                    //bool IsLCDate;
                    IsPurchaseDate = cImport.CheckDate(row["PurchaseDate"].ToString().Trim());
                    if (IsPurchaseDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in PurchaseDate field.");
                    }
                    IsIssueDate = cImport.CheckDate(row["IssueDate"].ToString().Trim());
                    if (IsIssueDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in IssueDate field.");
                    }
                    IsBillAmount = cImport.CheckNumericBool(row["BillAmount"].ToString().Trim());
                    if (IsBillAmount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in BillAmount field.");
                    }
                    IsVDSAmount = cImport.CheckNumericBool(row["VDSAmount"].ToString().Trim());
                    if (IsVDSAmount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in VDSAmount field.");
                    }
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvVDS.Rows.Add(NewRow);


                    dgvVDS["VendorId", dgvVDS.RowCount - 1].Value = VendorId;
                    dgvVDS["VendorName", dgvVDS.RowCount - 1].Value = row["VendorName"].ToString().Trim();
                    dgvVDS["Remarks", dgvVDS.RowCount - 1].Value = row["Remarks"].ToString().Trim();
                    dgvVDS["IssueDate", dgvVDS.RowCount - 1].Value = row["IssueDate"].ToString().Trim();
                    dgvVDS["PurchaseDate", dgvVDS.RowCount - 1].Value = row["PurchaseDate"].ToString().Trim();
                    dgvVDS["PurchaseNumber", dgvVDS.RowCount - 1].Value = row["PurchaseNumber"].ToString().Trim();
                    dgvVDS["BillAmount", dgvVDS.RowCount - 1].Value = row["BillAmount"].ToString().Trim();
                    dgvVDS["VDSAmount", dgvVDS.RowCount - 1].Value = row["VDSAmount"].ToString().Trim();
                    dgvVDS["VDSPercent", dgvVDS.RowCount - 1].Value = (Convert.ToDecimal(row["VDSAmount"].ToString().Trim()) * 100 / Convert.ToDecimal(row["BillAmount"].ToString().Trim())).ToString();

                    dgvVDS["IsPercent", dgvVDS.RowCount - 1].Value = "N";
                    dgvVDS["IsPurchase", dgvVDS.RowCount - 1].Value = "Y";



                }
                Rowcalculate();


            }


            #endregion try
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VDSImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VDSImport", exMessage);
            }

            #endregion catch
        }

        private void btn6_6_Click(object sender, EventArgs e)
        {
            try
            {
                FormRptVDS12 frmRptDispose = new FormRptVDS12();
                if (rbtnVDS.Checked == true)
                {
                    frmRptDispose._isVDs = true;

                }


                if (dgvVDS.RowCount > 0)
                {



                    if (Program.CheckLicence(dtpIssueDate.Value) == true)
                    {
                        MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                        return;
                    }
                    //MDIMainInterface mdi = new MDIMainInterface();

                    //mdi.RollDetailsInfo(frmRptDispose.VFIN);
                    //if (Program.Access != "Y")
                    //{
                    //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}

                    frmRptDispose.txtVendorName.Text = dgvVDS.CurrentRow.Cells["VendorName"].Value.ToString();
                    frmRptDispose.txtVendorId.Text = dgvVDS.CurrentRow.Cells["VendorID"].Value.ToString();
                    frmRptDispose.txtDepositNumber.Text = txtDepositId.Text.Trim();
                    frmRptDispose.txtPurchaseNumber.Text = dgvVDS.CurrentRow.Cells["PurchaseNumber"].Value.ToString();
                    frmRptDispose.dtpBillDateFrom.Value = dtpPurchaseDate.Value;
                    frmRptDispose.dtpBillDateTo.Value = dtpPurchaseDate.Value;
                    frmRptDispose.dtpDepositDateFrom.Value = dtpDepositDate.Value;
                    frmRptDispose.dtpDepositDateTo.Value = dtpDepositDate.Value;
                    frmRptDispose.dtpIssueDateFrom.Value = dtpIssueDate.Value;
                    frmRptDispose.dtpIssueDateTo.Value = dtpIssueDate.Value;
                    frmRptDispose.chkPurchaseVDS.Checked = dgvVDS.CurrentRow.Cells["IsPurchase"].Value.ToString() == "Y" ? true : false;

                }
                if (rbtnVDS.Checked == true)
                {
                    frmRptDispose.ShowDialog();

                }
                else
                {
                    MessageBox.Show("No data to progress", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
            }
            #endregion
        }

        private void dtpChequeDate_Leave(object sender, EventArgs e)
        {
            dtpChequeDate.Value = Program.ParseDate(dtpChequeDate);
        }

        private void dtpDepositDate_Leave(object sender, EventArgs e)
        {
            dtpDepositDate.Value = Program.ParseDate(dtpDepositDate);
        }

        private void dtpBankDepositDate_Leave(object sender, EventArgs e)
        {
            dtpBankDepositDate.Value = Program.ParseDate(dtpBankDepositDate);
        }

        private void dtpPurchaseDate_Leave(object sender, EventArgs e)
        {
            dtpPurchaseDate.Value = Program.ParseDate(dtpPurchaseDate);
        }

        #endregion

        #region Methods 08

        private void dtpIssueDate_Leave(object sender, EventArgs e)
        {
            dtpIssueDate.Value = Program.ParseDate(dtpIssueDate);
        }

        private void dtpAdjDate_Leave(object sender, EventArgs e)
        {
            dtpAdjDate.Value = Program.ParseDate(dtpAdjDate);
        }

        private void txtBankName_DoubleClick(object sender, EventArgs e)
        {
            BankLoad();
        }

        private void chbInEnglish_Click(object sender, EventArgs e)
        {
            chbInEnglish.Text = "Bangla";
            if (chbInEnglish.Checked)
            {
                chbInEnglish.Text = "English";

            }
        }


        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                decimal billamount = 0;
                decimal vatamount = 0;

                //billamount = dgvVDS.CurrentRow.Cells["BillAmount"].Value.ToString();
                //vatamount = dgvVDS.CurrentRow.Cells["VATAmount"].Value.ToString();

                billamount = Convert.ToDecimal(dgvVDS.CurrentRow.Cells["BillAmount"].Value.ToString());
                vatamount = Convert.ToDecimal(dgvVDS.CurrentRow.Cells["VATAmount"].Value.ToString());

                decimal Finalamount = (billamount - vatamount) * (7.5M / 100.00M);
                txtVDSPercent.Text = Finalamount.ToString();
                txtVDSAmount.Text = Finalamount.ToString();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintVDS_Click", exMessage);
            }
            #endregion

        }

        private void txtDepositPerson_DoubleClick(object sender, EventArgs e)
        {
            UserLoad();
        }

    }
}
