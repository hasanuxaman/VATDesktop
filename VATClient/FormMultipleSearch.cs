using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormMultipleSearch : Form
    {
        public FormMultipleSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string SelectedValue = string.Empty;
        private static string varSQLText = string.Empty;
        private static string varSQLTextRecordCount = string.Empty;
        private static string varTableName = string.Empty;
        private static string varSearchValue = string.Empty;
        private static string varFixedConditions = string.Empty;

        private string RecordCount = "0";
        private static string[] varConditionFields = new string[] { "one" };

        private string uom = string.Empty;
        private string type = string.Empty;
        private string activeStatus = string.Empty;
        DataGridViewRow selectedRow = new DataGridViewRow();
        public static string ProductCategoryName = string.Empty;

        #region Global Variables For BackGroundWorker

        private string ProductData = string.Empty;

        #endregion

        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            try
            {

                DataTable dt = new DataTable();
                dgvMultipleSearch.DataSource = dt;
                CommonDAL dal = new CommonDAL();
                //ICommon dal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                //string[] shortColumnName = { "ItemNo", "ProductCode", "ProductName", "ProductDescription", "UOM", "OpeningBalance", "HSCodeNo", "VATRate", "SD", "Trading", "NonStock", "QuantityInHand", "OpeningDate", "RebatePercent", "ActiveStatus" };
                if (varTableName == "")
                {
                    dt = dal.MultipleSearchSQL(varSQLText, txtSearch.Text.Trim(), varConditionFields, RecordCount, varSQLTextRecordCount, connVM);
                }
                else
                {
                    dt = dal.MultipleSearch(varTableName, txtSearch.Text.Trim(), varConditionFields, "", connVM);
                }
                int TotalTecordCount = 0;
                dgvMultipleSearch.DataSource = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    //TotalTecordCount = dt.Rows.Count;
                    TotalTecordCount =Convert.ToInt32(dt.Rows[dt.Rows.Count - 1][0].ToString());

                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dgvMultipleSearch.DataSource = dt;
                }

                dgvMultipleSearch.DataSource = dt;
                if (dgvMultipleSearch.Rows.Count>1)
                {
                    dgvMultipleSearch.Columns[0].Visible = false;
                }

                LRecordCount.Text = "Record Count: " + dt.Rows.Count.ToString() + " of " + TotalTecordCount.ToString();

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", exMessage);
            }

            #endregion


        }

        private void FormMultipleSearch_Load(object sender, EventArgs e)
        {
            cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;
            txtSearch.Text = varSearchValue;
            RecordCount = cmbRecordCount.Text.Trim();
            Search();
        }

        public static DataGridViewRow SelectOne(string SQLText = "", string tableName = "", string SearchValue = "", string[] ConditionFields = null
            , string FixedConditions = "", string SQLTextRecordCount = "")
        {
            DataGridViewRow selectedRowTemp = null;

            #region try

            try
            {
                varSQLText = SQLText;
                varTableName = tableName;
                varSearchValue = SearchValue;
                varConditionFields = ConditionFields;
                varFixedConditions = FixedConditions;
                varSQLTextRecordCount = SQLTextRecordCount;
                FormMultipleSearch frmSearch = new FormMultipleSearch();
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
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductSearch", "SelectOne", exMessage);
            }

            #endregion



            return selectedRowTemp;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void dgvMultipleSearch_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RecordCount = cmbRecordCount.Text.Trim();
            Search();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Down))
            {
                dgvMultipleSearch.Focus();
            }
        }

        private void dgvMultipleSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvMultipleSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter) || e.KeyCode.Equals(Keys.F9))
            {
                GridSeleted();
            }

        }

    }
}
