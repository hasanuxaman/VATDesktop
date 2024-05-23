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
    public partial class FormVAT6_10 : Form
    {
        public FormVAT6_10()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string branchName = "";
        private string DBName = "";
        private DataTable dtbranchNames;
        private bool PreviewOnly = false;
        public string VFIN = "34";
        private int BranchId = 0;
        private string IsPost = "";
        public int SenderBranchId = 0;

        #region Global Variables For BackGroundWorker
        string vTotalAmount = "";
        decimal TotalAmount = 0;

        private DataSet ReportResult;
        string post1, post2 = string.Empty;
        string InEnglish= string.Empty;
        private ReportDocument reportDocument = new ReportDocument(); 
        #endregion
        private void FormVAT6_10_Load(object sender, EventArgs e)
        {
           

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


            CommonDAL commonDal = new CommonDAL();

            vTotalAmount = commonDal.settingsDesktop("VAT6_10", "ChallanRange", null, connVM);
            if (string.IsNullOrEmpty(vTotalAmount))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }
            TotalAmount = Convert.ToInt32(vTotalAmount);


            #region Preview button

            btnPrev.Visible = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM) == "Y";

            #endregion

            ClearAllFields();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void ClearAllFields()
        {
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        #region backgroundWorkerPreview
        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
           
                NBRReports _report = new NBRReports();
                _report.PreviewOnly = PreviewOnly;
                _report.branchName = branchName;
                reportDocument = _report.VAT6_10Report(TotalAmount.ToString(), dtpFromDate.Value.ToString("yyyy-MMM-dd")
                , dtpToDate.Value.ToString("yyyy-MMM-dd"), post1, post2, BranchId, InEnglish);
                //ReportResult = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();


                ////ReportResult = reportDsdal.VAT6_10Report(TotalAmount.ToString(), dtpFromDate.Value.ToString("yyyy-MMM-dd")
                //, dtpToDate.Value.ToString("yyyy-MMM-dd"), post1, post2, BranchId);

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

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                //branchName = cmbBranchName.Text;
                //// Start Complete
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}


                //ReportResult.Tables[0].TableName = "DsVA6_10";
                //ReportDocument objrpt = new ReportDocument();
                //objrpt = new RptVAT6_10();
                ////objrpt.Load(Program.ReportAppPath + @"\RptVAT6_10.rpt");


                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}
                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";

                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
              
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize",Program.FontSize);
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }
                FormReport reports = new FormReport(); 
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                // End Complete
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
                this.btnPreview.Enabled = true;
                this.btnPrev.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }
        #endregion
        private void ReportShowDs()
        {
            try
            {
                if (PreviewOnly == true)
                {
                    if (IsPost.ToLower()=="y")
                    {
                        post1 = "Y";
                        post2 = "Y";
                    }
                    else if (IsPost.ToLower()=="n")
                    {
                        post1 = "N";
                        post2 = "N";
                    }
                    else
                    {
                        post1 = "y";
                        post2 = "N";
                    }
                    
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                this.btnPreview.Enabled = false;
                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;
                branchName = cmbBranchName.Text;
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

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                //////DBName = cmbBranchName.SelectedValue.ToString();
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                IsPost = cmbPost.Text;

                PreviewOnly = true;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                //////DBName = cmbBranchName.SelectedValue.ToString();
                BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                IsPost = cmbPost.SelectedText;

                PreviewOnly = false;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
             
                ReportShowDs();
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

        private void chbInEnglish_Click(object sender, EventArgs e)
        {
            chbInEnglish.Text = "Bangla";
            if (chbInEnglish.Checked)
            {
                chbInEnglish.Text = "English";

            }
        }


    }
}
