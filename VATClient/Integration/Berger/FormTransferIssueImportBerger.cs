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
    public partial class FormTransferIssueImportBerger : Form
    {

        #region Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string _saleRow = "";
        public string preSelectTable;
        public string transactionType;
        private string EntryTime = "";
        private IntegrationParam param = null;
        private string ToDate;
        private string FromDate;
        DataTable dtTableResult = new DataTable();
        ResultVM rVM = null;

        ////string[] sqlResults = new string[6];
        ////private int _saleRowCount = 0;
        ////DataSet ds;

        #endregion

        public FormTransferIssueImportBerger()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormTransferIssueImportBerger form = new FormTransferIssueImportBerger();
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

        }

        private void GetSearchData(string invoiceNo = "", bool withIsProcessed = false)
        {
            NullCheck();

            BranchProfileDAL dal = new BranchProfileDAL();

            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

            param = new IntegrationParam
            {
                TransactionType = transactionType,
                RefNo = invoiceNo,
                dtConnectionInfo = dt,
                FromDate = FromDate,
                ToDate = ToDate,
                WithIsProcessed = withIsProcessed,
                BranchCode = Program.BranchCode
            };

            progressBar1.Visible = true;

            bgwDataLoad.RunWorkerAsync(param);
        }

        private void bgwDataLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IntegrationParam param = (IntegrationParam)e.Argument;

                BergerIntegrationDAL IntegrationDal = new BergerIntegrationDAL();

                dtTableResult = IntegrationDal.GetTransferData(param, connVM);

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormTransferIssueImportBerger", "bgwDataLoad_DoWork", exception.ToString());

                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void bgwDataLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
           
            NullCheck();
            this.progressBar1.Visible = true;
            bgwBigData.RunWorkerAsync();
        }

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
           
            try
            {
                BergerIntegrationDAL transferIssueDAL = new BergerIntegrationDAL();


                var transferData = dtTableResult.Copy();

                rVM = transferIssueDAL.SaveTransferData(transferData, Program.BranchCode, transactionType,
                    Program.CurrentUser, Program.BranchId, () => { }, null, null, false, connVM, EntryTime, Program.CurrentUserID);

            }
            catch (Exception ex)
            {
                rVM.Status = "fail";
                rVM.Message = ex.Message;
            }

        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (rVM.Status != null && rVM.Status.ToLower() == "success")
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




    }
}
