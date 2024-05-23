using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using VATServer.Library.Integration;
using VATClient.Integration.Bata;

namespace VATClient
{
    public partial class FormTransferImportBata : Form
    {
        CommonDAL _cDal = new CommonDAL();
        BataIntegrationDAL _IntegrationDAL = new BataIntegrationDAL();
        string[] sqlResults = new string[6];
        ResultVM rVM = new ResultVM();
        IntegrationParam paramVM = new IntegrationParam();

        #region Variables
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult;
        DataSet ds;
        public bool isFileSelected = false;
        private bool _isDeleteTemp = true;
        public string preSelectTable;
        public string transactionType;
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_ProcessOnly = "ProcessOnly";
        private const string CONST_DBNAME = "VATImport_DB";
        private const string CONST_SINGLEIMPORT = "SingleFileImport";
        private const string CONST_SALETYPE = "Sales";
        private const string CONST_PURCHASETYPE = "Purchases";
        private const string CONST_ISSUETYPE = "Issues";
        private const string CONST_RECEIVETYPE = "Receives";
        private const string CONST_VDSTYPE = "VDS";//
        private const string CONST_TRANSFERTYPE = "TransferIssues";
        private const string CONST_BOMTYPE = "BOM";
        private const string CONST_SALES_BIGDATA = "Sales_Big_Data";
        private string SalesFromDate = "";
        private string SalesToDate = "";
        private string DateRange = "";
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();






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

            SalesFromDate = dtpSaleFromDate.Checked == false ? new DateTime(2010, 1, 1).ToString("yyyy-MMM-dd") : dtpSaleFromDate.Value.ToString("yyyy-MMM-dd");
            SalesToDate = dtpSaleToDate.Checked == false ? new DateTime(2030, 1, 1).ToString("yyyy-MMM-dd") : dtpSaleToDate.Value.ToString("yyyy-MMM-dd");
            DateRange = SalesFromDate + " - " + SalesToDate;

        }

        private string _saleRow = "";

        private string _selectedDb = "Link3_Demo_DB";

        public bool IsCDN = false;

        #endregion

        public FormTransferImportBata()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormTransferImportBata form = new FormTransferImportBata();

            //form.IsCDN = cdn;

            form.preSelectTable = "TransferIssue";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            dgvLoadedTable.ReadOnly = true;
            typeLoad();
            cmbDBList.SelectedIndex = 0;
        }

        private void typeLoad()
        {
            //cmbImportType.Items.Add(CONST_EXCEL);
            //cmbImportType.Items.Add(CONST_TEXT);
            cmbImportType.Items.Add(CONST_DATABASE);
            // cmbImportType.Items.Add(CONST_ProcessOnly);

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection",null,connVM);

            cmbImportType.SelectedItem = CONST_EXCEL;

            cmbImportType.Text = value;

            #region IsIntegrationExcel & Other Lisence Check
            if (Program.IsIntegrationExcel == false)
            {
                cmbImportType.Items.Remove(CONST_EXCEL);
                cmbImportType.Items.Remove(CONST_TEXT);
                cmbImportType.SelectedItem = CONST_DATABASE;

            }
            if (Program.IsIntegrationOthers == false)
            {
                cmbImportType.Items.Remove(CONST_DATABASE);
                cmbImportType.SelectedItem = CONST_EXCEL;

            }
            #endregion


        }


        private void btnLoad_Click(object sender, EventArgs e)
        {

            try
            {

                NullCheck();

                #region Load Start
                string LoadStartTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Load Start Time: " + LoadStartTime);

                #endregion

                #region Data Load

                IntegrationParam paramVM = new IntegrationParam();

                paramVM.RefNo = txtId.Text.Trim();
                paramVM.FromDate = SalesFromDate;
                paramVM.ToDate = SalesToDate;
                paramVM.DataSourceType = cmbDBList.Text; ;

                //dtTableResult = _IntegrationDAL.GetSource_SaleData(paramVM);

                dgvLoadedTable.DataSource = dtTableResult;

                #endregion


                ////if (dtTableResult != null && dtTableResult.Rows.Count > 0)
                ////{

                ////}
                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;


                #region Load End

                string LoadEndTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Load End Time: " + LoadEndTime);

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void SetSteps(int steps = 4)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(steps); });
        }


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

                #region Select

                if (loadedTable == "")
                {
                    return;
                }

                if (!_isDeleteTemp)
                {
                    MessageBox.Show(this, "Please select new excel file");
                    return;
                }

                _selectedDb = cmbDBList.Text;
                selectedType = cmbImportType.Text;

                #endregion

                #region Param Set
                NullCheck();

                paramVM = new IntegrationParam();

                paramVM.RefNo = txtId.Text.Trim();
                paramVM.FromDate = SalesFromDate;
                paramVM.ToDate = SalesToDate;
                paramVM.CurrentUser = Program.CurrentUser;
                //paramVM.DataSourceType = cmbDBList.Text; ;

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
                var salesDal = new SaleDAL();

                if (selectedType == CONST_ProcessOnly)
                {
                    sqlResults = salesDal.SaveAndProcessTempData(new DataTable(), BulkCallBack, Program.BranchId,connVM);
                }
                else
                {

                    //if (InvokeRequired)
                    //    Invoke((MethodInvoker)delegate { PercentBar(5); });

                    //BulkCallBack();

                    paramVM.callBack = () =>{};
                    paramVM.SetSteps = (step) =>{};

                    rVM = _IntegrationDAL.SaveTransferIssue(paramVM, Program.CurrentUserID,connVM);
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
            finally { }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Save End

                string SaveEndTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                btnSaveTemp.Enabled = true;

                FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Save End Time: " + SaveEndTime);

                #endregion

                if (rVM.Status == "Fail")
                {
                    throw new ArgumentException(rVM.Message);
                }

                MessageBox.Show(rVM.Message, "Sales", MessageBoxButtons.OK, MessageBoxIcon.Information);

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



        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (cmbImportType.Text == CONST_DATABASE)
            {
                _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();


                this.Hide();
            }

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDataProcess_Click(object sender, EventArgs e)
        {
            string ImportId = FormTransferIssueDataTransfer.SelectOne(transactionType);
        }


    }
}
