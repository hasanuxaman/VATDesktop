using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.List;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATServer.License;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormRptVAT17Toll : Form
    {
        public FormRptVAT17Toll()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        string IssueFromDate, IssueToDate = string.Empty;
        string IssueFromDate0, IssueToDate0;
        private bool PreviewOnly = false;
        private bool AutoAdjustment = false;
        private string IsRaw = "NA";
        private bool TollProduct = false;
        private int BranchId = 0;

        public int SenderBranchId = 0;



        #region Global Variables For BackGroundWorker

        private string vSalePlaceTaka, vSalePlaceQty, vSalePlaceDollar, vSalePlaceRate, vAutoAdjustment;
        private int SalePlaceTaka, SalePlaceQty, SalePlaceDollar, SalePlaceRate = 0;

        private DataSet ReportResult;
        string post1, post2 = string.Empty;
        private decimal TCloseQty = 0;
        private decimal TCloseAmnt = 0;

        ReportDSDAL reportDsdal = new ReportDSDAL();
        #endregion

        private void FormRptVAT17_Load(object sender, EventArgs e)
        {


            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

            cmbBranch.SelectedValue = SenderBranchId;

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion


            CommonDAL commonDal = new CommonDAL();
            vSalePlaceTaka = commonDal.settingsDesktop("Sale", "TakaDecimalPlace", null, connVM);
            SalePlaceTaka = Convert.ToInt32(vSalePlaceTaka);
            vSalePlaceQty = commonDal.settingsDesktop("Sale", "QuantityDecimalPlace", null, connVM);
            SalePlaceQty = Convert.ToInt32(vSalePlaceQty);
            vSalePlaceDollar = commonDal.settingsDesktop("Sale", "DollerDecimalPlace", null, connVM);
            SalePlaceDollar = Convert.ToInt32(vSalePlaceDollar);
            vSalePlaceRate = commonDal.settingsDesktop("Sale", "RateDecimalPlace", null, connVM);
            SalePlaceRate = Convert.ToInt32(vSalePlaceRate);

            vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);
            AutoAdjustment = Convert.ToBoolean(vAutoAdjustment == "Y" ? true : false);
            this.Text = "Report (VAT 6.2) Sales Register";
            button1.Text = "VAT 6.2";

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";
                Program.R_F = "";
                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//
                    // Done by Ruba
                    IsRaw = selectedRow.Cells["IsRaw"].Value.ToString();
                    if (IsRaw == "Raw" || IsRaw == "Pack" || IsRaw == "WIP") //Raw=1,Pack=2,WIP=6
                    {
                        rbtnWIP.Checked = true;
                    }
                    else
                    {
                        rbtnWIP.Checked = false;
                    }
                    if (selectedRow.Cells["NonStock"].Value.ToString() == "Y")//22
                    {
                        rbtnService.Checked = true;
                    }
                    else
                    {
                        rbtnService.Checked = false;
                    }



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

        private void NullCheck()
        {
            try
            {

                if (dtpFromDate.Checked == false)
                {
                    IssueFromDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");// Convert.ToDateTime(dtpFromDate.MinDate.ToString("yyyy-MMM-dd"));
                    //IssueFromDate0 = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    IssueFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");// dtpFromDate.Value;
                    //IssueFromDate0 = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                if (dtpToDate.Checked == false)
                {
                    IssueToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");//  Convert.ToDateTime(dtpToDate.MaxDate.ToString("yyyy-MMM-dd"));
                    //IssueToDate0 = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    IssueToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");//  dtpToDate.Value;
                    //IssueToDate0 = dtpFromDate.Value.AddDays(-1).ToString("yyyy-MMM-dd");//  dtpToDate.Value;
                }

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

        private void button1_Click(object sender, EventArgs e)
        {
            //this.btnPrev.Enabled = false;
            //this.progressBar1.Visible = true;
            //this.button1.Enabled = false;
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);

                PreviewOnly = false;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("Please select the Product", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                //var tt = rbtnService.Checked;
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

        //===========================================================
        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //this.btnPrev.Enabled = false;
                //this.progressBar1.Visible = true;
                //this.button1.Enabled = false;

                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);

                PreviewOnly = true;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("Please Select the Product", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                ReportShowDs();
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


                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;
                this.button1.Enabled = false;

                backgroundWorkerPreview.RunWorkerAsync();


                #region Complete
                //if (ReportResult.Tables.Count <= 0)

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

                VAT6_2ParamVM vm = new VAT6_2ParamVM
                {
                    ItemNo = txtItemNo.Text.Trim(),
                    StartDate = IssueFromDate,
                    EndDate = IssueToDate,
                    BranchId = BranchId,
                    Post1 = post1,
                    Post2 = post2
                };

                ReportResult = reportDsdal.VAT6_2Toll(txtItemNo.Text.Trim(), IssueFromDate, IssueToDate, post1, post2,
                    BranchId, connVM, null,null,vm,false);

                #region Comments Sep-15-2020

                #endregion

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void temp17()
        {
            #region Statement

            // Start Complete


            #region
            decimal vColumn1 = 0;
            DateTime vColumn2 = DateTime.MinValue;
            decimal vColumn3 = 0;
            decimal vColumn4 = 0;
            decimal vColumn5 = 0;
            decimal vColumn6 = 0;
            string vColumn7 = string.Empty;
            string vColumn8 = string.Empty;
            string vColumn9 = string.Empty;
            string vColumn10 = string.Empty;
            DateTime vColumn11 = DateTime.MinValue;
            string vColumn12 = string.Empty;
            decimal vColumn13 = 0;
            decimal vColumn14 = 0;
            decimal vColumn15 = 0;
            decimal vColumn16 = 0;
            decimal vColumn17 = 0;
            decimal vColumn18 = 0;
            string vColumn19 = string.Empty;

            decimal vTempSerial = 0;
            DateTime vTempStartDateTime = DateTime.MinValue;
            decimal vTempStartingQuantity = 0;
            decimal vTempStartingAmount = 0;
            decimal vTempQuantity = 0;
            decimal vTempSubtotal = 0;
            string vTempCustomerName = string.Empty;
            string vTempVATRegistrationNo = string.Empty;
            string vTempAddress1 = string.Empty;
            string vTempTransID = string.Empty;
            DateTime vTemptransDate = DateTime.MinValue;
            string vTempProductName = string.Empty;
            decimal vTempSDAmount = 0;
            decimal vTempVATAmount = 0;
            string vTempremarks = string.Empty;
            string vTemptransType = string.Empty;

            decimal vClosingQuantity = 0;
            decimal vClosingAmount = 0;
            decimal vClosingAvgRatet = 0;

            decimal OpeningQty = 0;
            decimal OpeningAmnt = 0;
            decimal ProductionQty = 0;
            decimal ProductionAmnt = 0;
            decimal SaleQty = 0;
            decimal SaleAmnt = 0;


            decimal OpQty = 0;
            decimal OpAmnt = 0;
            decimal avgRate = 0;
            string HSCode = string.Empty;
            string ProductName = string.Empty;



            #endregion


            int i = 1;

            if (rbtnWIP.Checked == false)
            {
                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");
                #region Opening
                foreach (DataRow row in DetailRawsOpening)
                {
                    vTempremarks = row["remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();
                    ProductName = row["ProductName"].ToString().Trim();
                    HSCode = row["HSCodeNo"].ToString().Trim();

                    vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());

                    OpQty = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                    OpAmnt = Convert.ToDecimal(row["UnitCost"].ToString().Trim());

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                    OpeningAmnt = OpAmnt;
                    OpAmnt = 0;
                    OpQty = 0;

                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = 0;
                    SaleAmnt = 0;

                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                         Convert.ToDecimal(SaleQty));
                    TCloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                 Convert.ToDecimal(SaleAmnt));
                    vColumn2 = vTempStartDateTime;
                    vColumn3 = OpeningQty;
                    vColumn4 = OpeningAmnt;
                    vColumn5 = ProductionQty;
                    vColumn6 = ProductionAmnt;
                    vColumn7 = "-";
                    vColumn8 = "-";
                    vColumn9 = "-";
                    vColumn10 = "-";
                    vColumn11 = DateTime.MinValue;
                    vColumn12 = "-";
                    vColumn13 = SaleQty;
                    vColumn14 = SaleAmnt;
                    vColumn15 = 0;
                    vColumn16 = 0;
                    vColumn17 = TCloseQty;
                    vColumn18 = TCloseAmnt;
                    vColumn19 = vTempremarks;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                        Convert.ToDecimal(SaleQty));
                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;
                        vClosingAvgRatet = 0;

                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                          Convert.ToDecimal(SaleAmnt));
                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                    }
                    vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                    #endregion


                    #region AssignValue to List


                    i = i + 1;

                    #endregion AssignValue to List
                }



                #endregion Opening
            }


            DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
            if (DetailRawsOthers == null || !DetailRawsOthers.Any())
            {
                //MessageBox.Show("There is no data to preview", this.Text);
                return;
            }
            string strSort = "StartDateTime ASC, SerialNo ASC";

            DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
            dtview.Sort = strSort;
            DataTable dtsorted = dtview.ToTable();

            foreach (DataRow item in dtsorted.Rows)
            {
                #region Get from Datatable

                OpeningQty = 0;
                OpeningAmnt = 0;
                ProductionQty = 0;
                ProductionAmnt = 0;
                SaleQty = 0;
                SaleAmnt = 0;
                TCloseQty = 0;
                TCloseAmnt = 0;

                vColumn1 = i;
                vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                vTempStartingQuantity = Convert.ToDecimal(item["StartingQuantity"].ToString()); // Opening Quantity
                vTempStartingAmount = Convert.ToDecimal(item["StartingAmount"].ToString()); // Opening Price
                vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                vTempCustomerName = item["CustomerName"].ToString(); // Customer Name
                vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                vTempAddress1 = item["Address1"].ToString(); // Customer Address
                vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                vTemptransDate = Convert.ToDateTime(item["StartDateTime"].ToString()); // Sale Invoice Date and Time
                vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                vTempremarks = item["remarks"].ToString(); // Remarks
                vTemptransType = item["TransType"].ToString().Trim();

                #endregion Get from Datatable

                if (vTemptransType.Trim() == "Sale")
                {
                    #region if row 1 Opening

                    OpeningQty = OpQty + vClosingQuantity;
                    OpeningAmnt = OpAmnt + vClosingAmount;
                    OpAmnt = 0;
                    OpQty = 0;

                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = vTempQuantity;
                    if (vTempremarks.Trim() == "TradingTender"
                        || vTempremarks.Trim() == "ExportTradingTender"
                        || vTempremarks.Trim() == "ExportTender"
                        || vTempremarks.Trim() == "Tender"
                        || vTempremarks.Trim() == "Tender"
                        )
                    {
                        SaleAmnt = vTempQuantity * vClosingAvgRatet;
                        SaleAmnt = vTempSubtotal;

                    }
                    else
                    {
                        SaleAmnt = vTempSubtotal;
                    }
                    SaleAmnt = Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                    if (SaleQty == 0)
                    {
                        avgRate = 0;
                    }
                    else
                    {
                        avgRate = SaleAmnt / SaleQty;
                    }
                    avgRate = Convert.ToDecimal(Program.FormatingNumeric(avgRate.ToString(), SalePlaceRate));
                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                         Convert.ToDecimal(SaleQty));
                    TCloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                 Convert.ToDecimal(SaleAmnt));
                    vColumn2 = vTempStartDateTime;
                    if (vTempremarks.Trim() == "ServiceNS"
                        || vTempremarks.Trim() == "ExportServiceNS"
                        )
                    {
                        vColumn3 = 0;
                        vColumn4 = 0;
                        vColumn5 = 0;
                        vColumn6 = 0;
                    }
                    else
                    {
                        vColumn3 = Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                        vColumn4 = Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));

                        vColumn5 = Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                        vColumn6 = Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                    }
                    vColumn7 = vTempCustomerName;
                    vColumn8 = vTempVATRegistrationNo;
                    vColumn9 = vTempAddress1;
                    vColumn10 = vTempTransID;
                    vColumn11 = vTemptransDate;
                    vColumn12 = vTempProductName;
                    vColumn13 = Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                    vColumn14 = Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                    vColumn15 = vTempSDAmount;
                    vColumn16 = vTempVATAmount;
                    if (vTempremarks == "ServiceNS"
                        || vTempremarks == "ExportServiceNS"
                        )
                    {
                        vColumn17 = 0;
                        vColumn18 = 0;
                    }
                    else
                    {
                        vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), SalePlaceQty));
                        vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(TCloseAmnt.ToString(), SalePlaceTaka));
                    }
                    vColumn19 = vTempremarks;
                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                        Convert.ToDecimal(SaleQty));
                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;
                        vClosingAvgRatet = 0;
                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                          Convert.ToDecimal(SaleAmnt));
                        if (vTempremarks.Trim() == "TradingTender"
                        || vTempremarks.Trim() == "ExportTradingTender"
                        || vTempremarks.Trim() == "ExportTender"
                        || vTempremarks.Trim() == "Tender"
                        || vTempremarks.Trim() == "Tender"
                        )
                        {
                        }
                        else
                        {
                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                        }
                    }
                    vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                    #endregion
                    #region AssignValue to List


                    i = i + 1;

                    #endregion AssignValue to List

                    if (avgRate != vClosingAvgRatet)
                    {
                        decimal a = avgRate * vClosingQuantity;           //1950
                        decimal b = vClosingAvgRatet * vClosingQuantity; //1350
                        decimal c = b - a;
                        c = Convert.ToDecimal(Program.FormatingNumeric(c.ToString(), SalePlaceTaka));
                        #region if row 1 Opening

                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = 0;
                        ProductionAmnt = 0;
                        SaleQty = 0;
                        SaleAmnt = c;
                        if (SaleQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = SaleAmnt / SaleQty;
                        }
                        avgRate = Convert.ToDecimal(Program.FormatingNumeric(avgRate.ToString(), SalePlaceRate));
                        TCloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        TCloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                     Convert.ToDecimal(SaleAmnt));
                        vColumn2 = vTempStartDateTime;
                        vColumn3 = Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                        vColumn4 = Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                        vColumn5 = Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                        vColumn6 = Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                        vColumn7 = vTempCustomerName;
                        vColumn8 = vTempVATRegistrationNo;
                        vColumn9 = vTempAddress1;
                        vColumn10 = vTempTransID;
                        vColumn11 = vTemptransDate;
                        vColumn12 = vTempProductName;
                        vColumn13 = Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                        vColumn14 = Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                        vColumn15 = 0;
                        vColumn16 = 0;
                        vColumn17 = TCloseQty;
                        vColumn18 = TCloseAmnt;
                        vColumn19 = "SaleAdjustment";
                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                            vClosingAvgRatet = 0;
                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                              Convert.ToDecimal(SaleAmnt));
                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                        }
                        vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                        #endregion
                        #region AssignValue to List


                        i = i + 1;

                        #endregion AssignValue to List
                    }


                }
                else if (vTemptransType == "Receive")
                {

                    #region if row 1 Opening

                    OpeningQty = OpQty + vClosingQuantity;
                    OpeningAmnt = OpAmnt + vClosingAmount;
                    OpAmnt = 0;
                    OpQty = 0;

                    ProductionQty = vTempQuantity;
                    ProductionAmnt = vTempSubtotal;
                    SaleQty = 0;
                    SaleAmnt = 0;
                    if (ProductionQty == 0)
                    {
                        avgRate = 0;
                    }
                    else
                    {
                        avgRate = ProductionAmnt / ProductionQty;

                    }
                    avgRate = Convert.ToDecimal(Program.FormatingNumeric(avgRate.ToString(), SalePlaceRate));
                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                         Convert.ToDecimal(SaleQty));
                    TCloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                 Convert.ToDecimal(SaleAmnt));
                    vColumn2 = vTempStartDateTime;
                    vColumn3 = Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                    vColumn4 = Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                    vColumn5 = Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                    vColumn6 = Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                    vColumn7 = "-";
                    vColumn8 = "-";
                    vColumn9 = "-";
                    vColumn10 = "-";
                    vColumn11 = Convert.ToDateTime("1900/01/01");
                    vColumn12 = "-";
                    vColumn13 = 0;
                    vColumn14 = 0;
                    vColumn15 = 0;
                    vColumn16 = 0;
                    vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), SalePlaceQty));
                    vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(TCloseAmnt.ToString(), SalePlaceTaka));
                    vColumn19 = vTempremarks;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                        Convert.ToDecimal(SaleQty));
                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;
                        vClosingAvgRatet = 0;

                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                          Convert.ToDecimal(SaleAmnt));
                        if (vTempremarks.Trim() == "TradingTender"
                       || vTempremarks.Trim() == "ExportTradingTender"
                       || vTempremarks.Trim() == "ExportTender"
                       || vTempremarks.Trim() == "Tender"
                       || vTempremarks.Trim() == "Tender"
                       )
                        {
                        }
                        else
                        {
                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                Convert.ToDecimal(vClosingQuantity));
                        }

                    }
                    vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                    #endregion

                    #region AssignValue to List


                    i = i + 1;

                    #endregion AssignValue to List

                    if (avgRate != vClosingAvgRatet)
                    {

                        decimal a = avgRate * vClosingQuantity;         //7200
                        decimal b = vClosingAvgRatet * vClosingQuantity;//7300
                        decimal c = a - b;

                        if (vTempremarks.Trim() == "TradingTender"
                      || vTempremarks.Trim() == "ExportTradingTender"
                      || vTempremarks.Trim() == "ExportTender"
                      || vTempremarks.Trim() == "Tender"
                      || vTempremarks.Trim() == "Tender"
                      )
                        {
                            c = (vClosingAvgRatet - avgRate) * ProductionQty;
                        }
                        c = Convert.ToDecimal(Program.FormatingNumeric(c.ToString(), SalePlaceTaka));

                        #region if row 1 Opening

                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = 0;
                        ProductionAmnt = c;
                        SaleQty = 0;
                        SaleAmnt = 0;
                        TCloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        TCloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                     Convert.ToDecimal(SaleAmnt));
                        vColumn2 = vTempStartDateTime;
                        vColumn3 = Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                        vColumn4 = Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                        vColumn5 = Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                        vColumn6 = Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                        vColumn7 = "-";
                        vColumn8 = "-";
                        vColumn9 = "-";
                        vColumn10 = "-";
                        vColumn11 = Convert.ToDateTime("1900/01/01");
                        vColumn12 = "-";
                        vColumn13 = 0;
                        vColumn14 = 0;
                        vColumn15 = 0;
                        vColumn16 = 0;
                        vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), SalePlaceQty));
                        vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(TCloseAmnt.ToString(), SalePlaceTaka));
                        vColumn19 = "ProductionAdjustment";

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                            vClosingAvgRatet = 0;

                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                              Convert.ToDecimal(SaleAmnt));
                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                        }
                        vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                        #endregion

                        #region AssignValue to List


                        i = i + 1;

                        #endregion AssignValue to List
                    }

                }

            }
            #region Report preview

            // End Complete
            #endregion Report preview

            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Variables and Objects

            ProductVM vProductVM = new ProductVM();
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            List<VAT_17> vat17s = new List<VAT_17>();
            VAT_17 vat17 = new VAT_17();

            #endregion

            try
            {

                #region Data Call

                vProductVM = new ProductDAL().SelectAll(txtItemNo.Text,null,null,null,null,null,connVM).FirstOrDefault();
                
                if (vProductVM.ProductType.ToLower() == "service(nonstock)")
                {
                    IsRaw = vProductVM.ProductType.ToLower();
                }

                bool nonstock = false;

                if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" )
                {
                    nonstock = true;
                }

                #endregion

                #region Statement

                #region Check Point
                
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Variables

                decimal vColumn1 = 0;
                DateTime vColumn2 = DateTime.MinValue;
                decimal vColumn3 = 0;
                decimal vColumn4 = 0;
                decimal vColumn5 = 0;
                decimal vColumn6 = 0;
                string vColumn7 = string.Empty;
                string vColumn8 = string.Empty;
                string vColumn9 = string.Empty;
                string vColumn10 = string.Empty;
                DateTime vColumn11 = DateTime.MinValue;
                string vColumn12 = string.Empty;
                decimal vColumn13 = 0;
                decimal vColumn14 = 0;
                decimal vColumn15 = 0;
                decimal vColumn16 = 0;
                decimal vColumn17 = 0;
                decimal vColumn18 = 0;
                string vColumn19 = string.Empty;
                DateTime vTempStartDateTime = DateTime.MinValue;
                decimal vTempStartingQuantity = 0;
                decimal vTempStartingAmount = 0;
                decimal vTempQuantity = 0;
                decimal vTempSubtotal = 0;
                string vTempCustomerName = string.Empty;
                string vTempVATRegistrationNo = string.Empty;
                string vTempAddress1 = string.Empty;
                string vTempTransID = string.Empty;
                DateTime vTemptransDate = DateTime.MinValue;
                string vTempProductName = string.Empty;
                decimal vTempSDAmount = 0;
                decimal vTempVATAmount = 0;
                string vTempremarks = string.Empty;
                string vTemptransType = string.Empty;
                decimal vClosingQuantity = 0;
                decimal vClosingAmount = 0;
                decimal vClosingAvgRatet = 0;
                decimal OpeningQty = 0;
                decimal OpeningAmnt = 0;
                decimal ProductionQty = 0;
                decimal ProductionAmnt = 0;
                decimal SaleQty = 0;
                decimal SaleAmnt = 0;
                decimal CloseQty = 0;
                decimal CloseAmnt = 0;
                decimal OpQty = 0;
                decimal OpAmnt = 0;
                decimal avgRate = 0;
                string HSCode = string.Empty;
                string ProductName = string.Empty;
                decimal vClosingQuantityNew = 0;
                decimal vClosingAmountNew = 0;
                decimal vUnitRate = 0;
                decimal vAdjustmentValue = 0;

                #endregion

                int i = 1;

                #region Opening Data

                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");

                foreach (DataRow row in DetailRawsOpening)
                {
                    ProductDAL productDal = new ProductDAL();
                    vTempremarks = row["remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();
                    //vTemptransType = row["TransType"].ToString().Trim();
                    ProductName = row["ProductName"].ToString().Trim();
                    HSCode = row["HSCodeNo"].ToString().Trim();
                    string ItemNo = productDal.GetProductIdByName(ProductName,connVM);
                    vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());
                    decimal LastNBRPrice = productDal.GetLastNBRPriceFromBOM(ItemNo, Convert.ToDateTime(vTempStartDateTime).ToString("yyyy-MMM-dd"), null, null,"0",connVM);

                    //Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));

                    decimal q11 = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                    decimal q12 = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                    //OpQty = TCloseQty;//
                    //OpAmnt = TCloseAmnt;
                    OpQty = q11;//
                    OpAmnt = q12;//
                    vat17 = new VAT_17();

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                    OpeningAmnt = OpAmnt;// OpQty* LastNBRPrice;
                    OpAmnt = 0;
                    OpQty = 0;

                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = 0;
                    SaleAmnt = 0;

                    CloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                         Convert.ToDecimal(SaleQty));
                    CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                 Convert.ToDecimal(SaleAmnt));
                    vColumn2 = vTempStartDateTime;
                    vColumn3 = Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                    vColumn4 = Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                    vColumn5 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                    vColumn6 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                    vColumn7 = "-";
                    vColumn8 = "-";
                    vColumn9 = "-";
                    vColumn10 = "-";
                    vColumn11 = DateTime.MinValue;
                    vColumn12 = "-";
                    vColumn13 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                    vColumn14 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                    vColumn15 = 0;
                    vColumn16 = 0;
                    vColumn17 = Convert.ToDecimal(Program.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                    vColumn18 = Convert.ToDecimal(Program.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                    vColumn19 = vTempremarks;
                    vUnitRate = Convert.ToDecimal(row["UnitRate"].ToString().Trim());
                    vClosingQuantityNew = vColumn17;
                    vClosingAmountNew = vColumn18;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                        Convert.ToDecimal(SaleQty));

                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;
                        vClosingAvgRatet = 0;

                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                          Convert.ToDecimal(SaleAmnt));
                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                    }

                    #endregion

                    #region AssignValue to List
                    if (nonstock == true)
                    {
                        vColumn3 = 0;
                        vColumn4 = 0;
                        vColumn5 = 0;
                        vColumn6 = 0;
                        vColumn17 = 0;
                        vColumn18 = 0;
                    }

                    vat17.Column1 = i; //    i.ToString();      // Serial No   
                    vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date

                    vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                    vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                    vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                    vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                    vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                    vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                    vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                    vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                    vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                    vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                    vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                    vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                    vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                    vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                    vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                    vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                    vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                    vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks


                    vat17s.Add(vat17);
                    i = i + 1;

                    #endregion AssignValue to List

                }
                #endregion Opening

                #region Details Data

                DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
                if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                {
                    MessageBox.Show("There is no data to preview", this.Text);
                    return;
                }
                string strSort = "CreatedDateTime ASC, SerialNo ASC";

                DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                dtview.Sort = strSort;
                DataTable dtsorted = dtview.ToTable();

                #region Process / For Loop

                foreach (DataRow item in dtsorted.Rows)
                {
                    #region Get from Datatable

                    OpeningQty = 0;
                    OpeningAmnt = 0;
                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = 0;
                    SaleAmnt = 0;
                    CloseQty = 0;
                    CloseAmnt = 0;

                    vColumn1 = i;
                    vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                    vTempStartingQuantity = Convert.ToDecimal(item["StartingQuantity"].ToString()); // Opening Quantity
                    vTempStartingAmount = Convert.ToDecimal(item["StartingAmount"].ToString()); // Opening Price
                    vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                    vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                    vTempCustomerName = item["CustomerName"].ToString(); // Customer Name
                    vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                    vTempAddress1 = item["Address1"].ToString(); // Customer Address
                    vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                    vTemptransDate = Convert.ToDateTime(item["StartDateTime"].ToString()); // Sale Invoice Date and Time
                    vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                    vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                    vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                    vTempremarks = item["remarks"].ToString(); // Remarks
                    vTemptransType = item["TransType"].ToString().Trim();

                    #region Bureau
                    if (Program.IsBureau == true)
                    {
                        ProductName = item["ProductName"].ToString().Trim();
                        HSCode = item["HSCodeNo"].ToString().Trim();
                    }

                    #endregion

                    #endregion Get from Datatable

                    if (vTemptransType.Trim() == "Sale")
                    {
                        vat17 = new VAT_17();
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

                        ProductionQty = 0;
                        ProductionAmnt = 0;
                        SaleQty = vTempQuantity;
                        if (vTempremarks.Trim() == "TradingTender"
                            || vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Tender"
                            || vTempremarks.Trim() == "Export"
                            )
                        {
                            //SaleAmnt = vTempQuantity * vClosingAvgRatet;
                            SaleAmnt = vTempSubtotal;

                        }
                        else
                        {
                            SaleAmnt = vTempSubtotal;
                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        { }
                        else
                        {
                            SaleAmnt = Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                        }


                        if (SaleQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = SaleAmnt / SaleQty;

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        { }
                        else
                        {
                            avgRate = Convert.ToDecimal(Program.FormatingNumeric(avgRate.ToString(), SalePlaceRate));
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                        vColumn2 = vTempStartDateTime;

                        string returnTransType = reportDsdal.GetReturnType(txtItemNo.Text.Trim(), vTempremarks.Trim(),connVM);

                        if (vTempremarks.Trim() == "ServiceNS"
                            || vTempremarks.Trim() == "ExportServiceNS"
                            || returnTransType == "ServiceNS"
                            )
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                        }
                        else
                        {
                            vColumn3 = OpeningQty;// vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                            vColumn4 = OpeningAmnt;// vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));

                            vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                            vColumn6 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                        }
                        vColumn7 = vTempCustomerName;
                        vColumn8 = vTempVATRegistrationNo;
                        vColumn9 = vTempAddress1;
                        vColumn10 = vTempTransID;
                        vColumn11 = vTemptransDate;
                        vColumn12 = vTempProductName;
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        {
                            vColumn13 = SaleQty;
                            vColumn14 = SaleAmnt;
                        }
                        else
                        {
                            vColumn13 = Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                            vColumn14 = Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                        }
                        vColumn15 = vTempSDAmount;
                        vColumn16 = vTempVATAmount;
                        if (vTempremarks.Trim() == "ServiceNS"
                            || vTempremarks.Trim() == "ExportServiceNS"
                            || returnTransType == "ServiceNS"
                            )
                        {
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        else
                        {
                            vColumn17 = Convert.ToDecimal(Program.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            vColumn18 = Convert.ToDecimal(Program.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                            //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        }
                        vColumn19 = vTempremarks;
                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));

                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;
                        vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());

                        if (vClosingQuantity == 0)
                        {
                            //Change at 29-04-14
                            //vClosingAmount = 0;
                            //vClosingAvgRatet = 0;
                            vClosingAmount = CloseAmnt;

                        }
                        else
                        {
                            //vClosingAmount = vClosingQuantity * avgRate;
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            if (vTempremarks.Trim() == "TradingTender"
                            || vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Tender"
                            || vTempremarks.Trim() == "Export"
                            )
                            {

                            }
                            else
                            {
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                            }

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                        }
                        #endregion
                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat17.Column1 = i; //    i.ToString();      // Serial No   
                        vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                        vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat17.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat17.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                        vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat17.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat17.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                        vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                        if (vColumn18 != vColumn17 * vUnitRate)
                        {
                            //AutoAdjustment = true;
                            //vAdjustmentValue = vColumn18 - (vColumn17 * vUnitRate);
                            vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                        }

                        vat17s.Add(vat17);
                        i = i + 1;

                        #endregion AssignValue to List

                        //// Service related company has no need auto adjustment
                        if (Program.IsBureau == false)
                        {
                            if (avgRate == vClosingAvgRatet)
                            {
                                //vat17s.Add(vat17);
                            }
                            if (AutoAdjustment == true)
                            {
                                #region SaleAdjustment
                                //if (avgRate != vClosingAvgRatet)
                                if (vColumn18 != vColumn17 * vUnitRate)
                                {

                                    decimal a = 0;
                                    decimal b = 0;
                                    if (vClosingQuantity == 0)
                                    {
                                        a = avgRate;          //1950
                                        b = vClosingAvgRatet; //1350
                                    }
                                    else
                                    {
                                        a = avgRate * vClosingQuantity;           //1950
                                        b = vClosingAvgRatet * vClosingQuantity; //1350

                                    }

                                    decimal c = b - a;// Convert.ToDecimal(Program.FormatingNumeric(b.ToString(),1)) - Convert.ToDecimal(Program.FormatingNumeric(a.ToString(),1));
                                    c = Convert.ToDecimal(Program.FormatingNumeric(c.ToString(), SalePlaceTaka));
                                    //hide 0 value row
                                    if (c != 0)
                                    {
                                        VAT_17 vat17Adj = new VAT_17();
                                        #region if row 1 Opening

                                        OpeningQty = OpQty + vClosingQuantity;
                                        OpeningAmnt = OpAmnt + vClosingAmount;
                                        OpAmnt = 0;
                                        OpQty = 0;

                                        ProductionQty = 0;
                                        ProductionAmnt = 0;
                                        SaleQty = 0;
                                        if (vTempremarks.Trim() == "TradingTender"
                                   || vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Tender"
                                            || vTempremarks.Trim() == "Export"
                                   )
                                        {
                                            if (vClosingQuantity == 0)
                                            {

                                                //SaleAmnt = (avgRate - vClosingAvgRatet) * vTempQuantity;
                                                SaleAmnt = (vClosingAvgRatet - avgRate) * vTempQuantity;
                                            }
                                            else
                                            {
                                                SaleAmnt = (vTempQuantity * vClosingAvgRatet) - vTempSubtotal;

                                            }
                                        }
                                        else
                                        {
                                            //SaleAmnt=(avgRate* vClosingQuantity)-(vClosingAvgRatet * vClosingQuantity);  
                                            if (vClosingQuantity == 0)
                                            {
                                                SaleAmnt = c * SaleQty;
                                            }
                                            else
                                            {
                                                SaleAmnt = c;
                                            }
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                                || vTempremarks.Trim() == "ExportTender"
                                                || vTempremarks.Trim() == "Export"
                                        )
                                        {

                                        }
                                        else
                                        {
                                            SaleAmnt = Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                                        }


                                        //SaleAmnt = c;

                                        if (SaleQty == 0)
                                        {
                                            avgRate = 0;
                                        }
                                        else
                                        {
                                            avgRate = SaleAmnt / SaleQty;
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                             || vTempremarks.Trim() == "ExportTender"
                                             || vTempremarks.Trim() == "Export")
                                        {

                                        }
                                        else
                                        {
                                            avgRate = Convert.ToDecimal(Program.FormatingNumeric(avgRate.ToString(), SalePlaceRate));
                                        }
                                        CloseQty =
                                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                             Convert.ToDecimal(SaleQty));

                                        CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                        vColumn2 = vTempStartDateTime;
                                        vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                        vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                        vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                        vColumn6 = 0;//Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                        vColumn7 = vTempCustomerName;
                                        vColumn8 = vTempVATRegistrationNo;
                                        vColumn9 = vTempAddress1;
                                        vColumn10 = vTempTransID;
                                        vColumn11 = vTemptransDate;
                                        vColumn12 = vTempProductName;
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                            || vTempremarks.Trim() == "ExportTender"
                                            || vTempremarks.Trim() == "Export")
                                        {
                                            vColumn13 = 0;
                                            vColumn14 = vAdjustmentValue;//;
                                        }
                                        else
                                        {
                                            vColumn13 = 0;// Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                                            vColumn14 = Convert.ToDecimal(Program.FormatingNumeric(vAdjustmentValue.ToString(), SalePlaceTaka));
                                        }
                                        vColumn15 = 0;
                                        vColumn16 = 0;
                                        vColumn17 = Convert.ToDecimal(Program.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                        vColumn18 = Convert.ToDecimal(Program.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                                        vClosingQuantityNew = vColumn17;
                                        vClosingAmountNew = vColumn18;

                                        //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                        //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                        vColumn19 = "SaleAdjustment";
                                        //vClosingAmount = vClosingQuantity * avgRate;

                                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                                        if (vClosingQuantity == 0)
                                        {
                                            vClosingAmount = 0;
                                            vClosingAvgRatet = 0;
                                        }
                                        else
                                        {
                                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                                              Convert.ToDecimal(SaleAmnt));
                                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                        }

                                        #endregion
                                        #region AssignValue to List
                                        if (nonstock == true)
                                        {
                                            vColumn3 = 0;
                                            vColumn4 = 0;
                                            vColumn5 = 0;
                                            vColumn6 = 0;
                                            vColumn17 = 0;
                                            vColumn18 = 0;
                                        }
                                        vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                        vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                        vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                        vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                        vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                        vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                        vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                        vat17Adj.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                        vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                        vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                        vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                        vat17Adj.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                        vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                        vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                        vat17Adj.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                        vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                        vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount


                                        vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                        vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                        vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks


                                        //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                        //vat17s.Add(vat17);

                                        vat17Adj.Column4 = vat17.Column18;
                                        vat17s.Add(vat17Adj);
                                        //AutoAdjustment = false;

                                        i = i + 1;


                                        #endregion AssignValue to List
                                    }
                                }
                                #endregion SaleAdjustment
                            }

                        }
                    }
                    else if (vTemptransType == "Receive")
                    {
                        vat17 = new VAT_17();

                        #region if row 1 Opening

                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = vTempQuantity;
                        ProductionAmnt = vTempSubtotal;
                        SaleQty = 0;
                        SaleAmnt = 0;
                        if (ProductionQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = ProductionAmnt / ProductionQty;

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            avgRate = Convert.ToDecimal(Program.FormatingNumeric(avgRate.ToString(), SalePlaceRate));
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                        vColumn2 = vTempStartDateTime;
                        vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                        vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                        vColumn5 = Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                        vColumn6 = Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                        ////vColumn7 = "-";
                        ////vColumn8 = "-";
                        ////vColumn9 = "-";
                        ////vColumn10 = "-";
                        ////vColumn11 = Convert.ToDateTime("1900/01/01");

                        vColumn7 = vTempCustomerName;
                        vColumn8 = vTempVATRegistrationNo;
                        vColumn9 = vTempAddress1;
                        vColumn10 = vTempTransID;
                        vColumn11 = vTemptransDate;

                        vColumn12 = "-";
                        vColumn13 = 0;
                        vColumn14 = 0;
                        vColumn15 = 0;
                        vColumn16 = 0;
                        vColumn17 = Convert.ToDecimal(Program.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                        vColumn18 = Convert.ToDecimal(Program.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;
                        vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());
                        //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                        //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        vColumn19 = vTempremarks;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                            vClosingAvgRatet = 0;

                        }
                        else
                        {
                            //vClosingAmount = vClosingQuantity * avgRate;

                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            if (vTempremarks.Trim() == "TradingTender"
                           || vTempremarks.Trim() == "ExportTradingTender"
                           || vTempremarks.Trim() == "ExportTender"
                           || vTempremarks.Trim() == "Tender"
                           || vTempremarks.Trim() == "Export"
                           )
                            {
                                // change at 20150324 for Nita requierment
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                     Convert.ToDecimal(vClosingQuantity));
                            }
                            else
                            {
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                    Convert.ToDecimal(vClosingQuantity));
                            }

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender"
                                    || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            vClosingAvgRatet = Convert.ToDecimal(Program.FormatingNumeric(vClosingAvgRatet.ToString(), SalePlaceRate));
                        }
                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat17.Column1 = i; //    i.ToString();      // Serial No   
                        vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                        vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                        vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                        vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks


                        vat17s.Add(vat17);
                        i = i + 1;
                        if (vColumn18 != vColumn17 * vUnitRate)
                        {
                            //AutoAdjustment = true;
                            vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                        }

                        #endregion AssignValue to List

                        //if (avgRate == vClosingAvgRatet)
                        //{
                        //    vat17s.Add(vat17);
                        //}
                        if (AutoAdjustment == true)
                        {
                            #region ProductionAdjustment


                            //if (avgRate != vClosingAvgRatet)
                            if (vColumn18 != vColumn17 * vUnitRate)
                            {
                                //vColumn4
                                //vClosingAvgRatet = vColumn4 / vColumn3;
                                //decimal x = vColumn4 / vColumn3;
                                decimal a = avgRate * vClosingQuantity;         //7200
                                decimal b = vClosingAvgRatet * vClosingQuantity;//7300
                                decimal c = a - b;
                                //  b = x * vClosingQuantity;//7300
                                //c = a - b;
                                if (vTempremarks.Trim() == "TradingTender"
                              || vTempremarks.Trim() == "ExportTradingTender"
                              || vTempremarks.Trim() == "ExportTender"
                              || vTempremarks.Trim() == "Tender"
                              || vTempremarks.Trim() == "Export"
                              )
                                {
                                    c = (vClosingAvgRatet - avgRate) * ProductionQty;
                                }
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                       || vTempremarks.Trim() == "ExportTender"
                                       || vTempremarks.Trim() == "Export")
                                {

                                }
                                else
                                {
                                    c = Convert.ToDecimal(Program.FormatingNumeric(c.ToString(), SalePlaceTaka));
                                }
                                //hide 0 value row
                                if (c != 0)
                                {
                                    VAT_17 vat17Adj = new VAT_17();

                                    #region if row 1 Opening

                                    OpeningQty = OpQty + vClosingQuantity;
                                    OpeningAmnt = OpAmnt + vClosingAmount;
                                    OpAmnt = 0;
                                    OpQty = 0;

                                    ProductionQty = 0;
                                    ProductionAmnt = c;
                                    SaleQty = 0;
                                    SaleAmnt = 0;
                                    CloseQty =
                                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                         Convert.ToDecimal(SaleQty));
                                    CloseAmnt = avgRate * vClosingQuantity;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                    vColumn2 = vTempStartDateTime;
                                    vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                    vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                    vColumn5 = Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                    vColumn6 = vAdjustmentValue;// vColumn18 - vColumn4;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                    vColumn7 = "-";
                                    vColumn8 = "-";
                                    vColumn9 = "-";
                                    vColumn10 = "-";
                                    vColumn11 = Convert.ToDateTime("1900/01/01");
                                    vColumn12 = "-";
                                    vColumn13 = 0;
                                    vColumn14 = 0;
                                    vColumn15 = 0;
                                    vColumn16 = 0;
                                    vColumn17 = Convert.ToDecimal(Program.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                    vColumn18 = Convert.ToDecimal(Program.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), SalePlaceQty));// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                    vClosingQuantityNew = vColumn17;
                                    vClosingAmountNew = vColumn18;
                                    //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                    //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                    vColumn19 = "ProductionAdjustment";

                                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                        Convert.ToDecimal(SaleQty));
                                    if (vClosingQuantity == 0)
                                    {
                                        vClosingAmount = 0;
                                        vClosingAvgRatet = 0;

                                    }
                                    else
                                    {
                                        //vClosingAmount = vClosingQuantity * avgRate;
                                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                                    }

                                    #endregion

                                    #region AssignValue to List
                                    if (nonstock == true)
                                    {
                                        vColumn3 = 0;
                                        vColumn4 = 0;
                                        vColumn5 = 0;
                                        vColumn6 = 0;
                                        vColumn17 = 0;
                                        vColumn18 = 0;
                                    }
                                    vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                    vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                    vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                    vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                    vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                    vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                    vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                    vat17Adj.Column8 = vColumn8;
                                    //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                    vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                    vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                    vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                    vat17Adj.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                    vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                    vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                    vat17Adj.Column14 = vColumn14;
                                    //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                    vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                    vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                    vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                    vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                    vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks



                                    //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                    //vat17s.Add(vat17);
                                    //vat17Adj.Column4 = vat17.Column18;
                                    vat17s.Add(vat17Adj);
                                    i = i + 1;

                                    #endregion AssignValue to List
                                }
                            }
                            #endregion ProductionAdjustment
                        }

                    }

                }

                #endregion Process

                #endregion

                #region Report preview
                ReportDocument objrpt = new ReportDocument();
                CommonDAL commonDal = new CommonDAL();
                string v3Digits = commonDal.settingsDesktop("VAT6_2", "Report3Digits", null, connVM);
                string NewRegister = commonDal.settingsDesktop("Toll", "NewRegister", null, connVM);
                //if (v3Digits.ToLower() == "y")
                //{
                //    objrpt = new RptVAT17_NewDigit3();
                //}
                //else
                //{
                //    objrpt = new RptVAT17_New();
                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                  
                    objrpt = new ReportDocument();

                    //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_2.rpt");
                    if (NewRegister.ToLower() == "y")
                    {
                        objrpt = new RptVAT6_2_NewRegister();
                    }
                    else
                    {
                        objrpt = new RptVAT6_2();
                    
                    }

                }

                //}
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    //objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                if (rbtnService.Checked)
                {
                    //objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Service'";

                }
                else
                {
                    //  objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Others'";

                }
                if (TollProduct)
                {
                    objrpt.DataDefinition.FormulaFields["IsToll"].Text = "'Y'";

                }


                CommonDAL _cDal = new CommonDAL();
                string TollQuantity6_2 = _cDal.settingsDesktop("DecimalPlace", "TollQuantity6_2", null, connVM);
                string TollAmount6_2 = _cDal.settingsDesktop("DecimalPlace", "TollAmount6_2", null, connVM);

                objrpt.DataDefinition.FormulaFields["Quantity6_2"].Text = "'" + TollQuantity6_2 + "'";
                objrpt.DataDefinition.FormulaFields["Amount6_2"].Text = "'" + TollAmount6_2 + "'";




                objrpt.SetDataSource(vat17s);

                objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
                objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
                string ProductDescription = vProductVM.ProductDescription;//  
                objrpt.DataDefinition.FormulaFields["ProductDescription"].Text = "'" + ProductDescription + "'";
                // //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Toll'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                // //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                // objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                // objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                // //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(IssueFromDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", "N");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", "1");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(IssueFromDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(IssueToDate).ToString("dd/MM/yyyy"));

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete
                #endregion Report preview

                #endregion

            }

            #region catch

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
                this.btnPrev.Enabled = true;
                this.progressBar1.Visible = false;
                this.button1.Enabled = true;
            }

        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
        //============================================================



    }
}
