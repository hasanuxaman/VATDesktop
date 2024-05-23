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

//
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Data.OleDb;
using System.Threading;
using VATServer.Library;
using SymphonySofttech.Reports.List;
using CrystalDecisions.CrystalReports.Engine;
using VATViewModel.DTOs;
using VATServer.License;

namespace VATClient.ReportPreview
{
    public partial class FormRptVAT16Toll : Form
    {
        public FormRptVAT16Toll()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        private bool PreviewOnly = false;
        private bool IsTollRegister = false;
        private bool Digits3 = false;
        private string IsRaw = "NA";

        private int BranchId = 0;
        public int SenderBranchId = 0;

        #region Global Variables For BackGroundWorker

        private DataSet ReportResult;
        string post1, post2 = string.Empty;

        #endregion

        string IssueFromDate, IssueToDate;
        public string VFIN = "32";
        private string TransType;
        private void FormRptVAT16_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();

            string vTollItemReceive = string.Empty;
            string v3Digits = string.Empty;

            vTollItemReceive = commonDal.settingsDesktop("TollItemReceive", "AttachedWithVAT6_1", null, connVM);
            v3Digits = commonDal.settingsDesktop("VAT6_1", "Report3Digits", null, connVM);
            if (vTollItemReceive == "Y")
            {
                IsTollRegister = true;
            }
            if (v3Digits == "Y")
            {
                Digits3 = true;
            }
            this.Text = "Report (VAT 6.1) Purchase Register";
            button1.Text = "VAT 6.1";


            #region Branch DropDown

            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranchName = new CommonDAL().ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranchName.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            if (SenderBranchId > 0)
            {
                cmbBranchName.SelectedValue = SenderBranchId;
            }
            else
            {
                cmbBranchName.SelectedValue = Program.BranchId;

            }
             
            #endregion

        }
        private void button1_Click(object sender, EventArgs e)//Print VAT16
        {
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //this.button1.Enabled = false;
                //this.btnPrev.Enabled = false;
                //this.progressBar1.Visible = true;

                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);


                PreviewOnly = false;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("Please select the product", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                ReportShowDs();
            }
            #region Catch
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
            #endregion Catch
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {

        }
        private void NullCheck()
        {
            try
            {
                if (dtpFromDate.Checked == false)
                {
                    IssueFromDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    IssueFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");// dtpFromDate.Value.ToString("yyyy-MMM-dd 00:00:00");
                }
                if (dtpToDate.Checked == false)
                {
                    IssueToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    IssueToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");// dtpToDate.Value.ToString("yyyy-MMM-dd 00:00:00");
                }
                TransType = txtTransType.Text;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "NullCheck", exMessage);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            #endregion Catch
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            ////MDIMainInterface mdi = new MDIMainInterface();
            //FormProductSearch frm = new FormProductSearch();
            //mdi.RollDetailsInfo(frm.VFIN);
            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            try
            {
                Program.fromOpen = "Other";
                Program.R_F = "";

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch
        }


        //===================================================
        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                #region Font Size

                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                #endregion

                #region Comments

                //this.button1.Enabled = false;
                //this.btnPrev.Enabled = false;
                //this.progressBar1.Visible = true;

                #endregion

                #region Value Assign

                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);

                PreviewOnly = true;

                #endregion

                #region Check Point

                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("Please select the product", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Show Report

                ReportShowDs();

                #endregion
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            #endregion Catch

        }

        private void ReportShowDs()
        {
            try
            {

                #region Check Point

                NullCheck();
                //string post1, post2 = string.Empty;
                if (PreviewOnly == true)
                {
                    post1 = "y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }

                #endregion

                #region Button Sats

                this.button1.Enabled = false;
                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker

                backgroundWorkerPreview.RunWorkerAsync();

                #endregion

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            #endregion Catch

        }

        #region backgroundWorkerPreview

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReportResult = new DataSet();
                VAT6_1ParamVM vm = new VAT6_1ParamVM
                {
                    ItemNo = txtItemNo.Text.Trim(),
                    StartDate = IssueFromDate,
                    EndDate = IssueToDate,
                    BranchId = BranchId,
                    Post1 = post1,
                    Post2 = post2
                };

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.VAT6_1Toll(txtItemNo.Text.Trim(), Program.CurrentUser, IssueFromDate,
                    IssueToDate, post1, post2, "", BranchId, connVM, true,null, null, vm);

                #region Comments - Sep-15-2020


                #endregion

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Variables and Objects

                CommonDAL _cDal = new CommonDAL();
                ProductDAL _pDal = new ProductDAL();
                ProductVM vProductVM = new ProductVM();
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                ReportDocument objrpt = new ReportDocument();

                List<VAT_16> vat16s = new List<VAT_16>();
                VAT_16 vat16 = new VAT_16();

                #endregion

                DataTable settingsDt = new DataTable();
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null,connVM);
                }

                #region Product Call

                
                vProductVM = _pDal.SelectAll(txtItemNo.Text,null,null,null,null,null,connVM).FirstOrDefault();

                if (vProductVM.ProductType.ToLower() == "service(nonstock)")
                {
                    IsRaw = vProductVM.ProductType.ToLower();
                }

                bool nonstock = false;
                if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead")
                {
                    nonstock = true;
                }

                #endregion

                #region Statement

                #region Check Point

                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Variables

                decimal vColumn1 = 0;
                DateTime vColumn2 = DateTime.MinValue;
                decimal vColumn3 = 0;
                decimal vColumn4 = 0;
                string vColumn5 = "-";
                DateTime vColumn6 = DateTime.MinValue;
                string vColumn7 = string.Empty;
                string vColumn8 = string.Empty;
                string vColumn9 = string.Empty;
                string vColumn10 = string.Empty;
                decimal vColumn11 = 0;
                decimal vColumn12 = 0;
                decimal vColumn13 = 0;
                decimal vColumn14 = 0;
                decimal vColumn15 = 0;
                decimal vColumn16 = 0;
                decimal vColumn17 = 0;
                decimal vColumn18 = 0;
                string vColumn19 = string.Empty;
                DateTime vTempStartDateTime = DateTime.MinValue;
                decimal vTempQuantity = 0;
                decimal vTempSubtotal = 0;
                string vTempVendorName = string.Empty;
                string vTempVATRegistrationNo = string.Empty;
                string vTempAddress1 = string.Empty;
                string vTempTransID = string.Empty;
                DateTime vTempInvoiceDateTime = DateTime.MinValue;
                string vTempProductName = string.Empty;
                string vTempBENumber = string.Empty;
                decimal vTempSDAmount = 0;
                decimal vTempVATAmount = 0;
                string vTempremarks = string.Empty;
                string vTemptransType = string.Empty;
                decimal vClosingQuantity = 0;
                decimal vClosingAmount = 0;
                decimal vClosingAvgRate = 0;
                decimal OpeningQty = 0;
                decimal OpeningAmnt = 0;
                decimal PurchaseQty = 0;
                decimal PurchaseAmnt = 0;
                decimal IssueQty = 0;
                decimal IssueAmnt = 0;
                decimal CloseQty = 0;
                decimal CloseAmnt = 0;
                decimal OpQty = 0;
                decimal OpAmnt = 0;
                decimal avgRate = 0;
                string HSCode = string.Empty;
                string ProductName = string.Empty;

                #endregion

                int i = 1;

                #region Opening Data

                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");
                foreach (DataRow row in DetailRawsOpening)
                {
                    vTempremarks = row["Remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();
                    ProductName = row["ProductName"].ToString().Trim();
                    HSCode = row["HSCodeNo"].ToString().Trim();

                    vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());

                    OpQty = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                    OpAmnt = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                    vat16 = new VAT_16();

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                    OpeningAmnt = OpAmnt;
                    OpAmnt = 0;
                    OpQty = 0;

                    PurchaseQty = 0;
                    PurchaseAmnt = 0;
                    IssueQty = 0;
                    IssueAmnt = 0;

                    CloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                         Convert.ToDecimal(IssueQty));
                    CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                 Convert.ToDecimal(IssueAmnt));
                    vColumn2 = vTempStartDateTime;
                    vColumn3 = OpeningQty;
                    vColumn4 = OpeningAmnt;
                    vColumn5 = "-";
                    vColumn6 = vTempStartDateTime;
                    vColumn7 = "-";
                    vColumn8 = "-";
                    vColumn9 = "-";
                    vColumn10 = "-";
                    vColumn11 = PurchaseQty;
                    vColumn12 = PurchaseAmnt;
                    vColumn13 = 0;
                    vColumn14 = 0;
                    vColumn15 = IssueQty;
                    vColumn16 = IssueAmnt;
                    vColumn17 = CloseQty;
                    vColumn18 = CloseAmnt;
                    vColumn19 = vTempremarks;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                        Convert.ToDecimal(IssueQty));
                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;


                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                          Convert.ToDecimal(IssueAmnt));
                        vClosingAvgRate = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));


                    }

                    #endregion

                    #region AssignValue to List
                    if (nonstock == true)
                    {

                        vColumn3 = 0;
                        vColumn4 = 0;
                        vColumn17 = 0;
                        vColumn18 = 0;
                    }
                    vat16.Column1 = vColumn1; //    i.ToString();      // Serial No   
                    vat16.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                    vat16.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                    vat16.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                    vat16.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                    vat16.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                    vat16.Column6String = ""; //    item["UnitCost"].ToString();      // Production Price
                    vat16.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                    vat16.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                    vat16.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                    vat16.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                    vat16.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                    vat16.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                    vat16.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                    vat16.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                    vat16.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                    vat16.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount
                    vat16.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                    vat16.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                    vat16.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks



                    vat16s.Add(vat16);
                    i = i + 1;

                    #endregion AssignValue to List


                }

                #endregion

                #region Details Data

                DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
                if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                {
                    MessageBox.Show("There is no data to preview", this.Text);
                    return;
                }

                //////string strSort = "CreateDateTime ASC, SerialNo ASC";
                string strSort = "StartDateTime ASC, SerialNo ASC";

                DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                dtview.Sort = strSort;
                DataTable dtsorted = dtview.ToTable();


                #region For Loop

                foreach (DataRow item in dtsorted.Rows)
                {
                    #region Get from Datatable

                    OpeningQty = 0;
                    OpeningAmnt = 0;
                    PurchaseQty = 0;
                    PurchaseAmnt = 0;
                    IssueQty = 0;
                    IssueAmnt = 0;
                    CloseQty = 0;
                    CloseAmnt = 0;

                    vColumn1 = i;
                    vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                    vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                    vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                    vTempVendorName = item["VendorName"].ToString(); // Customer Name
                    vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                    vTempAddress1 = item["Address1"].ToString(); // Customer Address
                    vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                    vTempInvoiceDateTime = Convert.ToDateTime(item["InvoiceDateTime"].ToString()); // Sale Invoice Date and Time
                    vTempBENumber = item["BENumber"].ToString(); // Sale Invoice Date and Time
                    vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                    vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                    vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                    vTempremarks = item["remarks"].ToString(); // Remarks
                    vTemptransType = item["TransType"].ToString().Trim();

                    #endregion Get from Datatable

                    if (vTemptransType == "Issue")
                    {
                        vat16 = new VAT_16();
                        #region if row 1 Opening
                        if (vTempremarks.Trim() == "ServiceNS"
                           || vTempremarks.Trim() == "ServiceNSImport"
                           )
                        {
                            OpeningQty = 0;
                            OpeningAmnt = 0;
                        }
                        else
                        {


                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;

                        }



                        OpAmnt = 0;
                        OpQty = 0;

                        PurchaseQty = 0;
                        PurchaseAmnt = 0;
                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            IssueQty = 0;
                            IssueAmnt = 0;
                        }
                        else
                        {
                            IssueQty = vTempQuantity;
                            IssueAmnt = vTempSubtotal;// vTempQuantity* vClosingAvgRate;
                        }

                        if (IssueQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = IssueAmnt / IssueQty;
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                             Convert.ToDecimal(IssueQty));
                        CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                     Convert.ToDecimal(IssueAmnt));

                        vColumn2 = vTempStartDateTime;

                        vColumn3 = OpeningQty;
                        vColumn4 = OpeningAmnt;

                        vColumn5 = "";
                        vColumn6 = vTempStartDateTime;

                        vColumn7 = vTempVendorName;
                        vColumn8 = vTempAddress1;
                        vColumn9 = vTempVATRegistrationNo;
                        vColumn10 = vTempProductName;
                        vColumn11 = PurchaseQty;
                        vColumn12 = PurchaseAmnt;
                        vColumn13 = vTempSDAmount;
                        vColumn14 = vTempVATAmount;
                        vColumn15 = IssueQty;
                        vColumn16 = IssueAmnt;

                        vColumn17 = CloseQty;
                        vColumn18 = CloseAmnt;

                        vColumn19 = vTempremarks;
                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                            Convert.ToDecimal(IssueQty));
                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            vClosingQuantity = 0;
                            vClosingAmount = 0;
                        }
                        else if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                              Convert.ToDecimal(IssueAmnt));

                        }
                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            vClosingQuantity = 0;
                            vClosingAmount = 0;

                        }

                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat16.Column1 = vColumn1; //   
                        vat16.Column2 = vColumn2; //   
                        vat16.Column3 = vColumn3; //   
                        vat16.Column4 = vColumn4; //   
                        vat16.Column5 = vColumn5; //   
                        vat16.Column6 = vColumn6; //   
                        vat16.Column6String = ""; //   
                        vat16.Column7 = vColumn7; //   
                        vat16.Column8 = vColumn8;//    
                        vat16.Column9 = vColumn9; //   
                        vat16.Column10 = vColumn10; // 
                        vat16.Column11 = vColumn11;//  
                        vat16.Column12 = vColumn12; // 
                        vat16.Column13 = vColumn13; // 
                        vat16.Column14 = vColumn14;//  
                        vat16.Column15 = vColumn15; // 
                        vat16.Column16 = vColumn16; // 
                        vat16.Column17 = vColumn17; // 
                        vat16.Column18 = vColumn18; // 
                        vat16.Column19 = vColumn19; // 


                        vat16s.Add(vat16);
                        i = i + 1;

                        #endregion AssignValue to List

                    }
                    else if (vTemptransType == "Purchase")
                    {
                        vat16 = new VAT_16();

                        #region if row 1 Opening

                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            OpeningQty = 0;
                            OpeningAmnt = 0;
                        }
                        else
                        {
                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;
                        }
                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        //if (vTempremarks.Trim() == "ServiceNS"
                        //  || vTempremarks.Trim() == "ServiceNSImport"
                        //  )
                        //{
                        //    PurchaseQty = 0;
                        //    PurchaseAmnt = 0;
                        //}
                        //else
                        //{
                        PurchaseQty = vTempQuantity;
                        PurchaseAmnt = vTempSubtotal;
                        //}

                        IssueQty = 0;
                        IssueAmnt = 0;
                        if (PurchaseQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = PurchaseAmnt / PurchaseQty;
                        }


                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            CloseQty = 0;
                            CloseAmnt = 0;
                        }
                        else
                        {
                            CloseQty =
                           (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                            Convert.ToDecimal(IssueQty));
                            CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                         Convert.ToDecimal(IssueAmnt));
                        }



                        vColumn2 = vTempStartDateTime;
                        vColumn3 = OpeningQty;
                        vColumn4 = OpeningAmnt;
                        vColumn5 = vTempBENumber;
                        vColumn6 = vTempInvoiceDateTime;
                        vColumn7 = vTempVendorName;
                        vColumn8 = vTempAddress1;
                        vColumn9 = vTempVATRegistrationNo;
                        vColumn10 = vTempProductName;
                        vColumn11 = PurchaseQty;
                        vColumn12 = PurchaseAmnt;
                        vColumn13 = vTempSDAmount;
                        vColumn14 = vTempVATAmount;
                        vColumn15 = IssueQty;
                        vColumn16 = IssueAmnt;
                        vColumn17 = CloseQty;
                        vColumn18 = CloseAmnt;
                        vColumn19 = vTempremarks;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                            Convert.ToDecimal(IssueQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;


                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                              Convert.ToDecimal(IssueAmnt));

                            vClosingAvgRate = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                        }

                        if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                        {
                            vClosingQuantity = 0;
                            vClosingAmount = 0;
                        }


                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat16.Column1 = vColumn1; //   
                        vat16.Column2 = vColumn2; //   
                        vat16.Column3 = vColumn3; //   
                        vat16.Column4 = vColumn4; //   
                        vat16.Column5 = vColumn5; //   
                        vat16.Column6 = vColumn6; //   
                        vat16.Column6String = Convert.ToDateTime(vColumn6).ToString("dd/MMM/yyyy"); //    item["UnitCost"].ToString();      // Production Price
                        vat16.Column7 = vColumn7; //   
                        vat16.Column8 = vColumn8;   // 
                        vat16.Column9 = vColumn9;   // 
                        vat16.Column10 = vColumn10; // 
                        vat16.Column11 = vColumn11; // 
                        vat16.Column12 = vColumn12; // 
                        vat16.Column13 = vColumn13; // 
                        vat16.Column14 = vColumn14; // 
                        vat16.Column15 = vColumn15; // 
                        vat16.Column16 = vColumn16; // 
                        vat16.Column17 = vColumn17; // 
                        vat16.Column18 = vColumn18; // 
                        vat16.Column19 = vColumn19; // 


                        vat16s.Add(vat16);
                        i = i + 1;

                        #endregion AssignValue to List
                    }
                }

                #endregion

                #endregion

                #region Report Preview

                #region Comments Sep-15-2020

                ////ReportClass objrpt = new ReportClass();
                //if (rbtnTollRegisterRaw.Checked)
                //{
                //    objrpt = new RptTollRegister();
                //}
                //else if (TransType == "TollReceiveRaw" && IsTollRegister == true)
                //{
                //    objrpt = new RptTollRegister();
                //}
                //else
                //{
                //if (Digits3)
                //{
                //    objrpt = new RptVAT16_NewDigit3();
                //}
                //else
                //{
                //    objrpt = new RptVAT16_New();
                ////if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                ////{
                ////    objrpt = new RptVAT6_1();
                ////}
                //}
                //}

                #endregion

                #region Report Control

                string NewRegister = _cDal.settingsDesktop("Toll", "NewRegister", settingsDt,connVM);
                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    if (NewRegister.ToLower() == "y")
                    {
                        objrpt = new RptVAT6_1_NewRegister();
                    }
                    else
                    {
                        objrpt = new RptVAT6_1();
                    }

                    ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_1.rpt");

                }

                #endregion

                #region Set DataSource

                objrpt.SetDataSource(vat16s);

                #endregion

                #region Preview Condition


                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                #endregion

                #region Comments - Sep-15-2020 - Product Call


                #endregion

                #region Settings Values

                string TollQuantity6_1 = _cDal.settingsDesktop("DecimalPlace", "TollQuantity6_1", null, connVM);
                string TollAmount6_1 = _cDal.settingsDesktop("DecimalPlace", "TollAmount6_1", null, connVM);

                #endregion

                #region Formula Fields
                

                objrpt.DataDefinition.FormulaFields["Quantity6_1"].Text = "'" + TollQuantity6_1 + "'";
                objrpt.DataDefinition.FormulaFields["Amount6_1"].Text = "'" + TollAmount6_1 + "'";

                objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Toll'";
                objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
                objrpt.DataDefinition.FormulaFields["ProductDescription"].Text = "'" + vProductVM.ProductDescription + "'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", "1");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductCode", vProductVM.ProductCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(IssueFromDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", "N");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(IssueFromDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(IssueToDate).ToString("dd/MM/yyyy"));


                #endregion

                #region Show Report

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

                #endregion preview

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                this.button1.Enabled = true;
                this.btnPrev.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }
       
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {

            txtProductName.Clear();
            //VATClient.FormClearing.ClearAllFormControls(this);
        }


        private string DBName = "";
        private string BranchName = "";

        private void btnVAT6_1_Click(object sender, EventArgs e)
        {

            #region Statement

            NullCheck();
            if (PreviewOnly == true)
            {
                post1 = "y";
                post2 = "N";
            }
            else
            {
                post1 = "Y";
                post2 = "Y";
            }

            DBName = cmbBranchName.SelectedValue.ToString();

            BranchName = cmbBranchName.Text;

            backgroundWorkerPreview.RunWorkerAsync();


            #endregion


        }

        private void cmbBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void labelBranch_Click(object sender, EventArgs e)
        {

        }

        //====================================================
    }

}
