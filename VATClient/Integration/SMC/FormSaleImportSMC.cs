using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Library.Integration;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormSaleImportSMC : Form
    {
        #region Declarations

        CommonDAL _cDal = new CommonDAL();
        SMCIntegrationDAL _IntegrationDAL = new SMCIntegrationDAL();
        string[] sqlResults = new string[6];
        ResultVM rVM = new ResultVM();
        IntegrationParam paramVM = new IntegrationParam();

        DataTable dtTableResult;
        public string transactionType;
        private string SalesFromDate = "";
        private string SalesToDate = "";
        private string DateRange = "";

        private string _saleRow = "";
        public bool IsCDN = false;
        private string preSelectTable;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormSaleImportSMC()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        #region Form Load

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            dgvLoadedTable.ReadOnly = true;
            cmbDBList.SelectedIndex = 0;
        }


        #endregion

        #region Data Load

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

                dtTableResult = _IntegrationDAL.GetSource_SaleData(paramVM,connVM);

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

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportSMC form = new FormSaleImportSMC();

            form.IsCDN = cdn;

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }


        #endregion

        #region Data Save

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

                #region Param Set
                NullCheck();

                paramVM = new IntegrationParam();

                paramVM.RefNo = txtId.Text.Trim();
                paramVM.FromDate = SalesFromDate;
                paramVM.ToDate = SalesToDate;
                paramVM.DataSourceType = cmbDBList.Text; ;

                #endregion

                bgwBigData.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

                btnSaveTemp.Enabled = true;
                this.progressBar1.Visible = false;
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

                rVM = _IntegrationDAL.SaveSale_Pre(paramVM, connVM);



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            finally { }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                lblRecord.Text = "Record Count: " + rVM.RowCount;

                MessageBox.Show(rVM.Message, "Sales", MessageBoxButtons.OK, MessageBoxIcon.Information);


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
                _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();

                this.Hide();

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        #endregion

    }
}
