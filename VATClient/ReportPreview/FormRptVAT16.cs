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
using SymphonySofttech.Reports;
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;

namespace VATClient.ReportPreview
{
    public partial class FormRptVAT16 : Form
    {
        public FormRptVAT16()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad(); 
			 
        }

        #region Global Declarations

        SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

        private string DBName = "";
        private string BranchName = "";

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        private bool PreviewOnly = false;
        private bool IsTollRegister = false;
        private bool Digits3 = false;
        private string IsRaw = "NA";

        private DataTable ProductResult;
        int ProductRowsCount = 0;
        private bool IsMonthly = false;


        private decimal UOMConversion = 1;
        private int BranchId = 0;
        public int SenderBranchId = 0;
        private string UOMFromParam = "";
        private string UOMToParam = "";

        #region Global Variables For BackGroundWorker

        private DataSet ReportResult;
        string post1, post2 = string.Empty;
        string InEnglish = string.Empty;

        #endregion

        private ReportDocument reportDocument = new ReportDocument();
        private DataTable uomResult;
        private string UOM = "";
        private string ItemNo = null;

        string IssueFromDate, IssueToDate;
        public string VFIN = "32";
        private string TransType;
        private bool DayEndProcessFlug = false;
        bool ProductLoadByTextChanged = true;
        public string FormNumeric = string.Empty;
        public string ValueDecimal = string.Empty;

        #endregion

        private void FormRptVAT16_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();
            try
            {


                #region Settings

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

                #region Preview button

                btnPrev.Visible = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM) == "Y";
                FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);
                cmbDecimal.Text = FormNumeric;
                #endregion

                #endregion

                #region Branch Load

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranchName = new CommonDAL().ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);


                if (SenderBranchId > 0)
                {
                    cmbBranchName.SelectedValue = SenderBranchId;
                }
                else
                {
                    cmbBranchName.SelectedValue = Program.BranchId;

                }

                #region cmbBranch DropDownWidth Change

                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
                cmbBranchName.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);

                #endregion

                

                #endregion

                #region UOM Load

                UOMToParam = string.Empty;
                UOMFromParam = txtUOM.Text.Trim();
                ItemNo = txtItemNo.Text;

                UomLoad();

                #endregion

                #region Product Type Load

                cmbProductType.Items.Clear();

                ProductDAL productTypeDal = new ProductDAL();
                cmbProductType.DataSource = productTypeDal.ProductTypeList;
                cmbProductType.SelectedIndex = -1;

                #endregion

                #region Button Stats

                this.Text = "Report (VAT 6.1) Purchase Register";
                button1.Text = "VAT 6.1";

                #endregion

                #region Date Month Control

                DateMonthControl();

                #endregion

                ProductLoad();
            }
            #region Catch

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

        private void button1_Click(object sender, EventArgs e)//Print VAT16
        {
            try
            {

                
               
                PreviewOnly = false;
               
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

        private void UomLoad()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ItemNo))
                {
                    cmbUOM.DataSource = null;
                    cmbUOM.Items.Clear();

                    IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                    string[] cvals = new string[] { UOMFromParam, UOMToParam, "Y" };
                    string[] cfields = new string[] { "UOMFrom", "UOMTo", "ActiveStatus like" };
                    uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);
                    uomResult.Rows.Add(0, txtUOM.Text, txtUOM.Text, 1);

                    cmbUOM.DataSource = uomResult;
                    cmbUOM.DisplayMember = "UOMTo";
                    cmbUOM.ValueMember = "UOMTo";
                    cmbUOM.Text = txtUOM.Text;
                    UOMToParam = cmbUOM.Text;

                    FindUOMConversuon();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void FindUOMConversuon()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtUOM.Text.Trim()))
                {

                    if (UOMFromParam.ToLower() != UOMToParam.ToLower())
                    {
                        IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                        string[] cvals = new string[] { UOMFromParam, UOMToParam, "Y" };
                        string[] cfields = new string[] { "UOMFrom", "UOMTo", "ActiveStatus like" };
                        uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);
                        if (uomResult != null && uomResult.Rows.Count > 0)
                        {
                            txtConvertion.Text = Program.ParseDecimalObject(uomResult.Rows[0]["Convertion"].ToString());
                            decimal UomConversion = Convert.ToDecimal(txtConvertion.Text.Trim());
                            UOMConversion = UomConversion;
                        }
                    }
                    else
                    {
                        UOMConversion = 1;
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
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
            SetValue_FormElement();

        }

        private void SetValue_FormElement()
        {

            try
            {

                ProductRowsCount = 0;

                DataGridViewRow selectedRow = new DataGridViewRow();


                if (dgvProduct != null && dgvProduct.Rows.Count > 0)
                {
                    selectedRow = dgvProduct.SelectedRows[0];
                    ProductRowsCount = 1;
                }
                else
                {
                    return;
                }


                txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//

            }
            #region Catch

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

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {

                PreviewOnly = true;
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
                SetValue_FormElement();

                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                FormNumeric = cmbDecimal.Text.Trim();
                ValueDecimal = cmbValue.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);

                //if (dtpMonth.Checked)
                //{
                //    MonthDate();
                //}
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (ProductRowsCount <= 0)
                {
                    MessageBox.Show("You Must Select Product First!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                UOM = cmbUOM.Text;
                NullCheck();
 
                #region Post Status

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

                #region Button Stats


                this.button1.Enabled = false;
                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Parmeter Assign (VAT 6.1)

                varVAT6_1ParamVM = new VAT6_1ParamVM();
                if (chkMultiple.Checked)
                {
                    if (rbtnProductType.Checked)
                    {
                        varVAT6_1ParamVM.ProdutType = cmbProductType.Text.Trim();
                        
                    }
                    else if (rbtnProductGroup.Checked)
                    {
                        varVAT6_1ParamVM.ProdutCategoryId = txtPGroupId.Text.Trim();
                    }

                }
                else
                {
                    varVAT6_1ParamVM.ItemNo = txtItemNo.Text.Trim();
                }
                varVAT6_1ParamVM.StartDate = IssueFromDate;
                varVAT6_1ParamVM.EndDate = IssueToDate;
                varVAT6_1ParamVM.Post1 = post1;
                varVAT6_1ParamVM.Post2 = post2;
                varVAT6_1ParamVM.BranchId = BranchId;
                varVAT6_1ParamVM.PreviewOnly = PreviewOnly;
                varVAT6_1ParamVM.InEnglish = InEnglish;
                varVAT6_1ParamVM.UOMConversion = UOMConversion;
                varVAT6_1ParamVM.UOM = UOM;
                varVAT6_1ParamVM.UOMTo = cmbUOM.Text;
                varVAT6_1ParamVM.UserName = "1";
                varVAT6_1ParamVM.ReportName = "";
                varVAT6_1ParamVM.Opening = false;
                varVAT6_1ParamVM.OpeningFromProduct = false;

                varVAT6_1ParamVM.IsMonthly = IsMonthly;
                varVAT6_1ParamVM.IsTopSheet = chkTopSheet.Checked;
                varVAT6_1ParamVM.IsGroupTopSheet = chkGroupTopSheet.Checked;

                varVAT6_1ParamVM.BranchWise = chkBranch.Checked;
                varVAT6_1ParamVM.DecimalPlace = FormNumeric;
                varVAT6_1ParamVM.DecimalPlaceValue = ValueDecimal;
                varVAT6_1ParamVM.FontSize = Convert.ToInt32(OrdinaryVATDesktop.FontSize);


                #endregion

                backgroundWorkerPreview.RunWorkerAsync();

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

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                CommonDAL commonDAl = new CommonDAL();
                string dayend = commonDAl.settings("DayEnd", "DayEndProcess");
                DayEndProcessFlug = false;

                if (!string.IsNullOrEmpty(dayend) && dayend == "Y")
                {
                    try
                    {
                        CommonDAL commonDal = new CommonDAL();
                        commonDal.CheckProcessFlag(varVAT6_1ParamVM.ItemNo, varVAT6_1ParamVM.ProdutCategoryId, varVAT6_1ParamVM.ProdutType);

                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);

                        FormRegularProcess formRegular = new FormRegularProcess();
                        formRegular.ShowDialog();
                        DayEndProcessFlug = true;
                        return;
                    }
                }
                
                ReportResult = new DataSet();

                NBRReports _report = new NBRReports();
                varVAT6_1ParamVM.UserId = Program.CurrentUserID;
                //ReportDocument tt = _report.VAT6_1_WithConn(varVAT6_1ParamVM);
                //reportDocument = new ReportDocument();
                varVAT6_1ParamVM.FromSP = false;
                reportDocument = _report.VAT6_1_WithConn(varVAT6_1ParamVM, connVM);



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
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }
                if (DayEndProcessFlug)
                {
                    return;
                }
                FormReport reports = new FormReport();

                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();
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
                this.button1.Enabled = true;
                this.btnPrev.Enabled = true;
                this.progressBar1.Visible = false;

                //if (reportDocument != null)
                //{
                //    reportDocument.Close();
                //    reportDocument.Dispose();
                //}
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
            ////txtProductName.Clear();
            //VATClient.FormClearing.ClearAllFormControls(this);
        }

        private void ClearAllFields()
        {
            try
            {
                cmbProductType.Text = "";
                txtItemNo.Text = "";
                txtPGroupId.Text = "";
                txtPGroup.Text = "";
                txtProductName.Text = "";
                txtProName.Text = "";
                txtProductCode.Text = "";
                dgvProduct.DataSource = null;

            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion Catch
        }

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

        private void chbInEnglish_Click(object sender, EventArgs e)
        {
            chbInEnglish.Text = "Bangla";
            if (chbInEnglish.Checked)
            {
                chbInEnglish.Text = "English";

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void cmbUOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                UOMToParam = cmbUOM.Text.Trim().ToLower();
                FindUOMConversuon();

                #endregion

            }

            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            #endregion Catch
        }

        private void cmbUOM_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                UOMToParam = cmbUOM.Text.Trim().ToLower();
                FindUOMConversuon();

                #endregion

            }

            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            #endregion Catch
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbProductType_Leave(object sender, EventArgs e)
        {
            ProductLoad();
        }

        private void ProductLoad()
        {
            #region try

            try
            {
                string ProductType = "";
                string ProductGroupId = "";
                string ProductCode = "";
                string ProductName = "";


                if (rbtnProductType.Checked)
                {
                    ProductType = OrdinaryVATDesktop.SanitizeInput(cmbProductType.Text);
                }
                if (rbtnProductGroup.Checked)
                {
                    ProductGroupId = OrdinaryVATDesktop.SanitizeInput(txtPGroupId.Text.Trim());
                }

                if (rbtnProductName.Checked)
                {
                    ProductName = OrdinaryVATDesktop.SanitizeInput(txtProName.Text.Trim());

                }
                if (rbtnProductCode.Checked)
                {
                    ProductCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());

                }

                if (!string.IsNullOrWhiteSpace(ProductType) || !string.IsNullOrWhiteSpace(ProductGroupId) || !string.IsNullOrWhiteSpace(ProductCode) || !string.IsNullOrWhiteSpace(ProductName))
                {
                    txtItemNo.Text = "";

                }


                if (!string.IsNullOrWhiteSpace(ProductType) || !string.IsNullOrWhiteSpace(ProductGroupId) || !string.IsNullOrWhiteSpace(txtItemNo.Text) || !string.IsNullOrWhiteSpace(ProductCode) || !string.IsNullOrWhiteSpace(ProductName))
                {
                    ProductDAL ProductDAL = new ProductDAL();

                    string[] cFields = { "Products.ItemNo", "ProductCategories.IsRaw", "ProductCategories.CategoryID", "Products.ProductCode like", "Products.ProductName like", "Products.ActiveStatus", "SelectTop" };
                    string[] cValues = { OrdinaryVATDesktop.SanitizeInput(txtItemNo.Text.Trim()), ProductType, ProductGroupId, ProductCode, ProductName, "Y", "All" };

                    ProductResult = ProductDAL.SelectProductDTAll(cFields, cValues,null,null,false,0,"","",null,connVM);

                    if (ProductResult != null && ProductResult.Rows.Count > 0)
                    {
                        ProductResult.Rows.RemoveAt(ProductResult.Rows.Count - 1);
                    }



                    DataTable dtSelected = new DataTable();
                    dtSelected = ProductResult.Copy();

                    DataView dv = new DataView(dtSelected);

                    dtSelected = dv.ToTable("", true
                        
                        , "ProductCode"
                        , "ProductName"
                        , "CategoryName"
                        , "IsRaw"
                        , "UOM"
                        , "NonStock"
                        , "ItemNo"
                        );


                    dgvProduct.DataSource = null;
                    dgvProduct.DataSource = dtSelected;

                    LRecordCount.Text = "Total Record Count :" + dgvProduct.Rows.Count;


                }


            }

            #endregion

            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductLoad", exMessage);
            }

            #endregion Catch
        }

        private void btnSearchProductGroup_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtPGroupId.Text = ProductCategoryInfo[0];
                    txtPGroup.Text = ProductCategoryInfo[1];

                    ProductLoad();

                }
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductGroup_Click", exMessage);
            }
            #endregion Catch
        }

        private void txtProName_TextChanged(object sender, EventArgs e)
        {
            if (ProductLoadByTextChanged)
            {
                ProductLoad();
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            if (ProductLoadByTextChanged)
            {
                ProductLoad();
            }
        }


        #region MISC Funtions

        private void DateMonthControl()
        {
            dtpMonth.Enabled = !rbtnDate.Checked;
            dtpMonth.Checked = !rbtnDate.Checked;
            dtpFromDate.Checked = rbtnDate.Checked;
            dtpToDate.Checked = rbtnDate.Checked;
            IsMonthly = !rbtnDate.Checked;

            if (IsMonthly == false)
            {
                dtpMonth.Value = dtpFromDate.Value;
            }

        }

        private void MonthDate()
        {
            if (dtpMonth.Checked == true)
            {

                DateTime Month = dtpMonth.Value;
                DateTime firstDayOfMonth = new DateTime(Month.Year, Month.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                dtpFromDate.Value = firstDayOfMonth;
                dtpToDate.Value = lastDayOfMonth;
                dtpFromDate.Checked = false;
                dtpToDate.Checked = false;
                IsMonthly = true;

                IssueFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");
                IssueToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");
            }

        }

        #endregion

        #region Product Control

        private void rbtnProductGroup_CheckedChanged(object sender, EventArgs e)
        {
            ProductType_GroupControl();
        }

        private void ProductType_GroupControl()
        {
            btnSearchProductGroup.Enabled = rbtnProductGroup.Checked;
            cmbProductType.Enabled = !rbtnProductGroup.Checked;
            txtPGroup.Enabled = rbtnProductGroup.Checked;

        }

        private void rbtnProductType_CheckedChanged(object sender, EventArgs e)
        {
            ProductType_GroupControl();
        }

        private void rbtnMonth_CheckedChanged(object sender, EventArgs e)
        {
            IsMonthly = !rbtnDate.Checked;

            //DateMonthControl();
        }

        private void dtpMonth_ValueChanged(object sender, EventArgs e)
        {
            MonthDate();
        }

        private void dtpMonth_Leave(object sender, EventArgs e)
        {
            MonthDate();
        }

        private void ProductGroupLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();


                string SqlText = @" SELECT CategoryID,CategoryName,IsRaw,ActiveStatus FROM ProductCategories  
WHERE  1=1 AND ActiveStatus = 'Y' and isnull(IsArchive,0) = 0 ";

                string[] shortColumnName = { "CategoryName" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtPGroupId.Text = selectedRow.Cells["CategoryID"].Value.ToString();//ProductInfo[1];
                    txtPGroup.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[1];
                    //////cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    ProductLoad();
                }

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductGroupLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtPGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductGroupLoad();
            }
        }

        private void txtPGroup_DoubleClick(object sender, EventArgs e)
        {
            ProductGroupLoad();
        }

        private void txtProName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                #region ProductSelect

                ProductSelect();

                #endregion

                UOMFromParam = txtUOM.Text.Trim();
                UOMToParam = string.Empty;

                UomLoad();
            }
        }

        private void ProductSelect()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y'  ";



                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtProductCode.Clear();
                    txtProName.Clear();

                    ProductLoadByTextChanged = false;

                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    ItemNo = selectedRow.Cells["ItemNo"].Value.ToString();

                    ProductLoad();

                    txtProductCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();
                    txtProName.Text = selectedRow.Cells["ProductName"].Value.ToString();
                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();

                    ProductLoadByTextChanged = true;

                }



            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductGroupLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtProName_DoubleClick(object sender, EventArgs e)
        {
            ProductSelect();

            UOMFromParam = txtUOM.Text.Trim();
            UOMToParam = string.Empty;

            UomLoad();
        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductSelect();
                UOMFromParam = txtUOM.Text.Trim();
                UOMToParam = string.Empty;

                UomLoad();
            }

        }

        private void txtProductCode_DoubleClick(object sender, EventArgs e)
        {
            ProductSelect();
            UOMFromParam = txtUOM.Text.Trim();
            UOMToParam = string.Empty;

            UomLoad();

        }

        private void rbtnProductName_CheckedChanged(object sender, EventArgs e)
        {
            txtProName.Enabled = rbtnProductName.Checked;

        }

        private void rbtnProductCode_CheckedChanged(object sender, EventArgs e)
        {
            txtProductCode.Enabled = rbtnProductCode.Checked;

        }

        #endregion

        private void dgvProduct_Click(object sender, EventArgs e)
        {
            try
            {
                ProductRowsCount = 0;

                DataGridViewRow selectedRow = new DataGridViewRow();


                if (dgvProduct != null && dgvProduct.Rows.Count > 0)
                {
                    selectedRow = dgvProduct.SelectedRows[0];
                    ProductRowsCount = 1;
                }
                else
                {
                    return;
                }


                txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//
                txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
                ItemNo = selectedRow.Cells["ItemNo"].Value.ToString();

                UOMFromParam = txtUOM.Text.Trim();
                UOMToParam = string.Empty;

                UomLoad();
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "dgvProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void chkMultiple_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chbInEnglish_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkMultiple_Click(object sender, EventArgs e)
        {
            chkMultiple.Text = "Single";
            if (chkMultiple.Checked)
            {
                chkMultiple.Text = "Multiple";

            }
        }

        private void txtPGroup_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }

}
