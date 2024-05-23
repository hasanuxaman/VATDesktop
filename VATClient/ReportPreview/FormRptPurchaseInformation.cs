using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;

using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;

using VATServer.Library;

using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;
using VATViewModel.DTOs;
using VATServer.License;
using SymphonySofttech.Reports;
using VATServer.Ordinary;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using VATDesktop.Repo;
using VATServer.Interface;


namespace VATClient.ReportPages
{
    public partial class FormRptPurchaseInformation : Form
    {
        public FormRptPurchaseInformation()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }

		private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string transactionType = string.Empty;
        private string TransType = string.Empty;
        private string post = string.Empty;
        private string PurchaseType = string.Empty;
        private ReportDocument reportDocument = new ReportDocument(); 

        #region Global Variables For BackGroundWorker
        private bool ExcelRpt = false;

        private DataSet ReportResult;
        private DataSet ReportMIS;
        private string cmbPostText;
        //private string TransactionType;
        private string reportName;
        private int BranchId = 0;
        private string VatType = "";
        private string reporttype = "";
        public string FormNumeric = string.Empty;


        public int SenderBranchId = 0;

        #endregion


        string PurchaseFromDate, PurchaseToDate;

        #region Excel

        private DataTable ReportDataTable;
        private DataTable dtsorted;
        private DataGridView dgvSale;
        private Microsoft.Office.Interop.Excel.Application xlApp;
        private Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
        private object misValue = System.Reflection.Missing.Value;
        private string saveLocation;
        #endregion

         
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearAllFields()
        {
            try
            {
                txtVendorGroupID.Text = "";
                txtVendorGroupName.Text = "";

                txtInvoiceNo.Text = "";
                txtItemNo.Text = "";
                txtProductName.Text = "";
                txtPGroupId.Text = "";
                txtPGroup.Text = "";
                txtProductType.Text = "";
                txtProductTypeId.Text = "";
                txtVendorName.Text = "";
                txtVendorId.Text = "";
                cmbPost.SelectedIndex = -1;
                cmbPurchaseType.SelectedIndex = -1;
                dtpPurchaseFromDate.Value = Program.SessionDate;
                dtpPurchaseToDate.Value = Program.SessionDate;
                dtpPurchaseFromDate.Checked = false;
                dtpPurchaseToDate.Checked = false;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion Catch
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void NullCheck()
        {
            try
            {
                if (dtpPurchaseFromDate.Checked == false)
                {
                    PurchaseFromDate = "";
                }
                else
                {
                    PurchaseFromDate = dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd").ToString();// dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpPurchaseToDate.Checked == false)
                {
                    PurchaseToDate = "";
                }
                else
                {
                    PurchaseToDate = dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd").ToString();//  dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd");
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
        //No server Side Method
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                #region TransactionType

                DataGridViewRow selectedRow = null;

                string result;
                transType();
                //if (rbtnOther.Checked)
                //{
                selectedRow = FormPurchaseSearch.SelectOne(txtTransactionType.Text.Trim());
                //}
                //else if (rbtnTrading.Checked)
                //{
                //    selectedRow = FormPurchaseSearch.SelectOne("Trading"); //frm.rbtnTrading.Checked = true;
                //}
                //else if (rbtnIssueReturn.Checked)
                //{
                //    selectedRow = FormPurchaseSearch.SelectOne("IssueReturn"); //frm.rbtnIssueReturn.Checked = true;
                //}
                //else if (rbtnTollReceive.Checked)
                //{
                //    selectedRow = FormPurchaseSearch.SelectOne("TollReceive"); //frm.rbtnTollReceive.Checked = true;
                //}
                //else
                //{
                //    selectedRow = FormPurchaseSearch.SelectOne("-");
                //}

                #endregion TransactionType

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtInvoiceNo.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                    //txtVendorName.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    //reportName = selectedRow.Cells["TransType"].Value.ToString();

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

        //No server Side Method
        private void btnProduct_Click(object sender, EventArgs e)
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
                Program.fromOpen = "Me";
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
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            #endregion Catch

        }
        //No server Side Method
        private void btnVendor_Click(object sender, EventArgs e)
        {

            try
            {
                Program.fromOpen = "Me";

                DataGridViewRow selectedRow = new DataGridViewRow();
                
                selectedRow = FormVendorSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtVendorId.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                }

                //string result = FormVendorSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] VendorInfo = result.Split(FieldDelimeter.ToCharArray());
                //    txtVendorId.Text = VendorInfo[0];
                //    txtVendorName.Text = VendorInfo[1];
                //}
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVendor_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVendor_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVendor_Click", exMessage);
            }
            #endregion Catch
        }

        private void FormRptPurchaseInformation_Load(object sender, EventArgs e)
        {
            //RollDetailsInfo();
           
            #region Branch DropDown

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
            TypeName vname = new TypeName();
            cmbType.DataSource = vname.TypeNameList;
            FormMaker();
        }

        private void FormMaker()
        {
            try
            {
                transType();
                //VATTypeLoad();
                //this.Text = reportName;
                PTypeSearch();
                cmbReport.SelectedIndex = 0;

                #region Preview Only

                CommonDAL commonDal = new CommonDAL();

                string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);
                FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);
                cmbDecimal.Text = FormNumeric;

                if (PreviewOnly.ToLower() == "n")
                {
                    cmbPost.Text = "Y";
                    cmbPost.Visible = false;
                    label9.Visible = false;
                }

                #endregion

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormMaker", exMessage);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            #endregion Catch
        }

        private void PTypeSearch()
        {
            cmbProductType.Items.Clear();

            ProductDAL productTypeDal = new ProductDAL();
            cmbProductType.DataSource = productTypeDal.ProductTypeList;
            cmbProductType.SelectedIndex = -1;

        }
        //No server Side Method
        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            #region Try
            ExcelRpt = false;

            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                FormNumeric = cmbDecimal.Text.Trim();

                VatType =cmbType.Text;

                if (VatType=="All")
                {
                    VatType = "";
                }

                reporttype = cmbReport.Text.Trim();

                //Detail Summary Single Monthly
                if (Program.CheckLicence(dtpPurchaseToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                // cmbPostText = Convert.ToString(chkPost.Checked ? "Y" : "N");

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;

                NullCheck();
                transType();

                if (cmbReport.Text.Trim().ToLower() == "detail"
                    || cmbReport.Text.Trim().ToLower() == "summary"
                    || cmbReport.Text.Trim().ToLower() == "summarybyproduct"
                    || cmbReport.Text.Trim().ToLower() == "at"
                    )  //Detail   0
                {
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
                }
                
                else if (cmbReport.Text.Trim().ToLower() == "single")  //Single   2
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        MessageBox.Show("Please select the Purchase No", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                    transTypeZamil();
                    backgroundWorkerMIS.RunWorkerAsync();
                }
                else if (cmbReport.Text.Trim().ToLower() == "monthly") //Monthly  3
                {
                    if (dtpPurchaseFromDate.Checked == false)
                    {
                        MessageBox.Show("Please select start date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }
                    if (dtpPurchaseToDate.Checked == false)
                    {
                        MessageBox.Show("Please select end date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(txtVendorName.Text))
                    {
                        MessageBox.Show("Please select vendor.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }
                    backgroundWorkerMonth.RunWorkerAsync();
                }
                else if (cmbReport.Text.Trim().ToLower() == "tds")  //Detail   0
                {

                    string[] cFields = {    "h.PurchaseInvoiceNo like"
                                           ,"h.ReceiveDate>="
                                           ,"h.ReceiveDate<="         
                                           ,"h.Post"
                                           ,"h.VendorID"
                                           ,"h.BranchId"
                                          // ,"d.Type"

                                       };


                    string[] cValues = {    txtInvoiceNo.Text.Trim()
                                           ,PurchaseFromDate
                                           ,PurchaseToDate
                                           ,post
                                           ,txtVendorId.Text.Trim()
                                          ,BranchId.ToString()
                                         // ,VatType.ToString()
                                            };



                    new FormPurchase().TDSPreveiw(cFields, cValues, FormNumeric, BranchId);
                }



            }

            #endregion
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            #endregion Catch
            #region bad

 
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;

            }
        }



        #region Background Worker Events


        private void backgroundWorkerPreviewDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            ReportResult = null;
            ReportResult = new DataSet();
            MISReport _report = new MISReport();

            try
            {
                if (ExcelRpt)
                {

                    ReportDSDAL reportDsdal = new ReportDSDAL();

                    ReportResult = reportDsdal.PurchaseNew(txtInvoiceNo.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtVendorId.Text.Trim(), txtItemNo.Text.Trim(),
                         txtPGroupId.Text.Trim(), txtProductType.Text.Trim(), TransType,
                         post, PurchaseType, txtVendorGroupID.Text.Trim(), "N", "-", "-", 0, 0, 0, chkCategoryLike.Checked, txtPGroup.Text.Trim(), BranchId, VatType,"",connVM);
                }

                else
                {


                    reportDocument = _report.PurchaseNew(txtInvoiceNo.Text.Trim(), PurchaseFromDate, PurchaseToDate,
                        txtVendorId.Text.Trim(), txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), txtProductType.Text.Trim(), TransType,
                         post, PurchaseType, txtVendorGroupID.Text.Trim(), "N", "-", "-", 0, 0, 0, chkCategoryLike.Checked,
                         txtPGroup.Text.Trim(), BranchId, VatType, reporttype, reportName, txtProductName.Text.Trim(), txtVendorName.Text, chkLocal.Checked, "",FormNumeric ,connVM);
                }
            
            
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreviewDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
            DataTable settingsDt = new DataTable();

            try
            {
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }
                string CompanyCode = new CommonDAL().settingsDesktop("CompanyCode", "Code", settingsDt, connVM);

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                if (ExcelRpt)
                {
                    if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                    {
                        MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                    if (CompanyCode.ToLower() == "tk" && cmbReport.Text.ToLower() == "detail")
                    {
                        SaveExcelForTk(ReportResult);
                    }
                    else
                    {
                        SaveExcel(ReportResult);
                    }
                }
                else
                {
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }

              

                #region Old

                //DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //if (string.IsNullOrEmpty(reportName))
                //{
                //    reportName = "All";
                //}
                //ReportClass objrpt = new ReportClass();

                //ReportResult.Tables[0].TableName = "DsPurchase";
                //if (cmbReport.Text.Trim().ToLower() == "summary")
                //{
                //    objrpt = new RptPurchaseSummery();
                //    objrpt.SetDataSource(ReportResult);
                //    reportName = reportName + " (summary)";
                //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
                //}
                //else if (cmbReport.Text.Trim().ToLower() == "summarybyproduct")
                //{
                //    objrpt = new RptPurchaseSummeryByProduct();
                //    objrpt.SetDataSource(ReportResult);
                //    reportName = reportName + " (summary)";
                //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
                //}
                //else
                //{
                //    ReportResult.Tables[0].TableName = "DsPurchase";
                //    if (chkLocal.Checked == false)
                //    {
                //        objrpt = new RptPurchaseTransaction();
                //    }
                //    else if (chkLocal.Checked == true)
                //    {
                //        objrpt = new RptPurchaseTransaction_Duty();
                //    }
                //    objrpt.SetDataSource(ReportResult);
                //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
                //}


                ////}
                //#region Formulla
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";

                //if (txtProductName.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + txtProductName.Text.Trim() + "'  "; }

                //if (txtVendorName.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + txtVendorName.Text.Trim() + "'  "; }

                //if (txtInvoiceNo.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + txtInvoiceNo.Text.Trim() + "'  "; }

                //if (dtpPurchaseFromDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + dtpPurchaseFromDate.Value.ToString("dd/MMM/yyyy") + "'  "; }

                //if (dtpPurchaseToDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + dtpPurchaseToDate.Value.ToString("dd/MMM/yyyy") + "'  "; }

                //#endregion Formulla

                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

                //FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();

                #endregion

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               exMessage);
            }

            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        #endregion
        private void transType()
        {
            #region Transaction Type
            reportName = string.Empty;
            if (txtTransactionType.Text == "Other")
            {
                reportName = "Local Purchase";
            }
            else if (txtTransactionType.Text == "PurchaseDN")
            {
                reportName = "Purchase Debit Note";
            }
            else if (txtTransactionType.Text == "PurchaseCN")
            {
                reportName = "Purchase Credit Note";
            }
            else if (txtTransactionType.Text == "Trading")
            {
                reportName = "Trading Purchase";
            }
            else if (txtTransactionType.Text == "TradingImport")
            {
                reportName = "Trading Import Purchase";
            }

            else if (txtTransactionType.Text == "TollReceive")
            {
                reportName = "Toll Receive";
            }

            else if (txtTransactionType.Text == "PurchaseReturn")
            {
                reportName = "Purchase Return";
            }
            else if (txtTransactionType.Text == "InputService")
            {
                reportName = "Input Service Purchase";
            }
            else if (txtTransactionType.Text == "Service")
            {
                reportName = "Service Stock Purchase";
            }
            else if (txtTransactionType.Text == "ServiceNS")
            {
                reportName = "Service Service Purchase";
            }
            else if (txtTransactionType.Text == "Import")
            {
                reportName = "Import Purchase";
            }
            else if (txtTransactionType.Text == "TollReceiveRaw")
            {
                reportName = "Toll Receive Raw";
            }
            TransType = txtTransactionType.Text.Trim();
            post = cmbPost.Text.Trim();
            PurchaseType = cmbPurchaseType.Text.Trim();


            #endregion Transaction Type
        }
        private void transTypeZamil()
        {
            #region Transaction Type
            reportName = string.Empty;
            if (txtTransactionType.Text == "Other")
            {
                reportName = "Local Purchase";
            }
            else if (txtTransactionType.Text == "PurchaseDN")
            {
                reportName = "Purchase Debit Note";
            }
            else if (txtTransactionType.Text == "PurchaseCN")
            {
                reportName = "Purchase Credit Note";
            }
            else if (txtTransactionType.Text == "Trading")
            {
                reportName = "Trading Purchase";
            }
            else if (txtTransactionType.Text == "TradingImport")
            {
                reportName = "Trading Import Purchase";
            }

            else if (txtTransactionType.Text == "TollReceive")
            {
                reportName = "Toll Receive";
            }

            else if (txtTransactionType.Text == "PurchaseReturn")
            {
                reportName = "Purchase Return";
            }
            else if (txtTransactionType.Text == "InputService")
            {
                reportName = "Input Service Purchase";
            }
            else if (txtTransactionType.Text == "Service")
            {
                reportName = "Service Stock Purchase";
            }
            else if (txtTransactionType.Text == "ServiceNS")
            {
                reportName = "Service Service Purchase";
            }
            else if (txtTransactionType.Text == "Import")
            {
                reportName = "Import Purchase";
            }
            else if (txtTransactionType.Text == "TollReceiveRaw")
            {
                reportName = "Toll Receive Raw";
            }
            //TransType = txtTransactionType.Text.Trim();
            //post = cmbPost.Text.Trim();
            //PurchaseType = cmbPurchaseType.Text.Trim();


            #endregion Transaction Type
        }
        private void btnMis_Click(object sender, EventArgs e)
        {

            FormRptPurchaseWithLC frmPurLc = new FormRptPurchaseWithLC();
            frmPurLc.Show();

        }



        private void backgroundWorkerMIS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                //ReportMIS = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();

                //ReportMIS = reportDsdal.PurchaseMis(txtInvoiceNo.Text.Trim(), BranchId, VatType);

                MISReport _report = new MISReport();


                reportDocument = _report.PurchaseMis(txtInvoiceNo.Text.Trim(), BranchId, VatType, reportName, FormNumeric,connVM);



                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerMIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                #region Old

                //ReportClass objrpt = new ReportClass();                // Start Complete

                //DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                //if (ReportMIS.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //ReportMIS.Tables[0].TableName = "DsPurchaseHeader";
                //ReportMIS.Tables[1].TableName = "DsPurchaseDetails";
                ////ReportMIS.Tables[2].TableName = "DsPurchaseDuty";

                //objrpt = new RptMISPurchase1();

                //objrpt.SetDataSource(ReportMIS);

                ////if (PreviewOnly == true)
                ////{
                ////    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                ////}
                ////else
                ////{
                ////    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                ////}

                ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
                ////objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";

                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                //FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                //// End Complete

                #endregion

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                btnMis.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string result = FormProductCategorySearch.SelectOne();
            if (result == "") { return; }
            else//if (result == ""){return;}else//if (result != "")
            {
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                txtPGroupId.Text = ProductCategoryInfo[0];
                txtPGroup.Text = ProductCategoryInfo[1];
            }
        }

        private void btnVendorGroup_Click(object sender, EventArgs e)
        {
            Program.fromOpen = "Me";
            string result = FormVendorGroupSearch.SelectOne();
            try
            {
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] VendorGroupInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtVendorGroupID.Text = VendorGroupInfo[0];
                    txtVendorGroupName.Text = VendorGroupInfo[1];
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

        private void chkLocal_CheckedChanged(object sender, EventArgs e)
        {
            //if(chkLocal.Checked==false)
            //{
            //    chkLocal.Text = "Local";
            //}
            //else if (chkLocal.Checked == true)
            //{
            //    chkLocal.Text = "Import";
            //}
        }

        private void backgroundWorkerMonth_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportDataTable = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportDataTable = reportDsdal.MonthlyPurchases(txtInvoiceNo.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtVendorId.Text.Trim(), txtItemNo.Text.Trim(),
                    txtPGroupId.Text.Trim(), txtProductType.Text.Trim(), TransType,
                    post, PurchaseType, txtVendorGroupID.Text.Trim(), "N", "-", "-", 0, 0, 0, BranchId, VatType,connVM);

                #endregion

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerMonth_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                if (ReportDataTable.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataView dtview = new DataView(ReportDataTable);
                dtview.Sort = "YearSerial ASC, MonthSerial ASC";
                dtsorted = dtview.ToTable();

                #region assign column name of datagridview

                //purchase.Product_Name,purchase.Product_Code,SUM(purchase.Quantity) Quantity,SUM(purchase.SubTotal) Amount,
                // SUM(purchase.VATAmount) VAT,purchase.MonthNames,purchase.ItemNo,purchase.VendorName,
                //                                    purchase.MonthSerial,purchase.YearSerial
                dgvSale = new DataGridView();
                int ee = dgvSale.Rows.Count;
                dgvSale.Columns.Add("sl", "S.N");
                dgvSale.Columns.Add("Product_Name", dtsorted.Columns["Product_Name"].ColumnName.Replace('_', ' '));
                dgvSale.Columns.Add("Product_Code", dtsorted.Columns["Product_Code"].ColumnName.Replace('_', ' '));

                DataTable monthNames = dtsorted.DefaultView.ToTable(true, "MonthNames");

                for (int i = 0; i < monthNames.Rows.Count; i++)
                {
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "Quantity",
                                        monthNames.Rows[i][0].ToString() + "_" + "Quantity");
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "Amount",
                                        monthNames.Rows[i][0].ToString() + "_" + "Amount");
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "VAT",
                                        monthNames.Rows[i][0].ToString() + "_" + "VAT");

                }

                string lastClmn = monthNames.Rows[0][0].ToString() + "-" +
                                  monthNames.Rows[monthNames.Rows.Count - 1][0].ToString() + " summary";
                dgvSale.Columns.Add("TotQty", lastClmn + "_Quantity");
                dgvSale.Columns.Add("Total_Amount", "Total_Amount");
                dgvSale.Columns.Add("Total_VAT", "Total_VAT");

                #endregion

                #region assign value to grid columns

                int j = 0;
                decimal totalQty = 0;
                decimal totalAmount = 0;
                decimal totalVAT = 0;


                dgvSale.Rows.Clear();
                int aaa = dgvSale.Rows.Count;
                foreach (DataRow item in dtsorted.DefaultView.ToTable(true, "Product_Code", "Product_Name").Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);
                    dgvSale["sl", j].Value = j + 1;
                    dgvSale["Product_Code", j].Value = item["Product_Code"].ToString();
                    dgvSale["Product_Name", j].Value = item["Product_Name"].ToString();
                    for (int i = 0; i < monthNames.Rows.Count; i++)
                    {
                        dgvSale[monthNames.Rows[i][0] + "_" + "Quantity", j].Value = 0;
                        dgvSale[monthNames.Rows[i][0] + "_" + "Amount", j].Value = 0;
                        dgvSale[monthNames.Rows[i][0] + "_" + "VAT", j].Value = 0;
                    }

                    j++;

                }
                int a = dgvSale.Rows.Count;
                int aa = dtsorted.DefaultView.ToTable(true, "Product_Code", "Product_Name").Rows.Count;
                for (int rowItem = 0; rowItem < dgvSale.Rows.Count - 1; rowItem++)
                {
                    totalQty = 0;
                    totalAmount = 0;
                    totalVAT = 0;
                    DataRow[] detail =
                        dtsorted.Select("Product_Code='" + dgvSale["Product_Code", rowItem].Value.ToString() + "'");

                    foreach (var row1 in detail)
                    {
                        for (int i = 0; i < monthNames.Rows.Count; i++)
                        {
                            if (row1["MonthNames"].ToString() == monthNames.Rows[i][0].ToString())
                            {

                                dgvSale[monthNames.Rows[i][0] + "_" + "Quantity", rowItem].Value =
                                    Convert.ToDecimal(row1["Quantity"].ToString().Trim());

                                totalQty += Convert.ToDecimal(row1["Quantity"].ToString().Trim());

                                dgvSale[monthNames.Rows[i][0] + "_" + "Amount", rowItem].Value =
                                    Convert.ToDecimal(row1["Amount"].ToString().Trim());

                                totalAmount += Convert.ToDecimal(row1["Amount"].ToString().Trim());

                                dgvSale[monthNames.Rows[i][0] + "_" + "VAT", rowItem].Value =
                                    Convert.ToDecimal(row1["VAT"].ToString().Trim());

                                totalVAT += Convert.ToDecimal(row1["VAT"].ToString().Trim());
                            }
                        }
                    }
                    dgvSale["TotQty", rowItem].Value = totalQty.ToString();
                    dgvSale["Total_Amount", rowItem].Value = totalAmount.ToString();
                    dgvSale["Total_VAT", rowItem].Value = totalVAT.ToString();
                }

                #endregion

                //ExportToExcel();
                //OpenExcel();
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               exMessage);
            }

            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }
        //=================================================

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        //private void ExportToExcel()
        //{
        //    Microsoft.Office.Interop.Excel.Range chartRange;
        //    try
        //    {
        //        xlApp = new Excel.Application();
        //        xlWorkBook = xlApp.Workbooks.Add(misValue);
        //        xlWorkSheet = (Excel.Worksheet) xlWorkBook.Worksheets.get_Item(1);

        //        #region Report Header

        //        string companyName = Program.CompanyName;
        //        string companyAddress = Program.Address1;
        //        string reportName = "Vendor wise Monthly Purchase Statement";
        //        string customerName = "Vendor: " + dtsorted.Rows[0]["VendorName"].ToString();
        //        string productName = "ProductName: All";
        //        if (!string.IsNullOrEmpty(txtProductName.Text))
        //            productName = "ProductName: " + txtProductName.Text;
        //        string dateFrom = dtpPurchaseFromDate.Value.ToString("MMMM") + "," +
        //                          dtpPurchaseFromDate.Value.Year.ToString();
        //        string dateTo = dtpPurchaseToDate.Value.ToString("MMMM") + "," +
        //                        dtpPurchaseToDate.Value.Year.ToString();

        //        #endregion Report Header

        //        #region Report Header Style

        //        chartRange = xlWorkSheet.get_Range("a1", "m1");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = companyName;

        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;
        //        chartRange.Font.Bold = true;
        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a2", "m2");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = companyAddress;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a4", "m4");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = reportName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 16;
        //        chartRange.Font.Bold = true;


        //        chartRange = xlWorkSheet.get_Range("a5", "m5");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = customerName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;
        //        chartRange.Font.Bold = true;

        //        chartRange = xlWorkSheet.get_Range("a7", "m7");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = productName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a8", "f8");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = "Date From: " + dateFrom;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("g8", "m8");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = "Date To: " + dateTo;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        #endregion Report Header

        //        #region Table Caption

        //        // Excel column name
        //        string[] col = new string[]
        //                           {
        //                               "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
        //                               "q",
        //                               "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac",
        //                               "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap"
        //                           };
        //        for (int colItem = 0; colItem < dgvSale.Columns.Count; colItem++)
        //        {
        //            if (colItem < 3)
        //            {
        //                chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem] + "9");
        //                chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText;
        //                chartRange.Font.Bold = true;
        //                chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //            }
        //            else
        //            {
        //                if (colItem%3 == 0)
        //                {
        //                    chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem] + "9");
        //                    chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[0];
        //                    chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem + 2] + "9");
        //                    chartRange.Merge(false);
        //                    chartRange.Font.Bold = true;
        //                    chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                    chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                    chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                            Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                            Excel.XlColorIndex.xlColorIndexAutomatic);

        //                }
        //                chartRange = xlWorkSheet.get_Range(col[colItem] + "10", col[colItem] + "10");
        //                //chartRange.FormulaR1C1 = dgvSale.Columns[colItem].Name.Substring(3);
        //                chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[1];
        //                chartRange.Font.Bold = true;
        //                chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic);

        //            }
        //        }

        //        string headerStrRange = col[0] + "9";
        //        string headerEndRange = col[dgvSale.Columns.Count - 1].ToString() + "10";
        //        chartRange = xlWorkSheet.get_Range(headerStrRange, headerEndRange);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);


        //        #endregion Table Caption

        //        #region Assign value

        //        string data = null;
        //        int rowNo = 0;
        //        int colNo = 0;
        //        for (rowNo = 0; rowNo < dgvSale.Rows.Count - 1; rowNo++)
        //        {
        //            for (colNo = 0; colNo < dgvSale.Columns.Count; colNo++)
        //            {
        //                data = dgvSale.Rows[rowNo].Cells[colNo].Value.ToString();
        //                xlWorkSheet.Cells[10 + rowNo + 1, colNo + 1] = data;
        //                chartRange = xlWorkSheet.get_Range(col[colNo] + (10 + rowNo + 1).ToString(),
        //                                                   col[colNo] + (10 + rowNo + 1).ToString());
        //                chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic);
        //            }
        //        }
        //        // table border
        //        string cellStartName = col[0] + "11";
        //        string cellEndName = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //        chartRange = xlWorkSheet.get_Range(cellStartName, cellEndName);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);

        //        #endregion Assign value

        //        #region Grand Total Calculation

        //        //grand total style
        //        string gSRange = col[dgvSale.Columns.Count - 6].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        string gEndRange = col[dgvSale.Columns.Count - 4].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(gSRange, gEndRange);
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = " Grand Total:";

        //        chartRange.Font.Bold = true;
        //        chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

        //        //calculate total Qty
        //        string totQtyRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totQtyRange, totQtyRange);
        //        string startRange = col[dgvSale.Columns.Count - 3].ToString() + "11";
        //        string endRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";

        //        //calculate total Amount
        //        string totAmtRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totAmtRange, totAmtRange);
        //        startRange = col[dgvSale.Columns.Count - 2].ToString() + "11";
        //        endRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";

        //        //calculate total VAT
        //        string totVatRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totVatRange, totVatRange);
        //        startRange = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //        endRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";

        //        chartRange = xlWorkSheet.get_Range(gSRange, totVatRange);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);

        //        #endregion

        //        #region Save location

        //        string printLocation = Directory.GetCurrentDirectory() + @"\MIS_Reports\Purchase";
        //        saveLocation = printLocation + @"\" + "Monthly Purchase" + "_" +
        //                       dateFrom + " - " + dateTo + "_ " + DateTime.Now.ToString("MMM,yy, HH.mm") + ".xls";

        //        if (!Directory.Exists(printLocation))
        //            Directory.CreateDirectory(printLocation);

        //        xlWorkBook.SaveAs(saveLocation, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue,
        //                          misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue,
        //                          misValue);
        //        xlApp.Visible = true;

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        xlWorkBook.Close(true, misValue, misValue);

        //        xlApp.Quit();

        //        releaseObject(xlWorkSheet);
        //        releaseObject(xlWorkBook);
        //        releaseObject(xlApp);


        //    }
        //    finally
        //    {

        //    }
        //}

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtProductType.Text = cmbProductType.Text;
        }

        private void chkCategoryLike_Click(object sender, EventArgs e)
        {
            txtPGroup.ReadOnly = true;

            if (chkCategoryLike.Checked)
            {
                txtPGroup.ReadOnly = false;
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void l2_Click(object sender, EventArgs e)
        {

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            #region Try
            ExcelRpt = true;
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                VatType = cmbType.Text;

                if (VatType == "All")
                {
                    VatType = "";
                }

                reporttype = cmbReport.Text.Trim();

                //Detail Summary Single Monthly
                if (Program.CheckLicence(dtpPurchaseToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                // cmbPostText = Convert.ToString(chkPost.Checked ? "Y" : "N");

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;

                NullCheck();
                transType();

                if (cmbReport.Text.Trim().ToLower() == "detail"
                    || cmbReport.Text.Trim().ToLower() == "summary"
                    || cmbReport.Text.Trim().ToLower() == "summarybyproduct"
                    )  //Detail   0
                {
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnDownload_Click", exMessage);
            }
            #endregion Catch
            #region bad


            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;

            }
        }

        private void SaveExcel(DataSet ds, string ReportType = "", string BranchName = "")
        {
            DataTable dt = new DataTable();

            DataTable dtresult = ds.Tables["Table"];
            if (dtresult.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            var dataView = new DataView(dtresult);
            dt = dataView.ToTable(true, "PurchaseInvoiceNo", "LCNumber", "LCDate", "BENumber", "InvoiceDateTime", "VendorName", "Address1", "VATRegistrationNo",
                    "ProductCode", "ProductName", "Quantity", "UOM", "AssessableValue", "CDAmount", "RDAmount", "SDAmount", "VATAmount", "Total");
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("",connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();



            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo};

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k], OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }



            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "PurchaseInformation";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };



                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                }
                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total
                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion
                    }

                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
                #endregion



              string  fileDirectory1 = @"d:\Test" +DateTime.Now.ToString("yyyy_MM_dd_HHmmss") + ".pdf";


              //PdfSaveOptions pdfOptions = new PdfSaveOptions()
              //{
              //    Orientation = eOrientation.Landscape,
              //    PageSize = ePaperSize.A4,
              //    FitToWidth = true
              //};

              FileInfo pdfFile = new FileInfo(fileDirectory1);
              //package.SaveAs(pdfFile, pdfOptions);
              package.SaveAs(pdfFile);



              //package.Save();

                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);

        }

        private void SaveExcelForTk(DataSet ds, string ReportType = "", string BranchName = "")
        {
            DataTable dt = new DataTable();

            DataTable dtresult = ds.Tables["Table"];
            if (dtresult.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            var dataView = new DataView(dtresult);
            dt = dataView.ToTable(true, "LCNumber", "LCDate", "BENumber", "InvoiceDateTime", "VendorName", "Address1", "VATRegistrationNo",
                    "ProductName", "Quantity", "UOM", "AssessableValue", "CDAmount", "RDAmount", "SDAmount", "VATAmount", "Total");
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("", connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();



            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k], OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }



            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "PurchaseInformation";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };



                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                }
                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total
                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion
                    }

                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);

        }





    }
}
