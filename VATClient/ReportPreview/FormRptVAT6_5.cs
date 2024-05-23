using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using SymphonySofttech.Reports.Report;
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
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormRptVAT6_5 : Form
    {
        public FormRptVAT6_5()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private int BranchId = 0;

        private ReportDocument reportDocument = new ReportDocument(); 

        public string InEnglish=string.Empty;
        private string branchName = "";
        private string DBName = "";
        private DataTable dtbranchNames;
        private void FormRptVAT6_5_Load(object sender, EventArgs e)
        {
             

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //No DAL Side Method
            try
            {
                string IssueNo = FormTransferIssueSearch.SelectOne(rbtn61Out.Checked == true ? "61Out" : "62Out");
                if (IssueNo != null)
                {
                    txtIssueNo.Text = IssueNo.ToString();
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                //////BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);

                PreviewDetails(true);

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void PreviewDetails( bool PreviewOnly =false)
        {
            #region Try
            try
            {
                branchName = cmbBranchName.Text;
                CommonDAL cdal = new CommonDAL();
                string VAT11Name = cdal.settingsDesktop("Reports", "VAT6_3", null, connVM);

                if (string.IsNullOrWhiteSpace(txtIssueNo.Text.Trim()))
                {

                    MessageBox.Show("Must select the Issue No!", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    txtIssueNo.Focus();
                    return;
                }

               
                #region getData



                NBRReports _report = new NBRReports();
                _report.VAT11Name = VAT11Name;
                reportDocument = _report.TransferIssueNew(txtIssueNo.Text.Trim(), "", "", "", "", "", "", "", PreviewOnly, InEnglish,   connVM);
               
                #endregion
                #region showReport
              
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }

              

                if (PreviewOnly ==true)
                {
                    FormReport reports = new FormReport();
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {
                    var VPrinterName = new CommonDAL().settingValue("Printer", "DefaultPrinter", connVM);

                    reportDocument.PrintOptions.PrinterName = VPrinterName;
                    reportDocument.PrintToPrinter(1, false, 0, 0);
                    MessageBox.Show("You have successfully print ");

                }

            }
                #endregion
            #endregion
            #region Catch
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
            #region Finally
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
            #endregion
        }

        private void cmbBranchName_Leave(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //////BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);

                PreviewDetails(false);

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
