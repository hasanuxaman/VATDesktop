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

namespace VATClient.Integration.Bata
{
    public partial class FormTollissueGatePassNoSearch : Form
    {

        #region Variables

        CommonDAL _cDal = new CommonDAL();
        BataIntegrationDAL _IntegrationDAL = new BataIntegrationDAL();
        DataTable dtTableResult;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string _saleRow = "";
        DataTable dgvDt = new DataTable();
        DataTable dtGatePass = new DataTable();
        DataGridViewRow selectedRow = new DataGridViewRow();

        #endregion

        public FormTollissueGatePassNoSearch()
        {
            InitializeComponent();
        }

        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;

            #region try

            try
            {
                FormTollissueGatePassNoSearch frmSearch = new FormTollissueGatePassNoSearch();
                frmSearch.ShowDialog();
                selectedRowTemp = frmSearch.selectedRow;
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Gate Pass No Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTollissueGatePassNoSearch", "SelectOne", exMessage);
            }

            #endregion

            return selectedRowTemp;

        }

        private void FormTollissueGatePassNoSearch_Load(object sender, EventArgs e)
        {
            try
            {

                dtGatePass = _IntegrationDAL.SelectAllGatePassNo_Oracle();

                SelectGatePassNo();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SelectGatePassNo();
        }

        private void SelectGatePassNo()
        {
            #region try

            try
            {
                string gatePass = txtSearch.Text.Trim();

                if (!string.IsNullOrWhiteSpace(gatePass))
                {
                    DataRow[] rows = dtGatePass.Select("GATE_PASS_NO  LIKE '%" + gatePass + "%'");//LIKE '%" + textBox1.Text + "%'"
                    DataTable dtGatePassData = new DataTable();
                    dtGatePassData = rows.CopyToDataTable();

                    dgvMultipleSearch.DataSource = dtGatePassData;

                    LRecordCount.Text = "Record Count: " + dtGatePassData.Rows.Count.ToString();

                }
                else
                {
                    dgvMultipleSearch.DataSource = dtGatePass;

                    LRecordCount.Text = "Record Count: " + dtGatePass.Rows.Count.ToString();

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SelectGatePassNo();
        }


        private void GridSeleted()
        {

            #region try

            try
            {
                if (dgvMultipleSearch.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvMultipleSearch.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }


                this.Close();
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }

            #endregion

        }

        private void dgvMultipleSearch_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

    }
}
