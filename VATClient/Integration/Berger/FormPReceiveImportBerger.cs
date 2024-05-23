using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient.Integration.Berger
{
    public partial class FormPReceiveImportBerger : Form
    {
        #region Variables
        string[] sqlResults = new string[6];        
        private int _saleRowCount = 0;
        DataTable dtTableResult = new DataTable();
        DataSet ds;        
        public string preSelectTable;
        public string transactionType;       
        private string EntryTime = "";
        private string _saleRow = "";
        private IntegrationParam param = null;       
        private string ToDate;
        private string FromDate;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        ResultVM rVM = null;

        #endregion

        public FormPReceiveImportBerger()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormPReceiveImportBerger form = new FormPReceiveImportBerger();
            form.preSelectTable = "Transfer";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                ////selectedType = cmbImportType.Text;
                var invoiceNo = txtId.Text.Trim();

                GetSearchData(invoiceNo);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            
            EntryTime = dptTime.Checked ? dptTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

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
                WithIsProcessed = withIsProcessed
            };

            progressBar1.Visible = true;

            bgwLoad.RunWorkerAsync(param);
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IntegrationParam param = (IntegrationParam)e.Argument;

                BergerIntegrationDAL IntegrationDal = new BergerIntegrationDAL();

                dtTableResult = IntegrationDal.GetPReceiveData(param, null, null, connVM);

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormPReceiveImportBerger", "bgwLoad_DoWork", exception.ToString());

                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void btnSaveData_Click(object sender, EventArgs e)
        {

            try
            {
                #region Progress bar

                this.progressBar1.Visible = true;

                #endregion

                NullCheck();
                

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

                ////FromDate = DateTime.Now.ToString("yyyy-MM-01");
                ////ToDate = (Convert.ToDateTime(FromDate).AddMonths(1)).AddDays(-1).ToString("yyyy-MM-dd");

                var invoiceNo = txtId.Text.Trim();

                var dal = new BranchProfileDAL();

                DataTable Data = new DataTable();

                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

                IntegrationParam param = new IntegrationParam
                {
                    TransactionType = transactionType,
                    RefNo = invoiceNo,
                    dtConnectionInfo = dt,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    WithIsProcessed = false
                };

                ////IntegrationParam param = (IntegrationParam)e.Argument;

                BergerIntegrationDAL IntegrationDal = new BergerIntegrationDAL();

                rVM = null;

                rVM = IntegrationDal.SavePReceive(param, connVM);

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

                if (rVM != null && rVM.Status.ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, rVM.Message);

                }

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

        private void FormPReceiveImportBerger_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ////bgwBigData.Dispose();
                ////if (bgwBigData.CancellationPending)
                ////{
                ////    e.Cancel = true;
                ////    return;
                ////}

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }





    }
}
