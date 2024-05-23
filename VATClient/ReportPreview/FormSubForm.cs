using CrystalDecisions.CrystalReports.Engine;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymphonySofttech.Reports;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormSubForm : Form
    {
        public FormSubForm()
        {
            InitializeComponent();
             connVM = Program.OrdinaryLoad();
        }
        //private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        string post1, post2, vPostStatus = string.Empty;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        _9_1_VATReturnDAL _ReportDSDAL = new _9_1_VATReturnDAL();
        public int BranchId = 0;
        public string subFormName = "";
        VATReturnSubFormVM varVATReturnSubFormVM = new VATReturnSubFormVM();
        private ReportDocument reportDocument = new ReportDocument();



        #region Comments May-02-2020



        #endregion


        private void FormSubForm_Load(object sender, EventArgs e)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(cmbNoteNo.Text.Trim()))
                {
                    Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

                    dgvSubForm.DataSource = dt;
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
        private void PostStatus()
        {
            post1 = "y";
            post2 = "n";
            if (cmbPost.Text.Trim().ToLower() == "y")
            {
                post1 = "y";
                post2 = "y";
            }
            else if (cmbPost.Text.Trim().ToLower() == "n")
            {
                post1 = "n";
                post2 = "n";
            }
            this.progressBar1.Visible = true;
            this.Enabled = false;

        }

        private void Search(int NoteNo)
        {
            try
            {
                PostStatus();

                dt = new DataTable();

                varVATReturnSubFormVM = new VATReturnSubFormVM();
                varVATReturnSubFormVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                varVATReturnSubFormVM.ExportInBDT = "";
                varVATReturnSubFormVM.NoteNo = NoteNo;
                varVATReturnSubFormVM.BranchId = BranchId;
                varVATReturnSubFormVM.post1 = post1;
                varVATReturnSubFormVM.post2 = post2;
                varVATReturnSubFormVM.IsSummary = chkSummary.Checked;



                ////varVATReturnSubFormVM.IsVersion2 = ckbVAT9_1.Checked ? true : false;


                dt = _ReportDSDAL.VAT9_1_SubForm(varVATReturnSubFormVM,connVM);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int RowCount = dt.Rows.Count;
                    lblRowCount.Text = "Row Count: " + RowCount;

                    OrdinaryVATDesktop.DtSlColumnAdd(dt);
                }




                //////dgvSubForm.DataSource = dt;
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
                if (string.IsNullOrWhiteSpace(cmbNoteNo.Text.Trim()))
                {
                    MessageBox.Show("Select Note No. First!", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

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

                Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

                dgvSubForm.DataSource = dt;
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

        private void btnPrint_Click(object sender, EventArgs e)
        {

            try
            {

                PostStatus();
                #region Comments May-02-2020

                ////Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                ////if (!string.IsNullOrWhiteSpace(cmbNoteNo.Text.Trim()))
                ////{
                ////    Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

                ////}

                ////ds = new DataSet();
                ////ds.Tables.Add(dt);

                #endregion


                varVATReturnSubFormVM = new VATReturnSubFormVM();
                varVATReturnSubFormVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                varVATReturnSubFormVM.ExportInBDT = "";
                varVATReturnSubFormVM.NoteNo = Convert.ToInt32(cmbNoteNo.Text.Trim());
                varVATReturnSubFormVM.BranchId = BranchId;
                varVATReturnSubFormVM.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim(); ;
                varVATReturnSubFormVM.DecimalPlace = cmbDecimalPlace.Text.Trim() == "" ? "2" : cmbDecimalPlace.Text.Trim(); ;
                ////varVATReturnSubFormVM.IsVersion2 = ckbVAT9_1.Checked ? true : false;
                varVATReturnSubFormVM.post1 = post1;
                varVATReturnSubFormVM.post2 = post2;
                varVATReturnSubFormVM.IsSummary = chkSummary.Checked;

                reportDocument = new ReportDocument();
                reportDocument = new NBRReports().VAT9_1_SubForm(varVATReturnSubFormVM,connVM);

                PrintReport();

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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            #endregion
        }

        public void PrintReport()
        {

            try
            {
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                ////if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();


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
                FileLogger.Log(this.Name, "PrintReport", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }


        }


        private void Download_Click(object sender, EventArgs e)
        {
            try
            {
                PostStatus();
                ExcelDownload();
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
                FileLogger.Log(this.Name, "Download_Click", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }
        }

        private void ExcelDownload()
        {
            try
            {


                VATReturnSubFormVM vm = new VATReturnSubFormVM();

                vm.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                vm.ExportInBDT = "";
                vm.NoteNo = Convert.ToInt32(cmbNoteNo.Text.Trim());
                vm.BranchId = BranchId;
                //////vm.IsVersion2 = ckbVAT9_1.Checked ? true : false;
                vm.post1 = post1;
                vm.post2 = post2;
                vm.IsSummary = chkSummary.Checked;
                vm = new NBRReports().VAT9_1_SubForm_Download(vm);


                if (vm.varFileStream == null)
                {
                    MessageBox.Show("There Is No Data");
                    return;
                }




                vm.varExcelPackage.Save();
                vm.varFileStream.Close();

                //MessageBox.Show("Successfully Exported data in Excel files of root directory");
                ProcessStartInfo psi = new ProcessStartInfo(vm.varFileDirectory);
                psi.UseShellExecute = true;
                Process.Start(psi);

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
                FileLogger.Log(this.Name, "Download_Click", exMessage);
            }
            #endregion
        }

        private void ExcelDownloadBackup()
        {
            try
            {


                dt = new DataTable();


                if (!string.IsNullOrWhiteSpace(cmbNoteNo.Text.Trim()))
                {
                    Search(Convert.ToInt32(cmbNoteNo.Text.Trim()));

                }


                ////if (dt.Rows.Count == 0)
                ////{
                ////    MessageBox.Show("There Is No Data");
                ////    return;
                ////}


                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("",connVM);
                dtComapnyProfile = tempDS.Tables[0].Copy();
                var ComapnyName = dtComapnyProfile.Rows[0]["CompanyName"].ToString();
                var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
                var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();

                string sheetName = "";
                string NoteNo = "";
                string Type = "";

                if (dt != null && dt.Rows.Count > 0)
                {
                    sheetName = dt.Rows[0]["SubFormName"].ToString();
                    NoteNo = dt.Rows[0]["NoteNo"].ToString();
                    Type = dt.Rows[0]["Remarks"].ToString();
                }


                string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, sheetName, " Note No: " + NoteNo + "        Type: " + Type };

                #region SubForm
                switch (subFormName)
                {
                    case "SubFormAPart3":
                    case "SubFormAPart4":
                        DataView view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormA", false, "ProductDescription", "ProductCode", "ProductName", "TotalPrice", "SDAmount", "VATAmount", "DetailRemarks");

                        break;
                    case "SubFormBPart3":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormB", false, "ProductCategory", "ProductDescription", "ProductCode", "ProductName", "TotalPrice", "SDAmount", "VATAmount", "DetailRemarks");
                        break;
                    case "SubFormCPart3":
                    case "SubFormCPart4":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormC", false, "ProductDescription", "ProductCode", "ProductName", "UOM", "Quantity", "TotalPrice", "SDAmount", "VATAmount", "DetailRemarks");
                        break;

                    case "SubFormDPart5":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormD", false, "VendorBIN", "VendorName", "VendorAddress", "TotalPrice", "VDSAmount", "InvoiceNo", "InvoiceDate", "AccountCode", "DetailRemarks");
                        break;
                    case "SubFormEPart6":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormE", false, "CustomerBIN", "CustomerName", "CustomerAddress", "TotalPrice", "VDSAmount", "InvoiceNo", "InvoiceDate", "DetailRemarks");
                        break;
                    case "SubFormFPart6":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormF", false, "BENumber", "Date", "CustomHouse", "ATAmount", "DetailRemarks");
                        break;
                    case "SubFormGPart8":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormG", false, "ChallanNumber", "BankName", "BankBranch", "Date", "AccountCode", "Amount", "DetailRemarks");
                        break;
                    default:
                        break;
                }
                #endregion



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

                fileDirectory += "\\" + sheetName + "-" + dtpDate.Value.ToString("MMMM-yyyy") + ".xlsx";
                FileStream objFileStrm = File.Create(fileDirectory);

                int TableHeadRow = 0;
                TableHeadRow = ReportHeaders.Length + 2;

                int RowCount = 0;
                RowCount = dt.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dt.Columns.Count;

                int GrandTotalRow = 0;
                GrandTotalRow = TableHeadRow + RowCount + 1;
                if (string.IsNullOrEmpty(sheetName))
                {
                    sheetName = "DemoSubFormSheet";
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
                FileLogger.Log(this.Name, "Download_Click", exMessage);
            }
            #endregion
        }


    }
}
