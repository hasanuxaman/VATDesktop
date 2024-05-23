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
    public partial class FormDepositTDS : Form
    {
        #region Constructors

        public FormDepositTDS()
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

        //public string VFIN = "151";
        private void BankSearch()
        {
            try
            {
                //BankInformationDAL bankInformationDal = new BankInformationDAL();
                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);
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
                TransactionTypes();
                BankDetailsInfo();
                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                if (rbtnTDS.Checked)
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
                depositMaster.BankDepositDate = dtpBankDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");// dtpBankDepositDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
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

                depositMaster.DepositPersonContactNo = txtContactNo.Text.Trim();
                depositMaster.DepositPersonAddress = txtAddress.Text.Trim();

                depositMaster.Comments = txtComments.Text.Trim();
                depositMaster.CreatedBy = Program.CurrentUser;
                depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                depositMaster.LastModifiedBy = Program.CurrentUser;
                depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                if (lstDepositType.Text.Trim().ToLower() == "opening")
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
                //if (rbtnAdjCashPayble.Checked)
                //{
                //    adjustmentHistory = new AdjustmentHistoryVM();
                //    adjustmentHistory.AdjId = cmbAdjId.SelectedValue.ToString();
                //    adjustmentHistory.AdjDate =
                //        dtpAdjDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                //    adjustmentHistory.AdjInputAmount = Convert.ToDecimal(txtInputAmount.Text.Trim());
                //    adjustmentHistory.AdjInputPercent = Convert.ToDecimal(txtInputPercent.Text.Trim());
                //    adjustmentHistory.AdjAmount = Convert.ToDecimal(txtAdjAmount.Text.Trim());
                //    adjustmentHistory.AdjVATRate = 0;
                //    adjustmentHistory.AdjVATAmount = 0;
                //    adjustmentHistory.AdjSD = 0;
                //    adjustmentHistory.AdjSDAmount = 0;
                //    adjustmentHistory.AdjOtherAmount = 0;
                //    adjustmentHistory.AdjType = cmbAdjType.Text.Trim();
                //    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                //    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                //    adjustmentHistory.BranchId = Program.BranchId;

                //}

                #endregion AdjCashPayble


                #region VDS
                if (rbtnTDS.Checked)
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

                        if (dgvVDS.Rows[i].Cells["PaymentDate"].Value==null)
                        {
                            MessageBox.Show("Please select Payment Date.", this.Text);
                            dtpPaymentDate.Focus();
                            return;
                        }
                        vdsDetail.PaymentDate =

                              Convert.ToDateTime(dgvVDS.Rows[i].Cells["PaymentDate"].Value.ToString()).ToString(
                                   "yyyy-MMM-dd HH:mm:ss");

                       
                        vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                        vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                        vdsDetail.IsPercent = dgvVDS.Rows[i].Cells["IsPercent"].Value.ToString();
                        vdsDetail.IsPurchase = dgvVDS.Rows[i].Cells["IsPurchase"].Value.ToString();
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
                //DepositTDSDAL depositDal = new DepositTDSDAL();

                IDepositTDS depositDal = OrdinaryVATDesktop.GetObject<DepositTDSDAL, DepositTDSRepo, IDepositTDS>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.DepositInsert(depositMaster, vdmMaster, null, null,connVM); // Change 04

                if (sqlResults[0].ToLower() == "success")
                    depositDal.UpdateVdsComplete("Y", sqlResults[2],connVM);

                SAVE_DOWORK_SUCCESS = true;

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
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }
                ChangeData = false;
                #endregion Success
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
            if (chkPurchaseVDS.Checked == false)
            {
                lstDepositType.Text = "Cash";
            }
            if (lstDepositType.Text.Trim().ToLower() == "select")
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
            txtVDSPercent.Text = "";
            txtVDSAmount.Text = "";
            dtpPurchaseDate.Value = Program.SessionDate;
            dtpIssueDate.Value = Program.SessionDate;
            dgvVDS.Rows.Clear();
            txtBillTotal.Text = "";
            txtVDSTotal.Text = "";
            //txtAdjHistoryID.Text = string.Empty;
            //txtAdjAmount.Text = "0";
            //txtAdjDescription.Text = string.Empty;
            //dtpAdjDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            //cmbAdjType.SelectedIndex = 0;
            //txtDepositId2.Text = string.Empty;
            //txtInputAmount.Text = "0";
            //txtInputPercent.Text = "0";
            //txtAdjReferance.Text = string.Empty;
            IsPost = false;
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
                FormRptDepositTDS frmRptDepositTDS = new FormRptDepositTDS();

                if (txtDepositId.Text == "~~~ New ~~~")
                {
                    frmRptDepositTDS.txtDepositNo.Text = string.Empty;

                }
                else
                {
                    frmRptDepositTDS.txtDepositNo.Text = txtDepositId.Text.Trim();

                }

                frmRptDepositTDS.txtTransactionType.Text = transactionType;
                frmRptDepositTDS.Show();
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

            try
            {
                TransactionTypes();
                transId = string.Empty;
                Program.fromOpen = "Me";
                DataGridViewRow selectedRow = FormDepositTDSSearch.SelectOne(transactionType);

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtDepositId.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    txtDepositId1.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    // txtDepositId2.Text = selectedRow.Cells["DepositId"].Value.ToString();
                    transId = selectedRow.Cells["DepositId"].Value.ToString();
                    txtTreasuryNo.Text = selectedRow.Cells["TreasuryNo"].Value.ToString();
                    dtpDepositDate.Value = Convert.ToDateTime(selectedRow.Cells["DepositDate"].Value.ToString());
                    dtpBankDepositDate.Value = Convert.ToDateTime(selectedRow.Cells["BankDepositDate"].Value.ToString());
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
                    txtContactNo.Text = selectedRow.Cells["DepositPersonContactNo"].Value.ToString();
                    txtAddress.Text = selectedRow.Cells["DepositPersonAddress"].Value.ToString();
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;
                    searchBranchId = Convert.ToInt32(selectedRow.Cells["BranchId"].Value);

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
                    //btnAdd.Text = "&Save";
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
            searchExist();

        }
        private void bgwVDSSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                VDSResult = new DataTable();
                //DepositTDSDAL depositDal = new DepositTDSDAL();
                IDepositTDS depositDal = OrdinaryVATDesktop.GetObject<DepositTDSDAL, DepositTDSRepo, IDepositTDS>(OrdinaryVATDesktop.IsWCF);
                VDSResult = depositDal.SearchVDSDT(txtDepositId.Text,connVM);


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
                    dgvVDS.Rows[j].Cells["LineNo"].Value = item["TDSId"].ToString();
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
                    dgvVDS.Rows[j].Cells["IsPurchase"].Value = item["IsPurchase"].ToString();
                    dgvVDS.Rows[j].Cells["PaymentDate"].Value = item["PaymentDate"].ToString();

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
            transactionType = "TDS";

            #endregion Transaction Type
        }
        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                //BankInformationDAL bankInformationDal = new BankInformationDAL();
                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);
                //BankInformationResult = bankInformationDal.SearchBankDT("", "", "", "", "", "", "", "", "", "", "", "", "");
                BankInformationResult = bankInformationDal.SelectAll(0, null, null, null, null, true,connVM);



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
        private void FormDeposit_Load(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchBankName, "Bank");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchDepositNo, "Existing information");
                //BankSearch();

                //if (cmbBankName.Items.Count <= 0)
                //{
                //    MessageBox.Show("Please input Bank first", this.Text);
                //   return;
                //}
                formMaker();
                TransactionTypes();
                ClearAll();

                txtDepositId.Text = "~~~ New ~~~";
                txtDepositId1.Text = "~~~ New ~~~";
                // txtDepositId2.Text = "~~~ New ~~~";

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                bgwLoad.RunWorkerAsync();
                company = commonDal.settingsDesktop("Reports", "TR6",null,connVM);

                ChangeData = false;

                dtpPaymentDate.Enabled = true;
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
                FileLogger.Log(this.Name, "FormDeposit_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDeposit_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormDeposit_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
            DataGridViewRow selectedRow = new DataGridViewRow();

            string[] shortColumnName = { "UserID", "UserName", "FullName", "Designation", "ContactNo", "Address" };
            string tableName = "UserInformations";
            selectedRow = FormMultipleSearch.SelectOne("", tableName, "", shortColumnName);
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

        private void formMaker()
        {

            #region Button Import Integration Lisence
            if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
            {
                btnImport.Visible = false;
            }
            #endregion

            #region Button ImportPurshaseDetails Integration Lisence
            if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
            {
                btnImportPurchaseDrtails.Visible = false;
            }
            #endregion

            //////btnVAT18.Visible = true;
            //////btnPrintVDS.Visible = true;
            txtDepositAmount.ReadOnly = false;

            btnAddNew.Visible = true;
            btnAddNewVDS.Visible = false;
            //cmbAdjType.SelectedIndex = 0;
            lstDepositType.Items.Insert(0, "Select");
            if (lstDepositType.SelectedIndex != -1)
                lstDepositType.SelectedIndex = 0;
            //tabDeposit.TabPages.Remove(tabPage3);


            //////btnPrintVDS.Visible = true;
            txtDepositAmount.ReadOnly = true;
            tabDeposit.TabPages.Remove(tabPage1);
            tabDeposit.TabPages.Remove(tabPage2);
            // tabDeposit.TabPages.Remove(tabPage3);
            tabDeposit.TabPages.Add(tabPage2);
            tabDeposit.TabPages.Add(tabPage1);
            this.Text = "TDS Deposit Entry";
            btnAddNew.Visible = false;
            btnAddNewVDS.Visible = true;
            dtpPurchaseDate.Enabled = true;


            lstDepositType.SelectedIndex = 0;
            //lstDepositType.SelectedText = "Select";

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
                //if (rbtnAdjCashPayble.Checked)
                //{
                //    cmbAdjId.SelectedIndex = 0;
                //}
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

                //    if (Post == false)
                //    {
                //        MessageBox.Show(MessageVM.msgNotPostAccess, this.Text);
                //        return;
                //    }
                //    else if (
                //MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                //                MessageBoxIcon.Question) != DialogResult.Yes)
                //    {
                //        return;
                //    }
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
                if (txtTreasuryNo.Text.Trim() == "-" || txtTreasuryNo.Text.Trim() == "")
                {
                    if (lstDepositType.Text.Trim().ToLower() != "notdeposited")
                    {
                        MessageBox.Show("Please enter Treasury Number.", this.Text);
                        txtTreasuryNo.Focus();
                        return;
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
                TransactionTypes();

                if (rbtnTDS.Checked)
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
                depositMaster.BankDepositDate = dtpBankDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
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
                    depositMaster.TransactionType = transactionType + "-Opening";

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
                depositMaster.Post = "Y";
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
                    vdsDetail.PaymentDate = Convert.ToDateTime(dgvVDS.Rows[i].Cells["PaymentDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                    vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                    vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                    vdmMaster.Add(vdsDetail);

                }
                #region AdjCashPayble
                //if (rbtnAdjCashPayble.Checked)
                //{
                //    adjustmentHistory = new AdjustmentHistoryVM();
                //    //adjustmentHistory.AdjId = cmbAdjId.SelectedValue.ToString();
                //    //adjustmentHistory.AdjDate =
                //    //    dtpAdjDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                //    //adjustmentHistory.AdjInputAmount = Convert.ToDecimal(txtInputAmount.Text.Trim());
                //    //adjustmentHistory.AdjInputPercent = Convert.ToDecimal(txtInputPercent.Text.Trim());
                //    //adjustmentHistory.AdjAmount = Convert.ToDecimal(txtAdjAmount.Text.Trim());
                //    adjustmentHistory.AdjVATRate = 0;
                //    adjustmentHistory.AdjVATAmount = 0;
                //    adjustmentHistory.AdjSD = 0;
                //    adjustmentHistory.AdjSDAmount = 0;
                //    adjustmentHistory.AdjOtherAmount = 0;
                //    //adjustmentHistory.AdjType = cmbAdjType.Text.Trim();
                //    //adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                //    //adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                //}

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
                        txtVendorID.Text = selectedRow.Cells["CustomerID"].Value.ToString();//[0]
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
                string result;

                Program.fromOpen = "Me";

                dgvVDS.Rows.Clear();
                if (chkPurchaseVDS.Checked)
                {
                    //DataGridViewRow selectedRow 

                    var table = FormPurchaseSearch.SelectMultiple("All", true, true, true);

                    if (table.Rows.Count == 0)
                    {
                        return;
                    }


                    dtpPurchaseDate.Enabled = false;
                    PopulateGrid(table);
                    //if (selectedRow != null && selectedRow.Selected == true)
                    //{
                    //    //if (selectedRow.Cells["WithVDS"].Value.ToString() == "N")
                    //    //{
                    //    //    MessageBox.Show("Please select VDS in Purchase entry", this.Text);
                    //    //    return;
                    //    //}

                    //    //txtPurchaseNumber.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                    //    //txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    //    //txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                    //    //dtpPurchaseDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                    //    //txtBillAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString("0,0.00");
                    //    //txtVDSAmount.Text = Convert.ToDecimal(selectedRow.Cells["VDSAmount"].Value.ToString()).ToString("0,0.00");
                    //    //txtVDSPercent.Text = Convert.ToDecimal(selectedRow.Cells["VDSAmount"].Value.ToString()).ToString("0,0.00");

                    //}
                }
                else
                {
                    DataGridViewRow selectedRow = FormSaleSearch.SelectOne("All");
                    if (selectedRow != null && selectedRow.Selected == true)
                    {
                        txtPurchaseNumber.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                        txtVendorID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                        txtVendorName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                        txtBillAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.00");
                        dtpPurchaseDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                        dtpPurchaseDate.Enabled = false;
                    }
                }


                //txtVDSPercent.Text = "0";
                //txtVDSAmount.Text = "0";

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
                if (txtVendorID.Text.Trim() == "")
                {
                    MessageBox.Show("Please Select the Vendor");
                    return;
                }
                //int ErR = ErrorReturn();
                //if (ErR != 0)
                //{
                //    return;
                //}
                //for (int i = 0; i < dgvVDS.RowCount; i++)
                //{
                //    if (dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString() == txtPurchaseNumber.Text)
                //    {
                //        MessageBox.Show("Same purchase already exist.", this.Text);
                //        txtPurchaseNumber.Focus();
                //        return;
                //    }
                //}

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
                dgvVDS["IsPurchase", dgvVDS.RowCount - 1].Value = chkPurchaseVDS.Checked == true ? "Y" : "N";

                Rowcalculate();
                selectLastRow();
                txtVendorID.Text = "";
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

                //if (txtVDSComments.Text == "")
                //{
                //    txtVDSComments.Text = "NA";
                //}
                //if (txtPurchaseNumber.Text.Trim() == "")
                //{
                //    txtPurchaseNumber.Text = "NA";
                //}
                //if (txtVendorID.Text.Trim() == "")
                //{
                //    MessageBox.Show("Please Select the Vendor");
                //    return;
                //}
                //int ErR = ErrorReturn();
                //if (ErR != 0)
                //{
                //    return;
                //}
                //for (int i = 0; i < dgvVDS.RowCount; i++)
                //{
                //    if (dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString() == txtPurchaseNumber.Text)
                //    {
                //        MessageBox.Show("Same purchase already exist.", this.Text);
                //        txtPurchaseNumber.Focus();
                //        return;
                //    }
                //}

                // ChangeVDSAmount();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvVDS.Rows.Add(NewRow);

                    dgvVDS["VDSAmount", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["TDSAmount"].ToString();
                    dgvVDS["Remarks", dgvVDS.Rows.Count - 1].Value = "-";
                    decimal billAmount = Convert.ToDecimal(table.Rows[i]["TotalAmount"].ToString());
                    decimal VDSAmount = Convert.ToDecimal(table.Rows[i]["TDSAmount"].ToString());
                    VDSPercentRate = VDSAmount * 100 / billAmount;
                    //if (chkVDSPercent.Checked == false)
                    //{
                    //    dgvVDS["VDSPercent", dgvVDS.Rows.Count - 1].Value = txtVDSPercent.Text.Trim();
                    //}
                    //else
                    //{
                    dgvVDS["VDSPercent", dgvVDS.Rows.Count - 1].Value = VDSPercentRate;
                    //}

                    dgvVDS["VendorId", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["VendorID"].ToString();
                    dgvVDS["VendorName", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["VendorName"].ToString();

                    dgvVDS["IssueDate", dgvVDS.Rows.Count - 1].Value = dtpIssueDate.Value;

                    dgvVDS["PurchaseDate", dgvVDS.Rows.Count - 1].Value = Convert.ToDateTime(table.Rows[i]["InvoiceDateTime"].ToString());

                    dgvVDS["PurchaseNumber", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["PurchaseInvoiceNo"].ToString(); //PurchaseInvoiceNo

                    dgvVDS["BillAmount", dgvVDS.Rows.Count - 1].Value = table.Rows[i]["TotalAmount"].ToString(); //TotalAmount


                    dgvVDS["IsPercent", dgvVDS.Rows.Count - 1].Value = chkVDSPercent.Checked == true ? "N" : "Y";
                    dgvVDS["IsPurchase", dgvVDS.Rows.Count - 1].Value = chkPurchaseVDS.Checked == true ? "Y" : "N";
                }




                Rowcalculate();
                selectLastRow();
                txtVendorID.Text = "";
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
                    //dgvVDS.CurrentCell = dgvVDS.Rows[dgvVDS.Rows.Count - 1].Cells[1];

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
                //txtVDSPercent.Text = dgvVDS.CurrentRow.Cells["VDSPercent"].Value.ToString();
                txtVDSAmount.Text = dgvVDS.CurrentRow.Cells["VDSAmount"].Value.ToString();
                if (dgvVDS.CurrentRow.Cells["PaymentDate"].Value != null)
                {
                    dtpPaymentDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["PaymentDate"].Value);

                }
                dtpIssueDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["IssueDate"].Value);
                dtpPurchaseDate.Value = Convert.ToDateTime(dgvVDS.CurrentRow.Cells["PurchaseDate"].Value);

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
                TransactionTypes();
                BankDetailsInfo();

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                if (rbtnTDS.Checked)
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
                depositMaster.BankDepositDate = dtpBankDepositDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                depositMaster.DepositType = lstDepositType.Text.Trim();
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

                if (lstDepositType.Text.Trim().ToLower() == "opening")
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

                #region AdjCashPayble
                //if (rbtnAdjCashPayble.Checked)
                //{
                //    adjustmentHistory = new AdjustmentHistoryVM();
                //    adjustmentHistory.AdjId = cmbAdjId.SelectedValue.ToString();
                //    adjustmentHistory.AdjDate =
                //        dtpAdjDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                //    adjustmentHistory.AdjInputAmount = Convert.ToDecimal(txtInputAmount.Text.Trim());
                //    adjustmentHistory.AdjInputPercent = Convert.ToDecimal(txtInputPercent.Text.Trim());
                //    adjustmentHistory.AdjAmount = Convert.ToDecimal(txtAdjAmount.Text.Trim());
                //    adjustmentHistory.AdjVATRate = 0;
                //    adjustmentHistory.AdjVATAmount = 0;
                //    adjustmentHistory.AdjSD = 0;
                //    adjustmentHistory.AdjSDAmount = 0;
                //    adjustmentHistory.AdjOtherAmount = 0;
                //    adjustmentHistory.AdjType = cmbAdjType.Text.Trim();
                //    adjustmentHistory.AdjDescription = txtAdjDescription.Text.Trim();
                //    adjustmentHistory.AdjReferance = txtAdjReferance.Text.Trim();
                //}

                #endregion AdjCashPayble
                if (rbtnTDS.Checked)
                {
                    vdmMaster = new List<VDSMasterVM>();

                    for (int i = 0; i < dgvVDS.RowCount; i++)
                    {

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        vdsDetail.Id = NextID.ToString();
                        vdsDetail.VendorId = dgvVDS.Rows[i].Cells["VendorId"].Value.ToString();
                        vdsDetail.BillAmount = Convert.ToDecimal(dgvVDS.Rows[i].Cells["BillAmount"].Value.ToString());
                        //format test
                        vdsDetail.BillDate = Convert.ToDateTime(dgvVDS.Rows[i].Cells["PurchaseDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                        vdsDetail.BillDeductedAmount =
                            Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSAmount"].Value.ToString());
                        vdsDetail.Remarks = dgvVDS.Rows[i].Cells["Remarks"].Value.ToString();
                        vdsDetail.IssueDate =
                                Convert.ToDateTime(dgvVDS.Rows[i].Cells["IssueDate"].Value.ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                        vdsDetail.PaymentDate =

                            Convert.ToDateTime(dgvVDS.Rows[i].Cells["PaymentDate"].Value.ToString()).ToString(
                                 "yyyy-MMM-dd HH:mm:ss");

                        vdsDetail.PurchaseNumber = dgvVDS.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                        vdsDetail.VDSPercent = Convert.ToDecimal(dgvVDS.Rows[i].Cells["VDSPercent"].Value.ToString());
                        vdsDetail.IsPercent = dgvVDS.Rows[i].Cells["IsPercent"].Value.ToString();
                        vdsDetail.IsPurchase = dgvVDS.Rows[i].Cells["IsPurchase"].Value.ToString();
                        vdsDetail.BranchId = Program.BranchId;
                        vdmMaster.Add(vdsDetail);

                    }


                    if (rbtnTDS.Checked)
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
                //DepositTDSDAL depositDal = new DepositTDSDAL();

                IDepositTDS depositDal = OrdinaryVATDesktop.GetObject<DepositTDSDAL, DepositTDSRepo, IDepositTDS>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.DepositUpdate(depositMaster, vdmMaster,connVM); // Change 04

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
                //DepositTDSDAL depositDal = new DepositTDSDAL();

                IDepositTDS depositDal = OrdinaryVATDesktop.GetObject<DepositTDSDAL, DepositTDSRepo, IDepositTDS>(OrdinaryVATDesktop.IsWCF);
                sqlResults = depositDal.DepositPost(depositMaster, vdmMaster, adjustmentHistory,connVM); // Change 04

                //if (sqlResults[0].ToLower() == "success")
                //    depositDal.UpdateVdsComplete("Y", depositMaster.DepositId);

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
                //ReportDSDAL reportDsdal = new ReportDSDAL();
                IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                if (chkSingleTR6.Checked)
                {
                    ReportResult = reportDsdal.TDSDeposit(txtDepositId.Text.Trim(),connVM);
                }
                else
                {
                    ReportResult = reportDsdal.TDSDeposit(txtDepositId.Text.Trim(),connVM);
                    ReportSubResult = reportDsdal.TDSDepositDetail(txtDepositId.Text.Trim(),connVM);
                }


                //////if (rbtnTDS.Checked)
                //////{
                //////    if (chkSingleTR6.Checked)
                //////    {
                //////        ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim());
                //////    }
                //////    else
                //////    {
                //////        ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim());
                //////        ReportSubResult = reportDsdal.VDSDepositNew(txtDepositId.Text.Trim());
                //////    }
                //////}
                //////else
                //////{
                //////    ReportResult = reportDsdal.TrasurryDepositeNew(txtDepositId.Text.Trim());
                //////}
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

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                #region Comments

                //////if (!rbtnTDS.Checked)
                //////{
                //////    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                //////    if (company.ToLower() == "scbl")
                //////    {
                //////        objrpt = new RptTR_SCBL();

                //////    }
                //////    else
                //////    {
                //////        objrpt = new RptTR();
                //////    }
                //////    ////////objrpt.Load(Program.ReportAppPath + @"\RptTR.rpt");

                //////        objrpt = new RptTR();

                //////    objrpt.SetDataSource(ReportResult);
                //////    objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'N'";
                //////}

                //////else
                //////{
                ////}
                #endregion



                if (chkSingleTR6.Checked)
                {
                    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                    if (company.ToLower() == "scbl")
                    {
                        objrpt = new RptTR_SCBL();
                        //////objrpt.Load(Program.ReportAppPath + @"\RptTR_SCBL.rpt");
                    }
                    else
                    {
                        objrpt = new RptTR();
                    }
                    ////////objrpt.Load(Program.ReportAppPath + @"\RptTR.rpt");

                    objrpt.SetDataSource(ReportResult);
                    objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'Y'";

                }
                else
                {
                    ReportSubResult.Tables[0].TableName = "DsVDSDeposit";
                    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                    if (company.ToLower() == "scbl")
                    {
                        objrpt = new RptTRVDS_SCBL();
                        ////////objrpt.Load(Program.ReportAppPath + @"\RptTRVDS_SCBL.rpt");

                    }
                    else
                    {
                        objrpt = new RptTRVDS();
                    }
                    objrpt.SetDataSource(ReportResult);
                    objrpt.Subreports[0].SetDataSource(ReportSubResult);
                    objrpt.Subreports[1].SetDataSource(ReportSubResult);
                }




                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["VATCircle"].Text = "'" + Program.Comments + "'";
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

        private void txtVDSAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAdjAmount_Leave(object sender, EventArgs e)
        {


        }

        private void txtInputAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtInputAmount, "txtInputAmount");
            PercentCalculation();
        }

        private void txtInputPercent_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtInputAmount, "txtInputAmount");
            PercentCalculation();
        }
        private void PercentCalculation()
        {

            //string strInputAmount = txtInputAmount.Text.Trim();
            //string strPercent = txtInputPercent.Text.Trim();

            //decimal inputAmount = 0;
            //decimal inputPercent = 0;
            //decimal amount = 0;

            //if (!string.IsNullOrEmpty(strInputAmount))
            //{
            //    inputAmount = Convert.ToDecimal(strInputAmount);
            //}
            //if (!string.IsNullOrEmpty(strPercent))
            //{
            //    inputPercent = Convert.ToDecimal(strPercent);
            //}


            //amount = inputAmount * inputPercent / 100;

            //txtAdjAmount.Text = Convert.ToDecimal(amount).ToString();
            //txtDepositAmount.Text = Convert.ToDecimal(amount).ToString();

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
                // transId = string.Empty;
                // Start DoWork
                //AdjustmentHistoryDAL adjustmentDal = new AdjustmentHistoryDAL();
                IAdjustmentHistory adjustmentDal = OrdinaryVATDesktop.GetObject<AdjustmentHistoryDAL, AdjustmentHistoryRepo, IAdjustmentHistory>(OrdinaryVATDesktop.IsWCF);
                AdjTypeResult = adjustmentDal.SearchAdjustmentHistoryForDeposit(transId,connVM);

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
                //cmbAdjId.Text = AdjTypeResult.Rows[0]["AdjName"].ToString();//selectedRow.Cells["AdjName"].Value.ToString();
                //dtpAdjDate.Value = Convert.ToDateTime(Convert.ToDateTime(AdjTypeResult.Rows[0]["AdjDate"]).ToString("dd/MMM/yyyy"));
                //cmbAdjType.Text = AdjTypeResult.Rows[0]["AdjType"].ToString();//selectedRow.Cells["AdjType1"].Value.ToString();
                //txtInputAmount.Text = AdjTypeResult.Rows[0]["AdjInputAmount"].ToString();//selectedRow.Cells["AdjInputAmount"].Value.ToString();
                //txtInputPercent.Text = AdjTypeResult.Rows[0]["AdjInputPercent"].ToString();//selectedRow.Cells["AdjInputPercent"].Value.ToString();
                //txtAdjAmount.Text = AdjTypeResult.Rows[0]["AdjAmount"].ToString();//selectedRow.Cells["AdjAmount"].Value.ToString();
                //txtAdjDescription.Text = AdjTypeResult.Rows[0]["AdjDescription"].ToString();//selectedRow.Cells["AdjDescription"].Value.ToString();
                //txtAdjHistoryID.Text = AdjTypeResult.Rows[0]["AdjHistoryID"].ToString();// selectedRow.Cells["AdjHistoryID"].Value.ToString();

                //txtDepositId2.Text = AdjTypeResult.Rows[0]["AdjHistoryNo"].ToString();//selectedRow.Cells["AdjHistoryNo"].Value.ToString();
                //txtAdjReferance.Text = AdjTypeResult.Rows[0]["AdjReferance"].ToString();//selectedRow.Cells["AdjReferance"].Value.ToString();


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
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                //fdlg.Filter = "CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*";
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
                if (rbtnTDS.Checked == true)
                {
                    frmRptDispose._isTDs = true;

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

                    frmRptDispose.ShowDialog();
                }
                if (rbtnTDS.Checked == true)
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

        private void btnPaydate_Click(object sender, EventArgs e)
        {
            dgvVDS["PaymentDate", dgvVDS.CurrentRow.Index].Value = dtpPaymentDate.Value.ToString("dd-MMM-yyyy");

        }

        private void dtpPaymentDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

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


    }
}
