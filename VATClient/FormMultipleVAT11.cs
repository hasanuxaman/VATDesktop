using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using System.Web.Services.Protocols;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Threading;
using VATServer.License;
using VATDesktop.Repo;
using VATServer.Ordinary;
using VATServer.Interface;
using SymphonySofttech.Reports.Report.Rpt63;
using SymphonySofttech.Reports;

namespace VATClient
{
    public partial class FormMultipleVAT11 : Form
    {
        #region Global Variable

        #region New Variables

        bool IsCreditNote = false;
        bool IsDebitNote = false;
        bool IsRawCreditNote = false;
        bool IsTrading = false;
        bool IsTollIssue = false;
        bool IsVAT11GaGa = false;
        int AlReadyPrintNo = 0;
        bool Is11 = false;
        bool IsValueOnly = false;
        bool IsCommercialImporter = false;

        #endregion

        private ReportDocument reportDocument = new ReportDocument();
        CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
        FormulaFieldDefinitions crFormulaF;

        public string FormType = "";


        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string SelectedValue = string.Empty;
        private string ItemNature = string.Empty;
        private int PrintCopy;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static int NoOfCopies;
        private static int MaxNoOfPrint;
        private static int PageNo;
        private string Is3Plyer = string.Empty;
        private string PrinterName = string.Empty;
        private string vVAT11Letter, vVAT11A4, vVAT11Legal, vIs3Plyer, vItemNature, vMaxNoOfPrint, vPrintCopy, vPrinterName = string.Empty;
        private bool VAT11Letter;
        private bool VAT11A4;
        private bool VAT11Legal;
        private string VAT11PageSize = string.Empty;
        private string TransactionType = string.Empty;
        private bool PreviewOnly = false;
        private bool PrintAll = false;

        private DataSet ReportResultVAT11;
        private DataSet ReportResultCreditNote;
        private DataSet ReportResultDebitNote;
        private string[] sqlResults;
        private string post1, post2;
        string ChallanFromDate, ChallanToDate;
        string SalesInvoiceNo = string.Empty;
        private DataSet MultipleReportResult;
        private string varTrading = string.Empty;
        private string PrintLocation = String.Empty;

        private DataSet ReportResultTracking;
        private bool TrackingTrace;
        private string Heading1 = string.Empty;
        private string Heading2 = string.Empty;

        private int BranchId = 0;
        private int SenderBranchId = 0;
        private string RecordCount = "0";
        #endregion

        public FormMultipleVAT11()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        private void FormMultipleVAT11_Load(object sender, EventArgs e)
        {
            #region Labels

            if (FormType == "Credit")
            {
                this.Text = "Report (VAT 6.7) Credit Note";
            }
            else if (FormType == "Debit")
            {
                this.Text = "Report (VAT 6.8) Debit Note";
            }

            #endregion

            FormLoad();

        }


        private void FormLoad()
        {
            try
            {

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, true, connVM);
                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

                if (SenderBranchId > 0)
                {
                    cmbBranch.SelectedValue = SenderBranchId;
                }
                else
                {
                    cmbBranch.SelectedValue = Program.BranchId;

                }

                #region Transaction Dropdown
                cmbTransaction.DataSource = null;
                if (FormType == "Sale")
                {
                    cmbTransaction.DataSource = new EnumDAL().SaleTransactionTypeList;
                }
                else if (FormType == "Credit")
                {
                    cmbTransaction.Items.Add("Credit");
                    cmbTransaction.SelectedIndex = 0;
                }
                else if (FormType == "Debit")
                {
                    cmbTransaction.Items.Add("Debit");
                    cmbTransaction.SelectedIndex = 0;
                }


                #endregion

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth");
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                #region Settings
                CommonDAL commonDal = new CommonDAL();
                vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter");
                vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4");
                vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal");
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint");
                vMaxNoOfPrint = commonDal.settingsDesktop("Printer", "MaxNoOfPrint");
                vItemNature = commonDal.settingsDesktop("Sale", "ItemNature");
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint");
                vPrinterName = commonDal.settingsDesktop("Printer", "DefaultPrinter");
                vIs3Plyer = commonDal.settingsDesktop("Sale", "Page3Plyer");

                if (string.IsNullOrEmpty(vVAT11Letter)
                       || string.IsNullOrEmpty(vVAT11Legal)
                       || string.IsNullOrEmpty(vVAT11A4)
                       || string.IsNullOrEmpty(vPrintCopy)
                       || string.IsNullOrEmpty(vMaxNoOfPrint)
                       || string.IsNullOrEmpty(vItemNature)
                       || string.IsNullOrEmpty(vPrintCopy)
                       || string.IsNullOrEmpty(vPrinterName)
                       || string.IsNullOrEmpty(vIs3Plyer)

                       )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                PrinterName = vPrinterName;
                PrintCopy = Convert.ToInt32(vPrintCopy);
                MaxNoOfPrint = Convert.ToInt32(vMaxNoOfPrint);
                ItemNature = vItemNature;
                VAT11Letter = Convert.ToBoolean(vVAT11Letter == "Y" ? true : false);
                VAT11A4 = Convert.ToBoolean(vVAT11A4 == "Y" ? true : false);
                VAT11Legal = Convert.ToBoolean(vVAT11Legal == "Y" ? true : false);
                Is3Plyer = vIs3Plyer;
                if (VAT11A4 == true)
                {
                    VAT11PageSize = "A4";
                }
                else if (VAT11Letter == true)
                {
                    VAT11PageSize = "Letter";
                }
                else if (VAT11Legal == true)
                {
                    VAT11PageSize = "Legal";
                }
                else
                {
                    VAT11PageSize = "A4";
                }
                #endregion

                #region Set Printer Name

                List<string> names = new List<string>();
                foreach (string printerNames in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    names.Add(printerNames);
                }
                cmbPrinterName.DataSource = names;
                cmbPrinterName.Text = PrinterName;
                if (Program.IsBureau == true)
                {
                    cmbTransaction.Text = "Service(NonStock)";
                }
                else
                {
                    cmbTransaction.Text = "Local";
                }

                #endregion Set Printer Name

                #region Select page
                if (numericUDPrintCopy.Value == 1)
                {
                    lblPrintPage.Visible = true;
                    chk1st.Visible = true;
                    chk2nd.Visible = true;
                    chk3rd.Visible = true;
                }
                else
                {
                    lblPrintPage.Visible = false;
                    chk1st.Visible = false;
                    chk2nd.Visible = false;
                    chk3rd.Visible = false;
                }
                #endregion Select page

                #region No of Print

                if (!string.IsNullOrEmpty(Is3Plyer))
                {
                    if (Is3Plyer.ToUpper() == "Y")
                    {
                        numericUDPrintCopy.Value = 1;
                        numericUDPrintCopy.Enabled = false;
                    }
                    else
                    {
                        numericUDPrintCopy.Value = PrintCopy;
                        numericUDPrintCopy.Enabled = true;

                    }
                }
                else
                {
                    numericUDPrintCopy.Value = PrintCopy;
                    numericUDPrintCopy.Enabled = true;
                }

                #endregion No of Print

                #region Tracking
                string vTracking = string.Empty;
                string vHeading1, vHeading2 = string.Empty;
                vTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking");
                vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1");
                vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2");

                if (string.IsNullOrEmpty(vTracking) || string.IsNullOrEmpty(vHeading1) || string.IsNullOrEmpty(vHeading2))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                TrackingTrace = vTracking == "Y" ? true : false;

                if (TrackingTrace == true)
                {
                    Heading1 = vHeading1;
                    Heading2 = vHeading2;
                }

                #endregion

                #region Preview button

                btnPreview.Visible = commonDal.settingsDesktop("Reports", "PreviewOnly") == "Y";

                #endregion

                if (FormType == "Sale")
                {
                    btnPrintAll.Visible = false;
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
                MessageBox.Show(ex.Message, "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormMultipleVAT11", "SelectOne", exMessage);
            }
            #endregion Catch
        }


        #region Old Methods // Mar-18-2020


        public static string SelectOne(int alreadyPrint)
        {
            string SearchValue = string.Empty;

            try
            {
                NoOfCopies = alreadyPrint;
                FormMultipleVAT11 frmSalePrint = new FormMultipleVAT11();
                frmSalePrint.ShowDialog();
                SearchValue = frmSalePrint.SelectedValue;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log("FormMultipleVAT11", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormMultipleVAT11", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormMultipleVAT11", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormMultipleVAT11", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormMultipleVAT11", "SelectOne", exMessage);
            }
            #endregion Catch

            return SearchValue;

        }



        private void btnOk_Click(object sender, EventArgs e)
        {
            NullCheck();
            try
            {
                progressBar1.Visible = true;
                //ReportDSDAL reportDal = new ReportDSDAL();
                IReport reportDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
                MultipleReportResult = new DataSet();
                MultipleReportResult = reportDal.SelectMultipleInvoices(MaxNoOfPrint, TransactionType, ChallanFromDate, ChallanToDate, connVM);
                if (MultipleReportResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                CreateFolder();
                ReportVAT11Ds();
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
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void SendToPrinter()
        {
            foreach (var path in Directory.GetFiles(PrintLocation))
            {
                PrintPDF(path);

                PrintDocument pdoc = new PrintDocument();
                pdoc.PrinterSettings.PrinterName = PrinterName;
                pdoc.DocumentName = path;
                pdoc.Print();
            }
        }

        private void PrintPDF(string fileName)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.Verb = "print";
                info.FileName = fileName;
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;

                Process p = new Process();
                p.StartInfo = info;
                p.Start();

                p.WaitForInputIdle();
                System.Threading.Thread.Sleep(3000);
                if (false == p.CloseMainWindow())
                    p.Kill();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void CreateFolder()
        {
            try
            {
                #region Save & Print in PDF format
                PrintLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBR_Reports\\" + DateTime.Now.ToString("dd-MMM-yyyy hh.mm.ss"));

                if (!Directory.Exists(PrintLocation))
                    Directory.CreateDirectory(PrintLocation);
                #endregion Save & Print in PDF format
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NullCheck()
        {
            try
            {
                SalesInvoiceNo = cmbTransaction.Text;
                if (dtpFromDate.Checked == false)
                {
                    ChallanFromDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    ChallanFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd"); ;
                }
                if (dtpToDate.Checked == false)
                {
                    ChallanToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    ChallanToDate = dtpToDate.Value.AddDays(1).ToString("yyyy-MMM-dd"); ;
                }
                PrintCopy = Convert.ToInt32(numericUDPrintCopy.Value.ToString());
                if (numericUDPrintCopy.Value == 1)
                {
                    if (chk1st.Checked)
                    {
                        PageNo = 1;
                    }
                    else if (chk2nd.Checked)
                    {
                        PageNo = 2;
                    }
                    else if (chk3rd.Checked)
                    {
                        PageNo = 3;
                    }
                }
                else
                {
                    PageNo = 0;
                }
                PrinterName = cmbPrinterName.Text;
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void numericUDPrintCopy_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDPrintCopy.Value == 1)
            {
                lblPrintPage.Visible = true;
                chk1st.Visible = true;
                chk2nd.Visible = true;
                chk3rd.Visible = true;
            }
            else
            {
                lblPrintPage.Visible = false;
                chk1st.Visible = false;
                chk2nd.Visible = false;
                chk3rd.Visible = false;
            }
        }

        private void chk1st_CheckedChanged(object sender, EventArgs e)
        {
            if (chk1st.Checked)
            {
                chk2nd.Checked = false;
                chk3rd.Checked = false;
            }
        }

        private void chk2nd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk2nd.Checked)
            {
                chk1st.Checked = false;
                chk3rd.Checked = false;
            }
        }

        private void chk3rd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk3rd.Checked)
            {
                chk2nd.Checked = false;
                chk1st.Checked = false;
            }
        }

        private void ReportVAT11Ds()
        {
            try
            {
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

                #region Collect invoice no
                if (MultipleReportResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                    return;
                }
                bool isPrinterRunning = false;
                string msg = "Success";
                int saleCount = MultipleReportResult.Tables[0].Rows.Count;
                for (int i = 0; i < MultipleReportResult.Tables[0].Rows.Count; i++)
                {
                    SalesInvoiceNo = MultipleReportResult.Tables[0].Rows[i][0].ToString();
                    varTrading = MultipleReportResult.Tables[0].Rows[i][1].ToString();

                    msg = ExecuteVAT11();
                    if (msg == "Fail")
                    {
                        break;
                    }
                }

                if (msg == "Success")
                {
                    MessageBox.Show("You have successfully print " + saleCount + " Sale invoice(s)");
                }
                else
                {
                    MessageBox.Show("Fail");
                }
                #endregion Collect invoice no

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
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportVAT11Ds", exMessage);
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
                FileLogger.Log(this.Name, "ReportVAT11Ds", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportVAT11Ds", exMessage);
            }

            #endregion

        }

        private string ExecuteVAT11()
        {
            #region Close1
            string msg = "Success";
            #endregion Close1
            try
            {
                //ReportDSDAL showReport = new ReportDSDAL();
                IReport showReport = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
                ReportResultVAT11 = new DataSet();
                ReportResultCreditNote = new DataSet();
                ReportResultDebitNote = new DataSet();
                ReportResultTracking = new DataSet();

                if (Program.IsBureau == true)
                {
                    if (TransactionType == "Credit")
                    {
                        ReportResultCreditNote = showReport.BureauCreditNote(SalesInvoiceNo, post1, post2, connVM);
                    }

                    else
                    {
                        ReportResultVAT11 = showReport.BureauVAT11Report(SalesInvoiceNo, post1, post2, connVM);
                    }

                }
                else
                {

                    if (TransactionType == "Credit")
                    {
                        ReportResultCreditNote = showReport.CreditNoteNew(SalesInvoiceNo, post1, post2, connVM);
                    }
                    else if (TransactionType == "Debit")
                    {
                        ReportResultDebitNote = showReport.DebitNoteNew(SalesInvoiceNo, post1, post2, connVM);
                    }
                    else
                    {
                        ReportResultVAT11 = showReport.VAT6_3(SalesInvoiceNo, varTrading, post1, post2, connVM);
                        ReportResultTracking = showReport.SaleTrackingReport(SalesInvoiceNo, connVM);

                    }
                }
                msg = SavePrintVAT11();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
                msg = "Fail";
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                msg = "Fail";
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                msg = "Fail";
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
                msg = "Fail";
            }

            #endregion
            return msg;
        }

        private string SavePrintVAT11()
        {
            #region Close1
            string msg = "Success";
            #endregion Close1

            #region Settings

            CommonDAL commonDal = new CommonDAL();
            bool VAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter") == "Y" ? true : false;
            bool VAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4") == "Y" ? true : false;
            bool VAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal") == "Y" ? true : false;
            bool VAT11A5 = commonDal.settingsDesktop("Sale", "VAT6_3A5") == "Y" ? true : false;
            bool VAT11English = commonDal.settingsDesktop("Sale", "VAT6_3English") == "Y" ? true : false;
            string VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3");

            #endregion

            try
            {

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                #region Variables
                string vQuantity, vSDAmount, vQty_UnitCost, vQty_UnitCost_SDAmount, vVATAmount, vSubtotal, QtyInWord;// = string.Empty;  

                vQuantity = string.Empty;
                vSDAmount = string.Empty;
                vQty_UnitCost = string.Empty;
                vQty_UnitCost_SDAmount = string.Empty;
                vVATAmount = string.Empty;
                vSubtotal = string.Empty;
                QtyInWord = string.Empty;
                decimal Quantity = 0,
                           SDAmount = 0,
                           Qty_UnitCost = 0,
                           Qty_UnitCost_SDAmount = 0,
                           VATAmount = 0,
                           Subtotal = 0;

                #endregion

                #region Datatable - Table Name


                //start Complete
                if (TransactionType == "Credit")
                {
                    ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";
                }
                else if (TransactionType == "Debit")
                {

                    ReportResultDebitNote.Tables[0].TableName = "DsDebitNote";
                }
                else
                {
                    ReportResultVAT11.Tables[0].TableName = "DsVAT11";
                    #region InWord

                    if (Program.IsBureau == true)
                    {
                        for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                        {
                            Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                        }
                        QtyInWord = Program.NumberToWords(Convert.ToInt32(Quantity));

                        if (Convert.ToInt32(QtyInWord.Length) <= 0)
                        {
                            QtyInWord = "In Words(Quantity): .";

                        }
                        else
                        {
                            QtyInWord = "In Words(Quantity): " + QtyInWord.ToString() + " .";

                        }
                        QtyInWord = QtyInWord.ToUpper();
                    }
                    else
                    {
                        for (int i = 0; i < ReportResultVAT11.Tables[1].Rows.Count; i++)
                        {
                            QtyInWord = QtyInWord + ReportResultVAT11.Tables[1].Rows[i]["uom"].ToString() + ": " +
                                        Program.NumberToWords(Convert.ToInt32(ReportResultVAT11.Tables[1].Rows[i]["qty"])) + ", ";
                        }
                        if (Convert.ToInt32(QtyInWord.Length) <= 0)
                        {
                            QtyInWord = "In Words(Quantity): .";

                        }
                        else
                        {
                            QtyInWord = "In Words(Quantity): " +
                                        QtyInWord.Substring(0, Convert.ToInt32(QtyInWord.Length) - 2).ToString() + ".";

                        }
                        QtyInWord = QtyInWord.ToUpper();

                    }
                    for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    {
                        Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                        SDAmount = SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        Qty_UnitCost = Qty_UnitCost + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]);
                        Qty_UnitCost_SDAmount = Qty_UnitCost_SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]) + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        VATAmount = VATAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["VATAmount"]);
                        Subtotal = Subtotal + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Subtotal"]);
                    }

                    vQuantity = Convert.ToDecimal(Quantity).ToString("0,0.00");
                    vSDAmount = Convert.ToDecimal(SDAmount).ToString("0,0.00");
                    vQty_UnitCost = Convert.ToDecimal(Qty_UnitCost).ToString("0,0.00");
                    vQty_UnitCost_SDAmount = Convert.ToDecimal(Qty_UnitCost_SDAmount).ToString("0,0.00");
                    vVATAmount = Convert.ToDecimal(VATAmount).ToString("0,0.00");
                    vSubtotal = Convert.ToDecimal(Subtotal).ToString("0,0.00");


                    #endregion InWord


                }
                #endregion

                string prefix = "";
                ReportDocument objrpt = new ReportDocument();

                #region Comments


                ////#region CN

                ////if (TransactionType == "Credit")
                ////{

                ////    objrpt = new RptCreditNote();
                ////    prefix = "CreditNote";

                ////    objrpt.SetDataSource(ReportResultCreditNote);


                ////    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                ////    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";


                ////}
                ////#endregion CN
                ////#region DN
                ////else if (TransactionType == "Debit")
                ////{
                ////    objrpt = new RptDebitNote();
                ////    prefix = "DebitNote";


                ////    objrpt.SetDataSource(ReportResultDebitNote);
                ////    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                ////    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                ////}
                ////#endregion CN
                ////#region rbtnTrading
                ////else if (TransactionType == "Trading")
                ////{

                ////    objrpt = new RptVAT11Ka();
                ////    prefix = "VAT11Ka";

                ////    objrpt.SetDataSource(ReportResultVAT11);
                ////    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                ////}//end if
                ////#endregion rbtnTrading
                ////#region rbtnService
                ////else if (TransactionType == "Service" || TransactionType == "ServiceNS")
                ////{
                ////    objrpt = new RptVAT11();
                ////    prefix = "VAT11";
                ////    objrpt.SetDataSource(ReportResultVAT11);
                ////    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                ////    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11 Gha'";
                ////    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                ////    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo +
                ////                                                                    "'";

                ////    #region Pass value to report

                ////    objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";
                ////    objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";

                ////    objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                ////    objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                ////    objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                ////    objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                ////    objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                ////    objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";

                ////    #endregion Pass value to report


                ////}
                ////#endregion rbtnService
                ////#region rbtnTollIssue
                //////else if (rbtnTollIssue.Checked)
                //////{
                //////    objrpt = new RptVAT11Ga();
                //////    prefix = "VAT11Ga";
                //////    objrpt.SetDataSource(ReportResultVAT11);
                //////    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //////    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11 Ga'";
                //////    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //////    //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //////    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //////    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //////    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //////    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //////    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //////    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo +
                //////                                                                    "'";
                //////}
                ////#endregion rbtnTollIssue
                ////#region rbtnVAT11GaGa
                ////else if (TransactionType == "VAT11GaGa")
                ////{
                ////    objrpt = new RptVAT11GaGa();
                ////    prefix = "VAT11GaGa";
                ////    objrpt.SetDataSource(ReportResultVAT11);
                ////    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                ////}
                ////#endregion rbtnVAT11GaGa

                #endregion

                #region Other
                ////else
                ////{
                prefix = "VAT11";

                #region Comments

                //////if (chkIsBlank.Checked)
                //////{
                //////    if (TrackingTrace == true)
                //////    {
                //////        objrpt = new RptVAT11T_Blank();
                //////    }
                //////    else
                //////    {
                //////        objrpt = new RptVAT11_Blank();
                //////    }
                //////}
                //////else if (VAT11PageSize == "A4")
                //////{
                //////    if (TrackingTrace == true)
                //////    {
                //////        objrpt = new RptVAT11T();
                //////    }
                //////    else
                //////    {
                //////        objrpt = new RptVAT11();
                //////    }
                //////}
                //////else if (VAT11PageSize == "Letter")
                //////{

                //////    if (TrackingTrace == true)
                //////    {
                //////        objrpt = new RptVAT11LetterT();
                //////    }
                //////    else
                //////    {
                //////        objrpt = new RptVAT11Letter();
                //////    }
                //////}
                //////else if (VAT11PageSize == "Legal")
                //////{

                //////    if (TrackingTrace == true)
                //////    {
                //////        objrpt = new RptVAT11Legal();
                //////    }
                //////    else
                //////    {
                //////        objrpt = new RptVAT11LegalT();
                //////    }
                //////}
                //////else
                //////{
                //////    if (TrackingTrace == true)
                //////    {
                //////        objrpt = new RptVAT11T();
                //////    }
                //////    else
                //////    {
                //////        objrpt = new RptVAT11();
                //////    }
                //////}

                #endregion


                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    //////objrpt = new ReportDocument();
                    //////objrpt.Load(Program.ReportAppPath + @"\RptVAT6_3.rpt");
                    objrpt = new ReportDocument();

                    objrpt = new RptVAT6_3();

                    switch (VAT11PageSize)
                    {

                        case "A4": objrpt = new RptVAT6_3_A4(); break;
                        case "Letter": objrpt = new RptVAT6_3(); break;
                        case "Legal": objrpt = new RptVAT6_3(); break;
                        case "A5": objrpt = new RptVAT6_3_A5(); break;
                        default: objrpt = new RptVAT6_3(); break;
                    }

                    if (VAT11English == true)
                    {
                        objrpt = new ReportDocument();
                        objrpt = new RptVAT6_3_English();
                    }
                    if (VAT11Name.ToLower() == "scbl")
                    {
                        UserInformationVM uvm = new UserInformationVM();
                        //UserInformationDAL _udal = new UserInformationDAL();

                        IUserInformation _udal = OrdinaryVATDesktop.GetObject<UserInformationDAL, UserInformationRepo, IUserInformation>(OrdinaryVATDesktop.IsWCF);
                        uvm = _udal.SelectAll(Convert.ToInt32(Program.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();


                        objrpt = new ReportDocument();

                        objrpt = new RptVAT6_3_A5_SCBL();

                        objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                        objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";

                        //}
                    }
                    else if (VAT11Name.ToLower() == "cp")
                    {
                        objrpt = new ReportDocument();
                        if (chkIsBlank.Checked)
                        {
                            objrpt = new RptVAT6_3_English_CP();
                        }
                        else
                        {
                            objrpt = new RptVAT6_3_English();
                        }
                        //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_3_English_CP.rpt");
                    }
                    else if (VAT11Name.ToLower() == "pccl")
                    {
                        objrpt = new ReportDocument();
                        objrpt = new RptVAT6_3_Padma();

                        //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_3_Padma.rpt");
                    }
                    else if (VAT11Name.ToLower() == "bvcps")
                    {
                        objrpt = new ReportDocument();
                        objrpt = new RptVAT6_3_BVCPS();
                    }


                    objrpt.DataDefinition.FormulaFields["frmTotalVDSAmount"].Text = "'" + "0" + "'";
                    objrpt.DataDefinition.FormulaFields["PGroupInReport"].Text = "'" + "n" + "'";

                    objrpt.DataDefinition.FormulaFields["TrackingTrace"].Text = "'" + TrackingTrace + "'";

                }

                #region Formula Fields

                objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";
                objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";

                objrpt.SetDataSource(ReportResultVAT11);
                if (TrackingTrace == true)
                {
                    objrpt.Subreports[0].SetDataSource(ReportResultTracking);
                    objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName1"].Text = "'" + Heading1 + "'";
                    objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName2"].Text = "'" + Heading2 + "'";
                }

                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";

                #endregion

                ////}
                #endregion Other

                #region currency
                //SaleDAL saleDal = new SaleDAL();
                ISale saleDal = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);

                string currencyMajor = "";
                string currencyMinor = "";
                string currencySymbol = "";
                #region Bureau
                if (Program.IsBureau == true)
                {
                    currencyMajor = "taka";
                    currencyMinor = "paisa";
                    currencySymbol = "tk";
                }
                else
                {

                    sqlResults = new string[4];

                    sqlResults = saleDal.CurrencyInfo(SalesInvoiceNo, connVM);
                    if (sqlResults[0].ToString() != "Fail")
                    {
                        currencyMajor = sqlResults[1].ToString();
                        currencyMinor = sqlResults[2].ToString();
                        currencySymbol = sqlResults[3].ToString();
                    }
                }
                #endregion Bureau
                #endregion currency

                #region Formula Fields

                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                objrpt.DataDefinition.FormulaFields["CurrencyMajor"].Text = "'" + currencyMajor + "'";
                objrpt.DataDefinition.FormulaFields["CurrencyMinor"].Text = "'" + currencyMinor + "'";
                objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "'" + currencySymbol + "'";

                #endregion

                #region Report Printing

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                    objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "''";

                    reports.setReportSource(objrpt);
                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                        //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {
                    #region PrintCopy

                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";

                        string copiesNo = "";
                        int cpno = 0;
                        if (PrintCopy == 1 || PrintCopy == 0)
                        {
                            copiesNo = PageNo.ToString();
                            cpno = PageNo;
                        }
                        else
                        {
                            copiesNo = i.ToString();
                            cpno = i;
                        }


                        #region CopyName

                        if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                            (cpno >= 44 && cpno <= 50))
                        {
                            copiesNo = cpno + " th copy";
                        }
                        else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                        {
                            copiesNo = cpno + " st copy";
                        }
                        else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                        {
                            copiesNo = cpno + " nd copy";
                        }
                        else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                        {
                            copiesNo = cpno + " rd copy";
                        }
                        else
                        {

                            copiesNo = cpno + " copy";

                        }



                        #endregion CopyName

                        if (Is3Plyer == "Y")
                        {
                            objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "";
                        }
                        else
                        {
                            objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                        }


                        #region Save & Print in PDF format
                        // // print
                        ReportDocument rptDoc = objrpt;
                        rptDoc.PrintOptions.PrinterName = PrinterName;
                        rptDoc.PrintToPrinter(1, false, 1, 1);

                        // // Pdf Save
                        string printInvoiceNo = SalesInvoiceNo.Replace("/", "-");
                        string saveLocation = PrintLocation + @"\" + prefix + "_" + printInvoiceNo + "_" + copiesNo +
                                              ".pdf";

                        rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, saveLocation);
                        Application.DoEvents();



                        #endregion Save & Print in PDF format

                        #region Update ISPrint='Y'

                        //SaleDAL PrintUpdate = new SaleDAL();
                        ISale PrintUpdate = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);
                        sqlResults = PrintUpdate.UpdatePrintNew(SalesInvoiceNo, PrintCopy, connVM);// Change 04

                        #endregion
                    }
                    #endregion

                }
                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg = "Fail";
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", exMessage);
                msg = "Fail";
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                msg = "Fail";
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                msg = "Fail";
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", exMessage);
                msg = "Fail";
            }

            #endregion
            return msg;
        }


        #endregion

        #region TransactionTypes - Backup
        ////        Local
        ////Trading-Local
        ////Trading-Export
        ////Export
        ////Tender-Local
        ////Tender-Export
        ////Tender(Trading)-Local
        ////Tender(Trading)-Export
        ////Service(Stock)-Local
        ////Service(Stock)-Export
        ////Service(N Stock)-Local
        ////Service(N Stock)-Export
        ////Service(N Stock)-Export Credit
        ////Package-Local
        ////Package-Export
        ////Credit
        ////Debit
        ////VAT11GaGa


        #endregion

        private void cmbTransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTransaction.Text == "Credit")
            {
                TransactionType = "Credit";
            }
            else if (cmbTransaction.Text == "Debit")
            {
                TransactionType = "Debit";
            }
            else if (cmbTransaction.Text == "VAT11GaGa")
            {
                TransactionType = "VAT11GaGa";
            }
            else if (cmbTransaction.Text == "Trading-Local" || cmbTransaction.Text == "Trading")
            {
                TransactionType = "Trading";
            }
            else if (cmbTransaction.Text == "Trading-Export")
            {
                TransactionType = "ExportTrading";
            }
            else if (cmbTransaction.Text == "Service(Stock)-Local" || cmbTransaction.Text == "Service(Stock)")
            {
                TransactionType = "Service";
            }
            else if (cmbTransaction.Text == "Service(Stock)-Export")
            {
                TransactionType = "ExportService";
            }
            else if (cmbTransaction.Text == "Service(N Stock)-Local" || cmbTransaction.Text == "Service(NonStock)")
            {
                TransactionType = "ServiceNS";
            }
            else if (cmbTransaction.Text == "Service(N Stock)-Export")
            {
                TransactionType = "ExportServiceNS";
            }
            else if (cmbTransaction.Text == "Service(N Stock)-Export Credit")
            {
                TransactionType = "ExportServiceNSCredit ";
            }
            else if (cmbTransaction.Text == "Local")
            {
                TransactionType = "Other";
            }
            else if (cmbTransaction.Text == "Export")
            {
                TransactionType = "Export";
            }
            else if (cmbTransaction.Text == "Tender-Local")
            {
                TransactionType = "Tender";
            }
            else if (cmbTransaction.Text == "Tender-Export")
            {
                TransactionType = "ExportTender";
            }
            else if (cmbTransaction.Text == "Tender(Trading)-Local")
            {
                TransactionType = "TradingTender";
            }
            else if (cmbTransaction.Text == "Tender(Trading)-Export")
            {
                TransactionType = "ExportTradingTender";
            }
            else if (cmbTransaction.Text == "Package-Local")
            {
                TransactionType = "PackageSale";
            }
            else if (cmbTransaction.Text == "Package-Export")
            {
                TransactionType = "ExportPackage";
            }
            else if (cmbTransaction.Text == "TollIssue")
            {
                TransactionType = "TollIssue";
            }
            else if (cmbTransaction.Text == "TollFinishIssue")
            {
                TransactionType = "TollFinishIssue";
            }
            else if (cmbTransaction.Text == "InternalIssue")
            {
                TransactionType = "InternalIssue";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cmbTransaction.Text = "Local";
            dtpFromDate.Value = DateTime.Now.Date;
            dtpToDate.Value = DateTime.Now.Date;
            cmbPrinterName.Text = vPrinterName;
            numericUDPrintCopy.Value = Convert.ToInt32(vPrintCopy);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                RecordCount = cmbRecordCount.Text.Trim();

                SearchData();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SearchData()
        {
            try
            {

                NullCheck();
                string[] cFields = {
                                       "sih.SalesInvoiceNo like"
                                      ,"sih.InvoiceDateTime>="
                                      ,"sih.InvoiceDateTime<"
                                      ,"sih.TransactionType like"
                                      ////,"sih.IsPrint like"
                                      ,"sih.post like"
                                      ,"sih.BranchId"
                                      ,"SelectTop"
                                     };


                string[] cValues = { 
                                      txtSalesInvoiceNo.Text.Trim()
                                      ,ChallanFromDate
                                      ,ChallanToDate
                                      ,TransactionType
                                      ////,"N"
                                      ,"Y"
                                      ,BranchId.ToString()
                                      ////,"All"
                                      ,RecordCount
                                      
                                };


                DataTable dtSales = new DataTable();
                //SaleDAL _SaleDAL = new SaleDAL();

                ISale _SaleDAL = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);
                dtSales = _SaleDAL.SelectAll(0, cFields, cValues, null, null, null, true, null, "Y", connVM);



                DataView dView = new DataView(dtSales);
                dtSales = new DataTable();
                dtSales = dView.ToTable("NewTable", false, "Id", "SalesInvoiceNo", "CustomerName", "InvoiceDateTime", "IsPrint", "Trading");



                #region Load DGV

                dgvSales.Rows.Clear();


                int j = 0;
                int rowsCount = dtSales.Rows.Count - 1;
                for (var index = 0; index < rowsCount; index++)
                {
                    DataRow item = dtSales.Rows[index];
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSales.Rows.Add(NewRow);
                    dgvSales.Rows[j].Cells["ID"].Value = item["Id"].ToString();
                    dgvSales.Rows[j].Cells["InvoiceNo"].Value = item["SalesInvoiceNo"].ToString();
                    dgvSales.Rows[j].Cells["CustomerName"].Value = item["CustomerName"].ToString();
                    dgvSales.Rows[j].Cells["InvoiceDate"].Value = item["InvoiceDateTime"].ToString();
                    dgvSales.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                    dgvSales.Rows[j].Cells["IsPrint"].Value = item["IsPrint"].ToString();

                    j++;
                }

                #endregion




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
                MessageBox.Show(ex.Message, "FormMultipleVAT11", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchData", exMessage);
            }
            #endregion Catch

        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {

            SelectAll(dgvSales);

        }

        public void SelectAll(DataGridView dgv)
        {
            bool IsChecked = chkSelectAll.Checked;

            for (int i = 0; i < dgv.RowCount; i++)
            {
                dgv["Select", i].Value = IsChecked;
            }

        }

        private void btnSelectedPrint_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                PreviewOnly = false;

                Print_Or_Preview_Progress();

                {

                    #region Comments Mar-15-2020

                    //////msg = ExecuteVAT11();
                    //////if (msg == "Fail")
                    //////{
                    //////    break;
                    //////}

                    #endregion

                }

                progressBar1.Visible = false;



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
                MessageBox.Show(ex.Message, "btnSelectedPrint_Click", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSelectedPrint_Click", exMessage);
            }
            #endregion Catch

        }


        public void Print_Or_Preview_Progress()
        {

            try
            {
                SaleReport _report = new SaleReport();

                List<int> IndexList = new List<int>();
                string MultipleSalesInvoiceNo = "";

                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                    {
                        IndexList.Add(row.Index);
                    }
                }


                IsCreditNote = false;
                IsDebitNote = false;

                if (TransactionType == "Credit")
                {
                    IsCreditNote = true;
                }
                if (TransactionType == "Debit")
                {
                    IsDebitNote = true;
                }


                if (PreviewOnly == true)
                {
                    #region Preview
                    MultipleSalesInvoiceNo = "";
                    foreach (int Index in IndexList)
                    {
                        SalesInvoiceNo = dgvSales.Rows[Index].Cells["InvoiceNo"].Value.ToString();
                        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + SalesInvoiceNo;
                    }
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", "','");
                    MultipleSalesInvoiceNo = "'" + MultipleSalesInvoiceNo + "'";

                    reportDocument = new ReportDocument();

                    reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
                        , IsCreditNote
                        , IsDebitNote
                        , IsRawCreditNote
                        , IsTrading
                        , IsTollIssue
                        , IsVAT11GaGa
                        , PreviewOnly, PrintCopy, AlReadyPrintNo, chkIsBlank.Checked, Is11, IsValueOnly
                        , IsCommercialImporter, false, false, connVM);

                    Print_Or_Preview();



                    #endregion
                }
                else
                {
                    #region Print

                    foreach (int Index in IndexList)
                    {

                        SalesInvoiceNo = dgvSales.Rows[Index].Cells["InvoiceNo"].Value.ToString();
                        varTrading = dgvSales.Rows[Index].Cells["Trading"].Value.ToString();


                        reportDocument = new ReportDocument();

                        reportDocument = _report.Report_VAT6_3_Completed(SalesInvoiceNo, TransactionType
                            , IsCreditNote
                            , IsDebitNote
                            , IsRawCreditNote
                            , IsTrading
                            , IsTollIssue
                            , IsVAT11GaGa
                            , PreviewOnly, PrintCopy, AlReadyPrintNo, chkIsBlank.Checked, Is11, IsValueOnly
                            , IsCommercialImporter, false, false, connVM);

                        Print_Or_Preview();

                    }
                    MessageBox.Show("You have successfully print " + IndexList.Count() + " Invoice");

                    #endregion

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
                MessageBox.Show(ex.Message, "Print_Or_Preview_Progress", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Print_Or_Preview_Progress", exMessage);
            }
            #endregion Catch


        }

        public void Print_Or_Preview()
        {
            try
            {
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }
                //start Complete
                if (PreviewOnly)
                {
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    reports.Show();
                }
                else
                {
                    DataSet ds = new DataSet();
                    int AlreadyPrint = 0;
                    if (Program.IsBureau == true)
                    {
                        ds = new ReportDSDAL().BureauVAT11Report(SalesInvoiceNo, "Y", "N", connVM);
                        AlreadyPrint = Convert.ToInt32(ds.Tables[0].Rows[0]["AlreadyPrint"]);
                    }
                    string PrintPage = "";
                    int j = 0;
                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        j = AlreadyPrint - PrintCopy + i;
                        //////if (i == 1) PrintPage = "1 st Copy";
                        //////else if (i == 2) PrintPage = "2 nd Copy";
                        //////else if (i == 3) PrintPage = "3 rd Copy";

                        //////crFormulaF = reportDocument.DataDefinition.FormulaFields;
                        //////_vCommonFormMethod = new CommonFormMethod();
                        //////_vCommonFormMethod.FormulaField(reportDocument, crFormulaF, "PrintPage", PrintPage);




                        //////reportDocument.PrintOptions.PrinterName = PrinterName;
                        //////reportDocument.PrintToPrinter(1, false, 0, 0);


                        System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                        printerSettings.PrinterName = PrinterName;
                        if (Program.IsBureau == true)
                        {

                            FormulaFieldDefinitions crFormulaF;

                            crFormulaF = reportDocument.DataDefinition.FormulaFields;
                            _vCommonFormMethod = new CommonFormMethod();
                            _vCommonFormMethod.FormulaField(reportDocument, crFormulaF, "CurrentPrintCopy", j.ToString());


                        }

                        reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

                    }


                }
                //MessageBox.Show("You have successfully print " + PrintCopy + " Copy(s)");

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
                FileLogger.Log(this.Name, "Print_Or_Preview", exMessage);
            }

            #endregion
            finally
            {
                #region Button Stats
                this.progressBar1.Visible = false;
                #endregion
                if (!PreviewOnly)
                {
                    if (reportDocument != null)
                    {
                        reportDocument.Close();
                        reportDocument.Dispose();
                    }

                }

            }


        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {

                progressBar1.Visible = true;

                PreviewOnly = true;

                Print_Or_Preview_Progress();

                progressBar1.Visible = false;



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
                MessageBox.Show(ex.Message, "btnPreview_Click", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            #endregion Catch
        }

        private void cmbPrinterName_Leave(object sender, EventArgs e)
        {
            PrinterName = cmbPrinterName.Text;
        }

        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            try
            {

                progressBar1.Visible = true;

                PreviewOnly = true;
                PrintAll = true;

                PrintAll_Progress();

                progressBar1.Visible = false;



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
                MessageBox.Show(ex.Message, "btnPrintAll_Click", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintAll_Click", exMessage);
            }
            #endregion Catch
        }

        public void PrintAll_Progress()
        {

            try
            {
                SaleReport _report = new SaleReport();

                List<int> IndexList = new List<int>();
                string MultipleSalesInvoiceNo = "";

                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                    {
                        IndexList.Add(row.Index);
                    }
                }


                IsCreditNote = false;
                IsDebitNote = false;

                if (TransactionType == "Credit")
                {
                    IsCreditNote = true;
                }
                if (TransactionType == "Debit")
                {
                    IsDebitNote = true;
                }

                #region Preview
                MultipleSalesInvoiceNo = "";
                foreach (int Index in IndexList)
                {
                    SalesInvoiceNo = dgvSales.Rows[Index].Cells["InvoiceNo"].Value.ToString();
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + SalesInvoiceNo;
                }
                MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
                MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", "','");
                MultipleSalesInvoiceNo = "'" + MultipleSalesInvoiceNo + "'";

                reportDocument = new ReportDocument();

                reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
                    , IsCreditNote
                    , IsDebitNote
                    , IsRawCreditNote
                    , IsTrading
                    , IsTollIssue
                    , IsVAT11GaGa
                    , PreviewOnly, PrintCopy, AlReadyPrintNo, chkIsBlank.Checked, Is11, IsValueOnly
                    , IsCommercialImporter, false, false, connVM, "", false, false, true);

                PrintAll_Preview();

                #endregion

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
                MessageBox.Show(ex.Message, "PrintAll_Progress", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PrintAll_Progress", exMessage);
            }
            #endregion Catch


        }

        public void PrintAll_Preview()
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
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                reports.Show();

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
                FileLogger.Log(this.Name, "PrintAll_Preview", exMessage);
            }

            #endregion

            finally
            {
                #region Button Stats

                this.progressBar1.Visible = false;

                #endregion

                if (!PreviewOnly)
                {
                    if (reportDocument != null)
                    {
                        reportDocument.Close();
                        reportDocument.Dispose();
                    }

                }

            }


        }

    }
}
