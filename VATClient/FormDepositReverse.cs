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
    public partial class FormDepositReverse : Form
    {
        #region Constructors

        public FormDepositReverse()
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
        private DepositDAL depositDal = new DepositDAL();
        private DepositMasterVM depositMaster;
        private SDDepositVM sdDepositMaster;
        private List<VDSMasterVM> vdmMaster;
        private AdjustmentHistoryVM adjustmentHistory;

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
        List<AdjustmentVM> adjNames = new List<AdjustmentVM>();

        private bool ChangeData = false;
        string NextID = string.Empty;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
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
        private decimal ReturnAmount = 0;
        private DataTable AdjResult;
        private string IsExistingVDS="N";
        #endregion
        private void BankSearch()
        {
            try
            {
                //BankInformationDAL bankInformationDal = new BankInformationDAL();
                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);
                //BankInformationResult = bankInformationDal.SearchBankDT("", "", "", "", "", "", "", "", "", "", "", "", "");
                BankInformationResult = bankInformationDal.SelectAll(0, null, null, null, null, true,connVM);

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
            try
            {
                #region Save

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
                if (Convert.ToDecimal(txtDepositAmount.Text) > Convert.ToDecimal(txtRAmount.Text))
                {
                    MessageBox.Show("Reverse amount not greater than Depossit amount.");
                    return;
                }
                TransactionTypes();
                BankDetailsInfo();
                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
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


                #region Deposit
                depositMaster = new DepositMasterVM();
                depositMaster.DepositId = NextID.ToString();
                depositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                depositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.DepositType = lstDepositType.Text.Trim();
                depositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                depositMaster.ChequeNo = txtChequeNo.Text.Trim();
                depositMaster.ChequeBank = txtChequeBank.Text.Trim();
                depositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");//dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.BankId = txtBankID.Text.Trim();
                depositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                depositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                depositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();
                depositMaster.Comments = txtComments.Text.Trim();
                depositMaster.CreatedBy = Program.CurrentUser;
                depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                depositMaster.LastModifiedBy = Program.CurrentUser;
                depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                if (lstDepositType.Text.Trim().ToLower() == "opening")
                    {
                        //depositMaster.TransactionType = transactionType.Trim() + "-Opening";
                        depositMaster.TransactionType = transactionType.Trim() + "-Opening"+"-Credit";
                        
                    }
                    else
                    {
                        depositMaster.TransactionType = transactionType+"-Credit";
                    }
                depositMaster.Info2 = "Info2";
                depositMaster.Info3 = "Info3";
                depositMaster.Info4 = "Info4";
                depositMaster.Info5 = "Info5";
                depositMaster.Post = "N";
                depositMaster.ReturnID = txtReverseID.Text;
                #endregion Deposit
                #region SD
                if (rbtnSD.Checked)
                {
                    sdDepositMaster = new SDDepositVM();
                    sdDepositMaster.DepositId = NextID.ToString();
                    sdDepositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                    sdDepositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); // dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                    sdDepositMaster.DepositType = lstDepositType.Text.Trim();
                    sdDepositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                    sdDepositMaster.ChequeNo = txtChequeNo.Text.Trim();
                    sdDepositMaster.ChequeBank = txtChequeBank.Text.Trim();
                    sdDepositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                    sdDepositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); //  dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                    sdDepositMaster.BankId = txtBankID.Text.Trim();
                    sdDepositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                    sdDepositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                    sdDepositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();
                    sdDepositMaster.Comments = txtComments.Text.Trim();
                    sdDepositMaster.CreatedBy = Program.CurrentUser;
                    sdDepositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    sdDepositMaster.LastModifiedBy = Program.CurrentUser;
                    sdDepositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    if (lstDepositType.Text.Trim().ToLower() == "opening")
                    {
                        sdDepositMaster.TransactionType = "Treasury-Opening-Credit";

                    }
                    else
                    {
                        sdDepositMaster.TransactionType = "Treasury-Credit";
                    }

                    sdDepositMaster.Post = "N";
                    sdDepositMaster.ReturnID = txtReverseID.Text;
                }

                #endregion SD

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
                    adjustmentHistory.AdjType = cmbAdjType.Text.Trim()+ "-Credit";
                    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
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
                        vdmMaster.Add(vdsDetail);

                    }
                        if (vdmMaster.Count <= 0)
                        {
                            MessageBox.Show("Please insert Details information for transaction", this.Text,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                            return;

                        }
                }

                #endregion VDS

                #endregion Save

                bgwSave.RunWorkerAsync();
                this.btnAdd.Enabled = false;
                this.progressBar1.Visible = true;
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
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion
        }
        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                if (rbtnSD.Checked==true)
                {
                    //SDDepositDAL sddepositDal = new SDDepositDAL();
                    ISDDeposit sddepositDal = OrdinaryVATDesktop.GetObject<SDDepositDAL, SDDepositRepo, ISDDeposit>(OrdinaryVATDesktop.IsWCF);
                    sqlResults = sddepositDal.DepositInsert(sdDepositMaster,connVM); // Change 04
                }
                else
                {
                   sqlResults = depositDal.DepositInsert(depositMaster, vdmMaster, adjustmentHistory, null, null,connVM); // Change 04
                }
                
                

                SAVE_DOWORK_SUCCESS = true;
               
                // End DoWork

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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
                            txtDepositID2.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }
                ChangeData = false;
                #endregion Success
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
            }
           
        }

        public int ErrorReturn()
        {
            
            if (lstDepositType.Text.Trim().ToLower()=="select")
            {
                MessageBox.Show("Please select Deposit Type.", this.Text);
                lstDepositType.Focus();
                return 1;
            }
            if (lstDepositType.Text.Trim().ToLower() == "opening")
            {
                txtBankID.Text = "0";
            }
            else if (txtBankID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Deposit Bank Name.", this.Text);
                cmbBankName.Focus();
                //txtBankID.Focus();
                return 1;
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
                txtChequeBank.Text = "-";
            }
            if (txtChequeBankBranch.Text == "")
            {
                txtChequeBankBranch.Text = "-";
            }
            if (txtDepositPerson.Text == "")
            {
                txtDepositPerson.Text = "-";
            }
            if (txtDepositPersonDesignation.Text == "")
            {
                txtDepositPersonDesignation.Text = "-";
            }
            if (txtComments.Text == "")
            {
                txtComments.Text = "-";
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
            txtComments.Text = "";
            dtpChequeDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dtpDepositDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            //VDS Info:
            txtPurchaseNumber.Text = "";
            txtVendorName.Text = "";
            txtVDSComments.Text = "";
            txtBillAmount.Text = "";
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
            cmbAdjType.SelectedIndex = 0;
            txtDepositID2.Text = string.Empty;
            txtInputAmount.Text = "0";
            txtInputPercent.Text = "0";
            txtAdjReferance.Text = string.Empty;
            txtReverseID.Text = string.Empty;
            txtReverseID1.Text = string.Empty;
            txtReverseID2.Text = string.Empty;
            txtRAmount.Text = string.Empty;
            IsPost = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
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
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
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
            Program.FormatTextBox(txtDepositAmount, "Deposit Amount");
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
if (transactionType=="SD")
{
    transactionType = "Treasury";
}
string transactionType1 = transactionType + "-Credit";
                FormRptDepositTransaction frmRptDepositInformation = new FormRptDepositTransaction();

                if (txtDepositId.Text == "~~~ New ~~~")
                {
                    frmRptDepositInformation.txtDepositNo.Text = string.Empty;

                }
                else
                {
                    frmRptDepositInformation.txtDepositNo.Text = txtDepositId.Text.Trim();

                }

                frmRptDepositInformation.txtTransactionType.Text = transactionType1;
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
            BankDetailsInfo();
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
            
            try
            {
                TransactionTypes();
                 transId = string.Empty;
                Program.fromOpen = "Me";
                string transactionType1 = transactionType + "-Credit";
                DataGridViewRow selectedRow = null;
                if (transactionType == "SD")
                {
                    selectedRow = FormSDDepositSearch.SelectOne("Treasury-Credit");
                }
                else
                {
                    selectedRow = FormDepositSearch.SelectOne(transactionType1);
                }

                //DataGridViewRow selectedRow = FormDepositSearch.SelectOne(transactionType);

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtDepositId.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    txtDepositId1.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    txtDepositID2.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    transId = selectedRow.Cells["DepositId"].Value.ToString();
                    txtTreasuryNo.Text = selectedRow.Cells["TreasuryNo"].Value.ToString();
                    dtpDepositDate.Value = Convert.ToDateTime(selectedRow.Cells["DepositDate"].Value.ToString());
                    lstDepositType.Text = selectedRow.Cells["DepositType"].Value.ToString();
                    txtDepositAmount.Text = Convert.ToDecimal(selectedRow.Cells["DepositAmount"].Value.ToString()).ToString("0.00");
                    txtChequeNo.Text = selectedRow.Cells["ChequeNo"].Value.ToString();
                    txtChequeBank.Text = selectedRow.Cells["ChequeBank"].Value.ToString();
                    txtChequeBankBranch.Text = selectedRow.Cells["ChequeBankBranch"].Value.ToString();
                    dtpChequeDate.Value = Convert.ToDateTime(selectedRow.Cells["ChequeDate"].Value.ToString());
                    txtBankID.Text = selectedRow.Cells["BankID"].Value.ToString();
                    txtBankName.Text = selectedRow.Cells["BankName"].Value.ToString();
                    cmbBankName.Text = selectedRow.Cells["BankName"].Value.ToString();

                    txtBranchName.Text = selectedRow.Cells["BranchName"].Value.ToString();
                    txtAccountNumber.Text = selectedRow.Cells["AccountNumber"].Value.ToString();
                    txtDepositPerson.Text = selectedRow.Cells["DepositPerson"].Value.ToString();
                    txtDepositPersonDesignation.Text = selectedRow.Cells["DepositPersonDesignation"].Value.ToString();
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;
                    txtReverseID.Text = selectedRow.Cells["ReverseID"].Value.ToString();
                    txtReverseID1.Text = selectedRow.Cells["ReverseID"].Value.ToString();
                    txtReverseID2.Text = selectedRow.Cells["ReverseID"].Value.ToString();

                    #region Calculate Reverse Amount
                    ReturnAmount = depositDal.ReverseAmount(txtReverseID.Text,connVM);
                    decimal depositAmt = Convert.ToDecimal(selectedRow.Cells["DepositAmount"].Value.ToString());
                    txtRAmount.Text = (depositAmt - ReturnAmount).ToString("0.00");
                    #endregion Calculate Reverse Amount

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
                        else if (txtDepositID2.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwACPSearch.RunWorkerAsync();
                        }
                    }

                    IsUpdate = true;
                    ChangeData = false;


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
            IsExistingVDS = "Y";
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
                if (IsExistingVDS == "Y")
                {
                    VDSResult = depositDal.SearchVDSDT(txtDepositId.Text,connVM);
                }
                else
                {
                    VDSResult = depositDal.SearchVDSDT(txtReverseID.Text,connVM);
                }




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
                if (IsExistingVDS == "Y")
                {
                    dgvVDS.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in VDSResult.Rows)
                    {


                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvVDS.Rows.Add(NewRow);
                        dgvVDS.Rows[j].Cells["LineNo"].Value = item["VDSId"].ToString();
                        dgvVDS.Rows[j].Cells["VendorId"].Value = item["VendorId"].ToString();
                        dgvVDS.Rows[j].Cells["VendorName"].Value = item["VendorName"].ToString();
                        dgvVDS.Rows[j].Cells["BillAmount"].Value = item["BillAmount"].ToString();
                        dgvVDS.Rows[j].Cells["VDSPercent"].Value = item["VDSPercent"].ToString();
                        dgvVDS.Rows[j].Cells["VDSAmount"].Value = item["VDSAmount"].ToString();
                        dgvVDS.Rows[j].Cells["PurchaseDate"].Value = item["PurchaseDate"].ToString();
                        dgvVDS.Rows[j].Cells["IssueDate"].Value = item["IssueDate"].ToString();
                        dgvVDS.Rows[j].Cells["Remarks"].Value = item["Remarks"].ToString();
                        dgvVDS.Rows[j].Cells["PurchaseNumber"].Value = item["PurchaseNumber"].ToString();
                        dgvVDS.Rows[j].Cells["IsPercent"].Value = item["IsPercent"].ToString();
                        j = j + 1;
                    }
                }
                else
                {
                    dgvReverseVDS.Top = dgvVDS.Top;
                    dgvReverseVDS.Left = dgvVDS.Left;
                    dgvReverseVDS.Height = dgvVDS.Height;
                    dgvReverseVDS.Width = dgvVDS.Width;

                    dgvVDS.Visible = false;
                    dgvReverseVDS.Visible = true;


                    dgvReverseVDS.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in VDSResult.Rows)
                    {


                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvReverseVDS.Rows.Add(NewRow);
                        dgvReverseVDS.Rows[j].Cells["LineNoR"].Value = j + 1;
                        dgvReverseVDS.Rows[j].Cells["VendorIdR"].Value = item["VendorId"].ToString();
                        dgvReverseVDS.Rows[j].Cells["VendorNameR"].Value = item["VendorName"].ToString();
                        dgvReverseVDS.Rows[j].Cells["BillAmountR"].Value = item["BillAmount"].ToString();
                        dgvReverseVDS.Rows[j].Cells["VDSPercentR"].Value = item["VDSPercent"].ToString();
                        dgvReverseVDS.Rows[j].Cells["VDSAmountR"].Value = item["VDSAmount"].ToString();
                        dgvReverseVDS.Rows[j].Cells["PurchaseDateR"].Value = item["PurchaseDate"].ToString();
                        dgvReverseVDS.Rows[j].Cells["IssueDateR"].Value = item["IssueDate"].ToString();
                        dgvReverseVDS.Rows[j].Cells["RemarksR"].Value = item["Remarks"].ToString();
                        dgvReverseVDS.Rows[j].Cells["PurchaseNumberR"].Value = item["PurchaseNumber"].ToString();
                        dgvReverseVDS.Rows[j].Cells["IsPercentR"].Value = item["IsPercent"].ToString();
                        j = j + 1;
                    }
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

        
        private void dtpDepositDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtBranchName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

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
                transactionType = "VDS";
            }
            else if (rbtnSD.Checked)
            {
                transactionType = "SD";
            }
            else if (rbtnAdjCashPayble.Checked)
            {
                transactionType = "AdjCashPayble";
            }

            #endregion Transaction Type
        }
        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (rbtnAdjCashPayble.Checked)
                {
                    //AdjustmentDAL adjustmentDal = new AdjustmentDAL();
                    IAdjustment adjustmentDal = OrdinaryVATDesktop.GetObject<AdjustmentDAL, AdjustmentRepo, IAdjustment>(OrdinaryVATDesktop.IsWCF);
                    string[] cFields = new string[] { "Y" };
                    string[] cValues = new string[] { "ActiveStatus like" };
                    AdjTypeResult = adjustmentDal.SelectAll(null, cFields, cValues, null, null, false,connVM);

                    //AdjTypeResult = adjustmentDal.SearchAdjustmentName(string.Empty, "Y");
                }
                //BankInformationDAL bankInformationDal = new BankInformationDAL();
                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);
                BankInformationResult = bankInformationDal.SelectAll(0, null, null, null, null, true,connVM);
//                BankInformationResult = bankInformationDal.SearchBankDT("", "", "", "", "", "", "", "", "", "", "", "", "");

                
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                #region Bank
                cmbBankName.Items.Clear();
                foreach (DataRow item in BankInformationResult.Rows)
                {
                    cmbBankName.Items.Add(item[1].ToString());
                }
                cmbBankName.SelectedIndex = 0;
                #endregion Bank

                if (cmbBankName.Items.Count <= 0)
                {
                    MessageBox.Show("Please input Bank first", this.Text);
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
            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                ChangeData = false;
            }

        }
        private void FormDepositReverse_Load(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchBankName, "Bank");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchDepositNo, "Existing information");
               
                formMaker();
                TransactionTypes();
                //ClearAll();
               
                txtDepositId.Text = "~~~ New ~~~";
                txtDepositId1.Text = "~~~ New ~~~";

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
               
                bgwLoad.RunWorkerAsync();
               ChangeData = false;
                

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
                FileLogger.Log(this.Name, "FormDepositReverse_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDepositReverse_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDepositReverse_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDepositReverse_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormDepositReverse_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDepositReverse_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDepositReverse_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDepositReverse_Load", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }
        private void formMaker()
        {
            btnVAT18.Visible = true;
            btnPrintVDS.Visible = true;
            txtDepositAmount.ReadOnly = false;

            btnAddNew.Visible = true;
            btnAddNewVDS.Visible = false;
           lstDepositType.Items.Insert(0, "Select");
            //if (lstDepositType.SelectedIndex != -1)
            //    lstDepositType.SelectedIndex = 0;
           
            if (rbtnVDS.Checked)
            {
                btnVAT18.Visible = false;
                btnPrintVDS.Visible = true;
                txtDepositAmount.ReadOnly = true;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage2);
                tabDeposit.TabPages.Add(tabPage1);
                this.Text = "Reverse VDS Deposit Entry";
                btnAddNew.Visible = false;
                btnAddNewVDS.Visible = true;
                dtpPurchaseDate.Enabled = true;
            }
            else if (rbtnTreasury.Checked)
            {
                btnVAT18.Visible = true;
                btnPrintVDS.Visible = false;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage1);

                lstDepositType.Items.Insert(1, "Opening");

                this.Text = "Reverse Deposit Entry";


            }
            else if (rbtnSD.Checked)
            {
                btnVAT18.Visible = false;
                btnSD.Visible = true;
                btnPrintVDS.Visible = false;
                txtDepositAmount.ReadOnly = false;
                lstDepositType.Items.Insert(1, "Opening");
                //lstDepositType.SelectedIndex = 0;

                this.Text = "Reverse SD Deposit Entry";


                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage1);
                tabPage1.Text = "SD Deposit";

            }
            else if (rbtnAdjCashPayble.Checked)
            {
                btnVAT18.Visible = false;
                btnPrintVDS.Visible = false;
                txtDepositAmount.ReadOnly = true;
                tabDeposit.TabPages.Remove(tabPage1);
                tabDeposit.TabPages.Remove(tabPage2);
                tabDeposit.TabPages.Remove(tabPage3);
                tabDeposit.TabPages.Add(tabPage3);
                tabDeposit.TabPages.Add(tabPage1);
                btnAddNew.Visible = false;
                btnAddNewVDS.Visible = false;

                this.Text = "Reverse Cash Payable Adjustment Entry";


            }
            //lstDepositType.SelectedIndex = 0;
           
        }

        private void FormDepositReverse_FormClosing(object sender, FormClosingEventArgs e)
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
                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDepositReverse_FormClosing", exMessage);
            }
            #endregion
        }

        private void cmbBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private  void addNew()
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
            try
            {

                if (
            MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                else if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpDepositDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //if (txtTreasuryNo.Text.Trim() == "-" || txtTreasuryNo.Text.Trim() == "")
                //{
                //    MessageBox.Show("Please enter Treasury Number.", this.Text);
                //    txtTreasuryNo.Focus();
                //    return;
                //}
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
                TransactionTypes();

                if (rbtnVDS.Checked)
                {
                    if (dgvVDS.Rows.Count <= 0)
                    {
                        MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                        return;
                    }

                }
                depositMaster = new DepositMasterVM();
                depositMaster.DepositId = NextID.ToString();
                depositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                depositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.DepositType = lstDepositType.Text.Trim();
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
               
                    if (lstDepositType.Text.Trim().ToLower() == "opening")
                    {
                        depositMaster.TransactionType =transactionType+ "-Opening"+"-Credit";

                    }
                    else
                    {
                        depositMaster.TransactionType = transactionType + "-Credit";
                    }

                

                //depositMaster.TransactionType = Convert.ToString(chkVDS.Checked ? "VDS" : "Treasury");
                depositMaster.Info2 = "Info2";
                depositMaster.Info3 = "Info3";
                depositMaster.Info4 = "Info4";
                depositMaster.Info5 = "Info5";
                depositMaster.Post = "Y";
                depositMaster.ReturnID = txtReverseID.Text;
                vdmMaster = new List<VDSMasterVM>();
                for (int i = 0; i < dgvVDS.RowCount; i++)
                {

                    VDSMasterVM vdsDetail = new VDSMasterVM();

                    vdsDetail.Id = NextID.ToString();
                    vdsDetail.VendorId = dgvVDS.Rows[i].Cells["VendorId"].Value.ToString();
                    vdsDetail.BillAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["BillAmount"].Value.ToString());
                    vdsDetail.BillDate = dgvVDS.Rows[i].Cells["PurchaseDate"].Value.ToString();
                    vdsDetail.BillDeductedAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSAmount"].Value.ToString());
                    vdsDetail.Remarks = dgvVDS.Rows[i].Cells["Remarks"].Value.ToString();
                    vdsDetail.IssueDate = Convert.ToDateTime(dgvVDS.Rows[i].Cells["IssueDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss"); 
                    vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                    vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                    vdmMaster.Add(vdsDetail);

                }
                if (rbtnSD.Checked)
                {
                    sdDepositMaster = new SDDepositVM();
                    sdDepositMaster.DepositId = NextID.ToString();
                    sdDepositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                    sdDepositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); // dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                    sdDepositMaster.DepositType = lstDepositType.Text.Trim();
                    sdDepositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                    sdDepositMaster.ChequeNo = txtChequeNo.Text.Trim();
                    sdDepositMaster.ChequeBank = txtChequeBank.Text.Trim();
                    sdDepositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                    sdDepositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); //  dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                    sdDepositMaster.BankId = txtBankID.Text.Trim();
                    sdDepositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                    sdDepositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                    sdDepositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();
                    sdDepositMaster.Comments = txtComments.Text.Trim();
                    sdDepositMaster.CreatedBy = Program.CurrentUser;
                    sdDepositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    sdDepositMaster.LastModifiedBy = Program.CurrentUser;
                    sdDepositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    if (lstDepositType.Text.Trim().ToLower() == "opening")
                    {
                        sdDepositMaster.TransactionType = "Treasury-Opening-Credit";

                    }
                    else
                    {
                        sdDepositMaster.TransactionType = "Treasury-Credit";
                    }
                    sdDepositMaster.Post = "Y";
                    sdDepositMaster.ReturnID = txtReverseID.Text;
                }
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
                    adjustmentHistory.AdjType = cmbAdjType.Text.Trim() + "-Credit";
                    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                }

                #endregion AdjCashPayble

                bgwPost.RunWorkerAsync();
                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";

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

                if (chkVDSPercent.Checked==true)
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

       
        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            try
            {
                string result;
               
                Program.fromOpen = "Me";

               
                DataGridViewRow selectedRow = FormPurchaseSearch.SelectOne("All");


                if (selectedRow != null && selectedRow.Selected==true)
                {
                    if (selectedRow.Cells["WithVDS"].Value.ToString() == "N")
                    {
                        MessageBox.Show("Please select VDS in Purchase entry",this.Text);
                    return;    
                    }

                    txtPurchaseNumber.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                    txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                    dtpPurchaseDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                    txtBillAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString("0,0.00");
                    dtpPurchaseDate.Enabled = false;

                }
                txtVDSPercent.Text = "0";
                txtVDSAmount.Text = "0";
               
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


                if (txtVDSComments.Text == "")
                {
                    txtVDSComments.Text = "NA";
                }
                if (txtPurchaseNumber.Text.Trim() == "")
                {
                    txtPurchaseNumber.Text = "NA";
                }
                ChangeVDSAmount();
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvVDS.Rows.Add(NewRow);
                dgvVDS["VDSAmount", dgvVDS.RowCount - 1].Value = txtVDSAmount.Text.Trim();
                dgvVDS["Remarks", dgvVDS.RowCount - 1].Value = txtVDSComments.Text.Trim();
                if (chkVDSPercent.Checked == false)
                {
                    dgvVDS["VDSPercent", dgvVDS.RowCount - 1].Value = txtVDSPercent.Text.Trim();
                }
                else
                {
                    dgvVDS["VDSPercent", dgvVDS.RowCount - 1].Value = VDSPercentRate;
                }
                dgvVDS["VendorId", dgvVDS.RowCount - 1].Value = txtVendorID.Text.Trim();
                dgvVDS["VendorName", dgvVDS.RowCount - 1].Value = txtVendorName.Text.Trim();
                dgvVDS["IssueDate", dgvVDS.RowCount - 1].Value = dtpIssueDate.Value;
                dgvVDS["PurchaseDate", dgvVDS.RowCount - 1].Value = dtpPurchaseDate.Value;
                dgvVDS["PurchaseNumber", dgvVDS.RowCount - 1].Value = txtPurchaseNumber.Text.Trim();
                dgvVDS["BillAmount", dgvVDS.RowCount - 1].Value = txtBillAmount.Text.Trim();
                dgvVDS["IsPercent", dgvVDS.RowCount - 1].Value = chkVDSPercent.Checked == true ? "N" : "Y";

                Rowcalculate();
                selectLastRow();
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

                dgvVDS.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvVDS.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
                dgvVDS["IsPercent", dgvVDS.RowCount - 1].Value = chkVDSPercent.Checked == true ? "N" : "Y";

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
                txtVDSAmount.Text = dgvVDS.CurrentRow.Cells["VDSAmount"].Value.ToString();
                dtpPurchaseDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["PurchaseDate"].Value);
                dtpIssueDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["IssueDate"].Value);
                txtVDSComments.Text = dgvVDS.CurrentRow.Cells["Remarks"].Value.ToString();
                txtPurchaseNumber.Text = dgvVDS.CurrentRow.Cells["PurchaseNumber"].Value.ToString();

                chkVDSPercent.Checked = dgvVDS.CurrentRow.Cells["IsPercent"].Value.ToString() == "Y" ? false : true;

                if (chkVDSPercent.Checked == false)
                {
                    txtVDSPercent.Text = dgvVDS.CurrentRow.Cells["VDSPercent"].Value.ToString();
                }
                else {
                    txtVDSPercent.Text = dgvVDS.CurrentRow.Cells["VDSAmount"].Value.ToString();
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                #region Update
                //if (Edit == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotEditAccess, this.Text);
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
                TransactionTypes();
                BankDetailsInfo();

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
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
                depositMaster = new DepositMasterVM();
                depositMaster.DepositId = NextID.ToString();
                depositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                depositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                depositMaster.DepositType = lstDepositType.Text.Trim();
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
               
                    if (lstDepositType.Text.Trim().ToLower() == "opening")
                    {
                        depositMaster.TransactionType = transactionType.Trim()+"-Opening"+"-Credit";

                    }
                    else
                    {
                        depositMaster.TransactionType = transactionType.Trim() + "-Credit";
                    }
                depositMaster.Info2 = "Info2";
                depositMaster.Info3 = "Info3";
                depositMaster.Info4 = "Info4";
                depositMaster.Info5 = "Info5";
                depositMaster.Post = "N";
                depositMaster.ReturnID = txtReverseID.Text;

                #region SD
                if (rbtnSD.Checked)
                {
                    sdDepositMaster = new SDDepositVM();
                    sdDepositMaster.DepositId = NextID.ToString();
                    sdDepositMaster.TreasuryNo = txtTreasuryNo.Text.Trim();
                    sdDepositMaster.DepositDate = dtpDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); // dtpDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                    sdDepositMaster.DepositType = lstDepositType.Text.Trim();
                    sdDepositMaster.DepositAmount = Convert.ToDecimal(txtDepositAmount.Text.Trim());
                    sdDepositMaster.ChequeNo = txtChequeNo.Text.Trim();
                    sdDepositMaster.ChequeBank = txtChequeBank.Text.Trim();
                    sdDepositMaster.ChequeBankBranch = txtChequeBankBranch.Text.Trim();
                    sdDepositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); //  dtpChequeDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                    sdDepositMaster.BankId = txtBankID.Text.Trim();
                    sdDepositMaster.TreasuryCopy = txtTreasuryCopy.Text.Trim();
                    sdDepositMaster.DepositPerson = txtDepositPerson.Text.Trim();
                    sdDepositMaster.DepositPersonDesignation = txtDepositPersonDesignation.Text.Trim();
                    sdDepositMaster.Comments = txtComments.Text.Trim();
                    sdDepositMaster.CreatedBy = Program.CurrentUser;
                    sdDepositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    sdDepositMaster.LastModifiedBy = Program.CurrentUser;
                    sdDepositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    if (lstDepositType.Text.Trim().ToLower() == "opening")
                    {
                        sdDepositMaster.TransactionType = "Treasury-Opening-Credit";

                    }
                    else
                    {
                        sdDepositMaster.TransactionType = "Treasury-Credit";
                    }

                    sdDepositMaster.Post = "N";
                    sdDepositMaster.ReturnID = txtReverseID.Text;
                }

                #endregion SD
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
                    adjustmentHistory.AdjType = cmbAdjType.Text.Trim() + "-Credit";
                    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                }

                #endregion AdjCashPayble
                if (rbtnVDS.Checked)
                {
                    vdmMaster = new List<VDSMasterVM>();

                    for (int i = 0; i < dgvVDS.RowCount; i++)
                    {

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        vdsDetail.Id = NextID.ToString();
                        vdsDetail.VendorId = dgvVDS.Rows[i].Cells["VendorId"].Value.ToString();
                        vdsDetail.BillAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["BillAmount"].Value.ToString());
                        vdsDetail.BillDate =Convert.ToDateTime(dgvVDS.Rows[i].Cells["PurchaseDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                        vdsDetail.BillDeductedAmount =
                            Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSAmount"].Value.ToString());
                        vdsDetail.Remarks = dgvVDS.Rows[i].Cells["Remarks"].Value.ToString();
                        vdsDetail.IssueDate =
                                Convert.ToDateTime(dgvVDS.Rows[i].Cells["IssueDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                        
                        vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                        vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                        vdsDetail.IsPercent = dgvVDS.Rows[i].Cells["IsPercent"].Value.ToString();
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

                #endregion Save

                bgwUpdate.RunWorkerAsync();
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

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
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                if (rbtnSD.Checked == true)
                {
                    //SDDepositDAL sddepositDal = new SDDepositDAL();
                    ISDDeposit sddepositDal = OrdinaryVATDesktop.GetObject<SDDepositDAL, SDDepositRepo, ISDDeposit>(OrdinaryVATDesktop.IsWCF);
                    sqlResults = sddepositDal.DepositUpdate(sdDepositMaster,connVM); // Change 04
                }
                else
                {
                    sqlResults = depositDal.DepositUpdate(depositMaster, vdmMaster, adjustmentHistory,connVM); // Change 04
                }

               

                UPDATE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                if (rbtnSD.Checked == true)
                {
                    //SDDepositDAL sddepositDal = new SDDepositDAL();
                    ISDDeposit sddepositDal = OrdinaryVATDesktop.GetObject<SDDepositDAL, SDDepositRepo, ISDDeposit>(OrdinaryVATDesktop.IsWCF);
                    sqlResults = sddepositDal.DepositPost(sdDepositMaster,connVM); // Change 04
                }
                else
                {
                    sqlResults = depositDal.DepositPost(depositMaster, vdmMaster, adjustmentHistory,connVM); // Change 04
                }
                POST_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }
           
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
                { txtVDSAmount.Text = Convert.ToDecimal(vBillAmount * vVDSPercent / 100).ToString("0,0.0000"); }
                else
                {
                    txtVDSAmount.Text = Convert.ToDecimal(vVDSPercent).ToString("0,0.0000");
                    VDSPercentRate = Convert.ToDecimal(vVDSPercent * 100 / vBillAmount);
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
            Program.FormatTextBox(txtBillAmount, "BillAmount");
            ChangeVDSAmount();
        }

        private void txtVDSPercent_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVDSPercent, "VDSPercent");
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

        private void lstDepositType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

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
            IsExistingVDS = "Y";
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
                if (transactionType == "SD")
                {
                    bgwSDTR.RunWorkerAsync();
                }
                else
                {
                    bgwTR.RunWorkerAsync();
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
           try
            {
                #region Statement

                ReportResult = new DataSet();
                ReportSubResult = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();
                IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                if (rbtnVDS.Checked)
                {
                    ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim(),connVM);
                    ReportSubResult = reportDsdal.VDSDepositNew(txtDepositId.Text.Trim(),connVM);
                }
                else
                {
                    ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim(),connVM);
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


                DBSQLConnection _dbsqlConnection = new DBSQLConnection(); 
                
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                //objrpt = new RptTR();
                //objrpt.SetDataSource(ReportResult);

                if (!rbtnVDS.Checked)
                {
                    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                    objrpt = new RptTR();
                    objrpt.SetDataSource(ReportResult);
                }
                else
                {
                    ReportSubResult.Tables[0].TableName = "DsVDSDeposit";
                    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                    objrpt = new RptTRVDS();
                    //objrpt = new RptTRVDSTest();
                    objrpt.SetDataSource(ReportResult);
                    //objrpt.Subreports[0].DataSourceConnections.Clear();
                    objrpt.Subreports[0].SetDataSource(ReportSubResult);
                    objrpt.Subreports[1].SetDataSource(ReportSubResult);

                    //ReportDocument cryRpt = new ReportDocument();
                    //cryRpt.Load("rptMainReport.rpt");
                    //cryRpt.SetParameterValue("myRoleId", 2, "subreport name");

                }
               
                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
               objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["VATCircle"].Text = "'" + Program.Comments + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";
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
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
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

        private void txtInputAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtInputAmount, "txtInputAmount");
            PercentCalculation();
        }

        private void txtInputPercent_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtInputAmount, "txtInputAmount");
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

            txtAdjAmount.Text = Convert.ToDecimal(amount).ToString();
            txtDepositAmount.Text = Convert.ToDecimal(amount).ToString();

        }

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
                // Start DoWork
                
                //AdjustmentHistoryDAL adjustmentDal = new AdjustmentHistoryDAL();
                IAdjustmentHistory adjustmentDal = OrdinaryVATDesktop.GetObject<AdjustmentHistoryDAL, AdjustmentHistoryRepo, IAdjustmentHistory>(OrdinaryVATDesktop.IsWCF);
                AdjResult = adjustmentDal.SearchAdjustmentHistoryForDeposit(transId,connVM);
                
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
                cmbAdjId.Text = AdjResult.Rows[0]["AdjName"].ToString();//selectedRow.Cells["AdjName"].Value.ToString();
                dtpAdjDate.Value = Convert.ToDateTime(Convert.ToDateTime(AdjResult.Rows[0]["AdjDate"]).ToString("dd/MMM/yyyy"));
                cmbAdjType.Text = AdjResult.Rows[0]["AdjType"].ToString();//selectedRow.Cells["AdjType1"].Value.ToString();
                txtInputAmount.Text = AdjResult.Rows[0]["AdjInputAmount"].ToString();//selectedRow.Cells["AdjInputAmount"].Value.ToString();
                txtInputPercent.Text = AdjResult.Rows[0]["AdjInputPercent"].ToString();//selectedRow.Cells["AdjInputPercent"].Value.ToString();
                txtAdjAmount.Text = AdjResult.Rows[0]["AdjAmount"].ToString();//selectedRow.Cells["AdjAmount"].Value.ToString();
                txtAdjDescription.Text = AdjResult.Rows[0]["AdjDescription"].ToString();//selectedRow.Cells["AdjDescription"].Value.ToString();
                txtAdjHistoryID.Text = AdjResult.Rows[0]["AdjHistoryID"].ToString();// selectedRow.Cells["AdjHistoryID"].Value.ToString();

                //txtDepositID2.Text = AdjResult.Rows[0]["AdjHistoryNo"].ToString();//selectedRow.Cells["AdjHistoryNo"].Value.ToString();
                txtAdjReferance.Text = AdjResult.Rows[0]["AdjReferance"].ToString();//selectedRow.Cells["AdjReferance"].Value.ToString();


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

            #region new process for bom import

            string[] extention = fileName.Split(".".ToCharArray());
            string[] retResults = new string[4];
            if (extention[extention.Length - 1] == "txt")
            {
                //retResults = ImportFromText();
            }
            else
            {
                retResults = ImportFromExcel();
            }

            //string[] sqlResults = Import();
            string result = retResults[0];
            string message = retResults[1];
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException("Purchase Ipmort",
                                                "Unexpected error.");
            }
            else if (result == "Success" || result == "Fail")
            {
                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

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
                fdlg.Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (*.xlsm)|*.xlsm|Excel(97-2003) files (*.xls)|*.xls|Text files (*.txt)|*.txt";
                
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
                sqlResults = depositDal.ImportData(dtVDSM, dtVDSD,0,connVM);
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

        public void searchReverseExist(string TransType)
        {

            try
            {
                if (TransType=="VDS")
                {
                    rbtnVDS.Checked = true;
                }
                else if (TransType=="Treasury")
                {
                    rbtnTreasury.Checked = true;
                }
                else if (TransType=="SD")
                {
                    rbtnSD.Checked = true;
                }
                else if (TransType == "AdjCashPayble")
                {
                    rbtnAdjCashPayble.Checked = true;
                }
                
                transId = string.Empty;
                Program.fromOpen = "Me";
                DataGridViewRow selectedRow = null;
                if (TransType == "SD")
                {
                    selectedRow = FormSDDepositSearch.SelectOne(Convert.ToString("Treasury"));
                }
                else
                {
                    selectedRow = FormDepositSearch.SelectOne(TransType);
                }
                

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    if (selectedRow.Cells["Post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not posted ", this.Text);
                        return;
                    }
                    txtReverseID.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    txtReverseID1.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    txtReverseID2.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    transId = selectedRow.Cells["DepositId"].Value.ToString();
                    txtTreasuryNo.Text = selectedRow.Cells["TreasuryNo"].Value.ToString();
                    dtpDepositDate.Value = Convert.ToDateTime(selectedRow.Cells["DepositDate"].Value.ToString());
                    lstDepositType.Text = selectedRow.Cells["DepositType"].Value.ToString();
                    txtDepositAmount.Text = Convert.ToDecimal(selectedRow.Cells["DepositAmount"].Value.ToString()).ToString("0.00");
                    
                    txtChequeNo.Text = selectedRow.Cells["ChequeNo"].Value.ToString();
                    txtChequeBank.Text = selectedRow.Cells["ChequeBank"].Value.ToString();
                    txtChequeBankBranch.Text = selectedRow.Cells["ChequeBankBranch"].Value.ToString();
                    dtpChequeDate.Value = Convert.ToDateTime(selectedRow.Cells["ChequeDate"].Value.ToString());
                    txtBankID.Text = selectedRow.Cells["BankID"].Value.ToString();
                    txtBankName.Text = selectedRow.Cells["BankName"].Value.ToString();
                    cmbBankName.Text = selectedRow.Cells["BankName"].Value.ToString();

                    txtBranchName.Text = selectedRow.Cells["BranchName"].Value.ToString();
                    txtAccountNumber.Text = selectedRow.Cells["AccountNumber"].Value.ToString();
                    txtDepositPerson.Text = selectedRow.Cells["DepositPerson"].Value.ToString();
                    txtDepositPersonDesignation.Text = selectedRow.Cells["DepositPersonDesignation"].Value.ToString();
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();

                    #region Calculate Reverse Amount
                    ReturnAmount = depositDal.ReverseAmount(txtReverseID.Text,connVM);
                    decimal depositAmt = Convert.ToDecimal(selectedRow.Cells["DepositAmount"].Value.ToString());
                    txtRAmount.Text = (depositAmt - ReturnAmount).ToString("0.00");
                    #endregion Calculate Reverse Amount
                    
                    if (rbtnVDS.Checked)
                    {
                        if (txtReverseID.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwVDSSearch.RunWorkerAsync();

                        }
                        else if (txtReverseID1.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwVDSSearch.RunWorkerAsync();
                        }
                    }
                    else if (rbtnAdjCashPayble.Checked)
                    {
                        if (txtReverseID.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwACPSearch.RunWorkerAsync();

                        }
                        else if (txtDepositID2.Text != "")
                        {
                            this.btnSearchDepositNo.Enabled = false;
                            this.progressBar1.Visible = true;
                            bgwACPSearch.RunWorkerAsync();
                        }
                    }
                    IsUpdate = true;



                    ChangeData = false;


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

        private void btnReverse_Click(object sender, EventArgs e)
        {
            searchReverseExist(transactionType);
        }

        private void dgvReverseVDS_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvReverseVDS["Select", e.RowIndex].Value = !Convert.ToBoolean(dgvReverseVDS["Select", e.RowIndex].Value);
            }
        }

        private void dgvReverseVDS_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.F2))
                {
                    dgvVDS.Rows.Clear();
                    int j = 0;
                    for (int i = 0; i < dgvReverseVDS.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dgvReverseVDS["Select", i].Value) == true)
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvVDS.Rows.Add(NewRow);

                            dgvVDS.Rows[j].Cells["LineNo"].Value = j + 1;
                            dgvVDS.Rows[j].Cells["VendorId"].Value = dgvReverseVDS["VendorIdR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["VendorName"].Value = dgvReverseVDS["VendorNameR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["BillAmount"].Value = dgvReverseVDS["BillAmountR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["VDSPercent"].Value = dgvReverseVDS["VDSPercentR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["VDSAmount"].Value = dgvReverseVDS["VDSAmountR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["PurchaseDate"].Value = dgvReverseVDS["PurchaseDateR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["IssueDate"].Value = dgvReverseVDS["IssueDateR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["Remarks"].Value = dgvReverseVDS["RemarksR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["PurchaseNumber"].Value = dgvReverseVDS["PurchaseNumberR", i].Value.ToString();
                            dgvVDS.Rows[j].Cells["IsPercent"].Value = dgvReverseVDS["IsPercentR", i].Value.ToString();
                            j = j + 1;
                        }
                    }

                    Rowcalculate();
                    dgvReverseVDS.Visible = false;
                    dgvVDS.Visible = true;
                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvReverseVDS_KeyDown", exMessage);
            }

            #endregion
        }

        private void btnSD_Click(object sender, EventArgs e)
        {
            FormRptSD frmRptSD = new FormRptSD();// RollDetailsInfo(frmRptSD.VFIN);
            if (Program.Access != "Y")
            {
                MessageBox.Show("You do not have to access this form", frmRptSD.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //MDIMainInterface mdi = new MDIMainInterface();
            //mdi.RollDetailsInfo("8601");
            frmRptSD.Show();
        }


        private void btnReverseD_Click(object sender, EventArgs e)
        {
            searchReverseExist(transactionType);
        }

        private void btnReverseC_Click(object sender, EventArgs e)
        {
            searchReverseExist(transactionType);
        }

        private void bgwSDTR_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReportResult = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();

                IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
                ReportResult = reportDsdal.SDTrasurryDepositeNew(txtDepositId.Text.Trim(),connVM);



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

        private void bgwSDTR_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                ReportClass objrpt = new ReportClass();                // Start Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection(); 
                

                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                objrpt = new RptTR();
                objrpt.SetDataSource(ReportResult);

                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                // objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                // objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";

                FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
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

        private void txtVDSAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVDSAmount, "VDSAmount");
        }

        private void txtBillTotal_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtBillTotal, "BillTotal");
        }

        private void txtVDSTotal_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVDSTotal, "VDSTotal");
        }

        private void txtAdjAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtAdjAmount, "AdjAmount");
        }

        private void dtpChequeDate_Leave(object sender, EventArgs e)
        {
            //dtpChequeDate.Value = Program.ParseDate(dtpChequeDate);
        }

        private void dtpDepositDate_Leave(object sender, EventArgs e)
        {
            //dtpDepositDate.Value = Program.ParseDate(dtpDepositDate);
        }

        private void dtpPurchaseDate_Leave(object sender, EventArgs e)
        {
            //dtpPurchaseDate.Value = Program.ParseDate(dtpPurchaseDate);
        }

        private void dtpIssueDate_Leave(object sender, EventArgs e)
        {
            //dtpIssueDate.Value = Program.ParseDate(dtpIssueDate);
        }

        private void dtpAdjDate_Leave(object sender, EventArgs e)
        {
            //dtpAdjDate.Value = Program.ParseDate(dtpAdjDate);
        }
    }
}
