using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATDesktop.Repo;
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class Form9_1Statement : Form
    {

        private int BranchId = 0;
        string PeriodName = "";
        private DataTable ReportResult;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string fromDate = "";
        string ToDate = "";
        private DataSet dataSet;

        

        public Form9_1Statement()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        private void Form9_1Statement_Load(object sender, EventArgs e)
        {
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, true, connVM);

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            List<BranchProfileVM> VMs = new List<BranchProfileVM>();
            VMs = new BranchProfileDAL().SelectAllList(null, null, null, null, null, connVM);

            if (VMs != null && VMs.Count > 0)
            {
                if (VMs.Count == 1)
                {
                    cmbBranch.Enabled = false;
                }
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                this.progressBar1.Visible = true;
                this.Enabled = false;

                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                PeriodName = dtpDate.Value.ToString("MMyyyy");

                DateTime now = dtpDate.Value;
                DateTime first = new DateTime(now.Year, now.Month, 1);
                DateTime last = first.AddMonths(1).AddDays(-1);

                fromDate = first.ToString("yyyy-MM-dd 00:00:00.000");
                ToDate = last.ToString("yyyy-MM-dd 23:59:59.000");

                bgw9_1Statement.RunWorkerAsync();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }
            
        }

        private void bgw9_1Statement_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportResult = new DataTable();

                GDICIntegrationDAL vatReturnDal = new GDICIntegrationDAL();
                dataSet = vatReturnDal.StatementReport(PeriodName, BranchId, fromDate, ToDate, null, null, connVM);

                #endregion
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgw9_1Statement_DoWork", exMessage);
            }

            #endregion
        }

        private void bgw9_1Statement_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            DataTable settingsDt = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            try
            {
                if (dataSet == null || dataSet.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //SaveExcel(ReportResult, "SalesInformation");

                var sheetNames = new[] { "9.1 Data", "Daily", "Monthly", "Deff" };

                OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "GDICDataReconsile", sheetNames);

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgw9_1Statement_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }
        }

        private void SaveExcel(DataTable dt, string ReportType = "", string BranchName = "", string CompanyCode = null)
        {
            
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            
            ReportDSDAL ReportDSDal = new ReportDSDAL();
                
            tempDS = ReportDSDal.ComapnyProfile("");

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
            
            string sheetName = ReportType;

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

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

            MessageBox.Show("Successfully Exported data in Excel files of root directory");

            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }


    }
}
