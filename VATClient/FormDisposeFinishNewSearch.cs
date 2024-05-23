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
using System.Data.SqlClient;
using System.Configuration;
////
//
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo;
using VATServer.Interface;
namespace VATClient
{
    public partial class FormDisposeFinishNewSearch : Form
    {
        #region Constructors

        public FormDisposeFinishNewSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #endregion

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        DataGridViewRow selectedRow = new DataGridViewRow();

        private DataTable dtMaster;
        private static string transactionType;
        private string post = string.Empty;
        private string SearchBranchId = "0";
        private string RecordCount = "0";

        ParameterVM paramVM = new ParameterVM();
        ResultVM rVM = new ResultVM();

        #endregion

        public static DataGridViewRow SelectOne(string TransType)
        {
            DataGridViewRow selectedRowTemp = null;

            try
            {
                FormDisposeFinishNewSearch FrmDisposeFinishNewSearch = new FormDisposeFinishNewSearch();
                transactionType = TransType;
                if (TransType == "Other")
                    FrmDisposeFinishNewSearch.Text = "Dispose Finish Search";


                FrmDisposeFinishNewSearch.ShowDialog();
                selectedRowTemp = FrmDisposeFinishNewSearch.selectedRow;
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
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
            }
            #endregion Catch

            return selectedRowTemp;
        }

        string ReceiveFromDate, ReceiveToDate;

        private void NullCheck()
        {
            try
            {


                if (dtpFromDate.Checked == false)
                {
                    ReceiveFromDate = "";
                }
                else
                {
                    ReceiveFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpToDate.Checked == false)
                {
                    ReceiveToDate = "";
                }
                else
                {
                    ReceiveToDate = dtpToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }


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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            #endregion Catch

        }

        private void FormReceiveSearch_Load(object sender, EventArgs e)
        {
            try
            {
                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true);

                cmbBranch.SelectedValue = Program.BranchId;
                cmbExport.SelectedIndex = 0;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

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
                FileLogger.Log(this.Name, "FormReceiveSearch_Load", exMessage);
            }
            #endregion Catch
        }

        public void ClearAllFields()
        {
            try
            {
                txtDisposeNo.Text = "";
                txtRefNo.Text = "";
                dtpFromDate.Text = "";
                dtpToDate.Text = "";
                dgvDisposeFinishNewSearch.Rows.Clear();
                cmbPost.SelectedIndex = -1;
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion Catch
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        #region TextBox KeyDown Event

        private void txtReceiveNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpReceiveFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpReceiveToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVatAmountFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVatAmountTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtGrandTotalFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtGrandTotalTo_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.Focus();
        }

        #endregion

        private void GridSeleted()
        {
            try
            {
                if (dgvDisposeFinishNewSearch.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvDisposeFinishNewSearch.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                this.Close();
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }
            #endregion Catch
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void dgvReceiveHistory_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

        #region // == Search == //

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                RecordCount = cmbRecordCount.Text.Trim();

                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                Search();

                #endregion
            }
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion
        }

        private void Search()
        {
            try
            {
                NullCheck();
                SearchBranchId = cmbBranch.SelectedValue.ToString();
                post = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : string.Empty;
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                bgwSearch.RunWorkerAsync();
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
            #endregion Catch
        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                DisposeFinishDAL DisposeDAL = new DisposeFinishDAL();

                string[] cValues = { txtDisposeNo.Text.Trim(), ReceiveFromDate, ReceiveToDate, transactionType, post, SearchBranchId };
                string[] cFields = { "df.DisposeNo like", "df.TransactionDateTime>", "df.TransactionDateTime<", "df.TransactionType like", "df.Post like", "df.BranchId" };

                dtMaster = DisposeDAL.Select(cFields, cValues,null,null,connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                dtMaster = OrdinaryVATDesktop.DtDeleteColumns(dtMaster, columnNames);

                #endregion
            }
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
                FileLogger.Log(this.Name, "bgwSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string TotalTecordCount = "0";

            try
            {
                #region Statement
                dgvDisposeFinishNewSearch.DataSource = null;

                if (dtMaster != null && dtMaster.Rows.Count > 0)
                {


                    TotalTecordCount = dtMaster.Rows[dtMaster.Rows.Count - 1][0].ToString();

                    ////dtMaster.Rows.RemoveAt(dtMaster.Rows.Count - 1);

                    dgvDisposeFinishNewSearch.DataSource = dtMaster;
                }

                #endregion
            }
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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {

                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (dtMaster.Rows.Count) + " of " + TotalTecordCount.ToString();

            }
        }

        #endregion

        private void btnPost_Click(object sender, EventArgs e)
        {

            List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
            dgvr = dgvDisposeFinishNewSearch.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Select"].Value) == true && Convert.ToString(r.Cells["Post"].Value) == "N").ToList();

            paramVM = new ParameterVM();
            paramVM.IDs = new List<string>();

            string DisposeNo = "";

            foreach (DataGridViewRow dr in dgvr)
            {

                DisposeNo = dr.Cells["DisposeNo"].Value.ToString();

                paramVM.IDs.Add(DisposeNo);

            }

            #region Check Point

            if (paramVM.IDs.Count <= 1)
            {
                MessageBox.Show(this, "All data already posted !");
                return;
            }

            #endregion

            #region Background Worker - Post

            bgwMultiplePost.RunWorkerAsync();

            #endregion
        }

        private void bgwMultiplePost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (paramVM.IDs.Count <= 0)
                {
                    return;
                }

                string Message = MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1;

                DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgRes != DialogResult.Yes)
                {
                    return;
                }
                DisposeFinishDAL _DisposeFinishDal = new DisposeFinishDAL();

                rVM = _DisposeFinishDal.Post(paramVM,null,null,connVM);

                if (rVM.Status == "Success")
                {
                    MessageBox.Show("All Dispose Finish posted successfully");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwMultiplePost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Search();
        }

        private void chkSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvDisposeFinishNewSearch.RowCount; i++)
            {
                dgvDisposeFinishNewSearch["Select", i].Value = chkSelectAll.Checked;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvDisposeFinishNewSearch.Rows.Count;


                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvDisposeFinishNewSearch["Select", i].Value))
                    {
                        invoiceList.Add(dgvDisposeFinishNewSearch["ReceiveNo", i].Value.ToString());
                    }
                }

                IReceive dal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                var data = dal.GetExcelData(invoiceList, null, null, connVM);


                if (cmbExport.Text == "EXCEL")
                {
                    if (data.Rows.Count == 0)
                    {
                        data.Rows.Add(data.NewRow());
                    }
                    OrdinaryVATDesktop.SaveExcel(data, "Receive", "ReceiveM");
                    MessageBox.Show("Successfully Exported data in Excel files of root directory");
                }
                else if (cmbExport.Text == "TEXT")
                {
                    OrdinaryVATDesktop.WriteDataToFile(data, "Receive");
                    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }

        }

        private void dgvDisposeFinishNewSearch_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }
    }
}
