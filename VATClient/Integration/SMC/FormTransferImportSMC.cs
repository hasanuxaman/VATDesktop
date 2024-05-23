using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATServer.Library.Integration;

namespace VATClient
{
    public partial class FormTransferImportSMC : Form
    {
        #region Variables
        string[] sqlResults = new string[6];
        string loadedTable;
        static string selectedType;
        DataTable dtTableResult;
        DataSet ds;
        public bool isFileSelected = false;
        public string preSelectTable;
        public string transactionType;
        private bool Integration = false;
        public string _saleRow = "";

        private string SalesFromDate = "";
        private string SalesToDate = "";
        private string DateRange = "";

        IntegrationParam paramVM = new IntegrationParam();
        ResultVM rVM = new ResultVM();
        SMCIntegrationDAL _IntegrationDAL = new SMCIntegrationDAL();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();


        #endregion

        public FormTransferImportSMC()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static string SelectOne(string transactionType)
        {
            FormTransferImportSMC form = new FormTransferImportSMC();

            form.preSelectTable = "TransferIssues";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        #region Form Load
        
        private void FormMasterImport_Load(object sender, EventArgs e)
        {

        }

        #endregion


        #region Data Load
        
        private void btnLoad_Click(object sender, EventArgs e)
        {
                try
                {

                    NullCheck();

                    paramVM = new IntegrationParam();

                    paramVM.RefNo = txtId.Text;
                    paramVM.FromDate = SalesFromDate;
                    paramVM.ToDate = SalesToDate;

                    dtTableResult = _IntegrationDAL.GetTransfer_DBData_SMC_Consumer(paramVM,connVM);

                    dgvLoadedTable.DataSource = dtTableResult;

                    lblRecord.Text = "Records Count: " + dtTableResult.Rows.Count;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
        }


        #endregion

        #region Save Data
        
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                btnImport.Enabled = false;
                this.progressBar1.Visible = true;

                #region Save Start

                string SaveStartTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                FileLogger.Log(this.Name, "btnSaveTemp_Click", "Date Range: " + DateRange + Environment.NewLine + "Save Start Time: " + SaveStartTime);

                #endregion

                #region Param Set
                NullCheck();

                paramVM = new IntegrationParam();

                paramVM.RefNo = txtId.Text.Trim();
                paramVM.FromDate = SalesFromDate;
                paramVM.ToDate = SalesToDate;

                #endregion

                bgwTransferIssue.RunWorkerAsync();

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void bgwTransferIssue_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                //////if (InvokeRequired)
                //////    Invoke((MethodInvoker)delegate { PercentBar(5); });

                //////BulkCallBack();

                paramVM.callBack = BulkCallBack;
                paramVM.SetSteps = SetSteps;


                rVM = _IntegrationDAL.SaveTransfer_Pre(paramVM,connVM);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            finally { }
        }

        private void bgwTransferIssue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Save End

                string SaveEndTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Save End Time: " + SaveEndTime);

                #endregion

                if (rVM.Status == "Fail")
                {
                    throw new ArgumentException(rVM.Message);
                }

                MessageBox.Show(rVM.Message, "Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);


                btnImport.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnImport.Enabled = true;
                this.progressBar1.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
            }

        }

        #endregion

        #region MISC Functions

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

            SalesFromDate = dtpSaleFromDate.Checked == false ? "" : dtpSaleFromDate.Value.ToString("yyyy-MMM-dd");
            SalesToDate = dtpSaleToDate.Checked == false ? "" : dtpSaleToDate.Value.ToString("yyyy-MMM-dd");
            DateRange = SalesFromDate + " - " + SalesToDate;

        }

        private void SetSteps(int steps = 4)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(steps); });
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
        }


        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string Id = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();
            string refNo = dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value != null ? dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value.ToString() : "";


            _saleRow = string.IsNullOrEmpty(refNo) || refNo == "-" ? Id : refNo;

            this.Hide();
        }

        #endregion


    }
}
