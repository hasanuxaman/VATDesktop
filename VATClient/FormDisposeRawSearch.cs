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
    public partial class FormDisposeRawSearch : Form
    {
        #region Constructors

        public FormDisposeRawSearch()
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

        private DataTable ReceiveResult;
        private static string transactionType;
        private string post = string.Empty;
        string[] IdArray;
        private string SearchBranchId = "0";
        private string RecordCount = "0";

        #endregion

        public static DataGridViewRow SelectOne(string TransType)
        {
            DataGridViewRow selectedRowTemp = null;

            try
            {
                FormDisposeRawSearch FrmDisposeRawsSearch = new FormDisposeRawSearch();
                transactionType = TransType;
                if (TransType == "Other")
                    FrmDisposeRawsSearch.Text = "Dispose Raw Search";


                FrmDisposeRawsSearch.ShowDialog();
                selectedRowTemp = FrmDisposeRawsSearch.selectedRow;
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
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

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
                dgvDisposeRawsSearch.Rows.Clear();
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
                if (dgvDisposeRawsSearch.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvDisposeRawsSearch.SelectedRows;
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

                //BugsBD
                string DisposeNo = OrdinaryVATDesktop.SanitizeInput(txtDisposeNo.Text.Trim());
                

                DisposeRawDAL DisposeDAL = new DisposeRawDAL();

                //string[] cValues = { txtDisposeNo.Text.Trim(), ReceiveFromDate, ReceiveToDate, transactionType, post, SearchBranchId, RecordCount };
                string[] cValues = { DisposeNo, ReceiveFromDate, ReceiveToDate, transactionType, post, SearchBranchId, RecordCount };
                string[] cFields = { "DisposeNo like", "TransactionDateTime>", "TransactionDateTime<", "TransactionType like", "Post like", "BranchId", "SelectTop" };

                ReceiveResult = DisposeDAL.SelectAll(cFields, cValues,null,null,connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                ReceiveResult = OrdinaryVATDesktop.DtDeleteColumns(ReceiveResult, columnNames);

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
                // Start Complete
                dgvDisposeRawsSearch.DataSource = null;
                if (ReceiveResult != null && ReceiveResult.Rows.Count > 0)
                {


                    TotalTecordCount = ReceiveResult.Rows[ReceiveResult.Rows.Count - 1][0].ToString();

                    ReceiveResult.Rows.RemoveAt(ReceiveResult.Rows.Count - 1);

                    dgvDisposeRawsSearch.DataSource = ReceiveResult;
                }

                //dgvReceiveHistory.Rows.Clear();
                //int j = 0;
                //if (transactionType == "All")
                //{
                //    DataRow[] receiveRows = ReceiveResult.Select("transactionType<>'InternalIssue'");

                //    ReceiveResult = receiveRows.CopyToDataTable();
                //}
                //if (ReceiveResult != null)
                //    foreach (DataRow item in ReceiveResult.Rows)
                //    {
                //        DataGridViewRow NewRow = new DataGridViewRow();
                //        dgvReceiveHistory.Rows.Add(NewRow);
                //        dgvReceiveHistory.Rows[j].Cells["Id"].Value = item["Id"].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["ReceiveNo"].Value = item["ReceiveNo"].ToString();// ReceiveFields[0].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["ReceiveDate"].Value = item["ReceiveDateTime"].ToString();// ReceiveFields[1].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["TotalAmount"].Value = item["TotalAmount"].ToString();// Convert.ToDecimal(ReceiveFields[2].ToString()).ToString("0.00");
                //        dgvReceiveHistory.Rows[j].Cells["TotalVATAmount"].Value = item["TotalVATAmount"].ToString();// Convert.ToDecimal(ReceiveFields[3].ToString()).ToString("0.00");
                //        dgvReceiveHistory.Rows[j].Cells["SerialNo"].Value = item["SerialNo"].ToString();// ReceiveFields[4].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["ReferenceNo"].Value = item["ReferenceNo"].ToString();// ReceiveFields[5].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// ReceiveFields[6].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["PostR"].Value = item["Post"].ToString();// ReceiveFields[7].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["ReturnId"].Value = item["ReturnId"].ToString();// ReceiveFields[8].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["ImportId"].Value = item["ImportIDExcel"].ToString();// ReceiveFields[8].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["CustomerID"].Value = item["CustomerID"].ToString();// ReceiveFields[8].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["CustomerName"].Value = item["CustomerName"].ToString();// ReceiveFields[8].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["WithToll"].Value = item["WithToll"].ToString();// ReceiveFields[8].ToString();
                //        dgvReceiveHistory.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();// ReceiveFields[8].ToString();

                //        j = j + 1;
                //    }
                // End Complete

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
                LRecordCount.Text = "Total Record Count " + (ReceiveResult.Rows.Count) + " of " + TotalTecordCount.ToString();

            }
        }

        #endregion

        private void btnPost_Click(object sender, EventArgs e)
        {
            int j = 0;
            string ids = "";

            List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
            dgvr = dgvDisposeRawsSearch.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Select"].Value) == true && Convert.ToString(r.Cells["Post"].Value) == "N").ToList();

            foreach (DataGridViewRow dr in dgvr)
            {

                string Id = dr.Cells["BOMId"].Value.ToString();

                ids += Id + "~";
            }

            #region Comments - Dec-01-2020

            ////for (int i = 0; i < dgvDisposeRawsSearch.Rows.Count; i++)
            ////{
            ////    if (Convert.ToBoolean(dgvDisposeRawsSearch["Select", i].Value) == true)
            ////    {
            ////        if (dgvDisposeRawsSearch["Post", i].Value.ToString() == "N")
            ////        {
            ////            string Id = dgvDisposeRawsSearch["ID", i].Value.ToString();
            ////            ids += Id + "~";
            ////        }
            ////    }
            ////}

            #endregion

            #region Check Point

            IdArray = ids.Split('~');
            if (IdArray.Length <= 1)
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
                if (IdArray == null || IdArray.Length <= 0)
                {
                    return;
                }

                string Message = MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1;

                DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgRes != DialogResult.Yes)
                {
                    return;
                }
                DisposeRawDAL _DisposeRowsDal = new DisposeRawDAL();

                string[] results = _DisposeRowsDal.MultiplePost(IdArray, connVM);

                if (results[0] == "Success")
                {
                    MessageBox.Show("All Dispose Raw posted successfully");
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
            for (int i = 0; i < dgvDisposeRawsSearch.RowCount; i++)
            {
                dgvDisposeRawsSearch["Select", i].Value = chkSelectAll.Checked;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvDisposeRawsSearch.Rows.Count;


                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvDisposeRawsSearch["Select", i].Value))
                    {
                        invoiceList.Add(dgvDisposeRawsSearch["ReceiveNo", i].Value.ToString());
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

        private void dgvDisposeRawsSearch_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }
    }
}
