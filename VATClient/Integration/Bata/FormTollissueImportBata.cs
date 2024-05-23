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
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.Integration.Bata
{
    public partial class FormTollissueImportBata : Form
    {
        #region Variables

        CommonDAL _cDal = new CommonDAL();
        BataIntegrationDAL _IntegrationDAL = new BataIntegrationDAL();
        string[] sqlResults = new string[6];
        ResultVM rVM = new ResultVM();
        IntegrationParam paramVM = new IntegrationParam();

        public string transactionType;
        DataTable dtTableResult;

        private string PoDateFrom = "";
        private string PoDateTo = "";
        private string DateRange = "";
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string GpDateFrom = "";
        private string GpDateTo = "";
        private string _saleRow = "";
        DataTable dgvDt = new DataTable();
        DataTable dtGatePass = new DataTable();

        #endregion

        public FormTollissueImportBata()
        {
            InitializeComponent();
        }

        public static string SelectOne(string transactionType)
        {
            FormTollissueImportBata form = new FormTollissueImportBata();

            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {

                //if (string.IsNullOrWhiteSpace(txtGatePassNo.Text.Trim()))
                //{
                //    MessageBox.Show(this, "Please Enter Gate Pass No", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                NullCheck();

                btnSaveTemp.Enabled = false;
                btnLoad.Enabled = false;
                btnOk.Enabled = false;
                this.progressBar1.Visible = true;


                bgwLoad.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                FileLogger.Log("FormTollissueImportBata", "btnLoad_Click", ex.Message + "\n" + ex.StackTrace);

                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void NullCheck()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) && string.IsNullOrWhiteSpace(txtGatePassNo.Text) && dtpPoDateFrom.Checked == false)
            {
                //if (dtpPoDateFrom.Checked == false)
                //{
                //    dtpPoDateFrom.Checked = true;
                //    dtpPoDateFrom.Value = DateTime.Now;
                //}

                //if (dtpPoDateTo.Checked == false)
                //{
                //    dtpPoDateTo.Checked = true;
                //    dtpPoDateTo.Value = DateTime.Now;
                //}

                if (dtpGpDateFrom.Checked == false)
                {
                    dtpGpDateFrom.Checked = true;
                    dtpGpDateFrom.Value = DateTime.Now;
                }

                if (dtpGpDateTo.Checked == false)
                {
                    dtpGpDateTo.Checked = true;
                    dtpGpDateTo.Value = DateTime.Now;
                }

            }

            GpDateFrom = dtpGpDateFrom.Checked == false ? new DateTime(2010, 1, 1).ToString("yyyy-MMM-dd") : dtpGpDateFrom.Value.ToString("yyyy-MMM-dd");
            GpDateTo = dtpGpDateTo.Checked == false ? new DateTime(2030, 1, 1).ToString("yyyy-MMM-dd") : dtpGpDateTo.Value.ToString("yyyy-MMM-dd");

            PoDateFrom = dtpPoDateFrom.Checked == false ? new DateTime(2010, 1, 1).ToString("yyyy-MMM-dd") : dtpPoDateFrom.Value.ToString("yyyy-MMM-dd");
            PoDateTo = dtpPoDateTo.Checked == false ? new DateTime(2030, 1, 1).ToString("yyyy-MMM-dd") : dtpPoDateTo.Value.ToString("yyyy-MMM-dd");

            DateRange = PoDateFrom + " - " + PoDateTo;

        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                NullCheck();

                if (dgvLoadedTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvLoadedTable.RowCount; i++)
                    {

                        decimal RestQNTY = Convert.ToDecimal(dgvLoadedTable["RestQNTY", i].Value);
                        decimal TransactionQuantity = Convert.ToDecimal(dgvLoadedTable["TransactionQuantity", i].Value);
                        string ITEM_NAME = dgvLoadedTable["ITEM_NAME", i].Value.ToString();
                        string ITEM_CODE = dgvLoadedTable["ITEM_CODE", i].Value.ToString();
                        if (TransactionQuantity > RestQNTY)
                        {
                            MessageBox.Show(this, "Transaction Quantity cannot geter then Rest Quantity of Product" + ITEM_NAME + " (" + ITEM_CODE + ")", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    #region Get gridview Data

                    dgvDt = dgvLoadedTable.DataSource as DataTable;

                    #endregion

                    btnSaveTemp.Enabled = false;
                    btnLoad.Enabled = false;
                    this.progressBar1.Visible = true;

                    bgwBigData.RunWorkerAsync();
                }

                #region Data Load

                ////IntegrationParam paramVM = new IntegrationParam();

                ////paramVM.RefNo = txtId.Text.Trim();
                ////paramVM.FromDate = PoDateFrom;
                ////paramVM.ToDate = PoDateTo;

                ////paramVM.GatePassNo = txtGatePassNo.Text.Trim();
                ////paramVM.GpDateFrom = GpDateFrom;
                ////paramVM.GpDateTo = GpDateTo;

                ////ResultVM vm = _IntegrationDAL.TollIssueDataGet(paramVM);

                ////dtTableResult = _IntegrationDAL.SelectAllTempData(paramVM);

                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //////NullCheck();

                #region Data Load

                IntegrationParam paramVM = new IntegrationParam();

                //////paramVM.RefNo = txtId.Text.Trim();
                //////paramVM.FromDate = PoDateFrom;
                //////paramVM.ToDate = PoDateTo;

                //////paramVM.GatePassNo = txtGatePassNo.Text.Trim();
                //////paramVM.GpDateFrom = GpDateFrom;
                //////paramVM.GpDateTo = GpDateTo;

                paramVM.BranchCode = Program.BranchCode;
                paramVM.BranchId = Program.BranchId.ToString();
                paramVM.CurrentUser = Program.CurrentUserID;
                paramVM.CurrentUserName = Program.CurrentUser;
                paramVM.TransactionType = transactionType;


                ////rVM = _IntegrationDAL.SaveTollIssueData(paramVM, null, Program.GetAppVersion());

                rVM = _IntegrationDAL.SaveTollIssue(paramVM, dgvDt, null, Program.GetAppVersion());


                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (rVM.Status.ToLower() == "success")
                {
                    btnOk.Enabled = true;
                }
                else
                {
                    btnOk.Enabled = false;
                }
                MessageBox.Show(this, rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnSaveTemp.Enabled = true;
                btnLoad.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {
            ////_saleRow = dgvLoadedTable.CurrentRow.Cells["GATE_PASS_NO"].Value.ToString();
            ////this.Hide();
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region Data Load

                IntegrationParam paramVM = new IntegrationParam();

                paramVM.RefNo = txtId.Text.Trim();
                paramVM.FromDate = PoDateFrom;
                paramVM.ToDate = PoDateTo;

                paramVM.GatePassNo = txtGatePassNo.Text.Trim();
                paramVM.GpDateFrom = GpDateFrom;
                paramVM.GpDateTo = GpDateTo;

                rVM = _IntegrationDAL.TollIssueDataGet(paramVM);

                dtTableResult = _IntegrationDAL.SelectAllTempData(paramVM);

                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                dgvLoadedTable.DataSource = dtTableResult;

                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnSaveTemp.Enabled = true;
                btnLoad.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _saleRow = dgvLoadedTable.CurrentRow.Cells["GATE_PASS_NO"].Value.ToString();
            this.Hide();
        }

        private void FormTollissueImportBata_Load(object sender, EventArgs e)
        {
            try
            {

                btnOk.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvLoadedTable_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvLoadedTable_RowLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvLoadedTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvLoadedTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvLoadedTable.RowCount; i++)
                    {

                        decimal RestQNTY = Convert.ToDecimal(dgvLoadedTable["RestQNTY", i].Value);
                        decimal TransactionQuantity = Convert.ToDecimal(dgvLoadedTable["TransactionQuantity", i].Value);
                        string ITEM_NAME = dgvLoadedTable["ITEM_NAME", i].Value.ToString();
                        string ITEM_CODE = dgvLoadedTable["ITEM_CODE", i].Value.ToString();
                        if (TransactionQuantity > RestQNTY)
                        {
                            MessageBox.Show(this, "Transaction Quantity cannot geter then Rest Quantity of Product" + ITEM_NAME + " (" + ITEM_CODE + ")", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvLoadedTable["TransactionQuantity", i].Value = "0";

                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            #region try

            try
            {

                dgvDt = dgvLoadedTable.DataSource as DataTable;

                OrdinaryVATDesktop.SaveExcel(dgvDt, "Tollissue6.4Bata", "Tollissue");
                MessageBox.Show("Successfully Exported data in Excel files of root directory");
                
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            #endregion

        }

        private void txtGatePassNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                LoadGatePassNo();
            }
        }

        private void LoadGatePassNo()
        {
            #region try

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormTollissueGatePassNoSearch.SelectOne();

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtGatePassNo.Text = selectedRow.Cells["GATE_PASS_NO"].Value.ToString();
                    
                }
                
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            #endregion

        }

        private void txtGatePassNo_DoubleClick(object sender, EventArgs e)
        {
            LoadGatePassNo();
        }


    }
}
