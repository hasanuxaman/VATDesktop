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
using VATServer.Library.Integration;
using VATViewModel.DTOs;

namespace VATClient.Integration.ShumiHotCake
{
    public partial class FormSaleImportShumiHotCake : Form
    {
        #region Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public string preSelectTable;
        public string transactionType;
        private string _saleRow = "";
        private string EntryTime = "";
        private string ToDate;
        private string FromDate;
        static string selectedType;
        string IsProcess;
        private IntegrationParam param = null;
        DataTable dtTableResult = new DataTable();
        string[] sqlResults = new string[6];


        ////string loadedTable;
        ////private int _saleRowCount = 0;
        ////DataSet ds;
        ////private long _timePassedInMs;
        ////private const string CONST_DATABASE = "Database";
        ////private const string CONST_TEXT = "Text";
        ////private const string CONST_EXCEL = "Excel";
        ////private const string CONST_ProcessOnly = "ProcessOnly";
        ////private const string CONST_SINGLEIMPORT = "SingleFileImport";
        ////private const string CONST_SALETYPE = "Sales";

        ////public bool IsCDN = false;
        
        #endregion

        public FormSaleImportShumiHotCake()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormSaleImportShumiHotCake form = new FormSaleImportShumiHotCake();
            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
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


        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceNo = txtId.Text.Trim();
                IsProcess = cmbIsProcessed.Text.Trim();

                GetSearchData(invoiceNo);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

                ShumiHotCakeIntegrationDAL IntegrationDal = new ShumiHotCakeIntegrationDAL();

                dtTableResult = IntegrationDal.GetSaleData(param, null, null, connVM, IsProcess);

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormSaleImportShumiHotCake", "bgwSaleLoad_DoWork", exception.ToString());

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

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {

                this.progressBar1.Visible = true;

                NullCheck();
                SaveAndProcessData();

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
                #region Process And Save Data

                IntegrationParam param = (IntegrationParam)e.Argument;

                ShumiHotCakeIntegrationDAL _IntegrationDAL = new ShumiHotCakeIntegrationDAL();

                sqlResults = _IntegrationDAL.SaveSaleData(param, connVM);

                #endregion
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
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, sqlResults[1]);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void SaveAndProcessData(string invoiceNo = "", bool withIsProcessed = false)
        {
            NullCheck();
            var dal = new BranchProfileDAL();

            param = new IntegrationParam
            {
                TransactionType = transactionType,
                RefNo = invoiceNo,
                ////dtConnectionInfo = dt,
                FromDate = FromDate,
                ToDate = ToDate,
                WithIsProcessed = withIsProcessed,
                BranchCode = Program.BranchCode,
                CurrentUserId = Program.CurrentUserID

            };

            progressBar1.Visible = true;

            bgwBigData.RunWorkerAsync(param);

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


    }
}
