using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using VATServer.Library.Integration;

namespace VATClient
{
    public partial class FormSaleImportGDIC : Form
    {
        #region Variables

        GDICIntegrationDAL _IntegrationDAL = new GDICIntegrationDAL();
        CommonDAL _cDal = new CommonDAL();
        ResultVM rVM = new ResultVM();
        IntegrationParam paramVM = new IntegrationParam();

        string[] sqlResults = new string[6];
        DataTable dtTableResult;
        public bool isFileSelected = false;
        private bool _isDeleteTemp = true;
        public string transactionType;
        private string SalesFromDate = "";
        private string SalesToDate = "";
        private string DateRange = "";


        public bool IsCDN = false;
        private string _saleRow;
        private string preSelectTable;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormSaleImportGDIC()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();


        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportGDIC form = new FormSaleImportGDIC();

            form.IsCDN = cdn;

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        #region Form Load

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            dgvLoadedTable.ReadOnly = true;

        }


        #endregion

        #region Data Load

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {

                NullCheck();

                #region Load Start
                //string LoadStartTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Load Start Time: " + LoadStartTime);

                #endregion

                DataLoad();

                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

                #region Load End

                //string LoadEndTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                //FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Load End Time: " + LoadEndTime);

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void DataLoad()
        {
            BranchProfileDAL dal = new BranchProfileDAL();
            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

            #region Data Load

            IntegrationParam paramVM = new IntegrationParam();

            paramVM.RefNo = txtId.Text.Trim();
            paramVM.FromDate = SalesFromDate;
            paramVM.ToDate = SalesToDate;
            paramVM.TransactionType = transactionType;
            paramVM.dtConnectionInfo = dt;

            dtTableResult = _IntegrationDAL.GetSource_SaleData_Master(paramVM,connVM);

            dgvLoadedTable.DataSource = dtTableResult;

            #endregion

        }

        #endregion

        #region Save Data


        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                btnSaveTemp.Enabled = false;
                this.progressBar1.Visible = true;

                #region Save Start

                string SaveStartTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                FileLogger.Log(this.Name, "btnSaveTemp_Click", "Date Range: " + DateRange + Environment.NewLine + "Save Start Time: " + SaveStartTime);

                #endregion

                BranchProfileDAL dal = new BranchProfileDAL();
                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

                #region Param Set
                NullCheck();

                paramVM = new IntegrationParam();

                paramVM.RefNo = txtId.Text.Trim();
                paramVM.FromDate = SalesFromDate;
                paramVM.ToDate = SalesToDate;
                paramVM.TransactionType = transactionType;
                paramVM.dtConnectionInfo = dt;
                paramVM.Processed = "N";

                #endregion

                bgwBigData.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ////if (InvokeRequired)
                ////    Invoke((MethodInvoker)delegate { PercentBar(5); });

                ////BulkCallBack();

                paramVM.callBack = BulkCallBack;
                paramVM.SetSteps = SetSteps;

                dtTableResult = new DataTable();
                dtTableResult = _IntegrationDAL.GetSaleData_TypeValidation(paramVM,connVM);

                if (dtTableResult.Rows.Count == 0)
                {
                    rVM = _IntegrationDAL.SaveSale_Pre(paramVM,connVM);
                }
                else
                {
                    e.Result = dtTableResult;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    DataTable errorTable = (DataTable)e.Result;

                    errorTable.Columns.Remove("SL");
                    errorTable.Columns.Remove("InvoiceNo");
                    errorTable.Columns.Remove("IsProcessed");
                    errorTable.Columns.Remove("CompanyCode");
                    FormErrorMessageIntegration.ShowDetails(errorTable);
                }
                else
                {
                    DataLoad();

                    if (rVM.Status == "Fail")
                    {
                        throw new ArgumentException(rVM.Message);
                    }

                    MessageBox.Show(rVM.Message, "Sales", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                

                btnSaveTemp.Enabled = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnSaveTemp.Enabled = true;
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

        private void UpdateProgressBar()
        {
            progressBar1.Value += 1;
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

        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
                _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();

                this.Hide();

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        #endregion



    }
}
