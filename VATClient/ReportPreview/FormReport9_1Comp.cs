using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using SymphonySofttech.Utilities;
////
using VATClient.ModelDTO;
using VATClient.ReportPages;
using VATServer.Library;
using VATViewModel.DTOs;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using VATServer.Ordinary;
using SymphonySofttech.Reports;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using VATDesktop.Repo;
using VATServer.Interface;
using MessageBox = System.Windows.Forms.MessageBox;

namespace VATClient
{
    public partial class FormReport9_1Comp : Form
    {
        
        #region Variable 

        string fDate, sDate, branchId = "0";

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #endregion


        public FormReport9_1Comp()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
   
        private void ReportSCBL_Load(object sender, EventArgs e)
        {
            cmbReportType.Text = "Sale";
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition,
                "varchar", true, true, connVM);

        }
        private void NullCheck()
        {
            try
            {
                if (dtpFromDate.Checked == false)
                {
                    fDate = "";
                    fDate = dtpFromDate.MinDate.ToString("yyyy");
                }
                else
                {
                    fDate = dtpFromDate.Value.ToString("yyyy");// dtpFromDate.Value.ToString("yyyy-MMM-dd");
                }

                if (dtpToDate.Checked == false)
                {
                    sDate = "";
                    sDate = dtpFromDate.MaxDate.ToString("yyyy");

                }
                else
                {
                    sDate = dtpToDate.Value.ToString("yyyy");//  dtpToDate.Value.ToString("yyyy-MMM-dd");
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



        private void SaveExcel(DataTable dt, string ReportType = "", string BranchName = "")
        {
            //DataTable dt = ds.Tables["Table"];
           
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("",connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();



            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo, ReportType, " Period: " + fDate + " TO " + sDate };

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

        private void Download_Click(object sender, EventArgs e)
        {
            DownloadExcel();
        }

        private void DownloadExcel(bool allBranch = false)
        {
            try
            {
                NullCheck();
                branchId = cmbBranch.SelectedValue.ToString();
                ParameterVM parameterVm = new ParameterVM
                {
                    BranchId = Convert.ToInt32(cmbBranch.SelectedValue),
                    ReportType = cmbReportType.Text,
                    FromDate = fDate,
                    ToDate = sDate,
                    AllBranch = allBranch
                };


                bgwPreview.RunWorkerAsync(parameterVm);
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
                FileLogger.Log(this.Name, "Download_Click", exMessage);
            }
        }


        private void bgwPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            ParameterVM parameterVm = (ParameterVM)e.Argument;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            DataTable dtresult = new DataTable();

            dtresult = string.Equals(parameterVm.ReportType, "sale", StringComparison.CurrentCultureIgnoreCase)
                ? reportDsdal.GetSale9_1_Comparison(parameterVm)
                : reportDsdal.GetPurchase9_1_Comparison(parameterVm);

            e.Result = dtresult;

        }

        private void bgwPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                    return;
                }

                DataTable dtResult = (DataTable)e.Result;

                SaveExcel(dtResult, "9.1"+cmbReportType.Text+" Comparison");

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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void btnDownloadDtls_Click(object sender, EventArgs e)
        {
            DownloadExcel(true);
        }

        private async void btnSummary_Click(object sender, EventArgs e)
        {
            await DownloadSummary();
        }

        private async Task DownloadSummary(bool allBranch = false)
        {
            try
            {
                NullCheck();
                branchId = cmbBranch.SelectedValue.ToString();
                ParameterVM parameterVm = new ParameterVM
                {
                    BranchId = Convert.ToInt32(cmbBranch.SelectedValue),
                    ReportType = cmbReportType.Text,
                    FromDate = fDate,
                    ToDate = sDate,
                    AllBranch = allBranch
                };
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dtResult = new DataTable();
                if (string.Equals(parameterVm.ReportType, "sale", StringComparison.CurrentCultureIgnoreCase))
                {
                    await Task.Run(() => { dtResult = reportDsdal.GetSale9_1_Total(parameterVm); });
                }
                else
                {
                    await Task.Run(() => { dtResult = reportDsdal.GetPurchase9_1_Total(parameterVm); });

                }


                SaveExcel(dtResult, "9.1" + cmbReportType.Text + " Comparison");
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
        }

        private void btnSumBranch_Click(object sender, EventArgs e)
        {
            DownloadSummary(true);
        }
    }
}
