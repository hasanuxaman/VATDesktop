using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
//using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using SymphonySofttech.Reports;
using VATClient.ReportPreview;

namespace VATClient.Integration.Decathlon
{
    public partial class FormSaleImportDecathlon : Form
    {
        #region Variables
        string[] sqlResults = new string[6];

        string IsProcess;
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult = new DataTable();
        DataSet ds;
        public string preSelectTable;
        public string transactionType;
        private long _timePassedInMs;
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_ProcessOnly = "ProcessOnly";
        private const string CONST_SINGLEIMPORT = "SingleFileImport";
        private const string CONST_SALETYPE = "Sales";

        private string EntryTime = "";
        private string _saleRow = "";
        private IntegrationParam param = null;

        public bool IsCDN = false;
        private string ToDate;
        private string FromDate;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #endregion
        public FormSaleImportDecathlon()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static string SelectOne(string transactionType)
        {
            FormSaleImportDecathlon form = new FormSaleImportDecathlon();
            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormSaleImportDecathlon_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");

            if (code.ToLower() == "decathlon")
            {

                //txtId.Visible = false;
                //label3.Visible = false;

                //dtpSaleToDate.Visible = false;
                //label4.Visible = false;
                cmbIsProcessed.Visible = false;
                label6.Visible = false;

                ////label5.Text = "Date";

            }

            dgvLoadedTable.ReadOnly = true;
            //////tableLoad();
            ////typeLoad();
            cmbIsProcessed.Text = "N";
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

            #region Old Code

            ////if (cmbImportType.Text != CONST_DATABASE)
            ////{
            ////    _isDeleteTemp = true;
            ////    LoadDataGrid();
            ////}
            ////else
            ////{
            ////    try
            ////    {
            ////        selectedType = cmbImportType.Text;
            ////        var invoiceNo = txtId.Text.Trim();

            ////        GetSearchData(invoiceNo);

            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////    }
            ////}

            #endregion

            try
            {
                selectedType = cmbImportType.Text;
                var invoiceNo = txtId.Text.Trim();
                IsProcess = cmbIsProcessed.Text.Trim();

                GetSearchData(invoiceNo);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {

                #region Excel Check and Progress bar

                //////if (loadedTable == "")
                //////{
                //////    return;
                //////}

                //////if (!_isDeleteTemp)
                //////{
                //////    MessageBox.Show(this, "Please select new excel file");
                //////    return;
                //////}

                //////_selectedDb = cmbDBList.Text;
                //////selectedType = cmbImportType.Text;

                #endregion

                this.progressBar1.Visible = true;


                dtTableResult = dtTableResult.Copy();
                NullCheck();
                bgwBigData.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void GetSearchData(string invoiceNo = "", bool withIsProcessed = false)
        {
            NullCheck();
            var dal = new BranchProfileDAL();

            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

            param = new IntegrationParam
            {
                TransactionType = transactionType,
                RefNo = invoiceNo,
                dtConnectionInfo = dt,
                FromDate = FromDate,
                ToDate = ToDate,
                WithIsProcessed = withIsProcessed,
                BranchId = (Program.BranchId).ToString()
            };

            progressBar1.Visible = true;


            bgwSaleLoad.RunWorkerAsync(param);
        }

        private void bgwSaleLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IntegrationParam param = (IntegrationParam)e.Argument;

                DecathlonIntegrationDAL decathlonIntegrationDal = new DecathlonIntegrationDAL();

                ////dtTableResult = eonIntegrationDal.GetSaleDataAPI(param);
                dtTableResult = decathlonIntegrationDal.GetSaleData(param, null, null, connVM);

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormSaleImportDecathlon", "bgwSaleLoad_DoWork", exception.ToString());

                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void bgwSaleLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                dgvLoadedTable.DataSource = dtTableResult;
                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar1.Visible = false;

            }
        }

        void NullCheck()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                if (dtpSaleFromDate.Checked == false)
                {
                    dtpSaleFromDate.Checked = true;
                    dtpSaleFromDate.Value = DateTime.Now;
                }

                if (dtpSaleToDate.Checked == false)
                {
                    dtpSaleToDate.Checked = true;
                    dtpSaleToDate.Value = DateTime.Now;
                }
            }

            FromDate = dtpSaleFromDate.Checked == false ? "1900-01-01" : dtpSaleFromDate.Value.ToString("yyyy-MM-dd");
            ToDate = dtpSaleToDate.Checked == false ? "9000-12-31" : dtpSaleToDate.Value.ToString("yyyy-MM-dd");
            //DateRange = FromDate + " - " + ToDate;

            EntryTime = dptTime.Checked ? dptTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        }

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDAL salesDal = new SaleDAL();

                DataTable salesData = dtTableResult.Copy();
                ////salesData.Columns.Remove("InvoiceNo");

                TableValidation(salesData);

                if (InvokeRequired)
                    Invoke((MethodInvoker)delegate { PercentBar(5); });

                BulkCallBack();
                sqlResults = salesDal.SaveAndProcess(salesData, BulkCallBack, Program.BranchId, "", connVM, param, null, null, Program.CurrentUserID);

            }
            catch (Exception ex)
            {

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    ////GetSearchData("", true);
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, sqlResults[1]);

                }

                loadedTable = CONST_SALETYPE;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void TableValidation(DataTable salesData)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = Program.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = transactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            //salesData.Columns.Add(TransactionType);
        }

        private void BulkCallBack()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { progressBar1.Value += 1; }));

        }

        private void PercentBar(int maximum)
        {
            progressBar1.Style = ProgressBarStyle.Blocks;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = maximum;

            progressBar1.Value = 0;
            //var percent = (int) ((progressBar1.Value - progressBar1.Minimum) /
            //                     (double) (progressBar1.Maximum - progressBar1.Minimum) * 100);
            //using (var gr = progressBar1.CreateGraphics())
            //{
            //    gr.DrawString(percent + "%",
            //        SystemFonts.DefaultFont,
            //        Brushes.Black,
            //        new PointF(progressBar1.Width / 2 - (gr.MeasureString(percent + "%",
            //                                                 SystemFonts.DefaultFont).Width / 2.0F),
            //            y: progressBar1.Height / 2 - (gr.MeasureString(percent + "%",
            //                                           SystemFonts.DefaultFont).Height / 2.0F)));
            //}
        }
    }
}
