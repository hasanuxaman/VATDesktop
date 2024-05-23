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
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormSaleSummary : Form
    {
        #region Variable

        string FromDate, ToDate;
        public string reportType = string.Empty;
        public string ReportName = string.Empty;
        private DataSet ds;
        DataTable dt = new DataTable();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        string FileName = "";
        string ReportHeaderName = "";

        #endregion

        public FormSaleSummary()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        private void FormSaleSummary_Load(object sender, EventArgs e)
        {

        }

        private void NullCheck()
        {
            try
            {
                if (dtpFromDate.Checked == false)
                {
                    FromDate = "";
                    FromDate = dtpFromDate.MinDate.ToString("yyyy-MM-dd 00:00:00");
                }
                else
                {
                    FromDate = dtpFromDate.Value.ToString("yyyy-MM-dd 00:00:00");
                }
                if (dtpToDate.Checked == false)
                {
                    ToDate = "";
                    ToDate = dtpFromDate.MaxDate.ToString("yyyy-MM-dd 23:59:59");

                }
                else
                {
                    ToDate = dtpToDate.Value.ToString("yyyy-MM-dd 23:59:59");
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            #endregion Catch

        }

        private void Download_Click(object sender, EventArgs e)
        {
            try
            {

                string ReportType = "";

                NullCheck();
                DataSet ds = new DataSet();

                ReportName = cmbReportName.Text;

                SaleMISViewModel Paramvm = new SaleMISViewModel();

                Paramvm.DateFrom = FromDate;
                Paramvm.DateTo = ToDate;

                ReportDSDAL ReportDSDal = new ReportDSDAL();

                if (ReportName == "DayWise")
                {
                    FileName = "DayWiseSalesData";
                    ReportType = "Day Wise Sale Summary";
                    dt = ReportDSDal.GetDayWiseSalesData(Paramvm, null, null, connVM);
                }
                else if (ReportName == "BranchWise")
                {
                    FileName = "BranchWiseSalesData";
                    ReportType = "Branch Wise Sale Summary";

                    dt = ReportDSDal.GetBranchWiseSalesData(Paramvm, null, null, connVM);
                }
                else if (ReportName == "ItemWise")
                {
                    FileName = "ItemWiseSalesData";
                    ReportType = "Product Wise Sale Summary";

                    dt = ReportDSDal.GetProductWiseSalesData(Paramvm, null, null, connVM);
                }

                SaveExcel(dt, ReportType, FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.Text, ex.Message);
            }

        }

        private void bgwPreview_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void bgwPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void SaveExcel(DataTable dt, string ReportType = "", string FileName = "")
        {
            //DataTable dt = ds.Tables["Table"];
            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("", connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();

            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo, ReportType
                , " Period: " + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + " TO " +  Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") };

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

            fileDirectory += "\\" + FileName + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
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


    }
}
