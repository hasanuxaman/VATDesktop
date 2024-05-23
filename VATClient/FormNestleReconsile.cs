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
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormNestleReconsile : Form
    {
        DataTable dt = new DataTable();

        DataSet ds = new DataSet();
        DataTable dtNestle = new DataTable();
        DataTable dtMiddlewareSale = new DataTable();
        DataTable dtMiddlewareCredit = new DataTable();
        DataTable dtMiddlewareDebit = new DataTable();
        DataTable dtShamphanSale = new DataTable();
        DataTable dtShamphanCredit = new DataTable();
        DataTable dtShamphanDebit = new DataTable();
        bool IsExcel = false;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string SalesFromDate, SalesToDate;


        public FormNestleReconsile()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            IsExcel = chkIsExcel.Checked;
            try
            {
                if (!dtpFromDate.Checked && !dtpToDate.Checked)
                {
                    MessageBox.Show(@"Please Select Date Range");
                    return;
                }
                if (IsExcel)
                {
                    BrowsFile();
                    string fileName = Program.ImportFileName;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        MessageBox.Show(this, "Please select the right file for import");
                        return;
                    }
                }
               
                progressBar1.Visible = true;

                //lblCurrentProcess.Visible = true;

                bgwProcess.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        
        private void bgwProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            NESTLEIntegrationDAL _IntegrationDAL = new NESTLEIntegrationDAL();

            DataTable dtTemp = new DataTable();
            BranchProfileDAL dal = new BranchProfileDAL();
            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);


            IntegrationParam paramVM = new IntegrationParam();

            paramVM.FromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"); ;
            paramVM.ToDate = dtpToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            paramVM.dtConnectionInfo = dt;



            if (IsExcel)
            {
                ds = LoadFromExcel();
                dtNestle = ds.Tables[0];
                dtNestle = OrdinaryVATDesktop.DtEmptyRowDelete(dtNestle, "Pack Size");
                dtNestle = OrdinaryVATDesktop.DtColumnNameChange(dtNestle, "SKUCode", "ProductCode");
                dtNestle = OrdinaryVATDesktop.DtColumnNameChange(dtNestle, "Sales TP", "DSValue");
                System.Data.DataColumn newColumn = new System.Data.DataColumn("DSQty", typeof(System.String));
                dtNestle.Columns.Add(newColumn);

                foreach (DataRow dr in dtNestle.Rows)
                {
                    int Pack_Size = Convert.ToInt32(dr["Pack Size"].ToString().Trim());
                    int Ctn = Convert.ToInt32(dr["Ctn"].ToString().Trim());
                    int Unit = Convert.ToInt32(dr["Unit"].ToString().Trim());
                    dr["DSQty"] = Pack_Size * Ctn + Unit;
                }
                dtTemp = dtNestle;
                DataView dv = new DataView(dtTemp);

                dtNestle = dv.ToTable(true, "ProductCode", "DSQty", "DSValue");
            }
            else
            {
                dtNestle = _IntegrationDAL.GetSource_DayEndClosingData(paramVM, connVM);
            }
           

            #region Data Load

            dtMiddlewareSale = _IntegrationDAL.GetSource_SaleData(paramVM, connVM);
            dtMiddlewareCredit = _IntegrationDAL.GetSource_CraditData(paramVM, connVM);
            dtMiddlewareDebit = _IntegrationDAL.GetSource_DebitData(paramVM, connVM);
            dtShamphanSale = _IntegrationDAL.GetSource_SalesDetailData(paramVM, connVM);
            dtShamphanCredit = _IntegrationDAL.GetSource_CreditDetailData(paramVM, connVM);
            dtShamphanDebit = _IntegrationDAL.GetSource_DebitDetailData(paramVM, connVM);

            e.Result = _IntegrationDAL.SaveNestleTempTable(dtNestle, dtMiddlewareSale, dtMiddlewareCredit, dtMiddlewareDebit, dtShamphanSale, dtShamphanCredit, dtShamphanDebit, null, null, connVM);



            #endregion
        }

        private void bgwProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data Successfully Processed !" : e.Error.Message);
            }
            finally
            {
                progressBar1.Visible = false;
                //lblCurrentProcess.Visible = false;
            }
        }


        private void SetLabel(string text)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() =>
                    {
                        lblCurrentProcess.Text = text;
                        progressBar1.Value += 1;
                    }
                ));

        }

        private void FormRegularProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwProcess.IsBusy)
            {
                bgwProcess.CancelAsync();
            }

        }

        private void FormRegularProcess_Load(object sender, EventArgs e)
        {
            
            dtpFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 00:00:00"));
            dtpToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 23:59:59"));
            dtpFromDate.Checked = false;
            dtpToDate.Checked = false;
        }
        private DataSet LoadFromExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                ds = new ImportDAL().GetDataSetFromExcel(paramVm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return ds;
        }

        private void BrowsFile()
        {
            #region try
            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
              
                    fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
               
               
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }
            }
            #endregion

            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            SalesFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd ");// dtpSalesFromDate.Value.ToString("yyyy-MMM-dd");
            SalesToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");//  dtpSalesToDate.Value.ToString("yyyy-MMM-dd");
            bgwDownload.RunWorkerAsync();
        }

        private void bgwDownload_DoWork(object sender, DoWorkEventArgs e)
        {

             try
            {
               
                dt= new DataTable();
               NESTLEIntegrationDAL _IntegrationDAL = new NESTLEIntegrationDAL();
               dt = _IntegrationDAL.GetTempData(connVM);
               DataTable dt1 = _IntegrationDAL.GetTempDataDetails(connVM);
                ds = new DataSet();

               ds.Tables.Add(dt);
               ds.Tables.Add(dt1);
            }
             catch (Exception ex)
             {
                 MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
             }

        }

        private void bgwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                OrdinaryVATDesktop.SaveExcelMultiple(ds, null, new[] {"SKU Wise" ,"Invoice Wize"}, new[] { SalesFromDate, SalesToDate });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                progressBar1.Visible = false;
                //lblCurrentProcess.Visible = false;
            }
        }
        private void SaveExcel(DataTable dt, string SalesFromDate = "", string SalesToDate = "")
        {

            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }



            string[] ReportHeaders = new string[] { " FromDate: " + SalesFromDate, " ToDate: " + SalesToDate };

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

            fileDirectory += "\\" + "DayEndTotalSale" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            //if (string.IsNullOrEmpty(sheetName))
            //{
            //    sheetName = ReportType;
            //}

            string sheetName = "DayEndTotalSale" + SalesFromDate;


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

        private void bgwExport_DoWork(object sender, DoWorkEventArgs e)
        {
           

            try
            {
                dt = new DataTable();
                NESTLEIntegrationDAL _IntegrationDAL = new NESTLEIntegrationDAL();
                dt = _IntegrationDAL.GetExcelData(connVM);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwExport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                OrdinaryVATDesktop.SaveExcel(dt, "Sale", "SaleM");
                MessageBox.Show("Successfully Exported data in Excel files of root directory");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                progressBar1.Visible = false;
                //lblCurrentProcess.Visible = false;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;

            bgwExport.RunWorkerAsync();
        }


    }
}
