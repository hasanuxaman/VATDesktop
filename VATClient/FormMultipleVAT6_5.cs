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
    public partial class FormMultipleVAT6_5 : Form
    {
        #region Global Variable

        #region New Variables

        bool IsCreditNote = false;
        bool IsDebitNote = false;
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
        private string transactiontype = String.Empty;

        private DataSet ReportResultTracking;
        private bool TrackingTrace;
        private string Heading1 = string.Empty;
        private string Heading2 = string.Empty;

        private int BranchId = 0;
        private int TransferTo = 0;
        private int SenderBranchId = 0;
        #endregion

        public FormMultipleVAT6_5()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        private void FormLoad()
        {
            try
            {

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                string[] condition = new string[] { "ActiveStatus='Y' AND BranchID NOT IN(" + Program.BranchId + ")" };

                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true);
                cmbTransferTo = new CommonDAL().ComboBoxLoad(cmbTransferTo, "BranchProfiles", "BranchID", "BranchName", condition, "varchar", true);

                if (SenderBranchId > 0)
                {
                    cmbBranch.SelectedValue = SenderBranchId;
                }
                else
                {
                    cmbBranch.SelectedValue = Program.BranchId;

                }
                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth");
                cmbTransferTo.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                comtt.Text = "=All=";
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
                //if (Program.IsBureau == true)
                //{
                //    cmbTransaction.Text = "Service(N Stock)-Local";
                //}
                //else
                //{
                //    cmbTransaction.Text = "Local";
                //}

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
                FormMultipleVAT6_5 frmSalePrint = new FormMultipleVAT6_5();
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



   

 

        private void NullCheck()
        {
            try
            {
                //SalesInvoiceNo = cmbTransaction.Text;
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

        private void FormMultipleVAT11_Load(object sender, EventArgs e)
        {
            FormLoad();

        }

        #endregion

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //cmbTransaction.Text = "Local";
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
                TransferTo = Convert.ToInt32(cmbTransferTo.SelectedValue);
                //6.1 Out
                 //61Out
                string TType = comtt.Text;
                TType=TType.Replace(".","");
                TType = TType.Replace(" ", "");
                if (TType=="=All=")
                {
                    TType = "";
                }
                transactiontype = TType;
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
                //string[] cFields = {
                //                       "ti.TransferIssueNo like"
                //                      ,"ti.InvoiceDateTime>="
                //                      ,"ti.InvoiceDateTime<"
                //                      ////,"sih.IsPrint like"
                //                      ,"ti.TransactionType like"
                //                      ,"ti.post like"
                //                      ,"ti.BranchId"
                //                      ,"ti.TransferTo"
                //                     };


                //string[] cValues = { 
                //                      txtSalesInvoiceNo.Text.Trim()
                //                      ,ChallanFromDate
                //                      ,ChallanToDate
                //                      ////,"N"
                //                      ,TransactionType
                //                      ,"Y"
                //                      ,BranchId.ToString()
                //                      ,TransferTo.ToString()
                //                };
                DataTable IssueResult = new DataTable();

                TransferIssueDAL issueDal = new TransferIssueDAL();

                TransferIssueVM vm = new TransferIssueVM();
                vm.TransferIssueNo = txtSalesInvoiceNo.Text.Trim();
                vm.IssueDateFrom = ChallanFromDate;
                vm.IssueDateTo = ChallanToDate;
                vm.Post = "Y";
                vm.ReferenceNo = "";
                vm.TransactionType = transactiontype;
                vm.BranchId = BranchId;//// Convert.ToInt32(cmbBranch.SelectedValue);
                vm.TransferTo = TransferTo;
                IssueResult = issueDal.SearchTransferIssue(vm, null, null, connVM);
                //SaleDAL _SaleDAL = new SaleDAL();

                //ISale _SaleDAL = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);
                ////IssueResult = _SaleDAL.SearchTransferIssue(0, cFields, cValues, null, null, null, true, connVM);

                DataView dView = new DataView(IssueResult);
                IssueResult = new DataTable();
                IssueResult = dView.ToTable("NewTable", false, "Id", "TransferIssueNo", "IssueDateTime", "BranchName", "BranchNameFrom");



                #region Load DGV

                dgvIssue.Rows.Clear();


                int j = 0;
                foreach (DataRow item in IssueResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvIssue.Rows.Add(NewRow);
                   dgvIssue.Rows[j].Cells["ID"].Value = item["Id"].ToString();
                   dgvIssue.Rows[j].Cells["TransferIssueNo"].Value = item["TransferIssueNo"].ToString();
                    dgvIssue.Rows[j].Cells["TransactionDateTime"].Value = item["IssueDateTime"].ToString();
                    dgvIssue.Rows[j].Cells["BranchName"].Value = item["BranchNameFrom"].ToString();
                    dgvIssue.Rows[j].Cells["TransferBranchName"].Value = item["BranchName"].ToString();

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

            SelectAll(dgvIssue);

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
                NBRReports _report = new NBRReports();

                List<int> IndexList = new List<int>();
                string MultipleSalesInvoiceNo = "";

                foreach (DataGridViewRow row in dgvIssue.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                    {
                        IndexList.Add(row.Index);
                    }
                }


                //IsCreditNote = false;
                //IsDebitNote = false;

                ////if (TransactionType == "Credit")
                //{
                //    IsCreditNote = true;
                //}
                //if (TransactionType == "Debit")
                //{
                //    IsDebitNote = true;
                //}


                if (PreviewOnly == true)
                {
                    #region Preview
                    MultipleSalesInvoiceNo = "";
                    foreach (int Index in IndexList)
                    {
                        SalesInvoiceNo = dgvIssue.Rows[Index].Cells["TransferIssueNo"].Value.ToString();
                        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + SalesInvoiceNo;
                    }
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", "','");
                    MultipleSalesInvoiceNo = "'" + MultipleSalesInvoiceNo + "'";

                    reportDocument = new ReportDocument();
                    reportDocument = _report.TransferIssueNew(MultipleSalesInvoiceNo, "", "", "", "", "", "", "", PreviewOnly,null,connVM);
                    //reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
                    //    , IsCreditNote
                    //    , IsDebitNote
                    //    , IsTrading
                    //    , IsTollIssue
                    //    , IsVAT11GaGa
                    //    , PreviewOnly, PrintCopy, AlReadyPrintNo, chkIsBlank.Checked, Is11, IsValueOnly
                    //    , IsCommercialImporter);


                    Print_Or_Preview();


                    #endregion
                }
                else
                {
                    #region Print
                    
                    foreach (int Index in IndexList)
                    {

                        SalesInvoiceNo = dgvIssue.Rows[Index].Cells["TransferIssueNo"].Value.ToString();
                        //varTrading = dgvIssue.Rows[Index].Cells["Trading"].Value.ToString();


                        reportDocument = new ReportDocument();
                        reportDocument = _report.TransferIssueNew(SalesInvoiceNo, "", "", "", "", "", "", "", PreviewOnly,null,connVM);

                        //reportDocument = _report.Report_VAT6_3_Completed(SalesInvoiceNo, TransactionType
                        //    , IsCreditNote
                        //    , IsDebitNote
                        //    , IsTrading
                        //    , IsTollIssue
                        //    , IsVAT11GaGa
                        //    , PreviewOnly, PrintCopy, AlReadyPrintNo, chkIsBlank.Checked, Is11, IsValueOnly
                        //    , IsCommercialImporter);

                        Print_Or_Preview();

                    }
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

                    string PrintPage = "";
                    for (int i = 1; i <= PrintCopy; i++)
                    {
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
                        reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

                    }

                    MessageBox.Show("You have successfully print " + PrintCopy + " Copy(s)");

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
                FileLogger.Log(this.Name, "Print_Or_Preview", exMessage);
            }

            #endregion


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

    }
}
