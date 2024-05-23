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

namespace VATClient.Integration.Decathlon
{
    public partial class FormPurchaseImportDecathlon : Form
    {
        #region Variables
        string IsProcess;
        string[] sqlResults = new string[6];
        string selectedTable;
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult = new DataTable();
        DataSet ds;
        private bool _isDeleteTemp = true;
        public string preSelectTable;
        public string transactionType;
        private DataTable _noBranch;
        private long _timePassedInMs;
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_ProcessOnly = "ProcessOnly";
        private const string CONST_SINGLEIMPORT = "SingleFileImport";
        private const string CONST_SALETYPE = "Sales";
        private const string CONST_PURCHASETYPE = "Purchases";
        private const string CONST_ISSUETYPE = "Issues";
        private const string CONST_RECEIVETYPE = "Receives";
        private const string CONST_VDSTYPE = "VDS";//
        private const string CONST_BOMTYPE = "BOM";
        private string EntryTime = "";
        private string _saleRow = "";
        private IntegrationParam param = null;
        private string _selectedDb = "Link3_Demo_DB";

        public bool IsCDN = false;
        private string ToDate;
        private string FromDate;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormPurchaseImportDecathlon()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static string SelectOne(string transactionType)
        {
            FormPurchaseImportDecathlon form = new FormPurchaseImportDecathlon();
            form.preSelectTable = "Purchase";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
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

            #region Old Code

            ////////if (cmbImportType.Text != CONST_DATABASE)
            ////////{
            ////////    _isDeleteTemp = true;
            ////////    LoadDataGrid();
            ////////}
            ////////else
            ////////{
            ////////    try
            ////////    {
            ////////        selectedType = cmbImportType.Text;
            ////////        var invoiceNo = txtId.Text.Trim();
            ////////        IsProcess = cmbIsProcessed.Text.Trim();
            ////////        GetSearchData(invoiceNo);

            ////////    }
            ////////    catch (Exception ex)
            ////////    {
            ////////        MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////////    }
            ////////}

            #endregion
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

                //dtTableResult = decathlonIntegrationDal.GetPurchaseData(param, connVM, null, null, IsProcess);
                dtTableResult = decathlonIntegrationDal.GetPurchseDataAPI(param, connVM);

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormPurchaseImportEON", "bgwSaleLoad_DoWork", exception.ToString());

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

        private void FormPurchaseImportDecathlon_Load(object sender, EventArgs e)
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

                this.progressBar1.Visible = true;

                #endregion


                dtTableResult = dtTableResult.Copy();
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

                PurchaseDAL purchaseDal = new PurchaseDAL();

                DataTable purchaseData = dtTableResult.Copy();

                if (purchaseData.Columns.Contains("VATRate"))
                {
                    purchaseData.Columns.Remove("VATRate");
                }

                sqlResults = purchaseDal.SaveTempPurchase(purchaseData, Program.BranchCode, transactionType, Program.CurrentUser,
                    Program.BranchId, () => { }, null, null, connVM);

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
    }
}
