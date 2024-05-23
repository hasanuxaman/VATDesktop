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
using SymphonySofttech.Utilities;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Data.Odbc;
using System.Threading;
//
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using SymphonySofttech.Reports;
using VATServer.Ordinary;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;

namespace VATClient.ReportPages
{
    public partial class FormRptTransferIssueInformation : Form
    {
        public FormRptTransferIssueInformation()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
    
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        
        #region Global Variables For BackGroundWorker
        string[] Condition = new string[] { "one" };
        private static string ShiftId = "0";

        private DataSet ReportResult;
        private ReportDocument reportDocument = new ReportDocument(); 
        public int SenderBranchId = 0;
        public int BranchId;
        public string TType;
        string transactionType = string.Empty;
        string IssueFromDate, IssueToDate;
        private int BranchTo;
        private bool ExcelRpt = false;
        #endregion
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ClearAllFields()
        {
            try
            {
                txtIssueNo.Text = "";
                txtItemNo.Text = "";
      
        
   

                dtpIssueFromDate.Text = "";
                dtpIssueFromDate.Text = "";
                dtpIssueFromDate.Checked = false;
                dtpIssueToDate.Checked = false;
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
                if (dtpIssueFromDate.Checked == false)
                {
                    IssueFromDate = "";
                }
                else
                {
                    IssueFromDate = dtpIssueFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss").ToString();// dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpIssueToDate.Checked == false)
                {
                    IssueToDate = "";
                }
                else
                {
                    IssueToDate = dtpIssueToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss").ToString();//  dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd");
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
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelRpt = false;
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                NullCheck();
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                BranchTo = Convert.ToInt32(cmbBranchTo.SelectedValue);
                TType = comtt.SelectedIndex != -1 ? comtt.Text.Trim() : "";
                transactionType = txtTransactionType.Text.Trim();
                ShiftId = cmbShift.SelectedValue.ToString();
   
                
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
               
               
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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //No DAL Side Method
            try
            {

                string invoiceNo = FormTransferIssueSearch.SelectOne(txtTransactionType.Text.Trim());
                TransferIssueDAL issueDal = new TransferIssueDAL();
                TransferIssueVM vm = new TransferIssueVM();
                vm.TransferIssueNo = invoiceNo;
                vm.IssueDateFrom = "";
                vm.IssueDateTo = "";
                vm.Post = "";
                vm.TransactionType = txtTransactionType.Text.Trim();
                DataTable IssueResult = issueDal.SearchTransferIssue(vm,null,null,connVM);  // Change 04
                if (IssueResult.Rows.Count > 0)
                {
                    txtIssueNo.Text = IssueResult.Rows[0]["TransferIssueNo"].ToString();
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
        private void FormRptIssueInformation_Load(object sender, EventArgs e)
        {
            FormMaker();
            #region Branch DropDown

            string[] Conditions = new string[] { "ActiveStatus='Y'" };
            cmbBranchName = new CommonDAL().ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName", Conditions, "varchar", true,true,connVM);
            cmbBranchTo = new CommonDAL().ComboBoxLoad(cmbBranchTo, "BranchProfiles", "BranchID", "BranchName", Conditions, "varchar", true, true, connVM);


            if (SenderBranchId > 0)
            {
                cmbBranchName.SelectedValue = SenderBranchId;
            }
            else
            { 
                cmbBranchName.SelectedValue = Program.BranchId;

            }

           // cmbBranchTo.Text = "All";

            #endregion

            #region cmbShift
            CommonDAL cDal = new CommonDAL();
            cmbShift.DataSource = null;
            cmbShift.Items.Clear();
            Condition = new string[0];
            cmbShift = cDal.ComboBoxLoad(cmbShift, "Shifts", "Id", "ShiftName", Condition, "varchar", true, true, connVM);
            #endregion cmbShift

        }
        private void FormMaker()
        {
            try
            {
                #region Transaction Type

                this.Text = "Report (Issue) ";
                if (txtTransactionType.Text.Trim().ToLower() == "other")
                {
                    this.Text = "Report (Issue) ";
                }
                else
                    this.Text = "Report (" + txtTransactionType.Text.Trim() + " Issue) ";

                if (txtTransactionType.Text.Trim() == "61Out")
                {
                    comtt.SelectedIndex = 0;
                }
                 else
                {
                    comtt.SelectedIndex = 1;
                }

                #endregion Transaction Type

               
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
        #region Background Worker Events
        private void backgroundWorkerPreviewDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            ReportResult = null;

            try
            {


                if (ExcelRpt)
                {
                    ReportDSDAL reportDsdal = new ReportDSDAL();
                    ReportResult = reportDsdal.TransferIssueOutReport(txtIssueNo.Text.Trim(),IssueFromDate,IssueToDate,TType,BranchId,BranchTo,connVM,ShiftId);

                }
                else
                {

                MISReport _report = new MISReport();
                reportDocument = _report.TransferIssueOutReport(txtIssueNo.Text.Trim(), IssueFromDate, IssueToDate, TType, BranchId, BranchTo,ShiftId,connVM);

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

            try
            {
                if (ExcelRpt)
                {
                    if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                    {
                        MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                    SaveExcel(ReportResult, "MIS Transfer Issue");
                }
                else
                {
                    //DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                    //if (ReportResult.Tables.Count <= 0)
                    //{
                    //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                    //                    MessageBoxIcon.Information);
                    //    return;
                    //}
                    //ReportDocument objrpt = new ReportDocument();

                    //ReportResult.Tables[0].TableName = "dtTransferIssue";


                    //objrpt = new rptTranserIssue();

                    //objrpt.SetDataSource(ReportResult);


                    //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Transfer Issue Information'";
                    //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    //FormulaFieldDefinitions crFormulaF;
                    //crFormulaF = objrpt.DataDefinition.FormulaFields;
                    //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

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

                    //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
              
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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            ExcelRpt = true;
            try
            {

                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                NullCheck();
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                BranchTo = Convert.ToInt32(cmbBranchTo.SelectedValue);
                TType = comtt.SelectedIndex != -1 ? comtt.Text.Trim() : "";
                transactionType = txtTransactionType.Text.Trim();
                ShiftId = cmbShift.SelectedValue.ToString();


                backgroundWorkerPreviewDetails.RunWorkerAsync();

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

            dt = ds.Tables["Table"];
            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            //var dataView = new DataView(dtresult);
            //  dtresult = OrdinaryVATDesktop.DtColumnNameChange(dtresult, "ImportIDExcel", "Reference ID");
            //dt = dataView.ToTable(true, "IssueDateTime", "ProductCode", "ProductName", "IssueNo", "Reference ID",
            //          "UOM", "UOMQty", "Comments");

                    
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            tempDS = new ReportDSDAL().ComapnyProfile("");
            //IReport ReportDSDal =
            //   OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            ReportDSDAL ReportDSDal = new ReportDSDAL();
            tempDS = ReportDSDal.ComapnyProfile("");

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();



            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, "MIS Report for transfer issue" };

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


    }
}
