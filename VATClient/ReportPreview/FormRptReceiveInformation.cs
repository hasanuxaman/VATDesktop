using System;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
////
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
//
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using VATServer.Ordinary;
using SymphonySofttech.Reports;
using VATServer.Interface;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using VATDesktop.Repo;

namespace VATClient.ReportPages
{
    public partial class FormRptReceiveInformation : Form
    {
        public FormRptReceiveInformation()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataSet ReportMIS;
        private static string ShiftId = "0";
        string[] Condition = new string[] { "one" };
        public string FormNumeric = string.Empty;

        public int SenderBranchId = 0;
        public int BranchId;
        private bool ExcelRpt = false;
        private ReportDocument reportDocument = new ReportDocument(); 

        #region Global Variables For BackGroundWorker

        private DataSet ReportResult;
        private string cmbPostText;
        private string TransactionType;

        #endregion

        string ReceiveFromDate, ReceiveToDate;

        private void grbBankInformation_Enter(object sender, EventArgs e)
        {

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
        private void ClearAllFields()
        {
            txtItemNo.Text = "";
            txtProductName.Text = "";
            txtPGroup.Text = "";
            txtPGroupId.Text = "";
            txtReceiveNo.Text = "";
            dtpReceiveFromDate.Text = "";
            dtpReceiveToDate.Text = "";
            dtpReceiveFromDate.Checked = false;
            dtpReceiveToDate.Checked = false;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void NullCheck()
        {
            try
            {
                if (dtpReceiveFromDate.Checked == false)
                {
                    ReceiveFromDate = "";
                }
                else
                {
                    ReceiveFromDate = dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd HH:mm");// dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpReceiveToDate.Checked == false)
                {
                    ReceiveToDate = "";
                }
                else
                {
                    ReceiveToDate = dtpReceiveToDate.Value.ToString("yyyy-MMM-dd HH:mm");// dtpReceiveToDate.Value.ToString("yyyy-MMM-dd");
                }
                if (chkShiftAll.Checked)
                {
                    ShiftId = "0";
                }
                else
                {
                    ShiftId = cmbShift.SelectedValue.ToString();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
           
            try
            {
                DataGridViewRow selectedRow = null;
                selectedRow = FormReceiveSearch.SelectOne(txtTransactionType.Text.Trim());
                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtReceiveNo.Text = selectedRow.Cells["ReceiveNo"].Value.ToString();
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
        private void btnProduct_Click(object sender, EventArgs e)
        {
            //No DAL Method

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

                    //txtPGroupId.Text = selectedRow.Cells["CategoryID"].Value.ToString();// ProductInfo[3];
                    //txtPGroup.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[4];

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
        private void FormRptReceiveInformation_Load(object sender, EventArgs e)
        {
            FormMaker();
            #region cmbShift
            CommonDAL cDal = new CommonDAL();
            cmbShift.DataSource = null;
            cmbShift.Items.Clear();
            Condition = new string[0];
            cmbShift = cDal.ComboBoxLoad(cmbShift, "Shifts", "Id", "ShiftName", Condition, "varchar", true,true,connVM);
            #endregion cmbShift

            #region Branch DropDown

            string[] Conditions = new string[] { "ActiveStatus='Y'" };
            cmbBranchName = new CommonDAL().ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName", Conditions, "varchar", true,true,connVM);


            if (SenderBranchId > 0)
            {
                cmbBranchName.SelectedValue = SenderBranchId;
            }
            else
            {
                cmbBranchName.SelectedValue = Program.BranchId;

            }
            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
            cmbBranchName.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion
 

            dtpReceiveFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 00:00:00"));
            dtpReceiveToDate.Value =   Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 23:59:59"));


            #endregion

        }
        private void FormMaker()
        {
            try
            {
                #region Transaction Type

                this.Text = "Report (Receive) ";
                if (txtTransactionType.Text.Trim().ToLower() == "other")
                {
                    this.Text = "Report (Receive) ";
                }
                else
                    this.Text = "Report (" + txtTransactionType.Text.Trim() + " Receive) ";

                #endregion Transaction Type
               
                PTypeSearch();

                #region Preview Only

                CommonDAL commonDal = new CommonDAL();

                FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);
                cmbDecimal.Text = FormNumeric;

                string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);

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
        private void button1_Click(object sender, EventArgs e)
        {
            //No DAL Method

            ////MDIMainInterface mdi = new MDIMainInterface();
            //FormProductCategorySearch frm = new FormProductCategorySearch();
            //mdi.RollDetailsInfo(frm.VFIN);
            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
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
                }
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

        //=========================================================
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelRpt = false;
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                FormNumeric = cmbDecimal.Text.Trim();
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                TransactionType = txtTransactionType.Text.Trim();
                cmbPostText = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                NullCheck();
                if (Program.CheckLicence(dtpReceiveToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (rbtnSingle.Checked == true)
                {
                    if (txtReceiveNo.Text == "")
                    {
                        MessageBox.Show("Please select the Receive No", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }

                    backgroundWorkerMIS.RunWorkerAsync();
                }
                else 
                {
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
                }
                
            }
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

        }

      
        #region backgroundWorkerReport

        private void backgroundWorkerPreviewSummary_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.ReceiveNew(txtReceiveNo.Text.Trim(), ReceiveFromDate, ReceiveToDate, txtItemNo.Text.Trim(),
                     txtPGroupId.Text.Trim(), txtProductType.Text.Trim(), TransactionType, cmbPostText, ShiftId,BranchId,connVM);
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

        private void backgroundWorkerPreviewSummary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                #region Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                ReportResult.Tables[0].TableName = "DsReceive";
                RptReceivingSummery objrpt = new RptReceivingSummery();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Receive Summery Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                if (txtProductName.Text == "")
                {
                    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + txtProductName.Text.Trim() + "'  ";
                }

                if (txtReceiveNo.Text == "")
                {
                    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + txtReceiveNo.Text.Trim() + "'  ";
                }

                if (dtpReceiveFromDate.Checked == false)
                {
                    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                                                                            dtpReceiveFromDate.Value.ToString(
                                                                                "dd/MMM/yyyy") + "'  ";
                }

                if (dtpReceiveToDate.Checked == false)
                {
                    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                                                                          dtpReceiveToDate.Value.ToString("dd/MMM/yyyy") +
                                                                          "'  ";
                }

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                #endregion
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

        private void backgroundWorkerPreviewDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                string ReportType = "";
                if (rbDetail.Checked == true)
                {
                    ReportType = "Detail";

                }
                else if (rbSummery.Checked == true)
                {
                    ReportType = "Summary";

                }

                MISReport _report = new MISReport();

                if(ExcelRpt)
                {

                    ReportDSDAL reportDsdal = new ReportDSDAL();
                    ReportResult = reportDsdal.ReceiveNew(
                        txtReceiveNo.Text.Trim()
                      , ReceiveFromDate
                      , ReceiveToDate
                      , txtItemNo.Text.Trim()
                      , txtPGroupId.Text.Trim()
                      , txtProductType.Text.Trim()
                      , TransactionType
                      , cmbPostText
                      , ShiftId
                      , BranchId, connVM);
                }
                else
                {
                reportDocument = _report.ReceiveNew(txtReceiveNo.Text.Trim(), ReceiveFromDate, ReceiveToDate, txtItemNo.Text.Trim(),
                     txtPGroupId.Text.Trim(), txtProductType.Text.Trim(), TransactionType, cmbPostText, ShiftId, BranchId, txtProductName.Text.Trim(), ReportType, FormNumeric,connVM);
                } 
            }

            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

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

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreviewDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                #region Complete
                if (ExcelRpt)
                {
                    if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                    {
                        MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                    SaveExcel(ReportResult, "MIS Production Receive");
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

                #region

                //DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //ReportClass objrpt= new ReportClass();
                //if (rbDetail.Checked == true)
                //{
                //    ReportResult.Tables[0].TableName = "DsReceive";
                //    objrpt = new RptReceivingReport();
                //}
                //else if (rbSummery.Checked == true)
                //{
                //    ReportResult.Tables[0].TableName = "DsReceive";
                //    objrpt = new RptReceivingSummery();
                //    //objrpt.PrintOptions.PaperSize = "A4";
                //}


                //objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Receive Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                //if (txtProductName.Text == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + txtProductName.Text.Trim() + "'  ";
                //}


                //if (txtReceiveNo.Text == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + txtReceiveNo.Text.Trim() + "'  ";
                //}

                //if (dtpReceiveFromDate.Checked == false)
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                //                                                            dtpReceiveFromDate.Value.ToString(
                //                                                                "dd/MMM/yyyy HH:mm") + "'  ";
                //}

                //if (dtpReceiveToDate.Checked == false)
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                //                                                          dtpReceiveToDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                //                                                          "'  ";
                //}


                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                //FormReport reports = new FormReport();
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                #endregion
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

     
      
        private void backgroundWorkerMIS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                //MISReport _report = new MISReport();

                //reportDocument = _report.ReceiveMis(txtReceiveNo.Text.Trim(), ShiftId, BranchId);


                ReportMIS = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportMIS = reportDsdal.ReceiveMis(txtReceiveNo.Text.Trim(), ShiftId, BranchId,connVM);



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

                //FormReport reports = new FormReport();
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(reportDocument);
                //reports.ShowDialog();

                #region Old
                ReportClass objrpt = new ReportClass();                // Start Complete


                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                //formula
                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (BranchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                    //DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }
                //end formula

                if (ReportMIS.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ReportMIS.Tables[0].TableName = "DsReceiveHeader";
                ReportMIS.Tables[1].TableName = "DsReceiveDetails";
                //////ReportMIS.Tables[2].TableName = "DsPurchaseDuty";
                objrpt = new RptMISReceive();
                objrpt.SetDataSource(ReportMIS);

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null,connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt,connVM);


               

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
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                // objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";
                objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
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
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string result = FormProductTypeSearch.SelectOne();
            //    if (result == "") { return; }
            //    else//if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());

            //        txtProductType.Text = TypeInfo[1];
            //    }
            //}
            //#region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log(this.Name, "button1_Click", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", exMessage);
            //}
            //#endregion Catch
        }

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

        private void cmbShift_Leave(object sender, EventArgs e)
        {

        }

        private void cmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtpReceiveFromDate_ValueChanged(object sender, EventArgs e)
        {
            //if (dtpReceiveFromDate.Checked == true)
            //{
            //    dtpReceiveFromDate.Value = Convert.ToDateTime(dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd 00:01"));
            //}
           
         }

        private void dtpReceiveToDate_ValueChanged(object sender, EventArgs e)
        {
            //if(dtpReceiveToDate.Checked==true)
            //{
            //    dtpReceiveToDate.Value = Convert.ToDateTime(dtpReceiveToDate.Value.ToString("yyyy-MMM-dd 23:59"));
            //}
           
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            ExcelRpt = true;
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                FormNumeric = cmbDecimal.Text.Trim();
                NullCheck();
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                cmbPostText = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";
                //cmbwasteText = cmbWaste.SelectedIndex != -1 ? cmbWaste.Text.Trim() : "";
                TransactionType = txtTransactionType.Text.Trim();
                if (Program.CheckLicence(dtpReceiveToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);

                if (rbDetail.Checked == true)
                {
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnDownload_Click", exMessage);
            }
            #endregion Catch
            finally
            {
                // this.button1.Enabled = true;
                this.btnPreview.Enabled = true;

                this.progressBar1.Visible = false;
            }
        }

        private void SaveExcel(DataSet ds, string ReportType = "", string BranchName = "", string CompanyCode = null)
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
            ////dtresult = OrdinaryVATDesktop.DtColumnNameChange(dtresult, "ImportIDExcel", "Reference ID");
            dt = dataView.ToTable(true, "ReceiveDateTime", "ProductCode", "ProductName", "ReceiveNo",
                      "UOM", "UOMQty", "Comments");
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal =
                OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("");

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();



            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, "MIS Report for production Receive" };

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
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k],
                    OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
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
            //if (string.IsNullOrEmpty(sheetName))
            //{
            //    sheetName = ReportType;
            //}

            string sheetName = ReportType;


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
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
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

                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                     ws.Cells[TableHeadRow + 1, colNumber].Address +
                                                                     ":" + ws.Cells[(TableHeadRow + RowCount),
                                                                         colNumber].Address + ")";

                        #endregion
                    }
                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] +
                    (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] +
                    (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

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


        //==========================================================


    }
}
