using System;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
//
using VATServer.Library;
using VATServer.Ordinary;
using SymphonySofttech.Reports;
using VATViewModel.DTOs;
using VATDesktop.Repo.VDSWCF;
using System.Collections.Generic;
using System.IO;

namespace VATClient.ReportPreview
{
    public partial class FormRptVDS12 : Form
    {
        public FormRptVDS12()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

		private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        string DepositFrom, DepositTo;
        string IssueFrom, IssueTo;
        string BillFrom, BillTo;
        public bool _isVDs = false;
        public bool _isTDs = false;
        public bool _IsTressury = false;
        #region Global Variables For BackGroundWorker
        private ReportDocument reportDocument = new ReportDocument();
        string transactionType = "VDS";
        DataTable VDSResult = new DataTable();
        DepositDAL depositDal = new DepositDAL();
        DataTable TDSResult = new DataTable();
        DepositTDSDAL depositTDSDal = new DepositTDSDAL();
        private DataSet ReportResult;

        #endregion

        private void NullCheck()
        {
            try
            {
                if (txtDepositNumber.Text=="")
                {
                    MessageBox.Show("Please Input Deposit No");
                    txtDepositNumber.Focus();

                    return;
                }
                if (dtpDepositDateFrom.Checked == false)
                {
                    DepositFrom = dtpDepositDateFrom.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    DepositFrom = dtpDepositDateFrom.Value.ToString("yyyy-MMM-dd"); ;
                }
                if (dtpDepositDateTo.Checked == false)
                {
                    DepositTo = dtpDepositDateTo.MaxDate.ToString("yyyy-MMM-dd"); ;
                }
                else
                {
                    DepositTo = dtpDepositDateTo.Value.ToString("yyyy-MMM-dd"); ;
                }

                if (dtpIssueDateFrom.Checked == false)
                {
                    IssueFrom = dtpIssueDateFrom.MinDate.ToString("yyyy-MMM-dd"); ;
                }
                else
                {
                    IssueFrom = dtpIssueDateFrom.Value.ToString("yyyy-MMM-dd"); ;
                }
                if (dtpIssueDateTo.Checked == false)
                {
                    IssueTo = dtpIssueDateTo.MaxDate.ToString("yyyy-MMM-dd"); ;
                }
                else
                {
                    IssueTo = dtpIssueDateTo.Value.ToString("yyyy-MMM-dd"); ;
                }

                if (dtpBillDateFrom.Checked == false)
                {
                    BillFrom = dtpBillDateFrom.MinDate.ToString("yyyy-MMM-dd"); ;
                }
                else
                {
                    BillFrom = dtpBillDateFrom.Value.ToString("yyyy-MMM-dd"); ;
                }
                if (dtpBillDateTo.Checked == false)
                {
                    BillTo = dtpBillDateTo.MaxDate.ToString("yyyy-MMM-dd"); ;
                }
                else
                {
                    BillTo = dtpBillDateTo.Value.ToString("yyyy-MMM-dd"); ;
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
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FormRptVDS_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();

            string mailSend = commonDal.settings("Report", "SendMail6_6", null, null, null);

            if (mailSend.ToLower() == "y")
            {
                btnSendMail.Visible = true;
            }
            else
            {
                btnSendMail.Visible = false;
            }

            if (_isVDs)
            {
                txtVendorName.Visible = false;
                VendorName.Visible = false;
                btnTDSPreveiw.Visible = false;
            }
            else if(_isTDs)
            {
                //FormRptVDS12.T
                //Deposit.Visible = false;
                //txtDepositNumber.Visible = false;
                txtVendorName.Visible = false;
                VendorName.Visible = false;
                txtPurchaseNumber.Visible = false;
                label5.Visible = false;
                chkPurchaseVDS.Visible = false;
                btnPreview.Visible = false;
                this.Text ="TDSCertificate";
            }
        }

        //=================================================================

        private void btnPreview_Click(object sender, EventArgs e)
        {
            OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

            ReportShowDs();
        }

        private void ReportShowDs()
        {
            try
            {
                NullCheck();

                //string ReportData =
                //    txtVendorId.Text.Trim() + FieldDelimeter +
                //    txtDepositNumber.Text.Trim() + FieldDelimeter +
                //    DepositFrom.ToString("yyyy-MMM-dd") + FieldDelimeter +
                //    DepositTo.ToString("yyyy-MMM-dd") + FieldDelimeter +
                //    IssueFrom.ToString("yyyy-MMM-dd") + FieldDelimeter +
                //    IssueTo.ToString("yyyy-MMM-dd") + FieldDelimeter +
                //    BillFrom.ToString("yyyy-MMM-dd") + FieldDelimeter +
                //    BillTo.ToString("yyyy-MMM-dd") + FieldDelimeter +
                //    txtPurchaseNumber.Text.Trim() + LineDelimeter;

                #region Start
                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);
                //ReportDSSoapClient ShowReport = new ReportDSSoapClient();
                //DataSet ReportResult = ShowReport.VDS12Kha(encriptedData.ToString(), Program.DatabaseName);
                #endregion

                this.btnPreview.Enabled = false;
                this.progressBar1.Visible = true;
                if (txtDepositNumber.Text != null && txtDepositNumber.Text !="")
                {
                    backgroundWorkerPreview.RunWorkerAsync();

                }
                this.btnPreview.Enabled =true;

                #region Complete
                //ReportClass objrpt = new ReportClass();
                //ReportResult.Tables[0].TableName = "DsVAT12Kha";
                //objrpt = new RptVAT12Kha();

                //objrpt.SetDataSource(ReportResult);

                ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 26'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address11"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address21"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address31"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["ZIPCode"].Text = "'" + Program.ZipCode + "'";

                ////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                //FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //reports.ShowDialog();
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

                //ReportResult = new DataSet();
                NBRReports _report =new NBRReports();
                reportDocument = _report.VDS12KhaNew(txtVendorId.Text.Trim(), txtDepositNumber.Text.Trim(), DepositFrom,
                                                      DepositTo, IssueFrom, IssueTo, BillFrom, BillTo,
                                                      txtPurchaseNumber.Text.Trim(), chkPurchaseVDS.Checked,connVM);
 

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
                #region Commented
                //ReportClass objrpt = new ReportClass();

                //ReportResult.Tables[0].TableName = "DsVAT12Kha";
                //objrpt = new RptVAT6_6();

                //objrpt.SetDataSource(ReportResult);

                ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 26'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address11"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address21"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address31"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["ZIPCode"].Text = "'" + Program.ZipCode + "'";

                ////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                #endregion Commented
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

            this.btnPreview.Enabled = true;
            this.progressBar1.Visible = false;
        }

        #endregion

        private void btnTDSPreveiw_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                DataSet ds = new DataSet();
                if (txtDepositNumber.Text=="")
                {
                    MessageBox.Show("Please Enter Deposit No");
                    txtDepositNumber.Focus();
                }
                if (txtDepositNumber.Text != null && txtDepositNumber.Text != "")
                {
                    ds = new ReportDSDAL().TDS_Certificatet(txtDepositNumber.Text.Trim(), connVM);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtTDSCertificate";


                    objrpt = new RptTDSCertificate();
                    //objrpt.Load(Program.ReportAppPath + @"\RptTDSCertificate.rpt");

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
               
            }
            #endregion
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
                FileLogger.Log(this.Name, "btnTDSPreveiw_Click", exMessage);
            }
            #endregion
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            #region Transaction Type
            transactionType = string.Empty;
            if (_isTDs)
            {
                transactionType =  "TDS";
            }


            else if (_isVDs)
            {
                if (chkPurchaseVDS.Checked)
                {
                    transactionType = "VDS";
                }
                else
                {
                    transactionType = "SaleVDS";
                }
            }
      
            
            #endregion Transaction Type

            //No DAL Side Method
            try
            {
                if(_isVDs)
                {
                    DataGridViewRow selectedRow = FormDepositSearch.SelectOne(transactionType);
                    if (selectedRow != null && selectedRow.Selected == true)
                    {

                        txtDepositNumber.Text = selectedRow.Cells["DepositId"].Value.ToString();

                        VDSResult = depositDal.SearchVDSDT(txtDepositNumber.Text, connVM);
                        txtVendorId.Text = VDSResult.Rows[0]["VendorID"].ToString();
                        txtPurchaseNumber.Text = VDSResult.Rows[0]["PurchaseNumber"].ToString();
                        chkPurchaseVDS.Checked = VDSResult.Rows[0]["IsPurchase"].ToString() == "Y" ? true : false;
                    }
                }
                else
                {
                    DataGridViewRow selectedRow = FormDepositTDSSearch.SelectOne(transactionType);
                    if (selectedRow != null && selectedRow.Selected == true)
                    {

                        txtDepositNumber.Text = selectedRow.Cells["DepositId"].Value.ToString();

                        TDSResult = depositTDSDal.SearchVDSDT(txtDepositNumber.Text, connVM);
                        txtVendorId.Text = TDSResult.Rows[0]["VendorID"].ToString();
                        txtPurchaseNumber.Text = TDSResult.Rows[0]["PurchaseNumber"].ToString();
                        chkPurchaseVDS.Checked = TDSResult.Rows[0]["IsPurchase"].ToString() == "Y" ? true : false;
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

        private void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                string logginUser = Program.CurrentUser;

                DialogResult = MessageBox.Show("Do You Want To Mail This Report?", "Send Mail", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult == DialogResult.Yes)
                {
                    OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                    VDSDAL vds = new VDSDAL();

                    var deposit = vds.SelectVDSDetail(txtDepositNumber.Text.Trim(), null, null, null, null, null, "VDS");
                    
                    string emailAddress = "";
                    foreach (var item in deposit)
                    {
                        if (item.IsMailSend == "N")
                        {
                            if (!string.IsNullOrWhiteSpace(item.Email) && item.Email != "-")
                            {
                                emailAddress = item.Email;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Already Mail Send!");
                            return;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(emailAddress))
                    {
                        MessageBox.Show("Vendor Email Address Not Found.");
                        return;
                    }

                    string directoryPath = @"D:\VAT6_6"; // Change this to your desired directory path
                    string fileName = "Report_VAT6_6.pdf";

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string filePath = Path.Combine(directoryPath, fileName);

                    NBRReports _report = new NBRReports();
                    var res = _report.Report_VAT6_6_WithMail("", txtDepositNumber.Text.Trim(), "", "", "", "", "", "", "", chkPurchaseVDS.Checked, connVM, logginUser,directoryPath, fileName, emailAddress);

                    MessageBox.Show(res[1]);
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //=================================================================
    }
}
