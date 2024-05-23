using CrystalDecisions.CrystalReports.Engine;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymphonySofttech.Reports;
using SymphonySofttech.Reports.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.Shared;
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.License;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormNestleSaleDetails : Form
    {
        public FormNestleSaleDetails()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }
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
        DataGridViewRow selectedRow = new DataGridViewRow();

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
        #endregion

        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        //ReportDSDAL _ReportDSDAL = new ReportDSDAL();
        NESTLEIntegrationDAL _ReportDSDAL = new NESTLEIntegrationDAL();
        IntegrationParam paramVM = new IntegrationParam();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string InvoiceNo;
        private DataTable SaleDetailResult;
        List<int> IndexList = new List<int>();



        private void FormSubForm_Load(object sender, EventArgs e)
        {
            try
            {
                #region Settings
                CommonDAL commonDal = new CommonDAL();
                vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter",null,connVM);
                vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4",null,connVM);
                vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal",null,connVM);
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint",null,connVM);
                vMaxNoOfPrint = commonDal.settingsDesktop("Printer", "MaxNoOfPrint",null,connVM);
                vItemNature = commonDal.settingsDesktop("Sale", "ItemNature",null,connVM);
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint",null,connVM);
                vPrinterName = commonDal.settingsDesktop("Printer", "DefaultPrinter",null,connVM);
                vIs3Plyer = commonDal.settingsDesktop("Sale", "Page3Plyer",null,connVM);

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
                #endregion Set Printer Name

                if (!string.IsNullOrWhiteSpace(txtChalanNo.Text.Trim()))
                {
                    Search(txtChalanNo.Text.Trim());

                    #region Statement
                    // Start Complete
                    dgvSales.DataSource = null;
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        System.Data.DataColumn newColumn = new System.Data.DataColumn("Select", typeof(Boolean));
                       
                        newColumn.DefaultValue = false;
                        dt.Columns.Add(newColumn);
                        newColumn.SetOrdinal(0);
                        DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                        dgvSales.Columns.Add(btn);
                        btn.HeaderText = "Drill Down";
                        btn.Name = "btn";
                        btn.UseColumnTextForButtonValue = true;
                        dgvSales.DataSource = dt;

                        dgvSales.Columns[0].Width =50;
                        dgvSales.Columns[1].Width = 50;
                        dgvSales.Columns[2].Width = 150;

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSubForm_Load", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }
        }

        private void Search(string  ChalanNo)
        {
            try
            {

                dt = new DataTable();
              
                IntegrationParam vm = new IntegrationParam();
                vm.RefNo = ChalanNo;
                vm.TransactionType="Other";
                dt = _ReportDSDAL.GetSource_SaleData_dis_Details(vm,connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int RowCount = dt.Rows.Count;
                    lblRowCount.Text = "Total Invoices: " + RowCount;

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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(cmbNoteNo.Text.Trim()))
                //{
                //    MessageBox.Show("Select Note No. First!", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Warning);
                //    return;
                //}

                //Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion


        }

        private void btnload_Click(object sender, EventArgs e)
        {
            try
            {

                //Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

                //dgvSubForm.DataSource = dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }

        }


        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvSales.RowCount; i++)
            {
                dgvSales["Select", i].Value = chkSelectAll.Checked;
            }
        }

        

        private void bgwChallanSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDAL saleDal = new SaleDAL();
                SaleDetailResult = saleDal.SearchSaleDetailDTNew(InvoiceNo, connVM);
                DataView view = new DataView(SaleDetailResult);

                dt = new DataTable();
                dt = view.ToTable(false, "ProductName", "ProductCode", "UOM","Quantity","Weight", "BatchNo", "VATRate", "NBRPrice", "SubTotal");
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
                FileLogger.Log(this.Name, "bgwChallanSearch_DoWork", exMessage);
            }

            #endregion

        }

        private void bgwChallanSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                dgvLoadedTable.DataSource = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    LRecordCount.Text = "Details Count: " + Convert.ToString(dt.Rows.Count);

                    dgvLoadedTable.DataSource = dt;
                    dgvLoadedTable.Columns[0].Width = 250;

                }



                // End Complete

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
                FileLogger.Log(this.Name, "bgwChallanSearch_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                #region Button Stats
                this.progressBar1.Visible = false;
                #endregion
            }

        }

        private void dgvSubForm_DoubleClick(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            InvoiceNo = dgvSales.CurrentRow.Cells["SalesInvoiceNo"].Value.ToString();
            bgwChallanSearch.RunWorkerAsync();
        }

        #region Old Print And Preview 
        //public void Print_Or_Preview_Progress()
        //{

        //    try
        //    {
        //        SaleReport _report = new SaleReport();

        //        string MultipleSalesInvoiceNo = "";

        //        foreach (DataGridViewRow row in dgvSales.Rows)
        //        {
        //            if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
        //            {
        //                IndexList.Add(row.Index);
        //            }
        //        }

        //        if (IndexList.Count <1)
        //        {
        //            MessageBox.Show(this, "Please Select Invoice First!");
        //            return;
        //        }
        //        IsCreditNote = false;
        //        IsDebitNote = false;

                


        //        if (PreviewOnly == true)
        //        {
        //            #region Preview
        //            MultipleSalesInvoiceNo = "";
        //            foreach (int Index in IndexList)
        //            {
        //                SalesInvoiceNo = dgvSales.Rows[Index].Cells["SalesInvoiceNo"].Value.ToString();
        //                MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + SalesInvoiceNo;
        //            }
        //            MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
        //            MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", "','");
        //            MultipleSalesInvoiceNo = "'" + MultipleSalesInvoiceNo + "'";

        //            reportDocument = new ReportDocument();

        //            reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
        //                , IsCreditNote
        //                , IsDebitNote
        //                , IsRawCreditNote
        //                , IsTrading
        //                , IsTollIssue
        //                , IsVAT11GaGa
        //                , PreviewOnly, PrintCopy, AlReadyPrintNo, false, Is11, IsValueOnly
        //                , IsCommercialImporter,false,false,connVM);

        //            Print_Or_Preview();


        //            #endregion
        //        }
        //        else
        //        {
        //            #region Print

        //            foreach (int Index in IndexList)
        //            {

        //                SalesInvoiceNo = dgvSales.Rows[Index].Cells["SalesInvoiceNo"].Value.ToString();
        //                //varTrading = dgvSales.Rows[Index].Cells["Trading"].Value.ToString();
        //                //var tt = "'INV-0321/0001','INV-0321/0002'";
        //                //SalesInvoiceNo = tt;
        //                reportDocument = new ReportDocument();

        //                reportDocument = _report.Report_VAT6_3_Completed(SalesInvoiceNo, TransactionType
        //                    , IsCreditNote
        //                    , IsDebitNote
        //                    , IsRawCreditNote
        //                    , IsTrading
        //                    , IsTollIssue
        //                    , IsVAT11GaGa
        //                    , PreviewOnly, PrintCopy, AlReadyPrintNo,true, Is11, IsValueOnly
        //                    , IsCommercialImporter,false,false,connVM);
        //                Print_Or_Preview();


        //            }
        //            MessageBox.Show("You have successfully print " + IndexList.Count() + " Invoice");

        //            #endregion

        //        }


        //    }
        //    #region Catch
        //    catch (Exception ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, "Print_Or_Preview_Progress", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "Print_Or_Preview_Progress", exMessage);
        //    }
        //    #endregion Catch


        //}

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
                System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

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
                   
                    string PrintPage = "";
                    int j = 0;
                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        j = AlreadyPrint - PrintCopy + i;

                        printerSettings.PrinterName = PrinterName;
                        //if (Program.IsBureau == true)
                        //{

                        //    FormulaFieldDefinitions crFormulaF;
                        //    crFormulaF = reportDocument.DataDefinition.FormulaFields;
                        //    _vCommonFormMethod = new CommonFormMethod();
                        //    _vCommonFormMethod.FormulaField(reportDocument, crFormulaF, "CurrentPrintCopy", j.ToString());
                        //}
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
        #endregion


        private void btnPreview_Click(object sender, EventArgs e)
        {

            try
            {
                progressBar1.Visible = true;
                IndexList = new List<int>();

                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                    {
                        IndexList.Add(row.Index);
                    }
                }

                if (IndexList.Count < 1)
                {
                    MessageBox.Show(this, "Please Select Invoice First!");
                    progressBar1.Visible = false;
                    return;

                }
                IsCreditNote = false;
                IsDebitNote = false;
                PreviewOnly = true;

                bgwPrint.RunWorkerAsync();
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
                MessageBox.Show(ex.Message, "bgwPrint", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPrint", exMessage);
            }
            #endregion Catch


        }

        private void btnSelectedPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbPrinterName.Text))
                {
                    MessageBox.Show(this, "Please Select Printer First!");
                    cmbPrinterName.Focus();
                    return;
                }
                progressBar1.Visible = true;
                IndexList = new List<int>();

                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                    {
                        IndexList.Add(row.Index);
                    }
                }

                if (IndexList.Count < 1)
                {
                    MessageBox.Show(this, "Please Select Invoice First!");
                    progressBar1.Visible = false;
                    return;

                }
                PreviewOnly = false;

                bgwPrint.RunWorkerAsync();

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

        private void cmbPrinterName_Leave(object sender, EventArgs e)
        {
            PrinterName = cmbPrinterName.Text;

        }

        private void dgvSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           

        }

        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            int rowIndex = dgvSales.CurrentCell.RowIndex;
            if (e.ColumnIndex == 0)
            {
                string ChalanNo = dgvSales.CurrentRow.Cells["SalesInvoiceNo"].Value.ToString();
                //DataGridViewSelectedRowCollection selectedRows = dgvSales.SelectedRows;
                //if (selectedRows != null && selectedRows.Count > 0)
                //{
                //    selectedRow = selectedRows[0];
                //}
                //this.Hide();

                FormSaleNestle frm = new FormSaleNestle(ChalanNo);
                frm.rbtnOther.Checked = true;

                frm.ShowDialog();

                //this.Close();

               

            }
            if (e.ColumnIndex == 1)
            {
                bool Check =Convert.ToBoolean( dgvSales.CurrentRow.Cells["Select"].Value);

                if(Check)
                {
                    dgvSales["Select", rowIndex].Value = false;


                }
                else
                {
                    dgvSales["Select", rowIndex].Value = true;

                }
            }
        }

        private void dgvSales_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.code.Width;
                var h = Properties.Resources.code.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.code, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void bgwPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
               


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
                MessageBox.Show(ex.Message, "bgwPrint_DoWork", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPrint_DoWork", exMessage);
            }
            #endregion Catch


        }

        private void bgwPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                SaleReport _report = new SaleReport();
                string MultipleSalesInvoiceNo = "";
                string MultipleSalesInvoiceRows = "";
                if (PreviewOnly == true)
                {
                    #region Preview
                    MultipleSalesInvoiceNo = "";
                    foreach (int Index in IndexList)
                    {
                      string  InvoiceRow = dgvSales.Rows[Index].Cells["SKUCount"].Value.ToString();
                        SalesInvoiceNo = dgvSales.Rows[Index].Cells["SalesInvoiceNo"].Value.ToString();
                        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + SalesInvoiceNo;
                        MultipleSalesInvoiceRows = MultipleSalesInvoiceRows + "~" + InvoiceRow;

                    }
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", "','");
                    MultipleSalesInvoiceNo = "'" + MultipleSalesInvoiceNo + "'";
                    MultipleSalesInvoiceRows =  MultipleSalesInvoiceRows.Substring(1,MultipleSalesInvoiceRows.Length-1 );

                    reportDocument = new ReportDocument();

                    reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
           , IsCreditNote
                        , IsDebitNote
                        , IsRawCreditNote
                        , IsTrading
                        , IsTollIssue
                        , IsVAT11GaGa
                        , PreviewOnly, PrintCopy, AlReadyPrintNo, false, Is11, IsValueOnly
                        , IsCommercialImporter, false, false, connVM, MultipleSalesInvoiceRows);


                    Print_Or_Preview();


                    #endregion
                }
                else
                {

                    #region Preview
                    MultipleSalesInvoiceNo = "";
                    foreach (int Index in IndexList)
                    {
                        string InvoiceRow = dgvSales.Rows[Index].Cells["SKUCount"].Value.ToString();
                        SalesInvoiceNo = dgvSales.Rows[Index].Cells["SalesInvoiceNo"].Value.ToString();
                        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + SalesInvoiceNo;
                        MultipleSalesInvoiceRows = MultipleSalesInvoiceRows + "~" + InvoiceRow;

                    }
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
                    MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", "','");
                    MultipleSalesInvoiceNo = "'" + MultipleSalesInvoiceNo + "'";
                    MultipleSalesInvoiceRows = MultipleSalesInvoiceRows.Substring(1, MultipleSalesInvoiceRows.Length - 1);

                    reportDocument = new ReportDocument();

                    reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
           , IsCreditNote
                        , IsDebitNote
                        , IsRawCreditNote
                        , IsTrading
                        , IsTollIssue
                        , IsVAT11GaGa
                        , PreviewOnly, PrintCopy, AlReadyPrintNo, true, Is11, IsValueOnly
                        , IsCommercialImporter, false, false, connVM, MultipleSalesInvoiceRows);


                    Print_Or_Preview();


                    #endregion

                    #region Print old
                  
                    //MultipleSalesInvoiceNo = "";
                    //  MultipleSalesInvoiceRows = "";
                    
                    //    MultipleSalesInvoiceNo = "";
                    //    int go = 0;
                    //    int invoiceCount = IndexList.Count();
                    //    for (var index = 0; index < IndexList.Count; index++)
                    //    {
                    //        int Index = IndexList[index];

                           

                    //        go++;
                    //        string InvoiceRow = dgvSales.Rows[Index].Cells["SKUCount"].Value.ToString();
                    //        SalesInvoiceNo = dgvSales.Rows[Index].Cells["SalesInvoiceNo"].Value.ToString();
                    //        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo + "~" + "'" + SalesInvoiceNo + "'";
                    //        MultipleSalesInvoiceRows = MultipleSalesInvoiceRows + "~" + InvoiceRow;

                           
                    //        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Trim('~');
                    //        MultipleSalesInvoiceNo = MultipleSalesInvoiceNo.Replace("~", ",");




                    //        if (go == 2 || invoiceCount < 2)
                    //        {

                    //            //if (!PreviewOnly)
                    //            //{
                    //            //    PrintServer ps = new PrintServer();
                    //            //    PrintQueue pq = ps.GetPrintQueue(PrinterName);
                    //            //    int count = pq.NumberOfJobs;

                    //            //    if (count >= 20)
                    //            //    {
                    //            //        index -= 2;
                    //            //        go = 0;

                    //            //        continue;
                    //            //    }
                    //            //}
                    //            MultipleSalesInvoiceRows =
                    //                MultipleSalesInvoiceRows.Substring(1, MultipleSalesInvoiceRows.Length - 1);

                    //            reportDocument = new ReportDocument();

                    //            reportDocument = _report.Report_VAT6_3_Completed(MultipleSalesInvoiceNo, TransactionType
                    //                , IsCreditNote
                    //                , IsDebitNote
                    //                , IsRawCreditNote
                    //                , IsTrading
                    //                , IsTollIssue
                    //                , IsVAT11GaGa
                    //                , PreviewOnly, PrintCopy, AlReadyPrintNo, true, Is11, IsValueOnly
                    //                , IsCommercialImporter, false, false, connVM, MultipleSalesInvoiceRows);

                               

                    //            Print_Or_Preview();

                    //            go = 0;
                    //            invoiceCount = invoiceCount - 2;
                    //            MultipleSalesInvoiceNo = "";
                    //            MultipleSalesInvoiceRows = "";
                    //        }
                    //    }
                    
                    #endregion



                }
                //MessageBox.Show("You have successfully print " + PrintCopy + " Copy(s)");
                if(!PreviewOnly)
                {
                    MessageBox.Show("You have successfully print " + IndexList.Count() + " Invoice");

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
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", exMessage);
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
