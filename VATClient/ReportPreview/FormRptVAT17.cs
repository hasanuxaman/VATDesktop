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
using SymphonySofttech.Reports;
using VATServer.Ordinary;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient.ReportPreview
{
    public partial class FormRptVAT17 : Form
    {
        public FormRptVAT17()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        #region Global Variables

        int ProductRowsCount = 0;

        SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

        string IssueFromDate, IssueToDate = string.Empty;
        private bool PreviewOnly = false;
        private bool AutoAdjustment = false;
        private string IsRaw = "NA";
        private int BranchId = 0;
        private ReportDocument reportDocument = new ReportDocument();
        private decimal UOMConversion = 1;
        public int SenderBranchId = 0;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string UOM = "";
        private string ItemNo = null;
        private bool IsMonthly = false;

        private DataTable ProductCategoryResult;
        private DataTable ProductResult;
        public string FormNumeric = string.Empty;
        public string comboDecimalValue = string.Empty;
        bool IsException = false;

        #region Global Variables For BackGroundWorker

        private string vSalePlaceTaka, vSalePlaceQty, vSalePlaceDollar, vSalePlaceRate, vAutoAdjustment;
        private int SalePlaceTaka, SalePlaceQty, SalePlaceDollar, SalePlaceRate = 0;

        private DataSet ReportResult;
        string post1, post2 = string.Empty;
        string InEnglish = string.Empty;
        private decimal TCloseQty = 0;
        private decimal TCloseAmnt = 0;
        private DataTable uomResult;


        ReportDSDAL reportDsdal = new ReportDSDAL();
        private bool ProductLoadByTextChanged = true;

        #endregion

        #endregion

        private void FormRptVAT17_Load(object sender, EventArgs e)
        {
            #region Branch Load

            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, true, connVM);

            if (SenderBranchId > 0)
            {
                cmbBranch.SelectedValue = SenderBranchId;
            }
            else
            {
                cmbBranch.SelectedValue = Program.BranchId;

            }

            #endregion



            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            string code = new CommonDAL().settingValue("DayEnd", "BigDataProcess",connVM);

            if (code.ToLower() == "y")
            {
                rbtnMonth.Checked = true;
            }


            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
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

            #region Settings

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

            #endregion

            #region Preview button

            btnPrev.Visible = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM) == "Y";
            FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);
            cmbDecimal.Text = FormNumeric;

            #endregion

            #region Button Text

            this.Text = "Report (VAT 6.2) Sales Register";
            btnPrint.Text = "VAT 6.2";

            #endregion

            #region Date Month Control

            DateMonthControl();

            #endregion


            ProductLoad();

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
            }
            catch (Exception ex)
            {

                throw ex;
            }
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
                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
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
                UOMFromParam = txtUOM.Text.Trim();
                UOMToParam = string.Empty;

                UomLoad();

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
                //if (dtpMonth.Checked)
                //{
                //    MonthDate();
                //}

                IsException = false;

                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);

                PreviewOnly = false;

                #region CheckPoint

                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //if (txtProductName.Text == "")
                //{
                //    MessageBox.Show("Please select the Product", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}

                #endregion

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
                //if (dtpMonth.Checked)
                //{
                //    MonthDate();
                //}

                IsException = false;

                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);

                PreviewOnly = true;

                #region CheckPoint

                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }


                #endregion
                UOM = cmbUOM.Text;

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

        private void ReportShowDs()
        {
            try
            {
                #region selecte Product Rows
                FormNumeric = cmbDecimal.Text.Trim();
                comboDecimalValue = cmbDecimalValue.Text.Trim();
                selectedRowLoad();

                if (ProductRowsCount <= 0 && !chkMultiple.Checked)
                {
                    MessageBox.Show("You Must Select Product First!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Check Point

                //if (txtProductName.Text == "")
                //{
                //    MessageBox.Show("Please Select the Product", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}

                UOM = cmbUOM.Text;

                NullCheck();

                //if (dtpMonth.Checked)
                //{
                //    MonthDate();
                //}

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

                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;
                this.btnPrint.Enabled = false;

                #endregion

                #region Parmeter Assign

                varVAT6_2ParamVM = new VAT6_2ParamVM();
                if (chkMultiple.Checked)
                {
                    if (rbtnProductType.Checked)
                    {
                        varVAT6_2ParamVM.ProdutType = cmbProductType.Text.Trim();

                    }
                    else if (rbtnProductGroup.Checked)
                    {
                        varVAT6_2ParamVM.ProdutCategoryId = txtPGroupId.Text.Trim();
                    }

                }
                else
                {
                    varVAT6_2ParamVM.ItemNo = txtItemNo.Text.Trim();
                }
                varVAT6_2ParamVM.StartDate = IssueFromDate;
                varVAT6_2ParamVM.EndDate = IssueToDate;
                varVAT6_2ParamVM.IsChecked = chkMultiple.Checked;
                varVAT6_2ParamVM.Post1 = post1;
                varVAT6_2ParamVM.Post2 = post2;
                varVAT6_2ParamVM.BranchId = BranchId;
                varVAT6_2ParamVM.rbtnService = rbtnService.Checked;
                varVAT6_2ParamVM.rbtnWIP = rbtnWIP.Checked;
                varVAT6_2ParamVM.UOMTo = cmbUOM.Text;

                varVAT6_2ParamVM.IsBureau = Program.IsBureau;
                varVAT6_2ParamVM.AutoAdjustment = AutoAdjustment;
                varVAT6_2ParamVM.PreviewOnly = PreviewOnly;
                varVAT6_2ParamVM.InEnglish = InEnglish;
                varVAT6_2ParamVM.UOMConversion = UOMConversion;

                varVAT6_2ParamVM.UOM = UOM.ToString() == "" ? txtUOM.Text : UOM;
                varVAT6_2ParamVM.IsMonthly = IsMonthly;
                varVAT6_2ParamVM.IsTopSheet = chkTopSheet.Checked;

                varVAT6_2ParamVM.UserId = Program.CurrentUserID;
                varVAT6_2ParamVM.BranchWise = chkBranch.Checked;
                varVAT6_2ParamVM.DecimalPlace = FormNumeric;
                varVAT6_2ParamVM.ValuePlace = comboDecimalValue;
                varVAT6_2ParamVM.FontSize =OrdinaryVATDesktop.FontSize;

                // item / type check/ category check

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

                CommonDAL commonDal = new CommonDAL();

                string dayend = commonDal.settings("DayEnd", "DayEndProcess");

                if (!string.IsNullOrEmpty(dayend) && dayend == "Y")
                {
                    try
                    {
                        commonDal.CheckProcessFlag(varVAT6_2ParamVM.ItemNo, varVAT6_2ParamVM.ProdutCategoryId,
                            varVAT6_2ParamVM.ProdutType);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);

                        FormRegularProcess formRegular = new FormRegularProcess();
                        formRegular.ShowDialog();
                        IsException = true;
                        return;
                    }
                }

                NBRReports _report = new NBRReports();
                varVAT6_2ParamVM.FromSP = false;
                reportDocument = _report.VAT6_2(varVAT6_2ParamVM);
                ////reportDocument = _report.VAT6_2_Multiple(varVAT6_2ParamVM);

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
                IsException = true;

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
                FormReport reports = new FormReport();

                if (!IsException)
                {
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnPrev.Enabled = true;
                this.progressBar1.Visible = false;
                this.btnPrint.Enabled = true;
            }

        }

        #endregion


        private void ClearAllFields()
        {
            try
            {
                cmbProductType.SelectedIndex = -1;
                cmbUOM.SelectedIndex = -1;
                txtItemNo.Text = "";
                txtPGroupId.Text = "";
                txtPGroup.Text = "";
                txtProductName.Text = "";
                txtProName.Text = "";
                txtProductCode.Text = "";
                txtUOM.Text = "";
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void chbInEnglish_CheckedChanged(object sender, EventArgs e)
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

        private void progressBar1_Click(object sender, EventArgs e)
        {

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

        private void rbtnMonth_CheckedChanged(object sender, EventArgs e)
        {
            IsMonthly = !rbtnDate.Checked;

            //DateMonthControl();

        }

        private void rbtnDate_CheckedChanged(object sender, EventArgs e)
        {
            //DateMonthControl();
        }


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

        private void dtpMonth_ValueChanged(object sender, EventArgs e)
        {
            MonthDate();
        }

        private void dtpMonth_Leave(object sender, EventArgs e)
        {
            MonthDate();
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
                else //if (result == ""){return;}else//if (result != "")
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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            #endregion Catch
        }

        private void cmbProductType_Leave(object sender, EventArgs e)
        {
            #region try

            try
            {
                //txtItemNo.Clear();

                ProductLoad();

                #region Product Group Search


                #endregion

                //productLoad();

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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }

            #endregion Catch


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
                    //
                }
                if (rbtnProductGroup.Checked)
                {
                    ProductGroupId = OrdinaryVATDesktop.SanitizeInput(txtPGroupId.Text);
                    //txtItemNo.Clear();

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

                ProductDAL ProductDAL = new ProductDAL();

                if (!string.IsNullOrWhiteSpace(ProductType) || !string.IsNullOrWhiteSpace(ProductGroupId) || !string.IsNullOrWhiteSpace(txtItemNo.Text) || !string.IsNullOrWhiteSpace(ProductCode) || !string.IsNullOrWhiteSpace(ProductName))
                {
                    string[] cFields = { "Products.ItemNo", "ProductCategories.IsRaw", "ProductCategories.CategoryID", "Products.ProductCode like", "Products.ProductName like", "Products.ActiveStatus", "SelectTop" };
                    string[] cValues = { OrdinaryVATDesktop.SanitizeInput(txtItemNo.Text.Trim()), ProductType, ProductGroupId, ProductCode, ProductName, "Y", "All" };

                    ProductResult = ProductDAL.SelectProductDTAll(cFields, cValues, null, null, false, 0, "", "", null, connVM);

                    if (ProductResult != null && ProductResult.Rows.Count > 0)
                    {
                        ProductResult.Rows.RemoveAt(ProductResult.Rows.Count - 1);

                    }

                    DataView view = new DataView(ProductResult);
                    // Create a new DataTable from the DataView with just the columns desired - and in the order desired
                    DataTable dtProductResult = view.ToTable("Selected", false, "ItemNo", "ProductCode", "ProductName", "ProductType", "ProductGroup", "UOM", "NonStock");

                    dgvProduct.DataSource = null;
                    dgvProduct.DataSource = dtProductResult;

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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }

            #endregion Catch
        }

        private void selectedRowLoad()
        {

            #region try

            try
            {
                ProductRowsCount = 0;

                DataGridViewRow row = new DataGridViewRow();

                if (dgvProduct != null && dgvProduct.Rows.Count > 0)
                {
                    row = dgvProduct.SelectedRows[0];
                    ProductRowsCount = 1;
                }
                else
                {
                    return;
                }


                ////foreach (DataGridViewRow row in dgvProduct.Rows)
                ////{

                txtItemNo.Text = row.Cells["ItemNo"].Value.ToString();
                txtProductName.Text = row.Cells["ProductName"].Value.ToString();
                IsRaw = row.Cells["ProductType"].Value.ToString();
                if (IsRaw == "Raw" || IsRaw == "Pack" || IsRaw == "WIP") //Raw=1,Pack=2,WIP=6
                {
                    rbtnWIP.Checked = true;
                }
                else
                {
                    rbtnWIP.Checked = false;
                }
                if (row.Cells["NonStock"].Value.ToString() == "Y")//22
                {
                    rbtnService.Checked = true;
                }
                else
                {
                    rbtnService.Checked = false;
                }

                txtUOM.Text = row.Cells["UOM"].Value.ToString();
                //txtPGroupId.Text = "";

                ////}

                if (ProductRowsCount <= 0)
                {
                    return;
                }

                //UOMFromParam = txtUOM.Text.Trim();
                //UOMToParam = string.Empty;

                //UomLoad();
                //FindUOMConversuon();

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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }

            #endregion Catch
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

        private void cmbProductGroup_Leave(object sender, EventArgs e)
        {
            #region try

            try
            {

                ProductLoad();

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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }

            #endregion Catch

        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            ////txtItemNo.Clear();
            if (ProductLoadByTextChanged)
            {
                ProductLoad();
            }

        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProName_TextChanged(object sender, EventArgs e)
        {
            ////txtItemNo.Clear();

            if (ProductLoadByTextChanged)
            {
                ProductLoad();
            }

        }

        private void rbtnProductGroup_CheckedChanged(object sender, EventArgs e)
        {
            btnSearchProductGroup.Enabled = rbtnProductGroup.Checked;
            txtPGroup.Enabled = rbtnProductGroup.Checked;
        }

        private void cmbProductType_TextChanged(object sender, EventArgs e)
        {
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private void txtPGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode.Equals(Keys.F9))
                {
                    //txtItemNo.Clear();

                    ProductGroupLoad();
                    //cmbProduct.Focus();
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }

            #endregion
        }

        private void rbtnProductType_CheckedChanged(object sender, EventArgs e)
        {
            cmbProductType.Enabled = rbtnProductType.Checked;

        }

        private void txtPGroup_DoubleClick(object sender, EventArgs e)
        {
            //txtItemNo.Clear();

            ProductGroupLoad();
        }

        private void txtProName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode.Equals(Keys.F9))
                {
                    //txtItemNo.Clear();

                    ProductSelect();
                    UOMFromParam = txtUOM.Text.Trim();
                    UOMToParam = string.Empty;

                    UomLoad();

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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }

            #endregion
        }

        private void txtProName_DoubleClick(object sender, EventArgs e)
        {
            //txtItemNo.Clear();

            ProductSelect();
            UOMFromParam = txtUOM.Text.Trim();
            UOMToParam = string.Empty;

            UomLoad();
        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode.Equals(Keys.F9))
                {
                    //txtItemNo.Clear();

                    ProductSelect();
                    //cmbProduct.Focus();
                    UOMFromParam = txtUOM.Text.Trim();
                    UOMToParam = string.Empty;

                    UomLoad();
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }

            #endregion
        }

        private void txtProductCode_DoubleClick(object sender, EventArgs e)
        {
            //txtItemNo.Clear();

            ProductSelect();
            UOMFromParam = txtUOM.Text.Trim();
            UOMToParam = string.Empty;

            UomLoad();
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

        private void rbtnProductName_CheckedChanged(object sender, EventArgs e)
        {
            txtProName.Enabled = rbtnProductName.Checked;

        }

        private void rbtnProductCode_CheckedChanged(object sender, EventArgs e)
        {
            txtProductCode.Enabled = rbtnProductCode.Checked;
        }

        private void chkMultiple_Click(object sender, EventArgs e)
        {
            chkMultiple.Text = "Single";
            if (chkMultiple.Checked)
            {
                chkMultiple.Text = "Multiple";

            }
        }


        //============================================================



    }
}
